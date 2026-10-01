using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public abstract partial class Numerics
    {
        /// var inter = Interpolator(new double[] {1,2,3,4} , new double[] {1,3,5,7});
        /// double val = inter(2.5);
        public static Func<double, double> Interpolator(double[] xs, double[] ys)
        {
            var pchip = new LinearInterpolator(xs, ys);
            return pchip.Interpolate;
        }
        public class LinearInterpolator
        {
            private readonly double[] xs;
            private readonly double[] ys;
            public LinearInterpolator(double[] xs, double[] ys) { this.xs = xs; this.ys = ys; }
            public double Interpolate(double x) { return Numerics.Interpolate(xs, ys, x); }
        }
        public static double Interpolate(double[] xs, double[] ys, double x)
        {
            if (xs.Length != ys.Length || xs.Length < 2)
                throw new ArgumentException("Provide matching arrays with at least two points.");

            if (double.IsNaN(x) || x < xs[0] || x > xs.Last())
                throw new ArgumentOutOfRangeException(nameof(x));

            int index = Array.BinarySearch(xs, x);

            // Return the known value for an exact match.
            if (index >= 0)
                return ys[index];

            // For a missing value, ~index gives its insertion position.
            int right = ~index;
            int left = right - 1;

            double t = (x - xs[left]) / (xs[right] - xs[left]);

            return (1 - t) * ys[left] + t * ys[right];
        }

        /// https://docs.scipy.org/doc/scipy/reference/generated/scipy.interpolate.PchipInterpolator.html
        public static double Interpolate_Pchip(double[] xs, double[] ys, double x)
        {
            var pchip = new PchipInterpolator(xs, ys);
            return pchip.Interpolate(x);
        }
        public static Func<double,double> Interpolator_Pchip(double[] xs, double[] ys)
        {
            var pchip = new PchipInterpolator(xs, ys);
            return pchip.Interpolate;
        }
        public sealed class PchipInterpolator
        {
            private readonly double[] x;
            private readonly double[] y;
            private readonly double[] slopes;

            public PchipInterpolator(double[] xs, double[] ys)
            {
                if (xs is null || ys is null)
                    throw new ArgumentNullException(xs is null ? nameof(xs) : nameof(ys));

                if (xs.Length != ys.Length || xs.Length < 2)
                    throw new ArgumentException("Provide matching arrays with at least two points.");

                for (int i = 0; i < xs.Length; i++)
                {
                    if (double.IsInfinity(xs[i]) || double.IsNaN(xs[i])
                     || double.IsInfinity(ys[i]) || double.IsNaN(ys[i]))
                        throw new ArgumentException("Values must be finite.");

                    if (i > 0 && xs[i] <= xs[i - 1])
                        throw new ArgumentException("X values must be strictly increasing.");
                }

                // Copy inputs so later changes cannot affect the interpolator.
                x = (double[])xs.Clone();
                y = (double[])ys.Clone();

                int n = x.Length;
                slopes = new double[n];

                double[] h = new double[n - 1];
                double[] delta = new double[n - 1];

                for (int i = 0; i < n - 1; i++)
                {
                    h[i] = x[i + 1] - x[i];
                    delta[i] = (y[i + 1] - y[i]) / h[i];
                }

                // With only two points, use linear interpolation.
                if (n == 2)
                {
                    slopes[0] = delta[0];
                    slopes[1] = delta[0];
                    return;
                }

                // Interior slopes.
                for (int i = 1; i < n - 1; i++)
                {
                    double previous = delta[i - 1];
                    double next = delta[i];

                    // Flatten the curve at a plateau or a change in direction.
                    if (previous == 0 || next == 0 || Math.Sign(previous) != Math.Sign(next))
                    {
                        slopes[i] = 0;
                    }
                    else
                    {
                        // Weighted harmonic mean of neighboring slopes.
                        double w1 = 2 * h[i] + h[i - 1];
                        double w2 = h[i] + 2 * h[i - 1];

                        slopes[i] = (w1 + w2)
                            / (w1 / previous + w2 / next);
                    }
                }

                // Endpoint slopes use a one-sided estimate with limiting.
                slopes[0    ] = EndpointSlope(h[0  ], h[1  ], delta[0  ], delta[1  ]);
                slopes[n - 1] = EndpointSlope(h[n-2], h[n-3], delta[n-2], delta[n-3]);
            }

            public double Interpolate(double value)
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value < x[0] || value > x.Last())
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be within the sample range.");
                }

                int index = Array.BinarySearch(x, value);

                // Exact sample point.
                if (index >= 0)
                    return y[index];

                // BinarySearch encodes the insertion position as its complement.
                int right = ~index;
                int left = right - 1;

                double h = x[right] - x[left];
                double t = (value - x[left]) / h;
                double t2 = t * t;
                double t3 = t2 * t;

                // Cubic Hermite basis functions.
                double h00 = 2 * t3 - 3 * t2 + 1;
                double h10 = t3 - 2 * t2 + t;
                double h01 = -2 * t3 + 3 * t2;
                double h11 = t3 - t2;

                return h00 * y[left]
                        + h10 * h * slopes[left]
                        + h01 * y[right]
                        + h11 * h * slopes[right];
            }

            private static double EndpointSlope(double h0, double h1, double d0, double d1)
            {
                double slope = ((2 * h0 + h1) * d0 - h0 * d1) / (h0 + h1);

                // Prevent the endpoint from going in the wrong direction.
                if (Math.Sign(slope) != Math.Sign(d0))
                    return 0;

                // Limit the slope near a change in direction.
                if (Math.Sign(d0) != Math.Sign(d1) && Math.Abs(slope) > 3 * Math.Abs(d0))
                    return 3 * d0;

                return slope;
            }
        }
    }
}
