using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] NewArray<T>(int size)
            where T : new()
        {
            T[] arr = new T[size];
            for(int i=0; i<size; i++)
                arr[i] = new T();
            return arr;
        }
        public static void NewArray<T>(out T[] arr, int size)
            where T : new()
        {
            arr = new T[size];
            for(int i=0; i<size; i++)
                arr[i] = new T();
        }
    }
}
