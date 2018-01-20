using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        public static Mode[] GetModes(MatrixByArr hess, string cachepath = null)
        {
            Vector[] eigvec;
            double[] eigval;
            if(cachepath != null && HFile.Exists(cachepath))
            {
                HSerialize.Deserialize(cachepath, null, out eigval, out eigvec);
            }
            else
            {
                HDebug.Verify(NumericSolver.Eig(hess, out eigvec, out eigval));
                HSerialize.SerializeDepreciated(cachepath, null, eigval, eigvec);
            }

            List<Mode> modes;
            {   // sort by eigenvalues
                int[] idx = eigval.HAbs().HIdxSorted();
                modes = new List<Mode>(idx.Length);
                for(int i=0; i<eigval.Length; i++)
                {
                    Mode mode = new Mode{eigval = eigval[idx[i]],
                                         eigvec = eigvec[idx[i]]
                                        };
                    modes.Add(mode);
                }
            }

            return modes.ToArray();
        }
        //public static Mode[] GetModes(Matrix hessMassWeighted, double[] mass, string cachepath = null)
        //{
        //    int size = mass.Length;
        //    Debug.Assert(hessMassWeighted.RowSize==size*3, hessMassWeighted.ColSize==size*3);
        //
        //    Vector[] eigvec;
        //    double[] eigval;
        //    if(cachepath != null && File.Exists(cachepath))
        //    {
        //        Serializer.Deserialize(cachepath, out eigval, out eigvec);
        //    }
        //    else
        //    {
        //        Debug.Verify(NumericSolver.Eig(hessMassWeighted, out eigvec, out eigval));
        //        Serializer.Serialize(cachepath, eigval, eigvec);
        //    }
        //
        //    List<Mode> modes;
        //    {   // sort by eigenvalues
        //        int[] idx = eigval.Abs().IdxSorted();
        //        modes = new List<Mode>(idx.Length);
        //        for(int i=0; i<eigval.Length; i++)
        //        {
        //            Mode mode = new Mode
        //            {
        //                eigval = eigval[idx[i]],
        //                eigvec = eigvec[idx[i]]
        //            };
        //            modes.Add(mode);
        //        }
        //    }
        //
        //    return modes.ToArray();
        //}
    }
}
