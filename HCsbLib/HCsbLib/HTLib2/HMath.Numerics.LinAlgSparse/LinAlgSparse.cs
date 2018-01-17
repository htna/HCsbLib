using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public static partial class LinAlgSparse
    {
        public static MatrixSparse<T> ToColMatrix<T>(VectorSparse<T> vec)
        {
            MatrixSparse<T> mat = new MatrixSparse<T>(vec.Size, 1, GetDefault: vec.GetDefault);
            foreach(var i_val in vec.EnumElements())
            {
                int i   = i_val.Item1;
                var val = i_val.Item2;
                mat[i, 0] = val;
            }
            return mat;
        }

        public static double[,] ToArray(MatrixSparse<MatrixByArr> mat)
        {
            return MatrixByArr.FromMatrixArray(mat.ToArray());
        }
        public static double[,] ToArray(VectorSparse<MatrixByArr> vec)
        {
            return MatrixByArr.FromMatrixArray(ToColMatrix(vec).ToArray());
        }

        public static MatrixByArr VtV(VectorSparse<MatrixByArr> l, VectorSparse<MatrixByArr> r)
        {
            HDebug.Assert(l.Size == r.Size);

            IList<int> l_idx = new List<int>(l.EnumIndex());
            IList<int> r_idx = new List<int>(r.EnumIndex());
            IList<int> idxs = l_idx.HListCommonT(r_idx);

            MatrixByArr val = l.GetDefault();
            foreach(var idx in idxs)
            {
                val += l[idx].Tr() * r[idx];
            }

            if(HDebug.IsDebuggerAttached)
            {
                MatrixByArr tl = ToArray(l);
                MatrixByArr tr = ToArray(r);
                MatrixByArr tval = tl.Tr() * tr;
                HDebug.AssertTolerance(0.00000001, val-tval);
            }
            return val;
        }
        public static MatrixSparse<MatrixByArr> VVt(VectorSparse<MatrixByArr> v0, VectorSparse<MatrixByArr> v1)
        {
            MatrixSparse<MatrixByArr> mat = new MatrixSparse<MatrixByArr>(v0.Size, v1.Size, GetDefault: v0.GetDefault);

            foreach(var i1_val1 in v1.EnumElements())
            {
                int      i1  = i1_val1.Item1;
                MatrixByArr val1t = i1_val1.Item2.Tr();
                foreach(var i0_val0 in v0.EnumElements())
                {
                    int      i0 = i0_val0.Item1;
                    MatrixByArr val0 = i0_val0.Item2;
                    mat[i0, i1] = val0 * val1t;
                }
            }

            if(HDebug.IsDebuggerAttached)
            {
                MatrixByArr tv0 = ToArray(v0);
                MatrixByArr tv1 = ToArray(v1);
                MatrixByArr tmat = tv0 * tv1.Tr();
                HDebug.AssertTolerance(0.00000001, ToArray(mat)-tmat);
            }
            return mat;
        }
        public static MatrixSparse<MatrixByArr> Sum_M_Mt(MatrixSparse<MatrixByArr> mat)
        {
            HDebug.Assert(mat.ColSize == mat.RowSize);
            MatrixSparse<MatrixByArr> result = new MatrixSparse<MatrixByArr>(mat.ColSize, mat.RowSize, GetDefault:mat.GetDefault);
            foreach(var c_r_val in mat.EnumElements())
            {
                int c = c_r_val.Item1;
                int r = c_r_val.Item2;
                var val = c_r_val.Item3;
                result[c, r] += val;
                result[r, c] += val.Tr();
            }
            return result;
        }
        public static VectorSparse<MatrixByArr> MV(MatrixSparse<MatrixByArr> lmat, VectorSparse<MatrixByArr> rvec)
        {
            HDebug.Assert(lmat.RowSize == rvec.Size);
            VectorSparse<MatrixByArr> result = new VectorSparse<MatrixByArr>(lmat.ColSize, lmat.GetDefault);

            foreach(var c_r_val in lmat.EnumElements())
            {
                int c = c_r_val.Item1;
                int r = c_r_val.Item2;
                MatrixByArr mat_cr = c_r_val.Item3;
                if(rvec.HasElement(r) == false)
                    continue;
                MatrixByArr vec_r = rvec[r];
                result[c] += mat_cr * vec_r;
            }

            if(HDebug.IsDebuggerAttached)
            {
                MatrixByArr tlmat = ToArray(lmat);
                MatrixByArr trvec = ToArray(rvec);
                MatrixByArr tmat = tlmat * trvec;
                HDebug.AssertTolerance(0.00000001, ToArray(result)-tmat);
            }
            return result;
        }
        public static VectorSparse<MatrixByArr> Mul(VectorSparse<MatrixByArr> lvec, MatrixByArr rval)
        {
            VectorSparse<MatrixByArr> mul = new VectorSparse<MatrixByArr>(lvec.Size, lvec.GetDefault);
            foreach(var i_val in lvec.EnumElements())
            {
                int i = i_val.Item1;
                MatrixByArr val = i_val.Item2;
                mul[i] = val*rval;
            }

            if(HDebug.IsDebuggerAttached)
            {
                MatrixByArr tlvec = ToArray(lvec);
                MatrixByArr tmat = tlvec * rval;
                HDebug.AssertTolerance(0.00000001, ToArray(mul)-tmat);
            }
            return mul;
        }
        public static VectorSparse<MatrixByArr> Mul(MatrixByArr lval, VectorSparse<MatrixByArr> rvec)
        {
            VectorSparse<MatrixByArr> mul = new VectorSparse<MatrixByArr>(rvec.Size, rvec.GetDefault);
            foreach(var i_val in rvec.EnumElements())
            {
                int i = i_val.Item1;
                MatrixByArr val = i_val.Item2;
                mul[i] = val*lval;
            }

            if(HDebug.IsDebuggerAttached)
            {
                MatrixByArr trvec = ToArray(rvec);
                MatrixByArr tmat = lval * trvec;
                HDebug.AssertTolerance(0.00000001, ToArray(mul)-tmat);
            }
            return mul;
        }



        public static VectorSparse<MatrixByArr> Sum(double s1, VectorSparse<MatrixByArr> m1, double s2, VectorSparse<MatrixByArr> m2)
        {
            return Sum(new Tuple<double, VectorSparse<MatrixByArr>>[]{
                new Tuple<double,VectorSparse<MatrixByArr>>(s1, m1),
                new Tuple<double,VectorSparse<MatrixByArr>>(s2, m2),
            });
        }
        public static VectorSparse<MatrixByArr> Sum(double s1, VectorSparse<MatrixByArr> m1, double s2, VectorSparse<MatrixByArr> m2, double s3, VectorSparse<MatrixByArr> m3)
        {
            return Sum(new Tuple<double, VectorSparse<MatrixByArr>>[]{
                new Tuple<double,VectorSparse<MatrixByArr>>(s1, m1),
                new Tuple<double,VectorSparse<MatrixByArr>>(s2, m2),
                new Tuple<double,VectorSparse<MatrixByArr>>(s3, m3),
            });
        }
        public static VectorSparse<MatrixByArr> Sum(params Tuple<double, VectorSparse<MatrixByArr>>[] lst_mat_scale)
        {
            int size       = lst_mat_scale[0].Item2.Size;
            var GetDefault = lst_mat_scale[0].Item2.GetDefault;
            VectorSparse<MatrixByArr> sum = new VectorSparse<MatrixByArr>(size, GetDefault);

            foreach(var mat_scale in lst_mat_scale)
            {
                HDebug.Assert(size == mat_scale.Item2.Size);
                double scale = mat_scale.Item1;
                foreach(var i_val in mat_scale.Item2.EnumElements())
                {
                    int i   = i_val.Item1;
                    var val = i_val.Item2;
                    sum[i] += val * scale;
                }
            }

            return sum;
        }
        public static MatrixSparse<MatrixByArr> Sum(double s1, MatrixSparse<MatrixByArr> m1, double s2, MatrixSparse<MatrixByArr> m2)
        {
            return Sum(new Tuple<double, MatrixSparse<MatrixByArr>>[]{
                new Tuple<double,MatrixSparse<MatrixByArr>>(s1, m1),
                new Tuple<double,MatrixSparse<MatrixByArr>>(s2, m2),
            });
        }
        public static MatrixSparse<MatrixByArr> Sum(double s1, MatrixSparse<MatrixByArr> m1, double s2, MatrixSparse<MatrixByArr> m2, double s3, MatrixSparse<MatrixByArr> m3)
        {
            return Sum(new Tuple<double,MatrixSparse<MatrixByArr>>[]{
                new Tuple<double,MatrixSparse<MatrixByArr>>(s1, m1),
                new Tuple<double,MatrixSparse<MatrixByArr>>(s2, m2),
                new Tuple<double,MatrixSparse<MatrixByArr>>(s3, m3),
            });
        }
        public static MatrixSparse<MatrixByArr> Sum(params Tuple<double,MatrixSparse<MatrixByArr>>[] lst_mat_scale)
        {
            int colsize    = lst_mat_scale[0].Item2.ColSize;
            int rowsize    = lst_mat_scale[0].Item2.RowSize;
            var GetDefault = lst_mat_scale[0].Item2.GetDefault;
            MatrixSparse<MatrixByArr> sum = new MatrixSparse<MatrixByArr>(colsize, rowsize, GetDefault);

            foreach(var mat_scale in lst_mat_scale)
            {
                HDebug.Assert(colsize == mat_scale.Item2.ColSize);
                HDebug.Assert(rowsize == mat_scale.Item2.RowSize);
                double scale = mat_scale.Item1;
                foreach(var c_r_val in mat_scale.Item2.EnumElements())
                {
                    int c   = c_r_val.Item1;
                    int r   = c_r_val.Item2;
                    var val = c_r_val.Item3;
                    sum[c, r] += val * scale;
                }
            }

            return sum;
        }
    }
}
