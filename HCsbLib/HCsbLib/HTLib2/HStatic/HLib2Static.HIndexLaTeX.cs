using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static int[] HIndexNeq<T>(this IList<T> list, T comp) where T : IEquatable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].Equals(comp) == false)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexEql<T>(this IList<T> list, T comp) where T : IEquatable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].Equals(comp) == true)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexLes<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) < 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexLeq<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) <= 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexGre<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) > 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexGeq<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) >= 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexIn<T>(this IList<T> list, IList<T> comp) where T : IEquatable<T>
        {
            HashSet<T> set = new HashSet<T>(comp);
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(set.Contains(list[i]))
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexNin<T>(this IList<T> list, IList<T> comp) where T : IEquatable<T>
        {
            HashSet<T> set = new HashSet<T>(comp);
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(set.Contains(list[i]) == false)
                    idxs.Add(i);
            return idxs.ToArray();
        }
    }
}
