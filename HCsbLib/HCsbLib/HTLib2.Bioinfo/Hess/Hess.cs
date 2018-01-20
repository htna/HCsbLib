using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static HessMatrix GetHessCoarseBlkmat(Matrix hess, IList<int> idx_heavy, ILinAlg ila, double? chkDiagToler, string invtype, params object[] invopt)
        {
            throw new NotImplementedException();
        }
        public static HessMatrixDense GetHessCoarseBlkmat(HessMatrix hess, IList<int> idx_heavy, string invopt = "inv")
        {
            throw new NotImplementedException();
        }
        public static HessMatrix GetHessCoarseBlkmat(Matrix hess, IList<int> idx_heavy, ILinAlg ila)
        {
            throw new NotImplementedException();
        }
    }
}
