using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
		public static MatrixByArr SubMatrixByFromCount(this MatrixByArr _this, int colfrom, int colcount, int rowfrom, int rowcount)
		{
			MatrixByArr submat = new MatrixByArr(colcount, rowcount);
			for(int c=0; c<colcount; c++)
				for(int r=0; r<rowcount; r++)
                    submat[c, r] = _this[c+colfrom, r+rowfrom];
			return submat;
		}
        public static MatrixByArr SubMatrixByFromTo(this MatrixByArr _this, int? colfrom, int? colto, int? rowfrom, int? rowto)
		{
			int _colfrom = colfrom ?? int.MinValue; _colfrom = Math.Max(_colfrom, 0              );
			int _colto   = colto   ?? int.MaxValue; _colto   = Math.Min(_colto  , _this.ColSize-1);
			int _rowfrom = rowfrom ?? int.MinValue; _rowfrom = Math.Max(_rowfrom, 0              );
			int _rowto   = rowto   ?? int.MaxValue; _rowto   = Math.Min(_rowto  , _this.RowSize-1);
			MatrixByArr submat = new MatrixByArr(_colto-_colfrom+1, _rowto-_rowfrom+1);
			for(int c=0; c<submat.ColSize; c++)
				for(int r=0; r<submat.RowSize; r++)
					submat[c, r] = _this[c+_colfrom, r+_rowfrom];
			return submat;
		}
        public static Matrix SubMatrix(this Matrix _this, IList<int> idxcols, IList<int> idxrows)
        {
            Matrix submat = Matrix.Zeros(idxcols.Count, idxrows.Count);
            for(int nc=0; nc<idxcols.Count; nc++)
                for(int nr=0; nr<idxrows.Count; nr++)
                {
                    int c = idxcols[nc];
                    int r = idxrows[nr];
                    if(c < 0) continue;
                    if(r < 0) continue;

                    submat[nc, nr] = _this[c, r];
                }
            return submat;
        }
        public static Matrix[,] SubMatrix(this Matrix _this, IList<IList<int>> idxcolss, IList<IList<int>> idxrowss)
        {
            Matrix[,] submats = new Matrix[idxcolss.Count, idxrowss.Count];
            for(int mc=0; mc<idxcolss.Count; mc++)
                for(int mr=0; mr<idxrowss.Count; mr++)
                    submats[mc, mr] = _this.SubMatrix(idxcolss[mc], idxrowss[mr]);
            return submats;
        }
        public static Matrix SubMatrix(this Matrix _this, (int from, int count) cols, (int from, int count) rows)
        {
            HDebug.Assert( cols.from >= 0 && cols.from+cols.count-1 < _this.ColSize-1 );
            HDebug.Assert( rows.from >= 0 && rows.from+rows.count-1 < _this.RowSize-1 );

            Matrix submat = Matrix.Zeros(cols.count, rows.count);
            for(int nc=0; nc<cols.count; nc++)
                for(int nr=0; nr<rows.count; nr++)
                {
                    int c = cols.from + nc;
                    int r = rows.from + nr;

                    submat[nc, nr] = _this[c, r];
                }
            return submat;
        }
    }
}
