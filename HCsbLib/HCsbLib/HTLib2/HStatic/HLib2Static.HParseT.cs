using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T[] HParse<T>(this IList<string> strings, Func<string, T> parse)
        {
            T[] values = new T[strings.Count];
            for(int i=0; i<strings.Count; i++)
                values[i] = parse(strings[i]);
            return values;
        }
        public static int[] HParseInt(this IList<string> strings)
        {
            return HParse(strings, int.Parse);
        }
        public static double[] HParseDouble(this IList<string> strings)
        {
            return HParse(strings, double.Parse);
        }
    }
}
