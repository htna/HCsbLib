using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Hess
{
    public partial class STeM
    {
        public static HessMatrix FirstTerm(IList<Vector> caArray, double K_r, HessMatrix hessian)
        {
            if(hessian == null)
                hessian = new double[caArray.Count*3, caArray.Count*3];

            for(int i=0; i<caArray.Count-1; i++)
                FirstTerm(caArray, K_r, hessian, i, i+1);

            return hessian;
        }
        public static void FirstTerm(IList<Vector> caArray, double K_r, HessMatrix hessian, int i, int j)
        {
            int[] idx = new int[] { i, j };
            Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]] };
            Matrix lhess = FirstTerm(lcaArray, K_r);
            double maxabs_lhess = lhess.ToArray().HAbs().HMax();
            HDebug.Assert(maxabs_lhess < 10000);

            for(int c=0; c<2; c++) for(int dc=0; dc<3; dc++)
            for(int r=0; r<2; r++) for(int dr=0; dr<3; dr++)
                hessian[idx[c]*3+dc, idx[r]*3+dr] += lhess[c*3+dc, r*3+dr];

            if(HDebug.IsDebuggerAttached)
            {
                Matrix anm = (2*K_r)* Hess.GetHessAnm(lcaArray, double.PositiveInfinity);
                HDebug.AssertToleranceMatrix(0.00000001, lhess - anm);
            }
        }
        //private static void FirstTerm(IList<Vector> caArray, double K_r, MatrixSparse<MatrixByArr> hessian, int i, int j)
        //{
        //    int[] idx = new int[] { i, j };
        //    Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]] };
        //    MatrixByArr lhess = FirstTerm(lcaArray, K_r);
        //    double maxabs_lhess = lhess.ToArray().HAbs().HMax();
        //    HDebug.Assert(maxabs_lhess < 10000);
        //
        //    for(int c=0; c<2; c++)
        //    for(int r=0; r<2; r++)
        //    {
        //        MatrixByArr lhess_cr = lhess.SubMatrixByFromCount(c*3, 3, r*3, 3);
        //        hessian[idx[c], idx[r]] += lhess_cr;
        //    }
        //
        //    if(HDebug.IsDebuggerAttached)
        //    {
        //        MatrixByArr anm = (2*K_r)* Hess.GetHessAnm(lcaArray, double.PositiveInfinity);
        //        HDebug.AssertTolerance(0.00000001, lhess - anm);
        //    }
        //}
        /// <summary>
        /// Bond Term
        ///  V1(r,r0) = K_r * (r - r0)^2
        /// </summary>
        /// <param name="caArray_"></param>
        /// <param name="K_r"></param>
        /// <returns></returns>
        public static Matrix FirstTerm (IList<Vector> caArray_, double K_r)
        {
            VECTORS caArray  = new VECTORS(caArray_);
            MATRIX hessian = new MATRIX(new double[6,6]);

            // derive the hessian of the first term (off diagonal)
            {
                int i=1;
                int j=2;

                double bx=caArray[i,1] - caArray[j,1];
                double by=caArray[i,2] - caArray[j,2];
                double bz=caArray[i,3] - caArray[j,3];
                double distijsqr = bx*bx + by*by + bz*bz;

                //Hij
                // diagonals of off-diagonal super elements (1st term)

                hessian[3*i-2,3*j-2]       += -2*K_r*bx*bx/distijsqr;
                hessian[3*i-1,3*j-1]       += -2*K_r*by*by/distijsqr;
                hessian[3*i  ,3*j  ]       += -2*K_r*bz*bz/distijsqr;

                // off-diagonals of off-diagonal super elements (1st term)

                hessian[3*i-2,3*j-1]       += -2*K_r*bx*by/distijsqr;
                hessian[3*i-2,3*j  ]       += -2*K_r*bx*bz/distijsqr;
                hessian[3*i-1,3*j-2]       += -2*K_r*by*bx/distijsqr;
                hessian[3*i-1,3*j  ]       += -2*K_r*by*bz/distijsqr;
                hessian[3*i  ,3*j-2]       += -2*K_r*bz*bx/distijsqr;
                hessian[3*i  ,3*j-1]       += -2*K_r*bz*by/distijsqr;

                //Hji
                // diagonals of off-diagonal super elements (1st term)

                hessian[3*j-2,3*i-2]       += -2*K_r*bx*bx/distijsqr;
                hessian[3*j-1,3*i-1]       += -2*K_r*by*by/distijsqr;
                hessian[3*j  ,3*i  ]       += -2*K_r*bz*bz/distijsqr;

                // off-diagonals of off-diagonal super elements (1st term)

                hessian[3*j-2,3*i-1]       += -2*K_r*bx*by/distijsqr;
                hessian[3*j-2,3*i  ]       += -2*K_r*bx*bz/distijsqr;
                hessian[3*j-1,3*i-2]       += -2*K_r*by*bx/distijsqr;
                hessian[3*j-1,3*i  ]       += -2*K_r*by*bz/distijsqr;
                hessian[3*j  ,3*i-2]       += -2*K_r*bz*bx/distijsqr;
                hessian[3*j  ,3*i-1]       += -2*K_r*bz*by/distijsqr;

                //Hii
                //update the diagonals of diagonal super elements

                hessian[3*i-2,3*i-2]       += +2*K_r*bx*bx/distijsqr;
                hessian[3*i-1,3*i-1]       += +2*K_r*by*by/distijsqr;
                hessian[3*i  ,3*i  ]       += +2*K_r*bz*bz/distijsqr;
            
                // update the off-diagonals of diagonal super elements
                hessian[3*i-2,3*i-1]       += +2*K_r*bx*by/distijsqr;
                hessian[3*i-2,3*i  ]       += +2*K_r*bx*bz/distijsqr;
                hessian[3*i-1,3*i-2]       += +2*K_r*by*bx/distijsqr;
                hessian[3*i-1,3*i  ]       += +2*K_r*by*bz/distijsqr;
                hessian[3*i  ,3*i-2]       += +2*K_r*bz*bx/distijsqr;
                hessian[3*i  ,3*i-1]       += +2*K_r*bz*by/distijsqr;

                //Hjj
                //update the diagonals of diagonal super elements

                hessian[3*j-2,3*j-2]       += +2*K_r*bx*bx/distijsqr;
                hessian[3*j-1,3*j-1]       += +2*K_r*by*by/distijsqr;
                hessian[3*j  ,3*j  ]       += +2*K_r*bz*bz/distijsqr;

                // update the off-diagonals of diagonal super elements
                hessian[3*j-2,3*j-1]       += +2*K_r*bx*by/distijsqr;
                hessian[3*j-2,3*j  ]       += +2*K_r*bx*bz/distijsqr;
                hessian[3*j-1,3*j-2]       += +2*K_r*by*bx/distijsqr;
                hessian[3*j-1,3*j  ]       += +2*K_r*by*bz/distijsqr;
                hessian[3*j  ,3*j-2]       += +2*K_r*bz*bx/distijsqr;
                hessian[3*j  ,3*j-1]       += +2*K_r*bz*by/distijsqr;
            }
            HDebug.Assert(hessian.matrix.IsComputable());
            return hessian.matrix;
        }
    }
}
}
