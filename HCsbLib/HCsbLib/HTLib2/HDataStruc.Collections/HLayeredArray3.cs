using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class HCsbLibStatic
    {
        public static bool HLayeredArray3_doselftest = HDebug.IsDebuggerAttached;
        public static void HLayeredArray3_selftest()
        {
            if(HLayeredArray3_doselftest == false)
                return;
            HLayeredArray3_doselftest = false;
            HLayeredArray3<int> arr = new HLayeredArray3<int>(0, 8, 2);
            HDebug.Assert(arr.Count == 0, arr[0] == 0, arr[1] == 0, arr[2] == 0, arr[3] == 0);

            arr[0] = 1; HDebug.Assert(arr.Count == 1, arr[0] == 1, arr[1] == 0, arr[2] == 0, arr[3] == 0, arr[4] == 0, arr[5] == 0, arr[6] == 0, arr[7] == 0);
            arr[1] = 2; HDebug.Assert(arr.Count == 1, arr[0] == 1, arr[1] == 2, arr[2] == 0, arr[3] == 0, arr[4] == 0, arr[5] == 0, arr[6] == 0, arr[7] == 0);
            arr[2] = 3; HDebug.Assert(arr.Count == 1, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 0, arr[4] == 0, arr[5] == 0, arr[6] == 0, arr[7] == 0);
            arr[3] = 4; HDebug.Assert(arr.Count == 1, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 0, arr[5] == 0, arr[6] == 0, arr[7] == 0);
            arr[4] = 5; HDebug.Assert(arr.Count == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5, arr[5] == 0, arr[6] == 0, arr[7] == 0);
            arr[5] = 6; HDebug.Assert(arr.Count == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5, arr[5] == 6, arr[6] == 0, arr[7] == 0);
            arr[6] = 7; HDebug.Assert(arr.Count == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5, arr[5] == 6, arr[6] == 7, arr[7] == 0);
            arr[7] = 8; HDebug.Assert(arr.Count == 2, arr[0] == 1, arr[1] == 2, arr[2] == 3, arr[3] == 4, arr[4] == 5, arr[5] == 6, arr[6] == 7, arr[7] == 8);
        }
    }
    public class HLayeredArray3<T>
        where T : IEquatable<T>
    {
        long                Size;
        long                blocksize, blocksize2;
        readonly T          def;
        long                arrcount;
        HLayeredArray2<T>[] arr;
        Func<long, HLayeredArray2<T>> newarri;
        public HLayeredArray3(T def, long size, long blocksize)
        {
            if(HCsbLibStatic.HLayeredArray3_doselftest)
                HCsbLibStatic.HLayeredArray3_selftest();
            this.Size       = size;
            this.blocksize  = blocksize;
            this.blocksize2 = blocksize*blocksize;
            this.newarri    = delegate (long lsize) { return new HLayeredArray2<T>(def, lsize, blocksize); };
            this.def        = def;
            this.arrcount   = 0;
            this.arr        = new HLayeredArray2<T>[(size-1)/blocksize2+1];
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
            long idx2 = index % blocksize2;
            long idx3 = index / blocksize2;
            var  arri = arr[idx3];
            if(arri == null)
                return def;
            return arri[idx2];
        }
        public void SetAt(long index, T value)
        {
            long idx2 = index % blocksize2;
            long idx3 = index / blocksize2;
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
                    if(idx3 == arr.Length-1) arr[idx3] = newarri((Size-1) % blocksize2+1);
                    else                     arr[idx3] = newarri(blocksize2);
                    arr[idx3][idx2] = value;
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
                    arr[idx3][idx2] = def;
                    if(arr[idx3].Count == 0)
                    {
                        arr[idx3] = null;
                        arrcount--;
                        HDebug.Assert(arrcount >= 0);
                    }
                }
                else
                {
                    // arr[i] != null
                    // value  != def
                    arr[idx3][idx2] = value;
                }
            }
        }
    }
}
