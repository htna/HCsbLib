using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    partial class PdbDepreciated
	{
        public partial class Struct
        {
            string[] BuildNames()
            {
                string[] names = new string[] { "GLY", "ALA", "PRO", "VAL", "CYS",
                                                "SER", "THR", "ILE", "LEU", "ASP",
                                                "ASN", "HIS", "PHE", "TYR", "TRP",
                                                "MET", "GLU", "GLN", "LYS", "ARG",
                                              };
                return names;
            }
            public static List<Tuple<string, string>> BuildBonds(params string[] atomss)
            {
                List<Tuple<string,string>> bonds = new List<Tuple<string,string>>();
                foreach(string atoms in atomss)
                {
                    string[] tokens = atoms.Split('-');
                    for(int i=1; i<tokens.Length; i++)
                    {
                        List<string> bond = new List<string>();
                        bond.Add(tokens[i-1]);
                        bond.Add(tokens[i  ]);
                        bond.Sort();
                        bonds.Add(new Tuple<string, string>(bond[0], bond[1]));
                    }
                }
                return bonds;
            }
            public static Dictionary<Pdb.Atom, HashSet<Pdb.Atom>> BuildBonds(params Pdb.Atom[] atoms)
            {
                int    resSeq  = atoms[0].resSeq;
                string resName = atoms[0].resName;

                List<Tuple<string, string>> list_name1_name2 = BuildBonds(resName);

                Dictionary<Pdb.Atom,HashSet<Pdb.Atom>> bonds = new Dictionary<Pdb.Atom, HashSet<Pdb.Atom>>();

                foreach(Tuple<string, string> name1_name2 in list_name1_name2)
                {
                    string name1 = name1_name2.Item1;
                    string name2 = name1_name2.Item2;
                    List<Pdb.Atom> atom1s = atoms.SelectByName(name1.Trim());
                    List<Pdb.Atom> atom2s = atoms.SelectByName(name2.Trim());
                    if(atom1s.Count == 0 || atom2s.Count == 0)
                        continue;
                    HDebug.Assert(atom1s.Count == 1, atom2s.Count == 1);
                    Pdb.Atom atom1 = atom1s[0];
                    Pdb.Atom atom2 = atom2s[0];
                    HDebug.Assert(atom1.resSeq == atom2.resSeq);
                    HDebug.Assert(atom1.resName == atom2.resName);
                    if(bonds.ContainsKey(atom1) == false) bonds.Add(atom1, new HashSet<Pdb.Atom>());
                    if(bonds.ContainsKey(atom2) == false) bonds.Add(atom2, new HashSet<Pdb.Atom>());
                    bonds[atom1].Add(atom2);
                    bonds[atom2].Add(atom1);
                }

                HDebug.Assert(bonds.Count == atoms.Length);
                return bonds;
            }
            public static List<Tuple<string, string>> BuildBonds(string name)
            {
                switch(name)
                {
                    case "GLY": return BuildBonds( "N  -CA -C  -O  "                        //     |     
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"   //     N-H   
                                                 , "CA-HA1", "CA-HA2"//, "CA-HA3"           //     |     
                                                 );                                         //     |     
                                                                                            // HA1-CA-HA2
                                                                                            //     |     
                                                                                            //     |     
                                                                                            //     C=O   
                                                                                            //     |     

                    case "ALA": return BuildBonds( "N  -CA -C  -O  ", "CA -CB "             //    |           
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"   // HN-N           
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"             //    |     HB1   
                                                 );                                         //    |    /      
                                                                                            // HA-CA--CB-HB2  
                                                                                            //    |    \      
                                                                                            //    |     HB3   
                                                                                            //  O=C           
                                                                                            //    |           

                    case "PRO": return BuildBonds( "N  -CA -C  -O  ","CA -CB -CG -CD ",     //       HD1 HD2   
                                                   "N  -CD "                                //     |   \ /     
                                                 , "C-OXT"                                  //     N---CD   HG1
                                                 , "CA-HA"                                  //     |     \  /  
                                                 , "CB-HB1", "CB-HB2"//, "CB-HB3"           //     |      CG   
                                                 , "CG-HG1", "CG-HG2"//, "CG-HG3"           //     |     /  \  
                                                 , "CD-HD1", "CD-HD2"//, "CD-HD3"           //  HA-CA--CB   HG2
                                                 );                                         //     |   / \     
                                                                                            //     | HB1 HB2   
                                                                                            //   O=C           
                                                                                            //     |           

                    case "VAL": return BuildBonds( "N  -CA -C  -O  ","CA -CB -CG1",         //     |    HG11 HG12 
                                                                        "CB -CG2"           //  HN-N      | /     
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"   //     |     CG1--HG13
                                                 , "CB-HB"                                  //     |    /         
                                                 , "CG1-HG11", "CG1-HG12", "CG1-HG12"       //  HA-CA--CB-HB      
                                                 , "CG2-HG21", "CG2-HG22", "CG2-HG23"       //     |    \         
                                                 );                                         //     |     CG2--HG21
                                                                                            //   O=C    / \       
                                                                                            //     | HG21 HG22    

                    case "CYS": return BuildBonds("N  -CA -C  -O  ","CA -CB -SG "
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 );
                    case "SER": return BuildBonds("N  -CA -C  -O  ","CA -CB -OG "
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 , "OG-HG"
                                                 );
                    case "THR": return BuildBonds("N  -CA -C  -O  ","CA -CB -OG1",
                                                                        "CB -CG2"
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB"
                                                 , "OG1-HG1"
                                                 , "CG2-HG21", "CG2-HG22", "CG2-HG23"
                                                 );
                    case "ILE": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG2",
                                                                        "CB -CG1-CD1");
                    case "LEU": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD1",
                                                                            "CG -CD2");
                    case "ASP": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -OD1",
                                                                            "CG -OD2"
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 );
                    case "ASN": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -OD1",
                                                                            "CG -ND2"
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 , "ND2-HD21", "ND2-HD22"
                                                 );
                    case "HIS": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD2-NE2",
                                                                            "CG -ND1-CE1-NE2"
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 , "ND1-HD1", "CD2-HD2"
                                                 , "CE1-HE1"
                                                 );
                    case "PHE": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD1-CE1-CZ ",
                                                                            "CG -CD2-CE2-CZ ");
                    case "TYR": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD1-CE1-CZ -OH ",
                                                                            "CG -CD2-CE2-CZ ");
                    case "TRP": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD2-CE3-CZ3-CH2",
                                                                                "CD2-CE2-CZ2-CH2",
                                                                            "CG -CD1-NE1-CE2"     );
                    case "MET": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -SD -CE ");
                    case "GLU": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD -OE1",
                                                                                "CD -OE2"
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 , "CG-HG1", "CG-HG2", "CG-HG3"
                                                 );
                    case "GLN": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD -OE1",
                                                                                "CD -NE2"
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 , "CG-HG1", "CG-HG2", "CG-HG3"
                                                 , "NE2-HE21", "NE2-HE22"
                                                 );
                    case "LYS": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD -CE -NZ "
                                                 , "N-H", "N-H1", "N-H2", "N-H3", "C-OXT"
                                                 , "CA-HA"
                                                 , "CB-HB1", "CB-HB2", "CB-HB3"
                                                 , "CG-HG1", "CG-HG2", "CG-HG3"
                                                 , "CD-HD1", "CD-HD2", "CD-HD3"
                                                 , "CE-HE1", "CE-HE2", "CE-HE3"
                                                 , "NZ-HZ1", "NZ-HZ2", "NZ-HZ3"
                                                 );
                    case "ARG": return BuildBonds("N  -CA -C  -O  ","CA -CB -CG -CD -NE -CZ -NH1",
                                                                                        "CZ -NH2");
                }
                HDebug.Assert(false);
                return null;
            }
        }
    }
}
