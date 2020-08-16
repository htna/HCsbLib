using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    public partial class Matlab
    {
        public static double GetValue(string name)
        {
            double value;
            try
            {
                Execute("htlib2_matlab_GetValue = "+name+";");
                System.Array real = new double[1, 1];
                System.Array imag = new double[1, 1];
                matlab.GetFullMatrix("htlib2_matlab_GetValue", "base", ref real, ref imag);
                Execute("clear htlib2_matlab_GetValue;");
                value = (double)real.GetValue(0, 0);
            }
            catch(System.Runtime.InteropServices.COMException)
            {
                HDebug.Assert(false);
                value = double.NaN;
            }
            return value;
        }
        public static double[] GetVector(string name, bool bUseFile=false)
        {
            double[] vector;
            try
            {
                Execute("htlib2_matlab_GetVector = "+name+";");
                int size = GetValueInt("length(htlib2_matlab_GetVector)");
                if(size < 10000_0000)
                {
                    System.Array real = new double[size];
                    System.Array imag = new double[size];
                    matlab.GetFullMatrix("htlib2_matlab_GetVector", "base", ref real, ref imag);
                    Execute("clear htlib2_matlab_GetVector;");
                    vector = new double[size];
                    for(int i=0; i<size; i++)
                        vector[i] = (double)real.GetValue(i);
                }
                else
                {
                    bUseFile = true;
                    double[,] mat = GetMatrix(name, bUseFile);
                    HDebug.Assert(mat.GetLength(0) == size && mat.GetLength(1) == 1);
                    vector = new double[size];
                    for(int i = 0; i < size; i++)
                        vector[i] = mat[i, 0];
                }
            }
            catch(System.Runtime.InteropServices.COMException)
            {
                HDebug.Assert(false);
                vector = null;
            }
            return vector;
        }
        public static TVector<double> GetVectorLarge(string name, bool bUseFile=false)
        {
            TVector<double> vector;
            try
            {
                Execute("htlib2_matlab_GetVector = "+name+";");
                int size = GetValueInt("length(htlib2_matlab_GetVector)");
                if(size < TVector<double>.MaxBlockCapacity)
                {
                    System.Array real = new double[size];
                    System.Array imag = new double[size];
                    matlab.GetFullMatrix("htlib2_matlab_GetVector", "base", ref real, ref imag);
                    Execute("clear htlib2_matlab_GetVector;");
                    vector = new TVector<double>(size);
                    for(int i=0; i<size; i++)
                        vector[i] = (double)real.GetValue(i);
                }
                else
                {
                    HDebug.Assert(bUseFile, _path_temporary != null);
                    string tmppath = HFile.GetTempPath(_path_temporary, ".dat");
                    Execute("htlib2_matlab_GetVectorLarge.vec = " + name+";");
                    {
                        Execute("htlib2_matlab_GetVectorLarge.fid=fopen('" + tmppath+"','w');");
                        Execute("htlib2_matlab_GetVectorLarge.vec=fwrite(htlib2_matlab_GetVectorLarge.fid,htlib2_matlab_GetVectorLarge.vec','double');");
                        Execute("fclose(htlib2_matlab_GetVectorLarge.fid);");
                        Execute("clear htlib2_matlab_GetVectorLarge;");
                    }
                    {
                        bool clear_var = false;
                        if(clear_var)
                            Execute("clear "+name+";");
                    }
                    vector = new TVector<double>(size);
                    {
                        System.IO.BinaryReader reader = new System.IO.BinaryReader(new System.IO.FileStream(tmppath, System.IO.FileMode.Open));
                        for(long i=0; i< size; i++)
                            vector[i] = reader.ReadDouble();
                        reader.Close();
                    }
                    HFile.Delete(tmppath);
                    return vector;
                }
            }
            catch(System.Runtime.InteropServices.COMException)
            {
                HDebug.Assert(false);
                vector = null;
            }
            return vector;
        }
        public static double[] GetVector(string name, int size)
        {
            double[] vector = GetVector(name);
            HDebug.Assert(vector.Length == size);
            return vector;
        }
        public static double[,] GetMatrix(string name, bool bUseFile=false)
        {
            if((bUseFile == false) || (_path_temporary == null))
            {
                double[,] matrix;
                System.Array real;
                System.Array imag;
                Execute("htlib2_matlab_GetGetMatrix = "+name+";");
                int colsize = GetValueInt("size(htlib2_matlab_GetGetMatrix, 1)");
                int rowsize = GetValueInt("size(htlib2_matlab_GetGetMatrix, 2)");
                HDebug.Assert(colsize*rowsize < 2000*2000);
                real = new double[colsize, rowsize];
                imag = new double[colsize, rowsize];
                matlab.GetFullMatrix("htlib2_matlab_GetGetMatrix", "base", ref real, ref imag);
                Execute("clear htlib2_matlab_GetGetMatrix;");
                matrix = new double[colsize, rowsize];
                for(int c=0; c<colsize; c++)
                    for(int r=0; r<rowsize; r++)
                        matrix[c, r] = (double)real.GetValue(c, r);
                real = null;
                imag = null;
                return matrix;
            }
            //else
            //{
            //    Debug.Assert(false);
            //    return null;
            //}
            else
            {
                HDebug.Assert(bUseFile, _path_temporary != null);
                string tmppath = HFile.GetTempPath(_path_temporary, ".dat");
                Execute("clear htlib2_matlab_GetGetMatrix;");
                Execute("htlib2_matlab_GetGetMatrix.mat = 0;");
                Execute("htlib2_matlab_GetGetMatrix.mat = "+name+";");
                int colsize = GetValueInt("size(htlib2_matlab_GetGetMatrix.mat, 1)");
                int rowsize = GetValueInt("size(htlib2_matlab_GetGetMatrix.mat, 2)");
                {
                    Execute("htlib2_matlab_GetGetMatrix.fid=fopen('"+tmppath+"','w');");
                    Execute("htlib2_matlab_GetGetMatrix.mat=fwrite(htlib2_matlab_GetGetMatrix.fid,htlib2_matlab_GetGetMatrix.mat','double');");
                    Execute("fclose(htlib2_matlab_GetGetMatrix.fid);");
                    Execute("clear htlib2_matlab_GetGetMatrix;");
                }
                double[,] matrix = new double[colsize, rowsize];
                {
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(new System.IO.FileStream(tmppath, System.IO.FileMode.Open));
                    for(int c=0; c<colsize; c++)
                        for(int r=0; r<rowsize; r++)
                            matrix[c, r] = reader.ReadDouble();
                    reader.Close();
                }
                HFile.Delete(tmppath);
                return matrix;
            }
        }
        public static IMATRIX GetMatrix<IMATRIX>(string name, Func<int, int, IMATRIX> Zeros, bool bUseFile)
            where IMATRIX : IMatrix<double>
        {
            bool clear_var = false;
            return GetMatrix(name, Zeros, bUseFile, clear_var);
        }
        public static IMATRIX GetMatrix<IMATRIX>(string name, Func<int, int, IMATRIX> Zeros, bool bUseFile, bool clear_var)
            where IMATRIX : IMatrix<double>
        {
            if((bUseFile == false) || (_path_temporary == null))
            {
                System.Array real;
                System.Array imag;
                Execute("htlib2_matlab_GetGetMatrix = "+name+";");
                int colsize = GetValueInt("size(htlib2_matlab_GetGetMatrix, 1)");
                int rowsize = GetValueInt("size(htlib2_matlab_GetGetMatrix, 2)");
                HDebug.Assert(colsize*rowsize < 3000*3000);
                real = new double[colsize, rowsize];
                imag = new double[colsize, rowsize];
                matlab.GetFullMatrix("htlib2_matlab_GetGetMatrix", "base", ref real, ref imag);
                Execute("clear htlib2_matlab_GetGetMatrix;");
                IMATRIX matrix = Zeros(colsize, rowsize);
                for(int c=0; c<colsize; c++)
                    for(int r=0; r<rowsize; r++)
                        matrix[c, r] = (double)real.GetValue(c, r);
                real = null;
                imag = null;
                return matrix;
            }
            else
            {
                HDebug.Assert(bUseFile, _path_temporary != null);
                string tmppath = HFile.GetTempPath(_path_temporary, ".dat");
                Execute("clear htlib2_matlab_GetGetMatrix;");
                Execute("htlib2_matlab_GetGetMatrix.mat = 0;");
                Execute("htlib2_matlab_GetGetMatrix.mat = "+name+";");
                int colsize = GetValueInt("size(htlib2_matlab_GetGetMatrix.mat, 1)");
                int rowsize = GetValueInt("size(htlib2_matlab_GetGetMatrix.mat, 2)");
                {
                    Execute("htlib2_matlab_GetGetMatrix.fid=fopen('"+tmppath+"','w');");
                    Execute("htlib2_matlab_GetGetMatrix.mat=fwrite(htlib2_matlab_GetGetMatrix.fid,htlib2_matlab_GetGetMatrix.mat','double');");
                    Execute("fclose(htlib2_matlab_GetGetMatrix.fid);");
                    Execute("clear htlib2_matlab_GetGetMatrix;");
                }
                if(clear_var)
                    Execute("clear "+name+";");
                IMATRIX matrix = Zeros(colsize, rowsize);
                {
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(new System.IO.FileStream(tmppath, System.IO.FileMode.Open));
                    for(int c=0; c<colsize; c++)
                        for(int r=0; r<rowsize; r++)
                            matrix[c, r] = reader.ReadDouble();
                    reader.Close();
                }
                HFile.Delete(tmppath);
                return matrix;
            }
        }
        public static void GetMatrix(string name, out List<Vector> matrix)
        {
            double[,] _matrix = GetMatrix(name);
            matrix = new List<Vector>();
            for(int c=0; c<_matrix.GetLength(0); c++)
            {
                Vector vector = new double[_matrix.GetLength(1)];
                for(int r=0; r<_matrix.GetLength(1); r++)
                    vector[r] = _matrix[c,r];
                matrix[c] = vector;
            }
        }
        public static double[,] GetMatrix(string name, int colsize, int rowsize)
        {
            double[,] matrix = GetMatrix(name);
            HDebug.Assert(matrix.GetLength(0) == colsize);
            HDebug.Assert(matrix.GetLength(1) == rowsize);
            return matrix;
        }
        public static void PutValue(string name, double real)
        {
            System.Array arr_real = new double[1] { real };
            System.Array arr_imag = new double[1];
            matlab.PutFullMatrix("htlib2_matlab_PutValue", "base", arr_real, arr_imag);
            Execute(name+" = htlib2_matlab_PutValue;");
            Execute("clear htlib2_matlab_PutValue;");
        }
        public static void PutVector(string name, double[] real)
        {
            System.Array arr_real = real;
            System.Array arr_imag = new double[real.GetLength(0)];
            matlab.PutFullMatrix("htlib2_matlab_PutVector", "base", arr_real, arr_imag);
            Execute(name+" = htlib2_matlab_PutVector;");
            Execute("clear htlib2_matlab_PutVector;");
        }
        public static void PutMatrix(string name, IMatrix<double> real)
        {
            bool bUseFile = false;
            PutMatrix(name, real.ToArray(), bUseFile);
        }
        public static void PutMatrix(string name, double[,] real, bool bUseFile=false)
        {
            if((bUseFile == false) || (_path_temporary == null))
            {
                HDebug.Assert(real.Length < 2000*2000);
                System.Array arr_real = real;
                System.Array arr_imag = new double[real.GetLength(0), real.GetLength(1)];
                matlab.PutFullMatrix("htlib2_matlab_PutMatrix", "base", arr_real, arr_imag);
                Execute(name+" = htlib2_matlab_PutMatrix;");
                Execute("clear htlib2_matlab_PutMatrix;");
            }
            else
            {
                HDebug.Assert(bUseFile, _path_temporary != null);
                string tmppath = HFile.GetTempPath(_path_temporary, ".dat");
                int colsize = real.GetLength(0);
                int rowsize = real.GetLength(1);
                {
                    System.IO.BinaryWriter writer = new System.IO.BinaryWriter(new System.IO.FileStream(tmppath, System.IO.FileMode.CreateNew));
                    for(int c=0; c<colsize; c++)
                        for(int r=0; r<rowsize; r++)
                            writer.Write(real[c, r]);
                    writer.Flush();
                    writer.Close();
                }
                {
                    Execute("htlib2_matlab_PutMatrix.fid=fopen('"+tmppath+"','r');");
                    Execute("htlib2_matlab_PutMatrix.mat=fread(htlib2_matlab_PutMatrix.fid,["+rowsize+","+colsize+"],'*double')';");
                    Execute("fclose(htlib2_matlab_PutMatrix.fid);");
                    Execute(name+" = htlib2_matlab_PutMatrix.mat;");
                    Execute("clear htlib2_matlab_PutMatrix;");
                }
                HFile.Delete(tmppath);
            }
        }
        public static void PutMatrix(string name, IMatrix<double> real, bool bUseFile)
        {
            bool call_GC = false;
            PutMatrix(name, ref real, bUseFile, call_GC);
        }
        public static void PutMatrix(string name, ref IMatrix<double> real, bool bUseFile, bool call_GC)
        {
            if((bUseFile == false) || (_path_temporary == null))
            {
                HDebug.Assert(real.RowSize * real.ColSize < 2000*2000);
                System.Array arr_real = real.ToArray();
                System.Array arr_imag = new double[real.ColSize, real.RowSize];
                matlab.PutFullMatrix("htlib2_matlab_PutMatrix", "base", arr_real, arr_imag);
                Execute(name+" = htlib2_matlab_PutMatrix;");
                Execute("clear htlib2_matlab_PutMatrix;");
            }
            else
            {
                HDebug.Assert(bUseFile, _path_temporary != null);
                string tmppath = HFile.GetTempPath(_path_temporary, ".dat");
                int colsize = real.ColSize;
                int rowsize = real.RowSize;
                {
                    System.IO.BinaryWriter writer = new System.IO.BinaryWriter(new System.IO.FileStream(tmppath, System.IO.FileMode.CreateNew));
                    for(int c=0; c<colsize; c++)
                        for(int r=0; r<rowsize; r++)
                            writer.Write(real[c, r]);
                    writer.Flush();
                    writer.Close();
                }
                if(call_GC)
                {
                    real = null;
                    System.GC.Collect();
                }
                {
                    Execute("htlib2_matlab_PutMatrix.fid=fopen('"+tmppath+"','r');");
                    Execute("htlib2_matlab_PutMatrix.mat=fread(htlib2_matlab_PutMatrix.fid,["+rowsize+","+colsize+"],'*double')';");
                    Execute("fclose(htlib2_matlab_PutMatrix.fid);");
                    Execute(name+" = htlib2_matlab_PutMatrix.mat;");
                    Execute("clear htlib2_matlab_PutMatrix;");
                }
                HFile.Delete(tmppath);
            }
        }
        public static void PutMatrix(string name, IList<Vector> value)
        {
            int col = value.Count;
            int row = value[0].Size;
            double[,] real = new double[col, row];
            for(int c=0; c<col; c++)
                for(int r=0; r<row; r++)
                    real[c, r] = value[c][r];
            PutMatrix(name, real);
        }
    }
}
