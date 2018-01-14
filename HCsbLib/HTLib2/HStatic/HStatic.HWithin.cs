using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HNext_SelectIdx<T>(this IList<int> idxs, IList<T> within)
        {
            List<T> sele = new List<T>();
            foreach(int idx in idxs)
                sele.Add(within[idx]);
            return sele.ToArray();
        }
        public static int[] HNext_IdxEqual<T>(this T value, IList<T> within)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<within.Count; i++)
            {
                dynamic withini = within[i];
                if(withini == value)
                    idxs.Add(i);
            }
            return idxs.ToArray();
        }
    }
}
