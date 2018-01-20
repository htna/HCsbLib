using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;

	public partial class Universe
	{
        public class AtomPack : System.IComparable<AtomPack>
        {
            public readonly Atom[] atoms;
            public readonly bool   sort;
            //public object          info;
            public AtomPack(Atom atom1, Atom atom2)
            {
                this.sort = true;
                HDebug.Assert(atom1.ID != atom2.ID);
                this.atoms = new Atom[2];
                this.atoms[0] = (atom1.ID < atom2.ID) ? atom1 : atom2;
                this.atoms[1] = (atom1.ID < atom2.ID) ? atom2 : atom1;
                HDebug.Assert(atoms[0].ID < atoms[1].ID);
            }
            public AtomPack(bool sort, params Atom[] atoms)
            {
                this.sort = sort;
                for(int i=0; i<atoms.Length-1; i++)
                    for(int j=i+1; j<atoms.Length; j++)
                    {
                        HDebug.Assert(atoms[i].ID != atoms[j].ID);
                        if(sort && (atoms[i].ID > atoms[j].ID))
                        {
                            Atom temp = atoms[i];
                            atoms[i] = atoms[j];
                            atoms[j] = temp;
                        }
                    }
                this.atoms = atoms;
                //Debug.Assert(atom1.ID != atom2.ID);
                //this.atoms = new Atom[2];
                //this.atoms[0] = (atom1.ID < atom2.ID) ? atom1 : atom2;
                //this.atoms[1] = (atom1.ID < atom2.ID) ? atom2 : atom1;
                //Debug.Assert(atoms[0].ID < atoms[1].ID);
            }
            public override string ToString()
            {
                StringBuilder str = new StringBuilder();
                str.Append("{" + atoms[0].ToString());
                for(int i=1; i<atoms.Length; i++)
                    str.Append("}, {" + atoms[i].ToString());
                str.Append("}");
                return str.ToString();
            }

            public bool Equals(AtomPack other)
            {
                if(ReferenceEquals(null, other)) return false;
                if(ReferenceEquals(this, other)) return true;
                if(sort         != other.sort        ) return false;
                if(atoms.Length != other.atoms.Length) return false;
                for(int i=0; i<atoms.Length; i++)
                    if(atoms[i] != other.atoms[i]) return false;
                //if(atoms[0] != other.atoms[0]) return false;
                //if(atoms[1] != other.atoms[1]) return false;
                throw new Exception("check AtomPack.Equals(AtomPack). If reference.equal == false, this should return false!!");
                return true;
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj)) return false;
                if(ReferenceEquals(this, obj)) return true;
                if(obj.GetType().IsInstanceOfType(typeof(AtomPack))) return false;
                return Equals((AtomPack)obj);
            }

            public override int GetHashCode()
            {
                int hash = Atom.GetHashCode(atoms);
                return hash;
                //int hash0 = atoms[0].GetHashCode();
                //int hash1 = atoms[1].GetHashCode();
                //return (hash0 + hash1);
            }

            public static bool operator ==(AtomPack left, AtomPack right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(AtomPack left, AtomPack right)
            {
                return !Equals(left, right);
            }
            int System.IComparable<AtomPack>.CompareTo(AtomPack other)
            {
                throw new HException(); // depreciated !!!
                //  if(atoms.Length < other.atoms.Length) return -1;
                //  if(atoms.Length > other.atoms.Length) return +1;
                //  for(int i=0; i<atoms.Length; i++)
                //  {
                //      if(atoms[i].ID < other.atoms[i].ID) return -1;
                //      if(atoms[i].ID > other.atoms[i].ID) return +1;
                //  }
                //  if(sort==false && other.sort==true ) return -1;
                //  if(sort==true  && other.sort==false) return +1;
                //  return 0;
                //  //if(this.atoms[0].ID < other.atoms[0].ID) return -1;
                //  //if(this.atoms[0].ID > other.atoms[0].ID) return +1;
                //  //if(this.atoms[1].ID < other.atoms[1].ID) return -1;
                //  //if(this.atoms[1].ID > other.atoms[1].ID) return +1;
                //  //return 0;
            }

            //////////////////////////////////////////////////////////////////////////////////////
            //// IEquatable<Pair<T, U>>
            //public bool Equals(NonbondedBase x, NonbondedBase y)
            //{
            //    if(x.atoms[0] != y.atoms[0]) return false;
            //    if(x.atoms[1] != y.atoms[1]) return false;
            //    return true;
            //}
            //public int GetHashCode(NonbondedBase obj)
            //{
            //    int hash0 = obj.atoms[0].GetHashCode();
            //    int hash1 = obj.atoms[1].GetHashCode();
            //    return (hash0 + hash1);
            //}
        }
    }
}
