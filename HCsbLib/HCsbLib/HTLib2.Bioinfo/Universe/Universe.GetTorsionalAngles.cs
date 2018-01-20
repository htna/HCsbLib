using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public class TorsionalAngle
        {
            public Atom[] atoms;
            public double angle;
        }
        public List<TorsionalAngle> GetGetTorsionalAngles()
        {
            List<TorsionalAngle> torangles = new List<TorsionalAngle>();
            foreach(Dihedral dihedral in dihedrals)
            {
                Atom[] toratoms = dihedral.atoms.HClone<Atom>();
                double torangle = Geometry.TorsionalAngle(atoms[0].Coord, atoms[1].Coord, atoms[2].Coord, atoms[3].Coord);
                torangles.Add(new TorsionalAngle { atoms=toratoms, angle=torangle });
            }
            return torangles;
        }
        public double GetGetTorsionalAngle(Atom atom1, Atom atom2)
        {
            List<double> torangle = new List<double>();
            foreach(Dihedral dihedral in dihedrals)
            {
                if((dihedral.atoms[1].ID == atom1.ID) && (dihedral.atoms[2].ID == atom2.ID)) torangle.Add(Geometry.TorsionalAngle(atoms[0].Coord, atoms[1].Coord, atoms[2].Coord, atoms[3].Coord));
                if((dihedral.atoms[2].ID == atom1.ID) && (dihedral.atoms[1].ID == atom2.ID)) torangle.Add(Geometry.TorsionalAngle(atoms[3].Coord, atoms[2].Coord, atoms[1].Coord, atoms[0].Coord));
            }
            HDebug.Assert(torangle.Count == 1);
            return torangle.Average();
        }
    }
}
