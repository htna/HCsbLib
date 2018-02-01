using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class HessMatrixDense : HessMatrix
    {
        public Matrix hess;

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix
        public override int ColSize { get { return hess.ColSize; } }
        public override int RowSize { get { return hess.RowSize; } }
        public override double this[int c, int r]
        {
            get { return hess[c, r]; }
            set { hess[c, r] = value; }
        }
        public override double this[long c, long r]
        {
            get { return hess[c, r]; }
            set { hess[c, r] = value; }
        }
        public override HessMatrix CloneHess()  { return CloneT(); }
        public HessMatrixDense CloneT() { return new HessMatrixDense { hess = hess.Clone() }; }

        public override int NumUsedBlocks
        {
            get
            {
                int count = 0;
                for(int bc=0; bc<ColBlockSize; bc++)
                    for(int br=0; br<RowBlockSize; br++)
                        if(HasBlock(bc, br))
                            count++;
                return count;
            }
        }

        public override MatrixByArr GetBlock(int bc, int br)
        {
            MatrixByArr bval = new double[3, 3];
            int nbc = bc*3;
            int nbr = br*3;
            bval[0, 0] = hess[nbc+0, nbr+0];
            bval[0, 1] = hess[nbc+0, nbr+1];
            bval[0, 2] = hess[nbc+0, nbr+2];
            bval[1, 0] = hess[nbc+1, nbr+0];
            bval[1, 1] = hess[nbc+1, nbr+1];
            bval[1, 2] = hess[nbc+1, nbr+2];
            bval[2, 0] = hess[nbc+2, nbr+0];
            bval[2, 1] = hess[nbc+2, nbr+1];
            bval[2, 2] = hess[nbc+2, nbr+2];
            return bval;
        }
        public override MatrixByArr GetBlockLock(int bc, int br)
        {
            lock(hess)
            {
                return GetBlock(bc, br);
            }
        }
        public override void SetBlock(int bc, int br, MatrixByArr bval)
        {
            if(bval == null)
                bval = new double[3, 3];
            int nbc = bc*3;
            int nbr = br*3;
            hess[nbc+0, nbr+0] = bval[0, 0];
            hess[nbc+0, nbr+1] = bval[0, 1];
            hess[nbc+0, nbr+2] = bval[0, 2];
            hess[nbc+1, nbr+0] = bval[1, 0];
            hess[nbc+1, nbr+1] = bval[1, 1];
            hess[nbc+1, nbr+2] = bval[1, 2];
            hess[nbc+2, nbr+0] = bval[2, 0];
            hess[nbc+2, nbr+1] = bval[2, 1];
            hess[nbc+2, nbr+2] = bval[2, 2];
        }
        public override void SetBlockLock(int bc, int br, MatrixByArr bval)
        {
            lock(hess)
            {
                SetBlock(bc, br, bval);
            }
        }
        public override bool HasBlock(int bc, int br)
        {
            int nbc = bc*3;
            int nbr = br*3;
            if(hess[nbc+0, nbr+0] != 0) return true;
            if(hess[nbc+0, nbr+1] != 0) return true;
            if(hess[nbc+0, nbr+2] != 0) return true;
            if(hess[nbc+1, nbr+0] != 0) return true;
            if(hess[nbc+1, nbr+1] != 0) return true;
            if(hess[nbc+1, nbr+2] != 0) return true;
            if(hess[nbc+2, nbr+0] != 0) return true;
            if(hess[nbc+2, nbr+1] != 0) return true;
            if(hess[nbc+2, nbr+2] != 0) return true;
            return false;
        }
        public override bool HasBlockLock(int bc, int br)
        {
            lock(hess)
            {
                return HasBlock(bc, br);
            }
        }
        public override IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocks()
        {
            for(int bc=0; bc<ColBlockSize; bc++)
                for(int br=0; br<RowBlockSize; br++)
                    if(HasBlock(bc, br))
                        yield return new ValueTuple<int, int, MatrixByArr>(bc, br, GetBlock(bc, br));
        }
        //public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocksInCols_dep(int[] lstBlkCol)
        //{
        //    foreach(int bc in lstBlkCol)
        //        for(int br=0; br<RowBlockSize; br++)
        //            if(HasBlock(bc, br))
        //                yield return new Tuple<int, int, MatrixByArr>(bc, br, GetBlock(bc, br));
        //}
        public override IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocksInCols(int[] lstBlkCol)
        {
            foreach(int bc in lstBlkCol)
                for(int br = 0; br < RowBlockSize; br++)
                    if(HasBlock(bc, br))
                        yield return new ValueTuple<int, int, MatrixByArr>(bc, br, GetBlock(bc, br));
        }
        public IEnumerable<Tuple<int, int>> EnumIndicesAll()
        {
            for(int bc=0; bc<ColBlockSize; bc++)
                for(int br=0; br<RowBlockSize; br++)
                    yield return new Tuple<int, int>(bc, br);
        }
        public override IEnumerable<Tuple<int, int>> EnumIndices_dep()
        {
            for(int bc=0; bc<ColBlockSize; bc++)
                for(int br=0; br<RowBlockSize; br++)
                    if(HasBlock(bc, br))
                        yield return new Tuple<int, int>(bc, br);
        }

        public override HessMatrix Zeros(int colsize, int rowsize)
        {
            return ZerosDense(colsize, rowsize);
        }
        public static HessMatrix ZerosDense(int colsize, int rowsize)
        {
            return new HessMatrixDense
            {
                hess = Matrix.Zeros(colsize, rowsize),
            };
        }

        public override double[,] ToArray()
        {
            return hess.ToArray().Clone() as double[,];
        }

        public static HessMatrix FromMatrix(Matrix mat)
        {
            return new HessMatrixDense
            {
                hess = mat.ToArray(),
            };
        }
    }
}
