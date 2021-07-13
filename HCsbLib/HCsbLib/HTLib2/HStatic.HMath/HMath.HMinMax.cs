using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class HMath
	{
        public static double HMin(this IMatrix<double> mat)
        {
            HDebug.Assert((mat.ColSize > 0) && (mat.RowSize > 0));
            double min = mat[0, 0];
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    min = Math.Min(min, mat[c, r]);
            return min;
        }
        public static double HMax(this IMatrix<double> mat)
        {
            HDebug.Assert((mat.ColSize > 0) && (mat.RowSize > 0));
            double max = mat[0, 0];
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    max = Math.Max(max, mat[c, r]);
            return max;
        }
        public static int HIdxMax<T>(this IList<T> values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Count > 0);
            int idxmax = 0;
            for(int i=1; i<values.Count; i++)
                if(values[idxmax].CompareTo(values[i]) < 0)
                    idxmax = i;
            return idxmax;
        }
        public static int HIdxMin<T>(this IList<T> values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Count > 0);
            int idxmax = 0;
            for(int i=1; i<values.Count; i++)
                if(values[idxmax].CompareTo(values[i]) > 0)
                    idxmax = i;
            return idxmax;
        }
        public static int[] HIdxMax<T>(this T[,] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            int idxmax0 = 0, idxmax1 = 0;
            for(int i0=0; i0<values.GetLength(0); i0++)
                for(int i1=0; i1<values.GetLength(1); i1++)
                    if(values[idxmax0, idxmax1].CompareTo(values[i0, i1]) < 0)
                    {
                        idxmax0 = i0;
                        idxmax1 = i1;
                    }
            return new int[]{idxmax0, idxmax1};
        }
        public static int[] HIdxMin<T>(this T[,] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            int idxmax0 = 0, idxmax1 = 0;
            for(int i0=0; i0<values.GetLength(0); i0++)
                for(int i1=0; i1<values.GetLength(1); i1++)
                    if(values[idxmax0, idxmax1].CompareTo(values[i0, i1]) > 0)
                    {
                        idxmax0 = i0;
                        idxmax1 = i1;
                    }
            return new int[] { idxmax0, idxmax1 };
        }
        public static int[] HIdxMax<T>(this T[,,] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            int idxmax0 = 0, idxmax1 = 0, idxmax2 = 0;
            for(int i0=0; i0<values.GetLength(0); i0++)
                for(int i1=0; i1<values.GetLength(1); i1++)
                    for(int i2=0; i2<values.GetLength(2); i2++)
                        if(values[idxmax0, idxmax1, idxmax2].CompareTo(values[i0, i1, i2]) < 0)
                        {
                            idxmax0 = i0;
                            idxmax1 = i1;
                            idxmax2 = i2;
                        }
            return new int[] { idxmax0, idxmax1, idxmax2 };
        }
        public static int[] HIdxMin<T>(this T[,,] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            int idxmax0 = 0, idxmax1 = 0, idxmax2 = 0;
            for(int i0=0; i0<values.GetLength(0); i0++)
                for(int i1=0; i1<values.GetLength(1); i1++)
                    for(int i2=0; i2<values.GetLength(2); i2++)
                        if(values[idxmax0, idxmax1, idxmax2].CompareTo(values[i0, i1, i2]) > 0)
                        {
                            idxmax0 = i0;
                            idxmax1 = i1;
                            idxmax2 = i2;
                        }
            return new int[] { idxmax0, idxmax1, idxmax2 };
        }

        public static T HMax<T>(this T[,] values)
            where T : IComparable<T>
		{
            HDebug.Assert(values.Length > 0);
            int[] idxmax = HIdxMax(values);
            HDebug.Assert(idxmax.Length == 2);
            return values[idxmax[0], idxmax[1]];
		}
        public static T HMin<T>(this T[,] values)
            where T : IComparable<T>
		{
            HDebug.Assert(values.Length > 0);
            int[] idxmin = HIdxMin(values);
            HDebug.Assert(idxmin.Length == 2);
            return values[idxmin[0], idxmin[1]];
        }
        public static T HMax<T>(this T[,,] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            int[] idxmax = HIdxMax(values);
            HDebug.Assert(idxmax.Length == 3);
            return values[idxmax[0], idxmax[1], idxmax[2]];
        }
        public static T HMin<T>(this T[,,] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            int[] idxmin = HIdxMin(values);
            HDebug.Assert(idxmin.Length == 3);
            return values[idxmin[0], idxmin[1], idxmin[2]];
        }

        public static T HMaxNth<T>(this IList<T> values, int nth)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Count > 0);
            int[] idxsortd = values.HIdxSorted();
            nth = (values.Count-1) - nth;
            return values[idxsortd[nth]];
        }
        public static T HMinNth<T>(this IList<T> values, int nth)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Count > 0);
            int[] idxsortd = values.HIdxSorted();
            return values[idxsortd[nth]];
        }

        public static T HMax<T>(params T[] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            T max = values[0];
            for(int i=1; i<values.Length; i++)
                if(max.CompareTo(values[i]) < 0)
                    max = values[i];
            return max;
        }
        public static T HMin<T>(params T[] values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Length > 0);
            T min = values[0];
            for(int i=1; i<values.Length; i++)
                if(min.CompareTo(values[i]) > 0)
                    min = values[i];
            return min;
        }

        public static T HMax<T>(this IEnumerable<T> values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Count() > 0);
            T max = values.First();
            foreach(T val in values)
                if(max.CompareTo(val) < 0)
                    max = val;
            return max;
        }
        public static T HMin<T>(this IEnumerable<T> values)
            where T : IComparable<T>
        {
            HDebug.Assert(values.Count() > 0);
            T max = values.First();
            foreach(T val in values)
                if(max.CompareTo(val) > 0)
                    max = val;
            return max;
        }

        public static T? HMin<T>(this IList<T[]    > valuess) where T : struct { T? min = null; foreach(var values in valuess) foreach(dynamic value in values) { if(min == null) min = value; else min = (value < min) ? value : min; } return min; }
        public static T? HMin<T>(this IList<List<T>> valuess) where T : struct { T? min = null; foreach(var values in valuess) foreach(dynamic value in values) { if(min == null) min = value; else min = (value < min) ? value : min; } return min; }
        public static T? HMax<T>(this IList<T[]    > valuess) where T : struct { T? min = null; foreach(var values in valuess) foreach(dynamic value in values) { if(min == null) min = value; else min = (value > min) ? value : min; } return min; }
        public static T? HMax<T>(this IList<List<T>> valuess) where T : struct { T? min = null; foreach(var values in valuess) foreach(dynamic value in values) { if(min == null) min = value; else min = (value > min) ? value : min; } return min; }
    }
}
