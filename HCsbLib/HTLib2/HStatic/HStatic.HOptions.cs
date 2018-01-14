using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static string HOptionsString(this IList<string> options, string prefix)
        {
            string val = null;
            foreach(string option in options)
            {
                if(option.StartsWith(prefix) == false) continue;
                if(val != null) throw new HException("non-unique option for "+prefix);
                val = option.Replace(prefix, "");
            }
            return val;
        }
        public static int    HOptionsInt   (this IList<string> options, string prefix) { return int   .Parse(options.HOptionsString(prefix)); }
        public static double HOptionsDouble(this IList<string> options, string prefix) { return double.Parse(options.HOptionsString(prefix)); }
        public static char   HOptionsChar  (this IList<string> options, string prefix) { return char  .Parse(options.HOptionsString(prefix)); }
        public static bool   HOptionsBool  (this IList<string> options, string prefix) { return bool  .Parse(options.HOptionsString(prefix)); }

        public static object HOptionsCreate(this IList<string> options, string prefix)
        {
            // create an object using AssemblyQualifiedName
            string AssemblyQualifiedName = options.HOptionsString(prefix);
            return HReflection.CreateInstance(AssemblyQualifiedName);
        }
        public static object HOptionsObject(this IList<string> options, string prefix)
        {
            // getting object serialized as string
            string AssemblyQualifiedName = options.HOptionsString(prefix);
            return HReflection.StringToObject(AssemblyQualifiedName);
        }
    }
}
