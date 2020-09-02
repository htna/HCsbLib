using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class UnitConversion
	{
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
    }
}
