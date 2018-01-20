using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public class Improper : AtomPack
		{
            //public readonly Namd.Prm.Improper param;
            public readonly object[] sources;
            // !V(improper) = Kpsi(psi - psi0)**2
            public readonly double Kpsi;    // Kpsi: kcal/mole/rad**2
            public readonly int    n = 0;
            public readonly double psi0;    // psi0: degrees
            public Improper( Atom atom0, Atom atom1, Atom atom2, Atom atom3
                           , double Kpsi, double psi0
                           , params object[] sources)
                : base(false, atom0, atom1, atom2, atom3)
            {
                this.Kpsi = Kpsi;
                this.psi0 = psi0;
                this.sources = sources;
            }
            public Improper(Atom atom0, Atom atom1, Atom atom2, Atom atom3, Namd.Prm.Improper param)
                : base(false, atom0, atom1, atom2, atom3)
			{
                this.Kpsi = param.Kpsi;
                this.psi0 = param.psi0;
                this.sources = new object[] { param };
			}
            public override string ToString()
            {
                string str = "";
                HDebug.Assert(atoms.Length == 4);
                str += "{"+string.Format("{0},{1},{2},{3}", atoms[0].ID, atoms[1].ID, atoms[2].ID, atoms[3].ID) +"} - ";
                str += string.Format("Kψ({0:0.0})", Kpsi);
                str += string.Format(",ψ0({0:0.0})", psi0);
                str += " - " + base.ToString();
                return str;
            }
        }
        public class Impropers : List<Improper>
        {
            public double[] ListKpsi()
            {
                List<double> lstKpsi = new List<double>();
                foreach(Improper improper in this)
                    lstKpsi.Add(improper.Kpsi);
                return lstKpsi.ToArray();
            }
            public new void Add(Improper improper)
            {
                HDebug.Assert(improper.atoms.Length == 4);
                //HDebug.Assert(improper.atoms[0].ID < improper.atoms[3].ID);
                //HDebug.Assert(this.Contains(dihedral) == false); //
                base.Add(improper);
            }
        }
	}
}
