using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T HGetItem<T>(this Tuple<T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this Tuple<T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this Tuple<T, T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                case 2: return values.Item3;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
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
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
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
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }

        public static T HGetItem<T>(this ValueTuple<T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this ValueTuple<T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this ValueTuple<T, T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                case 2: return values.Item3;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this ValueTuple<T, T, T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                case 2: return values.Item3;
                case 3: return values.Item4;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
        public static T HGetItem<T>(this ValueTuple<T, T, T, T, T> values, int idx)
        {
            switch(idx)
            {
                case 0: return values.Item1;
                case 1: return values.Item2;
                case 2: return values.Item3;
                case 3: return values.Item4;
                case 4: return values.Item5;
                default: HDebug.Assert(false); throw new IndexOutOfRangeException();
            }
        }
    }
}
