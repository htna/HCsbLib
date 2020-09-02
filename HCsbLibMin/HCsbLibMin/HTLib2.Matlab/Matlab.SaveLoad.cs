using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
        public static void SaveMatrix(string filename, double[,] matrix, string varname=null, bool bUseFile=false)
        {
            if(varname == null)
                varname = "htlib2matlab_SavMat";
            PutMatrix(varname, matrix, bUseFile);
            Execute("save('$$','$1','-v4')", "$$", filename, "$1", varname);
            Execute("clear "+varname+";");
        }
        public static double[,] LoadMatrix(string filename, string varname=null, bool bUseFile=false)
        {
            if(varname == null)
                varname = "htlib2matlab_SavMat";
            Execute("load('$$','$1')", "$$", filename, "$1", varname);
            double[,] mat = GetMatrix(varname, bUseFile);
            Execute("clear "+varname+";");
            return mat;
        }
        public static void SaveVector(string filename, double[] vector, string varname=null)
        {
            if(varname == null)
                varname = "htlib2matlab_SavMat";
            PutVector(varname, vector);
            Execute("save('$$','$1','-v4')", "$$", filename, "$1", varname);
            Execute("clear "+varname+";");
        }
        public static double[] LoadVector(string filename, string varname=null)
        {
            if(varname == null)
                varname = "htlib2matlab_SavMat";
            Execute("load('$$','$1')", "$$", filename, "$1", varname);
            double[] mat = GetVector(varname);
            Execute("clear "+varname+";");
            return mat;
        }
    }
}
