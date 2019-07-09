using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class UnitConversion
	{
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
    }
}
