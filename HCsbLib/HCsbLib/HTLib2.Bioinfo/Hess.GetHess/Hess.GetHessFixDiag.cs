using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static HessMatrix GetHessFixDiag(HessMatrix hess)
        {
            int size = hess.ColSize/3;
            HDebug.Assert(size*3 == hess.ColSize, size*3 == hess.RowSize);
            HessMatrix fix = hess.Zeros(size*3, size*3);

            for(int c=0; c<size; c++)
            {
                for(int r=0; r<size; r++)
                {
                    if(c == r) continue;
                    double v;
                    HDebug.AssertTolerance(0.00000001, hess[c*3+0, r*3+0] - hess[r*3+0, c*3+0]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+0, r*3+1] - hess[r*3+1, c*3+0]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+0, r*3+2] - hess[r*3+2, c*3+0]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+1, r*3+0] - hess[r*3+0, c*3+1]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+1, r*3+1] - hess[r*3+1, c*3+1]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+1, r*3+2] - hess[r*3+2, c*3+1]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+2, r*3+0] - hess[r*3+0, c*3+2]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+2, r*3+1] - hess[r*3+1, c*3+2]);
                    HDebug.AssertTolerance(0.00000001, hess[c*3+2, r*3+2] - hess[r*3+2, c*3+2]);

                    v = fix[c*3+0, r*3+0] = hess[c*3+0, r*3+0];     fix[c*3+0, c*3+0] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+0, r*3+1] = hess[c*3+0, r*3+1];     fix[c*3+0, c*3+1] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+0, r*3+2] = hess[c*3+0, r*3+2];     fix[c*3+0, c*3+2] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+1, r*3+0] = hess[c*3+1, r*3+0];     fix[c*3+1, c*3+0] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+1, r*3+1] = hess[c*3+1, r*3+1];     fix[c*3+1, c*3+1] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+1, r*3+2] = hess[c*3+1, r*3+2];     fix[c*3+1, c*3+2] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+2, r*3+0] = hess[c*3+2, r*3+0];     fix[c*3+2, c*3+0] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+2, r*3+1] = hess[c*3+2, r*3+1];     fix[c*3+2, c*3+1] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                    v = fix[c*3+2, r*3+2] = hess[c*3+2, r*3+2];     fix[c*3+2, c*3+2] -= v;     HDebug.Assert(double.IsNaN(v) == false);
                }
            }

            if(HDebug.IsDebuggerAttached)
            {
                for(int c=0; c<size*3; c++)
                {
                    double sum0 = 0;
                    double sum1 = 0;
                    double sum2 = 0;
                    for(int r=0; r<size*3; r+=3)
                    {
                        sum0 += fix[c, r+0];
                        sum1 += fix[c, r+1];
                        sum2 += fix[c, r+2];
                        if(c/3 != r/3)
                        {
                            HDebug.Assert(fix[c,r+0] == hess[c,r+0]);
                            HDebug.Assert(fix[c,r+1] == hess[c,r+1]);
                            HDebug.Assert(fix[c,r+2] == hess[c,r+2]);
                        }
                    }
                    HDebug.AssertTolerance(0.00000001, sum0);
                    HDebug.AssertTolerance(0.00000001, sum1);
                    HDebug.AssertTolerance(0.00000001, sum2);
                }
            }

            return fix;
        }
    }
}
