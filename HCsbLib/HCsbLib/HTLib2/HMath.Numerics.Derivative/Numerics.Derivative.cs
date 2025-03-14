using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public abstract partial class Numerics
    {
        public static double Derivative(Func<double, double> func, double x, double dx)
        {
            double x0 = x     ; double y0 = func(x0);
            double x1 = x + dx; double y1 = func(x1);
            double dy = (y1 - y0) / (x1 - x0);
            return dy;
        }
        public static double Derivative(Func<double, double> func, double x, double dx0, double dx1)
        {
            double x0 = x + dx0; double y0 = func(x0);
            double x1 = x + dx1; double y1 = func(x1);
            double dy = (y1 - y0) / (x1 - x0);
            return dy;
        }
        public static Vector Derivative(Func<Vector, double> func, Vector x, double dx)
        {
            HDebug.ToDo("implement");
            return null;
        }
        public static MatrixByArr Derivative2(Func<Vector, double> func, Vector x, double dx)
        {
            int size = x.Size;
            MatrixByArr dy2 = new double[size, size];
            Derivative2(func, x, dx, ref dy2);
            return dy2;
        }
        public static void Derivative2(Func<Vector, double> func, Vector x, double dx, ref MatrixByArr dy2)
        {
            // [df/dxdx,  df/dxdy]
            // [df/dydx,  df/dydy]
            //
            // df/dx = ( f(x+dx,y)-f(x-dx,y) )
            //         -----------------------
            //            (x+dx)  - (x-dx)
            //       = ( f(x+dx,y)-f(x-dx,y) ) / 2dx
            //       = dfx(y)
            //
            // df/dxdy = ( dfx(y+dy) - dfx(y-dy) )
            //           -------------------------
            //                (y+dy) -    (y-dy)
            //         = [ (f(x+dx,y+dy)-f(x-dx,y+dy))/2dx - (f(x+dx,y-dy)-f(x-dx,y-dy))/2dx ]
            //           ---------------------------------------------------------------------
            //               2dy
            //         = [ f(x+dx,y+dy) - f(x-dx,y+dy) - f(x+dx,y-dy) + f(x-dx,y-dy) ] / (2dx * 2dy)
            //         = [ f(x0       ) - f(x1       ) - f(x2       ) + f(x3       ) ] / (2dx * 2dy)
            //
            // df/dxdx = [ f((x+dx)+dx) - f((x+dx)-dx) - f((x-dx)+dx) + f((x-dx)-dx) ] / (2dx * 2dx)
            //         = [ f(x+2dx) - f(x) - f(x) + f(x-2dx) ] / (2dx * 2dx)
            //

            int size = x.Size;
            HDebug.Assert(dy2.RowSize == size, dy2.ColSize == size);
            double dx2 = dx*dx;

            for(int i=0; i<size; i++)
            {
                for(int j=0; j<size; j++)
                {
                    Vector x0 = x.Clone(); x0[i] += dx; x0[j] += dx; double y0 = func(x0);
                    Vector x1 = x.Clone(); x1[i] -= dx; x1[j] += dx; double y1 = func(x1);
                    Vector x2 = x.Clone(); x2[i] += dx; x2[j] -= dx; double y2 = func(x2);
                    Vector x3 = x.Clone(); x3[i] -= dx; x3[j] -= dx; double y3 = func(x3);
                    double d2f_didj = (y0 - y1 - y2 + y3) / (4*dx2);
                    dy2[i, j] = d2f_didj;
                }
            }
        }
        public static void Derivative2<INFO>(Func<Vector[], INFO, double> func, Vector[] x, double dx, ref MatrixByArr[,] dy2, INFO info)
        {
            // assume that (x[i].Size == 3)
            int size = x.Length;
            HDebug.Assert(dy2.GetLength(0) == size, dy2.GetLength(1) == size);
            double dx2 = dx*dx;
            Vectors xx = x;

            for(int i=0; i<size; i++)
                for(int di=0; di<3; di++)
                {
                    for(int j=0; j<size; j++)
                        for(int dj=0; dj<3; dj++)
                        {
                            Vector[] x0 = xx.Clone(); x0[i][di] += dx; x0[j][dj] += dx; double y0 = func(x0, info);
                            Vector[] x1 = xx.Clone(); x1[i][di] -= dx; x1[j][dj] += dx; double y1 = func(x1, info);
                            Vector[] x2 = xx.Clone(); x2[i][di] += dx; x2[j][dj] -= dx; double y2 = func(x2, info);
                            Vector[] x3 = xx.Clone(); x3[i][di] -= dx; x3[j][dj] -= dx; double y3 = func(x3, info);
                            double d2f_didj = (y0 - y1 - y2 + y3) / (4*dx2);
                            dy2[i, j][di, dj] = d2f_didj;
                        }
                }
        }
        public static void Derivative2(Func<Vector[], double[], double> func, Vector[] x, double dx, ref MatrixByArr[,] dy2, params double[] info)
        {
            // assume that (x[i].Size == 3)
            int size = x.Length;
            HDebug.Assert(dy2.GetLength(0) == size, dy2.GetLength(1) == size);
            double dx2 = dx*dx;
            Vectors xx = x;

            for(int i=0; i<size; i++)
            for(int di=0; di<3; di++)
            {
                for(int j=0; j<size; j++)
                for(int dj=0; dj<3; dj++)
                {
                    Vector[] x0 = xx.Clone(); x0[i][di] += dx; x0[j][dj] += dx; double y0 = func(x0, info);
                    Vector[] x1 = xx.Clone(); x1[i][di] -= dx; x1[j][dj] += dx; double y1 = func(x1, info);
                    Vector[] x2 = xx.Clone(); x2[i][di] += dx; x2[j][dj] -= dx; double y2 = func(x2, info);
                    Vector[] x3 = xx.Clone(); x3[i][di] -= dx; x3[j][dj] -= dx; double y3 = func(x3, info);
                    double d2f_didj = (y0 - y1 - y2 + y3) / (4*dx2);
                    dy2[i,j][di,dj] = d2f_didj;
                }
            }
        }
        public static double[,] Derivative2(Func<double, double, double> func, double x, double y, double h)
        {
            double ddFunc_dxdx = (func(x+h,y) - 2*func(x,y) + func(x-h,y))/(h*h);
            double ddFunc_dydy = (func(x,y+h) - 2*func(x,y) + func(x,h-h))/(h*h);
            double ddFunc_dxdy = (func(x+h,y+h) - func(x+h,y-h) - func(x-h,y+h) + func(x-h,y-h))/(4*h*h);
            double ddFunc_dydx = ddFunc_dxdy;
            return new double[2,2]
            {
                { ddFunc_dxdx, ddFunc_dxdy },
                { ddFunc_dydx, ddFunc_dydy },
            };
        }
    }
}
