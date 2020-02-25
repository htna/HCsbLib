using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Coarse
    {
        public static partial class CoarseHessForc
        {
            public static void SelfTest()
            {
                CSelfTest.SelfTest();
            }
            class CSelfTest
            {
                public static void SelfTest()
                {
                    if(HDebug.Selftest() == false)
                        return;

                    string temppath = @"K:\temp\";
                    string tinkerpath_testgrad = "\""+@"C:\Program Files\Tinker\bin-win64-8.2.1\testgrad.exe"+"\"";
                    string tinkerpath_testhess = "\""+@"C:\Program Files\Tinker\bin-win64-8.2.1\testhess.exe"+"\"";

                    var xyz = Tinker.Xyz.FromLines(SelftestData.lines_1L2Y_xyz);
                    var prm = Tinker.Prm.FromLines(SelftestData.lines_charmm22_prm);
                    var univ = Universe.Build(xyz, prm);

                    var testhess = Tinker.Run.Testhess(tinkerpath_testhess, xyz, prm, temppath
                        , HessMatrixZeros: HessMatrixLayeredArray.ZerosHessMatrixLayeredArray
                        );
                    var testgrad = Tinker.Run.Testgrad(tinkerpath_testgrad, xyz, prm, temppath);
                    var hessinfo = Hess.HessInfo.FromTinker(xyz, prm, testhess.hess);

                    var hessforcinfo = Coarse.CoarseHessForc.HessForcInfo.From(hessinfo);
                        hessforcinfo.forc = testgrad.anlyts.GetForces(xyz.atoms);
                    var coarseinfo_debug = Coarse.CoarseHessForc.GetCoarseHessForc
                    ( hessforcinfo
                    , coords            : hessinfo.coords
                    , GetIdxKeepListRemv: GetIdxKeepListRemv
                    , ila               : null
                    , thres_zeroblk     : double.Epsilon
                    , options           : new string[] { "Debug" }
                    );

                    var coarseinfo_simple = Coarse.CoarseHessForc.GetCoarseHessForc
                    ( hessforcinfo
                    , coords            : hessinfo.coords
                    , GetIdxKeepListRemv: GetIdxKeepListRemv
                    , ila               : null
                    , thres_zeroblk     : double.Epsilon
                    , options           : new string[] { "SubSimple" }
                    );
                    double absmax_simple = (coarseinfo_debug.hess - coarseinfo_simple.hess).HAbsMax();
                    HDebug.Assert(Math.Abs(absmax_simple) < 0.00000001);
                    double absmax_simple_forc = (coarseinfo_debug.forc.ToVector() - coarseinfo_simple.forc.ToVector()).ToArray().MaxAbs();
                    HDebug.Assert(Math.Abs(absmax_simple_forc) < 0.00000001);

                    var coarseinfo_1iter = Coarse.CoarseHessForc.GetCoarseHessForc
                    ( hessforcinfo
                    , coords            : hessinfo.coords
                    , GetIdxKeepListRemv: GetIdxKeepListRemv
                    , ila               : null
                    , thres_zeroblk     : double.Epsilon
                    , options           : new string[] { "OneIter" }
                    );
                    double absmax_1iter = (coarseinfo_debug.hess - coarseinfo_1iter.hess).HAbsMax();
                    HDebug.Assert(Math.Abs(absmax_1iter) < 0.00000001);
                    double absmax_1iter_forc = (coarseinfo_debug.forc.ToVector() - coarseinfo_1iter.forc.ToVector()).ToArray().MaxAbs();
                    HDebug.Assert(Math.Abs(absmax_1iter_forc) < 0.00000001);

                    var coarseinfo_iter = Coarse.CoarseHessForc.GetCoarseHessForc
                    ( hessforcinfo
                    , coords            : hessinfo.coords
                    , GetIdxKeepListRemv: GetIdxKeepListRemv
                    , ila               : null
                    , thres_zeroblk     : double.Epsilon
                    , options           : null
                    );
                    double absmax_iter = (coarseinfo_debug.hess - coarseinfo_iter.hess).HAbsMax();
                    HDebug.Assert(Math.Abs(absmax_iter) < 0.00000001);
                    double absmax_iter_forc = (coarseinfo_debug.forc.ToVector() - coarseinfo_iter.forc.ToVector()).ToArray().MaxAbs();
                    HDebug.Assert(Math.Abs(absmax_iter_forc) < 0.00000001);

                    double tolerance = 1.0E-6; // 0.00001;
                    var coarseinfo_1iter_tolerant = Coarse.CoarseHessForc.GetCoarseHessForc
                    ( hessforcinfo
                    , coords            : hessinfo.coords
                    , GetIdxKeepListRemv: GetIdxKeepListRemv
                    , ila               : null
                    , thres_zeroblk     : tolerance
                    , options           : new string[] { "OneIter" }
                    );
                    double absmax_1iter_tolerant = (coarseinfo_debug.hess - coarseinfo_1iter_tolerant.hess).HAbsMax();
                    HDebug.Assert(Math.Abs(absmax_1iter_tolerant) < tolerance*10);
                    double absmax_1iter_tolerant_forc = (coarseinfo_debug.forc.ToVector() - coarseinfo_1iter_tolerant.forc.ToVector()).ToArray().MaxAbs();
                    HDebug.Assert(Math.Abs(absmax_1iter_tolerant_forc) < tolerance * 10);

                    var coarseinfo_iter_tolerant = Coarse.CoarseHessForc.GetCoarseHessForc
                    ( hessforcinfo
                    , coords            : hessinfo.coords
                    , GetIdxKeepListRemv: GetIdxKeepListRemv
                    , ila               : null
                    , thres_zeroblk     : tolerance
                    , options           : null
                    );
                    double absmax_iter_tolerant = (coarseinfo_debug.hess - coarseinfo_iter_tolerant.hess).HAbsMax();
                    HDebug.Assert(Math.Abs(absmax_iter_tolerant) < tolerance*10);
                    double absmax_iter_tolerant_forc = (coarseinfo_debug.forc.ToVector() - coarseinfo_iter_tolerant.forc.ToVector()).ToArray().MaxAbs();
                    HDebug.Assert(Math.Abs(absmax_iter_tolerant_forc) < tolerance * 10);

                    string tempfilepath = HFile.GetTempPath(temppath, "test_serialzation_CoarseHessForc.dat");
                    HSerialize.Serialize(tempfilepath, null, coarseinfo_iter_tolerant);
                    var coarseinfo_iter_tolerant2 = HSerialize.Deserialize<HessForcInfo>(tempfilepath, null);
                    double absmax_iter_tolerant_file = (coarseinfo_iter_tolerant.hess - coarseinfo_iter_tolerant.hess).HAbsMax();
                    HDebug.Assert(Math.Abs(absmax_iter_tolerant_file) == 0);
                    double absmax_iter_tolerant_file_forc = (coarseinfo_iter_tolerant.forc.ToVector() - coarseinfo_iter_tolerant.forc.ToVector()).ToArray().MaxAbs();
                    HDebug.Assert(Math.Abs(absmax_iter_tolerant_file_forc) == 0);
                    HFile.Delete(tempfilepath);
                }
                public static Tuple<int[], int[][]> GetIdxKeepListRemv(object[] atoms, Vector[] coords)
                {
                    var xyzatoms = atoms as Tinker.Xyz.Atom[];

                    int[] BiotypeN = new int[]
                    {
                        63, //"atom         63   24    NH1   \"Peptide Nitrogen\"             7    14.007    3",
                        //64, //"atom         64   25    NH2   \"Amide Nitrogen\"               7    14.007    3",
                        65, //"atom         65   26    NH3   \"Ammonium Nitrogen\"            7    14.007    4",
                        66, //"atom         66   27    N     \"PRO Nitrogen\"                 7    14.007    3",
                        67, //"atom         67   28    NP    \"N-Terminal PRO N\"             7    14.007    4",
                    };

                    List<List<int>> resi_atoms = new List<List<int>>();
                    for(int i=0; i<xyzatoms.Length; i++)
                    {
                        var atom = xyzatoms[i];
                        if(BiotypeN.Contains(atom.AtomId))
                            resi_atoms.Add(new List<int>());
                        resi_atoms.Last().Add(i);
                    }

                    List<int>   keep = new List<int>();
                    List<int[]> remv = new List<int[]>();
                    for(int i=0; i< resi_atoms.Count; i++)
                    {
                        if(i < 10)
                            keep.AddRange(resi_atoms[i]);
                        else
                            remv.Add(resi_atoms[i].ToArray());
                    }

                    return new Tuple<int[], int[][]>(keep.ToArray(), remv.ToArray());
                }
            }
        }
    }
}
