using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	using _daMatrix = global::dnAnalytics.LinearAlgebra.Matrix;
	using _daSparseMatrix = global::dnAnalytics.LinearAlgebra.SparseMatrix;

	[Serializable]
	public class daSparseMatrix : _daSparseMatrix
	{
		public daSparseMatrix(double[,] source)         : base(source) {}
		public daSparseMatrix(int order)                : base(order) {}
		public daSparseMatrix(daSparseMatrix source)    : base(source) {}
		public daSparseMatrix(_daSparseMatrix source)   : base(source) {}
		public daSparseMatrix(int rows, int columns)    : base(rows, columns) {}

		public _daMatrix ToDaMatrix() { return this; }

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
