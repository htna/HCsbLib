c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1992  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ############################################################
c     ##                                                        ##
c     ##  units.i  --  physical constants and unit conversions  ##
c     ##                                                        ##
c     ############################################################
c
c
c     literature references:
c
c     P. J. Mohr, B. N. Taylor and D. B. Newell, "CODATA Recommended
c     Values of the Fundamental Physical Constants: 2010", Reviews of
c     Modern Physics, 84, 1527-1605 (2012)
c
c     Most values below are taken from 2010 CODATA reference values;
c     available on the web from the National Institute of Standards
c     and Technology at http://physics.nist.gov/constants/
c
c     The conversion from calorie to Joule is the definition of the
c     thermochemical calorie as 1 cal = 4.1840 J from ISO 31-4 (1992)
c
c     The "coulomb" energy conversion factor is found by dimensional
c     analysis of Coulomb's Law, ie, by dividing the square of the
c     elementary charge in Coulombs by 4*pi*eps0*rij, where eps0 is
c     the permittivity of vacuum (the "electric constant"); note that
c     eps0 is typically given in F/m, equivalent to C**2/(J-m)
c
c     The approximate value used for the Debye, 3.33564 x 10-30 C-m,
c     is from IUPAC Compendium of Chemical Technology, 2nd Ed. (1997)
c
c     The value of "prescon" is based on definition of 1 atmosphere
c     as 101325 Pa set by the 10th Conference Generale des Poids et
c     Mesures (1954), where a Pascal (Pa) is equal to a J/m**3
c
c     avogadro    Avogadro's number (N) in particles/mole
c     lightspd    speed of light in vacuum (c) in cm/ps
c     boltzmann   Boltzmann constant (kB) in g*Ang**2/ps**2/mole/K
c     gasconst    ideal gas constant (R) in kcal/mole/K
c     emass       mass of an electron in atomic mass units
c     planck      Planck's constant (h) in J-s
c     joule       conversion from calories to joules
c     convert     conversion from kcal to g*Ang**2/ps**2
c     bohr        conversion from Bohrs to Angstroms
c     hartree     conversion from Hartree to kcal/mole
c     evolt       conversion from Hartree to electron-volts
c     efreq       conversion from Hartree to cm-1
c     coulomb     conversion from electron**2/Ang to kcal/mole
c     debye       conversion from electron-Ang to Debyes
c     prescon     conversion from kcal/mole/Ang**3 to Atm
c
c
      real*8 avogadro,lightspd
      real*8 boltzmann,gasconst
      real*8 emass,planck
      real*8 joule,convert
      real*8 bohr,hartree
      real*8 evolt,efreq
      real*8 coulomb,debye
      real*8 prescon
      parameter (avogadro=6.02214129d+23)
      parameter (lightspd=2.99792458d-2)
      parameter (boltzmann=0.831446215d0)
      parameter (gasconst=1.98720415d-3)
      parameter (emass=5.485799095d-4)
      parameter (planck=6.62606957d-34)
      parameter (joule=4.1840d0)
      parameter (convert=4.1840d+2)
      parameter (bohr=0.52917721092d0)
      parameter (hartree=627.5094743d0)
      parameter (evolt=27.21138503d0)
      parameter (efreq=2.194746313708d+5)
      parameter (coulomb=332.063714d0)
      parameter (debye=4.80321d0)
      parameter (prescon=6.85684112d+4)
