using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static MatrixSymmetric ToMatrixSymmetric(this Matrix mat)
        {
            return MatrixSymmetric.FromMatrix(mat);
        }
    }
    public class MatrixSymmetric : IMatrix<double>, IBinarySerializable
    {
        int _size;
        double[][] _arr;
        ///////////////////////////////////////////////////
        // IMatrix<double>
        public int ColSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _size; } }
        public int RowSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _size; } }
        public double this[int  c, int  r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt(c,r);                   } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt(c,r,value);                    } }
        public double this[long c, long r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { throw new NotImplementedException(); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { throw new NotImplementedException(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double[,] ToArray()
        {
            double[,] arr = new double[_size,_size];
            for(int c=0; c<_size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double val = _arr[c][r];
                    arr[c,r] = val;
                    arr[r,c] = val;
                }
            }
            return arr;
        }
        // IMatrix<double>
        ///////////////////////////////////////////////////
        // IBinarySerializable
        public void BinarySerialize(HBinaryWriter writer)
        {
            writer.Write(_size);
            for(int c=0; c<_size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double val = _arr[c][r];
                    writer.Write(val);
                }
            }
        }
        public MatrixSymmetric(HBinaryReader reader)
        {
            reader.Read(out _size);
            _arr = new double[_size][];
            for(int c=0; c<_size; c++)
            {
                _arr[c] = new double[c+1];
                for(int r=0; r<=c; r++)
                {
                    double val;
                    reader.Read(out val);
                    _arr[c][r] = val;
                }
            }
        }
        // IBinarySerializable
        ///////////////////////////////////////////////////

        public MatrixSymmetric(int size)
        {
            _size = size;
            _arr = new double[_size][];
            for(int c=0; c<_size; c++)
                _arr[c] = new double[c+1];
        }
        public static MatrixSymmetric Zeros(int size)
        {
            return new MatrixSymmetric(size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetAt(int c, int r)
        {
            if((c < 0) || (c >= _size)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _size)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            HMath.SortDecr(ref c, ref r);
            return _arr[c][r];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAt(int c, int r, double value)
        {
            if((c < 0) || (c >= _size)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _size)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            HMath.SortDecr(ref c, ref r);
            _arr[c][r] = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix ToMatrix()
        {
            Matrix mat = Matrix.Zeros(_size, _size);
            for(int c=0; c<_size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double val = _arr[c][r];
                    mat[c,r] = val;
                    mat[r,c] = val;
                }
            }
            return mat;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixSymmetric FromMatrix(Matrix mat)
        {
            if(mat.ColSize != mat.RowSize)
                throw new Exception("(mat.ColSize != mat.RowSize)");
            int size = mat.ColSize;
            MatrixSymmetric smat = MatrixSymmetric.Zeros(size);
            for(int c=0; c<size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double val = (mat[c,r] + mat[r,c])/2;
                    smat[c,r] = val;
                }
            }
            return smat;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSymmetric(Matrix mat, double tol)
        {
            if(mat.ColSize != mat.RowSize) return false;
            int size = mat.ColSize;
            for(int c=0; c<size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double cr_val = mat[c,r];
                    double rc_val = mat[r,c];
                    if(Math.Abs(cr_val - rc_val) > tol)
                        return false;
                }
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualContents(MatrixSymmetric a, MatrixSymmetric b)
        {
            if(a._size != b._size) return false;
            for(int c=0; c<a._size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double a_val = a[c,r];
                    double b_val = b[c,r];
                    if(a_val != b_val) return false;
                }
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualContents(Matrix a, Matrix b)
        {
            if(a.ColSize != b.ColSize) return false;
            if(a.RowSize != b.RowSize) return false;
            for(int c=0; c<a.ColSize; c++)
            {
                for(int r=0; r<a.RowSize; r++)
                {
                    double a_val = a[c,r];
                    double b_val = b[c,r];
                    if(a_val != b_val) return false;
                }
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualContents(Matrix a, Matrix b, double tol)
        {
            if(a.ColSize != b.ColSize) return false;
            if(a.RowSize != b.RowSize) return false;
            for(int c=0; c<a.ColSize; c++)
            {
                for(int r=0; r<a.RowSize; r++)
                {
                    double a_val = a[c,r];
                    double b_val = b[c,r];
                    if(Math.Abs(a_val - b_val) > tol) return false;
                }
            }
            return true;
        }
    }
}
