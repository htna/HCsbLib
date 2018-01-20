using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public static HashSet<Atom>[] BuildRigids(Universe univ)
        {
            List<HashSet<Atom>> rigidgroups;
            {
                rigidgroups = new List<HashSet<Atom>>();
                //List<Atom[]> rigidgroups = univ.FindRigidUnits();
                foreach(Atom[] atoms in univ.FindRigidUnits())
                {
                    HashSet<Atom> rigidgroup = new HashSet<Atom>(atoms);
                    {
                        /// When rigid group is "ghij" and its rigid structure is same to the below,
                        /// add "f","c","k" also (which are the bonded atoms of "ghij") into the rigid group.
                        /// 
                        ///     def      klm
                        ///        \    /
                        ///   abc---ghij
                        ///
                        foreach(Atom atom in atoms)
                        {
                            foreach(Bond bond in atom.Bonds)
                            {
                                rigidgroup.Add(bond.atoms[0]);
                                rigidgroup.Add(bond.atoms[1]);
                            }
                        }
                    }
                    rigidgroups.Add(rigidgroup);
                }
            }

            HashSet<Atom>[] rigids = new HashSet<Atom>[univ.size];
            foreach(HashSet<Atom> rigidgroup in rigidgroups)
            {
                foreach(Atom atom in rigidgroup)
                {
                    if(rigids[atom.ID] == null)
                    {
                        rigids[atom.ID] = rigidgroup;
                    }
                    else
                    {
                        /// When the case of "g", it is belonged to three rigid groups "abc", "def", and "ghij".
                        /// In this case, the atom "g" has a unique rigid groups, which combines all them.
                        IEnumerable<Atom> union = Enumerable.Union(rigids[atom.ID], rigidgroup);
                        rigids[atom.ID] = new HashSet<Atom>(union);
                    }
                }
            }

            return rigids;
        }
	}
}
