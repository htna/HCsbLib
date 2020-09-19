using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public static partial class HBioinfo
{
    public partial class BFactor
    {
        public static List<double> GetBFactor(Pdb[] ensemble)
        {
            List<Vector>[] coordss = new List<Vector>[ensemble.Length];
            for(int i=0; i<ensemble.Length; i++)
                coordss[i] = ensemble[i].atoms.ListCoord();

            return GetBFactor(coordss);
        }
        public static List<double> GetBFactor(List<Pdb.Atom>[] ensemble)
        {
            List<Vector>[] coordss = new List<Vector>[ensemble.Length];
            for(int i=0; i<ensemble.Length; i++)
                coordss[i] = ensemble[i].ListCoord();

            return GetBFactor(coordss);
        }
        public static List<double> GetBFactor(List<Vector>[] ensemble)
        {
            int size = ensemble[0].Count;
            double[] bfactor = new double[size];
            for(int i=0; i<size; i++)
            {
                Vector meancoord = new double[3];
                foreach(List<Vector> coords in ensemble)
                    meancoord += coords[i];
                meancoord /= ensemble.Length;

                double bfactori = 0;
                foreach(List<Vector> coords in ensemble)
                    bfactori += (meancoord - coords[i]).Dist2;
                bfactori /= ensemble.Length;

                bfactor[i] = bfactori;
            }

            return new List<double>(bfactor);
        }

        public static double[] GetBFactor(Mode[] modes, double[] mass=null)
        {
            if(HDebug.Selftest())
                #region selftest
            {
                using(new Matlab.NamedLock("SELFTEST"))
                {
                    Matlab.Clear("SELFTEST");
                    Matlab.Execute("SELFTEST.hess = rand(30);");
                    Matlab.Execute("SELFTEST.hess = SELFTEST.hess + SELFTEST.hess';");
                    Matlab.Execute("SELFTEST.invhess = inv(SELFTEST.hess);");
                    Matlab.Execute("SELFTEST.bfactor3 = diag(SELFTEST.invhess);");
                    Matlab.Execute("SELFTEST.bfactor = SELFTEST.bfactor3(1:3:end) + SELFTEST.bfactor3(2:3:end) + SELFTEST.bfactor3(3:3:end);");
                    MatrixByArr selftest_hess = Matlab.GetMatrix("SELFTEST.hess");
                    Mode[] selftest_mode = Hess.GetModesFromHess(selftest_hess);
                    Vector selftest_bfactor = BFactor.GetBFactor(selftest_mode);
                    Vector selftest_check   = Matlab.GetVector("SELFTEST.bfactor");
                    Vector selftest_diff = selftest_bfactor - selftest_check;
                    HDebug.AssertTolerance(0.00000001, selftest_diff);
                    Matlab.Clear("SELFTEST");
                }
            }
                #endregion

            int size = modes[0].size;
            if(mass != null) HDebug.Assert(size == mass.Length);
            double[] bfactor = new double[size];
            for(int i=0; i<size; i++)
            {
                foreach(Mode mode in modes)
                {
                    bfactor[i] += mode.eigvec[i*3+0]*mode.eigvec[i*3+0]/mode.eigval;
                    bfactor[i] += mode.eigvec[i*3+1]*mode.eigvec[i*3+1]/mode.eigval;
                    bfactor[i] += mode.eigvec[i*3+2]*mode.eigvec[i*3+2]/mode.eigval;
                }
                if(mass != null)
                    bfactor[i] /= mass[i];
            }
            return bfactor;
        }

        public static double Corr( Tuple<Vector, string[], int[]> bfactor1
                                 , Tuple<Vector, string[], int[]> bfactor2
                                 )
        {
            double corr = Corr( bfactor1.Item1, bfactor1.Item2, bfactor1.Item3
                              , bfactor2.Item1, bfactor2.Item2, bfactor2.Item3
                              );
            return corr;
        }

        private static List<Tuple<int,int>> GetIndexCommon(string[] lstName1, int[] lstResSeq1, string[] lstName2, int[] lstResSeq2)
        {
            if(HDebug.Selftest())
            #region selftest
            {
                List<int> idx1, idx2;
                {
                    HDebug.Assert(lstName1.Length == lstResSeq1.Length);
                    int size1 = lstName1.Length;
                    List<Tuple<string, int>> name_resseq_1 = new List<Tuple<string, int>>(size1);
                    for(int i=0; i<size1; i++)
                    {
                        string name = new string(lstName1[i].HToArray().HSort().ToArray());
                        int resseq = lstResSeq1[i];
                        name_resseq_1.Add(new Tuple<string, int>(name, resseq));
                    }

                    HDebug.Assert(lstName2.Length == lstResSeq2.Length);
                    int size2 = lstName2.Length;
                    List<Tuple<string, int>> name_resseq_2 = new List<Tuple<string, int>>(size2);
                    for(int i=0; i<size2; i++)
                    {
                        string name = new string(lstName2[i].HToArray().HSort().ToArray());
                        int resseq = lstResSeq2[i];
                        name_resseq_2.Add(new Tuple<string, int>(name, resseq));
                    }

                    idx1 = new List<int>();
                    idx2 = new List<int>();
                    for(int i=0; i<name_resseq_1.Count; i++)
                    {
                        int j = name_resseq_2.IndexOf(name_resseq_1[i]);
                        if(j != -1)
                        {
                            idx1.Add(i);
                            idx2.Add(j);
                        }
                    }
                }
                List<Tuple<int, int>> idx12 = GetIndexCommon(lstName1, lstResSeq1, lstName2, lstResSeq2);
                HDebug.Assert(idx12.Count == idx1.Count);

            }
            #endregion

            lstName1 = lstName1.HTrim().ToArray();
            lstName2 = lstName2.HTrim().ToArray();
            List<Tuple<string, int>> lstNameResseq1 = lstName1.HToIListTuple(lstResSeq1).ToList();
            List<Tuple<string, int>> lstNameResseq2 = lstName2.HToIListTuple(lstResSeq2).ToList();

            List<Tuple<int, int>> lstIdxCommon = lstNameResseq1.HListIndexCommon(lstNameResseq2);
            return lstIdxCommon;
        }
        public static double Corr(Vector bfactor1, string[] lstName1, int[] lstResSeq1
                                 , Vector bfactor2, string[] lstName2, int[] lstResSeq2
                                 )
        {
            List<Tuple<int,int>> idx12 = GetIndexCommon(lstName1, lstResSeq1, lstName2, lstResSeq2);
            List<int> idx1 = idx12.HListItem1().ToList();
            List<int> idx2 = idx12.HListItem2().ToList();

            bfactor1 = bfactor1.ToArray().HSelectByIndex(idx1).ToArray();
            bfactor2 = bfactor2.ToArray().HSelectByIndex(idx2).ToArray();

            return Corr(bfactor1, bfactor2);
        }

        public static double Corr(Vector bfactor1, Vector bfactor2, IList<int> idxcorr)
        {
            Vector bfactor1s = bfactor1.ToArray().HSelectByIndex(idxcorr);
            Vector bfactor2s = bfactor2.ToArray().HSelectByIndex(idxcorr);
            double corr = Corr(bfactor1s, bfactor2s);
            return corr;
        }
        public static double Corr(Vector bfactor1, Vector bfactor2, bool ignore_nan = false)
        {
            double hcorr = HMath.HCorr(bfactor1, bfactor2, ignore_nan);

            /////////////////////////////////////////////////////////////////////
            // this has been tested enough
            if(HDebug.False && HDebug.IsDebuggerAttached)
            #region selftest
            {
                double corr = double.NaN;
                using(new Matlab.NamedLock("CORR"))
                {
                    Matlab.Clear("CORR");
                    Matlab.PutVector("CORR.bfactor1", bfactor1);
                    Matlab.PutVector("CORR.bfactor2", bfactor2);
                    if(ignore_nan)
                    {
                        Matlab.Execute("CORR.idxnan = isnan(CORR.bfactor1) | isnan(CORR.bfactor2);");
                        Matlab.Execute("CORR.bfactor1 = CORR.bfactor1(~CORR.idxnan);");
                        Matlab.Execute("CORR.bfactor2 = CORR.bfactor2(~CORR.idxnan);");
                    }
                    if(Matlab.GetValueInt("min(size(CORR.bfactor1))") != 0)
                        corr = Matlab.GetValue("corr(CORR.bfactor1, CORR.bfactor2)");
                    Matlab.Clear("CORR");
                }
                if((double.IsNaN(hcorr) && double.IsNaN(corr)) == false)
                    HDebug.AssertTolerance(0.00000001, hcorr-corr);
                //HDebug.ToDo("use HMath.HCorr(...) instead");
            }
            #endregion
            // this has been tested enough
            /////////////////////////////////////////////////////////////////////
            return hcorr;
        }

        public static List<double> ScaleBFactorComp(List<double> bfactorExpr, List<double> bfactorComp, HPack<double> outScale4BfactorComp, HPack<double> outAdd4BfactorComp)
        {
            HDebug.Assert(bfactorExpr.Count == bfactorComp.Count);
            int size = bfactorComp.Count;
            Matrix A = new double[size, 2];
            Vector b = new double[size];
            for(int i=0; i<size; i++)
            {
                A[i, 0] = bfactorComp[i];
                A[i, 1] = 1;
                b[i] = bfactorExpr[i];
            }
            /// A x = b
            /// (A' A) x = (A' b)
            /// x = inv(A' A) * (A' b)
            Matrix At = A.Tr();
            Matrix AtA = At * A;
            Vector Atb = LinAlg.MV(At, b);      // = At * b;
            Matrix invAtA = NumericSolver.Inv(AtA);
            Vector x = LinAlg.MV(invAtA, Atb);  // = invAtA * Atb;
            double scale = x[0];
            double   add = x[1];
            double[] bfactorCompScaled = new double[size];
            for(int i=0; i<size; i++)
                bfactorCompScaled[i] = bfactorComp[i] * scale + add;
            if(outScale4BfactorComp != null) outScale4BfactorComp.value = scale;
            if(  outAdd4BfactorComp != null)   outAdd4BfactorComp.value = add;
            return new List<double>(bfactorCompScaled);
        }
        public static List<double> ScaleBFactorComp(List<double> bfactorExpr, List<double> bfactorComp, HPack<double> outScale4BfactorComp)
        {
            HDebug.Assert(bfactorExpr.Count == bfactorComp.Count);
            int size = bfactorComp.Count;
            Matrix A = new double[size, 1];
            //Matrix A = new double[size, 2];
            Vector b = new double[size];
            for(int i=0; i<size; i++)
            {
                A[i, 0] = bfactorComp[i];
                //A[i, 1] = 1;
                b[i] = bfactorExpr[i];
            }
            /// A x = b
            /// (A' A) x = (A' b)
            /// x = inv(A' A) * (A' b)
            Matrix At = A.Tr();
            Matrix AtA = At * A;
            Vector Atb = LinAlg.MV(At, b);      // = At * b;
            Matrix invAtA = NumericSolver.Inv(AtA);
            Vector x = LinAlg.MV(invAtA, Atb);  // = invAtA * Atb;
            double scale = x[0];
            double add = 0;//= x[1];
            double[] bfactorCompScaled = new double[size];
            for(int i=0; i<size; i++)
                bfactorCompScaled[i] = bfactorComp[i] * scale + add;
            if(outScale4BfactorComp != null) outScale4BfactorComp.value = scale;
            return new List<double>(bfactorCompScaled);
        }
    }
}
}
