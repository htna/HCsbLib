using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Atom = Pdb.Atom;
    using Conect = Pdb.Conect;
    static partial class PdbCollection
    {
        public static Tuple<string, Pdb.Atom, Pdb.Atom>[] GetBonds(this Pdb pdb)
        {
            return PdbDepreciated.Struct.GetBonds(pdb);
        }
    }
    partial class PdbDepreciated
	{
        public partial class Struct
        {
            public static Tuple<string, Atom, Atom>[] GetBonds(Pdb pdb)
            {
                return GetBonds(pdb.atoms, pdb.conects);
            }
            public static Tuple<string, Atom, Atom>[] GetBonds(IList<Atom> atoms, IList<Conect> conects)
            {
                List<Tuple<string, Atom, Atom>> bonds = new List<Tuple<string, Atom, Atom>>();

                char[] chainIDs = atoms.ListChainID().HUnion().ToArray();
                foreach(char chainID in chainIDs)
                {
                    var chainbonds = GetBondsInChain(atoms, chainID);
                    bonds.AddRange(chainbonds);
                }

                Dictionary<int,Atom> ser2atom = atoms.ToDictionaryBySerial();
                IList<Tuple<int, int>> lstSerBond = conects.HListConectBonds();
                foreach(var serbond in lstSerBond)
                {
                    if(ser2atom.ContainsKey(serbond.Item1) == false) continue;
                    if(ser2atom.ContainsKey(serbond.Item2) == false) continue;
                    Atom atom1 = ser2atom[serbond.Item1];
                    Atom atom2 = ser2atom[serbond.Item2];
                    bonds.Add(new Tuple<string, Atom, Atom>("conect", atom1, atom2));
                }

                return bonds.ToArray();
            }
            public static Tuple<string, Atom, Atom>[] GetBondsInChain(IList<Atom> atoms, char chainID)
            {
                IList<Atom> latoms = atoms.SelectByChainID(chainID);
                IList<int> lstResSeq = latoms.ListResSeq().HToHashSet().ToList().HSort();
                List<Tuple<string, Atom, Atom>> bonds = new List<Tuple<string, Atom, Atom>>();
                for(int i=0; i<lstResSeq.Count; i++)
                {
                    IList<Tuple<string, Atom, Atom>> lbonds;
                    if(i != 0)
                    {
                        int resid1 = lstResSeq[i-1];
                        int resid2 = lstResSeq[i  ];
                        lbonds = GetBondsBwRes(latoms, resid1, resid2);
                        bonds.AddRange(lbonds);
                    }
                    int resid = lstResSeq[i];
                    lbonds = GetBondsInRes(latoms, resid);
                    bonds.AddRange(lbonds);
                }

                if(HDebug.IsDebuggerAttached)
                {
                    Graph<Atom,Tuple<string, Atom, Atom>> graph = new Graph<Atom, Tuple<string, Atom, Atom>>();
                    foreach(var atom in latoms)
                        graph.AddNode(atom);
                    foreach(var bond in bonds)
                        graph.AddEdge(bond.Item2, bond.Item3, bond);
                    List<List<Graph<Atom,Tuple<string, Atom, Atom>>.Node>> nodess = graph.FindConnectedNodes();
                    HDebug.Assert(nodess.Count == 1);
                }

                return bonds.ToArray();
            }
            public static Tuple<string, Atom, Atom>[] GetBondsBwRes(IList<Atom> atoms, int resseq1, int resseq2)
            {
                HDebug.Assert(resseq1+1 == resseq2);
                List<Atom> latoms1 = atoms.SelectByResSeq(resseq1);
                List<Atom> latoms2 = atoms.SelectByResSeq(resseq2);
                List<Tuple<string, Atom, Atom>> bonds = new List<Tuple<string, Atom, Atom>>();
                for(int i1=0; i1<latoms1.Count; i1++)
                    for(int i2=0; i2<latoms2.Count; i2++)
                    {
                        Atom atom1 = latoms1[i1];
                        Atom atom2 = latoms2[i2];
                        var bond = GetBond(atom1, atom2);
                        if(bond != null)
                        {
                            bonds.Add(bond);
                        }
                    }
                HDebug.Assert(bonds.Count == 1);
                return bonds.ToArray();
            }
            public static Tuple<string, Atom, Atom>[] GetBondsInRes(IList<Atom> atoms, int resseq)
            {
                List<Atom> latoms = atoms.SelectByResSeq(resseq);
                string resName = latoms[0].resName;
                List<Tuple<string, Atom, Atom>> bonds = new List<Tuple<string, Atom, Atom>>();
                for(int i=0; i<latoms.Count-1; i++)
                    for(int j=i+1; j<latoms.Count; j++)
                    {
                        Atom atom1 = latoms[i];
                        Atom atom2 = latoms[j];
                        var bond = GetBond(atom1, atom2);
                        if(bond != null)
                        {
                            bonds.Add(bond);
                        }
                    }

                if(HDebug.IsDebuggerAttached)
                {
                    // Check the number of bonds
                    switch(resName)
                    {
                        case "PHE": HDebug.Assert(latoms.Count   == bonds.Count); break; // 1 ring structure : +1
                        case "PRO": HDebug.Assert(latoms.Count   == bonds.Count); break; // 1 ring structure : +1
                        case "HIS": HDebug.Assert(latoms.Count   == bonds.Count); break; // 1 ring structure : +1
                        case "TYR": HDebug.Assert(latoms.Count   == bonds.Count); break; // 1 ring structure : +1
                        case "TRP": HDebug.Assert(latoms.Count+1 == bonds.Count); break; // 2 ring structure : +2
                        default   : HDebug.Assert(latoms.Count-1 == bonds.Count); break;
                    }

                    // check if all atoms are connected
                    {
                        Graph<Atom,Tuple<string, Atom, Atom>> graph = new Graph<Atom, Tuple<string, Atom, Atom>>();
                        foreach(var latom in latoms)
                            graph.AddNode(latom);
                        foreach(var bond in bonds)
                            graph.AddEdge(bond.Item2, bond.Item3, bond);
                        List<List<Graph<Atom,Tuple<string, Atom, Atom>>.Node>> nodess = graph.FindConnectedNodes();
                        HDebug.Assert(nodess.Count == 1);
                    }
                }
                return bonds.ToArray();
            }
            public static Tuple<string,Atom,Atom> GetBond(Atom atom1, Atom atom2)
            {
                var key_atom12 = GetKey(atom1, atom2);
                if(key_atom12 == null)
                    return null;
                string key = key_atom12.Item1;
                atom1 = key_atom12.Item2;
                atom2 = key_atom12.Item3;

                if(atom1.resSeq != atom2.resSeq)
                {
                    if((key == "C   -N   ") && (atom1.resSeq+1 == atom2.resSeq))
                        return new Tuple<string, Atom, Atom>("backbone-nrt-C-+N", atom1, atom2);
                    return null;
                }

                switch(key)
                {
                    case "N   -CA  ": return new Tuple<string, Atom, Atom>("backbone-rot-N-CA"    , atom1, atom2);
                    case "CA  -C   ": return new Tuple<string, Atom, Atom>("backbone-rot-CA-C"    , atom1, atom2);
                    case "C   -O   ": return new Tuple<string, Atom, Atom>("backbone-nrt-C-O"     , atom1, atom2);
                    case "C   -OXT ": return new Tuple<string, Atom, Atom>("backbone-nrt-C-OXT"   , atom1, atom2); // C-terminal
                    case "N   -H   ": return new Tuple<string, Atom, Atom>("backbone-nrt-hydrogen", atom1, atom2);
                    case "CA  -HA  ": return new Tuple<string, Atom, Atom>("backbone-nrt-hydrogen", atom1, atom2);
                    case "CA  -HA1 ": return new Tuple<string, Atom, Atom>("backbone-nrt-hydrogen", atom1, atom2);
                    case "C   -N   ": return null;
                }

                string resi_key = atom1.resName.ToUpper()+"-"+key;
                Tuple<string, Atom, Atom> key_atom12_rot = new Tuple<string, Atom, Atom>("sidechain-rot-"+key.Replace(" ", ""), atom1, atom2);
                Tuple<string, Atom, Atom> key_atom12_nrt = new Tuple<string, Atom, Atom>("sidechain-nrt-"+key.Replace(" ", ""), atom1, atom2);
                switch(resi_key)
                {
                    case "GLY-CA  -HA2 ": return key_atom12_nrt; ///   H-N       
                                                                 /// HA1-CA-HA2  
                                                                 ///   O=C       

                    ////////////////////////////////////////////////////////////////////////////
                    ///  Hydrophobic

                    case "ALA-CA  -CB  ": return key_atom12_rot; /// HN-N     HB1      
                    case "ALA-CB  -HB1 ": return key_atom12_nrt; ///    |    /         
                    case "ALA-CB  -HB2 ": return key_atom12_nrt; /// HA-CA--CB-HB2     
                    case "ALA-CB  -HB3 ": return key_atom12_nrt; ///    |    \         
                                                                 ///  O=C     HB3      

                    case "VAL-CA  -CB  ": return key_atom12_rot;
                    case "VAL-CB  -HB  ": return key_atom12_nrt; ///    |    HG11 HG12  
                    case "VAL-CB  -CG1 ": return key_atom12_rot; /// HN-N      | /      
                    case "VAL-CB  -CG2 ": return key_atom12_rot; ///    |     CG1--HG13 
                    case "VAL-CG1 -HG11": return key_atom12_nrt; ///    |    /          
                    case "VAL-CG1 -HG12": return key_atom12_nrt; /// HA-CA--CB-HB       
                    case "VAL-CG1 -HG13": return key_atom12_nrt; ///    |    \          
                    case "VAL-CG2 -HG21": return key_atom12_nrt; ///    |     CG2--HG21 
                    case "VAL-CG2 -HG22": return key_atom12_nrt; ///  O=C    / \        
                    case "VAL-CG2 -HG23": return key_atom12_nrt; ///    | HG21 HG22     

                    case "PHE-CA  -CB  ": return key_atom12_rot;
                    case "PHE-CB  -CG  ": return key_atom12_rot; ///    |        HD1  HE1            
                    case "PHE-CG  -CD1 ": return key_atom12_nrt; /// HN-N         |    |             
                    case "PHE-CG  -CD2 ": return key_atom12_nrt; ///    |   HB1  CD1--CE1            
                    case "PHE-CD1 -CE1 ": return key_atom12_nrt; ///    |   |    //     \\           
                    case "PHE-CD2 -CE2 ": return key_atom12_nrt; /// HA-CA--CB--CG      CZ--HZ       
                    case "PHE-CE1 -CZ  ": return key_atom12_nrt; ///    |   |    \  __  /            
                    case "PHE-CE2 -CZ  ": return key_atom12_nrt; ///    |   HB2  CD2--CE2            
                    case "PHE-CB  -HB1 ": return key_atom12_nrt; ///  O=C         |    |             
                    case "PHE-CB  -HB2 ": return key_atom12_nrt; ///    |        HD2  HE2            
                    case "PHE-CD1 -HD1 ": return key_atom12_nrt;
                    case "PHE-CD2 -HD2 ": return key_atom12_nrt;
                    case "PHE-CE1 -HE1 ": return key_atom12_nrt;
                    case "PHE-CE2 -HE2 ": return key_atom12_nrt;
                    case "PHE-CZ  -HZ  ": return key_atom12_nrt;

                    case "PRO-CA  -CB  ": return key_atom12_nrt; ///      HD1 HD2                  
                    case "PRO-CB  -CG  ": return key_atom12_nrt; ///    |   \ /                    
                    case "PRO-CG  -CD  ": return key_atom12_nrt; ///    N---CD   HG1               
                    case "PRO-N   -CD  ": return key_atom12_nrt; ///    |     \  /                 
                    case "PRO-CB  -HB1 ": return key_atom12_nrt; ///    |      CG                  
                    case "PRO-CB  -HB2 ": return key_atom12_nrt; ///    |     /  \                 
                    case "PRO-CG  -HG1 ": return key_atom12_nrt; /// HA-CA--CB   HG2               
                    case "PRO-CG  -HG2 ": return key_atom12_nrt; ///    |   / \                    
                    case "PRO-CD  -HD1 ": return key_atom12_nrt; ///    | HB1 HB2                  
                    case "PRO-CD  -HD2 ": return key_atom12_nrt; ///  O=C                          

                    case "LEU-CA  -CB  ": return key_atom12_rot;
                    case "LEU-CB  -CG  ": return key_atom12_rot; ///    |        HD11 HD12  
                    case "LEU-CG  -CD1 ": return key_atom12_rot; /// HN-N          | /      
                    case "LEU-CG  -CD2 ": return key_atom12_rot; ///    |   HB1   CD1--HD13 
                    case "LEU-CB  -HB1 ": return key_atom12_nrt; ///    |   |    /          
                    case "LEU-CB  -HB2 ": return key_atom12_nrt; /// HA-CA--CB--CG-HG       
                    case "LEU-CG  -HG  ": return key_atom12_nrt; ///    |   |    \          
                    case "LEU-CD1 -HD11": return key_atom12_nrt; ///    |   HB2   CD2--HD23 
                    case "LEU-CD1 -HD12": return key_atom12_nrt; ///  O=C          | \      
                    case "LEU-CD1 -HD13": return key_atom12_nrt; ///    |        HD21 HD22  
                    case "LEU-CD2 -HD21": return key_atom12_nrt;
                    case "LEU-CD2 -HD22": return key_atom12_nrt;
                    case "LEU-CD2 -HD23": return key_atom12_nrt;

                    case "ILE-CA  -CB  ": return key_atom12_rot;
                    case "ILE-CB  -CG1 ": return key_atom12_rot; ///    |    HG21 HG22          
                    case "ILE-CB  -CG2 ": return key_atom12_rot; /// HN-N      | /              
                    case "ILE-CG1 -CD  ": return key_atom12_rot; ///    |     CG2--HG23         
                    case "ILE-CB  -HB  ": return key_atom12_nrt; ///    |    /                  
                    case "ILE-CG1 -HG11": return key_atom12_nrt; /// HA-CA--CB-HB    HD1        
                    case "ILE-CG1 -HG12": return key_atom12_nrt; ///    |    \       /          
                    case "ILE-CG2 -HG21": return key_atom12_nrt; ///    |     CG1--CD--HD2      
                    case "ILE-CG2 -HG22": return key_atom12_nrt; ///  O=C    / \     \	        
                    case "ILE-CG2 -HG23": return key_atom12_nrt; ///    | HG11 HG12  HD3        
                    case "ILE-CD  -HD1 ": return key_atom12_nrt;
                    case "ILE-CD  -HD2 ": return key_atom12_nrt;
                    case "ILE-CD  -HD3 ": return key_atom12_nrt;
                    case "ILE-CG1 -CD1 ": return key_atom12_rot; // CD -> CD1

                    ////////////////////////////////////////////////////////////////////////////
                    ///  Hydrophilic

                    case "ARG-CA  -CB  ": return key_atom12_rot;
                    case "ARG-CB  -CG  ": return key_atom12_rot; ///    |                      HH11            
                    case "ARG-CG  -CD  ": return key_atom12_rot; /// HN-N                       |              
                    case "ARG-CD  -NE  ": return key_atom12_rot; ///    |   HB1 HG1 HD1 HE     NH1-HH12        
                    case "ARG-NE  -CZ  ": return key_atom12_rot; ///    |   |   |   |   |    //(+)             
                    case "ARG-CZ  -NH1 ": return key_atom12_nrt; /// HA-CA--CB--CG--CD--NE--CZ                 
                    case "ARG-CZ  -NH2 ": return key_atom12_rot; ///    |   |   |   |         \                
                    case "ARG-CB  -HB1 ": return key_atom12_nrt; ///    |   HB2 HG2 HD2        NH2-HH22        
                    case "ARG-CB  -HB2 ": return key_atom12_nrt; ///  O=C                       |              
                    case "ARG-CG  -HG1 ": return key_atom12_nrt; ///    |                      HH21            
                    case "ARG-CG  -HG2 ": return key_atom12_nrt;
                    case "ARG-CD  -HD1 ": return key_atom12_nrt;
                    case "ARG-CD  -HD2 ": return key_atom12_nrt;
                    case "ARG-NE  -HE  ": return key_atom12_nrt;
                    case "ARG-NH1 -HH11": return key_atom12_nrt;
                    case "ARG-NH1 -HH12": return key_atom12_nrt;
                    case "ARG-NH2 -HH21": return key_atom12_nrt;
                    case "ARG-NH2 -HH22": return key_atom12_nrt;

                    case "ASP-CA  -CB  ": return key_atom12_rot;
                    case "ASP-CB  -CG  ": return key_atom12_rot; /// HN-N   HB1   OD1    
                    case "ASP-CG  -OD1 ": return key_atom12_nrt; ///    |   |    //      
                    case "ASP-CG  -OD2 ": return key_atom12_nrt; /// HA-CA--CB--CG       
                    case "ASP-CB  -HB1 ": return key_atom12_nrt; ///    |   |    \       
                    case "ASP-CB  -HB2 ": return key_atom12_nrt; ///  O=C   HB2   OD2(-) 

                    case "GLU-CA  -CB  ": return key_atom12_rot; ///    |                          
                    case "GLU-CB  -CG  ": return key_atom12_rot; /// HN-N                          
                    case "GLU-CG  -CD  ": return key_atom12_rot; ///    |   HB1 HG1   OE1          
                    case "GLU-CD  -OE1 ": return key_atom12_nrt; ///    |   |   |    //            
                    case "GLU-CD  -OE2 ": return key_atom12_nrt; /// HA-CA--CB--CG--CD             
                    case "GLU-CB  -HB1 ": return key_atom12_nrt; ///    |   |   |    \             
                    case "GLU-CB  -HB2 ": return key_atom12_nrt; ///    |   HB2 HG2   OE2(-)       
                    case "GLU-CG  -HG1 ": return key_atom12_nrt; ///  O=C                          
                    case "GLU-CG  -HG2 ": return key_atom12_nrt; ///    |                          

                    case "SER-CA  -CB  ": return key_atom12_rot; /// HN-N   HB1          
                    case "SER-CB  -OG  ": return key_atom12_rot; ///    |   |            
                    case "SER-CB  -HB1 ": return key_atom12_nrt; /// HA-CA--CB--OG       
                    case "SER-CB  -HB2 ": return key_atom12_nrt; ///    |   |     \      
                    case "SER-OG  -HG1 ": return key_atom12_nrt; ///  O=C   HB2    HG1   

                    case "CYS-CA  -CB  ": return key_atom12_rot; /// HN-N   HB1                         
                    case "CYS-CB  -SG  ": return key_atom12_rot; ///    |   |              
                    case "CYS-CB  -HB1 ": return key_atom12_nrt; /// HA-CA--CB--SG         
                    case "CYS-CB  -HB2 ": return key_atom12_nrt; ///    |   |     \        
                    case "CYS-SG  -HG1 ": return key_atom12_nrt; ///  O=C   HB2    HG1                  

                    case "ASN-CA  -CB  ": return key_atom12_rot;
                    case "ASN-CB  -CG  ": return key_atom12_rot; /// HN-N                                      
                    case "ASN-CG  -OD1 ": return key_atom12_nrt; ///    |   HB1 OD1    HD21 (cis to OD1)       
                    case "ASN-CG  -ND2 ": return key_atom12_rot; ///    |   |   ||    /                        
                    case "ASN-CB  -HB1 ": return key_atom12_nrt; /// HA-CA--CB--CG--ND2                        
                    case "ASN-CB  -HB2 ": return key_atom12_nrt; ///    |   |         \                        
                    case "ASN-ND2 -HD21": return key_atom12_nrt; ///    |   HB2        HD22 (trans to OD1)     
                    case "ASN-ND2 -HD22": return key_atom12_nrt; ///  O=C                                      

                    case "GLN-CA  -CB  ": return key_atom12_rot;
                    case "GLN-CB  -CG  ": return key_atom12_rot; ///    |                                         
                    case "GLN-CG  -CD  ": return key_atom12_rot; /// HN-N                                         
                    case "GLN-CD  -OE1 ": return key_atom12_nrt; ///    |   HB1 HG1 OE1   HE21 (cis to OE1)       
                    case "GLN-CD  -NE2 ": return key_atom12_rot; ///    |   |   |   ||    /                       
                    case "GLN-CB  -HB1 ": return key_atom12_nrt; /// HA-CA--CB--CG--CD--NE2                       
                    case "GLN-CB  -HB2 ": return key_atom12_nrt; ///    |   |   |         \                       
                    case "GLN-CG  -HG1 ": return key_atom12_nrt; ///    |   HB2 HG2       HE22 (trans to OE1)     
                    case "GLN-CG  -HG2 ": return key_atom12_nrt; ///  O=C                                         
                    case "GLN-NE2 -HE21": return key_atom12_nrt; ///    |                                         
                    case "GLN-NE2 -HE22": return key_atom12_nrt;

                    case "HIS-CA  -CB  ": return key_atom12_rot; /// RESI HSD
                    case "HIS-CB  -CG  ": return key_atom12_rot; ///    |          HD1    HE1         
                    case "HIS-CG  -ND1 ": return key_atom12_nrt; /// HN-N           |     /           
                    case "HIS-CG  -CD2 ": return key_atom12_nrt; ///    |   HB1    ND1--CE1           
                    case "HIS-ND1 -CE1 ": return key_atom12_nrt; ///    |   |     /      ||           
                    case "HIS-CD2 -NE2 ": return key_atom12_nrt; /// HA-CA--CB--CG       ||           
                    case "HIS-CE1 -HE1 ": return key_atom12_nrt; ///    |   |     \\     ||           
                    case "HIS-CE1 -NE2 ": return key_atom12_nrt; ///    |   HB2    CD2--NE2           
                    case "HIS-CB  -HB1 ": return key_atom12_nrt; ///  O=C           |                 
                    case "HIS-CB  -HB2 ": return key_atom12_nrt; ///    |          HD2                
                    case "HIS-ND1 -HD1 ": return key_atom12_nrt;
                    case "HIS-CD2 -HD2 ": return key_atom12_nrt;

                    ////////////////////////////////////////////////////////////////////////////
                    ///  Amphipathic

                    case "THR-CA  -CB  ": return key_atom12_rot; /// HN-N                          
                    case "THR-CB  -OG1 ": return key_atom12_rot; ///    |     OG1--HG1             
                    case "THR-CB  -CG2 ": return key_atom12_rot; ///    |    /                     
                    case "THR-CB  -HB  ": return key_atom12_nrt; /// HA-CA--CB-HB                  
                    case "THR-OG1 -HG1 ": return key_atom12_nrt; ///    |    \                     
                    case "THR-CG2 -HG21": return key_atom12_nrt; ///    |     CG2--HG21            
                    case "THR-CG2 -HG22": return key_atom12_nrt; ///  O=C    / \                   
                    case "THR-CG2 -HG23": return key_atom12_nrt; ///    | HG21 HG22                

                    case "LYS-CA  -CB  ": return key_atom12_rot;
                    case "LYS-CB  -CG  ": return key_atom12_rot; ///    |                              
                    case "LYS-CG  -CD  ": return key_atom12_rot; /// HN-N                              
                    case "LYS-CD  -CE  ": return key_atom12_rot; ///    |   HB1 HG1 HD1 HE1    HZ1     
                    case "LYS-CE  -NZ  ": return key_atom12_rot; ///    |   |   |   |   |     /        
                    case "LYS-CB  -HB1 ": return key_atom12_nrt; /// HA-CA--CB--CG--CD--CE--NZ--HZ2    
                    case "LYS-CB  -HB2 ": return key_atom12_nrt; ///    |   |   |   |   |     \        
                    case "LYS-CG  -HG1 ": return key_atom12_nrt; ///    |   HB2 HG2 HD2 HE2    HZ3     
                    case "LYS-CG  -HG2 ": return key_atom12_nrt; ///  O=C                              
                    case "LYS-CD  -HD1 ": return key_atom12_nrt; ///    |                              
                    case "LYS-CD  -HD2 ": return key_atom12_nrt;
                    case "LYS-CE  -HE1 ": return key_atom12_nrt;
                    case "LYS-CE  -HE2 ": return key_atom12_nrt;
                    case "LYS-NZ  -HZ1 ": return key_atom12_nrt;
                    case "LYS-NZ  -HZ2 ": return key_atom12_nrt;
                    case "LYS-NZ  -HZ3 ": return key_atom12_nrt;

                    case "TYR-CA  -CB  ": return key_atom12_rot;
                    case "TYR-CB  -CG  ": return key_atom12_rot; ///    |        HD1  HE1                   
                    case "TYR-CG  -CD1 ": return key_atom12_nrt; /// HN-N         |    |                    
                    case "TYR-CG  -CD2 ": return key_atom12_nrt; ///    |   HB1  CD1--CE1                   
                    case "TYR-CD1 -CE1 ": return key_atom12_nrt; ///    |   |   //      \\                  
                    case "TYR-CD2 -CE2 ": return key_atom12_nrt; /// HA-CA--CB--CG      CZ--OH              
                    case "TYR-CE1 -CZ  ": return key_atom12_nrt; ///    |   |    \  __  /     \             
                    case "TYR-CE2 -CZ  ": return key_atom12_nrt; ///    |   HB2  CD2--CE2     HH            
                    case "TYR-CZ  -OH  ": return key_atom12_rot; ///  O=C         |    |                    
                    case "TYR-CB  -HB1 ": return key_atom12_nrt; ///    |        HD2  HE2                   
                    case "TYR-CB  -HB2 ": return key_atom12_nrt;
                    case "TYR-CD1 -HD1 ": return key_atom12_nrt;
                    case "TYR-CD2 -HD2 ": return key_atom12_nrt;
                    case "TYR-CE1 -HE1 ": return key_atom12_nrt;
                    case "TYR-CE2 -HE2 ": return key_atom12_nrt;
                    case "TYR-OH  -HH  ": return key_atom12_nrt;

                    case "MET-CA  -CB  ": return key_atom12_rot;
                    case "MET-CB  -CG  ": return key_atom12_rot; ///    |                             
                    case "MET-CG  -SD  ": return key_atom12_rot; /// HN-N                             
                    case "MET-SD  -CE  ": return key_atom12_rot; ///    |   HB1 HG1     HE1           
                    case "MET-CB  -HB1 ": return key_atom12_nrt; ///    |   |   |       |             
                    case "MET-CB  -HB2 ": return key_atom12_nrt; /// HA-CA--CB--CG--SD--CE--HE3       
                    case "MET-CG  -HG1 ": return key_atom12_nrt; ///    |   |   |       |             
                    case "MET-CG  -HG2 ": return key_atom12_nrt; ///    |   HB2 HG2     HE2           
                    case "MET-CE  -HE1 ": return key_atom12_nrt; ///  O=C                             
                    case "MET-CE  -HE2 ": return key_atom12_nrt; ///    |                             
                    case "MET-CE  -HE3 ": return key_atom12_nrt;

                    case "TRP-CA  -CB  ": return key_atom12_rot;
                    case "TRP-CB  -CG  ": return key_atom12_rot; ///    |                  HE3             
                    case "TRP-CG  -CD1 ": return key_atom12_nrt; /// HN-N                   |              
                    case "TRP-CG  -CD2 ": return key_atom12_nrt; ///    |   HB1            CE3             
                    case "TRP-CD1 -NE1 ": return key_atom12_nrt; ///    |   |             /  \\            
                    case "TRP-CD2 -CE2 ": return key_atom12_nrt; /// HA-CA--CB---CG-----CD2   CZ3-HZ3      
                    case "TRP-CD2 -CE3 ": return key_atom12_nrt; ///    |   |    ||     ||     |           
                    case "TRP-NE1 -CE2 ": return key_atom12_nrt; ///    |   HB2  CD1    CE2   CH2-HH2      
                    case "TRP-CE2 -CZ2 ": return key_atom12_nrt; ///  O=C       /   \   / \  //            
                    case "TRP-CE3 -CZ3 ": return key_atom12_nrt; ///    |     HD1    NE1   CZ2             
                    case "TRP-CZ2 -CH2 ": return key_atom12_nrt; ///                  |     |              
                    case "TRP-CZ3 -CH2 ": return key_atom12_nrt; ///                 HE1   HZ2             
                    case "TRP-CB  -HB1 ": return key_atom12_nrt;
                    case "TRP-CB  -HB2 ": return key_atom12_nrt;
                    case "TRP-CD1 -HD1 ": return key_atom12_nrt;
                    case "TRP-NE1 -HE1 ": return key_atom12_nrt;
                    case "TRP-CE3 -HE3 ": return key_atom12_nrt;
                    case "TRP-CZ2 -HZ2 ": return key_atom12_nrt;
                    case "TRP-CZ3 -HZ3 ": return key_atom12_nrt;
                    case "TRP-CH2 -HH2 ": return key_atom12_nrt;


// {(sidechain-rot-CB    -CG  , atom 53 CB, 7 TRP, pos(3.673,15.42,21.22), alt( ), chain(A), atom 54 CG, 7 TRP, pos(2.828,16.14,20.2), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CG    -CD1 , atom 54 CG, 7 TRP, pos(2.828,16.14,20.2), alt( ), chain(A), atom 55 CD1, 7 TRP, pos(1.446,16.31,20.21), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CG    -CD2 , atom 54 CG, 7 TRP, pos(2.828,16.14,20.2), alt( ), chain(A), atom 56 CD2, 7 TRP, pos(3.252,16.84,19.02), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CD1   -NE1 , atom 55 CD1, 7 TRP, pos(1.446,16.31,20.21), alt( ), chain(A), atom 57 NE1, 7 TRP, pos(1.064,17.01,19.14), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CD2   -CE2 , atom 56 CD2, 7 TRP, pos(3.252,16.84,19.02), alt( ), chain(A), atom 58 CE2, 7 TRP, pos(2.146,17.38,18.37), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CD2   -CE3 , atom 56 CD2, 7 TRP, pos(3.252,16.84,19.02), alt( ), chain(A), atom 59 CE3, 7 TRP, pos(4.541,17.1,18.46), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CE2 -CZ2 , atom 58 CE2, 7 TRP, pos(2.146,17.38,18.37), alt( ), chain(A), atom 60 CZ2, 7 TRP, pos(2.219,18.09,17.19), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CE3 -CZ3 , atom 59 CE3, 7 TRP, pos(4.541,17.1,18.46), alt( ), chain(A), atom 61 CZ3, 7 TRP, pos(4.611,17.82,17.28), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CZ2 -CH2 , atom 60 CZ2, 7 TRP, pos(2.219,18.09,17.19), alt( ), chain(A), atom 62 CH2, 7 TRP, pos(3.451,18.31,16.65), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>
// {(sidechain-nrt-CZ3 -CH2 , atom 61 CZ3, 7 TRP, pos(4.611,17.82,17.28), alt( ), chain(A), atom 62 CH2, 7 TRP, pos(3.451,18.31,16.65), alt( ), chain(A))}	System.Tuple<string,HTLib2.Bioinfo.Pdb.Atom,HTLib2.Bioinfo.Pdb.Atom>

                }
                return null;
            }
            public static Tuple<string,Atom,Atom> GetKey(Atom atom1, Atom atom2)
            {
                string name1 = (atom1.name.Trim().ToUpper()+"    ").Substring(0, 4);
                string name2 = (atom2.name.Trim().ToUpper()+"    ").Substring(0, 4);
                if(name1 == name2)
                    return null;
                switch(name1+"-"+name2)
                {
                    case "N   -CA  ": return new Tuple<string, Atom, Atom>("N   -CA  ", atom1, atom2);  ///      |
                    case "CA  -C   ": return new Tuple<string, Atom, Atom>("CA  -C   ", atom1, atom2);  ///      N-H
                    case "C   -O   ": return new Tuple<string, Atom, Atom>("C   -O   ", atom1, atom2);  ///      |  
                    case "C   -N   ": return new Tuple<string, Atom, Atom>("C   -N   ", atom1, atom2);  ///  HA1-CA-...
                    case "N   -H   ": return new Tuple<string, Atom, Atom>("N   -H   ", atom1, atom2);  ///      |  
                    case "CA  -HA1 ": return new Tuple<string, Atom, Atom>("CA  -HA1 ", atom1, atom2);  ///      C=O
                                                                                                        ///      |
                    case "CA  -N   ": return new Tuple<string, Atom, Atom>("N   -CA  ", atom2, atom1);
                    case "C   -CA  ": return new Tuple<string, Atom, Atom>("CA  -C   ", atom2, atom1);
                    case "O   -C   ": return new Tuple<string, Atom, Atom>("C   -O   ", atom2, atom1);
                    case "N   -C   ": return new Tuple<string, Atom, Atom>("C   -N   ", atom2, atom1);
                    case "H   -N   ": return new Tuple<string, Atom, Atom>("N   -H   ", atom2, atom1);
                    case "HA1 -CA  ": return new Tuple<string, Atom, Atom>("CA  -HA1 ", atom2, atom1);

                    case "N   -CD  ": return new Tuple<string, Atom, Atom>("N   -CD  ", atom1, atom2);  /// PRO: --CA----N--
                    case "CD  -N   ": return new Tuple<string, Atom, Atom>("N   -CD  ", atom2, atom1);  ///        CB-CG-CD
                    case "C   -OXT ": return new Tuple<string, Atom, Atom>("C   -OXT ", atom1, atom2);
                    case "OXT -C   ": return new Tuple<string, Atom, Atom>("C   -OXT ", atom2, atom1);
                }
                if((name1.Trim().Length == 1) || (name2.Trim().Length == 1))
                    return null;
                Func<string,int> GetLevel = delegate(string atom)
                {
                    if(atom == "OXT ") return int.MaxValue;
                    int level = 0;
                    switch(atom[1])
                    {
                        case 'A': level = level*10 + 1; break;
                        case 'B': level = level*10 + 2; break;
                        case 'G': level = level*10 + 3; break;
                        case 'D': level = level*10 + 4; break;
                        case 'E': level = level*10 + 5; break;
                        case 'Z': level = level*10 + 6; break;
                        case 'H': level = level*10 + 7; break;
                        case 'X': level = level*10 + 8; break; // OXT
                        default: HDebug.Assert(false); return -1;
                    }
                    switch(atom[2])
                    {
                        case ' ': level = level*10 + 0; break;
                        case '1': level = level*10 + 1; break;
                        case '2': level = level*10 + 2; break;
                        case '3': level = level*10 + 3; break;
                        case '4': level = level*10 + 4; break;
                        default: HDebug.Assert(false); return -1;
                    }
                    switch(atom[3])
                    {
                        case ' ': level = level*10 + 0; break;
                        case '1': level = level*10 + 1; break;
                        case '2': level = level*10 + 2; break;
                        case '3': level = level*10 + 3; break;
                        case '4': level = level*10 + 4; break;
                        default: HDebug.Assert(false); return -1;
                    }
                    switch(atom[0])
                    {
                        case 'C': level = level*10 + 1; break;
                        case 'N': level = level*10 + 2; break;
                        case 'O': level = level*10 + 3; break;
                        case 'S': level = level*10 + 4; break;
                        case 'H': level = level*10 + 5; break;
                        default:  level = level*10 + 6; break;
                    }
                    HDebug.Assert(level >= 1);
                    return level;
                };
                int level1 = GetLevel(name1);
                int level2 = GetLevel(name2);
                if(level1 < level2) return new Tuple<string,Atom,Atom>(name1+"-"+name2, atom1, atom2);
                if(level2 < level1) return new Tuple<string,Atom,Atom>(name2+"-"+name1, atom2, atom1);
                HDebug.Assert(level1 == level2);
                HDebug.Assert(name1.Trim().Length == name2.Trim().Length);
                if((name1.Substring(0, 3) == name2.Substring(0, 3)) && (name1[3] < name2[3])) return new Tuple<string, Atom, Atom>(name1+"-"+name2, atom1, atom2);
                if((name1.Substring(0, 3) == name2.Substring(0, 3)) && (name1[3] > name2[3])) return new Tuple<string, Atom, Atom>(name2+"-"+name1, atom2, atom1);
                if((name1.Substring(0, 2) == name2.Substring(0, 2)) && (name1[2] < name2[2])) return new Tuple<string, Atom, Atom>(name1+"-"+name2, atom1, atom2);
                if((name1.Substring(0, 2) == name2.Substring(0, 2)) && (name1[2] > name2[2])) return new Tuple<string, Atom, Atom>(name2+"-"+name1, atom2, atom1);
                if((name1.Substring(0, 1) == name2.Substring(0, 1)) && (name1[1] < name2[1])) return new Tuple<string, Atom, Atom>(name1+"-"+name2, atom1, atom2);
                if((name1.Substring(0, 1) == name2.Substring(0, 1)) && (name1[1] > name2[1])) return new Tuple<string, Atom, Atom>(name2+"-"+name1, atom2, atom1);
                HDebug.Assert(false);
                return null;
            }
        }
    }
}
