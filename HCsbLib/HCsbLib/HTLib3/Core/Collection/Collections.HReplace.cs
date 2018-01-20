using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static string[] HReplaceAll(this string[] values, char oldChar, char newChar)
        {
            values = values.HClone<string>();
            for(int i=0; i<values.Length; i++)
                values[i] = values[i].Replace(oldChar, newChar);
            return values;
        }
        public static string[] HReplaceAll(this string[] values, string oldValue, string newValue)
        {
            values = values.HClone<string>();
            for(int i=0; i<values.Length; i++)
                values[i] = values[i].Replace(oldValue, newValue);
            return values;
        }
        public static string[] HReplaceAll(this string[] values, string oldValue, object newValue)
        {
            values = values.HClone<string>();
            for(int i=0; i<values.Length; i++)
                values[i] = values[i].Replace(oldValue, ""+newValue);
            return values;
        }
    }
}
