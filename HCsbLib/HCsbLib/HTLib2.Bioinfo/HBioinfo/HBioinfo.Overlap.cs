using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        public static Matrix OverlapSigned(IList<Mode> modes1, IList<Mode> modes2, ILinAlg ila, bool bResetUnitVector)
        {
            return OverlapSigned(modes1, null, null, modes2, ila, bResetUnitVector);
        }
        public static Matrix OverlapSigned(IList<Mode> modes1, double[] mass1, double[] mass2, IList<Mode> modes2, ILinAlg ila, bool bResetUnitVector)
        {
            Matrix mat1;
            Matrix mat2;
            string mat12opt = "memory-save";
            switch(mat12opt)
            {
                case "initial":
                    {
                        Vector[] eigvecs1 = new Vector[modes1.Count]; for(int i=0; i<modes1.Count; i++) eigvecs1[i] = modes1[i].eigvec.Clone();
                        Vector[] eigvecs2 = new Vector[modes2.Count]; for(int i=0; i<modes2.Count; i++) eigvecs2[i] = modes2[i].eigvec.Clone();
                        {
                            // 1. Hess
                            // 2. mwHess <- M^-0.5 * H * M^-0.5
                            // 3. [V,D]  <- eig(mwHess)
                            // 4. mrMode <- M^-0.5 * V
                            //
                            // overlap: vi . vj
                            // 1. vi <- M^0.5 * mrMode_i
                            // 2. vj <- M^0.5 * mrMode_j
                            // 3. vi.vj <- dot(vi,vj)
                            //
                            //         [ 2           ]   [4]   [2*4]   [ 8]
                            //         [   2         ]   [5]   [2*5]   [10]
                            // M * v = [     2       ] * [6] = [2*6] = [12]
                            //         [       3     ]   [7]   [3*7]   [21]
                            //         [         3   ]   [8]   [3*8]   [24]
                            //         [           3 ]   [9]   [3*9]   [27]
                            //
                            // V1 <- sqrt(M1) * V1
                            // V2 <- sqrt(M2) * V2
                            //                           1. get sqrt(mass)          2. for each eigenvector              3 eigveci[j] = eigvec[j] * sqrt_mass[j/3]
                            if(mass1 != null) { double[] mass1sqrt = mass1.HSqrt(); for(int i=0; i<eigvecs1.Length; i++) for(int j=0; j<eigvecs1[i].Size; j++) eigvecs1[i][j] *= mass1sqrt[j/3]; }
                            if(mass2 != null) { double[] mass2sqrt = mass2.HSqrt(); for(int i=0; i<eigvecs2.Length; i++) for(int j=0; j<eigvecs2[i].Size; j++) eigvecs2[i][j] *= mass2sqrt[j/3]; }
                        }
                        if(bResetUnitVector)
                        {
                            for(int i=0; i<modes1.Count; i++) eigvecs1[i] = eigvecs1[i].UnitVector();
                            for(int i=0; i<modes2.Count; i++) eigvecs2[i] = eigvecs2[i].UnitVector();
                        }

                        mat1 = eigvecs1.ToMatrix(false); HDebug.Assert(mat1.ColSize == eigvecs1.Length); eigvecs1 = null; GC.Collect(0);
                        mat2 = eigvecs2.ToMatrix(true ); HDebug.Assert(mat2.RowSize == eigvecs2.Length); eigvecs2 = null; GC.Collect(0);
                    }
                    break;
                case "memory-save":
                    {
                        int vecsize = modes1[0].eigvec.Size;
                        HDebug.Exception(vecsize == modes1[0].eigvec.Size);
                        HDebug.Exception(vecsize == modes2[0].eigvec.Size);

                        //Vector[] eigvecs1 = new Vector[modes1.Count]; for(int i=0; i<modes1.Count; i++) eigvecs1[i] = modes1[i].eigvec.Clone();
                        //if(mass1 != null) { double[] mass1sqrt = mass1.HSqrt(); for(int i=0; i<modes1.Count; i++) for(int j=0; j<eigvecs1[i].Size; j++) eigvecs1[i][j] *= mass1sqrt[j/3]; }
                        //if(bResetUnitVector) for(int i=0; i<modes1.Count; i++) eigvecs1[i] = eigvecs1[i].UnitVector();
                        //mat1 = eigvecs1.ToMatrix(false); HDebug.Assert(mat1.ColSize == modes1.Count); eigvecs1 = null; GC.Collect(0);
                        mat1 = Matrix.Zeros(modes1.Count, vecsize     );
                        double[] mass1sqrt = null; if(mass1 != null) mass1sqrt = mass1.HSqrt();
                        for(int i=0; i<modes1.Count; i++)
                        {
                            Vector eigvecs1i = modes1[i].eigvec.Clone();
                            if(mass1 != null) { for(int j=0; j<eigvecs1i.Size; j++) eigvecs1i[j] *= mass1sqrt[j/3]; }
                            if(bResetUnitVector) eigvecs1i = eigvecs1i.UnitVector();
                            for(int j=0; j<eigvecs1i.Size; j++) mat1[i, j] = eigvecs1i[j];
                        }
                        HDebug.Assert(mat1.ColSize == modes1.Count);
                        GC.Collect(0);

                        //Vector[] eigvecs2 = new Vector[modes2.Count]; for(int i=0; i<modes2.Count; i++) eigvecs2[i] = modes2[i].eigvec.Clone();
                        //if(mass2 != null) { double[] mass2sqrt = mass2.HSqrt(); for(int i=0; i<modes2.Count; i++) for(int j=0; j<eigvecs2[i].Size; j++) eigvecs2[i][j] *= mass2sqrt[j/3]; }
                        //if(bResetUnitVector) for(int i=0; i<modes2.Count; i++) eigvecs2[i] = eigvecs2[i].UnitVector();
                        //mat2 = eigvecs2.ToMatrix(true ); HDebug.Assert(mat2.RowSize == modes2.Count); eigvecs2 = null; GC.Collect(0);
                        mat2 = Matrix.Zeros(vecsize, modes2.Count);
                        double[] mass2sqrt = null; if(mass2 != null) mass2sqrt = mass2.HSqrt();
                        for(int i=0; i<modes2.Count; i++)
                        {
                            Vector eigvecs2i = modes2[i].eigvec.Clone();
                            if(mass2 != null) { for(int j=0; j<eigvecs2i.Size; j++) eigvecs2i[j] *= mass2sqrt[j/3]; }
                            if(bResetUnitVector) eigvecs2i = eigvecs2i.UnitVector();
                            for(int j=0; j<eigvecs2i.Size; j++) mat2[j, i] = eigvecs2i[j];
                        }
                        HDebug.Assert(mat2.RowSize == modes2.Count);
                        GC.Collect(0);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            HDebug.Assert(mat1.RowSize == mat2.ColSize);
            Matrix overlap = null;
            if(ila != null) overlap = ila.Mul(mat1, mat2);
            else            overlap = Matlab.ila.Mul(mat1, mat2);
            mat1 = mat2 = null;
            GC.Collect(0);
            //overlap.UpdateAbs();

            if(HDebug.IsDebuggerAttached)
            {
                double[] sum_c2 = new double[overlap.ColSize];
                double[] sum_r2 = new double[overlap.RowSize];
                for(int c=0; c<overlap.ColSize; c++)
                    for(int r=0; r<overlap.RowSize; r++)
                    {
                        double v = overlap[c,r];
                        double v2 = v*v;
                        HDebug.Assert(v2 <= 1.00000000001);
                        sum_c2[c] += v2;
                        sum_r2[r] += v2;
                    }
                for(int c=0; c<overlap.ColSize; c++) HDebug.AssertTolerance(0.00001, sum_c2[c] - 1.0);
                for(int r=0; r<overlap.RowSize; r++) HDebug.AssertTolerance(0.00001, sum_r2[r] - 1.0);
            }

            return overlap;
        }
        public class OOverlapWeighted
        {
            public Matrix     soverlap1to2;
            public double[]   weights;
            public double     woverlap;
            public double[]    overlaps;
            public double[] sgnoverlaps;
            public int[]    idxOverlap1to2;

            public Mode[] modes1;
            public Mode[] modes2;

            public int[]    idxOverlap2to1
            {
                get
                {
                    /// -1          : there is no matching pair
                    /// int.MinValue: there are many matching pairs
                    /// >=0         : index of mode1, that is best matching to mode2
                    int[] idxs2 = new int[modes2.Length];
                    idxs2 = idxs2.HUpdateValueAll(-1);
                    for(int i1=0; i1<idxOverlap1to2.Length; i1++)
                    {
                        int i2 = idxOverlap1to2[i1];
                        switch(idxs2[i2])
                        {
                            case -1          : idxs2[i2] = i1;           break; /// first assignment
                            case int.MinValue: idxs2[i2] = int.MinValue; break; /// there are 1(idx1) -- n(idx2) connections
                            default:
                                if(idxs2[i2] < 0) throw new HException();       /// exception
                                idxs2[i2] = int.MinValue;                       /// there are 1(idx1) -- 2(idx2) connections
                                break;
                        }
                    }
                    return idxs2;
                }
            }
        }
        public static OOverlapWeighted OverlapWeightedByEigval(IList<Mode> modes1, IList<Mode> modes2, ILinAlg ila, bool bResetUnitVector, string optSelectOverlap)
        {
            return OverlapWeightedByEigval(modes1, null, null, modes2, ila, bResetUnitVector, optSelectOverlap);
        }
        public static OOverlapWeighted OverlapWeightedByEigval(IList<Mode> modes1, double[] mass1, double[] mass2, IList<Mode> modes2, ILinAlg ila, bool bResetUnitVector, string optSelectOverlap)
        {
            Vector weights = new double[modes1.Count];
            for(int i=0; i<weights.Size; i++)
                weights[i] = 1/Math.Abs(modes1[i].eigval);
            weights /= weights.Sum();

            return OverlapWeighted(modes1, mass1, mass2, modes2, weights.ToArray(), ila, bResetUnitVector, optSelectOverlap);
        }
        //public static OOverlapWeighted OverlapWeightedByFreq(IList<Mode> modes1, IList<Mode> modes2, ILinAlg ila, bool bResetUnitVector, string optSelectOverlap)
        //{
        //    Vector weights = new double[modes1.Count];
        //    for(int i=0; i<weights.Size; i++)
        //        weights[i] = 1/Math.Sqrt(Math.Abs(modes1[i].eigval));
        //    weights /= weights.Sum();
        //
        //    return OverlapWeighted(modes1, modes2, weights.ToArray(), ila, bResetUnitVector, optSelectOverlap);
        //}
        public static OOverlapWeighted OverlapWeighted(IList<Mode> modes1, double[] mass1, double[] mass2, IList<Mode> modes2, IList<double> weights, ILinAlg ila, bool bResetUnitVector, string optSelectOverlap)
        {
            Matrix soverlap = OverlapSigned(modes1, mass1, mass2, modes2, ila, bResetUnitVector);

            HDebug.Exception(modes1.Count == weights.Count);

            int[] idxOverlap1to2;
            switch(optSelectOverlap)
            {
                case "corresponding index":
                    if(modes1.Count != modes2.Count)
                        throw new HException();
                    idxOverlap1to2 = HEnum.HEnumCount(modes1.Count).ToArray();
                    break;
                case "best matching overlap":
                    idxOverlap1to2 = new int[modes1.Count];
                    for(int im=0; im<modes1.Count; im++)
                    {
                        idxOverlap1to2[im] = 0;
                        for(int k=0; k<modes2.Count; k++)
                            if(Math.Abs(soverlap[im,idxOverlap1to2[im]]) < Math.Abs(soverlap[im,k]))
                                idxOverlap1to2[im] = k;
                    }
                    break;
                default:
                    throw new HException();
            }

            Vector overlap1to2 = new double[modes1.Count];
            Vector overlap1to2signed = new double[modes1.Count];
            double woverlap = 0;
            for(int i=0; i<modes1.Count; i++)
            {
                overlap1to2      [i] = Math.Abs(soverlap[i, idxOverlap1to2[i]]);
                overlap1to2signed[i] =          soverlap[i, idxOverlap1to2[i]];
                woverlap += weights[i] * overlap1to2[i];
            }

            return new OOverlapWeighted
            {
                soverlap1to2   = soverlap,
                woverlap       = woverlap,
                weights        = weights.ToArray(),
                overlaps       = overlap1to2.ToArray(),
                sgnoverlaps    = overlap1to2signed.ToArray(),
                idxOverlap1to2 = idxOverlap1to2,
                modes1         = modes1.ToArray(),
                modes2         = modes2.ToArray(),
            };
        }
    }
}
