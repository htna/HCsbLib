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
        public class Endmdl : Element
		{
			/// http://www.wwpdb.org/documentation/format23/sect9.html#ENDMDL
			///
            /// ENDMDL
            /// 
            /// The ENDMDL records are paired with MODEL records to
            /// group individual structures found in a coordinate entry. 
            /// 
            /// Record Format 
            /// 
            /// COLUMNS    DATA TYPE       FIELD        DEFINITION
            /// --------------------------------------------------
            ///  1 - 6     Record name     "ENDMDL"
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// ...
            /// ...
            /// ATOM  14550 1HG  GLU   122     -14.364  14.787 -14.258  1.00  0.00           H
            /// ATOM  14551 2HG  GLU   122     -13.794  13.738 -12.961  1.00  0.00           H
            /// TER   14552      GLU   122                                             
            /// ENDMDL                                                                 
            /// MODEL        9                                                         
            /// ATOM  14553  N   SER     1     -28.280   1.567  12.004  1.00  0.00           N
            /// ATOM  14554  CA  SER     1     -27.749   0.392  11.256  1.00  0.00           C
            /// ...
            /// ...
            /// ATOM  16369 1HG  GLU   122      -3.757  18.546  -8.439  1.00  0.00           H
            /// ATOM  16370 2HG  GLU   122      -3.066  17.166  -7.584  1.00  0.00           H
            /// TER   16371      GLU   122                                             
            /// ENDMDL                                                                 
            /// MODEL       10                                                         
            /// ATOM  16372  N   SER     1     -22.285   7.041  10.003  1.00  0.00           N
            /// ATOM  16373  CA  SER     1     -23.026   6.872   8.720  1.00  0.00           C
            /// ...
            /// ...
            /// ATOM  18188 1HG  GLU   122      -1.467  18.282 -17.144  1.00  0.00           H
            /// ATOM  18189 2HG  GLU   122      -2.711  18.067 -15.913  1.00  0.00           H
            /// TER   18190      GLU   122                                             
            /// ENDMDL
            /// 
            /// 
            public static string RecordName { get { return "ENDMDL"; } }

            Endmdl(string line)
                : base(line)
			{
			}
			public static Endmdl FromString(string line)
			{
				HDebug.Assert(IsEndmdl(line));
				return new Endmdl(line);
			}
            public static Endmdl From()
            {
                            // 0        1         2         3         4         5         6         7         8
                            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
                string line = "ENDMDL                                                                          ";
                return new Endmdl(line);
            }
            public static bool IsEndmdl(string line) { return (line.Substring(0, 6) == "ENDMDL"); }

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Endmdl(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
