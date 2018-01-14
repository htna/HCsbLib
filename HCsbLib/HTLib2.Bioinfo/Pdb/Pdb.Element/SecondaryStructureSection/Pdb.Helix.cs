using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Helix = Pdb.Helix;
    using IAtom = Pdb.IAtom;
    public static partial class PdbStatic
    {
        public static Tuple<Helix[], Atom[]>[] HSelectAtoms<Atom>(this IList<Helix> helixs, IList<Atom> atoms)
            where Atom : IAtom
        {
            var chain_resi_atoms = atoms.GroupChainIDResSeq();

            List<Tuple<Helix[], Atom[]>> list = new List<Tuple<Helix[], Atom[]>>();
            foreach(var helix in helixs)
            {
                HDebug.Exception(helix.initChainID == helix.endChainID);
                char chain = helix.initChainID;
                if(chain_resi_atoms.ContainsKey(chain) == false)
                    continue;
                var chainresi_atoms = chain_resi_atoms[chain];

                int[] resis;
                resis = new int[] { helix.initSeqNum, helix.endSeqNum };
                resis = resis.HSort();
                resis = HEnum.HEnumFromTo(resis[0], resis[1]).ToArray();

                List<Atom> helix_atoms = new List<Atom>();
                foreach(var resi in resis)
                {
                    if(chainresi_atoms.ContainsKey(resi) == false)
                        continue;
                    helix_atoms.AddRange(chainresi_atoms[resi]);
                }

                list.Add(new Tuple<Helix[], Atom[]>
                (
                    new Helix[] { helix },
                    helix_atoms.ToArray()
                ));
            }

            return list.ToArray();
        }
    }
    public partial class Pdb
	{
        [Serializable]
        public class Helix : Element
		{
			/// http://www.wwpdb.org/documentation/format32/sect5.html#HELIX
			///
			/// COLUMNS        DATA  TYPE     FIELD         DEFINITION
			/// -----------------------------------------------------------------------------------
			///  1 -  6        Record name    "HELIX "
			///  8 - 10        Integer        serNum        Serial number of the helix. This starts
			///                                             at 1  and increases incrementally.
			/// 12 - 14        LString(3)     helixID       Helix  identifier. In addition to a serial
			///                                             number, each helix is given an 
			///                                             alphanumeric character helix identifier.
			/// 16 - 18        Residue name   initResName   Name of the initial residue.
			/// 20             Character      initChainID   Chain identifier for the chain containing this helix.
			/// 22 - 25        Integer        initSeqNum    Sequence number of the initial residue.
			/// 26             AChar          initICode     Insertion code of the initial residue.
			/// 28 - 30        Residue  name  endResName    Name of the terminal residue of the helix.
			/// 32             Character      endChainID    Chain identifier for the chain containing this helix.
			/// 34 - 37        Integer        endSeqNum     Sequence number of the terminal residue.
			/// 38             AChar          endICode      Insertion code of the terminal residue.
			/// 39 - 40        Integer        helixClass    Helix class (see below).
			/// 41 - 70        String         comment       Comment about this helix.
			/// 72 - 76        Integer        length        Length of this helix.
            /// 
            ///                                      CLASS NUMBER
            /// TYPE OF  HELIX                     (COLUMNS 39 - 40)
            /// --------------------------------------------------------------
            /// Right-handed alpha (default)                1
            /// Right-handed omega                          2
            /// Right-handed pi                             3
            /// Right-handed gamma                          4
            /// Right-handed 3 - 10                         5
            /// Left-handed alpha                           6
            /// Left-handed omega                           7
            /// Left-handed gamma                           8
            /// 2 - 7 ribbon/helix                          9
            /// Polyproline                                10
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// HELIX    1  HA GLY A   86  GLY A   94  1                                   9   
            /// HELIX    2  HB GLY B   86  GLY B   94  1                                   9  
            ///  
            /// HELIX   21  21 PRO J  385  LEU J  388  5                                   4    
            /// HELIX   22  22 PHE J  397  PHE J  402  5                                   6   

//          string Element.RecordName { get { return "HELIX "; } }
            public static string RecordName { get { return "HELIX "; } }
            Helix(string line)
                : base(line)
            {
            }
            public static Helix FromString(string line)
            {
                HDebug.Assert(IsHelix(line));
                return new Helix(line);
            }
            public static bool IsHelix(string line) { return (line.Substring(0, 6) == "HELIX "); }
                                                                        // COLUMNS        DATA  TYPE     FIELD         DEFINITION
                                                                        // -----------------------------------------------------------------------------------
            public    int serNum     { get { return Integer( 8,10).Value; } } //  8 - 10        Integer        serNum        Serial number of the helix. This starts
                                                                              //                                             at 1  and increases incrementally.
            public string helixID    { get { return String (12,14);       } } // 12 - 14        LString(3)     helixID       Helix  identifier. In addition to a serial
                                                                              //                                             number, each helix is given an 
                                                                              //                                             alphanumeric character helix identifier.
            public string initResName{ get { return String (16,18);       } } // 16 - 18        Residue name   initResName   Name of the initial residue.
            public   char initChainID{ get { return Char   (20   );       } } // 20             Character      initChainID   Chain identifier for the chain containing this helix.
            public    int initSeqNum { get { return Integer(22,25).Value; } } // 22 - 25        Integer        initSeqNum    Sequence number of the initial residue.
            public   char initICode  { get { return Char   (26   );       } } // 26             AChar          initICode     Insertion code of the initial residue.
            public string endResName { get { return String (28,30);       } } // 28 - 30        Residue  name  endResName    Name of the terminal residue of the helix.
            public   char endChainID { get { return Char   (32   );       } } // 32             Character      endChainID    Chain identifier for the chain containing this helix.
            public    int endSeqNum  { get { return Integer(34,37).Value; } } // 34 - 37        Integer        endSeqNum     Sequence number of the terminal residue.
            public   char endICode   { get { return Char   (38   );       } } // 38             AChar          endICode      Insertion code of the terminal residue.
            public    int helixClass { get { return Integer(39,40).Value; } } // 39 - 40        Integer        helixClass    Helix class (see below).
            public string comment    { get { return String (41,70);       } } // 41 - 70        String         comment       Comment about this helix.
            public    int length     { get { return Integer(72,76).Value; } } // 72 - 76        Integer        length        Length of this helix.

            public PdbStatic.ResInfo init { get { return new PdbStatic.ResInfo { resName = initResName, chainID = initChainID, resSeq  = initSeqNum, iCode   = initICode }; } }
            public PdbStatic.ResInfo  end { get { return new PdbStatic.ResInfo { resName =  endResName, chainID =  endChainID, resSeq  =  endSeqNum, iCode   =  endICode }; } }

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Helix(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
	}
}
