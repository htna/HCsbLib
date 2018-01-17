using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
		public static bool Double2Bool(double value)
		{
			if(value >= 0.5)
			{
				HDebug.Assert(value < 1.5);
				return true;
			}
			if(value < 0.5)
			{
				HDebug.Assert(value >= -0.5);
				return false;
			}
			HDebug.Assert(false);
			return default(bool);
		}
		public static double Bool2Double(bool value)
		{
			if(value == true)
				return 1;
			return 0;
		}
		public static bool GetValueBool(string name)
		{
			double value = GetValue(name);
			return Double2Bool(value);
		}
		public static bool[] GetVectorBool(string name)
		{
			double[] real = GetVector(name);
			bool[] vector = new bool[real.Length];
			for(int i=0; i<vector.Length; i++)
				vector[i] = Double2Bool(real[i]);
			return vector;
		}
		public static bool[] GetVectorBool(string name, int size)
		{
			bool[] vector = GetVectorBool(name);
			HDebug.Assert(vector.Length == size);
			return vector;
		}
		public static bool[,] GetMatrixBool(string name)
		{
			double[,] real = GetMatrix(name);
			bool[,] matrix = new bool[real.GetLength(0), real.GetLength(1)];
			for(int c=0; c<real.GetLength(0); c++)
				for(int r=0; r<real.GetLength(1); r++)
					matrix[c, r] = Double2Bool(real[c, r]);
			return matrix;
		}
		public static bool[,] GetMatrixBool(string name, int colsize, int rowsize)
		{
			bool[,] matrix = GetMatrixBool(name);
			HDebug.Assert(matrix.GetLength(0) == colsize);
			HDebug.Assert(matrix.GetLength(1) == rowsize);
			return matrix;
		}
		public static void PutValue(string name, bool value)
		{
			double real = Bool2Double(value);
			PutValue(name, real);
			Execute(name+"=("+name+"~=0);");
		}
		public static void PutVector(string name, bool[] vector)
		{
			double[] real = new double[vector.Length];
			for(int i=0; i<real.Length; i++)
				real[i] = Bool2Double(vector[i]);
			PutVector(name, real);
			Execute(name+"=("+name+"~=0);");
		}
		public static void PutMatrix(string name, bool[,] matrix)
		{
			double[,] real = new double[matrix.GetLength(0), matrix.GetLength(1)];
			for(int c=0; c<real.GetLength(0); c++)
				for(int r=0; r<real.GetLength(1); r++)
					real[c, r] = Bool2Double(matrix[c, r]);
			PutMatrix(name, real);
			Execute(name+"=("+name+"~=0);");
		}
	}
}
