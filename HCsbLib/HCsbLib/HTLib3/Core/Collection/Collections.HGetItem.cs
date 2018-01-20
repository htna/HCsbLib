using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static T HGetItem<T>(this Tuple<T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                default: Debug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this Tuple<T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                default: Debug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this Tuple<T, T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                case 2: return values.Item3;
                default: Debug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this Tuple<T, T, T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                case 2: return values.Item3;
                case 3: return values.Item4;
                default: Debug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this Tuple<T, T, T, T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                case 2: return values.Item3;
                case 3: return values.Item4;
                case 4: return values.Item5;
                default: Debug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
    }
}
