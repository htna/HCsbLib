using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Matrix : Matrix<double>
    {
        public static Matrix operator+(Matrix val1, Matrix val2)
        {
            int ColSize = val1.ColSize;     Debug.Assert(ColSize == val2.ColSize);
            int RowSize = val1.RowSize;     Debug.Assert(RowSize == val2.RowSize);
            int Length  = val1.data.Length; Debug.Assert(Length  == val2.data.Length);
            Matrix ret = new Matrix(ColSize, RowSize);

            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    ret[c, r] = val1[c, r] + val2[c, r];

            return ret;
        }

        public static Matrix operator-(Matrix val1, Matrix val2)
        {
            int ColSize = val1.ColSize;     Debug.Assert(ColSize == val2.ColSize);
            int RowSize = val1.RowSize;     Debug.Assert(RowSize == val2.RowSize);
            int Length  = val1.data.Length; Debug.Assert(Length  == val2.data.Length);
            Matrix ret = new Matrix(ColSize, RowSize);

            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    ret[c, r] = val1[c, r] - val2[c, r];

            return ret;
        }

        public static Matrix operator*(Matrix val1, double val2)
        {
            Matrix ret = new Matrix(val1.ColSize, val1.RowSize);
            for(int c=0; c<ret.ColSize; c++)
                for(int r=0; r<ret.RowSize; r++)
                    ret[c, r] = val1[c, r] * val2;
            return ret;
        }
        public static Matrix operator*(double val1, Matrix val2)
        {
            return (val2*val1);
        }

        public static Matrix operator/(Matrix val1, double val2)
        {
            Matrix ret = new Matrix(val1.ColSize, val1.RowSize);
            for(int c=0; c<ret.ColSize; c++)
                for(int r=0; r<ret.RowSize; r++)
                    ret[c, r] = val1[c, r] / val2;
            return ret;
        }

        public static HFunc1<IList<Matrix>, Matrix, bool> funcMul = null;
        public static Matrix operator*(Matrix val1, Matrix val2)
        {
            Matrix mul;
            bool succ = Mul(new Matrix[]{ val1, val2 }, out mul);
            Debug.Assert(succ);
            return mul;
        }
        public static bool Mul(IList<Matrix> vals, out Matrix mul)
        {
            if(funcMul != null) return funcMul(vals, out mul);
            mul = null;
            return false;
        }

        //public static bool operator>(Matrix val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i > val2) == false)
        //            return false;
        //    return true;
        //}
        //public static bool operator>=(Matrix val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i >= val2) == false)
        //            return false;
        //    return true;
        //}
        //public static bool operator<(Matrix val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i < val2) == false)
        //            return false;
        //    return true;
        //}
        //public static bool operator<=(Matrix val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i <= val2) == false)
        //            return false;
        //    return true;
        //}
    }
}
