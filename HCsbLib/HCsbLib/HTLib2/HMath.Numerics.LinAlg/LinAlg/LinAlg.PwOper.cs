using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public static partial class LinAlg
    {
        public static Matrix PwDiv(Matrix A, Matrix B, bool bSkipDivZero, double mulA=1)
        {
            int colsize = A.ColSize; HDebug.Assert(A.ColSize == B.ColSize);
            int rowsize = A.RowSize; HDebug.Assert(A.RowSize == B.RowSize);
            Matrix pwdiv = Matrix.Zeros(colsize, rowsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                {
                    if(bSkipDivZero && B[c, r] == 0)
                        continue;
                    pwdiv[c, r] = mulA * A[c, r] / B[c, r];
                }
            return pwdiv;
        }
    }
}
