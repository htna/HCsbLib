using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
    {
        [Serializable]
        public partial class Atom : IAtom
        {
            /// http://www.wwpdb.org/documentation/format32/sect9.html
            ///
            /// COLUMNS        DATA  TYPE    FIELD        DEFINITION
            /// -------------------------------------------------------------------------------------
            ///  1 -  6        Record name   "ATOM  "
            ///  7 - 11        Integer       serial       Atom  serial number.
            /// 13 - 16        Atom          name         Atom name.
            /// 17             Character     altLoc       Alternate location indicator.
            /// 18 - 20        Residue name  resName      Residue name.
            /// 22             Character     chainID      Chain identifier.
            /// 23 - 26        Integer       resSeq       Residue sequence number.
            /// 27             AChar         iCode        Code for insertion of residues.
            /// 31 - 38        Real(8.3)     x            Orthogonal coordinates for X in Angstroms.
            /// 39 - 46        Real(8.3)     y            Orthogonal coordinates for Y in Angstroms.
            /// 47 - 54        Real(8.3)     z            Orthogonal coordinates for Z in Angstroms.
            /// 55 - 60        Real(6.2)     occupancy    Occupancy.
            /// 61 - 66        Real(6.2)     tempFactor   Temperature  factor.
            /// 77 - 78        LString(2)    element      Element symbol, right-justified.
            /// 79 - 80        LString(2)    charge       Charge  on the atom.
            /// 
            /// 67 - 76        etc_67_76
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// ATOM     32  N  AARG A  -3      11.281  86.699  94.383  0.50 35.88           N  
            /// ATOM     33  N  BARG A  -3      11.296  86.721  94.521  0.50 35.60           N
            /// ATOM     34  CA AARG A  -3      12.353  85.696  94.456  0.50 36.67           C
            /// ATOM     35  CA BARG A  -3      12.333  85.862  95.041  0.50 36.42           C
            /// ATOM     36  C  AARG A  -3      13.559  86.257  95.222  0.50 37.37           C
            /// ATOM     37  C  BARG A  -3      12.759  86.530  96.365  0.50 36.39           C
            /// ATOM     38  O  AARG A  -3      13.753  87.471  95.270  0.50 37.74           O
            /// ATOM     39  O  BARG A  -3      12.924  87.757  96.420  0.50 37.26           O


            public static string RecordName { get { return "ATOM  "; } }

            Atom(string line)
                : base(line)
            {
            }
            public static Atom FromString(string line)
            {
                HDebug.Assert(IsAtom(line));
                return new Atom(line);
            }
            public static Atom FromData(int serial, string name, string resName, char chainID, int resSeq, double x, double y, double z, char? altLoc=null, char? iCode=null, double? occupancy=null, double? tempFactor=null, string element=null, string charge=null, string segment=null)
            {
                Atom atom = Atom.FromString(LineFromData(RecordName, serial, name, resName, chainID, resSeq, x, y, z
                    , altLoc    : altLoc
                    , iCode     : iCode
                    , occupancy : occupancy
                    , tempFactor: tempFactor
                    , element   : element
                    , charge    : charge
                    , segment   : segment
                    ));
                return atom;
            }
            public static Atom FromHetatm(Hetatm hetatm)
            {
                string header = hetatm.line.Substring(0, 6);
                string data   = hetatm.line.Substring(6);
                HDebug.Assert(header.ToUpper() == Hetatm.RecordName.ToUpper());
                HDebug.Assert(header+data == hetatm.line);

                string atom_line = Atom.RecordName+data;
                Atom   atom = Atom.FromString(atom_line);
                return atom;
            }

            public static bool IsAtom(string line) { return (line.Substring(0, 6) == "ATOM  "); }

            static Dictionary<string, char> _tblResNameSyn = null;
            public  char? resNameSyn{ get { return GetResNameSyn(null); } }
            public  char? GetResNameSyn(Dictionary<string, char> tblResNameSyn) // tblResNameSyn [default] : null
            {
                if(tblResNameSyn == null) tblResNameSyn = _tblResNameSyn;
                if(tblResNameSyn == null)
                {
                    _tblResNameSyn = new Dictionary<string, char>();// http://www.bmsc.washington.edu/CrystaLinks/man/pdb/part_79.html
                                                                    // RESIDUE                     ABBREVIATION                SYNONYM
                                                                    // -----------------------------------------------------------------------------
                    _tblResNameSyn.Add("ALA", 'A');                 // Alanine                     ALA                         A
                    _tblResNameSyn.Add("ARG", 'R');                 // Arginine                    ARG                         R
                    _tblResNameSyn.Add("ASN", 'N');                 // Asparagine                  ASN                         N
                    _tblResNameSyn.Add("ASP", 'D');                 // Aspartic acid               ASP                         D
                    _tblResNameSyn.Add("ASX", 'B');                 // ASP/ASN ambiguous           ASX                         B
                    _tblResNameSyn.Add("CYS", 'C');                 // Cysteine                    CYS                         C
                    _tblResNameSyn.Add("GLN", 'Q');                 // Glutamine                   GLN                         Q
                    _tblResNameSyn.Add("GLU", 'E');                 // Glutamic acid               GLU                         E
                    _tblResNameSyn.Add("GLX", 'Z');                 // GLU/GLN ambiguous           GLX                         Z
                    _tblResNameSyn.Add("GLY", 'G');                 // Glycine                     GLY                         G
                    _tblResNameSyn.Add("HIS", 'H');                 // Histidine                   HIS                         H
                    _tblResNameSyn.Add("ILE", 'I');                 // Isoleucine                  ILE                         I
                    _tblResNameSyn.Add("LEU", 'L');                 // Leucine                     LEU                         L
                    _tblResNameSyn.Add("LYS", 'K');                 // Lysine                      LYS                         K
                    _tblResNameSyn.Add("MET", 'M');                 // Methionine                  MET                         M
                    _tblResNameSyn.Add("PHE", 'F');                 // Phenylalanine               PHE                         F
                    _tblResNameSyn.Add("PRO", 'P');                 // Proline                     PRO                         P
                    _tblResNameSyn.Add("SER", 'S');                 // Serine                      SER                         S
                    _tblResNameSyn.Add("THR", 'T');                 // Threonine                   THR                         T
                    _tblResNameSyn.Add("TRP", 'W');                 // Tryptophan                  TRP                         W
                    _tblResNameSyn.Add("TYR", 'Y');                 // Tyrosine                    TYR                         Y
                    _tblResNameSyn.Add("UNK", '?');                 // Unknown                     UNK
                    _tblResNameSyn.Add("VAL", 'V');                 // Valine                      VAL                         V
                    // pdbalias
                    _tblResNameSyn.Add("HSD", 'H'); // HIS -> HSD
                    _tblResNameSyn.Add("HSE", 'H'); // HIS -> HSE
                    _tblResNameSyn.Add("HSP", 'H'); // HIS -> HSP
                    tblResNameSyn = _tblResNameSyn;
                }
                HDebug.Assert(tblResNameSyn != null);
                string resname = resName.ToUpper();
                if(tblResNameSyn.ContainsKey(resname) == false)
                    return null;
                return tblResNameSyn[resname];
            }

            public override string ToString()
            {
                return base.ToString("atom");
            }

            ////////////////////////////////////////////////////////////////////////////////////
            // Serializable
            public Atom(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
    }
}
