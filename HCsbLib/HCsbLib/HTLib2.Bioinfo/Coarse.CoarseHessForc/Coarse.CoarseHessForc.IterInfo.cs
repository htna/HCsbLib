using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Coarse
    {
        public static partial class CoarseHessForc
        {
            [Serializable]
            public class IterInfo
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
            public class HessForcInfo
            {
                public List<IterInfo> iterinfos = null;

                public object[]     atoms   = null;
                public Vector       mass    = null;
                public Vector[]     coords  = null;
                public HessMatrix   hess    = null;
                public Vector       forc    = null;

                public static HessForcInfo From(Hess.HessInfo hessinfo)
                {
                    return new HessForcInfo{
                        atoms  = hessinfo.atoms ,
                        mass   = hessinfo.mass  ,
                        coords = hessinfo.coords,
                        hess   = hessinfo.hess  ,
                    };
                }
                public static HessForcInfo From(Hess.HessInfo hessinfo, Vector forc)
                {
                    return new HessForcInfo{
                        atoms  = hessinfo.atoms ,
                        mass   = hessinfo.mass  ,
                        coords = hessinfo.coords,
                        hess   = hessinfo.hess  ,
                        forc   = forc           ,
                    };
                }
            }
            public class CGetHessCoarseResiIterImpl
            {
                public List<IterInfo> iterinfos = null;
                public HessMatrix H = null;
            };
            public delegate Tuple<int[], int[][]> FuncGetIdxKeepListRemv(object[] atoms, Vector[] coords);
        }
    }
}
