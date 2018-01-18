/*
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

            HDebug.Assert((new HLayeredArray2<int>(0, 1, 2)).ArrLength == 1);
            HDebug.Assert((new HLayeredArray2<int>(0, 2, 2)).ArrLength == 1);
            HDebug.Assert((new HLayeredArray2<int>(0, 3, 2)).ArrLength == 2);
            HDebug.Assert((new HLayeredArray2<int>(0, 4, 2)).ArrLength == 2);
            HDebug.Assert((new HLayeredArray2<int>(0, 5, 2)).ArrLength == 3);
            HDebug.Assert((new HLayeredArray2<int>(0, 6, 2)).ArrLength == 3);

            HLayeredArray2<int> arr = new HLayeredArray2<int>(0, 5, 3);
            HDebug.Assert(arr.Count == 0, arr[0] == 0, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 0);

            arr[0] = 1; HDebug.Assert(arr.ArrCount == 1, arr[0] == 1, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 0);
            arr[1] = 2; HDebug.Assert(arr.ArrCount == 1, arr[0] == 1, arr[1] == 2, arr[2] == 0, arr[3] == 0, arr[4] == 0);
            arr[2] = 3; HDebug.Assert(arr.ArrCount == 1, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 0, arr[4] == 0);
            arr[3] = 4; HDebug.Assert(arr.ArrCount == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 0);
            arr[4] = 5; HDebug.Assert(arr.ArrCount == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5);

            arr[0] = 0; HDebug.Assert(arr.ArrCount == 2, arr[0] == 0, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5);
            arr[3] = 0; HDebug.Assert(arr.ArrCount == 2, arr[0] == 0, arr[1] == 2, arr[2] == 3, arr[3] == 0, arr[4] == 5);
            arr[1] = 0; HDebug.Assert(arr.ArrCount == 2, arr[0] == 0, arr[1] == 0, arr[2] == 3, arr[3] == 0, arr[4] == 5);
            arr[2] = 0; HDebug.Assert(arr.ArrCount == 1, arr[0] == 0, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 5);
            arr[4] = 0; HDebug.Assert(arr.ArrCount == 0, arr[0] == 0, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 0);
        }
    }
    public class HLayeredArray2<T>
        where T : IEquatable<T>
    {
        public readonly long Size;
        long                 blocksize;
        readonly T           def;
        long                 arrcount;
        HLayeredArray1<T>[]  arr;
        Func<long, HLayeredArray1<T>> newarri;
        public HLayeredArray2(T def, long size, long blocksize)
        {
            if(HCsbLibStatic.HLayeredArray2_doselftest)
                HCsbLibStatic.HLayeredArray2_selftest();
            this.Size       = size;
            this.blocksize  = blocksize;
            this.newarri    = delegate (long lsize) { return new HLayeredArray1<T>(def, lsize); };
            this.def        = def;
            this.arrcount   = 0;
            this.arr        = new HLayeredArray1<T>[(size-1)/blocksize+1];
        }
        public long ArrLength
        {
            get
            {
                return arr.Length;
            }
        }
        public long ArrCount
        {
            get
            {
                return arrcount;
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
                    if(idx2 == arr.Length-1) arr[idx2] = newarri((Size-1) % blocksize+1);
                    else                     arr[idx2] = newarri(blocksize);
                    arr[idx2].SetAt(idx1, value);
                    arrcount++;
                    HDebug.Assert(arrcount <= ArrLength);
                }
            }
            else
            {
                if(vdef)
                {
                    // arr[i] != null
                    // value  == def
                    arr[idx2].SetAt(idx1, def);
                    if(arr[idx2].Count == 0)
                    {
                        arr[idx2] = null;
                        arrcount--;
                        HDebug.Assert(arrcount >= 0);
                    }
                }
                else
                {
                    // arr[i] != null
                    // value  != def
                    arr[idx2].SetAt(idx1, value);
                }
            }
        }
    }
}
*/