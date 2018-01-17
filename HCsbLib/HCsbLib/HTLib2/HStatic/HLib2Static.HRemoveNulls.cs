using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static bool HRemoveAllNull_break = HDebug.IsDebuggerAttached;
        public static IList<T> HRemoveAllNull<T>(this IList<T> values, bool bAssert=true)
                              where T : class
        {
            //HDebug.Depreciated("use HRemoveAll(null), instead");
            HDebug.Break(bAssert && HRemoveAllNull_break);

            Predicate<T> match = delegate(T val)
            {
                return (val == null);
            };

            List<T> _values = new List<T>(values);
            _values.RemoveAll(match);
            return _values;
        }
        public static bool _HRemoveAll_selftest = HDebug.IsDebuggerAttached;
        static T[] _HRemoveAll<T>(this IList<T> values, T toremove)
        {
            if(_HRemoveAll_selftest)
            {
                _HRemoveAll_selftest = false;
                object   objx = "ab--c".Replace("-","");
                object[] tlst0 = new object[] { null, 1, 2, null, 3, 4, null, 1, objx, 3, null, "abc" };
                object[] tlst1 = tlst0._HRemoveAll(null);
                HDebug.Assert(tlst1.Length == 8);
                HDebug.Assert((int   )(tlst1[0]) == 1);
                HDebug.Assert((int   )(tlst1[1]) == 2);
                HDebug.Assert((int   )(tlst1[2]) == 3);
                HDebug.Assert((int   )(tlst1[3]) == 4);
                HDebug.Assert((int   )(tlst1[4]) == 1);
                HDebug.Assert((string)(tlst1[5]) == "abc");
                HDebug.Assert(         tlst1[5]  == objx);
                HDebug.Assert((int   )(tlst1[6]) == 3);
                HDebug.Assert((string)(tlst1[7]) == "abc");
                HDebug.Assert(         tlst1[7]  != objx);

                object[] tlst2 = tlst1._HRemoveAll(objx);
                HDebug.Assert(tlst2.Length == 7);
                HDebug.Assert((int   )(tlst2[0]) == 1);
                HDebug.Assert((int   )(tlst2[1]) == 2);
                HDebug.Assert((int   )(tlst2[2]) == 3);
                HDebug.Assert((int   )(tlst2[3]) == 4);
                HDebug.Assert((int   )(tlst2[4]) == 1);
                HDebug.Assert((int   )(tlst2[5]) == 3);
                HDebug.Assert((string)(tlst2[6]) == "abc");
                HDebug.Assert(         tlst2[6]  != objx);

                int[] tlst3 = (new int[] { 1, 2, 3, 4, 1, 3 })._HRemoveAll(1);
                HDebug.Assert(tlst3.Length == 4);
                HDebug.Assert((int)(tlst3[0]) == 2);
                HDebug.Assert((int)(tlst3[1]) == 3);
                HDebug.Assert((int)(tlst3[2]) == 4);
                HDebug.Assert((int)(tlst3[3]) == 3);
            }

            Predicate<T> match;
            if(toremove == null)
            {
                match = delegate(T val) { return (val == null); };
            }
            else if(toremove is IEquatable<T>)
            {
                IEquatable<T> eqt = toremove as IEquatable<T>;
                match = delegate(T val) { return eqt.Equals(val); };
            }
            else if(typeof(T) == typeof(object))
            {
                match = delegate(T val) { return object.ReferenceEquals(toremove, val); };
            }
            else
            {
                throw new Exception();
            }

            List<T> _values = new List<T>(values);
            _values.RemoveAll(match);
            return _values.ToArray();
        }
        public static T[] HRemoveAll<T>(this IList<T> values, params T[] toremoves)
        {
            if(toremoves == null)
            {
                dynamic toremove = null;
                return _HRemoveAll(values, toremove);
            }
            else
            {
                T[] lvalues = values.ToArray();
                foreach(T toremove in toremoves)
                    lvalues = lvalues._HRemoveAll(toremove);
                return lvalues;
            }
        }
        //public static T[] HRemoveAll<T>(this IList<T> values, params T[] toremoves)
        //    where T : IEquatable<T>
        //{
        //    if(toremoves == null)
        //    {
        //        Predicate<T> match = delegate(T val)
        //        {
        //            return (val == null);
        //        };
        //
        //        List<T> _values = new List<T>(values);
        //        _values.RemoveAll(match);
        //        return _values.ToArray();
        //    }
        //
        //    {
        //        List<T> _values = new List<T>(values);
        //        foreach(T toremove in toremoves)
        //        {
        //            Predicate<T> match = HFunc.EqualTo<T>(toremove);
        //            _values.RemoveAll(match);
        //        }
        //        return _values.ToArray();
        //    }
        //}
        public static T[] HRemoveAllReference<T>(this IList<T> values, params T[] toremoves)
            where T : class
        {
            List<T> _values = new List<T>(values);
            foreach(T toremove in toremoves)
            {
                Predicate<T> match = delegate(T obj) { return object.ReferenceEquals(toremove, obj); };
                _values.RemoveAll(match);
            }
            return _values.ToArray();
        }
        public static string HRemoveAll(this string values, params char[] toremoves)
        {
            string nvalues = values;
            foreach(char toremove in toremoves)
            {
                nvalues = nvalues.Replace(""+toremove, "");
            }
            return nvalues;
        }
        public static string[] HRemoveAllEmptyString(this IList<string> values, bool trim=true)
        {
            List<string> nvalues = new List<string>();
            foreach(var value in values)
            {
                if(value == null) continue;
                if(value.Trim().Length == 0) continue;
                nvalues.Add(value);
            }
            return nvalues.ToArray();
        }
        //public static int HRemoveAll<T>(this List<string> valus, T toremove)
        //    where T : IEquatable<T>
        //{
        //    Predicate<string> match = delegate(string value)
        //    {
        //        return toremove.Equals(value);
        //    };
        //    return valus.RemoveAll(match);
        //}
    }
}
