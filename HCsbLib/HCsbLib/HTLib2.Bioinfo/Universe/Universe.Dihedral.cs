using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public class Dihedral : AtomPack
		{
            //public readonly Namd.Prm.Dihedral param;
            public readonly object[] sources;
            // !V(dihedral) = Kchi(1 + cos(n(chi) - delta))
			public double Kchi ;    // Kchi : kcal/mole
			public double n    ;    // n    : multiplicity
            public double delta;    // delta: degrees
            public Dihedral( Atom atom0, Atom atom1, Atom atom2, Atom atom3
                           , double Kchi, double n, double delta
                           , params object[] sources)
                : base(false, atom0, atom1, atom2, atom3)
            {
                this.Kchi  = Kchi;
                this.n     = n;
                this.delta = delta;
                this.sources = sources;
            }
            public Dihedral(Atom atom0, Atom atom1, Atom atom2, Atom atom3, Namd.Prm.Dihedral param)
                : base(false, atom0, atom1, atom2, atom3)
			{
                this.Kchi  = param.Kchi;
                this.n     = param.n;
                this.delta = param.delta;
                this.sources = new object[] { param };
			}
            public override string ToString()
            {
                string str = "";
                HDebug.Assert(atoms.Length == 4);
                str += "{"+string.Format("{0},{1},{2},{3}", atoms[0].ID, atoms[1].ID, atoms[2].ID, atoms[3].ID) +"} - ";
                str += string.Format("Kϕ({0:0.0})", Kchi);
                str += string.Format(",ϕ0({0:0.0})", delta);
                str += string.Format(",n({0})", n);
                str += " - " + base.ToString();
                return str;
            }
        }
        public class Dihedrals : List<Dihedral>
        {
            public Dictionary<double,List<double>> DictNKchi()
            {
                Dictionary<double,List<double>> n_Kchi = new Dictionary<double,List<double>>();
                foreach(Dihedral dihedral in this)
                {
                    if(n_Kchi.ContainsKey(dihedral.n) == false)
                        n_Kchi.Add(dihedral.n, new List<double>());
                    n_Kchi[dihedral.n].Add(dihedral.Kchi);
                }
                return n_Kchi;
            }
            public new void Add(Dihedral dihedral)
            {
                HDebug.Assert(dihedral.atoms.Length == 4);
                HDebug.Assert(dihedral.atoms[0].ID < dihedral.atoms[3].ID);
                //HDebug.Assert(this.Contains(dihedral) == false); //
                base.Add(dihedral);
            }
        }
	}
}
