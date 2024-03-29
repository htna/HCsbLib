﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    public static partial class HessMatrixStatic
    {
        public static HessMatrix CorrectHessDiag(this HessMatrix _this)
        {
            return Hess.CorrectHessDiag(_this);
        }

        public static HessMatrix SubMatrixByAtoms(this HessMatrix _this, bool ignNegIdx, bool bCloneBlock, IList<int>   idxAtoms) { return SubMatrixByAtomsImpl(_this, ignNegIdx, bCloneBlock, idxAtoms); }
        public static HessMatrix SubMatrixByAtoms(this HessMatrix _this, bool ignNegIdx, bool bCloneBlock, params int[] idxAtoms) { return SubMatrixByAtomsImpl(_this, ignNegIdx, bCloneBlock, idxAtoms); }
        public static bool       SubMatrixByAtomsImpl_selftest = HDebug.IsDebuggerAttached;
        public static HessMatrix SubMatrixByAtomsImpl(this HessMatrix _this, bool ignNegIdx, bool bCloneBlock, IList<int> idxAtoms)
        {
            if(SubMatrixByAtomsImpl_selftest)
            {
                SubMatrixByAtomsImpl_selftest = false;
                Vector[] tcoords = new Vector[]{
                    new Vector(1,2,3),
                    new Vector(1,3,2),
                    new Vector(1,2,9),
                };
                HessMatrix thess0  = Hess.GetHessAnm(tcoords);
                int[] tidxs = new int[]{0, 2};
                HessMatrix thess1a = thess0.SubMatrixByAtoms(false, bCloneBlock, tidxs);
                HessMatrix thess1b = (new double[,]
                {
                    { thess0[0,0], thess0[0,1], thess0[0,2],        thess0[0,6], thess0[0,7], thess0[0,8] },
                    { thess0[1,0], thess0[1,1], thess0[1,2],        thess0[1,6], thess0[1,7], thess0[1,8] },
                    { thess0[2,0], thess0[2,1], thess0[2,2],        thess0[2,6], thess0[2,7], thess0[2,8] },
                    { thess0[6,0], thess0[6,1], thess0[6,2],        thess0[6,6], thess0[6,7], thess0[6,8] },
                    { thess0[7,0], thess0[7,1], thess0[7,2],        thess0[7,6], thess0[7,7], thess0[7,8] },
                    { thess0[8,0], thess0[8,1], thess0[8,2],        thess0[8,6], thess0[8,7], thess0[8,8] },
                }).ToHessMatrix();

//                           thess1a = Hess.CorrectHessDiag(thess1a);                     // diagonal of original matrix contains the interaction between 0-1 and 1-2 also,
//                HessMatrix thess1b = Hess.GetHessAnm(tcoords.HSelectByIndex(tidxs));    // while new generated hessian matrix does not.
                Matrix tdiffhess = (thess1a - thess1b).ToMatrix();
                double max_tdiffhess = tdiffhess.EnumValues().HEnumAbs().HMax(); 
                HDebug.Exception(0 == max_tdiffhess);
            }

            HessMatrix nhess = SubMatrixByAtomsImpl(_this, ignNegIdx, idxAtoms, idxAtoms, bCloneBlock);

            if(HDebug.IsDebuggerAttached && idxAtoms.Count < 1000)
            {
                List<int> idx3Atoms = new List<int>();
                foreach(int idx in idxAtoms) for(int i=0; i<3; i++) idx3Atoms.Add(idx*3+i);
                Matrix tnhess = _this.ToMatrix().SubMatrix(idx3Atoms, idx3Atoms);
                double max2_tdiffhess = (nhess.ToMatrix() - tnhess).HEnumElement().HEnumAbs().HMax();
                HDebug.AssertTolerance(0.00000001, max2_tdiffhess);
            }
            return nhess;
        }

        public static HessMatrix SubMatrixByAtoms
            ( this HessMatrix _this
            , bool ignNegIdx        // [false]
            , IList<int> idxColAtoms
            , IList<int> idxRowAtoms
            , bool parallel=false
            )
        {
            return SubMatrixByAtomsImpl
                ( _this
                , ignNegIdx, idxColAtoms, idxRowAtoms, true
                , parallel: parallel
                );
        }
        public static HessMatrix SubMatrixByAtoms
            ( this HessMatrix _this
            , bool ignNegIdx        // [false]
            , IList<int> idxColAtoms
            , IList<int> idxRowAtoms
            , bool bCloneBlock
            , bool parallel=false
            )
        {
            return SubMatrixByAtomsImpl
                ( _this
                , ignNegIdx, idxColAtoms, idxRowAtoms, bCloneBlock
                , parallel: parallel
                );
        }
        public static bool       SubMatrixByAtomsImpl_selftest2 = HDebug.IsDebuggerAttached;
        public static HessMatrix SubMatrixByAtomsImpl
            ( this HessMatrix _this
            , bool ignNegIdx        // [false]
            , IList<int> idxColAtoms
            , IList<int> idxRowAtoms
            , bool bCloneBlock
            , bool parallel=false
            )
        {
            Dictionary<int, int[]> col_idx2nidx = new Dictionary<int, int[]>();
            HashSet<int> col_idxs = new HashSet<int>();
            for(int nidx=0; nidx<idxColAtoms.Count; nidx++)
            {
                int idx = idxColAtoms[nidx];
                if(idx < 0)
                {
                    if(ignNegIdx) continue;
                    throw new IndexOutOfRangeException();
                }
                if(col_idx2nidx.ContainsKey(idx) == false) col_idx2nidx.Add(idx, new int[0]);
                col_idx2nidx[idx] = col_idx2nidx[idx].HAdd(nidx);
                col_idxs.Add(idx);
            }
            Dictionary<int, int[]> row_idx2nidx = new Dictionary<int, int[]>();
            for(int nidx=0; nidx<idxRowAtoms.Count; nidx++)
            {
                int idx = idxRowAtoms[nidx];
                if(idx < 0)
                {
                    if(ignNegIdx) continue;
                    throw new IndexOutOfRangeException();
                }
                if(row_idx2nidx.ContainsKey(idx) == false) row_idx2nidx.Add(idx, new int[0]);
                row_idx2nidx[idx] = row_idx2nidx[idx].HAdd(nidx);
            }

            HessMatrix nhess = HessMatrix.Zeros(idxColAtoms.Count*3, idxRowAtoms.Count*3);
            {
                Action<ValueTuple<int, int, MatrixByArr>> func = delegate(ValueTuple<int, int, MatrixByArr> bc_br_bval)
                {
                    if(bCloneBlock)
                    {
                        int bc   = bc_br_bval.Item1; if(col_idx2nidx.ContainsKey(bc) == false) return;
                        int br   = bc_br_bval.Item2; if(row_idx2nidx.ContainsKey(br) == false) return;
                        var bval = bc_br_bval.Item3;
                        foreach(int nbc in col_idx2nidx[bc])
                            foreach(int nbr in row_idx2nidx[br])
                                lock(nhess)
                                    nhess.SetBlock(nbc, nbr, bval.CloneT());
                    }
                    else
                    {
                        int bc   = bc_br_bval.Item1; if(col_idx2nidx.ContainsKey(bc) == false) return;
                        int br   = bc_br_bval.Item2; if(row_idx2nidx.ContainsKey(br) == false) return;
                        var bval = bc_br_bval.Item3;
                        foreach(int nbc in col_idx2nidx[bc])
                            foreach(int nbr in row_idx2nidx[br])
                                lock(nhess)
                                    nhess.SetBlock(nbc, nbr, bval);
                    }
                };
                static void ForeachSetBlockCloneBlock(ValueTuple<int, int, MatrixByArr> bc_br_bval, HessMatrix nhess, Dictionary<int, int[]> col_idx2nidx, Dictionary<int, int[]> row_idx2nidx)
                {
                    int bc   = bc_br_bval.Item1; if(col_idx2nidx.ContainsKey(bc) == false) return;
                    int br   = bc_br_bval.Item2; if(row_idx2nidx.ContainsKey(br) == false) return;
                    var bval = bc_br_bval.Item3;
                    foreach(int nbc in col_idx2nidx[bc])
                        foreach(int nbr in row_idx2nidx[br])
                            nhess.SetBlock(nbc, nbr, bval.CloneT());
                };
                static void ForeachSetBlock          (ValueTuple<int, int, MatrixByArr> bc_br_bval, HessMatrix nhess, Dictionary<int, int[]> col_idx2nidx, Dictionary<int, int[]> row_idx2nidx)
                {
                    int bc   = bc_br_bval.Item1; if(col_idx2nidx.ContainsKey(bc) == false) return;
                    int br   = bc_br_bval.Item2; if(row_idx2nidx.ContainsKey(br) == false) return;
                    var bval = bc_br_bval.Item3;
                    foreach(int nbc in col_idx2nidx[bc])
                        foreach(int nbr in row_idx2nidx[br])
                            nhess.SetBlock(nbc, nbr, bval);
                };

                if(parallel)        Parallel.ForEach(         _this.EnumBlocksInCols(col_idxs.ToArray()), func           );
                else {
                    if(bCloneBlock) foreach(var bc_br_bval in _this.EnumBlocksInCols(col_idxs.ToArray())) ForeachSetBlockCloneBlock(bc_br_bval, nhess, col_idx2nidx, row_idx2nidx);
                    else            foreach(var bc_br_bval in _this.EnumBlocksInCols(col_idxs.ToArray())) ForeachSetBlock          (bc_br_bval, nhess, col_idx2nidx, row_idx2nidx);
                }
            
            }
            if(SubMatrixByAtomsImpl_selftest2)
            {
                SubMatrixByAtomsImpl_selftest2 = false;
                HessMatrix tnhess = SubMatrixByAtomsImpl0(_this, idxColAtoms, idxRowAtoms);
                HDebug.Assert(HessMatrixStatic.HessMatrixEqual(nhess, tnhess));
                //////////////////////////////////////////
                Matrix thess1 = new double[,]{{0,1,2,3,4,5}
                                             ,{1,2,3,4,5,6}
                                             ,{2,3,4,5,6,7}
                                             ,{3,4,5,6,7,8}
                                             ,{4,5,6,7,8,9}
                                             ,{5,6,7,8,9,0}};
                HessMatrix thess2 = thess1.ToHessMatrix();
                HessMatrix thess3 = thess2.SubMatrixByAtomsImpl(false, new int[] { 0 }, new int[] { 1 }, true);
                Matrix thess4 = new double[,] {{3,4,5}
                                              ,{4,5,6}
                                              ,{5,6,7}};
                HDebug.AssertToleranceMatrix(0, thess3.ToMatrix()-thess4);
            }

            return nhess;
        }
        public static bool       SubMatrixByAtomsImpl0_selftest2 = HDebug.IsDebuggerAttached;
        public static HessMatrix SubMatrixByAtomsImpl0(this HessMatrix _this, IList<int> idxColAtoms, IList<int> idxRowAtoms)
        {
            if(SubMatrixByAtomsImpl0_selftest2)
            {
                SubMatrixByAtomsImpl0_selftest2 = false;
                Matrix thess1 = new double[,]{{0,1,2,3,4,5}
                                             ,{1,2,3,4,5,6}
                                             ,{2,3,4,5,6,7}
                                             ,{3,4,5,6,7,8}
                                             ,{4,5,6,7,8,9}
                                             ,{5,6,7,8,9,0}};
                HessMatrix thess2 = thess1.ToHessMatrix();
                HessMatrix thess3 = thess2.SubMatrixByAtomsImpl0(new int[] { 0 }, new int[] { 1 });
                Matrix thess4 = new double[,] {{3,4,5}
                                              ,{4,5,6}
                                              ,{5,6,7}};
                HDebug.AssertToleranceMatrix(0, thess3.ToMatrix()-thess4);
            }
            HessMatrix nhess = HessMatrix.Zeros(idxColAtoms.Count*3, idxRowAtoms.Count*3);

            for(int nbc=0; nbc<idxColAtoms.Count; nbc++)
            {
                for(int nbr=0; nbr<idxRowAtoms.Count; nbr++)
                {
                    int bc = idxColAtoms[nbc]; if(bc < 0) continue;
                    int br = idxRowAtoms[nbr]; if(br < 0) continue;
                    if(_this.HasBlock(bc, br) == false)
                        continue;
                    MatrixByArr block = _this.GetBlock(bc, br).CloneT(); // hessian matrix for interaction between atom i and j
                    HDebug.Assert(block.IsZero() == false);
                    nhess.SetBlock(nbc, nbr, block);
                }
            }

            return nhess;
        }

        public static bool HessMatrixEqual(HessMatrix left, HessMatrix right, double threshold=0)
        {
            if(left == null && right == null) return true;
            if(left.ColSize != right.ColSize) return false;
            if(left.RowSize != right.RowSize) return false;

            double[,] zeros = new double[3,3];

            foreach(var bc_br_bval in left.EnumBlocks())
            {
                int bc  = bc_br_bval.Item1;
                int br  = bc_br_bval.Item2;
                var bv  = bc_br_bval.Item3;
                var bv0 = right.GetBlock(bc, br);
                if(bv0 == null) bv0 = zeros;
                double absmax = (bv, bv0).HAbsMaxDiffWith();
                if(absmax > threshold) return false;
            }

            foreach(var bc_br_bval in right.EnumBlocks())
            {
                int bc  = bc_br_bval.Item1;
                int br  = bc_br_bval.Item2;
                var bv  = bc_br_bval.Item3;
                var bv0 = left.GetBlock(bc, br);
                if(bv0 == null) bv0 = zeros;
                double absmax = (bv, bv0).HAbsMaxDiffWith();
                if(absmax > threshold) return false;
            }

            return true;
        }
    }
}
