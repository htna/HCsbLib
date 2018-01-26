using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public abstract partial class HessMatrix : Matrix
    {
        ///////////////////////////////////////////////////////////////////////
        // MakeNearZeroBlockAsZero
        public int MakeNearZeroBlockAsZero(double thres_absmax)
        {
            double min_absmax_bval = double.MaxValue;
            List<Tuple<int,int>> lstIdxToMakeZero = new List<Tuple<int, int>>();
            foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in EnumBlocks_dep())
            {
                int bc   = bc_br_bval.Item1;
                int br   = bc_br_bval.Item2;
                var bval = bc_br_bval.Item3;
                double absmax_bval = bval.HAbsMax();
                min_absmax_bval = Math.Min(min_absmax_bval, absmax_bval);
                if(absmax_bval < thres_absmax)
                {
                    lstIdxToMakeZero.Add(new Tuple<int, int>(bc, br));
                }
            }
            foreach(var bc_br in lstIdxToMakeZero)
            {
                int bc = bc_br.Item1;
                int br = bc_br.Item2;
                HDebug.Assert(GetBlock(bc,br).HAbsMax() < thres_absmax);
                SetBlock(bc, br, null);
            }
            return lstIdxToMakeZero.Count;
        }
        ///////////////////////////////////////////////////////////////////////
        // UpdateSubMatrixByAtom
        public void UpdateSubMatrixByAtom(HessMatrix submat, IList<int> idxcolatoms, IList<int> idxrowatoms)
        {
            HDebug.Assert(idxcolatoms.Max() < ColBlockSize);
            HDebug.Assert(idxrowatoms.Max() < RowBlockSize);
            HDebug.Assert(submat.ColBlockSize == idxcolatoms.Count);
            HDebug.Assert(submat.RowBlockSize == idxrowatoms.Count);
            for(int ic=0; ic<idxcolatoms.Count; ic++)
                for(int ir=0; ir<idxrowatoms.Count; ir++)
                {
                    int hess_ic = idxcolatoms[ic];
                    int hess_ir = idxrowatoms[ir];
                    SetBlock(hess_ic, hess_ir, submat.GetBlock(ic, ir).CloneT());
                }
        }
        ///////////////////////////////////////////////////////////////////////
        // ReshapeByAtom
        // ReshapeByAtomGroupLeftRight
        public HessMatrix ReshapeByAtom(IList<int> idxatms) { return ReshapeByAtomImpl(idxatms, true); }
        public HessMatrix ReshapeByAtom(params int[] idxatms) { return ReshapeByAtomImpl(idxatms, true); }
        static bool       ReshapeByAtomImpl_selftest = HDebug.IsDebuggerAttached;
        public HessMatrix ReshapeByAtomImpl(IList<int> idxatms, bool ignNegIdx)
        {
            HessMatrix reshape = SubMatrixByAtoms(ignNegIdx, idxatms);

            if(ReshapeByAtomImpl_selftest && idxatms.Count<3000)
            {
                ReshapeByAtomImpl_selftest = false;
                HessMatrix treshape = ReshapeByAtomImpl0(idxatms, ignNegIdx);
                HDebug.Assert(HessMatrix.HessMatrixSparseEqual(reshape, treshape));
            }
            return reshape;
        }
        public HessMatrix ReshapeByAtomImpl0(IList<int> idxatms, bool ignNegIdx)
        {
            HDebug.Assert(idxatms.Count == idxatms.HUnion().Length); // check no-duplication of indexes
            HessMatrix reshape = Zeros(idxatms.Count*3, idxatms.Count*3);
            for(int nbc=0; nbc<idxatms.Count; nbc++)
                for(int nbr=0; nbr<idxatms.Count; nbr++)
                {
                    int bc = idxatms[nbc]; if(ignNegIdx && bc < 0) continue;
                    int br = idxatms[nbr]; if(ignNegIdx && br < 0) continue;
                    if(HasBlock(bc, br) == false)
                    {
                        HDebug.AssertTolerance(0, GetBlock(bc, br).ToArray());
                        continue;
                    }
                    MatrixByArr blkrc = GetBlock(bc, br);
                    reshape.SetBlock(nbc, nbr, blkrc.CloneT());
                }
            return reshape;
        }
        public class InfoReshapeByAtomGroupLeftRight
        {
            //public HessMatrix            hess                  = null;
            //public int[]                 idxatoms              = null;
            //public int                   numleft               = 0;
            //public int                   numright              = 0;
            //public Tuple<int[], int[]>[] lstGroupNIdxLeftRight = null;
        };
        public InfoReshapeByAtomGroupLeftRight ReshapeByAtomGroupLeftRight(IList<Tuple<int[], int[]>> lstGroupIdxLeftRight)
        {
            throw new NotImplementedException();
            //Tuple<int[], int[]>[] lstGroupNIdxLeftRight = new Tuple<int[], int[]>[lstGroupIdxLeftRight.Count];
            //for(int i=0; i<lstGroupIdxLeftRight.Count; i++)
            //{
            //    lstGroupNIdxLeftRight[i] = new Tuple<int[], int[]>
            //    (
            //        new int[lstGroupIdxLeftRight[i].Item1.Length],
            //        new int[lstGroupIdxLeftRight[i].Item2.Length]
            //    );
            //    for(int j=0; j<lstGroupNIdxLeftRight[i].Item1.Length; j++) lstGroupNIdxLeftRight[i].Item1[j] = int.MinValue;
            //    for(int j=0; j<lstGroupNIdxLeftRight[i].Item2.Length; j++) lstGroupNIdxLeftRight[i].Item2[j] = int.MinValue;
            //}
            //
            //List<int>  idxatoms = new List<int>();
            //
            //int numleft = 0;
            //for(int i=0; i<lstGroupIdxLeftRight.Count; i++)
            //{
            //    int[] idxleft  = lstGroupIdxLeftRight[i].Item1;
            //    //int[] idxright = lstGroupIdxLeftRight[i].Item2;
            //    for(int j=0; j<idxleft.Length; j++)
            //    {
            //        int  idx = idxleft[j];
            //        int nidx = idxatoms.Count;
            //        lstGroupNIdxLeftRight[i].Item1[j] = nidx;
            //        idxatoms.Add(idx);
            //        numleft++;
            //    }
            //}
            //
            //int numright = 0;
            //for(int i=0; i<lstGroupIdxLeftRight.Count; i++)
            //{
            //    //int[] idxleft  = lstGroupIdxLeftRight[i].Item1;
            //    int[] idxright = lstGroupIdxLeftRight[i].Item2;
            //    for(int j=0; j<idxright.Length; j++)
            //    {
            //        int  idx = idxright[j];
            //        int nidx = idxatoms.Count;
            //        lstGroupNIdxLeftRight[i].Item2[j] = nidx;
            //        idxatoms.Add(idx);
            //        numright++;
            //    }
            //}
            //
            //if(HDebug.IsDebuggerAttached)
            //{
            //    foreach(var left_right in lstGroupNIdxLeftRight)
            //    {
            //        foreach(int idx in left_right.Item1) HDebug.Assert(idx != int.MinValue);
            //        foreach(int idx in left_right.Item2) HDebug.Assert(idx != int.MinValue);
            //    }
            //}
            //
            //HessMatrix nhess = this.ReshapeByAtom(idxatoms);
            //return new InfoReshapeByAtomGroupLeftRight
            //{
            //    hess                  = nhess,
            //    idxatoms              = idxatoms.ToArray(),
            //    numleft               = numleft,
            //    numright              = numright,
            //    lstGroupNIdxLeftRight = lstGroupNIdxLeftRight,
            //};
        }
    }
}
