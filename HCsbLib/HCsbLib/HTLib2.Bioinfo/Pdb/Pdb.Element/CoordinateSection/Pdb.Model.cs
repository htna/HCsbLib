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
        public class Model : Element, IComparable<Model>, IBinarySerializable
		{
			/// http://www.wwpdb.org/documentation/format23/sect9.html#MODEL
			///
            /// MODEL
            /// 
            /// The MODEL record specifies the model serial number when multiple
            /// structures are presented in a single coordinate entry, as is
            /// often the case with structures determined by NMR. 
            /// 
            /// Record Format 
            /// 
            /// COLUMNS     DATA TYPE       FIELD       DEFINITION
            /// -------------------------------------------------------------
            ///  1 - 6      Record name     "MODEL "
            /// 11 - 14     Integer         serial      Model serial number.
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// MODEL        1
            /// ATOM      1  N   ALA     1      11.104   6.134  -6.504  1.00  0.00           N
            /// ATOM      2  CA  ALA     1      11.639   6.071  -5.147  1.00  0.00           C
            /// ...
            /// ...
            /// ATOM    293 1HG  GLU    18     -14.861  -4.847   0.361  1.00  0.00           H
            /// ATOM    294 2HG  GLU    18     -13.518  -3.769   0.084  1.00  0.00           H
            /// TER     295      GLU    18                                           
            /// ENDMDL                                                              
            /// MODEL        2                                                       
            /// ATOM    296  N   ALA     1      10.883   6.779  -6.464  1.00  0.00           N
            /// ATOM    297  CA  ALA     1      11.451   6.531  -5.142  1.00  0.00           C
            /// ...
            /// ...
            /// ATOM    588 1HG  GLU    18     -13.363  -4.163  -2.372  1.00  0.00           H
            /// ATOM    589 2HG  GLU    18     -12.634  -3.023  -3.475  1.00  0.00           H
            /// TER     590      GLU    18                                          
            /// ENDMDL

            public static string RecordName { get { return "MODEL "; } }

            Model(string line)
                : base(line)
			{
			}
			public static Model FromString(string line)
			{
				HDebug.Assert(IsModel(line));
				return new Model(line);
			}
            public static Model FromModelSerial(int serial)
            {
                            // 0        1         2         3         4         5         6         7         8
                            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890
                char[] line = "MODEL                                                                           ".ToArray();
                string strserial = string.Format("{0,4}", serial);
                for(int i=0; i<4; i++) line[i+idxs_serial[0]] = strserial[i];
                return new Model(new string(line));
            }
            public static bool IsModel(string line) { return (line.Substring(0, 6) == "MODEL "); }
            public    int serial    { get { return Integer(idxs_serial    ).Value; } } static readonly int[] idxs_serial     = new int[]{11,14}; // 11 - 14     Integer         serial      Model serial number.

			// IComparable<Model>
			int IComparable<Model>.CompareTo(Model other)
			{
				return serial.CompareTo(other.serial);
			}

            //public override string ToString()
            //{
            //    string str  = string.Format("{0:D} {1}, {2:d} {3}", serial, name.Trim(), resSeq, resName.Trim());
            //           str += string.Format(", pos({0:G4},{1:G4},{2:G4})", x, y, z);
            //           str += string.Format(", alt({0}), chain({1})", altLoc, chainID);
            //    return str;
            //}

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Model(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Model(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
