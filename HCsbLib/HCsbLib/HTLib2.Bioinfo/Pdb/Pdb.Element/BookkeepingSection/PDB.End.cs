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
        public class End : Element
		{
            /// http://www.wwpdb.org/documentation/format23/sect11.html#END
            /// 
            /// END
            /// 
            /// The END record marks the end of the PDB file.
            /// 
            /// COLUMNS              DATA TYPE          FIELD     DEFINITION
            /// -------------------------------------------------------
            ///  1 -     6           Record name        "END    "
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// END    
            /// 
            /// 

            public static string RecordName { get { return "END   "; } }

            End(string line)
                : base(line)
			{
			}
			public static End FromString(string line)
			{
				HDebug.Assert(IsEnd(line));
				return new End(line);
			}
            public static bool IsEnd(string line) { return (line.Substring(0, 6) == "END   "); }

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public End(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
