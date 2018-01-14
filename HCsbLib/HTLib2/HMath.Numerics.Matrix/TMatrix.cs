using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public class TMatrix<T>
    {
        public T[,] data;

        public T this[int c, int r] {
            get { return data[c,r]; }
            set { data[c,r] = value; }
        }
        public int ColSize { get { return data.GetLength(0); } }
        public int RowSize { get { return data.GetLength(1); } }

        public static implicit operator T[,](TMatrix<T> vec)
        {
            return vec.data;
        }
        public static implicit operator TMatrix<T>(T[,] vec)
        {
            return new TMatrix<T> { data = vec };
        }
        public static TMatrix<T> operator*(TMatrix<T> val1, T val2)
        {
            int colsize = val1.ColSize;
            int rowsize = val1.RowSize;
            T[,] ret = new T[colsize, rowsize];
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    ret[c,r] = (dynamic)ret[c,r] * val2;
            return ret;
        }
        public static TMatrix<T> operator+(TMatrix<T> val1, T[,] val2)
        {
            int colsize = val1.ColSize; HDebug.Assert(colsize == val2.GetLength(0));
            int rowsize = val1.RowSize; HDebug.Assert(rowsize == val2.GetLength(1));
            HDebug.Assert(val1.data.Length == val2.Length);
            T[,] ret = new T[colsize, rowsize];
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    ret[c,r] = (dynamic)ret[c,r] + val2[c,r];
            return ret;
        }

        public static bool operator !=(TMatrix<T> val1, T[,] val2)
        {
            return ((val1 == val2) == false);
        }
        public static bool operator ==(TMatrix<T> val1, T[,] val2)
        {
            int colsize = val1.ColSize; if(colsize != val2.GetLength(0)) return false;
            int rowsize = val1.RowSize; if(rowsize != val2.GetLength(1)) return false;
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    if((dynamic)val1.data[c,r] != val2[c,r])
                    return false;
            return true;
        }
        public static bool operator>(TMatrix<T> val1, T val2)
        {
            foreach(dynamic val1i in val1.data)
                if((val1i > val2) == false)
                    return false;
            return true;
        }
        public static bool operator>=(TMatrix<T> val1, T val2)
        {
            foreach(dynamic val1i in val1.data)
                if((val1i >= val2) == false)
                    return false;
            return true;
        }
        public static bool operator<(TMatrix<T> val1, T val2)
        {
            foreach(dynamic val1i in val1.data)
                if((val1i < val2) == false)
                    return false;
            return true;
        }
        public static bool operator<=(TMatrix<T> val1, T val2)
        {
            foreach(dynamic val1i in val1.data)
                if((val1i <= val2) == false)
                    return false;
            return true;
        }

        public override string ToString()
        {
            return ToString(null);
        }
        public string ToString(int? maxNumPrint)
        {
            StringBuilder str = new StringBuilder();
            str.Append("{ ");
            int length = data.Length;
            if(maxNumPrint != null) length = Math.Min(length, maxNumPrint.Value);
            //bool addcomma = false;
            //for(int i=0; i<length; i++)
            //{
            //    if(addcomma == true) str.Append(", ");
            //    str.Append(data[i]);
            //    addcomma = true;
            //}
            if(length != data.Length)
                str.Append(", ...");
            str.Append("}");
            return str.ToString();
        }
    }
}
