using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        static bool LineOnTwoPlanes_SelfTest = HDebug.IsDebuggerAttached;
        public static Tuple<Vector,Vector> LineOnTwoPlanes(Vector norm1, double val1, Vector norm2, double val2)
        {
            /// http://intumath.org/Math/Geometry/Analytic%20Geometry/plane-planeinter.html
            /// 
            if(LineOnTwoPlanes_SelfTest)
            {
                LineOnTwoPlanes_SelfTest = false;
                Vector tn1 = new double[] { 1, 2, 3 };
                Vector tn2 = new double[] { 2, 5, 3 };
                double td1 = 10;
                double td2 = 2;
                LineOnTwoPlanes(tn1, td1, tn2, td2);
            }

            Vector norm = LinAlg.CrossProd(norm1, norm2);
            /// norm1' * pt = -val1      [n1x n1y n1z]   [ptx]   [-val1]      [n1x n1y 0]   [ptx]   [-val1]
            /// norm2' * pt = -val2  =>  [n2x n2y n2z] * [pty] = [-val2]  =>  [n2x n2y 0] * [pty] = [-val2]
            ///                                          [ptz]                              [0  ]          
            ///                      =>  [n1x n1y] * [ptx] = [-val1]  =>  [n1x n1y] * [ptx] = [-val1]
            ///                          [n2x n2y]   [pty]   [-val2]      [n2x n2y]   [pty]   [-val2]
            ///                      =>  [ptx] = [n1x n1y]-1  [-val1] = [ n2y -n1y] * [-val1] / (n1x*n2y - n1y*n2x)
            ///                          [pty]   [n2x n2y]  * [-val2]   [-n2x  n1x]   [-val2]
            ///                                = [ n2y*-val1 + -n1y*-val2 ] / (n1x*n2y - n1y*n2x)
            ///                                  [-n2x*-val1 +  n1x*-val2 ]
            double n1x=norm1[0], n1y=norm1[1];
            double n2x=norm2[0], n2y=norm2[1];
            double div = n1x*n2y - n1y*n2x;
            Vector pt = new double[]{( n2y*-val1 + -n1y*-val2)/div,
                                     (-n2x*-val1 +  n1x*-val2)/div,
                                     0};

            if(HDebug.IsDebuggerAttached)
            {
                Vector dbg_pt;
                dbg_pt = pt + 1 * norm;
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(norm1, dbg_pt)+val1);
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(norm2, dbg_pt)+val2);
                dbg_pt = pt + 2 * norm;
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(norm1, dbg_pt)+val1);
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(norm2, dbg_pt)+val2);
                dbg_pt = pt - 2 * norm;
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(norm1, dbg_pt)+val1);
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(norm2, dbg_pt)+val2);
            }
            return new Tuple<Vector,Vector>(pt,norm);
        }
	}
}
