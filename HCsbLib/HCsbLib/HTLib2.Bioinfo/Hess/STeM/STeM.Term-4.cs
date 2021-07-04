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
        public static HessMatrix FourthTerm(IList<Vector> caArray, double Epsilon, HessMatrix hessian)
        {
            if(hessian == null)
                hessian = HessMatrix.FromMatrix(new double[caArray.Count*3, caArray.Count*3]);

            for(int i=0; i<caArray.Count; i++)
            {
                for(int j=0; j<caArray.Count; j++)
                {
                    if(abs(i-j)>3)
                    {
                        FourthTerm(caArray, Epsilon, hessian, i, j);
                    }
                }
            }
            return hessian;
        }
        private static void FourthTerm(IList<Vector> caArray, double Epsilon, HessMatrix hessian, int i, int j)
        {
            int[] idx = new int[] { i, j };
            Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]] };
            Matrix lhess = FourthTerm(lcaArray, Epsilon);
            for(int c=0; c<2; c++) for(int dc=0; dc<3; dc++)
            for(int r=0; r<2; r++) for(int dr=0; dr<3; dr++)
                hessian[idx[c]*3+dc, idx[r]*3+dr] += lhess[c*3+dc, r*3+dr];

            if(HDebug.IsDebuggerAttachedWithProb(0.1))
            {
                if(caArray.Count == 2)
                    // avoid stack overflow
                    return;
                HessMatrix lhess0 = HessMatrix.FromMatrix(new double[6, 6]);
                FourthTerm(lcaArray, Epsilon, lhess0, 0, 1);
                FourthTerm(lcaArray, Epsilon, lhess0, 1, 0);
                double r2 = (lcaArray[0] - lcaArray[1]).Dist2;
                HessMatrix dhess0 = (120*Epsilon/r2) * Hess.GetHessAnm(lcaArray, double.PositiveInfinity);
                HDebug.AssertToleranceMatrix(0.00000001, lhess0-dhess0);
            }
        }
        /// <summary>
        /// Nonbonded Term (Van der Waals)
        /// V4 = Epsilon ( 5 (r0_ij / r_ij)^12 - 6 (r0_ij / r_ij)^10 )
        /// </summary>
        /// <param name="caArray_"></param>
        /// <param name="Epsilon"></param>
        /// <returns></returns>
        public static Matrix FourthTerm(IList<Vector> caArray_, double Epsilon)
        {
            VECTORS caArray = new VECTORS(caArray_);
            MATRIX<Matrix> hessian = new MATRIX<Matrix>(new double[6,6]);

            // derive the hessian of the first term (off diagonal)
            {
                int i=1;
                int j=2;

                double bx=caArray[i,1] - caArray[j,1];
                double by=caArray[i,2] - caArray[j,2];
                double bz=caArray[i,3] - caArray[j,3];
                double distijsqr = pow(sqrt(bx*bx + by*by + bz*bz), 4);

                // diagonals of off-diagonal super elements (1st term)
                hessian[3*i-2,3*j-2]       += -120*Epsilon*bx*bx/distijsqr;
                hessian[3*i-1,3*j-1]       += -120*Epsilon*by*by/distijsqr;
                hessian[3*i  ,3*j  ]       += -120*Epsilon*bz*bz/distijsqr;

                // off-diagonals of off-diagonal super elements (1st term)

                hessian[3*i-2,3*j-1]       += -120*Epsilon*bx*by/distijsqr;
                hessian[3*i-2,3*j  ]       += -120*Epsilon*bx*bz/distijsqr;
                hessian[3*i-1,3*j-2]       += -120*Epsilon*by*bx/distijsqr;
                hessian[3*i-1,3*j  ]       += -120*Epsilon*by*bz/distijsqr;
                hessian[3*i  ,3*j-2]       += -120*Epsilon*bx*bz/distijsqr;
                hessian[3*i  ,3*j-1]       += -120*Epsilon*by*bz/distijsqr;

                //Hii
                //update the diagonals of diagonal super elements

                hessian[3*i-2,3*i-2]       += +120*Epsilon*bx*bx/distijsqr;
                hessian[3*i-1,3*i-1]       += +120*Epsilon*by*by/distijsqr;
                hessian[3*i  ,3*i  ]       += +120*Epsilon*bz*bz/distijsqr;

                // update the off-diagonals of diagonal super elements
                hessian[3*i-2,3*i-1]       += +120*Epsilon*bx*by/distijsqr;
                hessian[3*i-2,3*i  ]       += +120*Epsilon*bx*bz/distijsqr;
                hessian[3*i-1,3*i-2]       += +120*Epsilon*by*bx/distijsqr;
                hessian[3*i-1,3*i  ]       += +120*Epsilon*by*bz/distijsqr;
                hessian[3*i  ,3*i-2]       += +120*Epsilon*bz*bx/distijsqr;
                hessian[3*i  ,3*i-1]       += +120*Epsilon*bz*by/distijsqr;
            }
            HDebug.Assert(hessian.matrix.IsComputable());
            return hessian.matrix;
        }
    }
}
}
