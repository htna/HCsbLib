namespace HTLib2.Bioinfo
{
	public partial class UnitConversion
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
    }
}
