using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class HDebug
	{
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertToleranceVector(double tolerance, params IVector<double>[] values)
        {
            bool assert = true;
            for(int i=0; i<values.Length; i++)
                for(int j=0; j<values[i].Size; j++)
                    assert &= (Math.Abs(values[i][j]) <= tolerance);
            System.Diagnostics.Debug.Assert(assert);
        }
    }
}
