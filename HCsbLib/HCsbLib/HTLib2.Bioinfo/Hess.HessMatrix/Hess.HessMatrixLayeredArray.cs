using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class HessMatrixLayeredArray : HessMatrix
    {
        List<double    [,]>     diag;
        List<double[][][,]>  offdiag;
        List<int   []     >  offdiag_count;
        int colblksize;
        int rowblksize;
        int layersize;
        int numusedblocks_offdiag;

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix
        public override int ColSize       { get { return colblksize * 3; } }
        public override int RowSize       { get { return rowblksize * 3; } }
        public override int NumUsedBlocks
        {
            get
            {
                return numusedblocks_offdiag + diag.Count;
            }
        }
        //public override double RatioUsedBlocks { get {
        //    return hess.RatioUsedBlocks;
        //}}

        public override double this[int c, int r]
        {
            get { return GetValue(c, r); }
            set {        SetValue(c, r, value, true); }
        }
        public override double this[long c, long r]
        {
            get { return GetValue((int)c, (int)r); }
            set {        SetValue((int)c, (int)r, value, true); }
        }
        public double GetValue(int c, int r)
        {
            int ic = c % 3; int bc = c / 3;
            int ir = r % 3; int br = r / 3;
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;

            double[,] blk = _GetBlock(bc, br, br1, br2);
            if(blk == null)
                return 0;
            return blk[ic, ir];
        }
        public void SetValue(int c, int r, double value, bool dispose_zeroblock)
        {
            int ic = c % 3; int bc = c / 3;
            int ir = r % 3; int br = r / 3;
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;

            double[,] blk = _GetBlock(bc, br, br1, br2);
            if(value == 0)
            {
                if(blk == null)
                    return;
                else if(bc == br)
                    blk[ic, ir] = 0;
                else
                {
                    blk[ic, ir] = 0;
                    
                    if(dispose_zeroblock)
                    {
                        bool zeros = (blk[0, 0] == 0 && blk[0, 1] == 0 && blk[0, 2] == 0 &&
                                      blk[1, 0] == 0 && blk[1, 1] == 0 && blk[1, 2] == 0 &&
                                      blk[2, 0] == 0 && blk[2, 1] == 0 && blk[2, 2] == 0);
                        if(zeros)
                            _SetBlockNull(bc, br, br1, br2);
                    }
                }
            }
            else
            {
                if(blk != null)
                    blk[ic, ir] = value;
                else
                {
                    blk = new double[3,3];
                    blk[ic, ir] = value;
                    _SetBlock(bc, br, br1, br2, blk);
                }
            }
        }
        public override MatrixByArr GetBlock(int bc, int br)
        {
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;
            double[,] blk = _GetBlock(bc, br, br1, br2);
            if(blk == null)
                return null;
            return new MatrixByArr(blk);
        }
        public override MatrixByArr GetBlockLock(int bc, int br)
        {
            throw new NotImplementedException();
            //return hess.GetBlockLock(bc, br);
        }
        public override void SetBlock(int bc, int br, MatrixByArr bval)
        {
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;
            _SetBlock(bc, br, br1, br2, bval.ToArray());
        }
        public override void SetBlockLock(int bc, int br, MatrixByArr bval)
        {
            throw new NotImplementedException();
            //if(bval != null && bval.IsZero())
            //    bval = null;
            //hess.SetBlockLock(bc, br, bval);
        }
        public double[,] _GetBlock(int bc, int br, int br1, int br2)
        {
            if(bc == br)
            {
                double[,] diag_bc = diag[bc];
                return diag_bc;
            }
            else
            {
                HDebug.Assert(br2 == br % layersize        );
                HDebug.Assert(br1 == (br - br2) / layersize);
                double[][][,] offdiag_bc     = offdiag[bc];
                double[][,]   offdiag_bc_br1 = offdiag_bc[br1];

                if(offdiag_bc_br1 == null)
                    return null;

                double[,]   offdiag_bc_br1br2 = offdiag_bc_br1[br2];
                if(offdiag_bc_br1br2 == null)
                    return null;

                return offdiag_bc_br1br2;
            }
        }
        public void _SetBlockNull(int bc, int br, int br1, int br2)
        {
            if(bc == br)
            {
                double[,] diag_bc = diag[bc];
                diag_bc[0, 0] = 0; diag_bc[0, 1] = 0; diag_bc[0, 2] = 0;
                diag_bc[1, 0] = 0; diag_bc[1, 1] = 0; diag_bc[1, 2] = 0;
                diag_bc[2, 0] = 0; diag_bc[2, 1] = 0; diag_bc[2, 2] = 0;
            }
            else
            {
                HDebug.Assert(br2 == br % layersize        );
                HDebug.Assert(br1 == (br - br2) / layersize);
                double[][,]   offdiag_bc_br1 = offdiag[bc][br1];

                if(offdiag_bc_br1 == null)
                    return;

                double[,] offdiag_bc_br1br2 = offdiag_bc_br1[br2];
                if(offdiag_bc_br1br2 == null)
                    return;

                offdiag_bc_br1[br2] = null;
                offdiag_count[bc][br1] --;
                numusedblocks_offdiag --;
            }
        }
        public void _SetBlock(int bc, int br, int br1, int br2, double[,] blk)
        {
            if(blk == null)
            {
                _SetBlockNull(bc, br, br1, br2);
                return;
            }

            if(bc == br)
            {
                diag[bc] = blk;
            }
            else
            {
                HDebug.Assert(br2 == br % layersize        );
                HDebug.Assert(br1 == (br - br2) / layersize);
                double[][][,] offdiag_bc     = offdiag[bc];
                double[][,]   offdiag_bc_br1 = offdiag_bc[br1];

                if(offdiag_bc_br1 == null)
                {
                    HDebug.Assert(offdiag_count[bc][br1] == 0);
                    int br2_size = layersize; // rowblksize % layersize+1;
                    offdiag_bc_br1 = new double[br2_size][,];
                    offdiag_bc[br1] = offdiag_bc_br1;
                    offdiag_count[bc][br1] = 0;
                }

                if(offdiag_bc_br1[br2] == null)
                {
                    offdiag_count[bc][br1] ++;
                    numusedblocks_offdiag ++;
                }
                offdiag_bc_br1[br2] = blk;
            }
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
        public static void SelfTest()
        {
            if(HDebug.Selftest())
            {
                Random rand = new Random(0);
                int colblksize = 3;
                int rowblksize = 10;
                int layersize = 3;
                var  mat = Matrix.Zeros              (colblksize*3,rowblksize*3);
                var hess = new HessMatrixLayeredArray(colblksize*3,rowblksize*3,layersize);
                HDebug.Assert(mat.ColSize == hess.ColSize);
                HDebug.Assert(mat.RowSize == hess.RowSize);
                int count = mat.ColSize*mat.RowSize*10;
                for(int i=0; i<count; i++)
                {
                    int c = rand.NextInt(0, colblksize*3-1);
                    int r = rand.NextInt(0, rowblksize*3-1);
                    double v = rand.NextDouble();
                    mat[c, r] = v;
                    hess[c, r] = v;
                    HDebug.AssertTolerance(double.Epsilon, (mat - hess).ToArray());
                }
                for(int i = 0; i < 700; i++)
                {
                    int c = rand.NextInt(0, colblksize*3-1);
                    int r = rand.NextInt(0, rowblksize*3-1);
                    double v = 0;
                    mat[c, r] = v;
                    hess[c, r] = v;
                    HDebug.AssertTolerance(double.Epsilon, (mat - hess).ToArray());
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // HessMatrix
        HessMatrixLayeredArray(int colsize, int rowsize, int layersize)
        {
            if(HDebug.Selftest())
                SelfTest();

            HDebug.Assert(colsize % 3 == 0);
            HDebug.Assert(rowsize % 3 == 0);
            this.colblksize = colsize / 3;
            this.rowblksize = rowsize / 3;
            this.layersize  = layersize;
            this.numusedblocks_offdiag = 0;

            int br2_size =  rowblksize % layersize;
            int br1_size = (rowblksize - br2_size) / layersize + 1;

               diag       = new List<double    [,]>(colblksize);
            offdiag       = new List<double[][][,]>(colblksize);
            offdiag_count = new List<int   []     >(colblksize);
            for(int bc=0; bc<colblksize; bc++)
            {
                   diag      .Add(new double          [3,3]);
                offdiag      .Add(new double[br1_size][][,]);
                offdiag_count.Add(new int   [br1_size]     );
                for(int br1=0; br1<br1_size; br1++)
                {
                    offdiag      [bc][br1] = null;
                    offdiag_count[bc][br1] = 0;
                }
            }
        }

        public HessMatrixLayeredArray CloneT()
        {
            throw new NotImplementedException();
            //return new HessMatrixLayeredArray(hess.CloneT());
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
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;
            return (_GetBlock(bc, br, br1, br2) != null);
        }
        public override bool HasBlockLock(int bc, int br)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<ValueTuple<int, int, double[,]>> _EnumBlocks()
        {
            for(int bc=0; bc<colblksize; bc++)
            {
                yield return new ValueTuple<int, int, double[,]>(bc, bc, diag[bc]);
                double[][][,] offdiag_bc = offdiag[bc];
                for(int br1=0; br1<offdiag_bc.Length; br1++)
                {
                    double[][,] offdiag_bc_br1 = offdiag_bc[br1];
                    if(offdiag_bc_br1 == null)
                        continue;
                    for(int br2=0; br2<offdiag_bc_br1.Length; br2++)
                    {
                        double[,] offdiag_bc_br1br2 = offdiag_bc_br1[br2];
                        if(offdiag_bc_br1br2 != null)
                        {
                            int br = br1 * layersize + br2;
                            yield return new ValueTuple<int, int, double[,]>(bc, bc, offdiag_bc_br1br2);
                        }
                    }
                }
            }
        }
        public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocks_dep()
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocks_dep(params int[] lstBlkCol)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocks(params int[] lstBlkCol)
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
        public static HessMatrixLayeredArray ZerosBlkMat(int colsize, int rowsize)
        {
            int layersize = 64;
            return ZerosBlkMat(colsize, rowsize, layersize);
        }
        public static HessMatrixLayeredArray ZerosBlkMat(int colsize, int rowsize, int layersize)
        {
            if(ZerosSparse_selftest)
            {
                ZerosSparse_selftest = false;
                HessMatrixLayeredArray tmat = ZerosBlkMat(300, 300, layersize);
                bool bzero = true;
                for(int c=0; c<tmat.ColSize; c++)
                    for(int r=0; r<tmat.ColSize; r++)
                        if(tmat[c, r] != 0)
                            bzero = false;
                HDebug.Assert(bzero);
            }

            HessMatrixLayeredArray mat = new HessMatrixLayeredArray(colsize, rowsize, layersize);
            return mat;
        }
        //public static HessMatrixLayeredArray FromMatrix(Matrix mat, bool parallel=false)
        //{
        //    if(mat is HessMatrixLayeredArray)
        //        return (mat as HessMatrixLayeredArray).CloneT();
        //
        //    HDebug.Assert(mat.ColSize % 3 == 0);
        //    HDebug.Assert(mat.RowSize % 3 == 0);
        //    HessMatrixLayeredArray hess = ZerosSparse(mat.ColSize, mat.RowSize);
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
        //        HessMatrixLayeredArray dhess = ZerosSparse(mat.ColSize, mat.RowSize);
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
