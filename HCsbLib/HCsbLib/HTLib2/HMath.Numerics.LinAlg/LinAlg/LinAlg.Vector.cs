using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public static partial class LinAlg
    {
        public static double Angle(Vector vec, Vector xvec)
        {
            // function angle = torangle(atom1, atom2, atom3, atom4)
            //     plane_123 = cross(atom2-atom1, atom3-atom2); n1 = plane_123; n1 = n1 / sqrt(dot(n1,n1));
            //     plane_234 = cross(atom3-atom2, atom4-atom3); n2 = plane_234; n2 = n2 / sqrt(dot(n2,n2));
            //     vec_23 = atom3 - atom2;                      b  = vec_23;    b  = b  / sqrt(dot(b,b));
            //     angle = atan2(dot(cross(n1,n2),b), dot(n1,n2));
            // end
            //vec = vec.UnitVector();
            //xvec = xvec.UnitVector();
            //Vector b = Vector.CrossProd(vec, xvec);
            //double dx = Vector.DotProd(vec, xvec);
            //double dy = Vector.DotProd(Vector.CrossProd(vec, xvec), b);
            vec = vec.UnitVector();
            xvec = xvec.UnitVector();
            double acos = VtV(vec, xvec) / (vec.Dist * xvec.Dist);
            double absacos = Math.Abs(acos);
            if(absacos > 1 && absacos < 1.1)
                acos = Math.Sign(acos); // resolve numerical error
            double angle0 = Math.Acos(acos);
            double angle1 = Math.PI - angle0;
            double angle = Math.Min(angle0, angle1);
            HDebug.Assert(angle >= 0);
            return angle;
        }
    }
}
