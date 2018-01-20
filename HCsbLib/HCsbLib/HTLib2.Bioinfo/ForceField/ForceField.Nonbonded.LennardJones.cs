using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class NonbondedLennardJones : INonbonded, IHessBuilder4PwIntrAct
        {
            public virtual string[] FrcFldType { get { return new string[] { "Nonbonded", "LennardJones" }; } }
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
            // ! Wildcards used to minimize memory requirements
            // NONBONDED  NBXMOD 5  ATOM CDIEL FSHIFT VATOM VDISTANCE VFSWITCH -
            //      CUTNB 14.0  CTOFNB 12.0  CTONNB 10.0  EPS 1.0  E14FAC 1.0  WMIN 1.5
            // !
            // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
            // !
            // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
            // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
            // !
            // !atom  ignored    epsilon      Rmin/2   ignored   eps,1-4       Rmin/2,1-4
            // !
            // HT       0.0       -0.0460    0.2245 ! TIP3P
            // HN1      0.0       -0.0460    0.2245 
            // CN7      0.0       -0.02      2.275  0.0   -0.01 1.90 !equivalent to protein CT1
            // CN7B     0.0       -0.02      2.275  0.0   -0.01 1.90 !equivalent to protein CT1
            // ...
            /////////////////////////////////////////////////////////////
            public virtual void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double radi = atom1.Rmin2_14; radi = (double.IsNaN(radi)==false) ? radi : atom1.Rmin2;
                double radj = atom2.Rmin2_14; radj = (double.IsNaN(radj)==false) ? radj : atom2.Rmin2;
                double epsi = atom1.eps_14; epsi = (double.IsNaN(epsi)==false) ? epsi : atom1.epsilon;
                double epsj = atom2.eps_14; epsj = (double.IsNaN(epsj)==false) ? epsj : atom2.epsilon;
                Compute(nonbonded, coords, ref energy, ref forces, ref hessian, radi, radj, epsi, epsj, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Nonbonded nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double radi = atom1.Rmin2;
                double radj = atom2.Rmin2;
                double epsi = atom1.epsilon;
                double epsj = atom2.epsilon;
                Compute(nonbonded, coords, ref energy, ref forces, ref hessian, radi, radj, epsi, epsj, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.AtomPack nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian
                                       ,double radi ,double radj ,double epsi ,double epsj, double[,] pwfrc=null, double[,] pwspr=null)
            {
                HDebug.Depreciated("check idx1 and idx2");
                int idx1 = 0; // nonbonded.atoms[0].ID;
                int idx2 = 1; // nonbonded.atoms[1].ID;
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];

                Vector diff   = (coords[idx2] - coords[idx1]);
                double dx   = diff[0];
                double dy   = diff[1];
                double dz   = diff[2];
                //double radi = atom1.Rmin2;
                //double radj = atom2.Rmin2;
                //double epsi = atom1.epsilon;
                //double epsj = atom2.epsilon;

				///////////////////////////////////////////////////////////////////////////////
				// energy
                energy += EnergyVdw(dx, dy, dz, radi, radj, epsi, epsj);
				///////////////////////////////////////////////////////////////////////////////
				// force
				if(forces != null)
                {
                    Vector f12 = ForceVdw(dx, dy, dz, radi, radj, epsi, epsj);
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
                    //hessian[0, 1] += SprngVdw(diff01, radi, radj, epsi, epsj, option);
                    //hessian[1, 0] += SprngVdw(diff10, radi, radj, epsi, epsj, option);
                    {
                        // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                        // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                        // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                        //
                        // V(r) =           epsij * r0^12 * rij^-12  -        2 * epsij * r0^6 * rij^-6
                        // F(r) =     -12 * epsij * r0^12 * rij^-13  -     -6*2 * epsij * r0^6 * rij^-7
                        // K(r) = -13*-12 * epsij * r0^12 * rij^-14  -  -7*-6*2 * epsij * r0^6 * rij^-8
                        double r   = (radi + radj);
                        double r6  = Math.Pow(r, 6);
                        double r12 = Math.Pow(r, 12);
                        double rij2  = (dx*dx + dy*dy + dz*dz);
                        double rij   = Math.Sqrt(rij2);
                        double rij7  = Math.Pow(rij2, 3)*rij;
                        double rij8  = Math.Pow(rij2, 4);
                        double rij13 = Math.Pow(rij2, 6)*rij;
                        double rij14 = Math.Pow(rij2, 7);
                        double epsij = epsi*epsj;
                        double fij = (    -12) * epsij * r12 / rij13  -  (   -6*2) * epsij * r6 / rij7;
                        double kij = (-13*-12) * epsij * r12 / rij14  -  (-7*-6*2) * epsij * r6 / rij8;
                        fij *= optHessianForceFactor;
                        //Matrix Hij = ForceField.GetHessianBlock(coords[0], coords[1], kij, fij);
                        hessian[0, 1] += ForceField.GetHessianBlock(coords[0], coords[1], kij, fij);
                        hessian[1, 0] += ForceField.GetHessianBlock(coords[1], coords[0], kij, fij);
                    }
                }
            }
            public void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out Pair<int, int>[] pwidxs, out PwIntrActInfo[] pwhessinfos)
            {
                HDebug.Depreciated("check idx1 and idx2");
                int idx1 = 0; // nonbonded.atoms[0].ID;
                int idx2 = 1; // nonbonded.atoms[1].ID;
                Universe.Atom atom1 = info.atoms[0];
                Universe.Atom atom2 = info.atoms[1];

                Vector diff   = (coords[idx2] - coords[idx1]);
                double dx   = diff[0];
                double dy   = diff[1];
                double dz   = diff[2];
                double radi = double.NaN;
                double radj = double.NaN;
                double epsi = double.NaN;
                double epsj = double.NaN;

                if(typeof(Universe.Nonbonded14).IsInstanceOfType(info))
                {
                    radi = atom1.Rmin2_14; radi = (double.IsNaN(radi)==false) ? radi : atom1.Rmin2;
                    radj = atom2.Rmin2_14; radj = (double.IsNaN(radj)==false) ? radj : atom2.Rmin2;
                    epsi = atom1.eps_14; epsi = (double.IsNaN(epsi)==false) ? epsi : atom1.epsilon;
                    epsj = atom2.eps_14; epsj = (double.IsNaN(epsj)==false) ? epsj : atom2.epsilon;
                }
                if(typeof(Universe.Nonbonded).IsInstanceOfType(info))
                {
                    radi = atom1.Rmin2;
                    radj = atom2.Rmin2;
                    epsi = atom1.epsilon;
                    epsj = atom2.epsilon;
                }
                HDebug.Assert(double.IsNaN(radi) == false, double.IsNaN(radj) == false, double.IsNaN(epsi) == false, double.IsNaN(epsj) == false);

                // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                //
                // V(r) =           epsij * r0^12 * rij^-12  -        2 * epsij * r0^6 * rij^-6
                // F(r) =     -12 * epsij * r0^12 * rij^-13  -     -6*2 * epsij * r0^6 * rij^-7
                // K(r) = -13*-12 * epsij * r0^12 * rij^-14  -  -7*-6*2 * epsij * r0^6 * rij^-8
                double r   = (radi + radj);
                double r6  = Math.Pow(r, 6);
                double r12 = Math.Pow(r, 12);
                double rij2  = (dx*dx + dy*dy + dz*dz);
                double rij   = Math.Sqrt(rij2);
                double rij7  = Math.Pow(rij2, 3)*rij;
                double rij8  = Math.Pow(rij2, 4);
                double rij13 = Math.Pow(rij2, 6)*rij;
                double rij14 = Math.Pow(rij2, 7);
                double epsij = epsi*epsj;
                double fij = (    -12) * epsij * r12 / rij13  -  (   -6*2) * epsij * r6 / rij7;
                double kij = (-13*-12) * epsij * r12 / rij14  -  (-7*-6*2) * epsij * r6 / rij8;

                pwidxs = new Pair<int, int>[1];
                pwidxs[0] = new Pair<int, int>(0, 1);
                pwhessinfos = new PwIntrActInfo[1];
                pwhessinfos[0] = new PwIntrActInfo(kij, fij);
            }
            public static double EnergyVdw(double dx, double dy, double dz, double radi, double radj, double epsi, double epsj)
            {
                double rij    = Math.Sqrt(dx*dx + dy*dy + dz*dz);
                double rij2    = 1/(dx*dx + dy*dy + dz*dz);
                double Rminij = (radi + radj)/2;
                double Epsij  = Math.Sqrt(epsi * epsj);
                double vdwA = 4*Epsij*Math.Pow(Rminij, 12);
                double vdwB = 4*Epsij*Math.Pow(Rminij, 6);
                double rij6 = rij2 * rij2 * rij2;
                double energy = 4*Epsij*(Math.Pow(Rminij/rij, 12) - Math.Pow(Rminij/rij, 6));
                energy = (vdwA * rij6 - vdwB)*rij6;
                return energy;
            }
            public static Vector ForceVdw(double dx, double dy, double dz, double radi, double radj, double epsi, double epsj)
            {
                double radij   = (radi + radj);
                double radij6  = Math.Pow(radij, 6);
                double radij12 = Math.Pow(radij, 12);
                double dx2 = dx*dx;
                double dy2 = dy*dy;
                double dz2 = dz*dz;
                double dist2 = dx2+dy2+dz2;
                double dist = Math.Sqrt(dist2);
                double dist8  = Math.Pow(dist2, 4);
                double dist14 = Math.Pow(dist2, 7);
                double epsij = epsi*epsj;

                // Fvdw x:  epsi epsj ((12 dx (ri+rj)^6)/(dx^2+dy^2+dz^2)^4-(12 dx (ri+rj)^12)/(dx^2+dy^2+dz^2)^7)
                // Fvdw y:  epsi epsj ((12 dy (ri+rj)^6)/(dx^2+dy^2+dz^2)^4-(12 dy (ri+rj)^12)/(dx^2+dy^2+dz^2)^7)
                // Fvdw z:  epsi epsj ((12 dz (ri+rj)^6)/(dx^2+dy^2+dz^2)^4-(12 dz (ri+rj)^12)/(dx^2+dy^2+dz^2)^7)

                double x =  epsij*((12*dx*radij6)/dist8-(12*dx*radij12)/dist14);
                double y =  epsij*((12*dy*radij6)/dist8-(12*dy*radij12)/dist14);
                double z =  epsij*((12*dz*radij6)/dist8-(12*dz*radij12)/dist14);

                Vector force = new double[] { x, y, z };
                return force;
            }
            public static MatrixByArr SprngVdw(Vector dvij, double radi, double radj, double epsi, double epsj, string option)
            {
                return SprngVdw(dvij[0], dvij[1], dvij[2], radi, radj, epsi, epsj, option);
            }
            public static MatrixByArr SprngVdw(double dx, double dy, double dz, double radi, double radj, double epsi, double epsj, string option)
            {
                double radij6  = Math.Pow(radi+radj, 6);
                double radij12 = Math.Pow(radi+radj, 12);
                double dxdx = dx*dx; double dxdy = dx*dy;
                double dydy = dy*dy; double dxdz = dx*dz;
                double dzdz = dz*dz; double dydz = dy*dz;
                double dist2 = dxdx+dydy+dzdz;
                double dist8  = Math.Pow(dist2, 4);
                double dist10 = Math.Pow(dist2, 5);
                double dist14 = Math.Pow(dist2, 7);
                double dist16 = Math.Pow(dist2, 8);
                double epsij = epsi*epsj;

                switch(option)
                {
                    case "Spring+Force":
                        {
                            // Svdw xx:  epsi epsj ((12 (ri+rj)^6)/dist^8-(96 dx^2 (ri+rj)^6)/dist^10-(12 (ri+rj)^12)/dist^14+(168 dx^2 (ri+rj)^12)/dist^16)
                            // Svdw xy:  epsi epsj (-((96 dx dy (ri+rj)^6)/dist^10)+(168 dx dy (ri+rj)^12)/dist^16)
                            // Svdw xz:  epsi epsj (-((96 dx dz (ri+rj)^6)/dist^10)+(168 dx dz (ri+rj)^12)/dist^16)
                            // Svdw yx:  epsi epsj (-((96 dx dy (ri+rj)^6)/dist^10)+(168 dx dy (ri+rj)^12)/dist^16)
                            // Svdw yy:  epsi epsj ((12 (ri+rj)^6)/dist^8-(96 dy^2 (ri+rj)^6)/dist^10-(12 (ri+rj)^12)/dist^14+(168 dy^2 (ri+rj)^12)/dist^16)
                            // Svdw yz:  epsi epsj (-((96 dy dz (ri+rj)^6)/dist^10)+(168 dy dz (ri+rj)^12)/dist^16)
                            // Svdw zx:  epsi epsj (-((96 dx dz (ri+rj)^6)/dist^10)+(168 dx dz (ri+rj)^12)/dist^16)
                            // Svdw zy:  epsi epsj (-((96 dy dz (ri+rj)^6)/dist^10)+(168 dy dz (ri+rj)^12)/dist^16)
                            // Svdw zz:  epsi epsj ((12 (ri+rj)^6)/dist^8-(96 dz^2 (ri+rj)^6)/dist^10-(12 (ri+rj)^12)/dist^14+(168 dz^2 (ri+rj)^12)/dist^16)

                            double xx =  epsij*(-((96*dxdx*radij6)/dist10)    + (168*dxdx*radij12)/dist16    + (12*radij6)/dist8     - (12*radij12)/dist14);
                            double xy =  epsij*(-((96*dxdy*radij6)/dist10)    + (168*dxdy*radij12)/dist16                                                 );
                            double xz =  epsij*(-((96*dxdz*radij6)/dist10)    + (168*dxdz*radij12)/dist16                                                 );
                            double yx =  epsij*(-((96*dxdy*radij6)/dist10)    + (168*dxdy*radij12)/dist16                                                 );
                            double yy =  epsij*(-((96*dydy*radij6)/dist10)    + (168*dydy*radij12)/dist16    + (12*radij6)/dist8     - (12*radij12)/dist14);
                            double yz =  epsij*(-((96*dydz*radij6)/dist10)    + (168*dydz*radij12)/dist16                                                 );
                            double zx =  epsij*(-((96*dxdz*radij6)/dist10)    + (168*dxdz*radij12)/dist16                                                 );
                            double zy =  epsij*(-((96*dydz*radij6)/dist10)    + (168*dydz*radij12)/dist16                                                 );
                            double zz =  epsij*(-((96*dzdz*radij6)/dist10)    + (168*dzdz*radij12)/dist16    + (12*radij6)/dist8     - (12*radij12)/dist14);
                            MatrixByArr sprng = new double[,] { { xx, xy, xz }, { yx, yy, yz }, { zx, zy, zz } };
                            return sprng;
                        }
                    case "Spring":
                        {
                            double xx =  epsij*(-((84*dxdx*radij6)/dist10)    + (156*dxdx*radij12)/dist16);
                            double xy =  epsij*(-((84*dxdy*radij6)/dist10)    + (156*dxdy*radij12)/dist16);
                            double xz =  epsij*(-((84*dxdz*radij6)/dist10)    + (156*dxdz*radij12)/dist16);
                            double yx =  epsij*(-((84*dxdy*radij6)/dist10)    + (156*dxdy*radij12)/dist16);
                            double yy =  epsij*(-((84*dydy*radij6)/dist10)    + (156*dydy*radij12)/dist16);
                            double yz =  epsij*(-((84*dydz*radij6)/dist10)    + (156*dydz*radij12)/dist16);
                            double zx =  epsij*(-((84*dxdz*radij6)/dist10)    + (156*dxdz*radij12)/dist16);
                            double zy =  epsij*(-((84*dydz*radij6)/dist10)    + (156*dydz*radij12)/dist16);
                            double zz =  epsij*(-((84*dzdz*radij6)/dist10)    + (156*dzdz*radij12)/dist16);
                            MatrixByArr sprng = new double[,] { { xx, xy, xz }, { yx, yy, yz }, { zx, zy, zz } };
                            return sprng;
                        }
                    case "Force":
                        {
                            double xx =  epsij*(-((12*dxdx*radij6)/dist10)    + (12*dxdx*radij12)/dist16    + (12*radij6)/dist8     - (12*radij12)/dist14);
                            double xy =  epsij*(-((12*dxdy*radij6)/dist10)    + (12*dxdy*radij12)/dist16                                                 );
                            double xz =  epsij*(-((12*dxdz*radij6)/dist10)    + (12*dxdz*radij12)/dist16                                                 );
                            double yx =  epsij*(-((12*dxdy*radij6)/dist10)    + (12*dxdy*radij12)/dist16                                                 );
                            double yy =  epsij*(-((12*dydy*radij6)/dist10)    + (12*dydy*radij12)/dist16    + (12*radij6)/dist8     - (12*radij12)/dist14);
                            double yz =  epsij*(-((12*dydz*radij6)/dist10)    + (12*dydz*radij12)/dist16                                                 );
                            double zx =  epsij*(-((12*dxdz*radij6)/dist10)    + (12*dxdz*radij12)/dist16                                                 );
                            double zy =  epsij*(-((12*dydz*radij6)/dist10)    + (12*dydz*radij12)/dist16                                                 );
                            double zz =  epsij*(-((12*dzdz*radij6)/dist10)    + (12*dzdz*radij12)/dist16    + (12*radij6)/dist8     - (12*radij12)/dist14);
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
