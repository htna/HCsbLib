using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Sheet = Pdb.Sheet;
    using IAtom = Pdb.IAtom;
    public static partial class PdbStatic
    {
        public static Dictionary<string,Sheet[]> HGroupBySheetID(this IList<Sheet> sheets)
        {
            Dictionary<string, List<Sheet>> id_sheet = new Dictionary<string, List<Sheet>>();
            foreach(var sheet in sheets)
            {
                if(id_sheet.ContainsKey(sheet.sheetID) == false)
                    id_sheet.Add(sheet.sheetID, new List<Sheet>());
                id_sheet[sheet.sheetID].Add(sheet);
            }
            return id_sheet.HToArray();
        }
        public static Tuple<Sheet[], Atom[]>[] HSelectAtoms<Atom>(this IList<Sheet> sheets, IList<Atom> atoms)
            where Atom : IAtom
        {
            var chain_resi_atoms = atoms.GroupChainIDResSeq();
            var id_sheets = sheets.HGroupBySheetID();

            List<Tuple<Sheet[], Atom[]>> list = new List<Tuple<Sheet[], Atom[]>>();
            foreach(string id in id_sheets.Keys)
            {
                Sheet[] idsheets = id_sheets[id];

                List<Atom> idsheets_atoms = new List<Atom>();
                foreach(var sheet in idsheets)
                {
                    HDebug.Exception(sheet.initChainID == sheet.endChainID);
                    char chain = sheet.initChainID;
                    if(chain_resi_atoms.ContainsKey(chain) == false)
                        continue;
                    var chainresi_atoms = chain_resi_atoms[chain];
                    
                    int[] resis;
                    resis = new int[] { sheet.initSeqNum, sheet.endSeqNum };
                    resis = resis.HSort();
                    resis = HEnum.HEnumFromTo(resis[0], resis[1]).ToArray();

                    foreach(var resi in resis)
                    {
                        if(chainresi_atoms.ContainsKey(resi) == false)
                            continue;
                        idsheets_atoms.AddRange(chainresi_atoms[resi]);
                    }
                }

                list.Add(new Tuple<Sheet[], Atom[]>
                (
                    idsheets,
                    idsheets_atoms.ToArray()
                ));
            }

            return list.ToArray();
        }
    }
	public partial class Pdb
	{
        [Serializable]
        public class Sheet : Element, IBinarySerializable
		{
			/// http://www.wwpdb.org/documentation/format32/sect5.html#SHEET
			///
			///COLUMNS       DATA  TYPE     FIELD          DEFINITION
			///-------------------------------------------------------------------------------------
			/// 1 -  6        Record name   "SHEET "
			/// 8 - 10        Integer       strand         Strand  number which starts at 1 for each
			///                                            strand within a sheet and increases by one.
			///12 - 14        LString(3)    sheetID        Sheet  identifier.
			///15 - 16        Integer       numStrands     Number  of strands in sheet.
			///18 - 20        Residue name  initResName    Residue  name of initial residue.
			///22             Character     initChainID    Chain identifier of initial residue in strand. 
			///23 - 26        Integer       initSeqNum     Sequence number of initial residue in strand.
			///27             AChar         initICode      Insertion code of initial residue in strand.
			///29 - 31        Residue name  endResName     Residue name of terminal residue.
			///33             Character     endChainID     Chain identifier of terminal residue.
			///34 - 37        Integer       endSeqNum      Sequence number of terminal residue.
			///38             AChar         endICode       Insertion code of terminal residue.
			///39 - 40        Integer       sense          Sense of strand with respect to previous
			///                                            strand in the sheet. 0 if first strand,
			///                                            1 if  parallel,and -1 if anti-parallel.
			///42 - 45        Atom          curAtom        Registration.  Atom name in current strand.
			///46 - 48        Residue name  curResName     Registration.  Residue name in current strand
			///50             Character     curChainId     Registration. Chain identifier in current strand.
			///51 - 54        Integer       curResSeq      Registration.  Residue sequence number in current strand.
			///55             AChar         curICode       Registration. Insertion code in current strand.
			///57 - 60        Atom          prevAtom       Registration.  Atom name in previous strand.
			///61 - 63        Residue name  prevResName    Registration.  Residue name in previous strand.
			///65             Character     prevChainId    Registration.  Chain identifier in previous  strand.
			///66 - 69        Integer       prevResSeq     Registration. Residue sequence number in previous strand.
			///70             AChar         prevICode      Registration.  Insertion code in
            /// 
            /// Examples
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// SHEET    1   A 5 THR A 107  ARG A 110  0
            /// SHEET    2   A 5 ILE A  96  THR A  99 -1  N  LYS A  98   O  THR A 107
            /// SHEET    3   A 5 ARG A  87  SER A  91 -1  N  LEU A  89   O  TYR A  97
            /// SHEET    4   A 5 TRP A  71  ASP A  75 -1  N  ALA A  74   O  ILE A  88
            /// SHEET    5   A 5 GLY A  52  PHE A  56 -1  N  PHE A  56   O  TRP A  71
            /// SHEET    1   B 5 THR B 107  ARG B 110  0
            /// SHEET    2   B 5 ILE B  96  THR B  99 -1  N  LYS B  98   O  THR B 107
            /// SHEET    3   B 5 ARG B  87  SER B  91 -1  N  LEU B  89   O  TYR B  97
            /// SHEET    4   B 5 TRP B  71  ASP B  75 -1  N  ALA B  74   O  ILE B  88
            /// SHEET    5   B 5 GLY B  52  ILE B  55 -1  N  ASP B  54   O  GLU B  73
            /// 
            /// The sheet presented as BS1 below is an eight-stranded beta-barrel. This is represented
            /// by a nine-stranded sheet in which the first and last strands are identical. 
            /// SHEET    1 BS1 9  VAL   13  ILE    17  0                               
            /// SHEET    2 BS1 9  ALA   70  ILE    73  1  O  TRP    72   N  ILE    17  
            /// SHEET    3 BS1 9  LYS  127  PHE   132  1  O  ILE   129   N  ILE    73  
            /// SHEET    4 BS1 9  GLY  221  ASP   225  1  O  GLY   221   N  ILE   130  
            /// SHEET    5 BS1 9  VAL  248  GLU   253  1  O  PHE   249   N  ILE   222  
            /// SHEET    6 BS1 9  LEU  276  ASP   278  1  N  LEU   277   O  GLY   252  
            /// SHEET    7 BS1 9  TYR  310  THR   318  1  O  VAL   317   N  ASP   278  
            /// SHEET    8 BS1 9  VAL  351  TYR   356  1  O  VAL   351   N  THR   318  
            /// SHEET    9 BS1 9  VAL   13  ILE    17  1  N  VAL    14   O  PRO   352  
            /// 
            /// The sheet structure of this example is bifurcated. In order to represent this feature,
            /// two sheets are defined. Strands 2 and 3 of BS7 and BS8 are identical.
            /// SHEET    1 BS7 3  HIS  662  THR   665  0                               
            /// SHEET    2 BS7 3  LYS  639  LYS   648 -1  N  PHE   643   O  HIS   662  
            /// SHEET    3 BS7 3  ASN  596  VAL   600 -1  N  TYR   598   O  ILE   646  
            /// SHEET    1 BS8 3  ASN  653  TRP   656  0                               
            /// SHEET    2 BS8 3  LYS  639  LYS   648 -1  N  LYS   647   O  THR   655  
            /// SHEET    3 BS8 3  ASN  596  VAL   600 -1  N  TYR   598   O  ILE   646  




//			public override string Element.RecordName { get { return "SHEET "; } }
			public static string RecordName { get { return "SHEET "; } }
            Sheet(string line)
				: base(line)
			{
			}
            public static Sheet FromString(string line)
			{
				HDebug.Assert(IsSheet(line));
				return new Sheet(line);
			}
            public static bool IsSheet(string line) { return (line.Substring(0, 6) == "SHEET "); }
            public    int strand     { get { return Integer( 8,10).Value; } } // 8 - 10        Integer       strand         Strand  number which starts at 1 for each
                                                                              //                                            strand within a sheet and increases by one.
            public string sheetID    { get { return String (12,14);       } } //12 - 14        LString(3)    sheetID        Sheet  identifier.
            public    int numStrands { get { return Integer(15,16).Value; } } //15 - 16        Integer       numStrands     Number  of strands in sheet.
            public string initResName{ get { return String (18,20);       } } //18 - 20        Residue name  initResName    Residue  name of initial residue.
            public   char initChainID{ get { return Char   (22   );       } } //22             Character     initChainID    Chain identifier of initial residue in strand. 
            public    int initSeqNum { get { return Integer(23,26).Value; } } //23 - 26        Integer       initSeqNum     Sequence number of initial residue in strand.
            public   char initICode  { get { return Char   (27   );       } } //27             AChar         initICode      Insertion code of initial residue in strand.
            public string endResName { get { return String (29,31);       } } //29 - 31        Residue name  endResName     Residue name of terminal residue.
            public   char endChainID { get { return Char   (33   );       } } //33             Character     endChainID     Chain identifier of terminal residue.
            public    int endSeqNum  { get { return Integer(34,37).Value; } } //34 - 37        Integer       endSeqNum      Sequence number of terminal residue.
            public   char endICode   { get { return Char   (38   );       } } //38             AChar         endICode       Insertion code of terminal residue.
            public    int sense      { get { return Integer(39,40).Value; } } //39 - 40        Integer       sense          Sense of strand with respect to previous
                                                                              //                                            strand in the sheet. 0 if first strand,
                                                                              //                                            1 if  parallel,and -1 if anti-parallel.
            public string curAtom    { get { return String (42,45);       } } //42 - 45        Atom          curAtom        Registration.  Atom name in current strand.
            public string curResName { get { return String (46,48);       } } //46 - 48        Residue name  curResName     Registration.  Residue name in current strand
            public   char curChainId { get { return Char   (50   );       } } //50             Character     curChainId     Registration. Chain identifier in current strand.
            public    int curResSeq  { get { return Integer(51,54).Value; } } //51 - 54        Integer       curResSeq      Registration.  Residue sequence number in current strand.
            public   char curICode   { get { return Char   (55   );       } } //55             AChar         curICode       Registration. Insertion code in current strand.
            public string prevAtom   { get { return String (57,60);       } } //57 - 60        Atom          prevAtom       Registration.  Atom name in previous strand.
            public string prevResName{ get { return String (61,63);       } } //61 - 63        Residue name  prevResName    Registration.  Residue name in previous strand.
            public   char prevChainId{ get { return Char   (65   );       } } //65             Character     prevChainId    Registration.  Chain identifier in previous  strand.
            public    int prevResSeq { get { return Integer(66,69).Value; } } //66 - 69        Integer       prevResSeq     Registration. Residue sequence number in previous strand.
            public   char prevICode  { get { return Char   (70   );       } } //70             AChar         prevICode      Registration.  Insertion code in

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Sheet(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
            ////////////////////////////////////////////////////////////////////////////////////
            // Serializable
            public Sheet(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
	}
}
