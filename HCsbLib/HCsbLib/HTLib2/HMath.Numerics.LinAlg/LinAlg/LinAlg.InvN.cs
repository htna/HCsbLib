using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HTLib2
{
	//////////////////////////////////////////////////////////////////////////
	// class for Inverse Matrix Functions
	//////////////////////////////////////////////////////////////////////////
    public static partial class LinAlg
	{
		static public MatrixByArr Inv2x2(MatrixByArr _this)
		{
            if(HDebug.Selftest())
            {
                //  >>A = [ 1, 2 ; 3, 4 ];
                //  >>invA = inv(A)
                //  invA =
                //     -2.0000    1.0000
                //      1.5000   -0.5000
                //  >> invA* A
                //  ans =
                //      1.0000         0
                //      0.0000    1.0000
                //  >> A*invA
                //  ans =
                //      1.0000         0
                //      0.0000    1.0000
                MatrixByArr _A = new double[2,2] { { 1, 2 }, { 3, 4 } };
                MatrixByArr _invA = Inv2x2(_A);
                HDebug.Assert(_invA[0,0] == -2 ); HDebug.Assert(_invA[0,1] == 1   );
                HDebug.Assert(_invA[1,0] == 1.5); HDebug.Assert(_invA[1,1] == -0.5);
            }
            
            // http://www.cvl.iis.u-tokyo.ac.jp/~miyazaki/tech/teche23.html
			if(_this.RowSize != 2 || _this.ColSize != 2)
				return null;

			double a = _this[0, 0];
			double b = _this[0, 1];
			double c = _this[1, 0];
			double d = _this[1, 1];
			double detA = a*d - b*c;
			MatrixByArr inv = new MatrixByArr(2, 2);
			inv[0, 0] =  d;
			inv[0, 1] = -b;
			inv[1, 0] = -c;
			inv[1, 1] =  a;
			inv /= detA;
			return inv;
		}
		static public MatrixByArr Inv3x3(MatrixByArr _this)
		{
            if(HDebug.Selftest())
            {
                MatrixByArr tA = new double[,]{{1,2,3},
                                          {2,9,5},
                                          {3,5,6}};
                MatrixByArr tB0 = new double[,]{{-1.8125, -0.1875,  1.0625},
                                           {-0.1875,  0.1875, -0.0625},
                                           { 1.0625, -0.0625, -0.3125}};
                MatrixByArr tI = LinAlg.Eye(3);

                MatrixByArr tB1 = Inv3x3(tA);

                HDebug.AssertTolerance(0.0001, tB0-tB1);
                HDebug.AssertTolerance(0.0001, tI-tA*tB1);
                HDebug.AssertTolerance(0.0001, tI-tB1*tA);
            }
            
            // http://www.cvl.iis.u-tokyo.ac.jp/~miyazaki/tech/teche23.html
			if(_this.RowSize != 3 || _this.ColSize != 3)
				return null;

			double a11 = _this[0, 0];
			double a12 = _this[0, 1];
			double a13 = _this[0, 2];
			double a21 = _this[1, 0];
			double a22 = _this[1, 1];
			double a23 = _this[1, 2];
			double a31 = _this[2, 0];
			double a32 = _this[2, 1];
			double a33 = _this[2, 2];
			double detA = a11*a22*a33 + a21*a32*a13 + a31*a12*a23
						- a11*a32*a23 - a31*a22*a13 - a21*a12*a33;

			MatrixByArr inv = new MatrixByArr(3, 3);
			inv[0, 0] = a22*a33 - a23*a32;
			inv[0, 1] = a13*a32 - a12*a33;
			inv[0, 2] = a12*a23 - a13*a22;
			inv[1, 0] = a23*a31 - a21*a33;
			inv[1, 1] = a11*a33 - a13*a31;
			inv[1, 2] = a13*a21 - a11*a23;
			inv[2, 0] = a21*a32 - a22*a31;
			inv[2, 1] = a12*a31 - a11*a32;
			inv[2, 2] = a11*a22 - a12*a21;
			inv /= detA;

			if(HDebug.IsDebuggerAttached)
			{
				MatrixByArr I33 = _this * inv;
				for(int r=0; r<3; r++)
				{
					for(int c=0; c<3; c++)
					{
						if(r==c)
							Debug.Assert(Math.Abs(I33[r, c]-1) < 0.00001);
						else
							Debug.Assert(Math.Abs(I33[r, c]-0) < 0.00001);
					}
				}
				I33 = inv * _this;
				for(int r=0; r<3; r++)
				{
					for(int c=0; c<3; c++)
					{
						if(r==c)
							Debug.Assert(Math.Abs(I33[r, c]-1) < 0.00001);
						else
							Debug.Assert(Math.Abs(I33[r, c]-0) < 0.00001);
					}
				}
			}
			return inv;
		}
		static public MatrixByArr Inv4x4(MatrixByArr _this)
		{
            if(HDebug.Selftest())
            {
                Debug.Assert(false);
            }

			//////////////////////////////////////////////////////////////////////////
			// http://www.koders.com/cpp/fidFB7C4F93FDDB86E33EB66D177335BA81D86E58B5.aspx
			// Matrix.cpp
			// bool idMat4::InverseFastSelf( void )
			//////////////////////////////////////////////////////////////////////////
			// 	//	6*8+2*6 = 60 multiplications
			// 	//		2*1 =  2 divisions
			// 	idMat2 r0, r1, r2, r3;
			// 	float a, det, invDet;
			// 	float *mat = reinterpret_cast<float *>(this);
			// 
			// 	// r0 = m0.Inverse();
			// 	det = mat[0*4+0] * mat[1*4+1] - mat[0*4+1] * mat[1*4+0];
			// 
			// 	if ( idMath::Fabs( det ) < MATRIX_INVERSE_EPSILON ) {
			// 		return false;
			// 	}
			// 
			// 	invDet = 1.0f / det;
			// 
			// 	r0[0][0] =   mat[1*4+1] * invDet;
			// 	r0[0][1] = - mat[0*4+1] * invDet;
			// 	r0[1][0] = - mat[1*4+0] * invDet;
			// 	r0[1][1] =   mat[0*4+0] * invDet;
			// 
			// 	// r1 = r0 * m1;
			// 	r1[0][0] = r0[0][0] * mat[0*4+2] + r0[0][1] * mat[1*4+2];
			// 	r1[0][1] = r0[0][0] * mat[0*4+3] + r0[0][1] * mat[1*4+3];
			// 	r1[1][0] = r0[1][0] * mat[0*4+2] + r0[1][1] * mat[1*4+2];
			// 	r1[1][1] = r0[1][0] * mat[0*4+3] + r0[1][1] * mat[1*4+3];
			// 
			// 	// r2 = m2 * r1;
			// 	r2[0][0] = mat[2*4+0] * r1[0][0] + mat[2*4+1] * r1[1][0];
			// 	r2[0][1] = mat[2*4+0] * r1[0][1] + mat[2*4+1] * r1[1][1];
			// 	r2[1][0] = mat[3*4+0] * r1[0][0] + mat[3*4+1] * r1[1][0];
			// 	r2[1][1] = mat[3*4+0] * r1[0][1] + mat[3*4+1] * r1[1][1];
			// 
			// 	// r3 = r2 - m3;
			// 	r3[0][0] = r2[0][0] - mat[2*4+2];
			// 	r3[0][1] = r2[0][1] - mat[2*4+3];
			// 	r3[1][0] = r2[1][0] - mat[3*4+2];
			// 	r3[1][1] = r2[1][1] - mat[3*4+3];
			// 
			// 	// r3.InverseSelf();
			// 	det = r3[0][0] * r3[1][1] - r3[0][1] * r3[1][0];
			// 
			// 	if ( idMath::Fabs( det ) < MATRIX_INVERSE_EPSILON ) {
			// 		return false;
			// 	}
			// 
			// 	invDet = 1.0f / det;
			// 
			// 	a = r3[0][0];
			// 	r3[0][0] =   r3[1][1] * invDet;
			// 	r3[0][1] = - r3[0][1] * invDet;
			// 	r3[1][0] = - r3[1][0] * invDet;
			// 	r3[1][1] =   a * invDet;
			// 
			// 	// r2 = m2 * r0;
			// 	r2[0][0] = mat[2*4+0] * r0[0][0] + mat[2*4+1] * r0[1][0];
			// 	r2[0][1] = mat[2*4+0] * r0[0][1] + mat[2*4+1] * r0[1][1];
			// 	r2[1][0] = mat[3*4+0] * r0[0][0] + mat[3*4+1] * r0[1][0];
			// 	r2[1][1] = mat[3*4+0] * r0[0][1] + mat[3*4+1] * r0[1][1];
			// 
			// 	// m2 = r3 * r2;
			// 	mat[2*4+0] = r3[0][0] * r2[0][0] + r3[0][1] * r2[1][0];
			// 	mat[2*4+1] = r3[0][0] * r2[0][1] + r3[0][1] * r2[1][1];
			// 	mat[3*4+0] = r3[1][0] * r2[0][0] + r3[1][1] * r2[1][0];
			// 	mat[3*4+1] = r3[1][0] * r2[0][1] + r3[1][1] * r2[1][1];
			// 
			// 	// m0 = r0 - r1 * m2;
			// 	mat[0*4+0] = r0[0][0] - r1[0][0] * mat[2*4+0] - r1[0][1] * mat[3*4+0];
			// 	mat[0*4+1] = r0[0][1] - r1[0][0] * mat[2*4+1] - r1[0][1] * mat[3*4+1];
			// 	mat[1*4+0] = r0[1][0] - r1[1][0] * mat[2*4+0] - r1[1][1] * mat[3*4+0];
			// 	mat[1*4+1] = r0[1][1] - r1[1][0] * mat[2*4+1] - r1[1][1] * mat[3*4+1];
			// 
			// 	// m1 = r1 * r3;
			// 	mat[0*4+2] = r1[0][0] * r3[0][0] + r1[0][1] * r3[1][0];
			// 	mat[0*4+3] = r1[0][0] * r3[0][1] + r1[0][1] * r3[1][1];
			// 	mat[1*4+2] = r1[1][0] * r3[0][0] + r1[1][1] * r3[1][0];
			// 	mat[1*4+3] = r1[1][0] * r3[0][1] + r1[1][1] * r3[1][1];
			// 
			// 	// m3 = -r3;
			// 	mat[2*4+2] = -r3[0][0];
			// 	mat[2*4+3] = -r3[0][1];
			// 	mat[3*4+2] = -r3[1][0];
			// 	mat[3*4+3] = -r3[1][1];
			// 
			// 	return true;

			if(_this.RowSize != 4 || _this.ColSize != 4)
				return null;

			MatrixByArr mat = new MatrixByArr(_this);
			const double MATRIX_INVERSE_EPSILON = 0.000000001;

			//	6*8+2*6 = 60 multiplications
			//		2*1 =  2 divisions
			double det, invDet;

			// r0 = m0.Inverse();
			det = mat[0,0] * mat[1,1] - mat[0,1] * mat[1,0];

			if(Math.Abs(det) < MATRIX_INVERSE_EPSILON)
			{
				Debug.Assert(false);
				return null;
			}

			invDet = 1.0f / det;

			double r0_00 =   mat[1,1] * invDet;
			double r0_01 = -mat[0,1] * invDet;
			double r0_10 = -mat[1,0] * invDet;
			double r0_11 =   mat[0,0] * invDet;

			// r1 = r0 * m1;
			double r1_00 = r0_00 * mat[0,2] + r0_01 * mat[1,2];
			double r1_01 = r0_00 * mat[0,3] + r0_01 * mat[1,3];
			double r1_10 = r0_10 * mat[0,2] + r0_11 * mat[1,2];
			double r1_11 = r0_10 * mat[0,3] + r0_11 * mat[1,3];

			// r2 = m2 * r1;
			double r2_00 = mat[2,0] * r1_00 + mat[2,1] * r1_10;
			double r2_01 = mat[2,0] * r1_01 + mat[2,1] * r1_11;
			double r2_10 = mat[3,0] * r1_00 + mat[3,1] * r1_10;
			double r2_11 = mat[3,0] * r1_01 + mat[3,1] * r1_11;

			// r3 = r2 - m3;
			double r3_00 = r2_00 - mat[2,2];
			double r3_01 = r2_01 - mat[2,3];
			double r3_10 = r2_10 - mat[3,2];
			double r3_11 = r2_11 - mat[3,3];

			// r3.InverseSelf();
			det = r3_00 * r3_11 - r3_01 * r3_10;

			if(Math.Abs(det) < MATRIX_INVERSE_EPSILON)
			{
				Debug.Assert(false);
				return null;
			}

			invDet = 1.0f / det;

			double r3_00_prv = r3_00;
			r3_00 =   r3_11 * invDet;
			r3_01 = -r3_01 * invDet;
			r3_10 = -r3_10 * invDet;
			r3_11 =   r3_00_prv * invDet;

			// r2 = m2 * r0;
			r2_00 = mat[2,0] * r0_00 + mat[2,1] * r0_10;
			r2_01 = mat[2,0] * r0_01 + mat[2,1] * r0_11;
			r2_10 = mat[3,0] * r0_00 + mat[3,1] * r0_10;
			r2_11 = mat[3,0] * r0_01 + mat[3,1] * r0_11;

			// m2 = r3 * r2;
			mat[2,0] = r3_00 * r2_00 + r3_01 * r2_10;
			mat[2,1] = r3_00 * r2_01 + r3_01 * r2_11;
			mat[3,0] = r3_10 * r2_00 + r3_11 * r2_10;
			mat[3,1] = r3_10 * r2_01 + r3_11 * r2_11;

			// m0 = r0 - r1 * m2;
			mat[0,0] = r0_00 - r1_00 * mat[2,0] - r1_01 * mat[3,0];
			mat[0,1] = r0_01 - r1_00 * mat[2,1] - r1_01 * mat[3,1];
			mat[1,0] = r0_10 - r1_10 * mat[2,0] - r1_11 * mat[3,0];
			mat[1,1] = r0_11 - r1_10 * mat[2,1] - r1_11 * mat[3,1];

			// m1 = r1 * r3;
			mat[0,2] = r1_00 * r3_00 + r1_01 * r3_10;
			mat[0,3] = r1_00 * r3_01 + r1_01 * r3_11;
			mat[1,2] = r1_10 * r3_00 + r1_11 * r3_10;
			mat[1,3] = r1_10 * r3_01 + r1_11 * r3_11;

			// m3 = -r3;
			mat[2,2] = -r3_00;
			mat[2,3] = -r3_01;
			mat[3,2] = -r3_10;
			mat[3,3] = -r3_11;

			return mat;
		}
	}
}
