using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class HCsbLibStatic
    {
        public static bool HLayeredArray1_doselftest = HDebug.IsDebuggerAttached;
        public static void HLayeredArray1_selftest()
        {
            if(HLayeredArray1_doselftest == false)
                return;
            HLayeredArray1_doselftest = false;
            HLayeredArray1<int> arr = new HLayeredArray1<int>(0, 2);
            HDebug.Assert(arr.Count == 0);
            arr[0] = 1;
            HDebug.Assert(arr.Count == 1, arr[0] == 1, arr[1] == 0);
            arr[0] = 2;
            HDebug.Assert(arr.Count == 1, arr[0] == 2, arr[1] == 0);
            arr[1] = 3;
            HDebug.Assert(arr.Count == 2, arr[0] == 2, arr[1] == 3);
            arr[1] = 4;
            HDebug.Assert(arr.Count == 2, arr[0] == 2, arr[1] == 4);
            arr[0] = 0;
            HDebug.Assert(arr.Count == 1, arr[0] == 0, arr[1] == 4);
            arr[1] = 0;
            HDebug.Assert(arr.Count == 0, arr[0] == 0, arr[1] == 0);
        }
    }
    public class HLayeredArray1<T>
        where T : IEquatable<T>
    {
        readonly long capacity1;
        readonly T    def;
        long          count;
        T[]           arr;
        public HLayeredArray1(T def, long capacity1)
        {
            if(HCsbLibStatic.HLayeredArray1_doselftest)
                HCsbLibStatic.HLayeredArray1_selftest();
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
