using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public class Angle : AtomPack
		{
            //public readonly Namd.Prm.Angle param;
            public readonly object[] sources;
            // V(angle) = Ktheta(Theta - Theta0)**2
            // V(Urey-Bradley) = Kub(S - S0)**2
            public readonly double Ktheta;  // Ktheta: kcal/mole/rad**2
			public readonly double Theta0;  // Theta0: degrees
			public readonly double Kub   ;  // Kub   : kcal/mole/A**2 (Urey-Bradley)
			public readonly double S0    ;  // S0    : A
            public Angle( Atom atom0, Atom atom1, Atom atom2
                        , double Ktheta, double Theta0, double Kub, double S0    
                        , params object[] sources)
                : base(false, atom0, atom1, atom2)
            {
                this.Ktheta  = Ktheta;
                this.Theta0  = Theta0;
                this.Kub     = Kub   ;
                this.S0      = S0    ;
                this.sources = sources;
            }
            public Angle(Atom atom0, Atom atom1, Atom atom2, Namd.Prm.Angle param)
                : base(false, atom0, atom1, atom2)
			{
                this.Ktheta  = param.Ktheta;
                this.Theta0  = param.Theta0;
                this.Kub     = param.Kub   ;
                this.S0      = param.S0    ;
                this.sources = new object[] { param };
			}
            public override string ToString()
            {
                string str = "";
                HDebug.Assert(atoms.Length == 3);
                str += "{"+string.Format("{0},{1},{2}", atoms[0].ID, atoms[1].ID, atoms[2].ID) +"} - ";
                str += string.Format("Kθ({0:0.0})", Ktheta);
                str += string.Format(",θ0({0:0.0})", Theta0);
                str += string.Format(",Kub({0:0.0})", Kub);
                str += string.Format(",S0({0:0.0})", S0);
                str += " - " + base.ToString();
                return str;
            }
        }
        public class Angles : List<Angle>
        {
            public double[] ListKtheta()
            {
                List<double> lstKtheta = new List<double>();
                foreach(Angle angle in this)
                    lstKtheta.Add(angle.Ktheta);
                return lstKtheta.ToArray();
            }
            public double[] ListKub()
            {
                List<double> lstKub = new List<double>();
                foreach(Angle angle in this)
                    lstKub.Add(angle.Kub);
                return lstKub.ToArray();
            }
            //HashSet<Angle> _dbgangles = (HDebug.IsDebuggerAttached ? new HashSet<Angle>() : null);
            public new void Add(Angle angle)
            {
                HDebug.Assert(angle.atoms.Length == 3);
                //HDebug.Assert(angle.atoms[0].ID < angle.atoms[2].ID);
                //HDebug.Assert(this.Contains(angle) == false);
                //if(HDebug.IsDebuggerAttached)
                //{
                //    HDebug.Assert(_dbgangles.Contains(angle) == false);
                //    _dbgangles.Add(angle);
                //}
                base.Add(angle);
            }
        }
	}
}
