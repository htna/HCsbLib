using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static string[] HSelectContains(this IList<string> list, string contains)
        {
            List<string> sele = new List<string>();
            foreach(string item in list)
                if(item.Contains(contains))
                    sele.Add(item);
            return sele.ToArray();
        }
        public static string HSelectFirstContains(this IList<string> list, string contains)
        {
            foreach(string item in list)
                if(item.Contains(contains))
                    return item;
            return null;
        }

        public static string[] HSelectStartsWith(this IList<string> list, string startswith)
        {
            List<string> sele = new List<string>();
            foreach(string item in list)
                if(item.StartsWith(startswith))
                    sele.Add(item);
            return sele.ToArray();
        }
        public static string HSelectFirstStartsWith(this IList<string> list, string startswith)
        {
            foreach(string item in list)
                if(item.StartsWith(startswith))
                    return item;
            return null;
        }

        public static string[] HSelectEndsWith(this IList<string> list, string endswith)
        {
            List<string> sele = new List<string>();
            foreach(string item in list)
                if(item.EndsWith(endswith))
                    sele.Add(item);
            return sele.ToArray();
        }
        public static string HSelectFirstEndsWith(this IList<string> list, string endswith)
        {
            foreach(string item in list)
                if(item.EndsWith(endswith))
                    return item;
            return null;
        }
    }
}
