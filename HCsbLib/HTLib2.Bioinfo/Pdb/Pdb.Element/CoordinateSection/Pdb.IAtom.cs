using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class PdbStatic
    {
        public static PdbStatic.ResInfo GetResInfo(this Pdb.IAtom atom)
        {
            return new PdbStatic.ResInfo
            {
                resName = atom.resName,
                chainID = atom.chainID,
                resSeq  = atom.resSeq ,
                iCode   = atom.iCode  ,
            };
        }
        public static double[] GetCoord(this Pdb.IAtom iatom)
        {
            return new double[] { iatom.x, iatom.y, iatom.z };
        }
        public static Dictionary<PdbStatic.ResInfo, ATOM[]> HGroupByResInfo<ATOM>(this IList<ATOM> atoms)
            where ATOM: Pdb.IAtom
        {
            Dictionary<PdbStatic.ResInfo, List<ATOM>> dict = new Dictionary<ResInfo, List<ATOM>>();
            foreach(var atom in atoms)
            {
                PdbStatic.ResInfo resinfo = atom.GetResInfo();
                if(dict.ContainsKey(resinfo) == false)
                    dict.Add(resinfo, new List<ATOM>());
                dict[resinfo].Add(atom);
            }
            return dict.HToArray();
        }
        public static Dictionary<string, ATOM[]> HGroupBySegment<ATOM>(this IList<ATOM> atoms)
            where ATOM: Pdb.IAtom
        {
            Dictionary<string, List<ATOM>> dict = new Dictionary<string, List<ATOM>>();
            foreach(var atom in atoms)
            {
                string segment = atom.segment;
                if(dict.ContainsKey(segment) == false)
                    dict.Add(segment, new List<ATOM>());
                dict[segment].Add(atom);
            }
            return dict.HToArray();
        }
        public static Dictionary<char, ATOM[]> HGroupByChain<ATOM>(this IList<ATOM> atoms)
            where ATOM: Pdb.IAtom
        {
            Dictionary<char, List<ATOM>> dict = new Dictionary<char, List<ATOM>>();
            foreach(var atom in atoms)
            {
                char chainID = atom.chainID;
                if(dict.ContainsKey(chainID) == false)
                    dict.Add(chainID, new List<ATOM>());
                dict[chainID].Add(atom);
            }
            return dict.HToArray();
        }
        public static Dictionary<int, ATOM[]> HGroupByResseq<ATOM>(this IList<ATOM> atoms)
            where ATOM: Pdb.IAtom
        {
            Dictionary<int, List<ATOM>> dict = new Dictionary<int, List<ATOM>>();
            foreach(var atom in atoms)
            {
                int resseq = atom.resSeq;
                if(dict.ContainsKey(resseq) == false)
                    dict.Add(resseq, new List<ATOM>());
                dict[resseq].Add(atom);
            }
            return dict.HToArray();
        }
        public static Dictionary<char, Dictionary<string, ATOM[]>> HGroupByChainSegment<ATOM>(this IList<ATOM> atoms)
            where ATOM: Pdb.IAtom
        {
            Dictionary<char, Dictionary<string, ATOM[]>> dict = new Dictionary<char, Dictionary<string, ATOM[]>>();
            foreach(var ch_atoms in atoms.HGroupByChain())
            {
                char chain = ch_atoms.Key;
                if(dict.ContainsKey(chain) == false)
                    dict.Add(chain, new Dictionary<string, ATOM[]>());
                var dict_chain = dict[chain];
                foreach(var seg_atoms in ch_atoms.Value.HGroupBySegment())
                {
                    string segment = seg_atoms.Key;
                    ATOM[] ch_seg_atoms = seg_atoms.Value;
                    dict_chain.Add(segment, ch_seg_atoms);
                }
            }
            return dict;
        }
        public static Dictionary<char, Dictionary<string, Dictionary<int, ATOM[]>>> HGroupByChainSegmentResseq<ATOM>(this IList<ATOM> atoms)
            where ATOM: Pdb.IAtom
        {
            Dictionary<char, Dictionary<string, Dictionary<int, ATOM[]>>> dict = new Dictionary<char, Dictionary<string, Dictionary<int, ATOM[]>>>();
            foreach(var ch_atoms in atoms.HGroupByChain())
            {
                char chain = ch_atoms.Key;
                if(dict.ContainsKey(chain) == false)
                    dict.Add(chain, new Dictionary<string, Dictionary<int, ATOM[]>>());
                var dict_chain = dict[chain];
                foreach(var seg_atoms in ch_atoms.Value.HGroupBySegment())
                {
                    string segment = seg_atoms.Key;
                    if(dict_chain.ContainsKey(segment) == false)
                        dict_chain.Add(segment, new Dictionary<int, ATOM[]>());
                    var dict_chain_segment = dict_chain[segment];
                    foreach(var resi_atoms in seg_atoms.Value.HGroupByResseq())
                    {
                        int resi = resi_atoms.Key;
                        ATOM[] ch_seg_resi_atoms = resi_atoms.Value;
                        dict_chain_segment.Add(resi, ch_seg_resi_atoms);
                    }
                }
            }
            return dict;
        }
        public static Dictionary<char, Dictionary<int, ATOM[]>> HGroupByChainResseq<ATOM>(this IList<ATOM> atoms)
            where ATOM: Pdb.IAtom
        {
            Dictionary<char, Dictionary<int, ATOM[]>> dict = new Dictionary<char, Dictionary<int, ATOM[]>>();
            foreach(var ch_atoms in atoms.HGroupByChain())
            {
                char chain = ch_atoms.Key;
                if(dict.ContainsKey(chain) == false)
                    dict.Add(chain, new Dictionary<int, ATOM[]>());
                var dict_chain = dict[chain];
                foreach(var resi_atoms in ch_atoms.Value.HGroupByResseq())
                {
                    int resi = resi_atoms.Key;
                    ATOM[] ch_resi_atoms = resi_atoms.Value;
                    dict_chain.Add(resi, ch_resi_atoms);
                }
            }
            return dict;
        }
    }
    public partial class Pdb
    {
        public abstract partial class IAtom : Element, IComparable<IAtom>
        {
            /// http://www.wwpdb.org/documentation/format32/sect9.html                                  | http://www.wwpdb.org/documentation/format32/sect9.html#HETATM
            ///                                                                                         | 
            /// COLUMNS        DATA  TYPE    FIELD        DEFINITION                                    | COLUMNS       DATA  TYPE     FIELD         DEFINITION
            /// -------------------------------------------------------------------------------------   | -----------------------------------------------------------------------
            ///  1 -  6        Record name   "ATOM  "                                                   |  1 - 6        Record name    "HETATM"
            ///  7 - 11        Integer       serial       Atom serial number.                           |  7 - 11       Integer        serial        Atom serial number.
            /// 13 - 16        Atom          name         Atom name.                                    | 13 - 16       Atom           name          Atom name.
            /// 17             Character     altLoc       Alternate location indicator.                 | 17            Character      altLoc        Alternate location indicator.
            /// 18 - 20        Residue name  resName      Residue name.                                 | 18 - 20       Residue name   resName       Residue name.
            /// 22             Character     chainID      Chain identifier.                             | 22            Character      chainID       Chain identifier.
            /// 23 - 26        Integer       resSeq       Residue sequence number.                      | 23 - 26       Integer        resSeq        Residue sequence number.
            /// 27             AChar         iCode        Code for insertion of residues.               | 27            AChar          iCode         Code for insertion of residues.
            /// 31 - 38        Real(8.3)     x            Orthogonal coordinates for X in Angstroms.    | 31 - 38       Real(8.3)      x             Orthogonal coordinates for X.
            /// 39 - 46        Real(8.3)     y            Orthogonal coordinates for Y in Angstroms.    | 39 - 46       Real(8.3)      y             Orthogonal coordinates for Y.
            /// 47 - 54        Real(8.3)     z            Orthogonal coordinates for Z in Angstroms.    | 47 - 54       Real(8.3)      z             Orthogonal coordinates for Z.
            /// 55 - 60        Real(6.2)     occupancy    Occupancy.                                    | 55 - 60       Real(6.2)      occupancy     Occupancy.
            /// 61 - 66        Real(6.2)     tempFactor   Temperature factor.                           | 61 - 66       Real(6.2)      tempFactor    Temperature factor.
            /// 73 - 76        LString(4)    segment      Segment identifier, left-justified.           | 73 - 76        LString(4)    segment      Segment identifier, left-justified. http://deposit.rcsb.org/adit/docs/pdb_atom_format.html
            /// 77 - 78        LString(2)    element      Element symbol, right-justified.              | 77 - 78       LString(2)     element       Element symbol; right-justified.
            /// 79 - 80        LString(2)    charge       Charge  on the atom.                          | 79 - 80       LString(2)     charge        Charge on the atom.
            ///                                                                                         | 
            /// 67 - 76        etc_67_76                                                                | 67 - 76       etc_67_76
            ///                                                                                         | 
            /// Example                                                                                 | Example
            ///          1         2         3         4         5         6         7         8        |          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890        | 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// ATOM     32  N  AARG A  -3      11.281  86.699  94.383  0.50 35.88           N          | HETATM 8237 MG    MG A1001      13.872  -2.555 -29.045  1.00 27.36          MG 
            /// ATOM     33  N  BARG A  -3      11.296  86.721  94.521  0.50 35.60           N          |  
            /// ATOM     34  CA AARG A  -3      12.353  85.696  94.456  0.50 36.67           C          | HETATM 3835 FE   HEM A   1      17.140   3.115  15.066  1.00 14.14          FE
            /// ATOM     35  CA BARG A  -3      12.333  85.862  95.041  0.50 36.42           C          | HETATM 8238  S   SO4 A2001      10.885 -15.746 -14.404  1.00 47.84           S  
            /// ATOM     36  C  AARG A  -3      13.559  86.257  95.222  0.50 37.37           C          | HETATM 8239  O1  SO4 A2001      11.191 -14.833 -15.531  1.00 50.12           O  
            /// ATOM     37  C  BARG A  -3      12.759  86.530  96.365  0.50 36.39           C          | HETATM 8240  O2  SO4 A2001       9.576 -16.338 -14.706  1.00 48.55           O  
            /// ATOM     38  O  AARG A  -3      13.753  87.471  95.270  0.50 37.74           O          | HETATM 8241  O3  SO4 A2001      11.995 -16.703 -14.431  1.00 49.88           O  
            /// ATOM     39  O  BARG A  -3      12.924  87.757  96.420  0.50 37.26           O          | HETATM 8242  O4  SO4 A2001      10.932 -15.073 -13.100  1.00 49.91           O  
            /// ATOM    145  N   VAL A  25      32.433  16.336  57.540  1.00 11.92      A1   N

            public bool hexserial = false;
            public IAtom(string line)
                : base(line)
            {
            }

            public static string StringFormat(string format, params object[] args)
            {
                string str = string.Format(format, args);
                return str;
            }
            public static string LineFromData(string RecordName, int serial, string name, string resName, char chainID, int resSeq, double x, double y, double z, char? altLoc=null, char? iCode=null, double? occupancy=null, double? tempFactor=null, string element=null, string charge=null, string segment=null)
            {
                HDebug.Assert(0 <= serial, serial <= 99999);    //  7 - 11        Integer       serial       Atom  serial number.       => 00000-99999
                HDebug.Assert(0 <= resSeq, resSeq <=  9999);    // 23 - 26        Integer       resSeq       Residue sequence number.   =>  0000- 9999
                string _serial    = StringFormat("            {0}", serial    ); _serial    = _serial    .HSubEndStringCount(   1+idxs_serial    [1]-idxs_serial    [0]);
                string _name      = StringFormat("{0}            ", name      ); _name      = _name      .Substring         (0, 1+idxs_name      [1]-idxs_name      [0]);
                string _altLoc    = StringFormat("{0}            ", altLoc    ); _altLoc    = _altLoc    .Substring         (0, 1+idxs_altLoc    [1]-idxs_altLoc    [0]);
                string _resName   = StringFormat("{0}            ", resName   ); _resName   = _resName   .Substring         (0, 1+idxs_resName   [1]-idxs_resName   [0]);
                string _chainID   = StringFormat("{0}            ", chainID   ); _chainID   = _chainID   .Substring         (0, 1+idxs_chainID   [1]-idxs_chainID   [0]);
                string _resSeq    = StringFormat("            {0}", resSeq    ); _resSeq    = _resSeq    .HSubEndStringCount(   1+idxs_resSeq    [1]-idxs_resSeq    [0]);
                string _iCode     = StringFormat("{0}            ", iCode     ); _iCode     = _iCode     .Substring         (0, 1+idxs_iCode     [1]-idxs_iCode     [0]);
                string _x         = StringFormat("      {0:0.000}", x         ); _x         = _x         .HSubEndStringCount(   1+idxs_x         [1]-idxs_x         [0]);
                string _y         = StringFormat("      {0:0.000}", y         ); _y         = _y         .HSubEndStringCount(   1+idxs_y         [1]-idxs_y         [0]);
                string _z         = StringFormat("      {0:0.000}", z         ); _z         = _z         .HSubEndStringCount(   1+idxs_z         [1]-idxs_z         [0]);
                string _occupancy = StringFormat("       {0:0.00}", occupancy ); _occupancy = _occupancy .HSubEndStringCount(   1+idxs_occupancy [1]-idxs_occupancy [0]);
                string _tempFactor= StringFormat("       {0:0.00}", tempFactor); _tempFactor= _tempFactor.HSubEndStringCount(   1+idxs_tempFactor[1]-idxs_tempFactor[0]);
                string _segment   = StringFormat("{0,4}"          , segment   ); _segment   = _segment   .Substring         (0, 1+idxs_segment   [1]-idxs_segment   [0]);
                string _element   = StringFormat("{0}            ", element   ); _element   = _element   .Substring         (0, 1+idxs_element   [1]-idxs_element   [0]);
                string _charge    = StringFormat("{0}            ", charge    ); _charge    = _charge    .Substring         (0, 1+idxs_charge    [1]-idxs_charge    [0]);
                HDebug.Exception(RecordName.Length == 6);
                char[] chars = "RECORD__________________________________________________________________________"
                               .Replace("RECORD", RecordName)
                               .ToArray();
                for(int i=0; i<_serial    .Length; i++) { int idx=idxs_serial    [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _serial    [i]; }
                for(int i=0; i<_name      .Length; i++) { int idx=idxs_name      [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _name      [i]; }
                for(int i=0; i<_altLoc    .Length; i++) { int idx=idxs_altLoc    [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _altLoc    [i]; }
                for(int i=0; i<_resName   .Length; i++) { int idx=idxs_resName   [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _resName   [i]; }
                for(int i=0; i<_chainID   .Length; i++) { int idx=idxs_chainID   [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _chainID   [i]; }
                for(int i=0; i<_resSeq    .Length; i++) { int idx=idxs_resSeq    [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _resSeq    [i]; }
                for(int i=0; i<_iCode     .Length; i++) { int idx=idxs_iCode     [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _iCode     [i]; }
                for(int i=0; i<_x         .Length; i++) { int idx=idxs_x         [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _x         [i]; }
                for(int i=0; i<_y         .Length; i++) { int idx=idxs_y         [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _y         [i]; }
                for(int i=0; i<_z         .Length; i++) { int idx=idxs_z         [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _z         [i]; }
                for(int i=0; i<_occupancy .Length; i++) { int idx=idxs_occupancy [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _occupancy [i]; }
                for(int i=0; i<_tempFactor.Length; i++) { int idx=idxs_tempFactor[0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _tempFactor[i]; }
                for(int i=0; i<_segment   .Length; i++) { int idx=idxs_segment   [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _segment   [i]; }
                for(int i=0; i<_element   .Length; i++) { int idx=idxs_element   [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _element   [i]; }
                for(int i=0; i<_charge    .Length; i++) { int idx=idxs_charge    [0]-1+i; HDebug.Assert(chars[idx] == '_'); chars[idx] = _charge    [i]; }
                string line = (new string(chars)).Replace('_', ' ');
                return line;
            }

            public int? Serial()
            {
                return Serial(hexserial);
            }
            public int? Serial(bool hexserial)
            {
                string substr = String(idxs_serial);
                int? seri;
                if(hexserial) seri = substr.HParseIntHex();
                else          seri = substr.HParseInt();
                return seri;
            }
            public    int serial    { get { return Serial (               ).Value; } } static readonly int[] idxs_serial     = new int[]{ 7,11}; //  7 - 11        Integer       serial       Atom  serial number.
            public string name      { get { return String (idxs_name      );       } } static readonly int[] idxs_name       = new int[]{13,16}; // 13 - 16        Atom          name         Atom name.
            public   char altLoc    { get { return Char   (idxs_altLoc    );       } } static readonly int[] idxs_altLoc     = new int[]{17,17}; // 17             Character     altLoc       Alternate location indicator.
            public string resName   { get { return String (idxs_resName   );       } } static readonly int[] idxs_resName    = new int[]{18,20}; // 18 - 20        Residue name  resName      Residue name.
            public   char chainID   { get { return Char   (idxs_chainID   );       } } static readonly int[] idxs_chainID    = new int[]{22,22}; // 22             Character     chainID      Chain identifier.
            public    int resSeq    { get { return Integer(idxs_resSeq    ).Value; } } static readonly int[] idxs_resSeq     = new int[]{23,26}; // 23 - 26        Integer       resSeq       Residue sequence number.
            public   char iCode     { get { return Char   (idxs_iCode     );       } } static readonly int[] idxs_iCode      = new int[]{27,27}; // 27             AChar         iCode        Code for insertion of residues.
            public double x         { get { return Double (idxs_x         ).Value; } } static readonly int[] idxs_x          = new int[]{31,38}; // 31 - 38        Real(8.3)     x            Orthogonal coordinates for X in Angstroms.
            public double y         { get { return Double (idxs_y         ).Value; } } static readonly int[] idxs_y          = new int[]{39,46}; // 39 - 46        Real(8.3)     y            Orthogonal coordinates for Y in Angstroms.
            public double z         { get { return Double (idxs_z         ).Value; } } static readonly int[] idxs_z          = new int[]{47,54}; // 47 - 54        Real(8.3)     z            Orthogonal coordinates for Z in Angstroms.
            public double occupancy { get { return Double (idxs_occupancy ).Value; } } static readonly int[] idxs_occupancy  = new int[]{55,60}; // 55 - 60        Real(6.2)     occupancy    Occupancy.
            public double tempFactor{ get { return Double (idxs_tempFactor).Value; } } static readonly int[] idxs_tempFactor = new int[]{61,66}; // 61 - 66        Real(6.2)     tempFactor   Temperature  factor.
            public string segment   { get { return String (idxs_segment   );       } } static readonly int[] idxs_segment    = new int[]{73,76}; // 73 - 76        LString(4)    segment      Segment identifier, left-justified.  
            public string element   { get { return String (idxs_element   );       } } static readonly int[] idxs_element    = new int[]{77,78}; // 77 - 78        LString(2)    element      Element symbol, right-justified.
            public string charge    { get { return String (idxs_charge    );       } } static readonly int[] idxs_charge     = new int[]{79,80}; // 79 - 80        LString(2)    charge       Charge  on the atom.
            public string etc_67_76 { get { return String (idxs_etc       );       } } static readonly int[] idxs_etc        = new int[]{67,76}; // 67 - 76        etc

            public    int? try_serial { get { return Integer(idxs_serial    ); } }

            public double[] coord   { get { return new double[]{ x, y, z }; } }
            public bool IsHydrogen() { string lname = name.Trim(); return ((lname[0] == 'H') || (lname[0] == 'h')); }

            // IComparable<Atom>
            //int IComparable<Atom>.CompareTo(Atom other)
            //{
            //    return serial.CompareTo(other.serial);
            //}
            int IComparable<IAtom>.CompareTo(IAtom other)
            {
                int cmp = resSeq.CompareTo(other.resSeq);
                if(cmp != 0) return cmp;
                return serial.CompareTo(other.serial);
            }

            public string GetUpdatedLine(Vector xyz)
            {
                HDebug.Assert(xyz.Size == 3);
                return GetUpdatedLine(xyz[0], xyz[1], xyz[2]);
            }
            public string GetUpdatedLine(double x, double y, double z)
            {
                string strx = string.Format("     {0,7:0.000}", x); strx = strx.Substring(strx.Length-8, 8);
                string stry = string.Format("     {0,7:0.000}", y); stry = stry.Substring(stry.Length-8, 8);
                string strz = string.Format("     {0,7:0.000}", z); strz = strz.Substring(strz.Length-8, 8);
                char[] line = this.line.ToArray();
                for(int i=0; i<(idxs_x[1]-idxs_x[0]+1); i++) line[i+idxs_x[0]-1] = strx[i];
                for(int i=0; i<(idxs_y[1]-idxs_y[0]+1); i++) line[i+idxs_y[0]-1] = stry[i];
                for(int i=0; i<(idxs_z[1]-idxs_z[0]+1); i++) line[i+idxs_z[0]-1] = strz[i];
                string result = new string(line);
                return result;
            }
            public string GetUpdatedLineResSeq(int resSeq)
            {
                string strResSeq = string.Format("{0,4}", resSeq);
                char[] line = this.line.ToArray();
                for(int i=0; i<(idxs_resSeq[1]-idxs_resSeq[0]+1); i++) line[i+idxs_resSeq[0]-1] = strResSeq[i];
                string result = new string(line);
                return result;
            }
            public string GetUpdatedLineTempFactor(double tempFactor)
            {
                char[] line = this.line.ToArray();
                string str = string.Format("        {0,5:0.00}", tempFactor); str = str.Substring(str.Length-6, 6);
                for(int i=0; i<(idxs_tempFactor[1]-idxs_tempFactor[0]+1); i++) line[i+idxs_tempFactor[0]-1] = str[i];
                return new string(line);
            }
            public string GetUpdatedLineName(string name)
            {
                char[] line = this.line.ToArray();
                if     (name.Length == 3) name = " "+name;
                else if(name.Length == 4) name =     name;
                else HDebug.Assert(false);
                
                for(int i=0; i<(idxs_name[1]-idxs_name[0]+1); i++) line[i+idxs_name[0]-1] = name[i];
                string newline = new string(line);
                return newline;
            }
            public string GetUpdatedLineResName(string name)
            {
                char[] line = this.line.ToArray();
                HDebug.Assert(name.Length == 3);
                for(int i=0; i<(idxs_resName[1]-idxs_resName[0]+1); i++) line[i+idxs_resName[0]-1] = name[i];
                string newline = new string(line);
                return newline;
            }
            public string GetUpdatedLineChainID(char chainID)
            {
                HDebug.Exception(idxs_chainID[0] == idxs_chainID[1]);
                char[] line = this.line.ToArray();
                line[idxs_chainID[0]-1] = chainID;
                string newline = new string(line);
                return newline;
            }

            public string ToString(string RecordName)
            {
                string header = RecordName.Trim().ToLower();
                string str  = header;
                {   // str += string.Format(" {0:D}", serial);
                    if(Serial() != null) str += string.Format(" {0:D}", serial);
                    else                 str += " "+String(idxs_serial);
                }
                str += string.Format(", {0}, {1} {2:d}", name.Trim(), resName.Trim(), resSeq);
                str += string.Format(", {0}", chainID);
                //str += string.Format(", pos({0:G4},{1:G4},{2:G4})", x, y, z);
                //str += string.Format(", pos({0:0.000},{1:0.000},{2:0.000})", x, y, z);
                str += string.Format(", pos({0},{1},{2})", x, y, z);
                str += string.Format(", alt({0})"  , altLoc );
                str += string.Format(", segm({0})", segment);
                return str;
            }
            ////////////////////////////////////////////////////////////////////////////////////
            // Serializable
            public IAtom(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
        }
    }
}
