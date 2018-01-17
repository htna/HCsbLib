using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static ReadOnlyCollection<T2> HToType<T1, T2>(this ReadOnlyCollection<T1> values)
        {
            return values.ToList().HToType<T1, T2>().AsReadOnly();
        }
        public static List<T2> HToType<T1, T2>(this List<T1> values)
        {
            if(values == null)
                return null;
            List<T2> values2 = new List<T2>();
            foreach(dynamic value in values)
            {
                T2 value2 = (T2)value;
                if(value != null) HDebug.Assert(value2 != null);
                if(value == null) HDebug.Assert(value2 == null);
                values2.Add(value2);
            }
            return values2;
        }
        public static T2[] HToType<T1,T2>(this T1[] values, T2[] totype)
        {
            return HToType<T1, T2>(values);
        }
        public static T2[] HToType<T1,T2>(this T1[] values)
        {
            if(values == null)
                return null;
            T2[] values2 = new T2[values.Length];
            for(int i=0; i<values.Length; i++)
            {
                dynamic value  = values[i];
                T2 value2 = (T2)value;
                if(value != null) HDebug.Assert(value2 != null);
                if(value == null) HDebug.Assert(value2 == null);
                values2[i] = value2;
            }
            return values2;
        }
        public static T2[,] HToType<T1, T2>(this T1[,] values)
        {
            if(values == null)
                return null;
            T2[,] values2 = new T2[values.GetLength(0), values.GetLength(1)];
            for(int i0=0; i0<values.GetLength(0); i0++)
            {
                for(int i1=0; i1<values.GetLength(1); i1++)
                {
                    dynamic value  = values[i0,i1];
                    T2 value2 = (T2)value;
                    if(value != null) HDebug.Assert(value2 != null);
                    if(value == null) HDebug.Assert(value2 == null);
                    values2[i0,i1] = value2;
                }
            }
            return values2;
        }
        public static List<List<T2>> HToType<T1, T2>(this List<List<T1>> valuess)
        {
            if(valuess == null)
                return null;
            List<List<T2>> valuess2 = new List<List<T2>>();
            foreach(var values in valuess)
            {
                valuess2.Add(values.HToType<T1, T2>());
            }
            return valuess2;
        }
        public static Dictionary<KEY2, VAL2> HToType<KEY1, VAL1, KEY2, VAL2>(this Dictionary<KEY1, VAL1> values)
        {
            if(values == null)
                return null;
            Dictionary<KEY2, VAL2> values2 = new Dictionary<KEY2, VAL2>();
            foreach(dynamic key in values.Keys)
            {
                dynamic val = values[key];
                KEY2 key2 = (KEY2)key;
                VAL2 val2 = (VAL2)val;
                values2.Add(key2, val2);
            }
            return values2;
        }
    }
}
