using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public class Random : System.Random
	{
		public Random() { }
		public Random(int Seed) : base(Seed) { }
		public virtual int NextInt()							{ return base.Next(); }
		public virtual int NextInt(int maxValue)				{ return base.Next(maxValue); }
		public virtual int NextInt(int minValue, int maxValue)	{ return base.Next(minValue, maxValue); }

		public ValueTuple<double, double> NextNormal2()
		{
			// The Art of Computer Programming
			//
			// volume 2
			// Seminumerical Algorithms
			// Third Edition
			//
			// DONALD E. KNUTH
			//
			// page 122: "3.4.1 Numerical Distributions" -> "C.The normal distribution" -> "Algorithm P (Polar method for normal deviates)"
			double v1, v2, s;
			s = 2;
			do
			{
				// P1
				double u1 = NextDouble();
				double U2 = NextDouble();
				v1 = 2*u1 - 1;
				v2 = 2*U2 - 1;
				// P2
				s = v1*v1 + v2*v2;
			}
			// P3
			while(s >= 1);
			// P4
			double lnS = Math.Sqrt(-2*Math.Log(s)/s);
			double x1 = v1 * lnS;
			double x2 = v2 * lnS;
			return new ValueTuple<double, double>(x1, x2);
		}
		public double NextNormal()
		{
			return NextNormal2().Item1;
		}
		double _var     = 0;
		double _sqrtvar = 0;
		public double NextNormal(double var)
		{
			// X ~ N(m,v*v) => aX+b ~ N(am+b, a*a*v*v)
			// http://en.wikipedia.org/wiki/Normal_distribution
			if(var != _var)
			{
				_var     = var;
				_sqrtvar = Math.Sqrt(var);
			}

			return (NextNormal() * _sqrtvar);
		}
		public double NextNormalRanged(double range)
		{
			double rand;
			do
			{
				rand = NextNormal() / 3;
				rand *= range;
			} while(Math.Abs(rand) <= range);
			return rand;
		}
		public Vector NextNormalVector(Vector mean, double var)
		{
			return mean + NextNormalVector(mean.Size, var);
		}
		public Vector NextNormalVector(int vecsize, double var)
		{
			if(var == 0)
			{
				Vector vec = new Vector(vecsize);
				return vec;
			}
			if(var != _var)
			{
				_var     = var;
				_sqrtvar = Math.Sqrt(var);
			}
			{
				Vector vec = new Vector(vecsize);
				for(int i=0; i<vecsize; i+=2)
				{
					ValueTuple<double, double> val = NextNormal2();
					if(i+0 < vecsize) vec[i+0] = val.Item1 * _sqrtvar;
					if(i+1 < vecsize) vec[i+1] = val.Item2 * _sqrtvar;
				}
				return vec;
			}
		}
		public double NextUniform(double min, double max)
		{
			global::dnAnalytics.Random.AbstractRandomNumberGenerator rand = new global::dnAnalytics.Random.WH2006();
			double value0 = rand.NextDouble() * (max-min) + min;
			return value0;

			//double value = NextDouble();
			//value = min + (max-min)*value;
			//return value;
		}
		public double NextUniform()
		{
			return NextDouble();
		}
		public double NextUniform(ValueTuple<double, double> boundary, bool allowboundary)
		{
			return NextUniform(boundary.Item1, boundary.Item2, allowboundary);
		}
		public double NextUniform(double min, double max, bool allowboundary)
		{
			while(true)
			{
				double value = this.NextDouble();
				value = min + (max-min)*value;
				if((min<value) && (value<max))
					return value;
			}
		}
	}
}
