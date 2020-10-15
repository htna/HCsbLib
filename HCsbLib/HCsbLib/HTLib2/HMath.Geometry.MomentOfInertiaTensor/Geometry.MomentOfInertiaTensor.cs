using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public partial class CMomentOfInertiaTensor
        {
            public static void AddToI(double[,] I, double mi, double[] dveci)
            {
                HDebug.Assert(I.GetLength(0) == 3 && I.GetLength(1) == 3);
                HDebug.Assert(dveci.Length == 3);
                AddToI(I, mi, dveci[0], dveci[1], dveci[2]);
            }
            public static void AddToI(double[,] I, double mi, double dxi, double dyi, double dzi)
            {
                HDebug.Assert(I.GetLength(0) == 3 && I.GetLength(1) == 3);
                ///               [ yi^2 + zi^2     - xi yi         - xi zi     ]
                /// I^ = sum_i mi [ - xi yi         xi^2 + zi^2     - yi zi     ]
                ///               [ - xi zi         - yi zi         xi^2 + yi^2 ]
                double xi2  = dxi * dxi;
                double yi2  = dyi * dyi;
                double zi2  = dzi * dzi;
                double xiyi = dxi * dyi;
                double xizi = dxi * dzi;
                double yizi = dyi * dzi;
                I[0,0] += yi2 + zi2;    I[0,1] += -1*xiyi  ;    I[0,2] += -1*xizi  ;  
                I[1,0] += -1*xiyi  ;    I[1,1] += xi2 + zi2;    I[1,2] += -1*yizi  ;  
                I[2,0] += -1*xizi  ;    I[2,1] += -1*yizi  ;    I[2,2] += xi2 + yi2;
            }
        }
	}
}
