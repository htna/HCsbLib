﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class MindyDihedral : IDihedral
		{
			// DIHEDRALS
			// !
			// !V(dihedral) = Kchi(1 + cos(n(chi) - delta))
			// !
			// !Kchi: kcal/mole
			// !n: multiplicity
			// !delta: degrees
			// !
			// !atom types             Kchi    n   delta
			// !
			// CN7  ON6  CN8B HN8      0.195   1     0.0
			// ON6  CN8B CN8  HN8      0.195   1     0.0
			// ...
			/////////////////////////////////////////////////////////////
            public virtual string[] FrcFldType { get { return new string[] { "Dihedral", "Mindy" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.0001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            public virtual void Compute(Universe.Dihedral dihedral, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
			{
			    double Kchi  = dihedral.Kchi ;
			    double n     = dihedral.n    ;
			    double delta = dihedral.delta;
                Compute(coords, ref energy, ref forces, ref hessian, Kchi, n, delta, pwfrc, pwspr);
            }
            public static void Compute(Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                       double Kchi, double n, double delta, double[,] pwfrc=null, double[,] pwspr=null)
			{
				#region original source in mindy
				// double ComputeBonded::compute_dihedrals(const Vector *coords, Vector *f) const {
				//   double energy = 0.0;
				//   DihedralElem *dihedral = dihedrals;
				//   for (int i=0; i<ndihedrals; i++) {
				//     const Vector *pos0 = coords + dihedral->atom1;
				//     const Vector *pos1 = coords + dihedral->atom2;
				//     const Vector *pos2 = coords + dihedral->atom3;
				//     const Vector *pos3 = coords + dihedral->atom4;
				//     const Vector r12 = *pos0 - *pos1;
				//     const Vector r23 = *pos1 - *pos2;
				//     const Vector r34 = *pos2 - *pos3;
				// 
				//     Vector dcosdA;
				//     Vector dcosdB;
				//     Vector dsindC;
				//     Vector dsindB;
				//     Vector f1, f2, f3;
				// 
				//     Vector A, B, C;
				//     A.cross(r12, r23);
				//     B.cross(r23, r34);
				//     C.cross(r23, A);
				// 
				//     double rA = A.length();
				//     double rB = B.length(); 
				//     double rC = C.length();
				// 
				//     double cos_phi = (A*B)/(rA*rB);
				//     double sin_phi = (C*B)/(rC*rB);
				// 
				//     // Normalize B
				//     rB = 1.0/rB;
				//     B *= rB;
				// 
				//     double phi = -atan2(sin_phi, cos_phi);
				// 
				//     if (fabs(sin_phi) > 0.1) {
				//       // Normalize A
				//       rA = 1.0/rA;
				//       A *= rA;
				//       dcosdA = rA*(cos_phi*A-B);
				//       dcosdB = rB*(cos_phi*B-A);
				//     }
				//     else {
				//       // Normalize C
				//       rC = 1.0/rC;
				//       C *= rC;
				//       dsindC = rC*(sin_phi*C-B);
				//       dsindB = rB*(sin_phi*B-C);
				//     }
				// 
				//     int mult = dihedral->multiplicity; 
				//     for (int j=0; j<mult; j++) {
				//       double k = dihedral->k[j];
				//       double n = dihedral->n[j];
				//       double delta = dihedral->delta[j];
				//       double K, K1;
				//       if (n) {
				//         K = k * (1.0+cos(n*phi + delta)); 
				//         K1 = -n*k*sin(n*phi + delta);
				//       }
				//       else {
				//         double diff = phi-delta;
				//         if (diff < -M_PI) diff += 2.0*M_PI;
				//         else if (diff > M_PI) diff -= 2.0*M_PI;
				//         K = k*diff*diff;
				//         K1 = 2.0*k*diff;
				//       }
				//       energy += K;
				// 
				//       // forces
				//       if (fabs(sin_phi) > 0.1) {
				//         K1 = K1/sin_phi;
				//         f1.x += K1*(r23.y*dcosdA.z - r23.z*dcosdA.y);
				//         f1.y += K1*(r23.z*dcosdA.x - r23.x*dcosdA.z);
				//         f1.z += K1*(r23.x*dcosdA.y - r23.y*dcosdA.x);
				// 
				//         f3.x += K1*(r23.z*dcosdB.y - r23.y*dcosdB.z);
				//         f3.y += K1*(r23.x*dcosdB.z - r23.z*dcosdB.x);
				//         f3.z += K1*(r23.y*dcosdB.x - r23.x*dcosdB.y);
				// 
				//         f2.x += K1*(r12.z*dcosdA.y - r12.y*dcosdA.z
				//                  + r34.y*dcosdB.z - r34.z*dcosdB.y);
				//         f2.y += K1*(r12.x*dcosdA.z - r12.z*dcosdA.x
				//                  + r34.z*dcosdB.x - r34.x*dcosdB.z);
				//         f2.z += K1*(r12.y*dcosdA.x - r12.x*dcosdA.y
				//                  + r34.x*dcosdB.y - r34.y*dcosdB.x);
				//       }
				//       else {
				//         //  This angle is closer to 0 or 180 than it is to
				//         //  90, so use the cos version to avoid 1/sin terms
				//         K1 = -K1/cos_phi;
				// 
				//         f1.x += K1*((r23.y*r23.y + r23.z*r23.z)*dsindC.x
				//                 - r23.x*r23.y*dsindC.y
				//                 - r23.x*r23.z*dsindC.z);
				//         f1.y += K1*((r23.z*r23.z + r23.x*r23.x)*dsindC.y
				//                 - r23.y*r23.z*dsindC.z
				//                 - r23.y*r23.x*dsindC.x);
				//         f1.z += K1*((r23.x*r23.x + r23.y*r23.y)*dsindC.z
				//                 - r23.z*r23.x*dsindC.x
				//                 - r23.z*r23.y*dsindC.y);
				// 
				//         f3 += cross(K1,dsindB,r23);
				// 
				//         f2.x += K1*(-(r23.y*r12.y + r23.z*r12.z)*dsindC.x
				//                +(2.0*r23.x*r12.y - r12.x*r23.y)*dsindC.y
				//                +(2.0*r23.x*r12.z - r12.x*r23.z)*dsindC.z
				//                +dsindB.z*r34.y - dsindB.y*r34.z);
				//         f2.y += K1*(-(r23.z*r12.z + r23.x*r12.x)*dsindC.y
				//                +(2.0*r23.y*r12.z - r12.y*r23.z)*dsindC.z
				//                +(2.0*r23.y*r12.x - r12.y*r23.x)*dsindC.x
				//                +dsindB.x*r34.z - dsindB.z*r34.x);
				//         f2.z += K1*(-(r23.x*r12.x + r23.y*r12.y)*dsindC.z
				//                +(2.0*r23.z*r12.x - r12.z*r23.x)*dsindC.x
				//                +(2.0*r23.z*r12.y - r12.z*r23.y)*dsindC.y
				//                +dsindB.y*r34.x - dsindB.x*r34.y);
				//       }
				//     }    // end loop over multiplicity
				//     f[dihedral->atom1] += f1;
				//     f[dihedral->atom2] += f2-f1;
				//     f[dihedral->atom3] += f3-f2;
				//     f[dihedral->atom4] += -f3;
				// 
				//     dihedral++; 
				//   }
				//   return energy;
				// }
				#endregion

				///////////////////////////////////////////////////////////////////////////////
				// energy
                Vector pos0 = coords[0];                             //     const Vector *pos0 = coords + dihedral->atom1;
                Vector pos1 = coords[1];                             //     const Vector *pos1 = coords + dihedral->atom2;
                Vector pos2 = coords[2];                             //     const Vector *pos2 = coords + dihedral->atom3;
                Vector pos3 = coords[3];                             //     const Vector *pos3 = coords + dihedral->atom4;
                Vector r12 = pos0 - pos1;                            //     const Vector r12 = *pos0 - *pos1;
                Vector r23 = pos1 - pos2;                            //     const Vector r23 = *pos1 - *pos2;
                Vector r34 = pos2 - pos3;                            //     const Vector r34 = *pos2 - *pos3;
                                                                     // 
                Vector dcosdA = null;                                //     Vector dcosdA;
                Vector dcosdB = null;                                //     Vector dcosdB;
                Vector dsindC = null;                                //     Vector dsindC;
                Vector dsindB = null;                                //     Vector dsindB;
                                                                     // 
                Vector A, B, C;                                      //     Vector A, B, C;
				A = LinAlg.CrossProd(r12, r23);                      //     A.cross(r12, r23);
				B = LinAlg.CrossProd(r23, r34);                      //     B.cross(r23, r34);
				C = LinAlg.CrossProd(r23, A);                        //     C.cross(r23, A);
                                                                     // 
                double rA = A.Dist;                                  //     double rA = A.length();
				double rB = B.Dist;                                  //     double rB = B.length(); 
				double rC = C.Dist;                                  //     double rC = C.length();
                                                                     // 
                double cos_phi = LinAlg.DotProd(A,B)/(rA*rB);        //     double cos_phi = (A*B)/(rA*rB);
				double sin_phi = LinAlg.DotProd(C,B)/(rC*rB);        //     double sin_phi = (C*B)/(rC*rB);
                                                                     // 
                // Normalize B                                       //     // Normalize B
                rB = 1.0/rB;                                         //     rB = 1.0/rB;
                B *= rB;                                             //     B *= rB;
                                                                     // 
                double phi = -Math.Atan2(sin_phi, cos_phi);          //     double phi = -atan2(sin_phi, cos_phi);
                                                                     // 
                if (Math.Abs(sin_phi) > 0.1) {                       //     if (fabs(sin_phi) > 0.1) {
                // Normalize A                                       //       // Normalize A
                rA = 1.0/rA;                                         //       rA = 1.0/rA;
                A *= rA;                                             //       A *= rA;
                dcosdA = rA*(cos_phi*A-B);                           //       dcosdA = rA*(cos_phi*A-B);
                dcosdB = rB*(cos_phi*B-A);                           //       dcosdB = rB*(cos_phi*B-A);
                }                                                    //     }
                else {                                               //     else {
                // Normalize C                                       //       // Normalize C
                rC = 1.0/rC;                                         //       rC = 1.0/rC;
                C *= rC;                                             //       C *= rC;
                dsindC = rC*(sin_phi*C-B);                           //       dsindC = rC*(sin_phi*C-B);
                dsindB = rB*(sin_phi*B-C);                           //       dsindB = rB*(sin_phi*B-C);
                }                                                    //     }
                                                                     // 
                //int mult = dihedral->multiplicity;                 //     int mult = dihedral->multiplicity; 
                //for (int j=0; j<mult; j++) {                       //     for (int j=0; j<mult; j++) {
                //double k = dihedral->k[j];                         //       double k = dihedral->k[j];
                //double n = dihedral->n[j];                         //       double n = dihedral->n[j];
                //double delta = dihedral->delta[j];                 //       double delta = dihedral->delta[j];
                double K, K1;                                        //       double K, K1;
                if (n != 0) {                                        //       if (n) {
                    K = Kchi * (1.0+Math.Cos(n*phi + delta));        //         K = k * (1.0+cos(n*phi + delta)); 
                    K1 = -n*Kchi*Math.Sin(n*phi + delta);            //         K1 = -n*k*sin(n*phi + delta);
                }                                                    //       }
                else {                                               //       else {
                    double diff = phi-delta;                         //         double diff = phi-delta;
                    if (diff < -Math.PI) diff += 2.0*Math.PI;        //         if (diff < -M_PI) diff += 2.0*M_PI;
                    else if (diff > Math.PI) diff -= 2.0*Math.PI;    //         else if (diff > M_PI) diff -= 2.0*M_PI;
                    K = Kchi*diff*diff;                              //         K = k*diff*diff;
                    K1 = 2.0*Kchi*diff;                              //         K1 = 2.0*k*diff;
                }                                                    //       }
                HDebug.Assert(double.IsNaN(K) == false, double.IsInfinity(K) == false);
                energy += K;                                         //       energy += K;
				///////////////////////////////////////////////////////////////////////////////
				// force
				if(forces != null)
				{
					Vector f1 = new double[3];                                    //     Vector f1, f2, f3;
					Vector f2 = new double[3];                                    // 
					Vector f3 = new double[3];                                    // 
					// forces                                                     //       // forces
					if(Math.Abs(sin_phi) > 0.1) {                                 //       if (fabs(sin_phi) > 0.1) {
						K1 = K1/sin_phi;                                          //         K1 = K1/sin_phi;
						f1[0] += K1*(r23[1]*dcosdA[2] - r23[2]*dcosdA[1]);        //         f1.x += K1*(r23.y*dcosdA.z - r23.z*dcosdA.y);
						f1[1] += K1*(r23[2]*dcosdA[0] - r23[0]*dcosdA[2]);        //         f1.y += K1*(r23.z*dcosdA.x - r23.x*dcosdA.z);
						f1[2] += K1*(r23[0]*dcosdA[1] - r23[1]*dcosdA[0]);        //         f1.z += K1*(r23.x*dcosdA.y - r23.y*dcosdA.x);
				 		                                                          // 
						f3[0] += K1*(r23[2]*dcosdB[1] - r23[1]*dcosdB[2]);        //         f3.x += K1*(r23.z*dcosdB.y - r23.y*dcosdB.z);
						f3[1] += K1*(r23[0]*dcosdB[2] - r23[2]*dcosdB[0]);        //         f3.y += K1*(r23.x*dcosdB.z - r23.z*dcosdB.x);
						f3[2] += K1*(r23[1]*dcosdB[0] - r23[0]*dcosdB[1]);        //         f3.z += K1*(r23.y*dcosdB.x - r23.x*dcosdB.y);
				 		                                                          // 
						f2[0] += K1*(r12[2]*dcosdA[1] - r12[1]*dcosdA[2]          //         f2.x += K1*(r12.z*dcosdA.y - r12.y*dcosdA.z
						         + r34[1]*dcosdB[2] - r34[2]*dcosdB[1]);          //                  + r34.y*dcosdB.z - r34.z*dcosdB.y);
						f2[1] += K1*(r12[0]*dcosdA[2] - r12[2]*dcosdA[0]          //         f2.y += K1*(r12.x*dcosdA.z - r12.z*dcosdA.x
						         + r34[2]*dcosdB[0] - r34[0]*dcosdB[2]);          //                  + r34.z*dcosdB.x - r34.x*dcosdB.z);
						f2[2] += K1*(r12[1]*dcosdA[0] - r12[0]*dcosdA[1]          //         f2.z += K1*(r12.y*dcosdA.x - r12.x*dcosdA.y
						         + r34[0]*dcosdB[1] - r34[1]*dcosdB[0]);          //                  + r34.x*dcosdB.y - r34.y*dcosdB.x);
					}		                                                      //       }
					else {	                                                      //       else {
						//  This angle is closer to 0 or 180 than it is to        //         //  This angle is closer to 0 or 180 than it is to
						//  90, so use the cos version to avoid 1/sin terms       //         //  90, so use the cos version to avoid 1/sin terms
						K1 = -K1/cos_phi;                                         //         K1 = -K1/cos_phi;
				 		                                                          // 
						f1[0] += K1*((r23[1]*r23[1] + r23[2]*r23[2])*dsindC[0]    //         f1.x += K1*((r23.y*r23.y + r23.z*r23.z)*dsindC.x
						         - r23[0]*r23[1]*dsindC[1]                        //                 - r23.x*r23.y*dsindC.y
						         - r23[0]*r23[2]*dsindC[2]);                      //                 - r23.x*r23.z*dsindC.z);
						f1[1] += K1*((r23[2]*r23[2] + r23[0]*r23[0])*dsindC[1]    //         f1.y += K1*((r23.z*r23.z + r23.x*r23.x)*dsindC.y
						         - r23[1]*r23[2]*dsindC[2]                        //                 - r23.y*r23.z*dsindC.z
						         - r23[1]*r23[0]*dsindC[0]);                      //                 - r23.y*r23.x*dsindC.x);
						f1[2] += K1*((r23[0]*r23[0] + r23[1]*r23[1])*dsindC[2]    //         f1.z += K1*((r23.x*r23.x + r23.y*r23.y)*dsindC.z
						         - r23[2]*r23[0]*dsindC[0]                        //                 - r23.z*r23.x*dsindC.x
						         - r23[2]*r23[1]*dsindC[1]);                      //                 - r23.z*r23.y*dsindC.y);
				 		                                                          // 
						f3 += K1*LinAlg.CrossProd(dsindB,r23);                    //         f3 += cross(K1,dsindB,r23);
				 		                                                          // 
						f2[0] += K1*(-(r23[1]*r12[1] + r23[2]*r12[2])*dsindC[0]   //         f2.x += K1*(-(r23.y*r12.y + r23.z*r12.z)*dsindC.x
						         +(2.0*r23[0]*r12[1] - r12[0]*r23[1])*dsindC[1]   //                +(2.0*r23.x*r12.y - r12.x*r23.y)*dsindC.y
						         +(2.0*r23[0]*r12[2] - r12[0]*r23[2])*dsindC[2]   //                +(2.0*r23.x*r12.z - r12.x*r23.z)*dsindC.z
						         +dsindB[2]*r34[1] - dsindB[1]*r34[2]);           //                +dsindB.z*r34.y - dsindB.y*r34.z);
						f2[1] += K1*(-(r23[2]*r12[2] + r23[0]*r12[0])*dsindC[1]   //         f2.y += K1*(-(r23.z*r12.z + r23.x*r12.x)*dsindC.y
						         +(2.0*r23[1]*r12[2] - r12[1]*r23[2])*dsindC[2]   //                +(2.0*r23.y*r12.z - r12.y*r23.z)*dsindC.z
						         +(2.0*r23[1]*r12[0] - r12[1]*r23[0])*dsindC[0]   //                +(2.0*r23.y*r12.x - r12.y*r23.x)*dsindC.x
						         +dsindB[0]*r34[2] - dsindB[2]*r34[0]);           //                +dsindB.x*r34.z - dsindB.z*r34.x);
						f2[2] += K1*(-(r23[0]*r12[0] + r23[1]*r12[1])*dsindC[2]   //         f2.z += K1*(-(r23.x*r12.x + r23.y*r12.y)*dsindC.z
						         +(2.0*r23[2]*r12[0] - r12[2]*r23[0])*dsindC[0]   //                +(2.0*r23.z*r12.x - r12.z*r23.x)*dsindC.x
						         +(2.0*r23[2]*r12[1] - r12[2]*r23[1])*dsindC[1]   //                +(2.0*r23.z*r12.y - r12.z*r23.y)*dsindC.y
						         +dsindB[1]*r34[0] - dsindB[0]*r34[1]);           //                +dsindB.y*r34.x - dsindB.x*r34.y);
					}		                                                      //       }
                    //}    // end loop over multiplicity                          //     }    // end loop over multiplicity
					forces[0] += f1;                                              //     f[dihedral->atom1] += f1;
					forces[1] += f2-f1;                                           //     f[dihedral->atom2] += f2-f1;
					forces[2] += f3-f2;                                           //     f[dihedral->atom3] += f3-f2;
					forces[3] += -f3;                                             //     f[dihedral->atom4] += -f3;
				}
				///////////////////////////////////////////////////////////////////////////////
				// hessian
				if(hessian != null)
				{
                    //Debug.Assert(false);
                    double dcoord = 0.0001;
                    HDebug.Assert(hessian.GetLength(0) == 4, hessian.GetLength(1) == 4);
                    NumericSolver.Derivative2(ComputeFunc, coords, dcoord, ref hessian, Kchi, n, delta);
                }
			}
            public static double ComputeFunc(Vector[] coords, double[] info)
            {
                HDebug.Assert(info.Length == 3);
                double energy = 0;
                Vector[] forces = null;
                MatrixByArr[,] hessian = null;
                Compute(coords, ref energy, ref forces, ref hessian, info[0], info[1], info[2]);
                return energy;
            }
        }
	}
}
