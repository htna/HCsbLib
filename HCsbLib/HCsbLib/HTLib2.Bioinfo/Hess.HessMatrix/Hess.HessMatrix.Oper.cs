using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    public partial class HessMatrix : IHessMatrix
    {
        //public static HessMatrix operator-(HessMatrix left               ) { HessMatrix mat = left.CloneHess(); mat.UpdateMul(-1     ); return mat; }
        //public static HessMatrix operator+(HessMatrix left, IMatrix right) { HessMatrix mat = left.CloneHess(); mat.UpdateAdd(right, 1); return mat; }
        //public static HessMatrix operator-(HessMatrix left, IMatrix right) { HessMatrix mat = left.CloneHess(); mat.UpdateAdd(right,-1); return mat; }
        //public static HessMatrix operator*(HessMatrix left, IMatrix right)
        //{
        //    HessMatrix mat = HessMatrix.GetMul(left, right);
        //    return mat;
        //}
        public static HessMatrix operator*(HessMatrix left,HessMatrix right){HessMatrix mat = GetMulImpl(null, true, left, right); return mat; }
        public static HessMatrix operator*(HessMatrix left, double  right) { HessMatrix mat = left .CloneHessMatrix(); mat.UpdateMul(right  ); return mat; }
        public static HessMatrix operator*(double left, HessMatrix  right) { HessMatrix mat = right.CloneHessMatrix(); mat.UpdateMul(left  ); return mat; }
        public static HessMatrix operator/(HessMatrix left, double  right) { HessMatrix mat = left .CloneHessMatrix(); mat.UpdateMul(1/right); return mat; }

        public static HessMatrix operator+(HessMatrix left, HessMatrix right) { HessMatrix mat = left.CloneHessMatrix(); mat.UpdateAdd(right,  1, null, 0); return mat; }
        public static HessMatrix operator-(HessMatrix left, HessMatrix right) { HessMatrix mat = left.CloneHessMatrix(); mat.UpdateAdd(right, -1, null, 0); return mat; }
        //public static HessMatrix GetMul(HessMatrix left, IMatrix right)
        //{
        //    if(right is HessMatrix)
        //        return HessMatrix.GetMul(left, right as HessMatrix);
        //    else
        //        return Matrix.GetMul(left, right).ToHessMatrix();
        //}
        public static HessMatrix GetMul(ILinAlg ila, params HessMatrix[] mats)
        {
            return GetMulImpl(ila, true, mats);
        }
        public static HessMatrix GetMulImpl(ILinAlg ila, bool warning, params HessMatrix[] mats)
        {
            if(ila != null)
                if(HDebug.Selftest())
                {
                    Matrix     h0 = new double[,]{{0,1,2,3,4,5}
                                                 ,{1,2,3,4,5,6}
                                                 ,{2,3,4,5,6,7}
                                                 ,{3,4,5,6,7,8}
                                                 ,{4,5,6,7,8,9}
                                                 ,{5,6,7,8,9,0}};
                    HessMatrix h1 = HessMatrix.FromMatrix(h0);
                    Matrix t0 = Matrix.GetMul(Matrix.GetMul(h0, h0), h0);
                    {
                        Matrix t1 = GetMulImpl(ila, false, h1, h1, h1).ToArray(); double d1=(t0-t1).HAbsMax(); HDebug.Assert(0 == d1);
                    }
                    {
                        Matrix t1 = GetMulImpl(null,false, h1, h1, h1).ToArray(); double d1=(t0-t1).HAbsMax(); HDebug.Assert(0 == d1);
                    }
                }

            HessMatrix mul = null;
            foreach(HessMatrix mat in mats)
            {
                if(mul == null) mul = mat;
                else            mul = GetMulImpl(mul, mat, ila, warning);
            }
            return mul;
        }
        static HessMatrix GetMulImpl(HessMatrix left, HessMatrix right, ILinAlg ila, bool warning)
        {
            if(HDebug.Selftest())
            {
                Matrix h1 = new double[,]{{0,1,2,3,4,5}
                                         ,{1,2,3,4,5,6}
                                         ,{2,3,4,5,6,7}
                                         ,{3,4,5,6,7,8}
                                         ,{4,5,6,7,8,9}
                                         ,{5,6,7,8,9,0}};
                HessMatrix h2 = HessMatrix.FromMatrix(h1);
                Matrix     h11 =     Matrix.GetMul    (h1, h1);
                HessMatrix h22 = HessMatrix.GetMulImpl(h2, h2, null, false);
                Matrix     hdiff = h11 - h22.ToMatrix();
                HDebug.AssertToleranceMatrix(0, hdiff);
            }

            Dictionary<int, Dictionary<int, Tuple<int, int, MatrixByArr>>> left_ic_rows = new Dictionary<int, Dictionary<int, Tuple<int, int, MatrixByArr>>>();
            foreach(var ic_row in left.EnumRowBlocksAll()) left_ic_rows.Add(ic_row.Item1, ic_row.Item2.HToDictionaryWithKeyItem2());

            Dictionary<int, Dictionary<int, Tuple<int, int, MatrixByArr>>> right_ir_cols = new Dictionary<int, Dictionary<int, Tuple<int, int, MatrixByArr>>>();
            foreach(var ir_col in right.EnumColBlocksAll()) right_ir_cols.Add(ir_col.Item1, ir_col.Item2.HToDictionaryWithKeyItem1());

            HessMatrix mul = null;
            mul = HessMatrix.ZerosHessMatrix(left.ColSize, right.RowSize);
            for(int ic=0; ic<left.ColBlockSize; ic++)
            {
                var left_row = left_ic_rows[ic];
                if(left_row.Count == 0)
                    continue;
                for(int ir=0; ir<right.RowBlockSize; ir++)
                {
                    var right_col = right_ir_cols[ir];
                    if(right_col.Count == 0)
                        continue;
                    foreach(var left_ck in left_row)
                    {
                        int ik = left_ck.Key;
                        HDebug.Assert(ic == left_ck.Value.Item1);
                        HDebug.Assert(ik == left_ck.Value.Item2);
                        if(right_col.ContainsKey(ik))
                        {
                            var right_kr = right_col[ik];
                            HDebug.Assert(ik == right_kr.Item1);
                            HDebug.Assert(ir == right_kr.Item2);
                            MatrixByArr mul_ckr = mul.GetBlock(ic,ir) + left_ck.Value.Item3 * right_kr.Item3;
                            mul.SetBlock(ic, ir, mul_ckr);
                        }
                    }
                }
            }

            return mul;
        }
        public override void UpdateMul(double other)
        {
            double[,] debug_prv = null;
            if(HDebug.IsDebuggerAttached)
                debug_prv = this.ToArray();

            foreach(var bc_br_bval in EnumBlocks())
            {
                int         bc   = bc_br_bval.Item1;
                int         br   = bc_br_bval.Item2;
                MatrixByArr bmat = bc_br_bval.Item3;
                SetBlock(bc, br, bmat * other);
            }

            if(HDebug.IsDebuggerAttached)
            {
                double[,] debug_now = this.ToArray();
                for(int c=0; c<ColSize; c++)
                    for(int r=0; r<RowSize; r++)
                    {
                        double val0 = debug_now[c, r];
                        double val1 = other * debug_prv[c, r];
                        HDebug.Assert(val0 == val1);
                    }
            }
        }
        public void UpdateAdd(HessMatrix other, double other_mul)
        {
            UpdateAdd
            ( other
            , other_mul
            , null  //idxOther
            , 0     //thres_NearZeroBlock
            );
        }
        static bool UpdateAdd_SelfTest = HDebug.IsDebuggerAttached;
        public int UpdateAdd(HessMatrix other, double mul_other, IList<int> idxOther, double thres_NearZeroBlock, bool parallel=false)
        {
            Matrix debug_updateadd = null;
            if(UpdateAdd_SelfTest && idxOther == null && thres_NearZeroBlock==0)
            {
                if((100 < ColBlockSize) && (ColBlockSize < 1000)
                && (100 < RowBlockSize) && (RowBlockSize < 1000)
                && (other.NumUsedBlocks > 20))
                {
                    UpdateAdd_SelfTest = false;
                    debug_updateadd = this.ToArray();
                    debug_updateadd.UpdateAdd(other, mul_other);
                }
            }

            int[] idx_other;
            if(idxOther == null)
            {
                //HDebug.Exception(ColSize == other.ColSize);
                //HDebug.Exception(RowSize == other.RowSize);
                idx_other = HEnum.HEnumCount(other.ColSize).ToArray();
            }
            else
            {
                idx_other = idxOther.ToArray();
            }

            int count = 0;
            int count_ignored = 0;

            object _lock = new object();
            object _lock_ignored = new object();
            Action<ValueTuple<int, int, MatrixByArr>> func = delegate(ValueTuple<int, int, MatrixByArr> bc_br_bval)
            {
                count++;
                int         other_bc   = bc_br_bval.Item1;
                int         other_br   = bc_br_bval.Item2;
                MatrixByArr other_bmat = bc_br_bval.Item3;
                if(other_bmat.HAbsMax() <= thres_NearZeroBlock)
                {
                    lock(_lock_ignored)
                        count_ignored ++;
                    //continue;
                    return;
                }
                int               bc   = idx_other[other_bc];
                int               br   = idx_other[other_br];
                lock(_lock)
                {
                    MatrixByArr  this_bmat = GetBlock(bc, br);
                    MatrixByArr   new_bmat;
                    if(this_bmat == null)
                    {
                        if(other_bmat == null)  new_bmat = null;
                        else                    new_bmat =             mul_other*other_bmat;
                    }
                    else
                    {
                        if(other_bmat == null)  new_bmat = this_bmat.CloneT()              ;
                        else                    new_bmat = this_bmat + mul_other*other_bmat;
                    }
                    SetBlock(bc, br, new_bmat);
                }
            };

            if(parallel) Parallel.ForEach(other.EnumBlocks(), func);
            else                  foreach(var bc_br_bval in other.EnumBlocks())   func(bc_br_bval);
            
            if(debug_updateadd != null)
            {
                Matrix debug_diff = debug_updateadd-this;
                double debug_absmax = debug_diff.HAbsMax();
                HDebug.AssertToleranceMatrix(0, debug_diff);
            }

            return count_ignored;
        }
        public virtual HessMatrix Tr()
        {
            HessMatrix tr = ZerosHessMatrix(RowSize, ColSize);
            foreach(var bc_br_bval in EnumBlocks())
            {
                int bc = bc_br_bval.Item1;
                int br = bc_br_bval.Item2;
                var bval = bc_br_bval.Item3;
                tr.SetBlock(br, bc, bval.Tr());
            }
            return tr;
        }
    }
}
