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
        public class Cryst1 : Element, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format23/sect8.html#CRYST1
			///
            /// CRYST1
            /// 
            /// The CRYST1 record presents the unit cell parameters, space
            /// group, and Z value. If the structure was not determined by
            /// crystallographic means, CRYST1 simply defines a unit cube. 
            /// 
            /// COLUMNS      DATA TYPE            FIELD        DEFINITION
            /// ----------------------------------------------------------
            ///  1 -  6      Record name          "CRYST1"
            ///  7 - 15      Real(9.3)            a            a (Angstroms).
            /// 16 - 24      Real(9.3)            b            b (Angstroms).
            /// 25 - 33      Real(9.3)            c            c (Angstroms).
            /// 34 - 40      Real(7.2)            alpha        alpha (degrees).
            /// 41 - 47      Real(7.2)            beta         beta (degrees).
            /// 48 - 54      Real(7.2)            gamma        gamma (degrees).
            /// 56 - 66      LString              sGroup       Space group.
            /// 67 - 70      Integer              z            Z value.
            /// 
            /// Example
            ///          1         2         3         4         5         6         7 
            /// 1234567890123456789012345678901234567890123456789012345678901234567890 
            /// CRYST1   52.000   58.600   61.900  90.00  90.00  90.00 P 21 21 21    8 
            /// CRYST1    1.000    1.000    1.000  90.00  90.00  90.00 P 1           1 
            /// CRYST1   42.544   69.085   50.950  90.00  95.55  90.00 P 1 21 1      2

            public static string RecordName { get { return "CRYST1"; } }
            Cryst1(string line)
				: base(line)
			{
			}
			public static Cryst1 FromString(string line)
			{
				HDebug.Assert(IsCryst1(line));
				return new Cryst1(line);
			}
            public static bool IsCryst1(string line) { return (line.Substring(0, 6) == "CRYST1"); }
            public double a      { get { return Double (idxs_a     ).Value; } } static readonly int[] idxs_a      = new int[]{ 7,15};  //  7 - 15      Real(9.3)            a            a (Angstroms).
            public double b      { get { return Double (idxs_b     ).Value; } } static readonly int[] idxs_b      = new int[]{16,24};  // 16 - 24      Real(9.3)            b            b (Angstroms).
            public double c      { get { return Double (idxs_c     ).Value; } } static readonly int[] idxs_c      = new int[]{25,33};  // 25 - 33      Real(9.3)            c            c (Angstroms).
            public double alpha  { get { return Double (idxs_alpha ).Value; } } static readonly int[] idxs_alpha  = new int[]{34,40};  // 34 - 40      Real(7.2)            alpha        alpha (degrees).
            public double beta   { get { return Double (idxs_beta  ).Value; } } static readonly int[] idxs_beta   = new int[]{41,47};  // 41 - 47      Real(7.2)            beta         beta (degrees).
            public double gamma  { get { return Double (idxs_gamma ).Value; } } static readonly int[] idxs_gamma  = new int[]{48,54};  // 48 - 54      Real(7.2)            gamma        gamma (degrees).
            public string sGroup { get { return String (idxs_sGroup);       } } static readonly int[] idxs_sGroup = new int[]{56,66};  // 56 - 66      LString              sGroup       Space group.
            public    int z      { get { return Integer(idxs_z     ).Value; } } static readonly int[] idxs_z      = new int[]{67,70};  // 67 - 70      Integer              z            Z value.

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Cryst1(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
            ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Cryst1(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
	}
}
