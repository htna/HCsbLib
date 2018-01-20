using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static bool SelfTest()
        {
            HDebug.Verify(Hess.GetHessAnmSelfTest());
            HDebug.Verify(Hess.GetHessGnmSelfTest());
            HDebug.Verify(Hess.GetModesSelftest());
            HDebug.Verify(Hess.GetBFactorSelfTest());
            //HDebug.Verify(Hess.HessSbNMA.SelfTest());
            return true;
        }
    }
}
