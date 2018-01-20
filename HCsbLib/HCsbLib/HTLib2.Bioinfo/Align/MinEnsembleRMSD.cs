using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Align
    {
        public class MinEnsembleRMSD : IMinAlign
        {
            public static void Align( List<Vector>[] ensemble)
            {
                List<Vector> meanconf = ensemble[0].HClone();
                int iter = 0;
                while(true)
                {
                    iter++;

                    List<Vector> meanconf0 = meanconf.HClone();
                    for(int i=0; i<ensemble.Length; i++)
                        MinRMSD.Align(meanconf, ref ensemble[i]);
                    for(int i=0; i<meanconf.Count; i++)
                    {
                        meanconf[i] = new double[3];
                        foreach(List<Vector> conf in ensemble)
                            meanconf[i] += conf[i];
                        meanconf[i] /= ensemble.Length;
                    }
                    double maxmove = VectorBlock.PwSub(meanconf0, meanconf).ToVector().ToArray().HAbs().Max();
                    if(maxmove < 0.001)
                        break;
                    if(iter >= 100)
                        break;
                }
            }
            public static void Align(List<Vector>[] ensemble, IList<double> mass)
            {
                List<Vector> meanconf = ensemble[0].HClone();
                int iter = 0;
                double[] bfactor = new double[mass.Count];
                for(int i=0; i<bfactor.Length; i++)
                    bfactor[i] = 1 / (mass[i]*mass[i]);

                while(true)
                {
                    iter++;

                    List<Vector> meanconf0 = meanconf.HClone();
                    for(int i=0; i<ensemble.Length; i++)
                        MinBFactor.Align(meanconf, bfactor, ref ensemble[i]);
                    for(int i=0; i<meanconf.Count; i++)
                    {
                        meanconf[i] = new double[3];
                        foreach(List<Vector> conf in ensemble)
                            meanconf[i] += conf[i];
                        meanconf[i] /= ensemble.Length;
                    }
                    double maxmove = VectorBlock.PwSub(meanconf0, meanconf).ToVector().ToArray().HAbs().Max();
                    if(maxmove < 0.001)
                        break;
                    if(iter >= 100)
                        break;
                }
            }
        }
    }
}
