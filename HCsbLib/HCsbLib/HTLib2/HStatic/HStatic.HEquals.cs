using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public static partial class HStatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals(int a, int b)
        {
            return (a == b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualsInt(int a, int b)
        {
            return (a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals(double a, double b)
        {
            return HEqualsDouble(a,b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualsDouble(double a, double b)
        {
            bool a_nan  = double.IsNaN             (a); bool b_nan  = double.IsNaN             (b); if(a_nan  || b_nan ) return (a_nan  == b_nan ); 
            bool a_pinf = double.IsPositiveInfinity(a); bool b_pinf = double.IsPositiveInfinity(b); if(a_pinf || b_pinf) return (a_pinf == b_pinf); 
            bool a_ninf = double.IsNegativeInfinity(a); bool b_ninf = double.IsNegativeInfinity(b); if(a_ninf || b_ninf) return (a_ninf == b_ninf); 
            return (a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals<T1,T2>(ValueTuple<T1,T2> a, ValueTuple<T1,T2> b)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            if(a.Item1.Equals(b.Item1) == false) return false;
            if(a.Item2.Equals(b.Item2) == false) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualsValueTuple<T1,T2>(ValueTuple<T1,T2> a, ValueTuple<T1,T2> b)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            if(a.Item1.Equals(b.Item1) == false) return false;
            if(a.Item2.Equals(b.Item2) == false) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualsValueTuple<T1,T2,T3>(ValueTuple<T1,T2,T3> a, ValueTuple<T1,T2,T3> b)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            if(a.Item1.Equals(b.Item1) == false) return false;
            if(a.Item2.Equals(b.Item2) == false) return false;
            if(a.Item3.Equals(b.Item3) == false) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals(double[] a, double[] b)
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Length != b.Length  ) return false;
            for(int i=0; i<a.Length; i++)
                if(HEquals(a[i], b[i]) == false)
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals<T>(T[] a, T[] b)
            where T : IEquatable<T>
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Length != b.Length  ) return false;
            for(int i=0; i<a.Length; i++)
                if(a[i].Equals(b[i]) == false)
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals<T>(List<T> a, List<T> b)
            where T : IEquatable<T>
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Count != b.Count    ) return false;
            for(int i=0; i<a.Count; i++)
                if(a[i].Equals(b[i]) == false)
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualsEnumerable<T>(IEnumerable<T> a, IEnumerable<T> b)
            where T : IEquatable<T>
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Count() != b.Count()) return false;

            var a_enum = a.GetEnumerator();
            var b_enum = b.GetEnumerator();
            int cnt = 0;
            while(true)
            {
                bool a_move = a_enum.MoveNext();
                bool b_move = b_enum.MoveNext();
                if(a_move != b_move)
                    return false;
                if(a_move == false)
                    break;

                cnt ++;
                if(a_enum.Current.Equals(b_enum.Current) == false)
                    return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualsDictionary<TKey,TValue>(IDictionary<TKey,TValue> a, IDictionary<TKey,TValue> b)
            where TKey   : IEquatable<TKey  >
            where TValue : IEquatable<TValue>
        {
            return HEqualsDictionary( a, b, EqualsKey, EqualsValue );
            static bool EqualsKey  (TKey   a, TKey   b) { return a.Equals(b); }
            static bool EqualsValue(TValue a, TValue b) { return a.Equals(b); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualsDictionary<TKey,TValue>
            ( IDictionary<TKey,TValue> a
            , IDictionary<TKey,TValue> b
            , Func<TKey,TKey,bool> EqualsKey
            , Func<TValue,TValue,bool> EqualsValue
            )
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Count() != b.Count()) return false;

            var a_enum = a.GetEnumerator();
            var b_enum = b.GetEnumerator();
            int cnt = 0;
            while(true)
            {
                bool a_move = a_enum.MoveNext();
                bool b_move = b_enum.MoveNext();
                if(a_move != b_move)
                    return false;
                if(a_move == false)
                    break;

                cnt ++;
                if(EqualsKey  (a_enum.Current.Key  , b_enum.Current.Key  ) == false) return false;
                if(EqualsValue(a_enum.Current.Value, b_enum.Current.Value) == false) return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Obsolete]
        public static bool HEquals<T>(List<T> a, List<T> b, Func<T,T,bool> Equals)
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Count != b.Count    ) return false;
            for(int i=0; i<a.Count; i++)
                if(Equals(a[i], b[i]) == false)
                    return false;
            return true;
        }
    }
}
