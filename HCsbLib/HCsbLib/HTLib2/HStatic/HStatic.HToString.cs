using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static string[] HToStrings<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> values, string separator)  { List<string> strs=new List<string>(); Action<object> add=delegate(object v) { strs.Add(v.ToString()); }; add(values.Item1); add(values.Item2); add(values.Item3); add(values.Item4); add(values.Item5); return strs.ToArray(); }
        public static string[] HToStrings<T1, T2, T3, T4    >(this Tuple<T1, T2, T3, T4    > values, string separator)  { List<string> strs=new List<string>(); Action<object> add=delegate(object v) { strs.Add(v.ToString()); }; add(values.Item1); add(values.Item2); add(values.Item3); add(values.Item4);                    return strs.ToArray(); }
        public static string[] HToStrings<T1, T2, T3        >(this Tuple<T1, T2, T3        > values, string separator)  { List<string> strs=new List<string>(); Action<object> add=delegate(object v) { strs.Add(v.ToString()); }; add(values.Item1); add(values.Item2); add(values.Item3);                                       return strs.ToArray(); }
        public static string[] HToStrings<T1, T2            >(this Tuple<T1, T2            > values, string separator)  { List<string> strs=new List<string>(); Action<object> add=delegate(object v) { strs.Add(v.ToString()); }; add(values.Item1); add(values.Item2);                                                          return strs.ToArray(); }
        public static string[] HToStrings<T1                >(this Tuple<T1                > values, string separator)  { List<string> strs=new List<string>(); Action<object> add=delegate(object v) { strs.Add(v.ToString()); }; add(values.Item1);                                                                             return strs.ToArray(); }

        public static string HToString(this IEnumerable<string> values, string separator)
        {
            StringBuilder sb = new StringBuilder();
            bool add_separator = false;
            foreach(string line in values)
            {
                if(add_separator) sb.Append(separator);
                sb.Append(line);
                add_separator = true;
            }
            return sb.ToString();
        }
        public static Func<T, string> HFuncToString<T>(string format)
        {
            Func<T, string> tostring = delegate(T value)
            {
                //return string.Format(format, value);
                string str = (value as dynamic).ToString(format);
                return str;
            };
            return tostring;
        }
        public static string HToStringSeparated<T>(this IEnumerable<T> values, string separator)
        {
            Func<T,string> tostring = delegate(T val)
            {
                return val.ToString();
            };
            return HToStringSeparated(values, tostring, separator);
        }
        public static string HToStringSeparated<T>(this IEnumerable<T> values, string tostring_format, string separator)
        {
            Func<T,string> tostring = delegate(T value)
            {
                string str = string.Format(tostring_format, value);
                return str;
            };
            return HToStringSeparated(values, tostring, separator);
        }
        public static string HToStringSeparated<T>(this IEnumerable<T> values, Func<T,string> tostring, string separator)
        {
            StringBuilder sb = new StringBuilder();
            bool addsep = false;
            foreach(var value in values)
            {
                if(addsep) sb.Append(separator);
                sb.Append(tostring(value));
                addsep = true;
            }
            return sb.ToString();
        }
        public static string HToStringSeparated<T>(this T[,] values, Func<T, string> tostring
                                                  ,string colbegin, string colsep, string colend
                                                  ,string rowbegin, string rowsep, string rowend
                                                  )
        {
            StringBuilder sb = new StringBuilder();
            int colsize = values.GetLength(0);
            int rowsize = values.GetLength(1);

            sb.Append(colbegin);
            for(int c=0; c<colsize; c++)
            {
                if(c != 0) sb.Append(colsep);

                sb.Append(rowbegin);
                for(int r=0; r<rowsize; r++)
                {
                    if(r != 0) sb.Append(rowsep);
                    var val = values[c, r];
                    var str = tostring(val);
                    sb.Append(str);
                }
                sb.Append(rowend);
            }
            sb.Append(colend);

            return sb.ToString();
        }
    }
}
