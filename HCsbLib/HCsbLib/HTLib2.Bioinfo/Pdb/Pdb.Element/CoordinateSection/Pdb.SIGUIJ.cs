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
        public class Siguij : Element, IComparable<Siguij>, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format23/sect9.html#SIGUIJ
			///
            /// SIGUIJ
            /// 
            /// The SIGUIJ records present the standard deviations of anisotropic temperature factors scaled by a factor of 10**4 (Angstroms**2). 
            /// 
            /// COLUMNS      DATA TYPE        FIELD         DEFINITION
            /// ------------------------------------------------------------
            ///  1 - 6       Record name      "SIGUIJ"
            ///  7 - 11      Integer          serial        Atom serial number.
            /// 13 - 16      Atom             name          Atom name.
            /// 17           Character        altLoc        Alternate location indicator.
            /// 18 - 20      Residue name     resName       Residue name.
            /// 22           Character        chainID       Chain identifier.
            /// 23 - 26      Integer          resSeq        Residue sequence number.
            /// 27           AChar            iCode         Insertion code.
            /// 29 - 35      Integer          sig[1][1]     Sigma U(1,1)
            /// 36 - 42      Integer          sig[2][2]     Sigma U(2,2)
            /// 43 - 49      Integer          sig[3][3]     Sigma U(3,3)
            /// 50 - 56      Integer          sig[1][2]     Sigma U(1,2)
            /// 57 - 63      Integer          sig[1][3]     Sigma U(1,3)
            /// 64 - 70      Integer          sig[2][3]     Sigma U(2,3)
            /// 77 - 78      LString(2)       element       Element symbol, right-justified.
            /// 79 - 80      LString(2)       charge        Charge on the atom.
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// ATOM    107  N   GLY    13      12.681  37.302 -25.211 1.000 15.56           N
            /// ANISOU  107  N   GLY    13     2406   1892   1614    198    519   -328       N
            /// SIGUIJ  107  N   GLY    13       10     10     10     10    10      10       N
            /// ATOM    108  CA  GLY    13      11.982  37.996 -26.241 1.000 16.92           C
            /// ANISOU  108  CA  GLY    13     2748   2004   1679    -21    155   -419       C
            /// SIGUIJ  108  CA  GLY    13       10     10     10     10    10      10       C
            /// ATOM    109  C   GLY    13      11.678  39.447 -26.008 1.000 15.73           C
            /// ANISOU  109  C   GLY    13     2555   1955   1468     87    357   -109       C
            /// SIGUIJ  109  C   GLY    13       10     10     10     10    10      10       C
            /// ATOM    110  O   GLY    13      11.444  40.201 -26.971 1.000 20.93           O
            /// ANISOU  110  O   GLY    13     3837   2505   1611    164   -121    189       O
            /// SIGUIJ  110  O   GLY    13       10     10     10     10    10      10       O
            /// ATOM    111  N   ASN    14      11.608  39.863 -24.755 1.000 13.68           N
            /// ANISOU  111  N   ASN    14     2059   1674   1462     27    244    -96       N
            /// SIGUIJ  111  N   ASN    14       10     10     10     10    10      10       N
            /// 
			public static string RecordName { get { return "SIGUIJ"; } }

            Siguij(string line)
                : base(line)
			{
			}
			public static Siguij FromString(string line)
			{
				HDebug.Assert(IsSiguij(line));
				return new Siguij(line);
			}
            public static bool IsSiguij(string line) { return (line.Substring(0, 6) == "SIGUIJ"); }
            public    int serial    { get { return Integer(idxs_serial    ).Value; } } static readonly int[] idxs_serial     = new int[]{ 7,11}; //  7 - 11      Integer          serial        Atom serial number.
            public string name      { get { return String (idxs_name      );       } } static readonly int[] idxs_name       = new int[]{13,16}; // 13 - 16      Atom             name          Atom name.
            public   char altLoc    { get { return Char   (idxs_altLoc    );       } } static readonly int[] idxs_altLoc     = new int[]{17,17}; // 17           Character        altLoc        Alternate location indicator.
            public string resName   { get { return String (idxs_resName   );       } } static readonly int[] idxs_resName    = new int[]{18,20}; // 18 - 20      Residue name     resName       Residue name.
            public   char chainID   { get { return Char   (idxs_chainID   );       } } static readonly int[] idxs_chainID    = new int[]{22,22}; // 22           Character        chainID       Chain identifier.
            public    int resSeq    { get { return Integer(idxs_resSeq    ).Value; } } static readonly int[] idxs_resSeq     = new int[]{23,26}; // 23 - 26      Integer          resSeq        Residue sequence number.
            public   char iCode     { get { return Char   (idxs_iCode     );       } } static readonly int[] idxs_iCode      = new int[]{27,27}; // 27           AChar            iCode         Insertion code.
            public    int sig11     { get { return Integer(idxs_sig11     ).Value; } } static readonly int[] idxs_sig11      = new int[]{29,35}; // 29 - 35      Integer          sig[1][1]     Sigma U(1,1)
            public    int sig22     { get { return Integer(idxs_sig22     ).Value; } } static readonly int[] idxs_sig22      = new int[]{36,42}; // 36 - 42      Integer          sig[2][2]     Sigma U(2,2)
            public    int sig33     { get { return Integer(idxs_sig33     ).Value; } } static readonly int[] idxs_sig33      = new int[]{43,49}; // 43 - 49      Integer          sig[3][3]     Sigma U(3,3)
            public    int sig12     { get { return Integer(idxs_sig12     ).Value; } } static readonly int[] idxs_sig12      = new int[]{50,56}; // 50 - 56      Integer          sig[1][2]     Sigma U(1,2)
            public    int sig13     { get { return Integer(idxs_sig13     ).Value; } } static readonly int[] idxs_sig13      = new int[]{57,63}; // 57 - 63      Integer          sig[1][3]     Sigma U(1,3)
            public    int sig23     { get { return Integer(idxs_sig23     ).Value; } } static readonly int[] idxs_sig23      = new int[]{64,70}; // 64 - 70      Integer          sig[2][3]     Sigma U(2,3)
            public string element   { get { return String (idxs_element   );       } } static readonly int[] idxs_element    = new int[]{77,78}; // 77 - 78      LString(2)       element       Element symbol, right-justified.
            public string charge    { get { return String (idxs_charge    );       } } static readonly int[] idxs_charge     = new int[]{79,80}; // 79 - 80      LString(2)       charge        Charge on the atom.

            public double[,] SigmaU  { get { return new double[3,3]{{sig11,sig12,sig13}
                                                                   ,{sig12,sig22,sig23}
                                                                   ,{sig13,sig23,sig33}}; } }
            public bool IsHydrogen() { return ((name[0] == 'H') || (name[0] == 'h')); }

			// IComparable<Siguij>
			int IComparable<Siguij>.CompareTo(Siguij other)
			{
				return serial.CompareTo(other.serial);
			}

            public string GetUpdatedLine(double x, double y, double z)
            {
                //string strx = string.Format("{0,7:#0.000} ", x);
                //string stry = string.Format("{0,7:#0.000} ", y);
                //string strz = string.Format("{0,7:#0.000} ", z);
                char[] line = this.line.ToArray();
                //for(int i=0; i<(idxs_x[1]-idxs_x[0]+1); i++) line[i+idxs_x[0]] = strx[i];
                //for(int i=0; i<(idxs_y[1]-idxs_y[0]+1); i++) line[i+idxs_y[0]] = stry[i];
                //for(int i=0; i<(idxs_z[1]-idxs_z[0]+1); i++) line[i+idxs_z[0]] = strz[i];
                string result = new string(line);
                return result;
            }

            public override string ToString()
            {
                string str  = string.Format("{0:D} {1}, {2:d} {3}", serial, name.Trim(), resSeq, resName.Trim());
                       //str += string.Format(", pos({0:G4},{1:G4},{2:G4})", x, y, z);
                       //str += string.Format(", alt({0}), chain({1})", altLoc, chainID);
                return str;
            }

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Siguij(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Siguij(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
