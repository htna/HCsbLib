using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static double TorsionalAngle(Vector p1, Vector p2, Vector p3, Vector p4)
        {
            //function angle = torangle(atom1, atom2, atom3, atom4)
            //    plane_123 = cross(atom2-atom1, atom3-atom2); n1 = plane_123; n1 = n1 / sqrt(dot(n1,n1));
            //    plane_234 = cross(atom3-atom2, atom4-atom3); n2 = plane_234; n2 = n2 / sqrt(dot(n2,n2));
            //    vec_23 = atom3 - atom2;                      b  = vec_23;    b  = b  / sqrt(dot(b,b));
            //    angle = atan2(dot(cross(n1,n2),b), dot(n1,n2));
            //end

            double angle;
            {
                Vector plane_123 = LinAlg.CrossProd(p2-p1, p3-p2); Vector n1 = plane_123.UnitVector();
                Vector plane_234 = LinAlg.CrossProd(p3-p2, p4-p3); Vector n2 = plane_234.UnitVector();
                Vector vec_23 = p3 - p2; Vector b  = vec_23.UnitVector();
                angle = Math.Atan2(LinAlg.VtV(LinAlg.CrossProd(n1, n2), b), LinAlg.VtV(n1, n2));
            }
            if(HDebug.IsDebuggerAttached)
            {
                Vector p12 = p2 - p1;
                Vector p23 = p3 - p2;
                Vector p34 = p4 - p3;
                Vector nn1 = LinAlg.CrossProd(p12, p23).UnitVector();
                Vector nn2 = LinAlg.CrossProd(p23, p34).UnitVector();
                double a = AngleBetween(nn1, nn2);
                HDebug.Assert(Math.Abs(Math.Abs(a) - Math.Abs(angle)) < 0.00000001);
            }
            if(HDebug.IsDebuggerAttached)
            {
                /// book: "Protein Actions Principles & Modeling" by Ivet Bahar, Robert L Jernigan, and Ken A Dill
                /// equation (9.6) in box 9.2 in page 204 (Chapter 9)
                Vector l2 = p2 - p1;
                Vector l3 = p3 - p2;
                Vector l4 = p4 - p3;
              //Vector n1 = LinAlg.CrossProd(l1, l2).UnitVector();
                Vector n2 = LinAlg.CrossProd(l2, l3).UnitVector();
                Vector n3 = LinAlg.CrossProd(l3, l4).UnitVector();
              //double d1 = LinAlg.DotProd(n0, n1);     double a1 = Math.Sign(Math.Sin(d1)) * Math.Acos(d1);
              //double d2 = LinAlg.DotProd(n1, n2);     double a2 = Math.Sign(Math.Sin(d2)) * Math.Acos(d2);
                double d3 = LinAlg.DotProd(n2, n3);     double a3 = Math.Sign(Math.Sin(d3)) * Math.Acos(d3);
              //double d4 = LinAlg.DotProd(n3, n4);     double a4 = Math.Sign(Math.Sin(d4)) * Math.Acos(d4);

                HDebug.Assert(Math.Abs(d3 - angle) < 0.00000001);
            }
            return angle;
        }
	}
}
