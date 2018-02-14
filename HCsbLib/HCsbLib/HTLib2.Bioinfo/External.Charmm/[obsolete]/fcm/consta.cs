using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Charmm
{
public partial class Src
{
//CHARMM Element source/fcm/consta.fcm 1.1
///-----------------------------------------------------------------------
/// this file contains all physical and mathematical constants
/// and conversion factors.
///
///  The following units are used:
///
///   length: Angstroms
///   time: ps
///   energy: Kcal/mol
///   mass: atomic-mass-unit
///   charge: electron-charge
///   pressure: atmosphere
///
/// physical constants in SI units
/// ------------------------------
///     Kb = 1.380662 E-23 J/K
///     Na = 6.022045 E23  1/mol
///     e = 1.6021892 E-19 C
///     eps = 8.85418782 E-12 F/m
///
///     1 Kcal = 4184.0 J
///     1 amu = 1.6605655 E-27 Kg
///     1 A = 1.0 E-10 m
///
/// reference: CRC Handbook for Chemistry and Physics, 1983/84
///
/// PI and conversion to degrees
//      REAL*8 PI,RADDEG,DEGRAD,TWOPI
//      PARAMETER(PI=3.14159265358979323846D0,TWOPI=2.0D0*PI)
//      PARAMETER (RADDEG=180.0D0/PI)
//      PARAMETER (DEGRAD=PI/180.0D0)
    static readonly double PI=3.14159265358979323846, TWOPI=2.0*PI;
    static readonly double RADDEG=180.0/PI;
    static readonly double DEGRAD=PI/180.0;

///
/// Maximum cosine value for angle and dihedral tolerance code.
//      REAL*8 COSMAX
//      PARAMETER (COSMAX=0.9999999999D0)
    static readonly double COSMAX = 0.9999999999;
///
///     TIMFAC is the conversion factor from AKMA time to picoseconds.
///            (TIMFAC = SQRT ( ( 1A )**2 * 1amu * Na  / 1Kcal )
///            this factor is used only intrinsically, all I/O is in ps.
///
//      REAL*8 TIMFAC
//      PARAMETER (TIMFAC=4.88882129D-02)
///      PARAMETER (TIMFAC=0.04888826)
///
/// KBOLTZ is Boltzman constant AKMA units (KBOLTZ = N *K  / 1 Kcal)
///                                                   a  b
//      REAL*8 KBOLTZ
//      PARAMETER (KBOLTZ=1.987191D-03)
///
/// CCELEC is 1/ (4 pi eps ) in AKMA units, conversion from SI
/// units: CCELEC = e*e*Na / (4*pi*eps*1Kcal*1A)
///
//      REAL*8 CCELEC
///      PARAMETER (CCELEC=332.0636D0)! old value of dubious origin
///      PARAMETER (CCELEC=331.843D0) ! value from 1986-1987 CRC Handbook
///                                   ! of Chemistry and Physics
//##IF AMBER
/// Note: This value provides compatibility with electrostatics in AMBER
//      PARAMETER (CCELEC=332.0522173D0)
//##ELIF DISCOVER
/// Note: This value provides compatibility with electrostatics in DISCOVER
//      PARAMETER (CCELEC=332.054D0)
//##ELSE
/// Note: this old CHARMM value is kept for compatibility reasons
//      PARAMETER (CCELEC=332.0716D0)
//##ENDIF
///
/// CNVFRQ converts from root eigenvalues to frequencies in CM-1.
///        CNVFRQ = SQRT( 1KCAL )/( C * 2 * Pi )
///
//      REAL*8 CNVFRQ
//      PARAMETER (CNVFRQ=2045.5D0/(2.99793D0*6.28319D0))
///
/// SPEEDL is the speed of light in cm/ps
///
//      REAL*8 SPEEDL
//      PARAMETER (SPEEDL=2.99793D-02)
///
/// ATMOSP is the converting factor to calculate the boundary pressure
///     1 ATMOSP = 1.01325   10**5  Joule/meter**3
///              = 1.4584007 10**-5 Kcal/Mol/angs.**3
//      REAL*8 ATMOSP
//      PARAMETER (ATMOSP=1.4584007D-05)
///
/// PATMOS is the inverse of ATMOSP.
///
//      REAL*8 PATMOS
//      PARAMETER (PATMOS = 1.D0 / ATMOSP )
///
/// BOHRR is the Bohr radius.
///  (This constant is consistant with GAMESS)
///
//      REAL*8 BOHRR
//      PARAMETER (BOHRR = 0.529177249D0 )
///
///
/// TOKCAL - conversion from atomic units to kcal/mole
/// In GAMESS the number is 627.51d0
/// This number is from GAUSSIAN
///
//      REAL*8 TOKCAL
//      PARAMETER (TOKCAL = 627.5095D0 )
///
//##IF MMFF
/// Jay Banks 25 Oct 95: added MMFF-specific constants
///
///   MDAKCAL = Conversion factor from millidynes-Angstroms to kcal/mol;
///   based on Avogadro's number = 6.0221367 x 10**23 (E. R. Cohen and B.
///   N. Taylor, "The 1986 Adjustment of the Fundamental Physical
///   Constants," Pergamon, Elmsford, NY, Vol 63, 1986, and on 4.184 joules
///   = 1 calorie (Pure and Applied Chemistry, 51, 1 (1979)
///
//      real*8 MDAKCAL ! Conversion factor from mdyne-A to kcal/mol
//      parameter(MDAKCAL=143.9325D0)
///
//##ENDIF
///
/// DEBYEC - convert from atomic charges and Angstroms to Debyes.
///  (This constant is consistant with GAMESS)
///
//      REAL*8 DEBYEC
//      PARAMETER ( DEBYEC = 2.541766D0 / BOHRR )
///
/// ZEROC - Temperature of 0C (in Kelvin)
///
//      REAL*8 ZEROC,ROOMT
//      PARAMETER ( ZEROC = 273.15D0 )
//      PARAMETER ( ROOMT = 298.15D0 )
///
//
///.. 30a2fix 002 sb/tr ##IF PATHINT PBEQ
///     ARD 00-11-28
///     Moved these constants out of EPINT to avoid duplicating them in 
///     EPIMC.  Curiously the kcal value appears to be a little different 
///     from the value given in the comments at the top of the file.
//      REAL*8     HPLANCK, HBAR
//      PARAMETER (HPLANCK=6.626176D-34, HBAR=HPLANCK/TWOPI)
// 
//      REAL*8     JKBOLTZ, AVOGADRO
//      PARAMETER (JKBOLTZ=1.380662D-23, AVOGADRO=6.022045D23)
// 
//      REAL*8     JKCAL, KCALMOL
//      PARAMETER (JKCAL=4186.05D0, KCALMOL=JKCAL/AVOGADRO)
// 
//      REAL*8     AMU, ANGSTROM
//      PARAMETER (AMU=1.6605655D-27, ANGSTROM=1.0D-10)
///.. 30a2fix 002 ##ENDIF
///
}
}
}
