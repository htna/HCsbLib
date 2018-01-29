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
        public static partial class CoarseHessForcIter
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
            public class HessForcInfoIter : HessForcInfo
            {
                public List<IterInfo> iterinfos = null;
            }
            public delegate Tuple<int[], int[][]> FuncGetIdxKeepListRemv(object[] atoms, Vector[] coords);
        }
        }
    }
}
