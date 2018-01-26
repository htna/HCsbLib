using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public abstract partial class HessMatrix : Matrix
    {
        public int ColBlockSize { get { HDebug.Assert(ColSize%3 == 0); return (ColSize / 3); } }
        public int RowBlockSize { get { HDebug.Assert(RowSize%3 == 0); return (RowSize / 3); } }
        public int NumAtoms
        {
            get
            {
                HDebug.Assert(ColSize == RowSize);
                HDebug.Assert(ColSize%3 == 0);
                return ColSize/3;
            }
        }
        public abstract int NumUsedBlocks { get; }
        public double RatioUsedBlocks
        {
            get
            {
                return (double)NumUsedBlocks / ((double)ColBlockSize * (double)RowBlockSize);
            }
        }

        public override Matrix     Clone() { return CloneHess(); }
        public abstract HessMatrix CloneHess();
        //public abstract IHessMatrix CloneT();
        public abstract MatrixByArr GetBlock(int bc, int br);
        public abstract MatrixByArr GetBlockLock(int bc, int br);
        public abstract void SetBlock(int bc, int br, MatrixByArr bval);
        public abstract void SetBlockLock(int bc, int br, MatrixByArr bval);
        public void SetBlock(IList<int> bcs, IList<int> brs, HessMatrix subhess)
        {
            HDebug.Assert(bcs.Count == subhess.ColBlockSize);
            HDebug.Assert(brs.Count == subhess.RowBlockSize);
            for(int ic=0; ic<bcs.Count; ic++)
                for(int ir=0; ir<brs.Count; ir++)
                {
                    int bc = bcs[ic];
                    int br = brs[ir];
                    SetBlock(bc, br, subhess.GetBlock(ic, ir));
                }
        }
        // foreach(Tuple<int, int, MatrixByArr> bc_br_bval in hess.EnumBlocks())
        public abstract bool HasBlock(int bc, int br);
        public abstract bool HasBlockLock(int bc, int br);
        public abstract IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocks_dep();
        public abstract IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocks_dep(int[] lstBlkCol);
        public abstract IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocks(int[] lstBlkCol);
        public abstract IEnumerable<Tuple<int, int>> EnumIndices_dep();
        public IEnumerable<Tuple<int, Tuple<int, int, MatrixByArr>[]>> EnumColBlocks()
        {
            Dictionary<int, List<Tuple<int, int, MatrixByArr>>> ibr_listBlock = new Dictionary<int, List<Tuple<int, int, MatrixByArr>>>();
            for(int ibr=0; ibr<RowBlockSize; ibr++)
                ibr_listBlock.Add(ibr, new List<Tuple<int, int, MatrixByArr>>());
            foreach(Tuple<int, int, MatrixByArr> bc_br_bval in EnumBlocks_dep())
            {
                //int ibc = bc_br_bval.Item1;
                int ibr = bc_br_bval.Item2;
                ibr_listBlock[ibr].Add(bc_br_bval);
            }
            for(int ibr=0; ibr<RowBlockSize; ibr++)
            {
                int[] lst_ibc = ibr_listBlock[ibr].HListItem1().ToArray();
                int[] idxsrt  = lst_ibc.HIdxSorted();
                Tuple<int, int, MatrixByArr>[] colblk = ibr_listBlock[ibr].HSelectByIndex(idxsrt);
                yield return new Tuple<int, Tuple<int, int, MatrixByArr>[]>(ibr, colblk);
            }
        }
        public IEnumerable<Tuple<int, Tuple<int, int, MatrixByArr>[]>> EnumRowBlocks()
        {
            Dictionary<int, List<Tuple<int, int, MatrixByArr>>> ibc_listBlock = new Dictionary<int, List<Tuple<int, int, MatrixByArr>>>();
            for(int ibc=0; ibc<ColBlockSize; ibc++)
                ibc_listBlock.Add(ibc, new List<Tuple<int, int, MatrixByArr>>());
            foreach(Tuple<int, int, MatrixByArr> bc_br_bval in EnumBlocks_dep())
            {
                int ibc = bc_br_bval.Item1;
                //int ibr = bc_br_bval.Item2;
                ibc_listBlock[ibc].Add(bc_br_bval);
            }
            for(int ibc=0; ibc<ColBlockSize; ibc++)
            {
                int[] lst_ibc = ibc_listBlock[ibc].HListItem1().ToArray();
                int[] idxsrt  = lst_ibc.HIdxSorted();
                Tuple<int, int, MatrixByArr>[] rowblk = ibc_listBlock[ibc].HSelectByIndex(idxsrt);
                yield return new Tuple<int, Tuple<int, int, MatrixByArr>[]>(ibc, rowblk);
            }
        }

        public new abstract HessMatrix Zeros(int colsize, int rowsize);

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // implicit conversion
        public static implicit operator HessMatrix(double[,] hess)
        {
            return HessMatrixDense.FromMatrix(hess);
        }

        public MatrixSparse<MatrixByArr> GetMatrixSparse()
        {
            if(this is HessMatrixSparse) return (this as HessMatrixSparse).GetMatrixSparse();
            if(this is HessMatrixDense ) return HessMatrixSparse.FromMatrix(this).GetMatrixSparse();
            throw new NotImplementedException();
        }
    }
}
