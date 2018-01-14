using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        // Dictionary<string, object>
        public static double AsDouble(this object value)
        {
            return (double)value;
        }
        public static MatrixByArr AsMatrixByArr(this object value)
        {
            return value as MatrixByArr;
        }
        public static Vector AsVector(this object value)
        {
            return value as Vector;
        }
        public static Vector[] AsArrayVector(this object value)
        {
            return value as Vector[];
        }
        public static List<double> AsListDouble(this object value)
        {
            return value as List<double>;
        }
        public static List<Vector> AsListVector(this object value)
        {
            return value as List<Vector>;
        }
        public static T AsGet<T>(this object value)
            where T : class
        {
            return value as T;
        }
        public static int DynamicLength(this object value)
        {
            return ((dynamic)value).Length;
        }
        public static object DynamicIndex(this object value, int idx)
        {
            return ((dynamic)value)[idx];
        }
    }
}
