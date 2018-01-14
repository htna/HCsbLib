using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static string[] HSplit(this string line, params char[] separator)
        {
            return line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }
        //public static string[] HSplit(this string line, string separator)
        //{
        //    return line.HSplit(separator.ToCharArray());
        //}
        public static string[] HSplit(this string line)
        {
            return line.Split().HRemoveAll("");
        }
    }
}
