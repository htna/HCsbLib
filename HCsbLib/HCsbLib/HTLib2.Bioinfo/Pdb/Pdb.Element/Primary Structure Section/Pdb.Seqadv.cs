using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Seqadv  = Pdb.Seqadv;
    public static partial class HStaticBioinfo
    {
    }
	public partial class Pdb
	{
        [Serializable]
        public class Seqadv : Element, IBinarySerializable
		{
			/// http://www.wwpdb.org/documentation/format32/sect3.html#SEQADV
			///
            /// The SEQADV record identifies differences between sequence information in the SEQRES records
            /// of the PDB entry and the sequence database entry given in DBREF.  Please note that these
            /// records were designed to identify differences and not errors.  No assumption is made as to
            /// which database contains the correct data.  A comment explaining any engineered differences
            /// in the sequence between the PDB and the sequence database may also be included here.
            /// 
            /// COLUMNS        DATA TYPE     FIELD         DEFINITION
            /// -----------------------------------------------------------------
            ///  1 -  6        Record name   "SEQADV"
            ///  8 - 11        IDcode        idCode        ID code of this entry.
            /// 13 - 15        Residue name  resName       Name of the PDB residue in conflict.
            /// 17             Character     chainID       PDB chain identifier.
            /// 19 - 22        Integer       seqNum        PDB sequence number.
            /// 23             AChar         iCode         PDB insertion code.
            /// 25 - 28        LString       database
            /// 30 - 38        LString       dbIdCode      Sequence database accession number.
            /// 40 - 42        Residue name  dbRes         Sequence database residue name.
            /// 44 - 48        Integer       dbSeq         Sequence database sequence number.
            /// 50 - 70        LString       conflict      Conflict comment.
            /// 
            /// Examples
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// SEQADV 3ABC MET A   -1  UNP  P10725              EXPRESSION TAG
            /// SEQADV 3ABC GLY A   50  UNP  P10725    VAL    50 ENGINEERED
            /// SEQADV 2QLE CRO A   66  UNP  P42212    SER    65 CHROMOPHORE
            /// SEQADV 2OKW LEU A   64  UNP  P42212    PHE    64 SEE REMARK 999  
            /// SEQADV 2OKW LEU A   64  NOR  NOR00669  PHE    14 SEE REMARK 999

//			string Element.RecordName { get { return "SEQADV "; } }
			public static string RecordName { get { return "SEQADV "; } }
            Seqadv(string line)
				: base(line)
			{
			}
			public static Seqadv FromString(string line)
			{
				HDebug.Assert(IsSeqadv(line));
				return new Seqadv(line);
			}
			public static bool IsSeqadv(string line) { return (line.Substring(0, 6) == "SEQADV"); }
                                                                              // COLUMNS        DATA TYPE     FIELD         DEFINITION
                                                                              // -----------------------------------------------------------------
                                                                              //  1 -  6        Record name   "SEQADV"
            public string idCode     { get { return String ( 8,11);       } } //  8 - 11        IDcode        idCode        ID code of this entry.
            public string resName    { get { return String (13,15);       } } // 13 - 15        Residue name  resName       Name of the PDB residue in conflict.
            public   char chainID    { get { return Char   (17   );       } } // 17             Character     chainID       PDB chain identifier.
            public    int seqNum     { get { return Integer(19,22).Value; } } // 19 - 22        Integer       seqNum        PDB sequence number.
            public   char iCode      { get { return Char   (23   );       } } // 23             AChar         iCode         PDB insertion code.
            public string database   { get { return String (25,28);       } } // 25 - 28        LString       database
            public string dbIdCode   { get { return String (30,38);       } } // 30 - 38        LString       dbIdCode      Sequence database accession number.
            public string dbRes      { get { return String (40,42);       } } // 40 - 42        Residue name  dbRes         Sequence database residue name.
            public    int dbSeq      { get { return Integer(44,48).Value; } } // 44 - 48        Integer       dbSeq         Sequence database sequence number.
            public string conflict   { get { return String (50,70);       } } // 50 - 70        LString       conflict      Conflict comment.

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Seqadv(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Seqadv(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
	}
}
