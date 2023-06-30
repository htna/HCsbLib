using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
	public partial class Vector : ICloneable
	{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector SubVector(params int[] idxs)
        {
            return SubVector((IList<int>)idxs);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
