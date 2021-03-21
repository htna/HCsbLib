using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class PdbStatic
    {
        public static IEnumerable<Pdb.Remark> HEnumByRemarkNum(this IEnumerable<Pdb.Remark> remarks, int remarkNum)
        {
            foreach(var remark in remarks)
            {
                if(remark.remarkNum == remarkNum)
                    yield return remark;
            }
        }
    }
	public partial class Pdb
	{
        [Serializable]
        public class Remark : Element
		{
            /// http://www.wwpdb.org/documentation/format23/remarks.html
			///
            /// REMARKS
            /// 
            /// REMARK records present experimental details, annotations, comments, and
            /// information not included in other records. In a number of cases, REMARKs
            /// are used to expand the contents of other record types. A new level of
            /// structure is being used for some REMARK records. This is expected to
            /// facilitate searching and will assist in the conversion to a relational
            /// database. 
            /// 
            /// The very first line of every set of REMARK records is used as a spacer to
            /// aid in reading.
            /// 
            /// COLUMNS    DATA TYPE      FIELD        DEFINITION
            /// -----------------------------------------------------------------------
            ///  1 -  6    Record name    "REMARK"
            ///  8 - 10    Integer        remarkNum    Remark number. It is not an error
            ///                                        for remark n to exist in an entry
            ///                                        when remark n-1 does not.
            /// 12 - 70    LString        empty        Left as white space in first line of
            ///                                        each new remark.
            /// 
            public static string RecordName { get { return "REMARK"; } }

            Remark(string line)
                : base(line)
			{
			}
			public static Remark FromString(string line)
			{
				HDebug.Assert(IsRemark(line));
				return new Remark(line);
			}
			public static bool IsRemark(string line) { return (line.Substring(0, 6) == "REMARK"); }
			public    int remarkNum   { get { return Integer(idxs_remarkNum ).Value; } } int[] idxs_remarkNum = new int[]{ 8,10}; //  8 - 10    Integer        remarkNum    Remark number.
			public string empty       { get { return String (idxs_empty     );       } } int[] idxs_empty     = new int[]{12,70}; // 12 - 70    LString        empty        Left as white space in first line of each new remark.
            public string contents    { get { return empty;                          } }

			// IComparable<Atom>
			//int IComparable<Atom>.CompareTo(Atom other)
			//{
			//	return serial.CompareTo(other.serial);
			//}

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Remark(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
