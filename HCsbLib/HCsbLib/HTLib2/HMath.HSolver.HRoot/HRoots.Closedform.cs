using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HTLib2
{
    public partial class HRoots
    {
        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
        //public static Complex[] Roots1c(double p1, double p0)
        //{
        //    return new Complex[1] { new Complex(-1 * p0 / p1) };
        //}
        public static double[] GetRootsClosedFormDegree1(double p1, double p0)
        {
            return new double[1] { -1 * p0 / p1 };
        }

        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
        //public static Complex[] Roots2c(double p2, double p1, double p0)
        //{
        //    Complex d = Complex.Sqrt(new Complex(p1 * p1 - 4 * p2 * p0));
        //    Complex[] roots = new Complex[2] {
        //        (-1*p1 + d)/(2*p2),
        //        (-1*p1 - d)/(2*p2)
        //    };
        //    return roots;
        //}
        public static double[] GetRootsClosedFormDegree2(double p2, double p1, double p0)
        {
            if(HDebug.Selftest())
            {
                double[] tsol;

                tsol = GetRootsClosedFormDegree2(1, 2, -3);
                HDebug.Assert(tsol[0] == 1, tsol[1] == -3);

                tsol = GetRootsClosedFormDegree2(4, 3, 10);
                HDebug.Assert(tsol == null);
            }
            double d = p1 * p1 - 4 * p2 * p0;
            if (d >= 0)
            {
                d = Math.Sqrt(d);
                double[] roots = new double[2] {
                    (-1*p1 + d)/(2*p2),
                    (-1*p1 - d)/(2*p2)
                };
                return roots;
            }
            return null;
        }

        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
        //public static Complex[] Roots3c(double p3, double p2, double p1, double p0)
        //{
        //    // p3 * x^3 + p2 * x^2 + p1 * x + p0 = 0
        //    double p = p2 / p3;
        //    double q = p1 / p3;
        //    double r = p0 / p3;
        //
        //    double a = (3 * q - p * p) / 3;
        //    double b = (2 * p * p * p - 9 * p * q + 27 * r) / 27;
        //    double A, B;
        //    {
        //        double temp = Math.Sqrt(b * b / 4 + a * a * a / 27);
        //        A = -1 * b / 2 + temp;
        //        A = (A >= 0) ? Math.Pow(A, (1.0 / 3)) : -1 * Math.Pow(-1 * A, (1.0 / 3));
        //        B = -1 * b / 2 - temp;
        //        B = (B >= 0) ? Math.Pow(B, (1.0 / 3)) : -1 * Math.Pow(-1 * B, (1.0 / 3));
        //    };
        //
        //    double check = b * b / 4 + a * a * a / 27;
        //
        //    Complex[] roots;
        //    if (check == 0)
        //    {
        //        double y1, y2, y3;
        //        if (b > 0)
        //        {
        //            double temp = Math.Sqrt(-1 * a / 3);
        //            y1 = -2 * temp;
        //            y2 = y3 = temp;
        //        }
        //        else if (b < 0)
        //        {
        //            double temp = Math.Sqrt(-1 * a / 3);
        //            y1 = 2 * temp;
        //            y2 = y3 = -1 * temp;
        //        }
        //        else
        //        {
        //            Debug.Assert(b == 0);
        //            y1 = y2 = y3 = 0;
        //        }
        //        double x1 = y1 - p / 3;
        //        double x2 = y2 - p / 3;
        //        double x3 = y3 - p / 3;
        //        roots = new Complex[3] { new Complex(x1), new Complex(x2), new Complex(x3) };
        //    }
        //    else if (check > 0)
        //    {
        //        double y1 = A + B;
        //        Complex x1 = new Complex(y1 - p / 3);
        //        double real = -0.5 * (A + B) - p/3;
        //        double imag = Math.Sqrt(3.0)*(A - B)/2;
        //        Complex x2 = new Complex(real, imag);
        //        Complex x3 = new Complex(real, -imag);
        //        /* debug */    if (System.Diagnostics.Debugger.IsAttached)
        //        /* debug */    {
        //        /* debug */        double error;
        //        /* debug */        error = (p3 * (x1 * x1 * x1) + p2 * (x1 * x1) + p1 * (x1) + p0).ABS;
        //        /* debug */        //Debug.Assert(Math.Abs(error) < 0.0001);
        //        /* debug */        error = (p3 * (x2 * x2 * x2) + p2 * (x2 * x2) + p1 * (x2) + p0).ABS;
        //        /* debug */        //Debug.Assert(Math.Abs(error) < 0.0001);
        //        /* debug */        error = (p3 * (x3 * x3 * x3) + p2 * (x3 * x3) + p1 * (x3) + p0).ABS;
        //        /* debug */        //Debug.Assert(Math.Abs(error) < 0.0001);
        //        /* debug */    }
        //        roots = new Complex[3] { x1, x2, x2 };
        //    }
        //    else
        //    {
        //        Debug.Assert(check < 0);
        //        double pi;
        //        {
        //            double temp = Math.Sqrt((b * b / 4) / (-1 * a * a * a / 27));
        //            if (b > 0)
        //            {
        //                pi = Math.Acos(-1 * temp);
        //            }
        //            else if (b < 0)
        //            {
        //                pi = Math.Acos(temp);
        //            }
        //            else
        //            {
        //                pi = Math.Acos(temp);
        //            }
        //        }
        //        double[] x = new double[3];
        //        for (int k = 0; k < 3; k++)
        //        {
        //            x[k] = 2 * Math.Sqrt(-1 * a / 3) * Math.Cos(pi / 3 + 2 * k * Math.PI / 3);
        //            x[k] = x[k] - p / 3;
        //        }
        //        roots = new Complex[3] { new Complex(x[0]), new Complex(x[1]), new Complex(x[2]) };
        //    }
        //    return roots;
        //}
        ////{
        ////    // p3 * x^3 + p2 * x^2 + p1 * x + p0 = 0
        ////    double p = p2 / p3;
        ////    double q = p1 / p3;
        ////    double r = p0 / p3;
        ////
        ////    double a = (3 * q - p * p) / 3;
        ////    double b = (2 * p * p * p - 9 * p * q + 27 * r) / 27;
        ////
        ////    Complex A, B;
        ////    {
        ////        Complex temp = Complex.Sqrt(new Complex(b * b / 4 + a * a * a / 27));
        ////        Complex AA = (new Complex(-1 * b / 2) + temp);
        ////        if (AA.imaginary == 0)
        ////        {
        ////            A = (AA.real >= 0) ?
        ////                (new Complex(Math.Pow(AA.real, (1.0 / 3)))) :
        ////                (new Complex(-1 * Math.Pow(-1 * AA.real, (1.0 / 3))));
        ////        }
        ////        else
        ////        {
        ////            A = Complex.Pow(AA, (1.0 / 3));
        ////        }
        ////        Debug.Assert((A * A * A - AA).ABS < 0.00001);
        ////        Complex BB = (new Complex(-1 * b / 2) - temp);
        ////        if (BB.IsApproximateDouble)
        ////        {
        ////            B = (BB.real >= 0) ?
        ////                (new Complex(Math.Pow(BB.real, (1.0 / 3)))) :
        ////                (new Complex(-1 * Math.Pow(-1 * BB.real, (1.0 / 3))));
        ////        }
        ////        else
        ////        {
        ////            B = Complex.Pow(BB, (1.0 / 3));
        ////        }
        ////        Debug.Assert((B * B * B - BB).ABS < 0.00001);
        ////        //A = Complex.Pow((new Complex(-1 * b / 2) + temp), (1.0 / 3));
        ////        //B = Complex.Pow((new Complex(-1 * b / 2) - temp), (1.0 / 3));
        ////    }
        ////
        ////    Complex y1, y2, y3;
        ////    {
        ////        Complex addAB = (A + B);
        ////        Complex subAB = (A - B);
        ////        Complex temp = new Complex(0, Math.Sqrt(3) / 2);
        ////
        ////        y1 = addAB;
        ////        y2 = -0.5 * addAB + temp * subAB;
        ////        y3 = -0.5 * addAB - temp * subAB;
        ////    }
        ////    Complex x1 = y1 - p / 3;
        ////    Complex x2 = y2 - p / 3;
        ////    Complex x3 = y3 - p / 3;
        ////
        ////    /* debug */    if (System.Diagnostics.Debugger.IsAttached)
        ////    /* debug */    {
        ////    /* debug */        Complex error;
        ////    /* debug */        error = p3 * (x1 * x1 * x1) + p2 * (x1 * x1) + p1 * (x1) + p0;
        ////    /* debug */        Debug.Assert(Complex.Abs(error) < 0.0001);
        ////    /* debug */        error = p3 * (x2 * x2 * x2) + p2 * (x2 * x2) + p1 * (x2) + p0;
        ////    /* debug */        Debug.Assert(Complex.Abs(error) < 0.0001);
        ////    /* debug */        error = p3 * (x3 * x3 * x3) + p2 * (x3 * x3) + p1 * (x3) + p0;
        ////    /* debug */        Debug.Assert(Complex.Abs(error) < 0.0001);
        ////    /* debug */    }
        ////
        ////    Complex[] roots = new Complex[3] { x1, x2, x3 };
        ////    return roots;
        ////}
        public static double[] GetRootsClosedFormDegree3(double p3, double p2, double p1, double p0)
        {
            // p3 * x^3 + p2 * x^2 + p1 * x + p0 = 0
            double p = p2 / p3;
            double q = p1 / p3;
            double r = p0 / p3;

            double a = (3 * q - p * p) / 3;
            double b = (2 * p * p * p - 9 * p * q + 27 * r) / 27;
            double A, B;
            {
                double temp = Math.Sqrt(b * b / 4 + a * a * a / 27);
                A = -1 * b / 2 + temp;
                A = (A >= 0) ? Math.Pow(A, (1.0 / 3)) : -1 * Math.Pow(-1 * A, (1.0 / 3));
                B = -1 * b / 2 - temp;
                B = (B >= 0) ? Math.Pow(B, (1.0 / 3)) : -1 * Math.Pow(-1 * B, (1.0 / 3));
            };

            double check = b * b / 4 + a * a * a / 27;

            double[] roots;
            if (check == 0)
            {
                double y1, y2, y3;
                if (b > 0)
                {
                    double temp = Math.Sqrt(-1 * a / 3);
                    y1 = -2 * temp;
                    y2 = y3 = temp;
                }
                else if (b < 0)
                {
                    double temp = Math.Sqrt(-1 * a / 3);
                    y1 = 2 * temp;
                    y2 = y3 = -1 * temp;
                }
                else
                {
                    Debug.Assert(b == 0);
                    y1 = y2 = y3 = 0;
                }
                double x1 = y1 - p / 3;
                double x2 = y2 - p / 3;
                double x3 = y3 - p / 3;
                roots = new double[3] { x1, x2, x3 };
            }
            else if (check > 0)
            {
                double y1 = A + B;
                double x1 = y1 - p / 3;
                /* debug */    if (System.Diagnostics.Debugger.IsAttached)
                /* debug */    {
                /* debug */        double error = p3 * (x1 * x1 * x1) + p2 * (x1 * x1) + p1 * (x1) + p0;
                /* debug */        //Debug.Assert(Math.Abs(error) < 0.0001);
                /* debug */    }
                roots = new double[1] { x1 };
            }
            else
            {
                Debug.Assert(check < 0);
                double pi;
                {
                    double temp = Math.Sqrt((b * b / 4) / (-1 * a * a * a / 27));
                    if (b > 0)
                    {
                        pi = Math.Acos(-1 * temp);
                    }
                    else if (b < 0)
                    {
                        pi = Math.Acos(temp);
                    }
                    else
                    {
                        pi = Math.Acos(temp);
                    }
                }
                double[] x = new double[3];
                for (int k = 0; k < 3; k++)
                {
                    x[k] = 2 * Math.Sqrt(-1 * a / 3) * Math.Cos(pi / 3 + 2 * k * Math.PI / 3);
                    x[k] = x[k] - p / 3;
                }
                roots = x;
            }
            return roots;
        }

        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
        //public static Complex[] Roots4c(double p4, double p3, double p2, double p1, double p0)
        //{
        //    // p4 * x^4 + p3 * x^3 + p2 * x^2 + p1 * x + p0 = 0
        //    double p = (p3 / p4);
        //    double q = (p2 / p4);
        //    double r = (p1 / p4);
        //    double s = (p0 / p4);
        //
        //    double a = q - (3 * p * p / 8);
        //    double b = r + (p * p * p / 8) - (p * q / 2);
        //    double c = s - (3 * p * p * p * p / 256) + (p * p * q / 16) - (p * r / 4);
        //
        //    double z1;
        //    {
        //        double[] z = Roots3(1, -q, (p * r - 4 * s), (4 * q * s - r * r - p * p * s));
        //        if (z.Length < 1)
        //        {
        //            Debug.Assert(false);
        //            return new Complex[0];
        //        }
        //        z1 = z[0];
        //    }
        //
        //    Complex R, D, E;
        //    {
        //        R = Complex.Sqrt(p * p / 4 - q + z1);
        //        if (R != 0)
        //        {
        //            D = Complex.Sqrt((3.0 / 4) * (p * p) - (R * R) - (2 * q) + (1.0 / 4) * ((4 * p * q) - (8 * r) - (p * p * p)) / R);
        //            E = Complex.Sqrt((3.0 / 4) * (p * p) - (R * R) - (2 * q) - (1.0 / 4) * ((4 * p * q) - (8 * r) - (p * p * p)) / R);
        //        }
        //        else
        //        {
        //            D = Complex.Sqrt((3.0 / 4) * (p * p) - (2 * q) + 2 * Complex.Sqrt(z1 * z1 - 4 * s));
        //            E = Complex.Sqrt((3.0 / 4) * (p * p) - (2 * q) - 2 * Complex.Sqrt(z1 * z1 - 4 * s));
        //        }
        //    }
        //
        //    Complex x1 = (-1 * p / 4) + (1.0 / 2) * (R + D);
        //    Complex x2 = (-1 * p / 4) + (1.0 / 2) * (R - D);
        //    Complex x3 = (-1 * p / 4) - (1.0 / 2) * (R - E);
        //    Complex x4 = (-1 * p / 4) - (1.0 / 2) * (R + E);
        //
        //    return new Complex[4] { x1, x2, x3, x4 };
        //}
        public static double[] GetRootsClosedFormDegree4(double p4, double p3, double p2, double p1, double p0)
        {
            // p4 * x^4 + p3 * x^3 + p2 * x^2 + p1 * x + p0 = 0
            double p = p3 / p4;
            double q = p2 / p4;
            double r = p1 / p4;
            double s = p0 / p4;

            double a = q - (3 * p * p / 8);
            double b = r + (p * p * p / 8) - (p * q / 2);
            double c = s - (3 * p * p * p * p / 256) + (p * p * q / 16) - (p * r / 4);

            double z1;
            {
                double[] z = GetRootsClosedFormDegree3(1, -q, (p * r - 4 * s), (4 * q * s - r * r - p * p * s));
                if (z.Length < 1)
                {
                    Debug.Assert(false);
                    return new double[0];
                }
                z1 = z[0];
            }

            bool Db, Eb;
            double R, D, E;
            {
                R = (p * p / 4 - q + z1);
                Debug.Assert(R >= 0);
                R = Math.Sqrt(R);
                if (R != 0)
                {
                    D = ((3.0 / 4) * (p * p) - (R * R) - (2 * q) + (1.0 / 4) * ((4 * p * q) - (8 * r) - (p * p * p)) / R);
                    Db = (D >= 0) ? true : false;
                    D = Math.Sqrt(D);

                    E = ((3.0 / 4) * (p * p) - (R * R) - (2 * q) - (1.0 / 4) * ((4 * p * q) - (8 * r) - (p * p * p)) / R);
                    Eb = (E >= 0) ? true : false;
                    E = Math.Sqrt(E);
                }
                else
                {
                    D = ((3.0 / 4) * (p * p) - (2 * q) + 2 * Math.Sqrt(z1 * z1 - 4 * s));
                    Db = (D >= 0) ? true : false;
                    D = Math.Sqrt(D);

                    E = ((3.0 / 4) * (p * p) - (2 * q) - 2 * Math.Sqrt(z1 * z1 - 4 * s));
                    Eb = (E >= 0) ? true : false;
                    E = Math.Sqrt(E);
                }
            }

            double x1 = (-1 * p / 4) + (1.0 / 2) * (R + D);
            double x2 = (-1 * p / 4) + (1.0 / 2) * (R - D);
            double x3 = (-1 * p / 4) - (1.0 / 2) * (R - E);
            double x4 = (-1 * p / 4) - (1.0 / 2) * (R + E);

            if (Db == true && Eb == true)
            {
                return new double[4] { x1, x2, x3, x4 };
            }
            else if (Db == true && Eb == false)
            {
                return new double[2] { x1, x2 };
            }
            else if (Db == false && Eb == true)
            {
                return new double[2] { x3, x4 };
            }
            else
            {
                return new double[0];
            }
        }
    }
}