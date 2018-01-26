using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class HessMatrixSparse : HessMatrix
    {
        public static class LinAlg
        {
            static public Vector MV(HessMatrixSparse M, Vector V)
            {
                Vector mv = new double[M.ColSize];
                Vector bvec = new double[3];
                Vector bmv = new double[3];
                MV(M, V, mv, bvec, bmv);
                return mv;
            }
            static public void MV(HessMatrixSparse M, Vector V, Vector mv, Vector bvec, Vector bmv)
            {
                HDebug.Exception(V.Size == M.RowSize);
                HDebug.Exception(mv.Size == M.ColSize); //Vector mv = new double[M.ColSize];
                HDebug.Exception(bvec.Size == 3);       //Vector bvec = new double[3];
                HDebug.Exception(bmv .Size == 3);       //Vector bmv = new double[3];
                foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in M.EnumBlocks_dep())
                {
                    int bc = bc_br_bval.Item1;
                    int br = bc_br_bval.Item2;
                    var bmat=bc_br_bval.Item3;
                    bvec[0] = V[br*3+0];
                    bvec[1] = V[br*3+1];
                    bvec[2] = V[br*3+2];
                    HTLib2.LinAlg.MV(bmat, bvec, bmv);
                    mv[bc*3+0] += bmv[0];
                    mv[bc*3+1] += bmv[1];
                    mv[bc*3+2] += bmv[2];
                }

                if(HDebug.Selftest())
                {
                    Matlab.Clear();
                    Matlab.PutSparseMatrix("M", M.GetMatrixSparse(), 3, 3);
                    Matlab.PutVector("V", V);
                    Matlab.Execute("MV = M*V;");
                    Matlab.PutVector("MV1", mv);
                    Vector err = Matlab.GetVector("MV-MV1");
                    double err_max = err.ToArray().HAbs().Max();
                    HDebug.Assert(err_max < 0.00000001);
                }
            }
        }
    }
}
