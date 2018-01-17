using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HStatic
    {
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
    }
}
