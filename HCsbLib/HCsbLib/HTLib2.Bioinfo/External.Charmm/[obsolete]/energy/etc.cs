using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Charmm
{
public partial class Src
{
    static readonly Func<double,double> ABS = Math.Abs;
    static readonly Func<double,double> SQRT = Math.Sqrt;
    static readonly Func<double,double> ASIN = Math.Asin;
    static readonly Func<double,double> ACOS = Math.Acos;
    static readonly Func<double,double,double> POW = Math.Pow;
    static readonly Func<double,double,double> MAX = Math.Max;

    /// http://gcc.gnu.org/onlinedocs/gfortran/SIGN.html
    /// 
    /// 8.223 SIGN — Sign copying function
    /// 
    /// Description:    SIGN(A,B) returns the value of A with the sign of B. 
    /// Standard:       Fortran 77 and later 
    /// Class:          Elemental function 
    /// Syntax:         RESULT = SIGN(A, B) 
    /// Arguments:      A  Shall be of type INTEGER or REAL 
    ///                 B  Shall be of the same type and kind as A 
    /// Return value:   The kind of the return value is that of A and B. If B\ge 0 then the result is ABS(A), else it is -ABS(A). 
    static double SIGN(double A, double B)
    {
        if(B >= 0) return    Math.Abs(A);
        else       return -1*Math.Abs(A);
    }
}
}
}
