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

            }
        }
    }
}
