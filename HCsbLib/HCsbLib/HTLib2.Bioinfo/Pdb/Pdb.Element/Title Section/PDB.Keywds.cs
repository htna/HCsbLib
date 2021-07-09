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
        public class Keywds : Element, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format23/sect2.html#KEYWDS
            /// 
            /// KEYWDS
            /// 
            /// The KEYWDS record contains a set of terms relevant to the entry. Terms in the KEYWDS record provide a simple means of categorizing entries and may be used to generate index files. This record addresses some of the limitations found in the classification field of the HEADER record. It provides the opportunity to add further annotation to the entry in a concise and computer-searchable fashion. 
            /// 
            /// Record Format 
            /// 
            /// COLUMNS        DATA TYPE       FIELD          DEFINITION                         
            /// ---------------------------------------------------------------------------------
            ///  1 -  6        Record name     "KEYWDS"                                          
            ///  9 - 10        Continuation    continuation   Allows concatenation of records if necessary
            /// 11 - 70        List            keywds         Comma-separated list of keywords   
            ///                                               relevant to the entry.            
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// KEYWDS    LYASE, TRICARBOXYLIC ACID CYCLE, MITOCHONDRION, OXIDATIVE
            /// KEYWDS   2 METABOLISM
            /// 
            /// 

			public static string RecordName { get { return "KEYWDS"; } }

            Keywds(string line)
                : base(line)
			{
			}
			public static Keywds FromString(string line)
			{
				HDebug.Assert(IsKeywds(line));
				return new Keywds(line);
			}
			public static bool IsKeywds(string line) { return (line.Substring(0, 6) == "KEYWDS"); }
			public string continuation { get { return String (idxs_continuation); } } static readonly int[] idxs_continuation = new int[]{ 9,10}; //  9 - 10        Continuation      continuation   Allows concatenation of multiple records.
			public string keywds       { get { return String (idxs_keywds      ); } } static readonly int[] idxs_keywds       = new int[]{11,70}; // 11 - 70        Specification     compound       Description of the molecular components.

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Keywds(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Keywds(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
