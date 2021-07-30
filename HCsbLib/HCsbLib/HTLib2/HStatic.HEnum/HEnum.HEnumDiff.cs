using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<double> HEnumDiff(this IEnumerable<(double,double)> valss)
        {
            foreach(var vals in valss)
            {
                double val1 = vals.Item1;
                double val2 = vals.Item2;
                yield return (val1 - val2);
            }
        }
        public static IEnumerable<double> HEnumDiff(this (IEnumerable<double>, IEnumerable<double>) valss)
        {
            IEnumerable<double> val1s = valss.Item1;
            IEnumerable<double> val2s = valss.Item2;
            HDebug.Assert(val1s.Count() == val2s.Count());
            IEnumerator<double> enum1  = val1s.GetEnumerator();
            IEnumerator<double> enum2  = val2s.GetEnumerator();
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
                double val1 = enum1.Current;
                double val2 = enum2.Current;
                yield return (val1 - val2);
            }
            HDebug.Assert(cnt == val1s.Count());
            HDebug.Assert(cnt == val2s.Count());
        }
        public static IEnumerable<Vector> HEnumDiff(this IEnumerable<(Vector,Vector)> valss)
        {
            foreach(var vals in valss)
            {
                Vector val1 = vals.Item1;
                Vector val2 = vals.Item2;
                yield return (val1 - val2);
            }
        }
        public static IEnumerable<Vector> HEnumDiff(this (IEnumerable<Vector>, IEnumerable<Vector>) valss)
        {
            IEnumerable<Vector> val1s = valss.Item1;
            IEnumerable<Vector> val2s = valss.Item2;
            HDebug.Assert(val1s.Count() == val2s.Count());
            IEnumerator<Vector> enum1  = val1s.GetEnumerator();
            IEnumerator<Vector> enum2  = val2s.GetEnumerator();
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
                Vector val1 = enum1.Current;
                Vector val2 = enum2.Current;
                yield return (val1 - val2);
            }
            HDebug.Assert(cnt == val1s.Count());
            HDebug.Assert(cnt == val2s.Count());
        }
        public static IEnumerable<MatrixByArr> HEnumDiff(this IEnumerable<(MatrixByArr,MatrixByArr)> valss)
        {
            foreach(var vals in valss)
            {
                MatrixByArr val1 = vals.Item1;
                MatrixByArr val2 = vals.Item2;
                yield return (val1 - val2);
            }
        }
        public static IEnumerable<MatrixByArr> HEnumDiff(this (IEnumerable<MatrixByArr>, IEnumerable<MatrixByArr>) valss)
        {
            IEnumerable<MatrixByArr> val1s = valss.Item1;
            IEnumerable<MatrixByArr> val2s = valss.Item2;
            HDebug.Assert(val1s.Count() == val2s.Count());
            IEnumerator<MatrixByArr> enum1  = val1s.GetEnumerator();
            IEnumerator<MatrixByArr> enum2  = val2s.GetEnumerator();
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
                MatrixByArr val1 = enum1.Current;
                MatrixByArr val2 = enum2.Current;
                yield return (val1 - val2);
            }
            HDebug.Assert(cnt == val1s.Count());
            HDebug.Assert(cnt == val2s.Count());
        }
    }
}
