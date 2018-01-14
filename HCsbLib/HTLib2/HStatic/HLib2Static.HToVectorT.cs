using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class HLib2Static
    {
        public static TVector<T> HToVectorT<T>(this T[] val)
        {
            return new TVector<T>(val);
        }
        public static TVector<T> HToVectorT<T>(this IList<T> val)
        {
            return new TVector<T>(val.ToArray());
        }
    }
}
