using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	using _daMatrix = global::dnAnalytics.LinearAlgebra.Matrix;
	using _daDenseMatrix = global::dnAnalytics.LinearAlgebra.DenseMatrix;

	[Serializable]
	public class daDenseMatrix : _daDenseMatrix
	{
		public daDenseMatrix(double[,] source)      : base(source) {}
		public daDenseMatrix(int order)             : base(order) {}
		public daDenseMatrix(daDenseMatrix source)  : base(source) {}
		public daDenseMatrix(_daDenseMatrix source) : base(source) {}
		public daDenseMatrix(int rows, int columns) : base(rows, columns) {}

		//public daMatrix Inverse()
		//{
		//    return Inverse("svd");
		//}
		//public daMatrix Inverse(string option)
		//{
		//    return daMatrix.Inverse(this, option);
		//}
	}
}
