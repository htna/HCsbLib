using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    using Quality = Hess.HessCoarseResiIter.SelfTest_Program20150331_SparsityVsAccuracy.Quality;
    public partial class HessStatic
    {
        public static double[]   HListCorr       (this IList<Quality> qualities) { List<double  > list = new List<double  >(); foreach(var quality in qualities) list.Add(quality.corr        ); return list.ToArray(); }
        public static double[]   HListWovlp      (this IList<Quality> qualities) { List<double  > list = new List<double  >(); foreach(var quality in qualities) list.Add(quality.wovlp       ); return list.ToArray(); }
        public static double[]   HListSparsityAll(this IList<Quality> qualities) { List<double  > list = new List<double  >(); foreach(var quality in qualities) list.Add(quality.sparsity_all); return list.ToArray(); }
        public static double[]   HListSparsityCa (this IList<Quality> qualities) { List<double  > list = new List<double  >(); foreach(var quality in qualities) list.Add(quality.sparsity_ca ); return list.ToArray(); }
        public static double[][] HListOvlps      (this IList<Quality> qualities) { List<double[]> list = new List<double[]>(); foreach(var quality in qualities) list.Add(quality.ovlps       ); return list.ToArray(); }
    }
    public partial class Hess
    {
        public static partial class HessCoarseResiIter
        {
            public static partial class SelfTest_Program20150331_SparsityVsAccuracy
            {
                static string tempbase = @"C:\temp\";

                static string[] pdbids = new string[]
                {
                    "1I2T", "1Y0M", "2J5Y", "3MP9", "3HFO", "3ZR8", "1HG7", "1TG0", "2HIN", "2VE8", //  10
                    "3RNJ", "1WYX", "2YIZ", "3I4O", "3OMT", "4B6X", "3NAR", "3ZZL", "2IC6", "1WM3", //  20
                    "3N27", "3E56", "3HTU", "3IGE", "3L1X", "2PMR", "3B7H", "3DS4", "2VKL", "3G21", //  30
                    "4F26", "3RQ9", "4ABM", "2QCP", "4AGH", "2GBN", "2O37", "3LLB", "1R6J", "3O5Z", //  40
                    "3R3M", "3R45", "4ERR", "2F5K", "1W53", "3FX7", "3R27", "3VBG", "4ES3", "3MAB", //  50
                    "3R69", "4E6S", "2RK5", "3JQU", "3L7H", "3LNW", "3TOE", "2Y4X", "4IOG", "3BRL", //  60
                    "3CNK", "3JTN", "3RZW", "3TXS", "4A75", "4HE6", "4HTJ", "1U07", "2BT9", "2X3D", //  70
                    "3KOV", "3M8J", "3MSH", "3PE9", "4AEQ", "4GS3", "4EWI", "4GMQ", "2X5T", "3ID4", //  80
                    "1G2R", "2I5C", "3ID1", "3KIK", "3SHU", "4EZA", "1C5E", "2Y2T", "3SQF", "4FQN", //  90
                    "3PYJ", "3QGL", "1JO0", "1URR", "3L1F", "3QMQ", "3VI6", "2Y9R", "3KNG", "3PO8", // 100
                    "2QJL", "3NS6", "2XRH", "3K6F", "3NXA", "3RHB", "1MG4", "3SSQ", "3U1C", "3M9J", // 110
                    "3FG7", "3CX2", "1BKR", "3LRG", "4BBD", "2CKK", "2ZWM", "3TDM", "2YH5", "3LKY", // 120
                    "4A6S", "1R29", "3NJM", "3P38", "3P6J", "3RNV", "1MM9", "3UJ3", "1DBF", "3HA4", // 130
                    "3VA9", "4FYH", "4GCN", "4HBX", "2XX6", "4DRO", "4F55", "2AAJ", "3LRD", "3O48", // 140
                    "3T8N", "3UD8", "4EDL", "2XW6", "2LIS", "2R8U", "3QF3", "3D4W", "3LLO", "3OBL", // 150
                    "3Q47", "3R87", "4HX8", "3KXY", "3RSW", "2R6Q", "2XDH", "2XG3", "3R85", "2PWO", // 160
                    "3GZ2", "1NWW", "2JDC", "2XEM", "3S02", "3SEI", "4F8A", "1J8Q", "2FL4", "3HGM", // 170
                    "3RKV", "4E8O", "3NBC", "1GU1", "2Y9F", "3AXC", "3MBT",                         // 177
                };


                static ILinAlg la = null;
                static Dictionary<string, object> _global = new Dictionary<string, object>();
                static Dictionary<string, object> locks = new Dictionary<string, object>();

                public static void Main(string[] args, ILinAlg la)
                {
                    SelfTest_Program20150331_SparsityVsAccuracy.la = la;

                    string[] hesstypes = new string[] { "NMA", "scrnNMA", "sbNMA", "ssNMA", "eANM", "AA-ANM" };
                    double[] threszeroblks = new double[] { 0.0001, 0.001, 0.01, 0.1 };

                    hesstypes = new string[] { "sbNMA" };
                    threszeroblks = new double[] { 0.01 };

                    Main(args, hesstypes, threszeroblks);
                }
                public static void Main
                    ( string[] args
                    , string[] hesstypes
                    , double[] threszeroblks
                    )
                {
                    string cachebase = @"K:\cache\CoarseGraining-20150111\proteins-177\";

                    Dictionary<string, List<Quality>> data = new Dictionary<string, List<Quality>>();

                    foreach(string pdbid in pdbids)
                    {
                        string pathbase = cachebase + pdbid + @"\";
                        // load univ
                        var pdb0 = Pdb       .FromFile(pathbase + "xry-struc.pdb");
                        var xyz0 = Tinker.Xyz.FromFile(pathbase + "xry-struc.xyz", false);
                        var xyz1 = Tinker.Xyz.FromFile(pathbase + "min-struc-charmm22.xyz", false);
                        var prm  = Tinker.Prm.FromFile(cachebase+ "charmm22.prm");
                        if(HFile.Exists(pathbase + "min-struc-charmm22-screened.xyz") == false)
                        {
                            var newton = Tinker.Run.Newton
                                ( xyz1, prm, tempbase
                                , null // copytemp
                                , "A 0.0001" // param
                                , null // atomsToFix
                                , false
                                , "CUTOFF 9", "TAPER"
                                );
                            newton.minxyz.ToFile(pathbase + "min-struc-charmm22-screened.xyz", false);
                        }
                        var xyz2 = Tinker.Xyz.FromFile(pathbase + "min-struc-charmm22-screened.xyz", false);
                        Universe univ      = Universe.BuilderTinker.Build(xyz1, prm, pdb0, xyz0, 0.001);
                        Universe univ_scrn = Universe.BuilderTinker.Build(xyz2, prm, pdb0, xyz0, 0.001);

                        if(HDebug.IsDebuggerAttached)
                        {
                            var grad = Tinker.Run.Testgrad(xyz1, prm, tempbase);
                            var forc = grad.anlyts.GetForces(xyz1.atoms);
                            var mforc= forc.Dist().Max();
                            HDebug.Assert(mforc < 0.1);

                            grad = Tinker.Run.Testgrad(xyz2, prm, tempbase, new string[] { "CUTOFF 9", "TAPER" }
                                , optOutSource: null
                                );
                            forc = grad.anlyts.GetForces(xyz1.atoms);
                            mforc= forc.Dist().Max();
                            HDebug.Assert(mforc < 0.1);
                        }

                        System.Console.Write(pdbid + " : ");
                        foreach(string hesstype in hesstypes)
                        {
                            foreach(double threszeroblk in threszeroblks)
                            {
                                double GetHessCoarseResiIter_thres_zeroblk = threszeroblk;
                                Quality quality = GetQuality(pathbase, univ, univ_scrn, hesstype, GetHessCoarseResiIter_thres_zeroblk);
                                if(quality == null)
                                {
                                    System.Console.Write(hesstype+"(Unknown exception                                                            ),    ");
                                    continue;
                                }
                                System.Console.Write(hesstype+"(");
                                System.Console.Write("thod "+threszeroblk+" : ");
                                System.Console.Write( "corr {0,6:0.0000}, " , quality.corr );
                                System.Console.Write("spcty(all {0,6:0.0000}, ca {1,6:0.0000}), ", quality.sparsity_all, quality.sparsity_ca);
                                System.Console.Write("wovlp {0,6:0.0000} : ", quality.wovlp);
                                System.Console.Write("{0}), ", quality.ovlps.HToStringSeparated("{0,4:0.00}", ","));
                                System.Console.Write("eigval({0}), ", quality.eigvals.HSelectCount(7).HToStringSeparated("{0,10:0.0000000}", ","));

                                string datakey = hesstype+"-"+threszeroblk;
                                if(data.ContainsKey(datakey) == false)
                                    data.Add(datakey, new List<Quality>());
                                data[datakey].Add(quality);
                            }
                        }
                        System.Console.WriteLine();
                    }

                    System.Console.WriteLine("=================================================================================================");
                    System.Console.Write("avg  : ");
                    foreach(string hesstype in hesstypes)
                    {
                        foreach(double threszeroblk in threszeroblks)
                        {
                            string datakey = hesstype+"-"+threszeroblk;
                            List<Quality> datai = data[datakey];

                            double[]   lst_corr  = datai.HListCorr ().ToArray();
                            double[]   lst_wovlp = datai.HListWovlp().ToArray();
                            double[][] lst_ovlps = datai.HListOvlps().ToArray();
                            double[]   lst_sparsity_all = datai.HListSparsityAll().ToArray();
                            double[]   lst_sparsity_ca  = datai.HListSparsityCa ().ToArray();

                            double   corr  = lst_corr .HRemoveAll(double.NaN).Average();
                            double   wovlp = lst_wovlp.HRemoveAll(double.NaN).Average();
                            double   sparsity_all = lst_sparsity_all.HRemoveAll(double.NaN).Average();
                            double   sparsity_ca  = lst_sparsity_ca .HRemoveAll(double.NaN).Average();
                            double[] ovlps = new double[10];
                            for(int i=0; i<10; i++)
                            {
                                double[] lst_ovlpsi = new double[lst_ovlps.Length];
                                for(int j=0; j<lst_ovlps.Length; j++)
                                    lst_ovlpsi[j] = lst_ovlps[j][i];
                                ovlps[i] = lst_ovlpsi.HRemoveAll(double.NaN).Average();
                            }

                            System.Console.Write(hesstype+"(");
                            System.Console.Write("thod "+threszeroblk+" : ");
                            System.Console.Write("corr {0,6:0.0000}, ", corr);
                            System.Console.Write("spcty(all {0,6:0.0000}, ca {1,6:0.0000}), ", sparsity_all, sparsity_ca);
                            System.Console.Write("wovlp {0,6:0.0000} : ", wovlp);
                            System.Console.Write("{0}),    ", ovlps.HToStringSeparated("{0,4:0.00}", ","));
                        }
                    }
                    System.Console.WriteLine();
                    System.Console.Write("min  : ");
                    foreach(string hesstype in hesstypes)
                    {
                        foreach(double threszeroblk in threszeroblks)
                        {
                            string datakey = hesstype+"-"+threszeroblk;
                            List<Quality> datai = data[datakey];

                            double[]   lst_corr  = datai.HListCorr ().ToArray();
                            double[]   lst_wovlp = datai.HListWovlp().ToArray();
                            double[][] lst_ovlps = datai.HListOvlps().ToArray();
                            double[]   lst_sparsity_all = datai.HListSparsityAll().ToArray();
                            double[]   lst_sparsity_ca  = datai.HListSparsityCa ().ToArray();

                            double   corr  = lst_corr .HRemoveAll(double.NaN).Min();
                            double   wovlp = lst_wovlp.HRemoveAll(double.NaN).Min();
                            double   sparsity_all = lst_sparsity_all.HRemoveAll(double.NaN).Min();
                            double   sparsity_ca  = lst_sparsity_ca .HRemoveAll(double.NaN).Min();
                            double[] ovlps = new double[10];
                            for(int i=0; i<10; i++)
                            {
                                double[] lst_ovlpsi = new double[lst_ovlps.Length];
                                for(int j=0; j<lst_ovlps.Length; j++)
                                    lst_ovlpsi[j] = lst_ovlps[j][i];
                                ovlps[i] = lst_ovlpsi.HRemoveAll(double.NaN).Min();
                            }

                            System.Console.Write(hesstype+"(");
                            System.Console.Write("thod "+threszeroblk+" : ");
                            System.Console.Write("corr {0,6:0.0000}, ", corr);
                            System.Console.Write("spcty(all {0,6:0.0000}, ca {1,6:0.0000}), ", sparsity_all, sparsity_ca);
                            System.Console.Write("wovlp {0,6:0.0000} : ", wovlp);
                            System.Console.Write("{0}),    ", ovlps.HToStringSeparated("{0,4:0.00}", ","));
                        }
                    }
                    System.Console.WriteLine();
                    System.Console.Write("#miss: ");
                    foreach(string hesstype in hesstypes)
                    {
                        foreach(double threszeroblk in threszeroblks)
                        {
                            string datakey = hesstype+"-"+threszeroblk;
                            List<Quality> datai = data[datakey];

                            double[]   lst_corr  = datai.HListCorr ().ToArray();
                            double[]   lst_sparsity_all = datai.HListSparsityAll().ToArray();
                            double[]   lst_sparsity_ca  = datai.HListSparsityCa ().ToArray();
                            double[]   lst_wovlp = datai.HListWovlp().ToArray();
                            double[][] lst_ovlps = datai.HListOvlps().ToArray();

                            int   cnt_corr  = lst_corr .Length - lst_corr .HRemoveAll(double.NaN).Length;
                            int   cnt_wovlp = lst_wovlp.Length - lst_wovlp.HRemoveAll(double.NaN).Length;
                            int   cnt_sparsity_all = lst_sparsity_all.Length - lst_sparsity_all.HRemoveAll(double.NaN).Length;
                            int   cnt_sparsity_ca  = lst_sparsity_ca .Length - lst_sparsity_ca .HRemoveAll(double.NaN).Length;
                            int[] cnt_ovlps = new int[10];
                            for(int i=0; i<10; i++)
                            {
                                double[] lst_ovlpsi = new double[lst_ovlps.Length];
                                for(int j=0; j<lst_ovlps.Length; j++)
                                    lst_ovlpsi[j] = lst_ovlps[j][i];
                                cnt_ovlps[i] = lst_ovlpsi.Length - lst_ovlpsi.HRemoveAll(double.NaN).Length;
                            }

                            System.Console.Write(hesstype+"(");
                            System.Console.Write("thod "+threszeroblk+" : ");
                            System.Console.Write("corr {0,6}, "  , cnt_corr );
                            System.Console.Write("spcty(all {0,6}, ca {1,6}), ", cnt_sparsity_all, cnt_sparsity_ca);
                            System.Console.Write("wovlp {0,6} : ", cnt_wovlp);
                            System.Console.Write("{0}),    ", cnt_ovlps.HToStringSeparated("{0,4}", ","));
                        }
                    }
                    System.Console.WriteLine();
                }
                public static Quality GetQuality
                    ( string pathbase
                    , Universe univ
                    , Universe univ_scrn
                    , string hesstype
                    , double GetHessCoarseResiIter_thres_zeroblk
                    )
                {
                    Universe luniv;
                    Func<Hess.HessInfo> GetHessInfo;
                    switch(hesstype)
                    {
                        case "NMA"    : luniv=univ;      GetHessInfo=delegate() { return Hess.GetHessNMA  (luniv, luniv.GetCoords(), tempbase, 16                     ); }; break;
                        case "scrnNMA": luniv=univ_scrn; GetHessInfo=delegate() { return Hess.GetHessNMA  (luniv, luniv.GetCoords(), tempbase, 16, "CUTOFF 9", "TAPER"); }; break;
                        case "sbNMA"  : luniv=univ;      GetHessInfo=delegate() { return Hess.GetHessSbNMA(luniv, luniv.GetCoords(), double.PositiveInfinity          ); }; break;
                        case "ssNMA"  : luniv=univ;      GetHessInfo=delegate() { return Hess.GetHessSsNMA(luniv, luniv.GetCoords(), double.PositiveInfinity          ); }; break;
                        case "eANM"   : luniv=univ;      GetHessInfo=delegate() { return Hess.GetHessEAnm (luniv, luniv.GetCoords()                                   ); }; break;
                        case "AA-ANM" : luniv=univ;      GetHessInfo=delegate() { return Hess.GetHessAnm  (luniv, luniv.GetCoords(), 4.5                              ); }; break;
                        default:
                            throw new HException();
                    }

                    string pathcache = pathbase + string.Format("{0}.sparsity+{1}.txt", hesstype, GetHessCoarseResiIter_thres_zeroblk);

                    try
                    {
                        Quality quality = GetQuality(pathcache, luniv, GetHessInfo, GetHessCoarseResiIter_thres_zeroblk);
                        return quality;
                    }
                    catch(Exception)
                    {
                        return null;
                    }
                }
                public class Quality
                {
                    public double corr;
                    public double wovlp;
                    public double sparsity_all;
                    public double sparsity_ca;
                    public double[] ovlps;
                    public double[] eigvals;
                }
                public static Quality GetQuality
                    ( string pathcache
                    , Universe univ
                    , Func<Hess.HessInfo> GetHessInfo
                    , double GetHessCoarseResiIter_thres_zeroblk
                    )
                {
                    double corr  = double.NaN;
                    double wovlp = double.NaN;
                    double sparsityall = double.NaN;
                    double sparsityca  = double.NaN;
                    double[] ovlps = new double[]
                    {
                        double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                        double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                    };
                    double[] eigvals = new double[]
                    {
                        double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                        double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                    };

                    try
                    {
                        Hess.HessInfo hessinfo = GetHessInfo();
                        double lsparsityall = 1 - hessinfo.hess.RatioUsedBlocks;

                        Mode[] camodes_orig;
                        {
                            int[] idxca = (hessinfo.atoms as Universe.Atom[]).ListPdbAtomName(true).HIdxEqual("CA");
                            HessMatrix cahess  = Hess.GetHessCoarseBlkmat  (hessinfo.hess, idxca, "inv");
                            Mode[] lcamodes = Hess.GetModesFromHess(cahess, la);
                            var camodes_nzero_zero = lcamodes.SeparateTolerants();
                            if(bool.Parse("false")) { camodes_nzero_zero = lcamodes.SeparateTolerantsByCountSigned(6); } /// manually fix 3LKY, 4EDL
                            if(camodes_nzero_zero.Item2.Length != 6)
                                throw new HException("# zero-eigval != 6");
                            camodes_orig = camodes_nzero_zero.Item1;
                        }
                        GC.Collect();
                        Vector cabfactor_orig = camodes_orig.GetBFactor().ToArray();

                        double lsparsityca;
                        Mode[] camodes_iter;
                        Mode[] camodes;
                        {
                            var coords = univ.GetCoords();
                            var cahess = Hess.GetHessCoarseResiIter_BlockWise(hessinfo, coords, la, 18, 500, GetHessCoarseResiIter_thres_zeroblk);
                            lsparsityca = 1 - cahess.hess.RatioUsedBlocks;
                            camodes = Hess.GetModesFromHess(cahess.hess, la);
                            var camodes_nzero_zero = camodes.SeparateTolerantsByCountSigned(6);
                            if(camodes_nzero_zero.Item2.Length != 6)
                                throw new HException("# zero-eigval != 6");
                            camodes_iter = camodes_nzero_zero.Item1;
                        }
                        GC.Collect();
                        Vector cabfactor_iter = camodes_iter.GetBFactor().ToArray();

                        corr  = HBioinfo.BFactor.Corr(cabfactor_orig, cabfactor_iter);
                        var lwovlp = HBioinfo.OverlapWeightedByEigval(camodes_orig, camodes_iter, la, false, "corresponding index");
                        wovlp = lwovlp.woverlap;
                        sparsityall = lsparsityall;
                        sparsityca  = lsparsityca;
                        ovlps = new double[]
                        {
                            lwovlp.overlaps[0], lwovlp.overlaps[1], lwovlp.overlaps[2], lwovlp.overlaps[3], lwovlp.overlaps[4], 
                            lwovlp.overlaps[5], lwovlp.overlaps[6], lwovlp.overlaps[7], lwovlp.overlaps[8], lwovlp.overlaps[9], 
                        };
                        eigvals = new double[]
                        {
                            camodes[0].eigval, camodes[1].eigval, camodes[2].eigval, camodes[3].eigval, camodes[4].eigval,
                            camodes[5].eigval, camodes[6].eigval, camodes[7].eigval, camodes[8].eigval, camodes[9].eigval,
                        };
                    }
                    catch(Exception e)
                    {
                        if(e.Message != "# zero-eigval != 6")
                            throw;
                    }

                    return new Quality
                    {
                        corr         = corr,
                        wovlp        = wovlp,
                        sparsity_all = sparsityall,
                        sparsity_ca  = sparsityca,
                        ovlps        = ovlps,
                        eigvals      = eigvals,
                    };
                }
            }
        }
    }
}
