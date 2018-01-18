using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class HCsbLibStatic
    {
        public static bool HLayeredArray2_doselftest = HDebug.IsDebuggerAttached;
        public static void HLayeredArray2_selftest()
        {
            if(HLayeredArray2_doselftest == false)
                return;
            HLayeredArray2_doselftest = false;

            HDebug.Assert((new HLayeredArray2<int>(0, 1, 2)).BlockCapacity == 1);
            HDebug.Assert((new HLayeredArray2<int>(0, 2, 2)).BlockCapacity == 1);
            HDebug.Assert((new HLayeredArray2<int>(0, 3, 2)).BlockCapacity == 2);
            HDebug.Assert((new HLayeredArray2<int>(0, 4, 2)).BlockCapacity == 2);
            HDebug.Assert((new HLayeredArray2<int>(0, 5, 2)).BlockCapacity == 3);
            HDebug.Assert((new HLayeredArray2<int>(0, 6, 2)).BlockCapacity == 3);

            HLayeredArray2<int> arr = new HLayeredArray2<int>(0, 5, 3);
            HDebug.Assert(arr.Count == 0, arr[0] == 0, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 0);

            arr[0] = 1; HDebug.Assert(arr.BlockCount == 1, arr[0] == 1, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 0);
            arr[1] = 2; HDebug.Assert(arr.BlockCount == 1, arr[0] == 1, arr[1] == 2, arr[2] == 0, arr[3] == 0, arr[4] == 0);
            arr[2] = 3; HDebug.Assert(arr.BlockCount == 1, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 0, arr[4] == 0);
            arr[3] = 4; HDebug.Assert(arr.BlockCount == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 0);
            arr[4] = 5; HDebug.Assert(arr.BlockCount == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5);

            arr[0] = 0; HDebug.Assert(arr.BlockCount == 2, arr[0] == 0, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5);
            arr[3] = 0; HDebug.Assert(arr.BlockCount == 2, arr[0] == 0, arr[1] == 2, arr[2] == 3, arr[3] == 0, arr[4] == 5);
            arr[1] = 0; HDebug.Assert(arr.BlockCount == 2, arr[0] == 0, arr[1] == 0, arr[2] == 3, arr[3] == 0, arr[4] == 5);
            arr[2] = 0; HDebug.Assert(arr.BlockCount == 1, arr[0] == 0, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 5);
            arr[4] = 0; HDebug.Assert(arr.BlockCount == 0, arr[0] == 0, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 0);
        }
    }
    public class HLayeredArray2<T>
        where T : IEquatable<T>
    {
        long                capacity;
        long                blocksize;
        readonly T          def;
        long                count;
        HLayeredArray1<T>[] arr;
        Func<long, HLayeredArray1<T>> newarri;
        public HLayeredArray2(T def, long capacity, long blocksize)
        {
            if(HCsbLibStatic.HLayeredArray2_doselftest)
                HCsbLibStatic.HLayeredArray2_selftest();
            this.capacity   = capacity;
            this.blocksize  = blocksize;
            this.newarri    = delegate (long size) { return new HLayeredArray1<T>(def, size); };
            this.def        = def;
            this.count      = 0;
            this.arr        = new HLayeredArray1<T>[(capacity-1)/blocksize+1];
        }
        public long BlockCapacity
        {
            get
            {
                return arr.Length;
            }
        }
        public long BlockCount
        {
            get
            {
                return count;
            }
        }
        public long Count
        {
            get
            {
                long lcount = 0;
                foreach(var arri in arr)
                    if(arri != null)
                        lcount += arri.Count;
                return lcount;
            }
        }
        public T this[long index]
        {
            get { return GetAt(index); }
            set { SetAt(index, value); }
        }
        public T GetAt(long index)
        {
            long idx1 = index % blocksize;
            long idx2 = index / blocksize;
            var  arri = arr[idx2];
            if(arri == null)
                return def;
            return arri[idx1];
        }
        public void SetAt(long index, T value)
        {
            long idx1 = index % blocksize;
            long idx2 = index / blocksize;
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
                    if(idx2 == arr.Length-1) arr[idx2] = newarri((capacity-1) % blocksize+1);
                    else                     arr[idx2] = newarri(blocksize);
                    arr[idx2][idx1] = value;
                    count++;
                    HDebug.Assert(count <= capacity);
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
                    arr[idx2][idx1] = value;
                }
            }
        }
    }
}
