using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class UnitConversion
    {
        /// http://wild.life.nctu.edu.tw/class/common/energy-unit-conv-table.html
        /// 
        /// Most of these numbers have been taken from an old book by Karplus and Porter. To convert from a unit Y in the left hand
        /// column to a unit X in the top row, mutiply by the table element, MX,Y. Example: 1 hartree = 27.2107 eV
        /// When accuracy is very important, I recommend going instead to the NIST website: Fundamental Physical Constants from NIST
        /// (http://physics.nist.gov/cuu/Constants/index.html)
        /// 
        /// Energy Conversion Table
        /// ============================================================================================================================
        ///             hartree             eV                  cm^-1               kcal/mol            kJ/mol          K                   J                   Hz
        /// hartree     1                   27.2107             219 474.63          627.503             2 625.5         315 777.            43.60 x 10-19       6.57966 x 10+15
        /// eV          0.0367502           1                   8 065.73            23.060 9            96.486 9        11 604.9            1.602 10 x 10-19    2.418 04 x 10+14
        /// cm^-1       4.556 33 x 10-6     1.239 81 x 10-4     1                   0.002 859 11        0.011 962 7     1.428 79            1.986 30 x 10-23    2.997 93 x 10+10
        /// kcal/mol    0.001 593 62        0.043 363 4         349.757             1                   4.18400         503.228             6.95 x 10-21        1.048 54 x 10+13
        /// kJ/mol      0.000 380 88        0.010 364 10        83.593              0.239001            1               120.274             1.66 x 10-21        2.506 07 x 10+12
        /// K           0.000 003 166 78    0.000 086 170 5     0.695 028           0.001 987 17        0.008 314 35    1                   1.380 54 x 10-23    2.083 64 x 10+10
        /// J           2.294 x 10+17       6.241 81 x 10+18    5.034 45 x 10+22    1.44 x 10+20        6.02 x 10+20    7.243 54 x 10+22    1                   1.509 30 x 10+33
        /// Hz          1.519 83 x 10-16    4.135 58 x 10-15    3.335 65 x 10-11    9.537 02 x 10-14                    4.799 30 x 10-11    6.625 61 x 10-34    1
    }
}
