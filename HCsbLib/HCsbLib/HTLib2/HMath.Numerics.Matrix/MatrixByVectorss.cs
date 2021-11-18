using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public class MatrixByVectorss : IMatrix<double>
    {
        Vector[][] vecss;
        int vecsize;
        int colsize;
        int rowsize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MatrixByVectorss(Vector[][] vecss, int vecsize)
        {
            this.vecss   = vecss  ;
            this.vecsize = vecsize;
            colsize = vecss.Length * vecsize;
            rowsize = vecss[0].Length;
            // validate
            for(int c1=0; c1<vecss.Length; c1++)
            {
                if(vecss[c1] == null)
                    continue;
                HDebug.Assert(vecss[c1].Length == rowsize);
                for(int r=0; r<rowsize; r++)
                {
                    if(vecss[c1][r] == null)
                        continue;
                    HDebug.Assert(vecss[c1][r].Size == vecsize);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetAt(int c, int r)
        {
            HDebug.Assert(0 <= c && c < colsize);
            HDebug.Assert(0 <= r && r < rowsize);
            int c1 = c / vecsize;
            int c0 = c % vecsize;
            Vector[] vecs = vecss[c1];
            if(vecs == null) return 0;
            Vector vec = vecs[r];
            if(vec == null) return 0;
            HDebug.Assert(vecss[c1][r] == vec);
            return vec[c0];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAt(int c, int r, double value)
        {
            HDebug.Assert(0 <= c && c < colsize);
            HDebug.Assert(0 <= r && r < rowsize);
            int c1 = c / vecsize;
            int c0 = c % vecsize;
            if(vecss[c1] == null) vecss[c1] = new Vector[rowsize];
            Vector[] vecs = vecss[c1];
            if(vecs[r] == null) vecs[r] = new double[vecsize];
            Vector vec = vecs[r];
            HDebug.Assert(vecss[c1][r] == vec);
            vec[c0] = value;
        }
        /////////////////////////////////////////////////////////////////
        // IMatrix<double>
        public int ColSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return colsize; } }
        public int RowSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return rowsize; } }
        public double this[int  c, int  r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt(     c,      r); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt(     c,      r, value); } }
        public double this[long c, long r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt((int)c, (int)r); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt((int)c, (int)r, value); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double[,] ToArray()
        {
            double[,] mat = new double[colsize, rowsize];
            for(int r=0; r<rowsize; r++)
            {
                int colsize_vecsize = colsize/vecsize;
                for(int c1=0; c1<colsize_vecsize; c1++)
                {
                    int c1_vecsize = c1*vecsize;
                    Vector vec = vecss[c1][r];
                    for(int c0=0; c0<vecsize; c0++)
                        mat[c1_vecsize+c0,r] = vec[c0];
                }
            }
            return mat;
        }
        // IMatrix<double>
        /////////////////////////////////////////////////////////////////
    }
}
