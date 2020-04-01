using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    [Serializable]
    public class MatrixSparse<T> : IMatrixSparse<T>
    {
        int IMatrix<T>.ColSize { get { return ColSize; } } //int IMatrix<T>.NumRows { get { return ColSize; } }
        int IMatrix<T>.RowSize { get { return RowSize; } } //int IMatrix<T>.NumCols { get { return RowSize; } }
        public readonly int ColSize;
        public readonly int RowSize;
        T[]                 diagonal;
        Dictionary<int,T>[] offdiagonal;
        public readonly Func<T> GetDefault;

        public MatrixSparse(int colsize, int rowsize, Func<T> GetDefault=null)
        {
            this.ColSize     = colsize;
            this.RowSize     = rowsize;
            this.GetDefault  = GetDefault;
            this.diagonal    = new T[ColSize];
            this.offdiagonal = new Dictionary<int,T>[ColSize];
            for(int c=0; c<ColSize; c++)
                offdiagonal[c] = new Dictionary<int,T>();
        }
        public override string ToString()
        {
            string msg = "";
            msg += string.Format("size({0}, {1}), elements({2})", ColSize, RowSize, NumElements);
            return msg;
        }

        public T this[Tuple<int, int> cr]
        {
            get { return GetAt(cr.Item1, cr.Item2); }
            set { SetAt(cr.Item1, cr.Item2, value); }
        }
        public T this[int c, int r]
        {
            get { return GetAt(c, r); }
            set { SetAt(c, r, value); }
        }
        public T this[long c, long r]
        {
            get { return GetAt((int)c, (int)r); }
            set { SetAt((int)c, (int)r, value); }
        }
        public T GetAt(int c, int r)
        {
            HDebug.Assert(0<=c, c<ColSize, 0<=r, r<RowSize);
            if(c==r)
            {
                if(diagonal[c] != null) return diagonal[c];
            }
            else
            {
                if(offdiagonal[c].ContainsKey(r)) return offdiagonal[c][r];
            }
            if(GetDefault != null) return GetDefault();
            return default(T);
        }
        public T GetAtLock(int c, int r)
        {
            HDebug.Assert(0<=c, c<ColSize, 0<=r, r<RowSize);
            if(c==r)
            {
                lock(diagonal)
                {
                    if(diagonal[c] != null) return diagonal[c];
                }
            }
            else
            {
                var offdiagonal_c = offdiagonal[c];
                lock(offdiagonal_c)
                {
                    if(offdiagonal_c.ContainsKey(r)) return offdiagonal_c[r];
                }
            }
            if(GetDefault != null) return GetDefault();
            return default(T);
        }
        public void SetAt(int c, int r, T value)
        {
            HDebug.Assert(0<=c, c<ColSize, 0<=r, r<RowSize);
            if(c==r)
            {
                lock(diagonal)
                {
                    diagonal[c] = value;
                    return;
                }
            }
            if(offdiagonal[c].ContainsKey(r))
            {
                if(value == null) offdiagonal[c].Remove(r);
                else              offdiagonal[c][r] = value;
                return;
            }
            if(value != null)
                offdiagonal[c].Add(r, value);
        }
        public void SetAtLock(int c, int r, T value)
        {
            HDebug.Assert(0<=c, c<ColSize, 0<=r, r<RowSize);
            if(c==r)
            {
                lock(diagonal)
                {
                    diagonal[c] = value;
                    return;
                }
            }
            if(offdiagonal[c].ContainsKey(r))
            {
                var offdiagonal_c = offdiagonal[c];
                lock(offdiagonal_c)
                {
                    if(value == null) offdiagonal_c.Remove(r);
                    else offdiagonal_c[r] = value;
                    return;
                }
            }
            if(value != null)
            {
                var offdiagonal_c = offdiagonal[c];
                lock(offdiagonal_c)
                {
                    offdiagonal_c.Add(r, value);
                }
            }
        }
        public int NumElements
        {
            get
            {
                int num = 0;
                for(int c=0; c<ColSize; c++)
                {
                    if(diagonal[c] != null) num++;
                    num += offdiagonal[c].Count;
                }
                return num;
            }
        }
        public int GetNumElementsInRow(int col)
        {
            int num=0;
            if(diagonal[col] != null) num++;
            num += offdiagonal[col].Count;
            return num;
        }

        public bool HasElement(int c, int r)
        {
            HDebug.Assert(0<=c, c<ColSize, 0<=r, r<RowSize);
            if(c==r)
            {
                if(diagonal[c] != null) return true;
            }
            else
            {
                if(offdiagonal[c].ContainsKey(r)) return true;
            }
            return false;
        }
        public bool HasElementLock(int c, int r)
        {
            HDebug.Assert(0 <= c, c < ColSize, 0 <= r, r < RowSize);
            if(c == r)
            {
                lock(diagonal)
                {
                    if(diagonal[c] != null) return true;
                }
            }
            else
            {
                var offdiagonal_c = offdiagonal[c];
                lock(offdiagonal_c)
                {
                    if(offdiagonal_c.ContainsKey(r)) return true;
                }
            }
            return false;
        }

        public IEnumerable<ValueTuple<int,int,T>> EnumElements()
        {
            int[] cols = HEnum.HEnumCount(ColSize).ToArray();
            return EnumElements(cols);
        }
        public IEnumerable<Tuple<int,int,T>> EnumElements_dep(params int[] cols)
        {
            foreach(int c in cols)
            //for(int c=0; c<ColSize; c++)
            {
                if(diagonal[c] != null)
                    yield return new Tuple<int, int, T>(c, c, diagonal[c]);
                foreach(var r_val in offdiagonal[c])
                {
                    int r   = r_val.Key;
                    T   val = r_val.Value;
                    yield return new Tuple<int, int, T>(c, r, val);
                }
            }
        }
        public IEnumerable<ValueTuple<int,int,T>> EnumElements(params int[] cols)
        {
            foreach(int c in cols)
            //for(int c=0; c<ColSize; c++)
            {
                if(diagonal[c] != null)
                    yield return new ValueTuple<int, int, T>(c, c, diagonal[c]);
                foreach(var r_val in offdiagonal[c])
                {
                    int r   = r_val.Key;
                    T   val = r_val.Value;
                    yield return new ValueTuple<int, int, T>(c, r, val);
                }
            }
        }
        public IEnumerable<Tuple<int,int>> EnumIndices()
        {
            for(int c=0; c<ColSize; c++)
            {
                if(diagonal[c] != null)
                    yield return new Tuple<int, int>(c, c);
                foreach(var r_val in offdiagonal[c])
                {
                    int r   = r_val.Key;
                    yield return new Tuple<int, int>(c, r);
                }
            }
        }
        public IEnumerable<Tuple<int, int>> EnumNulls()
        {
            for(int c=0; c<ColSize; c++)
            {
                if(diagonal[c] == null)
                    yield return new Tuple<int, int>(c, c);
                
                int[] rows = new int[RowSize];
                rows[c] = 1;
                foreach(int r in offdiagonal[c].Keys)
                    rows[r] = 1;

                for(int r=0; r<RowSize; r++)
                    if(rows[r] == 0)
                        yield return new Tuple<int, int>(c, r);
            }
        }

        public VectorSparse<T> GetColVector(int row)
        {
            HDebug.ToDo();
            return null;
        }
        public VectorSparse<T> GetRowVector(int col)
        {
            HDebug.AssertAnd(0<=col, col<RowSize);
            VectorSparse<T> vec = new VectorSparse<T>(RowSize, GetDefault: GetDefault);
            if(diagonal[col] != null)
                vec[col] = diagonal[col];
            foreach(var r_val in offdiagonal[col])
            {
                int r = r_val.Key;
                T val = r_val.Value;
                vec[r] = val;
            }
            return vec;
        }

        public MatrixSparse<T> GetSubMatrix(int col_count, int row_count)
        {
            return GetSubMatrixFromTo(0, col_count-1, 0, row_count-1);
        }
        public MatrixSparse<T> GetSubMatrixFromTo(int col_from, int col_to, int row_from, int row_to)
        {
            IList<int> colidxs = HEnum.HEnumFromTo(col_from, col_to).ToList();
            IList<int> rowidxs = HEnum.HEnumFromTo(row_from, row_to).ToList();
            return GetSubMatrix(colidxs, rowidxs);
        }
        public MatrixSparse<T> GetSubMatrix(IList<int> colidxs, IList<int> rowidxs)
        {
            MatrixSparse<T> submat = new MatrixSparse<T>(colidxs.Count, rowidxs.Count, this.GetDefault);

            Dictionary<int,int> col_subcol = colidxs.HToDictionaryAsValueIndex();
            Dictionary<int,int> row_subrow = rowidxs.HToDictionaryAsValueIndex();

            foreach(var c_r_val in EnumElements())
            {
                int c = c_r_val.Item1; if(col_subcol.ContainsKey(c) == false) continue;
                int r = c_r_val.Item2; if(row_subrow.ContainsKey(r) == false) continue;
                T val = c_r_val.Item3;
                int sc = col_subcol[c];
                int sr = row_subrow[r];
                submat[sc,sr] = val;
            }
            return submat;
        }
        public MatrixSparse<T> GetSubMatrixExcept(int colremove, int rowremove)
        {
            MatrixSparse<T> submat = new MatrixSparse<T>(ColSize-1, RowSize-1, this.GetDefault);

            foreach(var c_r_val in EnumElements())
            {
                int c = c_r_val.Item1; if(c == colremove) continue;
                int r = c_r_val.Item2; if(r == rowremove) continue;
                T val = c_r_val.Item3;
                int sc = (c < colremove) ? c : (c-1);
                int sr = (r < rowremove) ? r : (r-1);
                submat[sc, sr] = val;
            }
            return submat;
        }

        public T[,] ToArray()
        {
            T[,] arr = new T[ColSize, RowSize];
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    arr[c, r] = this[c, r];
            return arr;
        }
        public T[,] ToArray(T defvalue)
        {
            Func<T> GetDefault = delegate()
            {
                return defvalue;
            };
            return ToArray(GetDefault);
        }
        public T[,] ToArray(Func<T> GetDefault)
        {
            T[,] arr = new T[ColSize, RowSize];
            foreach(var c_r_val in EnumElements())
            {
                int c = c_r_val.Item1;
                int r = c_r_val.Item2;
                T val = c_r_val.Item3;
                arr[c,r] = val;
            }
            foreach(var c_r in EnumNulls())
            {
                int c = c_r.Item1;
                int r = c_r.Item2;
                T val = GetDefault();
                arr[c, r] = val;
            }
            return arr;
        }
    }
}
