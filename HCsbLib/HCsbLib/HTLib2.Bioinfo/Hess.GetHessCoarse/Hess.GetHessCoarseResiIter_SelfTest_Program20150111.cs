using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static partial class HessCoarseResiIter
        {
            public partial class SelfTest_Program20150111_SparsityVsAccuracy
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


                static ILinAlg la;
                static Dictionary<string, object> _global = new Dictionary<string, object>();
                static Dictionary<string, object> locks = new Dictionary<string, object>();

                public static void Main(string[] args, ILinAlg la)
                {
                    SelfTest_Program20150111_SparsityVsAccuracy.la = la;

                    string[] hesstypes = new string[] { "NMA", "scrnNMA", "sbNMA", "ssNMA", "eANM", "AA-ANM" };
                    double[] threszeroblks = new double[] { 0.0001, 0.001, 0.01, 0.1 };

                    Main(args, hesstypes, threszeroblks);
                }
                public static void Main
                    ( string[] args
                    , string[] hesstypes
                    , double[] threszeroblks
                    )
                {
                    string cachebase = @"K:\cache\CoarseGraining-20150111\proteins-177\";
                    string lockbase  = cachebase + @"lock\"; if(HDirectory.Exists(lockbase) == false) HDirectory.CreateDirectory(lockbase);

                    Dictionary<string, List<Tuple<double, double, double[]>>> data = new Dictionary<string, List<Tuple<double, double, double[]>>>();

                    foreach(string pdbid in pdbids)
                    {
                        if(locks.ContainsKey(pdbid) == false)
                        {
                            var filelock = HFile.LockFile(lockbase+pdbid);
                            if(filelock == null)
                            {
                                System.Console.WriteLine("{0} is locked", pdbid);
                                continue;
                            }
                            locks.Add(pdbid, filelock);
                        }

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
                                Tuple<double, double, double[]> corr_wovlp_ovlps = GetQuality(pathbase, univ, univ_scrn, hesstype, GetHessCoarseResiIter_thres_zeroblk);
                                if(corr_wovlp_ovlps == null)
                                {
                                    System.Console.Write(hesstype+"(Unknown exception                                                            ),    ");
                                    continue;
                                }
                                System.Console.Write(hesstype+"(");
                                System.Console.Write("thod "+threszeroblk+" : ");
                                System.Console.Write( "corr {0,6:0.0000}, ", corr_wovlp_ovlps.Item1);
                                System.Console.Write("wovlp {0,6:0.0000} : ", corr_wovlp_ovlps.Item2);
                                System.Console.Write("{0}),    ", corr_wovlp_ovlps.Item3.HToStringSeparated("{0,4:0.00}", ","));

                                string datakey = hesstype+"-"+threszeroblk;
                                if(data.ContainsKey(datakey) == false)
                                    data.Add(datakey, new List<Tuple<double, double, double[]>>());
                                data[datakey].Add(corr_wovlp_ovlps);
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
                            List<Tuple<double, double, double[]>> datai = data[datakey];

                            double[]   lst_corr  = datai.HListItem1().ToArray();
                            double[]   lst_wovlp = datai.HListItem2().ToArray();
                            double[][] lst_ovlps = datai.HListItem3().ToArray();

                            double   corr  = lst_corr .HRemoveAll(double.NaN).Average();
                            double   wovlp = lst_wovlp.HRemoveAll(double.NaN).Average();
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
                            List<Tuple<double, double, double[]>> datai = data[datakey];

                            double[]   lst_corr  = datai.HListItem1().ToArray();
                            double[]   lst_wovlp = datai.HListItem2().ToArray();
                            double[][] lst_ovlps = datai.HListItem3().ToArray();

                            double   corr  = lst_corr .HRemoveAll(double.NaN).Min();
                            double   wovlp = lst_wovlp.HRemoveAll(double.NaN).Min();
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
                            List<Tuple<double, double, double[]>> datai = data[datakey];

                            double[]   lst_corr  = datai.HListItem1().ToArray();
                            double[]   lst_wovlp = datai.HListItem2().ToArray();
                            double[][] lst_ovlps = datai.HListItem3().ToArray();

                            int   cnt_corr  = lst_corr .Length - lst_corr .HRemoveAll(double.NaN).Length;
                            int   cnt_wovlp = lst_wovlp.Length - lst_wovlp.HRemoveAll(double.NaN).Length;
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
                            System.Console.Write("wovlp {0,6} : ", cnt_wovlp);
                            System.Console.Write("{0}),    ", cnt_ovlps.HToStringSeparated("{0,4}", ","));
                        }
                    }
                    System.Console.WriteLine();
                }
                public static Tuple<double, double, double[]> GetQuality
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

                    string pathcache = pathbase + string.Format("{0}.{1}.txt", hesstype, GetHessCoarseResiIter_thres_zeroblk);

                    Tuple<double, double, double[]> corr_wovlp_ovlps;
                    try
                    {
                        corr_wovlp_ovlps = GetQuality(pathcache, luniv, GetHessInfo, GetHessCoarseResiIter_thres_zeroblk);
                        return corr_wovlp_ovlps;
                    }
                    catch
                    {
                        return null;
                    }
                }
                public static Tuple<double, double, double[]> GetQuality
                    ( string pathcache
                    , Universe univ
                    , Func<Hess.HessInfo> GetHessInfo
                    , double GetHessCoarseResiIter_thres_zeroblk
                    )
                {
                    if(HFile.Exists(pathcache) == false)
                    {
                        double corr  = double.NaN;
                        double wovlp = double.NaN;
                        double[] ovlps = new double[]
                        {
                            double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                            double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                        };

                        try
                        {
                            Hess.HessInfo hessinfo = GetHessInfo();

                            Mode[] camodes_orig;
                            {
                                int[] idxca = (hessinfo.atoms as Universe.Atom[]).ListPdbAtomName(true).HIdxEqual("CA");
                                Matrix cahess  = Hess.GetHessCoarseBlkmat  (hessinfo.hess, idxca, "inv");
                                Mode[] lcamodes = Hess.GetModesFromHess(cahess, la);
                                var camodes_nzero_zero = lcamodes.SeparateTolerants();
                                if(bool.Parse("false")) { camodes_nzero_zero = lcamodes.SeparateTolerantsByCountSigned(6); } /// manually fix 3LKY, 4EDL
                                if(camodes_nzero_zero.Item2.Length != 6)
                                    throw new HException("# zero-eigval != 6");
                                camodes_orig = camodes_nzero_zero.Item1;
                            }
                            GC.Collect();
                            Vector cabfactor_orig = camodes_orig.GetBFactor().ToArray();

                            Mode[] camodes_iter;
                            {
                                var cahess = Hess.GetHessCoarseResiIter_BlockWise(hessinfo, univ.GetCoords(), la, 18, 500, GetHessCoarseResiIter_thres_zeroblk);
                                Mode[] lcamodes = Hess.GetModesFromHess(cahess.hess, la);
                                var camodes_nzero_zero = lcamodes.SeparateTolerantsByCountSigned(6);
                                if(camodes_nzero_zero.Item2.Length != 6)
                                    throw new HException("# zero-eigval != 6");
                                camodes_iter = camodes_nzero_zero.Item1;
                            }
                            GC.Collect();
                            Vector cabfactor_iter = camodes_iter.GetBFactor().ToArray();

                            corr  = HBioinfo.BFactor.Corr(cabfactor_orig, cabfactor_iter);
                            var lwovlp = HBioinfo.OverlapWeightedByEigval(camodes_orig, camodes_iter, la, false, "corresponding index");
                            wovlp = lwovlp.woverlap;
                            ovlps = new double[]
                            {
                                lwovlp.overlaps[0], lwovlp.overlaps[1], lwovlp.overlaps[2], lwovlp.overlaps[3], lwovlp.overlaps[4], 
                                lwovlp.overlaps[5], lwovlp.overlaps[6], lwovlp.overlaps[7], lwovlp.overlaps[8], lwovlp.overlaps[9], 
                            };
                        }
                        catch(Exception e)
                        {
                            if(e.Message != "# zero-eigval != 6")
                                throw;
                        }

                        HSerialize.SerializeText(pathcache, new double[]
                        {   corr, wovlp,
                            ovlps[0], ovlps[1], ovlps[2], ovlps[3], ovlps[4], 
                            ovlps[5], ovlps[6], ovlps[7], ovlps[8], ovlps[9], 
                        });
                    }

                    {
                        double[] buff;
                        HSerialize.DeserializeText(pathcache, out buff);
                        double   corr  = buff[0];
                        double   wovlp = buff[1];
                        double[] ovlps = new double[] { buff[2], buff[3], buff[4], buff[ 5], buff[ 6]
                                                      , buff[7], buff[8], buff[9], buff[10], buff[11]
                                                      };
                        if(double.IsNaN(corr))
                            HDebug.Assert(false);
                        return new Tuple<double, double, double[]>(corr, wovlp, ovlps);
                    }
                }
            }
        }
    }
}
