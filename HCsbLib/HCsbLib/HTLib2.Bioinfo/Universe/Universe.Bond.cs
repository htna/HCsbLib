using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class UniverseStatic
    {
        public static IList<Universe.Bond> ListDisulfideBond(this IList<Universe.Bond> bonds)
        {
            return bonds.HSelect(delegate(Universe.Bond bond)
            {
                return bond.IsDisulfideBond();
            });
        }
        public static IList<Universe.Bond> ListBackbond(this IList<Universe.Bond> bonds)
        {
            return bonds.HSelect(delegate(Universe.Bond bond)
            {
                return (bond.IsBackbone() != null);
            });
        }
        public static IList<Universe.Bond> ListInterResiBond(this IList<Universe.Bond> bonds)
        {
            return bonds.HSelect(delegate(Universe.Bond bond)
            {
                return bond.IsInterResiBond();
            });
        }
    }
	public partial class Universe
	{
        public class Bond : AtomPack
		{
            //public readonly Namd.Prm.Bond param;
            public readonly object[] sources;
            // V(bond) = Kb(b - b0)**2
            // Kb: kcal/mole/A**2
            // b0: A
            public readonly double Kb;
            public readonly double b0;
            public Bond(Atom atom0, Atom atom1
                       , double Kb, double b0
                       , params object[] sources)
                : base(false, atom0, atom1)
			{
                //this.param = param;
                this.Kb = Kb;
                this.b0 = b0;
                this.sources = sources;
            }
            public Bond(Atom atom0, Atom atom1, Namd.Prm.Bond param)
                : base(false, atom0, atom1)
			{
                //this.param = param;
                this.Kb = param.Kb;
                this.b0 = param.b0;
                this.sources = new object[] { param };
            }
            public bool? IsDoubleBond()
            {
                HDebug.Assert(atoms.Length == 2);
                int val0 = atoms[0].GetValence();
                int bnd0 = atoms[0].Bonds.Count;
                if(val0 == bnd0) return false;
                if((val0 >= 2) && bnd0 == 1) return true;

                int val1 = atoms[1].GetValence();
                int bnd1 = atoms[1].Bonds.Count;
                if(val1 == bnd1) return false;
                if((val1 >= 2) && bnd1 == 1) return true;

                return null;
            }
            public override string ToString()
            {
                string str = "";
                HDebug.Assert(atoms.Length == 2);
                str += "{"+string.Format("{0},{1}", atoms[0].ID, atoms[1].ID) +"} - ";
                str += string.Format("Kb({0:0.0})", Kb);
                str += string.Format(",b0({0:0.0})", b0);
                str += " - " + base.ToString();
                return str;
            }
            public Tuple<string, Atom, Atom> IsBackbone()
            {
                Atom atom0 = atoms[0]; string name0 = atom0.AtomName;
                Atom atom1 = atoms[1]; string name1 = atom1.AtomName;
                switch(name0 +"-"+ name1)
                {
                    case "CA-N": HDebug.Assert(name1=="N" ); HDebug.Assert(name0=="CA"); return new Tuple<string, Universe.Atom, Universe.Atom>("N-CA", atom1, atom0);
                    case "N-CA": HDebug.Assert(name0=="N" ); HDebug.Assert(name1=="CA"); return new Tuple<string, Universe.Atom, Universe.Atom>("N-CA", atom0, atom1);
                    case "CA-C": HDebug.Assert(name0=="CA"); HDebug.Assert(name1=="C" ); return new Tuple<string, Universe.Atom, Universe.Atom>("CA-C", atom0, atom1);
                    case "C-CA": HDebug.Assert(name1=="CA"); HDebug.Assert(name0=="C" ); return new Tuple<string, Universe.Atom, Universe.Atom>("CA-C", atom1, atom0);
                    case "C-N ": HDebug.Assert(name0=="C" ); HDebug.Assert(name1=="N" ); return new Tuple<string, Universe.Atom, Universe.Atom>("C-N" , atom0, atom1);
                    case "N-N ": HDebug.Assert(name1=="C" ); HDebug.Assert(name0=="N" ); return new Tuple<string, Universe.Atom, Universe.Atom>("C-N" , atom1, atom0);
                }
                return null;
            }
            public bool IsDisulfideBond()
            {
                Atom atom0 = atoms[0]; string name0 = atom0.AtomName; if(name0[0] != 'S') return false;
                Atom atom1 = atoms[1]; string name1 = atom1.AtomName; if(name1[0] != 'S') return false;
                return true;
            }
            public bool IsInterResiBond()
            {
                HDebug.Assert(atoms.Length == 2);
                Atom atom0 = atoms[0]; string resi0 = atom0.ResidueId;
                Atom atom1 = atoms[1]; string resi1 = atom1.ResidueId;
                if(resi0 == resi1)
                    return false;
                return true;
            }
        }
        public class Bonds : List<Bond>
        {
            public double[] ListKb()
            {
                List<double> lstKb = new List<double>();
                foreach(Bond bond in this)
                    lstKb.Add(bond.Kb);
                return lstKb.ToArray();
            }
            //HashSet<Bond> _dbgbonds = (HDebug.IsDebuggerAttached ? new HashSet<Bond>() : null);
            public new void Add(Bond bond)
            {
                HDebug.Assert(bond.atoms.Length == 2);
                //HDebug.Assert(bond.atoms[0].ID < bond.atoms[1].ID);
                //HDebug.Assert(this.Contains(bond) == false);
                //if(HDebug.IsDebuggerAttached)
                //{
                //    HDebug.Assert(_dbgbonds.Contains(bond) == false);
                //    _dbgbonds.Add(bond);
                //}
                base.Add(bond);
            }
        }
	}
}
