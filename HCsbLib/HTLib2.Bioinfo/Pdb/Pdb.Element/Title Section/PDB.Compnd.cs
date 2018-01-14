using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    //using Compnd = Pdb.Compnd;
    public static partial class PdbStatic
    {
        public static Pdb.Compnd[] SortByContinuation(this IList<Pdb.Compnd> compnds)
        {
            Dictionary<int, Pdb.Compnd> dict = new Dictionary<int, Pdb.Compnd>();
            foreach(var compnd in compnds)
            {
                string sconti = compnd.continuation;
                int?   iconti = sconti.HParseInt();
                if(iconti == null) iconti = 0;
                dict.Add(iconti.Value, compnd);
            }
            HDebug.Assert(compnds.Count == dict.Count);

            int[] keys = dict.Keys.ToArray().HSort();

            Pdb.Compnd[] sortcompnds = new Pdb.Compnd[compnds.Count];
            for(int i=0; i<sortcompnds.Length; i++)
            {
                var key = keys[i];
                var compnd = dict[key];
                sortcompnds[i] = compnd;
            }

            return sortcompnds;
        }
        public static Dictionary<string, string> GroupByMolID(this IList<Pdb.Compnd> compnds)
        {
            compnds = compnds.SortByContinuation();

            List<Tuple<string, StringBuilder>> lst_id_spec = new List<Tuple<string, StringBuilder>>();
            foreach(var compnd in compnds)
            {
                string spec = compnd.specification.Trim();
                if(spec.StartsWith("MOL_ID:")) lst_id_spec.Add(new Tuple<string, StringBuilder>(spec, new StringBuilder()));
                else                           lst_id_spec.Last().Item2.Append(spec);
            }

            Dictionary<string, string> id_spec = new Dictionary<string, string>();
            for(int i=0; i<lst_id_spec.Count; i++)
            {
                string sid = lst_id_spec[i].Item1;
                //sid = sid.Replace("MOL_ID:", "");
                //sid = sid.Replace(";", "");
                //sid = sid.Replace(" ", "");
                //int id = sid.HParseInt().Value;
                string spec = lst_id_spec[i].Item2.ToString();
                id_spec.Add(sid, spec);
            }

            return id_spec;
        }
    }
	public partial class Pdb
	{
        [Serializable]
        public class Compnd : Element
		{
            /// http://www.wwpdb.org/documentation/format23/sect2.html#COMPND
            /// 
            /// COMPND
            /// 
            /// The COMPND record describes the macromolecular contents of an entry. Each macromolecule
            /// found in the entry is described by a set of token: value pairs, and is referred to as a
            /// COMPND record component. Since the concept of a molecule is difficult to specify exactly,
            /// PDB staff may exercise editorial judgment in consultation with depositors in assigning
            /// these names.
            /// 
            /// COLUMNS        DATA TYPE         FIELD          DEFINITION                        
            /// ----------------------------------------------------------------------------------
            ///  1 -  6        Record name       "COMPND"                                         
            ///  9 - 10        Continuation      continuation   Allows concatenation of multiple records.                          
            /// 11 - 70        Specification     compound       Description of the molecular      
            ///                list                             components.                  
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// COMPND    MOL_ID: 1;
            /// COMPND   2 MOLECULE: HEMOGLOBIN;
            /// COMPND   3 CHAIN: A, B, C, D;
            /// COMPND   4 ENGINEERED: YES;
            /// COMPND   5 MUTATION: YES
            /// COMPND   6 OTHER_DETAILS: DEOXY FORM
            /// 
            /// COMPND    MOL_ID: 1;
            /// COMPND   2 MOLECULE: COWPEA CHLOROTIC MOTTLE VIRUS;
            /// COMPND   3 CHAIN: A, B, C;
            /// COMPND   4 SYNONYM: CCMV;
            /// COMPND   5 MOL_ID: 2;
            /// COMPND   6 MOLECULE: RNA (5'-(*AP*UP*AP*U)-3');
            /// COMPND   7 CHAIN: D, F;
            /// COMPND   8 ENGINEERED: YES;
            /// COMPND   9 MOL_ID: 3;
            /// COMPND  10 MOLECULE: RNA (5'-(*AP*U)-3');
            /// COMPND  11 CHAIN: E;
            /// COMPND  12 ENGINEERED: YES
            /// 
            /// COMPND    MOL_ID: 1;                                            
            /// COMPND   2 MOLECULE: HEVAMINE A;                                
            /// COMPND   3 CHAIN: A;                                         
            /// COMPND   4 EC: 3.2.1.14, 3.2.1.17;                              
            /// COMPND   5 OTHER_DETAILS: PLANT ENDOCHITINASE/LYSOZYME          
            /// 
            /// 

			public static string RecordName { get { return "COMPND"; } }

            Compnd(string line)
                : base(line)
			{
			}
			public static Compnd FromString(string line)
			{
				HDebug.Assert(IsCompnd(line));
				return new Compnd(line);
			}
			public static bool IsCompnd(string line) { return (line.Substring(0, 6) == "COMPND"); }
			public string continuation { get { return String (idxs_continuation ); } } int[] idxs_continuation  = new int[]{ 8,10}; //  9 - 10        Continuation      continuation   Allows concatenation of multiple records.
			public string specification{ get { return String (idxs_specification); } } int[] idxs_specification = new int[]{11,70}; // 11 - 70        Specification     compound       Description of the molecular components.

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Compnd(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
