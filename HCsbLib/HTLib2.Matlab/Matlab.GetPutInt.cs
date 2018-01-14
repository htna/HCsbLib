using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
		public static int Double2Int(double value)
		{
			int rnd = System.Convert.ToInt32(value);
			return rnd;
		}
		public static double Int2Double(int value)
		{
			return value;
		}
		public static int GetValueInt(string name)
		{
			double value = GetValue(name);
			return Double2Int(value);
		}
        public static int[] GetVectorInt(string name)
        {
            double[] real = GetVector(name);
            int[] vector = new int[real.Length];
            for(int i=0; i<vector.Length; i++)
                vector[i] = Double2Int(real[i]);
            return vector;
        }
        public static TVector<int> GetVectorLargeInt(string name, bool bUseFile = false)
        {
            TVector<double> real = GetVectorLarge(name, bUseFile);
            TVector<int>  vector = new TVector<int>(real.SizeLong);
            for(long i=0; i<vector.SizeLong; i++)
                vector[i] = Double2Int(real[i]);
            return vector;
        }
        public static int[] GetVectorInt(string name, params object[] replace)
		{
            HDebug.Assert(replace.Length %2 == 0);
            for(int i=0; i<replace.Length; i+=2)
            {
                string from = (string)replace[i+0];
                string to   = replace[i+1].ToString();
                name = name.Replace(from, to);
            }
            return GetVectorInt(name);
		}
		public static int[] GetVectorInt(string name, int size)
		{
			int[] vector = GetVectorInt(name);
			HDebug.Assert(vector.Length == size);
			return vector;
		}
		public static int[,] GetMatrixInt(string name)
		{
			double[,] real = GetMatrix(name);
			int[,] matrix = new int[real.GetLength(0), real.GetLength(1)];
			for(int c=0; c<real.GetLength(0); c++)
				for(int r=0; r<real.GetLength(1); r++)
					matrix[c, r] = Double2Int(real[c, r]);
			return matrix;
		}
		public static int[,] GetMatrixInt(string name, int colsize, int rowsize)
		{
			int[,] matrix = GetMatrixInt(name);
			HDebug.Assert(matrix.GetLength(0) == colsize);
			HDebug.Assert(matrix.GetLength(1) == rowsize);
			return matrix;
		}
		public static void PutValue(string name, int value)
		{
			double real = Int2Double(value);
			PutValue(name, real);
		}
		public static void PutVector(string name, int[] vector)
		{
			double[] real = new double[vector.Length];
			for(int i=0; i<real.Length; i++)
				real[i] = Int2Double(vector[i]);
			PutVector(name, real);
		}
		public static void PutMatrix(string name, int[,] matrix)
		{
			double[,] real = new double[matrix.GetLength(0), matrix.GetLength(1)];
			for(int c=0; c<real.GetLength(0); c++)
				for(int r=0; r<real.GetLength(1); r++)
					real[c, r] = Int2Double(matrix[c, r]);
			PutMatrix(name, real);
		}
	}
}
