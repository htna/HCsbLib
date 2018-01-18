using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public class HLayeredArray2<T>
        where T : IEquatable<T>
    {
        long                capacity2;
        readonly T          def;
        long                count;
        HLayeredArray1<T>[] arr;
        Func<HLayeredArray1<T>> newarri;
        public HLayeredArray2(T def, long capacity2, long capacity1)
        {
            this.capacity2  = capacity2;
            this.newarri    = delegate () { return new HLayeredArray1<T>(def, capacity1); };
            this.def        = def;
            this.count      = 0;
            this.arr        = new HLayeredArray1<T>[capacity2];
            for(long i = 0; i < capacity2; i++)
                arr[i] = null;
        }
        public long Count { get { return count; } }
        public T this[long index]
        {
            get { return GetAt(index); }
            set { SetAt(index, value); }
        }
        public T GetAt(long index)
        {
            long idx1 = index % capacity2;
            long idx2 = index / capacity2;
            var  arri = arr[idx2];
            if(arri == null)
                return def;
            return arri[idx1];
        }
        public void SetAt(long index, T value)
        {
            long idx1 = index % capacity2;
            long idx2 = index / capacity2;
            bool idef = (arr[idx2] == null);
            bool vdef = def.Equals(value);
            if(idef)
            {
                if(vdef)
                {
                    // arr[i] == null
                    // value  == def
                    return;
                }
                else
                {
                    // arr[i] == null
                    // value  != def
                    arr[idx2] = newarri();
                    arr[idx2][idx1] = value;
                    count++;
                    HDebug.Assert(count <= capacity2);
                }
            }
            else
            {
                if(vdef)
                {
                    // arr[i] != null
                    // value  == def
                    arr[idx2][idx1] = def;
                    if(arr[idx2].Count == 0)
                    {
                        arr[idx2] = null;
                        count--;
                        HDebug.Assert(count >= 0);
                    }
                }
                else
                {
                    // arr[i] != null
                    // value  != def
                    arr[idx2][idx1] = def;
                }
            }
        }
    }
}
