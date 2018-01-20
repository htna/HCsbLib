using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Tinker
{
public partial class Src
{
///
///
///     ###################################################
///     ##  COPYRIGHT (C)  1992  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ##########################################################
///     ##                                                      ##
///     ##  math.i  --  mathematical and geometrical constants  ##
///     ##                                                      ##
///     ##########################################################
///
///
///     radian   conversion factor from radians to degrees
///     pi       numerical value of the geometric constant
///     sqrtpi   numerical value of the square root of Pi
///     logten   numerical value of the natural log of ten
///     sqrttwo  numerical value of the square root of two
///     twosix   numerical value of the sixth root of two
///
///
//      real*8 radian,pi
//      real*8 sqrtpi,logten
//      real*8 sqrttwo,twosix
//      parameter (radian=57.29577951308232088d0)
//      parameter (pi=3.141592653589793238d0)
//      parameter (sqrtpi=1.772453850905516027d0)
//      parameter (logten=2.302585092994045684d0)
//      parameter (sqrttwo=1.414213562373095049d0)
//      parameter (twosix=1.122462048309372981d0)
    static readonly double radian=57.29577951308232088;
    //static readonly double pi=3.141592653589793238;
    //static readonly double sqrtpi=1.772453850905516027;
    //static readonly double logten=2.302585092994045684;
    //static readonly double sqrttwo=1.414213562373095049;
    //static readonly double twosix=1.122462048309372981;

    static readonly Func<double,double> sqrt = Math.Sqrt;
    static readonly Func<double,double> asin = Math.Asin;
    static readonly Func<double,double> acos = Math.Acos;
    static readonly Func<double,double> sin = Math.Sin;
    static readonly Func<double,double> cos = Math.Cos;
    static readonly Func<double,double> abs = Math.Abs;
    static readonly Func<double,double> exp = Math.Exp;

//  static readonly Func<double,double,double> max = Math.Max;
//  static readonly Func<double,double,double> min = Math.Min;
    static readonly Func<double,double,double> pow = Math.Pow;

    double max(params double[] vals) { double max=vals[0]; foreach(var val in vals) max=Math.Max(max,val); return max; }
    double min(params double[] vals) { double min=vals[0]; foreach(var val in vals) min=Math.Max(min,val); return min; }
    int max(params int[] vals) { int max=vals[0]; foreach(var val in vals) max=Math.Max(max,val); return max; }
    int min(params int[] vals) { int min=vals[0]; foreach(var val in vals) min=Math.Max(min,val); return min; }


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
    static double sign(double A, double B)
    {
        if(B >= 0) return Math.Abs(A);
        else return -1*Math.Abs(A);
    }

    static int[] dolist(int from, int to)
    {
        int[] list = new int[to-from+1];
        for(int i=from; i<=to; i++)
            list[i-from] = i;
        return list;
    }
}
}
}
