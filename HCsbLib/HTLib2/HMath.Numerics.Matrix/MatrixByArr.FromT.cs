using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public partial class Matrix
	{
        public static Matrix FromColumVector(Vector vec)
        {
            Matrix mat = Matrix.Zeros(vec.Size, 1);
            for(int i=0; i<vec.Size; i++)
                mat[i, 0] = vec[i];
            return mat;
        }
        public static Matrix FromColVectorList(IList<Vector>   vec) { return FromColVectorListImpl(vec); }
        public static Matrix FromColVectorList(params Vector[] vec) { return FromColVectorListImpl(vec); }
        public static Matrix FromColVectorListImpl(IList<Vector> vec)
        {
            int colsize = vec[0].Size;
            int rowsize = vec.Count;
            Matrix mat = Matrix.Zeros(colsize, rowsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    mat[c, r] = vec[r][c];
            return mat;
        }
        public static Matrix FromRowVector(Vector vec)
        {
            Matrix mat = Matrix.Zeros(1, vec.Size);
            for(int i=0; i<vec.Size; i++)
                mat[0, i] = vec[i];
            return mat;
        }
        public static Matrix FromRowVectorList(IList<Vector> vec)
        {
            int[] colidxs = new int[vec.Count];
            for(int i=0; i<colidxs.Length; i++)
                colidxs[i] = i;
            return FromRowVectorList(vec, colidxs);
        }
        public static Matrix FromRowVectorList(IList<Vector> vec, IList<int> colidxs)
        {
            int colsize = colidxs.Count;
            int rowsize = vec[0].Size;
            Matrix mat = Matrix.Zeros(colsize, rowsize);
            foreach(int c in colidxs)
            {
                if(vec[c].Size != rowsize)
                {
                    HDebug.Assert(false);
                    return null;
                }
                for(int r=0; r<rowsize; r++)
                    mat[c, r] = vec[c][r];
            }
            return mat;
        }
        static bool FromMatrixArray_selftest = true;
        public static MatrixByArr FromMatrixArray(MatrixByArr[,] blockmatrix)
        {
            return FromMatrixArray(blockmatrix.HToType<MatrixByArr, Matrix>()).ToArray();
        }
        public static Matrix FromMatrixArray(Matrix[,] blockmatrix)
        {
            if(FromMatrixArray_selftest == true)
                #region selftest
            {
                {
                    FromMatrixArray_selftest = false;
                    Matrix[,] bmat = new Matrix[3, 2];
                    bmat[0, 0] = new double[1, 2] { { 0, 1 } }          ; bmat[0, 1] = new double[1, 3] { { 2, 3, 4 } }             ;
                    bmat[1, 0] = new double[2, 2] { { 0, 1 }, { 2, 3 } }; bmat[1, 1] = new double[2, 3] { { 4, 5, 6 }, { 7, 8, 9 } };
                    bmat[2, 0] = new double[1, 2] { { 9, 8 } }          ; bmat[2, 1] = new double[1, 3] { { 7, 6, 5 } }             ;
                    Matrix smat1 = FromMatrixArray(bmat);
                    Matrix smat0 = new double[,] { { 0, 1, 2, 3, 4 },
                                                   { 0, 1, 4, 5, 6 },
                                                   { 2, 3, 7, 8, 9 },
                                                   { 9, 8, 7, 6, 5 }
                                                 };
                    HDebug.AssertToleranceMatrix(0, smat1 - smat0);
                }
                {
                    FromMatrixArray_selftest = false;
                    Matrix[,] bmat = new Matrix[3, 2];
                    bmat[0, 0] = new double[1, 2] { { 0, 1 } }          ; bmat[0, 1] = new double[1, 3] { { 2, 3, 4 } }             ;
                    bmat[1, 0] = new double[2, 2] { { 0, 1 }, { 2, 3 } }; bmat[1, 1] = new double[2, 3] { { 4, 5, 6 }, { 7, 8, 9 } };
                    bmat[2, 0] = new double[1, 2] { { 9, 8 } }          ; bmat[2, 1] = null;
                    Matrix smat1 = FromMatrixArray(bmat);
                    HDebug.Assert(smat1 == null);
                }
                {
                    FromMatrixArray_selftest = false;
                    Matrix[,] bmat = new Matrix[3, 2];
                    bmat[0, 0] = new double[1, 2] { { 0, 1 } }          ; bmat[0, 1] = new double[1, 3] { { 2, 3, 4 } };
                    bmat[1, 0] = new double[2, 2] { { 0, 1 }, { 2, 3 } }; bmat[1, 1] = new double[1, 3] { { 4, 5, 6 } };
                    bmat[2, 0] = new double[1, 2] { { 9, 8 } }          ; bmat[2, 1] = new double[1, 3] { { 7, 6, 5 } };
                    Matrix smat1 = FromMatrixArray(bmat);
                    HDebug.Assert(smat1 == null);
                }
            }
            #endregion

            for(int bc=0; bc<blockmatrix.GetLength(0); bc++)
                for(int br=0; br<blockmatrix.GetLength(1); br++)
                {
                    if(blockmatrix[bc, br] == null) return null;
                    if(blockmatrix[bc,  0] == null) return null;
                    if(blockmatrix[ 0, br] == null) return null;
                    if(blockmatrix[bc, br].ColSize != blockmatrix[bc, 0].ColSize) return null;
                    if(blockmatrix[bc, br].RowSize != blockmatrix[0, br].RowSize) return null;
                }

            int[] colsize_acc = new int[blockmatrix.GetLength(0)+1];
            for(int bc=0; bc<blockmatrix.GetLength(0); bc++)
                colsize_acc[bc+1] = colsize_acc[bc] + blockmatrix[bc, 0].ColSize;
            int[] rowsize_acc = new int[blockmatrix.GetLength(1)+1];
            for(int br=0; br<blockmatrix.GetLength(1); br++)
                rowsize_acc[br+1] = rowsize_acc[br] + blockmatrix[0, br].RowSize;

            Matrix mat = Matrix.Zeros(colsize_acc.Last(), rowsize_acc.Last());
            for(int bc=0; bc<blockmatrix.GetLength(0); bc++)
                for(int br=0; br<blockmatrix.GetLength(1); br++)
                {
                    for(int ic=0; ic<blockmatrix[bc,br].ColSize; ic++)
                        for(int ir=0; ir<blockmatrix[bc,br].RowSize; ir++)
                        {
                            int c = ic + colsize_acc[bc];
                            int r = ir + rowsize_acc[br];
                            mat[c, r] = blockmatrix[bc, br][ic, ir];
                        }
                }

            return mat;
        }
        public static Matrix FromVectorArray(Vector[,] blockvector)
        {
            Matrix[,] blockmatrix = new Matrix[blockvector.GetLength(0), blockvector.GetLength(1)];
            for(int c=0; c<blockvector.GetLength(0); c++)
                for(int r=0; r<blockvector.GetLength(1); r++)
                    blockmatrix[c, r] = blockvector[c, r].ToColMatrix();
            return FromMatrixArray(blockmatrix);
        }
    }
}
