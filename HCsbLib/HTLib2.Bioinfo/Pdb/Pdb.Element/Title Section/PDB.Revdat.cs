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
        public class Revdat : Element
		{
            /// http://www.wwpdb.org/documentation/format32/sect2.html#REVDAT
            /// 
            /// REVDAT (updated)
            /// 
            /// REVDAT records contain a history of the modifications made to an entry since its release.
            /// 
            /// COLUMNS       DATA  TYPE     FIELD         DEFINITION                             
            /// -------------------------------------------------------------------------------------
            ///  1 -  6       Record name    "REVDAT"                                             
            ///  8 - 10       Integer        modNum        Modification number.                   
            /// 11 - 12       Continuation   continuation  Allows concatenation of multiple records.
            /// 14 - 22       Date           modDate       Date of modification (or release  for   
            ///                                            new entries)  in DD-MMM-YY format. This is
            ///                                            not repeated on continued lines.
            /// 24 - 27       IDCode         modId         ID code of this entry. This is not repeated on 
            ///                                            continuation lines.    
            /// 32            Integer        modType       An integer identifying the type of    
            ///                                            modification. For all  revisions, the
            ///                                            modification type is listed as 1 
            /// 40 - 45       LString(6)     record        Modification detail. 
            /// 47 - 52       LString(6)     record        Modification detail. 
            /// 54 - 59       LString(6)     record        Modification detail. 
            /// 61 - 66       LString(6)     record        Modification detail.
            /// 
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// REVDAT   2   15-OCT-99 1ABC    1       REMARK
            /// REVDAT   1   09-JAN-89 1ABC    0
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// REVDAT   2   11-MAR-08 2ABC    1       JRNL   VERSN
            /// REVDAT   1   09-DEC-03 2ABC    0
            /// 
            /// 

            public static string RecordName { get { return "REVDAT"; } }

            Revdat(string line)
                : base(line)
			{
			}
			public static Revdat FromString(string line)
			{
				HDebug.Assert(IsRevdat(line));
				return new Revdat(line);
			}
            public static bool IsRevdat(string line) { return (line.Substring(0, 6) == "REVDAT"); }
            public int    modNum      { get { return Integer(idxs_modNum      ).Value; } } int[] idxs_modNum       = new int[] { 8,10}; //  8 - 10       Integer        modNum        Modification number.                   
            public string continuation{ get { return String (idxs_continuation);       } } int[] idxs_continuation = new int[] {11,12}; // 11 - 12       Continuation   continuation  Allows concatenation of multiple records.
            public string modDate     { get { return String (idxs_modDate     );       } } int[] idxs_modDate      = new int[] {14,22}; // 14 - 22       Date           modDate       Date of modification (or release  for new entries) in DD-MMM-YY format. This is not repeated on continued lines.
            public string modId       { get { return String (idxs_modId       );       } } int[] idxs_modId        = new int[] {24,27}; // 24 - 27       IDCode         modId         ID code of this entry. This is not repeated on  continuation lines.
            public int    modType     { get { return Integer(idxs_modType     ).Value; } } int[] idxs_modType      = new int[] {32,32}; // 32            Integer        modType       An integer identifying the type of modification. For all  revisions, the modification type is listed as 1 
            public string record1     { get { return String (idxs_record1     );       } } int[] idxs_record1      = new int[] {40,45}; // 40 - 45       LString(6)     record        Modification detail. 
            public string record2     { get { return String (idxs_record2     );       } } int[] idxs_record2      = new int[] {47,52}; // 47 - 52       LString(6)     record        Modification detail. 
            public string record3     { get { return String (idxs_record3     );       } } int[] idxs_record3      = new int[] {54,59}; // 54 - 59       LString(6)     record        Modification detail. 
            public string record4     { get { return String (idxs_record4     );       } } int[] idxs_record4      = new int[] {61,66}; // 61 - 66       LString(6)     record        Modification detail.

            ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Revdat(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
