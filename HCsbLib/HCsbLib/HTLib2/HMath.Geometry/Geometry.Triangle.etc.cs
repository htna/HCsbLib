using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        static void GetTriGeom( Vector pt1, Vector pt2, Vector pt3
                                     , out double a, out double b, out double c
                                     , out double A, out double B, out double C
                                     , out double r
                                     , out Vector pto
                                     )
        {
            Func<Vector,Vector,Vector,double> angle2 = delegate(Vector p1, Vector p2, Vector p3)
            {
                Vector v21 = p1 - p2;
                Vector v23 = p3 - p2;
                double d21 = v21.Dist;
                double d23 = v23.Dist;
                double cos2 = LinAlg.DotProd(v21, v23) / (d21 * d23);
                double ang2 = Math.Acos(cos2);
                return ang2;
            };
            ////////////////////////////////////////////////////////////////
            //             1     (1,2,3)
            //            /A\      |
            //           / | \     r   
            //          c  |  b    |
            //         /   o   \   origin
            //        /B       C\
            //       2-----a-----3
            //
            /////////////////////////////////////////////////////////////////
            // http://schools-wikipedia.org/wp/t/Trigonometric_functions.htm
            // a/sinA = b/sinB = c/sinC = 2R
            Vector pt23 = pt2 - pt3; double dist23 = pt23.Dist;
            Vector pt31 = pt3 - pt1; double dist31 = pt31.Dist;
            Vector pt12 = pt1 - pt2; double dist12 = pt12.Dist;
            a = dist23; // =(pt2-pt3).Dist; // distance between pt2 and pt3
            b = dist31; // =(pt3-pt1).Dist; // distance between pt2 and pt3
            c = dist12; // =(pt1-pt2).Dist; // distance between pt2 and pt3
            A = angle2(pt3, pt1, pt2); // angle A = ∠pt3-pt1(a)-pt2
            B = angle2(pt1, pt2, pt3); // angle B = ∠pt1-pt2(b)-pt3
            C = angle2(pt2, pt3, pt1); // angle C = ∠pt2-pt3(c)-pt1
            r = 0.5 * a / Math.Sin(A);
            // Barycentric coordinates from cross- and dot-products
            // http://en.wikipedia.org/wiki/Circumscribed_circle#Barycentric_coordinates_as_a_function_of_the_side_lengths
            /// po = v1.p1 + v2.p2 + v3.p3
            /// v1 = (|p2-p3|^2 * (p1-p2).(p1-p3)) / (2*|(p1-p2)x(p2-p3)|^2) = a*a*Dot(
            /// v2 = (|p1-p3|^2 * (p2-p1).(p2-p3)) / (2*|(p1-p2)x(p2-p3)|^2) = b*b*Dot(
            /// v3 = (|p1-p2|^2 * (p3-p1).(p3-p2)) / (2*|(p1-p2)x(p2-p3)|^2) = c*c*Dot(
            {
                double div = 2 * LinAlg.CrossProd(pt1-pt2, pt2-pt3).Dist2;
                double v1  = dist23*dist23 * LinAlg.DotProd( pt12, -pt31) / div;
                double v2  = dist31*dist31 * LinAlg.DotProd(-pt12,  pt23) / div;
                double v3  = dist12*dist12 * LinAlg.DotProd( pt31, -pt23) / div;
                pto = v1*pt1 + v2*pt2 + v3*pt3;
            }
        }
        static double RadiusOfTriangle(Vector pt1, Vector pt2, Vector pt3)
        {
            double a, b, c, A, B, C, r;
            Vector pto;
            GetTriGeom(pt1, pt2, pt3,
                       out a, out b, out c,
                       out A, out B, out C,
                       out r,
                       out pto
                      );
            return r;
        }
    }
}
