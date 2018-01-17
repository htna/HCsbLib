using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HTLib2
{
	public partial class Vector : ICloneable
	{
        public Vector SubVector(IList<int> idxs)
        {
            double[] subvec = new double[idxs.Count];
            for(int ni=0; ni<idxs.Count; ni++)
            {
                int i = idxs[ni];
                subvec[ni] = this[i];
            }
            return subvec;
        }
	}
}
