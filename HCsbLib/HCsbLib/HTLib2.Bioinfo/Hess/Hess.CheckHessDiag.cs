using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static bool CheckHessDiag_selftest = HDebug.IsDebuggerAttached;
        public static bool CheckHessDiag(Matrix hessian, double tolerSumDiag, string exceptmsg=null)
        {
            if(CheckHessDiag_selftest)
            {
                CheckHessDiag_selftest = false;

                Matrix thess = new double[,]{ {-1,-2, 0,    1, 2, 3 }
                                            , {-2,-4,-5,    2, 4, 5 }
                                            , {-3,-5,-6,    3, 5, 6 }
                                            , { 1,-2, 3,   -1, 2,-3 }
                                            , { 2,-4,-5,   -2, 4, 5 }
                                            , {-3, 5, 6,    3,-5,-6 }
                                            };
                HDebug.Assert(Hess.CheckHessDiag(thess, 0.0000001) == false);
                thess[0, 2] = -3;
                HDebug.Assert(Hess.CheckHessDiag(thess, 0.0000001) == true);
                for(int i=0; i<thess.ColSize; i++) thess[i, i] = 0;
                HDebug.Assert(Hess.CheckHessDiag(thess, 0.0000001) == false);
            }

            // tolerSumDiag = 0.000001
            HDebug.Assert(hessian.ColSize == hessian.RowSize);
            HDebug.Assert(hessian.ColSize % 3 == 0);
            int size = hessian.ColSize / 3;
            List<double> sums = new List<double>();

            for(int i = 0; i < size; i++)
                for(int di = 0; di < 3; di++)
                    for(int dj = 0; dj < 3; dj++)
                    {
                        double diagval = 0;
                        for(int j = 0; j < size; j++)
                        {
                            if(i == j) continue;
                            double hessian_idi_jdj = hessian[3*i+di, 3*j+dj];
                            if(double.IsNaN     (hessian_idi_jdj)) { HDebug.Assert(false); return false; } // throw new Exception("NotComputable :"+exceptmsg);
                            if(double.IsInfinity(hessian_idi_jdj)) { HDebug.Assert(false); return false; } //throw new Exception("NotComputable :"+exceptmsg);
                            diagval += hessian_idi_jdj;
                        }
                        double sum = diagval+hessian[3*i+di, 3*i+dj];
                        sums.Add(sum);
                    }
            double maxsum = Math.Max(Math.Abs(sums.Min()), Math.Abs(sums.Max()));
            if(HDebug.CheckTolerance(tolerSumDiag, maxsum) == false)
                return false;
            return true;
        }
    }
}
