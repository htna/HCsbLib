using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
	{
        public static HessInfo GetHessEAnm(Universe univ, IList<Vector> coords, Matrix anmKij)
        {
            IEnumerable<Tuple<int, int, double>> enumKij = anmKij.HEnumNonZeros();
            return GetHessEAnm
                ( univ, coords, enumKij
                , b_bonds    : true
                , b_angles   : true
                , b_impropers: true
                , b_dihedrals: true
                );
        }
        public static HessInfo GetHessEAnm
            ( Universe univ
            , IList<Vector> coords
            , IEnumerable<Tuple<int, int, double>> enumKij
            , bool b_bonds
            , bool b_angles
            , bool b_impropers
            , bool b_dihedrals
            )
        {
          //bool vdW  = true;       // use vdW
          //bool elec = false;      // ignore electrostatic
          //double D = double.PositiveInfinity; // dielectric constant for Tinker is "1"
          //bool ignNegSpr = true;  // ignore negative spring (do not add the spring into hessian matrix)

            HessMatrix hess = null;
            hess = Hess.GetHessAnm(coords, enumKij);
            if(b_bonds    ) hess = STeM.GetHessBond(coords, univ.bonds, 340.00, hessian: hess);
            if(b_angles   ) hess = STeM.GetHessAngle         (coords, univ.angles   , true,  45.00, 10.00, hessian: hess);
            if(b_impropers) hess = STeM.GetHessImproper      (coords, univ.impropers      ,  70.00,        hessian: hess,                  useArnaud96:true);
            if(b_dihedrals) hess = STeM.GetHessDihedral      (coords, univ.dihedrals      ,   1.00,  1   , hessian: hess, useAbsSpr: true, useArnaud96:true);

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
        public static HessInfo GetHessEAnm(Universe univ, IList<Vector> coords)
        {
            double anmCutoff = 4.5;
            double anmSprCst = 1;
            HessInfo hessinfo = GetHessEAnm(univ, coords, anmCutoff, anmSprCst);
            return hessinfo;
        }
        public static HessInfo GetHessEAnm(Universe univ, IList<Vector> coords, double anmCutoff)
        {
            double anmSprCst = 1;
            HessInfo hessinfo = GetHessEAnm(univ, coords, anmCutoff, anmSprCst);
            return hessinfo;
        }
        public static HessInfo GetHessEAnm(Universe univ, IList<Vector> coords, double anmCutoff, double anmSprCst)
        {
            return GetHessEAnm
                ( univ, coords
                , anmCutoff, anmSprCst
                , b_bonds    : true
                , b_angles   : true
                , b_impropers: true
                , b_dihedrals: true
                );
        }
        public static HessInfo GetHessEAnm
            ( Universe univ
            , IList<Vector> coords
            , double anmCutoff, double anmSprCst
            , bool b_bonds
            , bool b_angles
            , bool b_impropers
            , bool b_dihedrals
            )
        {
            IEnumerable<Tuple<int, int, double>> enumKij = EnumHessAnmSpr(coords, anmCutoff, anmSprCst);
            HessInfo hessinfo = GetHessEAnm
                ( univ, coords, enumKij
                , b_bonds    : b_bonds
                , b_angles   : b_angles
                , b_impropers: b_impropers
                , b_dihedrals: b_dihedrals
                );
            return hessinfo;
        }
        //public static SparseHessInfo GetSparseHessEAnm(Universe univ, IList<Vector> coords, double anmCutoff)
        //{
        //    bool dbgCheckWithFullHess = false;
        //    if(HDebug.Selftest())
        //    {
        //        dbgCheckWithFullHess = true;
        //    }
        //    return GetSparseHessEAnm(univ, coords, anmCutoff
        //                            , dbgCheckWithFullHess: dbgCheckWithFullHess
        //                            );
        //}
        //public static SparseHessInfo GetSparseHessEAnm(Universe univ, IList<Vector> coords, double anmCutoff, bool dbgCheckWithFullHess)
        //{
        //    bool vdW  = true;       // use vdW
        //    bool elec = false;      // ignore electrostatic
        //    double D = double.PositiveInfinity; // dielectric constant for Tinker is "1"
        //    bool ignNegSpr = true;  // ignore negative spring (do not add the spring into hessian matrix)
        //
        //    Func<MatrixByArr> GetDefaultHessElement = delegate() { return new double[3, 3]; };
        //    MatrixSparse<MatrixByArr> hess = new MatrixSparse<MatrixByArr>(univ.size, univ.size, GetDefault: GetDefaultHessElement);
        //    Hess.GetHessAnm       (coords, 4.5,                                 hess);
        //    STeM.GetHessBond      (coords, univ.bonds          , 340.00,        hess);
        //    STeM.GetHessAngle     (coords, univ.angles   , true,  45.00, 10.00, hess);
        //    STeM.GetHessImproper  (coords, univ.impropers      ,  70.00,        hess,                  useArnaud96:true);
        //    STeM.GetHessDihedral  (coords, univ.dihedrals      ,   1.00,  1   , hess, useAbsSpr: true, useArnaud96:true);
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
        //        var thessinfo = GetHessEAnm(univ, coords, anmCutoff);
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
