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
        public class Source : Element
		{
            /// http://www.wwpdb.org/documentation/format23/sect2.html#SOURCE
            /// 
            /// 
            /// SOURCE
            /// 
            /// The SOURCE record specifies the biological and/or chemical source of each biological
            /// molecule in the entry. Sources are described by both the common name and the scientific
            /// name, e.g., genus and species. Strain and/or cell-line for immortalized cells are
            /// given when they help to uniquely identify the biological entity studied. 
            /// 
            /// COLUMNS   DATA TYPE         FIELD          DEFINITION                        
            /// -------------------------------------------------------------------------------
            ///  1 -  6   Record name       "SOURCE"                                         
            ///  9 - 10   Continuation      continuation   Allows concatenation of multiple records.                         
            /// 11 - 70   Specification     srcName        Identifies the source of the macromolecule in 
            ///            list                            a token: value format.                        
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// SOURCE    MOL_ID: 1;
            /// SOURCE   2 ORGANISM_SCIENTIFIC: AVIAN SARCOMA VIRUS;
            /// SOURCE   3 STRAIN: SCHMIDT-RUPPIN B;
            /// SOURCE   4 EXPRESSION_SYSTEM: ESCHERICHIA COLI;
            /// SOURCE   5 EXPRESSION_SYSTEM_PLASMID: PRC23IN
            /// 
            /// SOURCE    MOL_ID: 1;
            /// SOURCE   2 ORGANISM_SCIENTIFIC: GALLUS GALLUS;
            /// SOURCE   3 ORGANISM_COMMON: CHICKEN;
            /// SOURCE   4 ORGAN: HEART;
            /// SOURCE   5 TISSUE: MUSCLE
            /// 
            /// SOURCE    MOL_ID: 1;
            /// SOURCE   2 EXPRESSION_SYSTEM: ESCHERICHIA COLI;
            /// SOURCE   3 EXPRESSION_SYSTEM_STRAIN: BE167;
            /// SOURCE   4 FRAGMENT: RESIDUES 1-16;
            /// SOURCE   5 ORGANISM_SCIENTIFIC: BACILLUS AMYLOLIQUEFACIENS;
            /// SOURCE   6 EXPRESSION_SYSTEM: ESCHERICHIA COLI;
            /// SOURCE   7 FRAGMENT: RESIDUES 17-214;
            /// SOURCE   8 ORGANISM_SCIENTIFIC: BACILLUS MACERANS
            /// 
            /// 

			public static string RecordName { get { return "SOURCE"; } }

            Source(string line)
                : base(line)
			{
			}
			public static Source FromString(string line)
			{
				HDebug.Assert(IsSource(line));
				return new Source(line);
			}
			public static bool IsSource(string line) { return (line.Substring(0, 6) == "SOURCE"); }
			public string continuation { get { return String (idxs_continuation); } } int[] idxs_continuation = new int[]{ 9,10}; //  9 - 10        Continuation      continuation   Allows concatenation of multiple records.
			public string srcName      { get { return String (idxs_srcName     ); } } int[] idxs_srcName      = new int[]{11,70}; // 11 - 70        Specification     compound       Description of the molecular components.

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Source(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
