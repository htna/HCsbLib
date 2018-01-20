using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Matrix : Matrix<double>
    {
        public static Matrix IdentityMatrix(int size)
        {
            Matrix mat = new Matrix(size, size);
            for(int i=0; i<size; i++)
                mat[i, i] = 1;
            return mat;
        }

        public Matrix Tr()
        {
            Matrix tr = new Matrix(RowSize, ColSize);
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    tr[r, c] = this[c, r];
            return tr;
        }

        public static HFunc2<Matrix, Matrix, Vector, bool> funcEigSymm = null;
        public static bool EigSymm(Matrix mat, out Matrix V, out Vector D)
        {
            if(funcEigSymm == null)
                throw new Exception("funcEigSymm is not defined");
            return funcEigSymm(mat, out V, out D);
        }

        public static HFunc1<Matrix, Matrix, bool> funcPinv = null;
        public static bool Pinv(Matrix mat, out Matrix invmat)
        {
            if(funcPinv == null)
                throw new Exception("funcInv is not defined");
            
            return funcPinv(mat, out invmat);
        }

        public static HFunc1<Matrix, Matrix, bool> funcInvSymm = null;
        public static bool InvSymm(Matrix mat, out Matrix invmat)
        {
            if(funcInvSymm != null)
                return funcInvSymm(mat, out invmat);
            if(funcEigSymm != null)
            {
                Debug.Assert(mat.ColSize == mat.RowSize);
                int size = mat.ColSize;
                Matrix V;
                Vector D;
                Debug.Verify(funcEigSymm(mat, out V, out D));
                Vector invD = new double[size]; for(int i=0; i<size; i++) { if(D[i] != 0) invD[i] = 1 / D[i]; }
                Debug.Verify(Mul(new Matrix[]{V, invD.ToDiagMatrix(), V.Tr()}, out invmat));
                if(Debug.IsDebuggerAttached && mat.ColSize==mat.RowSize)
                {
                    Matrix I = Matrix.IdentityMatrix(mat.ColSize);
                    double tolbase = HMath.HAvg(mat.ToArray().HToArray1D().HAvg() + invmat.ToArray().HToArray1D().HAvg());
                    double err1 = (mat * invmat - I).ToArray().HAbs().HMax();
                    Debug.AssertTolerant(0.00000001 * tolbase, err1);
                    double err2 = (invmat * mat - I).ToArray().HAbs().HMax();
                    Debug.AssertTolerant(0.00000001 * tolbase, err2);
                }
                return true;
            }

            throw new Exception("funcInv is not defined");
        }
    }
}
