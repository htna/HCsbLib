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
        public class Title : Element
		{
            /// http://www.wwpdb.org/documentation/format23/sect2.html#TITLE
			///
            /// TITLE
            /// 
            /// The TITLE record contains a title for the experiment or analysis that is
            /// represented in the entry. It should identify an entry in the PDB in the same
            /// way that a title identifies a paper. 
            /// 
            /// COLUMNS    DATA TYPE        FIELD            DEFINITION
            /// ----------------------------------------------------------------------------
            ///  1 -  6    Record name      "TITLE "
            ///  9 - 10    Continuation     continuation     Allows concatenation of multiple records.
            /// 11 - 70    String           title            Title of the experiment.
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// TITLE     RHIZOPUSPEPSIN COMPLEXED WITH REDUCED PEPTIDE INHIBITOR
            /// 
            /// TITLE     BETA-GLUCOSYLTRANSFERASE, ALPHA CARBON COORDINATES ONLY
            /// 
            /// TITLE     NMR STUDY OF OXIDIZED THIOREDOXIN MUTANT (C62A,C69A,C73A)
            /// TITLE    2 MINIMIZED AVERAGE STRUCTURE
            /// 
            /// 

			public static string RecordName { get { return "TITLE "; } }

            Title(string line)
                : base(line)
			{
			}
			public static Title FromString(string line)
			{
				HDebug.Assert(IsTitle(line));
				return new Title(line);
			}
			public static bool IsTitle(string line) { return (line.Substring(0, 6) == "TITLE "); }
			public string continuation{ get { return String (idxs_continuation); } } int[] idxs_continuation = new int[]{ 9,10}; //  9 - 10    Continuation     continuation     Allows concatenation of multiple records.
			public string title       { get { return String (idxs_title       ); } } int[] idxs_title        = new int[]{11,70}; // 11 - 70    String           title            Title of the experiment.

			// IComparable<Atom>
			//int IComparable<Atom>.CompareTo(Atom other)
			//{
			//	return serial.CompareTo(other.serial);
			//}

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Title(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
