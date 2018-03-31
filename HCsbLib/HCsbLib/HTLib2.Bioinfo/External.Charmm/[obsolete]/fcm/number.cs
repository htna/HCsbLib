using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CS0414

namespace HTLib2.Bioinfo
{
public partial class Charmm
{
public partial class Src
{
//CHARMM Element source/fcm/number.fcm 1.1
///
/// This file contains floating point numbers.
///
/// positive numbers
//      REAL*8     ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX,
//     &           SEVEN, EIGHT, NINE, TEN, ELEVEN, TWELVE, THIRTN,
//     &           FIFTN, NINETN, TWENTY, THIRTY
//##IF SINGLE
//      PARAMETER (ZERO   =  0.E0, ONE    =  1.E0, TWO    =  2.E0,
//     &           THREE  =  3.E0, FOUR   =  4.E0, FIVE   =  5.E0,
//     &           SIX    =  6.E0, SEVEN  =  7.E0, EIGHT  =  8.E0,
//     &           NINE   =  9.E0, TEN    = 10.E0, ELEVEN = 11.E0,
//     &           TWELVE = 12.E0, THIRTN = 13.E0, FIFTN  = 15.E0,
//     &           NINETN = 19.E0, TWENTY = 20.E0, THIRTY = 30.E0)
//##ELSE
//      PARAMETER (ZERO   =  0.D0, ONE    =  1.D0, TWO    =  2.D0,
//     &           THREE  =  3.D0, FOUR   =  4.D0, FIVE   =  5.D0,
//     &           SIX    =  6.D0, SEVEN  =  7.D0, EIGHT  =  8.D0,
//     &           NINE   =  9.D0, TEN    = 10.D0, ELEVEN = 11.D0,
//     &           TWELVE = 12.D0, THIRTN = 13.D0, FIFTN  = 15.D0,
//     &           NINETN = 19.D0, TWENTY = 20.D0, THIRTY = 30.D0)
//##ENDIF
    static readonly double ZERO   =  0.0, ONE    =  1.0, TWO    =  2.0;
    static readonly double THREE  =  3.0, FOUR   =  4.0, FIVE   =  5.0;
    static readonly double SIX    =  6.0, SEVEN  =  7.0, EIGHT  =  8.0;
    static readonly double NINE   =  9.0, TEN    = 10.0, ELEVEN = 11.0;
    static readonly double TWELVE = 12.0, THIRTN = 13.0, FIFTN  = 15.0;
    static readonly double NINETN = 19.0, TWENTY = 20.0, THIRTY = 30.0;

///
//      REAL*8     FIFTY, SIXTY, SVNTY2, EIGHTY, NINETY, HUNDRD,
//     &           ONE2TY, ONE8TY, THRHUN, THR6TY, NINE99, FIFHUN, THOSND,
//     &           FTHSND,MEGA
//##IF SINGLE
//      PARAMETER (FIFTY  = 50.E0,  SIXTY  =  60.E0,  SVNTY2 =   72.E0,
//     &           EIGHTY = 80.E0,  NINETY =  90.E0,  HUNDRD =  100.E0,
//     &           ONE2TY = 120.E0, ONE8TY = 180.E0,  THRHUN =  300.E0,
//     &           THR6TY=360.E0,   NINE99 = 999.E0,  FIFHUN = 1500.E0,
//     &           THOSND = 1000.E0,FTHSND = 5000.E0, MEGA   =   1.0E6)
//##ELSE
//      PARAMETER (FIFTY  = 50.D0,  SIXTY  =  60.D0,  SVNTY2 =   72.D0,
//     &           EIGHTY = 80.D0,  NINETY =  90.D0,  HUNDRD =  100.D0,
//     &           ONE2TY = 120.D0, ONE8TY = 180.D0,  THRHUN =  300.D0,
//     &           THR6TY=360.D0,   NINE99 = 999.D0,  FIFHUN = 1500.D0,
//     &           THOSND = 1000.D0,FTHSND = 5000.D0, MEGA   =   1.0D6)
//##ENDIF
    static readonly double FIFTY  =   50.0, SIXTY  =   60.0, SVNTY2 =   72.0;
    static readonly double EIGHTY =   80.0, NINETY =   90.0, HUNDRD =  100.0;
    static readonly double ONE2TY =  120.0, ONE8TY =  180.0, THRHUN =  300.0;
    static readonly double THR6TY =  360.0, NINE99 =  999.0, FIFHUN = 1500.0;
    static readonly double THOSND = 1000.0, FTHSND = 5000.0, MEGA   =    1.0E6;

///
/// negative numbers
//      REAL*8     MINONE, MINTWO, MINSIX
//##IF SINGLE
//      PARAMETER (MINONE = -1.E0,  MINTWO = -2.E0,  MINSIX = -6.E0)
//##ELSE
//      PARAMETER (MINONE = -1.D0,  MINTWO = -2.D0,  MINSIX = -6.D0)
//##ENDIF
    static readonly double MINONE = -1.0,  MINTWO = -2.0,  MINSIX = -6.0;

///
/// common fractions
//      REAL*8     TENM20,TENM14,TENM8,TENM5,PT0001,PT0005,PT001,PT005,
//     &           PT01, PT02, PT05, PTONE, PT125, PT25, SIXTH, THIRD,
//     &           PTFOUR, PTSIX, HALF, PT75, PT9999, ONEPT5, TWOPT4
//##IF SINGLE
//      PARAMETER (TENM20 = 1.0E-20,  TENM14 = 1.0E-14,  TENM8  = 1.0E-8,
//     &           TENM5  = 1.0E-5,   PT0001 = 1.0E-4, PT0005 = 5.0E-4,
//     &           PT001  = 1.0E-3,   PT005  = 5.0E-3, PT01   = 0.01E0,
//     &           PT02   = 0.02E0,   PT05   = 0.05E0, PTONE  = 0.1E0,
//     &           PT125  = 0.125E0,  SIXTH  = ONE/SIX,PT25   = 0.25E0,
//     &           THIRD  = ONE/THREE,PTFOUR = 0.4E0,  HALF   = 0.5E0,
//     &           PTSIX  = 0.6E0,    PT75   = 0.75E0, PT9999 = 0.9999E0,
//     &           ONEPT5 = 1.5E0,    TWOPT4 = 2.4E0)
//##ELSE
//      PARAMETER (TENM20 = 1.0D-20,  TENM14 = 1.0D-14,  TENM8  = 1.0D-8,
//     &           TENM5  = 1.0D-5,   PT0001 = 1.0D-4, PT0005 = 5.0D-4,
//     &           PT001  = 1.0D-3,   PT005  = 5.0D-3, PT01   = 0.01D0,
//     &           PT02   = 0.02D0,   PT05   = 0.05D0, PTONE  = 0.1D0,
//     &           PT125  = 0.125D0,  SIXTH  = ONE/SIX,PT25   = 0.25D0,
//     &           THIRD  = ONE/THREE,PTFOUR = 0.4D0,  HALF   = 0.5D0,
//     &           PTSIX  = 0.6D0,    PT75   = 0.75D0, PT9999 = 0.9999D0,
//     &           ONEPT5 = 1.5D0,    TWOPT4 = 2.4D0)
//##ENDIF
    static readonly double TENM20 = 1.0E-20   ,TENM14 = 1.0E-14 ,TENM8  = 1.0E-8;
    static readonly double TENM5  = 1.0E-5    ,PT0001 = 1.0E-4  ,PT0005 = 5.0E-4;
    static readonly double PT001  = 1.0E-3    ,PT005  = 5.0E-3  ,PT01   = 0.01  ;
    static readonly double PT02   = 0.02      ,PT05   = 0.05    ,PTONE  = 0.1   ;
    static readonly double PT125  = 0.125     ,SIXTH  = ONE/SIX ,PT25   = 0.25  ;
    static readonly double THIRD  = ONE/THREE ,PTFOUR = 0.4     ,HALF   = 0.5   ;
    static readonly double PTSIX  = 0.6       ,PT75   = 0.75    ,PT9999 = 0.9999;
    static readonly double ONEPT5 = 1.5       ,TWOPT4 = 2.4                     ;

///
/// others
//      REAL*8 ANUM,FMARK
//      REAL*8 RSMALL,RBIG
//##IF SINGLE
//      PARAMETER (ANUM=9999.0E0, FMARK=-999.0E0)
//      PARAMETER (RSMALL=1.0E-10,RBIG=1.0E20)
//##ELSE
//      PARAMETER (ANUM=9999.0D0, FMARK=-999.0D0)
//      PARAMETER (RSMALL=1.0D-10,RBIG=1.0D20)
//##ENDIF
///
/// Machine constants (these are very machine dependent).
///
/// RPRECI should be the smallest number you can add to 1.0 and get a number
/// that is different than 1.0.  Actually: the following code must pass for
/// for all real numbers A (where no overflow or underflow conditions exist).
/// 
///         B = A * RPRECI
///         C = A + B
///         IF(C.EQ.A) STOP 'precision variable is too small'
/// 
/// The RBIGST value should be the smaller of:
/// 
///         - The largest real value               
///         - The reciprocal of the smallest real value 
///
/// If there is doubt, be conservative.
/// 
///!!!!! NOTE:  Some of the values have not been checked....
///!!!!! Please fix these values - BRB .......
///
//      REAL*8 RPRECI,RBIGST
    static readonly double RPRECI = number.RPRECI;
    static class number
    {
        static number()
        {
            double A = 1.0;
            double B = A * RPRECI;
            double C = A + B;
            HDebug.Assert(C != A);
        }
        public static readonly double RPRECI = double.Epsilon;
    }
///
//##IF VAX DEC
//      PARAMETER (RPRECI = 2.22045D-16, RBIGST = 1.7D+38)
//##ELIF IBM
///  for IBM 3090;
//      PARAMETER (RPRECI = 2.2D-15, RBIGST = 5.0D+75)
//##ELIF CRAY
//##IF POSIX
//      PARAMETER (RPRECI = 7.1054273576E-15, RBIGST = 2.72687033904E+307)
//##ELSE
//      PARAMETER (RPRECI = 7.1054273576E-15, RBIGST = 2.72687033904E2465)
//##ENDIF
//##ELIF ALPHA T3D T3E
///!!!!!!!!!
/// I don't know what these values should be, so if you know, fix it!!!
///    BRB 7/3/95
//      PARAMETER (RPRECI = 2.22045D-16, RBIGST = 4.49423D+307)
///!!!!!!!!
//##ELSE
/// Assume IEEE floating point standards (ignoring unnormalized numbers).
///       All exceptions should appear above (before the ##ELSE)!
//##IF SINGLE
//      PARAMETER (RPRECI = 1.192E-7, RBIGST = 1.7E+38)
//##ELSE
//      PARAMETER (RPRECI = 2.22045D-16, RBIGST = 4.49423D+307)
//##ENDIF
//##ENDIF
///
}
}
}
