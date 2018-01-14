using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static string HStrcat(this IEnumerable<string> values)
        {
            StringBuilder str = new StringBuilder();
            foreach(string value in values)
                str.Append(value);
            return str.ToString();
        }
    }
}
