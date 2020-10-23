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
        public static Vector[] RotationalDegreesOfFreedom(IList<Vector> points, IList<double> masses)
        {
            Vector[] rotdof = new Vector[3];
            rotdof[0] = new double[points.Count*3];
            rotdof[1] = new double[points.Count*3];
            rotdof[2] = new double[points.Count*3];
            RotationalDegreesOfFreedom(points, masses, rotdof);

            if(HDebug.IsDebuggerAttached)
            {
                double dot01 = LinAlg.DotProd(rotdof[0], rotdof[1]);
                double dot02 = LinAlg.DotProd(rotdof[0], rotdof[2]);
                double dot12 = LinAlg.DotProd(rotdof[1], rotdof[2]);
            }

            return rotdof;
        }
        public static void RotationalDegreesOfFreedom_sub(IList<Vector> points, Vector axis, Vector mc, Vector rotdof)
        {
            for(int i=0; i<points.Count; i++)
            {
                Vector pt = points[i] - mc;
                Vector cross = LinAlg.CrossProd3(axis, pt);
                double pt_axis0_dist = Geometry.DistancePointLine(points[i], mc, mc+axis);
                HDebug.Assert(pt_axis0_dist >= 0);
                rotdof[i*3+0] = pt_axis0_dist * cross[0];
                rotdof[i*3+1] = pt_axis0_dist * cross[1];
                rotdof[i*3+2] = pt_axis0_dist * cross[2];
            }
            rotdof = rotdof.UnitVector();
        }
        public static void RotationalDegreesOfFreedom(IList<Vector> points, IList<double> masses, Vector[] rotdof)
        {
            // mass center
            Vector mc = new double[3];
            for(int i=0; i<points.Count; i++)
            {
                Vector pi = points[i];
                double mi = masses[i];
                mc[0] += pi[0]*mi;
                mc[1] += pi[1]*mi;
                mc[2] += pi[2]*mi;
            }
            mc[0] /= points.Count;
            mc[1] /= points.Count;
            mc[2] /= points.Count;

            var axis012 = CMomentOfInertiaTensor.MomentOfInertiaTensor(points, masses, mc);

            RotationalDegreesOfFreedom_sub(points, axis012.Item1, mc, rotdof[0]);
            RotationalDegreesOfFreedom_sub(points, axis012.Item2, mc, rotdof[1]);
            RotationalDegreesOfFreedom_sub(points, axis012.Item3, mc, rotdof[2]);

            return;
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
                // mass center
                Vector mc = new double[3];
                for(int i=0; i<points.Count; i++)
                {
                    Vector pi = points[i];
                    double mi = masses[i];
                    mc[0] += pi[0]*mi;
                    mc[1] += pi[1]*mi;
                    mc[2] += pi[2]*mi;
                }
                mc[0] /= points.Count;
                mc[1] /= points.Count;
                mc[2] /= points.Count;

                return MomentOfInertiaTensor(points, masses, mc);
            }
            public static (double[], double[], double[]) MomentOfInertiaTensor(IList<Vector> points, IList<double> masses, Vector mc)
            {
                int count = points.Count;

                double[,] I = new double[3,3];
                for(int i=0; i<count; i++)
                {
                    Vector pi = points[i];
                    double mi = masses[i];
                    AddToI(I, mi, pi[0]-mc[0], pi[1]-mc[1], pi[2]-mc[2]);
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
