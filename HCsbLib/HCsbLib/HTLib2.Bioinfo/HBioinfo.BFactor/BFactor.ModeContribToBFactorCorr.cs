using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public static partial class HBioinfo
{
    public partial class BFactor
    {
        public static Vector GetBFactorNormalized(Vector bfactor)
        {
            /// nbfactor (normalized bfactor) has mean     0
            ///                               and variance 1.
            /// The correlation (corr) of two normalized bfactors (nbfactor1, nbfactor2) is the dot product of them:
            ///   corr = Math.HCorr( bfactor1,  bfactor2)
            ///        = Math.HCorr(nbfactor1, nbfactor2)
            ///        = LinAlg.VtV(nbfactor1, nbfactor2)            (for   biased estimation)
            ///        = LinAlg.VtV(nbfactor1, nbfactor2) / (n-1)    (for unbiased estimation)
            ///        
            double avg = bfactor.HAvg();
            double var = bfactor.HVar();
            double std = Math.Sqrt(var);
            int n = bfactor.Size;
            Vector nbfactor = new double[n];
            for(int i=0; i<n; i++)
                nbfactor[i] = (bfactor[i] - avg)/std;
            HDebug.AssertTolerance(0.00000001, nbfactor.HAvg());
            HDebug.AssertTolerance(0.00000001, nbfactor.HVar() - 1);
            return nbfactor;
        }
        public static Vector[] GetBFactorModewise(IList<Mode> modes)
        {
            /// The "b-factor of modes (m1,m2,m3,...)" is the same to
            /// the "b-factor of m1" + "b-factor of m2" + "b-factor of m3" + ...
            /// 
            int m = modes.Count;
            Vector[] mwbfactors = new Vector[m];
            for(int i=0; i<m; i++)
            {
                mwbfactors[i] = (new Mode[] { modes[i] }).GetBFactor().ToArray();
            }
            
            if(HDebug.IsDebuggerAttached)
            {
                Vector bfactor0 = modes.GetBFactor().ToArray();
                Vector bfactor1 = mwbfactors.HSum();
                HDebug.AssertTolerance(0.00000001, bfactor0-bfactor1);
            }

            return mwbfactors;
        }
        public static Vector[] GetBFactorModewiseNormalized(IList<Mode> modes)
        {
            /// normalized bfactor (nbfactor) of bfactor is determined as follows:
            ///     nbfactor = (bfactor - mean(bfactor)) / sqrt(var(bfactor))
            ///              = (bfactor - mean(bfactor)) / sd(bfactor))
            /// 
            /// bfactor of modes(m1,m2,...) is the sum of "bfactor of m1", "bfactor of m2", ...:
            ///     bfactor(modes) = bfactor(m1) + bfactor(m2) + bfactor(m3) + ...
            /// 
            /// Therefore, the normalized bfactor (nbfactor) of modes is the same to
            ///            the sum of "normalized bfactor of m1", "normalized bfactor of m2", ...:
            ///     normal.bfactor(modes) = (modes.bfactor                 - mean(modes.bfactor                      )) / sd(modes.bfactor))
            ///                           = meanzero(modes.bfactor) / sd(modes.bfactor)                                                      : meanzero make it have zeromean
            ///                           = nbfactor(meanzero(modes.bfactor))                                                                : nbfactor divide it by sd
            ///                           = (modes.bfactor                 - mean(modes.bfactor                      )) / sd(modes.bfactor))
            ///                           = (m1.bfactor + m2.bfactor + ... - mean(m1.bfactor + m2.bfactor + ...      )) / sd(modes.bfactor))
            ///                           = ((m1.bfactor-mean(m1.bfactor)) + (m2.bfactor-mean(m2.bfactor))  + ...) / sd(modes.bfactor))
            ///                           = (meanzero(m1.bfactor)          + meanzero(m2.bfactor)           + ...) / sd(modes.bfactor))
            ///                           = meanzero(m1.bfactor)/sd        + meanzero(m2.bfactor)/sd        + ...
            ///                           = nbfactor(meanzero(m1.bfactor)) + nbfactor(meanzero(m2.bfactor)) + ...
            ///                           = noral.modewise.bfactor(m1)     + noral.modewise.bfactor(m2)     + ...
            ///                           = nmwbfactor1                    + nmwbfactor2                    + ...
            /// where mean = mean(bfactor)
            ///       sd   = sd(bfactor)=sqrt(var(bfactor))
            ///       meanzero(v) := v-mean(v) : make v become zero-mean
            ///       nbfactor(v) := v/sd
            /// 
            Vector[] mwbfactors = GetBFactorModewise(modes);

            int n = mwbfactors[0].Size;
            int m = mwbfactors.Length;

            Vector bfactor = mwbfactors.HSum();
            HDebug.AssertTolerance(0.00000001, bfactor - modes.GetBFactor().ToArray());
            double mean = bfactor.HAvg();
            double var = bfactor.HVar();
            double sd  = Math.Sqrt(var);
            double[] eigvals = modes.ListEigval().ToArray();
            double   eigvals_sum = eigvals.HAbs().Sum();

            Vector[] nmwbfactors = new Vector[m];
            for(int i=0; i<m; i++)
            {
                HDebug.Assert(mwbfactors[i].Size == n);
                Vector  mwbfactori = mwbfactors[i];
                Vector nmwbfactori = new double[n];
                double meani = mwbfactori.HAvg();                       // (1)
                //double meani = mean/m;                                  // (2)
                //double meani = mean*Math.Abs(eigvals[i])/(eigvals_sum); // (3)
                /// There is no different patterns between (1), (2), and (3).
                /// This is because (2) just decrease the vector tolerant amound: "mean/m" becomes very small when "m=# of modes of models" is large
                ///                                                             : usually m=3*n-6>200.
                /// But, (1) shows the higher contribution in low frequency mode thatn (2) shows : 0.250->0.201->0.250 in blue line in PC1 {(1)->(2)->(3)}
                ///                                                                              , 0.122->0.102->0.124 in PC2
                ///                                                                              , 0.064->0.056->0.070 in PC3
                /// Therefore, (1) is the better choice in terms i) more highlight lower frequency correlation
                ///                                              ii) its equation is simpler
                ///                                              than (2) and (3)
                for(int j=0; j<n; j++)
                    nmwbfactori[j] = (mwbfactori[j] - meani)/sd;
                HDebug.AssertTolerance(0.00000001, nmwbfactori.HAvg()-0); // check zero-mean in (1)
                nmwbfactors[i] = nmwbfactori;
            }

            if(HDebug.IsDebuggerAttached)
            {
                Vector nbfactor1 = GetBFactorNormalized(modes.GetBFactor().ToArray());
                Vector nbfactor2 = nmwbfactors.HSum();
                HDebug.AssertTolerance(0.00000001, nbfactor1-nbfactor2);
            }

            return nmwbfactors;
        }
        public static double[] ModeContribToBFactorCorr(Vector bfactor1, IList<Mode> modes2)
        {
            /// the correlation(corr) of two bfactors (bfactor1, bfactor 2) is the same to
            /// the dot product of their normalizeds (mean 0, variance 1):
            ///   corr = Math.HCorr( bfactor1,  bfactor2)
            ///        = Math.HCorr(nbfactor1, nbfactor2)
            ///        = LinAlg.VtV(nbfactor1, nbfactor2)            (for   biased estimation)
            ///        = LinAlg.VtV(nbfactor1, nbfactor2) / (n-1)    (for unbiased estimation)
            /// 
            /// bfactor of mode(m1,m2,...) can be determined by the sum of bfactors of each mode:
            ///     bfactor(mode) = bfactor(m1) + bfactor(m2) + ...
            /// 
            /// in the similar manner, the normalized bfactor of mode(m1,m2,...) is determined by
            /// sum of normalized-modewise-bfactor (GetBFactorModewiseNormalized):
            ///     normal-bfactor(mode) = nmwbfactor(m1) + nmwbfactor(m2) + ...
            /// 
            /// therefore, the correlation of bfactor1 and bfactor(modes2) is determined as their normalized dot-product:
            ///     corr = dot(nbfactor1, nbfactor2)                                                                           / (n-1)
            ///          = dot(nbfactor1, nmwbfactor(modes2.m1)        + nmwbfactor(modes2.m2)                         + ... ) / (n-1)
            ///          = dot(nbfactor1, nmwbfactor(modes2.m1))/(n-1) + dot(nbfactor1, nmwbfactor(modes2.m2)) / (n-1) + ...
            ///        
            Vector   nbfactor1 = GetBFactorNormalized(bfactor1);
            Vector[] nbfactor2mw = GetBFactorModewiseNormalized(modes2);

            int n = bfactor1.Size;
            int m = modes2.Count;
            HDebug.Assert(nbfactor2mw.Length == m);

            Vector contrib = new double[m];
            for(int i=0; i<m; i++)
                contrib[i] = LinAlg.VtV(nbfactor1, nbfactor2mw[i])/(n-1);

            if(HDebug.IsDebuggerAttached)
            {
                Vector  bfactor2 = modes2.GetBFactor().ToArray();
                Vector nbfactor2 = GetBFactorNormalized(bfactor2);
                double corr0 = HMath.HCorr(bfactor1, bfactor2);
                double corr1 = LinAlg.VtV(nbfactor1, nbfactor2) / (n-1);
                double corr2 = contrib.Sum();
                HDebug.AssertTolerance(0.00000001, corr0-corr1, corr0-corr2, corr1-corr2);
            }

            return contrib;
        }
        public static double[,] ModeContribToBFactorCorr(IList<Mode> modes1, IList<Mode> modes2)
        {
            /// Similar to "double[] ModeContribToBFactorCorr(bfactor1, modes2)",
            /// the correlation between bfactor1 and bfactor 2 can be decomposed as
            /// the list of correlation contributions by "normalized bfactor by modes1.m_i"
            ///                                      and "normalized bfactor by modes2.m_j"
            /// in a matrix form.
            /// 
            /// therefore, the correlation of bfactor1 and bfactor(modes2) is determined as their normalized dot-product:
            ///     corr = dot(nbfactor1, nbfactor2) / (n-1)
            ///          = 1/(n-1) *   dot(nbfactor1, nmwbfactor(modes2.m1)                 + nmwbfactor(modes2.m2)  + ... )
            ///          = 1/(n-1) * ( dot(nbfactor1, nmwbfactor(modes2.m1)) + dot(nbfactor1, nmwbfactor(modes2.m2)) + ... )
            ///          = 1/(n-1) * ( dot(nmwbfactor(modes1.m1), nmwbfactor(modes2.m1)) + dot(nmwbfactor(modes1.m1), nmwbfactor(modes2.m2)) + ... 
            ///                        dot(nmwbfactor(modes1.m2), nmwbfactor(modes2.m1)) + dot(nmwbfactor(modes1.m2), nmwbfactor(modes2.m2)) + ... )
            ///          = 1/(n-1) * sum_{ i1=1..m1, i2=1..m2 } dot(nmwbfactor(modes1.m_i), nmwbfactor(modes2.m_j))
            ///          = sum_{ i1=1..m1, i2=1..m2 } dot(nmwbfactor(modes1.m_i), nmwbfactor(modes2.m_j))/(n-1)
            ///          = sum_{ i1=1..m1, i2=1..m2 } "correlation contribution by modes1.m_i and modes2.m_j"
            ///        
            Vector[] nbfactor1mw = GetBFactorModewiseNormalized(modes1);
            Vector[] nbfactor2mw = GetBFactorModewiseNormalized(modes2);

            int n = nbfactor1mw[0].Size;
            int m1 = modes1.Count; HDebug.Assert(nbfactor1mw.Length == m1);
            int m2 = modes2.Count; HDebug.Assert(nbfactor2mw.Length == m2);

            MatrixByArr contrib = new double[m1,m2];
            for(int i1=0; i1<m1; i1++)
                for(int i2=0; i2<m2; i2++)
                {
                    HDebug.Assert(nbfactor1mw[i1].Size == n);
                    HDebug.Assert(nbfactor2mw[i2].Size == n);
                    contrib[i1,i2] = LinAlg.VtV(nbfactor1mw[i1], nbfactor2mw[i2])/(n-1);
                }

            if(HDebug.IsDebuggerAttached)
            {
                Vector  bfactor1 = modes1.GetBFactor().ToArray();
                Vector nbfactor1 = GetBFactorNormalized(bfactor1);
                Vector  bfactor2 = modes2.GetBFactor().ToArray();
                Vector nbfactor2 = GetBFactorNormalized(bfactor2);
                double corr0 = HMath.HCorr(bfactor1, bfactor2);
                double corr1 = LinAlg.VtV(nbfactor1, nbfactor2) / (n-1);
                double corr2 = contrib.ToArray().HSum();
                HDebug.AssertTolerance(0.00000001, corr0-corr1, corr0-corr2, corr1-corr2);
            }

            return contrib;
        }
    }
}
}
