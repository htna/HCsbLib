using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<double> HEnumAbs(this IEnumerable<double> values)
        {
            foreach(var v in values)
                yield return Math.Abs(v);
        }
        public static IEnumerable<int> HEnumAbs(this IEnumerable<int> values)
        {
            foreach(var v in values)
                yield return Math.Abs(v);
        }
    }
}
