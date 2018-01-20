using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class Rigidbody : IBond, IAngle, IDihedral, IImproper//, INonbonded
        {
            //MindyNonbondedElectrostatic nbES = new MindyNonbondedElectrostatic();
            //MindyNonbondedLennardJones  nbLJ = new MindyNonbondedLennardJones(false);
            double sprcstRgdbdy = 100000;
            double scaleSpring = 10;
            public Rigidbody(double scaleSpring = 10)
            {
                this.scaleSpring = scaleSpring;
            }

            public string[] FrcFldType { get {return null;} }
            public virtual double? GetDefaultMinimizeStep() { return 0.0000001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            public void Compute(Universe.Bond bond, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                double Kb = bond.Kb;
                       Kb = sprcstRgdbdy;
                       Kb = bond.Kb * scaleSpring;
                double b0 = bond.b0;
                MindyBond.Compute(coords, ref energy, ref forces, ref hessian, Kb, b0, pwfrc, pwspr);
            }
            public void Compute(Universe.Angle angle, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                double Ktheta = angle.Ktheta;
                       Ktheta = sprcstRgdbdy;
                       Ktheta = angle.Ktheta * scaleSpring;
                double Theta0 = angle.Theta0;
                double Kub    = angle.Kub;
                double S0     = angle.S0;
                MindyAngle.Compute(coords, ref energy, ref forces, ref hessian, Ktheta, Theta0, Kub, S0, pwfrc, pwspr);
            }
            public void Compute(Universe.Dihedral dihedral, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
			{
                //double Kchi  = dihedral.param.Kchi ;
                //double n     = dihedral.param.n    ;
                //double delta = dihedral.param.delta;
                //MindyDihedral.Compute(coords, ref energy, ref forces, ref hessian, Kchi, n, delta);
            }
            public void Compute(Universe.Improper improper, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                double Kchi  = improper.Kpsi;
                       Kchi  = sprcstRgdbdy;
                       Kchi  = improper.Kpsi * scaleSpring;
                int    n     = improper.n;
                double delta = improper.psi0;
                MindyImproper.Compute(coords, ref energy, ref forces, ref hessian, Kchi, n, delta, pwfrc, pwspr);
            }
            //public virtual void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref Matrix[,] hessian)
            //{
            //    Debug.Assert(hessian == null);
            //
            //    double engES = 0; Vector[] frcES = Vectors.Clone(forces); nbES.Compute(nonbonded, coords, ref engES, ref frcES, ref hessian);
            //    double engLJ = 0; Vector[] frcLJ = Vectors.Clone(forces); nbES.Compute(nonbonded, coords, ref engLJ, ref frcLJ, ref hessian);
            //    forces[0] += frcES[0]; forces[1] += frcES[1];
            //    forces[0] += frcLJ[0]; forces[1] += frcLJ[1];
            //    energy = engES + engLJ;
            //}
            //public virtual void Compute(Universe.Nonbonded nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref Matrix[,] hessian)
            //{
            //    Debug.Assert(hessian == null);
            //
            //    double engES = 0; Vector[] frcES = Vectors.Clone(forces); nbES.Compute(nonbonded, coords, ref engES, ref frcES, ref hessian);
            //    double engLJ = 0; Vector[] frcLJ = Vectors.Clone(forces); nbES.Compute(nonbonded, coords, ref engLJ, ref frcLJ, ref hessian);
            //    forces[0] += frcES[0]; forces[1] += frcES[1];
            //    forces[0] += frcLJ[0]; forces[1] += frcLJ[1]; 
            //    energy = engES + engLJ;
            //}
        }
    }
}
