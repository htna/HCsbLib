using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class MindyNonbondedLennardJones : INonbonded
        {
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
            // http://localscf.com/LJPotential.aspx
            //   * Switching off the LJ potential
            //   * (0.4 - 1.0) weight for 1-4 interaction
            // http://www.mpip-mainz.mpg.de/~andrienk/journal_club/opls.pdf
            //   * 0.5 weight for 1-4 interaction
            // http://lammps.sandia.gov/doc/special_bonds.html
            // http://www.wag.caltech.edu/home-pages/mario/lammps2001/force_fields.html
            //   * LAMMPS Force Fields
            // e14fac
            //   * http://www.mdy.univie.ac.at/lehre/charmm/course1/course1-4.html
            //   * http://www.charmm.org/documentation/c35b1/facts.html
            /////////////////////////////////////////////////////////////
            static readonly double SWITCHDIST   = 8.5;
            static readonly double CUTOFF       = 10;
            static readonly double PAIRLISTDIST = 11.5;
            static readonly double pair2   = PAIRLISTDIST * PAIRLISTDIST;
            static readonly double cut2    = CUTOFF * CUTOFF;
            static readonly double switch2 = SWITCHDIST * SWITCHDIST;
            static readonly double c1 = Math.Pow(1.0/(cut2-switch2), 3);
            static readonly double c3 = 4*c1;
            bool divideRadijByTwo = true;

            public MindyNonbondedLennardJones()                      { this.divideRadijByTwo = true; }
            public MindyNonbondedLennardJones(bool divideRadijByTwo) { HDebug.Assert(divideRadijByTwo==false);  this.divideRadijByTwo = divideRadijByTwo; }
            public virtual string[] FrcFldType { get { return new string[] { "Nonbonded", "Mindy", "LennardJones" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.0001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            public virtual void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double radi = atom1.Rmin2_14; radi = (double.IsNaN(radi)==false) ? radi : atom1.Rmin2;
                double radj = atom2.Rmin2_14; radj = (double.IsNaN(radj)==false) ? radj : atom2.Rmin2;
                double epsi = atom1.eps_14;   epsi = (double.IsNaN(epsi)==false) ? epsi : atom1.epsilon;
                double epsj = atom2.eps_14;   epsj = (double.IsNaN(epsj)==false) ? epsj : atom2.epsilon;
                double radij =          (radi + radj);
                if(divideRadijByTwo) radij = radij / 2;
                double epsij = Math.Sqrt(epsi * epsj);
                if(double.IsNaN(radij) || double.IsNaN(epsij))
                {
                    HDebug.Assert(false);
                    return;
                }
                Compute(coords, ref energy, ref forces, ref hessian, radij, epsij, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Nonbonded nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double radij =          (atom1.Rmin2   + atom2.Rmin2);
                if(divideRadijByTwo) radij = radij / 2;
                double epsij = Math.Sqrt(atom1.epsilon * atom2.epsilon);
                Compute(coords, ref energy, ref forces, ref hessian, radij, epsij, pwfrc, pwspr);
            }
            void Compute(Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double radij, double epsij, double[,] pwfrc=null, double[,] pwspr=null)
            {
                #region original source in mindy
                // if(dist > cut2) continue;
                //
                // double r = sqrt(dist);
                // double r_1 = 1.0/r; 
                // double r_2 = r_1*r_1;
                // double r_6 = r_2*r_2*r_2;
                // double r_12 = r_6*r_6;
                // double switchVal = 1, dSwitchVal = 0;
                // if (dist > switch2) {
                //     double c2 = cut2 - dist;
                //     double c4 = c2*(cut2 + 2*dist - 3.0*switch2);
                //     switchVal = c2*c4*c1;
                //     dSwitchVal = c3*r*(c2*c2-c4);
                // }
                // 
                // // get VDW constants
                // Index vdwtype2 = mol->atomvdwtype(ind2);
                // const LJTableEntry *entry;
                // 
                // if (mol->check14excl(ind1,ind2))
                //     entry = ljTable->table_val_scaled14(vdwtype1, vdwtype2);
                // else
                //     entry = ljTable->table_val(vdwtype1, vdwtype2);
                // double vdwA = entry->A;
                // double vdwB = entry->B;
                // double AmBterm = (vdwA * r_6 - vdwB)*r_6;
                // Evdw += switchVal*AmBterm;
                // double force_r = ( switchVal * 6.0 * (vdwA*r_12 + AmBterm) *
                //                     r_1 - AmBterm*dSwitchVal )*r_1;
                // 
                // // Electrostatics
                // double kqq = kq * mol->atomcharge(ind2);
                // double efac = 1.0-dist/cut2;
                // double prefac = kqq * r_1 * efac;
                // Eelec += prefac * efac;
                // force_r += prefac * r_1 * (r_1 + 3.0*r/cut2);
                // 
                // tmpf -= force_r * dr; 
                // nbrbox[j].force += force_r * dr;
                #endregion
                int idx1 = 0;
                int idx2 = 1;

                Vector diff  = (coords[idx2] - coords[idx1]);

                double Evdw = 0;
                double force_r = 0;

                double dist = diff.Dist2;

                if(dist > cut2) return;                                            // if(dist > cut2) continue;
                //                                                                 // 
                double r = Math.Sqrt(dist);                                        // double r = sqrt(dist);
                double r_1 = 1.0/r;                                                // double r_1 = 1.0/r; 
                double r_2 = r_1*r_1;                                              // double r_2 = r_1*r_1;
                double r_6 = r_2*r_2*r_2;                                          // double r_6 = r_2*r_2*r_2;
                double r_12 = r_6*r_6;                                             // double r_12 = r_6*r_6;
                double switchVal = 1, dSwitchVal = 0;                              // double switchVal = 1, dSwitchVal = 0;
                if (dist > switch2) {                                              // if (dist > switch2) {
                     double c2 = cut2 - dist;                                      //     double c2 = cut2 - dist;
                     double c4 = c2*(cut2 + 2*dist - 3.0*switch2);                 //     double c4 = c2*(cut2 + 2*dist - 3.0*switch2);
                     switchVal = c2*c4*c1;                                         //     switchVal = c2*c4*c1;
                     dSwitchVal = c3*r*(c2*c2-c4);                                 //     dSwitchVal = c3*r*(c2*c2-c4);
                }                                                                  // }
                //                                                                 //
                // get VDW constants                                               // // get VDW constants
                // Index vdwtype2 = mol->atomvdwtype(ind2);                        // Index vdwtype2 = mol->atomvdwtype(ind2);
                // const LJTableEntry *entry;                                      // const LJTableEntry *entry;
                //                                                                 // 
                // if (mol->check14excl(ind1,ind2))                                // if (mol->check14excl(ind1,ind2))
                //     entry = ljTable->table_val_scaled14(vdwtype1, vdwtype2);    //     entry = ljTable->table_val_scaled14(vdwtype1, vdwtype2);
                // else                                                            // else
                //     entry = ljTable->table_val(vdwtype1, vdwtype2);             //     entry = ljTable->table_val(vdwtype1, vdwtype2);
                double vdwA = 4*epsij*Math.Pow(radij, 12);                         // double vdwA = entry->A;
                double vdwB = 4*epsij*Math.Pow(radij, 6);                          // double vdwB = entry->B;
                double AmBterm = (vdwA * r_6 - vdwB)*r_6;                          // double AmBterm = (vdwA * r_6 - vdwB)*r_6;
                Evdw += switchVal*AmBterm;                                         // Evdw += switchVal*AmBterm;
                force_r += ( switchVal * 6.0 * (vdwA*r_12 + AmBterm) *             // double force_r = ( switchVal * 6.0 * (vdwA*r_12 + AmBterm) *
                                    r_1 - AmBterm*dSwitchVal )*r_1;                //                     r_1 - AmBterm*dSwitchVal )*r_1;
                                                                                   // 
                // // Electrostatics                                               // // Electrostatics
                // double kqq = kq * mol->atomcharge(ind2);                        // double kqq = kq * mol->atomcharge(ind2);
                // double efac = 1.0-dist/cut2;                                    // double efac = 1.0-dist/cut2;
                // double prefac = kqq * r_1 * efac;                               // double prefac = kqq * r_1 * efac;
                // Eelec += prefac * efac;                                         // Eelec += prefac * efac;
                // force_r += prefac * r_1 * (r_1 + 3.0*r/cut2);                   // force_r += prefac * r_1 * (r_1 + 3.0*r/cut2);

                HDebug.Assert(double.IsNaN(Evdw) == false, double.IsInfinity(Evdw) == false);
                energy += Evdw;
//System.Console.Write("" + Math.Min(idx1,idx2) + ", " + Math.Max(idx1,idx2) + " - vdw(" + Evdw + "), ");
                ///////////////////////////////////////////////////////////////////////////////
				// force
				if(forces != null)
                {
                    Vector force = diff * force_r;                                      // Vector tmppos = tmpbox[i].pos;
                                                                                        // Vector dr = nbrbox[j].pos - tmppos;
                    forces[idx1] -= force;                                              // tmpf -= force_r * dr; 
                    forces[idx2] += force;                                              // nbrbox[j].force += force_r * dr;
                }
				///////////////////////////////////////////////////////////////////////////////
				// hessian
				if(hessian != null)
				{
					//Debug.Assert(false);
                    double dcoord = 0.0001;
                    HDebug.Assert(hessian.GetLength(0) == 2, hessian.GetLength(1) == 2);
                    NumericSolver.Derivative2(ComputeFunc, coords, dcoord, ref hessian, radij, epsij);
                }
            }
            public double ComputeFunc(Vector[] coords, double[] info)
            {
                HDebug.Assert(info.Length == 2);
                //Universe.Atom atom1 = (Universe.Atom)info[0];
                //Universe.Atom atom2 = (Universe.Atom)info[1];
                //double        radij = (double       )info[2];
                //double        epsij = (double       )info[3];
                double energy = 0;
                Vector[] forces = null;
                MatrixByArr[,] hessian = null;
                Compute(coords, ref energy, ref forces, ref hessian, info[0], info[1]);
                return energy;
            }
        }
    }
}
