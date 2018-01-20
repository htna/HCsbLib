using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public static partial class Charmm22
        {
            public class PPotential
            {
                public double    energy = double.NaN;
                public Vector[]  force  = null;
                public Matrix[,] hess   = null;
                public double[,] pwfrc  = null;
                public double[,] pwspr  = null;
            }
            // Lennard-Jones
            public static PPotential LJ(Universe.Nonbonded nonbonded, Vector[] coords)
            {
                Universe.Atom atom1 = nonbonded.atoms[0]; Vector coord1 = coords[atom1.ID];
                Universe.Atom atom2 = nonbonded.atoms[1]; Vector coord2 = coords[atom2.ID];

                double rad1 = atom1.Rmin2;
                double rad2 = atom2.Rmin2;
                double eps1 = atom1.epsilon;
                double eps2 = atom2.epsilon;

                return LJ(coord1, coord2, rad1, rad2, eps1, eps2);
            }
            public static PPotential LJ(Universe.Nonbonded14 nonbonded, Vector[] coords)
            {
                Universe.Atom atom1 = nonbonded.atoms[0]; Vector coord1 = coords[atom1.ID];
                Universe.Atom atom2 = nonbonded.atoms[1]; Vector coord2 = coords[atom2.ID];

                double rad1 = atom1.Rmin2_14; rad1 = (double.IsNaN(rad1)==false) ? rad1 : atom1.Rmin2;
                double rad2 = atom2.Rmin2_14; rad2 = (double.IsNaN(rad2)==false) ? rad2 : atom2.Rmin2;
                double eps1 = atom1.eps_14; eps1 = (double.IsNaN(eps1)==false) ? eps1 : atom1.epsilon;
                double eps2 = atom2.eps_14; eps2 = (double.IsNaN(eps2)==false) ? eps2 : atom2.epsilon;

                return LJ(coord1, coord2, rad1, rad2, eps1, eps2);
            }
            public static PPotential LJ(Vector coordi, Vector coordj, double radi, double radj, double epsi, double epsj)
            {
                double rij    = (coordi - coordj).Dist;
                double Rminij = (radi + radj);
                double Epsij  = Math.Sqrt(epsi * epsj);

                double energy = Epsij * (Math.Pow(Rminij/rij, 12) - 2*Math.Pow(Rminij/rij, 6));

                return new PPotential
                {
                    energy = energy,
                };
            }
            // electrostatic
            public static PPotential Elec(Universe.Nonbonded nonbonded, Vector[] coords, double ee)
            {
                Universe.Atom atom1 = nonbonded.atoms[0]; Vector coord1 = coords[atom1.ID];
                Universe.Atom atom2 = nonbonded.atoms[1]; Vector coord2 = coords[atom2.ID];

                //double ee = 1; // 80

                double pch1 = atom1.Charge;
                double pch2 = atom2.Charge;

                return Elec(coord1, coord2, pch1, pch2, ee);
            }
            public static PPotential Elec(Universe.Nonbonded14 nonbonded, Vector[] coords, double ee)
            {
                Universe.Atom atom1 = nonbonded.atoms[0]; Vector coord1 = coords[atom1.ID];
                Universe.Atom atom2 = nonbonded.atoms[1]; Vector coord2 = coords[atom2.ID];

                //double ee = 1; // 80

                double pch1 = atom1.Charge;
                double pch2 = atom2.Charge;

                return Elec(coord1, coord2, pch1, pch2, ee);
            }
            public static PPotential Elec(Vector coordi, Vector coordj, double pchi, double pchj, double ee)
            {
                double rij    = (coordi - coordj).Dist;
                double pchij  = pchi * pchj;
                double energy = (332.0 * pchij) / (rij * ee);

                return new PPotential
                {
                    energy = energy,
                };
            }
        }
    }
}
