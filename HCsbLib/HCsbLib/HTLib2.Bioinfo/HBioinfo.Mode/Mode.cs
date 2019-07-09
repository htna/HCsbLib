﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class Mode : ICloneable
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// http://halas.rice.edu/conversions
        /// 
        /// Energy Unit Conversions
        /// 
        /// As the field of nanophotonics continues to become more interdisciplinary, it is essential to be
        /// able to convert between different units of energy by memory.  When reading papers, attending
        /// talks, having conversations with colleagues, and answering questions in your own presentations,
        /// you should always know exactly where a certain measurement lies in the electromagnetic spectrum,
        /// regardless of units.
        /// 
        /// For quick conversions, enter a value into any of the boxes below, and the remaining units will be
        /// calculated automatically and rounded to the fifth decimal place. Further below is a table that
        /// gives spectral ranges relevant to photonics.
        /// 
        /// 
        /// [   100000000.00000] eV        [       0.00001] nm      [806554429019.41455] cm^-1      [    0.00000] fs
        /// [100000000000      ] meV       [       0.00000] µm      [ 24179893478.65168] THz        [    0.00000] ps
        /// --------------------------------------------------------------------------------------------------------------------------------------------
        /// [           0.00012] eV        [10000000.00000] nm      [           1      ] cm^-1      [33356.40952] fs
        /// [           0.12398] meV       [   10000.00000] µm      [           0.02998] THz        [   33.35641] ps
        /// --------------------------------------------------------------------------------------------------------------------------------------------
        /// meV   : 1           5           12.39842    10          20          24.79684     30          37.19526   40          49.59368
        /// cm^-1 : 8.06554    40.32772    100          80.65544   161.31089   200          241.96633   300        322.62175   400
        /// 
        /// 
        /// Range               Subrange        Abbreviation    eV                  nm              cm^-1               THz             fs
        /// ============================================================================================================================================
        /// Ultraviolet (UV)    Extreme UV      EUV             1240 - 12.4         1 - 100         1e7 - 1e5           3e5 - 3e3       0.00334 - 0.334
        ///                     Vacuum UV       VUV, UV-C       12.4 - 6.53         100 - 190       100000 - 52600      3000 - 1580     0.334 - 0.634
        ///                     Deep UV         DUV, UV-C       6.53 - 4.43         190 - 280       52600 - 35700       1580 - 1070     0.634 - 0.934
        ///                     Mid UV          UV-B            4.43 - 3.94         280 - 315       35700 - 31700       1070 - 952      0.934 - 1.05
        ///                     Near UV         UV-A            3.94 - 3.26         315 - 380       31700 - 26300       952 - 789       1.05 - 1.27
        /// --------------------------------------------------------------------------------------------------------------------------------------------
        /// Visible (Vis)       Violet          -               3.26 - 2.85         380 - 435       26300 - 23000       789 - 689       1.27 - 1.45
        ///                     Blue            -               2.85 - 2.48         435 - 500       23000 - 20000       689 - 600       1.45 - 1.67
        ///                     Cyan            -               2.48 - 2.38         500 - 520       20000 - 19200       600 - 577       1.67 - 1.73
        ///                     Green           -               2.38 - 2.19         520 - 565       19200 - 17700       577 - 531       1.73 - 1.88
        ///                     Yellow          -               2.19 - 2.10         565 - 590       17700 - 16900       531 - 508       1.88 - 1.97
        ///                     Orange          -               2.10 - 1.98         590 - 625       16900 - 16000       508 - 480       1.97 - 2.08
        ///                     Red             -               1.98 - 1.59         625 - 780       16000 - 12800       480 - 384       2.08 - 2.60
        /// --------------------------------------------------------------------------------------------------------------------------------------------
        /// Infrared (IR)       Near Infrared   NIR, IR-A       1.58 - 0.886        780 - 1400      12800 - 7140        384 - 214       2.60 - 4.67
        ///                     -               NIR, IR-B       0.886 - 0.413       1400 - 3000     7140 - 3330         214 - 100       4.67 - 10.0
        ///                     Mid Infrared    MIR, IR-C       413 - 24.8 meV      3 - 50 µm       3330 - 200          100 - 6.0       10 - 167
        ///                     Far Infrared    FIR, IR-C       24.8 - 1.24 meV     50 µm - 1 mm    200 - 10            6.0 - 0.3       167 - 3340
        /// --------------------------------------------------------------------------------------------------------------------------------------------
        /// Terahertz (THz)     -               -               124 - 1.24 meV      10 µm - 1 mm    1000 - 10           30 - 0.3        33.4 - 3340
        /// ============================================================================================================================================
        /// 
        /// Relevant Formulas:
        /// E = hc/λ
        /// ν = c/λ
        /// ṽ = 1/λ
        /// T = 1/ν
        /// 
        /// Definitions:
        /// E = energy (eV)
        /// λ = wavelength (m)
        /// ṽ = wavenumber (m-1)
        /// T = period (s)
        /// ν = frequency (s-1 or Hz)
        /// h = Planck's constant = 4.135667516 x 10-15 eV*s
        /// c = speed of light = 299792458 m/s
        /// 
        public static double FreqToMeV(double freq)
        {
            /// (freq cm^-1) / (0.01 cm to m) * (4.135667516 * 10^-15 eV*s) * (299792458 m/s) * (1000 eV to meV )
            /// = (freq cm^-1) * 0.1239841930092394328
            const double freq_to_meV = 0.1239841930092394328;
            return (freq * freq_to_meV);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// https://www.phys.ksu.edu/personal/cdlin/phystable/econvert.html
        /// 
        /// Energy Converter:
        /// 1 degree kelvin = 8.621738 X10-5  eV 
        ///                 = 0.0862          meV 
        ///                 = 0.695           cm^-1
        /// 
        ///      1 a.u =     27.211396 eV   =  219 474.63 05 cm^-1 
        ///      1 Ry  =     13.6057   eV 
        ///      1 eV  =   8065.54     cm^-1 
        ///      1 eV  = 11,600        degrees Kelvin
        ///      1 meV =      8.065    cm^-1
        /// 
        /// 1 Kcal/mol= 0.0434 eV = 43.4 meV
        /// 
        /// Photon momentum    k= 2.7 x10-4 E (eV)
        /// 
        /// Atomic units: 
        ///     in time = a /v0 = 2.41 x10 -17 sec 
        ///     in frequency =     4.13 x1016 Hz 
        ///     in electric field=  5.14 x 10 9 V/cm 
        ///  
        /// 
        /// Oscillator strength and transition rates 
        ///     A = 2*(E2/c3) f   (4.13 x1016)    1/sec 
        ///     where E is in a.u. , c=137.03604 and f  is the oscillator strength for emission
        /// 
        /// Laser intensity and field strength 
        ///     1 a.u. in E= 5x109   V/cm 
        ///     1 a.u. in intensity= 3.5 x1016 (w/cm2) 
        ///     I (watts/cm2)= 1.33 x10-3 E2  (V/cm2) 
        /// 
        /// Energy difference between D(1s) and H(1s) is 3.7 meV, or 1.36 X10-4   a.u.
        /// 
        /// mass of proton in a.u. is   1836.152701 
        /// mass of deuteron in a.u. is 3670.483014 
        /// mass of neutron in a.u. is  1838.683662
        /// 
        public static double FreqToKelvin(double freq)
        {
             /// 1 degree kelvin = 0.695 cm^-1
            const double freq_to_kelvin = 1/0.695;
            return (freq * freq_to_kelvin);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        public int th = -1; // '-1' means not identified
        public double eigval;
        public Vector eigvec;

        static readonly double freq_convert     = 4.1840     * 100;
        static readonly double freq_lightspd    = 2.99792458 / 100;
        static readonly double freq_factor      = Math.Sqrt(freq_convert) / (2.0 * Math.PI * freq_lightspd);
        public double freq
        {
            get
            {
                return EigvalToFreq(eigval);
            }
        }
        public double psec
        {
            get
            {
                return FreqToPsec(freq);
            }
        }
        public double meV
        {
            get
            {
                return FreqToMeV(freq);
            }
        }
        public double Kelvin
        {
            get
            {
                return FreqToKelvin(freq);
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
                psecs[i] = FreqToPsec(freqs[i]);
            return psecs;
        }
        public static bool FreqToPsec_selftest = HDebug.IsDebuggerAttached;
        public static double FreqToPsec(double freq)
        {
            /// You asked how to convert 5 / cm to 7psec?
            ///
            /// This is actually a hack, invented by spectropscopists who excite particular modes using photons: they
            /// essentially equate the wavelength with a photon frequency of the same wavelength.
            ///
            /// so mulitply 5 / cm by speed of light, and invert:
            ///
            ///                5 / cm x 3 E10 cm/ s = 15 x E10/ sec.This inverst to .6666E-12 or ~7psec.
            if(FreqToPsec_selftest)
            {
                FreqToPsec_selftest = false;
                double t_freq = 5;
                double t_psec = FreqToPsec(t_freq);
                HDebug.Assert(Math.Abs(t_psec - 6.66666666666666666666) < 0.00000001);
            }

            /// psec
            /// = 1 / ( freq (1/cm) x 3*E10 (cm/s)) * 10^12
            /// = 1 / ( freq * 3 * 10^10) * 10^12
            /// = 1 / ( freq * 3 ) * 10^-10 * 10^12
            /// = 1 / (freq * 3) * 100
            /// = 100 / (freq * 3)
            double psec = 100.0 / (freq * 3.0);
            return psec;
        }
        public static double[] EigvalToFreq(IList<double> eigvals)
        {
            double[] freqs = new double[eigvals.Count];
            for(int i=0; i<freqs.Length; i++)
                freqs[i] = EigvalToFreq(eigvals[i]);
            return freqs;
        }
        public static double EigvalToFreq(double eigval)
        {
            {
                /// Discussion
                /// https://www.charmm.org/ubbthreads/ubbthreads.php?ubb=showflat&Number=32622
                /// 
                /// =============================================================================================================
                /// Frequencies from Quasiharmonic Analysis 
                /// =============================================================================================================
                /// slaw :  Hi CHARMM Community,
                ///         
                ///         I am trying to understand the output produced from quasiharmonic analysis but I am getting stuck
                ///         on something that seems minor to me. I have calculated the variance/covariance matrix from a
                ///         simulation trajectory, diagonalized the matrix, and obtained the eigenvalues and eigenvectors
                ///         using CHARMM. 
                ///         
                ///         The first thing that I noticed was that the eigenvalues reported in the CHARMM output was different
                ///         from the eigenvalues that are generated by the variance/covariance matrix. After going through the
                ///         JCP 2001 paper, I realized that CHARMM was not outputting the eigenvalues from the variance/covariance
                ///         matrix and, instead, was outputting a scaled eigenvalue (scaled by kT/eigenvalue).
                ///         
                ///         Next, I examined the frequencies output by CHARMM and compared them with frequencies that I calculated
                ///         directly from the scaled eigenvalues. According to the JCP paper, the frequencies, w, are calculated from
                ///         
                ///         w[i] = sqrt(kT/lambda[i])
                ///         
                ///         where lambda are the unscaled eigenvalues from the variance/covariance matrix. According to the
                ///         CHARMM documentation, the frequency is expressed in units of cm^-1 so I tried playing around with
                ///         the unit conversions. It took me a while to remember that lambda[i] were in units of
                ///         mass*Angstroms*Angstroms (and not simply Angstroms*Angstroms) since the matrix is mass weighted.
                ///         Realizing this, then the frequencies, w[i], would result in units of inverse seconds (or Hz). 
                ///         
                ///         Here is where I am having some problem with the final conversion. Normally, to convert the frequencies
                ///         into units cm^-1, one simply divides the frequencies by the speed of light (c = 2.99 x 10^8 m/s):
                ///         
                ///         Frequency[i] = w[i]/c
                ///         
                ///         However, this did not suffice to recapituate the frequencies reported by CHARMM. Instead, I found
                ///         that CHARMM is multiplying the frequencies, w[i], by:
                ///         
                ///         Frequency[i] = (2045.5/(2.99793*6.28319)) * w[i]
                ///         
                ///         I can see that the 2.99793 corresponds to the speed of light (although I don't see where the 10^8
                ///         disappeared to) and the 6.28319 corresponds to 2*Pi. However, I have no idea where the 2045.5 comes
                ///         from (I see that it is somehow related to sqrt(cal) = sqrt(4.814 J) but it isn't clear to me why
                ///         it is needed and why the value is so large).
                ///         
                ///         I would appreciate any clarification on the following:
                ///         
                ///         1) Where the "10^8" disappeared to
                ///         2) Why are we dividing by 2*Pi
                ///         3) Where does the 2045.5 come from
                ///         
                ///         Thanks in advance!
                /// =============================================================================================================
                /// lennart :
                ///         CHARMM does not use SI-units; usage.doc. 2*Pi to convert to/from angular frequency.
                ///         _________________________
                ///         Lennart Nilsson
                ///         Karolinska Institutet
                ///         Stockholm, Sweden
                /// =============================================================================================================
            }
            {
                /// A part of the original manuscript of universal paper.
                /// ================================================================
                /// \begin{equation} 
                ///  \label{eqn:eig2freq} 
                ///  \omega_i = \frac{1}{2 \pi c} \sqrt{d\,\lambda_i}, 
                ///  \end{equation} 
                ///  % 
                ///  %http://users.mccammon.ucsd.edu/~dzhang/energy-unit-conv-table.html 
                ///  where $c$ is the speed of light in vaccum whose value is $2.997925\times 10^{-2}\,[\mbox{cm/ps}]$, and
                ///  $d$ is the conversion constant from {{kcal/mol}} to {{J/mol}} whose value is
                ///  $4.184\times10^{2}\,[\mbox{g}\mbox{\AA}^2/\mbox{ps}^2/\mbox{kcal}]$;
                /// ================================================================
            }
            {
                /// tinker-6.2.06: units.i
                ///     convert     conversion from kcal to g*Ang**2/ps**2
                ///     lightspd    speed of light in vacuum (c) in cm/ps
                ///     convert =4.1840d+2
                ///     lightspd=2.99792458d-2
                /// 
                /// tinker-6.2.06: vibrate.f
                ///      call diagq (nfreq,nfreq,matrix,eigen,vects)
                ///      factor = sqrt(convert) / (2.0d0*pi*lightspd)
                ///      do i = 1, nvib
                ///         eigen(i) = factor * sign(1.0d0,eigen(i)) * sqrt(abs(eigen(i)))
                ///      end do
                ///      write (iout,30)
                ///   30 format (/,' Vibrational Frequencies (cm-1) :',/)
                ///      write (iout,40)  (i,eigen(i),i=1,nvib)
                ///   40 format (5(i5,f10.3))
                /// 
                /// vibrate frequency (cm^-1) = factor * sign(eigen(i)) * sqrt(abs(eigen(i)))
                ///                           = sign(eigen(i)) * sqrt(abs(eigen(i))) * factor
                ///                           = sign(eigen(i)) * sqrt(abs(eigen(i))) * (sqrt(convert) / (2 * pi * lightspd))
                double freq = Math.Sign(eigval) * Math.Sqrt(Math.Abs(eigval)) * freq_factor;
                return freq;
            }
        }
        public static double FreqToEigval(double freq)
        {
            double eigval = Math.Sign(freq) * (freq*freq) / (freq_factor*freq_factor);
            if(HDebug.IsDebuggerAttached)
            {
                double _freq = EigvalToFreq(eigval);
                double _diff = freq - _freq;
                HDebug.Assert(Math.Abs(_diff) < 0.00000001);
            }
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
