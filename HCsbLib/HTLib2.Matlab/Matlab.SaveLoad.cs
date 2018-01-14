using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
        public static void SaveMatrix(string filename, double[,] matrix, bool bUseFile=false)
        {
            PutMatrix("htlib2matlab_SavMat", matrix, bUseFile);
            Execute("save('$$','htlib2matlab_SavMat','-v4')", "$$", filename);
            Execute("clear htlib2matlab_SavMat;");
        }
        public static double[,] LoadMatrix(string filename, bool bUseFile=false)
        {
            Execute("load('$$','htlib2matlab_SavMat')", "$$", filename);
            double[,] mat = GetMatrix("htlib2matlab_SavMat", bUseFile);
            Execute("clear htlib2matlab_SavMat;");
            return mat;
        }
        public static void SaveVector(string filename, double[] vector)
        {
            PutVector("htlib2matlab_SavMat", vector);
            Execute("save('$$','htlib2matlab_SavMat','-v4')", "$$", filename);
            Execute("clear htlib2matlab_SavMat;");
        }
        public static double[] LoadVector(string filename)
        {
            Execute("load('$$','htlib2matlab_SavMat')", "$$", filename);
            double[] mat = GetVector("htlib2matlab_SavMat");
            Execute("clear htlib2matlab_SavMat;");
            return mat;
        }
    }
}
