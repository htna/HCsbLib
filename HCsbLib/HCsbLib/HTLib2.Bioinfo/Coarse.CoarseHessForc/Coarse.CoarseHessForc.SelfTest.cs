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
                if(HDebug.Selftest() == false)
                    return;

                string temppath = @"K:\temp\";
                string tinkerpath_testgrad = "\""+@"C:\Program Files\Tinker\bin-win64-8.2.1\testgrad.exe"+"\"";
                string tinkerpath_testhess = "\""+@"C:\Program Files\Tinker\bin-win64-8.2.1\testhess.exe"+"\"";

                var xyz = Tinker.Xyz.FromLines(SelftestData.lines_1L2Y_xyz);
                var prm = Tinker.Prm.FromLines(SelftestData.lines_charmm22_prm);
                var univ = Universe.Build(xyz, prm);

                var hess = Tinker.Run.Testhess(tinkerpath_testhess, xyz, prm, temppath);
                var grad = Tinker.Run.Testgrad(tinkerpath_testgrad, xyz, prm, temppath);
            }
        }
    }
}
