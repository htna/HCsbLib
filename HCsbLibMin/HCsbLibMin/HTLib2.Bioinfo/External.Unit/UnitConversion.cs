using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class UnitConversion
    {
        /// http://www.newlightphotonics.com/v1/energy-converter2.html
        /// 
        /// Unit                | Comments
        /// =================================================================================================
        /// Wavenumber(cm-1)    | A wavelength of energy that is also called a reciprocal centimeter.
        ///                     | Wavenumbers are obtained when frequency is expressed in Hertz and
        ///                     | the speed of light is expressed in cm/s. This unit is commonly used
        ///                     | in infrared spectroscopy.
        /// -------------------------------------------------------------------------------------------------
        /// Kilojoules per mole | A Joule, J, is the SI unit of energy and is defined as one kg.m2/s2.
        /// (kJ/mol)            | The prefix "kilo" means 1,000, so one kJ = 1,000 J. As the energies
        ///                     | associated with a single molecule or atom are quite small, we often
        ///                     | find it easier to discuss the energy found in one mole of the
        ///                     | substance, hence "per mole". To get the energy for one molecule,
        ///                     | divide kJ/mol by Avogadro's number, 6.022 x 1023.
        /// -------------------------------------------------------------------------------------------------
        /// Kilocalories per    | A calorie was originally defined as the amount of energy required
        /// mole (kcal/mol)     | to raise the temperature of one gram of water by one degree Celsius.
        ///                     | One calorie = 4.184 J. One kcal = 1,000 cal.
        /// -------------------------------------------------------------------------------------------------
        /// Nanometer(nm)       | The prefix "nano" means 1 x 10-9 = 0.000000001 = 1/1,000,000,000.
        ///                     | Therefore, a nanometer refers to energy with a wavelength that is
        ///                     | 1/1,000,000,000th of a meter. Visible light is made up of
        ///                     | electromagnetic radiation that has wavelengths ranging from roughly
        ///                     | 400 to 800 nm.
        /// -------------------------------------------------------------------------------------------------
        /// Hertz (s-1, Hz)     | A Hertz is a unit of frequency defined as a reciprocal second, s-1.
        ///                     | For example, AC current cycles polarity 60 times per second, so we
        ///                     | could call this 60 Hz = 60 s-1. Human hearing has a frequency range
        ///                     | from a few hundred Hz up to approximately 20,000 Hz.
        /// -------------------------------------------------------------------------------------------------
        /// Megahertz (MHz)     | The prefix "mega" means 1,000,000, so there are 1,000,000 Hz in one
        ///                     | MHz. This is a typical frequency for radio equipment as well as
        ///                     | high-tech scientific instruments such as magnetic resonance imaging
        ///                     | (MRI, or NMR) scanners.
        /// -------------------------------------------------------------------------------------------------
        /// Electron Volt (eV)  | The electron volt is the energy that we would give an electron if
        ///                     | it were accelerated by a one volt potential difference.
        ///                     | 1 eV = 1.602 x 10-19 J. This term is most often used by physicists
        ///                     | and electrochemists.
    }
}
