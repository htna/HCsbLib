using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class HessMatrixSparse : HessMatrix
    {
        MatrixBySparseMatrix hess;
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix

        public override int ColSize       { get { return hess.ColSize; } }
        public override int RowSize       { get { return hess.RowSize; } }
        public override int NumUsedBlocks { get { return hess.NumUsedBlocks; } }
        //public override double RatioUsedBlocks { get {
        //    return hess.RatioUsedBlocks;
        //}}

        public override double this[int c, int r]
        {
            get { return hess.GetValue(c, r); }
            set { hess.SetValue(c, r, value); }
        }
        public override double this[long c, long r]
        {
            get { return hess.GetValue((int)c, (int)r); }
            set { hess.SetValue((int)c, (int)r, value); }
        }

        public override HessMatrix CloneHess()
        {
            return CloneT();
        }

        //public static implicit operator MatrixByArr(HessMatrix hess)
        //{
        //    return hess.ToArray();
        //}
        public override double[,] ToArray()
        {
            double[,] arr = new double[ColSize, RowSize];
            foreach(Tuple<int, int, MatrixByArr> bc_br_bval in EnumBlocks_dep())
            {
                int bc   = bc_br_bval.Item1;
                int br   = bc_br_bval.Item2;
                var bval = bc_br_bval.Item3;
                arr[bc*3+0, br*3+0] = bval[0, 0];
                arr[bc*3+0, br*3+1] = bval[0, 1];
                arr[bc*3+0, br*3+2] = bval[0, 2];
                arr[bc*3+1, br*3+0] = bval[1, 0];
                arr[bc*3+1, br*3+1] = bval[1, 1];
                arr[bc*3+1, br*3+2] = bval[1, 2];
                arr[bc*3+2, br*3+0] = bval[2, 0];
                arr[bc*3+2, br*3+1] = bval[2, 1];
                arr[bc*3+2, br*3+2] = bval[2, 2];
            }
            return arr;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // HessMatrix
        HessMatrixSparse(int colsize, int rowsize)
        {
            HDebug.Assert(colsize % 3 == 0);
            HDebug.Assert(rowsize % 3 == 0);
            this.hess = new MatrixBySparseMatrix(colsize, rowsize, 3);
        }
        HessMatrixSparse(MatrixBySparseMatrix hess)
        {
            this.hess = hess;
        }
        public new MatrixSparse<MatrixByArr> GetMatrixSparse()
        {
            return hess.GetMatrixSparse();
        }

        public HessMatrixSparse CloneT()
        {
            return new HessMatrixSparse(hess.CloneT());
        }

        public override MatrixByArr GetBlock(int bc, int br)
        {
            return hess.GetBlock(bc, br);
        }
        public override MatrixByArr GetBlockLock(int bc, int br)
        {
            return hess.GetBlockLock(bc, br);
        }
        public override void SetBlock(int bc, int br, MatrixByArr bval)
        {
            if(bval != null && bval.IsZero())
                bval = null;
            hess.SetBlock(bc, br, bval);
        }
        public override void SetBlockLock(int bc, int br, MatrixByArr bval)
        {
            if(bval != null && bval.IsZero())
                bval = null;
            hess.SetBlockLock(bc, br, bval);
        }
        //public void SetBlock(IList<int> bcs, IList<int> brs, HessMatrix subhess)
        //{
        //    HDebug.Assert(bcs.Count == subhess.ColBlockSize);
        //    HDebug.Assert(brs.Count == subhess.RowBlockSize);
        //    for(int ic=0; ic<bcs.Count; ic++)
        //        for(int ir=0; ir<brs.Count; ir++)
        //        {
        //            int bc = bcs[ic];
        //            int br = brs[ir];
        //            SetBlock(bc, br, subhess.GetBlock(ic, ir));
        //        }
        //}
        public override bool HasBlock(int bc, int br)
        {
            return hess.HasBlock(bc, br);
        }
        public override bool HasBlockLock(int bc, int br)
        {
            return hess.HasBlockLock(bc, br);
        }
        public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocks_dep()
        {
            return hess.EnumBlocks();
        }
        //public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocksInCols_dep(int[] lstBlkCol)
        //{
        //    return hess.EnumBlocks_dep(lstBlkCol);
        //}
        public override IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocksInCols(int[] lstBlkCol)
        {
            return hess.EnumBlocks(lstBlkCol);
        }
        public override IEnumerable<Tuple<int, int>> EnumIndices_dep()
        {
            return hess.EnumIndices();
        }

        public override HessMatrix Zeros(int colsize, int rowsize)
        {
            return ZerosSparse(colsize, rowsize);
        }
        public static bool ZerosSparse_selftest = HDebug.IsDebuggerAttached;
        public static HessMatrixSparse ZerosSparse(int colsize, int rowsize)
        {
            if(ZerosSparse_selftest)
            {
                ZerosSparse_selftest = false;
                HessMatrixSparse tmat = ZerosSparse(300, 300);
                bool bzero = true;
                for(int c=0; c<tmat.ColSize; c++)
                    for(int r=0; r<tmat.ColSize; r++)
                        if(tmat[c, r] != 0)
                            bzero = false;
                HDebug.Assert(bzero);
            }

            HessMatrixSparse mat = new HessMatrixSparse(colsize, rowsize);
            return mat;
        }
        public static HessMatrixSparse FromMatrix(Matrix mat, bool parallel=false)
        {
            if(mat is HessMatrixSparse)
                return (mat as HessMatrixSparse).CloneT();

            HDebug.Assert(mat.ColSize % 3 == 0);
            HDebug.Assert(mat.RowSize % 3 == 0);
            HessMatrixSparse hess = ZerosSparse(mat.ColSize, mat.RowSize);

            {
                HessMatrixDense dense = new HessMatrixDense { hess = mat };

                Action<Tuple<int, int>> func = delegate(Tuple<int, int> bc_br)
                {
                    int bc   = bc_br.Item1;
                    int br   = bc_br.Item2;
                    if(dense.HasBlock(bc, br) == false)
                        return;

                    var bval = dense.GetBlock(bc, br);
                    lock(hess)
                        hess.SetBlock(bc, br, bval);
                };
                if(parallel)    Parallel.ForEach(         dense.EnumIndicesAll(), func           );
                else            foreach(var bc_br_bval in dense.EnumIndicesAll()) func(bc_br_bval);
                
            }

            if(HDebug.IsDebuggerAttached)
            {
                // old version
                HessMatrixSparse dhess = ZerosSparse(mat.ColSize, mat.RowSize);
                int colsize = mat.ColSize;
                int rowsize = mat.RowSize;
                for(int c=0; c<colsize; c++)
                    for(int r=0; r<rowsize; r++)
                        if(mat[c, r] != 0)
                            dhess[c, r] = mat[c, r];
                HDebug.Assert(HessMatrixSparseEqual(hess, dhess));

                if(HDebug.IsDebuggerAttached && ((mat.ColSize/3) < 3000))
                {
                    double maxAbsDiff = mat.HAbsMaxDiffWith(hess);
                    HDebug.Assert(maxAbsDiff == 0);
                }
            }

            return hess;
        }
    }
}
