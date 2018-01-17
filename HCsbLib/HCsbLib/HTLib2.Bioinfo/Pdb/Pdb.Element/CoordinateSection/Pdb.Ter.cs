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
        public class Ter : Element//, IComparable<Ter>
		{
			/// http://www.wwpdb.org/documentation/format23/sect9.html#TER
			///
            /// TER
            /// 
            /// The TER record indicates the end of a list of ATOM/HETATM records for a chain.
            /// 
            /// COLUMNS     DATA TYPE         FIELD           DEFINITION
            /// ------------------------------------------------------
            ///  1 - 6      Record name       "TER     "
            ///  7 - 11     Integer           serial          Serial number.
            /// 18 - 20     Residue name      resName         Residue name.
            /// 22          Character         chainID         Chain identifier.
            /// 23 - 26     Integer           resSeq          Residue sequence number.
            /// 27          AChar             iCode           Insertion code.
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// ATOM   4150  H   ALA A 431       8.674  16.036  12.858  1.00  0.00           H
            /// TER    4151      ALA A 431
            /// ATOM   1403  O   PRO P  22      12.701  33.564  15.827  1.09 18.03           O
            /// ATOM   1404  CB  PRO P  22      13.512  32.617  18.642  1.09  9.32           C
            /// ATOM   1405  CG  PRO P  22      12.828  33.382  19.740  1.09 12.23           C
            /// ATOM   1406  CD  PRO P  22      12.324  34.603  18.985  1.09 11.47           C
            /// HETATM 1407  CA  BLE P   1      14.625  32.240  14.151  1.09 16.76           C
            /// HETATM 1408  CB  BLE P   1      15.610  33.091  13.297  1.09 16.56           C
            /// HETATM 1409  CG  BLE P   1      15.558  34.629  13.373  1.09 14.27           C
            /// HETATM 1410  CD1 BLE P   1      16.601  35.208  12.440  1.09 14.75           C
            /// HETATM 1411  CD2 BLE P   1      14.209  35.160  12.930  1.09 15.60           C
            /// HETATM 1412  N   BLE P   1      14.777  32.703  15.531  1.09 14.79           N
            /// HETATM 1413  B   BLE P   1      14.921  30.655  14.194  1.09 15.56           B
            /// HETATM 1414  O1  BLE P   1      14.852  30.178  12.832  1.09 16.10           O
            /// HETATM 1415  O2  BLE P   1      13.775  30.147  14.862  1.09 20.95           O
            /// TER    1416      BLE P   1                                            

			public static string RecordName { get { return "TER   "; } }

            Ter(string line)
                : base(line)
			{
			}
			public static Ter FromString(string line)
			{
				HDebug.Assert(IsTer(line));
				return new Ter(line);
			}
            public static bool IsTer(string line) { return (line.Substring(0, 6) == "TER   "); }
            public    int serial  { get { return Integer(idxs_serial ).Value; } } int[] idxs_serial  = new int[]{ 7,11}; // //  7 - 11     Integer           serial          Serial number.
            public string resName { get { return String (idxs_resName);       } } int[] idxs_resName = new int[]{18,20}; // // 18 - 20     Residue name      resName         Residue name.
            public   char chainID { get { return Char   (idxs_chainID);       } } int[] idxs_chainID = new int[]{22,22}; // // 22          Character         chainID         Chain identifier.
            public    int resSeq  { get { return Integer(idxs_resSeq ).Value; } } int[] idxs_resSeq  = new int[]{23,26}; // // 23 - 26     Integer           resSeq          Residue sequence number.
            public   char iCode   { get { return Char   (idxs_iCode  );       } } int[] idxs_iCode   = new int[]{27,27}; // // 27          AChar             iCode           Insertion code.

			//// IComparable<Atom>
			//int IComparable<Atom>.CompareTo(Atom other)
			//{
			//	return serial.CompareTo(other.serial);
			//}

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Ter(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
