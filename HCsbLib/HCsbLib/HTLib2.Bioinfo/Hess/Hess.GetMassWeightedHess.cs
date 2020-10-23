using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static Matrix GetMassWeightedHessGnm(Matrix hess, Vector mass)
        {
            if(HDebug.Selftest())
                //#region selftest
            {
            }
                //#endregion

            HDebug.Assert(hess.ColSize == hess.RowSize, hess.RowSize == mass.Size);
            Vector mass05 = mass.ToArray().HSqrt();

            // mass weighted hessian
            // MH = M^(-1/2) * H * M^(-1/2)
            // MH_ij = H_IJ * sqrt(M[i] * M[j])
            Matrix mwhess = hess.Clone();
            {
                // mass weighted block hessian
                for(int c=0; c<hess.ColSize; c++)
                {
                    for(int r=0; r<hess.RowSize; r++)
                    {
                        mwhess[c, r] = hess[c, r] / (mass05[c] * mass05[r]);
                    }
                }
            }

            return mwhess;
        }
        //static bool GetMassWeightedHess_selftest1 = true;
        //public static MatrixByArr GetMassWeightedHess(MatrixByArr hess, Vector mass)
        //{
        //    return GetMassWeightedHess(hess as Matrix, mass).ToArray();
        //}
        public static Matrix GetMassWeightedHess(Matrix hess, Vector mass)
        {
            Matrix mwhess = hess.Clone();

            double[] mass3 = new double[mass.Size*3];
            for(int i=0; i<mass3.Length; i++) mass3[i] = mass[i/3];

            UpdateMassWeightedHess(mwhess, mass3);
            return mwhess;
        }
        public static HessMatrix GetMassWeightedHess(HessMatrix hess, Vector mass)
        {
            HessMatrix mwhess = hess.CloneHess();

            HDebug.ToDo("check: mass -> mass3");
            double[] mass3 = new double[mass.Size*3];
            for(int i=0; i<mass3.Length; i++) mass3[i] = mass[i/3];

            UpdateMassWeightedHess(mwhess, mass3);
            return mwhess;
        }
        public static void UpdateMassWeightedHess(Matrix hess, Vector mass)
        {
            if(HDebug.Selftest())
            //if(GetMassWeightedHess_selftest1)
                #region selftest
            {
                //HDebug.ToDo("replace examplt not to use blocked hessian matrix");
                //GetMassWeightedHess_selftest1 = false;
                MatrixByArr[,] _bhess = new MatrixByArr[2, 2];
                _bhess[0, 0] = new double[3, 3] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
                _bhess[0, 1] = _bhess[0, 0] + 10;
                _bhess[1, 0] = _bhess[0, 0] + 20;
                _bhess[1, 1] = _bhess[0, 0] + 30;
                Vector _mass = new double[2] { 2, 3 };
                MatrixByArr _hess = MatrixByArr.FromMatrixArray(_bhess);

                Matrix    _mwhess = GetMassWeightedHess(_hess , _mass);
                MatrixByArr[,] _mwbhess = GetMassWeightedHess(_bhess, _mass);

                HDebug.AssertTolerance(0.00000001, MatrixByArr.FromMatrixArray(_mwbhess) - _mwhess.ToArray());
            }
                #endregion

            HDebug.Exception(hess.ColSize == mass.Size);
            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);

            Vector mass05 = mass.ToArray().HSqrt();

            // mass weighted hessian
            // MH = M^(-1/2) * H * M^(-1/2)
            // MH_ij = H_IJ * sqrt(M[i] * M[j])
            {
                // mass weighted block hessian
                for(int i=0; i<hess.ColSize; i++)
                {
                    for(int j=0; j<hess.RowSize; j++)
                    {
                        //if(i == j) continue;
                        hess[i, j] = hess[i, j] / (mass05[i] * mass05[j]);
                        //mbhess[i, i] -= mbhess[i, j];
                    }
                }
            }
        }
        public static void UpdateMassWeightedHess(HessMatrix hess, Vector mass)
        {
            if(hess.ColSize < 15000)
                if(HDebug.Selftest())
                {
                    Matrix tmat0 = hess.ToArray();
                    UpdateMassWeightedHess(tmat0, mass);
                    HessMatrix tmat1 = hess.CloneHess();
                    UpdateMassWeightedHess(tmat1, mass);
                    double absmax = (tmat0 - tmat1).HAbsMax();
                    HDebug.Exception(absmax < 0.00000001);
                }

            HDebug.Exception(mass.Size % 3 == 0);
            double[] mass03sqrt = new double[mass.Size / 3];
            for(int i=0; i<mass03sqrt.Length; i++)
            {
                HDebug.Exception(mass[i*3+0] == mass[i*3+1]);
                HDebug.Exception(mass[i*3+0] == mass[i*3+2]);
                mass03sqrt[i] = mass[i*3+0];
            }
            mass03sqrt = mass03sqrt.HSqrt();

            foreach(var bc_br_bval in hess.EnumBlocks().ToArray())
            {
                int bc   = bc_br_bval.Item1;
                int br   = bc_br_bval.Item2;
                var bval = bc_br_bval.Item3;
                bval = bval / (mass03sqrt[bc] * mass03sqrt[br]);

                hess.SetBlock(bc, br, bval);
            }
        }
        public static MatrixByArr[,] GetMassWeightedHess(MatrixByArr[,] hess, Vector mass, InfoPack extra=null)
        {
            //HDebug.Depreciated();
            HDebug.Assert(hess.GetLength(0) == hess.GetLength(1));
            int n = hess.GetLength(0);
        
            // mass weighted hessian
            // MH = M^(-1/2) * H * M^(-1/2)
            // MH_ij = H_IJ * sqrt(M[i] * M[j])
            MatrixByArr[,] mwhess = new MatrixByArr[n, n];
            {
                // mass weighted block hessian
                //Matrix[,] mbhess = new Matrix[n, n];
                for(int i=0; i<n; i++)
                {
                    //mbhess[i, i] = new double[3, 3];
                    for(int j=0; j<n; j++)
                    {
                        //if(i == j) continue;
                        HDebug.Assert(hess[i, j].ColSize == 3, hess[i, j].RowSize == 3);
                        mwhess[i, j] = hess[i, j] / Math.Sqrt(mass[i] * mass[j]);
                        //mbhess[i, i] -= mbhess[i, j];
                    }
                }
            }
        
            return mwhess;
        }
        //public static MatrixSparse<MatrixByArr> GetMassWeightedHess(MatrixSparse<MatrixByArr> hess, Vector mass, InfoPack extra=null)
        //{
        //    MatrixSparse<MatrixByArr> mwhess = new MatrixSparse<MatrixByArr>(hess.ColSize, hess.RowSize, hess.GetDefault);
        //    Vector mass05 = mass.ToArray().HSqrt().ToArray();
        //
        //    foreach(var c_r_val in hess.EnumElements())
        //    {
        //        int c = c_r_val.Item1;
        //        int r = c_r_val.Item2;
        //        MatrixByArr hesscr = c_r_val.Item3;
        //        MatrixByArr mwhesscr = hesscr / (mass05[c] * mass05[r]);
        //        mwhess[c,r] = mwhesscr;
        //    }
        //
        //    if(HDebug.IsDebuggerAttached && HDebug.Selftest())
        //    {
        //        MatrixByArr tmwhess = MatrixByArr.FromMatrixArray(mwhess.ToArray());
        //        MatrixByArr thess   = MatrixByArr.FromMatrixArray(hess.ToArray());
        //        MatrixByArr mwthess = GetMassWeightedHess(thess, mass).ToArray();
        //        HDebug.AssertTolerance(0.000000001, tmwhess-mwthess);
        //    }
        //
        //    return mwhess;
        //}
    }
}
