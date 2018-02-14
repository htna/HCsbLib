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
//CHARMM Element source/fcm/dimens.fcm 1.1                                               ;//CHARMM Element source/fcm/dimens.fcm 1.1
///                                                                                      ;//C
/// This common file contains all useful dimensioning information.                       ;//C This common file contains all useful dimensioning information.
///                                   BRB - 01/12/89                                     ;//C                                   BRB - 01/12/89
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///     Standard Size parameters                                                         ;//C     Standard Size parameters
///                                                                                      ;//C
///   HUGE    version -  1,000,000 atoms                                                 ;//C   HUGE    version -  1,000,000 atoms
///   XXLARGE version -   ~360,000 atoms                                                 ;//C   XXLARGE version -   ~360,000 atoms 
///   XLARGE  version -   ~240,000 atoms                                                 ;//C   XLARGE  version -   ~240,000 atoms 
///   LARGE   version -    ~60,000 atoms                                                 ;//C   LARGE   version -    ~60,000 atoms
///   MEDIUM  version -    ~25,000 atoms                                                 ;//C   MEDIUM  version -    ~25,000 atoms
///   SMALL   version -     ~6,000 atoms                                                 ;//C   SMALL   version -     ~6,000 atoms
///   XSMALL  version -     ~2,000 atoms                                                 ;//C   XSMALL  version -     ~2,000 atoms
///   REDUCE  version - A special version for non-virtual memory machines.               ;//C   REDUCE  version - A special version for non-virtual memory machines.
///                    There is an attempt to greatly limit static memory.               ;//C                    There is an attempt to greatly limit static memory.
///                                                                                      ;//C
///   The actual size varies by machine type.                                            ;//C   The actual size varies by machine type.
///   It is listed in the header of the CHARMM output file.                              ;//C   It is listed in the header of the CHARMM output file.
///                                                                                      ;//C
//      INTEGER LARGE,MEDIUM,SMALL,REDUCE                                                ;//      INTEGER LARGE,MEDIUM,SMALL,REDUCE
//##IF QUANTA                                                                            ;//##IF QUANTA
//      PARAMETER (LARGE=60000, MEDIUM=30000, SMALL=15000)                               ;//      PARAMETER (LARGE=60000, MEDIUM=30000, SMALL=15000)
//##ELIF T3D                                                                             ;//##ELIF T3D
//      PARAMETER (LARGE=30120, MEDIUM=20160, SMALL=6120)                                ;//      PARAMETER (LARGE=30120, MEDIUM=20160, SMALL=6120)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (LARGE=60120, MEDIUM=25140, SMALL=6120)                                ;//      PARAMETER (LARGE=60120, MEDIUM=25140, SMALL=6120)
//##ENDIF                                                                                ;//##ENDIF
//      PARAMETER (REDUCE=15000)                                                         ;//      PARAMETER (REDUCE=15000)
//      INTEGER SIZE                                                                     ;//      INTEGER SIZE
//##IF XLARGE                                                                            ;//##IF XLARGE
//      PARAMETER (SIZE=LARGE*4)                                                         ;//      PARAMETER (SIZE=LARGE*4)
//##ELIF XXLARGE                                                                         ;//##ELIF XXLARGE
//      PARAMETER (SIZE=LARGE*6)                                                         ;//      PARAMETER (SIZE=LARGE*6)
//##ELIF HUGE                                                                            ;//##ELIF HUGE
//      PARAMETER (SIZE=1000000)                                                         ;//      PARAMETER (SIZE=1000000)
//##ELIF LARGE                                                                           ;//##ELIF LARGE
//      PARAMETER (SIZE=LARGE)                                                           ;//      PARAMETER (SIZE=LARGE)
//##ELIF MEDIUM                                                                          ;//##ELIF MEDIUM
//      PARAMETER (SIZE=MEDIUM)                                                          ;//      PARAMETER (SIZE=MEDIUM)
//##ELIF REDUCE                                                                          ;//##ELIF REDUCE
//      PARAMETER (SIZE=REDUCE)                                                          ;//      PARAMETER (SIZE=REDUCE)
//##ELIF SMALL                                                                           ;//##ELIF SMALL
//      PARAMETER (SIZE=SMALL)                                                           ;//      PARAMETER (SIZE=SMALL)
//##ELIF XSMALL                                                                          ;//##ELIF XSMALL
//      PARAMETER (SIZE=SMALL/3)                                                         ;//      PARAMETER (SIZE=SMALL/3)
//##ENDIF                                                                                ;//##ENDIF
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
//##IF MMFF                                                                              ;//##IF MMFF
/// MMFF-specific information from (MSI/Merck version of) dimens.fcm                     ;//C MMFF-specific information from (MSI/Merck version of) dimens.fcm
///                                                                                      ;//C
//      integer MAXDEFI       ! maximum number of atom types                             ;//      integer MAXDEFI       ! maximum number of atom types
//      parameter(MAXDEFI=250)! not to be confused with maximum                          ;//      parameter(MAXDEFI=250)! not to be confused with maximum
///                           ! allowed atom type MAXATC                                 ;//C                           ! allowed atom type MAXATC
///                                                                                      ;//C
///  NAME0  = LENGTH OF CHARACTER STRING FOR AN ATOM NAME AS RETURNED                    ;//C  NAME0  = LENGTH OF CHARACTER STRING FOR AN ATOM NAME AS RETURNED
///           BY FUNCTION NAME OR XNAME                                                  ;//C           BY FUNCTION NAME OR XNAME
///  NAMEQ0 = LENGTH OF CHARACTER STRING FOR A PARTIALLY OR FULLY                        ;//C  NAMEQ0 = LENGTH OF CHARACTER STRING FOR A PARTIALLY OR FULLY
///           QUALIFIED ATOM NAME AS RETURNED BY FUNCTION QNAME OR XQNAME                ;//C           QUALIFIED ATOM NAME AS RETURNED BY FUNCTION QNAME OR XQNAME
///  NRES0  = LENGTH OF CHARACTER STRING FOR A RESIDUE NAME FIELD                        ;//C  NRES0  = LENGTH OF CHARACTER STRING FOR A RESIDUE NAME FIELD
///  KRES0  = LENGTH OF CHARACTER STRING FOR A RESIDUE TYPE (E.G., "ALA")                ;//C  KRES0  = LENGTH OF CHARACTER STRING FOR A RESIDUE TYPE (E.G., "ALA")
///                                                                                      ;//C
//      INTEGER NAME0,NAMEQ0,NRES0,KRES0                                                 ;//      INTEGER NAME0,NAMEQ0,NRES0,KRES0
//      PARAMETER (NAME0=4,NAMEQ0=10,NRES0=4,KRES0=4)                                    ;//      PARAMETER (NAME0=4,NAMEQ0=10,NRES0=4,KRES0=4)
///                                                                                      ;//C
//      integer MaxAtN  ! maximum atomic number                                          ;//      integer MaxAtN  ! maximum atomic number
//      parameter (MaxAtN=55)                                                            ;//      parameter (MaxAtN=55)
//                                                                                       ;//
//      INTEGER MAXAUX ! Maximum number of auxiliary parameters                          ;//      INTEGER MAXAUX ! Maximum number of auxiliary parameters
//      PARAMETER (MAXAUX = 10) ! AuxPar(MAXAUX)                                         ;//      PARAMETER (MAXAUX = 10) ! AuxPar(MAXAUX)
//                                                                                       ;//
//##ENDIF                                                                                ;//##ENDIF
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  cnst.fcm                                                                     ;//C  FROM:  cnst.fcm
///                                                                                      ;//C
///  MAXHSET - The maximum number of harmonic restraint sets                             ;//C  MAXHSET - The maximum number of harmonic restraint sets
///  MAXCSP  - The maximum number of restrained dihedrals.                               ;//C  MAXCSP  - The maximum number of restrained dihedrals.
///                                                                                      ;//C
//      INTEGER MAXCSP, MAXHSET                                                          ;//      INTEGER MAXCSP, MAXHSET
//      PARAMETER (MAXHSET = 200)                                                        ;//      PARAMETER (MAXHSET = 200)
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (MAXCSP = 10)                                                          ;//      PARAMETER (MAXCSP = 10)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXCSP = 500)                                                         ;//      PARAMETER (MAXCSP = 500)
//##ENDIF                                                                                ;//##ENDIF
//##IF HMCM                                                                              ;//##IF HMCM
//      INTEGER MAXHCM,MAXPCM,MAXRCM                                                     ;//      INTEGER MAXHCM,MAXPCM,MAXRCM
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (MAXHCM=150)                                                           ;//      PARAMETER (MAXHCM=150)
//      PARAMETER (MAXPCM=500)                                                           ;//      PARAMETER (MAXPCM=500)
//      PARAMETER (MAXRCM=500)                                                           ;//      PARAMETER (MAXRCM=500)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXHCM=500)                                                           ;//      PARAMETER (MAXHCM=500)
//      PARAMETER (MAXPCM=5000)                                                          ;//      PARAMETER (MAXPCM=5000)
//      PARAMETER (MAXRCM=2000)                                                          ;//      PARAMETER (MAXRCM=2000)
//##ENDIF                                                                                ;//##ENDIF
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  comand.fcm                                                                   ;//C  FROM:  comand.fcm
///                                                                                      ;//C
///  MXCMSZ - The maximum command length (inluding all continuation lines)               ;//C  MXCMSZ - The maximum command length (inluding all continuation lines)
///                                                                                      ;//C
//      INTEGER MXCMSZ                                                                   ;//      INTEGER MXCMSZ
//##IF IBM IBMRS CRAY INTEL IBMSP T3D REDUCE                                             ;//##IF IBM IBMRS CRAY INTEL IBMSP T3D REDUCE
//      PARAMETER (MXCMSZ = 500)                                                         ;//      PARAMETER (MXCMSZ = 500)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MXCMSZ = 5000)                                                        ;//      PARAMETER (MXCMSZ = 5000)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  cstack.fcm                                                                   ;//C  FROM:  cstack.fcm
///                                                                                      ;//C
///  CHRSIZ - Length of the character stack (should be at least as                       ;//C  CHRSIZ - Length of the character stack (should be at least as
///           large as MAXA).                                                            ;//C           large as MAXA).
///                                                                                      ;//C
//      INTEGER CHRSIZ                                                                   ;//      INTEGER CHRSIZ
//      PARAMETER (CHRSIZ = SIZE)                                                        ;//      PARAMETER (CHRSIZ = SIZE)
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  etable.fcm                                                                   ;//C  FROM:  etable.fcm
///                                                                                      ;//C
///  MAXATB - Maximum number of atom types for table uasge. Should                       ;//C  MAXATB - Maximum number of atom types for table uasge. Should
///           probably match MAXATC.                                                     ;//C           probably match MAXATC.
///                                                                                      ;//C
//      INTEGER MAXATB                                                                   ;//      INTEGER MAXATB
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (MAXATB = 10)                                                          ;//      PARAMETER (MAXATB = 10)
//##ELIF QUANTA                                                                          ;//##ELIF QUANTA
//      PARAMETER (MAXATB = 500)                                                         ;//      PARAMETER (MAXATB = 500)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXATB = 200)                                                         ;//      PARAMETER (MAXATB = 200)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  graph.fcm                                                                    ;//C  FROM:  graph.fcm
///                                                                                      ;//C
///  IATBMX - Maximum number of bonds for any single atom.                               ;//C  IATBMX - Maximum number of bonds for any single atom.
///                                                                                      ;//C
//      INTEGER IATBMX                                                                   ;//      INTEGER IATBMX
//      PARAMETER (IATBMX = 8)                                                           ;//      PARAMETER (IATBMX = 8)
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  hbond.fcm                                                                    ;//C  FROM:  hbond.fcm
///                                                                                      ;//C
///  MAXHB - The maximum number of active hydrogen bonds.                                ;//C  MAXHB - The maximum number of active hydrogen bonds.
///      Note: Hydrogen bonds removed by post processing of hbond list                   ;//C      Note: Hydrogen bonds removed by post processing of hbond list
///      for the BEST and HBEXcl option also count against this total.                   ;//C      for the BEST and HBEXcl option also count against this total.
///                                                                                      ;//C
//      INTEGER MAXHB                                                                    ;//      INTEGER MAXHB
//##IF LARGE XLARGE XXLARGE HUGE                                                         ;//##IF LARGE XLARGE XXLARGE HUGE
//      PARAMETER (MAXHB = 14000)                                                        ;//      PARAMETER (MAXHB = 14000)
//##ELIF MEDIUM                                                                          ;//##ELIF MEDIUM
//      PARAMETER (MAXHB = 8000)                                                         ;//      PARAMETER (MAXHB = 8000)
//##ELIF SMALL                                                                           ;//##ELIF SMALL
//      PARAMETER (MAXHB = 2500)                                                         ;//      PARAMETER (MAXHB = 2500)
//##ELIF REDUCE XSMALL                                                                   ;//##ELIF REDUCE XSMALL
//      PARAMETER (MAXHB = 20)                                                           ;//      PARAMETER (MAXHB = 20)
//##ELSE                                                                                 ;//##ELSE
//##ERROR 'Unrecognized size directive in DIMENS.FCM.'                                   ;//##ERROR 'Unrecognized size directive in DIMENS.FCM.'
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  image.fcm                                                                    ;//C  FROM:  image.fcm
///                                                                                      ;//C
///  MAXTRN - The maximum number of image transformations.                               ;//C  MAXTRN - The maximum number of image transformations.
///                                                                                      ;//C
//      INTEGER MAXTRN,MAXSYM                                                            ;//      INTEGER MAXTRN,MAXSYM
//##IFN NOIMAGES                                                                         ;//##IFN NOIMAGES
//      PARAMETER (MAXTRN = 5000)                                                        ;//      PARAMETER (MAXTRN = 5000)
///                                                                                      ;//C
///  MAXSYM - The maximum number of crystal symmetry operations allowed.                 ;//C  MAXSYM - The maximum number of crystal symmetry operations allowed.
///           The maximum number ever needed in a crystal is 192 but it                  ;//C           The maximum number ever needed in a crystal is 192 but it
///           is conceivable that in bizarre cases one may require more.                 ;//C           is conceivable that in bizarre cases one may require more.
///               (such as for some finite space groups)                                 ;//C               (such as for some finite space groups)
///                                                                                      ;//C
//      PARAMETER (MAXSYM = 192)                                                         ;//      PARAMETER (MAXSYM = 192)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXTRN = 1)                                                           ;//      PARAMETER (MAXTRN = 1)
//      PARAMETER (MAXSYM = 1)                                                           ;//      PARAMETER (MAXSYM = 1)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  lonepr.fcm                                                                   ;//C  FROM:  lonepr.fcm
///                                                                                      ;//C
///  MAXLP  - Maximum number of lone-pair atoms (typ 100)                                ;//C  MAXLP  - Maximum number of lone-pair atoms (typ 100)
///  MAXLPH - Maximum number of lone-pair hosts (typ 500)                                ;//C  MAXLPH - Maximum number of lone-pair hosts (typ 500)
///                                                                                      ;//C
//##IF LONEPAIR (lonepair_max)                                                           ;//##IF LONEPAIR (lonepair_max)
//      INTEGER MAXLP,MAXLPH                                                             ;//      INTEGER MAXLP,MAXLPH
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (MAXLP  = 10)                                                          ;//      PARAMETER (MAXLP  = 10)
//      PARAMETER (MAXLPH = 20)                                                          ;//      PARAMETER (MAXLPH = 20)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXLP  = 20000)                                                       ;//      PARAMETER (MAXLP  = 20000)
//      PARAMETER (MAXLPH = 20000)                                                       ;//      PARAMETER (MAXLPH = 20000)
//##ENDIF                                                                                ;//##ENDIF
//##ENDIF (lonepair_max)                                                                 ;//##ENDIF (lonepair_max)
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  noe.fcm                                                                      ;//C  FROM:  noe.fcm
///                                                                                      ;//C
///  NOEMAX - The maximum number of NOE restraints.                                      ;//C  NOEMAX - The maximum number of NOE restraints.
///  NOEMX2 - The maximum number of NOE atoms.                                           ;//C  NOEMX2 - The maximum number of NOE atoms.
///                                                                                      ;//C
//      INTEGER NOEMAX,NOEMX2                                                            ;//      INTEGER NOEMAX,NOEMX2
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (NOEMAX = 10)                                                          ;//      PARAMETER (NOEMAX = 10)
//      PARAMETER (NOEMX2 = 20)                                                          ;//      PARAMETER (NOEMX2 = 20)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (NOEMAX = 2000)                                                        ;//      PARAMETER (NOEMAX = 2000)
//      PARAMETER (NOEMX2 = 4000)                                                        ;//      PARAMETER (NOEMX2 = 4000)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  param.fcm                                                                    ;//C  FROM:  param.fcm
///                                                                                      ;//C
///  MAXATC - Maximum number of different atom types.                                    ;//C  MAXATC - Maximum number of different atom types.
///  MAXCB  - Maximum number of bond parameters.                                         ;//C  MAXCB  - Maximum number of bond parameters.
///  MAXCT  - Maximum number of angle parameters.                                        ;//C  MAXCT  - Maximum number of angle parameters.
///  MAXCP  - Maximum number of dihedral parameters.                                     ;//C  MAXCP  - Maximum number of dihedral parameters.
///  MAXCTP - Maximum number of cross-term maps                  !## CMAP                ;//C  MAXCTP - Maximum number of cross-term maps                  !## CMAP
///  MAXCI  - Maximum number of improper dihedral parameters.                            ;//C  MAXCI  - Maximum number of improper dihedral parameters.
///  MAXCN  - Maximum number of vdw lookup values                                        ;//C  MAXCN  - Maximum number of vdw lookup values
///  MAXCH  - Maximum number of hydrogen bond parameters.                                ;//C  MAXCH  - Maximum number of hydrogen bond parameters.
///  MAXNBF - Maximum number of nonbond fixes (vdw).                                     ;//C  MAXNBF - Maximum number of nonbond fixes (vdw).
///  MAXACTEQV- Maximum number of atom equivalences            !##FLEXPARM               ;//C  MAXACTEQV- Maximum number of atom equivalences            !##FLEXPARM
///                                                                                      ;//C
//      INTEGER MAXATC, MAXCB, MAXCH, MAXCI, MAXCP, MAXCT, MAXITC, MAXNBF                ;//      INTEGER MAXATC, MAXCB, MAXCH, MAXCI, MAXCP, MAXCT, MAXITC, MAXNBF
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (MAXATC = 200, MAXCB = 700,  MAXCH = 1600, MAXCI = 500,                ;//      PARAMETER (MAXATC = 200, MAXCB = 700,  MAXCH = 1600, MAXCI = 500,
//     &           MAXCP  = 500, MAXCT = 2500, MAXITC=   95, MAXNBF= 100)                ;//     &           MAXCP  = 500, MAXCT = 2500, MAXITC=   95, MAXNBF= 100)
//##ELIF MMFF CFF                                                                        ;//##ELIF MMFF CFF
//      PARAMETER (MAXATC = 500, MAXCB = 1500, MAXCH = 3200, MAXCI = 600,                ;//      PARAMETER (MAXATC = 500, MAXCB = 1500, MAXCH = 3200, MAXCI = 600,
//     &           MAXCP  = 3000,MAXCT = 15500,MAXITC = 200, MAXNBF=1000)                ;//     &           MAXCP  = 3000,MAXCT = 15500,MAXITC = 200, MAXNBF=1000)
//##ELIF YAMMP                                                                           ;//##ELIF YAMMP
//      PARAMETER (MAXATC = 1500, MAXCB = 2000, MAXCH = 300, MAXCI = 1000,               ;//      PARAMETER (MAXATC = 1500, MAXCB = 2000, MAXCH = 300, MAXCI = 1000,
//     &           MAXCP  = 1000, MAXCT = 2000, MAXITC=  200, MAXNBF=1000)               ;//     &           MAXCP  = 1000, MAXCT = 2000, MAXITC=  200, MAXNBF=1000)
//##ELIF LARGE XLARGE XXLARGE HUGE                                                       ;//##ELIF LARGE XLARGE XXLARGE HUGE
//      PARAMETER (MAXATC = 500, MAXCB = 1500, MAXCH = 3200, MAXCI = 600,                ;//      PARAMETER (MAXATC = 500, MAXCB = 1500, MAXCH = 3200, MAXCI = 600,
//     &           MAXCP  = 1500, MAXCT = 5500, MAXITC=  200, MAXNBF=1000)               ;//     &           MAXCP  = 1500, MAXCT = 5500, MAXITC=  200, MAXNBF=1000)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXATC = 500, MAXCB = 1500, MAXCH = 3200, MAXCI = 600,                ;//      PARAMETER (MAXATC = 500, MAXCB = 1500, MAXCH = 3200, MAXCI = 600,
//     &           MAXCP  = 700, MAXCT = 5500, MAXITC=  200, MAXNBF=1000)                ;//     &           MAXCP  = 700, MAXCT = 5500, MAXITC=  200, MAXNBF=1000)
//##ENDIF                                                                                ;//##ENDIF
//      INTEGER MAXCN                                                                    ;//      INTEGER MAXCN
//      PARAMETER (MAXCN = MAXITC*(MAXITC+1)/2)                                          ;//      PARAMETER (MAXCN = MAXITC*(MAXITC+1)/2)
///                                                                                      ;//C
//      INTEGER MAXCTP                                           !##CMAP                 ;//      INTEGER MAXCTP                                           !##CMAP
//      PARAMETER (MAXCTP = 10)                                  !##CMAP                 ;//      PARAMETER (MAXCTP = 10)                                  !##CMAP
//      INTEGER MAXACTEQV                                        !##FLEXPARM             ;//      INTEGER MAXACTEQV                                        !##FLEXPARM
//      PARAMETER (MAXACTEQV = 40)                               !##FLEXPARM             ;//      PARAMETER (MAXACTEQV = 40)                               !##FLEXPARM
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  psf.fcm                                                                      ;//C  FROM:  psf.fcm
///                                                                                      ;//C
///  MAXA   - Maximum number of atoms.                                                   ;//C  MAXA   - Maximum number of atoms.
///  MAXAIM - Maximum number of atoms including image atoms.                             ;//C  MAXAIM - Maximum number of atoms including image atoms.
///  MAXB   - Maximum number of bonds.                                                   ;//C  MAXB   - Maximum number of bonds.
///  MAXT   - Maximum number of angles (thetas).                                         ;//C  MAXT   - Maximum number of angles (thetas).
///  MAXP   - Maximum number of dihedrals (phis).                                        ;//C  MAXP   - Maximum number of dihedrals (phis).
///  MAXIMP - Maximum number of improper dihedrals.                                      ;//C  MAXIMP - Maximum number of improper dihedrals.
///  MAXCRT - Maximum number of cross terms                      !## CMAP                ;//C  MAXCRT - Maximum number of cross terms                      !## CMAP
///  MAXNB  - Maximum number of explicit nonbond exclusions.                             ;//C  MAXNB  - Maximum number of explicit nonbond exclusions.
///  MAXPAD - Maximum number of donors or acceptors.                                     ;//C  MAXPAD - Maximum number of donors or acceptors.
///  MAXRES - Maximum number of residues.                                                ;//C  MAXRES - Maximum number of residues.
///  MAXSEG - Maximum number of segments.                                                ;//C  MAXSEG - Maximum number of segments.
///  MAXGRP - Maximum number of groups.                                                  ;//C  MAXGRP - Maximum number of groups.
///  MAXZ14 - Maximum size of Z14 table. Should be nearly indep of prot size.            ;//C  MAXZ14 - Maximum size of Z14 table. Should be nearly indep of prot size.
///  MXZ14G - Maximum size of Z14G table. Should be nearly indep of prot size.           ;//C  MXZ14G - Maximum size of Z14G table. Should be nearly indep of prot size.
///                                                                                      ;//C
    static int MAXA, MAXAIM, MAXB, MAXT, MAXP                                            ;//      INTEGER MAXA, MAXAIM, MAXB, MAXT, MAXP
//      INTEGER MAXIMP, MAXNB, MAXPAD, MAXRES                                            ;//      INTEGER MAXIMP, MAXNB, MAXPAD, MAXRES
//      INTEGER MAXCRT                                           !## CMAP                ;//      INTEGER MAXCRT                                           !## CMAP
//      INTEGER MAXSEG, MAXGRP                                                           ;//      INTEGER MAXSEG, MAXGRP
//##IF LARGE XLARGE XXLARGE                                                              ;//##IF LARGE XLARGE XXLARGE
//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE*2,                              ;//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE*2,
//     &           MAXP = SIZE*3)                                                        ;//     &           MAXP = SIZE*3)
//      PARAMETER (MAXIMP =SIZE/2, MAXNB = 17200, MAXPAD = 72000,                        ;//      PARAMETER (MAXIMP =SIZE/2, MAXNB = 17200, MAXPAD = 72000,
//     &           MAXRES =SIZE/4)        ! was 72000                                    ;//     &           MAXRES =SIZE/4)        ! was 72000
//      PARAMETER (MAXCRT = 20000)                               !## CMAP                ;//      PARAMETER (MAXCRT = 20000)                               !## CMAP
//                                                                                       ;//
//##IF MCSS XXLARGE                                                                      ;//##IF MCSS XXLARGE
//      PARAMETER (MAXSEG = 10000)                                                       ;//      PARAMETER (MAXSEG = 10000)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXSEG = 1000)                                                        ;//      PARAMETER (MAXSEG = 1000)
//##ENDIF                                                                                ;//##ENDIF
//##ELIF HUGE                                                                            ;//##ELIF HUGE
//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE*2,                              ;//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE*2,
//     &           MAXP = SIZE*3)                                                        ;//     &           MAXP = SIZE*3)
//      PARAMETER (MAXIMP =SIZE/2, MAXNB = 17200, MAXPAD = 72000,                        ;//      PARAMETER (MAXIMP =SIZE/2, MAXNB = 17200, MAXPAD = 72000,
//     &           MAXRES =800000)                                                       ;//     &           MAXRES =800000)
//      PARAMETER (MAXCRT = 20000)                               !## CMAP                ;//      PARAMETER (MAXCRT = 20000)                               !## CMAP
//      PARAMETER (MAXSEG = 10000)                                                       ;//      PARAMETER (MAXSEG = 10000)
//##ELIF MEDIUM                                                                          ;//##ELIF MEDIUM
//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE*2,                              ;//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE*2,
//     &           MAXP = 3*SIZE)                                                        ;//     &           MAXP = 3*SIZE)
//      PARAMETER (MAXIMP = SIZE/2, MAXNB = 17200, MAXPAD = 8160,                        ;//      PARAMETER (MAXIMP = SIZE/2, MAXNB = 17200, MAXPAD = 8160,
//     &           MAXRES = 14000)                                                       ;//     &           MAXRES = 14000)
//      PARAMETER (MAXCRT = 8000)                                !## CMAP                ;//      PARAMETER (MAXCRT = 8000)                                !## CMAP
//                                                                                       ;//
//##IF MCSS                                                                              ;//##IF MCSS
//      PARAMETER (MAXSEG = 5000)                                                        ;//      PARAMETER (MAXSEG = 5000)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXSEG = 1000)                                                        ;//      PARAMETER (MAXSEG = 1000)
//##ENDIF                                                                                ;//##ENDIF
//##ELIF SMALL                                                                           ;//##ELIF SMALL
//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE,                                ;//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = SIZE,
//     &           MAXP = 2*SIZE)                                                        ;//     &           MAXP = 2*SIZE)
//      PARAMETER (MAXIMP = 4200, MAXNB = 6200, MAXPAD = 4160,                           ;//      PARAMETER (MAXIMP = 4200, MAXNB = 6200, MAXPAD = 4160,
//     &           MAXRES = 4000)                                                        ;//     &           MAXRES = 4000)
//      PARAMETER (MAXCRT = 4000)                                !## CMAP                ;//      PARAMETER (MAXCRT = 4000)                                !## CMAP
//##IF MCSS                                                                              ;//##IF MCSS
//      PARAMETER (MAXSEG = 5000)                                                        ;//      PARAMETER (MAXSEG = 5000)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXSEG = 1000)                                                        ;//      PARAMETER (MAXSEG = 1000)
//##ENDIF                                                                                ;//##ENDIF
//##ELIF XSMALL                                                                          ;//##ELIF XSMALL
//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = 2*SIZE,                              ;//      PARAMETER (MAXA = SIZE, MAXB = SIZE, MAXT = 2*SIZE,
//     &           MAXP = SIZE)                                                          ;//     &           MAXP = SIZE)
//      PARAMETER (MAXIMP = 1000, MAXNB = 600, MAXPAD = 300,                             ;//      PARAMETER (MAXIMP = 1000, MAXNB = 600, MAXPAD = 300,
//     &           MAXRES = 500)                                                         ;//     &           MAXRES = 500)
//      PARAMETER (MAXSEG = 500)                                                         ;//      PARAMETER (MAXSEG = 500)
//      PARAMETER (MAXCRT = 1000)       f                        !## CMAP                ;//      PARAMETER (MAXCRT = 1000)       f                        !## CMAP
//##ELIF REDUCE                                                                          ;//##ELIF REDUCE
/// optimal for small protein in water (80% water, 20% protein)                          ;//C optimal for small protein in water (80% water, 20% protein)
//      PARAMETER (MAXA = 15000, MAXB = 15000, MAXT = 9000,                              ;//      PARAMETER (MAXA = 15000, MAXB = 15000, MAXT = 9000,
//     &           MAXP = 15000)                                                         ;//     &           MAXP = 15000)
//      PARAMETER (MAXIMP = 1200, MAXNB = 100, MAXPAD = 1000,                            ;//      PARAMETER (MAXIMP = 1200, MAXNB = 100, MAXPAD = 1000,
//     &           MAXRES = 7000)                                                        ;//     &           MAXRES = 7000)
//      PARAMETER (MAXSEG = 1000)                                                        ;//      PARAMETER (MAXSEG = 1000)
//      PARAMETER (MAXCRT = 1000)                                !## CMAP                ;//      PARAMETER (MAXCRT = 1000)                                !## CMAP
//##ELSE                                                                                 ;//##ELSE
//##ERROR 'Unrecognized size directive in DIMENS.FCM.'                                   ;//##ERROR 'Unrecognized size directive in DIMENS.FCM.'
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
//##IF NOIMAGES                                                                          ;//##IF NOIMAGES
//      PARAMETER (MAXAIM = SIZE)                                                        ;//      PARAMETER (MAXAIM = SIZE)
//      PARAMETER (MAXGRP = SIZE/3)                                                      ;//      PARAMETER (MAXGRP = SIZE/3)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXAIM = 2*SIZE)                                                      ;//      PARAMETER (MAXAIM = 2*SIZE)
//      PARAMETER (MAXGRP = 2*SIZE/3)                                                    ;//      PARAMETER (MAXGRP = 2*SIZE/3)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  resdist.fcm                                                                  ;//C  FROM:  resdist.fcm
///                                                                                      ;//C
///  REDMAX - The maximum number of distance restraints.                                 ;//C  REDMAX - The maximum number of distance restraints.
///  REDMX2 - The maximum number of specified atom pairs.                                ;//C  REDMX2 - The maximum number of specified atom pairs.
///                                                                                      ;//C
//      INTEGER REDMAX,REDMX2                                                            ;//      INTEGER REDMAX,REDMX2
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (REDMAX = 2)                                                           ;//      PARAMETER (REDMAX = 2)
//      PARAMETER (REDMX2 = 8)                                                           ;//      PARAMETER (REDMX2 = 8)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (REDMAX = 20)                                                          ;//      PARAMETER (REDMAX = 20)
//      PARAMETER (REDMX2 = 80)                                                          ;//      PARAMETER (REDMX2 = 80)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  rtf.fcm                                                                      ;//C  FROM:  rtf.fcm
///                                                                                      ;//C
///  MXRTRS - Maximum number of residues in this data structure                          ;//C  MXRTRS - Maximum number of residues in this data structure
///  MXRTA  - Maximum number of atoms in all residues.                                   ;//C  MXRTA  - Maximum number of atoms in all residues.
///  MXRTB  - Maximum number of bonds in all residues.                                   ;//C  MXRTB  - Maximum number of bonds in all residues.
///  MXRTBL - Maximum number of internal coordinates in all residues.                    ;//C  MXRTBL - Maximum number of internal coordinates in all residues.
///  MXRTHA - Maximum number of hydrogen bond acceptors in all residues.                 ;//C  MXRTHA - Maximum number of hydrogen bond acceptors in all residues.
///  MXRTHD - Maximum number of hydrogen bond donors in all residue.                     ;//C  MXRTHD - Maximum number of hydrogen bond donors in all residue.
///  MXRTI  - Maximum number of impropers in all residue.                                ;//C  MXRTI  - Maximum number of impropers in all residue.
///  MXRTCT - Maximum number of crossterms in all residues        !## CMAP               ;//C  MXRTCT - Maximum number of crossterms in all residues        !## CMAP
///  MXRTP  - Maximum number of torsions (phi's) in all residues.                        ;//C  MXRTP  - Maximum number of torsions (phi's) in all residues.
///  MXRTT  - Maximum number of bond angles (theta's) in all residues.                   ;//C  MXRTT  - Maximum number of bond angles (theta's) in all residues.
///  MXRTX  - Maximum number of non-bonded exclusions in all residues.                   ;//C  MXRTX  - Maximum number of non-bonded exclusions in all residues.
///  NICM   - Number of different types of information stored in the RTF.                ;//C  NICM   - Number of different types of information stored in the RTF.
///                                                                                      ;//C
//      INTEGER MXRTRS, MXRTA, MXRTB, MXRTT, MXRTP, MXRTI, MXRTX,                        ;//      INTEGER MXRTRS, MXRTA, MXRTB, MXRTT, MXRTP, MXRTI, MXRTX,
//     &        MXRTCT,                                           !## CMAP               ;//     &        MXRTCT,                                           !## CMAP
//     &        MXRTHA, MXRTHD, MXRTBL, NICM                                             ;//     &        MXRTHA, MXRTHD, MXRTBL, NICM
//      PARAMETER (MXRTRS = 200, MXRTA = 5000, MXRTB = 5000,                             ;//      PARAMETER (MXRTRS = 200, MXRTA = 5000, MXRTB = 5000,
//     &           MXRTT = 5000, MXRTP = 5000, MXRTI = 2000,                             ;//     &           MXRTT = 5000, MXRTP = 5000, MXRTI = 2000,
//##IF YAMMP                                                                             ;//##IF YAMMP
//     &           MXRTX = 20000, MXRTHA = 300, MXRTHD = 300,                            ;//     &           MXRTX = 20000, MXRTHA = 300, MXRTHD = 300,
//##ELSE                                                                                 ;//##ELSE
//     &           MXRTX = 5000, MXRTHA = 300, MXRTHD = 300,                             ;//     &           MXRTX = 5000, MXRTHA = 300, MXRTHD = 300,
//##ENDIF                                                                                ;//##ENDIF
//##IF CMAP                                                                              ;//##IF CMAP
//     &           MXRTCT = 1000, MXRTBL = 5000, NICM = 11)                              ;//     &           MXRTCT = 1000, MXRTBL = 5000, NICM = 11)
//##ELSE                                                                                 ;//##ELSE
//     &           MXRTBL = 5000, NICM = 10)                                             ;//     &           MXRTBL = 5000, NICM = 10)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  sbound.fcm                                                                   ;//C  FROM:  sbound.fcm
///                                                                                      ;//C
///  NMFTAB - Maximum number of distance lookup points for any table.                    ;//C  NMFTAB - Maximum number of distance lookup points for any table.
///  NMCTAB - Maximum number of boundary tables.                                         ;//C  NMCTAB - Maximum number of boundary tables.
///  NMCATM - Maximum number of atoms for any boundary table.                            ;//C  NMCATM - Maximum number of atoms for any boundary table.
///  NSPLIN - Order of fit (3=cubic spline).                                             ;//C  NSPLIN - Order of fit (3=cubic spline).
///                                                                                      ;//C
//      INTEGER NMFTAB,  NMCTAB,  NMCATM,  NSPLIN                                        ;//      INTEGER NMFTAB,  NMCTAB,  NMCATM,  NSPLIN
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (NMFTAB = 10, NMCTAB = 3, NMCATM = 10, NSPLIN = 3)                     ;//      PARAMETER (NMFTAB = 10, NMCTAB = 3, NMCATM = 10, NSPLIN = 3)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (NMFTAB = 200, NMCTAB = 3, NMCATM = 12000, NSPLIN = 3)                 ;//      PARAMETER (NMFTAB = 200, NMCTAB = 3, NMCATM = 12000, NSPLIN = 3)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  shake.fcm                                                                    ;//C  FROM:  shake.fcm
///                                                                                      ;//C
///  MAXSHK - The maximum number of SHAKE constraints.                                   ;//C  MAXSHK - The maximum number of SHAKE constraints.
///                                                                                      ;//C
//      INTEGER MAXSHK                                                                   ;//      INTEGER MAXSHK
//##IF XSMALL                                                                            ;//##IF XSMALL
//      PARAMETER (MAXSHK = 20)                                                          ;//      PARAMETER (MAXSHK = 20)
//##ELIF REDUCE                                                                          ;//##ELIF REDUCE
//      PARAMETER (MAXSHK = 5120)                                                        ;//      PARAMETER (MAXSHK = 5120)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXSHK = SIZE*3/4)                                                    ;//      PARAMETER (MAXSHK = SIZE*3/4)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  FROM:  string.fcm                                                                   ;//C  FROM:  string.fcm
///                                                                                      ;//C
///  SCRMAX - The maximum string length. Should match MXCMSZ.                            ;//C  SCRMAX - The maximum string length. Should match MXCMSZ.
///                                                                                      ;//C
//      INTEGER SCRMAX                                                                   ;//      INTEGER SCRMAX
//##IF IBM IBMRS CRAY INTEL IBMSP T3D REDUCE                                             ;//##IF IBM IBMRS CRAY INTEL IBMSP T3D REDUCE
//      PARAMETER (SCRMAX = 500)                                                         ;//      PARAMETER (SCRMAX = 500)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (SCRMAX = 5000)                                                        ;//      PARAMETER (SCRMAX = 5000)
//##ENDIF                                                                                ;//##ENDIF
///                                                                                      ;//C
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
//##IF TSM                                                                               ;//##IF TSM
///  FROM:  tsms.fcm                                                                     ;//C  FROM:  tsms.fcm
//      INTEGER MXPIGG                                                                   ;//      INTEGER MXPIGG
///  MXPIGG - The maximum number of "piggyback atom pairs".                              ;//C  MXPIGG - The maximum number of "piggyback atom pairs".
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (MXPIGG=50)                                                            ;//      PARAMETER (MXPIGG=50)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MXPIGG=500)                                                           ;//      PARAMETER (MXPIGG=500)
//##ENDIF                                                                                ;//##ENDIF
//      INTEGER MXCOLO,MXPUMB                                                            ;//      INTEGER MXCOLO,MXPUMB
//      PARAMETER (MXCOLO=20,MXPUMB=20)                                                  ;//      PARAMETER (MXCOLO=20,MXPUMB=20)
///                                                                                      ;//C
//##ENDIF                                                                                ;//##ENDIF
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
//##IF ADUMB                                                                             ;//##IF ADUMB
///  FROM:  umb.fcm                                                                      ;//C  FROM:  umb.fcm
///                                                                                      ;//C
///  MAXUMP - The maximum number of adaptive umbrella potentials.                        ;//C  MAXUMP - The maximum number of adaptive umbrella potentials.
///  MAXNUM - Maximum number of NOE data sets for NOE constraints                        ;//C  MAXNUM - Maximum number of NOE data sets for NOE constraints
///  C.Bartels, 1996/1998                                                                ;//C  C.Bartels, 1996/1998
///                                                                                      ;//C
//      INTEGER MAXUMP, MAXEPA, MAXNUM                                                   ;//      INTEGER MAXUMP, MAXEPA, MAXNUM
//##IF REDUCE                                                                            ;//##IF REDUCE
//      PARAMETER (MAXUMP = 1, MAXNUM = 2)                                               ;//      PARAMETER (MAXUMP = 1, MAXNUM = 2)
//##ELSE                                                                                 ;//##ELSE
//      PARAMETER (MAXUMP = 10, MAXNUM = 4)                                              ;//      PARAMETER (MAXUMP = 10, MAXNUM = 4)
//##ENDIF                                                                                ;//##ENDIF
//##ENDIF                                                                                ;//##ENDIF
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///  Miscellaneous:                                                                      ;//C  Miscellaneous:
///                                                                                      ;//C
///  MAXING - The maximum number of atoms in any electrostatic group.                    ;//C  MAXING - The maximum number of atoms in any electrostatic group.
///                                                                                      ;//C
//      INTEGER MAXING                                                                   ;//      INTEGER MAXING
//      PARAMETER (MAXING=1000)                                                          ;//      PARAMETER (MAXING=1000)
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
//##IF MMFF                                                                              ;//##IF MMFF
///                                                                                      ;//C
///       ADDITIONAL PARAMETERS FOR RING_FIND ROUTINES                                   ;//C       ADDITIONAL PARAMETERS FOR RING_FIND ROUTINES
///       JAY BANKS, 24 SEPT 1993.                                                       ;//C       JAY BANKS, 24 SEPT 1993.
///                                                                                      ;//C
///       MAX_RINGSIZE, MAX_EACH_SIZE, and MAXPATHS are array dimensions.                ;//C       MAX_RINGSIZE, MAX_EACH_SIZE, and MAXPATHS are array dimensions.
//      integer MAX_RINGSIZE, MAX_EACH_SIZE                                              ;//      integer MAX_RINGSIZE, MAX_EACH_SIZE
//      parameter (MAX_RINGSIZE = 20, MAX_EACH_SIZE = 1000)                              ;//      parameter (MAX_RINGSIZE = 20, MAX_EACH_SIZE = 1000)
//      integer MAXPATHS                                                                 ;//      integer MAXPATHS
//      parameter (MAXPATHS = 8000)                                                      ;//      parameter (MAXPATHS = 8000)
///       MAX_TO_SEARCH is passed as an argument to ring_find, telling it the            ;//C       MAX_TO_SEARCH is passed as an argument to ring_find, telling it the
///       largest size ring it should look for.                                          ;//C       largest size ring it should look for.
//      integer MAX_TO_SEARCH                                                            ;//      integer MAX_TO_SEARCH
//      parameter (MAX_TO_SEARCH = 6)                                                    ;//      parameter (MAX_TO_SEARCH = 6)
//##ENDIF                                                                                ;//##ENDIF
///-----------------------------------------------------------------------               ;//C-----------------------------------------------------------------------
///                                                                                      ;//C
}
}
}
