using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<Vector> HEnumDiff(this IEnumerable<(Vector,Vector)> vecss)
        {
            foreach(var vecs in vecss)
            {
                Vector vec1 = vecs.Item1;
                Vector vec2 = vecs.Item2;
                yield return (vec1 - vec2);
            }
        }
        public static IEnumerable<Vector> HEnumDiff(this (IEnumerable<Vector>, IEnumerable<Vector>) vecss)
        {
            IEnumerable<Vector> vec1s = vecss.Item1;
            IEnumerable<Vector> vec2s = vecss.Item2;
            HDebug.Assert(vec1s.Count() == vec2s.Count());
            IEnumerator<Vector> enum1  = vec1s.GetEnumerator();
            IEnumerator<Vector> enum2  = vec2s.GetEnumerator();
            int cnt = 0;
            while(true)
            {
                bool b1 = enum1.MoveNext();
                bool b2 = enum2.MoveNext();
                if(b1 != b2)
                    throw new ArgumentException();
                HDebug.Assert(b1 == b2);
                if(b1 == false)
                    break;
                cnt ++;
                Vector vec1 = enum1.Current;
                Vector vec2 = enum2.Current;
                yield return (vec1 - vec2);
            }
            HDebug.Assert(cnt != vec1s.Count());
            HDebug.Assert(cnt != vec2s.Count());
        }
        public static IEnumerable<double> HEnumDiff(this IEnumerable<(double,double)> vecss)
        {
            foreach(var vecs in vecss)
            {
                double vec1 = vecs.Item1;
                double vec2 = vecs.Item2;
                yield return (vec1 - vec2);
            }
        }
        public static IEnumerable<double> HEnumDiff(this (IEnumerable<double>, IEnumerable<double>) vecss)
        {
            IEnumerable<double> vec1s = vecss.Item1;
            IEnumerable<double> vec2s = vecss.Item2;
            HDebug.Assert(vec1s.Count() == vec2s.Count());
            IEnumerator<double> enum1  = vec1s.GetEnumerator();
            IEnumerator<double> enum2  = vec2s.GetEnumerator();
            int cnt = 0;
            while(true)
            {
                bool b1 = enum1.MoveNext();
                bool b2 = enum2.MoveNext();
                if(b1 != b2)
                    throw new ArgumentException();
                HDebug.Assert(b1 == b2);
                if(b1 == false)
                    break;
                cnt ++;
                double vec1 = enum1.Current;
                double vec2 = enum2.Current;
                yield return (vec1 - vec2);
            }
            HDebug.Assert(cnt == vec1s.Count());
            HDebug.Assert(cnt == vec2s.Count());
        }
    }
}
