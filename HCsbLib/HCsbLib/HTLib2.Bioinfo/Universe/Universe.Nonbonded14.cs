using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using IEnumerator = System.Collections.IEnumerator;
    using IEnumerable = System.Collections.IEnumerable;

	public partial class Universe
	{
        public class Nonbonded14 : AtomPack
        {
            public Nonbonded14(Atom atom1, Atom atom2)
                : base(atom1, atom2) { }
        }
        public class Nonbonded14s
        {
            //List<Nonbonded14> nonbonded14s = null;
            Atoms atoms = null;

            public void Build(Atoms atoms)
            {
                this.atoms = atoms;
                //  nonbonded14s = new List<Nonbonded14>();
                //  foreach(Atom atom in atoms)
                //  {
                //      foreach(Atom inter14 in atom.Inter14)
                //      {
                //          if(atom.ID > inter14.ID)
                //              continue;
                //          nonbonded14s.Add(new Nonbonded14(atom, inter14));
                //      }
                //  }

                //  Dictionary<Tuple<int,int>, Nonbonded14> key_nonbonded14s = new Dictionary<Tuple<int, int>, Nonbonded14>();
                //  foreach(Atom atom in atoms)
                //  {
                //      foreach(Atom inter14 in atom.Inter14)
                //      {
                //          Atom atm1 = (atom.ID < inter14.ID) ? atom    : inter14;
                //          Atom atm2 = (atom.ID < inter14.ID) ? inter14 : atom   ;
                //          Tuple<int, int> key = new Tuple<int, int>(atm1.ID, atm2.ID);
                //          if(key_nonbonded14s.ContainsKey(key))
                //              continue;
                //          key_nonbonded14s.Add(key, new Nonbonded14(atm1, atm2));
                //      }
                //  }
                //  nonbonded14s = key_nonbonded14s.Values.ToList();
                //  foreach(Nonbonded14 nonbonded14 in nonbonded14s)
                //  {
                //      foreach(Atom atom in nonbonded14.atoms)
                //          atom.Nonbonded14s.Add(nonbonded14);
                //  }
            }
            public IEnumerator<Nonbonded14> GetEnumerator()
            {
                //return nonbonded14s.GetEnumerator();
                return GetEnumerable().GetEnumerator();
            }
            public bool Remove(Nonbonded14 item)
            {
                HDebug.Assert(false);
                return true;
                //return nonbonded14s.Remove(item);
            }
            public IEnumerable<Nonbonded14> GetEnumerable()
            {
                foreach(Atom atom in atoms)
                {
                    foreach(Atom inter14 in atom.Inter14)
                    {
                        if(atom.ID > inter14.ID)
                            continue;
                        yield return new Nonbonded14(atom, inter14);
                    }
                }
            }
        }
	}
}
