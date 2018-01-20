using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class HBioinfo
    {
        public static Matrix ModesCov(this IList<Mode> modes, double kB, double T)
        {
            /// HBioinfo.Corr.Dmitry07.pdf
            ///     http://www.cell.com/structure/abstract/S0969-2126(07)00029-9
            ///     http://www.ncbi.nlm.nih.gov/pubmed/17292835
            ///     Structure. 2007 Feb;15(2):169-77.
            ///     Protein structural variation in computational models and crystallographic data.
            ///     Kondrashov DA, Van Wynsberghe AW, Bannen RM, Cui Q, Phillips GN Jr.
            ///     DOI: http://dx.doi.org/10.1016/j.str.2006.12.006
            int n = modes[0].size;
            Matrix cov = Matrix.Zeros(n, n);
            foreach(var mode in modes)
            {
                double   eigval = mode.eigval;
                Vector[] eigvec = mode.GetEigvecsOfAtoms();
                for(int i=0; i<n; i++)
                    for(int j=0; j<n; j++)
                    {
                        double cov_ij = LinAlg.VtV(eigvec[i], eigvec[j]) / eigval;
                        cov[i, j] += cov_ij;
                    }
            }
            return cov;
        }
        public static Matrix ModesCorr(this IList<Mode> modes)
        {
            /// HBioinfo.Corr.Dmitry07.pdf
            ///     http://www.cell.com/structure/abstract/S0969-2126(07)00029-9
            ///     http://www.ncbi.nlm.nih.gov/pubmed/17292835
            ///     Structure. 2007 Feb;15(2):169-77.
            ///     Protein structural variation in computational models and crystallographic data.
            ///     Kondrashov DA, Van Wynsberghe AW, Bannen RM, Cui Q, Phillips GN Jr.
            ///     DOI: http://dx.doi.org/10.1016/j.str.2006.12.006
            double kB = 1;
            double T = 300;
            Matrix cov = ModesCov(modes, kB, T);
            HDebug.Assert(cov.ColSize == cov.RowSize);
            int n = cov.ColSize;

            Vector sqrt_cov_ii = new double[n];
            for(int i=0; i<n; i++)
                sqrt_cov_ii[i] = Math.Sqrt(cov[i, i]);

            Matrix corr = Matrix.Zeros(n, n);
            for(int i=0; i<n; i++)
                for(int j=0; j<n; j++)
                {
                    corr[i, j] = cov[i, j] / (sqrt_cov_ii[i] * sqrt_cov_ii[j]);
                }
            return corr;
        }
    }
}
