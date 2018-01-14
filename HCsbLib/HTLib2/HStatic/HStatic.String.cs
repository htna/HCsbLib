using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Tuple<string, string>[] HTrim(this IList<Tuple<string, string>> values)
        {
            Tuple<string, string>[] trims = new Tuple<string, string>[values.Count];
            for(int i=0; i<values.Count; i++)
                trims[i] = new Tuple<string, string>(values[i].Item1.Trim(), values[i].Item2.Trim());
            return trims;
        }
        public static string[] HTrim(this IList<string> values)
        {
            string[] trims = new string[values.Count];
            for(int i=0; i<values.Count; i++)
                trims[i] = values[i].Trim();
            return trims;
        }
        public static string[] HTrimStart(this IList<string> values, params char[] trimChars)
        {
            string[] trims = new string[values.Count];
            for(int i=0; i<values.Count; i++)
                trims[i] = values[i].TrimStart(trimChars);
            return trims;
        }
        public static string[] HTrimEnd(this IList<string> values, params char[] trimChars)
        {
            string[] trims = new string[values.Count];
            for(int i=0; i<values.Count; i++)
                trims[i] = values[i].TrimEnd(trimChars);
            return trims;
        }
        public static string HConcat(this IList<string> values, string delim)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0; i<values.Count; i++)
            {
                if(i != 0) sb.Append(delim);
                sb.Append(values[i]);
            }
            return sb.ToString();
        }
        public static string[] HRemoveAllContains(this IList<string> values, string toremove)
        {
            string[] removeds = values.ToArray();
            /// remove "+Xe"
            for(int i=0; i<removeds.Length; i++)
                if(removeds[i].Contains(toremove))
                    removeds[i] = null;
            removeds = removeds.HRemoveAll(null);
            return removeds;
        }
        public static bool HIndexAllContains_selftest = HDebug.IsDebuggerAttached;
        public static int[] HIndexAllContains(this IList<string> values, string contain)
        {
            if(HIndexAllContains_selftest)
            {
                HIndexAllContains_selftest = false;
                string[] tvalues = new string[]
                {
                    "",
                    "abc",
                    " abc ",
                    " ab ",
                    " cba ",
                };
                int[] tidx;
                tidx = HIndexAllContains(tvalues, "ab");
                HDebug.Exception(tidx.Length == 3);
                HDebug.Exception(tidx[0] == 1, tidx[1] == 2, tidx[2] == 3);
                tidx = HIndexAllContains(tvalues, "abc");
                HDebug.Exception(tidx.Length == 2);
                HDebug.Exception(tidx[0] == 1, tidx[1] == 2);
                tidx = HIndexAllContains(tvalues, "abc ");
                HDebug.Exception(tidx.Length == 1);
                HDebug.Exception(tidx[0] == 2);
                tidx = HIndexAllContains(tvalues, "abcd");
                HDebug.Exception(tidx.Length == 0);
            }
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(values[i].Contains(contain))
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIndexAllEqual(this IList<string> values, string equalto)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(values[i] == equalto)
                    idxs.Add(i);
            return idxs.ToArray();
        }
    }
}
