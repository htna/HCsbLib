using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class HessMatrixBlkMat : HessMatrix
    {
        MatrixByRowVec<double[,]> hess;

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix
        public override int ColSize       { get { return (int)hess.ColSize * 3; } }
        public override int RowSize       { get { return (int)hess.RowSize * 3; } }
        public override int NumUsedBlocks
        {
            get
            {
                int count = 0;
                for(int c = 0; c < hess.ColSize; c++)
                {
                    double[][,] hess_c = hess.GetRowVector(c);
                    HDebug.Assert(hess_c.Length == hess.RowSize);
                    for(int r = 0; r < hess.RowSize; r++)
                    {
                        if(hess_c[r] != null)
                            count++;
                    }
                }
                return count;
            }
        }
        //public override double RatioUsedBlocks { get {
        //    return hess.RatioUsedBlocks;
        //}}

        public override double this[int c, int r]
        {
            get { return GetValue((int)c, (int)r); }
            set {        SetValue((int)c, (int)r, value); }
        }
        public override double this[long c, long r]
        {
            get { return GetValue(c, r); }
            set {        SetValue(c, r, value); }
        }
        public double GetValue(long c, long r)
        {
            long bc = c / 3; long ic = c % 3;
            long br = r / 3; long ir = r % 3;
            return hess[bc,br][ic, ir];
        }
        public void SetValue(long c, long r, double value)
        {
            long bc = c / 3; long ic = c % 3;
            long br = r / 3; long ir = r % 3;
            double[,] hess_bcbr = hess[bc, br];
            if(hess_bcbr == null)
            {
                hess_bcbr = new double[3, 3];
                hess[bc, br] = hess_bcbr;
            }
            hess_bcbr[ic, ir] = value;
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
            foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in _EnumBlocks())
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
        HessMatrixBlkMat(int colsize, int rowsize)
        {
            HDebug.Assert(colsize % 3 == 0);
            HDebug.Assert(rowsize % 3 == 0);
            hess = new MatrixByRowVec<double[,]>(colsize / 3, rowsize / 3);
        }

        public HessMatrixBlkMat CloneT()
        {
            throw new NotImplementedException();
            //return new HessMatrixBlkMat(hess.CloneT());
        }

        public override MatrixByArr GetBlock(int bc, int br)
        {
            return hess[bc, br];
        }
        public override MatrixByArr GetBlockLock(int bc, int br)
        {
            throw new NotImplementedException();
            //return hess.GetBlockLock(bc, br);
        }
        public override void SetBlock(int bc, int br, MatrixByArr bval)
        {
            hess[bc, br] = bval;
        }
        public override void SetBlockLock(int bc, int br, MatrixByArr bval)
        {
            throw new NotImplementedException();
            //if(bval != null && bval.IsZero())
            //    bval = null;
            //hess.SetBlockLock(bc, br, bval);
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
            return (hess[bc, br] != null);
        }
        public override bool HasBlockLock(int bc, int br)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<ValueTuple<int, int, double[,]>> _EnumBlocks()
        {
            for(int c=0; c<hess.ColSize; c++)
            {
                double[][,] hess_c = hess.GetRowVector(c);
                for(int r = 0; r < hess_c.Length; r++)
                {
                    double[,] hess_cr = hess_c[r];
                    if(hess_cr != null)
                        yield return new ValueTuple<int, int, double[,]>(c, r, hess_cr);
                }
            }
        }
        public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocks_dep()
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocksInCols_dep(int[] lstBlkCol)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocks(int[] lstBlkCol)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<Tuple<int, int>> EnumIndices_dep()
        {
            throw new NotImplementedException();
        }

        public override HessMatrix Zeros(int colsize, int rowsize)
        {
            return ZerosBlkMat(colsize, rowsize);
        }
        public static bool ZerosSparse_selftest = HDebug.IsDebuggerAttached;
        public static HessMatrixBlkMat ZerosBlkMat(int colsize, int rowsize)
        {
            if(ZerosSparse_selftest)
            {
                ZerosSparse_selftest = false;
                HessMatrixBlkMat tmat = ZerosBlkMat(300, 300);
                bool bzero = true;
                for(int c=0; c<tmat.ColSize; c++)
                    for(int r=0; r<tmat.ColSize; r++)
                        if(tmat[c, r] != 0)
                            bzero = false;
                HDebug.Assert(bzero);
            }

            HessMatrixBlkMat mat = new HessMatrixBlkMat(colsize, rowsize);
            return mat;
        }
        //public static HessMatrixBlkMat FromMatrix(Matrix mat, bool parallel=false)
        //{
        //    if(mat is HessMatrixBlkMat)
        //        return (mat as HessMatrixBlkMat).CloneT();
        //
        //    HDebug.Assert(mat.ColSize % 3 == 0);
        //    HDebug.Assert(mat.RowSize % 3 == 0);
        //    HessMatrixBlkMat hess = ZerosSparse(mat.ColSize, mat.RowSize);
        //
        //    {
        //        HessMatrixDense dense = new HessMatrixDense { hess = mat };
        //
        //        Action<Tuple<int, int>> func = delegate(Tuple<int, int> bc_br)
        //        {
        //            int bc   = bc_br.Item1;
        //            int br   = bc_br.Item2;
        //            if(dense.HasBlock(bc, br) == false)
        //                return;
        //
        //            var bval = dense.GetBlock(bc, br);
        //            lock(hess)
        //                hess.SetBlock(bc, br, bval);
        //        };
        //        if(parallel)    Parallel.ForEach(         dense.EnumIndicesAll(), func           );
        //        else            foreach(var bc_br_bval in dense.EnumIndicesAll()) func(bc_br_bval);
        //        
        //    }
        //
        //    if(HDebug.IsDebuggerAttached)
        //    {
        //        // old version
        //        HessMatrixBlkMat dhess = ZerosSparse(mat.ColSize, mat.RowSize);
        //        int colsize = mat.ColSize;
        //        int rowsize = mat.RowSize;
        //        for(int c=0; c<colsize; c++)
        //            for(int r=0; r<rowsize; r++)
        //                if(mat[c, r] != 0)
        //                    dhess[c, r] = mat[c, r];
        //        HDebug.Assert(HessMatrixSparseEqual(hess, dhess));
        //
        //        if(HDebug.IsDebuggerAttached && ((mat.ColSize/3) < 3000))
        //        {
        //            double maxAbsDiff = mat.HAbsMaxDiffWith(hess);
        //            HDebug.Assert(maxAbsDiff == 0);
        //        }
        //    }
        //
        //    return hess;
        //}
    }
}
