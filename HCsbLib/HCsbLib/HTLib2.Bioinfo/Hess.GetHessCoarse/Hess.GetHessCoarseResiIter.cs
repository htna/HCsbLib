using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public static partial class HessStatic
    {
        public static int []   ListNumAtomsRemoved(this IList<Hess.HessCoarseResiIter.HessCoarseResiIterInfo> iterinfos) { var values = new List<int   >(); foreach(var info in iterinfos) values.Add(info.numAtomsRemoved); return values.ToArray(); }
        public static int []   ListNumSetZeroBlock(this IList<Hess.HessCoarseResiIter.HessCoarseResiIterInfo> iterinfos) { var values = new List<int   >(); foreach(var info in iterinfos) values.Add(info.numSetZeroBlock); return values.ToArray(); }
        public static int []   ListNumNonZeroBlock(this IList<Hess.HessCoarseResiIter.HessCoarseResiIterInfo> iterinfos) { var values = new List<int   >(); foreach(var info in iterinfos) values.Add(info.numNonZeroBlock); return values.ToArray(); }
        public static int []   ListNumAddIgnrBlock(this IList<Hess.HessCoarseResiIter.HessCoarseResiIterInfo> iterinfos) { var values = new List<int   >(); foreach(var info in iterinfos) values.Add(info.numAddIgnrBlock); return values.ToArray(); }
        public static long[]   ListUsedMemoryByte (this IList<Hess.HessCoarseResiIter.HessCoarseResiIterInfo> iterinfos) { var values = new List<long  >(); foreach(var info in iterinfos) values.Add(info.usedMemoryByte ); return values.ToArray(); }
        public static double[] ListCompSec        (this IList<Hess.HessCoarseResiIter.HessCoarseResiIterInfo> iterinfos) { var values = new List<double>(); foreach(var info in iterinfos) values.Add(info.compSec        ); return values.ToArray(); }
    }
    public partial class Hess
    {
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , HessCoarseResiIter.FuncGetIdxKeepListRemvObject GetIdxKeepListRemv
            , ILinAlg ila
            , double thres_zeroblk=0.001
            , HessCoarseResiIter.IterOption iteropt = HessCoarseResiIter.IterOption.Matlab_experimental
            , string[] options=null
            )
        {
            return HessCoarseResiIter.GetHessCoarseResiIter
            ( hessinfo          : hessinfo
            , coords            : coords
            , GetIdxKeepListRemv: GetIdxKeepListRemv
            , ila               : ila
            , thres_zeroblk     : thres_zeroblk
            , iteropt           : iteropt 
            , options           : options
            );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv
            , ILinAlg ila
            , double thres_zeroblk=0.001
            , HessCoarseResiIter.IterOption iteropt = HessCoarseResiIter.IterOption.Matlab_experimental
            , string[] options=null
            )
        {
            return HessCoarseResiIter.GetHessCoarseResiIter
            ( hessinfo          : hessinfo
            , coords            : coords
            , GetIdxKeepListRemv: GetIdxKeepListRemv
            , ila               : ila
            , thres_zeroblk     : thres_zeroblk
            , iteropt           : iteropt 
            , options           : options
            );
        }


        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_BlockWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , ILinAlg ila
            , int clus_width       ///= 18    | (18,500) is good for large sparse proteins
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            )
        {
            string[] nameToKeep = new string[] { "CA" };
            return GetHessCoarseResiIter_BlockWise
                ( hessinfo
                , coords
                , ila
                , clus_width       
                , num_atom_merge   
                , thres_zeroblk 
                , nameToKeep
                );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_BlockWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , ILinAlg ila
            , int clus_width       ///= 18    | (18,500) is good for large sparse proteins
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            , IList<Universe.Atom> keeps
            , params string[] options
            )
        {
            if(clus_width <= 0)
                throw new Exception("clus_width should be > 0");

            HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv = delegate(Universe.Atom[] latoms, Vector[] lcoords)
            {
                return HessCoarseResiIter.GetIdxKeepListRemv_ResiCluster2(latoms, lcoords, clus_width, num_atom_merge, keeps);
            };
            return HessCoarseResiIter.GetHessCoarseResiIter
                ( hessinfo, coords, GetIdxKeepListRemv, ila, thres_zeroblk
                , HessCoarseResiIter.IterOption.Matlab_experimental
                , options:options
                );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_BlockWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , ILinAlg ila
            , int clus_width       ///= 18    | (18,500) is good for large sparse proteins
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            , IList<Universe.Atom> keeps
            , HessCoarseResiIter.IterOption iteropt
            , params string[] options
            )
        {
            if(clus_width <= 0)
                throw new Exception("clus_width should be > 0");

            HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv = delegate(Universe.Atom[] latoms, Vector[] lcoords)
            {
                return HessCoarseResiIter.GetIdxKeepListRemv_ResiCluster2(latoms, lcoords, clus_width, num_atom_merge, keeps);
            };
            return HessCoarseResiIter.GetHessCoarseResiIter
                ( hessinfo, coords, GetIdxKeepListRemv, ila, thres_zeroblk
                , iteropt
                , options:options
                );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_BlockWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , ILinAlg ila
            , int clus_width       ///= 18    | (18,500) is good for large sparse proteins
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            , string[] nameToKeep
            , params string[] options
            )
        {
            if(clus_width <= 0)
                throw new Exception("clus_width should be > 0");

            HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv = delegate(Universe.Atom[] latoms, Vector[] lcoords)
            {
                return HessCoarseResiIter.GetIdxKeepListRemv_ResiCluster2(latoms, lcoords, clus_width, num_atom_merge, nameToKeep);
            };
            return HessCoarseResiIter.GetHessCoarseResiIter
                ( hessinfo, coords, GetIdxKeepListRemv, ila, thres_zeroblk
                , HessCoarseResiIter.IterOption.Matlab_experimental
                , options:options
                );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_SymrcmBlockWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , double? symrcm_filter_blckwise_interact // null (use 1 as default)
            , ILinAlg ila
            , int clus_width       ///= 18    | (18,500) is good for large sparse proteins
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            , IList<Universe.Atom> keeps
            , params string[] options
            )
        {
            if(clus_width <= 0)
                throw new Exception("clus_width should be > 0");

            HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv = delegate(Universe.Atom[] latoms, Vector[] lcoords)
            {
                return HessCoarseResiIter.GetIdxKeepListRemv_ResiCluster_SymrcmBlockWise(latoms, lcoords, hessinfo.hess, clus_width, num_atom_merge, symrcm_filter_blckwise_interact, keeps);
            };
            return HessCoarseResiIter.GetHessCoarseResiIter
                ( hessinfo, coords, GetIdxKeepListRemv, ila, thres_zeroblk
                , HessCoarseResiIter.IterOption.Matlab_experimental
                , options:options
                );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_SymrcmBlockWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , double? symrcm_filter_blckwise_interact // null (use 1 as default)
            , ILinAlg ila
            , int clus_width       ///= 18    | (18,500) is good for large sparse proteins
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            , string[] nameToKeep
            )
        {
            if(clus_width <= 0)
                throw new Exception("clus_width should be > 0");

            HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv = delegate(Universe.Atom[] latoms, Vector[] lcoords)
            {
                return HessCoarseResiIter.GetIdxKeepListRemv_ResiCluster_SymrcmBlockWise(latoms, lcoords, hessinfo.hess, clus_width, num_atom_merge, symrcm_filter_blckwise_interact, nameToKeep);
            };
            return HessCoarseResiIter.GetHessCoarseResiIter
                ( hessinfo, coords, GetIdxKeepListRemv, ila, thres_zeroblk
                , HessCoarseResiIter.IterOption.Matlab_experimental
                );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_SymrcmResiWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , ILinAlg ila
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            , string[] nameToKeep
            )
        {
            HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv = delegate(Universe.Atom[] latoms, Vector[] lcoords)
            {
                return HessCoarseResiIter.GetIdxKeepListRemv_ResiCluster_SymrcmResiWise(latoms, lcoords, hessinfo.hess, num_atom_merge, thres_zeroblk, nameToKeep);
            };
            return HessCoarseResiIter.GetHessCoarseResiIter
                ( hessinfo, coords, GetIdxKeepListRemv, ila, thres_zeroblk
                , HessCoarseResiIter.IterOption.Matlab_experimental
                );
        }
        public static HessCoarseResiIter.HessInfoCoarseResiIter GetHessCoarseResiIter_SymrcmAtomWise
            ( Hess.HessInfo hessinfo
            , Vector[] coords
            , ILinAlg ila
            , int num_atom_merge   ///= 500   | (14,400) is good for small densely packed globular proteins
            , double thres_zeroblk ///= 0.001 | 0.001 could be fairly good for ssNMA (possibly for sbNMA and NMA, too)
            , string[] nameToKeep
            )
        {
            HessCoarseResiIter.FuncGetIdxKeepListRemv GetIdxKeepListRemv = delegate(Universe.Atom[] latoms, Vector[] lcoords)
            {
                return HessCoarseResiIter.GetIdxKeepListRemv_ResiCluster_SymrcmAtomWise(latoms, lcoords, hessinfo.hess, num_atom_merge, thres_zeroblk, nameToKeep);
            };
            return HessCoarseResiIter.GetHessCoarseResiIter
                ( hessinfo, coords, GetIdxKeepListRemv, ila, thres_zeroblk
                , HessCoarseResiIter.IterOption.Matlab_experimental
                );
        }


        public static partial class HessCoarseResiIter
        {
            [Serializable]
            public class HessCoarseResiIterInfo
            {
                public int  sizeHessBlkMat ;    // number of atoms in A
                public int  numAtomsRemoved;    // number of atoms removed in D
                public int  numSetZeroBlock;    // number of set-as-zero blocks in C
                public int  numNonZeroBlock;    // number of non-zero blocks in C, after applying numSetZeroBlock
                public int  numAddIgnrBlock;    // ignored adding when A <- A - B*invD*C because the element B(B*
                public int[] idxkeep;
                public int[] idxremv;
                public long usedMemoryByte;
                public DateTime time0;
                public DateTime time1;
                public TimeSpan compTime { get { return (time1 - time0); } }
                public double   compSec  { get { return compTime.TotalSeconds; } }
                public override string ToString()
                {
                    string str="";
                    str += string.Format("atoms({0,6})", sizeHessBlkMat);
                    str += string.Format(", remAtom({0,6})", numAtomsRemoved);
                    str += string.Format(", C_setZeroBlk({0,6})", numSetZeroBlock);
                    str += string.Format(", C_nnzBlk({0,6})", numNonZeroBlock);
                    double C_nnzratio = ((double)numNonZeroBlock) / (sizeHessBlkMat * numAtomsRemoved);
                    str += string.Format(", C_nnzratio({0:0.00000008})", C_nnzratio);
                    return str;
                }
            }
            public class HessInfoCoarseResiIter : HessInfo
            {
                public List<HessCoarseResiIterInfo> iterinfos = null;
            }
            public class CGetHessCoarseResiIterImpl
            {
                public List<HessCoarseResiIterInfo> iterinfos = null;
                public HessMatrix H = null;
            };
            public delegate Tuple<int[], int[][]> FuncGetIdxKeepListRemvObject(object[] atoms, Vector[] coords);
            public delegate Tuple<int[], int[][]> FuncGetIdxKeepListRemv(Universe.Atom[] atoms, Vector[] coords);
        }
    }
}
