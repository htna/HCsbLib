using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    using NumberStyles = System.Globalization.NumberStyles;
    public static partial class HStatic
    {
        public static int   ? HParseIntHex(this string str) { int    val; if(int   .TryParse(str, NumberStyles.HexNumber, null, out val)) return val; return null; }
        public static int   ? HParseInt   (this string str) { int    val; if(int   .TryParse(str, out val)) return val; return null; }
        public static double? HParseDouble(this string str) { double val; if(double.TryParse(str, out val)) return val; return null; }
        public static bool  ? HParseBool  (this string str) { bool   val; if(bool  .TryParse(str, out val)) return val; return null; }

        static bool HParseIntArr_selftest = HDebug.IsDebuggerAttached;
        public static int[]   HParseIntArr(this string str)
        {
            if(HParseIntArr_selftest)
            {
                HParseIntArr_selftest = false;
                int[] tarr;
                tarr = HParseIntArr("1, 2 ,3");
                HDebug.Assert(tarr.Length == 3);
                HDebug.Assert(tarr[0] == 1);
                HDebug.Assert(tarr[1] == 2);
                HDebug.Assert(tarr[2] == 3);
                tarr = HParseIntArr("1,4--6");
                HDebug.Assert(tarr.Length == 4);
                HDebug.Assert(tarr[0] == 1);
                HDebug.Assert(tarr[1] == 4);
                HDebug.Assert(tarr[2] == 5);
                HDebug.Assert(tarr[3] == 6);
                tarr = HParseIntArr("1,-6-- -4");
                HDebug.Assert(tarr.Length == 4);
                HDebug.Assert(tarr[0] ==  1);
                HDebug.Assert(tarr[1] == -6);
                HDebug.Assert(tarr[2] == -5);
                HDebug.Assert(tarr[3] == -4);
            }

            List<int> arr = new List<int>();
            string[] tokens = str.HSplit(',');
            foreach(var token in tokens)
            {
                string[] ltokens = token.Split(new string[] {"--"}, StringSplitOptions.RemoveEmptyEntries);
                if(ltokens.Length == 1)
                {
                    int idx = int.Parse(ltokens[0]);
                    arr.Add(idx);
                }
                else if(ltokens.Length == 2)
                {
                    int idx0 = int.Parse(ltokens[0]);
                    int idx1 = int.Parse(ltokens[1]);
                    if(idx0 >= idx1) throw new Exception(string.Format("wrong format for HParseIntArr({0}) : {1}", str, token));
                    for(int idx=idx0; idx<=idx1; idx++)
                        arr.Add(idx);
                }
                else
                {
                    throw new Exception(string.Format("wrong format for HParseIntArr({0})", str));
                }
            }
            return arr.ToArray();
        }

        //public static int[]   HParseIntArr_v0(this string str) { return HParseIntArr_v0(str, ',', ' '); }
        //public static int[]   HParseIntArr_v0(this string str, params char[] separator)
        //{
        //    string[] tokens = str.HSplit(separator);
        //    List<int> arr = new List<int>();
        //    foreach(string token in tokens)
        //        arr.Add(HParseInt(token).Value);
        //    return arr.ToArray();
        //}
    }
}
