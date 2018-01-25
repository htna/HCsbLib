using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        // Dictionary<string, object>
        [Obsolete("Do not use this function")]
        public static double AsDouble(this object value)
        {
            return (double)value;
        }
        [Obsolete("Do not use this function")]
        public static MatrixByArr AsMatrixByArr(this object value)
        {
            return value as MatrixByArr;
        }
        [Obsolete("Do not use this function")]
        public static Vector AsVector(this object value)
        {
            return value as Vector;
        }
        [Obsolete("Do not use this function")]
        public static Vector[] AsArrayVector(this object value)
        {
            return value as Vector[];
        }
        [Obsolete("Do not use this function")]
        public static List<double> AsListDouble(this object value)
        {
            return value as List<double>;
        }
        [Obsolete("Do not use this function")]
        public static List<Vector> AsListVector(this object value)
        {
            return value as List<Vector>;
        }
        [Obsolete("Do not use this function")]
        public static T AsGet<T>(this object value)
            where T : class
        {
            return value as T;
        }
        [Obsolete("Do not use this function")]
        public static int DynamicLength(this object value)
        {
            return ((dynamic)value).Length;
        }
        [Obsolete("Do not use this function")]
        public static object DynamicIndex(this object value, int idx)
        {
            return ((dynamic)value)[idx];
        }
    }
}
