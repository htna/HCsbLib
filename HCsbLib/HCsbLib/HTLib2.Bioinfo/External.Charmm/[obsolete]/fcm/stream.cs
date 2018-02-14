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
///HARMM Element source/fcm/stream.fcm 1.1
///
///     This is the STREAM data block.
///     It contains information abount the current runstream.
///
///     MXSTRM - Maximum number of active stream files.
///     POUTU  - Default output unit number.
///     NSTRM  - Number of active input streams.
///     ISTRM = JSTRM(NSTRM) - Current input unit number
///     JSTRM(*) - stack of input streams numbers.
///     OUTU   - Unit number for all standard CHARMM output.
///     PRNLEV - Print level control for all writing to OUTU
///     IOLEV  - -1 to 1  -1=write no files.  1= write all files.
///     WRNLEV - -5 TO 10  0=SEVERE ONLY, 10=LIST ALL WARNINGS
///     LOWER  - if .true. all files with names not in double quotes
///              will be opened in lower case for write. For read
///              UPPER case will be tried first and if not succesful
///              file name will be converted to lower case.
///     QLONGL - Use long lines in the output where appropriate.
///              (Otherwise, restrict output lines to 80 characters)
///     IECHO -  Unit number for output from ECHO command
///yw++
///     qoldfmt  enforce old-format i/o, .false. unless set by IOFO NOEX
///     qnewfmt  enforce new-format i/o, .false. unless set by IOFO EXTE
///     qextfmt  new-format i/o if natom>100000 or character*8 is used
///     idleng   character ID length (4 for old format and 8 for extended)
//
//      logical qoldfmt,qnewfmt,qextfmt
//      integer idleng
///yw--
//      LOGICAL LOWER,QLONGL
//      INTEGER MXSTRM,POUTU
//      PARAMETER (MXSTRM=20,POUTU=6)
//      INTEGER   NSTRM,ISTRM,JSTRM,OUTU,PRNLEV,WRNLEV,IOLEV,IECHO
///
//      common /iofmt/ qoldfmt,qnewfmt,qextfmt    ! yw
//      common /id48/  idleng
//      COMMON /CASE/   LOWER, QLONGL
//      COMMON /STREAM/ NSTRM,ISTRM,JSTRM(MXSTRM),OUTU,PRNLEV,WRNLEV,
//     &       IOLEV,IECHO
///
//##IF T3ETRAJ
/// mfc   added T3ETRAJ flag set and unset in readcv()
/// ---  flag for rdtitl() to get the integer ntitl as int*8 for t3e traj's
///         when not read on t3e (on sgi for instance).
//      logical t3etraj
//      common /t3etrajfile/t3etraj
//##ENDIF
//
//##IF SAVEFCM
//      save /iofmt/                              ! yw
//      SAVE /CASE/,/STREAM/
//      SAVE /t3etrajfile/   !##T3ETRAJ
//##ENDIF
///
    int  PRNLEV { get { HDebug.Assert(false); return 0; } }     /// Print level control for all writing to OUTU
    int  WRNLEV { get { return 10; } }    /// -5 TO 10  0=SEVERE ONLY, 10=LIST ALL WARNINGS
    bool QLONGL { get { HDebug.Assert(false); return false; } } /// Use long lines in the output where appropriate.
                                                                /// (Otherwise, restrict output lines to 80 characters)
}
}
}
