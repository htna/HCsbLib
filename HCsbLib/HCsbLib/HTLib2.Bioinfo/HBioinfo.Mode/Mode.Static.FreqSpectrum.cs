using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class ModeStatic
    {
        public static List<(double freq, List<int> idxs)> GetIdxFreqSpectrum
            ( this IList<Mode> modes
            , double freq_from  = 0
            , double freq_upto  = 4000
            , double freq_delta = 5
            )
        {
            double[] freqs = modes.ListFreq();
            return HBioinfo.FreqSpectrum.GetIdxFreqSpectrum(freqs, freq_from, freq_upto, freq_delta);
        }
        public static List<(double freq, double prob)> GetFreqSpectrum
            ( this IList<Mode> modes
            , List<(double freq, List<int> idxs)> freq_idxs
            )
        {
            double[] freqs = modes.ListFreq();
            return HBioinfo.FreqSpectrum.GetFreqSpectrum(freqs, freq_idxs);
        }
        public static List<(double freq, double prob)> GetFreqSpectrum
            ( this IList<Mode> modes
            , double freq_from = 0
            , double freq_upto = 4000
            , double freq_delta = 5
            )
        {
            double[] freqs = modes.ListFreq();
            return HBioinfo.FreqSpectrum.GetFreqSpectrum(freqs, freq_from, freq_upto, freq_delta);
        }
    }
}
