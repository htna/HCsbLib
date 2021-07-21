using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class Mode : ICloneable
    {
        public int th = -1; // '-1' means not identified
        public double eigval;
        public Vector eigvec;

        public double freq
        {
            get
            {
                return UnitConversion.EigvalToFreq(eigval);
            }
        }
        public double psec
        {
            get
            {
                return UnitConversion.FreqToPsec(freq);
            }
        }
        public double meV
        {
            get
            {
                return UnitConversion.FreqToMeV(freq);
            }
        }
        public double Kelvin
        {
            get
            {
                return UnitConversion.FreqToKelvin(freq);
            }
        }
        public static double[] EigvalToPsec(IList<double> eigvals)
        {
            double[] freqs = EigvalToFreq(eigvals);
            double[] psecs = FreqToPsec(freqs);
            return psecs;
        }
        public static double[] FreqToPsec(IList<double> freqs)
        {
            double[] psecs = new double[freqs.Count];
            for(int i = 0; i < psecs.Length; i++)
                psecs[i] = UnitConversion.FreqToPsec(freqs[i]);
            return psecs;
        }
        public static double[] EigvalToFreq(IList<double> eigvals)
        {
            double[] freqs = new double[eigvals.Count];
            for(int i=0; i<freqs.Length; i++)
                freqs[i] = UnitConversion.EigvalToFreq(eigvals[i]);
            return freqs;
        }
        public static double EigvalToFreq(double eigval)
        {
            double freq = UnitConversion.EigvalToFreq(eigval);
            return freq;
        }
        public static double FreqToPsec(double freq)
        {
            double psec = UnitConversion.FreqToPsec(freq);
            return psec;
        }
        public static double EigvalToPsec(double eigval)
        {
            double freq = EigvalToFreq(eigval);
            double psec = FreqToPsec(freq);
            return psec;
        }
        public static double FreqToEigval(double freq)
        {
            double eigval = UnitConversion.FreqToEigval(freq);
            return eigval;
        }

        public Mode()
        {
        }

        [Serializable]
        public class FloatMode
        {
            public int     th;
            public double  eigval;
            public float[] eigvec;
            public FloatMode()
            {
            }
            ////////////////////////////////////////////////////////////////////////////////////
            // Serializable
            public FloatMode(SerializationInfo info, StreamingContext ctxt)
            {
                th     = (int    )info.GetValue("th"    , typeof(int    ));
                eigval = (double )info.GetValue("eigval", typeof(double ));
                eigvec = (float[])info.GetValue("eigvec", typeof(float[]));
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("th"    , this.th    );
                info.AddValue("eigval", this.eigval);
                info.AddValue("eigvec", this.eigvec);
            }
        }
        public FloatMode ToFloatMode()
        {
            FloatMode fmode = new FloatMode();
            fmode.th     = th    ;
            fmode.eigval = eigval;
            fmode.eigvec = new float[eigvec.Size];
            for(int i=0; i<eigvec.Size; i++)
                fmode.eigvec[i] = (float)eigvec[i];
            return fmode;
        }
        public static Mode FromFloatMode(FloatMode fmode)
        {
            Mode mode = new Mode();
            mode.th     = fmode.th;
            mode.eigval = fmode.eigval;
            mode.eigvec = new double[fmode.eigvec.Length];
            for(int i=0; i<mode.eigvec.Size; i++)
                mode.eigvec[i] = fmode.eigvec[i];
            return mode;
        }

        public int size
        {
            get
            {
                HDebug.Assert(eigvec.Size %3 == 0);
                return eigvec.Size/3;
            }
        }
        public Vector[] GetEigvecsOfAtoms(double scale=1)
        {
            Vector[] vecs = new Vector[size];
            for(int i=0; i<size; i++)
                vecs[i] = GetEigvecOfAtom(i, scale);
            return vecs;
        }
        public Vector GetEigvecOfAtom(int idxAtom, double scale=1)
        {
            int i = idxAtom;
            Vector vec = new double[3] { scale*eigvec[i*3+0], scale*eigvec[i*3+1], scale*eigvec[i*3+2] };
            return vec;
        }
        public override string ToString()
        {
            return "freq " + freq.ToString("0.0000") + ", eigval " + eigval.ToString("0.0000000000") + ", {" + eigvec.ToString() + "}";
        }
        public Mode Clone()
        {
            Mode mode = new Mode();
            mode.th     = th;
            mode.eigval = eigval;
            mode.eigvec = eigvec.Clone();
            return mode;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
        public Mode GetMassReducedGnm(IList<double> masses)
        {
            Mode mode = this.Clone();
            int leng = masses.Count;
            if(mode.eigvec.Size != leng)
                throw new Exception("messes.Count != mode.eigvec.size");
            for(int j=0; j<leng; j++)
            {
                double sqrt_mass = Math.Sqrt(masses[j]);
                mode.eigvec[j] /= sqrt_mass;
            }
            return mode;
        }
        public Mode GetMassReduced(IList<double> masses)
        {
            Mode mode = this.Clone();
            int leng = masses.Count;
            if(mode.size != leng)
                throw new Exception("messes.Count != mode.size");
            for(int j=0; j<leng; j++)
            {
                double sqrt_mass = Math.Sqrt(masses[j]);
                mode.eigvec[j*3+0] /= sqrt_mass;
                mode.eigvec[j*3+1] /= sqrt_mass;
                mode.eigvec[j*3+2] /= sqrt_mass;
            }
            return mode;
        }
        public void GetBFactor(IList<double> bfactor)
        {
            HDebug.Assert(eigval > 0);
            if(bfactor.Count != size)
                throw new Exception("bfactor.Count != mode.size");
            for(int i=0; i<size; i++)
            {
                Vector eigveci = GetEigvecOfAtom(i);
                double bfactori = LinAlg.VtV(eigveci, eigveci) / eigval;
                bfactor[i] += bfactori;
            }
        }
        public void GetBFactor(IList<double> bfactor, double T)
        {
            /// http://bioinformatics.oxfordjournals.org/content/22/21/2619.full
            /// ANM mean-square fluctuation:
            ///   B^ANM_i = (8*pi^2 * kB * T) / (3 & gamma) * Tr[H^-1_ii];
            ///   kB=0.0019872041 [ kcal/(mol.K) ]
            double kB = 0.0019872041;
            double scale = 8.0 * Math.PI * Math.PI * kB * T / 3.0;
            HDebug.Assert(eigval > 0);
            if(bfactor.Count != size)
                throw new Exception("bfactor.Count != mode.size");
            for(int i=0; i<size; i++)
            {
                Vector eigveci = GetEigvecOfAtom(i);
                double bfactori = scale*LinAlg.VtV(eigveci, eigveci) / eigval;
                bfactor[i] += bfactori;
            }
        }
        public void GetBFactorGnm(IList<double> bfactor)
        {
            HDebug.Assert(eigval > 0);
            if(bfactor.Count != eigvec.Size)
                throw new Exception("bfactor.Count != mode.size");
            for(int i=0; i<bfactor.Count; i++)
            {
                double bfactori = (eigvec[i] * eigvec[i]) / eigval;
                bfactor[i] += bfactori;
            }
        }
        public void GetAnisou(IList<MatrixByArr> anisou)
        {
            //Debug.Assert(false);
            if(anisou.Count != size)
                throw new Exception("anisou.Count == mode.size");
            for(int i=0; i<size; i++)
            {
                Vector eigveci = GetEigvecOfAtom(i);
                anisou[i] += LinAlg.VVt(eigveci, eigveci) / eigval;
            }
        }
        public Mode GetTransposed(Trans3 trans)
        {
            /// trans(pt + mode) = R*(pt + mode) + T
            ///                  = R*pt + T + R*mode
            ///                  = (R*pt+T) + R*mode
            ///                  = trans(pt)+ R*mode
            Vector[] reigvec = GetEigvecsOfAtoms();

            if((trans.ds != 1) || (trans.dt.Dist2 != 0))        // if trans is rot+sca+mov,
                trans = new Trans3(new double[3], 1, trans.dr); // then redetermine trans as rot only

            for(int i=0; i<reigvec.Length; i++)
                reigvec[i] = trans.DoTransform(reigvec[i]);

            return new Mode
            {
                th     = this.th,
                eigval = this.eigval,
                eigvec = reigvec.ToVector(),
            };
        }
        public void GetHessian(MatrixByArr hess)
        {
            if(HDebug.Selftest())
            {
                Mode tmode = new Mode();
                tmode.eigval = 2;
                tmode.eigvec = new double[] { 1, 2, 3 };
                MatrixByArr thess0 = new double[,]{{ 2,  4,  6 }
                                             ,{ 4,  8, 12 }
                                             ,{ 6, 12, 18 }};
                MatrixByArr thess1 = new double[3,3];
                tmode.GetHessian(thess1);
                HDebug.AssertTolerance(0.00000001, thess0-thess1);
            }
            HDebug.Assert(hess.RowSize == eigvec.Size, hess.ColSize == eigvec.Size);
            //unsafe
            {
                double[] pvec = eigvec._data;
                {
                    for(int c=0; c<eigvec.Size; c++)
                        for(int r=0; r<eigvec.Size; r++)
                            hess[c, r] += eigval*pvec[c]*pvec[r];
                }
            }
        }
        public Mode GetNormalized()
        {
            if(HDebug.Selftest())
            {
                Mode lmode0 = new Mode { eigval=0.123, eigvec=new double[] { 1, 2, 3, 2, 3, 4, 3, 4, 5 } };
                Mode lmode1 = lmode0.GetNormalized();
                HDebug.Assert(lmode0.eigvec.Size == lmode1.eigvec.Size);
                MatrixByArr lhess0 = new double[lmode0.eigvec.Size, lmode0.eigvec.Size];
                MatrixByArr lhess1 = new double[lmode1.eigvec.Size, lmode1.eigvec.Size];
                lmode0.GetHessian(lhess0);
                lmode1.GetHessian(lhess1);
                double tolbase = Math.Max(lhess0.ToArray().HMax(), lhess1.ToArray().HMax());
                HDebug.AssertTolerance(tolbase*0.00000001, lhess0-lhess1);
            }

            /// H = sum vi' * di * vi
            ///   = sum (vi/|vi|)'  * (di |vi|^2) * (vi/|vi|)
            double leigvec = eigvec.Dist;
            Vector neigvec = eigvec.UnitVector();
            return new Mode
            {
                th     = th,
                eigval = eigval*leigvec*leigvec,
                eigvec = neigvec
            };
        }
        public Mode ToModesUnnormalized(double[] masses)
        {
            /// In Tinker, "vibration normal mode" is normalized tomake its length "1".
            /// It is not the original mode since the mode is the computed by projecting
            ///   "the eigenvalue of mass-weighted hessian matrix" into the mass-reduced
            ///   coordinate. It is generally OK.
            ///             MHM = M^-0.5 * H * M^-0.5
            ///             [V,D] = eig(MHM)
            ///             MD_i = M^-0.5 * V_i
            ///             |MD_i| != 1
            #region /// where ...
            /// where M       : mass matrix
            ///       H       : Hessian matrix
            ///       MHM     : mass-weighted Hessian matrix
            ///       V, D    : matrix form of eigenvectors and eigenvalues
            ///       V_i, D_i: i-th eigenvector and eigenvalue
            ///       MD_i    : i-th mode (eigenvector in mass-reduced coordinate)
            #endregion
            ///    
            /// However, sometimes like computing RMSIP, the original mode is preferred.
            /// This function converts the normalized mode (returned by ToModesNormalized()) 
            ///   into the original un-normalized mode
            // I need eigenvector (V_i) or un-normalized mode (MD_i), not normalized mode (VNM_i)
            /// 
            /// Here, VNM_i and VEV_i is "vibration normal mode" and "vibration eigen value"
            ///   returned by ToModesNormalized().
            ///   
            #region /// derivation...
            ///    MHM = M^-0.5 * H * M^-0.5
            ///    [V,D] = eig(MHM)
            ///    MD_i = M^-0.5 * V_i
            ///    |MD_i| != 1
            ///    
            ///    VNM_i : vibration normal mode, returned by ToModesNormalized()
            ///    VEV_i : vibration eigen value, returned by ToModesNormalized()
            /// 
            ///    VNM_i = MD_i/|MD_i|
            ///          = (M^-0.5 * V_i) / |M^-0.5 * V_i|
            ///          = M^-0.5 * (V_i / |M^-0.5 * V_i|)
            ///    VEV_i = |M^-0.5 * V_i|^2 * D_i
            ///    
            ///    M^0.5 * VNM_i = V_i / |M^-0.5 * V_i|
            ///    |M^0.5 * VNM_i| = |V_i / |M^-0.5 * V_i||
            ///                    = |V_i| / |M^-0.5 * V_i|
            ///                    = 1     / |M^-0.5 * V_i|
            ///    |M^-0.5 * V_i| = 1 / |M^0.5 * VNM_i|
            ///    M^-0.5 * (V_i / |M^-0.5 * V_i|) = VNM_i
            ///             (V_i / |M^-0.5 * V_i|) = M^0.5 * VNM_i
            ///              V_i                   = M^0.5 * VNM_i * |M^-0.5 * V_i|
            ///                                    = M^0.5 * VNM_i * (1 / |M^0.5 * VNM_i|)
            ///                                    = M^0.5 * VNM_i / |M^0.5 * VNM_i|
            ///                                    = (M^0.5 * VNM_i) / |M^0.5 * VNM_i|
            ///    V_i = (M^0.5 * VNM_i) / |M^0.5 * VNM_i|
            ///    
            ///    VEV_i = |M^-0.5 * V_i|^2 * D_i
            ///          = (1 / |M^0.5 * VNM_i|)^2 * D_i
            ///          = D_i / |M^0.5 * VNM_i|^2
            ///    D_i   = VEV_i * |M^0.5 * VNM_i|^2
            #endregion
            ///    
            ///    V_i = (M^0.5 * VNM_i) / |M^0.5 * VNM_i|   : eigvec in mass-weighted hessian matrix
            ///    D_i = VEV_i * |M^0.5 * VNM_i|^2           : eigval in mass-weighted hessian matrix
            ///    M_i =          VNM_i  / |M^0.5 * VNM_i|   : mode in mass-reduced coordinate (M^-0.5 * V_i)
            ///    VEV_i : eigen value in Tinker
            ///    VNM_i : vibration normal mode in Tinker
            /// //////////////////////////////////////////////////////////////////////////////////////////////////

            Mode   mode_i = this;

            double VEV_i  = mode_i.eigval;
            Vector VNM_i  = mode_i.eigvec.Clone();
            Vector M05VNM_i = new double[VNM_i.Size];
            HDebug.Assert(masses.Length*3 == VNM_i.Size);
            for(int j=0; j<masses.Length; j++)
            {
                double massj05 = Math.Sqrt(masses[j]);
                M05VNM_i[j*3+0] = massj05 * VNM_i[j*3+0];
                M05VNM_i[j*3+1] = massj05 * VNM_i[j*3+1];
                M05VNM_i[j*3+2] = massj05 * VNM_i[j*3+2];
            }
            double len_M05VNM_i = M05VNM_i.Dist;
            Vector M_i = VNM_i / len_M05VNM_i;
            double D_i = VEV_i * (len_M05VNM_i * len_M05VNM_i);

            Mode unmode_i = new Mode { eigval=D_i, eigvec=M_i, th=mode_i.th };
            return unmode_i;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        // Serializable
        public Mode(SerializationInfo info, StreamingContext ctxt)
        {
            th     = (int     )info.GetValue("th"    , typeof(int     ));
            eigval = (double  )info.GetValue("eigval", typeof(double  ));
            eigvec = (double[])info.GetValue("eigvec", typeof(double[]));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("th"    , this.th    );
            info.AddValue("eigval", this.eigval);
            info.AddValue("eigvec", this.eigvec);
        }
    }
}
