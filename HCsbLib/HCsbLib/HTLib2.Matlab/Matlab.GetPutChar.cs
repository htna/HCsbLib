using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
		public static char Double2Char(double value)
		{
			char rnd = (char)(value + 0.5);
			return rnd;
		}
		public static double Char2Double(char value)
		{
			return value;
		}
		public static char GetValueChar(string name)
		{
			double value = GetValue("double("+name+")");
			return Double2Char(value);
		}
		public static char[] GetVectorChar(string name)
		{
			double[] real = GetVector("double("+name+")");
			char[] vector = new char[real.Length];
			for(int i=0; i<vector.Length; i++)
				vector[i] = Double2Char(real[i]);
			return vector;
		}
		public static char[] GetVectorChar(string name, int size)
		{
			char[] vector = GetVectorChar(name);
			HDebug.Assert(vector.Length == size);
			return vector;
		}
		public static char[,] GetMatrixChar(string name)
		{
			double[,] real = GetMatrix("double("+name+")");
			char[,] matrix = new char[real.GetLength(0), real.GetLength(1)];
			for(int c=0; c<real.GetLength(0); c++)
				for(int r=0; r<real.GetLength(1); r++)
					matrix[c, r] = Double2Char(real[c, r]);
			return matrix;
		}
		public static char[,] GetMatrixChar(string name, int colsize, int rowsize)
		{
			char[,] matrix = GetMatrixChar(name);
			HDebug.Assert(matrix.GetLength(0) == colsize);
			HDebug.Assert(matrix.GetLength(1) == rowsize);
			return matrix;
		}
		public static void PutValue(string name, char value)
		{
			double real = Char2Double(value);
			PutValue(name, real);
			Execute(name+"=char("+name+");");
		}
		public static void PutVector(string name, char[] vector)
		{
			double[] real = new double[vector.Length];
			for(int i=0; i<real.Length; i++)
				real[i] = Char2Double(vector[i]);
			PutVector(name, real);
			Execute(name+"=char("+name+");");
		}
		public static void PutMatrix(string name, char[,] matrix)
		{
			double[,] real = new double[matrix.GetLength(0), matrix.GetLength(1)];
			for(int c=0; c<real.GetLength(0); c++)
				for(int r=0; r<real.GetLength(1); r++)
					real[c, r] = Char2Double(matrix[c, r]);
			PutMatrix(name, real);
			Execute(name+"=char("+name+");");
		}
	}
}
