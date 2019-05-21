using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        public static partial class FreqSpectrum
        {
            /// frequency spectrum
            /// mode spectrum
            /// (*) vibrational spectrum
            public static List<(double freq, List<int> idxs)> GetIdxFreqSpectrum
                ( IList<double> freqs
                , double freq_from  = 0
                , double freq_upto  = 4000
                , double freq_delta = 5
                )
            {
                HDebug.ToDo("check");
                int idx_max = (int)Math.Round((freq_upto - freq_from) / freq_delta);

                List<(double freq, List<int> idxs)> freq_idxs = new List<(double freq, List<int> idxs)>(idx_max + 1);
                for(int idx=0; idx<idx_max+1; idx++)
                {
                    double freq = freq_from + freq_delta * idx;
                    freq_idxs.Add((freq, new List<int>()));
                    HDebug.Assert(freq_idxs[idx].freq == freq);
                }

                for(int i=0; i<freqs.Count; i++)
                {
                    double freq = freqs[i];
                    int idx = (int)Math.Round((freq - freq_from) / freq_delta);
                    double freqto = freq_from + freq_delta * idx;
                    if(idx < 0      ) continue;
                    if(idx > idx_max) continue;
                    freq_idxs[idx].Item2.Add(i);
                }

                return freq_idxs;
            }
            public static List<(double freq, double prob)> GetFreqSpectrum
                ( IList<double> freqs
                , List<(double freq, List<int> idxs)> freq_idxs
                , double freq_delta
                )
            {
                HDebug.ToDo("check");

                List<(double freq, double prob)> spectrum = new List<(double freq, double prob)>(freq_idxs.Count);

                for(int i=0; i<freq_idxs.Count; i++)
                {
                    double freq =  freq_idxs[i].freq;
                    double prob = (freq_idxs[i].idxs.Count() / freqs.Count()) / freq_delta;
                    spectrum.Add((freq, prob));
                }

                return spectrum;
            }
            public static List<(double freq, double prob)> GetFreqSpectrum
                ( IList<double> freqs
                , double freq_from = 0
                , double freq_upto = 4000
                , double freq_delta = 5
                )
            {
                List<(double freq, List<int> idxs)> freq_idxs = GetIdxFreqSpectrum(freqs, freq_from, freq_upto, freq_delta);
                return GetFreqSpectrum(freqs, freq_idxs, freq_delta);
            }
        }
    }
}
