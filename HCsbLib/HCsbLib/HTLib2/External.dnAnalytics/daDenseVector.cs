using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	using _daDenseVector = global::dnAnalytics.LinearAlgebra.DenseVector;

	[Serializable]
	public class daDenseVector : _daDenseVector
	{
		public daDenseVector(double[] values)           : base(values) {}
		public daDenseVector(int size)                  : base(size) {}
		public daDenseVector(_daDenseVector source)     : base(source) {}
		public daDenseVector(daDenseVector source)      : base(source) {}
		public daDenseVector(int size, double value)    : base(size, value) {}
	}
}
