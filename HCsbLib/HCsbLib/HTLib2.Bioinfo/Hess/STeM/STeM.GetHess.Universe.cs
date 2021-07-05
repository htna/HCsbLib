using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Hess
{
    public partial class STeM
    {
        public static HessMatrix GetHess(Universe univ)
        {
            Vector[] coords = univ.GetCoords();

            HessMatrix hessian_spr = HessMatrix.Zeros(univ.size * 3, univ.size * 3);
            Universe.Nonbondeds_v1 nonbondeds = new Universe.Nonbondeds_v1(univ.atoms, univ.size, 12);
            nonbondeds.UpdateNonbondeds(coords, 0);
            hessian_spr = STeM.GetHessBond    (coords, univ.bonds                       , null, hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with bond"));
            hessian_spr = STeM.GetHessAngle   (coords, univ.angles, true          , null, null, hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with angle"));
            hessian_spr = STeM.GetHessImproper(coords, univ.impropers                   , null, hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with improper"));
            hessian_spr = STeM.GetHessDihedral(coords, univ.dihedrals             , null, null, hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with dihedral"));
            hessian_spr = STeM.GetHessVdw     (coords, univ.nonbonded14s.GetEnumerable(), null, null, hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with vdw14"));
            hessian_spr = STeM.GetHessElec    (coords, univ.nonbonded14s.GetEnumerable()      , hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with elec14"));
            hessian_spr = STeM.GetHessVdw     (coords, nonbondeds.GetEnumerable()       , null, null, hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with vdw"));
            hessian_spr = STeM.GetHessElec    (coords, nonbondeds.GetEnumerable()             , hessian: hessian_spr); HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr with elec"));
          //hessian_spr = GetHessSprElec(coords, nonbondeds                                   , hessian: hessian_spr); Debug.Verify(Hess.CheckHessDiag(hessian_spr));
          //hessian_spr = GetHessSprVdw(coords, nonbondeds                                    , hessian: hessian_spr); Debug.Verify(Hess.CheckHessDiag(hessian_spr));
            hessian_spr = Hess.CorrectHessDiag(hessian_spr);                                                           HDebug.Verify(Hess.CheckHessDiag(hessian_spr, 0.000001, "non-computable exception while generating STeM hess_spr"));

            return hessian_spr;
        }

        public static HessMatrix GetHessBond(IList<Vector> coords, IEnumerable<Universe.Bond> bonds, double? K_r, HessMatrix hessian)
        {
            Matrix pwspr=null;
            Action<Universe.Atom, Vector, Universe.Atom, Vector, double> collector = null;
            return GetHessBond(coords, bonds, K_r, hessian, pwspr, collector);
        }
        public static HessMatrix GetHessBond
            (IList<Vector> coords, IEnumerable<Universe.Bond> bonds, double? K_r, HessMatrix hessian
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, double> collector
            )
        {
            Matrix pwspr=null;
            return GetHessBond(coords, bonds, K_r, hessian, pwspr, collector);
        }
        public static HessMatrix GetHessBond
            ( IList<Vector> coords, IEnumerable<Universe.Bond> bonds, double? K_r, HessMatrix hessian, Matrix pwspr
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, double> collector
            )
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.Bond bond in bonds)
            {
                int idx0 = bond.atoms[0].ID; if(coords[idx0] == null) continue;
                int idx1 = bond.atoms[1].ID; if(coords[idx1] == null) continue;
                double lK_r = (K_r != null)? K_r.Value : bond.Kb;
                HDebug.Assert(lK_r >= 0);
                FirstTerm(coords, lK_r, hessian, idx0, idx1);

                if(pwspr != null)
                {
                    pwspr[idx0, idx1] += lK_r;
                    pwspr[idx1, idx0] += lK_r;
                }

                if(collector != null)
                    collector
                    ( bond.atoms[0], coords[idx0]
                    , bond.atoms[1], coords[idx1]
                    , lK_r);
            }

            return hessian;
        }
        //public static void GetHessBond(IList<Vector> coords, IEnumerable<Universe.Bond> bonds, double? K_r, MatrixSparse<MatrixByArr> hessian)
        //{
        //    Matrix pwspr=null;
        //    GetHessBond(coords, bonds, K_r, hessian, pwspr);
        //}
        //public static void GetHessBond(IList<Vector> coords, IEnumerable<Universe.Bond> bonds, double? K_r, MatrixSparse<MatrixByArr> hessian, Matrix pwspr)
        //{
        //    int size = coords.Count;
        //    HDebug.AssertAllEquals(size, hessian.ColSize, hessian.RowSize);
        //
        //    foreach(Universe.Bond bond in bonds)
        //    {
        //        int idx0 = bond.atoms[0].ID; if(coords[idx0] == null) continue;
        //        int idx1 = bond.atoms[1].ID; if(coords[idx1] == null) continue;
        //        double lK_r = (K_r != null)? K_r.Value : bond.Kb;
        //        HDebug.Assert(lK_r >= 0);
        //        FirstTerm(coords, lK_r, hessian, idx0, idx1);
        //
        //        if(pwspr != null)
        //        {
        //            pwspr[idx0, idx1] += lK_r;
        //            pwspr[idx1, idx0] += lK_r;
        //        }
        //    }
        //}
        public static HessMatrix GetHessAngle(IList<Vector> coords, IEnumerable<Universe.Angle> angles, bool useUreybrad, double? K_theta, double? K_ub, HessMatrix hessian)
        {
            Matrix pwspr = null;
            Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collector = null;
            return GetHessAngle(coords, angles, useUreybrad, K_theta, K_ub, hessian, pwspr, collector);
        }
        public static HessMatrix GetHessAngle
            (IList<Vector> coords, IEnumerable<Universe.Angle> angles, bool useUreybrad, double? K_theta, double? K_ub, HessMatrix hessian
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collector
            )
        {
            Matrix pwspr = null;
            return GetHessAngle(coords, angles, useUreybrad, K_theta, K_ub, hessian, pwspr, collector);
        }
        public static HessMatrix GetHessAngle
            (IList<Vector> coords, IEnumerable<Universe.Angle> angles, bool useUreybrad, double? K_theta, double? K_ub, HessMatrix hessian
            , Matrix pwspr
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collector
            )
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.Angle angle in angles)
            {
                int idx0 = angle.atoms[0].ID; if(coords[idx0] == null) continue;
                int idx1 = angle.atoms[1].ID; if(coords[idx1] == null) continue;
                int idx2 = angle.atoms[2].ID; if(coords[idx2] == null) continue;
                double lK_theta = (K_theta != null)? K_theta.Value : angle.Ktheta;
                HDebug.Assert(lK_theta >= 0);
                SecondTerm(coords, lK_theta, hessian, idx0, idx1, idx2);

                double lK_ub = 0;
                if(useUreybrad)
                {
                    HDebug.Assert(angle.Kub >= 0);
                    lK_ub = (K_ub != null)? K_ub.Value : angle.Kub;
                    FirstTerm(coords, lK_ub, hessian, idx0, idx2);

                    if(pwspr != null)
                    {
                        pwspr[idx0, idx1] += lK_ub;
                        pwspr[idx1, idx0] += lK_ub;
                    }
                }

                if(collector != null)
                    collector
                    ( angle.atoms[0], coords[idx0]
                    , angle.atoms[1], coords[idx1]
                    , angle.atoms[2], coords[idx2]
                    , lK_theta, lK_ub);
            }

            return hessian;
        }
        //public static void GetHessAngle(IList<Vector> coords, IEnumerable<Universe.Angle> angles, bool useUreybrad, double? K_theta, double? K_ub, MatrixSparse<MatrixByArr> hessian)
        //{
        //    Matrix pwspr = null;
        //    GetHessAngle(coords, angles, useUreybrad, K_theta, K_ub, hessian, pwspr);
        //}
        //public static void GetHessAngle(IList<Vector> coords, IEnumerable<Universe.Angle> angles, bool useUreybrad, double? K_theta, double? K_ub, MatrixSparse<MatrixByArr> hessian
        //                               , Matrix pwspr)
        //{
        //    int size = coords.Count;
        //    HDebug.AssertAllEquals(size, hessian.ColSize, hessian.RowSize);
        //
        //    foreach(Universe.Angle angle in angles)
        //    {
        //        int idx0 = angle.atoms[0].ID; if(coords[idx0] == null) continue;
        //        int idx1 = angle.atoms[1].ID; if(coords[idx1] == null) continue;
        //        int idx2 = angle.atoms[2].ID; if(coords[idx2] == null) continue;
        //        double lK_theta = (K_theta != null)? K_theta.Value : angle.Ktheta;
        //        HDebug.Assert(lK_theta >= 0);
        //        SecondTerm(coords, lK_theta, hessian, idx0, idx1, idx2);
        //        if(useUreybrad)
        //        {
        //            HDebug.Assert(angle.Kub >= 0);
        //            double lK_ub = (K_ub != null)? K_ub.Value : angle.Kub;
        //            FirstTerm(coords, lK_ub, hessian, idx0, idx2);
        //
        //            if(pwspr != null)
        //            {
        //                pwspr[idx0, idx1] += lK_ub;
        //                pwspr[idx1, idx0] += lK_ub;
        //            }
        //        }
        //    }
        //}

        //////////////////////////////////////////////////////////////////////////////
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
        public static HessMatrix GetHessImproper(IList<Vector> coords, IEnumerable<Universe.Improper> impropers, double? K_psi, HessMatrix hessian
                                            , bool useArnaud96=false)
        {
            Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collector = null;
            return GetHessImproper(coords, impropers, K_psi, hessian, collector: collector, useArnaud96: useArnaud96);
        }
        public static HessMatrix GetHessImproper
            ( IList<Vector> coords, IEnumerable<Universe.Improper> impropers, double? K_psi, HessMatrix hessian
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collector
            , bool useArnaud96=false
            )
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            double sqrt2 = Math.Sqrt(2);

            foreach(Universe.Improper improper in impropers)
            {
                int idx0 = improper.atoms[0].ID; if(coords[idx0] == null) continue;
                int idx1 = improper.atoms[1].ID; if(coords[idx1] == null) continue;
                int idx2 = improper.atoms[2].ID; if(coords[idx2] == null) continue;
                int idx3 = improper.atoms[3].ID; if(coords[idx3] == null) continue;
                double lK_psi = (K_psi != null)? K_psi.Value : improper.Kpsi;
                double     n = sqrt2; // make (K_chi * n^2 / 2 == K_chi) when calling ThirdTermN(IList<Vector> caArray, double K_phi, double n)

                lK_psi = lK_psi*2;
                n = 1;
                HDebug.Assert(lK_psi >= 0);
                ThirdTermN(coords, lK_psi, n, hessian, idx0, idx1, idx2, idx3, useArnaud96:useArnaud96);

                if(collector != null)
                    collector
                    ( improper.atoms[0], coords[idx0]
                    , improper.atoms[1], coords[idx1]
                    , improper.atoms[2], coords[idx2]
                    , improper.atoms[3], coords[idx3]
                    , lK_psi, n);
            }

            return hessian;
        }
        //public static void GetHessImproper(IList<Vector> coords, IEnumerable<Universe.Improper> impropers, double? K_psi, MatrixSparse<MatrixByArr> hessian
        //                                  , bool useArnaud96=true
        //                                  )
        //{
        //    int size = coords.Count;
        //    HDebug.AssertAllEquals(size, hessian.ColSize, hessian.RowSize);
        //
        //    double sqrt2 = Math.Sqrt(2);
        //
        //    foreach(Universe.Improper improper in impropers)
        //    {
        //        int idx0 = improper.atoms[0].ID; if(coords[idx0] == null) continue;
        //        int idx1 = improper.atoms[1].ID; if(coords[idx1] == null) continue;
        //        int idx2 = improper.atoms[2].ID; if(coords[idx2] == null) continue;
        //        int idx3 = improper.atoms[3].ID; if(coords[idx3] == null) continue;
        //        double lK_psi = (K_psi != null)? K_psi.Value : improper.Kpsi;
        //        double     n = sqrt2; // make (K_chi * n^2 / 2 == K_chi) when calling ThirdTermN(IList<Vector> caArray, double K_phi, double n)
        //
        //        lK_psi = lK_psi*2;
        //        n = 1;
        //        HDebug.Assert(lK_psi >= 0);
        //        ThirdTermN(coords, lK_psi, n, hessian, idx0, idx1, idx2, idx3, useArnaud96: useArnaud96);
        //    }
        //}
        public static HessMatrix GetHessDihedral(IList<Vector> coords, IEnumerable<Universe.Dihedral> dihedrals, double? K_chi, double? n, HessMatrix hessian, bool useAbsSpr=false
                                            , bool useArnaud96=false)
        {
            Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collector = null;
            return GetHessDihedral(coords, dihedrals, K_chi, n, hessian, collector, useAbsSpr: useAbsSpr, useArnaud96: useArnaud96);
        }
        public static HessMatrix GetHessDihedral
            (IList<Vector> coords, IEnumerable<Universe.Dihedral> dihedrals, double? K_chi, double? n, HessMatrix hessian
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collector
            , bool useAbsSpr=false
            , bool useArnaud96=false
            )
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.Dihedral dihedral in dihedrals)
            {
                int idx0 = dihedral.atoms[0].ID; if(coords[idx0] == null) continue;
                int idx1 = dihedral.atoms[1].ID; if(coords[idx1] == null) continue;
                int idx2 = dihedral.atoms[2].ID; if(coords[idx2] == null) continue;
                int idx3 = dihedral.atoms[3].ID; if(coords[idx3] == null) continue;
                double lK_chi = (K_chi != null)? K_chi.Value : dihedral.Kchi;
                double ln     = (n     != null)? n.Value     : dihedral.n;
                if(useAbsSpr)
                {
                    /// https://www.charmm.org/ubbthreads/ubbthreads.php?ubb=showflat&Number=24521
                    /// ==========================================================================
                    /// Dear all, Recently I found a parameter file for some carbohydrates, like
                    /// NAG and FUC, which described the dihedral angle as following:
                    /// 
                    /// OC CC CTS CTS -1.2007 1 0.0 
                    /// OC CC CTS CTS -0.3145 2 0.0
                    /// OC CC CTS CTS -0.0618 3 0.0
                    /// CA OSL SL O2L 0.0000 3 0.0 
                    /// CA CA OSL SL 0.0000 3 0.0
                    /// 
                    /// In some entries the values of the Kchi for the dihedral angle are 0 or
                    /// negatives. But this value usually is a positive. I understand that the
                    /// kchi value reflect the flexibility of the dihedral, so what do the
                    /// negatives here mean? Thanks a million.
                    /// --------------------------------------------------------------------------
                    /// May I ask where you found these parameters?
                    /// 
                    /// Although the negative values violate the convention, they are not
                    /// incorrect. Switching the sign of the amplitude is mathematically
                    /// equivalent to reversing the phase. (See the class I potential energy
                    /// function, as discussed in any paper or textbook on the subject.) In other
                    /// words, -1.2 1 0 is equivalent to 1.2 1 180. CHARMM handles this correctly.
                    /// ==========================================================================
                    /// htna:
                    /// Still "taking absolute value" is not a good choice, but because there is
                    /// no option that I can taek, maybe this is OK in this situation, in order
                    /// to avoid negative springs.
                    lK_chi = Math.Abs(lK_chi);
                }
                HDebug.Assert(lK_chi >= 0);
                ThirdTermN(coords, lK_chi, ln, hessian, idx0, idx1, idx2, idx3, useArnaud96: useArnaud96);

                if(collector != null)
                    collector
                    ( dihedral.atoms[0], coords[idx0]
                    , dihedral.atoms[1], coords[idx1]
                    , dihedral.atoms[2], coords[idx2]
                    , dihedral.atoms[3], coords[idx3]
                    , lK_chi, ln);
            }

            return hessian;
        }
        //public static void GetHessDihedral(IList<Vector> coords, IEnumerable<Universe.Dihedral> dihedrals, double? K_chi, double? n, MatrixSparse<MatrixByArr> hessian
        //                                  , bool useAbsSpr=true
        //                                  , bool useArnaud96=true
        //                                  )
        //{
        //    int size = coords.Count;
        //    HDebug.AssertAllEquals(size, hessian.ColSize, hessian.RowSize);
        //
        //    foreach(Universe.Dihedral dihedral in dihedrals)
        //    {
        //        int idx0 = dihedral.atoms[0].ID; if(coords[idx0] == null) continue;
        //        int idx1 = dihedral.atoms[1].ID; if(coords[idx1] == null) continue;
        //        int idx2 = dihedral.atoms[2].ID; if(coords[idx2] == null) continue;
        //        int idx3 = dihedral.atoms[3].ID; if(coords[idx3] == null) continue;
        //        double lK_chi = (K_chi != null)? K_chi.Value : dihedral.Kchi;
        //        double ln     = (n     != null)? n.Value     : dihedral.n;
        //        if(useAbsSpr)
        //            lK_chi = Math.Abs(lK_chi);
        //        HDebug.Assert(lK_chi >= 0);
        //        ThirdTermN(coords, lK_chi, ln, hessian, idx0, idx1, idx2, idx3, useArnaud96:useArnaud96);
        //    }
        //}
        public static HessMatrix GetHessVdw(IList<Vector> coords, IEnumerable<Universe.Nonbonded> nonbondeds, string opt, double? Epsilon, HessMatrix hessian)
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.Nonbonded nonbonded in nonbondeds)
            {
                Universe.Atom atom0 = nonbonded.atoms[0];
                Universe.Atom atom1 = nonbonded.atoms[1];
                int idx0 = atom0.ID; if(coords[idx0] == null) continue;
                int idx1 = atom1.ID; if(coords[idx1] == null) continue;
                double lEpsilon;
                if(Epsilon != null)
                    lEpsilon = Epsilon.Value;
                else
                {
                    switch(opt)
                    {
                        case "gromacs":
                            {
                                double eps0 = atom0.ConvertGromacsToCharmm.eps;
                                double eps1 = atom1.ConvertGromacsToCharmm.eps;
                                lEpsilon = Math.Sqrt(eps0 * eps1);
                            }
                            break;
                        default:
                            {
                                double eps0 = atom0.epsilon;
                                double eps1 = atom1.epsilon;
                                lEpsilon = Math.Sqrt(eps0 * eps1);
                            }
                            break;
                    }
                }
                FourthTerm(coords, lEpsilon, hessian, idx0, idx1);
                FourthTerm(coords, lEpsilon, hessian, idx1, idx0);
            }

            return hessian;
        }
        public static HessMatrix GetHessVdw(IList<Vector> coords, IEnumerable<Universe.AtomPack> pairs, double? Epsilon, string opt, HessMatrix hessian)
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.AtomPack pair in pairs)
            {
                Universe.Atom atom0 = pair.atoms.First();
                Universe.Atom atom1 = pair.atoms.Last();
                int idx0 = atom0.ID; if(coords[idx0] == null) continue;
                int idx1 = atom1.ID; if(coords[idx1] == null) continue;
                double lEpsilon;
                if(Epsilon != null)
                    lEpsilon = Epsilon.Value;
                else
                {
                    switch(opt)
                    {
                        case "gromacs":
                            {
                                double eps0 = atom0.ConvertGromacsToCharmm.eps;
                                double eps1 = atom1.ConvertGromacsToCharmm.eps;
                                lEpsilon = Math.Sqrt(eps0 * eps1);
                            }
                            break;
                        default:
                            {
                                double eps0 = atom0.epsilon;
                                double eps1 = atom1.epsilon;
                                lEpsilon = Math.Sqrt(eps0 * eps1);
                            }
                            break;
                    }
                }
                FourthTerm(coords, lEpsilon, hessian, idx0, idx1);
                FourthTerm(coords, lEpsilon, hessian, idx1, idx0);
            }

            return hessian;
        }
        public static HessMatrix GetHessVdw(IList<Vector> coords, IEnumerable<Universe.Nonbonded14> nonbonded14s, double? Epsilon, string opt, HessMatrix hessian)
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.Nonbonded14 nonbonded14 in nonbonded14s)
            {
                Universe.Atom atom0 = nonbonded14.atoms[0];
                Universe.Atom atom1 = nonbonded14.atoms[1];
                int idx0 = atom0.ID; if(coords[idx0] == null) continue;
                int idx1 = atom1.ID; if(coords[idx1] == null) continue;
                double lEpsilon;
                if(Epsilon != null)
                    lEpsilon = Epsilon.Value;
                else
                {
                    switch(opt)
                    {
                        case "gromacs":
                            {
                                var nbndpar0 = atom0.ConvertGromacsToCharmm;
                                var nbndpar1 = atom1.ConvertGromacsToCharmm;
                                double eps0_14 = nbndpar0.eps_14; eps0_14 = (double.IsNaN(eps0_14)==false) ? eps0_14 : nbndpar0.eps;
                                double eps1_14 = nbndpar1.eps_14; eps1_14 = (double.IsNaN(eps1_14)==false) ? eps1_14 : nbndpar1.eps;
                                lEpsilon = Math.Sqrt(eps0_14 * eps1_14);
                            }
                            break;
                        default:
                            {
                                double eps0_14 = atom0.eps_14; eps0_14 = (double.IsNaN(eps0_14)==false) ? eps0_14 : atom0.epsilon;
                                double eps1_14 = atom1.eps_14; eps1_14 = (double.IsNaN(eps1_14)==false) ? eps1_14 : atom1.epsilon;
                                lEpsilon = Math.Sqrt(eps0_14 * eps1_14);
                            }
                            break;
                    }
                }
                FourthTerm(coords, lEpsilon, hessian, idx0, idx1);
                FourthTerm(coords, lEpsilon, hessian, idx1, idx0);
            }

            return hessian;
        }
        public static HessMatrix GetHessElec(IList<Vector> coords, IEnumerable<Universe.Nonbonded> nonbondeds, HessMatrix hessian)
        {
            double ee=80;
            return GetHessElec(coords, nonbondeds, hessian, ee);
        }
        public static HessMatrix GetHessElec(IList<Vector> coords, IEnumerable<Universe.Nonbonded> nonbondeds, HessMatrix hessian, double ee)
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.Nonbonded nonbonded in nonbondeds)
            {
                Universe.Atom atom0 = nonbonded.atoms[0];
                Universe.Atom atom1 = nonbonded.atoms[1];
                int idx0 = atom0.ID; if(coords[idx0] == null) continue;
                int idx1 = atom1.ID; if(coords[idx1] == null) continue;
                double pch0 = atom0.Charge;
                double pch1 = atom1.Charge;
                TermElec(coords, pch0, pch1, ee, hessian, idx0, idx1);
                TermElec(coords, pch0, pch1, ee, hessian, idx1, idx0);
            }

            return hessian;
        }
        public static HessMatrix GetHessElec(IList<Vector> coords, IEnumerable<Universe.Nonbonded14> nonbonded14s, HessMatrix hessian)
        {
            double ee=80;
            return GetHessElec(coords, nonbonded14s, hessian, ee);
        }
        public static HessMatrix GetHessElec(IList<Vector> coords, IEnumerable<Universe.Nonbonded14> nonbonded14s, HessMatrix hessian, double ee)
        {
            int size = coords.Count;
            if(hessian == null)
                hessian = HessMatrix.Zeros(size*3, size*3);

            foreach(Universe.Nonbonded14 nonbonded14 in nonbonded14s)
            {
                Universe.Atom atom0 = nonbonded14.atoms[0];
                Universe.Atom atom1 = nonbonded14.atoms[1];
                int idx0 = atom0.ID; if(coords[idx0] == null) continue;
                int idx1 = atom1.ID; if(coords[idx1] == null) continue;
                double pch0 = atom0.Charge;
                double pch1 = atom1.Charge;
                TermElec(coords, pch0, pch1, ee, hessian, idx0, idx1);
                TermElec(coords, pch0, pch1, ee, hessian, idx1, idx0);
            }

            return hessian;
        }
    }
}
}
