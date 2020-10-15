using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
        public static void Eig3Symm
            ( double[,] A
            , out double eig1
            , out double eig2
            , out double eig3
            )
        {
            HDebug.Assert(A.GetLength(0) == 3);
            HDebug.Assert(A.GetLength(1) == 3);
            HDebug.Assert(A[0,1] == A[1,0]);
            HDebug.Assert(A[0,2] == A[2,0]);
            HDebug.Assert(A[1,2] == A[2,1]);
            HDebug.Assert(false); // test
            /// https://en.wikipedia.org/wiki/Eigenvalue_algorithm
            /// % Given a real symmetric 3x3 matrix A, compute the eigenvalues
            /// % Note that acos and cos operate on angles in radians
            /// 
            /// p1 = A(1,2)^2 + A(1,3)^2 + A(2,3)^2
            /// if (p1 == 0) 
            ///    % A is diagonal.
            ///    eig1 = A(1,1)
            ///    eig2 = A(2,2)
            ///    eig3 = A(3,3)
            /// else
            ///    q = trace(A)/3               % trace(A) is the sum of all diagonal values
            ///    p2 = (A(1,1) - q)^2 + (A(2,2) - q)^2 + (A(3,3) - q)^2 + 2 * p1
            ///    p = sqrt(p2 / 6)
            ///    B = (1 / p) * (A - q * I)    % I is the identity matrix
            ///    r = det(B) / 2
            /// 
            ///    % In exact arithmetic for a symmetric matrix  -1 <= r <= 1
            ///    % but computation error can leave it slightly outside this range.
            ///    if (r <= -1) 
            ///       phi = pi / 3
            ///    elseif (r >= 1)
            ///       phi = 0
            ///    else
            ///       phi = acos(r) / 3
            ///    end
            /// 
            ///    % the eigenvalues satisfy eig3 <= eig2 <= eig1
            ///    eig1 = q + 2 * p * cos(phi)
            ///    eig3 = q + 2 * p * cos(phi + (2*pi/3))
            ///    eig2 = 3 * q - eig1 - eig3     % since trace(A) = eig1 + eig2 + eig3
            /// end
            double A11 = A[0,0];
            double A12 = A[0,1];
            double A13 = A[0,2];
            double A22 = A[1,1];
            double A23 = A[1,2];
            double A33 = A[2,2];
            // Given a real symmetric 3x3 matrix A, compute the eigenvalues
            // Note that acos and cos operate on angles in radians
            double p1 = A12*A12 + A13*A13 + A23*A23;
            if (p1 == 0) {
                // A is diagonal.
                eig1 = A11;
                eig2 = A22;
                eig3 = A33;
            } else {
                double trace_A = A11 + A22 + A33;
                double q = trace_A/3;   // trace(A) is the sum of all diagonal values
                double p2 = (A11 - q)*(A11 - q) + (A22 - q)*(A22 - q) + (A33 - q)*(A33 - q) + 2 * p1;
                double p = Math.Sqrt(p2 / 6);
                double r;
                {
                    double[,] B = new double[3,3];
                    //B = (1 / p) * (A - q * I)    % I is the identity matrix
                    B[0,0] = (A[0,0] - q) / p;    B[0,1] = (A[0,1]    ) / p;    B[0,2] = (A[0,2]    ) / p;
                    B[1,0] = (A[1,0]    ) / p;    B[1,1] = (A[1,1] - q) / p;    B[1,2] = (A[1,2]    ) / p;
                    B[2,0] = (A[2,0]    ) / p;    B[2,1] = (A[2,1]    ) / p;    B[2,2] = (A[2,2] - q) / p;
                    r = Det3(B) / 2;
                }
                    
                // In exact arithmetic for a symmetric matrix  -1 <= r <= 1
                // but computation error can leave it slightly outside this range.
                double phi;
                if (r <= -1) 
                    phi = Math.PI / 3;
                else if (r >= 1)
                    phi = 0;
                else
                    phi = Math.Acos(r) / 3;
                    
                // the eigenvalues satisfy eig3 <= eig2 <= eig1
                eig1 = q + 2 * p * Math.Cos(phi);
                eig3 = q + 2 * p * Math.Cos(phi + (2*Math.PI/3));
                eig2 = 3 * q - eig1 - eig3;     // since trace(A) = eig1 + eig2 + eig3
            }
        }
    }
}
