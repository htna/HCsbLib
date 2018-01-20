using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using StreamWriter = System.IO.StreamWriter;
    public static partial class UniverseStatic
    {
        public static Universe.RotableInfo SelectByBond(this IList<Universe.RotableInfo> rotinfos
                                                       , Tuple<int, string> atom1ResidName
                                                       , Tuple<int, string> atom2ResidName
                                                       )
        {
            int    res1 = atom1ResidName.Item1;
            string atm1 = atom1ResidName.Item2;
            int    res2 = atom2ResidName.Item1;
            string atm2 = atom2ResidName.Item2;
            Universe.RotableInfo sele_rotinfo = null;
            foreach(var rotinfo in rotinfos)
            {
                List<Tuple<Universe.Atom, Universe.Atom>> atompairs = new List<Tuple<Universe.Atom, Universe.Atom>>();
                atompairs.Add(new Tuple<Universe.Atom, Universe.Atom>(rotinfo.bond.atoms[0], rotinfo.bond.atoms[1]));
                atompairs.Add(new Tuple<Universe.Atom, Universe.Atom>(rotinfo.bond.atoms[1], rotinfo.bond.atoms[0]));
                foreach(var atompair in atompairs)
                {
                    if( (atm1 == atompair.Item1.AtomName.Trim())
                     && (atm2 == atompair.Item2.AtomName.Trim())
                     && (res1 == atompair.Item1.ResiduePdbId   )
                     && (res2 == atompair.Item2.ResiduePdbId   )
                        )
                    {
                        HDebug.Assert(sele_rotinfo == null);
                        sele_rotinfo=rotinfo;
                    }
                }
            }
            return sele_rotinfo;
        }
        public static double[] GetDEnergeByDRotinfo(this IList<Universe.RotableInfo> rotinfos, Universe univ, double[,] pwfrc, Universe.Nonbondeds_v1 nonbondeds)
        {
            double[] denerge_dangles = new double[rotinfos.Count];
            for(int i=0; i<rotinfos.Count; i++)
            {
                denerge_dangles[i] = rotinfos[i].GetDEnergeByDRotinfo(univ, pwfrc, nonbondeds);
                //System.Console.Write(", "+i);
            }
            //System.Console.WriteLine();
            return denerge_dangles;
        }
        public static Universe.RotableInfo[] ListRotableInfoBackbone_depreciated(this IList<Universe.RotableInfo> rotinfos)
        {
            HDebug.Depreciated();
            List<Universe.RotableInfo> backbones = new List<Universe.RotableInfo>();
            foreach(var rotinfo in rotinfos)
            {
                Tuple<string, string, Universe.Atom, Universe.Atom, string> torinfo = rotinfo.GetTorInfo();
                if(torinfo.Item1 == "backbone")
                    backbones.Add(rotinfo);
            }
            return backbones.ToArray();
        }
        public static Universe.RotableInfo[] ListRotableInfoBackbone(this IList<Universe.RotableInfo> rotinfos, params string[] nameBkbnExt)
        {
            Tuple<int, string>[] lstResiType = rotinfos.GetPdbResiBackboneOrSidechain(nameBkbnExt);
            List<Universe.RotableInfo> backbones = new List<Universe.RotableInfo>();
            for(int irot=0; irot<rotinfos.Count; irot++)
            {
                int    resi = lstResiType[irot].Item1;
                string type = lstResiType[irot].Item2;
                if(type == "backbone")
                    backbones.Add(rotinfos[irot]);
            }
            return backbones.ToArray();
        }
        public static Universe.Molecule[] ListMolecule(this IList<Universe.RotableInfo> rotinfos)
        {
            Universe.Molecule[] moles = new Universe.Molecule[rotinfos.Count];
            for(int i=0; i<rotinfos.Count; i++)
                moles[i] = rotinfos[i].mole;
            return moles;
        }
        public static Dictionary<Universe.Molecule, List<Universe.RotableInfo>> GroupByMolecule(this IList<Universe.RotableInfo> rotinfos)
        {
            Dictionary<Universe.Molecule, List<Universe.RotableInfo>> mole2rotinfos = new Dictionary<Universe.Molecule,List<Universe.RotableInfo>>();
            foreach(var rotinfo in rotinfos)
            {
                var mole = rotinfo.mole;
                if(mole2rotinfos.ContainsKey(mole) == false)
                    mole2rotinfos.Add(mole, new List<Universe.RotableInfo>());
                mole2rotinfos[mole].Add(rotinfo);
            }
            return mole2rotinfos;
        }
        public static Tuple<int, int>[] GetIdxPdbResiBackbone(this IList<Universe.RotableInfo> rotinfos, params string[] nameBkbnExt)
        {
            List<Tuple<int, int>> lstIdxResi = new List<Tuple<int, int>>();
            Tuple<int, string>[] lstResiType = rotinfos.GetPdbResiBackboneOrSidechain(nameBkbnExt);
            for(int irot=0; irot<rotinfos.Count; irot++)
            {
                int    resi = lstResiType[irot].Item1;
                string type = lstResiType[irot].Item2;
                if(type == "backbone")
                    lstIdxResi.Add(new Tuple<int, int>(irot, resi));
            }
            return lstIdxResi.ToArray();
        }
        public static Tuple<int, int>[] GetIdxPdbResiSidechain(this IList<Universe.RotableInfo> rotinfos, bool ignoreHydrogenRot, params string[] nameBkbnExt)
        {
            List<Tuple<int, int>> lstIdxResi = new List<Tuple<int, int>>();
            Tuple<int, string>[] lstResiType = rotinfos.GetPdbResiBackboneOrSidechain(nameBkbnExt);
            for(int irot=0; irot<rotinfos.Count; irot++)
            {
                int    resi = lstResiType[irot].Item1;
                string type = lstResiType[irot].Item2;
                if(type == "sidechain")
                {
                    var torinfo = rotinfos[irot].GetTorInfo();
                    if(ignoreHydrogenRot)
                    {
                        switch(torinfo.Item5)
                        {
                            case "-CHHH": continue;
                            case "-CHH" : continue;
                            case "-OH"  : continue;
                            case "-SH"  : continue;
                            case "-NHH" : continue;
                            case "-NHHH": continue;
                            case "": break;
                            default: HDebug.Assert(false); break;
                        }
                    }
                    lstIdxResi.Add(new Tuple<int, int>(irot, resi));
                }
            }
            return lstIdxResi.ToArray();
        }
        public static Tuple<int, string>[] GetPdbResiBackboneOrSidechain(this IList<Universe.RotableInfo> rotinfos, params string[] nameBkbnExt)
        {
            Tuple<int, string>[] lstResiType = new Tuple<int, string>[rotinfos.Count];
            string[] nameBkbn = new string[] { "N", "CA", "C", "O" };

            for(int irot=0; irot<rotinfos.Count; irot++)
            {
                Universe.Atom atm0 = rotinfos[irot].bond.atoms[0]; HDebug.Assert(atm0.AtomName[0] != 'H');
                Universe.Atom atm1 = rotinfos[irot].bond.atoms[1]; HDebug.Assert(atm1.AtomName[0] != 'H');
                
                bool bkbn = false;
                int  resi = -1;
                {
                    if(nameBkbn.Contains(atm0.AtomName) && nameBkbn.Contains(atm1.AtomName))
                    {
                        HDebug.Assert(atm0.ResidueId == atm1.ResidueId);
                        HDebug.Assert(bkbn == false);
                        bkbn = true;
                        resi = atm0.ResiduePdbId;
                    }
                    if(nameBkbnExt.Contains(atm0.AtomName))
                    {
                        HDebug.Assert(bkbn == false);
                        bkbn = true;
                        resi = atm0.ResiduePdbId;
                    }
                    if(nameBkbnExt.Contains(atm1.AtomName))
                    {
                        HDebug.Assert(bkbn == false);
                        bkbn = true;
                        resi = atm1.ResiduePdbId;
                    }
                    if(resi == -1)
                    {
                        HDebug.Assert(atm0.ResidueId == atm1.ResidueId);
                        resi = atm0.ResiduePdbId;
                    }
                }

                string type = (bkbn ? "backbone" : "sidechain");
                lstResiType[irot] = new Tuple<int, string>(resi, type);
            }

            return lstResiType;
        }
    }
	public partial class Universe
	{
        public class RotableInfo
        {
            public Bond   bond      ; // rotatable bond
            public Atom[] rotAtoms  ; // atoms moving by rotating the bond
            public Atom   bondedAtom; // atoms in both rotAtoms and bond
            public Molecule mole;

            public double GetDEnergeByDRotinfo(Universe univ, double[,] pwfrc, Nonbondeds_v1 nonbondeds)
            {
                double denerge_dangle = Universe.GetDEnergeByDRotinfo(univ, this, pwfrc, nonbondeds);
                return denerge_dangle;
            }
            public Trans3 GetTrans(Vector[] coords, double dangle)
            {
                int id0, id1;
                {
                    var bondatoms = bond.atoms.HToHashSet();
                    bondatoms.Remove(bondedAtom);
                    HDebug.Assert(bondatoms.Count == 1);
                    HDebug.Assert(rotAtoms.Contains(bondatoms.First()) == false);
                    id0 = bondedAtom.ID;
                    id1 = bondatoms.First().ID;
                }
                Vector axisPt  = coords[id0];
                Vector axisVec = (coords[id0] - coords[id1]).UnitVector();

                Trans3 trans = Trans3.UnitTrans;
                trans = Trans3.AppendTrans(trans, Trans3.FromMove(-axisPt));
                trans = Trans3.AppendTrans(trans, Trans3.FromRotate(axisVec, dangle));
                trans = Trans3.AppendTrans(trans, Trans3.FromMove(axisPt));

                return trans;
            }
            public override string ToString()
            {
                var torinfo = GetTorInfo();
                string str = string.Format("{0}={1}, rot {2} atoms,", torinfo.Item1, torinfo.Item2, rotAtoms.Length);
                return str;
            }
            public Tuple<string, string, Universe.Atom, Universe.Atom, string> GetTorInfo()
            {
                Universe.Atom atom0 = bond.atoms[0];
                Universe.Atom atom1 = bond.atoms[1];
                string tortype;
                {
                    IList<string> atmnames = (new string[2] { atom0.AtomName, atom1.AtomName }).HSort();
                    tortype = atmnames[0]+"-"+atmnames[1];
                }
                // check backbone
                switch(tortype)
                {
                    case "CA-N": HDebug.Assert(bond.IsBackbone().Item1 == "N-CA"); return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("backbone","N-CA   ",atom0,atom1,"");
                    case "C-CA": HDebug.Assert(bond.IsBackbone().Item1 == "CA-C"); return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("backbone","CA-C   ",atom0,atom1,"");
                    case "C-N" :
                        HDebug.Assert(bond.IsBackbone().Item1 == "C-N");
                        HDebug.Assert(false); // improper
                        return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("backbone","C-N",atom0,atom1,"");
                }
                // side chain
                if(atom0.ResidueId   == atom1.ResidueId)
                {
                    string resid = atom0.ResidueId;
                    string ltortype = atom0.ResidueName+"-"+tortype;
                    switch(ltortype)
                    {
                        /// Hydrophobic
                        case "ALA-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ALA","CA-CB  ",atom0,atom1,"-CHHH");
                        case "VAL-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("VAL","CA-CB  ",atom0,atom1,"");
                        case "VAL-CB-CG1": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("VAL","CB-CG1 ",atom0,atom1,"-CHHH");
                        case "VAL-CB-CG2": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("VAL","CB-CG2 ",atom0,atom1,"-CHHH");
                        case "PHE-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("PHE","CA-CB  ",atom0,atom1,"");
                        case "PHE-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("PHE","CB-CG  ",atom0,atom1,"");
                        case "LEU-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LEU","CA-CB  ",atom0,atom1,"");
                        case "LEU-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LEU","CB-CG  ",atom0,atom1,"");
                        case "LEU-CD1-CG": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LEU","CG-CD1 ",atom1,atom0,"-CHHH");
                        case "LEU-CD2-CG": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LEU","CG-CD2 ",atom1,atom0,"-CHHH");
                        case "ILE-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ILE","CA-CB  ",atom0,atom1,"");
                        case "ILE-CB-CG1": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ILE","CB-CG1 ",atom0,atom1,"");
                        case "ILE-CB-CG2": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ILE","CB-CG2 ",atom0,atom1,"-CHHH");
                        case "ILE-CD1-CG1":return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ILE","CG1-CD1",atom1,atom0,"-CHHH");
                        case "ILE-CD-CG1": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ILE","CG1-CD ",atom1,atom0,"-CHHH");
                        /// Hydrophilic
                        case "ARG-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ARG","CA-CB  ",atom0,atom1,"");
                        case "ARG-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ARG","CB-CG  ",atom0,atom1,"");
                        case "ARG-CD-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ARG","CG-CD  ",atom1,atom0,"");
                        case "ARG-CD-NE" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ARG","CD-NE  ",atom0,atom1,"");
                        case "ARG-CZ-NE" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ARG","NE-CZ  ",atom0,atom1,"");
                        case "ARG-CZ-NH2": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ARG","CZ-NH2 ",atom0,atom1,"-CHH");
                        case "ASP-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ASP","CA-CB  ",atom0,atom1,"");
                        case "ASP-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ASP","CB-CG  ",atom0,atom1,"");
                        case "GLU-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("GLU","CA-CB  ",atom0,atom1,"");
                        case "GLU-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("GLU","CB-CG  ",atom0,atom1,"");
                        case "GLU-CD-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("GLU","CG-CD  ",atom1,atom0,"");
                        case "SER-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("SER","CA-CB  ",atom0,atom1,"");
                        case "SER-CB-OG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("SER","CB-OG  ",atom0,atom1,"-OH");
                        case "CYS-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("CYS","CA-CB  ",atom0,atom1,"");
                        case "CYS-CB-SG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("CYS","CB-SG  ",atom0,atom1,"-SH");
                        case "ASN-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ASN","CA-CB  ",atom0,atom1,"");
                        case "ASN-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ASN","CB-CG  ",atom0,atom1,"");
                        case "ASN-CG-ND2": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("ASN","CG-ND2 ",atom0,atom1,"-NHH");
                        case "GLN-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("GLN","CA-CB  ",atom0,atom1,"");
                        case "GLN-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("GLN","CB-CG  ",atom0,atom1,"");
                        case "GLN-CD-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("GLN","CG-CD  ",atom1,atom0,"");
                        case "GLN-CD-NE2": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("GLN","CD-NE2 ",atom0,atom1,"-NHH");
                /*HIS*/ case "HIS-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HIS","CA-CB  ",atom0,atom1,"");
                /*HIS*/ case "HIS-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HIS","CB-CG  ",atom0,atom1,"");
                /*HIS*/ case "HSD-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HSD","CA-CB  ",atom0,atom1,"");
                /*HIS*/ case "HSD-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HSD","CB-CG  ",atom0,atom1,"");
                /*HIS*/ case "HSE-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HSE","CA-CB  ",atom0,atom1,"");
                /*HIS*/ case "HSE-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HSE","CB-CG  ",atom0,atom1,"");
                        /// Hydrophilic-Amphipathic
                        case "THR-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("THR","CA-CB  ",atom0,atom1,"");
                        case "THR-CB-OG1": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("THR","CB-OG1 ",atom0,atom1,"-OH");
                        case "THR-CB-CG2": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("THR","CB-CG2 ",atom0,atom1,"-CHHH");
                        /// Amphipathic
                        case "LYS-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LYS","CA-CB  ",atom0,atom1,"");
                        case "LYS-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LYS","CB-CG  ",atom0,atom1,"");
                        case "LYS-CD-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LYS","CG-CD  ",atom1,atom0,"");
                        case "LYS-CD-CE" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LYS","CD-CE  ",atom0,atom1,"");
                        case "LYS-CE-NZ" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("LYS","CE-NZ  ",atom0,atom1,"-NHHH");
                        case "TYR-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("TYR","CA-CB  ",atom0,atom1,"");
                        case "TYR-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("TYR","CB-CG  ",atom0,atom1,"");
                        case "TYR-CZ-OH" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("TYR","CZ-OH  ",atom0,atom1,"-OH");
                        case "MET-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("MET","CA-CB  ",atom0,atom1,"");
                        case "MET-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("MET","CB-CG  ",atom0,atom1,"");
                        case "MET-CG-SD" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("MET","CG-SD  ",atom0,atom1,"");
                        case "MET-CE-SD" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("MET","SD-CE  ",atom0,atom1,"-CHHH");
                        case "TRP-CA-CB" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("TRP","CA-CB  ",atom0,atom1,"");
                        case "TRP-CB-CG" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("TRP","CB-CG  ",atom0,atom1,"");
                    }
                }
                if(atom0.ResidueName == "HEM" || atom1.ResidueName == "HEM")
                {
                    string resid = null;
                    if(atom0.ResidueName == "HEM") { HDebug.Assert(resid==null || resid==atom0.ResidueId); resid = atom0.ResidueId; }
                    if(atom1.ResidueName == "HEM") { HDebug.Assert(resid==null || resid==atom1.ResidueId); resid = atom1.ResidueId; }
                    string ltortype = "HEM-"+tortype;
                    switch(ltortype)
                    {
                        case "HEM-FE-SG"  : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","SG-FE  ",atom0,atom1,"");
                        case "HEM-FE-NE2" : return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","NE2-FE ",atom0,atom1,"");
                        case "HEM-C2A-CAA": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C2A-CAA",atom0,atom1,"");
                        case "HEM-CAA-CBA": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","CAA-CBA",atom0,atom1,"");
                        case "HEM-CBA-CGA": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","CBA-CGA",atom0,atom1,"");
                        case "HEM-C3A-CMA": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C3A-CMA",atom0,atom1,"");
                        case "HEM-C2B-CMB": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C2B-CMB",atom0,atom1,"");
                        case "HEM-C3B-CAB": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C3B-CAB",atom0,atom1,"");
                        case "HEM-CAB-CBB": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","CAB-CBB",atom0,atom1,"");
                        case "HEM-C2C-CMC": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C2C-CMC",atom0,atom1,"");
                        case "HEM-C3C-CAC": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C3C-CAC",atom0,atom1,"");
                        case "HEM-CAC-CBC": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","CAC-CBC",atom0,atom1,"");
                        case "HEM-C2D-CMD": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C2D-CMD",atom0,atom1,"");
                        case "HEM-C3D-CAD": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","C3D-CAD",atom0,atom1,"");
                        case "HEM-CAD-CBD": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","CAD-CBD",atom0,atom1,"");
                        case "HEM-CBD-CGD": return new Tuple<string,string,Universe.Atom,Universe.Atom,string>("HEM","CBD-CGD",atom0,atom1,"");
                    }
                }
                                //tortype = atom0.ResidueName+"-"+tortype;
                //Debug.Assert(atom0.ResidueId   == atom1.ResidueId  );
                //Debug.Assert(atom0.ResidueName == atom1.ResidueName);

                //if(
                HDebug.Assert(false);
                return null;
            }
        }
        public static double GetDEnergeByDRotinfo(Universe univ, RotableInfo rotinfo, double[,] pwfrc, Nonbondeds_v1 nonbondeds)
        {
            if(pwfrc == null)
            {
                HDebug.Assert(false);
                Vector[] forces  = univ.GetVectorsZero();
                MatrixByArr hess = null;// new double[univ.size*3, univ.size*3];
                pwfrc = new double[univ.size, univ.size];
                List<ForceField.IForceField> frcflds = new List<ForceField.IForceField>();
                Dictionary<string, object> univ_cache = new Dictionary<string, object>();
                double energy  = univ.GetPotential(frcflds, ref forces, ref hess, univ_cache, pwfrc: pwfrc);
            }
            Vector[] coords = univ.GetCoords();
            if(nonbondeds == null)
            {
                nonbondeds = new Nonbondeds_v1(univ.atoms, univ.size, 12);
                nonbondeds.UpdateNonbondeds(coords, 0);
            }

            //foreach(Universe.RotableInfo rotinfo in univrotinfos)
            double dtotenrg_dangle = 0;
            {
                int[] id2info = new int[univ.size];                         //  0 : base atoms
                foreach(var atom in rotinfo.rotAtoms) id2info[atom.ID] = 1; //  1 : rot atoms
                id2info[rotinfo.bond.atoms[0].ID] = -1;                     // -1 : axis
                id2info[rotinfo.bond.atoms[1].ID] = -1;
                List<int> idrots  = id2info.HIdxEqual(1).ToList();
                List<int> idbases = id2info.HIdxEqual(0).ToList();
                int count = 0;
                if(idrots.Count < idbases.Count)
                {
                    // rotate atom(s) in rotinfo.rotAtoms
                    Vector rotOrigin = coords[rotinfo.bondedAtom.ID];
                    Vector rotAxis   = coords[rotinfo.bond.atoms[1].ID] - coords[rotinfo.bond.atoms[0].ID];
                    double rotAngle  = 0.00001;
                    Quaternion rot   = new Quaternion(rotAxis, rotAngle);
                    MatrixByArr rotMat    = rot.RotationMatrix;

                    foreach(int idrot in idrots)
                    {
                        Universe.Atom atmrot = univ.atoms[idrot];
                        Vector coord_before = coords[idrot];
                        Vector coord_after  = rotMat * (coord_before - rotOrigin) + rotOrigin;

                        List<Universe.Atom> atmbases = nonbondeds.ListAtomsNonbondedsOf(atmrot);
                        foreach(Universe.Atom atmbase in atmbases)
                        {
                            if(id2info[atmbase.ID] == 0)
                            {
                                HDebug.Assert(id2info[atmrot.ID] != id2info[atmbase.ID]);
                                HDebug.Assert(id2info[atmrot.ID] != -1);
                                HDebug.Assert(id2info[atmbase.ID] != -1);
                                count++;
                                double dist_before  = (coords[atmbase.ID] - coord_before).Dist;
                                double dist_after   = (coords[atmbase.ID] - coord_after).Dist;
                                double ddist_dangle = (dist_after-dist_before)/rotAngle;
                                double denrg_ddist  = pwfrc[atmrot.ID, atmbase.ID];
                                double denrg_dangle = denrg_ddist * ddist_dangle;
                                dtotenrg_dangle += denrg_dangle;
                            }
                        }
                    }
                }
                else
                {
                    // The result of this should be same to the above, but this save computation
                    // rotate atom(s) NOT in rotinfo.rotAtoms
                    Vector rotOrigin = coords[rotinfo.bondedAtom.ID];
                    Vector rotAxis   = coords[rotinfo.bond.atoms[1].ID] - coords[rotinfo.bond.atoms[0].ID];
                    double rotAngle  = 0.00001;
                    Quaternion rot   = new Quaternion(rotAxis, -rotAngle);
                    MatrixByArr rotMat    = rot.RotationMatrix;

                    foreach(int idbase in idbases)
                    {
                        Universe.Atom atmbase = univ.atoms[idbase];
                        Vector coord_before = coords[idbase];
                        Vector coord_after  = rotMat * (coord_before - rotOrigin) + rotOrigin;

                        List<Universe.Atom> atmrots = nonbondeds.ListAtomsNonbondedsOf(atmbase);
                        foreach(Universe.Atom atmrot in atmrots)
                        {
                            if(id2info[atmrot.ID] == 1)
                            {
                                HDebug.Assert(id2info[atmrot.ID] != id2info[atmbase.ID]);
                                HDebug.Assert(id2info[atmrot.ID] != -1);
                                HDebug.Assert(id2info[atmbase.ID] != -1);
                                count++;
                                double dist_before  = (coords[atmrot.ID] - coord_before).Dist;
                                double dist_after   = (coords[atmrot.ID] - coord_after).Dist;
                                double ddist_dangle = (dist_after-dist_before)/rotAngle;
                                double denrg_ddist  = pwfrc[atmrot.ID, atmbase.ID];
                                double denrg_dangle = denrg_ddist * ddist_dangle;
                                dtotenrg_dangle += denrg_dangle;
                            }
                        }
                    }
                }
            }

            return dtotenrg_dangle;
        }
        public List<RotableInfo> GetRotableInfo()
        {
            HDebug.Depreciated("call GetRotableInfo(null,null), instead");
            Graph<Atom[], Bond> flexgraph = null;
            return GetRotableInfo(flexgraph);
        }
        public List<RotableInfo> GetRotableInfo(Graph<Atom[], Bond> flexgraph //=null
                                               )
        {
            if(flexgraph == null)
                flexgraph = BuildFlexibilityGraph();
            //////////////////////////////////////////////////////////////////////////
            // check that there is no loop in the graph
            List<List<Graph<Atom[],Bond>.Node>> loops = flexgraph.FindLoops();
            HDebug.Assert(loops.Count == 0);
            //////////////////////////////////////////////////////////////////////////
            // for all molecules
            Molecule[] moles = GetMolecules();
            List<RotableInfo> rotInfos = new List<RotableInfo>();
            for(int im=0; im<moles.Length; im++)
            {
                // select atom0 whose ID is the smallest in molecule
                Atom atom0 = moles[im].atoms.First();
                foreach(var atom in moles[im].atoms)
                    if(atom.ID < atom0.ID)
                        atom0 = atom;
                //////////////////////////////////////////////////////////////////////////
                // find the root node, which has the atom0
                Graph<Atom[],Bond>.Node root = null;
                foreach(Graph<Atom[],Bond>.Node node in flexgraph.Nodes)
                {
                    if(flexgraph.GetValue(node).Contains(atom0))
                    {
                        HDebug.Assert(root == null);
                        root = node;
                    }
                }
                //////////////////////////////////////////////////////////////////////////
                // 
                Tree<Tuple<Graph<Atom[],Bond>.Node,Graph<Atom[],Bond>.Edge>> tree = flexgraph.BuildTree(root);
                foreach(Tree.Node node in tree.ListDescendents(tree.Root,false))
                {
                    Atom[] parents = flexgraph.GetValue(tree.GetValue(node.parent).Item1);
                    Bond   bond    = flexgraph.GetValue(tree.GetValue(node).Item2);

                    List<Tree.Node> descendent_nodes = tree.ListDescendents(node, true);
                    List<Atom[]> descendentss = new List<Atom[]>(descendent_nodes.Count);
                    List<Atom> descendents = new List<Atom>();
                    foreach(Tree.Node descendent_node in descendent_nodes)
                    {
                        Atom[] ldescendents = flexgraph.GetValue(tree.GetValue(descendent_node).Item1);
                        descendentss.Add(ldescendents);
                        descendents.AddRange(ldescendents);
                    }
                    Atom bondedInDescendents = null;
                    if(descendents.Contains(bond.atoms[0])) { HDebug.Assert(bondedInDescendents == null); bondedInDescendents = bond.atoms[0]; }
                    if(descendents.Contains(bond.atoms[1])) { HDebug.Assert(bondedInDescendents == null); bondedInDescendents = bond.atoms[1]; }
                    HDebug.Assert(bondedInDescendents != null);

                    rotInfos.Add(new RotableInfo{bond = bond
                                                ,bondedAtom = bondedInDescendents
                                                ,rotAtoms = descendents.ToArray()
                                                ,mole     = moles[im]
                                                });
                }
            }
            return rotInfos;
        }
        public Graph<Atom[], Bond> BuildFlexibilityGraph()
        {
            HDebug.Depreciated("call BuildFlexibilityGraph(null), instead");
            IList<Bond> cutrotbonds = null;
            return BuildFlexibilityGraph(null as IList<Bond>);
        }
        public Graph<Atom[], Bond> BuildFlexibilityGraph(IList<Bond> cutrotbonds //=null
                                                        )
        {
            List<Atom[]> rigids = FindRigidUnits(cutrotbonds);
            HDebug.Assert(rigids.HListCount().Sum() == atoms.Count);
            List<Bond> rotables = FindRotableBonds(rigids, cutrotbonds);

            Graph<Atom[], Bond> flexGraph = new Graph<Atom[], Bond>();
            foreach(Atom[] rigid in rigids)
                flexGraph.AddNode(rigid);
            HDebug.Assert(flexGraph.Nodes.Count == rigids.Count);
            foreach(Bond rotable in rotables)
            {
                Atom[] rigid0=null; foreach(Atom[] rigid in rigids) if(rigid.Contains(rotable.atoms[0])) { rigid0 = rigid; break; }
                Atom[] rigid1=null; foreach(Atom[] rigid in rigids) if(rigid.Contains(rotable.atoms[1])) { rigid1 = rigid; break; }
                HDebug.Assert(rigid0 != null, rigid1 != null);
                HDebug.Verify(flexGraph.AddEdge(rigid0, rigid1, rotable) != null);
            }
            HDebug.Assert(flexGraph.Nodes.Count == rigids.Count);
            HDebug.Assert(flexGraph.Edges.Count == rotables.Count);
            return flexGraph;
        }
        public List<Bond> FindRotableBonds()
        {
            HDebug.Depreciated("call FindRotableBonds(null,null), instead");
            IList<Bond> cutrotbonds = null;
            List<Atom[]> rigids  = FindRigidUnits(cutrotbonds);
            return FindRotableBonds(rigids, cutrotbonds);
        }
        public List<Bond> FindRotableBonds(List<Atom[]> rigids
                                          , IList<Bond> cutrotbonds
                                          )
        {
            if(cutrotbonds == null)
                cutrotbonds = new Bond[0];
            if(rigids == null)
                rigids = FindRigidUnits(cutrotbonds);

            Dictionary<Tuple<int,int>,Bond> ids_bond = new Dictionary<Tuple<int, int>, Bond>();
            HashSet<Bond> setBondCut = new HashSet<Bond>(cutrotbonds);
            foreach(Bond bond in bonds)
            {
                if(setBondCut.Contains(bond))
                    continue;
                int id0 = Math.Min(bond.atoms[0].ID, bond.atoms[1].ID);
                int id1 = Math.Max(bond.atoms[0].ID, bond.atoms[1].ID);
                HDebug.Assert(id0 < id1);
                ids_bond.Add(new Tuple<int,int>(id0,id1), bond);
            }

            StreamWriter rigidwriter = null;
            string writeRigid="false";
            if(writeRigid == "true")
                #region write into py file
            {
                rigidwriter = HFile.CreateText(@"C:\temp\rigidbonds.py");
                rigidwriter.WriteLine("from pymol.cgo import *");
                rigidwriter.WriteLine("from pymol import cmd");
                rigidwriter.WriteLine("obj = [");
                rigidwriter.WriteLine("BEGIN, LINES,");
            }
                #endregion

            foreach(Atom[] rigid in rigids)
            {
                for(int i=0; i<rigid.Length-1; i++)
                    for(int j=i+1; j<rigid.Length; j++)
                    {
                        int id0 = Math.Min(rigid[i].ID, rigid[j].ID);
                        int id1 = Math.Max(rigid[i].ID, rigid[j].ID);
                        Tuple<int,int> key = new Tuple<int, int>(id0, id1);
                        bool bondremoved = ids_bond.Remove(key);

                        if(bondremoved && (rigidwriter != null))
                            #region write into py file
                        {
                            rigidwriter.Write("ALPHA,    1.000, ");
                            rigidwriter.Write("COLOR,   1.000,   0.000,   0.000, ");
                            rigidwriter.Write("VERTEX,   {0:##.###}, {1:##.###}, {2:##.###}, ", rigid[i].Coord[0], rigid[i].Coord[1], rigid[i].Coord[2]);
                            rigidwriter.Write("VERTEX,   {0:##.###}, {1:##.###}, {2:##.###}, ", rigid[j].Coord[0], rigid[j].Coord[1], rigid[j].Coord[2]);
                            rigidwriter.WriteLine();
                        }
                            #endregion
                    }
            }

            if(rigidwriter != null)
                #region write into py file
            {
                rigidwriter.WriteLine("END");
                rigidwriter.WriteLine("]");
                rigidwriter.WriteLine("cmd.load_cgo(obj,\"rigid_bonds\")");
                rigidwriter.WriteLine("cmd.set(\"cgo_line_width\", 4, \"rigid_bonds\" )");
                rigidwriter.Close();
            }
                #endregion

            List<Bond> rotBonds = new List<Bond>(ids_bond.Values);

            string writeRotable="false";
            if(writeRotable == "true")
                #region write into py file
            {
                StreamWriter rotablewriter = HFile.CreateText(@"C:\temp\rotablebonds.py");
                rotablewriter.WriteLine("from pymol.cgo import *");
                rotablewriter.WriteLine("from pymol import cmd");
                rotablewriter.WriteLine("obj = [");
                rotablewriter.WriteLine("BEGIN, LINES,");
                foreach(Bond bond in rotBonds)
                {
                    Atom atom0 = bond.atoms[0];
                    Atom atom1 = bond.atoms[1];
                    rotablewriter.Write("ALPHA,    1.000, ");
                    rotablewriter.Write("COLOR,    0.000,   1.000,   0.000, ");
                    rotablewriter.Write("VERTEX,   {0:##.###}, {1:##.###}, {2:##.###}, ", atom0.Coord[0], atom0.Coord[1], atom0.Coord[2]);
                    rotablewriter.Write("VERTEX,   {0:##.###}, {1:##.###}, {2:##.###}, ", atom1.Coord[0], atom1.Coord[1], atom1.Coord[2]);
                    rotablewriter.WriteLine();
                }

                rotablewriter.WriteLine("END");
                rotablewriter.WriteLine("]");
                rotablewriter.WriteLine("cmd.load_cgo(obj,\"rotable_bonds\")");
                rotablewriter.WriteLine("cmd.set(\"cgo_line_width\", 4, \"rotable_bonds\" )");
                rotablewriter.Close();
            }
                #endregion

            return rotBonds;
        }
        public List<Tuple<Atom[],Bond[]>> FindRings()
        {
            HDebug.Depreciated("call FindRings(null), instead");
            IList<Bond> cutrotbonds = null;
            return FindRings(cutrotbonds);
        }
        public List<Tuple<Atom[],Bond[]>> FindRings(IList<Bond> cutrotbonds //=null
                                                   )
        {
            if(cutrotbonds == null)
                cutrotbonds = new Bond[0];

            Graph<int, Bond> graph = new Graph<int, Bond>();
            for(int i=0; i<bonds.Count; i++)
            {
                Universe.Bond bond = bonds[i];
                Universe.Atom atom0 = bond.atoms[0];
                Universe.Atom atom1 = bond.atoms[1];
                HDebug.Assert(atom0.Bonds.Count > 0);
                HDebug.Assert(atom1.Bonds.Count > 0);
                if((atom0.Bonds.Count != 1) && (atom1.Bonds.Count != 1))
                {
                    // atom0 and atom1 are not the leaf.
                    // if any of them are leaf, there is no ring having edge atom0-atom1.
                    graph.AddEdge(atom0.ID, atom1.ID, bond);
                }
            }
            List<List<Graph<int,Bond>.Node>> loops = graph.FindLoops(50);
            List<Tuple<Atom[],Bond[]>> rings = new List<Tuple<Atom[],Bond[]>>();
            HashSet<Bond> setBondCut = new HashSet<Bond>(cutrotbonds);
            foreach(List<Graph<int,Bond>.Node> loop in loops)
            {
                List<Atom> atomring = new List<Atom>(loop.Count);
                List<Bond> bondring = new List<Bond>(loop.Count-1);
                for(int i=0; i<loop.Count; i++)
                {
                    int atomid = graph.GetValue(loop[i]);
                    atomring.Add(atoms[atomid]);
                    if(i>=1)
                    {
                        int atomidPrv = graph.GetValue(loop[i-1]);
                        Bond bond = graph.FindEdge(atomidPrv, atomid, null);
                        HDebug.Assert(bond.atoms.Contains(atoms[atomidPrv])
                                     ,bond.atoms.Contains(atoms[atomid   ])
                                     );

                        if(setBondCut.Contains(bond))
                        {
                            /// do not add to rings
                            /// because this contains cutting-bond(s)
                            bondring = null;
                            break;
                        }
                        bondring.Add(bond);
                    }
                }
                if((atomring == null) || (bondring == null))
                    continue;
                rings.Add(new Tuple<Atom[],Bond[]>(atomring.ToArray(), bondring.ToArray()));
            }
            return rings;
        }
        public List<Atom[]> FindRigidUnits()
        {
            HDebug.Depreciated("call FindRigidUnits(null), instead");
            IList<Bond> cutrotbonds = null;
            return FindRigidUnits(cutrotbonds);
        }
        public List<Atom[]> FindRigidUnits(IList<Bond> cutrotbonds //=null
                                          )
        {
            /////////////////////////////////////////////////////////////////////////////////
            // list of rings
            List<Tuple<Atom[],Bond[]>> rings = FindRings(cutrotbonds);
            /////////////////////////////////////////////////////////////////////////////////
            // list of tip atoms
            //
            //  A-B-C-D-E-F    <= where A, B', D', F are tips
            //    |   |
            //    B'  D'
            List<Atom[]> tips = new List<Atom[]>();
            {
                for(int i=0; i<bonds.Count; i++)
                {
                    Universe.Bond bond = bonds[i];
                    Universe.Atom atom0 = bond.atoms[0];
                    Universe.Atom atom1 = bond.atoms[1];
                    HDebug.Assert(atom0.Bonds.Count > 0);
                    HDebug.Assert(atom1.Bonds.Count > 0);
                    if((atom0.Bonds.Count == 1) || (atom1.Bonds.Count == 1))
                    {
                        // atom0 or atom1 is the leaf
                        tips.Add(new Atom[] { atom0, atom1 });
                    }
                }
            }
            /////////////////////////////////////////////////////////////////////////////////
            // special treatment for the bond which connects two rigid units
            //                                            as one regid unit
            List<Atom[]> rigidbonds = new List<Atom[]>();
            {
                foreach(Bond bond in bonds)
                {
                    Atom atom0 = (string.Compare(bond.atoms[0].AtomName,bond.atoms[1].AtomName) <= 0) ? bond.atoms[0] : bond.atoms[1];
                    Atom atom1 = (string.Compare(bond.atoms[0].AtomName,bond.atoms[1].AtomName) <= 0) ? bond.atoms[1] : bond.atoms[0];

                    if((atom0.AtomName == "C") && (atom1.AtomName == "N")
                    && (atom0.ResidueId+1 == atom1.ResidueId))
                    {
                        ////////////////////////////////////////////////
                        // Backbone                                   //
                        //          H      O                          //
                        // N-CA-  C*N  -CA-C                          //
                        //        O                                   //
                        ////////////////////////////////////////////////
                        rigidbonds.Add(new Atom[] { atom0, atom1 });
                    }
                    else if((atom0.ResidueName == "ARG")
                         && (atom0.AtomName == "CZ") && (atom1.AtomName == "NH1")
                         && (atom0.ResidueId == atom1.ResidueId))
                    {
                        ////////////////////////////////////////////
                        // ARG                                    //
                        // !     |                      HH11      //
                        // !  HN-N                       |        //
                        // !     |   HB1 HG1 HD1 HE     NH1-HH12  //
                        // !     |   |   |   |   |    //(+)       //
                        // !  HA-CA--CB--CG--CD--NE--CZ           //
                        // !     |   |   |   |         \          //
                        // !     |   HB2 HG2 HD2        NH2-HH22  //
                        // !   O=C                       |        //
                        // !     |                      HH21      //
                        ////////////////////////////////////////////
                        rigidbonds.Add(new Atom[] { atom0, atom1 });
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////////////
            // (rings, tips, rigidbonds) are the list of atoms for bonded units                         //
            // build another graph, and determine rigid units.                                          //
            //                                                                                          //
            //                                                                                          //
            //                  M     O       <= rings      : B-C-D-F, H-F-G, O-L-P                     //
            //   A  B-C   H-J    \   / \         tips       : H-J, G-I, M-K, L-R, P-Q                   //
            //      | |  / \      K=L---P-Q      rigidbonds : E=F, K=L                                  //
            //      D-E=F---G-I     |                                                                   //
            //                      R         <= rigids     : (A), (B,C,D,E,F,G,H,I,J), (K,L,M,O,P,Q,R) //
            //////////////////////////////////////////////////////////////////////////////////////////////
            List<Atom[]> rigids = new List<Atom[]>();
            {
                Graph<int,Tuple<int,int>> graph = new Graph<int, Tuple<int,int>>();
                foreach(Atom atom in atoms)
                    graph.AddNode(atom.ID);
                foreach(Tuple<Atom[],Bond[]> ring in rings)
                {
                    Atom[] atomring = ring.Item1;
                    for(int i=1; i<atomring.Length; i++)
                    {
                        Tuple<int,int> edge = (new int[2] { atomring[i-1].ID, atomring[i].ID }).HSort().HToTuple2();
                        graph.AddEdge(edge.Item1, edge.Item2, edge);
                    }
                }
                foreach(Atom[] tip in tips)
                {
                    Tuple<int,int> edge = (new int[2]{tip[0].ID, tip[1].ID}).HSort().HToTuple2();
                    graph.AddEdge(edge.Item1, edge.Item2, edge);
                }
                foreach(Atom[] rigidbond in rigidbonds)
                {
                    for(int i=1; i<rigidbond.Length; i++)
                    {
                        Tuple<int,int> edge = (new int[2]{rigidbond[i-1].ID, rigidbond[i].ID}).HSort().HToTuple2();
                        graph.AddEdge(edge.Item1, edge.Item2, edge);
                    }
                }

                List<List<Graph<int,Tuple<int,int>>.Node>> connecteds = graph.FindConnectedNodes();
                List<List<int>> connecteds_atomid = graph.GetValue(connecteds);

                HashSet<int> check = new HashSet<int>();
                foreach(List<int> conn in connecteds_atomid)
                {
                    conn.Sort();
                    List<Atom> rigid = new List<Atom>(conn.Count);
                    foreach(int atomid in conn)
                    {
                        rigid.Add(atoms[atomid]);
                        check.Add(atomid);
                    }
                    rigids.Add(rigid.ToArray());
                }
                HDebug.Assert(check.Count == atoms.Count);
            }

            return rigids;
        }
	}
}
