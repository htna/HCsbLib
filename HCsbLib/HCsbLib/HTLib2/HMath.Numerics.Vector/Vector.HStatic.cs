using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HStatic
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Dist(this (Vector, Vector) pt1_pt2)
        {
            return Math.Sqrt(Dist2(pt1_pt2));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Dist2(this (Vector, Vector) pt1_pt2)
        {
            Vector pt1 = pt1_pt2.Item1;
            Vector pt2 = pt1_pt2.Item2;

            if(pt1.Size != pt2.Size)
                throw new Exception();

            double dist2 = 0;
            for(int i=0; i<pt1.Size; i++)
            {
                double disti = pt1[i] - pt2[i];
                dist2 += disti * disti;
            }

            return dist2;
        }

        public static double[] HToArrayOfIndex(this IList<Vector> vecs, int idx)
        {
            int count = vecs.Count;
            double[] arr = new double[count];
            for(int i = 0; i < count; i++)
                arr[i] = vecs[i][idx];
            return arr;
        }
        public static double[][] HToArrayArray(this IList<Vector> vecs)
        {
            int count = vecs.Count;
            double[][] arrarr = new double[count][];
            for(int i=0; i<count; i++)
                arrarr[i] = vecs[i].ToArray().HClone();
            return arrarr;
        }
        public static Vector[] HToVectorArray(this double[][] arrarr)
        {
            int count = arrarr.Length;
            Vector[] vecs = new Vector[count];
            for(int i=0; i<count; i++)
                vecs[i] = arrarr[i].HClone();
            return vecs;
        }
        public static Vector[] HCloneVectors(this IList<Vector> vectors)
        {
            Vector[] clone = new Vector[vectors.Count];
            for(int i=0; i<vectors.Count; i++)
                clone[i] = (vectors[i] == null) ? null : vectors[i].Clone();
            return clone;
        }
        public static void HVectorSetValue(this IList<Vector> vecs, double value)
        {
            foreach(Vector vec in vecs)
                vec.SetValue(value);
        }
        public static void HToString
            ( this IVector<double> vec
            , StringBuilder sb
            , string format="0.00000"
            , IFormatProvider formatProvider=null
            , string begindelim  = "{"
            , string enddelim    = "}"
            , string delim       = ", "
            )
        {
            sb.Append(begindelim);

            int tsize = Math.Min(vec.Size, 100);

            for(int i=0; i<tsize; i++)
            {
                if(i != 0) sb.Append(delim);
                sb.Append(vec[i].ToString(format));
            }
            if(tsize != vec.Size)
                sb.Append(", ...");

            sb.Append(enddelim);
        }
    }
}
