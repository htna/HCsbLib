using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<int> HEnumIntByString(string str)
        {
            string[] ranges = str.HSplit(',');
            foreach(string range in ranges)
            {
                if(range.Contains("--"))
                {
                    string[] from_to = range.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
                    HDebug.Assert(from_to.Length == 2);
                    int from = from_to[0].HParseInt().Value;
                    int to   = from_to[1].HParseInt().Value;
                    HDebug.Exception(from <= to);
                    for(int i=from; i<=to; i++)
                        yield return i;
                }
                else
                {
                    yield return range.HParseInt().Value;
                }
            }
        }
        public static IEnumerable<int> HEnumCount(int count)
        {
            for(int i=0; i<count; i++)
                yield return i;
        }
        public static IEnumerable<int> HEnumFromTo(int from, int to)
        {
            for(int i=from; i<=to; i++)
                yield return i;
        }
        public static IEnumerable<int>  HEnumFromCount(int from, int count)
        {
            for(int i=0; i<count; i++)
                yield return (from+i);
        }
        public static IEnumerable<Tuple<int, int>> HEnumCount2D(int count1, int count2)
        {
            for(int i1=0; i1<count1; i1++)
                for(int i2=0; i2<count2; i2++)
                    yield return new Tuple<int, int>(i1, i2);
        }
        public static IEnumerable<Tuple<int, int>> HEnumFromTo2D(int from1, int to1, int from2, int to2)
        {
            for(int i1=from1; i1<=to1; i1++)
                for(int i2=from2; i2<=to2; i2++)
                    yield return new Tuple<int, int>(i1, i2);
        }
        public static IEnumerable<Tuple<int, int>> HEnumFromCount2D(int from1, int count1, int from2, int count2)
        {
            int to1 = from1 + count1 - 1;
            int to2 = from2 + count2 - 1;
            return HEnumFromTo2D(from1, to1, from2, to2);
        }
    }
}
