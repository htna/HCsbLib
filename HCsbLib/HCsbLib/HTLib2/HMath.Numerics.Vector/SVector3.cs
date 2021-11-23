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
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SVector3 ["+Size+"] ");
            HStatic.HToString(this, sb, "0.00000", null, "{", "}", ", ");
            return sb.ToString();
        }
        public void UpdateByMul(double mul)
        {
            v0 *= mul;
            v1 *= mul;
            v2 *= mul;
        }
        public void UpdateByAdd(double add)
        {
            v0 += add;
            v1 += add;
            v2 += add;
        }
        public void UpdateByAdd(SVector3 add)
        {
            v0 += add.v0;
            v1 += add.v1;
            v2 += add.v2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SVector3 operator+(SVector3 l, SVector3 r)
		{
			SVector3 result = default;
            result.v0 = l.v0 + r.v0;
            result.v1 = l.v1 + r.v1;
            result.v2 = l.v2 + r.v2;
			return result;
		}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SVector3 operator-(SVector3 l, SVector3 r)
		{
			SVector3 result = default;
            result.v0 = l.v0 - r.v0;
            result.v1 = l.v1 - r.v1;
            result.v2 = l.v2 - r.v2;
			return result;
		}
  //      [MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static SVector3 operator -(SVector3 v)
		//{
		//	int length = v.Length;
		//	SVector3 result = new SVector3(length);
		//	for(int i=0; i<length; i++)
		//		result[i] = -1 * v[i];
		//	return result;
		//}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SVector3 operator*(double l, SVector3 r)
		{
			SVector3 result = default;
            result.v0 = l * r.v0;
            result.v1 = l * r.v1;
            result.v2 = l * r.v2;
			return result;
		}
		public static SVector3 operator*(SVector3 l, double r)
		{
			SVector3 result = default;
            result.v0 = l.v0 * r;
            result.v1 = l.v1 * r;
            result.v2 = l.v2 * r;
			return result;
		}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SVector3 operator/(SVector3 l, double r)
		{
			SVector3 result = default;
            result.v0 = l.v0 / r;
            result.v1 = l.v1 / r;
            result.v2 = l.v2 / r;
			return result;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SVector3 operator*(SMatrix3x3 mat, SVector3 vec)
        {
            return new SVector3
                ( mat.v00*vec.v0  +  mat.v01*vec.v1  +  mat.v02*vec.v2
                , mat.v10*vec.v0  +  mat.v11*vec.v1  +  mat.v12*vec.v2
                , mat.v20*vec.v0  +  mat.v21*vec.v1  +  mat.v22*vec.v2
                );
        }
    }
}
