using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] _HUpdateValueAll<T>(this T[] arr, T val)
        {
            for(int i=0; i<arr.Length; i++)
                arr[i] = val;
            return arr;
        }
        public static T[] HUpdateValueAll<T>(this T[] arr, T val)
        {
            for(int i=0; i<arr.Length; i++)
                arr[i] = val;
            return arr;
        }
        public static T[,] HUpdateValueAll<T>(this T[,] arr, T val)
        {
            for(int c=0; c<arr.GetLength(0); c++)
                for(int r=0; r<arr.GetLength(1); r++)
                    arr[c, r] = val;
            return arr;
        }
    }
}
