using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
		public static Matrix ToMatrix(this IList<Vector> vecs, bool colvec=true)
		{
			if(colvec)
			{
				// [v1, v2, ... ]
				Matrix mat = Matrix.Zeros(vecs[0].Size, vecs.Count);
				for(int r=0; r<mat.RowSize; r++)
				{
					HDebug.Assert(mat.ColSize == vecs[r].Size);
					for(int c=0; c<mat.ColSize; c++)
						mat[c, r] = vecs[r][c];
				}
				return mat;
			}
			else
			{

				// [v1 ]
				// [v2 ]
				// [...]
				Matrix mat = Matrix.Zeros(vecs.Count, vecs[0].Size);
				for(int c=0; c<mat.ColSize; c++)
				{
					HDebug.Assert(mat.RowSize == vecs[c].Size);
					for(int r=0; r<mat.RowSize; r++)
						mat[c, r] = vecs[c][r];
				}
				if(HDebug.IsDebuggerAttachedWithProb(0.1))
					#region verify result
				{
                    Matrix matc = vecs.ToMatrix(true).Tr();
					HDebug.AssertToleranceMatrix(0, matc - mat);
				}
					#endregion
				return mat;
			}
		}

		public static MatrixByArr ToColMatrix(this Vector vec)
		{
			MatrixByArr mat = new MatrixByArr(vec.Size, 1);
			for(int i=0; i< vec.Size; i++)
				mat[i, 0] = vec[i];
			return mat;
		}
		public static MatrixByArr ToRowMatrix(this Vector vec)
        {
			MatrixByArr mat = new MatrixByArr(1, vec.Size);
			for(int i=0; i< vec.Size; i++)
				mat[0, i] = vec[i];
			return mat;
		}
    }
}
