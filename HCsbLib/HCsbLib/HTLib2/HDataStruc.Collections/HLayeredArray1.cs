using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public class HLayeredArray1<T>
        where T : IEquatable<T>
    {
        long       capacity1;
        readonly T def;
        long       count;
        T[]        arr;
        public HLayeredArray1(T def, long capacity1)
        {
            HDebug.ToDo("check");
            this.capacity1 = capacity1;
            this.def       = def;
            this.count     = 0;
            this.arr       = new T[capacity1];
            for(long i = 0; i < capacity1; i++)
                arr[i] = def;
        }
        public long Count { get { return count; } }
        public T this[long index]
        {
            get { return GetAt(index); }
            set { SetAt(index, value); }
        }
        public T GetAt(long index)
        {
            return arr[index];
        }
        public void SetAt(long index, T value)
        {
            bool idef = def.Equals(arr[index]);
            bool vdef = def.Equals(value);
            if(idef)
            {
                if(vdef)
                {
                    // arr[i] == def
                    // value  == def
                    return;
                }
                else
                {
                    // arr[i] == def
                    // value  != def
                    arr[index] = value;
                    count++;
                    HDebug.Assert(count <= capacity1);
                }
            }
            else
            {
                if(vdef)
                {
                    // arr[i] != def
                    // value  == def
                    arr[index] = def;
                    count--;
                    HDebug.Assert(count >= 0);
                }
                else
                {
                    // arr[i] != def
                    // value  != def
                    arr[index] = value;
                }
            }
        }
    }
}
