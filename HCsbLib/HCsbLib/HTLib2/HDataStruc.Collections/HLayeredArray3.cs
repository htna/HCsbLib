using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public class HLayeredArray3<T>
        where T : IEquatable<T>
    {
        long                capacity3;
        readonly T          def;
        long                count;
        HLayeredArray2<T>[] arr;
        Func<HLayeredArray2<T>> newarri;
        public HLayeredArray3(T def, long capacity3, long capacity2, long capacity1)
        {
            HDebug.ToDo("check");
            this.capacity3  = capacity3;
            this.newarri    = delegate () { return new HLayeredArray2<T>(def, capacity2, capacity1); };
            this.def        = def;
            this.count      = 0;
            this.arr        = new HLayeredArray2<T>[capacity3];
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
            long idx2 = index % capacity3;
            long idx3 = index / capacity3;
            var  arri = arr[idx3];
            if(arri == null)
                return def;
            return arri[idx2];
        }
        public void SetAt(long index, T value)
        {
            long idx2 = index % capacity3;
            long idx3 = index / capacity3;
            bool idef = (arr[idx3] == null);
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
                    arr[idx3] = newarri();
                    arr[idx3][idx2] = value;
                    count++;
                    HDebug.Assert(count <= capacity3);
                }
            }
            else
            {
                if(vdef)
                {
                    // arr[i] != null
                    // value  == def
                    arr[idx3][idx2] = def;
                    if(arr[idx3].Count == 0)
                    {
                        arr[idx3] = null;
                        count--;
                        HDebug.Assert(count >= 0);
                    }
                }
                else
                {
                    // arr[i] != null
                    // value  != def
                    arr[idx3][idx2] = def;
                }
            }
        }
    }
}
