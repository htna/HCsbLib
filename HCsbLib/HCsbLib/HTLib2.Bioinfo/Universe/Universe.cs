using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        //public int          maxid        = -1;

        public Pdb      pdb;
        public Dictionary<string, HDynamic> refs = new Dictionary<string, HDynamic>();
        //public Namd.Psf psf;
        //public Namd.Prm prm;
        public Dictionary<string, HDynamic> lextra = new Dictionary<string, HDynamic>(); //public InfoPack extra = new InfoPack();

        public int size { get { return atoms.Count; } }
        public Atoms        atoms        = null;// new Atoms       ();
		public Bonds        bonds        = new Bonds       ();
		public Angles       angles       = new Angles      ();
		public Dihedrals    dihedrals    = new Dihedrals   ();
		public Impropers    impropers    = new Impropers   ();
        //public Nonbondeds   nonbondeds   = new Nonbondeds  ();  // do not make this list in advance, because it depends on the atom positions
        public HashSet<Atom>[] bondeds   = null;
        public HashSet<Atom>[] _rigids   = null;
        public HashSet<Atom> GetAtomsInRigid(Atom atom)
        {
            if(_rigids == null)
                _rigids = BuildRigids(this);
            return _rigids[atom.ID];
        }
        public Nonbonded14s nonbonded14s = new Nonbonded14s();
	}
}
