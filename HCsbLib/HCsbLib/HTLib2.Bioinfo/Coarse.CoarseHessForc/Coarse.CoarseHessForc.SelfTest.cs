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

                    var testhess = Tinker.Run.Testhess(tinkerpath_testhess, xyz, prm, temppath);
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

                    var coarseinfo_noiter = Coarse.CoarseHessForc.GetCoarseHessForc
                    ( hessforcinfo
                    , coords            : hessinfo.coords
                    , GetIdxKeepListRemv: GetIdxKeepListRemv
                    , ila               : null
                    , thres_zeroblk     : double.Epsilon
                    , options           : new string[] { "NoIter" }
                    );
                    double absmax_noiter = (coarseinfo_debug.hess - coarseinfo_noiter.hess).HAbsMax();
                    HDebug.Assert(Math.Abs(absmax_noiter) < 0.00000001);

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
