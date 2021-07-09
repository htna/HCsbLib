using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Seqres  = Pdb.Seqres;
    public static partial class HStaticBioinfo
    {
        public static Tuple<char,string[]>[] HGroupSeqres(this IList<Seqres> seqress)
        {
            Dictionary<char,Dictionary<int,Seqres>> chain_sernum_seqres = new Dictionary<char, Dictionary<int, Seqres>>();
            foreach(Seqres seqres in seqress)
            {
                if(chain_sernum_seqres.ContainsKey(seqres.chainID) == false)
                    chain_sernum_seqres.Add(seqres.chainID, new Dictionary<int, Seqres>());
                chain_sernum_seqres[seqres.chainID].Add(seqres.serNum, seqres);
            }

            List<Tuple<char,string[]>> lst_chain_resname = new List<Tuple<char, string[]>>();
            foreach(char chain in chain_sernum_seqres.Keys)
            {
                Dictionary<int,Seqres> sernum_seqres = chain_sernum_seqres[chain];
                List<string> resName = new List<string>();
                foreach(int sernum in sernum_seqres.Keys.ToArray().HSort())
                    resName.AddRange(sernum_seqres[sernum].resName);
                lst_chain_resname.Add(new Tuple<char, string[]>(chain, resName.ToArray()));
            }

            return lst_chain_resname.ToArray();
        }
    }
	public partial class Pdb
	{
        [Serializable]
        public class Seqres : Element, IBinarySerializable
		{
			/// http://www.wwpdb.org/documentation/format32/sect3.html#SEQRES
			///
            /// COLUMNS        DATA TYPE      FIELD        DEFINITION
            /// -------------------------------------------------------------------------------------
            ///  1 -  6        Record name    "SEQRES"
            ///  8 - 10        Integer        serNum       Serial number of the SEQRES record for  the
            ///                                            current  chain. Starts at 1 and increments
            ///                                            by one  each line. Reset to 1 for each chain.
            /// 12             Character      chainID      Chain identifier. This may be any single
            ///                                            legal  character, including a blank which is
            ///                                            is  used if there is only one chain.
            /// 14 - 17        Integer        numRes       Number of residues in the chain.
            ///                                            This  value is repeated on every record.
            /// 20 - 22        Residue name   resName      Residue name.
            /// 24 - 26        Residue name   resName      Residue name.
            /// 28 - 30        Residue name   resName      Residue name.
            /// 32 - 34        Residue name   resName      Residue name.
            /// 36 - 38        Residue name   resName      Residue name.
            /// 40 - 42        Residue name   resName      Residue name.
            /// 44 - 46        Residue name   resName      Residue name.
            /// 48 - 50        Residue name   resName      Residue name.
            /// 52 - 54        Residue name   resName      Residue name.
            /// 56 - 58        Residue name   resName      Residue name.
            /// 60 - 62        Residue name   resName      Residue name.
            /// 64 - 66        Residue name   resName      Residue name.
            /// 68 - 70        Residue name   resName      Residue name.
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// SEQRES   1 A   21  GLY ILE VAL GLU GLN CYS CYS THR SER ILE CYS SER LEU          
            /// SEQRES   2 A   21  TYR GLN LEU GLU ASN TYR CYS ASN                              
            /// SEQRES   1 B   30  PHE VAL ASN GLN HIS LEU CYS GLY SER HIS LEU VAL GLU          
            /// SEQRES   2 B   30  ALA LEU TYR LEU VAL CYS GLY GLU ARG GLY PHE PHE TYR          
            /// SEQRES   3 B   30  THR PRO LYS ALA                                              
            /// SEQRES   1 C   21  GLY ILE VAL GLU GLN CYS CYS THR SER ILE CYS SER LEU          
            /// SEQRES   2 C   21  TYR GLN LEU GLU ASN TYR CYS ASN                               
            /// SEQRES   1 D   30  PHE VAL ASN GLN HIS LEU CYS GLY SER HIS LEU VAL GLU          
            /// SEQRES   2 D   30  ALA LEU TYR LEU VAL CYS GLY GLU ARG GLY PHE PHE TYR          
            /// SEQRES   3 D   30  THR PRO LYS ALA
            /// SEQRES   1 A    8   DA  DA  DC  DC  DG  DG  DT  DT                              
            /// SEQRES   1 B    8   DA  DA  DC  DC  DG  DG  DT  DT 
            /// 
            /// SEQRES   1 X   39    U   C   C   C   C   C   G   U   G   C   C   C   A          
            /// SEQRES   2 X   39    U   A   G   C   G   G   C   G   U   G   G   A   A           
            /// SEQRES   3 X   39    C   C   A   C   C   C   G   U   U   C   C   C   A        

//			string Element.RecordName { get { return "SEQRES "; } }
			public static string RecordName { get { return "SEQRES "; } }
            Seqres(string line)
				: base(line)
			{
			}
			public static Seqres FromString(string line)
			{
				HDebug.Assert(IsSeqres(line));
				return new Seqres(line);
			}
			public static bool IsSeqres(string line) { return (line.Substring(0, 6) == "SEQRES"); }
                                                                              // COLUMNS        DATA TYPE      FIELD        DEFINITION
                                                                              // -------------------------------------------------------------------------------------
                                                                              //  1 -  6        Record name    "SEQRES"
            public    int serNum     { get { return Integer( 8,10).Value; } } //  8 - 10        Integer        serNum       Serial number of the SEQRES record for  the
                                                                              //                                            current  chain. Starts at 1 and increments
                                                                              //                                            by one  each line. Reset to 1 for each chain.
            public   char chainID    { get { return Char   (12   );       } } // 12             Character      chainID      Chain identifier. This may be any single
                                                                              //                                            legal  character, including a blank which is
                                                                              //                                            is  used if there is only one chain.
            public    int numRes     { get { return Integer(14,17).Value; } } // 14 - 17        Integer        numRes       Number of residues in the chain.
                                                                              //                                            This  value is repeated on every record.
            public string resName00  { get { return String (20,22);       } } // 20 - 22        Residue name   resName      Residue name.
            public string resName01  { get { return String (24,26);       } } // 24 - 26        Residue name   resName      Residue name.
            public string resName02  { get { return String (28,30);       } } // 28 - 30        Residue name   resName      Residue name.
            public string resName03  { get { return String (32,34);       } } // 32 - 34        Residue name   resName      Residue name.
            public string resName04  { get { return String (36,38);       } } // 36 - 38        Residue name   resName      Residue name.
            public string resName05  { get { return String (40,42);       } } // 40 - 42        Residue name   resName      Residue name.
            public string resName06  { get { return String (44,46);       } } // 44 - 46        Residue name   resName      Residue name.
            public string resName07  { get { return String (48,50);       } } // 48 - 50        Residue name   resName      Residue name.
            public string resName08  { get { return String (52,54);       } } // 52 - 54        Residue name   resName      Residue name.
            public string resName09  { get { return String (56,58);       } } // 56 - 58        Residue name   resName      Residue name.
            public string resName10  { get { return String (60,62);       } } // 60 - 62        Residue name   resName      Residue name.
            public string resName11  { get { return String (64,66);       } } // 64 - 66        Residue name   resName      Residue name.
            public string resName12  { get { return String (68,70);       } } // 68 - 70        Residue name   resName      Residue name.

            public string[] resName
            {
                get
                {
                    string[] list = new string[]
                    { 
                        resName00, resName01, resName02, resName03, resName04, resName05, resName06, resName07, resName08, resName09,
                        resName10, resName11, resName12,
                    };
                    list = list.HTrim();
                    list = list.HRemoveAll("", null);
                    return list;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Seqres(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Seqres(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
	}
}
