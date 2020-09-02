using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static bool HCompEqual<T>(this T[] left, T[] right)
            where T : IComparable<T>
        {
            if(left == null || right == null)
            {
                if(left  != null) return false;
                if(right != null) return false;
                return true;
            }
            if(left.Length != right.Length) return false;
            for(int i=0; i<left.Length; i++)
                if(left[i].CompareTo(right[i]) != 0)
                    return false;
            return true;
        }
    }
}
