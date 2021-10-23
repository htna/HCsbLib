using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public struct SVector3 : IVector<double>
    {
        public double v0, v1, v2;
        ///////////////////////////////////////////////////
        // IVector<double>
        public long SizeLong { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return 3; } }
        public int   Size    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return 3; } }
        public double this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if     (i==0) return v0;
                else if(i==1) return v1;
                else if(i==2) return v2;
                throw new IndexOutOfRangeException();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if     (i==0) { v0=value; return; }
                else if(i==1) { v1=value; return; }
                else if(i==2) { v2=value; return; }
                throw new IndexOutOfRangeException();
            }
        }
        public double this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if     (i==0) return v0;
                else if(i==1) return v1;
                else if(i==2) return v2;
                throw new IndexOutOfRangeException();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if     (i==0) { v0=value; return; }
                else if(i==1) { v1=value; return; }
                else if(i==2) { v2=value; return; }
                throw new IndexOutOfRangeException();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double[] ToArray()
        {
            return new double[] { v0, v1, v2 };
        }
        // IVector<double>
        ///////////////////////////////////////////////////

        public SVector3( double v0, double v1, double v2 )
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
        public static SVector3 Zeros()
        {
            return new SVector3();
        }
        public static bool EqualContents(SVector3 a, SVector3 b)
        {
            if(a.v0 != b.v0) return false;
            if(a.v1 != b.v1) return false;
            if(a.v2 != b.v2) return false;
            return true;
        }
    }
}
