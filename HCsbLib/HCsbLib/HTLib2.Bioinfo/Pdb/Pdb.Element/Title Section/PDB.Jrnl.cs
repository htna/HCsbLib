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
        public class Jrnl : Element, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format32/sect2.html#JRNL
            /// 
            /// JRNL (updated)
            /// 
            /// The JRNL record contains the primary literature citation that describes
            /// the experiment which resulted in the deposited coordinate set. There is
            /// at most one JRNL reference per entry. If there is no primary reference,
            /// then there is no JRNL reference. Other references are given in REMARK 1. 
            /// 
            /// COLUMNS       DATA TYPE      FIELD         DEFINITION                  
            /// -----------------------------------------------------------------------
            ///  1 -  6       Record name    "JRNL"                                  
            /// 13 - 79       LString        text          See Details below.           
            /// 
            #region details : AUTH, TITL, EDIT, REF, PUBL, REFN, PMID, DOI             
            /// 1. AUTH 
            ///     COLUMNS       DATA  TYPE     FIELD               DEFINITION                         
            ///     -------------------------------------------------------------------------------
            ///      1 -  6       Record name    "REMARK"                                          
            ///     10            LString(1)     "1"                                               
            ///     13 - 16       LString(4)     "AUTH"              Appears on all continuation records.
            ///     17 - 18       Continuation   continuation        Allows  a long list of authors.     
            ///     20 - 79       List           authorList          List of the authors.               
            ///     
            /// 2. TITL 
            ///     COLUMNS       DATA  TYPE     FIELD               DEFINITION                         
            ///     -----------------------------------------------------------------------------------
            ///      1 -  6       Record name    "REMARK"                                          
            ///     10            LString(1)     "1"                                               
            ///     13 - 16       LString(4)     "TITL"              Appears on all continuation records.
            ///     17 - 18       Continuation   continuation        Permits long titles.               
            ///     20 - 79       LString        title               Title of the article. 
            ///     
            /// 3. EDIT
            ///     COLUMNS       DATA  TYPE     FIELD               DEFINITION                         
            ///     -----------------------------------------------------------------------------------
            ///     1 -  6        Record name    "REMARK"                                          
            ///     10            LString(1)     "1"                                               
            ///     13 - 16       LString(4)     "TITL"              Appears  on all continuation records.
            ///     17 - 18       Continuation   continuation        Permits long titles.               
            ///     20 - 79       LString        title               Title of the article.
            ///     
            /// 4. REF
            ///     COLUMNS       DATA  TYPE     FIELD               DEFINITION
            ///     --------------------------------------------------------------------------------
            ///      1 -  6       Record name    "JRNL  "
            ///     13 - 16       LString(3)     "REF"
            ///     20 - 34       LString(15)    "TO BE PUBLISHED"
            ///     
            ///     COLUMNS       DATA  TYPE     FIELD              DEFINITION
            ///     ---------------------------------------------------------------------------------------
            ///      1 -  6       Record name    "JRNL  "
            ///     13 - 16       LString(3)     "REF "
            ///     17 - 18       Continuation   continuation       Allows long publication names.
            ///     20 - 47       LString        pubName            Name  of the publication including section
            ///                                                     or series designation. This is the only
            ///                                                     field of this sub-record which may be
            ///                                                     continued on  successive sub-records.
            ///     50 - 51       LString(2)     "V."               Appears in the first sub-record only,
            ///                                                     and  only if column 55 is non-blank.
            ///     52 - 55       String         volume             Right-justified blank-filled volume
            ///                                                     information; appears in the first
            ///                                                     sub-record only.
            ///     57 - 61       String         page               First page of the article; appears in 
            ///                                                     the first sub-record only.
            ///     63 - 66       Integer        year               Year of publication; first sub-record only.
            ///     
            /// 5. PUBL
            ///     COLUMNS       DATA  TYPE     FIELD              DEFINITION
            ///     --------------------------------------------------------------------------------------
            ///      1 -  6       Record name    "JRNL  "
            ///     13 - 16       LString(4)     "PUBL"
            ///     17 - 18       Continuation   continuation       Allows long publisher and place names.
            ///     20 - 70       LString        pub                City  of publication and name of the
            ///                                                     publisher/institution.
            ///     
            /// 6. REFN
            ///     COLUMNS       DATA  TYPE     FIELD              DEFINITION
            ///     --------------------------------------------------------------------------------
            ///      1 -  6       Record  name   "JRNL  "
            ///     13 - 16       LString(4)     "REFN"
            ///     6b. This form of the REFN sub-record type group is used if the citation has been published.
            ///     
            ///     COLUMNS       DATA TYPE      FIELD              DEFINITION
            ///     -------------------------------------------------------------------------------
            ///      1 -  6       Record name    "JRNL  "
            ///     13 - 16       LString(4)     "REFN"
            ///     36 - 39       LString(4)     "ISSN" or          International Standard Serial Number or 
            ///                                  "ESSN"             Electronic Standard Serial Number.
            ///     41 - 65       LString        issn               ISSN number (final digit may be a
            ///                                                     letter and may contain one or 
            ///                                                     more dashes).
            /// 7. PMID
            ///     COLUMNS       DATA  TYPE     FIELD              DEFINITION
            ///     --------------------------------------------------------------------------------
            ///      1 -  6       Record  name   "JRNL  "
            ///     13 - 16       LString(4)     "PMID"
            ///     20 – 79       Integer        continuation       unique PubMed identifier number assigned to 
            ///                                                     the publication  describing the experiment.
            ///                                                     Allows  for a long PubMed ID number.
            /// 8. DOI
            ///     COLUMNS       DATA  TYPE     FIELD              DEFINITION
            ///     --------------------------------------------------------------------------------
            ///      1 -  6       Record  name   "JRNL  "
            ///     13 - 16       LString(4)     "DOI "
            ///     20 – 79       LString        continuation       Unique DOI assigned to the publication
            ///                                                     describing the experiment.
            #endregion
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// JRNL        AUTH   G.FERMI,M.F.PERUTZ,B.SHAANAN,R.FOURME                        
            /// JRNL        TITL   THE CRYSTAL STRUCTURE OF  HUMAN DEOXYHAEMOGLOBIN AT           
            /// JRNL        TITL 2 1.74 A RESOLUTION                                            
            /// JRNL        REF    J.MOL.BIOL.                   V. 175   159 1984               
            /// JRNL        REFN                   ISSN 0022-2836                               
            /// JRNL        PMID   6726807                                                      
            /// JRNL        DOI    10.1016/0022-2836(84)90472-8                                 
            /// 
            /// 


            public static string RecordName { get { return "JRNL  "; } }

            Jrnl(string line)
                : base(line)
			{
			}
			public static Jrnl FromString(string line)
			{
				HDebug.Assert(IsJrnl(line));
				return new Jrnl(line);
			}
            public static bool IsJrnl(string line) { return (line.Substring(0, 6) == "JRNL  "); }
            public string text { get { return String(idxs_text); } } static readonly int[] idxs_text = new int[] { 13, 79 }; // 13 - 79       LString        text          See Details below.           

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Jrnl(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Jrnl(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
