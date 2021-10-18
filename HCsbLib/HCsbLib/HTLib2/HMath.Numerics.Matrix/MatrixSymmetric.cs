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
        public static MatrixSymmetric<T> ToMatrixSymmetric<T>(this IMatrix<T> mat)
        {
            return MatrixSymmetric<T>.FromMatrix(mat);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualContents<T>(this MatrixSymmetric<T> a, MatrixSymmetric<T> b)
            where T : IEquatable<T>
        {
            if(a.ColSize != b.ColSize) return false;
            for(int c=0; c<a.ColSize; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    T a_val = a[c,r];
                    T b_val = b[c,r];
                    //if(a_val != b_val) return false;
                    if(a_val.Equals(b_val) == false) return false;
                }
            }
            return true;
        }
    }
    public class MatrixSymmetric<T> : IMatrix<T>, IBinarySerializable
    {
        int _size;
        T[][] _arr;
        ///////////////////////////////////////////////////
        // IMatrix<T>
        public int ColSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _size; } }
        public int RowSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _size; } }
        public T this[int  c, int  r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt(c,r);                   } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt(c,r,value);                    } }
        public T this[long c, long r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { throw new NotImplementedException(); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { throw new NotImplementedException(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[,] ToArray()
        {
            T[,] arr = new T[_size,_size];
            for(int c=0; c<_size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    T val = _arr[c][r];
                    arr[c,r] = val;
                    arr[r,c] = val;
                }
            }
            return arr;
        }
        // IMatrix<T>
        ///////////////////////////////////////////////////
        // IBinarySerializable
        public void BinarySerialize(HBinaryWriter writer)
        {
            writer.Write(_size);
            for(int c=0; c<_size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    T val = _arr[c][r];
                    writer.Write(val);
                }
            }
        }
        public MatrixSymmetric(HBinaryReader reader)
        {
            reader.Read(out _size);
            _arr = new T[_size][];
            for(int c=0; c<_size; c++)
            {
                _arr[c] = new T[c+1];
                for(int r=0; r<=c; r++)
                {
                    T val;
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
            _arr = new T[_size][];
            for(int c=0; c<_size; c++)
                _arr[c] = new T[c+1];
        }
        public static MatrixSymmetric<T> Zeros(int size)
        {
            return new MatrixSymmetric<T>(size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetAt(int c, int r)
        {
            if((c < 0) || (c >= _size)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _size)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            HMath.SortDecr(ref c, ref r);
            return _arr[c][r];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAt(int c, int r, T value)
        {
            if((c < 0) || (c >= _size)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _size)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            HMath.SortDecr(ref c, ref r);
            _arr[c][r] = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMatrix<T> ToMatrix<MATRIX>(Func<int,int,MATRIX> Zeros)
            where MATRIX : IMatrix<T>
        {
            IMatrix<T> mat = Zeros(_size, _size);
            for(int c=0; c<_size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    T val = _arr[c][r];
                    mat[c,r] = val;
                    mat[r,c] = val;
                }
            }
            return mat;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixSymmetric<T> FromMatrix<T>(IMatrix<T> mat)
        {
            if(mat.ColSize != mat.RowSize)
                throw new Exception("(mat.ColSize != mat.RowSize)");
            int size = mat.ColSize;
            MatrixSymmetric<T> smat = MatrixSymmetric<T>.Zeros(size);
            for(int c=0; c<size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    dynamic cr_val = mat[c,r];
                    dynamic rc_val = mat[r,c];
                    T val = (cr_val + rc_val)/2;
                    smat[c,r] = val;
                }
            }
            return smat;
        }
    }
}
