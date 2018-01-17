using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static string HSubEndStringCount(this string str, int length)
        {
            if(str.Length <= length)
                return str;
            str = str.Substring(str.Length-length);
            HDebug.Assert(str.Length == length);
            return str;
        }
        static bool HSubEndStringFrom_selftest = HDebug.IsDebuggerAttached;
        public static string HSubEndStringFrom(this string str, int lastIdxFrom)
        {
            if(HSubEndStringFrom_selftest)
            {
                HSubEndStringFrom_selftest = false;
                HDebug.Assert("abcdef".HSubEndStringFrom(1) == "abcde" );
                HDebug.Assert("abcdef".HSubEndStringFrom(0) == "abcdef");
            }

            int leng  = str.Length;
            int from  = 0;
            int count = leng - lastIdxFrom;
            string substr = str.Substring(from, count);
            return substr;
        }
        public static string HSubEndStringFromCount(this string str, int from, int length)
        {
            HDebug.ToDo();
            throw new Exception();
            //if(str.Length <= length)
            //    return str;
            //str = str.Substring(str.Length-length);
            //HDebug.Assert(str.Length == length);
            //return str;
        }
    }
}
