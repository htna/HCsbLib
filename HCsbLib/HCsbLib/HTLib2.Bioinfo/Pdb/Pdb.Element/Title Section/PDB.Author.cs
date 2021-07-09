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
        public class Author : Element, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format32/sect2.html#AUTHOR
            /// 
            /// AUTHOR
            /// 
            /// The AUTHOR record contains the names of the people responsible for the contents of the entry.
            /// 
            /// COLUMNS      DATA  TYPE      FIELD         DEFINITION                           
            /// ------------------------------------------------------------------------------------
            ///  1 -  6      Record name     "AUTHOR"                                             
            ///  9 - 10      Continuation    continuation  Allows concatenation of multiple records.
            /// 11 - 79      List            authorList    List of the author names, separated    
            ///                                            by commas.
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// AUTHOR    M.B.BERRY,B.MEADOR,T.BILDERBACK,P.LIANG,M.GLASER,
            /// AUTHOR   2 G.N.PHILLIPS JR.,T.L.ST. STEVENS
            /// 
            /// 

            public static string RecordName { get { return "AUTHOR"; } }

            Author(string line)
                : base(line)
			{
			}
			public static Author FromString(string line)
			{
				HDebug.Assert(IsAuthor(line));
				return new Author(line);
			}
            public static bool IsAuthor(string line) { return (line.Substring(0, 6) == "AUTHOR"); }
			public string continuation{ get { return String(idxs_continuation); } } static readonly int[] idxs_continuation = new int[] { 9,10}; //  9 - 10      Continuation    continuation  Allows concatenation of multiple records.
            public string authorList  { get { return String(idxs_authorList  ); } } static readonly int[] idxs_authorList   = new int[] {11,79}; // 11 - 79      List            authorList    List of the author names, separated by commas.

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Author(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Author(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
