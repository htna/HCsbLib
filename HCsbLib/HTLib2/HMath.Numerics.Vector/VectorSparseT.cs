using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public class VectorSparse<T> : IVectorSparse<T>
    {
        long IVector<T>.SizeLong { get { return Size; } }
        int  IVector<T>.Size     { get { return Size; } }
        public readonly int Size;
        Dictionary<int, T>  data;
        public readonly Func<T> GetDefault;

        public VectorSparse(int size, Func<T> GetDefault=null)
        {
            this.Size = size;
            this.data = new Dictionary<int, T>();
            this.GetDefault = GetDefault;
        }
        public string ToString()
        {
            string msg = "";
            msg += string.Format("size({0}), elements({1})", Size, NumElements);
            return msg;
        }

        public T this[long i]
        {
            get
            {
                int ii = (int)i;
                HDebug.Assert(0<=i, i<Size);
                if(data.ContainsKey(ii)) return data[ii];
                if(GetDefault != null) return GetDefault();
                return default(T);
            }
            set
            {
                int ii = (int)i;
                HDebug.Assert(0<=i, i<Size);
                if(data.ContainsKey(ii)) { data[ii] = value; return; }
                data.Add(ii, value);
            }
        }

        public int NumElements
        {
            get
            {
                return data.Count;
            }
        }
        public IEnumerable<Tuple<int, T>> EnumElements()
        {
            foreach(var i_val in data)
            {
                int i   = i_val.Key;
                T   val = i_val.Value;
                yield return new Tuple<int, T>(i, val);
            }
        }
        public IEnumerable<int> EnumNulls()
        {
            int[] rows = new int[Size];
            foreach(int i in data.Keys)
                rows[i] = 1;

            for(int i=0; i<Size; i++)
                if(rows[i] == 0)
                    yield return i;
        }
        public IEnumerable<int> EnumIndex()
        {
            return data.Keys;
        }
        public bool HasElement(int idx)
        {
            return data.ContainsKey(idx);
        }

        public VectorSparse<T> GetSubVector(IList<int> idxs)
        {
            VectorSparse<T> subvec = new VectorSparse<T>(idxs.Count, this.GetDefault);

            Dictionary<int,int> idx_subidx = idxs.HToDictionaryAsValueIndex();

            foreach(var i_val in EnumElements())
            {
                int i = i_val.Item1; if(idx_subidx.ContainsKey(i) == false) continue;
                T val = i_val.Item2;
                int si = idx_subidx[i];
                subvec[si] = val;
            }
            return subvec;
        }
        public VectorSparse<T> GetSubVectorExcept(int idxremove)
        {
            VectorSparse<T> subvec = new VectorSparse<T>(Size-1, this.GetDefault);

            foreach(var i_val in EnumElements())
            {
                int i = i_val.Item1; if(i == idxremove) continue;
                T val = i_val.Item2;
                int si = (i < idxremove) ? i : (i-1);
                subvec[si] = val;
            }
            return subvec;
        }

        public T[] ToArray()
        {
            T[] arr = new T[Size];
            for(int i=0; i<Size; i++)
                arr[i] = this[i];
            return arr;
        }
        public T[] ToArray(T defvalue)
        {
            Func<T> GetDefault = delegate()
            {
                return defvalue;
            };
            return ToArray(GetDefault);
        }
        public T[] ToArray(Func<T> GetDefault)
        {
            T[] arr = new T[Size];
            foreach(var i_val in EnumElements())
            {
                int i = i_val.Item1;
                T val = i_val.Item2;
                arr[i] = val;
            }
            foreach(var i in EnumNulls())
            {
                T val = GetDefault();
                arr[i] = val;
            }
            return arr;
        }
    }
}
