using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class PdbStatic
    {
        public static IList<Tuple<int, int>> HListConectBonds(this IList<Pdb.Conect> conects)
        {
            HashSet<Tuple<int,int>> bonds = new HashSet<Tuple<int, int>>();
            foreach(Pdb.Conect conect in conects)
                bonds.UnionWith(conect.ListConectBonds());
            return bonds.ToList();
        }
        public static Dictionary<int, int[]> HDictConectAtoms(this IList<Pdb.Conect> conects)
        {
            Dictionary<int, int[]> dict = new Dictionary<int, int[]>();
            foreach(Pdb.Conect conect in conects)
            {
                foreach(var bond in conect.ListConectBonds())
                {
                    int ser1 = bond.Item1;
                    int ser2 = bond.Item2;

                    if(dict.ContainsKey(ser1) == false) dict.Add(ser1, new int[0]);
                    if(dict.ContainsKey(ser2) == false) dict.Add(ser2, new int[0]);
                    dict[ser1] = dict[ser1].HUnionWith(ser2);
                    dict[ser2] = dict[ser2].HUnionWith(ser1);
                }
            }
            return dict;
        }
        public static int[] ListSerial(this IList<Pdb.Conect> conects)
        {
            HashSet<int> serials = new HashSet<int>();
            foreach(var conect in conects)
            {
                serials.Add(conect.serial);
                foreach(var bond in conect.serialBondeds)
                    serials.Add(bond);
            }
            serials.Remove(-1);
            return serials.ToArray().HSort();
        }
    }
	public partial class Pdb
	{
        public List<Atom> ListAtomInConect()
        {
            IList<int> idConects = elements.ListType<Pdb.Conect>().ListSerial();
            IList<int> idAtoms   = atoms.ListSerial();
            IList<int> idCommon  = idConects.HListCommonT(idAtoms);
            return atoms.SelectBySerial(idCommon.ToArray());
        }
        public IAtom[] ListIAtomInConect()
        {
            IAtom[]    iatoms = this.iatoms;
            IList<int> idConects = elements.ListType<Pdb.Conect>().ListSerial();
            IList<int> idIAtoms  = iatoms.ListSerial();
            IList<int> idCommon  = idConects.HListCommonT(idIAtoms);
            return iatoms.SelectBySerial(idCommon.ToArray()).ToArray();
        }
        public IAtom[] ListHetatmInConect()
        {
            IList<int> idConects = elements.ListType<Pdb.Conect>().ListSerial();
            IList<int> idIAtoms  = hetatms.ListSerial();
            IList<int> idCommon  = idConects.HListCommonT(idIAtoms);
            return hetatms.SelectBySerial(idCommon.ToArray()).ToArray();
        }

        [Serializable]
        public class Conect : Element
		{
            /// http://www.wwpdb.org/documentation/format32/sect10.html
            /// 
            /// The CONECT records specify connectivity between atoms for which coordinates
            /// are supplied. The connectivity is described using the atom serial number as
            /// shown in the entry. CONECT records are mandatory for HET groups (excluding
            /// water) and for other bonds not specified in the standard residue connectivity
            /// table. These records are generated automatically.
			///
            /// COLUMNS       DATA  TYPE      FIELD        DEFINITION
            /// -------------------------------------------------------------------------
            ///  1 -  6        Record name    "CONECT"
            ///  7 - 11        Integer        serial       Atom  serial number
            /// 12 - 16        Integer        serial       Serial number of bonded atom
            /// 17 - 21        Integer        serial       Serial number of bonded atom
            /// 22 - 26        Integer        serial       Serial number of bonded atom
            /// 27 - 31        Integer        serial       Serial number of bonded atom
            /// 
            #region Details
            /// Details
            /// * CONECT records are present for:
            ///    - Intra-residue connectivity within  non-standard (HET) residues (excluding water).
            ///    - Inter-residue connectivity of HET  groups to standard groups (including water) or
            ///      to other HET groups.
            ///    - Disulfide bridges specified in the  SSBOND records have corresponding records.
            /// * No differentiation is made between atoms with delocalized charges (excess negative or
            ///   positive charge).
            /// * Atoms specified in the CONECT records have the same numbers as given in the coordinate
            ///   section.
            /// * All atoms connected to the atom with serial number in columns 7 - 11 are listed in the
            ///   remaining fields of the record.
            /// * If more than four fields are required for non-hydrogen and non-salt bridges, a second
            ///   CONECT record with the same atom serial number in columns 7 - 11 will be used.
            /// * These CONECT records occur in increasing order of the atom serial numbers they carry
            ///   in columns 7 - 11. The target-atom serial numbers carried on these records also occur
            ///   in increasing order.
            /// * The connectivity list given here is redundant in that each bond indicated is given twice,
            ///   once with each of the two atoms involved specified in columns 7 - 11.
            /// * For hydrogen bonds, when the hydrogen atom is present in the coordinates, a CONECT
            ///   record between the hydrogen atom and its acceptor atom is generated.
            /// * For NMR entries, CONECT records for one model are generated describing heterogen
            ///   connectivity and others for LINK records assuming that all models are homogeneous models.
            /// 
            /// Verification/Validation/Value Authority Control
            ///   Connectivity is checked for unusual bond lengths.
            /// 
            /// Relationships to Other Record Types
            ///   CONECT records must be present in an entry that contains either non-standard groups or disulfide bonds.
            ///   
            /// Known Problems
            ///   CONECT records involving atoms for which the coordinates are not present in the entry (e.g., symmetry-generated) are not given.
            ///   CONECT records involving atoms for which the coordinates are missing due to disorder, are also not provided.
            #endregion
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// CONECT 1179  746 1184 1195 1203                    
            /// CONECT 1179 1211 1222                              
            /// CONECT 1021  544 1017 1020 1022

            public static string RecordName { get { return "CONECT"; } }

            Conect(string line)
                : base(line)
			{
			}
			public static Conect FromString(string line)
			{
				HDebug.Assert(IsConect(line));
				return new Conect(line);
			}
            public static bool IsConect(string line) { return (line.Substring(0, 6) == "CONECT"); }
            public    int serial        { get { return Integer( 7,11    ).Value; } } //  7 - 11        Integer        serial       Atom  serial number
            public    int serialBonded1 { get { return Integer(12,16    ).Value; } } // 12 - 16        Integer        serial       Serial number of bonded atom
            public    int serialBonded2 { get { return Integer(17,21, -1);       } } // 17 - 21        Integer        serial       Serial number of bonded atom // return -1 if failed
            public    int serialBonded3 { get { return Integer(22,26, -1);       } } // 22 - 26        Integer        serial       Serial number of bonded atom // return -1 if failed
            public    int serialBonded4 { get { return Integer(27,31, -1);       } } // 27 - 31        Integer        serial       Serial number of bonded atom // return -1 if failed
            public  int[] serialBondeds { get { return (new int[] { serialBonded1, serialBonded2, serialBonded3, serialBonded4 }).HRemoveAll(-1); } }

            public List<Tuple<int,int>> ListConectBonds()
            {
                List<Tuple<int,int>> bonds = new List<Tuple<int, int>>();
                int serial = this.serial;
                int[] serialBondeds = new []{serialBonded1, serialBonded2, serialBonded3, serialBonded4};
                foreach(int serialBonded in serialBondeds)
                {
                    if(serialBonded == -1) continue;
                    int serial1 = Math.Min(serial, serialBonded);
                    int serial2 = Math.Max(serial, serialBonded);
                    bonds.Add(new Tuple<int,int>(serial1, serial2));
                }
                return bonds;
            }

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Conect(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
	}
}
