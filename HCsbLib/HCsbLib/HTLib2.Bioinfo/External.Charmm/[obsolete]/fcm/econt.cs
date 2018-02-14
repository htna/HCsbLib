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
//CHARMM Element source/fcm/econt.fcm 1.1
///
///  econt.fcm -  Energy contributions common block.
///
///  QECONT   - Flag to determine if partition analysis is on.
///  ECONT(*) - Partial energy contribution for each atom
///
///  QATERM   - flag to indicate energy term selection is used
///  ANSLCT(*)- Atom Selection for printing energy terms
///  
///  QAONLY   - Flag indicating that only energy terms where ALL
///                   atoms are selected be listed
///  QANBND   - Flag to print nonbond energy term table
///  QAUNIT   - Output unit number for printing data
///
///
//      LOGICAL QECONT,QAONLY,QATERM,QANBND
//      INTEGER ANSLCT,QAUNIT
//      REAL*8 ECONT
///
//      COMMON /ECONTL/QECONT,QAONLY,QATERM,QANBND
//      COMMON /ECONTI/ANSLCT(MAXAIM),QAUNIT
//      COMMON /ECONTR/ECONT(MAXAIM)
///
//##IF SAVEFCM
//      SAVE /ECONTL/
//      SAVE /ECONTI/
//      SAVE /ECONTR/
//##ENDIF
///
    bool     QECONT,QATERM,QANBND;
    bool     QAONLY = false;
    int[]    ANSLCT = new int[MAXAIM];
    int      QAUNIT;
    double[] ECONT = new double[MAXAIM];
}
}
}
