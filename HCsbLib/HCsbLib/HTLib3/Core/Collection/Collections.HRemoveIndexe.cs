using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<T> HRemoveIndex<T>(this List<T> values, IEnumerable<int> indexes)
        {
            if(Debug.SelftestDo())
                #region SelfTest
            {
                int[] test_values = new int[] { 0, 1, 2, 3, 4, 5, };
                int[] test_indexes = new int[] { 2, 5, 0, 3, };
                List<int> test_return = HRemoveIndex(new List<int>(test_values), test_indexes);
                Debug.Assert(test_return.Count == 2);
                Debug.Assert(test_return[0] == 1);
                Debug.Assert(test_return[1] == 4);
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
        public static T[] HRemoveIndex<T>(this T[] values, IEnumerable<int> indexes)
        {
            return HRemoveIndex(new List<T>(values), indexes).ToArray();
        }
    }
}
