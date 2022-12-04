using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public struct SVector3 : IVector<double>, IBinarySerializable
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
        ////////////////////////////////////////////////////////////////////////////////////
        // IBinarySerializable
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BinarySerialize(HBinaryWriter writer)
        {
            writer.Write(v0);
            writer.Write(v1);
            writer.Write(v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SVector3(HBinaryReader reader)
        {
            reader.Read(out v0);
            reader.Read(out v1);
            reader.Read(out v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BinarySerialize(ref SVector3 vec, HBinaryWriter writer)
        {
            writer.Write(vec.v0);
            writer.Write(vec.v1);
            writer.Write(vec.v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BinaryDeserialize(out SVector3 vec, HBinaryReader reader)
        {
            reader.Read(out vec.v0);
            reader.Read(out vec.v1);
            reader.Read(out vec.v2);
        }
        // IBinarySerializable
        ////////////////////////////////////////////////////////////////////////////////////

        public SVector3( double v0, double v1, double v2 )
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
        public double Dist2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                double result = 0;
                for(int i=0; i<Size; i++)
                    result += this[i] * this[i];
                return result;
            }
        }
        public double Dist
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                double dist = Math.Sqrt(Dist2);
                return dist;
            }
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
        public void UpdateByAdd(IVector<double> add)
        {
            if(add.Size != 3)
                throw new Exception();
            v0 += add[0];
            v1 += add[1];
            v2 += add[2];
        }
        public void UpdateByAdd(IVector<double> add, double add_mul)
        {
            if(add.Size != 3)
                throw new Exception();
            v0 += (add[0] * add_mul);
            v1 += (add[1] * add_mul);
            v2 += (add[2] * add_mul);
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
