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
        public static partial class HessSbNMA
        {
            public static bool SelfTest()
            {
                return CSelfTest.Main(null, null);
            }
            public static partial class CSelfTest
            {
                static string tempbase = @"C:\temp\";

                static ILinAlg la = null;
                static Dictionary<string, object> _global = new Dictionary<string, object>();
                static Dictionary<string, object> locks = new Dictionary<string, object>();

                public static bool Main(string[] args, ILinAlg la)
                {
                    string cachebase = @"K:\cache\CoarseGraining-20150111\proteins-177\";

                    foreach(var pdbid_corr_ovlps in lst_pdbid_corr_ovlps)
                    {
                        string pdbid = pdbid_corr_ovlps.Item1;

                        string pathbase = cachebase + pdbid + @"\";
                        // load univ
                        var pdb0 = Pdb       .FromFile(pathbase + "xry-struc.pdb");
                        var xyz0 = Tinker.Xyz.FromFile(pathbase + "xry-struc.xyz", false);
                        var xyz1 = Tinker.Xyz.FromFile(pathbase + "min-struc-charmm22.xyz", false);
                        var prm  = Tinker.Prm.FromFile(cachebase+ "charmm22.prm");
                        var xyz2 = Tinker.Xyz.FromFile(pathbase + "min-struc-charmm22-screened.xyz", false);
                        #region generating "min-struc-charmm22-screened.xyz"
                        //if(HFile.Exists(pathbase + "min-struc-charmm22-screened.xyz") == false)
                        //{
                        //    var newton = Tinker.Run.Newton
                        //        ( xyz1, prm, tempbase
                        //        , null // copytemp
                        //        , "A 0.0001" // param
                        //        , null // atomsToFix
                        //        , false
                        //        , "CUTOFF 9", "TAPER"
                        //        );
                        //    newton.minxyz.ToFile(pathbase + "min-struc-charmm22-screened.xyz", false);
                        //}
                        #endregion

                        Universe univ     = Universe.BuilderTinker.Build(xyz1, prm, pdb0, xyz0, 0.001);

                        System.Console.Write("{0}: ", pdbid);

                        HessInfo NMA_hessinfo = GetHessNMA(xyz1, prm, tempbase);
                        Mode[]   NMA_modes    = NMA_hessinfo.GetModesMassReduced();
                        Mode[]   NMA_nzmodes  = NMA_modes.HSelectFrom(6);
                        Vector   NMA_bfactor  = NMA_nzmodes.GetBFactor().ToArray();

                        HessInfo sbNMA_hessinfo = GetHessSbNMA(univ, xyz1.atoms.HListCoords(), 12, null     , true, true, true, true, true, true       , null, null, null, null, null, null     , null, null, null, null, null, null);
                        Mode[]   sbNMA_modes    = sbNMA_hessinfo.GetModesMassReduced();
                        Mode[]   sbNMA_nzmodes  = sbNMA_modes.HSelectFrom(6);
                        Vector   sbNMA_bfactor  = sbNMA_nzmodes.GetBFactor().ToArray();

                        double   corr  = HBioinfo.BFactor.Corr(NMA_bfactor, sbNMA_bfactor);
                        double   ovlp0 = Math.Abs(LinAlg.VtV(NMA_nzmodes[0].eigvec.UnitVector(), sbNMA_nzmodes[0].eigvec.UnitVector()));
                        double   ovlp1 = Math.Abs(LinAlg.VtV(NMA_nzmodes[1].eigvec.UnitVector(), sbNMA_nzmodes[1].eigvec.UnitVector()));
                        double   ovlp2 = Math.Abs(LinAlg.VtV(NMA_nzmodes[2].eigvec.UnitVector(), sbNMA_nzmodes[2].eigvec.UnitVector()));
                        double   ovlp3 = Math.Abs(LinAlg.VtV(NMA_nzmodes[3].eigvec.UnitVector(), sbNMA_nzmodes[3].eigvec.UnitVector()));
                        double   ovlp4 = Math.Abs(LinAlg.VtV(NMA_nzmodes[4].eigvec.UnitVector(), sbNMA_nzmodes[4].eigvec.UnitVector()));
                        double   ovlp5 = Math.Abs(LinAlg.VtV(NMA_nzmodes[5].eigvec.UnitVector(), sbNMA_nzmodes[5].eigvec.UnitVector()));
                        double   ovlp6 = Math.Abs(LinAlg.VtV(NMA_nzmodes[6].eigvec.UnitVector(), sbNMA_nzmodes[6].eigvec.UnitVector()));
                        double   ovlp7 = Math.Abs(LinAlg.VtV(NMA_nzmodes[7].eigvec.UnitVector(), sbNMA_nzmodes[7].eigvec.UnitVector()));
                        double   ovlp8 = Math.Abs(LinAlg.VtV(NMA_nzmodes[8].eigvec.UnitVector(), sbNMA_nzmodes[8].eigvec.UnitVector()));
                        double   ovlp9 = Math.Abs(LinAlg.VtV(NMA_nzmodes[9].eigvec.UnitVector(), sbNMA_nzmodes[9].eigvec.UnitVector()));

                        System.Console.Write("corr {0:0.0000}, " , corr );
                        System.Console.Write("ovlp0 {0:0.0000}, ", ovlp0);
                        System.Console.Write("ovlp1 {0:0.0000}, ", ovlp1);
                        System.Console.Write("ovlp2 {0:0.0000}, ", ovlp2);
                        System.Console.Write("ovlp3 {0:0.0000}, ", ovlp3);
                        System.Console.Write("ovlp4 {0:0.0000}, ", ovlp4);
                        System.Console.Write("ovlp5 {0:0.0000}, ", ovlp5);
                        System.Console.Write("ovlp6 {0:0.0000}, ", ovlp6);
                        System.Console.Write("ovlp7 {0:0.0000}, ", ovlp7);
                        System.Console.Write("ovlp8 {0:0.0000}, ", ovlp8);
                        System.Console.Write("ovlp9 {0:0.0000}, ", ovlp9);
                        System.Console.WriteLine();
                    }
                    return true;
                }

                static Tuple<string, double, double[]>[] lst_pdbid_corr_ovlps = new Tuple<string,double,double[]>[]
                {
                    new Tuple<string, double, double[]>("1I2T", 0.9057, new double[] { 0.6061, 0.6256, 0.6242, 0.7116, 0.7233, 0.4982, 0.0785, 0.2104, 0.2192, 0.4323 }),
                    new Tuple<string, double, double[]>("1Y0M", 0.9154, new double[] { 0.9598, 0.7421, 0.8077, 0.0693, 0.0964, 0.6223, 0.1065, 0.3189, 0.1329, 0.2327 }),
                    new Tuple<string, double, double[]>("2J5Y", 0.9671, new double[] { 0.9304, 0.5650, 0.5371, 0.3863, 0.4833, 0.1738, 0.1340, 0.1697, 0.2888, 0.0026 }),
                    new Tuple<string, double, double[]>("3MP9", 0.9401, new double[] { 0.6386, 0.6352, 0.8622, 0.7785, 0.8241, 0.1837, 0.2718, 0.7586, 0.6445, 0.5495 }),
                    new Tuple<string, double, double[]>("3HFO", 0.9327, new double[] { 0.9529, 0.5523, 0.0330, 0.2031, 0.0507, 0.3207, 0.2997, 0.1761, 0.1556, 0.3342 }),
                    new Tuple<string, double, double[]>("3ZR8", 0.8796, new double[] { 0.4447, 0.6229, 0.2327, 0.5427, 0.5738, 0.6203, 0.2554, 0.0599, 0.1333, 0.1364 }),
                    new Tuple<string, double, double[]>("1HG7", 0.9027, new double[] { 0.4985, 0.3366, 0.0952, 0.1908, 0.4653, 0.2317, 0.4367, 0.1372, 0.4850, 0.3253 }),
                    new Tuple<string, double, double[]>("1TG0", 0.9426, new double[] { 0.9612, 0.5834, 0.5582, 0.8890, 0.7708, 0.7830, 0.9152, 0.0824, 0.4791, 0.1802 }),
                    new Tuple<string, double, double[]>("2HIN", 0.9574, new double[] { 0.9687, 0.9714, 0.9544, 0.9501, 0.2157, 0.4580, 0.5046, 0.5780, 0.3491, 0.1495 }),
                    new Tuple<string, double, double[]>("2VE8", 0.8724, new double[] { 0.9156, 0.9299, 0.9194, 0.3774, 0.5919, 0.7397, 0.1101, 0.0035, 0.0085, 0.0540 }),
                    //  10
                    new Tuple<string, double, double[]>("3RNJ", 0.7416, new double[] { 0.7591, 0.4002, 0.1656, 0.4293, 0.2736, 0.0035, 0.5916, 0.5535, 0.2938, 0.2957 }),
                    new Tuple<string, double, double[]>("1WYX", 0.9662, new double[] { 0.8423, 0.0618, 0.5482, 0.2269, 0.7174, 0.8188, 0.4089, 0.4445, 0.4506, 0.1594 }),
                    new Tuple<string, double, double[]>("2YIZ", 0.8933, new double[] { 0.8351, 0.8281, 0.5058, 0.3782, 0.2462, 0.0996, 0.3166, 0.4992, 0.0356, 0.4543 }),
                    new Tuple<string, double, double[]>("3I4O", 0.9429, new double[] { 0.9611, 0.9192, 0.6940, 0.3282, 0.0735, 0.1874, 0.2922, 0.2601, 0.5065, 0.1927 }),
                    new Tuple<string, double, double[]>("3OMT", 0.9551, new double[] { 0.9743, 0.8019, 0.2062, 0.1996, 0.8069, 0.3407, 0.1714, 0.0397, 0.3357, 0.0336 }),
                    new Tuple<string, double, double[]>("4B6X", 0.9285, new double[] { 0.9951, 0.9869, 0.9404, 0.6835, 0.3457, 0.1805, 0.4266, 0.3316, 0.4910, 0.5454 }),
                    new Tuple<string, double, double[]>("3NAR", 0.9641, new double[] { 0.9782, 0.8426, 0.4622, 0.2906, 0.4815, 0.3871, 0.4588, 0.1421, 0.1885, 0.3063 }),
                    new Tuple<string, double, double[]>("3ZZL", 0.8800, new double[] { 0.8469, 0.6348, 0.5294, 0.1299, 0.5488, 0.1093, 0.2321, 0.0442, 0.1480, 0.2551 }),
                    new Tuple<string, double, double[]>("2IC6", 0.9321, new double[] { 0.9808, 0.9638, 0.9748, 0.9214, 0.9267, 0.4809, 0.4049, 0.1614, 0.2109, 0.2235 }),
                    new Tuple<string, double, double[]>("1WM3", 0.8802, new double[] { 0.8245, 0.8715, 0.3828, 0.3373, 0.0206, 0.1604, 0.7892, 0.2854, 0.2203, 0.3453 }),
                    //  20
                    new Tuple<string, double, double[]>("3N27", 0.9704, new double[] { 0.9886, 0.4380, 0.4601, 0.1544, 0.0380, 0.7258, 0.6161, 0.3671, 0.8719, 0.8656 }),
                    new Tuple<string, double, double[]>("3E56", 0.9300, new double[] { 0.6593, 0.8870, 0.5069, 0.6226, 0.1045, 0.1129, 0.3051, 0.4971, 0.1358, 0.3433 }),
                    new Tuple<string, double, double[]>("3HTU", 0.8432, new double[] { 0.7091, 0.1321, 0.0493, 0.0660, 0.6057, 0.6680, 0.0579, 0.1522, 0.0862, 0.0317 }),
                    new Tuple<string, double, double[]>("3IGE", 0.9215, new double[] { 0.8957, 0.5877, 0.5563, 0.7746, 0.4779, 0.5519, 0.0224, 0.0992, 0.4197, 0.1588 }),
                    new Tuple<string, double, double[]>("3L1X", 0.9769, new double[] { 0.9382, 0.9211, 0.7146, 0.3033, 0.3874, 0.7655, 0.0854, 0.1786, 0.0828, 0.3263 }),
                    new Tuple<string, double, double[]>("2PMR", 0.9113, new double[] { 0.5835, 0.6495, 0.0239, 0.7290, 0.1580, 0.2706, 0.7100, 0.2579, 0.0146, 0.0014 }),
                    new Tuple<string, double, double[]>("3B7H", 0.9111, new double[] { 0.7414, 0.4555, 0.0954, 0.2840, 0.7620, 0.2646, 0.2173, 0.2115, 0.6530, 0.2840 }),
                    new Tuple<string, double, double[]>("3DS4", 0.6922, new double[] { 0.0935, 0.0166, 0.1812, 0.1249, 0.4113, 0.1743, 0.3620, 0.0868, 0.0905, 0.0237 }),
                    new Tuple<string, double, double[]>("2VKL", 0.9294, new double[] { 0.0184, 0.2011, 0.7619, 0.5580, 0.6973, 0.2177, 0.7993, 0.8166, 0.5186, 0.4323 }),
                    new Tuple<string, double, double[]>("3G21", 0.8183, new double[] { 0.3471, 0.0189, 0.0365, 0.6295, 0.0691, 0.1696, 0.2136, 0.1593, 0.2885, 0.1098 }),
                    //  30
                    new Tuple<string, double, double[]>("4F26", 0.8525, new double[] { 0.7920, 0.8591, 0.1430, 0.4778, 0.0178, 0.2183, 0.0420, 0.0053, 0.0238, 0.1078 }),
                    new Tuple<string, double, double[]>("3RQ9", 0.9196, new double[] { 0.8840, 0.8689, 0.8466, 0.6247, 0.6244, 0.3048, 0.5731, 0.7054, 0.2890, 0.5537 }),
                    new Tuple<string, double, double[]>("4ABM", 0.9130, new double[] { 0.8105, 0.7160, 0.1741, 0.3611, 0.2843, 0.1418, 0.3970, 0.4104, 0.4112, 0.0863 }),
                    new Tuple<string, double, double[]>("2QCP", 0.9566, new double[] { 0.9698, 0.8015, 0.5205, 0.3563, 0.1109, 0.0661, 0.3830, 0.1810, 0.3476, 0.1706 }),
                    new Tuple<string, double, double[]>("4AGH", 0.9564, new double[] { 0.9809, 0.9159, 0.9357, 0.5801, 0.5861, 0.2948, 0.4699, 0.0278, 0.1088, 0.5493 }),
                    new Tuple<string, double, double[]>("2GBN", 0.9127, new double[] { 0.4211, 0.6060, 0.6330, 0.4840, 0.0523, 0.2712, 0.0678, 0.1045, 0.4506, 0.4292 }),
                    new Tuple<string, double, double[]>("2O37", 0.9048, new double[] { 0.9319, 0.7178, 0.9217, 0.6759, 0.5461, 0.6341, 0.5776, 0.1682, 0.0305, 0.3477 }),
                    new Tuple<string, double, double[]>("3LLB", 0.8898, new double[] { 0.7129, 0.6488, 0.4970, 0.6696, 0.6158, 0.2894, 0.4279, 0.2333, 0.0309, 0.3817 }),
                    new Tuple<string, double, double[]>("1R6J", 0.9254, new double[] { 0.3339, 0.0396, 0.2143, 0.2374, 0.0438, 0.1624, 0.3529, 0.3555, 0.2581, 0.3012 }),
                    new Tuple<string, double, double[]>("3O5Z", 0.7950, new double[] { 0.1736, 0.4105, 0.0151, 0.0465, 0.2971, 0.0623, 0.2579, 0.1706, 0.4732, 0.2000 }),
                    //  40
                    new Tuple<string, double, double[]>("3R3M", 0.8937, new double[] { 0.8956, 0.8928, 0.1988, 0.2957, 0.3078, 0.1327, 0.4538, 0.5623, 0.4290, 0.0376 }),
                    new Tuple<string, double, double[]>("3R45", 0.9690, new double[] { 0.2121, 0.1922, 0.9700, 0.8867, 0.8549, 0.8254, 0.8059, 0.4556, 0.1051, 0.0915 }),
                    new Tuple<string, double, double[]>("4ERR", 0.9799, new double[] { 0.9909, 0.9960, 0.9835, 0.9322, 0.6530, 0.6669, 0.1857, 0.2454, 0.2428, 0.5527 }),
                    new Tuple<string, double, double[]>("2F5K", 0.9275, new double[] { 0.2103, 0.1671, 0.2905, 0.3468, 0.0530, 0.1326, 0.5969, 0.2797, 0.1702, 0.4414 }),
                    new Tuple<string, double, double[]>("1W53", 0.9216, new double[] { 0.9664, 0.9342, 0.8768, 0.8186, 0.8794, 0.3412, 0.0003, 0.4146, 0.1116, 0.4926 }),
                    new Tuple<string, double, double[]>("3FX7", 0.9692, new double[] { 0.9789, 0.8329, 0.8916, 0.9194, 0.8810, 0.5125, 0.3518, 0.3656, 0.0395, 0.3504 }),
                    new Tuple<string, double, double[]>("3R27", 0.9270, new double[] { 0.5637, 0.2952, 0.2310, 0.1357, 0.6891, 0.4024, 0.4230, 0.3722, 0.4751, 0.2639 }),
                    new Tuple<string, double, double[]>("3VBG", 0.8424, new double[] { 0.7057, 0.4678, 0.4302, 0.2442, 0.0704, 0.1465, 0.5814, 0.1253, 0.2184, 0.0085 }),
                    new Tuple<string, double, double[]>("4ES3", 0.9344, new double[] { 0.8961, 0.6682, 0.6087, 0.8881, 0.2112, 0.0536, 0.0523, 0.0877, 0.5932, 0.4633 }),
                    new Tuple<string, double, double[]>("3MAB", 0.8310, new double[] { 0.9020, 0.4481, 0.2867, 0.3043, 0.1568, 0.0423, 0.0343, 0.2953, 0.3019, 0.0230 }),
                    //  50
                    new Tuple<string, double, double[]>("3R69", 0.9099, new double[] { 0.9608, 0.8438, 0.3114, 0.2100, 0.0170, 0.2050, 0.0551, 0.4567, 0.2300, 0.3610 }),
                    new Tuple<string, double, double[]>("4E6S", 0.8811, new double[] { 0.9596, 0.9775, 0.8397, 0.7228, 0.7665, 0.3177, 0.1703, 0.6203, 0.0627, 0.0998 }),
                    new Tuple<string, double, double[]>("2RK5", 0.7808, new double[] { 0.3439, 0.3748, 0.2001, 0.0653, 0.1727, 0.1620, 0.1478, 0.1872, 0.2302, 0.2397 }),
                    new Tuple<string, double, double[]>("3JQU", 0.7738, new double[] { 0.0633, 0.1309, 0.6588, 0.1149, 0.5131, 0.5310, 0.7577, 0.1163, 0.3659, 0.0634 }),
                    new Tuple<string, double, double[]>("3L7H", 0.9235, new double[] { 0.6110, 0.3827, 0.6352, 0.0339, 0.1163, 0.2952, 0.3010, 0.7121, 0.6025, 0.1990 }),
                    new Tuple<string, double, double[]>("3LNW", 0.8976, new double[] { 0.0466, 0.4840, 0.3675, 0.2372, 0.4821, 0.0286, 0.5030, 0.0072, 0.0993, 0.2372 }),
                    new Tuple<string, double, double[]>("3TOE", 0.8939, new double[] { 0.8889, 0.8916, 0.8266, 0.7399, 0.5535, 0.4529, 0.2088, 0.4300, 0.7450, 0.5259 }),
                    new Tuple<string, double, double[]>("2Y4X", 0.9276, new double[] { 0.9212, 0.9529, 0.8696, 0.4243, 0.0599, 0.0855, 0.4785, 0.6849, 0.3607, 0.1814 }),
                    new Tuple<string, double, double[]>("4IOG", 0.9419, new double[] { 0.9443, 0.9213, 0.9396, 0.9108, 0.9365, 0.1809, 0.1406, 0.6645, 0.1611, 0.7882 }),
                    new Tuple<string, double, double[]>("3BRL", 0.7801, new double[] { 0.3628, 0.3807, 0.3188, 0.2573, 0.0605, 0.4540, 0.0874, 0.0898, 0.0019, 0.3894 }),
                    //  60
                    new Tuple<string, double, double[]>("3CNK", 0.8455, new double[] { 0.0986, 0.0116, 0.5370, 0.8466, 0.1104, 0.2018, 0.3532, 0.1380, 0.0129, 0.0351 }),
                    new Tuple<string, double, double[]>("3JTN", 0.8705, new double[] { 0.2226, 0.2156, 0.3777, 0.2113, 0.6730, 0.2598, 0.6561, 0.3695, 0.2343, 0.0686 }),
                    new Tuple<string, double, double[]>("3RZW", 0.9337, new double[] { 0.9681, 0.9602, 0.9326, 0.5705, 0.2501, 0.0491, 0.0742, 0.2296, 0.0134, 0.0400 }),
                    new Tuple<string, double, double[]>("3TXS", 0.9922, new double[] { 0.9918, 0.9832, 0.9915, 0.0562, 0.1202, 0.6207, 0.7768, 0.7416, 0.6686, 0.7830 }),
                    new Tuple<string, double, double[]>("4A75", 0.9230, new double[] { 0.9364, 0.0076, 0.0966, 0.4354, 0.4841, 0.1423, 0.2291, 0.0715, 0.4932, 0.0602 }),
                    new Tuple<string, double, double[]>("4HE6", 0.9221, new double[] { 0.8627, 0.6613, 0.5968, 0.5374, 0.8221, 0.1192, 0.0899, 0.0083, 0.0659, 0.1390 }),
                    new Tuple<string, double, double[]>("4HTJ", 0.9496, new double[] { 0.9180, 0.6712, 0.5577, 0.0982, 0.2195, 0.1968, 0.0090, 0.3011, 0.1662, 0.0110 }),
                    new Tuple<string, double, double[]>("1U07", 0.9164, new double[] { 0.6105, 0.8352, 0.3946, 0.2570, 0.5799, 0.1263, 0.1614, 0.0571, 0.1940, 0.2485 }),
                    new Tuple<string, double, double[]>("2BT9", 0.9273, new double[] { 0.8784, 0.7642, 0.7994, 0.8744, 0.5388, 0.4072, 0.1010, 0.1594, 0.0417, 0.6518 }),
                    new Tuple<string, double, double[]>("2X3D", 0.8905, new double[] { 0.4778, 0.5498, 0.6146, 0.7540, 0.8110, 0.8386, 0.2650, 0.6087, 0.0896, 0.2591 }),
                    //  70
                    new Tuple<string, double, double[]>("3KOV", 0.9835, new double[] { 0.5988, 0.6155, 0.7890, 0.7372, 0.8935, 0.2221, 0.3268, 0.0869, 0.0401, 0.0367 }),
                    new Tuple<string, double, double[]>("3M8J", 0.9446, new double[] { 0.9670, 0.9625, 0.9331, 0.2812, 0.2352, 0.3020, 0.3517, 0.5781, 0.3796, 0.2935 }),
                    new Tuple<string, double, double[]>("3MSH", 0.9715, new double[] { 0.8909, 0.3801, 0.2610, 0.4399, 0.0199, 0.7160, 0.4865, 0.3662, 0.6298, 0.3318 }),
                    new Tuple<string, double, double[]>("3PE9", 0.8679, new double[] { 0.7248, 0.7559, 0.8491, 0.0771, 0.2314, 0.1077, 0.0467, 0.3050, 0.2602, 0.1621 }),
                    new Tuple<string, double, double[]>("4AEQ", 0.8926, new double[] { 0.6235, 0.7117, 0.6733, 0.6381, 0.6862, 0.7182, 0.0111, 0.1353, 0.2784, 0.1559 }),
                    new Tuple<string, double, double[]>("4GS3", 0.9662, new double[] { 0.9357, 0.4791, 0.3876, 0.3974, 0.5485, 0.7746, 0.2854, 0.3613, 0.5894, 0.7033 }),
                    new Tuple<string, double, double[]>("4EWI", 0.9012, new double[] { 0.9345, 0.6731, 0.7382, 0.4256, 0.6514, 0.6741, 0.2900, 0.2769, 0.0245, 0.0893 }),
                    new Tuple<string, double, double[]>("4GMQ", 0.9410, new double[] { 0.8977, 0.7615, 0.6774, 0.4453, 0.3615, 0.0333, 0.0806, 0.1362, 0.0053, 0.5572 }),
                    new Tuple<string, double, double[]>("2X5T", 0.8804, new double[] { 0.6605, 0.7352, 0.5035, 0.7019, 0.5914, 0.0997, 0.2937, 0.0509, 0.1210, 0.3997 }),
                    new Tuple<string, double, double[]>("3ID4", 0.8890, new double[] { 0.7731, 0.1446, 0.0511, 0.5394, 0.5063, 0.1594, 0.4668, 0.0282, 0.0881, 0.1827 }),
                    //  80
                    new Tuple<string, double, double[]>("1G2R", 0.8055, new double[] { 0.2081, 0.4710, 0.6589, 0.3861, 0.0686, 0.0521, 0.3840, 0.3162, 0.4080, 0.5478 }),
                    new Tuple<string, double, double[]>("2I5C", 0.7943, new double[] { 0.2682, 0.4561, 0.1564, 0.5655, 0.0493, 0.2232, 0.5012, 0.1692, 0.2581, 0.3533 }),
                    new Tuple<string, double, double[]>("3ID1", 0.8657, new double[] { 0.9487, 0.5124, 0.1611, 0.2964, 0.5064, 0.3181, 0.2297, 0.1125, 0.1595, 0.3894 }),
                    new Tuple<string, double, double[]>("3KIK", 0.9702, new double[] { 0.1272, 0.1228, 0.6990, 0.7151, 0.8986, 0.8902, 0.8353, 0.1986, 0.1192, 0.5515 }),
                    new Tuple<string, double, double[]>("3SHU", 0.8018, new double[] { 0.1492, 0.0577, 0.1744, 0.1610, 0.3407, 0.5561, 0.1239, 0.2224, 0.2526, 0.2292 }),
                    new Tuple<string, double, double[]>("4EZA", 0.8646, new double[] { 0.0081, 0.3316, 0.1588, 0.2468, 0.5623, 0.4574, 0.2307, 0.2271, 0.5111, 0.1851 }),
                    new Tuple<string, double, double[]>("1C5E", 0.8220, new double[] { 0.5643, 0.2526, 0.5026, 0.6751, 0.0492, 0.5565, 0.5548, 0.2175, 0.0991, 0.3132 }),
                    new Tuple<string, double, double[]>("2Y2T", 0.8998, new double[] { 0.8780, 0.4245, 0.4609, 0.7211, 0.4877, 0.1808, 0.7474, 0.4868, 0.0498, 0.0521 }),
                    new Tuple<string, double, double[]>("3SQF", 0.8743, new double[] { 0.8309, 0.5251, 0.2143, 0.2209, 0.0847, 0.0152, 0.1967, 0.2973, 0.1176, 0.2418 }),
                    new Tuple<string, double, double[]>("4FQN", 0.7835, new double[] { 0.3105, 0.3369, 0.9546, 0.4377, 0.1614, 0.3295, 0.1847, 0.4618, 0.1690, 0.4894 }),
                    //  90
                    new Tuple<string, double, double[]>("3PYJ", 0.9077, new double[] { 0.7219, 0.7077, 0.9253, 0.7403, 0.5157, 0.4629, 0.6499, 0.3384, 0.2989, 0.0524 }),
                    new Tuple<string, double, double[]>("3QGL", 0.8993, new double[] { 0.9166, 0.9307, 0.8670, 0.8401, 0.7218, 0.4354, 0.7747, 0.3239, 0.5351, 0.6321 }),
                    new Tuple<string, double, double[]>("1JO0", 0.8106, new double[] { 0.2152, 0.2988, 0.6060, 0.4107, 0.0925, 0.0533, 0.0972, 0.1042, 0.0932, 0.0416 }),
                    new Tuple<string, double, double[]>("1URR", 0.6236, new double[] { 0.7717, 0.2803, 0.3200, 0.0869, 0.2052, 0.3433, 0.3743, 0.3455, 0.2230, 0.0962 }),
                    new Tuple<string, double, double[]>("3L1F", 0.9839, new double[] { 0.9931, 0.9637, 0.9453, 0.8805, 0.8548, 0.8700, 0.8061, 0.8661, 0.8848, 0.5469 }),
                    new Tuple<string, double, double[]>("3QMQ", 0.9372, new double[] { 0.7336, 0.4249, 0.6052, 0.5972, 0.2848, 0.2671, 0.0050, 0.1014, 0.1119, 0.3401 }),
                    new Tuple<string, double, double[]>("3VI6", 0.9338, new double[] { 0.9411, 0.8112, 0.7572, 0.1302, 0.4940, 0.6382, 0.1570, 0.3583, 0.2477, 0.0832 }),
                    new Tuple<string, double, double[]>("2Y9R", 0.7644, new double[] { 0.5072, 0.3084, 0.2609, 0.2312, 0.4171, 0.3625, 0.1529, 0.1385, 0.2468, 0.0332 }),
                    new Tuple<string, double, double[]>("3KNG", 0.9029, new double[] { 0.9315, 0.4003, 0.3071, 0.0719, 0.6474, 0.0951, 0.5593, 0.3129, 0.2380, 0.3949 }),
                    new Tuple<string, double, double[]>("3PO8", 0.8099, new double[] { 0.8359, 0.6182, 0.0714, 0.4010, 0.3321, 0.2681, 0.1536, 0.1862, 0.2780, 0.5273 }),
                    // 100
                    new Tuple<string, double, double[]>("2QJL", 0.8300, new double[] { 0.5695, 0.0947, 0.0816, 0.6035, 0.4145, 0.2774, 0.3235, 0.4300, 0.2553, 0.3109 }),
                    new Tuple<string, double, double[]>("3NS6", 0.9532, new double[] { 0.9823, 0.7563, 0.7476, 0.9027, 0.1613, 0.6101, 0.0907, 0.3676, 0.2105, 0.1547 }),
                    new Tuple<string, double, double[]>("2XRH", 0.9900, new double[] { 0.9946, 0.8911, 0.7698, 0.8890, 0.9518, 0.3462, 0.1086, 0.3910, 0.7540, 0.5802 }),
                    new Tuple<string, double, double[]>("3K6F", 0.8780, new double[] { 0.9834, 0.9429, 0.9294, 0.2580, 0.6046, 0.4401, 0.0257, 0.1125, 0.0300, 0.5318 }),
                    new Tuple<string, double, double[]>("3NXA", 0.9762, new double[] { 0.9401, 0.9275, 0.5476, 0.5459, 0.8369, 0.6330, 0.7272, 0.5941, 0.6639, 0.3751 }),
                    new Tuple<string, double, double[]>("3RHB", 0.9044, new double[] { 0.0195, 0.6503, 0.1292, 0.1497, 0.3676, 0.1599, 0.0228, 0.5270, 0.3785, 0.0728 }),
                    new Tuple<string, double, double[]>("1MG4", 0.9211, new double[] { 0.7286, 0.1848, 0.1405, 0.6538, 0.3280, 0.2954, 0.4246, 0.0833, 0.2201, 0.4519 }),
                    new Tuple<string, double, double[]>("3SSQ", 0.8964, new double[] { 0.9167, 0.9047, 0.9480, 0.2635, 0.2888, 0.4867, 0.1808, 0.3802, 0.4726, 0.0319 }),
                    new Tuple<string, double, double[]>("3U1C", 0.9932, new double[] { 0.9602, 0.9599, 0.9915, 0.9900, 0.9712, 0.9534, 0.7761, 0.7804, 0.3860, 0.4965 }),
                    new Tuple<string, double, double[]>("3M9J", 0.8286, new double[] { 0.8297, 0.4286, 0.4697, 0.1271, 0.1682, 0.0036, 0.4266, 0.1022, 0.1261, 0.2385 }),
                    // 110
                    new Tuple<string, double, double[]>("3FG7", 0.9214, new double[] { 0.8107, 0.2658, 0.4836, 0.2632, 0.1679, 0.1266, 0.3018, 0.0298, 0.2791, 0.1318 }),
                    new Tuple<string, double, double[]>("3CX2", 0.9153, new double[] { 0.8220, 0.3367, 0.1810, 0.4380, 0.1804, 0.4670, 0.1833, 0.4266, 0.6919, 0.0404 }),
                    new Tuple<string, double, double[]>("1BKR", 0.8358, new double[] { 0.3336, 0.3207, 0.3375, 0.0744, 0.1182, 0.1364, 0.0321, 0.0762, 0.0680, 0.3356 }),
                    new Tuple<string, double, double[]>("3LRG", 0.7694, new double[] { 0.0402, 0.2520, 0.3358, 0.4982, 0.2573, 0.1622, 0.2875, 0.1650, 0.0116, 0.3150 }),
                    new Tuple<string, double, double[]>("4BBD", 0.8835, new double[] { 0.7326, 0.5754, 0.6572, 0.5158, 0.5739, 0.2586, 0.0855, 0.2834, 0.2624, 0.3975 }),
                    new Tuple<string, double, double[]>("2CKK", 0.5433, new double[] { 0.2420, 0.0490, 0.1779, 0.0608, 0.1243, 0.2359, 0.0755, 0.2071, 0.3683, 0.2983 }),
                    new Tuple<string, double, double[]>("2ZWM", 0.8656, new double[] { 0.1890, 0.0938, 0.5432, 0.2409, 0.5588, 0.0384, 0.2699, 0.2881, 0.1603, 0.2447 }),
                    new Tuple<string, double, double[]>("3TDM", 0.8755, new double[] { 0.9193, 0.0652, 0.1232, 0.2477, 0.3652, 0.1207, 0.3438, 0.1029, 0.0076, 0.1024 }),
                    new Tuple<string, double, double[]>("2YH5", 0.8578, new double[] { 0.3079, 0.2593, 0.4895, 0.5313, 0.1860, 0.4557, 0.6796, 0.3554, 0.3545, 0.5307 }),
                    new Tuple<string, double, double[]>("3LKY", 0.8899, new double[] { 0.7205, 0.6840, 0.2582, 0.3431, 0.2068, 0.1787, 0.1529, 0.3679, 0.2171, 0.2850 }),
                    // 120
                    new Tuple<string, double, double[]>("4A6S", 0.9403, new double[] { 0.9599, 0.9280, 0.8957, 0.4829, 0.5384, 0.6251, 0.2496, 0.2061, 0.3663, 0.0807 }),
                    new Tuple<string, double, double[]>("1R29", 0.9683, new double[] { 0.5241, 0.4913, 0.8552, 0.1791, 0.2430, 0.1571, 0.3341, 0.5805, 0.0786, 0.0703 }),
                    new Tuple<string, double, double[]>("3NJM", 0.9358, new double[] { 0.8783, 0.7963, 0.7502, 0.8442, 0.7139, 0.8786, 0.3080, 0.4615, 0.3842, 0.4498 }),
                    new Tuple<string, double, double[]>("3P38", 0.9541, new double[] { 0.8047, 0.8089, 0.4170, 0.0891, 0.0030, 0.2017, 0.0912, 0.3374, 0.3621, 0.6373 }),
                    new Tuple<string, double, double[]>("3P6J", 0.8228, new double[] { 0.5963, 0.6655, 0.7570, 0.5608, 0.0982, 0.2119, 0.4200, 0.2838, 0.2977, 0.0465 }),
                    new Tuple<string, double, double[]>("3RNV", 0.9454, new double[] { 0.0699, 0.0168, 0.6623, 0.5655, 0.5158, 0.4990, 0.0330, 0.3672, 0.0348, 0.3037 }),
                    new Tuple<string, double, double[]>("1MM9", 0.9694, new double[] { 0.9833, 0.5056, 0.6438, 0.4484, 0.4572, 0.4832, 0.0682, 0.5764, 0.6287, 0.1772 }),
                    new Tuple<string, double, double[]>("3UJ3", 0.7035, new double[] { 0.0731, 0.4327, 0.5193, 0.3668, 0.1137, 0.1123, 0.3789, 0.1814, 0.0611, 0.2026 }),
                    new Tuple<string, double, double[]>("1DBF", 0.9680, new double[] { 0.8727, 0.8408, 0.8310, 0.6435, 0.2203, 0.1972, 0.4642, 0.0883, 0.1424, 0.5127 }),
                    new Tuple<string, double, double[]>("3HA4", 0.8570, new double[] { 0.6245, 0.4083, 0.3350, 0.6346, 0.4496, 0.1655, 0.0565, 0.0083, 0.3711, 0.2120 }),
                    // 130
                    new Tuple<string, double, double[]>("3VA9", 0.8989, new double[] { 0.9491, 0.9234, 0.9159, 0.1846, 0.4382, 0.0725, 0.1883, 0.5129, 0.0370, 0.1410 }),
                    new Tuple<string, double, double[]>("4FYH", 0.8917, new double[] { 0.8532, 0.8547, 0.4830, 0.2530, 0.1911, 0.5514, 0.4552, 0.2508, 0.3810, 0.1713 }),
                    new Tuple<string, double, double[]>("4GCN", 0.8400, new double[] { 0.9013, 0.6241, 0.4203, 0.7815, 0.4088, 0.0947, 0.0267, 0.2170, 0.7423, 0.0027 }),
                    new Tuple<string, double, double[]>("4HBX", 0.8876, new double[] { 0.9456, 0.5326, 0.8353, 0.0650, 0.3993, 0.0507, 0.0291, 0.2072, 0.3342, 0.0148 }),
                    new Tuple<string, double, double[]>("2XX6", 0.9147, new double[] { 0.1299, 0.2371, 0.3809, 0.2564, 0.5143, 0.0307, 0.3403, 0.3940, 0.2634, 0.4771 }),
                    new Tuple<string, double, double[]>("4DRO", 0.9476, new double[] { 0.8284, 0.6152, 0.0419, 0.7311, 0.0955, 0.0674, 0.1875, 0.3576, 0.1610, 0.0582 }),
                    new Tuple<string, double, double[]>("4F55", 0.9540, new double[] { 0.7989, 0.7182, 0.0820, 0.1510, 0.1658, 0.4102, 0.0156, 0.3711, 0.2110, 0.2608 }),
                    new Tuple<string, double, double[]>("2AAJ", 0.3649, new double[] { 0.0059, 0.0643, 0.6154, 0.7142, 0.5231, 0.5070, 0.2378, 0.2858, 0.4474, 0.3625 }),
                    new Tuple<string, double, double[]>("3LRD", 0.9460, new double[] { 0.8807, 0.9630, 0.5654, 0.6034, 0.4295, 0.7417, 0.7859, 0.3356, 0.6963, 0.6384 }),
                    new Tuple<string, double, double[]>("3O48", 0.9349, new double[] { 0.9700, 0.9583, 0.7201, 0.2083, 0.0844, 0.6869, 0.7857, 0.1812, 0.5779, 0.0893 }),
                    // 140
                    new Tuple<string, double, double[]>("3T8N", 0.9374, new double[] { 0.8990, 0.9199, 0.8951, 0.8067, 0.7058, 0.3251, 0.2126, 0.6416, 0.4414, 0.6726 }),
                    new Tuple<string, double, double[]>("3UD8", 0.8592, new double[] { 0.0253, 0.2388, 0.0564, 0.0463, 0.4800, 0.2961, 0.1772, 0.3099, 0.3705, 0.1403 }),
                    new Tuple<string, double, double[]>("4EDL", 0.8202, new double[] { 0.1374, 0.1768, 0.0168, 0.2704, 0.2297, 0.5268, 0.1742, 0.2010, 0.2088, 0.0979 }),
                    new Tuple<string, double, double[]>("2XW6", 0.9322, new double[] { 0.2056, 0.1052, 0.8810, 0.0556, 0.1277, 0.4944, 0.3967, 0.0442, 0.4922, 0.1829 }),
                    new Tuple<string, double, double[]>("2LIS", 0.9273, new double[] { 0.9204, 0.8676, 0.8697, 0.1286, 0.4465, 0.0289, 0.0911, 0.4071, 0.0126, 0.2735 }),
                    new Tuple<string, double, double[]>("2R8U", 0.8833, new double[] { 0.1544, 0.3018, 0.5837, 0.1759, 0.3894, 0.3856, 0.3999, 0.0429, 0.1161, 0.2443 }),
                    new Tuple<string, double, double[]>("3QF3", 0.9363, new double[] { 0.9334, 0.9529, 0.6146, 0.6957, 0.1753, 0.5943, 0.1317, 0.2119, 0.1460, 0.0594 }),
                    new Tuple<string, double, double[]>("3D4W", 0.9260, new double[] { 0.0421, 0.5757, 0.2596, 0.5926, 0.5310, 0.7815, 0.3214, 0.2664, 0.0972, 0.1103 }),
                    new Tuple<string, double, double[]>("3LLO", 0.8811, new double[] { 0.7833, 0.0627, 0.2800, 0.5277, 0.6303, 0.2954, 0.5438, 0.3694, 0.1442, 0.1409 }),
                    new Tuple<string, double, double[]>("3OBL", 0.9200, new double[] { 0.9243, 0.8731, 0.4561, 0.3460, 0.1569, 0.0596, 0.0160, 0.3485, 0.0941, 0.2612 }),
                    // 150
                    new Tuple<string, double, double[]>("3Q47", 0.8464, new double[] { 0.9565, 0.9582, 0.1906, 0.7696, 0.0085, 0.0454, 0.7526, 0.4323, 0.1982, 0.1642 }),
                    new Tuple<string, double, double[]>("3R87", 0.9421, new double[] { 0.9555, 0.9495, 0.9272, 0.7857, 0.8347, 0.0889, 0.0845, 0.3794, 0.6251, 0.4488 }),
                    new Tuple<string, double, double[]>("4HX8", 0.9336, new double[] { 0.9661, 0.9478, 0.8066, 0.8125, 0.9595, 0.4544, 0.2114, 0.4270, 0.4497, 0.0063 }),
                    new Tuple<string, double, double[]>("3KXY", 0.8841, new double[] { 0.9665, 0.6750, 0.6228, 0.2081, 0.7023, 0.0741, 0.1507, 0.0198, 0.7290, 0.7159 }),
                    new Tuple<string, double, double[]>("3RSW", 0.9198, new double[] { 0.8389, 0.5524, 0.5734, 0.2733, 0.0977, 0.0171, 0.1932, 0.0549, 0.0384, 0.1833 }),
                    new Tuple<string, double, double[]>("2R6Q", 0.5802, new double[] { 0.2492, 0.1222, 0.3850, 0.0680, 0.5897, 0.0015, 0.3991, 0.6737, 0.4827, 0.0665 }),
                    new Tuple<string, double, double[]>("2XDH", 0.8005, new double[] { 0.6882, 0.8297, 0.4788, 0.2701, 0.0318, 0.3513, 0.1839, 0.1724, 0.2283, 0.0613 }),
                    new Tuple<string, double, double[]>("2XG3", 0.6686, new double[] { 0.0292, 0.3003, 0.1581, 0.0007, 0.3283, 0.4599, 0.3278, 0.1651, 0.0764, 0.1379 }),
                    new Tuple<string, double, double[]>("3R85", 0.7631, new double[] { 0.7681, 0.4024, 0.4020, 0.1836, 0.1977, 0.0427, 0.4732, 0.1614, 0.3314, 0.0161 }),
                    new Tuple<string, double, double[]>("2PWO", 0.9701, new double[] { 0.8710, 0.8250, 0.2307, 0.3363, 0.5837, 0.5787, 0.5110, 0.5090, 0.0411, 0.0146 }),
                    // 160
                    new Tuple<string, double, double[]>("3GZ2", 0.8964, new double[] { 0.8904, 0.6035, 0.4182, 0.7057, 0.6371, 0.4623, 0.4827, 0.0259, 0.5512, 0.0924 }),
                    new Tuple<string, double, double[]>("1NWW", 0.8669, new double[] { 0.4006, 0.4597, 0.4479, 0.3894, 0.6262, 0.6173, 0.4278, 0.3427, 0.2681, 0.2802 }),
                    new Tuple<string, double, double[]>("2JDC", 0.8235, new double[] { 0.4229, 0.2341, 0.7208, 0.6896, 0.2784, 0.3736, 0.3576, 0.0769, 0.0030, 0.2589 }),
                    new Tuple<string, double, double[]>("2XEM", 0.9504, new double[] { 0.9528, 0.9388, 0.4469, 0.4878, 0.7738, 0.7242, 0.1430, 0.4646, 0.3050, 0.1172 }),
                    new Tuple<string, double, double[]>("3S02", 0.9426, new double[] { 0.7640, 0.1253, 0.2667, 0.6139, 0.7451, 0.0006, 0.2313, 0.3131, 0.6242, 0.1462 }),
                    new Tuple<string, double, double[]>("3SEI", 0.8839, new double[] { 0.0960, 0.7641, 0.0973, 0.5190, 0.4819, 0.3181, 0.1960, 0.3377, 0.1189, 0.1630 }),
                    new Tuple<string, double, double[]>("4F8A", 0.8211, new double[] { 0.6814, 0.5529, 0.5428, 0.4518, 0.0971, 0.4139, 0.1041, 0.0112, 0.2275, 0.0263 }),
                    new Tuple<string, double, double[]>("1J8Q", 0.8255, new double[] { 0.6132, 0.7653, 0.6803, 0.0804, 0.3057, 0.0687, 0.1957, 0.1475, 0.1826, 0.4400 }),
                    new Tuple<string, double, double[]>("2FL4", 0.9093, new double[] { 0.5513, 0.5257, 0.4008, 0.0864, 0.5591, 0.1801, 0.0800, 0.0182, 0.2774, 0.2144 }),
                    new Tuple<string, double, double[]>("3HGM", 0.9175, new double[] { 0.3146, 0.4743, 0.3938, 0.7660, 0.0930, 0.0744, 0.2153, 0.1316, 0.1209, 0.1588 }),
                    // 170
                    new Tuple<string, double, double[]>("3RKV", 0.8984, new double[] { 0.9738, 0.9587, 0.6646, 0.7106, 0.4644, 0.5914, 0.6419, 0.4897, 0.0814, 0.4103 }),
                    new Tuple<string, double, double[]>("4E8O", 0.9505, new double[] { 0.6458, 0.8029, 0.5450, 0.5933, 0.6174, 0.5187, 0.1627, 0.5656, 0.2267, 0.1234 }),
                    new Tuple<string, double, double[]>("3NBC", 0.9197, new double[] { 0.7417, 0.7224, 0.2160, 0.4622, 0.7008, 0.2402, 0.2401, 0.1734, 0.1186, 0.3740 }),
                    new Tuple<string, double, double[]>("1GU1", 0.8878, new double[] { 0.6058, 0.3093, 0.3993, 0.4259, 0.2717, 0.0538, 0.4796, 0.1250, 0.1607, 0.1080 }),
                    new Tuple<string, double, double[]>("2Y9F", 0.7439, new double[] { 0.2286, 0.2521, 0.1823, 0.0533, 0.1889, 0.1974, 0.1352, 0.2557, 0.1816, 0.0804 }),
                    new Tuple<string, double, double[]>("3AXC", 0.9210, new double[] { 0.9395, 0.9308, 0.9407, 0.7507, 0.3783, 0.7050, 0.1710, 0.1850, 0.2819, 0.1997 }),
                    new Tuple<string, double, double[]>("3MBT", 0.8614, new double[] { 0.2007, 0.0107, 0.4824, 0.6315, 0.0053, 0.1804, 0.4296, 0.4480, 0.3615, 0.3834 }),
                    // 177
                };
            }
        }
    }
}