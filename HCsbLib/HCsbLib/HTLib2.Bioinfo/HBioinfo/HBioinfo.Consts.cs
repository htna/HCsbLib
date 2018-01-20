using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        public const double BOLTZMANN = (1.380658e-23)       ;// (J/K)
        public const double AVOGADRO  = (6.0221367e23)       ;// ()
        public const double KILO      = (1e3)                ;// Thousand
        public const double RGAS      = (BOLTZMANN*AVOGADRO) ;// (J/(mol K))
        public const double BOLTZ     = (RGAS/KILO)          ;// (kJ/(mol K))
    }
}
