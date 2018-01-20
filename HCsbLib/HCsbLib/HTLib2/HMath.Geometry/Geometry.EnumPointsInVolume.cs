using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static IEnumerable<Vector> EnumPointsInVolume_UniformCube(double maxx, double maxy, double maxz)
        {
            for(int x=0; x<maxx; x++)
            {
                for(int y=0; y<maxy; y++)
                {
                    for(int z=0; z<maxy; z++)
                    {
                        Vector point = new double[3] { x, y, z };
                        yield return point;
                    }
                }
            }
        }
        public static IEnumerable<Vector> EnumPointsInVolume_UniformTetrahedron(double maxx, double maxy, double maxz)
        {
            //double maxx = 7;
            //double maxy = 7;
            //double maxz = 7;

            // determine point locations of equilateral tetrahedron
            // where v1 is on the origin, and length of edge is 1
            Vector v1, v2, v3, v4;
            double dz, dx, dy;
            {
                var v1234 = Geometry.Points4TetrahedronEquilateral();
                v1 = v1234.Item1;
                v2 = v1234.Item2;
                v3 = v1234.Item3;
                v4 = v1234.Item4;
                // check length
                HDebug.AssertTolerance(0.00000001, 1 - (v1 - v2).Dist);
                HDebug.AssertTolerance(0.00000001, 1 - (v1 - v3).Dist);
                HDebug.AssertTolerance(0.00000001, 1 - (v1 - v4).Dist);
                HDebug.AssertTolerance(0.00000001, 1 - (v2 - v3).Dist);
                HDebug.AssertTolerance(0.00000001, 1 - (v2 - v4).Dist);
                HDebug.AssertTolerance(0.00000001, 1 - (v3 - v4).Dist);
                // check height (v1.z == v2.z == v3.z)
                HDebug.AssertTolerance(0.00000001, v1[2] - v2[2]);
                HDebug.AssertTolerance(0.00000001, v1[2] - v2[2]);
                HDebug.AssertTolerance(0.00000001, v1[2] - v3[2]);
                // check others
                HDebug.AssertTolerance(0.00000001, v1[2] - 0); // v1,v2,v3 are on z-plane
                HDebug.AssertTolerance(0.00000001, v2[2] - 0); // v1,v2,v3 are on z-plane
                HDebug.AssertTolerance(0.00000001, v3[2] - 0); // v1,v2,v3 are on z-plane
                HDebug.AssertTolerance(0.00000001, v2[1] - 0); // v2,v3 are on x-axis
                HDebug.AssertTolerance(0.00000001, v3[1] - 0); // v2,v3 are on x-axis
                HDebug.AssertTolerance(0.00000001, v2[0] - 1); // v3 is on (1,0,0)
                ///       v1
                ///      /  \
                ///     /    \
                ///    /  v4  \
                ///   /        \
                /// v3----------v2
                /// 
                // determine height
                dx = v2[0];
                dy = Math.Abs(v1[1]);
                dz = Math.Abs(v4[2] - v1[2]);
            }

            //List<Vector> points = new List<Vector>();
            //Vector points_last;
            //Vector refpt = v1.Clone();

            /// v1x        v1      
            ///  \        /  \     
            ///   \      /    \    
            /// x--\----/--v4  \   
            ///  \  \  /  /     \  
            ///   \  v3--/-------v2
            ///    \    /
            ///     \  /
            ///      vx
            for(int iz=0; true; iz++)
            {
                double z =                   iz*dz;
                double x0 = (iz%2 == 0) ? 0 : v4[0];
                double y0 = (iz%2 == 0) ? 0 : v4[1];

                //z *= edgeleng;
                if(z < 0) continue;
                if(z > maxz) break;


                for(int iy=0; true; iy++)
                {
                    double y =         iy*dy   + y0;
                    //y *= edgeleng;
                    if(y < 0) continue;
                    if(y > maxy) break;

                    for(int ix=0; true; ix++)
                    {
                        double x = ix*dx - iy*dx/2 + x0;
                        //x *= edgeleng;
                        if(x < 0) continue;
                        if(x > maxx) break;

                        Vector point = new double[3] { x, y, z };
                        yield return point;
                        //points_last = new double[3] { x, y, z };
                        //points.Add(points_last);
                    }
                }
            }
        }
	}
}
