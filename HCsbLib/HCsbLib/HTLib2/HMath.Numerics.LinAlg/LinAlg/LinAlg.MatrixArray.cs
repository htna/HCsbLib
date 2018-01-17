using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public static partial class LinAlg
	{
        public static MatrixByArr[,] CreateMatrixArray(int colsize, int rowsize, MatrixByArr src)
        {
            MatrixByArr[,] blockmatrix = new MatrixByArr[colsize, rowsize];
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    blockmatrix[c, r] = src.CloneT();
            return blockmatrix;
        }

        public static MatrixByArr Sum    (this IList<MatrixByArr> vecs) { MatrixByArr sum = vecs[0].CloneT(); for(int i=1; i<vecs.Count; i++) sum += vecs[i]; return sum; }
        public static MatrixByArr Average(this IList<MatrixByArr> vecs) { MatrixByArr avg = vecs.Sum(); avg /= vecs.Count; return avg; }
    }
}
