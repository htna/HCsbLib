using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    [Serializable]
    public class MatrixByRowVec<T> : IMatrix<T>
    {
        int IMatrix<T>.ColSize { get { return (int)ColSize; } } //int IMatrix<T>.NumRows { get { return (int)ColSize; } }
        int IMatrix<T>.RowSize { get { return (int)RowSize; } } //int IMatrix<T>.NumCols { get { return (int)RowSize; } }
        public readonly long ColSize;
        public readonly long RowSize;
        public T[][] valColRow;

        public MatrixByRowVec(long colsize, long rowsize)
        {
            ColSize   = colsize;
            RowSize   = rowsize;
            valColRow = new T[ColSize][];
            for(long c = 0; c < ColSize; c++)
                valColRow[c] = new T[RowSize];
        }
        public override string ToString()
        {
            string msg = "";
            msg += string.Format("size({0}, {1})", ColSize, RowSize);
            return msg;
        }

        public T this[int c, int r]
        {
            get { return valColRow[c][r]; }
            set { valColRow[c][r] = value; }
        }
        public T this[long c, long r]
        {
            get { return valColRow[c][r]; }
            set { valColRow[c][r] = value; }
        }
        public T GetAtLock(long c, long r)
        {
            throw new NotImplementedException();
            //HDebug.Assert(0<=c, c<ColSize, 0<=r, r<RowSize);
            //if(c==r)
            //{
            //    lock(diagonal)
            //    {
            //        if(diagonal[c] != null) return diagonal[c];
            //    }
            //}
            //else
            //{
            //    var offdiagonal_c = offdiagonal[c];
            //    lock(offdiagonal_c)
            //    {
            //        if(offdiagonal_c.ContainsKey(r)) return offdiagonal_c[r];
            //    }
            //}
            //if(GetDefault != null) return GetDefault();
            //return default(T);
        }
        public void SetAtLock(long c, long r, T value)
        {
            throw new NotImplementedException();
            //HDebug.Assert(0<=c, c<ColSize, 0<=r, r<RowSize);
            //if(c==r)
            //{
            //    lock(diagonal)
            //    {
            //        diagonal[c] = value;
            //        return;
            //    }
            //}
            //if(offdiagonal[c].ContainsKey(r))
            //{
            //    var offdiagonal_c = offdiagonal[c];
            //    lock(offdiagonal_c)
            //    {
            //        if(value == null) offdiagonal_c.Remove(r);
            //        else offdiagonal_c[r] = value;
            //        return;
            //    }
            //}
            //if(value != null)
            //{
            //    var offdiagonal_c = offdiagonal[c];
            //    lock(offdiagonal_c)
            //    {
            //        offdiagonal_c.Add(r, value);
            //    }
            //}
        }
        public bool HasElementLock(long c, long r)
        {
            throw new NotImplementedException();
            //HDebug.Assert(0 <= c, c < ColSize, 0 <= r, r < RowSize);
            //if(c == r)
            //{
            //    lock(diagonal)
            //    {
            //        if(diagonal[c] != null) return true;
            //    }
            //}
            //else
            //{
            //    var offdiagonal_c = offdiagonal[c];
            //    lock(offdiagonal_c)
            //    {
            //        if(offdiagonal_c.ContainsKey(r)) return true;
            //    }
            //}
            //return false;
        }

        public T[] GetColVector(long row)
        {
            throw new NotImplementedException();
        }
        public T[] GetRowVector(long col)
        {
            return valColRow[col];
        }

        //public MatrixSparse<T> GetSubMatrix(int col_count, int row_count)
        //{
        //    return GetSubMatrixFromTo(0, col_count-1, 0, row_count-1);
        //}
        //public MatrixSparse<T> GetSubMatrixFromTo(int col_from, int col_to, int row_from, int row_to)
        //{
        //    IList<int> colidxs = HEnum.HEnumFromTo(col_from, col_to).ToList();
        //    IList<int> rowidxs = HEnum.HEnumFromTo(row_from, row_to).ToList();
        //    return GetSubMatrix(colidxs, rowidxs);
        //}
        //public MatrixSparse<T> GetSubMatrix(IList<int> colidxs, IList<int> rowidxs)
        //{
        //    MatrixSparse<T> submat = new MatrixSparse<T>(colidxs.Count, rowidxs.Count, this.GetDefault);
        //
        //    Dictionary<int,int> col_subcol = colidxs.HToDictionaryAsValueIndex();
        //    Dictionary<int,int> row_subrow = rowidxs.HToDictionaryAsValueIndex();
        //
        //    foreach(var c_r_val in EnumElements())
        //    {
        //        int c = c_r_val.Item1; if(col_subcol.ContainsKey(c) == false) continue;
        //        int r = c_r_val.Item2; if(row_subrow.ContainsKey(r) == false) continue;
        //        T val = c_r_val.Item3;
        //        int sc = col_subcol[c];
        //        int sr = row_subrow[r];
        //        submat[sc,sr] = val;
        //    }
        //    return submat;
        //}
        //public MatrixSparse<T> GetSubMatrixExcept(int colremove, int rowremove)
        //{
        //    MatrixSparse<T> submat = new MatrixSparse<T>(ColSize-1, RowSize-1, this.GetDefault);
        //
        //    foreach(var c_r_val in EnumElements())
        //    {
        //        int c = c_r_val.Item1; if(c == colremove) continue;
        //        int r = c_r_val.Item2; if(r == rowremove) continue;
        //        T val = c_r_val.Item3;
        //        int sc = (c < colremove) ? c : (c-1);
        //        int sr = (r < rowremove) ? r : (r-1);
        //        submat[sc, sr] = val;
        //    }
        //    return submat;
        //}

        public T[,] ToArray()
        {
            throw new NotImplementedException();
            //  T[,] arr = new T[ColSize, RowSize];
            //  for(int c=0; c<ColSize; c++)
            //      for(int r=0; r<RowSize; r++)
            //          arr[c, r] = this[c, r];
            //  return arr;
        }
    }
}
