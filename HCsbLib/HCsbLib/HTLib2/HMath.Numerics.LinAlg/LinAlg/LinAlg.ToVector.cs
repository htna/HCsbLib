using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
        public static Vector[] GetColVectorList(this IMatrix<double> _this)
        {
            Vector[] vecs = new Vector[_this.RowSize];
            for(int r=0; r<_this.RowSize; r++)
                vecs[r] = _this.GetColVector(r);
            return vecs;
        }
        public static Vector[] GetRowVectorList(this IMatrix<double> _this)
        {
            Vector[] vecs = new Vector[_this.ColSize];
            for(int c=0; c<_this.ColSize; c++)
                vecs[c] = _this.GetRowVector(c);
            return vecs;
        }
        public static Vector[] GetColVectorList(this double[,] _this)
        {
            Matrix mat = _this;
            return mat.GetColVectorList();
        }
        public static Vector[] GetRowVectorList(this double[,] _this)
        {
            Matrix mat = _this;
            return mat.GetRowVectorList();
        }

        static bool selftest_ToColVector = HDebug.IsDebuggerAttached;
        public static Vector HReshapeToColVector(this IMatrix<double> _this)
        {
            if(HDebug.IsDebuggerAttached && selftest_ToColVector)
            #region self-test
            {
                selftest_ToColVector = false;
                MatrixByArr mat = new double[,] { { 1, 2 }, { 3, 4 } };
                Vector colvec0 = new double[] { 1, 3, 2, 4 };
                Vector colvec1 = mat.HReshapeToColVector();
                HDebug.AssertAnd(colvec0 == colvec1);
            }
            #endregion
            Vector vec = new double[_this.ColSize * _this.RowSize];
            for(int c=0; c<_this.ColSize; c++)
                for(int r=0; r<_this.RowSize; r++)
                {
                    vec[c+r*_this.ColSize] = _this[c, r];
                }
            return vec;
        }

        static bool selftest_ToVector = HDebug.IsDebuggerAttached;
        public static Vector HToVector(this IMatrix<double> _this)
        {
            if(HDebug.IsDebuggerAttached && selftest_ToVector)
            #region self-test
            {
                selftest_ToVector = false;
                Vector vec = new double[2] { 1, 2 };
                MatrixByArr mat1 = new double[1, 2] { { 1, 2 } };
                MatrixByArr mat2 = new double[2, 1] { { 1 }, { 2 } };
                HDebug.Assert(mat1.HToVector() == vec);
                HDebug.Assert(mat2.HToVector() == vec);
            }
            #endregion
            if(_this.RowSize == 1)
            {
                Vector vec = new double[_this.ColSize];
                for(int i=0; i<vec.Size; i++)
                    vec[i] = _this[i, 0];
                return vec;
            }
            if(_this.ColSize == 1)
            {
                Vector vec = new double[_this.RowSize];
                for(int i=0; i<vec.Size; i++)
                    vec[i] = _this[0, i];
                return vec;
            }
            HDebug.Assert(false);
            return null;
        }
    }
}
