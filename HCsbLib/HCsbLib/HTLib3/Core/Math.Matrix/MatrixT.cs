using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Matrix<T>
    {
        public readonly T[] data;           // list of col vectors
        public readonly int ColSize; // colsize
        public readonly int RowSize; // colsize
        private int index(int c, int r)
        {
            Debug.Assert(0<=c, c<ColSize);
            Debug.Assert(0<=r, r<RowSize);
            return c+r*ColSize;
        }

        //public int ColSize { get { return colsize; } }
        //public int RowSize { get { return rowsize; } }

        protected Matrix(T[] data, int ColSize, int RowSize)
        {
        //  Debug.Assert(false); /// aware, data is colum vector
            Debug.Assert(data.Length == ColSize*RowSize);
            this.ColSize = ColSize;
            this.RowSize = RowSize;
            this.data    = data;
        }
        public Matrix(int ColSize, int RowSize)
        {
            this.ColSize = ColSize;
            this.RowSize = RowSize;
            this.data    = new T[ColSize*RowSize];
        }
        public Matrix(int ColSize, int RowSize, T initvalue)
        {
            this.ColSize = ColSize;
            this.RowSize = RowSize;
            this.data    = new T[ColSize*RowSize];
            for(int i=0; i<data.Length; i++)
                data[i] = initvalue;
        }
        public Matrix(T[,] mat)
        {
            this.ColSize = mat.GetLength(0);
            this.RowSize = mat.GetLength(1);
            this.data    = new T[ColSize*RowSize];
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    data[index(c, r)] = mat[c, r];
        }
        public T this[int c, int r]
        {
            get { return data[index(c,r)]; }
            set { data[index(c, r)] = value; }
        }


        public T[,] ToArray()
        {
            T[,] arr = new T[ColSize, RowSize];
            for(int r=0; r<RowSize; r++)
                for(int c=0; c<ColSize; c++)
                    arr[c, r] = data[index(c, r)];
            return arr;
        }

        public static implicit operator T[,](Matrix<T> mat)
        {
            T[,] arr = new T[mat.ColSize, mat.RowSize];
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    arr[c, r] = mat[c, r];
            return arr;
        }
        //public static implicit operator Vector<T>(T[] vec)
        //{
        //    return new Vector<T> { data = vec };
        //}
        //public static Matrix<T> operator*(Matrix<T> val1, T val2)
        //{
        //    val1.ilarr.a
        //
        //    T[] ret = new T[val1.data.Length];
        //    for(int i=0; i<ret.Length; i++)
        //        ret[i] = (val1[i] as dynamic) * val2;
        //    return ret;
        //}

        public static bool operator != (Matrix<T> val1, Matrix<T> val2)
        {
            return ((val1 == val2) == false);
        }
        public static bool operator ==(Matrix<T> val1, Matrix<T> val2)
        {
            int ColSize = val1.ColSize; if(ColSize != val2.ColSize) return false;
            int RowSize = val1.RowSize; if(RowSize != val2.RowSize) return false;
            int Length  = val1.data.Length; Debug.Assert(Length  != val2.data.Length);

            if(typeof(T) == typeof(double))
            {
                double[] val1data = val1.data as double[];
                double[] val2data = val2.data as double[];
                for(int i=0; i<val1data.Length; i++)
                    if(val1data[i] != val2data[i])
                        return false;
            }
            {
                for(int i=0; i<val1.data.Length; i++)
                    if((val1.data[i] as dynamic) != val2.data[i])
                        return false;
            }
            return true;
        }
        public override bool Equals(object obj)
        {
            if(object.ReferenceEquals(this, obj)) return true;
            Matrix<T> mat = (obj as Matrix<T>);
            if(mat == null) return false;
            return (this == mat);
        }
        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
        public string ToString( int? maxNumPrint        // [default] null
                              , Func<T, string> tostring // [default] null
                              )
        {
            if(tostring == null)
                tostring = delegate(T val) { return val.ToString(); };

            StringBuilder str = new StringBuilder();
            str.Append(string.Format("Matrix[{0},{1}] ", ColSize, RowSize));
            str.Append("{ ");
            int length = data.Length;
            if(maxNumPrint != null) length = Math.Min(length, maxNumPrint.Value);
            for(int c=0; c<ColSize; c++)
            {
                if(c != 0) str.Append(", ");
                str.Append("{");
                for(int r=0; r<RowSize; r++)
                {
                    if(r != 0) str.Append(", ");
                    if(length <= 0)
                    {
                        str.Append(", ...");
                        return str.ToString();
                    }
                    str.Append(tostring(this[c, r]));
                    length--;
                }
                str.Append("}");
            }
            str.Append("}");
            return str.ToString();
        }
    }
}
