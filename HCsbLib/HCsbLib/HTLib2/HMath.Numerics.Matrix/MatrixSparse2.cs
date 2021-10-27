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
        public static MatrixSparse2 ToMatrixSparse2(this Matrix mat)
        {
            return MatrixSparse2.FromMatrix(mat);
        }
    }
    public class MatrixSparse2 : IMatrix<double>, IMatrixSparse<double>, IBinarySerializable
    {
        int _colsize;
        int _rowsize;
        Dictionary<int,double>[] _c_r_value;
        ///////////////////////////////////////////////////
        // IMatrix<double>
        public int ColSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _colsize; } }
        public int RowSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _rowsize; } }
        public double this[int  c, int  r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt(c,r);                   } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt(c,r,value);                    } }
        public double this[long c, long r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { throw new NotImplementedException(); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { throw new NotImplementedException(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public double[,] ToArray()
        {
            double[,] arr = new double[_colsize,_rowsize];
            for(int c=0; c<_colsize; c++)
            {
                Dictionary<int, double> r_value = _c_r_value[c];
                foreach(var item in r_value)
                {
                    int r = item.Key;
                    double val = item.Value;
                    arr[c,r] = val;
                }
            }
            return arr;
        }
        // IMatrix<double>
        ///////////////////////////////////////////////////
        // IEnumerable<ValueTuple<int, int, T>> EnumElements();
        public IEnumerable<ValueTuple<int, int, double>> EnumElements()
        {
            foreach(var c_r_val in EnumNonZeros())
                yield return c_r_val;
        }
        // IEnumerable<ValueTuple<int, int, T>> EnumElements();
        ///////////////////////////////////////////////////
        // IBinarySerializable
        public void BinarySerialize(HBinaryWriter writer)
        {
            writer.Write(_colsize);
            writer.Write(_rowsize);
            int count = 0;
            for(int c=0; c<_colsize; c++)
            {
                Dictionary<int, double> r_value = _c_r_value[c];
                writer.Write(r_value.Count);
                foreach(var item in r_value)
                {
                    int    r   = item.Key;
                    double val = item.Value;
                    writer.Write(r  );
                    writer.Write(val);
                    count ++;
                }
            }

            if(count != NumNonZero)
                throw new Exception("(count != NumNonZero)");
        }
        public MatrixSparse2(HBinaryReader reader)
        {
            reader.Read(out _colsize);
            reader.Read(out _rowsize);
            int count = 0;
            _c_r_value = new Dictionary<int, double>[_colsize];
            for(int c=0; c<_colsize; c++)
            {
                int c_count;
                reader.Read(out c_count);
                _c_r_value[c] = new Dictionary<int, double>(c_count);
                Dictionary<int, double> r_value = _c_r_value[c];
                foreach(var item in r_value)
                {
                    int    r  ; reader.Read(out r  ); HDebug.Assert(r < _rowsize);
                    double val; reader.Read(out val);
                    r_value.Add(r, val);
                    count ++;
                }
            }

            if(count != NumNonZero)
                throw new Exception("(count != NumNonZero)");
        }
        // IBinarySerializable
        ///////////////////////////////////////////////////

        public MatrixSparse2(int colsize, int rowsize)
        {
            _colsize   = colsize;
            _rowsize   = rowsize;
            _c_r_value = new Dictionary<int, double>[_colsize];
            for(int c=0; c<_colsize; c++)
                _c_r_value[c] = new Dictionary<int, double>();
        }
        public static MatrixSparse2 Zeros(int colsize, int rowsize)
        {
            return new MatrixSparse2(colsize, rowsize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetAt(int c, int r)
        {
            if((c < 0) || (c >= _colsize)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _rowsize)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            double value;
            bool hasvalue = _c_r_value[c].TryGetValue(r, out value);
            if(hasvalue)    return value;
            else            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAt(int c, int r, double value)
        {
            if((c < 0) || (c >= _colsize)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _rowsize)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            Dictionary<int,double> r_value = _c_r_value[c];
            if(r_value.ContainsKey(r))
            {
                if(value != 0)  r_value[r] = value;
                else            r_value.Remove(r);
            }
            else
            {
                if(value != 0)  r_value.Add(r, value);
                else            { } // do nothing
            }
        }
        public int NumNonZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int count = 0;
                for(int c=0; c<_colsize; c++)
                    count += _c_r_value[c].Count;
                return count;
            }
        }
        public double NumNonZeroRatio
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int count = 0;
                for(int c=0; c<_colsize; c++)
                    count += _c_r_value[c].Count;
                return count / (1.0 * _colsize * _rowsize);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(int c, int r, double val)> EnumNonZeros()
        {
            for(int c=0; c<_colsize; c++)
                foreach(var item in _c_r_value[c])
                    yield return (c, item.Key, item.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix ToMatrix()
        {
            Matrix mat = Matrix.Zeros(_colsize, _rowsize);
            foreach(var item in EnumNonZeros())
            {
                int    c   = item.c;
                int    r   = item.r;
                double val = item.val;
                mat[c,r] = val;
            }
            return mat;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixSparse2 FromMatrix(Matrix mat)
        {
            MatrixSparse2 smat = MatrixSparse2.Zeros(mat.ColSize, mat.RowSize);
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double val = mat[c,r];
                    if(val == 0) continue;
                    smat[c,r] = val;
                }
            return smat;
        }
        public static bool EqualContents(MatrixSparse2 a, MatrixSparse2 b)
        {
            if(a._colsize != b._colsize) return false;
            if(a._colsize != b._colsize) return false;
            for(int c=0; c<a._colsize; c++)
            {
                var a_r_val = a._c_r_value[c];
                var b_r_val = b._c_r_value[c];
                if(a_r_val.Count != b_r_val.Count) return false;
                foreach(var a_item in a_r_val)
                {
                    int    r   = a_item.Key;
                    double a_val = a_item.Value;
                    double b_val;
                    if(b_r_val.TryGetValue(r, out b_val) == false) return false;
                    if(a_val != b_val                            ) return false;
                }
            }
            return true;
        }
    }
}
