using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static void UpdateHessNaN(HessMatrix hess, IList<Vector> coords)
        {
            int[] idxNaN = coords.HIndexOfNull();
            Hess.UpdateHessNaN(hess, idxNaN);
        }
        public static void UpdateHessNaN(HessMatrix hess, IList<int> idxNaN)
        {
            int size3 = hess.ColSize;
            HDebug.Assert(hess.ColSize == hess.RowSize);
            foreach(int i in idxNaN)
            {
                int i3 = i*3;
                for(int j=0; j<size3; j++)
                {
                    hess[i3+0, j] = double.NaN;
                    hess[i3+1, j] = double.NaN;
                    hess[i3+2, j] = double.NaN;
                    hess[j, i3+0] = double.NaN;
                    hess[j, i3+1] = double.NaN;
                    hess[j, i3+2] = double.NaN;
                }
            }
        }
    }
}
