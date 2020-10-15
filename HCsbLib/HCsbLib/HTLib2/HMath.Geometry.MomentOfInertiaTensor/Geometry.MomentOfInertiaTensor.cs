using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
	public partial class Geometry
	{
        public static (double[], double[], double[]) MomentOfInertiaTensor(IList<Vector> points, IList<double> masses)
        {
            return CMomentOfInertiaTensor.MomentOfInertiaTensor(points, masses);
        }
        public partial class CMomentOfInertiaTensor
        {
            public static void AddToI(double[,] I, double mi, double[] dveci)
            {
                HDebug.Assert(I.GetLength(0) == 3 && I.GetLength(1) == 3);
                HDebug.Assert(dveci.Length == 3);
                AddToI(I, mi, dveci[0], dveci[1], dveci[2]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                I[0,0] += mi*(yi2 + zi2);   I[0,1] += mi*(-1*xiyi  );   I[0,2] += mi*(-1*xizi  );  
                I[1,0] += mi*(-1*xiyi  );   I[1,1] += mi*(xi2 + zi2);   I[1,2] += mi*(-1*yizi  );  
                I[2,0] += mi*(-1*xizi  );   I[2,1] += mi*(-1*yizi  );   I[2,2] += mi*(xi2 + yi2);
            }
            public static (double[], double[], double[]) MomentOfInertiaTensor(IList<Vector> points, IList<double> masses)
            {
                int count = points.Count;

                // mass center
                double mc0 = 0;
                double mc1 = 0;
                double mc2 = 0;
                for(int i=0; i<count; i++)
                {
                    Vector pi = points[i];
                    double mi = masses[i];
                    mc0 += pi[0]*mi;
                    mc1 += pi[1]*mi;
                    mc2 += pi[2]*mi;
                }

                double[,] I = new double[3,3];
                for(int i=0; i<count; i++)
                {
                    Vector pi = points[i];
                    double mi = masses[i];
                    AddToI(I, mi, pi[0]-mc0, pi[1]-mc1, pi[2]-mc2);
                }

                double[] eigval;
                double[,] eigvec;

                //LinAlg.Eig(I)
                bool success = alglib.smatrixevd(I, 3, 1, false, out eigval, out eigvec);
                double eigval0 = eigval[0]; double[] eigvec0 = new double[] { eigvec[0,0], eigvec[1,0], eigvec[2,0] };
                double eigval1 = eigval[1]; double[] eigvec1 = new double[] { eigvec[0,1], eigvec[1,1], eigvec[2,1] };
                double eigval2 = eigval[2]; double[] eigvec2 = new double[] { eigvec[0,2], eigvec[1,2], eigvec[2,2] };

                return (eigvec0, eigvec1, eigvec2);
            }
        }
	}
}
