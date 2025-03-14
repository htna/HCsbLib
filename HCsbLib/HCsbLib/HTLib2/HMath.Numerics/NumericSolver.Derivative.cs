using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public abstract partial class NumericSolver
    {
        //  public static double Derivative(Func<double, double> func, double x, double dx)
        //  {
        //      return Numerics.Derivative(func, x, dx);
        //  }
        //  public static double Derivative(Func<double, double> func, double x, double dx0, double dx1)
        //  {
        //      return Numerics.Derivative(func, x, dx0, dx1);
        //  }
        //  public static Vector Derivative(Func<Vector, double> func, Vector x, double dx)
        //  {
        //      return Numerics.Derivative(func, x, dx);
        //  }
        //  public static MatrixByArr Derivative2(Func<Vector, double> func, Vector x, double dx)
        //  {
        //      return Numerics.Derivative2(func, x, dx);
        //  }
        //  public static void Derivative2(Func<Vector, double> func, Vector x, double dx, ref MatrixByArr dy2)
        //  {
        //      Numerics.Derivative2(func, x, dx, ref dy2);
        //  }
        //  public static void Derivative2<INFO>(Func<Vector[], INFO, double> func, Vector[] x, double dx, ref MatrixByArr[,] dy2, INFO info)
        //  {
        //      Numerics.Derivative2<INFO>(func, x, dx, ref dy2, info);
        //  }
        public static void Derivative2(Func<Vector[], double[], double> func, Vector[] x, double dx, ref MatrixByArr[,] dy2, params double[] info)
        {
            Numerics.Derivative2(func, x, dx, ref dy2, info);
        }
    }
}
