using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
        /// Matlab.PutSparseMatrix("H", H.GetMatrixSparse(), 3, 3);
        //public static void PutSparseMatrix<MATRIX>(string name, MatrixSparse<MATRIX> real, int elemColSize, int elemRowSize)
        //public static void PutSparseMatrix<MATRIX>(string name, IMatrixSparse<MATRIX> real, int elemColSize, int elemRowSize)
        //    where MATRIX : Matrix
        public static void PutSparseMatrix(string name, IMatrixSparse<double> real, string opt=null)
		{
            /// http://www.mathworks.com/help/matlab/ref/sparse.html
            /// S = sparse(i,j,s,m,n)
            /// * create m-by-n sparse matrix
            /// * where S(i(k),j(k)) = s(k)
            /// * Vectors i, j, and s are all the same length.
            /// * Any elements of s that are zero are ignored.
            /// * Any elementsof s that have duplicate values of i and j are added together. 
            if(opt == null)
            {
                int m = real.ColSize;
                int n = real.RowSize;
                {
                    List<int   > i = new List<int   >();
                    List<int   > j = new List<int   >();
                    List<double> s = new List<double>();
                    foreach(var c_r_val in real.EnumElements())
                    {
                        int    c   = c_r_val.Item1;
                        int    r   = c_r_val.Item2;
                        double val = c_r_val.Item3;
                        if(val == 0) continue;
                        i.Add(c  );
                        j.Add(r  );
                        s.Add(val);
                    }
                    PutVector("htlib2_matlab_PutSparseMatrix.i", i.ToArray());
                    PutVector("htlib2_matlab_PutSparseMatrix.j", j.ToArray());
                    PutVector("htlib2_matlab_PutSparseMatrix.s", s.ToArray());
                }
                PutValue("htlib2_matlab_PutSparseMatrix.m", m);
                PutValue("htlib2_matlab_PutSparseMatrix.n", n);
                Execute("htlib2_matlab_PutSparseMatrix = sparse(htlib2_matlab_PutSparseMatrix.i+1, htlib2_matlab_PutSparseMatrix.j+1, htlib2_matlab_PutSparseMatrix.s, htlib2_matlab_PutSparseMatrix.m, htlib2_matlab_PutSparseMatrix.n);");
                Execute(name+" = htlib2_matlab_PutSparseMatrix;");
                Execute("clear htlib2_matlab_PutSparseMatrix;");
            }
            else if(opt == "use file")
            {
                string i_path = HFile.GetTempPath(_path_temporary, ".dat");
                string j_path = HFile.GetTempPath(_path_temporary, ".dat");
                string s_path = HFile.GetTempPath(_path_temporary, ".dat");
                ulong count = 0;
                {
                    System.IO.BinaryWriter i_writer = new System.IO.BinaryWriter(new System.IO.FileStream(i_path, System.IO.FileMode.CreateNew));
                    System.IO.BinaryWriter j_writer = new System.IO.BinaryWriter(new System.IO.FileStream(j_path, System.IO.FileMode.CreateNew));
                    System.IO.BinaryWriter s_writer = new System.IO.BinaryWriter(new System.IO.FileStream(s_path, System.IO.FileMode.CreateNew));
                    foreach (var c_r_val in real.EnumElements())
                    {
                        int    c   = c_r_val.Item1;
                        int    r   = c_r_val.Item2;
                        double val = c_r_val.Item3;

                        count ++;
                        i_writer.Write(c  );
                        j_writer.Write(r  );
                        s_writer.Write(val);
                        HDebug.Exception(count > 0);
                    }
                    i_writer.Flush(); i_writer.Close();
                    j_writer.Flush(); j_writer.Close();
                    s_writer.Flush(); s_writer.Close();
                }
                {
                    PutValue("htlib2_matlab_PutSparseMatrix.m", real.ColSize);
                    PutValue("htlib2_matlab_PutSparseMatrix.n", real.RowSize);
                    Execute("htlib2_matlab_PutSparseMatrix.ifid=fopen('" + i_path + "','r');");
                    Execute("htlib2_matlab_PutSparseMatrix.jfid=fopen('" + j_path + "','r');");
                    Execute("htlib2_matlab_PutSparseMatrix.sfid=fopen('" + s_path + "','r');");
                    // A = fread(fileID) reads all the data in the file into a vector of class double. By default, fread reads a file 1 byte at a time, interprets each byte as an 8-bit unsigned integer (uint8), and returns a double array.
                    Execute("htlib2_matlab_PutSparseMatrix.imat=fread(htlib2_matlab_PutSparseMatrix.ifid, ["+count+"],'*double')';");
                    Execute("htlib2_matlab_PutSparseMatrix.jmat=fread(htlib2_matlab_PutSparseMatrix.jfid, ["+count+"],'*double')';");
                    Execute("htlib2_matlab_PutSparseMatrix.smat=fread(htlib2_matlab_PutSparseMatrix.sfid, ["+count+"],'*double')';");
                    Execute("fclose(htlib2_matlab_PutSparseMatrix.ifid);");
                    Execute("fclose(htlib2_matlab_PutSparseMatrix.jfid);");
                    Execute("fclose(htlib2_matlab_PutSparseMatrix.sfid);");
                    Execute("htlib2_matlab_PutSparseMatrix = sparse(htlib2_matlab_PutSparseMatrix.imat+1, htlib2_matlab_PutSparseMatrix.jmat+1, htlib2_matlab_PutSparseMatrix.smat, htlib2_matlab_PutSparseMatrix.m, htlib2_matlab_PutSparseMatrix.n);");
                    Execute(name + " = htlib2_matlab_PutSparseMatrix;");
                    Execute("clear htlib2_matlab_PutSparseMatrix;");
                }
                HFile.Delete(i_path);
                HFile.Delete(j_path);
                HFile.Delete(s_path);
            }
        }
        public static void PutSparseMatrix(string name, IMatrixSparse<MatrixByArr> real, int elemColSize, int elemRowSize, string opt=null)
		{
            /// http://www.mathworks.com/help/matlab/ref/sparse.html
            /// S = sparse(i,j,s,m,n)
            /// * create m-by-n sparse matrix
            /// * where S(i(k),j(k)) = s(k)
            /// * Vectors i, j, and s are all the same length.
            /// * Any elements of s that are zero are ignored.
            /// * Any elementsof s that have duplicate values of i and j are added together. 
            if(opt == null)
            {
                int m = real.ColSize * elemColSize;
                int n = real.RowSize * elemRowSize;
                //if(opt == null)
                {
                    List<int   > i = new List<int   >();
                    List<int   > j = new List<int   >();
                    List<double> s = new List<double>();
                    foreach(var c_r_val in real.EnumElements())
                    {
                        int c = c_r_val.Item1;
                        int r = c_r_val.Item2;
                        Matrix hesscr = c_r_val.Item3;
                        HDebug.Assert(hesscr != null);
                        HDebug.Assert(hesscr.ColSize == elemColSize, hesscr.RowSize == elemRowSize);
                        for(int dc=0; dc<elemColSize; dc++)
                            for(int dr=0; dr<elemRowSize; dr++)
                            {
                                i.Add(c*elemColSize+dc);
                                j.Add(r*elemRowSize+dr);
                                s.Add(hesscr[dc, dr]);
                            }
                    }
                    //for(int ii=0; ii<i.Count; ii++)
                    //{
                    //    if(i[ii] == j[ii])
                    //        HDebug.Assert(s[ii] != 0);
                    //}
                    PutVector("htlib2_matlab_PutSparseMatrix.i", i.ToArray());
                    PutVector("htlib2_matlab_PutSparseMatrix.j", j.ToArray());
                    PutVector("htlib2_matlab_PutSparseMatrix.s", s.ToArray());
                }
                //else
                //{
                //    Execute("htlib2_matlab_PutSparseMatrix.i = [];");
                //    Execute("htlib2_matlab_PutSparseMatrix.j = [];");
                //    Execute("htlib2_matlab_PutSparseMatrix.s = [];");
                //
                //    int maxleng = 10_000_000; // Count = 134217728 (maximum)
                //    List<int   > i = new List<int   >(maxleng);
                //    List<int   > j = new List<int   >(maxleng);
                //    List<double> s = new List<double>(maxleng);
                //    foreach(var c_r_val in real.EnumElements())
                //    {
                //        int c = c_r_val.Item1;
                //        int r = c_r_val.Item2;
                //        Matrix hesscr = c_r_val.Item3;
                //        HDebug.Assert(hesscr != null);
                //        HDebug.Assert(hesscr.ColSize == elemColSize, hesscr.RowSize == elemRowSize);
                //        for(int dc=0; dc<elemColSize; dc++)
                //            for(int dr=0; dr<elemRowSize; dr++)
                //            {
                //                if(i.Count == maxleng)
                //                {
                //                    PutVector("htlib2_matlab_PutSparseMatrix.ix", i.ToArray());
                //                    PutVector("htlib2_matlab_PutSparseMatrix.jx", j.ToArray());
                //                    PutVector("htlib2_matlab_PutSparseMatrix.sx", s.ToArray());
                //                    Execute("htlib2_matlab_PutSparseMatrix.i = [htlib2_matlab_PutSparseMatrix.i; htlib2_matlab_PutSparseMatrix.ix];");
                //                    Execute("htlib2_matlab_PutSparseMatrix.j = [htlib2_matlab_PutSparseMatrix.j; htlib2_matlab_PutSparseMatrix.jx];");
                //                    Execute("htlib2_matlab_PutSparseMatrix.s = [htlib2_matlab_PutSparseMatrix.s; htlib2_matlab_PutSparseMatrix.sx];");
                //                    Execute("clear htlib2_matlab_PutSparseMatrix.ix;");
                //                    Execute("clear htlib2_matlab_PutSparseMatrix.jx;");
                //                    Execute("clear htlib2_matlab_PutSparseMatrix.sx;");
                //                    i.Clear();
                //                    j.Clear();
                //                    s.Clear();
                //                }
                //                i.Add(c*elemColSize+dc);
                //                j.Add(r*elemRowSize+dr);
                //                s.Add(hesscr[dc, dr]);
                //            }
                //    }
                //    if(i.Count != 0)
                //    {
                //        PutVector("htlib2_matlab_PutSparseMatrix.ix", i.ToArray());
                //        PutVector("htlib2_matlab_PutSparseMatrix.jx", j.ToArray());
                //        PutVector("htlib2_matlab_PutSparseMatrix.sx", s.ToArray());
                //        Execute("htlib2_matlab_PutSparseMatrix.i = [htlib2_matlab_PutSparseMatrix.i; htlib2_matlab_PutSparseMatrix.ix];");
                //        Execute("htlib2_matlab_PutSparseMatrix.j = [htlib2_matlab_PutSparseMatrix.j; htlib2_matlab_PutSparseMatrix.jx];");
                //        Execute("htlib2_matlab_PutSparseMatrix.s = [htlib2_matlab_PutSparseMatrix.s; htlib2_matlab_PutSparseMatrix.sx];");
                //        Execute("htlib2_matlab_PutSparseMatrix.ix = [];");
                //        Execute("htlib2_matlab_PutSparseMatrix.jx = [];");
                //        Execute("htlib2_matlab_PutSparseMatrix.sx = [];");
                //        i.Clear();
                //        j.Clear();
                //        s.Clear();
                //    }
                //    HDebug.Assert(i.Count == 0);
                //    HDebug.Assert(j.Count == 0);
                //    HDebug.Assert(s.Count == 0);
                //}
                PutValue("htlib2_matlab_PutSparseMatrix.m", m);
                PutValue("htlib2_matlab_PutSparseMatrix.n", n);
                Execute("htlib2_matlab_PutSparseMatrix = sparse(htlib2_matlab_PutSparseMatrix.i+1, htlib2_matlab_PutSparseMatrix.j+1, htlib2_matlab_PutSparseMatrix.s, htlib2_matlab_PutSparseMatrix.m, htlib2_matlab_PutSparseMatrix.n);");
                Execute(name+" = htlib2_matlab_PutSparseMatrix;");
                Execute("clear htlib2_matlab_PutSparseMatrix;");
            }
            else if(opt == "use file")
            {
                string i_path = HFile.GetTempPath(_path_temporary, ".dat");
                string j_path = HFile.GetTempPath(_path_temporary, ".dat");
                string s_path = HFile.GetTempPath(_path_temporary, ".dat");
                ulong count = 0;
                {
                    System.IO.BinaryWriter i_writer = new System.IO.BinaryWriter(new System.IO.FileStream(i_path, System.IO.FileMode.CreateNew));
                    System.IO.BinaryWriter j_writer = new System.IO.BinaryWriter(new System.IO.FileStream(j_path, System.IO.FileMode.CreateNew));
                    System.IO.BinaryWriter s_writer = new System.IO.BinaryWriter(new System.IO.FileStream(s_path, System.IO.FileMode.CreateNew));
                    foreach (var c_r_val in real.EnumElements())
                    {
                        int c = c_r_val.Item1;
                        int r = c_r_val.Item2;
                        Matrix hesscr = c_r_val.Item3;
                        HDebug.Assert(hesscr != null);
                        HDebug.Assert(hesscr.ColSize == elemColSize, hesscr.RowSize == elemRowSize);
                        for (int dc = 0; dc < elemColSize; dc++)
                            for (int dr = 0; dr < elemRowSize; dr++)
                            {
                                count ++;
                                double i = (c * elemColSize + dc);
                                double j = (r * elemRowSize + dr);
                                double s = (hesscr[dc, dr]);
                                i_writer.Write(i);
                                j_writer.Write(j);
                                s_writer.Write(s);
                                HDebug.Exception(count > 0);
                            }
                    }
                    i_writer.Flush(); i_writer.Close();
                    j_writer.Flush(); j_writer.Close();
                    s_writer.Flush(); s_writer.Close();
                }
                {
                    int m = real.ColSize * elemColSize;
                    int n = real.RowSize * elemRowSize;
                    PutValue("htlib2_matlab_PutSparseMatrix.m", m);
                    PutValue("htlib2_matlab_PutSparseMatrix.n", n);
                    Execute("htlib2_matlab_PutSparseMatrix.ifid=fopen('" + i_path + "','r');");
                    Execute("htlib2_matlab_PutSparseMatrix.jfid=fopen('" + j_path + "','r');");
                    Execute("htlib2_matlab_PutSparseMatrix.sfid=fopen('" + s_path + "','r');");
                    // A = fread(fileID) reads all the data in the file into a vector of class double. By default, fread reads a file 1 byte at a time, interprets each byte as an 8-bit unsigned integer (uint8), and returns a double array.
                    Execute("htlib2_matlab_PutSparseMatrix.imat=fread(htlib2_matlab_PutSparseMatrix.ifid, ["+count+"],'*double')';");
                    Execute("htlib2_matlab_PutSparseMatrix.jmat=fread(htlib2_matlab_PutSparseMatrix.jfid, ["+count+"],'*double')';");
                    Execute("htlib2_matlab_PutSparseMatrix.smat=fread(htlib2_matlab_PutSparseMatrix.sfid, ["+count+"],'*double')';");
                    Execute("fclose(htlib2_matlab_PutSparseMatrix.ifid);");
                    Execute("fclose(htlib2_matlab_PutSparseMatrix.jfid);");
                    Execute("fclose(htlib2_matlab_PutSparseMatrix.sfid);");
                    Execute("htlib2_matlab_PutSparseMatrix = sparse(htlib2_matlab_PutSparseMatrix.imat+1, htlib2_matlab_PutSparseMatrix.jmat+1, htlib2_matlab_PutSparseMatrix.smat, htlib2_matlab_PutSparseMatrix.m, htlib2_matlab_PutSparseMatrix.n);");
                    Execute(name + " = htlib2_matlab_PutSparseMatrix;");
                    Execute("clear htlib2_matlab_PutSparseMatrix;");
                }
                HFile.Delete(i_path);
                HFile.Delete(j_path);
                HFile.Delete(s_path);
            }
        }
	}
}
