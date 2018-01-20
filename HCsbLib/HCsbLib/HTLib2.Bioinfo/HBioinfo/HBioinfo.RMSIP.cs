using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        /// http://www.cell.com/structure/abstract/S0969-2126(08)00011-7
        /// http://download.cell.com/structure/pdf/PIIS0969212608000117.pdf
        /// Lei Yang, Guang Song, Alicia Carriquiry, and Robert L. Jernigan
        ///   "Close Correspondence between the Motions from Principal Component
        ///   Analysis of Multiple HIV-1 Protease Structures and Elastic Network Modes"
        ///   Structure, Volume 16, Issue 2, 321-330, 12 February 2008
        ///   doi:10.1016/j.str.2007.12.011
        /// 
        /// 
        ///   Relating the PC and Mode Spaces
        /// 
        /// The overlap between the motion spaces of the first I PCs and the first J lowfrequency
        /// modes is defined by the root mean-square inner product (RMSIP)
        /// (Amadei et al., 1999; Leo-Macias et al., 2005) as
        /// 
        /// RMSIP(I,J) = [ 1/I * \sum_{i=1..I} \sum_{j=1..J} (Pi . Mj)^2 ]^0.5          (9)
        /// 
        /// where Pi is the ith PC, and Mj is the jth normal mode. This RMSIP indicates how
        /// well the motion space spanned by the first I PCs is represented by the first
        /// J modes.
        public struct ORMSIP
        {
            public double    rmsip;
            public double[,] rmsip_ij;
        };
        public static ORMSIP RMSIP(IList<Vector> Ps, IList<Vector> Ms)
        {
            return RMSIP(Ps, null, Ms);
        }
        public static ORMSIP RMSIP(IList<Vector> Ps, Vector masses, IList<Vector> Ms)
        {
            return RMSIP(Ps, null, masses, Ms);
        }
        public static ORMSIP RMSIP(IList<Vector> modes1, Vector mass1, Vector mass2, IList<Vector> modes2, bool bDebugAssertOrthogonal=true)
        {
            if(HDebug.Selftest())
            {
                List<Vector> tPs = new List<Vector>();
                tPs.Add(new double[6] { 1, 2, 3, 4, 5, 6 });
                tPs.Add(new double[6] { 2, 3, 4, 5, 6, 7 });
                List<Vector> tMs = new List<Vector>();
                tMs.Add(new double[6] { 3, 4, 5, 6, 7, 8 });
                tMs.Add(new double[6] { 4, 5, 6, 7, 8, 9 });
                Vector tPmasses = new double[2] { 3, 4 };
                Vector tMmasses = new double[2] { 1, 2 };
                double trmsip = RMSIP(tPs, tPmasses, tMmasses, tMs, false).rmsip;
                /// [1 2]t  [ 3           ]   [ 1           ]   [3 4]      [1 2]t  [ 3  4]     
                /// [2 3]   [   3         ]   [   1         ]   [4 5]      [2 3]   [ 4  5]     
                /// [3 4] * [     3       ] * [     1       ] * [5 6]  ==  [3 4] * [ 5  6]  ==  [  934 1072 ]
                /// [4 5]   [       4     ]   [       2     ]   [6 7]      [4 5]   [12 14]      [ 1138 1309 ]
                /// [5 6]   [         4   ]   [         2   ]   [7 8]      [5 6]   [14 16]     
                /// [6 7]   [           4 ]   [           2 ]   [8 9]      [6 7]   [16 18]
                /// 
                ///   => 240^2 + 276^2 + 294^2 + 339^2 = 335133
                ///   => sqrt(335133 / 2) = 409.3489
                double trmsip0 = Math.Sqrt(5030065.0 / 2);
                HDebug.AssertTolerance(0.00000001, trmsip - trmsip0);
            }

            Vector[] nmodes1;
            if(mass1 == null)
                nmodes1 = modes1.ToArray();
            else
            {
                // orthogonalize modes1
                nmodes1 = new Vector[modes1.Count];
                Vector masses = new double[mass1.Size*3];
                for(int i=0; i<mass1.Size; i++)
                    masses[i*3+0] = masses[i*3+1] = masses[i*3+2] = mass1[i];
                for(int i=0; i<nmodes1.Length; i++)
                    nmodes1[i] = Vector.PtwiseMul(masses, modes1[i]);
            }
            if(bDebugAssertOrthogonal)
            {
                for(int i=0; i<nmodes1.Length; i++)
                    for(int j=0; j<i; j++)
                    {
                        double dot = LinAlg.VtV(nmodes1[i], nmodes1[j]);
                        HDebug.AssertTolerance(0.000001, dot);
                    }
            }

            Vector[] nmodes2;
            if(mass2 == null)
                nmodes2 = modes2.ToArray();
            else
            {
                // orthogonalize modes2
                nmodes2 = new Vector[modes2.Count];
                Vector masses = new double[mass2.Size*3];
                for(int i=0; i<mass2.Size; i++)
                    masses[i*3+0] = masses[i*3+1] = masses[i*3+2] = mass2[i];
                for(int i=0; i<nmodes2.Length; i++)
                    nmodes2[i] = Vector.PtwiseMul(masses, modes2[i]);
            }
            if(bDebugAssertOrthogonal)
            {
                for(int i=0; i<nmodes2.Length; i++)
                    for(int j=0; j<i; j++)
                    {
                        double dot = LinAlg.VtV(nmodes2[i], nmodes2[j]);
                        HDebug.AssertTolerance(0.00000001, dot);
                    }
            }


            int I = nmodes1.Length;
            int J = nmodes2.Length;
            double rmsip = 0;
            double[,] rmsip_ij = new double[I, J];

            for(int i=0; i < I; i++)
                for(int j=0; j < J; j++)
                {
                    double PiMj = LinAlg.VtV(nmodes1[i], nmodes2[j]);
                    rmsip += (PiMj * PiMj);
                    rmsip_ij[i, j] = PiMj;
                }
            rmsip = Math.Sqrt(rmsip / I);

            return new ORMSIP
            {
                rmsip    = rmsip,
                rmsip_ij = rmsip_ij,
            };
        }
    }
}
