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
        int        capacity;
        readonly T def;
        int        count;
        T[]        arr;
        public HLayeredArray1(int capacity, T def)
        {
            this.capacity = capacity;
            this.def      = def;
            this.count    = 0;
            this.arr      = new T[capacity];
            for(int i = 0; i < capacity; i++)
                arr[i] = def;
        }
        public int Count { get { return count; } }
        public T this[int index]
        {   get { return GetAt(index); }
            set { SetAt(index, value); }
        }
        public T GetAt(int index)
        {
            return arr[index];
        }
        public void SetAt(int index, T value)
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
