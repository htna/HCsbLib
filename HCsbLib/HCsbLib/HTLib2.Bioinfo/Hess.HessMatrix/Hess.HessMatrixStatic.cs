using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HessMatrixStatic
    {
        //public static HessMatrix ToHessMatrix(this Matrix mat)
        //{
        //    //return HessMatrixDense.FromMatrix(mat);
        //    return HessMatrixDense.FromMatrix(mat);
        //}
        public static HessMatrix ToHessMatrix(this double[,] mat)
        {
            Matrix _mat = mat;
            return _mat.ToHessMatrix();
        }
        public static HessMatrix ToHessMatrix(this Matrix mat)
        {
            HDebug.Assert(mat.ColSize % 3 == 0);
            HDebug.Assert(mat.RowSize % 3 == 0);
            HessMatrix hess = HessMatrix.Zeros(mat.ColSize, mat.RowSize);
        
            {
                HessMatrixDense dense = new HessMatrixDense { hess = mat };

                foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in dense.EnumBlocks())
                {
                    int bc           = bc_br_bval.Item1;
                    int br           = bc_br_bval.Item2;
                    MatrixByArr bval = bc_br_bval.Item3;
                    HDebug.Assert(bval != null);
                    HDebug.Assert(bval.HSumPow2() != 0);
                    hess.SetBlock(bc, br, bval.CloneT());
                }
            }
        
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(mat.ColSize == hess.ColSize);
                HDebug.Assert(mat.RowSize == hess.RowSize);
                int colsize = mat.ColSize;
                int rowsize = mat.RowSize;
                for(int c=0; c<colsize; c++)
                    for(int r=0; r<rowsize; r++)
                        HDebug.Assert(hess[c, r] == mat[c, r]);
            }
        
            return hess;
        }
    }
}
