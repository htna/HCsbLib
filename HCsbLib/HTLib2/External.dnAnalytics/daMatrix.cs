using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	using LinearAlgebra = global::dnAnalytics.LinearAlgebra;
	using _daMatrix = global::dnAnalytics.LinearAlgebra.Matrix;
	using _daVector = global::dnAnalytics.LinearAlgebra.Vector;
	using _daSparseMatrix = global::dnAnalytics.LinearAlgebra.SparseMatrix;
	using _daDenseMatrix  = global::dnAnalytics.LinearAlgebra.DenseMatrix;

	[Serializable]
	public class daMatrix
	{
		public readonly _daMatrix val;

		public daMatrix(_daMatrix source)						{ this.val = source; }
		public static explicit operator _daMatrix(daMatrix mat)	{ return mat.val; }
		public _daMatrix ToDaMatrix()							{ return this.val; }
		
		public static daMatrix operator -( daMatrix rightSide)						{ return new daMatrix(             - rightSide.val); }
		public static daMatrix operator -( daMatrix leftSide,  daMatrix rightSide)	{ return new daMatrix(leftSide.val + -1*rightSide.val); }
		public static daMatrix operator -( daMatrix leftSide, _daMatrix rightSide)	{ return new daMatrix(leftSide.val - rightSide    ); }
		public static daMatrix operator -(_daMatrix leftSide,  daMatrix rightSide)	{ return new daMatrix(leftSide     - rightSide.val); }
		public static daMatrix operator *(double    leftSide,  daMatrix rightSide)	{ return new daMatrix(leftSide     * rightSide.val); }
		public static daMatrix operator *( daMatrix leftSide, double   rightSide)	{ return new daMatrix(leftSide.val * rightSide    ); }
		public static daMatrix operator *( daMatrix leftSide,  daMatrix rightSide)	{ return new daMatrix(leftSide.val * rightSide.val); }
		public static daMatrix operator *( daMatrix leftSide, _daMatrix rightSide)	{ return new daMatrix(leftSide.val * rightSide    ); }
		public static daMatrix operator *(_daMatrix leftSide,  daMatrix rightSide)	{ return new daMatrix(leftSide     * rightSide.val); }
		public static daVector operator *( daMatrix leftSide,  daVector rightSide)	{ return new daVector(leftSide.val * rightSide.val); }
		public static daVector operator *( daMatrix leftSide, _daVector rightSide)	{ return new daVector(leftSide.val * rightSide    ); }
		public static daVector operator *( daVector leftSide,  daMatrix rightSide)	{ return new daVector(leftSide.val * rightSide.val); }
		public static daVector operator *(_daVector leftSide,  daMatrix rightSide)	{ return new daVector(leftSide     * rightSide.val); }
		public static daMatrix operator /( daMatrix leftSide, double   rightSide)	{ return new daMatrix(leftSide.val / rightSide    ); }
		public static daMatrix operator +( daMatrix rightSide)						{ return new daMatrix(             + rightSide.val); }
		public static daMatrix operator +( daMatrix leftSide,  daMatrix rightSide)	{ return new daMatrix(leftSide.val + rightSide.val); }
		public static daMatrix operator +( daMatrix leftSide, _daMatrix rightSide)	{ return new daMatrix(leftSide.val + rightSide    ); }
		public static daMatrix operator +(_daMatrix leftSide,  daMatrix rightSide)	{ return new daMatrix(leftSide     + rightSide.val); }
		
		public int Columns															{ get{ return val.Columns; } }
		public int Rows																{ get{ return val.Rows;    } }
		
		public double this[int row, int column]										{ get{ return val[row, column]; }
																					  set{ val[row, column] = value; }
																					}

		public void Add(double scalar)																					{ val.Add(scalar); }																					
		public void Add(_daMatrix other)																				{ val.Add(other); }
		public void Add(double scalar, _daMatrix result)																{ val.Add(scalar, result); }
		public void Add(_daMatrix other, _daMatrix result)																{ val.Add(other, result); }
		public daMatrix Append(_daMatrix right)																			{ return new daMatrix(val.Append(right)); }
		public void Append(_daMatrix right, _daMatrix result)															{ val.Append(right, result); }
		public void Clear()																								{ val.Clear(); }
		public daMatrix Clone()																							{ return new daMatrix(val.Clone()); }
		public double ConditionNumber()																					{ return val.ConditionNumber(); }
		public void CopyTo(_daMatrix target)																			{ val.CopyTo(target); }
		public double Determinant()																						{ return val.Determinant(); }
		public daMatrix DiagonalStack(_daMatrix lower)																	{ return new daMatrix(val.DiagonalStack(lower)); }
		public void DiagonalStack(_daMatrix lower, _daMatrix result)													{ val.DiagonalStack(lower, result); }
		public void Divide(double scalar)																				{ val.Divide(scalar); }
		public void Divide(double scalar, _daMatrix result)																{ val.Divide(scalar, result); }
		public bool Equals(_daMatrix other)																				{ return val.Equals(other); }
		public bool Equals(object obj)																					{ return val.Equals(obj); }
		public double FrobeniusNorm()																					{ return val.FrobeniusNorm(); }
		public void Gemm(double alpha, double beta, bool transposeA, bool transposeB, _daMatrix a, _daMatrix b)			{ val.Gemm(alpha, beta, transposeA, transposeB, a, b); }
		public daVector GetColumn(int index)																			{ return new daVector(val.GetColumn(index)); }
		public void GetColumn(int index, _daVector result)																{ val.GetColumn(index, result); }
		public daVector GetColumn(int columnIndex, int rowIndex, int length)											{ return new daVector(val.GetColumn(columnIndex, rowIndex, length)); }
		public void GetColumn(int columnIndex, int rowIndex, int length, _daVector result)								{ val.GetColumn(columnIndex, rowIndex, length, result); }
		public IEnumerable<KeyValuePair<int, daVector>> GetColumnEnumerator()											{ HDebug.Assert(false); return null; }
		public IEnumerable<KeyValuePair<int, daVector>> GetColumnEnumerator(int index, int length)						{ HDebug.Assert(false); return null; }
		public daVector GetDiagonal()																					{ return new daVector(val.GetDiagonal()); }
		public int GetHashCode()																						{ return val.GetHashCode(); }
		public daMatrix GetLowerTriangle()																				{ return new daMatrix(val.GetLowerTriangle()); }
		public void GetLowerTriangle(_daMatrix result)																	{ val.GetLowerTriangle(result); }
		public daVector GetRow(int index)																				{ return new daVector(val.GetRow(index)); }
		public void GetRow(int index, _daVector result)																	{ val.GetRow(index, result); }
		public daVector GetRow(int rowIndex, int columnIndex, int length)												{ return new daVector(val.GetRow(rowIndex, columnIndex, length)); }
		public void GetRow(int rowIndex, int columnIndex, int length, _daVector result)									{ val.GetRow(rowIndex, columnIndex, length, result); }
		public IEnumerable<KeyValuePair<int, daVector>> GetRowEnumerator()												{ HDebug.Assert(false); return null; }
		public IEnumerable<KeyValuePair<int, daVector>> GetRowEnumerator(int index, int length)							{ HDebug.Assert(false); return null; }
		public daMatrix GetStrictlyLowerTriangle()																		{ return new daMatrix(val.GetStrictlyLowerTriangle()); }
		public void GetStrictlyLowerTriangle(_daMatrix result)															{ val.GetStrictlyLowerTriangle(result); }
		public daMatrix GetStrictlyUpperTriangle()																		{ return new daMatrix(val.GetStrictlyUpperTriangle()); }
		public void GetStrictlyUpperTriangle(_daMatrix result)															{ val.GetStrictlyUpperTriangle(result); }
		public daMatrix GetSubdaMatrix(int rowIndex, int rowLength, int columnIndex, int columnLength)					{ return new daMatrix(val.GetSubMatrix(rowIndex, rowLength, columnIndex, columnLength)); }
		public daMatrix GetUpperTriangle()																				{ return new daMatrix(val.GetUpperTriangle()); }
		public void GetUpperTriangle(_daMatrix result)																	{ val.GetUpperTriangle(result); }
		public double InfinityNorm()																					{ return val.InfinityNorm(); }
		public daMatrix InsertColumn(int columnIndex, _daVector column)													{ return new daMatrix(val.InsertColumn(columnIndex, column)); }
		public daMatrix InsertRow(int rowIndex, _daVector row)															{ return new daMatrix(val.InsertRow(rowIndex, row)); }
		//public daMatrix Inverse()																						{ return new daMatrix(val.Inverse()); }
		public daMatrix KroneckerProduct(_daMatrix other)																{ return new daMatrix(val.KroneckerProduct(other)); }
		public void KroneckerProduct(_daMatrix other, _daMatrix result)													{ val.KroneckerProduct(other, result); }
		public double L1Norm()																							{ return val.L1Norm(); }
		public double L2Norm()																							{ return val.L2Norm(); }
		public daVector LeftMultiply(_daVector leftSide)																{ return new daVector(val.LeftMultiply(leftSide)); }
		public void LeftMultiply(_daVector leftSide, _daVector result)													{ val.LeftMultiply(leftSide, result); }
		public void Multiply(double scalar)																				{ val.Multiply(scalar); }
		public daMatrix Multiply(_daMatrix other)																		{ return new daMatrix(val.Multiply(other)); }
		public daVector Multiply(_daVector rightSide)																	{ return new daVector(val.Multiply(rightSide)); }
		public void Multiply(double scalar, _daMatrix result)															{ val.Multiply(scalar, result); }
		public void Multiply(_daMatrix other, _daMatrix result)															{ val.Multiply(other, result); }
		public void Multiply(_daVector rightSide, _daVector result)														{ val.Multiply(rightSide, result); }
		public void Negate()																							{ val.Negate(); }
		public void Negate(_daMatrix result)																			{ val.Negate(result); }
		public daMatrix NormalizeColumns(int pValue)																	{ return new daMatrix(val.NormalizeColumns(pValue)); }
		public daMatrix NormalizeRows(int pValue)																		{ return new daMatrix(val.NormalizeRows(pValue)); }
		public daMatrix Plus()																							{ return new daMatrix(val.Plus()); }
		public daMatrix PointwiseMultiply(_daMatrix other)																{ return new daMatrix(val.PointwiseMultiply(other)); }
		public void PointwiseMultiply(_daMatrix other, _daMatrix result)												{ val.PointwiseMultiply(other, result); }
		public void SetColumn(int index, double[] source)																{ val.SetColumn(index, source); }
		public void SetColumn(int index, _daVector source)																{ val.SetColumn(index, source); }
		public void SetDiagonal(double[] source)																		{ val.SetDiagonal(source); }
		public void SetDiagonal(_daVector source)																		{ val.SetDiagonal(source); }
		public void SetRow(int index, double[] source)																	{ val.SetRow(index, source); }
		public void SetRow(int index, _daVector source)																	{ val.SetRow(index, source); }
		public void SetSubdaMatrix(int rowIndex, int rowLength, int columnIndex, int columnLength, _daMatrix subMatrix)	{ val.SetSubMatrix(rowIndex, rowLength, columnIndex, columnLength, subMatrix); }
		public daMatrix Stack(_daMatrix lower)																			{ return new daMatrix(val.Stack(lower)); }
		public void Stack(_daMatrix lower, _daMatrix result)															{ val.Stack(lower, result); }
		public void Subtract(double scalar)																				{ val.Subtract(scalar); }
		public void Subtract(_daMatrix other)																			{ val.Subtract(other); }
		public void Subtract(double scalar, _daMatrix result)															{ val.Subtract(scalar, result); }
		public void Subtract(_daMatrix other, _daMatrix result)															{ val.Subtract(other, result); }
		public double[,] ToArray()																						{ return val.ToArray(); }
		public double[] ToColumnWiseArray()																				{ return val.ToColumnWiseArray(); }
		public double[] ToRowWiseArray()																				{ return val.ToRowWiseArray(); }
		public override string ToString()																				{ return val.ToString(); }
		public string ToString(IFormatProvider formatProvider)															{ return val.ToString(formatProvider); }
		public string ToString(string format)																			{ return val.ToString(format); }
		public string ToString(string format, IFormatProvider formatProvider)											{ return val.ToString(format, formatProvider); }
		public double Trace()																							{ return val.Trace(); }
		public daMatrix Transpose()																						{ return new daMatrix(val.Transpose()); }

		//public Pair<Matrix, Matrix> Eigensystem()
		//{
		//    Matrix mat = this.ToArray();
		//    Pair<Matrix, Matrix> val_vec = mat.Eigensystem();
		//    if(false)
		//    {
		//        Matrix val = val_vec.first;
		//        Matrix vec = val_vec.second;
		//        Matrix mat2 = vec * val * vec.Inverse();
		//        Matrix diff = mat - mat2;
		//    }
		//    return val_vec;
		//}
		//
		//public daMatrix Inverse()
		//{
		//    return Inverse("svd");
		//}
		//public daMatrix Inverse(string option)
		//{
		//    return daMatrix.Inverse(this.val, option);
		//}
		//static Triple<double[,], string, _daMatrix> Inverse_buffer;
		//public static _daMatrix InverseBuffered(double[,] mat, string option)
		//{
		//    if(Inverse_buffer == null) return null;
		//    if(option != Inverse_buffer.second) return null;
		//    if(mat.GetLength(0) != Inverse_buffer.first.GetLength(0)) return null;
		//    if(mat.GetLength(1) != Inverse_buffer.first.GetLength(1)) return null;
		//    for(int i=0; i<mat.GetLength(0); i++)
		//        for(int j=0; j<mat.GetLength(1); j++)
		//            if(mat[i, j] != Inverse_buffer.first[i, j])
		//            {
		//                return null;
		//            }
		//    return Inverse_buffer.third;
		//}
		//public static daMatrix Inverse(_daMatrix mat, string option)
		//{
		//    double[,] _mat = mat.ToArray();
		//    _daMatrix buffered = InverseBuffered(_mat, option);
		//    if(buffered != null)
		//        return new daMatrix(buffered);
		//
		//    _daMatrix imat;
		//    switch(option.ToLower())
		//    {
		//        case "svd":
		//            {
		//                Debug.Assert(mat.Rows == mat.Columns);
		//                int n = mat.Rows;
		//                global::dnAnalytics.LinearAlgebra.Decomposition.Svd svd = (new global::dnAnalytics.LinearAlgebra.Decomposition.Svd(mat, true));
		//                _daMatrix U = svd.U();
		//                _daMatrix W = svd.W();
		//                _daMatrix VT = svd.VT();
		//                _daSparseMatrix iW = new _daSparseMatrix(n, n);
		//                for(int i=0; i<n; i++)
		//                    if(W[i, i] >= 0.000000001)
		//                        iW[i, i] = 1 / W[i, i];
		//                    else iW[i, i] = 0;
		//                imat = VT.Transpose() * iW * U.Transpose();
		//            }
		//            break;
		//        case "default":
		//            {
		//                imat = mat.Inverse();
		//            }
		//            break;
		//        default:
		//            goto case "svd";
		//    }
		//
		//    Inverse_buffer = new Triple<double[,], string, _daMatrix>(_mat, option, imat);
		//    return new daMatrix(imat);
		//}
	}
}
