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
               static bool   Eye_SelfTest = HDebug.IsDebuggerAttached;
		public static MatrixByArr Eye(int size, double diagval=1)
		{
            if(Eye_SelfTest)
            {
                Eye_SelfTest = false;
                MatrixByArr tT0 = new double[3, 3] { { 2, 0, 0 }, { 0, 2, 0 }, { 0, 0, 2 }, };
                MatrixByArr tT1 = Eye(3, 2);
                HDebug.AssertTolerance(double.Epsilon, tT0 - tT1);
            }
			MatrixByArr mat = new MatrixByArr(size,size);
			for(int i=0; i<size; i++)
                mat[i, i] = diagval;
			return mat;
		}
               static bool   Tr_SelfTest = HDebug.IsDebuggerAttached;
        public static Matrix Tr(this Matrix M)
        {
            if(Tr_SelfTest)
            {
                Tr_SelfTest = false;
                Matrix tM0 = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
                Matrix tT0 = new double[3, 2] { { 1, 4 }, { 2, 5 }, { 3, 6 } };
                Matrix tT1 = Tr(tM0);
                HDebug.AssertToleranceMatrix(double.Epsilon, tT0 - tT1);
            }
            Matrix tr = Matrix.Zeros(M.RowSize, M.ColSize);
            for(int c=0; c<tr.ColSize; c++)
                for(int r=0; r<tr.RowSize; r++)
                    tr[c, r] = M[r, c];
            return tr;
        }
               static bool   Diagd_SelfTest = HDebug.IsDebuggerAttached;
        public static MatrixByArr Diag(this Vector d)
        {
            if(Diagd_SelfTest)
            {
                Diagd_SelfTest = false;
                MatrixByArr tD1 = new double[3, 3] { { 1, 0, 0 }, { 0, 2, 0 }, { 0, 0, 3 } };
                Vector td1 = new double[3] { 1, 2, 3 };
                MatrixByArr tD  = Diag(td1);
                HDebug.AssertTolerance(double.Epsilon, tD - tD1);
            }
            int size = d.Size;
            MatrixByArr D = new MatrixByArr(size, size);
            for(int i=0; i < size; i++)
                D[i, i] = d[i];
            return D;
        }
               static bool   DiagD_SelfTest = HDebug.IsDebuggerAttached;
        public static Vector Diag(this Matrix D)
        {
            if(DiagD_SelfTest)
            {
                DiagD_SelfTest = false;
                MatrixByArr tD1 = new double[3, 3] { { 1, 0, 0 }, { 0, 2, 0 }, { 0, 0, 3 } };
                Vector td1 = new double[3] { 1, 2, 3 };
                Vector td  = Diag(tD1);
                HDebug.AssertTolerance(double.Epsilon, td - td1);
            }
            HDebug.Assert(D.ColSize == D.RowSize);
            int size = D.ColSize;
            Vector d = new double[size];
            for(int i=0; i<size; i++)
                d[i] = D[i,i];
            return d;
        }
               static bool   DV_SelfTest1 = HDebug.IsDebuggerAttached;
        public static Vector DV(Matrix D, Vector V, bool assertDiag = true)
        {
            if(DV_SelfTest1)
            {
                DV_SelfTest1 = false;
                MatrixByArr tD  = new double[3, 3] { { 1, 0, 0 }, { 0, 2, 0 }, { 0, 0, 3 } };
                Vector tV  = new double[3] { 1, 2, 3 };
                Vector tDV = new double[3] { 1, 4, 9 };
                // [1 0 0]   [1]   [1]
                // [0 2 0] * [2] = [4]
                // [0 0 3]   [3]   [9]
                HDebug.AssertTolerance(double.Epsilon, DV(tD, tV) - tDV);
            }
            // D is the diagonal matrix
            HDebug.Assert(D.ColSize == D.RowSize);
            if(assertDiag) // check diagonal matrix
                HDebug.AssertToleranceMatrix(double.Epsilon, D - Diag(Diag(D)));
            HDebug.Assert(D.ColSize == V.Size);
            Vector diagD = Diag(D);
            Vector diagDV = DV(diagD, V);
            return diagDV;
        }
               static bool   DV_SelfTest2 = HDebug.IsDebuggerAttached;
        public static Vector DV(Vector D, Vector V, bool assertDiag = true)
        {
            if(DV_SelfTest2)
            {
                DV_SelfTest2 = false;
                Vector tD  = new double[3] { 1, 2, 3 };
                Vector tV  = new double[3] { 1, 2, 3 };
                Vector tDV = new double[3] { 1, 4, 9 };
                // [1 0 0]   [1]   [1]
                // [0 2 0] * [2] = [4]
                // [0 0 3]   [3]   [9]
                HDebug.AssertTolerance(double.Epsilon, DV(tD, tV) - tDV);
            }
            // D is the diagonal matrix
            HDebug.Assert(D.Size == V.Size);
            int size = V.Size;
            Vector dv = new double[size];
            for(int i=0; i < size; i++)
                dv[i] = D[i] * V[i];
            return dv;
        }
               static bool   MD_SelfTest = HDebug.IsDebuggerAttached;
        public static MatrixByArr MD(MatrixByArr M, Vector D)
        {   // M * Diag(D)
            if(MD_SelfTest)
            {
                MD_SelfTest = false;
                MatrixByArr tM = new double[3, 3] { { 1, 2, 3 }
                                             , { 4, 5, 6 }
                                             , { 7, 8, 9 } };
                Vector tD = new double[3] { 1, 2, 3 };
                MatrixByArr tMD0 = new double[3, 3] { { 1,  4,  9 }
                                               , { 4, 10, 18 }
                                               , { 7, 16, 27 } };
                MatrixByArr tMD1 = MD(tM, tD);
                MatrixByArr dtMD = tMD0 - tMD1;
                double maxAbsDtMD = dtMD.ToArray().HAbs().HMax();
                Debug.Assert(maxAbsDtMD == 0);
            }
            HDebug.Assert(M.RowSize == D.Size);
            MatrixByArr lMD = new double[M.ColSize, M.RowSize];
            for(int c=0; c<lMD.ColSize; c++)
                for(int r=0; r<lMD.RowSize; r++)
                    lMD[c, r] = M[c, r] * D[r];
            return lMD;
        }
               static bool   MV_SelfTest = HDebug.IsDebuggerAttached;
               static bool   MV_SelfTest_lmat_rvec = HDebug.IsDebuggerAttached;
        public static Vector MV<MATRIX>(MATRIX lmat, Vector rvec, string options="")
            where MATRIX : IMatrix<double>
        {
            Vector result = new Vector(lmat.ColSize);
            MV(lmat, rvec, result, options);
            if(MV_SelfTest_lmat_rvec)
            {
                MV_SelfTest_lmat_rvec = false;
                HDebug.Assert(lmat.RowSize == rvec.Size);
			    Vector lresult = new Vector(lmat.ColSize);
			    for(int c=0; c<lmat.ColSize; c++)
				    for(int r=0; r<lmat.RowSize; r++)
					    lresult[c] += lmat[c, r] * rvec[r];
                HDebug.AssertTolerance(double.Epsilon, lresult-result);
            }
            return result;
        }
        public static void MV<MATRIX>(MATRIX lmat, Vector rvec, Vector result, string options="")
            where MATRIX : IMatrix<double>
        {
            if(MV_SelfTest)
            {
                MV_SelfTest = false;
                MatrixByArr tM = new double[4, 3] { {  1,  2,  3 }
                                             , {  4,  5,  6 }
                                             , {  7,  8,  9 }
                                             , { 10, 11, 12 } };
                Vector tV = new double[3] { 1, 2, 3 };
                Vector tMV0 = new double[4] { 14, 32, 50, 68 };
                Vector tMV1 = MV(tM, tV);
                double err = (tMV0 - tMV1).ToArray().HAbs().Max();
                HDebug.Assert(err == 0);
            }
            HDebug.Assert(lmat.RowSize == rvec.Size);
            HDebug.Assert(lmat.ColSize == result.Size);

            if(options.Split(';').Contains("parallel") == false)
            {
                for(int c=0; c<lmat.ColSize; c++)
                    for(int r=0; r<lmat.RowSize; r++)
                        result[c] += lmat[c, r] * rvec[r];
            }
            else
            {
                System.Threading.Tasks.Parallel.For(0, lmat.ColSize, delegate(int c)
                {
                    for(int r=0; r<lmat.RowSize; r++)
                        result[c] += lmat[c, r] * rvec[r];
                });
            }
        }
               //static bool   MtM_SelfTest = HDebug.IsDebuggerAttached;
        public static Matrix MtM(Matrix lmat, Matrix rmat)
        {
            bool MtM_SelfTest = false;//HDebug.IsDebuggerAttached;
            if(MtM_SelfTest)
            {
                MtM_SelfTest = false;
                /// >> A=[ 1,5 ; 2,6 ; 3,7 ; 4,8 ];
                /// >> B=[ 1,2,3 ; 3,4,5 ; 5,6,7 ; 7,8,9 ];
                /// >> A'*B
                /// ans =
                ///     50    60    70
                ///    114   140   166
                Matrix _A = new double[4, 2] {{ 1,5 },{ 2,6 },{ 3,7 },{ 4,8 }};
                Matrix _B = new double[4, 3] {{ 1,2,3 },{ 3,4,5 },{ 5,6,7 },{ 7,8,9 }};
                Matrix _AtB = MtM(_A, _B);
                Matrix _AtB_sol = new double[2,3]
                            { {  50,  60,  70 }
                            , { 114, 140, 166 } };
                double err = (_AtB - _AtB_sol).HAbsMax();
                HDebug.Assert(err == 0);
            }
            HDebug.Assert(lmat.ColSize == rmat.ColSize);
            int size1 = lmat.RowSize;
            int size2 = rmat.ColSize;
            int size3 = rmat.RowSize;
            Matrix result = Matrix.Zeros(size1, size3);
            for(int c=0; c<size1; c++)
                for(int r=0; r<size3; r++)
                {
                    double sum = 0;
                    for(int i=0; i<size2; i++)
                        // tr(lmat[c,i]) * rmat[i,r] => lmat[i,c] * rmat[i,r]
                        sum += lmat[i,c] * rmat[i,r];
                    result[c, r] = sum;
                }
            return result;
        }
               //static bool   MMt_SelfTest = HDebug.IsDebuggerAttached;
        public static Matrix MMt(Matrix lmat, Matrix rmat)
        {
            bool MMt_SelfTest = false;//HDebug.IsDebuggerAttached;
            if(MMt_SelfTest)
            {
                MMt_SelfTest = false;
                /// >> A=[ 1,2,3,4 ; 5,6,7,8 ];
                /// >> B=[ 1,3,5,7 ; 2,4,6,8 ; 3,5,7,9 ];
                /// >> A*B'
                /// ans =
                ///     50    60    70
                ///    114   140   166
                Matrix _A = new double[2, 4]
                            { { 1, 2, 3, 4 }
                            , { 5, 6, 7, 8 } };
                Matrix _B = new double[3, 4]
                            { { 1, 3, 5, 7 }
                            , { 2, 4, 6, 8 }
                            , { 3, 5, 7, 9 } };
                Matrix _AtB = MMt(_A, _B);
                Matrix _AtB_sol = new double[2,3]
                            { {  50,  60,  70 }
                            , { 114, 140, 166 } };
                double err = (_AtB - _AtB_sol).HAbsMax();
                HDebug.Assert(err == 0);
            }
            HDebug.Assert(lmat.RowSize == rmat.RowSize);
            int size1 = lmat.ColSize;
            int size2 = lmat.RowSize;
            int size3 = rmat.ColSize;
            Matrix result = Matrix.Zeros(size1, size3);
            for(int c=0; c<size1; c++)
                for(int r=0; r<size3; r++)
                {
                    double sum = 0;
                    for(int i=0; i<size2; i++)
                        // lmat[c,i] * tr(rmat[i,r]) => lmat[c,i] * rmat[r,i]
                        sum += lmat[c,i] * rmat[r,i];
                    result[c, r] = sum;
                }
            return result;
        }
               static bool   MtV_SelfTest = HDebug.IsDebuggerAttached;
        public static Vector MtV(Matrix lmat, Vector rvec)
        {
            if(MtV_SelfTest)
            {
                MtV_SelfTest = false;
                /// >> A = [ 1,2,3 ; 4,5,6 ; 7,8,9 ; 10,11,12 ];
                /// >> B = [ 1; 2; 3; 4 ];
                /// >> A'*B
                /// ans =
                ///     70
                ///     80
                ///     90
                MatrixByArr tM = new double[4, 3] { {  1,  2,  3 }
                                             , {  4,  5,  6 }
                                             , {  7,  8,  9 }
                                             , { 10, 11, 12 } };
                Vector tV = new double[4] { 1, 2, 3, 4 };
                Vector tMtV0 = new double[3] { 70, 80, 90 };
                Vector tMtV1 = MtV(tM, tV);
                double err = (tMtV0 - tMtV1).ToArray().HAbs().Max();
                HDebug.Assert(err == 0);
            }
            HDebug.Assert(lmat.ColSize == rvec.Size);
            Vector result = new Vector(lmat.RowSize);
            for(int c=0; c<lmat.ColSize; c++)
                for(int r=0; r<lmat.RowSize; r++)
                    result[r] += lmat[c, r] * rvec[c];
            return result;
        }
        public static bool V1tD2V3_SelfTest = HDebug.IsDebuggerAttached;
        public static double V1tD2V3(Vector V1, Matrix D2, Vector V3, bool assertDiag=true)
        {
            if(V1tD2V3_SelfTest)
            {
                V1tD2V3_SelfTest = false;
                Vector tV1 = new double[3] { 1, 2, 3 };
                MatrixByArr tD2 = new double[3, 3] { { 2, 0, 0 }, { 0, 3, 0 }, { 0, 0, 4 } };
                Vector tV3 = new double[3] { 3, 4, 5 };
                //           [2    ]   [3]             [ 6]   
                // [1 2 3] * [  3  ] * [4] = [1 2 3] * [12] = 6+24+60 = 90
                //           [    4]   [5]             [20]   
                double tV1tD2V3 = 90;
                HDebug.AssertTolerance(double.Epsilon, tV1tD2V3 - V1tD2V3(tV1, tD2, tV3));
            }
            if(assertDiag) // check diagonal matrix
                HDebug.AssertToleranceMatrix(double.Epsilon, D2 - Diag(Diag(D2)));
            HDebug.Assert(V1.Size    == D2.ColSize);
            HDebug.Assert(D2.RowSize == V3.Size   );

            Vector lD2V3 = DV(D2, V3, assertDiag);
            double lV1tD2V3 = VtV(V1, lD2V3);
            return lV1tD2V3;
        }
        public static MatrixByArr VVt(Vector lvec, Vector rvec)
        {
            MatrixByArr outmat = new MatrixByArr(lvec.Size, rvec.Size);
            VVt(lvec, rvec, outmat);
            return outmat;
        }
        public static void VVt(Vector lvec, Vector rvec, MatrixByArr outmat)
        {
            HDebug.Exception(outmat.ColSize == lvec.Size);
            HDebug.Exception(outmat.RowSize == rvec.Size);
            //MatrixByArr mat = new MatrixByArr(lvec.Size, rvec.Size);
            for(int c = 0; c < lvec.Size; c++)
                for(int r = 0; r < rvec.Size; r++)
                    outmat[c, r] = lvec[c] * rvec[r];
        }
        public static void VVt_AddTo(Vector lvec, Vector rvec, MatrixByArr mat)
        {
            HDebug.Exception(mat.ColSize == lvec.Size);
            HDebug.Exception(mat.RowSize == rvec.Size);
            for(int c = 0; c < lvec.Size; c++)
                for(int r = 0; r < rvec.Size; r++)
                    mat[c, r] += lvec[c] * rvec[r];
        }
        public static bool   DMD_selftest = HDebug.IsDebuggerAttached;
        public static Matrix DMD(Vector diagmat1, Matrix mat,Vector diagmat2)
        {
            if(DMD_selftest)
                #region selftest
            {
                HDebug.ToDo("check");
                DMD_selftest = false;
                Vector td1 = new double[] { 1, 2, 3 };
                Vector td2 = new double[] { 4, 5, 6 };
                Matrix tm  = new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
                Matrix dmd0 = LinAlg.Diag(td1) * tm * LinAlg.Diag(td2);
                Matrix dmd1 = LinAlg.DMD(td1, tm, td2);
                double err = (dmd0 - dmd1).HAbsMax();
                HDebug.Assert(err == 0);
            }
                #endregion
            Matrix DMD = mat.Clone();
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double v0 = mat[c, r];
                    double v1 = diagmat1[c] * v0 * diagmat2[r];
                    if(v0 == v1) continue;
                    DMD[c, r] = v1;
                }
            return DMD;
        }
        public static double VtMV(Vector lvec, Matrix mat, Vector rvec, string options="")
        {
            Vector MV = LinAlg.MV(mat, rvec, options);
            double VMV = LinAlg.VtV(lvec, MV);
            //Debug.AssertToleranceIf(lvec.Size<100, 0.00000001, Vector.VtV(lvec, Vector.MV(mat, rvec)) - VMV);
            return VMV;
        }
        public static MatrixByArr M_Mt(MatrixByArr lmat, MatrixByArr rmat)
        {
            // M + Mt
            HDebug.Assert(lmat.ColSize == rmat.RowSize);
            HDebug.Assert(lmat.RowSize == rmat.ColSize);
            MatrixByArr MMt = lmat.CloneT();
            for(int c = 0; c < MMt.ColSize; c++)
                for(int r = 0; r < MMt.RowSize; r++)
                    MMt[c, r] += rmat[r, c];
            return MMt;
        }
        public static double VtV(Vector l, Vector r)
        {
            HDebug.Assert(l.Size == r.Size);
            int size = l.Size;
            double result = 0;
            for(int i=0; i < size; i++)
                result += l[i] * r[i];
            return result;
        }
        public static double[] ListVtV(Vector l, IList<Vector> rs)
        {
            double[] listvtv = new double[rs.Count];
            for(int i=0; i<rs.Count; i++)
                listvtv[i] = VtV(l, rs[i]);
            return listvtv;
        }
        public static double VtV(Vector l, Vector r, IList<int> idxsele)
        {
            HDebug.Assert(l.Size == r.Size);
            Vector ls = l.ToArray().HSelectByIndex(idxsele);
            Vector rs = r.ToArray().HSelectByIndex(idxsele);
            return VtV(ls, rs);
        }
        public static Vector VtMM(Vector v1, Matrix m2, Matrix m3)
        {
            Vector v12 = VtM(v1, m2);
            return VtM(v12, m3);
        }

        public static class AddToM
        {
            public static void VVt(Matrix M, Vector V)
            {
                HDebug.Assert(M.ColSize == V.Size);
                HDebug.Assert(M.RowSize == V.Size);
                int size = V.Size;
                for(int c=0; c<size; c++)
                    for(int r=0; r<size; r++)
                        M[c, r] += V[c] * V[r];
            }
        }

		public static Vector VtM<MATRIX>(Vector lvec, MATRIX rmat)
            where MATRIX : IMatrix<double>
        {
			HDebug.Assert(lvec.Size == rmat.ColSize);
			Vector result = new Vector(rmat.RowSize);
			for(int c=0; c<rmat.ColSize; c++)
				for(int r=0; r<rmat.RowSize; r++)
					result[r] += lvec[c] * rmat[c, r];
			return result;
		}
    }
}
