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
        public class Expdta : Element, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format23/sect2.html#EXPDTA
            /// 
            /// EXPDTA
            /// 
            /// The EXPDTA record presents information about the experiment.
            /// The EXPDTA record identifies the experimental technique used. This may
            /// refer to the type of radiation and sample, or include the spectroscopic
            /// or modeling technique. Permitted values include: 
            /// 
            /// COLUMNS       DATA TYPE      FIELD         DEFINITION                          
            /// -------------------------------------------------------
            ///  1 -  6       Record name    "EXPDTA"                                          
            ///  9 - 10       Continuation   continuation  Allows concatenation 
            ///                                            of multiple records
            /// 11 - 70       SList          technique     The experimental technique(s) 
            ///                                            with optional comment describing 
            ///                                            the sample or experiment. 
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// EXPDTA    X-RAY DIFFRACTION
            /// EXPDTA    NEUTRON DIFFRACTION; X-RAY DIFFRACTION
            /// EXPDTA    NMR, 32 STRUCTURES
            /// EXPDTA    NMR, REGULARIZED MEAN STRUCTURE
            /// EXPDTA    FIBER DIFFRACTION
            /// 
            /// 

            public static string RecordName { get { return "EXPDTA"; } }

            Expdta(string line)
                : base(line)
			{
			}
			public static Expdta FromString(string line)
			{
				HDebug.Assert(IsExpdta(line));
				return new Expdta(line);
			}
            public static bool IsExpdta(string line) { return (line.Substring(0, 6) == "EXPDTA"); }
			public string continuation{ get { return String(idxs_continuation); } } static readonly int[] idxs_continuation = new int[] { 9,10}; //  9 - 10        Continuation      continuation   Allows concatenation of multiple records.
            public string technique   { get { return String(idxs_technique   ); } } static readonly int[] idxs_technique    = new int[] {11,70}; // 11 - 70        Specification     compound       Description of the molecular components.

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Expdta(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Expdta(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
