#pragma warning disable CS0414

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        [Serializable]
        public partial class Prm : IBinarySerializable
        {
            ///////////////////////////////////////////////////
            // IBinarySerializable
            public void Serialize(HBinaryWriter writer)
            {
                writer.Write(_lines);
            }
            public Prm(HBinaryReader reader)
            {
                string[] lines; reader.Read(out lines);

                (List<Element> elements, Dictionary<int,int> id2idxatom, Dictionary<int,string> class2type) = GetDataFromLines(lines);

                this._lines     = lines.ToArray()   ;
                this._elements  = elements.ToArray();
                this.id2idxatom = id2idxatom        ;
                this.class2type = class2type        ;
            }
            // IBinarySerializable
            ///////////////////////////////////////////////////

            public static Universe UpdateUnivParams(string parampath, Universe univ)
            {
                throw new NotImplementedException("need to check!!!");

#pragma warning disable CS0162
                Prm prm = FromFile(parampath);
                Dictionary<int, Atom >  id2atom  = prm.atoms .ToIdDictionary();
                Dictionary<int, Vdw  > cls2vdw   = prm.vdws  .ToClassDictionary();
                Dictionary<int, Vdw14> cls2vdw14 = prm.vdw14s.ToClassDictionary();
                Bond    [] prm_bonds     = prm.bonds    ;
                Angle   [] prm_angles    = prm.angles   ;
                Ureybrad[] prm_ureybrads = prm.ureybrads;
                Improper[] prm_impropers = prm.impropers;
                Torsion [] prm_torsions  = prm.torsions ;
                Dictionary<int, Charge> id2charge = prm.charges.ToIdDictionary();
                Biotype [] prm_biotypes  = prm.biotypes ;

                int resNTerminal = univ.atoms[0].ResiduePdbId; foreach(var atom in univ.atoms) resNTerminal = Math.Min(resNTerminal, atom.ResiduePdbId);
                int resCTerminal = univ.atoms[0].ResiduePdbId; foreach(var atom in univ.atoms) resCTerminal = Math.Max(resCTerminal, atom.ResiduePdbId);
                
                univ.refs.Add("prm", prm);
                foreach(var atom in univ.atoms)
                {
                    string atomname = atom.AtomName;
                    string resiname = atom.ResidueName;
                    switch(resiname.ToUpper())
                    {
                        case "GLY": resiname = "Glycine"; break;
                        //case "Alanine"             : break;
                        case "VAL": resiname = "Valine" ; break;
                        case "LEU": resiname = "Leucine"; break;
                        //resiname = "Isoleucine"          ; break;
                        //resiname = "Serine"              ; break;
                        //resiname = "Threonine"           ; break;
                        //resiname = "Cysteine (SH)"       ; break;
                        //resiname = "Cystine (SS)"        ; break;
                        //resiname = "Cysteine (S-)"       ; break;
                        //resiname = "Proline"             ; break;
                        //resiname = "Phenylalanine"       ; break;
                        //resiname = "Tyrosine"            ; break;
                        //resiname = "Tyrosine (O-)"       ; break;
                        //resiname = "Tryptophan"          ; break;
                        //resiname = "Histidine (+)"       ; break;
                        //resiname = "Histidine (HD)"      ; break;
                        //resiname = "Histidine (HE)"      ; break;
                        //resiname = "Aspartic Acid"; break;
                        case "ASP": resiname = "Aspartic Acid (COOH)"; break;
                        //resiname = "Asparagine"; break;
                        case "GLU": resiname = "Glutamic Acid"       ; break;
                        //resiname = "Glutamic Acid (COOH)"; break;
                        case "GLN": resiname = "Glutamine"           ; break;
                        //resiname = "Methionine"          ; break;
                        case "LYS": resiname = "Lysine"; break;
                        //resiname = "Lysine (NH2)"        ; break;
                        //resiname = "Arginine"            ; break;
                        //resiname = "Ornithine"           ; break;
                        //resiname = "MethylAlanine (AIB)" ; break;
                        //resiname = "Pyroglutamic Acid"   ; break;
                        default:
                            HDebug.Assert(false);
                            break;
                    }
                    if(atom.ResiduePdbId == resNTerminal)
                    {
                        if(atomname == "H1" || atomname == "H2" || atomname == "H3") atomname = "HN";

                        if(atomname == "N"  ) resiname = "N-Terminal "+resiname;
                        if(atomname == "CA" ) resiname = "N-Terminal "+resiname;
                        if(atomname == "C"  ) resiname = "N-Terminal "+resiname;
                        if(atomname == "HN" ) resiname = "N-Terminal "+resiname;
                        if(atomname == "O"  ) resiname = "N-Terminal "+resiname;
                        if(atomname == "HA" ) resiname = "N-Terminal "+resiname;
                    }
                    if(atom.ResiduePdbId == resCTerminal)
                    {
                        if(atomname == "H1" || atomname == "H2" || atomname == "H3") atomname = "HN";

                        if(atomname == "N"  ) resiname = "N-Terminal "+resiname;
                        if(atomname == "CA" ) resiname = "N-Terminal "+resiname;
                        if(atomname == "C"  ) resiname = "N-Terminal "+resiname;
                        if(atomname == "HN" ) resiname = "N-Terminal "+resiname;
                        if(atomname == "OXT") resiname = "N-Terminal "+resiname;
                        if(atomname == "HA" ) resiname = "N-Terminal "+resiname;
                    }

                    int id    = prm_biotypes.FindId(atomname, resiname);
                    Atom   atm   = id2atom  [id];
                    int    cls   = atm.Class;
                    HDebug.Assert(atm.Type == atom.AtomType);
                    Charge chg   = id2charge[id];
                    Vdw    vdw   = cls2vdw  [cls];
                    Vdw14  vdw14 = null;
                    if(cls2vdw14.ContainsKey(cls)) vdw14 = cls2vdw14[cls];

                    var cvt = atom.ConvertGromacsToCharmm;

                    atom.Charge      = chg.pch;
                    atom.Mass        = atm.Mass;
                    atom.epsilon     = vdw.Epsilon;
                    atom.Rmin2       = vdw.Rmin2;
                    atom.eps_14      = (vdw14!=null) ? vdw14.Eps_14   : double.NaN;
                    atom.Rmin2_14    = (vdw14!=null) ? vdw14.Rmin2_14 : double.NaN;
                }

                /// Universe nuniv = new Universe();
                /// nuniv.pdb          = pdb;
                /// nuniv.refs.Add(top);
                /// nuniv.atoms        = atoms;
                /// nuniv.bonds        = bonds;
                /// nuniv.angles       = angles;
                /// nuniv.dihedrals    = dihedrals;
                /// nuniv.impropers    = impropers;
                /// //nuniv.nonbondeds   = nonbondeds  ;  // do not make this list in advance, because it depends on the atom positions
                /// nuniv.nonbonded14s = nonbonded14s;
                /// 
                return null;
#pragma warning restore CS0162
            }

            public string[]  lines    { get { return _lines   ; } } string[]  _lines   ;
            public Element[] elements { get { return _elements; } } Element[] _elements;
            Dictionary<int, int>    id2idxatom;
            Dictionary<int, string> class2type;
            public Atom    [] atoms    { get { return elements.HSelectByType<Element, Atom    >().ToArray(); } }
            public Vdw     [] vdws     { get { return elements.HSelectByType<Element, Vdw     >().ToArray(); } }
            public Vdw14   [] vdw14s   { get { return elements.HSelectByType<Element, Vdw14   >().ToArray(); } }
            public Bond    [] bonds    { get { return elements.HSelectByType<Element, Bond    >().ToArray(); } }
            public Angle   [] angles   { get { return elements.HSelectByType<Element, Angle   >().ToArray(); } }
            public Ureybrad[] ureybrads{ get { return elements.HSelectByType<Element, Ureybrad>().ToArray(); } }
            public Improper[] impropers{ get { return elements.HSelectByType<Element, Improper>().ToArray(); } }
            public Torsion [] torsions { get { return elements.HSelectByType<Element, Torsion >().ToArray(); } }
            public Charge  [] charges  { get { return elements.HSelectByType<Element, Charge  >().ToArray(); } }
            public Biotype [] biotypes { get { return elements.HSelectByType<Element, Biotype >().ToArray(); } }

            public Atom IdToAtom(int id)
            {
                int idxatom = id2idxatom[id];
                return (elements[idxatom] as Atom);
            }

            public static void SelfTest()
            {
                FromLines(Selftest.charmm22);
            }

            public static Prm FromFile(string filepath)
            {
                List<string> lines = HFile.ReadLines(filepath).ToList();
                return FromLines(lines);
            }
            public void ToFile(string filepath)
            {
                //List<string> lines = new List<string>();
                //foreach(Element element in elements)
                //    lines.Add(element.line);
                HFile.WriteAllLines(filepath, lines);
            }
            public Prm(string[] _lines, Element[] _elements, Dictionary<int, int> id2idxatom, Dictionary<int, string> class2type)
            {
                this._lines     = _lines    ;
                this._elements  = _elements ;
                this.id2idxatom = id2idxatom;
                this.class2type = class2type;
            }
            public static (List<Element> elements, Dictionary<int,int> id2idxatom, Dictionary<int,string> class2type) GetDataFromLines(IList<string> lines)
            {
                List<Element> elements            = new List<Element>();
                Dictionary<int,int   > id2idxatom = new Dictionary<int,int   >();
                Dictionary<int,string> class2type = new Dictionary<int,string>();
                foreach(string line in lines)
                {
                    {
                        var atom = Atom.FromLine(line);
                        if(atom != null)
                        {
                            int idxatom = elements.Count;
                            elements.Add(atom);
                            id2idxatom.Add(atom.Id, idxatom);
                            if(class2type.ContainsKey(atom.Class) == false)
                                class2type.Add(atom.Class, atom.Type);
                            HDebug.Assert(class2type[atom.Class] == atom.Type);
                            continue;
                        }
                    }
                    { var elem = Vdw     .FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Vdw14   .FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Bond    .FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Angle   .FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Ureybrad.FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Improper.FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Torsion .FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Charge  .FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    { var elem = Biotype .FromLine(line); if(elem != null) { elements.Add(elem); continue; } }
                    {
                        var elem = new Element { line = line };
                        elements.Add(elem);
                    }
                }

                return (elements, id2idxatom, class2type);
            }
            public static Prm FromLines(IList<string> lines)
            {
                (List<Element> elements, Dictionary<int,int> id2idxatom, Dictionary<int,string> class2type) = GetDataFromLines(lines);

                return new Prm( _lines     : lines.ToArray()
                              , _elements  : elements.ToArray()
                              , id2idxatom : id2idxatom
                              , class2type : class2type
                              );
            }
            public static List<Element> GetListElement(IList<string> lines)
            {
                return null;
            }

            [Serializable]
            public class Element
            {
                public string line;
                string[] tokens = null;
                public override string ToString() { return line; }
                public string[] GetTokens()
                {
                    if(tokens != null)
                        return tokens;
                    List<string> ltokens = new List<string>();
                    ltokens.Add(line.Trim());
                    while(ltokens.Last().Length > 0)
                    {
                        string lline = ltokens.Last();
                        ltokens.RemoveAt(ltokens.Count-1);
                        if(lline.StartsWith("\""))
                        {
                            // combile "\"xxx xxx xxx\"" as => "xxx xxx xxx"
                            lline = lline.Substring(1);
                            int idx = lline.IndexOf('\"');
                            string lltokens0 = lline.Substring(0, idx);
                            string lltokens1 = lline.Substring(idx+1);
                            ltokens.Add(lltokens0);
                            ltokens.Add(lltokens1.Trim());
                        }
                        else if(lline.StartsWith("#"))
                        {
                            // remove because it is a comment
                            // finish parsing by adding "" at the end
                            ltokens.Add("");
                        }
                        else
                        {
                            List<string> lltokens = new List<string>(lline.Split(new char[] { ' ' }, 2));
                            lltokens.Add("");
                            ltokens.Add(lltokens[0]);
                            ltokens.Add(lltokens[1].Trim());
                        }
                    }
                    ltokens.RemoveAt(ltokens.Count-1);
                    tokens = ltokens.ToArray();
                    return tokens;
                }
                public string  GetTokenString(int idx) { string[] tokens = GetTokens(); if(idx >= tokens.Length) return null;return              tokens[idx];  }
                public int?    GetTokenInt   (int idx) { string[] tokens = GetTokens(); if(idx >= tokens.Length) return null;return    int.Parse(tokens[idx]); }
                public double? GetTokenDouble(int idx) { string[] tokens = GetTokens(); if(idx >= tokens.Length) return null;return double.Parse(tokens[idx]); }
            }
            /// forcefield              CHARMM22
            /// vdwtype                 LENNARD-JONES
            /// radiusrule              ARITHMETIC
            /// radiustype              R-MIN
            /// radiussize              RADIUS
            /// epsilonrule             GEOMETRIC
            /// vdw-14-scale            1.0
            /// chg-14-scale            1.0
            /// electric                332.0716
            /// dielectric              1.0
            /// 
            ///       #############################
            ///       ##                         ##
            ///       ##  Literature References  ##
            ///       ##                         ##
            ///       #############################
            /// 
            /// A. D. MacKerrell, Jr., et al., "All-Atom Empirical Potential for
            /// ...
            /// 
            /// 
            /// class structures
            ///     Id   : Atom(Id,Class,Type,Desc,Mass), Vdw(Id,Rmin2,Epsilon), Charge(Id,pch), Biotype(BioId,Name,Resi,Id)
            ///     Class: Atom(Id,Class,Type,Desc,Mass), Vdw14(Class,Rmin2_14,Eps_14), Bond(Class1,Class2,Kb,b0),
            ///                                           Angle(Class1,Class2,Class3,Ktheta,Theta0), Ureybrad(Class1,Class2,Class3,Kub,S0),
            ///                                           Improper(Class1,Class2,Class3,Class4,Kpsi,psi0),
            ///                                           Torsion(Class1,Class2,Class3,Class4,Kchi0,delta0,n0,Kchi1,delta1,n1,Kchi2,delta2,n2)
            ///     Type : Atom(Id,Class,Type,Desc,Mass), 
            /// 
            [Serializable]
            public class Atom : Element
            {
                /// sample
                ///       #############################
                ///       ##                         ##
                ///       ##  Atom Type Definitions  ##
                ///       ##                         ##
                ///       #############################
                /// 
                /// 
                ///    ######################################################
                ///    ##                                                  ##
                ///    ##  TINKER Atom Class Numbers to CHARMM Atom Names  ##
                ///    ##                                                  ##
                ///    ##     1  HA      11  CA      21  CY      31  NR3   ##
                ///    ##     2  HP      12  CC      22  CPT     32  NY    ##
                ///    ##     3  H       13  CT1     23  CT      33  NC2   ##
                ///    ##     4  HB      14  CT2     24  NH1     34  O     ##
                ///    ##     5  HC      15  CT3     25  NH2     35  OH1   ##
                ///    ##     6  HR1     16  CP1     26  NH3     36  OC    ##
                ///    ##     7  HR2     17  CP2     27  N       37  S     ##
                ///    ##     8  HR3     18  CP3     28  NP      38  SM    ##
                ///    ##     9  HS      19  CH1     29  NR1               ##
                ///    ##    10  C       20  CH2     30  NR2               ##
                ///    ##                                                  ##
                ///    ######################################################
                /// 
                ///          1         2         3         4         5         6         7         8
                /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///              id   class type  description                         mass
                /// atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                /// atom          2    2    HP    "Aromatic Hydrogen"            1     1.008    1
                /// atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                /// ...
                public int    Id          { get { return GetTokenInt   (1).Value; } }
                public int    Class       { get { return GetTokenInt   (2).Value; } }
                public string Type        { get { return GetTokenString(3)      ; } }
                public string Description { get { return GetTokenString(4)      ; } }
                public int    AtomicNumber{ get { return GetTokenInt   (5).Value; } }
                public double Mass        { get { return GetTokenDouble(6).Value; } }
                public int    Valence     { get { return GetTokenInt   (7).Value; } }
                public override string ToString() { return line; }
                public static Atom FromLine(string line)
                {
                    if(line.StartsWith("atom "))
                    {
                        Atom atom = new Atom { line=line };
                        return atom;
                    }
                    return null;
                }
                public static Atom FromData
                    ( int    Id
                    , int    Class
                    , string Type
                    , string Description
                    , int?   AtomicNumber
                    , double Mass  
                    , int    Valence
                    )
                {   //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "      82 A    7    VAL  HG23 HA     0.090000        1.0080           0"
                    // "atom         89    3    H     "COOH Hydrogen"                1     1.008    1"
                    // "atom        108   57    HE    "Helium Atom"                  2     4.003    0"
                    // "atom        100   50    NPH   "Heme Pyrrole N"               7    14.007    3"
                    // "atom        106   55    SS    "Thiolate Sulfur"             16    32.060    1"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(100, 50, "NPH", "Heme Pyrrole N", 7, 14.007, 3);
                        var test_line = test.line;
                        var _____line = "atom        100   50    NPH   \"Heme Pyrrole N\"               7    14.007    3";
                        HDebug.Assert(test_line == _____line);
                    }
                    if(AtomicNumber == null)
                    {
                        switch(Type.ToUpper()[0])
                        {
                            case 'H': AtomicNumber =  1; break; // Hydrogen     https://en.wikipedia.org/wiki/Hydrogen
                            case 'C': AtomicNumber =  6; break; // Carbon       https://en.wikipedia.org/wiki/Carbon
                            case 'N': AtomicNumber =  7; break; // Nitrogen     https://en.wikipedia.org/wiki/Nitrogen
                            case 'O': AtomicNumber =  8; break; // Oxygen       https://en.wikipedia.org/wiki/Oxygen
                            case 'S': AtomicNumber = 16; break; // Sulfur       https://en.wikipedia.org/wiki/Sulfur
                            case 'F': AtomicNumber = 26; break; // Iron         https://en.wikipedia.org/wiki/Iron
                            default:
                                HDebug.ToDo("Tinker.Prm.Atom.FromData(...): AtomicNumber of "+Type+" is not defined");
                                AtomicNumber = (int)(Math.Round(Mass/2));
                                break;
                        }
                    }

                    string line = "atom ";
                    line += string.Format("{0,10}", Id);
                    line += string.Format("{0,5}" , Class);
                    line += "    ";
                    line += string.Format("{0,-6}", Type);
                    line += ("\""+Description+"\"                                                ").Substring(0, 27);
                    line += string.Format("{0,5}" , AtomicNumber);
                    line += string.Format("{0,10:0.000}", Mass);
                    line += string.Format("{0,5}" , Valence);
                    return FromLine(line);
                }

                public bool IsHydrogen
                {
                    get
                    {
                        bool isHydrogen = Type.Trim().StartsWith("H");
                        if(HDebug.IsDebuggerAttached)
                        {
                            if(isHydrogen)
                            {
                                //HDebug.Assert(prmatom.Description.Contains("Hydro") == true);
                                HDebug.Assert(Mass < 2);
                            }
                            else
                            {
                                HDebug.Assert(Description.Contains("Hydrog") == false);
                                HDebug.Assert(Mass >= 2);
                            }
                        }
                        return isHydrogen;
                    }
                }
                public string AtomElem
                {
                    get
                    {
                        switch(Type)
                        {
                            case "C": case "CA" : case "CC" : case "CH1": case "CH2": case "CP1": case "CP2": case "CP3": case "CPT": case "CT" : case "CT1": case "CT2": case "CT3": case "CY" :
                                //AtomElem = "C"; break;
                                return "C";
                            case "H": case "HA" : case "HB" : case "HC" : case "HP" : case "HR1": case "HR2": case "HR3": case "HS" : case "HT" :
                                //AtomElem = "H"; break;
                                return "H";
                            case "N": case "NC2": case "NH1": case "NH2": case "NH3": case "NP" : case "NR1": case "NR2": case "NR3": case "NY" :
                                //AtomElem = "N"; break;
                                return "N";
                            case "O": case "OC" : case "OH1": case "OT" :
                                //AtomElem = "O"; break;
                                return "O";
                            case "S": case "SM" :
                                //AtomElem = "S"; break;
                                return "S";
                            /// ///////////////////////////////////////////////////
                            /// heme special
                            case "FE": return "FE";
                            case "NPH": return "N";
                            case "CPA": case "CPB": case "CPM": return "C";
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
            [Serializable]
            public class Vdw : Element
            {
                ///       ################################
                ///       ##                            ##
                ///       ##  Van der Waals Parameters  ##
                ///       ##                            ##
                ///       ################################
                /// 
                ///              class            rmin2     epsilon
                /// vdw           1               1.3200    -0.0220
                /// vdw           2               1.3582    -0.0300
                /// vdw          34               1.7000    -0.1200
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                ///     atom          2    2    HP    "Aromatic Hydrogen"            1     1.008    1
                ///     atom         74   34    O     "Peptide Oxygen"               8    15.999    1
                /// charmm
                ///     !atom  ignored    epsilon      Rmin/2    ignored    eps,1-4       Rmin/2,1-4
                ///     HA     0.000000  -0.022000     1.320000                                    ! ALLOW PEP ALI POL SUL ARO PRO ALC
                ///     HP     0.000000  -0.030000     1.358200   0.000000  -0.030000     1.358200 ! ALLOW ARO
                ///     O      0.000000  -0.120000     1.700000   0.000000  -0.120000     1.400000 ! ALLOW   PEP POL
                public int    Class   { get { return GetTokenInt   (1).Value; } }
                public double Rmin2   { get { return GetTokenDouble(2).Value; } }
                public double Epsilon { get { return GetTokenDouble(3).Value; } }
                public override string ToString() { return line; }
                public static Vdw FromLine(string line)
                {
                    if(line.StartsWith("vdw "))
                    {
                        Vdw vdw = new Vdw { line=line };
                        return vdw;
                    }
                    return null;
                }
                public static Vdw FromData(int Class, double Rmin2, double Epsilon)
                {
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "      82 A    7    VAL  HG23 HA     0.090000        1.0080           0"
                    // "vdw           1               1.3200    -0.0220"
                    // "vdw           2               1.3582    -0.0300"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(1, 1.3200, -0.0220);
                        var test_line = test.line;
                        var _____line = "vdw           1               1.3200    -0.0220";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "vdw  ";
                    line += string.Format("{0,10}", Class);
                    line += "          ";
                    line += string.Format("{0,11:0.0000}", Rmin2);
                    line += string.Format("{0,11:0.0000}", Epsilon);
                    return FromLine(line);
                }

            }
            [Serializable]
            public class Vdw14 : Element
            {
                ///       ####################################
                ///       ##                                ##
                ///       ##  1-4 Van der Waals Parameters  ##
                ///       ##                                ##
                ///       ####################################
                /// 
                ///              class            Rmin2_14  Eps_14
                /// vdw14        13               1.9000    -0.0100
                /// vdw14        14               1.9000    -0.0100
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom         25   13    CT1   "Methine Carbon"               6    12.011    4
                ///     atom         26   14    CT2   "Methylene Carbon"             6    12.011    4
                /// charmm
                ///     !atom  ignored    epsilon      Rmin/2    ignored    eps,1-4       Rmin/2,1-4
                ///     CT1    0.000000  -0.020000     2.275000   0.000000  -0.010000     1.900000 ! ALLOW   ALI
                ///     CT2    0.000000  -0.055000     2.175000   0.000000  -0.010000     1.900000 ! ALLOW   ALI
                public int    Class    { get { return GetTokenInt   (1).Value; } }
                public double Rmin2_14 { get { return GetTokenDouble(2).Value; } }
                public double Eps_14   { get { return GetTokenDouble(3).Value; } }
                public override string ToString() { return line; }
                public static Vdw14 FromLine(string line)
                {
                    if(line.StartsWith("vdw14 "))
                    {
                        Vdw14 vdw14 = new Vdw14 { line=line };
                        return vdw14;
                    }
                    return null;
                }
                public static Vdw14 FromData(int Class, double Rmin2_14, double Eps_14)
                {
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "      82 A    7    VAL  HG23 HA     0.090000        1.0080           0"
                    // "vdw14        13               1.9000    -0.0100"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(13, 1.9000, -0.0100);
                        var test_line = test.line;
                        var _____line = "vdw14        13               1.9000    -0.0100";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "vdw14";
                    line += string.Format("{0,10}", Class);
                    line += "          ";
                    line += string.Format("{0,11:0.0000}", Rmin2_14);
                    line += string.Format("{0,11:0.0000}", Eps_14);
                    return FromLine(line);
                }
            }
            [Serializable]
            public class Bond : Element
            {
                ///       ##################################
                ///       ##                              ##
                ///       ##  Bond Stretching Parameters  ##
                ///       ##                              ##
                ///       ##################################
                /// 
                ///           class1 class2       Kb         b0
                /// bond          1   10          330.00     1.1000
                /// bond          1   11          340.00     1.0830
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                ///     atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                ///     atom         21   11    CA    "Aromatic Carbon"              6    12.011    3
                /// charmm
                ///     !atom type Kb          b0
                ///     HA   C     330.000     1.1000 ! ALLOW ARO HEM
                ///     HA   CA    340.000     1.0830 ! ALLOW ARO
                public int    Class1 { get { return GetTokenInt   (1).Value; } }
                public int    Class2 { get { return GetTokenInt   (2).Value; } }
                public double Kb     { get { return GetTokenDouble(3).Value; } }
                public double b0     { get { return GetTokenDouble(4).Value; } }
                public override string ToString() { return line; }
                public static Bond FromLine(string line)
                {
                    if(line.StartsWith("bond "))
                    {
                        Bond bond = new Bond { line=line };
                        return bond;
                    }
                    return null;
                }
                public static Bond FromData(int Class1, int Class2, double Kb, double b0)
                {
                    HDebug.Assert(Class1 <= Class2);
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "bond          1   10          330.00     1.1000"
                    // "bond          1   11          340.00     1.0830"
                    // "bond         10   13          250.00     1.4900"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(10, 13, 250.00, 1.4900);
                        var test_line = test.line;
                        var _____line = "bond         10   13          250.00     1.4900";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "bond      ";
                    line += string.Format("{0,5}", Class1);
                    line += string.Format("{0,5}", Class2);
                    line += string.Format("{0,16:0.00}", Kb);
                    line += string.Format("{0,11:0.0000}", b0);
                    return FromLine(line);
                }
            }
            [Serializable]
            public class Angle : Element
            {
                ///       ################################
                ///       ##                            ##
                ///       ##  Angle Bending Parameters  ##
                ///       ##                            ##
                ///       ################################
                /// 
                ///          class1 class2 class3  Ktheta    Theta0
                /// angle         3   10   34      50.00     121.70
                /// angle        13   10   24      80.00     116.50
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom          8    3    H     "Hydroxyl Hydrogen"            1     1.008    1
                ///     atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                ///     atom         36   13    CT1   "THR CB Carbon"                6    12.011    4
                ///     atom         74   34    O     "Peptide Oxygen"               8    15.999    1
                ///     atom         63   24    NH1   "Peptide Nitrogen"             7    14.007    3
                /// charmm
                ///     !atom types      Ktheta   Theta0   Kub     S0
                ///     O    C    H      50.000   121.7000 ! ALLOW   PEP POL ARO
                ///     NH1  C    CT1    80.000   116.5000 ! ALLOW   ALI PEP POL ARO
                public int    Class1 { get { return GetTokenInt   (1).Value; } }
                public int    Class2 { get { return GetTokenInt   (2).Value; } }
                public int    Class3 { get { return GetTokenInt   (3).Value; } }
                public double Ktheta { get { return GetTokenDouble(4).Value; } }
                public double Theta0 { get { return GetTokenDouble(5).Value; } }
                public override string ToString() { return line; }
                public static Angle FromLine(string line)
                {
                    if(line.StartsWith("angle "))
                    {
                        Angle angle = new Angle { line=line };
                        return angle;
                    }
                    return null;
                }
                public static Angle FromData(int Class1, int Class2, int Class3, double Ktheta, double Theta0)
                {
                    HDebug.Assert(Class1 <= Class3);
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "angle         3   10   34      50.00     121.70"
                    // "angle        13   10   24      80.00     116.50"
                    // "angle         9   37   14      38.80      95.00"
                    // "angle         1   47    1      35.50     108.40"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(9, 37, 14, 38.80, 95.00);
                        var test_line = test.line;
                        var _____line = "angle         9   37   14      38.80      95.00";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "angle     ";
                    line += string.Format("{0,5}", Class1);
                    line += string.Format("{0,5}", Class2);
                    line += string.Format("{0,5}", Class3);
                    line += string.Format("{0,11:0.00}", Ktheta);
                    line += string.Format("{0,11:0.00}", Theta0);
                    return FromLine(line);
                }
            }
            [Serializable]
            public class Ureybrad : Element
            {
                ///       ###############################
                ///       ##                           ##
                ///       ##  Urey-Bradley Parameters  ##
                ///       ##                           ##
                ///       ###############################
                /// 
                ///          class1 class2 class3  Kub       S0
                /// ureybrad     33   10   33      90.00     2.3642
                /// ureybrad      1   11   11      25.00     2.1525
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                ///     atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                ///     atom         21   11    CA    "Aromatic Carbon"              6    12.011    3
                ///     atom         72   33    NC2   "ARG NE Nitrogen"              7    14.007    3
                /// charmm
                ///     !atom types      Ktheta   Theta0   Kub     S0
                ///     NC2  C    NC2   52.000    120.00   90.00   2.36420 ! ALLOW   POL PEP ARO
                ///     HA   CA   CA    29.000    120.00   25.00   2.15250 ! ALLOW ARO
                public int    Class1 { get { return GetTokenInt   (1).Value; } }
                public int    Class2 { get { return GetTokenInt   (2).Value; } }
                public int    Class3 { get { return GetTokenInt   (3).Value; } }
                public double Kub    { get { return GetTokenDouble(4).Value; } }
                public double S0     { get { return GetTokenDouble(5).Value; } }
                public override string ToString() { return line; }
                public static Ureybrad FromLine(string line)
                {
                    if(line.StartsWith("ureybrad "))
                    {
                        Ureybrad ureybrad = new Ureybrad { line=line };
                        return ureybrad;
                    }
                    return null;
                }
                public static Ureybrad FromData(int Class1, int Class2, int Class3, double Kub, double S0)
                {
                    HDebug.Assert(Class1 <= Class3);
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "ureybrad     33   10   33      90.00     2.3642"
                    // "ureybrad      1   11   11      25.00     2.1525"
                    // "ureybrad     67   72   67       5.40     1.8020"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(1, 11, 11, 25.00, 2.1525);
                        var test_line = test.line;
                        var _____line = "ureybrad      1   11   11      25.00     2.1525";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "ureybrad  ";
                    line += string.Format("{0,5}", Class1);
                    line += string.Format("{0,5}", Class2);
                    line += string.Format("{0,5}", Class3);
                    line += string.Format("{0,11:0.00}", Kub);
                    line += string.Format("{0,11:0.0000}", S0);
                    return FromLine(line);
                }
            }
            [Serializable]
            public class Improper : Element
            {
                ///       ####################################
                ///       ##                                ##
                ///       ##  Improper Dihedral Parameters  ##
                ///       ##                                ##
                ///       ####################################
                /// 
                /// 
                ///    ##################################################################
                ///    ##                                                              ##
                ///    ##  Following CHARMM style, the improper for a trigonal atom    ##
                ///    ##  D bonded to atoms A, B and C could be input as improper     ##
                ///    ##  dihedral angle D-A-B-C. The actual angle computed by the    ##
                ///    ##  program is then literally the dihedral D-A-B-C, which will  ##
                ///    ##  always have as its ideal value zero degrees. In general     ##
                ///    ##  D-A-B-C is different from D-B-A-C; the order of the three   ##
                ///    ##  peripheral atoms matters. In the original CHARMM parameter  ##
                ///    ##  files, the trigonal atom is often listed last; ie, as       ##
                ///    ##  C-B-A-D instead of D-A-B-C.                                 ##
                ///    ##                                                              ##
                ///    ##  Some of the improper angles are "double counted" in the     ##
                ///    ##  CHARMM protein parameter set. Since TINKER uses only one    ##
                ///    ##  improper parameter per site, we have doubled these force    ##
                ///    ##  constants in the TINKER version of the CHARMM parameters.   ##
                ///    ##  Symmetric parameters, which are the origin of the "double   ##
                ///    ##  counted" CHARMM values, are handled in the TINKER package   ##
                ///    ##  by assigning all symmetric states and using the TINKER      ##
                ///    ##  force constant divided by the symmetry number.              ##
                ///    ##                                                              ##
                ///    ##  Below are the original CHARMM improper parameters:          ##
                ///    ##                                                              ##
                ///    ##  improper      6   29   30   20           0.50       0.00    ##
                ///    ##  improper      6   30   29   20           0.50       0.00    ##
                ///    ##  improper      8   19   29   19           0.50       0.00    ##
                ///    ##  improper      8   19   30   19           0.50       0.00    ##
                ///    ##  improper      8   19   31   19           1.00       0.00    ##
                ///    ##  improper      8   29   19   19           0.50       0.00    ##
                ///    ##  improper      8   30   19   19           0.50       0.00    ##
                ///    ##  improper     27   10   16   18           0.00       0.00    ##
                ///    ##  improper     29   19   20    3           0.45       0.00    ##
                ///    ##  improper     29   20   19    3           0.45       0.00    ##
                ///    ##  improper     31   19   20    3           1.20       0.00    ##
                ///    ##  improper     31   20   19    3           1.20       0.00    ##
                ///    ##  improper     32   11   21   22         100.00       0.00    ##
                ///    ##  improper     34   16   25   12          45.00       0.00    ##
                ///    ##  improper     34   13   25   12          45.00       0.00    ##
                ///    ##  improper     34   14   25   12          45.00       0.00    ##
                ///    ##  improper     34   15   25   12          45.00       0.00    ##
                ///    ##  improper     34    1   25   12          45.00       0.00    ##
                ///    ##  improper     34   27   14   12         120.00       0.00    ##
                ///    ##  improper     34   25   16   12          45.00       0.00    ##
                ///    ##  improper     34   25   13   12          45.00       0.00    ##
                ///    ##  improper     34   25   14   12          45.00       0.00    ##
                ///    ##  improper     34   25   15   12          45.00       0.00    ##
                ///    ##  improper     34   25    1   12          45.00       0.00    ##
                ///    ##  improper     33    X    X   10          40.00       0.00    ##
                ///    ##  improper     24    X    X    3          20.00       0.00    ##
                ///    ##  improper     25    X    X    3           4.00       0.00    ##
                ///    ##  improper     34    X    X   10         120.00       0.00    ##
                ///    ##  improper     36    X    X   12          96.00       0.00    ##
                ///    ##                                                              ##
                ///    ##################################################################
                /// 
                ///          class1 class2 class3 class4     Kpsi         psi0
                /// improper     10   13   24   34           120.00       0.00
                /// improper     10   13   25   34            90.00       0.00
                /// improper     10   13   27   34           120.00       0.00
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                ///     atom         23   13    CT1   "Peptide Alpha Carbon"         6    12.011    4
                ///     atom         63   24    NH1   "Peptide Nitrogen"             7    14.007    3
                ///     atom         64   25    NH2   "Amide Nitrogen"               7    14.007    3
                ///     atom         66   27    N     "PRO Nitrogen"                 7    14.007    3
                ///     atom         74   34    O     "Peptide Oxygen"               8    15.999    1
                /// charmm
                ///     !atom types           Kpsi                   psi0
                ///     O    N    CT2  CC    120.0000         0      0.0000 ! ALLOW PEP POL PRO
                ///     O    X    X    C     120.0000         0      0.0000 ! ALLOW   PEP POL ARO
                public int    Class1    { get { return GetTokenInt   (1).Value; } }
                public int    Class2    { get { return GetTokenInt   (2).Value; } }
                public int    Class3    { get { return GetTokenInt   (3).Value; } }
                public int    Class4    { get { return GetTokenInt   (4).Value; } }
                public double Kpsi      { get { return GetTokenDouble(5).Value; } }
                public double psi0      { get { return GetTokenDouble(6).Value; } }
                public override string ToString() { return line; }
                public static Improper FromLine(string line)
                {
                    if(line.StartsWith("improper "))
                    {
                        Improper improper = new Improper { line=line };
                        return improper;
                    }
                    return null;
                }
                public static Improper FromData(int Class1, int Class2, int Class3, int Class4, double Kpsi, double psi0)
                {
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "improper     10   10    1    1            20.00       0.00"
                    // "improper     49   49   40   40             3.00       0.00"
                    // "improper     10   13   25   34            90.00       0.00"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(10, 10, 1, 1, 20.00, 0.00);
                        var test_line = test.line;
                        var _____line = "improper     10   10    1    1            20.00       0.00";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "improper  ";
                    line += string.Format("{0,5}", Class1);
                    line += string.Format("{0,5}", Class2);
                    line += string.Format("{0,5}", Class3);
                    line += string.Format("{0,5}", Class4);
                    line += string.Format("{0,17:0.00}", Kpsi);
                    line += string.Format("{0,11:0.00}", psi0);
                    return FromLine(line);
                }
            }
            [Serializable]
            public class Torsion : Element
            {
                ///       ############################
                ///       ##                        ##
                ///       ##  Torsional Parameters  ##
                ///       ##                        ##
                ///       ############################
                /// 
                ///    ##############################################################
                ///    ##                                                          ##
                ///    ##  Below are the wildcard CHARMM torsional parameters:     ##
                ///    ##                                                          ##
                ///    ##  torsion      X   10   16    X          0.000  180.0  6  ##
                ///    ##  torsion      X   10   33    X          2.250  180.0  2  ##
                ///    ##  torsion      X   11   14    X          0.000    0.0  6  ##
                ///    ##  torsion      X   11   15    X          0.000    0.0  6  ##
                ///    ##  torsion      X   12   13    X          0.050  180.0  6  ##
                ///    ##  torsion      X   12   14    X          0.050  180.0  6  ##
                ///    ##  torsion      X   12   15    X          0.050  180.0  6  ##
                ///    ##  torsion      X   12   16    X          0.000  180.0  6  ##
                ///    ##  torsion      X   13   13    X          0.200    0.0  3  ##
                ///    ##  torsion      X   13   14    X          0.200    0.0  3  ##
                ///    ##  torsion      X   13   15    X          0.200    0.0  3  ##
                ///    ##  torsion      X   13   26    X          0.100    0.0  3  ##
                ///    ##  torsion      X   13   35    X          0.140    0.0  3  ##
                ///    ##  torsion      X   14   14    X          0.195    0.0  3  ##
                ///    ##  torsion      X   14   15    X          0.160    0.0  3  ##
                ///    ##  torsion      X   14   26    X          0.100    0.0  3  ##
                ///    ##  torsion      X   14   33    X          0.000  180.0  6  ##
                ///    ##  torsion      X   14   35    X          0.140    0.0  3  ##
                ///    ##  torsion      X   15   15    X          0.155    0.0  3  ##
                ///    ##  torsion      X   15   25    X          0.110    0.0  3  ##
                ///    ##  torsion      X   15   26    X          0.090    0.0  3  ##
                ///    ##  torsion      X   15   33    X          0.000  180.0  6  ##
                ///    ##  torsion      X   15   35    X          0.140    0.0  3  ##
                ///    ##  torsion      X   16   17    X          0.140    0.0  3  ##
                ///    ##  torsion      X   17   17    X          0.160    0.0  3  ##
                ///    ##  torsion      X   17   18    X          0.140    0.0  3  ##
                ///    ##  torsion      X   22   22    X          0.000  180.0  2  ##
                ///    ##                                                          ##
                ///    ##############################################################
                /// 
                ///          class1 class2 class3 class4    { Kchi   delta  n }  { Kchi   delta  n } ...
                /// torsion      24   10   13    4
                /// torsion      24   10   13   13
                /// ...
                /// torsion      24   10   13   24            0.600    0.0  1
                /// torsion      24   10   13   26            0.600    0.0  1
                /// ...
                /// torsion      24   10   16    4            0.400  180.0  1      0.600    0.0  2
                /// torsion      24   10   16   17            0.400    0.0  1      0.600    0.0  2
                /// ...
                /// torsion      13   13   35    3         1.330 0.0 1   0.180 0.0 2   0.320 0.0 3
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom         63   24    NH1   "Peptide Nitrogen"             7    14.007    3
                ///     atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                ///     atom         23   13    CT1   "Peptide Alpha Carbon"         6    12.011    4
                ///     atom          4    4    HB    "Peptide HCA"                  1     1.008    1
                ///     atom         63   24    NH1   "Peptide Nitrogen"             7    14.007    3
                ///     atom         65   26    NH3   "Ammonium Nitrogen"            7    14.007    4
                ///     atom         33   16    CP1   "N-Terminal PRO CA"            6    12.011    4
                ///     atom          4    4    HB    "Peptide HCA"                  1     1.008    1
                ///     atom         31   17    CP2   "PRO CB and CG"                6    12.011    4
                ///     atom         23   13    CT1   "Peptide Alpha Carbon"         6    12.011    4
                ///     atom         76   35    OH1   "Hydroxyl Oxygen"              8    15.999    2
                ///     atom          3    3    H     "Peptide Amide HN"             1     1.008    1
                /// charmm
                ///     !atom types             Kchi    n   delta
                ///     NH1  C    CT1  HB       0.0000  1     0.00 !   ALLOW PEP
                ///     NH1  C    CT1  CT1      0.0000  1     0.00 !   ALLOW PEP
                ///     
                ///     NH1  C    CT1  NH1      0.6000  1     0.00 !   ALLOW PEP
                ///     NH3  CT1  C    NH1      0.6000  1     0.00 ! ALLOW PEP PRO
                ///     
                ///     NH1  C    CP1  HB       0.4000  1   180.00 ! ALLOW PEP PRO
                ///     NH1  C    CP1  HB       0.6000  2     0.00 ! ALLOW PEP PRO
                ///     NH1  C    CP1  CP2      0.4000  1     0.00 ! ALLOW PEP PRO
                ///     NH1  C    CP1  CP2      0.6000  2     0.00 ! ALLOW PEP PRO
                ///     
                ///     H    OH1  CT1  CT1      1.3300  1     0.00 ! ALLOW ALC
                ///     H    OH1  CT1  CT1      0.1800  2     0.00 ! ALLOW ALC
                ///     H    OH1  CT1  CT1      0.3200  3     0.00 ! ALLOW ALC
                public int     Class1    { get { return GetTokenInt   ( 1).Value; } }
                public int     Class2    { get { return GetTokenInt   ( 2).Value; } }
                public int     Class3    { get { return GetTokenInt   ( 3).Value; } }
                public int     Class4    { get { return GetTokenInt   ( 4).Value; } }
                public double? Kchi0     { get { return GetTokenDouble( 5); } }
                public double? delta0    { get { return GetTokenDouble( 6); } }
                public double? n0        { get { return GetTokenDouble( 7); } }
                public double? Kchi1     { get { return GetTokenDouble( 8); } }
                public double? delta1    { get { return GetTokenDouble( 9); } }
                public double? n1        { get { return GetTokenDouble(10); } }
                public double? Kchi2     { get { return GetTokenDouble(11); } }
                public double? delta2    { get { return GetTokenDouble(12); } }
                public double? n2        { get { return GetTokenDouble(13); } }
                public override string ToString() { return line; }
                public static Torsion FromLine(string line)
                {
                    if(line.StartsWith("torsion "))
                    {
                        Torsion torsion = new Torsion { line=line };
                        return torsion;
                    }
                    return null;
                }
                public static Torsion FromData
                    ( int Class1, int Class2, int Class3, int Class4
                    , double? Kchi0, double? delta0, double? n0
                    , double? Kchi1, double? delta1, double? n1
                    , double? Kchi2, double? delta2, double? n2
                    )
                {
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "torsion      24   10   13    4"
                    // "torsion      24   10   13   13"
                    // "torsion      24   10   13   24            0.600    0.0  1"
                    // "torsion      24   10   13   26            0.600    0.0  1"
                    // "torsion      24   10   16    4            0.400  180.0  1      0.600    0.0  2"
                    // "torsion      24   10   16   17            0.400    0.0  1      0.600    0.0  2"
                    // "torsion      76   72   72   77            3.300  180.0  1     -0.400  180.0  3"
                    // "torsion      13   13   35    3         1.330 0.0 1   0.180 0.0 2   0.320 0.0 3"
                    // "torsion      72   77   80   77     1.200 180.0 1  0.100 180.0 2  0.100 180.0 3"
                    // "torsion      72   77   80   77     1.200 180.0 1  0.100 180.0 2  0.100 180.0 3"
                    // "torsion      72   72   72   73     0.100 180.0 2   0.150 0.0 4   0.100 180.0 6"
                    // "torsion      72   72   72   73     0.100 180.0 2   0.150 0.0 4   0.100 180.0 6"
                    if(HDebug.Selftest())
                    {
                        {
                            var test = FromData(24, 10, 13, 4, null, null, null, null, null, null, null, null, null);
                            var test_line = test.line;
                            var _____line = "torsion      24   10   13    4";
                            HDebug.Assert(test_line == _____line);
                        }
                        {
                            var test = FromData(24, 10, 13, 24, 0.600, 0.0, 1, null, null, null, null, null, null);
                            var test_line = test.line;
                            var _____line = "torsion      24   10   13   24            0.600    0.0  1";
                            HDebug.Assert(test_line == _____line);
                        }
                        {
                            var test = FromData(76, 72, 72, 77, 3.300, 180.0, 1, -0.400, 180.0, 3, null, null, null);
                            var test_line = test.line;
                            var _____line = "torsion      76   72   72   77            3.300  180.0  1     -0.400  180.0  3";
                            HDebug.Assert(test_line == _____line);
                        }
                        {
                            var test = FromData(72, 72, 72, 73, 0.100, 180.0, 2, 0.150, 0.0, 4, 0.100, 180.0, 6);
                            var test_line = test.line;
                            var _____line = "torsion      72   72   72   73     0.100 180.0 2   0.150 0.0 4   0.100 180.0 6";
                                _____line = "torsion      72   72   72   73     0.100 180.0 2  0.150   0.0 4  0.100 180.0 6";
                            HDebug.Assert(test_line == _____line);
                        }
                    }
                    string line = "torsion   ";
                    line += string.Format("{0,5}", Class1);
                    line += string.Format("{0,5}", Class2);
                    line += string.Format("{0,5}", Class3);
                    line += string.Format("{0,5}", Class4);
                    if(Kchi0 != null || delta0 != null || n0 != null)
                    {
                        HDebug.Exception(Kchi0 != null, delta0 != null, n0 != null);
                        line += string.Format("{0,17:0.000}", Kchi0);
                        line += string.Format("{0,7:0.0}", delta0);
                        line += string.Format("{0,3}", n0);
                    }
                    if(Kchi1 != null || delta1 != null || n1 != null)
                    {
                        HDebug.Exception(Kchi0 != null, delta0 != null, n0 != null);
                        HDebug.Exception(Kchi1 != null, delta1 != null, n1 != null);
                        line += string.Format("{0,11:0.000}", Kchi1);
                        line += string.Format("{0,7:0.0}", delta1);
                        line += string.Format("{0,3}", n1);
                    }
                    if(Kchi2 != null || delta2 != null || n2 != null)
                    {
                        HDebug.Exception(Kchi0 != null, delta0 != null, n0 != null);
                        HDebug.Exception(Kchi1 != null, delta1 != null, n1 != null);
                        HDebug.Exception(Kchi2 != null, delta2 != null, n2 != null);
                        //"torsion      72   72   72   73     0.100 180.0 2   0.150 0.0 4   0.100 180.0 6";
                        //"torsion      72   72   72   73     0.100 180.0 2  0.100 180.0 2  0.100 180.0 2";
                        line = "torsion   ";
                        line += string.Format("{0,5}", Class1);
                        line += string.Format("{0,5}", Class2);
                        line += string.Format("{0,5}", Class3);
                        line += string.Format("{0,5}", Class4);
                        line += "   ";
                        line += string.Format("{0,7:0.000}",  Kchi0);
                        line += string.Format("{0,6:0.0}" , delta0);
                        line += string.Format("{0,2}"     ,     n0);
                        line += string.Format("{0,7:0.000}",  Kchi1);
                        line += string.Format("{0,6:0.0}" , delta1);
                        line += string.Format("{0,2}"     ,     n1);
                        line += string.Format("{0,7:0.000}",  Kchi2);
                        line += string.Format("{0,6:0.0}" , delta2);
                        line += string.Format("{0,2}"     ,     n2);
                    }
                    return FromLine(line);
                }
                [Serializable]
                public class Data { public double Kchi, n, delta; }
                public Data[] GetListData()
                {
                    List<Data> list = new List<Data>();
                    if(Kchi0 != null) list.Add(new Data { Kchi=Kchi0.Value, n=n0.Value, delta=delta0.Value });
                    if(Kchi1 != null) list.Add(new Data { Kchi=Kchi1.Value, n=n1.Value, delta=delta1.Value });
                    if(Kchi2 != null) list.Add(new Data { Kchi=Kchi2.Value, n=n2.Value, delta=delta2.Value });
                    return list.ToArray();
                }
            }
            [Serializable]
            public class Charge : Element
            {
                ///       ########################################
                ///       ##                                    ##
                ///       ##  Atomic Partial Charge Parameters  ##
                ///       ##                                    ##
                ///       ########################################
                /// 
                ///              id               pch
                /// charge        1               0.0900
                /// charge        2               0.1150
                /// ...
                ///     charge       63              -0.4700
                ///     charge        3               0.3100
                ///     charge       23               0.0700
                ///     charge        4               0.0900
                ///     charge       27              -0.2700
                ///     charge        1               0.0900
                ///     charge        1               0.0900
                ///     charge        1               0.0900
                ///     charge       20               0.5100
                ///     charge       74              -0.5100
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom         63   24    NH1   "Peptide Nitrogen"             7    14.007    3
                ///     atom          3    3    H     "Peptide Amide HN"             1     1.008    1
                ///     atom         23   13    CT1   "Peptide Alpha Carbon"         6    12.011    4
                ///     atom          4    4    HB    "Peptide HCA"                  1     1.008    1
                ///     atom         27   15    CT3   "Methyl Carbon"                6    12.011    4
                ///     atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                ///     atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                ///     atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                ///     atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                ///     atom         74   34    O     "Peptide Oxygen"               8    15.999    1
                /// charmm
                ///     !    name type
                ///     RESI ALA          0.00
                ///     ATOM N    NH1    -0.47  !     |
                ///     ATOM HN   H       0.31  !  HN-N
                ///     ATOM CA   CT1     0.07  !     |     HB1
                ///     ATOM HA   HB      0.09  !     |    /
                ///     ATOM CB   CT3    -0.27  !  HA-CA--CB-HB2
                ///     ATOM HB1  HA      0.09  !     |    \
                ///     ATOM HB2  HA      0.09  !     |     HB3
                ///     ATOM HB3  HA      0.09  !   O=C
                ///     ATOM C    C       0.51  !     |
                ///     ATOM O    O      -0.51  !
                /// ...
                public int    Id    { get { return GetTokenInt   (1).Value; } }
                public double pch   { get { return GetTokenDouble(2).Value; } }
                public override string ToString() { return line; }
                public static Charge FromLine(string line)
                {
                    if(line.StartsWith("charge "))
                    {
                        Charge charge = new Charge { line=line };
                        return charge;
                    }
                    return null;
                }
                public static Charge FromData(int Id, double pch)
                {   //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "charge       63              -0.4700"
                    // "charge        3               0.3100"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(63, -0.4700);
                        var test_line = test.line;
                        var _____line = "charge       63              -0.4700";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "charge ";
                    line += string.Format("{0,8}", Id);
                    line += "          ";
                    line += string.Format("{0,11:0.0000}", pch);
                    return FromLine(line);
                }
            }
            [Serializable]
            public class Biotype : Element
            {
                ///       ########################################
                ///       ##                                    ##
                ///       ##  Biopolymer Atom Type Conversions  ##
                ///       ##                                    ##
                ///       ########################################
                /// 
                ///            bioid   name                                       id
                /// biotype       1    N       "Glycine"                          63
                /// ...
                /// biotype       7    N       "Alanine"                          63
                /// biotype       8    CA      "Alanine"                          23
                /// biotype       9    C       "Alanine"                          20
                /// biotype      10    HN      "Alanine"                           3
                /// biotype      11    O       "Alanine"                          74
                /// biotype      12    HA      "Alanine"                           4
                /// biotype      13    CB      "Alanine"                          27
                /// biotype      14    HB      "Alanine"                           1
                /// ...
                /// 
                /// atom             id   class type  description                         mass
                ///     atom         63   24    NH1   "Peptide Nitrogen"             7    14.007    3
                ///     atom         23   13    CT1   "Peptide Alpha Carbon"         6    12.011    4
                ///     atom         20   10    C     "Peptide Carbonyl"             6    12.011    3
                ///     atom          3    3    H     "Peptide Amide HN"             1     1.008    1
                ///     atom         74   34    O     "Peptide Oxygen"               8    15.999    1
                ///     atom          4    4    HB    "Peptide HCA"                  1     1.008    1
                ///     atom         27   15    CT3   "Methyl Carbon"                6    12.011    4
                ///     atom          1    1    HA    "Nonpolar Hydrogen"            1     1.008    1
                /// charmm
                ///     !    name type
                ///     RESI ALA          0.00
                ///     ATOM N    NH1    -0.47  !     |
                ///     ATOM HN   H       0.31  !  HN-N
                ///     ATOM CA   CT1     0.07  !     |     HB1
                ///     ATOM HA   HB      0.09  !     |    /
                ///     ATOM CB   CT3    -0.27  !  HA-CA--CB-HB2
                ///     ATOM HB1  HA      0.09  !     |    \
                ///     ATOM HB2  HA      0.09  !     |     HB3
                ///     ATOM HB3  HA      0.09  !   O=C
                ///     ATOM C    C       0.51  !     |
                ///     ATOM O    O      -0.51  !
                public int    BioId { get { return GetTokenInt   (1).Value; } }
                public string Name  { get { return GetTokenString(2)      ; } }
                public string Resn  { get { return GetTokenString(3)      ; } }
                public int    Id    { get { return GetTokenInt   (4).Value; } }
                public override string ToString() { return line; }
                public static Biotype FromLine(string line)
                {
                    if(line.StartsWith("biotype "))
                    {
                        Biotype biotype = new Biotype { line=line };
                        return biotype;
                    }
                    return null;
                }
                public static Biotype FromData(int BioId, string Name, string Resn, int Id)
                {
                    //  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    // "biotype       1    N       "Glycine"                          63"
                    // "biotype       8    CA      "Alanine"                          23"
                    // "biotype      23    CG1     "Valine"                           27"
                    if(HDebug.Selftest())
                    {
                        var test = FromData(23, "CG1", "Valine", 27);
                        var test_line = test.line;
                        var _____line = "biotype      23    CG1     \"Valine\"                           27";
                        HDebug.Assert(test_line == _____line);
                    }
                    string line = "biotype";
                    line += string.Format("{0,8}", BioId);
                    line += "    ";
                    line += string.Format("{0,-8}", Name);
                    line += ("\""+Resn.Trim()+"\"                                                  ").Substring(0, 27);
                    line += string.Format("{0,10}", Id);
                    return FromLine(line);
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public static Prm charmm22
            {
                get
                {
                    string[] charmm22_lines = GetResourceLines("charmm22.prm");
                    Prm prm = Prm.FromLines(charmm22_lines);
                    return prm;
                }
            }
        }
    }
}
