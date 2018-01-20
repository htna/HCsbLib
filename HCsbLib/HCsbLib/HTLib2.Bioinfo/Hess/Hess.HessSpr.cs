using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public partial class HessSpr
        {
            public static Matrix GetHess01(Vector coord0, Vector coord1, double k01)
            {
                Vector v01 = coord1 - coord0;
                double sca = k01 * (-1 / v01.Dist2);
                Matrix hess01 = new double[3, 3] {
                                    { sca*v01[0]*v01[0], sca*v01[0]*v01[1], sca*v01[0]*v01[2] },
                                    { sca*v01[1]*v01[0], sca*v01[1]*v01[1], sca*v01[1]*v01[2] },
                                    { sca*v01[2]*v01[0], sca*v01[2]*v01[1], sca*v01[2]*v01[2] },
                                };
                return hess01;
            }
            public static void UpdateHess01(IList<Vector> coords, int id0, int id1, double k01, Matrix hessian)
            {
                if(HDebug.Selftest())
                    #region Check with ANM hessian matrix when k01=1
                {
                    Random rand = new Random();
                    Vector[] pts = new Vector[10];
                    for(int i=0; i<pts.Length; i++)
                        pts[i] = new double[3] { rand.NextDouble(), rand.NextDouble(), rand.NextDouble() };
                    Matrix hess = new double[pts.Length*3, pts.Length*3];
                    for(int c=0; c<pts.Length-1; c++)
                        for(int r=c+1; r<pts.Length; r++)
                        {
                            UpdateHess01(pts, c, r, 1, hess);
                        }
                    Matrix anmhess = Hess.GetHessAnm(pts, double.PositiveInfinity);
                    Matrix dhess = hess - anmhess;
                    HDebug.AssertToleranceMatrix(0.00000001, dhess);
                }
                    #endregion

                if(k01 == 0)
                    return;
                //Debug.Assert(k01 > 0);
                Matrix hess01 = GetHess01(coords[id0], coords[id1], k01);
                Matrix hess10 = GetHess01(coords[id1], coords[id0], k01);
                double maxabs_lhess = hess01.ToArray().HAbs().HMax();
                //HDebug.Assert(maxabs_lhess < 10000);

                for(int dc=0; dc<3; dc++)
                    for(int dr=0; dr<3; dr++)
                    {
                        hessian[3*id0+dc, 3*id0+dr] -= hess01[dc, dr];
                        hessian[3*id0+dc, 3*id1+dr] += hess01[dc, dr];
                        hessian[3*id1+dc, 3*id0+dr] += hess10[dc, dr];
                        hessian[3*id1+dc, 3*id1+dr] -= hess10[dc, dr];
                    }
            }
            //public static void UpdateHess01(IList<Vector> coords, int id0, int id1, double k01, MatrixSparse<MatrixByArr> hessian)
            //{
            //    if(k01 == 0)
            //        return;
            //    //Debug.Assert(k01 > 0);
            //    MatrixByArr hess01 = GetHess01(coords[id0], coords[id1], k01);
            //    MatrixByArr hess10 = GetHess01(coords[id1], coords[id0], k01);
            //    double maxabs_lhess = hess01.ToArray().HAbs().HMax();
            //    HDebug.Assert(maxabs_lhess < 10000);
            //
            //    hessian[id0, id0] -= hess01;
            //    hessian[id0, id1] += hess01;
            //    hessian[id1, id0] += hess10;
            //    hessian[id1, id1] -= hess10;
            //}
            public static HessMatrix GetHessBond(IList<Vector> coords, IEnumerable<Universe.Bond> bonds, HessMatrix hessian=null)
            {
                /// BONDS
                /// 
                /// STeM     : V1      = Kr(r - r0)^2
                /// charmm22 : V(bond) = Kb(b - b0)**2
                /// 
                /// Kb: kcal/mole/A**2
                /// b0: A
                int size = coords.Count;
                if(hessian == null)
                    hessian = HessMatrixSparse.ZerosSparse(size*3, size*3);

                foreach(Universe.Bond bond in bonds)
                {
                    int id0 = bond.atoms[0].ID; if(coords[id0] == null) continue;
                    int id1 = bond.atoms[1].ID; if(coords[id1] == null) continue;
                    double k01 = bond.Kb;
                    UpdateHess01(coords, id0, id1, k01, hessian);
                }

                return hessian;
            }
            public static HessMatrix GetHessAngle(IList<Vector> coords, IEnumerable<Universe.Angle> angles, HessMatrix hessian=null)
            {
                /// ANGLES
                /// 
                /// STeM     : V2              = Ktheta (Theta - Theta0)^2
                /// charmm22 : V(angle)        = Ktheta (Theta - Theta0)**2
                ///            V(Urey-Bradley) = Kub(S - S0)**2
                /// 
                /// Ktheta: kcal/mole/rad**2
                /// Theta0: degrees
                /// Kub: kcal/mole/A**2 (Urey-Bradley)
                /// S0: A
                int size = coords.Count;
                if(hessian == null)
                    hessian = HessMatrixSparse.ZerosSparse(size*3, size*3);

                foreach(Universe.Angle angle in angles)
                {
                    //STeM.SecondTerm(caArray, K_theta, hessian, i, i+1, i+2);
                    int id0 = angle.atoms[0].ID; if(coords[id0] == null) continue;
                    int id1 = angle.atoms[1].ID; if(coords[id1] == null) continue;
                    int id2 = angle.atoms[2].ID; if(coords[id2] == null) continue;

                    double k012 = angle.Ktheta;
                    Vector[] coord012 = new Vector[] { coords[id0], coords[id1], coords[id2] };
                    Matrix hess012 = Hess.STeM.SecondTerm(coord012, k012);

                    int[] ids = new int[] { id0, id1, id2 };
                    for(int c=0; c<3; c++) for(int dc=0; dc<3; dc++)
                    for(int r=0; r<3; r++) for(int dr=0; dr<3; dr++)
                        hessian[ids[c]*3+dc, ids[r]*3+dr] += hess012[c*3+dc, r*3+dr];

                    double Kub = angle.Kub;
                    UpdateHess01(coords, id0, id2, Kub, hessian);
                }

                return hessian;
            }
            public static HessMatrix GetHessImproper(IList<Vector> coords, IEnumerable<Universe.Improper> impropers, HessMatrix hessian=null)
            {
                /// IMPROPER : Kpsi(psi - psi0)**2
                ///            Kpsi: kcal/mole/rad**2
                ///            psi0: degrees
                ///            note that the second column of numbers (0) is ignored
                /// 
                /// STeM     : V3          = ktheta1 ( 1 - cos(theta - theta0))
                ///                        = ktheta1/2 * (theta - theta0)^2 + O(theta^3)
                ///                       => ktheta1/2 * (theta - theta0)^2
                /// charmm22 : V(improper) = Kpsi        (psi   - psi0  )^2
                ///                       (= 2*Kpsi    * (psi   - psi0  )^2 - O(  psi^3)
                ///                       (= 2*Kpsi  ( 1 - cos(theta - theta0))
                /// charmm22 => STeM       : ktheta = 2*Kpsi
                ///                          n      = 1
                ///                          theta  = from coords
                ///                          theta0 = not use
                /// 
                int size = coords.Count;
                if(hessian == null)
                    hessian = HessMatrixSparse.ZerosSparse(size*3, size*3);

                foreach(Universe.Improper improper in impropers)
                {
                    int id0 = improper.atoms[0].ID; if(coords[id0] == null) continue;
                    int id1 = improper.atoms[1].ID; if(coords[id1] == null) continue;
                    int id2 = improper.atoms[2].ID; if(coords[id2] == null) continue;
                    int id3 = improper.atoms[3].ID; if(coords[id3] == null) continue;
                    double Ktheta = 2 * improper.Kpsi;
                    double n      = 1;
                  //double theta0 = double.NaN;

                    Vector[] coord0123 = new Vector[] { coords[id0], coords[id1], coords[id2], coords[id3] };
                    Matrix hess0123 = Hess.STeM.ThirdTermN(coord0123, Ktheta, n);

                    int[] ids = new int[] { id0, id1, id2, id3 };
                    for(int c=0; c<4; c++) for(int dc=0; dc<3; dc++)
                    for(int r=0; r<4; r++) for(int dr=0; dr<3; dr++)
                        hessian[ids[c]*3+dc, ids[r]*3+dr] += hess0123[c*3+dc, r*3+dr];
                }

                return hessian;
            }
            public static HessMatrix GetHessDihedral(IList<Vector> coords, IEnumerable<Universe.Dihedral> dihedrals, HessMatrix hessian=null)
            {
                /// DIHEDRALS
                /// 
                /// charmm22 : V(dihedral) = Kchi    ( 1 + cos(n*chi - delta ))
                ///                        = Kchi    ( 1 - cos(n*chi - delta + Pi))
                ///                        = Kchi    ( 1 - cos n(chi   - (delta-Pi)/n))
                /// STeM     : V3      = sum kthetan ( 1 - cos n(theta - theta0      )) , where n∈{1,3}
                ///                    = sum kthetan*n*n/2 * (theta - theta0)^2   , where n∈{1,3}
                ///                  => call ThirdTerm : Ktheta = ktheta1*1*1/2+ktheta3*3*3/2
                ///                                      n      = not use
                ///                                      theta  = from coords
                ///                                      theta0 = not use
                /// charmm22 -> STeM       : Ktheta = Kchi
                ///                          n      = n
                ///                          theta  = from coords
                ///                          theta0 = (delta-Pi)/n  ((= not use
                /// 
                /// Kchi: kcal/mole
                /// n: multiplicity
                /// delta: degrees
                int size = coords.Count;
                if(hessian == null)
                    hessian = HessMatrixSparse.ZerosSparse(size*3, size*3);

                foreach(Universe.Dihedral dihedral in dihedrals)
                {
                    //STeM.ThirdTermN(coords, lK_chi, ln, hessian, idx0, idx1, idx2, idx3);
                    int id0 = dihedral.atoms[0].ID; if(coords[id0] == null) continue;
                    int id1 = dihedral.atoms[1].ID; if(coords[id1] == null) continue;
                    int id2 = dihedral.atoms[2].ID; if(coords[id2] == null) continue;
                    int id3 = dihedral.atoms[3].ID; if(coords[id3] == null) continue;
                    double Ktheta = dihedral.Kchi;
                    double n      = dihedral.n;
                    double theta0 = (dihedral.delta - Math.PI)/n;
                    double theta  = Geometry.TorsionalAngle(coords[id0], coords[id1], coords[id2], coords[id3]);

                    HDebug.Assert(Ktheta >= 0);
                    Vector[] coord0123 = new Vector[] { coords[id0], coords[id1], coords[id2], coords[id3] };
                    Matrix hess0123 = Hess.STeM.ThirdTermN(coord0123, Ktheta, n);

                    int[] ids = new int[] { id0, id1, id2, id3 };
                    for(int c=0; c<4; c++) for(int dc=0; dc<3; dc++)
                    for(int r=0; r<4; r++) for(int dr=0; dr<3; dr++)
                        hessian[ids[c]*3+dc, ids[r]*3+dr] += hess0123[c*3+dc, r*3+dr];
                }

                return hessian;
            }
            public enum NbndType { nbnd, nbnd14 };
            public delegate double FnGetSprNonbondCustom       (Universe.Atom atom0, Universe.Atom atom1, Vector coord0, Vector coord1, NbndType nbndtype, bool vdW, bool elec);
            public class NbndInfo
            {
                public NbndType nbndtype;
                public string atomelem;
                public double rmin2, charge, epsilon;
            }
            public static double GetSprNonbondCustom( Universe.Atom atom0, Universe.Atom atom1
                                                    , Vector coord0, Vector coord1
                                                    , NbndType nbndtype, double D, bool vdW, bool elec
                                                    , HPack<NbndInfo> nbinfo0, HPack<NbndInfo> nbinfo1
                                                    , double? maxAbsSpring // = null
                                                    , params NbndInfo[] nbndinfos )
            {
                NbndInfo infoi = null; foreach(var info in nbndinfos) if((nbndtype == info.nbndtype) && (atom0.AtomElem == info.atomelem)) { infoi=info; break; } HDebug.Assert(infoi != null);
                NbndInfo infoj = null; foreach(var info in nbndinfos) if((nbndtype == info.nbndtype) && (atom1.AtomElem == info.atomelem)) { infoj=info; break; } HDebug.Assert(infoj != null);

                double ri0 = infoi.rmin2; double qi = infoi.charge; double ei = infoi.epsilon;
                double rj0 = infoj.rmin2; double qj = infoj.charge; double ej = infoj.epsilon;

                double Kij = GetKijFijNbnd(vdW, elec
                                   , coord0, ri0, qi, ei
                                   , coord1, rj0, qj, ej
                                   , D
                                   ).Kij;

                if(HDebug.False)
                    #region [for debugging] check maximum abs spring-constant between H-H, H-X, X-H
                    if((infoi.atomelem == "H") || (infoj.atomelem == "H"))
                    {
                        /// max spring: 9320.0095197395112 for protein (1I2T minimized by tinker with hydrogen, 61 residues, 472 atoms)
                        //HDebug.debuglog["max spring"] = Math.Max(HDebug.debuglog["max spring"].Double, Math.Abs(Kij)).HToHDynamic();
                    }
                    #endregion
                if(HDebug.False)
                    #region [for debugging] check maximum abs spring-constant
                    {
                        /// max spring: 9320.0095197395112 for protein (1I2T minimized by tinker with hydrogen, 61 residues, 472 atoms)
                        //HDebug.debuglog["max spring"] = Math.Max(HDebug.debuglog["max spring"].Double, Math.Abs(Kij)).HToHDynamic();
                    }
                    #endregion
                if(maxAbsSpring != null)
                {
                    if(Math.Abs(Kij) > maxAbsSpring)
                    {
                        double sign = Math.Sign(Kij);
                        Kij = sign * maxAbsSpring.Value;
                    }
                }

                if(nbinfo0 != null) nbinfo0.value = infoi;
                if(nbinfo1 != null) nbinfo1.value = infoj;
                return Kij;
            }
//          public static NbndInfo[] nbndinfo_set_unif0 = new NbndInfo[]
//          {
//              // http://en.wikipedia.org/wiki/Van_Der_Waals_Radius
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='H', rmin2=1.20, charge=0.0, epsilon=-0.1}, // Hydrogen   1.20
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='C', rmin2=1.70, charge=0.0, epsilon=-0.1}, // Carbon     1.70 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='N', rmin2=1.55, charge=0.0, epsilon=-0.1}, // Nitrogen   1.55 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='O', rmin2=1.52, charge=0.0, epsilon=-0.1}, // Oxygen     1.52 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='F', rmin2=1.47, charge=0.0, epsilon=-0.1}, // Fluorine   1.47 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='P', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Phosphorus 1.80 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='S', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Sulfur     1.80 
//                                                                                                         // Chlorine   1.75 
//                                                                                                         // Copper     1.4 
//
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='H', rmin2=1.20, charge=0.0, epsilon=-0.1}, // Hydrogen   1.20
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='C', rmin2=1.70, charge=0.0, epsilon=-0.1}, // Carbon     1.70 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='N', rmin2=1.55, charge=0.0, epsilon=-0.1}, // Nitrogen   1.55 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='O', rmin2=1.52, charge=0.0, epsilon=-0.1}, // Oxygen     1.52 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='F', rmin2=1.47, charge=0.0, epsilon=-0.1}, // Fluorine   1.47 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='P', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Phosphorus 1.80 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='S', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Sulfur     1.80 
//                                                                                                           // Chlorine   1.75 
//                                                                                                           // Copper     1.4 
//          };
//          public static NbndInfo[] nbndinfo_set_unif = new NbndInfo[]
//          {
//              // http://en.wikipedia.org/wiki/Van_Der_Waals_Radius
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='H', rmin2=1.20, charge=0.0, epsilon=-0.1}, // Hydrogen   1.20
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='C', rmin2=1.70, charge=0.0, epsilon=-0.1}, // Carbon     1.70 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='N', rmin2=1.55, charge=0.0, epsilon=-0.1}, // Nitrogen   1.55 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='O', rmin2=1.52, charge=0.0, epsilon=-0.1}, // Oxygen     1.52 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='F', rmin2=1.47, charge=0.0, epsilon=-0.1}, // Fluorine   1.47 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='P', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Phosphorus 1.80 
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='S', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Sulfur     1.80 
//                                                                                                         // Chlorine   1.75 
//                                                                                                         // Copper     1.4 
//
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='H', rmin2=1.20, charge=0.0, epsilon=-0.1}, // Hydrogen   1.20
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='C', rmin2=1.70, charge=0.0, epsilon=-0.1}, // Carbon     1.70 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='N', rmin2=1.55, charge=0.0, epsilon=-0.1}, // Nitrogen   1.55 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='O', rmin2=1.40, charge=0.0, epsilon=-0.1}, // Oxygen     1.52 /////// special treatment 1.52 -> 1.40
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='F', rmin2=1.47, charge=0.0, epsilon=-0.1}, // Fluorine   1.47 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='P', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Phosphorus 1.80 
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='S', rmin2=1.80, charge=0.0, epsilon=-0.1}, // Sulfur     1.80 
//                                                                                                           // Chlorine   1.75 
//                                                                                                           // Copper     1.4 
//          };
            public static NbndInfo[] nbndinfo_set_SSTeMcs6c = new NbndInfo[]
            {
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="C", rmin2=1.90, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="H", rmin2=1.20, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="O", rmin2=1.70, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="N", rmin2=1.85, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="S", rmin2=2.00, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="F", rmin2=1.47, charge=0.0, epsilon=-0.1}, /// same to unif0
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="P", rmin2=1.80, charge=0.0, epsilon=-0.1}, /// same to unif0
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="C", rmin2=1.90, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="H", rmin2=1.20, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="O", rmin2=1.40, charge=0.0, epsilon=-0.1}, /// special treatment: 1.70 -> 1.40
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="N", rmin2=1.55, charge=0.0, epsilon=-0.1}, /// special treatment: 1.85 -> 1.55
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="S", rmin2=2.00, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="F", rmin2=1.47, charge=0.0, epsilon=-0.1}, /// same to unif0
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="P", rmin2=1.80, charge=0.0, epsilon=-0.1}, /// same to unif0
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="", rmin2=0.00, charge=0.0, epsilon=-0.1},
                // http://en.wikipedia.org/wiki/Van_der_Waals_radius
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="CL", rmin2=1.75, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="CU", rmin2=1.40, charge=0.0, epsilon=-0.1},
                // http://en.wikipedia.org/wiki/Atomic_radii_of_the_elements_(data_page)
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="MG", rmin2=1.73, charge=0.0, epsilon=-0.1}, // magnesium
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="K" , rmin2=2.75, charge=0.0, epsilon=-0.1}, // potassium
                new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="POT",rmin2=2.75, charge=0.0, epsilon=-0.1}, // potassium
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // par_all27_prot_na.prm
                // FE     0.010000   0.000000     0.650000 ! ALLOW HEM
                new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="FE" ,rmin2=0.65, charge=0.0, epsilon=-0.1},
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="FE" ,rmin2=0.65, charge=0.0, epsilon=-0.1},
            };
            public static NbndInfo[] nbndinfo_set_L79 = new NbndInfo[]
            {
                new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="H", rmin2=2.6525/2, charge=0.0, epsilon=-0.001  },
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="H", rmin2=2.6525/2, charge=0.0, epsilon=-0.001  },
                new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="O", rmin2=3.2005/2, charge=0.0, epsilon=-0.18479},
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="O", rmin2=3.2005/2, charge=0.0, epsilon=-0.18479},
                new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="N", rmin2=3.2171/2, charge=0.0, epsilon=-0.41315},
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="N", rmin2=3.2171/2, charge=0.0, epsilon=-0.41315},
                new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="S", rmin2=3.9150/2, charge=0.0, epsilon=-0.07382},
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="S", rmin2=3.9150/2, charge=0.0, epsilon=-0.07382},
                new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="C", rmin2=3.9202/2, charge=0.0, epsilon=-0.05573}, // (0.07382 + 0.03763)/2 = 0.055725
                new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="C", rmin2=3.9202/2, charge=0.0, epsilon=-0.05573}, // (0.07382 + 0.03763)/2 = 0.055725
            };
//          public static NbndInfo[] nbndinfo_set_SSTeMcs6d = new NbndInfo[]
//          {
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='C', rmin2=1.90+0.6, charge=0.0, epsilon=-0.1},
//              //new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='H', rmin2=1.20, charge=0.0, epsilon=-0.1},
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='O', rmin2=1.70+0.6, charge=0.0, epsilon=-0.1},
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='N', rmin2=1.85+0.6, charge=0.0, epsilon=-0.1},
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='S', rmin2=2.00+0.6, charge=0.0, epsilon=-0.1},
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='F', rmin2=1.47+0.6, charge=0.0, epsilon=-0.1}, /// same to unif0
//              new NbndInfo{ nbndtype=NbndType.nbnd, atomtype='P', rmin2=1.80+0.6, charge=0.0, epsilon=-0.1}, /// same to unif0
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='C', rmin2=1.90+0.6, charge=0.0, epsilon=-0.1},
//              //new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='H', rmin2=1.20, charge=0.0, epsilon=-0.1},
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='O', rmin2=1.40+0.6, charge=0.0, epsilon=-0.1}, /// special treatment: 1.70 -> 1.40
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='N', rmin2=1.55+0.6, charge=0.0, epsilon=-0.1}, /// special treatment: 1.85 -> 1.55
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='S', rmin2=2.00+0.6, charge=0.0, epsilon=-0.1},
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='F', rmin2=1.47+0.6, charge=0.0, epsilon=-0.1}, /// same to unif0
//              new NbndInfo{ nbndtype=NbndType.nbnd14, atomtype='P', rmin2=1.80+0.6, charge=0.0, epsilon=-0.1}, /// same to unif0
//          };

            public static double GetSprNonbondCustomGromacs(Universe.Atom atom0, Universe.Atom atom1, Vector coord0, Vector coord1, NbndType nbndtype, double D, bool vdW, bool elec)
            {
                if(nbndtype == NbndType.nbnd)
                {
                    var par0 = atom0.ConvertGromacsToCharmm;
                    var par1 = atom1.ConvertGromacsToCharmm;
                    double ri0 = par0.Rmin2; //atom0.Rmin2;
                    double rj0 = par1.Rmin2; //atom1.Rmin2;
                    double qi  = atom0.Charge;
                    double qj  = atom1.Charge;
                    double ei  = par0.eps; //atom0.epsilon;
                    double ej  = par1.eps; //atom1.epsilon;

                    double Kij = GetKijFijNbnd(vdW, elec
                                        , coord0, ri0, qi, ei
                                        , coord1, rj0, qj, ej
                                        , D
                                        ).Kij;
                    return Kij;
                }
                if(nbndtype == NbndType.nbnd14)
                {
                    var par0 = atom0.ConvertGromacsToCharmm;
                    var par1 = atom1.ConvertGromacsToCharmm;
                    double ri0 = par0.Rmin2_14; if(double.IsNaN(ri0)) ri0 = par0.Rmin2; //atom0.Rmin2_14; if(double.IsNaN(ri0)) ri0 = atom0.Rmin2;
                    double rj0 = par1.Rmin2_14; if(double.IsNaN(rj0)) rj0 = par1.Rmin2; //atom1.Rmin2_14; if(double.IsNaN(rj0)) rj0 = atom1.Rmin2;
                    double qi = atom0.Charge;
                    double qj = atom1.Charge;
                    double ei = par0.eps_14; if(double.IsNaN(ei)) ei = par0.eps; //atom0.eps_14; if(double.IsNaN(ei)) ei = atom0.epsilon;
                    double ej = par1.eps_14; if(double.IsNaN(ej)) ej = par1.eps; //atom1.eps_14; if(double.IsNaN(ej)) ej = atom1.epsilon;

                    double Kij = GetKijFijNbnd(vdW, elec
                                        , coord0, ri0, qi, ei
                                        , coord1, rj0, qj, ej
                                        , D
                                        ).Kij;
                    return Kij;
                }
                HDebug.Assert(false);
                return double.NaN;
            }

            public struct CustomKijInfo
            {
                public Vector coordi; public double ri0, qi, ei;
                public Vector coordj; public double rj0, qj, ej;
                public double Kij, Fij;
                public double? maxAbsSpring; // = null
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// http://www.ks.uiuc.edu/Training/Tutorials/science/forcefield-tutorial/forcefield-html/node5.html
            /// http://www.charmmtutorial.org/index.php/The_Energy_Function
            /// 
            /// NONBONDED
            /// 
            /// charmm : V_nbnd   = epsilon_ij (              (r0ij / rij)^12  -       2  * (r0ij / rij)^6 )   +           (qi*qj)/(epsilon*rij)
            ///                   = epsilon_ij (             r0ij^12 * rij^-12 -       2  * r0ij^6 * rij^-6)   +           (qi*qj/epsilon) * rij^-1
            ///          spr_nbnd = epsilon_ij ( (-12*-13) * r0ij^12 * rij^-14 -(-6*-7*2) * r0ij^6 * rij^-8)   +   (-1*-2)*(qi*qj/epsilon) * rij^-3
            ///                   = epsilon_ij (      156  * r0ij^12 * rij^-14 +       84 * r0ij^6 * rij^-8)   +         2*(qi*qj/epsilon) * rij^-3
            /// 
            /// V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
            /// 
            /// epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
            /// Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
            /// 
            /// atom  ignored    epsilon      Rmin/2   ignored   eps,1-4       Rmin/2,1-4
            /// 
            /// V(electrostatic) : V(i,j) = (qi*qj)/(epsilon*rij)
            ///                           = 332*qi*qj/r_ij/D
            ///                    Where D is dielectric constant, for proteins. D is normally 80. (see equation (13) in the following pdf file). 
            ///                    http://pharmacy.ucsd.edu/labs/gilson/ce_www1a.pdf
            ///                    epsilon = D/332
            public static HessMatrix GetHessNonbond
                ( IList<Vector> coords
                , IEnumerable<Universe.Nonbonded> nonbondeds
                , double D
                , string opt
                , HessMatrix hessian=null
                , bool vdW=true
                , bool elec=true
                , bool ignNegSpr=false
                , double? maxAbsSpring=null
                , string pathFileRijKij=null
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, double> collector=null
                , Func<CustomKijInfo, double> GetCustomKij=null
                )
            {
                int size = coords.Count;
                if(hessian == null)
                    hessian = HessMatrixSparse.ZerosSparse(size*3, size*3);

                List<Tuple<double, Universe.Atom, Vector, Universe.Atom, Vector>> dbg_list_strong_Kij = null;
                double dbg_thld_strong_Kij_pos = +1.0E+05;
                double dbg_thld_strong_Kij_neg = -1.0E+05;
                if(HDebug.False)
                {
                    dbg_list_strong_Kij = new List<Tuple<double, Universe.Atom, Vector, Universe.Atom, Vector>>();
                }

                foreach(Universe.Nonbonded nonbonded in nonbondeds)
                {
                    Universe.Atom atom0 = nonbonded.atoms[0];
                    Universe.Atom atom1 = nonbonded.atoms[1];
                    int id0 = atom0.ID; if(coords[id0] == null) continue;
                    int id1 = atom1.ID; if(coords[id1] == null) continue;

                    Vector coord0 = coords[id0];
                    Vector coord1 = coords[id1];
                    double Kij;
                    switch(opt)
                    {
                        case "gromacs":
                            Kij = GetSprNonbondCustomGromacs(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec);
                            break;
                        case "UnifSgn": /// 6a. K_other = const, Kphi = const, Kvdw = 1
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);

                                if(Kij > 0) Kij = 1;
                                else        Kij = 0;
                            }
                            break;
//                      case "SSTeMcs6b": /// 6b. K_other = const, Kphi = const, Kvdw = 25/r_ij^2
//                          {
//                              Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, null, null, nbndinfo_set_unif);
//
//                              double rij2 = (coord0 - coord1).Dist2;
//                              if(Kij > 0) Kij = 25/rij2;
//                              else        Kij = 0;
//                          }
//                          break;
                        case "UnifSr2": /// sign(vdW)*(r0/rij)^2
                            {
                                HPack<NbndInfo> nbinfo0 = new HPack<NbndInfo>();
                                HPack<NbndInfo> nbinfo1 = new HPack<NbndInfo>();
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, nbinfo0, nbinfo1, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
                                double rij  = (coord0 - coord1).Dist;
                                double r0ij = (nbinfo0.value.rmin2 + nbinfo1.value.rmin2);
                                if(Kij > 0) Kij = Math.Pow(r0ij / rij, 2);
                                else        Kij = 0;
                            }
                            break;
                        case "UnifS25": /// sign(vdW)*25/(rij^2)
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
                                double rij  = (coord0 - coord1).Dist;
                                if(Kij > 0) Kij = 25 / (rij*rij);
                                else        Kij = 0;
                            }
                            break;
                        case "Unif": /// 6c. K_other = const, Kphi = const, Kvdw = Kvdw but using a different set of vdw radii that are:
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
                            }
                            break;
                        case "L79":
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_L79);
                            }
                            break;
                        case null:
                            if(GetCustomKij != null)
                                goto case "GetCustomKij";
                            else
                            {
                                double ri0 = atom0.Rmin2;
                                double rj0 = atom1.Rmin2;
                                double qi = atom0.Charge;
                                double qj = atom1.Charge;
                                double ei = atom0.epsilon;
                                double ej = atom1.epsilon;

                                Kij = GetKijFijNbnd(vdW, elec
                                                   , coord0, ri0, qi, ei
                                                   , coord1, rj0, qj, ej
                                                   , D
                                                   ).Kij;
                                if(maxAbsSpring != null)
                                {
                                    if(Math.Abs(Kij) > maxAbsSpring)
                                    {
                                        double sign = Math.Sign(Kij);
                                        Kij = sign * maxAbsSpring.Value;
                                    }
                                }
                            }
                            break;
                        case "TIP3P: (vdW+elec) for OH,OO,HH":
                        case "TIP3P: (vdW+elec) for OH":
                            {
                                bool lelec = elec;
                                double lD = D;
                                bool water0 = atom0.IsWater();
                                bool water1 = atom1.IsWater();
                                if(water0 && water1)
                                {
                                    string inttype = (atom0.AtomElem == atom1.AtomElem) ? "OO,HH" : "OH";
                                    if(opt.Contains(inttype))
                                    {
                                        lelec = true;
                                        lD    = 1; // dielectric constant for Tinker is "1"
                                    }
                                }

                                double ri0 = atom0.Rmin2;
                                double rj0 = atom1.Rmin2;
                                double qi = atom0.Charge;
                                double qj = atom1.Charge;
                                double ei = atom0.epsilon;
                                double ej = atom1.epsilon;

                                var kijfij = GetKijFijNbnd(vdW, lelec
                                                          , coord0, ri0, qi, ei
                                                          , coord1, rj0, qj, ej
                                                          , lD
                                                          );

                                Kij = kijfij.Kij;
                                if(water0 && water1)
                                {
                                    string inttype = (atom0.AtomElem == atom1.AtomElem) ? "OO,HH" : "OH";
                                    if(opt.Contains(inttype))
                                    {
                                        double rij = (coord0 - coord1).Dist;
                                        Kij = kijfij.Kij + kijfij.Fij/rij;
                                    }
                                }

                                if(maxAbsSpring != null)
                                {
                                    if(Math.Abs(Kij) > maxAbsSpring)
                                    {
                                        double sign = Math.Sign(Kij);
                                        Kij = sign * maxAbsSpring.Value;
                                    }
                                }
                            }
                            break;
                        case "KijRij":
                            {
                                double ri0 = atom0.Rmin2;
                                double rj0 = atom1.Rmin2;
                                double qi = atom0.Charge;
                                double qj = atom1.Charge;
                                double ei = atom0.epsilon;
                                double ej = atom1.epsilon;

                                double Kij_vdW = GetKijFijNbnd(vdW, false
                                                      , coord0, ri0, qi, ei
                                                      , coord1, rj0, qj, ej
                                                      , D
                                                      ).Kij;
                                double Kij_elec = GetKijFijNbnd(false, elec
                                                      , coord0, ri0, qi, ei
                                                      , coord1, rj0, qj, ej
                                                      , D
                                                      ).Kij;

                                HDebug.Assert(hessian[id0*3+0, id1*3+0] == 0);
                                HDebug.Assert(hessian[id0*3+1, id1*3+1] == 0);
                                HDebug.Assert(hessian[id0*3+2, id1*3+2] == 0);
                                             hessian[id0*3+0, id1*3+0] = Kij_vdW;
                                             hessian[id0*3+1, id1*3+1] = Kij_elec;
                                             hessian[id0*3+2, id1*3+2] = (coord0 - coord1).Dist;
                                HDebug.Assert(hessian[id1*3+0, id0*3+0] == 0);
                                HDebug.Assert(hessian[id1*3+1, id0*3+1] == 0);
                                HDebug.Assert(hessian[id1*3+2, id0*3+2] == 0);
                                             hessian[id1*3+0, id0*3+0] = Kij_vdW;
                                             hessian[id1*3+1, id0*3+1] = Kij_elec;
                                             hessian[id1*3+2, id0*3+2] = (coord0 - coord1).Dist;

                                //double Rij = (coordi-coordj).Dist;
                                //Debug.Assert(Kij > -10);
                                //if(pathFileRijKij != null)
                                //    if(Rij < 20)
                                //    {
                                //        string str = HTLib.Mathematica.ToString(new double[] { Rij, Kij });
                                //        File.AppendAllLines(pathFileRijKij, str+",");
                                //    }
                                Kij = double.NaN;
                            }
                            break;
                        case "GetCustomKij":
                            {
                                /// convert negative spring to positive spring,
                                /// so electrostatic can still affect the system's dynamics
                                // copy from case null:
                                {
                                    CustomKijInfo info = new CustomKijInfo();
                                    info.coordi = coord0;
                                    info.coordj = coord1;
                                    info.maxAbsSpring = maxAbsSpring;
                                    info.ri0 = atom0.Rmin2;
                                    info.rj0 = atom1.Rmin2;
                                    info.qi = atom0.Charge;
                                    info.qj = atom1.Charge;
                                    info.ei = atom0.epsilon;
                                    info.ej = atom1.epsilon;

                                    var Kij_Fij = GetKijFijNbnd(vdW, elec       // updated from "case null:"
                                                       , info.coordi, info.ri0, info.qi, info.ei    // updated from "case null:"
                                                       , info.coordj, info.rj0, info.qj, info.ej    // updated from "case null:"
                                                       , D                      // updated from "case null:"
                                                       );                       // updated from "case null:"
                                    info.Kij = Kij_Fij.Kij;
                                    info.Fij = Kij_Fij.Fij;

                                    Kij = GetCustomKij(info);
                                }
                            }
                            break;
                        default:
                            HDebug.Assert(false);
                            Kij = double.NaN;
                            break;
                    }

                    HDebug.AssertDouble(Kij);
                    if(dbg_list_strong_Kij != null)
                    {
                        if((Kij < dbg_thld_strong_Kij_neg) || (dbg_thld_strong_Kij_pos < Kij))
                        {
                            dbg_list_strong_Kij.Add(new Tuple<double, Universe.Atom, Vector, Universe.Atom, Vector>(Kij, atom0, coord0, atom1, coord1));
                        }
                    }
                    if(ignNegSpr && Kij < 0)
                        Kij = 0;

                    switch(opt)
                    {
                        case "KijRij":
                            // do nothing
                            break;
                        default:
                            if(Kij != 0)
                                UpdateHess01(coords, id0, id1, Kij, hessian);
                            break;
                    }

                    if(collector != null)
                        collector(atom0, coord0, atom1, coord1, Kij);
                }

                if(dbg_list_strong_Kij != null)
                {
                    int[] idxsrt = dbg_list_strong_Kij.HListItem1().HIdxSorted().HReverse();
                    dbg_list_strong_Kij = dbg_list_strong_Kij.HSelectByIndex(idxsrt).ToList();
                }

                return hessian;
            }
            //public static void GetHessNonbond(IList<Vector> coords, IEnumerable<Universe.Nonbonded> nonbondeds, double D, string opt, MatrixSparse<MatrixByArr> hessian
            //                                , bool vdW=true, bool elec=true, bool ignNegSpr=false
            //                                , double? maxAbsSpring=null, string pathFileRijKij=null
            //                                , Matrix pwspr = null, Matrix pwfrc = null
            //                                )
            //{
            //    int size = coords.Count;
            //    HDebug.AssertAllEquals(size, hessian.ColSize, hessian.RowSize);
            //
            //    if(pwspr != null) HDebug.Assert(pwspr.ColSize == size, pwspr.RowSize == size);
            //    if(pwfrc != null) HDebug.Assert(pwfrc.ColSize == size, pwfrc.RowSize == size);
            //
            //    int count=0;
            //    foreach(Universe.Nonbonded nonbonded in nonbondeds)
            //    {
            //        count++;
            //        Universe.Atom atom0 = nonbonded.atoms[0];
            //        Universe.Atom atom1 = nonbonded.atoms[1];
            //        int id0 = atom0.ID; if(coords[id0] == null) continue;
            //        int id1 = atom1.ID; if(coords[id1] == null) continue;
            //
            //        Vector coord0 = coords[id0];
            //        Vector coord1 = coords[id1];
            //        double Kij;
            //        double Fij=double.NaN;
            //        switch(opt)
            //        {
            //            case "Unif": /// 6c. K_other = const, Kphi = const, Kvdw = Kvdw but using a different set of vdw radii that are:
            //                {
            //                    Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
            //                }
            //                break;
            //            case null:
            //                {
            //                    double ri0 = atom0.Rmin2;
            //                    double rj0 = atom1.Rmin2;
            //                    double qi = atom0.Charge;
            //                    double qj = atom1.Charge;
            //                    double ei = atom0.epsilon;
            //                    double ej = atom1.epsilon;
            //
            //                    var KijFij = GetKijFijNbnd(vdW, elec
            //                                       , coord0, ri0, qi, ei
            //                                       , coord1, rj0, qj, ej
            //                                       , D
            //                                       );
            //                    Kij = KijFij.Kij;
            //                    Fij = KijFij.Fij;
            //                }
            //                break;
            //            default:
            //                HDebug.Assert(false);
            //                Kij = double.NaN;
            //                break;
            //        }
            //        if(ignNegSpr && Kij < 0)
            //            Kij = 0;
            //
            //        UpdateHess01(coords, id0, id1, Kij, hessian);
            //
            //        if(pwspr != null)
            //        {
            //            pwspr[id0, id1] += Kij;
            //            pwspr[id1, id0] += Kij;
            //        }
            //        if(pwfrc != null)
            //        {
            //            pwfrc[id0, id1] += Fij;
            //            pwfrc[id1, id0] += Fij;
            //        }
            //    }
            //}
            public static HessMatrix GetHessNonbond
                ( IList<Vector> coords
                , IEnumerable<Universe.Nonbonded14> nonbonded14s
                , double D
                , string opt
                , HessMatrix hessian=null
                , bool vdW=true
                , bool elec=true
                , bool ignNegSpr=false
                , double? maxAbsSpring=null
                , string pathFileRijKij=null
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, double> collector=null
                , Func<CustomKijInfo, double> GetCustomKij = null
                )
            {
                int size = coords.Count;
                if(hessian == null)
                    hessian = HessMatrixSparse.ZerosSparse(size*3, size*3);

                foreach(Universe.Nonbonded14 nonbonded14 in nonbonded14s)
                {
                    Universe.Atom atom0 = nonbonded14.atoms[0];
                    Universe.Atom atom1 = nonbonded14.atoms[1];
                    int id0 = atom0.ID; if(coords[id0] == null) continue;
                    int id1 = atom1.ID; if(coords[id1] == null) continue;

                    Vector coord0 = coords[id0];
                    Vector coord1 = coords[id1];
                    double Kij;
                    switch(opt)
                    {
                        case "gromacs":
                            Kij = GetSprNonbondCustomGromacs(atom0, atom1, coord0, coord1, NbndType.nbnd14, D, vdW, elec);
                            break;
                        case "UnifSgn": /// 6a. K_other = const, Kphi = const, Kvdw = 1
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd14, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);

                                if(Kij > 0) Kij = 1;
                                else        Kij = 0;
                            }
                            break;
//                      case "SSTeMcs6b": /// 6b. K_other = const, Kphi = const, Kvdw = 25/r_ij^2
//                          {
//                              Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd14, D, vdW, elec, null, null, nbndinfo_set_unif);
//
//                              double rij2 = (coord0 - coord1).Dist2;
//                              if(Kij > 0) Kij = 25/rij2;
//                              else        Kij = 0;
//                          }
//                          break;
                        case "UnifSr2": /// sign(vdW)*(r0/rij)^2
                            {
                                HPack<NbndInfo> nbinfo0 = new HPack<NbndInfo>();
                                HPack<NbndInfo> nbinfo1 = new HPack<NbndInfo>();
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd14, D, vdW, elec, nbinfo0, nbinfo1, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
                                double rij  = (coord0 - coord1).Dist;
                                double r0ij = (nbinfo0.value.rmin2 + nbinfo1.value.rmin2);
                                if(Kij > 0) Kij = Math.Pow(r0ij / rij, 2);
                                else        Kij = 0;
                            }
                            break;
                        case "UnifS25": /// sign(vdW)*25/(rij^2)
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd14, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
                                double rij  = (coord0 - coord1).Dist;
                                if(Kij > 0) Kij = 25 / (rij*rij);
                                else        Kij = 0;
                            }
                            break;
                        case "Unif": /// 6c. K_other = const, Kphi = const, Kvdw = Kvdw but using a different set of vdw radii that are:
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd14, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
                            }
                            break;
                        case "L79":
                            {
                                Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_L79);
                            }
                            break;
                        case "TIP3P: vdW + elec": // TIP3P does not have 1-4 interactions
                        case null:
                            {
                                double ri0 = atom0.Rmin2_14; if(double.IsNaN(ri0)) ri0 = atom0.Rmin2;
                                double rj0 = atom1.Rmin2_14; if(double.IsNaN(rj0)) rj0 = atom1.Rmin2;
                                double qi = atom0.Charge;
                                double qj = atom1.Charge;
                                double ei = atom0.eps_14; if(double.IsNaN(ei)) ei = atom0.epsilon;
                                double ej = atom1.eps_14; if(double.IsNaN(ej)) ej = atom1.epsilon;

                                Kij = GetKijFijNbnd(vdW, elec
                                                   , coord0, ri0, qi, ei
                                                   , coord1, rj0, qj, ej
                                                   , D
                                                   ).Kij;
                                if(maxAbsSpring != null)
                                {
                                    if(Math.Abs(Kij) > maxAbsSpring)
                                    {
                                        double sign = Math.Sign(Kij);
                                        Kij = sign * maxAbsSpring.Value;
                                    }
                                }
                            }
                            break;
                        case "KijRij":
                            {
                                double ri0 = atom0.Rmin2_14; if(double.IsNaN(ri0)) ri0 = atom0.Rmin2;
                                double rj0 = atom1.Rmin2_14; if(double.IsNaN(rj0)) rj0 = atom1.Rmin2;
                                double qi = atom0.Charge;
                                double qj = atom1.Charge;
                                double ei = atom0.eps_14; if(double.IsNaN(ei)) ei = atom0.epsilon;
                                double ej = atom1.eps_14; if(double.IsNaN(ej)) ej = atom1.epsilon;

                                double Kij_vdW = GetKijFijNbnd(vdW, false
                                                   , coord0, ri0, qi, ei
                                                   , coord1, rj0, qj, ej
                                                   , D
                                                   ).Kij;
                                double Kij_elec = GetKijFijNbnd(false, elec
                                                   , coord0, ri0, qi, ei
                                                   , coord1, rj0, qj, ej
                                                   , D
                                                   ).Kij;

                                HDebug.Assert(hessian[id0*3+0, id1*3+0] == 0);
                                HDebug.Assert(hessian[id0*3+1, id1*3+1] == 0);
                                HDebug.Assert(hessian[id0*3+2, id1*3+2] == 0);
                                             hessian[id0*3+0, id1*3+0] = Kij_vdW;
                                             hessian[id0*3+1, id1*3+1] = Kij_elec;
                                             hessian[id0*3+2, id1*3+2] = (coord0 - coord1).Dist;
                                HDebug.Assert(hessian[id1*3+0, id0*3+0] == 0);
                                HDebug.Assert(hessian[id1*3+1, id0*3+1] == 0);
                                HDebug.Assert(hessian[id1*3+2, id0*3+2] == 0);
                                             hessian[id1*3+0, id0*3+0] = Kij_vdW;
                                             hessian[id1*3+1, id0*3+1] = Kij_elec;
                                             hessian[id1*3+2, id0*3+2] = (coord0 - coord1).Dist;

                                //double Rij = (coordi-coordj).Dist;
                                //Debug.Assert(Kij > -10);
                                //if(pathFileRijKij != null)
                                //    if(Rij < 20)
                                //    {
                                //        string str = HTLib.Mathematica.ToString(new double[] { Rij, Kij });
                                //        File.AppendAllLines(pathFileRijKij, str+",");
                                //    }
                                Kij = double.NaN;
                            }
                            break;
                        case "Convert Nonbond Positive Spring":
                            {
                                /// convert negative spring to positive spring, 
                                /// so electrostatic can still affect the system's dynamics
                                // copy from case null:
                                {
                                    CustomKijInfo info = new CustomKijInfo();
                                    info.coordi = coord0;
                                    info.coordj = coord1;
                                    info.maxAbsSpring = maxAbsSpring;
                                    info.ri0 = atom0.Rmin2_14; if(double.IsNaN(info.ri0)) info.ri0 = atom0.Rmin2;
                                    info.rj0 = atom1.Rmin2_14; if(double.IsNaN(info.rj0)) info.rj0 = atom1.Rmin2;
                                    info.qi = atom0.Charge;
                                    info.qj = atom1.Charge;
                                    info.ei = atom0.eps_14; if(double.IsNaN(info.ei)) info.ei = atom0.epsilon;
                                    info.ej = atom1.eps_14; if(double.IsNaN(info.ej)) info.ej = atom1.epsilon;

                                    var Kij_Fij = GetKijFijNbnd(vdW, elec       // updated from "case null:"
                                                       , info.coordi, info.ri0, info.qi, info.ei    // updated from "case null:"
                                                       , info.coordj, info.rj0, info.qj, info.ej    // updated from "case null:"
                                                       , D                      // updated from "case null:"
                                                       );                       // updated from "case null:"
                                    info.Kij = Kij_Fij.Kij;
                                    info.Fij = Kij_Fij.Fij;

                                    Kij = GetCustomKij(info);
                                }
                            }
                            break;
                        default:
                            HDebug.Assert(false);
                            Kij = double.NaN;
                            break;
                    }

                    HDebug.AssertDouble(Kij);
                    if(ignNegSpr && Kij < 0)
                        Kij = 0;

                    switch(opt)
                    {
                        case "KijRij":
                            // do nothing
                            break;
                        default:
                            if(Kij != 0)
                                UpdateHess01(coords, id0, id1, Kij, hessian);
                            break;
                    }

                    if(collector != null)
                        collector(atom0, coord0, atom1, coord1, Kij);
                }

                return hessian;
            }
            //public static void GetHessNonbond(IList<Vector> coords, IEnumerable<Universe.Nonbonded14> nonbonded14s, double D, string opt, MatrixSparse<MatrixByArr> hessian
            //                                , bool vdW=true, bool elec=true, bool ignNegSpr=false
            //                                , double? maxAbsSpring=null, string pathFileRijKij=null
            //                                , Matrix pwspr = null, Matrix pwfrc = null
            //                                )
            //{
            //    int size = coords.Count;
            //    HDebug.AssertAllEquals(size, hessian.ColSize, hessian.RowSize);
            //
            //    if(pwspr != null) HDebug.Assert(pwspr.ColSize == size, pwspr.RowSize == size);
            //    if(pwfrc != null) HDebug.Assert(pwfrc.ColSize == size, pwfrc.RowSize == size);
            //
            //    foreach(Universe.Nonbonded14 nonbonded14 in nonbonded14s)
            //    {
            //        Universe.Atom atom0 = nonbonded14.atoms[0];
            //        Universe.Atom atom1 = nonbonded14.atoms[1];
            //        int id0 = atom0.ID; if(coords[id0] == null) continue;
            //        int id1 = atom1.ID; if(coords[id1] == null) continue;
            //
            //        Vector coord0 = coords[id0];
            //        Vector coord1 = coords[id1];
            //        double Kij;
            //        double Fij = double.NaN;
            //        switch(opt)
            //        {
            //            case "Unif": /// 6c. K_other = const, Kphi = const, Kvdw = Kvdw but using a different set of vdw radii that are:
            //                {
            //                    Kij = GetSprNonbondCustom(atom0, atom1, coord0, coord1, NbndType.nbnd14, D, vdW, elec, null, null, maxAbsSpring, nbndinfo_set_SSTeMcs6c);
            //                }
            //                break;
            //            case null:
            //                {
            //                    double ri0 = atom0.Rmin2_14; if(double.IsNaN(ri0)) ri0 = atom0.Rmin2;
            //                    double rj0 = atom1.Rmin2_14; if(double.IsNaN(rj0)) rj0 = atom1.Rmin2;
            //                    double qi = atom0.Charge;
            //                    double qj = atom1.Charge;
            //                    double ei = atom0.eps_14; if(double.IsNaN(ei)) ei = atom0.epsilon;
            //                    double ej = atom1.eps_14; if(double.IsNaN(ej)) ej = atom1.epsilon;
            //
            //                    var KijFij = GetKijFijNbnd(vdW, elec
            //                                       , coord0, ri0, qi, ei
            //                                       , coord1, rj0, qj, ej
            //                                       , D
            //                                       );
            //                    Kij = KijFij.Kij;
            //                    Fij = KijFij.Fij;
            //                }
            //                break;
            //            default:
            //                HDebug.Assert(false);
            //                Kij = double.NaN;
            //                break;
            //        }
            //        if(ignNegSpr && Kij < 0)
            //            Kij = 0;
            //
            //        UpdateHess01(coords, id0, id1, Kij, hessian);
            //
            //        if(pwspr != null)
            //        {
            //            pwspr[id0, id1] += Kij;
            //            pwspr[id1, id0] += Kij;
            //        }
            //        if(pwfrc != null)
            //        {
            //            pwfrc[id0, id1] += Fij;
            //            pwfrc[id1, id0] += Fij;
            //        }
            //    }
            //}
            //public static double GetSprNonbond(bool vdW, bool elec
            //                                   ,Vector coordi, double ri0, double qi, double ei
            //                                   ,Vector coordj, double rj0, double qj, double ej
            //                                   ,double D // dielectric constant: 80 for general, 1 for Tinker
            //                                   ,HPack<double> Fij = null
            //                                   )
            //{
            //    var KijFij = Hess.HessSpr.GetKijFijNbnd(vdW, elec
            //                                                  , coordi, ri0, qi, ei
            //                                                  , coordj, rj0, qj, ej
            //                                                  , D
            //                                                  );
            //
            //    if(Fij != null)
            //        Fij.value = KijFij.Fij;
            //    return KijFij.Kij;
            //}
        }
    }
}
