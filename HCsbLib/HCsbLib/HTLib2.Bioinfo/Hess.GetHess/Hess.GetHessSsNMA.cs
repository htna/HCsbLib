using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
	{
        public static HessInfo GetHessSsNMA
            (Universe univ
            , IList<Vector> coords
            , double nbondMaxDist       // =double.PositiveInfinity
            , double? maxAbsSpring      =null
            //, bool setNanForEmptyAtom   // =true
            )
        {
            IEnumerable<Universe.Nonbonded> nonbondeds;
            IEnumerable<Universe.Nonbonded14> nonbonded14s;
            {
                Universe.Nonbondeds _nonbondeds = new Universe.Nonbondeds(univ.atoms, univ.size, nbondMaxDist);
                _nonbondeds.UpdateCoords(coords, true);
                nonbondeds = _nonbondeds.EnumNonbondeds(true);
                nonbonded14s = univ.nonbonded14s.GetEnumerable();
            }

            return GetHessSsNMA( univ
                                , coords, univ.bonds, univ.angles, univ.impropers
                                , univ.dihedrals, nonbondeds, nonbonded14s
                                , maxAbsSpring
                                //, setNanForEmptyAtom
                                );
        }
        public static HessInfo GetHessSsNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist       // =double.PositiveInfinity
            , double? maxAbsSpring
            , double? K_r          = 340.00
            , double? K_theta      = 45.00
            , double? K_ub         = 10.00
            , double? K_psi        = 70.00
            , double? K_chi        = 1.00
            , double? n            = 1
            , string  k_vdW        = "Unif"
            //, bool setNanForEmptyAtom                         // =true
            )
        {
            IEnumerable<Universe.Nonbonded> nonbondeds;
            IEnumerable<Universe.Nonbonded14> nonbonded14s;
            {
                Universe.Nonbondeds _nonbondeds = new Universe.Nonbondeds(univ.atoms, univ.size, nbondMaxDist);
                _nonbondeds.UpdateCoords(coords, true);
                nonbondeds = _nonbondeds.EnumNonbondeds(true);
                nonbonded14s = univ.nonbonded14s.GetEnumerable();
            }

            return GetHessSsNMA
            ( univ
            , coords, univ.bonds, univ.angles, univ.impropers
            , univ.dihedrals, nonbondeds, nonbonded14s
            , maxAbsSpring
            , K_r         
            , K_theta     
            , K_ub        
            , K_psi       
            , K_chi       
            , n           
            , k_vdW       
            );
        }
        public static HessInfo GetHessSsNMA
            ( Universe univ
            , IList<Vector> coords
            , IEnumerable<Universe.Bond> bonds
            , IEnumerable<Universe.Angle> angles
            , IEnumerable<Universe.Improper> impropers
            , IEnumerable<Universe.Dihedral> dihedrals
            , IEnumerable<Universe.Nonbonded> nonbondeds
            , IEnumerable<Universe.Nonbonded14> nonbonded14s
            , double? maxAbsSpring = null
            , double? K_r          = 340.00
            , double? K_theta      = 45.00
            , double? K_ub         = 10.00
            , double? K_psi        = 70.00
            , double? K_chi        = 1.00
            , double? n            = 1
            , string  k_vdW        = "Unif"
            //, bool setNanForEmptyAtom                         // =true
            )
        {
            bool vdW  = true;       // use vdW
            bool elec = false;      // ignore electrostatic
            double D = double.PositiveInfinity; // dielectric constant for Tinker is "1"
            bool ignNegSpr = true;  // ignore negative spring (do not add the spring into hessian matrix)

            //maxAbsSpring = Math.Pow(10, 9);
            HessMatrix hess = null;
            hess = STeM.GetHessBond          (coords, bonds          , K_r,           hessian: hess);
            hess = STeM.GetHessAngle         (coords, angles   , true, K_theta, K_ub, hessian: hess);
            hess = STeM.GetHessImproper      (coords, impropers      , K_psi,         hessian: hess,                  useArnaud96:true);
            hess = STeM.GetHessDihedral      (coords, dihedrals      , K_chi,  n,     hessian: hess, useAbsSpr: true, useArnaud96:true);
            hess = HessSpr.GetHessNonbond(coords, nonbondeds      ,D , k_vdW,         hessian: hess, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr, maxAbsSpring: maxAbsSpring);
            hess = HessSpr.GetHessNonbond(coords, nonbonded14s    ,D , k_vdW,         hessian: hess, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr, maxAbsSpring: maxAbsSpring);

            //if(setNanForEmptyAtom)
            Hess.UpdateHessNaN(hess, coords);

            return new HessInfo
            {
                hess   = hess,
                mass   = univ.GetMasses(),
                atoms  = univ.atoms.ToArray(),
                coords = coords.HCloneVectors().ToArray(),
                numZeroEigval = 6,
            };
        }
        //public static SparseHessInfo GetSparseHessSsNMA(Universe univ, IList<Vector> coords)
        //{
        //    bool dbgCheckWithFullHess = false;
        //    if(HDebug.Selftest())
        //    {
        //        dbgCheckWithFullHess = true;
        //    }
        //    return GetSparseHessSsNMA(univ, coords
        //                            , dbgCheckWithFullHess: dbgCheckWithFullHess
        //                            );
        //}
        //public static SparseHessInfo GetSparseHessSsNMA(Universe univ, IList<Vector> coords, bool dbgCheckWithFullHess)
        //{
        //    IEnumerable<Universe.Nonbonded> nonbondeds;
        //    IEnumerable<Universe.Nonbonded14> nonbonded14s;
        //    {
        //        Universe.Nonbondeds _nonbondeds = new Universe.Nonbondeds(univ.atoms, univ.size, double.PositiveInfinity);
        //        _nonbondeds.UpdateCoords(univ.GetCoords());
        //        nonbondeds = _nonbondeds.EnumNonbondeds();
        //        nonbonded14s = univ.nonbonded14s.GetEnumerable();
        //    }
        //    bool vdW  = true;       // use vdW
        //    bool elec = false;      // ignore electrostatic
        //    double D = double.PositiveInfinity; // dielectric constant for Tinker is "1"
        //    bool ignNegSpr = true;  // ignore negative spring (do not add the spring into hessian matrix)
        //
        //    Func<MatrixByArr> GetDefaultHessElement = delegate() { return new double[3, 3]; };
        //    MatrixSparse<MatrixByArr> hess = new MatrixSparse<MatrixByArr>(univ.size, univ.size, GetDefault: GetDefaultHessElement);
        //    STeM.GetHessBond      (coords, univ.bonds          , 340.00,        hess);
        //    STeM.GetHessAngle     (coords, univ.angles   , true,  45.00, 10.00, hess);
        //    STeM.GetHessImproper  (coords, univ.impropers      ,  70.00,        hess,                  useArnaud96:true);
        //    STeM.GetHessDihedral  (coords, univ.dihedrals      ,   1.00,  1   , hess, useAbsSpr: true, useArnaud96:true);
        //    HessSpr.GetHessNonbond(coords, nonbondeds          ,D, "Unif",      hess, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr);
        //    HessSpr.GetHessNonbond(coords, nonbonded14s        ,D, "Unif",      hess, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr);
        //
        //    var hessinfo = new SparseHessInfo
        //    {
        //        hess   = hess,
        //        mass   = univ.GetMasses(),
        //        atoms  = univ.atoms.ToArray(),
        //        coords = coords.HCloneVectors().ToArray(),
        //        numZeroEigval = 6,
        //    };
        //
        //    if(dbgCheckWithFullHess)
        //    {
        //        HDebug.Assert(HDebug.IsDebuggerAttached);
        //        var thessinfo = GetHessSsNMA(univ, coords);
        //        HDebug.AssertTolerance(0, hessinfo.mass - thessinfo.mass);
        //        HDebug.Assert(hessinfo.numZeroEigval == thessinfo.numZeroEigval);
        //        HDebug.Assert(hessinfo.atoms.Length == thessinfo.atoms.Length);
        //        for(int i=0; i<hessinfo.atoms.Length; i++)
        //            HDebug.Assert(hessinfo.atoms[i] == thessinfo.atoms[i]);
        //
        //        MatrixByArr[,] arrs = hess.ToArray(new double[3,3]);
        //        MatrixByArr arr = MatrixByArr.FromMatrixArray(arrs);
        //        HDebug.AssertTolerance(0.0000000000000001, arr-thessinfo.hess.ToArray());
        //    }
        //
        //    return hessinfo;
        //}
	}
}
