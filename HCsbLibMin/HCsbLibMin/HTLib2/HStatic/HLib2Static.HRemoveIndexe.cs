using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        internal static bool HRemoveIndexe_SelfTest = HDebug.IsDebuggerAttached;
        public static List<T> HRemoveIndexe<T>(this List<T> values, IEnumerable<int> indexes)
        {
            if(HRemoveIndexe_SelfTest)
                #region SelfTest
            {
                HRemoveIndexe_SelfTest = false;
                int[] test_values = new int[] { 0, 1, 2, 3, 4, 5, };
                int[] test_indexes = new int[] { 2, 5, 0, 3, };
                List<int> test_return = HRemoveIndexe(new List<int>(test_values), test_indexes);
                HDebug.Assert(test_return.Count == 2);
                HDebug.Assert(test_return[0] == 1);
                HDebug.Assert(test_return[1] == 4);
            }
                #endregion
            values = values.HClone();
            List<int> idxs = new List<int>(indexes);
            idxs.Sort();
            idxs.Reverse();
            foreach(int idx in idxs)
                values.RemoveAt(idx);
            return values;
        }
        public static T[] HRemoveIndexe<T>(this T[] values, IEnumerable<int> indexes)
        {
            return HRemoveIndexe(new List<T>(values), indexes).ToArray();
        }
    }
}
