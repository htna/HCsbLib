using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	using _daMatrix = global::dnAnalytics.LinearAlgebra.Matrix;
	using _daVector = global::dnAnalytics.LinearAlgebra.Vector;

	[Serializable]
	public class daVector
	{
		internal _daVector val;

		public daVector(_daVector source)							{ this.val = source; }
		public static explicit operator _daVector(daVector value)	{ return value.val; }
		public _daVector ToDaVector()								{ return this.val; }

		public static daVector operator -(daVector rightSide)						{ return new daVector(             - rightSide.val); }
		public static daVector operator -( daVector leftSide,  daVector rightSide)	{ return new daVector(leftSide.val - rightSide.val); }
		public static daVector operator -( daVector leftSide, _daVector rightSide)	{ return new daVector(leftSide.val - rightSide    ); }
		public static daVector operator -(_daVector leftSide,  daVector rightSide)	{ return new daVector(leftSide     - rightSide.val); }
		public static daVector operator *(double    leftSide,  daVector rightSide)	{ return new daVector(leftSide     * rightSide.val); }
		public static daVector operator *( daVector leftSide, double    rightSide)	{ return new daVector(leftSide.val * rightSide    ); }
		public static daMatrix operator *( daVector leftSide,  daVector rightSide)	{ return new daMatrix(leftSide.val * rightSide.val); }
		public static daMatrix operator *( daVector leftSide, _daVector rightSide)	{ return new daMatrix(leftSide.val * rightSide    ); }
		public static daMatrix operator *(_daVector leftSide,  daVector rightSide)	{ return new daMatrix(leftSide     * rightSide.val); }
		public static daVector operator *(_daMatrix leftSide,  daVector rightSide)	{ return new daVector(leftSide     * rightSide.val); }
		public static daVector operator *( daVector leftSide, _daMatrix rightSide)	{ return new daVector(leftSide.val * rightSide    ); }
		public static daVector operator /( daVector leftSide, double    rightSide)	{ return new daVector(leftSide.val / rightSide    ); }
		public static daVector operator +( daVector rightSide)						{ return new daVector(             + rightSide.val); }
		public static daVector operator +( daVector leftSide,  daVector rightSide)	{ return new daVector(leftSide.val + rightSide.val); }
		public static daVector operator +( daVector leftSide, _daVector rightSide)	{ return new daVector(leftSide.val + rightSide    ); }
		public static daVector operator +(_daVector leftSide,  daVector rightSide)	{ return new daVector(leftSide     + rightSide.val); }

		public int Count															{ get{ return val.Count; } }

		public double this[int index]												{ get{ return val[index];  }
																					  set{ val[index] = value; }
																					}

		public double AbsoluteMaximum()																	{ return val.AbsoluteMaximum(); }																
		public int AbsoluteMaximumIndex()																{ return val.AbsoluteMaximumIndex(); }
		public double AbsoluteMinimum()																	{ return val.AbsoluteMinimum(); }
		public int AbsoluteMinimumIndex()																{ return val.AbsoluteMinimumIndex(); }
		public void Add(double scalar)																	{ val.Add(scalar); }
		public void Add(_daVector other)																{ val.Add(other); }
		public void Add(double scalar, _daVector result)												{ val.Add(scalar, result); }
		public void Add(_daVector other, _daVector result)												{ val.Add(other, result); }
		public void AddScaledVector(double scale, _daVector other)										{ val.AddScaledVector(scale, other); }
		public void AddScaledVector(double scale, _daVector other, _daVector result)					{ val.AddScaledVector(scale, other, result); }
		public void Clear()																				{ val.Clear(); }
		public daVector Clone()																			{ return new daVector(val.Clone()); }
		public void CopyTo(_daVector target)															{ val.CopyTo(target); }
		public void CopyTo(_daVector destination, int offset, int destinationOffset, int count)			{ val.CopyTo(destination, offset, destinationOffset, count); }
		public daMatrix CreateMatrix(int rows, int columns)												{ return new daMatrix(val.CreateMatrix(rows, columns)); }
		public void Divide(double scalar)																{ val.Divide(scalar); }
		public void Divide(double scalar, _daVector result)												{ val.Divide(scalar, result); }
		public double DotProduct()																		{ return val.DotProduct(); }
		public double DotProduct(_daVector other)														{ return val.DotProduct(other); }
override public bool Equals(object obj)																	{ return val.Equals(obj); }
		public bool Equals(_daVector other)																{ return val.Equals(other); }
		public IEnumerator<double> GetEnumerator()														{ return val.GetEnumerator(); }
		public IEnumerator<KeyValuePair<int, double>> GetEnumerator(int index, int length)				{ return val.GetEnumerator(index, length); }
		public override int GetHashCode()																{ return val.GetHashCode(); }
		public IEnumerable<KeyValuePair<int, double>> GetIndexedEnumerator()							{ return val.GetIndexedEnumerator(); }
		public daVector GetSubVector(int index, int length)												{ return new daVector(val.GetSubVector(index, length)); }
		public double InfinityNorm()																	{ return val.InfinityNorm(); }
		public double Maximum()																			{ return val.Maximum(); }
		public int MaximumIndex()																		{ return val.MaximumIndex(); }
		public double Minimum()																			{ return val.Minimum(); }
		public int MinimumIndex()																		{ return val.MinimumIndex(); }
		public void Multiply(double scalar)																{ val.Multiply(scalar); }
		public daMatrix Multiply(_daVector other)														{ return new daMatrix(val.Multiply(other)); }
		public void Multiply(double scalar, _daVector result)											{ val.Multiply(scalar, result); }
		public void Multiply(_daVector other, _daMatrix result)											{ val.Multiply(other, result); }
		public void Negate()																			{ val.Negate(); }
		public daVector Normalize(int pValue)															{ return new daVector(val.Normalize(pValue)); }
		public daVector Plus()																			{ return new daVector(val.Plus()); }
		public double PNorm(int pValue)																	{ return val.PNorm(pValue); }
		public daVector PointwiseMultiply(_daVector other)												{ return new daVector(val.PointwiseMultiply(other)); }
		public void PointwiseMultiply(_daVector other, _daVector result)								{ val.PointwiseMultiply(other, result); }
		public void SetValues(double[] values)															{ val.SetValues(values); }
		public void Subtract(double scalar)																{ val.Subtract(scalar); }
		public void Subtract(_daVector other)															{ val.Subtract(other); }
		public void Subtract(double scalar, _daVector result)											{ val.Subtract(scalar, result); }
		public void Subtract(_daVector other, _daVector result)											{ val.Subtract(other, result); }
		public double Sum()																				{ return val.Sum(); }
		public double SumMagnitudes()																	{ return val.SumMagnitudes(); }
		public double[] ToArray()																		{ return val.ToArray(); }
		public override string ToString()																{ return val.ToString(); }
		public string ToString(IFormatProvider formatProvider)											{ return val.ToString(formatProvider); }
		public string ToString(string format)															{ return val.ToString(format); }
		public string ToString(string format, IFormatProvider formatProvider)							{ return val.ToString(format, formatProvider); }
	}
}
