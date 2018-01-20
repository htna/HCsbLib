/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

// http://www.math.pitt.edu/~troy/math2070/lab_04.html
//
// Muller's Method 
//
// Muller's method is best understood as another attempt to "model" the unknown function.
// At each step of the method, three points are used. Muller's method determines the quadratic
// polynomial that passes through these three points, and then solves for the roots of that
// polynomial, and chooses one of them to add as its latest point, while discarding the oldest
// point. 
//
// Muller's method can be fast, but there are several things that can go wrong. If the data
// points are very close, or lie on a straight line, there are accuracy problems in computing   (( accuracy problem ))
// the coefficients of the quadratic, and more problems in determining its roots. Also, the
// coding for Muller's method is significantly more complicated than for other methods we've
// seen. 
//
// A strange feature of Muller's method is that the quadratic polynomial might have no real roots.
// However, in that case, it may be perfectly acceptable to chase one of the complex roots, and
// if you allow the method to do this, it works just the same. If you look at the algorithm I am
// giving you, you only have to delete one line to make it work on a problem with complex roots. 



namespace HTLib2
{
    public class HRootsMuller
    {
        struct xxx
        {
        };
        readonly PolynomialComplex poly;
        readonly Complex[] init;
        PolynomialComplex polys;
        List<Complex> roots = new List<Complex>();

        public RootsMuller(PolynomialComplex poly, Complex[] init)
        {
            this.poly = poly;
            this.init = init;
            this.polys = poly;
        }

        protected Complex funcDeplated(Complex x)
        {
            {
                Complex q = polys.GetValue(x);
                return q;
            }
            //{
            //    Complex q = poly.GetValue(x);
            //    for (int i = 0; i < roots.Count; i++)
            //    {
            //        q = q / (x - roots[i]);
            //    }
            //    return q;
            //}
        }
        protected Pair<bool, Complex> Polish(Complex init)
        {
            return RootsNewton.Newton(polys, init);
        }
        protected void AddRoot(Complex root)
        {
            //Debug.Assert(polys.GetValue(root).Length < 0.001);
            polys = polys / new PolynomialComplex(new Complex(1, 0), root);
            roots.Add(root);
        }
        public Complex[] GetRoots()
        {
            this.polys = poly;

            Pair<bool, Complex> root = GetRoot();
            while (polys.Degree > 0)
            {
                if (polys.Degree <= 4 && polys.IsApproximateDouble)
                {
                    double[] droot = Roots.Rootsx((PolynomialDouble)polys);
                    for (int i = 0; i < droot.Length; i++)
                        AddRoot(new Complex(droot[i]));
                    return roots.ToArray();
                }
                else
                {
                    root = GetRoot();
                }

                Debug.Assert(root.first == true);
                if (root.second.IsApproximateDouble)
                {
                    double error = Complex.Abs(polys.GetValue(root.second));
                    Debug.Assert(Complex.Abs(polys.GetValue(root.second)) < 0.001);
                    AddRoot(root.second);
                }
                else
                {
                    double error1 = Complex.Abs(polys.GetValue(root.second));
                    Debug.Assert(Complex.Abs(polys.GetValue(root.second)) < 0.001);
                    AddRoot(root.second);

                    double error2 = Complex.Abs(polys.GetValue(Complex.Conj(root.second)));
                    Debug.Assert(Complex.Abs(polys.GetValue(Complex.Conj(root.second))) < 0.001);
                    AddRoot(Complex.Conj(root.second));
                }

                root = GetRoot();
            }

            return roots.ToArray();
        }
        public Pair<bool, Complex> GetRoot()
        {
            Complex[] x;
            {
                x = new Complex[init.Length];
                for (int i = 0; i < init.Length; i++)
                {
                    x[i] = init[i];
                }
            }
            Debug.Assert(x.Length == 3);
            Complex[] f;
            {
                f = new Complex[3] { funcDeplated(x[0]), funcDeplated(x[1]), funcDeplated(x[2]) };
                if ((Complex.Abs(f[0] - f[1]) < 0.0001)
                    && (Complex.Abs(f[1] - f[2]) < 0.0001)
                    && (Complex.Abs(f[2] - f[0]) < 0.0001))
                {
                    return new Pair<bool, Complex>(false, new Complex(0, 0));
                }
            }

            double dx1 = Complex.Abs(f[1] - f[0]);
            double dx2 = Complex.Abs(f[2] - f[1]);
            while (Math.Abs(dx2 - dx1) > 0.0005)
            {
                Complex[][] coeff;
                {
                    int n = 3;
                    Complex[][] Fab = new Complex[n][];
                    for (int aa = 0; aa < n; aa++)
                    {
                        Fab[aa] = new Complex[n - aa];
                        Fab[aa][0] = f[aa];
                    }
                    for (int bb = 1; bb < n; bb++)
                    {
                        for (int aa = 0; aa < n - bb; aa++)
                        {
                            Fab[aa][bb] = (Fab[aa][bb - 1] - Fab[aa + 1][bb - 1]) / (x[aa] - x[aa + bb]);
                        }
                    }
                    coeff = Fab;
                }

                Complex a = coeff[0][2];
                // a = f[x0,x1,x2]
                Complex b = coeff[1][1] + coeff[0][2] * (x[2] - x[1]);
                // b = f[x1,x2] + f[x0,x1,x2](x2 - x1)

                Complex nx;
                {
                    Complex b2_4af = Complex.Sqrt(b * b - 4 * a * f[2]);
                    Complex b_b2_4af_plus = b + b2_4af;
                    Complex b_b2_4af_minus = b - b2_4af;
                    if (Complex.Abs(b_b2_4af_plus) > Complex.Abs(b_b2_4af_minus))
                    {
                        nx = x[2] - 2 * f[2] / b_b2_4af_plus;
                    }
                    else
                    {
                        nx = x[2] - 2 * f[2] / b_b2_4af_minus;
                    }
                }
                x[0] = x[1]; f[0] = f[1];
                x[1] = x[2]; f[1] = f[2];
                x[2] = nx; f[2] = funcDeplated(nx);
                if (x[2] == x[0]) x[2] = x[2] + 0.1;
                if (x[2] == x[1]) x[2] = x[2] + 0.1;

                dx1 = dx2;
                dx2 = Complex.Abs(x[2] - x[1]);
            }

            return Polish(poly, x[2]);
        }
        public static Pair<bool, Complex> GetRoot(PolynomialDouble poly)
        {
            PolynomialComplex cpoly = (PolynomialComplex)poly;
            Complex init1 = new Complex(1);
            Complex init2 = new Complex(2);
            Complex init3 = new Complex(3);
            Pair<bool, Complex> root = new Pair<bool, Complex>(false, Complex.Zero);
            int iter = 100;
            while (root.first == false && iter > 0)
            {
                root = GetRoot(cpoly, init1, init2, init3);
                iter--;
                init3.real += 1;
            }
            return root;
        }
        public static Pair<bool, Complex> GetRoot(PolynomialComplex poly, Complex init1, Complex init2, Complex init3)
        {
            const double tolerance = 0.000001;
            Complex[] x = new Complex[3] {
                init1,
                init2,
                init3
            };
            Complex[] f;
            {
                f = new Complex[3] { poly.GetValue(x[0]), poly.GetValue(x[1]), poly.GetValue(x[2]) };
                if ((Complex.Abs(f[0] - f[1]) < 0.0001)
                    && (Complex.Abs(f[1] - f[2]) < 0.0001)
                    && (Complex.Abs(f[2] - f[0]) < 0.0001))
                {
                    return new Pair<bool, Complex>(false, new Complex(0, 0));
                }
            }

            double dx1 = Complex.Abs(f[1] - f[0]);
            double dx2 = Complex.Abs(f[2] - f[1]);
            while (Complex.Abs(poly.GetValue(x[2])) > tolerance) //Math.Abs(dx2 - dx1) > 0.00000005)
            {
                Complex[][] coeff;
                {
                    int n = 3;
                    Complex[][] Fab = new Complex[n][];
                    for (int aa = 0; aa < n; aa++)
                    {
                        Fab[aa] = new Complex[n - aa];
                        Fab[aa][0] = f[aa];
                    }
                    for (int bb = 1; bb < n; bb++)
                    {
                        for (int aa = 0; aa < n - bb; aa++)
                        {
                            Fab[aa][bb] = (Fab[aa][bb - 1] - Fab[aa + 1][bb - 1]) / (x[aa] - x[aa + bb]);
                        }
                    }
                    coeff = Fab;
                }

                Complex a = coeff[0][2];
                // a = f[x0,x1,x2]
                Complex b = coeff[1][1] + coeff[0][2] * (x[2] - x[1]);
                // b = f[x1,x2] + f[x0,x1,x2](x2 - x1)

                Complex nx;
                {
                    Complex b2_4af = Complex.Sqrt(b * b - 4 * a * f[2]);
                    Complex b_b2_4af_plus = b + b2_4af;
                    Complex b_b2_4af_minus = b - b2_4af;
                    if (Complex.Abs(b_b2_4af_plus) > Complex.Abs(b_b2_4af_minus))
                    {
                        nx = x[2] - 2 * f[2] / b_b2_4af_plus;
                    }
                    else
                    {
                        nx = x[2] - 2 * f[2] / b_b2_4af_minus;
                    }
                }
                x[0] = x[1]; f[0] = f[1];
                x[1] = x[2]; f[1] = f[2];
                x[2] = nx; f[2] = poly.GetValue(nx);
                if (x[2] == x[0]) x[2] = x[2] + 0.1;
                if (x[2] == x[1]) x[2] = x[2] + 0.1;

                dx1 = dx2;
                dx2 = Complex.Abs(x[2] - x[1]);
                Complex err = poly.GetValue(x[2]);
            }

            return new Pair<bool, Complex>((Complex.Abs(poly.GetValue(x[2])) < tolerance), x[2]);
        }
        static public Pair<bool, Complex> Polish(PolynomialComplex poly, Complex init)
        {
            return RootsNewton.Newton(poly, init);
        }
    }
}
*/
