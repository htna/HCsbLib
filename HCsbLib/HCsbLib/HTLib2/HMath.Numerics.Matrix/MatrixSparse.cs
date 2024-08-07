using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public class MatrixSparse : IMatrix<double>, IBinarySerializable
    {
        int _colsize;
        int _rowsize;
        Dictionary<(int c,int r),double> _c_r_value;
        ///////////////////////////////////////////////////
        // IMatrix<double>
        public int ColSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _colsize; } }
        public int RowSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _rowsize; } }
        public double this[int  c, int  r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt(c,r);                   } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt(c,r,value);                    } }
        public double this[long c, long r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { throw new NotImplementedException(); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { throw new NotImplementedException(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public double[,] ToArray()
        {
            double[,] arr = new double[_colsize,_rowsize];
            foreach(var item in _c_r_value)
            {
                int c = item.Key.c;
                int r = item.Key.r;
                double val = item.Value;
                arr[c,r] = val;
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
            writer.Write(_c_r_value.Count);
            int count = 0;
            foreach(var item in _c_r_value)
            {
                int    c   = item.Key.c;
                int    r   = item.Key.r;
                double val = item.Value;
                writer.Write(c  );
                writer.Write(r  );
                writer.Write(val);
                count ++;
            }
            if(count != _c_r_value.Count)
                throw new Exception("(count != _c_r_value.Count)");
        }
        public MatrixSparse(HBinaryReader reader)
        {
                       reader.Read(out _colsize);
                       reader.Read(out _rowsize);
            int count; reader.Read(out count   );
            
            _c_r_value = new Dictionary<(int c, int r), double>();
            for(int i=0; i<count; i++)
            {
                int    c  ; reader.Read(out c  );
                int    r  ; reader.Read(out r  );
                double val; reader.Read(out val);
                SetAt(c, r, val);
            }
            if(count != _c_r_value.Count)
                throw new Exception("(count != _c_r_value.Count)");
        }
        // IBinarySerializable
        ///////////////////////////////////////////////////

        public MatrixSparse(int colsize, int rowsize)
        {
            _colsize   = colsize;
            _rowsize   = rowsize;
            _c_r_value = new Dictionary<(int c, int r), double>();
        }
        public static MatrixSparse Zeros(int colsize, int rowsize)
        {
            return new MatrixSparse(colsize, rowsize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetAt(int c, int r)
        {
            if((c < 0) || (c >= _colsize)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _rowsize)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            (int,int) key = (c,r);
            double value;
            bool hasvalue = _c_r_value.TryGetValue(key, out value);
            if(hasvalue)    return value;
            else            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAt(int c, int r, double value)
        {
            if((c < 0) || (c >= _colsize)) throw new IndexOutOfRangeException("((c < 0) || (c >= _ColSize))");
            if((c < 0) || (r >= _rowsize)) throw new IndexOutOfRangeException("((c < 0) || (r >= _RowSize))");
            (int,int) key = (c,r);
            if(_c_r_value.ContainsKey(key))
            {
                if(value != 0)  _c_r_value[key] = value;
                else            _c_r_value.Remove(key);
            }
            else
            {
                if(value != 0)  _c_r_value.Add(key, value);
                else            { } // do nothing
            }
        }
        public int NumNonZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _c_r_value.Count;
            }
        }
        public double NumNonZeroRatio
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _c_r_value.Count / (1.0 * _colsize * _rowsize);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(int c, int r, double val)> EnumNonZeros()
        {
            foreach(var item in _c_r_value)
            {
                int    c   = item.Key.c;
                int    r   = item.Key.r;
                double val = item.Value;
                yield return (c, r, val);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix ToMatrix()
        {
            Matrix mat = Matrix.Zeros(_colsize, _rowsize);
            foreach(var item in _c_r_value)
            {
                int    c   = item.Key.c;
                int    r   = item.Key.r;
                double val = item.Value;
                mat[c,r] = val;
            }
            return mat;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixSparse FromMatrix(Matrix mat)
        {
            MatrixSparse smat = MatrixSparse.Zeros(mat.ColSize, mat.RowSize);
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double val = mat[c,r];
                    if(val == 0) continue;
                    smat[c,r] = val;
                }
            return smat;
        }
        public static bool EqualContents(MatrixSparse a, MatrixSparse b)
        {
            if(a._colsize != b._colsize) return false;
            if(a._colsize != b._colsize) return false;
            foreach(var itema in a._c_r_value)
            {
                (int,int) key  = itema.Key;
                double    vala = itema.Value;
                if(b._c_r_value.ContainsKey(key) == false)
                    return false;
                double    valb = b._c_r_value[key];
                if(vala != valb)
                    return false;
            }
            return true;
        }
    }
}
