using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
        public static MatrixByArr InvSymm(MatrixByArr A)
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

                MatrixByArr tB1 = InvSymm(tA);

                HDebug.AssertTolerance(0.0001, tB0-tB1);
                HDebug.AssertTolerance(0.0001, tI-tA*tB1);
                HDebug.AssertTolerance(0.0001, tI-tB1*tA);
            }

            HDebug.Assert(A.ColSize == A.RowSize);
            //double[] eigval;
            //double[,] eigvec;

            bool success = false;//alglib.smatrixevd(A, A.ColSize, 1, false, out eigval, out eigvec);

            if(success == false)
            {
                HDebug.Assert(false);
                return null;
            }
            HDebug.Assert(false);
            return null;
        }
    }
}
