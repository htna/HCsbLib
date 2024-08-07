﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class HessMatrix : IHessMatrix, ISerializable
        , IMatrixSparse<double>, IBinarySerializable
    {
        List<double    [,]>     diag;
        List<double[][][,]>  offdiag;
        List<int   []     >  offdiag_count;
        int colblksize;
        int rowblksize;
        int layersize;
        int numusedblocks_offdiag
        {
            get
            {
                int count = 0;
                foreach(var items in offdiag_count)
                    foreach(var item in items)
                        count += item;
                return count;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Operators
        //public static HessMatrix operator-(HessMatrix left               ) { HessMatrix mat = left.CloneHess(); mat.UpdateMul(-1      ); return mat; }
        //public static HessMatrix operator+(HessMatrix left, IMatrix right) { HessMatrix mat = left.CloneHess(); mat.UpdateAdd(right, 1); return mat; }
        //public static HessMatrix operator-(HessMatrix left, IMatrix right) { HessMatrix mat = left.CloneHess(); mat.UpdateAdd(right,-1); return mat; }
        //public static HessMatrix operator*(HessMatrix left, IMatrix right) { HessMatrix mat = HessMatrix.GetMul(left, right);            return mat; }
        public static HessMatrix operator* (HessMatrix left, HessMatrix  right) {HessMatrix mat = HessMatrixStatic.GetMulImpl(null, true, left, right); return mat; }
        public static HessMatrix operator* (HessMatrix left, double      right) { HessMatrix mat = left .CloneHessMatrix(); mat.UpdateMul(right  );     return mat; }
        public static HessMatrix operator* (double     left, HessMatrix  right) { HessMatrix mat = right.CloneHessMatrix(); mat.UpdateMul(left   );     return mat; }
        public static HessMatrix operator/ (HessMatrix left, double      right) { HessMatrix mat = left .CloneHessMatrix(); mat.UpdateMul(1/right);     return mat; }

        public static HessMatrix operator+(HessMatrix left, HessMatrix right) { HessMatrix mat = left.CloneHessMatrix(); mat.UpdateAdd(right,  1, null, 0); return mat; }
        public static HessMatrix operator-(HessMatrix left, HessMatrix right) { HessMatrix mat = left.CloneHessMatrix(); mat.UpdateAdd(right, -1, null, 0); return mat; }
      //public static bool      operator==(HessMatrix left, HessMatrix right) { return HessMatrixStatic.HessMatrixEqual(left, right); }
      //public static bool      operator!=(HessMatrix left, HessMatrix right) { return HessMatrixStatic.HessMatrixEqual(left, right) == false; }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix
        public override int ColSize       { get { return colblksize * 3; } }    //public int NumRows { get { return ColSize; } }
        public override int RowSize       { get { return rowblksize * 3; } }    //public int NumCols { get { return RowSize; } }
        public override int NumUsedBlocks
        {
            get
            {
                int num = numusedblocks_offdiag;
                foreach(var diagi in diag)
                    if(diagi != null)
                        num ++;
                return num;
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
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;

            double[][][,] offdiag_bc     = offdiag[bc];
            double[][,]   offdiag_bc_br1 = offdiag[bc][br1];

            if(offdiag_bc_br1 != null)  lock(offdiag_bc_br1) return _GetBlock(bc, br, br1, br2);
            else                        lock(offdiag_bc    ) return _GetBlock(bc, br, br1, br2);
        }
        public override void SetBlock(int bc, int br, MatrixByArr bval)
        {
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;
            double[,] blk = null;
            if(bval != null)
                blk = bval.ToArray();
            _SetBlock(bc, br, br1, br2, blk);
        }
        public override void SetBlockLock(int bc, int br, MatrixByArr bval)
        {
            int br2 =  br        % layersize;
            int br1 = (br - br2) / layersize;
            double[,] blk = null;
            if(bval != null)
                blk = bval.ToArray();

            double[][][,] offdiag_bc     = offdiag[bc];
            double[][,]   offdiag_bc_br1 = offdiag[bc][br1];

            if(offdiag_bc_br1 != null)  lock(offdiag_bc_br1)    _SetBlock(bc, br, br1, br2, blk);
            else                        lock(offdiag_bc    )    _SetBlock(bc, br, br1, br2, blk);
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
                diag[bc] = null;
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
                offdiag_count[bc][br1]--;
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
                }
                offdiag_bc_br1[br2] = blk;
            }
        }

        //public static implicit operator MatrixByArr(HessMatrix hess)
        //{
        //    return hess.ToArray();
        //}
        public override double[,] ToArray()
        {
            double[,] arr = new double[ColSize, RowSize];
            foreach(ValueTuple<int, int, double[,]> bc_br_bval in _EnumBlocks())
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
        public Matrix ToMatrix()
        {
            Matrix mat = Matrix.Zeros(ColSize, RowSize);
            foreach(ValueTuple<int, int, double[,]> bc_br_bval in _EnumBlocks())
            {
                int bc3  = bc_br_bval.Item1 * 3;
                int br3  = bc_br_bval.Item2 * 3;
                var bval = bc_br_bval.Item3;
                mat[bc3+0, br3+0] = bval[0, 0];
                mat[bc3+0, br3+1] = bval[0, 1];
                mat[bc3+0, br3+2] = bval[0, 2];
                mat[bc3+1, br3+0] = bval[1, 0];
                mat[bc3+1, br3+1] = bval[1, 1];
                mat[bc3+1, br3+2] = bval[1, 2];
                mat[bc3+2, br3+0] = bval[2, 0];
                mat[bc3+2, br3+1] = bval[2, 1];
                mat[bc3+2, br3+2] = bval[2, 2];
            }
            return mat;
        }
        public static void SelfTest()
        {
            if(HDebug.Selftest())
            {
                Random rand = new Random(0);
                int colblksize = 3;
                int rowblksize = 10;
                int layersize = 3;
                var  mat = Matrix.Zeros  (colblksize*3,rowblksize*3);
                var hess = new HessMatrix(colblksize*3,rowblksize*3,layersize);
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
        HessMatrix(int colsize, int rowsize, int layersize)
        {
            if(HDebug.Selftest())
                SelfTest();

            HDebug.Assert(colsize % 3 == 0);
            HDebug.Assert(rowsize % 3 == 0);
            this.colblksize = colsize / 3;
            this.rowblksize = rowsize / 3;
            this.layersize  = layersize;

            int br2_size =  rowblksize % layersize;
            int br1_size = (rowblksize - br2_size) / layersize + 1;

               diag       = new List<double    [,]>(colblksize);
            offdiag       = new List<double[][][,]>(colblksize);
            offdiag_count = new List<int   []     >(colblksize);
            for(int bc=0; bc<colblksize; bc++)
            {
                   diag      .Add(null                     );
                offdiag      .Add(new double[br1_size][][,]);
                offdiag_count.Add(new int   [br1_size]     );
                for(int br1=0; br1<br1_size; br1++)
                {
                    offdiag      [bc][br1] = null;
                    offdiag_count[bc][br1] = 0;
                }
            }
        }

        public HessMatrix CloneHessMatrix()
        {
            HessMatrix clone = new HessMatrix(ColSize, RowSize, layersize);
            foreach(var bc_br_bval in EnumBlocks())
            {
                int bc   = bc_br_bval.Item1;
                int br   = bc_br_bval.Item2;
                var bval = bc_br_bval.Item3;

                clone.SetBlock(bc, br, bval.CloneT());
            }
            return clone;
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
            return (GetBlockLock(bc, br) != null);
        }
        public IEnumerable<ValueTuple<int, int, double[,]>> _EnumBlocks()
        {
            for(int bc=0; bc<colblksize; bc++)
            {
                foreach(var blk in _EnumBlocksInCol(bc))
                    yield return blk;
            }
        }
        public IEnumerable<ValueTuple<int, int, double[,]>> _EnumBlocksInCol(int bc)
        {
            if(diag[bc] != null)
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
                        yield return new ValueTuple<int, int, double[,]>(bc, br, offdiag_bc_br1br2);
                    }
                }
            }
        }
        public override IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocks()
        {
            foreach(var blk in _EnumBlocks())
                yield return new ValueTuple<int, int, MatrixByArr>(blk.Item1, blk.Item2, blk.Item3);
        }
        //public override IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocksInCols_dep(int[] lstBlkCol)
        //{
        //    throw new NotImplementedException();
        //}
        public override IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocksInCols(int[] lstBlkCol)
        {
            foreach(int bc in lstBlkCol)
            {
                foreach(var blk in _EnumBlocksInCol(bc))
                    yield return blk;
            }
        }
        //////////////////////////////////////////////////////////////
        // IMatrixSparse<double>
        public IEnumerable<ValueTuple<int, int, double>> EnumElements()
        {
            foreach(var blk in EnumBlocks())
            {
                int         bc3 = blk.Item1 * 3;
                int         br3 = blk.Item2 * 3;
                MatrixByArr bv  = blk.Item3;
                yield return (bc3+0, br3+0, bv[0,0]);
                yield return (bc3+0, br3+1, bv[0,1]);
                yield return (bc3+0, br3+2, bv[0,2]);
                yield return (bc3+1, br3+0, bv[1,0]);
                yield return (bc3+1, br3+1, bv[1,1]);
                yield return (bc3+1, br3+2, bv[1,2]);
                yield return (bc3+2, br3+0, bv[2,0]);
                yield return (bc3+2, br3+1, bv[2,1]);
                yield return (bc3+2, br3+2, bv[2,2]);
            }
        }
        // IMatrixSparse<double>
        //////////////////////////////////////////////////////////////
        public IEnumerable<double> EnumNonZeroValues()
        {
            foreach(var blk in EnumBlocks())
            {
                MatrixByArr bv  = blk.Item3;
                double val;
                val = bv[0,0]; if(val != 0) yield return val;
                val = bv[0,1]; if(val != 0) yield return val;
                val = bv[0,2]; if(val != 0) yield return val;
                val = bv[1,0]; if(val != 0) yield return val;
                val = bv[1,1]; if(val != 0) yield return val;
                val = bv[1,2]; if(val != 0) yield return val;
                val = bv[2,0]; if(val != 0) yield return val;
                val = bv[2,1]; if(val != 0) yield return val;
                val = bv[2,2]; if(val != 0) yield return val;
            }
        }

        public static bool ZerosSparse_selftest = HDebug.IsDebuggerAttached;
        public static HessMatrix Zeros(int colsize, int rowsize)
        {
            int layersize = 64;
            return Zeros(colsize, rowsize, layersize);
        }
        public static HessMatrix Zeros(int colsize, int rowsize, int layersize)
        {
            if(ZerosSparse_selftest)
            {
                ZerosSparse_selftest = false;
                HessMatrix tmat = Zeros(300, 300, layersize);
                bool bzero = true;
                for(int c=0; c<tmat.ColSize; c++)
                    for(int r=0; r<tmat.ColSize; r++)
                        if(tmat[c, r] != 0)
                            bzero = false;
                HDebug.Assert(bzero);
            }

            HessMatrix mat = new HessMatrix(colsize, rowsize, layersize);
            return mat;
        }
        public void MakeSparse()
        {
            for(int bc=0; bc<ColBlockSize; bc++)
                for(int br=0; br<RowBlockSize; br++)
                {
                    if(bc == br) continue;
                    MatrixByArr blk = GetBlock(bc, br);
                    if(blk == null) continue;
                    if(blk.MaxAbs() == 0)
                        SetBlock(bc, br, null);
                }
        }
        public HessMatrix(SerializationInfo info, StreamingContext ctxt)
        {
            this.colblksize            = info.GetInt32("colblksize"           );
            this.rowblksize            = info.GetInt32("rowblksize"           );
            this.layersize             = info.GetInt32("layersize"            );
            int numusedblocks_offdiag  = info.GetInt32("numusedblocks_offdiag");

            int br2_size =  rowblksize % layersize;
            int br1_size = (rowblksize - br2_size) / layersize + 1;

               diag       = new List<double    [,]>(colblksize);
            offdiag       = new List<double[][][,]>(colblksize);
            offdiag_count = new List<int   []     >(colblksize);
            for(int bc=0; bc<colblksize; bc++)
            {
                   diag      .Add(null                     );
                offdiag      .Add(new double[br1_size][][,]);
                offdiag_count.Add(new int   [br1_size]     );
                for(int br1=0; br1<br1_size; br1++)
                {
                    offdiag      [bc][br1] = null;
                    offdiag_count[bc][br1] = 0;
                }
            }

            List<int   > list_bc   = (List<int   >)info.GetValue("list_bc"  , typeof(List<int   >));
            List<int   > list_br   = (List<int   >)info.GetValue("list_br"  , typeof(List<int   >));
            List<double> list_bv00 = (List<double>)info.GetValue("list_bv00", typeof(List<double>));
            List<double> list_bv01 = (List<double>)info.GetValue("list_bv01", typeof(List<double>));
            List<double> list_bv02 = (List<double>)info.GetValue("list_bv02", typeof(List<double>));
            List<double> list_bv10 = (List<double>)info.GetValue("list_bv10", typeof(List<double>));
            List<double> list_bv11 = (List<double>)info.GetValue("list_bv11", typeof(List<double>));
            List<double> list_bv12 = (List<double>)info.GetValue("list_bv12", typeof(List<double>));
            List<double> list_bv20 = (List<double>)info.GetValue("list_bv20", typeof(List<double>));
            List<double> list_bv21 = (List<double>)info.GetValue("list_bv21", typeof(List<double>));
            List<double> list_bv22 = (List<double>)info.GetValue("list_bv22", typeof(List<double>));
            int count = list_bc  .Count;
            HDebug.Assert(count == list_bc  .Count);
            HDebug.Assert(count == list_br  .Count);
            HDebug.Assert(count == list_bv00.Count);
            HDebug.Assert(count == list_bv01.Count);
            HDebug.Assert(count == list_bv02.Count);
            HDebug.Assert(count == list_bv10.Count);
            HDebug.Assert(count == list_bv11.Count);
            HDebug.Assert(count == list_bv12.Count);
            HDebug.Assert(count == list_bv20.Count);
            HDebug.Assert(count == list_bv21.Count);
            HDebug.Assert(count == list_bv22.Count);
            for(int i=0; i<count; i++)
            {
                int bc = list_bc[i];
                int br = list_br[i];
                double[,] bval = new double[3,3];
                bval[0, 0] = list_bv00[i];
                bval[0, 1] = list_bv01[i];
                bval[0, 2] = list_bv02[i];
                bval[1, 0] = list_bv10[i];
                bval[1, 1] = list_bv11[i];
                bval[1, 2] = list_bv12[i];
                bval[2, 0] = list_bv20[i];
                bval[2, 1] = list_bv21[i];
                bval[2, 2] = list_bv22[i];
                SetBlock(bc, br, bval);
            }
            HDebug.Assert(count == this.NumUsedBlocks);
            HDebug.Assert(numusedblocks_offdiag == this.numusedblocks_offdiag);










            //  List<double    [,]>     diag;
            //  List<double[][][,]>  offdiag;
            //  List<int   []     >  offdiag_count;
            //  int colblksize;
            //  int rowblksize;
            //  int layersize;
            //  int numusedblocks_offdiag;




            //  this.BlkSize = info.GetInt32("BlkSize");
            //  this._ColSize = info.GetInt32("_ColSize");
            //  this._RowSize = info.GetInt32("_RowSize");
            //  
            //  int colblksize = (ColSize % BlkSize == 0) ? (ColSize/BlkSize) : (ColSize/BlkSize+1);
            //  int rowblksize = (RowSize % BlkSize == 0) ? (RowSize/BlkSize) : (RowSize/BlkSize+1);
            //  this.blkmatrix = new MatrixSparse<MatrixByArr>(colblksize, rowblksize, GetZeroBlock);
            //  
            //  int         block_num   =              info.GetInt32("block_num");
            //  int[]       block_Item1 = (int[]      )info.GetValue("block_Item1", typeof(int[]      ));
            //  int[]       block_Item2 = (int[]      )info.GetValue("block_Item2", typeof(int[]      ));
            //  double[,][] block_Item3 = (double[,][])info.GetValue("block_Item3", typeof(double[,][]));
            //  block_num = block_Item1.Length;
            //  HDebug.Assert(block_num == block_Item1.Length);
            //  HDebug.Assert(block_num == block_Item2.Length);
            //  HDebug.Assert(block_Item3.GetLength(0) == BlkSize);
            //  HDebug.Assert(block_Item3.GetLength(1) == BlkSize);
            //  for(int c = 0; c < BlkSize; c++)
            //      for(int r = 0; r < BlkSize; r++)
            //          HDebug.Assert(block_Item3[c, r].Length == block_num);
            //  for(int i = 0; i < block_num; i++)
            //  {
            //      int bc = block_Item1[i];
            //      int br = block_Item2[i];
            //      MatrixByArr bval = new double[BlkSize, BlkSize];
            //      for(int c = 0; c < BlkSize; c++)
            //          for(int r = 0; r < BlkSize; r++)
            //              bval[c, r] = block_Item3[c, r][i];
            //      SetBlock(bc, br, bval);
            //  }
            //  
            //  //throw new NotImplementedException();
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("colblksize"           , this.colblksize           );
            info.AddValue("rowblksize"           , this.rowblksize           );
            info.AddValue("layersize"            , this.layersize            );
            info.AddValue("numusedblocks_offdiag", this.numusedblocks_offdiag);

            List<int   > list_bc   = new List<int   >();
            List<int   > list_br   = new List<int   >();
            List<double> list_bv00 = new List<double>();
            List<double> list_bv01 = new List<double>();
            List<double> list_bv02 = new List<double>();
            List<double> list_bv10 = new List<double>();
            List<double> list_bv11 = new List<double>();
            List<double> list_bv12 = new List<double>();
            List<double> list_bv20 = new List<double>();
            List<double> list_bv21 = new List<double>();
            List<double> list_bv22 = new List<double>();
            foreach(var bc_br_bval in _EnumBlocks())
            {
                list_bc  .Add(bc_br_bval.Item1);
                list_br  .Add(bc_br_bval.Item2);
                list_bv00.Add(bc_br_bval.Item3[0,0]);
                list_bv01.Add(bc_br_bval.Item3[0,1]);
                list_bv02.Add(bc_br_bval.Item3[0,2]);
                list_bv10.Add(bc_br_bval.Item3[1,0]);
                list_bv11.Add(bc_br_bval.Item3[1,1]);
                list_bv12.Add(bc_br_bval.Item3[1,2]);
                list_bv20.Add(bc_br_bval.Item3[2,0]);
                list_bv21.Add(bc_br_bval.Item3[2,1]);
                list_bv22.Add(bc_br_bval.Item3[2,2]);
            }
            HDebug.Assert(list_bc  .Count == this.NumUsedBlocks);
            HDebug.Assert(list_br  .Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv00.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv01.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv02.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv10.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv11.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv12.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv20.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv21.Count == this.NumUsedBlocks);
            HDebug.Assert(list_bv22.Count == this.NumUsedBlocks);
            info.AddValue("list_bc"  , list_bc  );
            info.AddValue("list_br"  , list_br  );
            info.AddValue("list_bv00", list_bv00);
            info.AddValue("list_bv01", list_bv01);
            info.AddValue("list_bv02", list_bv02);
            info.AddValue("list_bv10", list_bv10);
            info.AddValue("list_bv11", list_bv11);
            info.AddValue("list_bv12", list_bv12);
            info.AddValue("list_bv20", list_bv20);
            info.AddValue("list_bv21", list_bv21);
            info.AddValue("list_bv22", list_bv22);

            //  info.AddValue("BlkSize", this.BlkSize);
            //  info.AddValue("_ColSize", this._ColSize);
            //  info.AddValue("_RowSize", this._RowSize);
            //  //info.AddValue("block_num", block_num);
            //  int         block_num  = blkmatrix.NumElements;
            //  int[]       block_Item1 = new int[block_num];
            //  int[]       block_Item2 = new int[block_num];
            //  double[,][] block_Item3 = new double[BlkSize, BlkSize][]; for(int c = 0; c < BlkSize; c++) for(int r = 0; r < BlkSize; r++) block_Item3[c, r] = new double[block_num];
            //  int i = 0;
            //  foreach(Tuple<int, int, MatrixByArr> block in blkmatrix.EnumElements())
            //  {
            //      block_Item1[i] = block.Item1;
            //      block_Item2[i] = block.Item2;
            //      for(int c = 0; c < BlkSize; c++)
            //          for(int r = 0; r < BlkSize; r++)
            //              block_Item3[c, r][i] = block.Item3[c, r];
            //      i++;
            //  }
            //  info.AddValue("block_num", block_num);
            //  info.AddValue("block_Item1", block_Item1);
            //  info.AddValue("block_Item2", block_Item2);
            //  info.AddValue("block_Item3", block_Item3);
            //  
            //  //throw new NotImplementedException();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // IBinarySerializable
        public void BinarySerialize(HBinaryWriter writer)
        {
            writer.Write(colblksize           );
            writer.Write(rowblksize           );
            writer.Write(layersize            );
            writer.Write(numusedblocks_offdiag);

            writer.Write(NumUsedBlocks        );
            int bcnt = 0;
            foreach(var bc_br_bval in _EnumBlocks())
            {
                bcnt ++;
                writer.Write(bc_br_bval.Item1     );
                writer.Write(bc_br_bval.Item2     );
                writer.Write(bc_br_bval.Item3[0,0]);
                writer.Write(bc_br_bval.Item3[0,1]);
                writer.Write(bc_br_bval.Item3[0,2]);
                writer.Write(bc_br_bval.Item3[1,0]);
                writer.Write(bc_br_bval.Item3[1,1]);
                writer.Write(bc_br_bval.Item3[1,2]);
                writer.Write(bc_br_bval.Item3[2,0]);
                writer.Write(bc_br_bval.Item3[2,1]);
                writer.Write(bc_br_bval.Item3[2,2]);
            }
            if(bcnt != NumUsedBlocks)
                throw new System.IO.InvalidDataException();
        }
        public HessMatrix(HBinaryReader reader)
        {
            this.colblksize           = reader.ReadInt32();
            this.rowblksize           = reader.ReadInt32();
            this.layersize            = reader.ReadInt32();
            int numusedblocks_offdiag = reader.ReadInt32();

            int br2_size =  rowblksize % layersize;
            int br1_size = (rowblksize - br2_size) / layersize + 1;

               diag       = new List<double    [,]>(colblksize);
            offdiag       = new List<double[][][,]>(colblksize);
            offdiag_count = new List<int   []     >(colblksize);
            for(int bc=0; bc<colblksize; bc++)
            {
                   diag      .Add(null                     );
                offdiag      .Add(new double[br1_size][][,]);
                offdiag_count.Add(new int   [br1_size]     );
                for(int br1=0; br1<br1_size; br1++)
                {
                    offdiag      [bc][br1] = null;
                    offdiag_count[bc][br1] = 0;
                }
            }

            int NumUsedBlocks         = reader.ReadInt32();
            for(int i=0; i<NumUsedBlocks; i++)
            {
                double[,] bval = new double[3,3];
                int     bc = reader.ReadInt32();
                int     br = reader.ReadInt32();
                bval[0, 0] = reader.ReadDouble();
                bval[0, 1] = reader.ReadDouble();
                bval[0, 2] = reader.ReadDouble();
                bval[1, 0] = reader.ReadDouble();
                bval[1, 1] = reader.ReadDouble();
                bval[1, 2] = reader.ReadDouble();
                bval[2, 0] = reader.ReadDouble();
                bval[2, 1] = reader.ReadDouble();
                bval[2, 2] = reader.ReadDouble();
                SetBlock(bc, br, bval);
            }
            HDebug.Assert(NumUsedBlocks         == this.NumUsedBlocks        );
            HDebug.Assert(numusedblocks_offdiag == this.numusedblocks_offdiag);
        }
        public void Deserialize(HBinaryReader reader)
        {
            // this is an abstract class
            throw new NotImplementedException();
        }
    }
}
