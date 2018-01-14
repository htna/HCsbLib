using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public class TVector<T> : IEnumerable<T>, IVector<T>
    {
        // double[] xx         = new double [0x0_0FFF_FFF8]; is the maximum possible size for double array
        public const long MaxBlockCapacity = 0x0_0100_0000;
        public T[][] data;
        long IVector<T>.SizeLong  { get { return      SizeLong; } }

        public          int  Size { get { return (int)SizeLong; } }
        public readonly long SizeLong;
        public T[] ToArray()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(T[] datai in data)
                for(int j = 0; j < datai.Length; j++)
                    yield return datai[j];
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public TVector(TVector<T> src)
        {
            this.SizeLong = src.SizeLong;
            this.data = new T[src.data.Length][];
            for(int i = 0; i < data.Length; i++)
                data[i] = src.data[i].HClone();
        }
        public TVector(params T[] values)
        {
            if(values.Length > MaxBlockCapacity)
                throw new Exception();
            data = new T[1][];
            data[0] = values;
            SizeLong = values.Length;
        }
        public TVector(long size)
        {
            this.SizeLong = size;
            List<T[]> ldata = new List<T[]>();
            while(size > 0)
            {
                long toalloc = Math.Min(size, MaxBlockCapacity);
                ldata.Add(new T[toalloc]);
                size -= toalloc;
            }
            this.data = ldata.ToArray();
        }

        public T this[long i]
        {
            get
            {
                HDebug.Assert(0 <= i && i < SizeLong);
                long idx = i / MaxBlockCapacity;
                long off = i % MaxBlockCapacity;
                return data[idx][off];
            }
            set
            {
                HDebug.Assert(0 <= i && i < SizeLong);
                long idx = i / MaxBlockCapacity;
                long off = i % MaxBlockCapacity;
                data[idx][off] = value;
            }
        }
        public static implicit operator T[](TVector<T> vec)
        {
            if(vec.data.Length >= 2)
                throw new Exception();
            return vec.data[0];
        }
        public static implicit operator TVector<T>(T[] vec)
        {
            return new TVector<T>(vec);
        }
        public TVector<T> HClone()
        {
            return new TVector<T>(this);
        }

        public void Mul(T val)
        {
            foreach(T[] datai in data)
                for(int j = 0; j < datai.Length; j++)
                    datai[j] = (dynamic)datai[j] * val;
        }
        public static TVector<T> operator*(TVector<T> val1, T val2)
        {
            TVector<T> ret = val1.HClone();
            ret.Mul(val2);
            return ret;
        }
        public void Add(T val)
        {
            foreach(T[] datai in data)
                for(int j = 0; j < datai.Length; j++)
                    datai[j] = (dynamic)datai[j] + val;
        }
        public void Add(TVector<T> val)
        {
            HDebug.Assert(SizeLong == val.SizeLong);
            for(int i=0; i<data.Length; i++)
            {
                T[] datai = data[i];
                T[] val_datai = val.data[i];
                for(int j = 0; j < datai.Length; j++)
                    datai[j] = (dynamic)datai[j] + val_datai[j];
            }
        }
        public void Sub(T val)
        {
            foreach(T[] datai in data)
                for(int j = 0; j < datai.Length; j++)
                    datai[j] = (dynamic)datai[j] - val;
        }
        public void Sub(TVector<T> val)
        {
            HDebug.Assert(SizeLong == val.SizeLong);
            for(int i = 0; i < data.Length; i++)
            {
                T[] datai = data[i];
                T[] val_datai = val.data[i];
                for(int j = 0; j < datai.Length; j++)
                    datai[j] = (dynamic)datai[j] - val_datai[j];
            }
        }
        public static TVector<T> operator +(TVector<T> val1, T val2)
        {
            TVector<T> ret = val1.HClone();
            ret.Add(val2);
            return ret;
        }
        public static TVector<T> operator -(TVector<T> val1, T val2)
        {
            TVector<T> ret = val1.HClone();
            ret.Sub(val2);
            return ret;
        }
        public static TVector<T> operator+(TVector<T> val1, TVector<T> val2)
        {
            TVector<T> ret = val1.HClone();
            ret.Add(val2);
            return ret;
        }
        public static TVector<T> operator-(TVector<T> val1, TVector<T> val2)
        {
            TVector<T> ret = val1.HClone();
            ret.Sub(val2);
            return ret;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(obj is TVector<T>)
                return Equals(this, obj as TVector<T>);
            return base.Equals(obj);
        }
        public static bool operator != (TVector<T> val1, TVector<T> val2)
        {
            return (Equals(val1, val2) == false);
        }
        public static bool operator == (TVector<T> val1, TVector<T> val2)
        {
            return Equals(val1, val2);
        }
        public static bool Equals(TVector<T> val1, TVector<T> val2)
        {
            if(val1.SizeLong != val2.SizeLong)
                return false;
            for(long i = 0; i < val1.SizeLong; i++)
                if((dynamic)val1[i] != val2[i])
                    return false;
            return true;
        }
        public static bool operator>(TVector<T> val1, T val2)
        {
            foreach(dynamic val1i in val1.data)
                if((val1i > val2) == false)
                    return false;
            return true;
        }
        public static bool operator>=(TVector<T> val1, T val2)
        {
            foreach(dynamic val1i in val1.data)
                if((val1i >= val2) == false)
                    return false;
            return true;
        }
        public static bool operator<(TVector<T> val1, T val2)
        {
            foreach(dynamic val1i in val1.data)
                if((val1i < val2) == false)
                    return false;
            return true;
        }
        public static bool operator<=(TVector<T> val1, T val2)
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
            bool addcomma = false;
            int length = data.Length;
            if(maxNumPrint != null) length = Math.Min(length, maxNumPrint.Value);
            for(int i=0; i<length; i++)
            {
                if(addcomma == true) str.Append(", ");
                str.Append(data[i]);
                addcomma = true;
            }
            if(length != data.Length)
                str.Append(", ...");
            str.Append("}");
            return str.ToString();
        }
    }
}
