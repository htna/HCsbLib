using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Linq;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static string HToString(this double value, string format, int sizepadding)
        {
            string str = value.ToString(format);
            if(str.Length > sizepadding)
                return str;
            str = ("                                                       " + str);
            str = str.Substring(str.Length-sizepadding);
            HDebug.Assert(str.Length == sizepadding);
            return str;
        }
        public static bool HToStringChars_selftest = HDebug.IsDebuggerAttached;
        public static string HToString(this IList<char> chars)
        {
            if(HToStringChars_selftest)
            {
                HToStringChars_selftest = false;

                string str0="";
                foreach(var ch in chars)
                    str0 += ch;
                string str1 = HToString(chars);
                HDebug.Exception(str0 == str1);
            }
            return new String(chars.ToArray());
        }
	}
}
