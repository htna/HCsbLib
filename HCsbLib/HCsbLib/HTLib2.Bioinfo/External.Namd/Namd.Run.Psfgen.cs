using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Namd
    {
        public static Run.CPsfgen RunPsfgen
                ( string psfgen_path
                , IList<Tuple<string, string, Pdb.IAtom[]>> lstSegFileAtoms
                , string tempbase //=null
                , string parameters //=null
                //, string namdversion //="2.8"
                , IList<string> infiles
                , IList<string> outfiles
                , string topology
                , IList<string> psfgen_lines = null
                , string psfgen_workdir = null
                , HOptions options = null
                )
        {
            return Run.Psfgen
                ( psfgen_path
                , lstSegFileAtoms
                , tempbase //=null
                , parameters //=null
                //, namdversion //="2.8"
                , infiles
                , outfiles
                , topology
                , psfgen_lines  :psfgen_lines
                , psfgen_workdir:psfgen_workdir
                , options       :options
                );
        }
        public partial class Run
        {
            public class CPsfgen
            {
                public List<string> psf_lines; public Psf psf { get { return Psf.FromLines(psf_lines); } }
                public List<string> pdb_lines; public Pdb pdb { get { return Pdb.FromLines(pdb_lines); } }
                    /// http://www.ks.uiuc.edu/Research/vmd/plugins/psfgen/ug.pdf
                    /// 
                    ///   (9) Guessing missing coordinates. The tolopogy file contains default internal
                    ///       coordinates which can be used to guess the locations of many atoms, hydrogens
                    ///       in particular. In the output pdb file, 
                    ///       * the occupancy field of guessed atoms will be set to 0,
                    ///       * atoms which are known are set to 1, and
                    ///       * atoms which could not be guessed are set to -1.
                    ///       Some atoms are “poorly guessed” if needed bond lengths and angles were missing
                    ///       from the topology file. Similarly, waters with missing hydrogen coordinates
                    ///       are given a default orientation.
            };

            public static string[] GetPsfgenLines
                ( IList<string> custom_pdbalias = null
                , IList<string> custom_patches  = null
                , IList<string> custom_segment  = null
                )
            {
                List<string> psfgen_lines = new List<string>();
                psfgen_lines.AddRange(new string[]
                { "resetpsf"
                , "topology $topology$"
                , ""
                });
                {   // alias
                    psfgen_lines.AddRange(new string[] {""
                    #region Default Aliases : http://www.ks.uiuc.edu/Research/vmd/plugins/autopsf/tech.html
                    , "  pdbalias residue G GUA                     "
                    , "  pdbalias residue C CYT                     "
                    , "  pdbalias residue A ADE                     "
                    , "  pdbalias residue T THY                     "
                    , "  pdbalias residue U URA                     "
                    , "                                             "
                    , "  foreach bp { GUA CYT ADE THY URA } {       "
                    , "     pdbalias atom $bp \"O5\\*\" O5'         "
                    , "     pdbalias atom $bp \"C5\\*\" C5'         "
                    , "     pdbalias atom $bp \"O4\\*\" O4'         "
                    , "     pdbalias atom $bp \"C4\\*\" C4'         "
                    , "     pdbalias atom $bp \"C3\\*\" C3'         "
                    , "     pdbalias atom $bp \"O3\\*\" O3'         "
                    , "     pdbalias atom $bp \"C2\\*\" C2'         "
                    , "     pdbalias atom $bp \"O2\\*\" O2'         "
                    , "     pdbalias atom $bp \"C1\\*\" C1'         "
                    , "  }                                          "
                    , "                                             "
                    , "  pdbalias atom ILE CD1 CD                   "
                    , "  pdbalias atom SER HG HG1                   "
                    , "  pdbalias residue HIS HSD                   "
                    , "                                             "
                    , "# Heme aliases                               "
                    , "  pdbalias residue HEM HEME                  "
                    , "  pdbalias atom HEME \"N A\" NA              "
                    , "  pdbalias atom HEME \"N B\" NB              "
                    , "  pdbalias atom HEME \"N C\" NC              "
                    , "  pdbalias atom HEME \"N D\" ND              "
                    , "                                             "
                    , "# Water aliases                              "
                    , "  pdbalias residue HOH TIP3                  "
                    , "  pdbalias atom TIP3 O OH2                   "
                    , "                                             "
                    , "# Ion aliases                                "
                    , "  pdbalias residue K POT                     "
                    , "  pdbalias atom K K POT                      "
                    , "  pdbalias residue ICL CLA                   "
                    , "  pdbalias atom ICL CL CLA                   "
                    , "  pdbalias residue INA SOD                   "
                    , "  pdbalias atom INA NA SOD                   "
                    , "  pdbalias residue CA CAL                    "
                    , "  pdbalias atom CA CA CAL                    "
                    , "                                             "
                    , "# Other aliases                              "
                    , "  pdbalias atom LYS 1HZ HZ1                  "
                    , "  pdbalias atom LYS 2HZ HZ2                  "
                    , "  pdbalias atom LYS 3HZ HZ3                  "
                    , "                                             "
                    , "  pdbalias atom ARG 1HH1 HH11                "
                    , "  pdbalias atom ARG 2HH1 HH12                "
                    , "  pdbalias atom ARG 1HH2 HH21                "
                    , "  pdbalias atom ARG 2HH2 HH22                "
                    , "                                             "
                    , "  pdbalias atom ASN 1HD2 HD21                "
                    , "  pdbalias atom ASN 2HD2 HD22                "
                    #endregion
                    , ""
                    });
                    if(custom_pdbalias != null) psfgen_lines.AddRange(custom_pdbalias);
                }
                {   // segments
                    if(custom_segment != null)
                        psfgen_lines.AddRange(custom_segment );
                    else
                    {
                        psfgen_lines.AddRange(new string[]
                        { ""
                        , "segment $segname$ { pdb $segfilename$.pdb }; coordpdb $segfilename$.pdb $segname$"
                        //, "segment $segname$ { pdb $segfilename$.pdb; first NTER; last CTER; auto angles dihedrals }; \n coordpdb $segfilename$.pdb $segname$"
                        //, "segment $segname$ { pdb $segfilename$.pdb; first NONE; last NONE; auto none             }; \n coordpdb $segfilename$.pdb $segname$"
                        , ""
                        });
                    }
                }
                if(custom_patches != null) psfgen_lines.AddRange(custom_patches);
                psfgen_lines.AddRange(new string[] {""
                , "guesscoord"
                , ""
                , "writepsf prot.psf"
                , "writepdb prot.pdb"
                });

                return psfgen_lines.ToArray();
            }
            public static CPsfgen Psfgen
                ( string psfgen_path
                , IList<Tuple<string, string, Pdb.IAtom[]>> lstSegFileAtoms // segname, filename, pdbatoms
                , string tempbase //=null
                , string parameters //=null
                //, string namdversion //="2.8"
                , IList<string> infiles
                , IList<string> outfiles
                , string topology
                , IList<string> psfgen_lines = null
                , string psfgen_workdir = null
                , HOptions options = null
                )
            {
                if(options == null)
                    options = new HOptions((string)null);
                Dictionary<System.IO.FileInfo, string[]> infile_lines = new Dictionary<System.IO.FileInfo, string[]>();
                foreach(string infile in infiles)
                    infile_lines.Add(HFile.GetFileInfo(infile), HFile.ReadAllLines(infile));

                string currpath = HEnvironment.CurrentDirectory;
                System.IO.DirectoryInfo tmpdir = null;
                if(psfgen_workdir != null)
                {
                    HEnvironment.CurrentDirectory = psfgen_workdir;
                }
                else
                {
                    tmpdir = HDirectory.CreateTempDirectory(tempbase);
                    HEnvironment.CurrentDirectory = tmpdir.FullName;
                }

                string[] lines = null;
                if((psfgen_lines != null) && (psfgen_lines.Count > 0))
                {
                    lines = psfgen_lines.ToArray();
                }
                else
                {
                    lines = GetPsfgenLines
                        ( custom_pdbalias:null
                        , custom_patches :null
                        );
                }

                if(topology != null)
                    lines = lines.ToArray().HReplace("$topology$", topology);

                List<string> psf_lines;
                List<string> pdb_lines;
                {
                    //  {
                    //      //foreach(var respath_filename in GetResourcePaths("2.8", "psfgen"))
                    //      foreach(var respath_filename in GetResourcePaths(namdversion, "psfgen"))
                    //      {
                    //          string respath  = respath_filename.Item1;
                    //          string filename = respath_filename.Item2;
                    //          HResource.CopyResourceTo<Tinker>(respath, filename);
                    //      }
                    //  }

                    //  Dictionary<string, Tuple<string, Pdb.IAtom[]>> segname_filename_pdbatoms = new Dictionary<string, Tuple<string, Pdb.IAtom[]>>();
                    //  //if(pdbs.Length != 1) throw new ArgumentException();
                    //  for(int i=0; i<lstSegFilePdb.Count; i++)
                    //  {
                    //      string  segnameprefix = lstSegFilePdb[i].Item1; if( segnameprefix == null)  segnameprefix = string.Format("{0:00}", i);
                    //      string filenameprefix = lstSegFilePdb[i].Item2; if(filenameprefix == null) filenameprefix = string.Format("{0:00}", i);
                    //      Pdb    pdb            = lstSegFilePdb[i].Item3;
                    //      List<Pdb.IAtom> pdb_atoms = new List<Pdb.IAtom>();
                    //      pdb_atoms.AddRange(pdb.atoms);
                    //      pdb_atoms.AddRange(pdb.hetatms);
                    //      char[] chains = pdb_atoms.ListChainID().HToHashSet().ToArray();
                    //  
                    //      HDebug.AssertIf(chains.Length> 1, segnameprefix.Length <= 5);
                    //      HDebug.AssertIf(chains.Length<=1, segnameprefix.Length <= 6);
                    //      foreach(char chain in chains)
                    //      {
                    //          Pdb.IAtom[] chain_atoms = pdb_atoms.SelectByChainID(chain).SelectByAltLoc().ToArray();
                    //          string suffix = null;
                    //          if(('a' <= chain) && (chain <= 'z')) suffix = string.Format("L{0}", chain);
                    //          if(('A' <= chain) && (chain <= 'Z')) suffix = string.Format("U{0}", chain);
                    //          if(('0' <= chain) && (chain <= '9')) suffix = string.Format("N{0}", chain);
                    //          string  segname =  segnameprefix + ((chains.Length <= 1) ? "" : suffix);
                    //          string filename = filenameprefix + ((chains.Length <= 1) ? "" : suffix);
                    //          segname_filename_pdbatoms.Add(segname, new Tuple<string,Pdb.IAtom[]>(filename, chain_atoms));
                    //      }
                    //  }

                    foreach(var finfo_line in infile_lines)
                    {
                        string   inname  = finfo_line.Key.Name;
                        string[] inlines = finfo_line.Value;
                        HFile.WriteAllLines(inname, inlines);
                    }

                    HashSet<string> segnames = new HashSet<string>();
                    int             segindex = 0;
                    foreach(var seg_file_atoms in lstSegFileAtoms)
                    {
                        string      segname     = seg_file_atoms.Item1;
                        string      filename    = seg_file_atoms.Item2;
                        Pdb.IAtom[] chain_atoms = seg_file_atoms.Item3.SelectByAltLoc().ToArray();
                        HDebug.Exception(chain_atoms.ListChainID().HToHashSet().Count == 1);
                        if(segname == null)
                        {
                            while(segindex <= 9999)
                            {
                                if(segnames.Contains(segindex.ToString()) == false)
                                {
                                    segname = segindex.ToString();
                                    segnames.Add(segname);
                                    break;
                                }
                                segindex++;
                            }
                        }
                        if(filename == null)
                            filename = segname;

                        Pdb.ToFile
                            ( filename+".pdb"
                            , chain_atoms.HToType<Pdb.IAtom, Pdb.Element>()
                            , false
                            );

                        for(int i=0; i<lines.Length; i++)
                        {
                            if(lines[i].Contains("$segname$"))
                            {
                                string insert = lines[i];
                                insert = insert.Replace("$segname$", segname);
                                insert = insert.Replace("$segfilename$", filename);
                                lines = lines.HInsert(i, insert);
                                i++;
                            }
                        }
                    }

                    lines = lines.HRemoveAllContains("$segname$");

                    HFile.WriteAllLines("prot.inp", lines);
                    string command0 = string.Format(psfgen_path+" < prot.inp");
                    bool pause = options.Contains("psfgen pause");
                    HProcess.StartAsBatchInConsole(null, pause, command0);

                    psf_lines = System.IO.File.ReadLines("prot.psf").ToList();
                    pdb_lines = System.IO.File.ReadLines("prot.pdb").ToList();
                }
                HEnvironment.CurrentDirectory = currpath;
                if(tmpdir != null) { try{ tmpdir.Delete(true); } catch {} }

                return new CPsfgen
                {
                    psf_lines = psf_lines,
                    pdb_lines = pdb_lines,
                };
            }
        }
    }
}
