using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public struct SMatrix3x3 : IMatrix<double>
    {
        public double v00, v01, v02;
        public double v10, v11, v12;
        public double v20, v21, v22;
        ///////////////////////////////////////////////////
        // IMatrix<double>
        public int ColSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return 3; } }
        public int RowSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return 3; } }
        public double this[int  c, int  r]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(c==0)
                {
                    if     (r==0) return v00;
                    else if(r==1) return v01;
                    else if(r==2) return v02;
                }
                else if(c==1)
                {
                    if     (r==0) return v10;
                    else if(r==1) return v11;
                    else if(r==2) return v12;
                }
                else if(c==2)
                {
                    if     (r==0) return v20;
                    else if(r==1) return v21;
                    else if(r==2) return v22;
                }
                throw new IndexOutOfRangeException();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if(c==0)
                {
                    if     (r==0) { v00=value; return; }
                    else if(r==1) { v01=value; return; }
                    else if(r==2) { v02=value; return; }
                }
                else if(c==1)
                {
                    if     (r==0) { v10=value; return; }
                    else if(r==1) { v11=value; return; }
                    else if(r==2) { v12=value; return; }
                }
                else if(c==2)
                {
                    if     (r==0) { v20=value; return; }
                    else if(r==1) { v21=value; return; }
                    else if(r==2) { v22=value; return; }
                }
                throw new IndexOutOfRangeException();
            }
        }
        public double this[long c, long r]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { throw new NotImplementedException(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] set { throw new NotImplementedException(); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public double[,] ToArray()
        {
            return new double[3,3]
            {
                { v00, v01, v02 },
                { v10, v11, v12 },
                { v20, v21, v22 },
            };
        }
        // IMatrix<double>
        ///////////////////////////////////////////////////

        public SMatrix3x3
            ( double v00, double v01, double v02
            , double v10, double v11, double v12
            , double v20, double v21, double v22
            )
        {
            this.v00 = v00;
            this.v01 = v01;
            this.v02 = v02;
            this.v10 = v10;
            this.v11 = v11;
            this.v12 = v12;
            this.v20 = v20;
            this.v21 = v21;
            this.v22 = v22;
        }
        public static SMatrix3x3 Zeros()
        {
            return new SMatrix3x3();
        }
        public static bool Equals(SMatrix3x3 a, SMatrix3x3 b)
        {
            if(a.v00 != b.v00) return false;
            if(a.v01 != b.v01) return false;
            if(a.v02 != b.v02) return false;
            if(a.v10 != b.v10) return false;
            if(a.v11 != b.v11) return false;
            if(a.v12 != b.v12) return false;
            if(a.v20 != b.v20) return false;
            if(a.v21 != b.v21) return false;
            if(a.v22 != b.v22) return false;
            return true;
        }
        public override string ToString()
        {
            //return "no display...";
            StringBuilder sb = new StringBuilder();
			sb.Append("SMatrix3x3 ["+ColSize+","+RowSize+"] ");
            sb.Append(sb);
            //str.Append(HToString("0.00000", null, "{{", "}}", ", ", "}, {", 100));
            MatrixStatic.HToString
                ( this, sb
                , "0.00000", null, "{{", "}}", ", ", "}, {", 100
                );
            return sb.ToString();
        }
        public void UpdateByMul(double mul)
        {
            v00 *= mul;    v01 *= mul;    v02 *= mul;
            v10 *= mul;    v11 *= mul;    v12 *= mul;
            v20 *= mul;    v21 *= mul;    v22 *= mul;
        }
        public void UpdateByAdd(double add)
        {
            v00 += add;    v01 += add;    v02 += add;
            v10 += add;    v11 += add;    v12 += add;
            v20 += add;    v21 += add;    v22 += add;
        }
        public void UpdateByAdd(SMatrix3x3 add)
        {
            v00 += add.v00;    v01 += add.v01;    v02 += add.v02;
            v10 += add.v10;    v11 += add.v11;    v12 += add.v12;
            v20 += add.v20;    v21 += add.v21;    v22 += add.v22;
        }
        public SMatrix3x3 Tr()
        {
            return new SMatrix3x3
            ( v00, v10, v20
            , v01, v11, v21
            , v02, v12, v22
            );
        }
    }
}
