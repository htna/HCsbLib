using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HTLib2
{
	public partial class LinAlg
	{
        public static Matrix InvOfSubMatrixOfInv(this IMatrix<double> mat, IList<int> idxs, ILinAlg ila, string invtype, params object[] invopt)
        {
            if(HDebug.Selftest())
            {
                MatrixByArr tH = new double[,]{{ 1,  2,  3,  4}
                                         ,{ 2,  5,  6,  7}
                                         ,{ 3,  6,  8,  9}
                                         ,{ 4,  7,  9, 10}};
                MatrixByArr tInvH = new double[,]{{ 0.5, -0.5, -1.5,  1.5}   // = inv(tH)
                                            ,{-0.5,  1.5, -1.5,  0.5}
                                            ,{-1.5, -1.5,  3.5, -1.5}
                                            ,{ 1.5,  0.5, -1.5,  0.5}};
                MatrixByArr tInvH12 = new double[,]{{ 0.5, -0.5}             // = block matrix of tInvH
                                              ,{-0.5,  1.5}};
                MatrixByArr tInvtInvH12 = new double[,]{{3, 1}               // = inv([ 0.5, -0.5])
                                                  ,{1, 1}};             //       [-0.5,  1.5]
                MatrixByArr ttInvtInvH12 = tH.InvOfSubMatrixOfInv(new int[] { 0, 1 }, ila, null).ToArray();
                HDebug.AssertTolerance(0.00000001, tInvtInvH12 - ttInvtInvH12);
            }

            int ColSize = mat.ColSize;
            int RowSize = mat.RowSize;
            if(ColSize != RowSize) { HDebug.Assert(false); return null; }
            if(idxs.Count != idxs.HToHashSet().Count) { HDebug.Assert(false); return null; }

            List<int> idxs0 = idxs.ToList();
            List<int> idxs1 = new List<int>();
            for(int i=0; i<ColSize; i++)
                if(idxs.Contains(i) == false)
                    idxs1.Add(i);

            Matrix InvInvA;
            {
                Matrix A = mat.SubMatrix(idxs0, idxs0); Matrix B = mat.SubMatrix(idxs0, idxs1); // [A B]
                Matrix C = mat.SubMatrix(idxs1, idxs0); Matrix D = mat.SubMatrix(idxs1, idxs1); // [C D]

                /// http://en.wikipedia.org/wiki/Invertable_matrix#Blockwise_inversion
                /// 
                /// M^-1 = [M_00 M_01] = [A B]-1 = [ (A - B D^-1 C)^-1  ... ]
                ///      = [M_10 M_11] = [C D]     [   ...              ... ]
                /// 
                /// (inv(M))_00 = inv(A - B inv(D) C)
                /// inv((inv(M))_00) = (A - B inv(D) C)
                var AA = ila.ToILMat(A);
                var BB = ila.ToILMat(B);
                var CC = ila.ToILMat(C);
                var DD = ila.ToILMat(D);
                var InvDD = ila.HInv(DD, invtype, invopt);
                var InvInvAA = AA - BB * InvDD * CC;
                //var xx = BB * InvDD * CC;

                InvInvA = InvInvAA.ToArray();
                AA.Dispose();
                BB.Dispose();
                CC.Dispose();
                DD.Dispose();
                InvDD.Dispose();
                InvInvAA.Dispose();
            }
            GC.Collect();
            return InvInvA;
        }
        public static Matrix SubMatrixOfInv(this Matrix mat, IList<int> idxs, ILinAlg ila, string invopt)
        {
            Matrix InvInvA = mat.InvOfSubMatrixOfInv(idxs, ila, invopt);
            /// (inv(M))_00 = inv(A - B inv(D) C)
            var InvInvAA = ila.ToILMat(InvInvA);
            var InvAA    = ila.PInv(InvInvAA);
            Matrix InvA = InvAA.ToArray();
            return InvA;
        }
    }
}
