using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public partial class Trans
        {
            public partial class Rotation
            {
                public static MatrixByArr GetRotMatrix(double rx, double ry, double rz)
                {
                    // http://en.wikipedia.org/wiki/Rotation_matrix#General_rotations
                    //
                    // R_z(\psi) \, R_y(\theta) \, R_x(\phi)\,=
                    // \begin{bmatrix}
                    // \cos\theta \cos\psi &amp; -\cos\phi \sin\psi + \sin\phi \sin\theta \cos\psi &amp;   \sin\phi \sin\psi + \cos\phi \sin\theta \cos\psi \\
                    // \cos\theta \sin\psi &amp;  \cos\phi \cos\psi + \sin\phi \sin\theta \sin\psi &amp; -\sin\phi \cos\psi + \cos\phi \sin\theta \sin\psi \\
                    // -\sin\theta             &amp;  \sin\phi \cos\theta                                          &amp;   \cos\phi \cos\theta \\
                    // \end{bmatrix}

                    double cosrx = Math.Cos(rx); double sinrx = Math.Sin(rx);
                    double cosry = Math.Cos(ry); double sinry = Math.Sin(ry);
                    double cosrz = Math.Cos(rz); double sinrz = Math.Sin(rz);
                    MatrixByArr RzRyRx = new double[3,3] { { cosry*cosrz ,  -cosrx*sinrz + sinrx*sinry*cosrz ,   sinrx*sinrz + cosrx*sinry*cosrz},
                                                      { cosry*sinrz ,   cosrx*cosrz + sinrx*sinry*sinrz ,  -sinrx*cosrz + cosrx*sinry*sinrz},
                                                      {-sinry       ,                 sinrx*cosry       ,                 cosrx*cosry      },
                                                    };
                    return RzRyRx;
                }
                public static Vector GetRotAxis(MatrixByArr rot)
                {
                    HDebug.ToDo("write selftest code");
                    // http://en.wikipedia.org/wiki/Rotation_matrix#Determining_the_axis
                    //
                    // Determining the axis
                    //     Given a rotation matrix R, a vector u parallel to the rotation axis must satisfy
                    //         R u = u
                    //     since the rotation of u around the rotation axis must result in u. The equation
                    //     above may be solved for u which is unique up to a scalar factor.
                    //     Further, the equation may be rewritten
                    //         R u = I u  =>  (R-I) u = 0
                    //     which shows that u is the null space of R-I. Viewed another way, u is an eigenvector
                    //     of R corresponding to the eigenvalue λ=1(every rotation matrix must have this eigenvalue).
                    HDebug.Assert(IsRotMatrix(rot));
                    MatrixByArr RI = rot - LinAlg.Eye(3);
                    Vector[] eigvec;
                    double[] eigval;
                    NumericSolver.Eig(RI, out eigvec, out eigval);
                    int idx = eigval.HAbs().HIdxMin();
                    Vector axis = eigvec[idx];
                    return axis;
                }
                public static double GetRotAngle(MatrixByArr rot)
                {
                    HDebug.ToDo("write selftest code");
                    // Determining the angle
                    //     To find the angle of a rotation, once the axis of the rotation is known, select
                    //     a vector v perpendicular to the axis. Then the angle of the rotation is the angle
                    //     between v and Rv.
                    //     A much easier method, however, is to calculate the trace (i.e. the sum of the
                    //     diagonal elements of the rotation matrix) which is 1+2cosθ.
                    HDebug.Assert(IsRotMatrix(rot));
                    double tr = rot.Trace();
                    double angle = Math.Acos((tr-1)/2);
                    return angle;
                }
                public static bool IsRotMatrix(MatrixByArr rot)
                {
                    // http://en.wikipedia.org/wiki/Rotation_matrix#Properties_of_a_rotation_matrix

                    if(rot.ColSize != 3) return false;
                    if(rot.RowSize != 3) return false;
                    // R^t = R^-1
                    // det R = 1
                    {   // (R-I)u = 0 where u is the null space of R-I
                        MatrixByArr RI = rot - LinAlg.Eye(3);
                        Vector[] eigvec;
                        double[] eigval;
                        NumericSolver.Eig(RI, out eigvec, out eigval);
                        double min_eigval = eigval.HAbs().Min();
                        if(min_eigval > 0.0000001) return false;
                    }
                    HDebug.ToDo("write selftest code");
                    return true;
                }
            }
        }
    }
}
