using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class ModeStatic
    {
        /// Each normal mode has its own frequency value that is determined from eigenvalue of the normal mode.
        /// In Ref.~[\citenum{Na16d}], we showed that normal modes can be highly degenerated when their frequencies are similar.
        /// To take account of the effect of degeneracy, we define the degeneracy overlap $\mbox{dovlp}(\ab,V)$ as the overlap
        /// between a mode $\ab$ and a set of modes $V$, as follows:
        /// %
        /// \begin{equation}
        /// \label{eq:degeneracy-overlap}
        /// \mbox{dovlp}(\ab,V)
        /// = \sqrt{\sum_{\vb \in f(\ab,V)} (\ab_i \cdot \vb)^2}\;,
        /// \end{equation}
        /// %
        /// where $f(\ab,V)$ returns a set of degenerated modes such that:
        /// %
        /// \begin{equation}
        /// \label{eq:degeneracy-overlap-modegroup}
        /// f(\ab,V)
        /// = \{\vb \mid \vb \in V \mbox{ and } \omega_{\ab} - \Delta\omega \leq \omega_{\vb} \leq \omega_{\ab} + \Delta\omega\}\;,
        /// \end{equation}
        /// %
        /// where $\Delta\omega$ is the degeneracy tolerance, and $\omega_{\ab}$ and $\omega_{\vb}$ are the frequencies of mode $\ab$ and $\vb$, respectively.
        /// In this work, we use a degeneracy tolerance $\Delta\omega = 0.1 \cminv$.
        /// 
        /// dovlp(a,V) = sqrt( sum_{v ∈ f(a,V)}  (a . v)^2 )
        /// f(a,V) = { v | v ∈ V and w_a - Δw ≤ w_v ≤ w_a + Δw }

        public static Dictionary<double,double> GetDegeneracyOverlap(this (IList<Mode>,IList<Mode>) modes12, IList<double> masses, double domega)
        {
            return GetDegeneracyOverlap(modes12.Item1, modes12.Item2, masses, domega);
        }

        public static Dictionary<double,double> GetDegeneracyOverlap(IList<Mode> modes1, IList<Mode> modes2, IList<double> masses, double domega)
        {
            List<double> invmasses = new List<double>();
            foreach(var mass in masses)
                invmasses.Add(1 / mass);

            List<Vector> modes1_massweighted = new List<Vector>();
            foreach(var mode in modes1)
                modes1_massweighted.Add(mode.GetMassReduced(invmasses).eigvec);
            List<Vector> modes2_massweighted = new List<Vector>();
            foreach(var mode in modes2)
                modes2_massweighted.Add(mode.GetMassReduced(invmasses).eigvec);

            Matrix modemat1_massweighted = Matrix.FromColVectorList(modes1_massweighted);
            Matrix modemat2_massweighted = Matrix.FromColVectorList(modes2_massweighted);

            Matlab.Clear();
            Matlab.PutMatrix("m1", modemat1_massweighted, true);
            Matlab.PutMatrix("m2", modemat2_massweighted, true);
            Matlab.Execute("dot12 = m1' * m2;");
            Matrix dot12 = Matlab.GetMatrix("dot12", Matrix.Zeros, true);
            Matlab.Clear();

            List<double> freqs2 = modes2.ListFreq().ToList();

            Dictionary<double,double> freq1_dovlp = new Dictionary<double, double>();
            for(int i1=0; i1<modes1.Count; i1++)
            {
                double freq1 = modes1[i1].freq;

                int idx0 = freqs2.BinarySearch(freq1 - domega);
                int idx1 = freqs2.BinarySearch(freq1 + domega);
                //     The zero-based index of item in the sorted System.Collections.Generic.List`1,
                //     if item is found; otherwise, a negative number that is the bitwise complement
                //     of the index of the next element that is larger than item or, if there is no
                //     larger element, the bitwise complement of System.Collections.Generic.List`1.Count.
                if(idx0 < 0) idx0 = Math.Abs(idx0);
                if(idx1 < 0) idx1 = Math.Abs(idx1) - 1;

                double dovlp = 0;
                for(int i2=idx0; i2<=idx1; i2++)
                {
                    double dot12_i1_i2 = dot12[i1, i2];
                    dovlp += dot12_i1_i2 * dot12_i1_i2;
                }
                dovlp = Math.Sqrt(dovlp);
                freq1_dovlp.Add(freq1, dovlp);
            }

            //modes1.GetMassReduced
            return freq1_dovlp;
        }
    }
}
