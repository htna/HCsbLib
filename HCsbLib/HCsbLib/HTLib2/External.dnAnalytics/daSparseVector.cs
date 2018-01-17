using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	using _daSparseVector = global::dnAnalytics.LinearAlgebra.SparseVector;

	[Serializable]
	public class daSparseVector : _daSparseVector
	{
		public daSparseVector(double[] values)          : base(values) {}
		public daSparseVector(int size)                 : base(size) {}
		public daSparseVector(_daSparseVector source)   : base(source) {}
		public daSparseVector(daSparseVector source)    : base(source) {}
		public daSparseVector(int size, double value)   : base(size, value) {}
	}
}
