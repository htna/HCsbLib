﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public abstract partial class IHessMatrix : IMatrix<double>
    {
        ////////////////////////////////////////////////////////////////////
        // IMatrix<double>
        public abstract int ColSize { get; }
        public abstract int RowSize { get; }
        public abstract double this[int  c, int r] { get; set; }
        public abstract double this[long c, long r] { get; set; }
        public abstract double[,] ToArray();
        // IMatrix<double>
        ////////////////////////////////////////////////////////////////////

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

        public abstract MatrixByArr GetBlock(int bc, int br);
        public abstract MatrixByArr GetBlockLock(int bc, int br);
        public abstract void SetBlock(int bc, int br, MatrixByArr bval);
        public abstract void SetBlockLock(int bc, int br, MatrixByArr bval);
        public void SetBlock(IList<int> bcs, IList<int> brs, IHessMatrix subhess)
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
        public abstract IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocks();
        public abstract IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocksInCols(int[] lstBlkCol);
        public IEnumerable<Tuple<int, Tuple<int, int, MatrixByArr>[]>> EnumColBlocksAll()
        {
            Dictionary<int, List<Tuple<int, int, MatrixByArr>>> ibr_listBlock = new Dictionary<int, List<Tuple<int, int, MatrixByArr>>>();
            for(int ibr=0; ibr<RowBlockSize; ibr++)
                ibr_listBlock.Add(ibr, new List<Tuple<int, int, MatrixByArr>>());
            foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in EnumBlocks())
            {
                //int ibc = bc_br_bval.Item1;
                int ibr = bc_br_bval.Item2;
                ibr_listBlock[ibr].Add(bc_br_bval.ToTuple());
            }
            for(int ibr=0; ibr<RowBlockSize; ibr++)
            {
                int[] lst_ibc = ibr_listBlock[ibr].HListItem1().ToArray();
                int[] idxsrt  = lst_ibc.HIdxSorted();
                Tuple<int, int, MatrixByArr>[] colblk = ibr_listBlock[ibr].HSelectByIndex(idxsrt);
                yield return new Tuple<int, Tuple<int, int, MatrixByArr>[]>(ibr, colblk);
            }
        }
        public IEnumerable<Tuple<int, Tuple<int, int, MatrixByArr>[]>> EnumRowBlocksAll()
        {
            Dictionary<int, List<Tuple<int, int, MatrixByArr>>> ibc_listBlock = new Dictionary<int, List<Tuple<int, int, MatrixByArr>>>();
            for(int ibc=0; ibc<ColBlockSize; ibc++)
                ibc_listBlock.Add(ibc, new List<Tuple<int, int, MatrixByArr>>());
            foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in EnumBlocks())
            {
                int ibc = bc_br_bval.Item1;
                //int ibr = bc_br_bval.Item2;
                ibc_listBlock[ibc].Add(bc_br_bval.ToTuple());
            }
            for(int ibc=0; ibc<ColBlockSize; ibc++)
            {
                int[] lst_ibc = ibc_listBlock[ibc].HListItem1().ToArray();
                int[] idxsrt  = lst_ibc.HIdxSorted();
                Tuple<int, int, MatrixByArr>[] rowblk = ibc_listBlock[ibc].HSelectByIndex(idxsrt);
                yield return new Tuple<int, Tuple<int, int, MatrixByArr>[]>(ibc, rowblk);
            }
        }
    }
}
