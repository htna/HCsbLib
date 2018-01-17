using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class HLib2Static
    {
        public static TMatrix<T> HToMatrixT<T>(this T[,] val)
        {
            return new TMatrix<T> { data = val };
        }
        public static TMatrix<T> HToMatrixT<T>(this IList<Tuple<T,T>> val)
        {
            return new TMatrix<T> { data = val.HToArray() };
        }
        public static TMatrix<T> HToMatrixT<T>(this IList<T[]> val)
        {
            int colsize = val.Count;
            int rowsize = val[0].Length;
            T[,] data = new T[colsize, rowsize];
            for(int c=0; c<colsize; c++)
            {
                if(val[c].Length != rowsize)
                    return null;
                for(int r=0; r<rowsize; r++)
                    data[c, r] = val[c][r];
            }
            return new TMatrix<T> { data = data };
        }
    }
}
