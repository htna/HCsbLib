using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class MindyNonbondedElectrostatic : INonbonded
        {
            static readonly double SWITCHDIST   = 8.5;
            static readonly double CUTOFF       = 10;
            static readonly double PAIRLISTDIST = 11.5;
            static readonly double pair2   = PAIRLISTDIST * PAIRLISTDIST;
            static readonly double cut2    = CUTOFF * CUTOFF;
            static readonly double switch2 = SWITCHDIST * SWITCHDIST;
            static readonly double c1 = Math.Pow(1.0/(cut2-switch2), 3);
            static readonly double c3 = 4*c1;
            static readonly double COULOMB = 332.0636;

            public virtual string[] FrcFldType { get { return new string[] { "Nonbonded", "Mindy", "Electrostatic" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.0001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            public virtual void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double pchij = atom1.Charge * atom2.Charge;
                if(double.IsNaN(pchij))
                {
                    HDebug.Assert(false);
                    return;
                }
                Compute(coords, ref energy, ref forces, ref hessian, pchij, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Nonbonded nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double pchij = atom1.Charge * atom2.Charge;
                Compute(coords, ref energy, ref forces, ref hessian, pchij, pwfrc, pwspr);
            }
            public static void Compute(Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double pchij, double[,] pwfrc=null, double[,] pwspr=null)
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

                //Debug.BreakAnd((Math.Min(idx1, idx2) == 1), (Math.Max(idx1, idx2) == 7));

                Vector diff   = (coords[idx2] - coords[idx1]);
                double dx   = diff[0];
                double dy   = diff[1];
                double dz   = diff[2];

                double Eelec = 0;
                double force_r = 0;

                double dist = diff.Dist2;

                if(dist > cut2) return;                                            // if(dist > cut2) continue;
                //                                                                 // 
                double r = Math.Sqrt(dist);                                        // double r = sqrt(dist);
                double r_1 = 1.0/r;                                                // double r_1 = 1.0/r; 
                // double r_2 = r_1*r_1;                                           // double r_2 = r_1*r_1;
                // double r_6 = r_2*r_2*r_2;                                       // double r_6 = r_2*r_2*r_2;
                // double r_12 = r_6*r_6;                                          // double r_12 = r_6*r_6;
                // double switchVal = 1, dSwitchVal = 0;                           // double switchVal = 1, dSwitchVal = 0;
                // if (dist > switch2) {                                           // if (dist > switch2) {
                //     double c2 = cut2 - dist;                                    //     double c2 = cut2 - dist;
                //     double c4 = c2*(cut2 + 2*dist - 3.0*switch2);               //     double c4 = c2*(cut2 + 2*dist - 3.0*switch2);
                //     switchVal = c2*c4*c1;                                       //     switchVal = c2*c4*c1;
                //     dSwitchVal = c3*r*(c2*c2-c4);                               //     dSwitchVal = c3*r*(c2*c2-c4);
                // }                                                               // }
                //                                                                 // 
                // // get VDW constants                                            // // get VDW constants
                // Index vdwtype2 = mol->atomvdwtype(ind2);                        // Index vdwtype2 = mol->atomvdwtype(ind2);
                // const LJTableEntry *entry;                                      // const LJTableEntry *entry;
                //                                                                 // 
                // if (mol->check14excl(ind1,ind2))                                // if (mol->check14excl(ind1,ind2))
                //     entry = ljTable->table_val_scaled14(vdwtype1, vdwtype2);    //     entry = ljTable->table_val_scaled14(vdwtype1, vdwtype2);
                // else                                                            // else
                //     entry = ljTable->table_val(vdwtype1, vdwtype2);             //     entry = ljTable->table_val(vdwtype1, vdwtype2);
                // double vdwA = entry->A;                                         // double vdwA = entry->A;
                // double vdwB = entry->B;                                         // double vdwB = entry->B;
                // double AmBterm = (vdwA * r_6 - vdwB)*r_6;                       // double AmBterm = (vdwA * r_6 - vdwB)*r_6;
                // Evdw += switchVal*AmBterm;                                      // Evdw += switchVal*AmBterm;
                // double force_r = ( switchVal * 6.0 * (vdwA*r_12 + AmBterm) *    // double force_r = ( switchVal * 6.0 * (vdwA*r_12 + AmBterm) *
                //                     r_1 - AmBterm*dSwitchVal )*r_1;             //                     r_1 - AmBterm*dSwitchVal )*r_1;
                                                                                   // 
                // Electrostatics                                                  // // Electrostatics
                double kqq = COULOMB * pchij;                                      // double kqq = kq * mol->atomcharge(ind2);
                double efac = 1.0-dist/cut2;                                       // double efac = 1.0-dist/cut2;
                double prefac = kqq * r_1 * efac;                                  // double prefac = kqq * r_1 * efac;
                Eelec += prefac * efac;                                            // Eelec += prefac * efac;
                force_r += prefac * r_1 * (r_1 + 3.0*r/cut2);                      // force_r += prefac * r_1 * (r_1 + 3.0*r/cut2);

                HDebug.Assert(double.IsNaN(Eelec) == false, double.IsInfinity(Eelec) == false);
                energy += Eelec;
//System.Console.WriteLine("elec(" + Eelec + ")");
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
                    NumericSolver.Derivative2(ComputeFunc, coords, dcoord, ref hessian, pchij);
                }
            }
            public static double ComputeFunc(Vector[] coords, double[] info)
            {
                HDebug.Assert(info.Length == 1);
                //double pchij = info[0];
                double energy = 0;
                Vector[] forces = null;
                MatrixByArr[,] hessian = null;
                Compute(coords, ref energy, ref forces, ref hessian, info[0]);
                return energy;
            }
        }
    }
}
