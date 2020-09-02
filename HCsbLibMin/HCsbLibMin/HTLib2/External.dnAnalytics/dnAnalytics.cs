using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class dnAnalytics
	{
		static global::dnAnalytics.Statistics.Distributions.Normal _NextNormal = new global::dnAnalytics.Statistics.Distributions.Normal();
		public static double NextNormal()
		{
			return _NextNormal.RandomNumberGenerator.NextDouble();
		}

		public static global::dnAnalytics.LinearAlgebra.Matrix ToMatrix(double[,] mat)
		{
			global::dnAnalytics.LinearAlgebra.Matrix MAT = new global::dnAnalytics.LinearAlgebra.DenseMatrix(mat.GetLength(0), mat.GetLength(1));
			for(int r=0; r<mat.GetLength(0); r++)
				for(int c=0; c<mat.GetLength(1); c++)
					MAT[r, c] = mat[r, c];
			return MAT;
		}
		public static double[,] ToArray(global::dnAnalytics.LinearAlgebra.Matrix MAT)
		{
			double[,] mat = new double[MAT.Rows, MAT.Columns];
			for(int r=0; r<mat.GetLength(0); r++)
				for(int c=0; c<mat.GetLength(1); c++)
					mat[r, c] = MAT[r, c];
			return mat;
		}

		public static double[,] Inverse(double[,] mat)
		{
			global::dnAnalytics.LinearAlgebra.Matrix MAT = ToMatrix(mat);
			global::dnAnalytics.LinearAlgebra.Matrix INVERSED = MAT.Inverse();
			//global::dnAnalytics.LinearAlgebra.Matrix TEST = MAT * INVERSED;
			return ToArray(INVERSED);
		}

		public static double[] SvdSolver(double[,] A, double[] b)
		{
            if(HDebug.True)
			{
				// make AA dnAnalytics matrix
				global::dnAnalytics.LinearAlgebra.Matrix AA = new global::dnAnalytics.LinearAlgebra.DenseMatrix(A.GetLength(0), A.GetLength(1));
				for(int r=0; r<A.GetLength(0); r++)
					for(int c=0; c<A.GetLength(1); c++)
						AA[r, c] = A[r, c];
				// make bb dnAnalytics vector
				global::dnAnalytics.LinearAlgebra.Vector bb = new global::dnAnalytics.LinearAlgebra.DenseVector(b.Length);
				for(int i=0; i<b.Length; i++)
					bb[i] = b[i];
				// call SVD solver
				global::dnAnalytics.LinearAlgebra.Vector xx = (new global::dnAnalytics.LinearAlgebra.Solvers.Direct.SvdSolver()).Solve(AA, bb);
				if(HDebug.IsDebuggerAttached)
				{
					global::dnAnalytics.LinearAlgebra.Vector err = AA * xx - bb;
					double errd = err.DotProduct();
				}
				// convert xx dnAnalytics vector to x double array
				return xx.ToArray();
				//double[] x = new double[xx.Count];
				//for(int i=0; i<xx.Count; i++)
				//    x[i] = xx[i];
				//return x;
			}
			{
				// make AA dnAnalytics matrix
				global::dnAnalytics.LinearAlgebra.Matrix AA = new global::dnAnalytics.LinearAlgebra.DenseMatrix(A.GetLength(1), A.GetLength(1));
				for(int r=0; r<A.GetLength(1); r++)
					for(int c=0; c<A.GetLength(1); c++)
						for(int i=0; i<A.GetLength(0); i++)
							AA[r, c] += A[i, r] * A[i, c];
				// make bb dnAnalytics vector
				global::dnAnalytics.LinearAlgebra.Vector bb = new global::dnAnalytics.LinearAlgebra.DenseVector(A.GetLength(1));
				for(int r=0; r<A.GetLength(1); r++)
					for(int c=0; c<A.GetLength(1); c++)
						bb[c] += b[r] * A[r, c];
				global::dnAnalytics.LinearAlgebra.Vector xx = (new global::dnAnalytics.LinearAlgebra.Solvers.Iterative.MlkBiCgStab()).Solve(AA, bb);
				// convert xx dnAnalytics vector to x double array
				double[] x = new double[xx.Count];
				for(int i=0; i<xx.Count; i++)
					x[i] = xx[i];
				return x;
			}
		}
	}
}
