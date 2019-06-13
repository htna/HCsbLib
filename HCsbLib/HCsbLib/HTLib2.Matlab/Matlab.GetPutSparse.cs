using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
        /// Matlab.PutSparseMatrix("H", H.GetMatrixSparse(), 3, 3);
        //public static void PutSparseMatrix<MATRIX>(string name, MatrixSparse<MATRIX> real, int elemColSize, int elemRowSize)
        public static void PutSparseMatrix<MATRIX>(string name, IMatrixSparse<MATRIX> real, int elemColSize, int elemRowSize)
            where MATRIX : Matrix
		{
            {
                /// http://www.mathworks.com/help/matlab/ref/sparse.html
                /// S = sparse(i,j,s,m,n)
                /// * create m-by-n sparse matrix
                /// * where S(i(k),j(k)) = s(k)
                /// * Vectors i, j, and s are all the same length.
                /// * Any elements of s that are zero are ignored.
                /// * Any elementsof s that have duplicate values of i and j are added together. 
                int m = real.ColSize * elemColSize;
                int n = real.RowSize * elemRowSize;
                List<int> i = new List<int>();
                List<int> j = new List<int>();
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
                PutValue("htlib2_matlab_PutSparseMatrix.m", m);
                PutValue("htlib2_matlab_PutSparseMatrix.n", n);
                PutVector("htlib2_matlab_PutSparseMatrix.i", i.ToArray());
                PutVector("htlib2_matlab_PutSparseMatrix.j", j.ToArray());
                PutVector("htlib2_matlab_PutSparseMatrix.s", s.ToArray());
                Execute("htlib2_matlab_PutSparseMatrix = sparse(htlib2_matlab_PutSparseMatrix.i+1, htlib2_matlab_PutSparseMatrix.j+1, htlib2_matlab_PutSparseMatrix.s, htlib2_matlab_PutSparseMatrix.m, htlib2_matlab_PutSparseMatrix.n);");
                Execute(name+" = htlib2_matlab_PutSparseMatrix;");
                Execute("clear htlib2_matlab_PutSparseMatrix;");
            }
		}
	}
}
