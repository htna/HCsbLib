using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static int[] HIdxNotNaN(this IList<double> values)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(double.IsNaN(values[i]) == false)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxNaN(this IList<double> values)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(double.IsNaN(values[i]))
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxNegativeInfinity(this IList<double> values)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(double.IsNegativeInfinity(values[i]))
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxPositiveInfinity(this IList<double> values)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(double.IsPositiveInfinity(values[i]))
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static Tuple<double, int>[] HSelectNotNaN(this IList<double> list)
        {
            List<Tuple<double,int>> sele = new List<Tuple<double, int>>();
            for(int i=0; i<list.Count; i++)
            {
                if(double.IsNaN(list[i]))
                    continue;
                sele.Add(new Tuple<double, int>(list[i], i));
            }
            return sele.ToArray();
        }
    }
}
