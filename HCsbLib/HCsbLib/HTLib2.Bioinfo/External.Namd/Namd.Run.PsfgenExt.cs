using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Namd
    {
        public class CPsfgenExt
        {
            public List<string> pdblines;
            public List<string> psflines;
        }
        static Pdb PsfgenExt_AlignTo(Pdb pdb, Pdb alignto)
        {
            var dictpdb = pdb    .atoms.SelectByName("CA").GroupChainIDResSeq();
            var dictaln = alignto.atoms.SelectByName("CA").GroupChainIDResSeq();
            List<Vector> coordpdb = new List<Vector>();
            List<Vector> coordaln  = new List<Vector>();
            foreach(char ch in dictpdb.Keys)
            {
                if(dictaln.ContainsKey(ch) == false)
                    continue;
                foreach(var resi in dictpdb[ch].Keys)
                {
                    var atompdb = dictpdb[ch][resi];
                    if(atompdb.Count != 1) throw new HException(string.Format(">1 number of CA in chain {0} resi {1} in atompdb", ch, resi));
                    if(dictaln[ch].ContainsKey(resi) == false)
                        continue;
                    var atomaln = dictaln[ch][resi];
                    if(atomaln.Count != 1) throw new HException(string.Format(">1 number of CA in chain {0} resi {1} in atomaln", ch, resi));
                    HDebug.Assert(coordpdb.Count == coordaln.Count);
                    coordpdb.Add(atompdb[0].coord);
                    coordaln.Add(atomaln[0].coord);
                }
            }

            var trans = Align.MinRMSD.GetTrans(coordaln, coordpdb);

            Dictionary<Pdb.IAtom, Vector> newcoords = new Dictionary<Pdb.IAtom, Vector>();
            foreach(var atom in pdb.atoms  ) newcoords.Add(atom, trans.DoTransform(atom.coord));
            foreach(var atom in pdb.hetatms) newcoords.Add(atom, trans.DoTransform(atom.coord));

            var npdb = pdb.CloneUpdateCoord(newcoords);
            return npdb;
        }
        public static CPsfgenExt PsfgenExt
            ( Pdb pdb
            , string[] toplines
            , string[] parlines
            , Pdb alignto
            , HOptions options
            )
        {
            string[] psfgen_lines = null;
            List<Tuple<string, string, Pdb.IAtom[]>> lstSegFileAtoms = new List<Tuple<string, string, Pdb.IAtom[]>>();
            var pdb_iatoms = pdb.iatoms;
            foreach(var ch_iatoms in pdb_iatoms.GroupChainID())
            {
                lstSegFileAtoms.Add(new Tuple<string, string, Pdb.IAtom[]>
                ( ch_iatoms.Key + ""
                , null
                , ch_iatoms.Value.ToArray()
                ));
            }
            return PsfgenExt
                ( lstSegFileAtoms, toplines, parlines, alignto, psfgen_lines
                , options: options
                );
        }
        public static CPsfgenExt PsfgenExt
            ( IList<Tuple<string, string, Pdb.IAtom[]>> lstSegFileAtoms
            , string[] toplines
            , string[] parlines
            , Pdb alignto
            , string[] psfgen_lines
            , IList<string> minimize_conf_lines = null
            , HOptions options = null
            )
        {
            if(options == null)
                options = new HOptions((string)null);
            string tempbase = @"C:\temp\";
            string psfgen_workdir = null;
            string topname = "prot.top";
            string parname = "prot.par";

            List<string> psf_lines = null;
            List<string> pdb_lines = null;

            using(var temp = new HTempDirectory(tempbase, null))
            {
                temp.EnterTemp();

                HFile.WriteAllLines(topname, toplines);
                HFile.WriteAllLines(parname, parlines);

                if((HFile.Exists("prot.pdb"    ) == false) || (HFile.Exists("prot.psf"   ) == false))
                {
                    var psfgen = Namd.RunPsfgen
                        (lstSegFileAtoms, tempbase, null, "2.10"
                        , new string[] { topname }
                        , new string[] {}
                        , topname
                        , psfgen_lines  : psfgen_lines
                        , psfgen_workdir: psfgen_workdir
                        , options       : options
                        );

                    psf_lines = psfgen.psf_lines;
                    pdb_lines = psfgen.pdb_lines;
                    if(alignto != null)
                    {
                        HDebug.Exception("check!!!");
                        ////////////////////////////
                        Pdb prot = Pdb.FromLines(pdb_lines);
                        prot = PsfgenExt_AlignTo(prot, alignto);
                        pdb_lines = prot.ToLines().ToList();
                    }
                    HFile.WriteAllLines("prot.pdb", pdb_lines);
                    HFile.WriteAllLines("prot.psf", psf_lines);
                }

                if(options.Contains("nomin") == false)
                if((HFile.Exists("protmin.coor") == false) || (HFile.Exists("protmin.pdb") == false))
                {
                    List<string> psfgen_pdb_lines = System.IO.File.ReadLines("prot.pdb").ToList();
                    List<string> psfgen_psf_lines = System.IO.File.ReadLines("prot.psf").ToList();

                    List<string> prm_lines        = System.IO.File.ReadLines(parname).ToList();
                    string Namd2_opt = null;
                    if(options.HSelectStartsWith("minimize option:").Length >= 1)
                        Namd2_opt = options.HSelectStartsWith("minimize option:").First().Replace("minimize option:", "");
                    var minpdb = Namd.Run.Namd2
                        ( psfgen_pdb_lines
                        , psfgen_psf_lines
                        , prm_lines
                        , tempbase
                        , "2.10"
                        , ((Namd2_opt == null) ? "+p3" : Namd2_opt)
                        , conf_lines: minimize_conf_lines
                        );
                    HFile.WriteAllLines("protmin.coor", minpdb.coor_lines);
                
                    Pdb prot0 = Pdb.FromLines(psfgen_pdb_lines);
                    Pdb prot1 = Pdb.FromLines(minpdb.coor_lines);
                    HDebug.Exception(prot0.atoms.Length    == prot1.atoms.Length   );
                    HDebug.Exception(prot0.elements.Length == prot1.elements.Length);
                    // update conformation to minimized conformation
                    for(int i=0; i<prot0.elements.Length; i++ )
                    {
                        if(prot0.elements[i].GetType() != prot1.elements[i].GetType())
                            throw new HException("prot0.elements[i].GetType() != prot1.elements[i].GetType()");
                        if((prot0.elements[i] is Pdb.IAtom) == false)
                            continue;
                        Pdb.IAtom iatom0 = prot0.elements[i] as Pdb.IAtom;
                        Pdb.IAtom iatom1 = prot1.elements[i] as Pdb.IAtom;
                        Vector coord0 = iatom0.coord;
                        Vector coord1 = iatom1.coord;
                        double dist = (coord0 - coord1).Dist;
                        if(iatom0.occupancy != 0)
                            if(dist != 0)
                                throw new HException("iatom0.coord - iatom1.coord != 0");
                        if(dist != 0)
                        {
                            if(iatom0 is Pdb.Atom)
                            {
                                string nline0 = (iatom0 as Pdb.Atom).GetUpdatedLine(coord1);
                                Pdb.Atom natom0 = Pdb.Atom.FromString(nline0);
                                prot0.elements[i] = natom0;
                                continue;
                            }
                            if(iatom0 is Pdb.Hetatm)
                            {
                                string nline0 = (iatom0 as Pdb.Hetatm).GetUpdatedLine(coord1);
                                Pdb.Hetatm natom0 = Pdb.Hetatm.FromString(nline0);
                                prot0.elements[i] = natom0;
                                continue;
                            }
                        }
                    }
                    if((prot0.elements[0] is Pdb.Remark) && (prot1.elements[0] is Pdb.Remark))
                        prot0.elements[0] = Pdb.Remark.FromString(prot1.elements[0].line);
                    prot0.ToFile("protmin.pdb");
                    pdb_lines = System.IO.File.ReadLines("protmin.pdb").ToList();
                }

                //{
                //    Pdb confpdb = GetConfPdb(options);
                //    var psf    = Namd.Psf.FromFile("prot.psf");
                //    var prm    = Namd.Prm.FromFile(parname);
                //    List<string> log = new List<string>();
                //    Universe  univ = Universe.BuilderNamd.Build(psf, prm, confpdb, true, new TextLogger(log));
                //    return univ;
                //}
                temp.QuitTemp();
            }

            return new CPsfgenExt
            {
                psflines = psf_lines,
                pdblines = pdb_lines,
            };
        }
    }
}
