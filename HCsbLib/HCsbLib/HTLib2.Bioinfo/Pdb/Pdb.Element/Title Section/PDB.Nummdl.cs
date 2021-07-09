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
        public class Nummdl : Element, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format32/sect2.html#NUMMDL
            /// 
            /// NUMMDL (added)
            /// 
            /// The NUMMDL record indicates total number of models in a PDB entry.
            /// 
            /// COLUMNS      DATA TYPE      FIELD         DEFINITION                           
            /// ------------------------------------------------------------------------------------
            ///  1 -  6      Record name    "NUMMDL"                                             
            /// 11 - 14      Integer        modelNumber   Number of models.   
            /// 
            /// Example
            ///          1         2         3         4         5          6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// NUMMDL    20 
            /// 
            /// 

            public static string RecordName { get { return "NUMMDL"; } }

            Nummdl(string line)
                : base(line)
			{
			}
			public static Nummdl FromString(string line)
			{
				HDebug.Assert(IsNummdl(line));
				return new Nummdl(line);
			}
            public static bool IsNummdl(string line) { return (line.Substring(0, 6) == "NUMMDL"); }
			public string continuation{ get { return String(idxs_continuation); } } static readonly int[] idxs_continuation = new int[] { 9,10}; //  9 - 10        Continuation      continuation   Allows concatenation of multiple records.
            public string technique   { get { return String(idxs_technique   ); } } static readonly int[] idxs_technique    = new int[] {11,70}; // 11 - 70        Specification     compound       Description of the molecular components.

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Nummdl(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Nummdl(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
