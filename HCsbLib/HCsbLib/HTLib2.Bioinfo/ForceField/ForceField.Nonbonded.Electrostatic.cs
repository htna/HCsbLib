using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class NonbondedElectrostatic : INonbonded, IHessBuilder4PwIntrAct
        {
            public virtual string[] FrcFldType { get { return new string[] { "Nonbonded", "Electrostatic" }; } }
            public virtual double? GetDefaultMinimizeStep() { return null; }
            double optHessianForceFactor = 1;
            public virtual void EnvClear() { optHessianForceFactor = 1; }
            public virtual bool EnvAdd(string key, object value)
            {
                if(key == "optHessianForceFactor")
                {
                    optHessianForceFactor = (double)value;
                    return true;
                }
                return false;
            }
            public virtual void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                Compute(atom1, atom2, coords, ref energy, ref forces, ref hessian, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Nonbonded nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                Compute(atom1, atom2, coords, ref energy, ref forces, ref hessian, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Atom atom1, Universe.Atom atom2, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                HDebug.Depreciated("check idx1 and idx2");
                int idx1 = 0; // nonbonded.atoms[0].ID;
                int idx2 = 1; // nonbonded.atoms[1].ID;

                Vector diff   = (coords[idx2] - coords[idx1]);
                double dx   = diff[0];
                double dy   = diff[1];
                double dz   = diff[2];
                double pchi = atom1.Charge;
                double pchj = atom2.Charge;
                double ee  = 80;

				///////////////////////////////////////////////////////////////////////////////
				// energy
                double lenergy = EnergyEs(dx, dy, dz, pchi, pchj, ee);
                energy += lenergy;
				///////////////////////////////////////////////////////////////////////////////
				// force
				if(forces != null)
                {
                    Vector f12 = ForceEs(dx, dy, dz, pchi, pchj, ee);
                    forces[idx1] += f12;
                    forces[idx2] += -f12;
                }
				///////////////////////////////////////////////////////////////////////////////
				// hessian
				if(hessian != null)
				{
                    //string option = "Spring+Force";
                    //Vector diff01 = (coords[1] - coords[0]);
                    //Vector diff10 = (coords[0] - coords[1]);
                    //hessian[0, 1] += SprngEs(diff01, pchi, pchj, ee, option);
                    //hessian[1, 0] += SprngEs(diff10, pchi, pchj, ee, option);
                    {
                        // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                        // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                        // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                        //
                        // V(rij) =           (332 * pchij / ee) * rij^-1
                        // F(rij) = (   -1) * (332 * pchij / ee) * rij^-2
                        // K(rij) = (-2*-1) * (332 * pchij / ee) * rij^-3
                        double pchij = pchi * pchj;
                        double rij2 = dx*dx + dy*dy + dz*dz;
                        double rij  = Math.Sqrt(rij2);
                        double rij3 = rij2*rij;
                        double fij = (   -1) * (332 * pchij / ee) / rij2;
                        double kij = (-2*-1) * (332 * pchij / ee) / rij3;
                        fij *= optHessianForceFactor;
                        //Matrix Hij = ForceField.GetHessianBlock(coords[0], coords[1], kij, fij);
                        hessian[0, 1] += ForceField.GetHessianBlock(coords[0], coords[1], kij, fij);
                        hessian[1, 0] += ForceField.GetHessianBlock(coords[1], coords[0], kij, fij);
                    }
                }
            }
            public void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out Pair<int, int>[] pwidxs, out PwIntrActInfo[] pwhessinfos)
            {
                Universe.Nonbonded nonbonded = (Universe.Nonbonded)info;

                HDebug.Depreciated("check idx1 and idx2");
                int idx1 = 0; // nonbonded.atoms[0].ID;
                int idx2 = 1; // nonbonded.atoms[1].ID;
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double pchij = atom1.Charge * atom2.Charge;
                if(double.IsNaN(pchij))
                {
                    HDebug.Assert(false);
                    pwidxs = null;
                    pwhessinfos = null;
                    return;
                }

                Vector diff   = (coords[idx2] - coords[idx1]);
                double dx   = diff[0];
                double dy   = diff[1];
                double dz   = diff[2];
                double pchi = atom1.Charge;
                double pchj = atom2.Charge;
                double ee  = 80;

                // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                //
                // V(rij) =           (332 * pchij / ee) * rij^-1
                // F(rij) = (   -1) * (332 * pchij / ee) * rij^-2
                // K(rij) = (-2*-1) * (332 * pchij / ee) * rij^-3
//                double pchij = pchi * pchj;
                double rij2 = dx*dx + dy*dy + dz*dz;
                double rij  = Math.Sqrt(rij2);
                double rij3 = rij2*rij;
                double fij = (   -1) * (332 * pchij / ee) / rij2;
                double kij = (-2*-1) * (332 * pchij / ee) / rij3;

                pwidxs = new Pair<int, int>[1];
                pwidxs[0] = new Pair<int, int>(0, 1);
                pwhessinfos = new PwIntrActInfo[1];
                pwhessinfos[0] = new PwIntrActInfo(kij, fij);
            }
            public static double EnergyEs(double dx, double dy, double dz, double pchi, double pchj, double ee)
            {
                double rij    = Math.Sqrt(dx*dx + dy*dy + dz*dz);
                double pchij  = pchi * pchj;
                double energy = 332 * pchij / (rij * ee);
                return energy;
            }
            public static Vector ForceEs(double dx, double dy, double dz, double pchi, double pchj, double ee)
            {
                double pchij = pchi * pchj;
                double dx2 = dx*dx;
                double dy2 = dy*dy;
                double dz2 = dz*dz;
                double dist2 = dx2+dy2+dz2;
                double dist  = Math.Sqrt(dist2);
                double dist3 = dist2*dist;

                // Fes x:  -((332 dx qi qj)/((dx^2+dy^2+dz^2)^(3/2) ee))
                // Fes x:  -((332 dy qi qj)/((dx^2+dy^2+dz^2)^(3/2) ee))
                // Fes x:  -((332 dz qi qj)/((dx^2+dy^2+dz^2)^(3/2) ee))

                double x =  -((332*dx*pchij)/(dist3*ee));
                double y =  -((332*dy*pchij)/(dist3*ee));
                double z =  -((332*dz*pchij)/(dist3*ee));

                Vector force = new double[] { x, y, z };
                return force;
            }
            public static MatrixByArr SprngEs(Vector dvij, double pchi, double pchj, double ee, string option)
            {
                return SprngEs(dvij[0], dvij[1], dvij[2], pchi, pchj, ee, option);
            }
            public static MatrixByArr SprngEs(double dx, double dy, double dz, double pchi, double pchj, double ee, string option)
            {
                double pchij = pchi * pchj;
                double dxdx = dx*dx; double dxdy = dx*dy;
                double dydy = dy*dy; double dxdz = dx*dz;
                double dzdz = dz*dz; double dydz = dy*dz;
                double dist2 = dxdx+dydy+dzdz;
                double dist  = Math.Sqrt(dist2);
                double dist3 = dist2*dist;
                double dist5 = dist2*dist2*dist;

                switch(option)
                {
                    case "Spring+Force":
                        {
                            // Ses xx:  -((332 qi qj)/((dist^2)^(3/2) ee))+(996 dx^2 qi qj)/((dist^2)^(5/2) ee)
                            // Ses xy:  (996 dx dy qi qj)/((dist^2)^(5/2) ee)
                            // Ses xz:  (996 dx dz qi qj)/((dist^2)^(5/2) ee)
                            // Ses yx:  (996 dx dy qi qj)/((dist^2)^(5/2) ee)
                            // Ses yy:  -((332 qi qj)/((dist^2)^(3/2) ee))+(996 dy^2 qi qj)/((dist^2)^(5/2) ee)
                            // Ses yz:  (996 dy dz qi qj)/((dist^2)^(5/2) ee)
                            // Ses zx:  (996 dx dz qi qj)/((dist^2)^(5/2) ee)
                            // Ses zy:  (996 dy dz qi qj)/((dist^2)^(5/2) ee)
                            // Ses zz:  -((332 qi qj)/((dist^2)^(3/2) ee))+(996 dz^2 qi qj)/((dist^2)^(5/2) ee)

                            double xx =  (996*dxdx*pchij)/(dist5*ee)    - ((332*pchij)/(dist3*ee));
                            double xy =  (996*dxdy*pchij)/(dist5*ee);
                            double xz =  (996*dxdz*pchij)/(dist5*ee);
                            double yx =  (996*dxdy*pchij)/(dist5*ee);
                            double yy =  (996*dydy*pchij)/(dist5*ee)    - ((332*pchij)/(dist3*ee));
                            double yz =  (996*dydz*pchij)/(dist5*ee);
                            double zx =  (996*dxdz*pchij)/(dist5*ee);
                            double zy =  (996*dydz*pchij)/(dist5*ee);
                            double zz =  (996*dzdz*pchij)/(dist5*ee)    - ((332*pchij)/(dist3*ee));
                            MatrixByArr sprng = new double[,] { { xx, xy, xz }, { yx, yy, yz }, { zx, zy, zz } };
                            return sprng;
                        }
                    case "Spring":
                        {
                            double xx =  (664*dxdx*pchij)/(dist5*ee);
                            double xy =  (664*dxdy*pchij)/(dist5*ee);
                            double xz =  (664*dxdz*pchij)/(dist5*ee);
                            double yx =  (664*dxdy*pchij)/(dist5*ee);
                            double yy =  (664*dydy*pchij)/(dist5*ee);
                            double yz =  (664*dydz*pchij)/(dist5*ee);
                            double zx =  (664*dxdz*pchij)/(dist5*ee);
                            double zy =  (664*dydz*pchij)/(dist5*ee);
                            double zz =  (664*dzdz*pchij)/(dist5*ee);
                            MatrixByArr sprng = new double[,] { { xx, xy, xz }, { yx, yy, yz }, { zx, zy, zz } };
                            return sprng;
                        }
                    case "Force":
                        {
                            double xx =  (332*dxdx*pchij)/(dist5*ee)    - ((332*pchij)/(dist3*ee));
                            double xy =  (332*dxdy*pchij)/(dist5*ee);
                            double xz =  (332*dxdz*pchij)/(dist5*ee);
                            double yx =  (332*dxdy*pchij)/(dist5*ee);
                            double yy =  (332*dydy*pchij)/(dist5*ee)    - ((332*pchij)/(dist3*ee));
                            double yz =  (332*dydz*pchij)/(dist5*ee);
                            double zx =  (332*dxdz*pchij)/(dist5*ee);
                            double zy =  (332*dydz*pchij)/(dist5*ee);
                            double zz =  (332*dzdz*pchij)/(dist5*ee)    - ((332*pchij)/(dist3*ee));
                            MatrixByArr sprng = new double[,] { { xx, xy, xz }, { yx, yy, yz }, { zx, zy, zz } };
                            return sprng;
                        }
                    default:
                        goto case "Spring+Force";
                }
            }
        }
    }
}
