using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public abstract partial class Numerics
    {
        public static double Integrate
            ( Func<double, double> func
            , double a
            , double b
            , double dx
            )
        {
            double sum = 0;
            double x0 = a;
            double f0 = func(x0);

            while((x0+dx) < b)
            {
                double x1 = x0 + dx;
                double f1 = func(x1);
                sum += dx * (f1 - f0);
            }

            {
                double x1 = b;
                double f1 = func(x1);
                sum += dx * (f1 - f0);
            }

            return sum;
        }

        /// composite Simpson’s rule
        public static double Integrate_CompositeSimpson
            ( Func<double, double> func
            , double a
            , double b
            , int intervals=1000
            )
        {
            if(func == null)
                throw new ArgumentNullException(nameof(func));
            if(double.IsNaN(a) || double.IsInfinity(a) || double.IsNaN(b) || double.IsInfinity(b))
                throw new ArgumentException("Integration limits must be finite.");
            if(intervals < 2 || intervals % 2 != 0)
                throw new ArgumentOutOfRangeException(nameof(intervals), "The number of intervals must be positive and even.");

            if(a == b)
                return 0.0;

            double h = (b - a) / intervals;
            double sum = 0.0;

            for(int i = 0; i <= intervals; i++)
            {
                double x = (i == intervals) ? b : a + i * h;
                double y = func(x);

                if(double.IsNaN(y) || double.IsInfinity(y))
                    throw new ArithmeticException($"The function returned a non-finite value at x = {x}.");

                int weight = (i == 0 || i == intervals)
                            ? 1
                            : (i % 2 == 0 ? 2 : 4);

                sum += weight * y;
            }

            return sum * h / 3.0;
        }

        /// Adaptive Simpson’s rule: automatically subdivides intervals where the estimated error is too large
        public static double Integrate_AdaptiveSimpson
            ( Func<double, double> func
            , double a
            , double b
            , double tolerance = 1e-8
            , int maxDepth = 20
            )
        {
            static bool IsFinite(double x) { return (!double.IsNaN(x) && !double.IsInfinity(x)); }

            if (func == null)                                       throw new ArgumentNullException(nameof(func));
            if (!IsFinite(a) || !IsFinite(b) || !IsFinite(b - a))   throw new ArgumentException("The integration limits and their difference must be finite.");
            if (!IsFinite(tolerance) || tolerance <= 0)             throw new ArgumentOutOfRangeException(nameof(tolerance));
            if (maxDepth < 1)                                       throw new ArgumentOutOfRangeException(nameof(maxDepth));

            if (a == b)
                return 0.0;

            // Support reversed integration limits.
            double sign = 1.0;
            if (a > b)
            {
                double temp = a;
                a = b;
                b = temp;
                sign = -1.0;
            }

            double Evaluate(double x)
            {
                double y = func(x);
                if (!IsFinite(y))
                    throw new ArithmeticException($"The function returned a non-finite value at x = {x}.");
                return y;
            }

            double Simpson
                ( double left, double right
                , double fLeft, double fMid, double fRight
                )
            {
                double estimate = ((right - left) / 6.0) * (fLeft + 4.0 * fMid + fRight);
                if (!IsFinite(estimate))
                    throw new ArithmeticException("The integration estimate overflowed.");
                return estimate;
            }

            double Refine
                ( double left, double right
                , double fLeft, double fMid, double fRight
                , double whole, double tol, int depth
                )
            {
                double mid      = left + (right - left) / 2.0;
                double leftMid  = left + (  mid - left) / 2.0;
                double rightMid = mid  + (right - mid ) / 2.0;

                if (leftMid <= left || leftMid >= mid ||
                    rightMid <= mid || rightMid >= right)
                    throw new ArithmeticException("Floating-point precision prevents further subdivision.");

                double fLeftMid  = Evaluate(leftMid );
                double fRightMid = Evaluate(rightMid);

                double leftEstimate  = Simpson(left, mid  , fLeft, fLeftMid , fMid  );
                double rightEstimate = Simpson(mid , right, fMid , fRightMid, fRight);

                double refined = leftEstimate + rightEstimate;
                double difference = refined - whole;

                if (!IsFinite(refined) || !IsFinite(difference))
                    throw new ArithmeticException("The integration estimate overflowed.");

                // Simpson error estimate and Richardson correction.
                if (Math.Abs(difference) / 15.0 <= tol)
                    return refined + difference / 15.0;

                if (depth == 0)
                    throw new InvalidOperationException("The requested tolerance was not reached. Increase maxDepth or relax tolerance.");

                return Refine(left, mid  , fLeft, fLeftMid , fMid  ,  leftEstimate, tol / 2.0, depth - 1)
                     + Refine(mid , right, fMid , fRightMid, fRight, rightEstimate, tol / 2.0, depth - 1);
            }

            double midpoint = a + (b - a) / 2.0;
            double fa = Evaluate(a);
            double fm = Evaluate(midpoint);
            double fb = Evaluate(b);

            double initial = Simpson(a, b, fa, fm, fb);
            double result = sign * Refine(a, b, fa, fm, fb, initial, tolerance, maxDepth);

            if (!IsFinite(result))
                throw new ArithmeticException("The integral overflowed.");

            return result;
        }
    }
}
