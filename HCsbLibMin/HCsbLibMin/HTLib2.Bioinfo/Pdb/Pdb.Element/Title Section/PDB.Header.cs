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
        public class Header : Element
		{
            /// http://www.wwpdb.org/documentation/format23/sect2.html#HEADER
            /// 
            /// HEADER
            /// 
            /// The HEADER record uniquely identifies a PDB entry through the idCode field.
            /// This record also provides a classification for the entry. Finally, it contains
            /// the date the coordinates were deposited at the PDB. 
            /// 
            /// COLUMNS      DATA TYPE      FIELD             DEFINITION
            /// ---------------------------------------------------------------------------
            ///  1 -  6      Record name    "HEADER"
            /// 11 - 50      String(40)     classification    Classifies the molecule(s)
            /// 51 - 59      Date           depDate           Deposition date. 
            ///                                               This is the date the coordinates were 
            ///                                               received by the PDB
            /// 63 - 66      IDcode         idCode            This identifier is unique within the PDB
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// HEADER    MUSCLE PROTEIN                          02-JUN-93   1MYS
            /// HEADER    HYDROLASE (CARBOXYLIC ESTER)            08-APR-93   2PHI
            /// HEADER    COMPLEX (LECTIN/TRANSFERRIN)            07-JAN-94   1LGB
            /// 
            /// 

			public static string RecordName { get { return "HEADER"; } }

            Header(string line)
                : base(line)
			{
			}
			public static Header FromString(string line)
			{
				HDebug.Assert(IsHeader(line));
				return new Header(line);
			}
			public static bool IsHeader(string line) { return (line.Substring(0, 6) == "HEADER"); }
			public string classification{ get { return String (idxs_classification); } } int[] idxs_classification = new int[]{11,50}; // 11 - 50      String(40)     classification    Classifies the molecule(s)
			public string depDate       { get { return String (idxs_depDate       ); } } int[] idxs_depDate        = new int[]{51,59}; // 51 - 59      Date           depDate           Deposition date. 
			public string idCode        { get { return String (idxs_idCode        ); } } int[] idxs_idCode         = new int[]{63,66}; // 63 - 66      IDcode         idCode            This identifier is unique within the PDB

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Header(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
