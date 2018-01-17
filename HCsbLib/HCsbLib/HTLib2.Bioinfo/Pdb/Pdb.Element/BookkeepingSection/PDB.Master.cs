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
        public class Master : Element
		{
			/// http://www.wwpdb.org/documentation/format23/sect11.html#MASTER
			///
            /// MASTER
            /// 
            /// The MASTER record is a control record for bookkeeping. It lists the number
            /// of lines in the coordinate entry or file for selected record types.
            /// 
            /// Record Format 
            /// 
            /// COLUMNS    DATA TYPE      FIELD         DEFINITION
            /// -------------------------------------------------------------------
            ///  1 -  6    Record name    "MASTER"
            /// 11 - 15    Integer        numRemark     Number of REMARK records
            /// 16 - 20    Integer        "0"
            /// 21 - 25    Integer        numHet        Number of HET records
            /// 26 - 30    Integer        numHelix      Number of HELIX records
            /// 31 - 35    Integer        numSheet      Number of SHEET records
            /// 36 - 40    Integer        numTurn       Number of TURN records
            /// 41 - 45    Integer        numSite       Number of SITE records
            /// 46 - 50    Integer        numXform      Number of coordinate 
            ///                                         transformation
            ///                                         records (ORIGX+SCALE+MTRIX)
            /// 51 - 55    Integer        numCoord      Number of atomic coordinate 
            ///                                         records (ATOM+HETATM)
            /// 56 - 60    Integer        numTer        Number of TER records
            /// 61 - 65    Integer        numConect     Number of CONECT records
            /// 66 - 70    Integer        numSeq        Number of SEQRES records
            /// 
            /// Example 
            ///          1         2         3         4         5         6         7
            /// 1234567890123456789012345678901234567890123456789012345678901234567890
            /// MASTER       40    0    0    0    0    0    0    6 2930    2    0   29
            /// 
            /// 

            public static string RecordName { get { return "MASTER"; } }

            Master(string line)
                : base(line)
			{
			}
			public static Master FromString(string line)
			{
				HDebug.Assert(IsMaster(line));
				return new Master(line);
			}
            public static bool IsMaster(string line) { return (line.Substring(0, 6) == "MASTER"); }
            public    int numRemark { get { return Integer(idxs_numRemark).Value; } } int[] idxs_numRemark = new int[]{11,15}; // 11 - 15    Integer        numRemark     Number of REMARK records
            public    int zero      { get { return Integer(idxs_zero     ).Value; } } int[] idxs_zero      = new int[]{16,20}; // 16 - 20    Integer        "0"
            public    int numHet    { get { return Integer(idxs_numHet   ).Value; } } int[] idxs_numHet    = new int[]{21,25}; // 21 - 25    Integer        numHet        Number of HET records
            public    int numHelix  { get { return Integer(idxs_numHelix ).Value; } } int[] idxs_numHelix  = new int[]{26,30}; // 26 - 30    Integer        numHelix      Number of HELIX records
            public    int numSheet  { get { return Integer(idxs_numSheet ).Value; } } int[] idxs_numSheet  = new int[]{31,35}; // 31 - 35    Integer        numSheet      Number of SHEET records
            public    int numTurn   { get { return Integer(idxs_numTurn  ).Value; } } int[] idxs_numTurn   = new int[]{36,40}; // 36 - 40    Integer        numTurn       Number of TURN records
            public    int numSite   { get { return Integer(idxs_numSite  ).Value; } } int[] idxs_numSite   = new int[]{41,45}; // 41 - 45    Integer        numSite       Number of SITE records
            public    int numXform  { get { return Integer(idxs_numXform ).Value; } } int[] idxs_numXform  = new int[]{46,50}; // 46 - 50    Integer        numXform      Number of coordinate transformation records (ORIGX+SCALE+MTRIX)
            public    int numCoord  { get { return Integer(idxs_numCoord ).Value; } } int[] idxs_numCoord  = new int[]{51,55}; // 51 - 55    Integer        numCoord      Number of atomic coordinate records (ATOM+HETATM)
            public    int numTer    { get { return Integer(idxs_numTer   ).Value; } } int[] idxs_numTer    = new int[]{56,60}; // 56 - 60    Integer        numTer        Number of TER records
            public    int numConect { get { return Integer(idxs_numConect).Value; } } int[] idxs_numConect = new int[]{61,65}; // 61 - 65    Integer        numConect     Number of CONECT records
            public    int numSeq    { get { return Integer(idxs_numSeq   ).Value; } } int[] idxs_numSeq    = new int[]{66,70}; // 66 - 70    Integer        numSeq        Number of SEQRES records

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Master(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////


		}
	}
}
