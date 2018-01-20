using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        /// http://www.ncbi.nlm.nih.gov/pmc/articles/PMC1913142/
        /// Cumulative Square Overlap (CSO)
        /// 
        /// CSO(k) = sum_{j=1}^{k} overlap_j^2
        ///        = sum_{j=1}^{k} (mode_A_x' * mode_B_j)^2
        ///        -> 1 as k->3n
        /// 
        public static double[] CSO(Vector mode1, IList<Vector> mode2s, double? tolDebugAssertOrthogonal=0.00000001)
        {
            return CSO(mode1, null, mode2s, tolDebugAssertOrthogonal);
        }
        public static double[] CSO(Vector mode1, Vector mass, IList<Vector> mode2s, double? tolDebugAssertOrthogonal=0.00000001)
        {
            if(HDebug.Selftest())
            {
            }

            Vector mass3sqrt = null;
            if((mass != null) && (mass._data != null))
            {
                mass3sqrt = new double[mass.Size*3];
                for(int i=0; i<mass.Size; i++)
                {
                    double massisqrt = Math.Sqrt(mass[i]);
                    mass3sqrt[i*3+0] = massisqrt;
                    mass3sqrt[i*3+1] = massisqrt;
                    mass3sqrt[i*3+2] = massisqrt;
                }
            }

            Vector nmode1 = mode1;
            if(mass3sqrt != null)
                nmode1 = Vector.PtwiseMul(mass3sqrt, nmode1);
            nmode1 = mode1.UnitVector();


            Vector[] nmode2s = new Vector[mode2s.Count];
            {
                // orthogonalize modes2
                nmode2s = new Vector[mode2s.Count];
                for(int i=0; i<mode2s.Count; i++)
                {
                    Vector nmode2i = mode2s[i];
                    if(mass3sqrt != null)
                        nmode2i = Vector.PtwiseMul(mass3sqrt, nmode2i);
                    nmode2i = nmode2i.UnitVector();
                    nmode2s[i] = nmode2i;
                }
            }
            if(tolDebugAssertOrthogonal!=null)
            {
                for(int i=0; i<nmode2s.Length; i++)
                    for(int j=0; j<i; j++)
                    {
                        double dot = LinAlg.VtV(nmode2s[i], nmode2s[j]);
                        HDebug.AssertTolerance(tolDebugAssertOrthogonal.Value, dot);
                    }
            }

            double[] SO = new double[nmode2s.Length];
            for(int i=0; i<SO.Length; i++)
            {
                double vtv = LinAlg.VtV(nmode1, nmode2s[i]);
                SO[i] += vtv*vtv;
            }
            //SO = SO.HSort().HReverse();

            double[] CSO = new double[nmode2s.Length];
            for(int i=0; i<CSO.Length; i++)
            {
                CSO[i] = (i==0) ? 0 : CSO[i-1];
                CSO[i] += SO[i];
            }

            return CSO;
        }
    }
}
