using System;

namespace HTLib2.Bioinfo
{
public partial class Charmm
{
public partial class Src
{
/// http://www.charmm.org/documentation/c34b1/mbond.html
/// In multi-body dynamics, aggregates of atoms are gathered into
/// "bodies".  For a dynamics run, the system comprises one or more bodies
/// and zero or more atoms which are not part of any body.  By gathering
/// the atoms in this way, the total number of variables in the system is
/// considerably reduced which is expected to significantly improve the
/// computational performance.  Furthermore because such a simulation aims
/// to reproduce the characteristic (i.e. low-frequency) motion of the
/// system, relatively long time steps are possible.  The final advantage
/// of this scheme is that bond-lengths may be explicitly constrained
/// (between bodies and in the atomistic regions) in a computationally
/// efficient manner.
///////////////////////////////////////////////////////////////////////////////////////////////////
    public void EBOND(double EB,int[] IB,int[] JB,int[] ICB,int NBOND,double[] CBC,double[] CBB,    //CHARMM Element source/energy/eintern.src 1.1
                      double[] DX,double[] DY,double[] DZ,double[] X,double[] Y,double[] Z,         //      SUBROUTINE EBOND(EB,IB,JB,ICB,NBOND,CBC,CBB,DX,DY,DZ,X,Y,Z,
                      bool QECONTX,double[] ECONTX,int ICONBH,int[] ISKB,double[] DD1, int[] IUPT,  //     $     QECONTX,ECONTX,ICONBH,ISKB,DD1,IUPT,QSECD,KBEXPN,LUPPER)
                      bool QSECD, int KBEXPN, bool LUPPER)                                          
    {                                                                                               
///-----------------------------------------------------------------------                         ;//C-----------------------------------------------------------------------
///     CALCULATES BOND ENERGIES AND FORCES.                                                       ;//C     CALCULATES BOND ENERGIES AND FORCES.
///     IT DOES NOT CALCULATE BOND ENERGY AND FORCES FOR ALL BONDS                                 ;//C     IT DOES NOT CALCULATE BOND ENERGY AND FORCES FOR ALL BONDS
///     WITH A NONZERO VALUE IN ISKB ONLY WHEN ICONBH .NE. 0                                       ;//C     WITH A NONZERO VALUE IN ISKB ONLY WHEN ICONBH .NE. 0
///                                                                                                ;//C
///     By Bernard R. Brooks  (mostly)  1981-1983                                                  ;//C     By Bernard R. Brooks  (mostly)  1981-1983
///                                                                                                ;//C
///     BLOCK incorporated (including seccond derivatives)                                         ;//C     BLOCK incorporated (including seccond derivatives)
///     By Youngdo Won, 12/17/90                                                                   ;//C     By Youngdo Won, 12/17/90
///     SPASIBA Force Field added by P. Lagant and R. Stote (11/01)                                ;//C     SPASIBA Force Field added by P. Lagant and R. Stote (11/01)
///       as it is non-functional, removed for c31b1 release, July 2004 YW                         ;//C       as it is non-functional, removed for c31b1 release, July 2004 YW
///                                                                                                ;//C
//##INCLUDE '~/charmm_fcm/impnon.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/impnon.fcm'
//##INCLUDE '~/charmm_fcm/dimens.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/dimens.fcm'
//##INCLUDE '~/charmm_fcm/exfunc.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/exfunc.fcm'
//##INCLUDE '~/charmm_fcm/number.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/number.fcm'
//##INCLUDE '~/charmm_fcm/fourd.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/fourd.fcm'
///                                                                                                ;//C
//##IF BLOCK DIMB MBOND                                                                            ;//##IF BLOCK DIMB MBOND
//##INCLUDE '~/charmm_fcm/heap.fcm'                                                                ;//##INCLUDE '~/charmm_fcm/heap.fcm'
//##ENDIF  ! BLOCK DIMB MBOND                                                                      ;//##ENDIF  ! BLOCK DIMB MBOND
//##INCLUDE '~/charmm_fcm/block.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/block.fcm'
//##INCLUDE '~/charmm_fcm/lambda.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/lambda.fcm'
//##INCLUDE '~/charmm_fcm/dimb.fcm'                                                                ;//##INCLUDE '~/charmm_fcm/dimb.fcm'
//##INCLUDE '~/charmm_fcm/econt.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/econt.fcm'
//##INCLUDE '~/charmm_fcm/mbond.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/mbond.fcm'
//##INCLUDE '~/charmm_fcm/stream.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/stream.fcm'
//##IF PARALLEL                                                                                    ;//##IF PARALLEL
//##INCLUDE '~/charmm_fcm/parallel.fcm'                                                            ;//##INCLUDE '~/charmm_fcm/parallel.fcm'
//      LOGICAL NOPARS                                                                             ;//      LOGICAL NOPARS
//##ENDIF                                                                                          ;//##ENDIF
///                                                                                                ;//C
//      REAL*8 EB                                                                                  ;//      REAL*8 EB
//      INTEGER IB(*),JB(*),ICB(*)                                                                 ;//      INTEGER IB(*),JB(*),ICB(*)
//      INTEGER NBOND,KBEXPN                                                                       ;//      INTEGER NBOND,KBEXPN
//      REAL*8 CBC(*),CBB(*)                                                                       ;//      REAL*8 CBC(*),CBB(*)
//      REAL*8 DX(*),DY(*),DZ(*),X(*),Y(*),Z(*)                                                    ;//      REAL*8 DX(*),DY(*),DZ(*),X(*),Y(*),Z(*)
//      LOGICAL QECONTX                                                                            ;//      LOGICAL QECONTX
//      REAL*8 ECONTX(*)                                                                           ;//      REAL*8 ECONTX(*)
//      INTEGER ICONBH                                                                             ;//      INTEGER ICONBH
//      INTEGER ISKB(*)                                                                            ;//      INTEGER ISKB(*)
//      REAL*8 DD1(*)                                                                              ;//      REAL*8 DD1(*)
//      INTEGER IUPT(*)                                                                            ;//      INTEGER IUPT(*)
//      LOGICAL QSECD,LUPPER                                                                       ;//      LOGICAL QSECD,LUPPER
///                                                                                                ;//C
        double RX,RY,RZ,S2,S,R,DB,DF,E,DXI,DYI,DZI,DDF,A                                           ;//      REAL*8 RX,RY,RZ,S2,S,R,DB,DF,E,DXI,DYI,DZI,DDF,A
        int     I,MM,J,IC,IADD,JJ,II,KK                                                            ;//      INTEGER I,MM,J,IC,IADD,JJ,II,KK
        bool    NOCONS    //,QAFIRST                                                               ;//      LOGICAL NOCONS,QAFIRST
//      string      SIDDNI,RIDDNI,RESDNI,ACDNI,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ                          ;//      CHARACTER*8 SIDDNI,RIDDNI,RESDNI,ACDNI,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ
///                                                                                                ;//C
                                                                                                   ;//##IF FOURD (4ddecl)
                                                                                                   ;//C     4-D variable:
                                                                                                   ;//      REAL*8 RFDIM,DFDIMI,DFDI 
                                                                                                   ;//##ENDIF (4ddecl)
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//      INTEGER IBL, JBL, KDOC
                                                                                                   ;//      REAL*8 COEF, DOCFI, DOCFJ
                                                                                                   ;//##IF LDM
                                                                                                   ;//      REAL*8 UNSCALE, FALPHA
                                                                                                   ;//##ENDIF ! LDM
                                                                                                   ;//##ENDIF ! BLOCK
///                                                                                                ;//C
        EB=ZERO                                                                                    ;//      EB=ZERO
        E=ZERO                                                                                     ;//      E=ZERO
        NOCONS=(ICONBH >  0)                                                                       ;//      NOCONS=(ICONBH.GT.0)
        if (NBOND == 0) return                                                                     ;//      IF (NBOND.EQ.0) RETURN
///                                                                                                ;//C
//      QAFIRST= true                                                                              ;//      QAFIRST=.TRUE.
///                                                                                                ;//C
                                                                                                    //##IF PARALLEL (parabond)
                                                                                                    //##IF PARAFULL (parfbond)
                                                                                                    //##IF PARASCAL (parastest)
                                                                                                    //##ERROR 'Illegal parallel compile option'
                                                                                                    //##ENDIF (parastest)
                                                                                                   ;//      DO 10 MM=MYNODP,NBOND,NUMNOD
                                                                                                    //##ELIF PARASCAL (parfbond)
                                                                                                   ;//      NOPARS=(ICONBH.GE.0)
                                                                                                   ;//      DO 10 MM=1,NBOND
                                                                                                   ;//        IF(NOPARS) THEN
                                                                                                   ;//          IF(JPMAT(IPBLOCK(IB(MM)),IPBLOCK(JB(MM))).NE.MYNOD) GOTO 10
                                                                                                   ;//        ENDIF
                                                                                                    //##ELSE (parfbond)
                                                                                                    //##ERROR 'Illegal parallel compile option'
                                                                                                    //##ENDIF (parfbond)
                                                                                                    //##ELSE (parabond)
        for(MM=1; MM<=NBOND; MM++) {                                                               ;//      DO 10 MM=1,NBOND
                                                                                                   //##ENDIF (parabond)
///                                                                                                ;//C
          I=IB[MM]                                                                                 ;//        I=IB(MM)
          if(NOCONS) {                                                                             ;//        IF(NOCONS) THEN
            if(ISKB[MM] != 0) continue                                                             ;//          IF(ISKB(MM).NE.0) GOTO 10
          }                                                                                        ;//        ENDIF
          J=JB[MM]                                                                                 ;//        J=JB(MM)
          IC=ICB[MM]                                                                               ;//        IC=ICB(MM)
          if(IC == 0) continue                                                                     ;//        IF(IC.EQ.0) GOTO 10
          if(CBC[IC] == ZERO) continue                                                             ;//        IF(CBC(IC).EQ.ZERO) GOTO 10
          RX=X[I]-X[J]                                                                             ;//        RX=X(I)-X(J)
          RY=Y[I]-Y[J]                                                                             ;//        RY=Y(I)-Y(J)
          RZ=Z[I]-Z[J]                                                                             ;//        RZ=Z(I)-Z(J)
          S2=RX*RX + RY*RY + RZ*RZ                                                                 ;//        S2=RX*RX + RY*RY + RZ*RZ
                                                                                                    //##IF FOURD (4dadd)
                                                                                                   ;//        IF (DIM4ON(1).EQ.1) THEN
                                                                                                   ;//          RFDIM=FDIM(I)-FDIM(J)
                                                                                                   ;//          S2=S2 + RFDIM*RFDIM
                                                                                                   ;//        ENDIF
                                                                                                    //##ENDIF (4dadd)
          S=SQRT(S2)                                                                               ;//        S=SQRT(S2)
          if(LUPPER  &&   (S <  CBB[IC])) continue                                                 ;//        IF(LUPPER .AND. (S.LT.CBB(IC))) GOTO 10
          if(CBB[IC] == ZERO) {                                                                    ;//        IF(CBB(IC).EQ.ZERO) THEN
            DB=S2                                                                                  ;//          DB=S2
            if(KBEXPN == 2) {                                                                      ;//          IF(KBEXPN.EQ.2) THEN
              R=TWO                                                                                ;//            R=TWO
              DF=CBC[IC]                                                                           ;//            DF=CBC(IC)
              DDF=ZERO                                                                             ;//            DDF=ZERO
            } else {                                                                               ;//          ELSE
              R=KBEXPN                                                                             ;//            R=KBEXPN
              DF=CBC[IC]*POW(S,(KBEXPN-2))                                                         ;//            DF=CBC(IC)*S**(KBEXPN-2)
              DDF=(KBEXPN-2)*CBC[IC]*POW(S,(KBEXPN-4))*R                                           ;//            DDF=(KBEXPN-2)*CBC(IC)*S**(KBEXPN-4)*R
            }                                                                                      ;//          ENDIF
          } else {                                                                                 ;//        ELSE
            if(S == ZERO) continue                                                                 ;//          IF(S.EQ.ZERO) GOTO 10
            DB=S-CBB[IC]                                                                           ;//          DB=S-CBB(IC)
            if(KBEXPN == 2) {                                                                      ;//          IF(KBEXPN.EQ.2) THEN
              R=TWO/S                                                                              ;//            R=TWO/S
                                                                                                    //##IF MBOND ! OMB
                                                                                                   ;//            IF (qMBSCALE .and. QMBEX2(I,J,HEAP(pMBAt(1)))) THEN
                                                                                                   ;//              DF=CBC(IC)*DB*MBSCALE
                                                                                                   ;//            ELSE
                                                                                                   //##ENDIF ! MBOND
                DF=CBC[IC]*DB                                                                      ;//              DF=CBC(IC)*DB
                                                                                                    //##IF MBOND
                                                                                                   ;//            ENDIF
                                                                                                    //##ENDIF ! MBOND
              DDF=TWO*CBC[IC]                                                                      ;//            DDF=TWO*CBC(IC)
              DDF=(DDF-R*DF)/S2                                                                    ;//            DDF=(DDF-R*DF)/S2
            } else {                                                                               ;//          ELSE
              R=KBEXPN/S                                                                           ;//            R=KBEXPN/S
              DF=CBC[IC]*POW(DB,(KBEXPN-1))                                                        ;//            DF=CBC(IC)*DB**(KBEXPN-1)
              DDF=KBEXPN*(KBEXPN-1)*CBC[IC]*POW(DB,(KBEXPN-2))                                     ;//            DDF=KBEXPN*(KBEXPN-1)*CBC(IC)*DB**(KBEXPN-2)
              DDF=(DDF-R*DF)/S2                                                                    ;//            DDF=(DDF-R*DF)/S2
            }                                                                                      ;//          ENDIF
          }                                                                                        ;//        ENDIF
///                                                                                                ;//C
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        IF (QBLOCK) THEN
                                                                                                   ;//          IBL=I4VAL(HEAP(IBLCKP),I)
                                                                                                   ;//          JBL=I4VAL(HEAP(IBLCKP),J)
                                                                                                   ;//##IF DOCK
                                                                                                   ;//c         get asymmetric matrix coefficient
                                                                                                   ;//          DOCFI = 1.0
                                                                                                   ;//          DOCFJ = 1.0
                                                                                                   ;//          IF(QDOCK) THEN
                                                                                                   ;//              KDOC  = (IBL - 1)*NBLOCK + JBL
                                                                                                   ;//              DOCFI = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (JBL - 1)*NBLOCK + IBL
                                                                                                   ;//              DOCFJ = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! DOCK
                                                                                                   ;//          IF (JBL .LT. IBL) THEN
                                                                                                   ;//            KK=JBL
                                                                                                   ;//            JBL=IBL
                                                                                                   ;//            IBL=KK
                                                                                                   ;//          ENDIF
                                                                                                   ;//          KK=IBL+JBL*(JBL-1)/2
                                                                                                   ;//          COEF=GTRR8(HEAP(BLCOEB),KK)
                                                                                                   ;//##IF BANBA
                                                                                                   ;//          IF (QPRNTV .AND. .NOT. QNOBO) THEN
                                                                                                   ;//            IF (IBL.EQ.1 .OR. JBL.EQ.1 .OR. IBL.EQ.JBL) THEN
                                                                                                   ;//              CALL ASUMR8(DF*DB,HEAP(VBBOND),JBL)
                                                                                                   ;//            ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! BANBA                    
                                                                                                   ;//##IF LDM  (ldm_1)
                                                                                                   ;//          IF(QLDM) THEN                                    !##.not.LMC
                                                                                                   ;//          IF(QLDM.or.QLMC) THEN                            !##LMC
                                                                                                   ;//C           first row or diagonal elements exclude (1,1).
                                                                                                   ;//            UNSCALE = 0.0
                                                                                                   ;//##IF LDMGEN
                                                                                                   ;//            IF((IBL.EQ.1.AND.JBL.GE.LSTRT) .or.
                                                                                                   ;//     &         (IBL.GE.LSTART.AND.IBL.EQ.JBL)) UNSCALE = DF
                                                                                                   ;//##ELSE
                                                                                                   ;//            IF(IBL.NE.1.AND.IBL.EQ.JBL) THEN 
                                                                                                   ;//               UNSCALE = DF
                                                                                                   ;//            ELSE IF(IBL.EQ.1.AND.JBL.GE.2) THEN
                                                                                                   ;//               UNSCALE = DF
                                                                                                   ;//            ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##IF BANBA
                                                                                                   ;//         IF(QNOBO) COEF = 1.0
                                                                                                   ;//##ELSE
                                                                                                   ;//         IF(QLDM .AND. QNOBO) COEF = 1.0
                                                                                                   ;//##ENDIF
                                                                                                   ;//##IF LRST
                                                                                                   ;//          IF(RSTP.AND. .NOT. QNOBO)THEN
                                                                                                   ;//           IF( (IBL.EQ.1 .AND. JBL.GE.LSTRT).or.
                                                                                                   ;//     &         (IBL.GE.LSTRT .AND. IBL.EQ.JBL)) THEN
                                                                                                   ;//             CALL ASUMR8(DF*R*RX,HEAP(ENVDX),I)
                                                                                                   ;//             CALL ASUMR8(DF*R*RY,HEAP(ENVDY),I)
                                                                                                   ;//             CALL ASUMR8(DF*R*RZ,HEAP(ENVDZ),I)
                                                                                                   ;//             CALL ASUMR8(-DF*R*RX,HEAP(ENVDX),J)
                                                                                                   ;//             CALL ASUMR8(-DF*R*RY,HEAP(ENVDY),J)
                                                                                                   ;//             CALL ASUMR8(-DF*R*RZ,HEAP(ENVDZ),J)
                                                                                                   ;//           ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! LRST
                                                                                                   ;//##ENDIF  (ldm_1)
                                                                                                   ;//
                                                                                                   ;//          DF=DF*COEF
                                                                                                   ;//          IF (QSECD) DDF=DDF*COEF
                                                                                                   ;//        ENDIF
                                                                                                   ;//##IF LDM
                                                                                                   ;//         IF( QLDM                                   !##.not.LMC
                                                                                                   ;//         IF((QLDM.or.QLMC)                         !##LMC
                                                                                                   ;//     $       .AND. .NOT. QNOBO) then
                                                                                                   ;//##IF LDMGEN
                                                                                                   ;//           IF( (IBL.EQ.1 .AND. JBL.GE.LSTRT).or.
                                                                                                   ;//     &         (IBL.GE.LSTRT .AND. IBL.EQ.JBL)) THEN
                                                                                                   ;//##ELSE
                                                                                                   ;//           IF((IBL.EQ.JBL).OR.(IBL.EQ.1.AND.JBL.GE.2)) THEN
                                                                                                   ;//##ENDIF
                                                                                                   ;//              FALPHA = UNSCALE*DB
                                                                                                   ;//              LAGMUL = LAGMUL + FALPHA
                                                                                                   ;//              CALL ASUMR8(FALPHA,HEAP(BIFLAM),JBL)
                                                                                                   ;//##IF LRST
                                                                                                   ;//              IF (NRST.EQ.2)THEN
                                                                                                   ;//                CALL ASUMR8(FALPHA,HEAP(BFRST),JBL)
                                                                                                   ;//              ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//           ENDIF
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        IF(QDOCK) THEN
                                                                                                   ;//C         Factor 0.5 to make sure no double counting
                                                                                                   ;//          E=DF*DB*0.5*(DOCFI + DOCFJ)
                                                                                                   ;//        ELSE
                                                                                                   ;//##ENDIF 
                                                                                                   ;//          E=DF*DB
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//##ELSE  ! BLOCK
          E=DF*DB                                                                                  ;//        E=DF*DB
                                                                                                   ;//##ENDIF ! BLOCK
          EB=EB+E                                                                                  ;//        EB=EB+E
///                                                                                                ;//C
          if(QATERM) {                                                                             ;//        IF(QATERM) THEN
            KK=ANSLCT[I]+ANSLCT[J]                                                                 ;//          KK=ANSLCT(I)+ANSLCT(J)
            if(KK == 2  ||  (KK >= 1  &&      ! QAONLY)) {                                         ;//          IF(KK.EQ.2 .OR. (KK.GE.1 .AND. .NOT.QAONLY)) THEN
//            if(QAUNIT <  0) {                                                                    ;//            IF(QAUNIT.LT.0) THEN
//              II=OUTU                                                                            ;//              II=OUTU
//            } else {                                                                             ;//            ELSE
//              II=QAUNIT                                                                          ;//              II=QAUNIT
//            }                                                                                    ;//            ENDIF
///                                                                                                ;//C
//            if(PRNLEV >= 5) {                                                                    ;//            IF(PRNLEV.GE.5) THEN
//              if(QAFIRST) {                                                                      ;//              IF(QAFIRST) THEN
//               if(QLONGL) {                                                                      ;//               IF(QLONGL) THEN
//                  WRITE(II,243)                                                                  ;//                  WRITE(II,243)
//               } else {                                                                          ;//               ELSE
//                  WRITE(II,244)                                                                  ;//                  WRITE(II,244)
//               }                                                                                 ;//               ENDIF
// 243           FORMAT('ANAL: BOND: Index        Atom-I               ',                          ;// 243           FORMAT('ANAL: BOND: Index        Atom-I               ',
//     &           '    Atom-J                  Dist           Energy   ',                         ;//     &           '    Atom-J                  Dist           Energy   ',
//     &                '      Force            Parameters')                                       ;//     &                '      Force            Parameters')
// 244           FORMAT('ANAL: BOND: Index        Atom-I               ',                          ;// 244           FORMAT('ANAL: BOND: Index        Atom-I               ',
//     &                '    Atom-J          ',/                                                   ;//     &                '    Atom-J          ',/
//     &                '        Dist           Energy   ',                                        ;//     &                '        Dist           Energy   ',
//     &                '      Force            Parameters')                                       ;//     &                '      Force            Parameters')
//               QAFIRST= false                                                                    ;//               QAFIRST=.FALSE.
//              }                                                                                  ;//              ENDIF
//              ATOMID(I,SIDDNI,RIDDNI,RESDNI,ACDNI)                                               ;//              CALL ATOMID(I,SIDDNI,RIDDNI,RESDNI,ACDNI)
//              ATOMID(J,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ)                                               ;//              CALL ATOMID(J,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ)
//              if(QLONGL) {                                                                       ;//              IF(QLONGL) THEN
//                 WRITE(II,245) MM,I,SIDDNI(1:idleng),RIDDNI(1:idleng),                           ;//                 WRITE(II,245) MM,I,SIDDNI(1:idleng),RIDDNI(1:idleng),
//     $                RESDNI(1:idleng),ACDNI(1:idleng),                                          ;//     $                RESDNI(1:idleng),ACDNI(1:idleng),
//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),                                       ;//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),
//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),                                          ;//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),
//     $                S,E,DF,IC,CBB(IC),CBC(IC)                                                  ;//     $                S,E,DF,IC,CBB(IC),CBC(IC)
//              } else {                                                                           ;//              ELSE
//                 WRITE(II,246) MM,I,SIDDNI(1:idleng),RIDDNI(1:idleng),                           ;//                 WRITE(II,246) MM,I,SIDDNI(1:idleng),RIDDNI(1:idleng),
//     $                RESDNI(1:idleng),ACDNI(1:idleng),                                          ;//     $                RESDNI(1:idleng),ACDNI(1:idleng),
//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),                                       ;//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),
//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),                                          ;//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),
//     $                S,E,DF,IC,CBB(IC),CBC(IC)                                                  ;//     $                S,E,DF,IC,CBB(IC),CBC(IC)
//              }                                                                                  ;//              ENDIF
// 245          FORMAT('ANAL: BOND>',2I5,4(1X,A),I5,4(1X,A),                                       ;// 245          FORMAT('ANAL: BOND>',2I5,4(1X,A),I5,4(1X,A),
//     &               3F15.6,I5,2F15.6)                                                           ;//     &               3F15.6,I5,2F15.6)
// 246          FORMAT('ANAL: BOND>',2I5,4(1X,A),I5,4(1X,A),/                                      ;// 246          FORMAT('ANAL: BOND>',2I5,4(1X,A),I5,4(1X,A),/
//     &               3F15.6,I5,2F15.6)                                                           ;//     &               3F15.6,I5,2F15.6)
//            }                                                                                    ;//            ENDIF
            }                                                                                      ;//          ENDIF
          }                                                                                        ;//        ENDIF
///                                                                                                ;//C
          if(QECONTX) {                                                                            ;//        IF(QECONTX) THEN
            E=E*HALF                                                                               ;//          E=E*HALF
            ECONTX[I]=ECONTX[I]+E                                                                  ;//          ECONTX(I)=ECONTX(I)+E
            ECONTX[J]=ECONTX[J]+E                                                                  ;//          ECONTX(J)=ECONTX(J)+E
          }                                                                                        ;//        ENDIF
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        IF (.NOT. NOFORC) THEN
                                                                                                   ;//##ENDIF ! BLOCK
          DF=DF*R                                                                                  ;//        DF=DF*R
///                                                                                                ;//C
          DXI=RX*DF                                                                                ;//        DXI=RX*DF
          DYI=RY*DF                                                                                ;//        DYI=RY*DF
          DZI=RZ*DF                                                                                ;//        DZI=RZ*DF
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        IF(QDOCK) THEN
                                                                                                   ;//          DX(I)=DX(I)+DXI*DOCFI
                                                                                                   ;//          DY(I)=DY(I)+DYI*DOCFI
                                                                                                   ;//          DZ(I)=DZ(I)+DZI*DOCFI
                                                                                                   ;//          DX(J)=DX(J)-DXI*DOCFJ
                                                                                                   ;//          DY(J)=DY(J)-DYI*DOCFJ
                                                                                                   ;//          DZ(J)=DZ(J)-DZI*DOCFJ
                                                                                                   ;//        ELSE
                                                                                                   ;//##ENDIF !DOCK
                                                                                                   ;//##ENDIF
            DX[I]=DX[I]+DXI                                                                        ;//          DX(I)=DX(I)+DXI 
            DY[I]=DY[I]+DYI                                                                        ;//          DY(I)=DY(I)+DYI
            DZ[I]=DZ[I]+DZI                                                                        ;//          DZ(I)=DZ(I)+DZI
            DX[J]=DX[J]-DXI                                                                        ;//          DX(J)=DX(J)-DXI
            DY[J]=DY[J]-DYI                                                                        ;//          DY(J)=DY(J)-DYI
            DZ[J]=DZ[J]-DZI                                                                        ;//          DZ(J)=DZ(J)-DZI
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF 
                                                                                                   ;//##ENDIF
                                                                                                   ;//##IF FOURD (4daddf)
                                                                                                   ;//        IF(DIM4ON(1).EQ.1) THEN
                                                                                                   ;//          DFDI=RFDIM*DF
                                                                                                   ;//          DFDIM(I)=DFDIM(I)+DFDI
                                                                                                   ;//          DFDIM(J)=DFDIM(J)-DFDI
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF (4daddf)
                                                                                                   ;//C
                                                                                                   ;//##IF IPRESS
                                                                                                   ;//        IF(QIPRSS) THEN
                                                                                                   ;//         PVIR(I)=PVIR(I)+S2*DF
                                                                                                   ;//         PVIR(J)=PVIR(J)+S2*DF
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
///                                                                                                ;//C
///       SECOND DERIVATIVE PART                                                                   ;//C       SECOND DERIVATIVE PART
          if (QSECD) {                                                                             ;//        IF (QSECD) THEN
                                                                                                   ;//##IF MBOND
                                                                                                   ;//      IF (qMBSec) THEN
                                                                                                   ;//
                                                                                                   ;//         CALL MBSecB(heap(pDD1Sys(1)),heap(pDD1Vac(1)),I,J,
                                                                                                   ;//     &                KBEXPN,RX,RY,RZ,DF,DDF)
                                                                                                   ;//
                                                                                                   ;//      ELSE
                                                                                                   ;//##ENDIF  ! MBOND
                                                                                                   ;//
                                                                                                   ;//##IF DIMB
                                                                                                   ;//         IF(QCMPCT) THEN
                                                                                                   ;//            CALL EBNCMP(I,J,RX,RY,RZ,DF,DDF,DD1,HEAP(PINBCM),
                                                                                                   ;//     1                  HEAP(PJNBCM),KBEXPN)
                                                                                                   ;//         ELSE
                                                                                                   ;//##ENDIF ! DIMB
                                                                                                   ;//
            if(KBEXPN  !=  2) {                                                                    ;//          IF(KBEXPN .NE. 2) THEN
//            CALL WRNDIE(-3,'<EBOND> ','NO SECOND DERIV. FOR ANHARMONIC BONDS')                   ;//            CALL WRNDIE(-3,'<EBOND> ','NO SECOND DERIV. FOR '
              throw new Exception("<EBOND> NO SECOND DERIV. FOR ANHARMONIC BONDS")                 ;//     1        //'ANHARMONIC BONDS')
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if (J <  I) {                                                                          ;//          IF (J.LT.I) THEN
              JJ=3*I-2                                                                             ;//            JJ=3*I-2
              II=3*J-2                                                                             ;//            II=3*J-2
            } else {                                                                               ;//          ELSE
              JJ=3*J-2                                                                             ;//            JJ=3*J-2
              II=3*I-2                                                                             ;//            II=3*I-2
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            A=RX*RX*DDF+DF                                                                         ;//          A=RX*RX*DDF+DF
            IADD=IUPT[II]+JJ                                                                       ;//          IADD=IUPT(II)+JJ
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II]+II                                                                       ;//          IADD=IUPT(II)+II
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
            IADD=IUPT[JJ]+JJ                                                                       ;//          IADD=IUPT(JJ)+JJ
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
///                                                                                                ;//C
            A=RY*RY*DDF+DF                                                                         ;//          A=RY*RY*DDF+DF
            IADD=IUPT[II+1]+JJ+1                                                                   ;//          IADD=IUPT(II+1)+JJ+1
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II+1]+II+1                                                                   ;//          IADD=IUPT(II+1)+II+1
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
            IADD=IUPT[JJ+1]+JJ+1                                                                   ;//          IADD=IUPT(JJ+1)+JJ+1
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
///                                                                                                ;//C
            A=RZ*RZ*DDF+DF                                                                         ;//          A=RZ*RZ*DDF+DF
            IADD=IUPT[II+2]+JJ+2                                                                   ;//          IADD=IUPT(II+2)+JJ+2
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II+2]+II+2                                                                   ;//          IADD=IUPT(II+2)+II+2
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
            IADD=IUPT[JJ+2]+JJ+2                                                                   ;//          IADD=IUPT(JJ+2)+JJ+2
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
///                                                                                                ;//C
            A=RX*RY*DDF                                                                            ;//          A=RX*RY*DDF
            IADD=IUPT[II]+JJ+1                                                                     ;//          IADD=IUPT(II)+JJ+1
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II+1]+JJ                                                                     ;//          IADD=IUPT(II+1)+JJ
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II]+II+1                                                                     ;//          IADD=IUPT(II)+II+1
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
            IADD=IUPT[JJ]+JJ+1                                                                     ;//          IADD=IUPT(JJ)+JJ+1
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
///                                                                                                ;//C
            A=RX*RZ*DDF                                                                            ;//          A=RX*RZ*DDF
            IADD=IUPT[II]+JJ+2                                                                     ;//          IADD=IUPT(II)+JJ+2
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II+2]+JJ                                                                     ;//          IADD=IUPT(II+2)+JJ
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II]+II+2                                                                     ;//          IADD=IUPT(II)+II+2
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
            IADD=IUPT[JJ]+JJ+2                                                                     ;//          IADD=IUPT(JJ)+JJ+2
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
///                                                                                                ;//C
            A=RY*RZ*DDF                                                                            ;//          A=RY*RZ*DDF
            IADD=IUPT[II+1]+JJ+2                                                                   ;//          IADD=IUPT(II+1)+JJ+2
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II+2]+JJ+1                                                                   ;//          IADD=IUPT(II+2)+JJ+1
            DD1[IADD]=DD1[IADD]-A                                                                  ;//          DD1(IADD)=DD1(IADD)-A
            IADD=IUPT[II+1]+II+2                                                                   ;//          IADD=IUPT(II+1)+II+2
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
            IADD=IUPT[JJ+1]+JJ+2                                                                   ;//          IADD=IUPT(JJ+1)+JJ+2
            DD1[IADD]=DD1[IADD]+A                                                                  ;//          DD1(IADD)=DD1(IADD)+A
                                                                                                   ;//##IF DIMB
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF ! DIMB
                                                                                                   ;//
                                                                                                   ;//##IF MBOND
                                                                                                   ;//      ENDIF
                                                                                                   ;//##ENDIF ! MBOND
                                                                                                   ;//
          }                                                                                        ;//        ENDIF
                                                                                                   ;//
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF ! BLOCK
                                                                                                   ;//
        }                                                                                          ;//   10 CONTINUE
        return                                                                                     ;//      RETURN
    }                                                                                               //      END
                                                                                                    //
    public void EANGLE(double ET,int[] IT,int[] JT,int[] KT,int[] ICT,int NTHETA,double[] CTC,      //      SUBROUTINE EANGLE(ET,IT,JT,KT,ICT,NTHETA,CTC,CTB,DX,DY,DZ,X,Y,Z,
                  double[] CTB,double[] DX,double[] DY,double[] DZ,double[] X,double[] Y,double[] Z,//     $ QECONTX,ECONTX,ICONAH,ISKT,DD1,IUPT,QSECD)
                  bool QECONTX,double[] ECONTX,int ICONAH,int[] ISKT,double[] DD1,int[] IUPT,bool QSECD)
    {
///-----------------------------------------------------------------------                         ;//C-----------------------------------------------------------------------
///     CALCULATES BOND ANGLES AND BOND ANGLE ENERGIES.                                            ;//C     CALCULATES BOND ANGLES AND BOND ANGLE ENERGIES.
///     IT DOES NOT CALCULATE ANGLE ENERGY AND FORCES FOR ALL ANGLES                               ;//C     IT DOES NOT CALCULATE ANGLE ENERGY AND FORCES FOR ALL ANGLES
///     WITH A NONZERO VALUE IN ISKT ARRAY ONLY WHEN ICONAH .NE. 0                                 ;//C     WITH A NONZERO VALUE IN ISKT ARRAY ONLY WHEN ICONAH .NE. 0
///                                                                                                ;//C
///     By Bernard R. Brooks  (mostly)  1981-1983                                                  ;//C     By Bernard R. Brooks  (mostly)  1981-1983
///                                                                                                ;//C
//##INCLUDE '~/charmm_fcm/impnon.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/impnon.fcm'
//##INCLUDE '~/charmm_fcm/dimens.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/dimens.fcm'
//##INCLUDE '~/charmm_fcm/exfunc.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/exfunc.fcm'
//##INCLUDE '~/charmm_fcm/number.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/number.fcm'
///                                                                                                ;//C
//##INCLUDE '~/charmm_fcm/fourd.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/fourd.fcm'
//##IF BLOCK DIMB MBOND                                                                            ;//##IF BLOCK DIMB MBOND
//##INCLUDE '~/charmm_fcm/heap.fcm'                                                                ;//##INCLUDE '~/charmm_fcm/heap.fcm'
//##ENDIF ! BLOCK DIMB MBOND                                                                       ;//##ENDIF ! BLOCK DIMB MBOND
//##INCLUDE '~/charmm_fcm/block.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/block.fcm'
//##INCLUDE '~/charmm_fcm/lambda.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/lambda.fcm'
//##INCLUDE '~/charmm_fcm/mbond.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/mbond.fcm'
//##INCLUDE '~/charmm_fcm/dimb.fcm'                                                                ;//##INCLUDE '~/charmm_fcm/dimb.fcm'
//##INCLUDE '~/charmm_fcm/econt.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/econt.fcm'
//##IF PARALLEL                                                                                    ;//##IF PARALLEL
//##INCLUDE '~/charmm_fcm/parallel.fcm'                                                            ;//##INCLUDE '~/charmm_fcm/parallel.fcm'
//      LOGICAL NOPARS                                                                             ;//      LOGICAL NOPARS
//##ENDIF                                                                                          ;//##ENDIF
///                                                                                                ;//C
//##IF LDM                                                                                         ;//##IF LDM
//##IF LRST                                                                                        ;//##IF LRST
//      REAL*8 DFORG                                                                               ;//      REAL*8 DFORG
//##ENDIF ! LRST                                                                                   ;//##ENDIF ! LRST
//##ENDIF                                                                                          ;//##ENDIF
//      REAL*8 ET                                                                                  ;//      REAL*8 ET
//      INTEGER IT(*),JT(*),KT(*),ICT(*)                                                           ;//      INTEGER IT(*),JT(*),KT(*),ICT(*)
//      INTEGER NTHETA                                                                             ;//      INTEGER NTHETA
//      REAL*8 CTC(*),CTB(*)                                                                       ;//      REAL*8 CTC(*),CTB(*)
//      REAL*8 DX(*),DY(*),DZ(*),X(*),Y(*),Z(*)                                                    ;//      REAL*8 DX(*),DY(*),DZ(*),X(*),Y(*),Z(*)
//      LOGICAL QECONTX                                                                            ;//      LOGICAL QECONTX
//      REAL*8 ECONTX(*)                                                                           ;//      REAL*8 ECONTX(*)
//      INTEGER ICONAH                                                                             ;//      INTEGER ICONAH
//      INTEGER ISKT(*)                                                                            ;//      INTEGER ISKT(*)
//      REAL*8 DD1(*)                                                                              ;//      REAL*8 DD1(*)
//      INTEGER IUPT(*)                                                                            ;//      INTEGER IUPT(*)
//      LOGICAL QSECD                                                                              ;//      LOGICAL QSECD
///                                                                                                ;//C
//##INCLUDE '~/charmm_fcm/stream.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/stream.fcm'
//##INCLUDE '~/charmm_fcm/consta.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/consta.fcm'
///                                                                                                ;//C
        double DXI,DYI,DZI,DXJ,DYJ,DZJ,RI2,RJ2,RI,RJ                                               ;//      REAL*8 DXI,DYI,DZI,DXJ,DYJ,DZJ,RI2,RJ2,RI,RJ
        double RIR,RJR,DXIR,DYIR,DZIR,DXJR,DYJR,DZJR,CST,AT,DA,DF,E                                ;//      REAL*8 RIR,RJR,DXIR,DYIR,DZIR,DXJR,DYJR,DZJR,CST,AT,DA,DF,E
        double ST2R,STR,DTXI,DTXJ,DTYI,DTYJ,DTZI,DTZJ                                              ;//      REAL*8 ST2R,STR,DTXI,DTXJ,DTYI,DTYJ,DTZI,DTZJ
        double DFX,DFY,DFZ,DGX,DGY,DGZ,DDF,RI2RF,RJ2RF,RIRJF                                       ;//      REAL*8 DFX,DFY,DFZ,DGX,DGY,DGZ,DDF,RI2RF,RJ2RF,RIRJF
        double DDXIXI,DDYIYI,DDZIZI,DDXJXJ,DDYJYJ,DDZJZJ                                           ;//      REAL*8 DDXIXI,DDYIYI,DDZIZI,DDXJXJ,DDYJYJ,DDZJZJ
        double DDXIXJ,DDYIYJ,DDZIZJ,DDXIYI,DDXIZI,DDYIZI                                           ;//      REAL*8 DDXIXJ,DDYIYJ,DDZIZJ,DDXIYI,DDXIZI,DDYIZI
        double DDXJYJ,DDXJZJ,DDYJZJ,DDXIYJ,DDYIXJ,DDXIZJ                                           ;//      REAL*8 DDXJYJ,DDXJZJ,DDYJZJ,DDXIYJ,DDYIXJ,DDXIZJ
        double DDZIXJ,DDYIZJ,DDZIYJ,A,SMALLV                                                       ;//      REAL*8 DDZIXJ,DDYIZJ,DDZIYJ,A,SMALLV
        int     I,NWARN,ITH,J,K,IC,JJ,II,KK,IADD                                                   ;//      INTEGER I,NWARN,ITH,J,K,IC,JJ,II,KK,IADD
        bool    NOCONS,IJTEST,IKTEST,JKTEST;//,QAFIRST                                             ;//      LOGICAL NOCONS,IJTEST,IKTEST,JKTEST,QAFIRST
      //string      SIDDNI,RIDDNI,RESDNI,ACDNI,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ                          ;//      CHARACTER*8 SIDDNI,RIDDNI,RESDNI,ACDNI,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ
      //string      SIDDNK,RIDDNK,RESDNK,ACDNK                                                     ;//      CHARACTER*8 SIDDNK,RIDDNK,RESDNK,ACDNK
///                                                                                                ;//C
                                                                                                   ;//##IF FOURD (4ddecl)
                                                                                                   ;//C     4-D variables:
                                                                                                   ;//      REAL*8 DFDIMI,DFDIMJ,DFDIMIR,DFDIMJR,DTFDI,DTFDJ,DFFD,DGFD
                                                                                                   ;//##ENDIF (4ddecl)
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//      REAL*8 COEF, DOCFI, DOCFJ, DOCFK, DOCFJ1
                                                                                                   ;//      INTEGER IBL, JBL, KDOC
                                                                                                   ;//##IF LDM
                                                                                                   ;//      REAL*8 UNSCALE, FALPHA
                                                                                                   ;//##ENDIF ! LDM
                                                                                                   ;//##ENDIF ! BLOCK
///                                                                                                ;//C
        ET=ZERO                                                                                    ;//      ET=ZERO
                                                                                                   ;//
        SMALLV=RPRECI                                                                              ;//      SMALLV=RPRECI
        NWARN=0                                                                                    ;//      NWARN=0
        NOCONS=(ICONAH >  0)                                                                       ;//      NOCONS=(ICONAH.GT.0)
        if(NTHETA == 0) return                                                                     ;//      IF(NTHETA.EQ.0) RETURN
//      QAFIRST= true                                                                              ;//      QAFIRST=.TRUE.
///                                                                                                ;//C
                                                                                                   ;//##IF PARALLEL (paraangle)
                                                                                                   ;//##IF PARAFULL (parfangle)
                                                                                                   ;//      DO 20 ITH=MYNODP,NTHETA,NUMNOD
                                                                                                   ;//##ELIF PARASCAL (parfangle)
                                                                                                   ;//      NOPARS=(ICONAH.GE.0)
                                                                                                   ;//      DO 20 ITH=1,NTHETA
                                                                                                   ;//        IF(NOPARS) THEN
                                                                                                   ;//          II=IPBLOCK(IT(ITH))
                                                                                                   ;//          JJ=IPBLOCK(JT(ITH))
                                                                                                   ;//          KK=IPBLOCK(KT(ITH))
                                                                                                   ;//          IC=II
                                                                                                   ;//          IF(II.EQ.JJ) IC=KK
                                                                                                   ;//          IF(KK.NE.JJ .AND. KK.NE.IC) THEN
                                                                                                   ;//C           the angle spans three block.
                                                                                                   ;//C           make sure that we have the coordinates.
                                                                                                   ;//            CALL PSADDTOCL(KT(ITH),JPMAT(JJ,IC))
                                                                                                   ;//          ENDIF
                                                                                                   ;//          IF(JPMAT(JJ,IC).NE.MYNOD) GOTO 20
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF (parfangle)
                                                                                                   ;//##ELSE (paraangle)
        for(ITH=1; ITH<=NTHETA; ITH++) {                                                           ;//      DO 20 ITH=1,NTHETA
                                                                                                   ;//##ENDIF (paraangle)
///                                                                                                ;//C
          I=IT[ITH]                                                                                ;//        I=IT(ITH)
          if(NOCONS) {                                                                             ;//        IF(NOCONS) THEN
            if(ISKT[ITH] != 0) continue                                                            ;//          IF(ISKT(ITH).NE.0) GOTO 20
          }                                                                                        ;//        ENDIF
          J=JT[ITH]                                                                                ;//        J=JT(ITH)
          K=KT[ITH]                                                                                ;//        K=KT(ITH)
          IC=ICT[ITH]                                                                              ;//        IC=ICT(ITH)
          if(IC == 0) continue                                                                     ;//        IF(IC.EQ.0) GOTO 20
          DXI=X[I]-X[J]                                                                            ;//        DXI=X(I)-X(J)
          DYI=Y[I]-Y[J]                                                                            ;//        DYI=Y(I)-Y(J)
          DZI=Z[I]-Z[J]                                                                            ;//        DZI=Z(I)-Z(J)
          DXJ=X[K]-X[J]                                                                            ;//        DXJ=X(K)-X(J)
          DYJ=Y[K]-Y[J]                                                                            ;//        DYJ=Y(K)-Y(J)
          DZJ=Z[K]-Z[J]                                                                            ;//        DZJ=Z(K)-Z(J)
          RI2=DXI*DXI+DYI*DYI+DZI*DZI                                                              ;//        RI2=DXI*DXI+DYI*DYI+DZI*DZI
          RJ2=DXJ*DXJ+DYJ*DYJ+DZJ*DZJ                                                              ;//        RJ2=DXJ*DXJ+DYJ*DYJ+DZJ*DZJ
                                                                                                   ;//##IF FOURD (4dang1)
                                                                                                   ;//        IF (DIM4ON(2).EQ.1) THEN
                                                                                                   ;//          DFDIMI=FDIM(I)-FDIM(J) 
                                                                                                   ;//          DFDIMJ=FDIM(K)-FDIM(J)
                                                                                                   ;//          RI2=RI2+DFDIMI*DFDIMI
                                                                                                   ;//          RJ2=RJ2+DFDIMJ*DFDIMJ
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF (4dang1)
          RI=SQRT(RI2)                                                                             ;//        RI=SQRT(RI2)
          RJ=SQRT(RJ2)                                                                             ;//        RJ=SQRT(RJ2)
          RIR=ONE/RI                                                                               ;//        RIR=ONE/RI
          RJR=ONE/RJ                                                                               ;//        RJR=ONE/RJ
          DXIR=DXI*RIR                                                                             ;//        DXIR=DXI*RIR
          DYIR=DYI*RIR                                                                             ;//        DYIR=DYI*RIR
          DZIR=DZI*RIR                                                                             ;//        DZIR=DZI*RIR
          DXJR=DXJ*RJR                                                                             ;//        DXJR=DXJ*RJR
          DYJR=DYJ*RJR                                                                             ;//        DYJR=DYJ*RJR
          DZJR=DZJ*RJR                                                                             ;//        DZJR=DZJ*RJR
          CST=DXIR*DXJR+DYIR*DYJR+DZIR*DZJR                                                        ;//        CST=DXIR*DXJR+DYIR*DYJR+DZIR*DZJR
                                                                                                   ;//##IF FOURD (4dang2)
                                                                                                   ;//        IF(DIM4ON(2).EQ.1) THEN
                                                                                                   ;//          DFDIMIR=DFDIMI*RIR
                                                                                                   ;//          DFDIMJR=DFDIMJ*RJR
                                                                                                   ;//          CST=CST+DFDIMIR*DFDIMJR
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF (4dang2)
///                                                                                                ;//C
          if(ABS(CST) >= COSMAX) {                                                                 ;//        IF(ABS(CST).GE.COSMAX) THEN
            if(ABS(CST) >  ONE) CST=SIGN(ONE,CST)                                                  ;//          IF(ABS(CST).GT.ONE) CST=SIGN(ONE,CST)
            AT=ACOS(CST)                                                                           ;//          AT=ACOS(CST)
            DA=AT-CTB[IC]                                                                          ;//          DA=AT-CTB(IC)
            if(ABS(DA) >  0.1) {                                                                   ;//          IF(ABS(DA).GT.0.1) THEN
              NWARN=NWARN+1                                                                        ;//            NWARN=NWARN+1
              if((NWARN <= 5  &&   WRNLEV >= 5)  ||  WRNLEV >= 6) { HDebug.Assert(false)           ;//            IF((NWARN.LE.5 .AND. WRNLEV.GE.5) .OR. WRNLEV.GE.6) THEN
//              WRITE(OUTU,10) ITH,I,J,K                                                           ;//              WRITE(OUTU,10) ITH,I,J,K
//   10         FORMAT(' WARNING FROM EANGLE. Angle',I5,                                           ;//   10         FORMAT(' WARNING FROM EANGLE. Angle',I5,
//     &               '  is almost linear.',                                                      ;//     &               '  is almost linear.',
//     &          /' Derivatives may be affected for atoms:',3I5)                                  ;//     &          /' Derivatives may be affected for atoms:',3I5)
//              WRITE(OUTU,101) 'I ATOM:',X[I],Y[I],Z[I]                                           ;//              WRITE(OUTU,101) 'I ATOM:',X(I),Y(I),Z(I)
//              WRITE(OUTU,101) 'J ATOM:',X[J],Y[J],Z[J]                                           ;//              WRITE(OUTU,101) 'J ATOM:',X(J),Y(J),Z(J)
//              WRITE(OUTU,101) 'K ATOM:',X[K],Y[K],Z[K]                                           ;//              WRITE(OUTU,101) 'K ATOM:',X(K),Y(K),Z(K)
//              WRITE(OUTU,101) 'DXIR  :',DXIR,DYIR,DZIR                                           ;//              WRITE(OUTU,101) 'DXIR  :',DXIR,DYIR,DZIR
//              WRITE(OUTU,101) 'DXJR  :',DXJR,DYJR,DZJR                                           ;//              WRITE(OUTU,101) 'DXJR  :',DXJR,DYJR,DZJR
//              WRITE(OUTU,101) 'CST   :',CST,AT*RADDEG,DA*RADDEG                                  ;//              WRITE(OUTU,101) 'CST   :',CST,AT*RADDEG,DA*RADDEG
// 101          FORMAT(5X,A,5F15.5)                                                                ;// 101          FORMAT(5X,A,5F15.5)
              }                                                                                    ;//            ENDIF
            }                                                                                      ;//          ENDIF
          }                                                                                        ;//        ENDIF
///                                                                                                ;//C
          AT=ACOS(CST)                                                                             ;//        AT=ACOS(CST)
///                                                                                                ;//C
          DA=AT-CTB[IC]                                                                            ;//        DA=AT-CTB(IC)
          DF=CTC[IC]*DA                                                                            ;//        DF=CTC(IC)*DA
          DDF=CTC[IC]                                                                              ;//        DDF=CTC(IC)
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        IF (QBLOCK) THEN
                                                                                                   ;//          IBL=I4VAL(HEAP(IBLCKP),I)
                                                                                                   ;//          JBL=I4VAL(HEAP(IBLCKP),J)
                                                                                                   ;//          KK= I4VAL(HEAP(IBLCKP),K)
                                                                                                   ;//##IF DOCK
                                                                                                   ;//c         two pairs in an angle (i,j), (k,j)
                                                                                                   ;//          DOCFI = 1.0
                                                                                                   ;//          DOCFJ = 1.0
                                                                                                   ;//          DOCFK = 1.0
                                                                                                   ;//          DOCFJ1 = 1.0
                                                                                                   ;//          IF(QDOCK) THEN
                                                                                                   ;//              KDOC  = (IBL - 1)*NBLOCK + JBL
                                                                                                   ;//              DOCFI = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (JBL - 1)*NBLOCK + IBL
                                                                                                   ;//              DOCFJ = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (KK - 1)*NBLOCK + JBL
                                                                                                   ;//              DOCFK = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (JBL - 1)*NBLOCK + KK
                                                                                                   ;//              DOCFJ1 = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! DOCK
                                                                                                   ;//          IF (IBL .EQ. JBL) JBL=KK
                                                                                                   ;//          IF (JBL .LT. IBL) THEN
                                                                                                   ;//            KK=JBL
                                                                                                   ;//            JBL=IBL
                                                                                                   ;//            IBL=KK
                                                                                                   ;//          ENDIF
                                                                                                   ;//          KK=IBL+JBL*(JBL-1)/2
                                                                                                   ;//          COEF=GTRR8(HEAP(BLCOEA),KK)
                                                                                                   ;//##IF BANBA
                                                                                                   ;//          IF (QPRNTV .AND. .NOT. QNOAN) THEN
                                                                                                   ;//            IF (IBL.EQ.1 .OR. JBL.EQ.1 .OR. IBL.EQ.JBL) THEN
                                                                                                   ;//              CALL ASUMR8(DF*DA,HEAP(VBANG),JBL)
                                                                                                   ;//            ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! BANBA                    
                                                                                                   ;//##IF LDM (ldm_2) 
                                                                                                   ;//          IF(QLDM) THEN                          !##.not.LMC
                                                                                                   ;//          IF(QLDM.or.QLMC) THEN                  !##LMC
                                                                                                   ;//C           first row or diagonal elements exclude (1,1).
                                                                                                   ;//            UNSCALE = 0.0
                                                                                                   ;//##IF LDMGEN (ldmgen_1)
                                                                                                   ;//            IF((IBL.EQ.1.AND.JBL.GE.LSTRT).or.
                                                                                                   ;//     &         (IBL.GE.LSTRT.AND.IBL.EQ.JBL)) UNSCALE = DF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ELSE (ldmgen_1)
                                                                                                   ;//            IF(IBL.NE.1.AND.IBL.EQ.JBL) THEN 
                                                                                                   ;//               UNSCALE = DF
                                                                                                   ;//            ELSE IF(IBL.EQ.1.AND.JBL.GE.2) THEN
                                                                                                   ;//               UNSCALE = DF
                                                                                                   ;//            ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF     (ldmgen_1)
                                                                                                   ;//##IF BANBA
                                                                                                   ;//         IF(QNOAN) COEF = 1.0
                                                                                                   ;//##ELSE
                                                                                                   ;//         IF(QLDM .AND. QNOAN) COEF = 1.0
                                                                                                   ;//##ENDIF
                                                                                                   ;//##IF LRST
                                                                                                   ;//          IF(RSTP.AND. .NOT. QNOAN)THEN
                                                                                                   ;//           IF( (IBL.EQ.1 .AND. JBL.GE.LSTRT) .or.
                                                                                                   ;//     &         (IBL.GE.LSTRT .AND. IBL.EQ.JBL) ) THEN
                                                                                                   ;//             DFORG = DF
                                                                                                   ;//           ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! LRST
                                                                                                   ;//##ENDIF (ldm_2) 
                                                                                                   ;//
                                                                                                   ;//          DF=DF*COEF
                                                                                                   ;//          DDF=DDF*COEF
                                                                                                   ;//        ENDIF
                                                                                                   ;//##IF LDM
                                                                                                   ;//          IF(QLDM.AND. .NOT. QNOAN) THEN              !##.not.LMC
                                                                                                   ;//          IF((QLDM.or.QLMC).AND. .NOT. QNOAN) THEN    !##LMC
                                                                                                   ;//C
                                                                                                   ;//##IF LDMGEN (ldmgen_2)
                                                                                                   ;//           IF((IBL.EQ.1.AND.JBL.GE.LSTRT).or.
                                                                                                   ;//     &        (IBL.GE.LSTRT.AND.IBL.EQ.JBL)) THEN 
                                                                                                   ;//##ELSE (ldmgen_2)
                                                                                                   ;//           IF((IBL.EQ.JBL).OR.(IBL.EQ.1.AND.JBL.GE.2)) THEN
                                                                                                   ;//##ENDIF (ldmgen_2)
                                                                                                   ;//              FALPHA = UNSCALE*DA
                                                                                                   ;//              LAGMUL = LAGMUL + FALPHA
                                                                                                   ;//              CALL ASUMR8(FALPHA,HEAP(BIFLAM),JBL)
                                                                                                   ;//##IF LRST
                                                                                                   ;//              IF(NRST.EQ.2)THEN
                                                                                                   ;//                CALL ASUMR8(FALPHA,HEAP(BFRST),JBL)
                                                                                                   ;//              ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//           ENDIF
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF !LDM
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        IF(QDOCK) THEN
                                                                                                   ;//C         Factor 0.25 to make sure no double counting
                                                                                                   ;//          E=DF*DA*0.25*(DOCFI+DOCFJ+DOCFK+DOCFJ1)
                                                                                                   ;//        ELSE
                                                                                                   ;//##ENDIF 
                                                                                                   ;//##ENDIF ! BLOCK
            E=DF*DA                                                                                ;//          E=DF*DA
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//##ENDIF
///                                                                                                ;//C
          ET=ET+E                                                                                  ;//        ET=ET+E
          DF=DF+DF                                                                                 ;//        DF=DF+DF
///                                                                                                ;//C
//        if(QATERM) {                                                                             ;//        IF(QATERM) THEN
//          KK=ANSLCT[I]+ANSLCT[J]+ANSLCT[K]                                                       ;//          KK=ANSLCT(I)+ANSLCT(J)+ANSLCT(K)
//          if(KK == 3  ||  (KK >= 1  &&      ! QAONLY)) {                                         ;//          IF(KK.EQ.3 .OR. (KK.GE.1 .AND. .NOT.QAONLY)) THEN
//            if(QAUNIT <  0) {                                                                    ;//            IF(QAUNIT.LT.0) THEN
//              II=OUTU                                                                            ;//              II=OUTU
//            } else {                                                                             ;//            ELSE
//              II=QAUNIT                                                                          ;//              II=QAUNIT
//            }                                                                                    ;//            ENDIF
///                                                                                                ;//C
//            if(PRNLEV >= 5) THEN                                                                 ;//            IF(PRNLEV.GE.5) THEN
//              if(QAFIRST) THEN                                                                   ;//              IF(QAFIRST) THEN
//                if(QLONGL) THEN                                                                  ;//                IF(QLONGL) THEN
//                  WRITE(II,243)                                                                  ;//                  WRITE(II,243)
//                ELSE                                                                             ;//                ELSE
//                  WRITE(II,244)                                                                  ;//                  WRITE(II,244)
//                ENDIF                                                                            ;//                ENDIF
// 243           FORMAT('ANAL: ANGL: Index        Atom-I             ',                            ;// 243           FORMAT('ANAL: ANGL: Index        Atom-I             ',
//     &       '      Atom-J                   Atom-K                ',                            ;//     &       '      Atom-J                   Atom-K                ',
//     &         '  Angle          Energy   ',                                                     ;//     &         '  Angle          Energy   ',
//     &                '      Force            Parameters')                                       ;//     &                '      Force            Parameters')
// 244           FORMAT('ANAL: ANGL: Index        Atom-I             ',                            ;// 244           FORMAT('ANAL: ANGL: Index        Atom-I             ',
//     &                '      Atom-J                   Atom-K          ',                         ;//     &                '      Atom-J                   Atom-K          ',
//     &               /'        Angle          Energy   ',                                        ;//     &               /'        Angle          Energy   ',
//     &                '      Force            Parameters')                                       ;//     &                '      Force            Parameters')
//               QAFIRST= false                                                                    ;//               QAFIRST=.FALSE.
//              ENDIF                                                                              ;//              ENDIF
//              CALL ATOMID(I,SIDDNI,RIDDNI,RESDNI,ACDNI)                                          ;//              CALL ATOMID(I,SIDDNI,RIDDNI,RESDNI,ACDNI)
//              CALL ATOMID(J,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ)                                          ;//              CALL ATOMID(J,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ)
//              CALL ATOMID(K,SIDDNK,RIDDNK,RESDNK,ACDNK)                                          ;//              CALL ATOMID(K,SIDDNK,RIDDNK,RESDNK,ACDNK)
//              if(QLONGL) THEN                                                                    ;//              IF(QLONGL) THEN
//                 WRITE(II,246) ITH,I,SIDDNI(1:idleng),RIDDNI(1:idleng),                          ;//                 WRITE(II,246) ITH,I,SIDDNI(1:idleng),RIDDNI(1:idleng),
//     $                RESDNI(1:idleng),ACDNI(1:idleng),                                          ;//     $                RESDNI(1:idleng),ACDNI(1:idleng),
//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),                                       ;//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),
//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),                                          ;//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),
//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),                                       ;//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),
//     $                RESDNK(1:idleng),ACDNK(1:idleng),                                          ;//     $                RESDNK(1:idleng),ACDNK(1:idleng),
//     $                AT*RADDEG,E,DF,IC,CTB(IC)*RADDEG,CTC(IC)                                   ;//     $                AT*RADDEG,E,DF,IC,CTB(IC)*RADDEG,CTC(IC)
//              ELSE                                                                               ;//              ELSE
//                 WRITE(II,245) ITH,I,SIDDNI(1:idleng),RIDDNI(1:idleng),                          ;//                 WRITE(II,245) ITH,I,SIDDNI(1:idleng),RIDDNI(1:idleng),
//     $                RESDNI(1:idleng),ACDNI(1:idleng),                                          ;//     $                RESDNI(1:idleng),ACDNI(1:idleng),
//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),                                       ;//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),
//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),                                          ;//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),
//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),                                       ;//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),
//     $                RESDNK(1:idleng),ACDNK(1:idleng),                                          ;//     $                RESDNK(1:idleng),ACDNK(1:idleng),
//     $                AT*RADDEG,E,DF,IC,CTB(IC)*RADDEG,CTC(IC)                                   ;//     $                AT*RADDEG,E,DF,IC,CTB(IC)*RADDEG,CTC(IC)
//              ENDIF                                                                              ;//              ENDIF
// 245          FORMAT('ANAL: ANGL>',2I5,4(1X,A),I5,4(1X,A),I5,4(1X,A),                            ;// 245          FORMAT('ANAL: ANGL>',2I5,4(1X,A),I5,4(1X,A),I5,4(1X,A),
//     &               /3F15.6,I5,2F15.6)                                                          ;//     &               /3F15.6,I5,2F15.6)
// 246          FORMAT('ANAL: ANGL>',2I5,4(1X,A),I5,4(1X,A),I5,4(1X,A),                            ;// 246          FORMAT('ANAL: ANGL>',2I5,4(1X,A),I5,4(1X,A),I5,4(1X,A),
//     &               3F15.6,I5,2F15.6)                                                           ;//     &               3F15.6,I5,2F15.6)
//            ENDIF                                                                                ;//            ENDIF
//          }                                                                                      ;//          ENDIF
//        }                                                                                        ;//        ENDIF
///                                                                                                ;//C
          if(QECONTX) {                                                                            ;//        IF(QECONTX) THEN
            E=E*THIRD                                                                              ;//          E=E*THIRD
            ECONTX[I]=ECONTX[I]+E                                                                  ;//          ECONTX(I)=ECONTX(I)+E
            ECONTX[J]=ECONTX[J]+E                                                                  ;//          ECONTX(J)=ECONTX(J)+E
            ECONTX[K]=ECONTX[K]+E                                                                  ;//          ECONTX(K)=ECONTX(K)+E
          }                                                                                        ;//        ENDIF
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        IF (.NOT. NOFORC) THEN
                                                                                                   ;//##ENDIF ! BLOCK
///                                                                                                ;//C
          if(ABS(CST) >= 0.999) {                                                                  ;//        IF(ABS(CST).GE.0.999) THEN
            ST2R=ONE/(ONE-CST*CST+SMALLV)                                                          ;//          ST2R=ONE/(ONE-CST*CST+SMALLV)
            STR=SQRT(ST2R)                                                                         ;//          STR=SQRT(ST2R)
            if(CTB[IC] <  PT001) {                                                                 ;//          IF(CTB(IC).LT.PT001) THEN
              DF=MINTWO*CTC[IC]*(ONE+DA*DA*SIXTH)                                                  ;//            DF=MINTWO*CTC(IC)*(ONE+DA*DA*SIXTH)
            } else if(PI-CTB[IC] <  PT001) {                                                       ;//          ELSE IF(PI-CTB(IC).LT.PT001) THEN
              DF=TWO*CTC[IC]*(ONE+DA*DA*SIXTH)                                                     ;//            DF=TWO*CTC(IC)*(ONE+DA*DA*SIXTH)
            } else {                                                                               ;//          ELSE
              DF=-DF*STR                                                                           ;//            DF=-DF*STR
            }                                                                                      ;//          ENDIF
          } else {                                                                                 ;//        ELSE
            ST2R=ONE/(ONE-CST*CST)                                                                 ;//          ST2R=ONE/(ONE-CST*CST)
            STR=SQRT(ST2R)                                                                         ;//          STR=SQRT(ST2R)
            DF=-DF*STR                                                                             ;//          DF=-DF*STR
          }                                                                                        ;//        ENDIF
///                                                                                                ;//C
          DTXI=RIR*(DXJR-CST*DXIR)                                                                 ;//        DTXI=RIR*(DXJR-CST*DXIR)
          DTXJ=RJR*(DXIR-CST*DXJR)                                                                 ;//        DTXJ=RJR*(DXIR-CST*DXJR)
          DTYI=RIR*(DYJR-CST*DYIR)                                                                 ;//        DTYI=RIR*(DYJR-CST*DYIR)
          DTYJ=RJR*(DYIR-CST*DYJR)                                                                 ;//        DTYJ=RJR*(DYIR-CST*DYJR)
          DTZI=RIR*(DZJR-CST*DZIR)                                                                 ;//        DTZI=RIR*(DZJR-CST*DZIR)
          DTZJ=RJR*(DZIR-CST*DZJR)                                                                 ;//        DTZJ=RJR*(DZIR-CST*DZJR)
///                                                                                                ;//C
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        IF(QDOCK) THEN
                                                                                                   ;//          DFX=DF*DTXI
                                                                                                   ;//          DGX=DF*DTXJ
                                                                                                   ;//          DX(I)=DX(I)+DFX*DOCFI
                                                                                                   ;//          DX(K)=DX(K)+DGX*DOCFK
                                                                                                   ;//          DX(J)=DX(J)-DFX*DOCFJ-DGX*DOCFJ1
                                                                                                   ;//C
                                                                                                   ;//          DFY=DF*DTYI
                                                                                                   ;//          DGY=DF*DTYJ
                                                                                                   ;//          DY(I)=DY(I)+DFY*DOCFI
                                                                                                   ;//          DY(K)=DY(K)+DGY*DOCFK
                                                                                                   ;//          DY(J)=DY(J)-DFY*DOCFJ-DGY*DOCFJ1
                                                                                                   ;//C
                                                                                                   ;//          DFZ=DF*DTZI
                                                                                                   ;//          DGZ=DF*DTZJ
                                                                                                   ;//          DZ(I)=DZ(I)+DFZ*DOCFI
                                                                                                   ;//          DZ(K)=DZ(K)+DGZ*DOCFK
                                                                                                   ;//          DZ(J)=DZ(J)-DFZ*DOCFJ-DGZ*DOCFJ1
                                                                                                   ;//        ELSE
                                                                                                   ;//##ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//
            DFX=DF*DTXI                                                                            ;//          DFX=DF*DTXI
            DGX=DF*DTXJ                                                                            ;//          DGX=DF*DTXJ
            DX[I]=DX[I]+DFX                                                                        ;//          DX(I)=DX(I)+DFX
            DX[K]=DX[K]+DGX                                                                        ;//          DX(K)=DX(K)+DGX
            DX[J]=DX[J]-DFX-DGX                                                                    ;//          DX(J)=DX(J)-DFX-DGX
///                                                                                                ;//C
            DFY=DF*DTYI                                                                            ;//          DFY=DF*DTYI
            DGY=DF*DTYJ                                                                            ;//          DGY=DF*DTYJ
            DY[I]=DY[I]+DFY                                                                        ;//          DY(I)=DY(I)+DFY
            DY[K]=DY[K]+DGY                                                                        ;//          DY(K)=DY(K)+DGY
            DY[J]=DY[J]-DFY-DGY                                                                    ;//          DY(J)=DY(J)-DFY-DGY
///                                                                                                ;//C
            DFZ=DF*DTZI                                                                            ;//          DFZ=DF*DTZI
            DGZ=DF*DTZJ                                                                            ;//          DGZ=DF*DTZJ
            DZ[I]=DZ[I]+DFZ                                                                        ;//          DZ(I)=DZ(I)+DFZ
            DZ[K]=DZ[K]+DGZ                                                                        ;//          DZ(K)=DZ(K)+DGZ
            DZ[J]=DZ[J]-DFZ-DGZ                                                                    ;//          DZ(J)=DZ(J)-DFZ-DGZ
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//##IF LDM
                                                                                                   ;//##IF LRST
                                                                                                   ;//          IF(RSTP.AND. .NOT. QNOAN)THEN
                                                                                                   ;//           IF( (IBL.EQ.1 .AND. JBL.GE.LSTRT) .or.
                                                                                                   ;//     &         (IBL.GE.LSTRT .AND. IBL.EQ.JBL) ) THEN
                                                                                                   ;//             CALL ASUMR8(DFORG*DTXI,HEAP(ENVDX),I)
                                                                                                   ;//             CALL ASUMR8(DFORG*DTYI,HEAP(ENVDY),I)
                                                                                                   ;//             CALL ASUMR8(DFORG*DTZI,HEAP(ENVDZ),I)
                                                                                                   ;//C
                                                                                                   ;//             CALL ASUMR8(DFORG*DTXJ,HEAP(ENVDX),K)
                                                                                                   ;//             CALL ASUMR8(DFORG*DTYJ,HEAP(ENVDY),K)
                                                                                                   ;//             CALL ASUMR8(DFORG*DTZJ,HEAP(ENVDZ),K)
                                                                                                   ;//C
                                                                                                   ;//             CALL ASUMR8(-DFORG*(DTXI+DTXJ),HEAP(ENVDX),J)
                                                                                                   ;//             CALL ASUMR8(-DFORG*(DTYI+DTYJ),HEAP(ENVDY),J)
                                                                                                   ;//             CALL ASUMR8(-DFORG*(DTZI+DTZJ),HEAP(ENVDZ),J)
                                                                                                   ;//           ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! LRST
                                                                                                   ;//##ENDIF  ! LDM
///                                                                                                ;//C
                                                                                                   ;//##IF FOURD (4dang3)
                                                                                                   ;//        IF(DIM4ON(2).EQ.1) THEN
                                                                                                   ;//          DTFDI=RIR*(DFDIMJR-CST*DFDIMIR)
                                                                                                   ;//          DTFDJ=RJR*(DFDIMIR-CST*DFDIMJR)
                                                                                                   ;//          DFFD=DF*DTFDI
                                                                                                   ;//          DGFD=DF*DTFDJ
                                                                                                   ;//          DFDIM(I)=DFDIM(I)+DFFD
                                                                                                   ;//          DFDIM(K)=DFDIM(K)+DGFD
                                                                                                   ;//          DFDIM(J)=DFDIM(J)-DFFD-DGFD
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF (4dang3)
///                                                                                                ;//C
                                                                                                   ;//##IF IPRESS
                                                                                                   ;//        IF(QIPRSS) THEN
                                                                                                   ;//         PVIR(I)=PVIR(I)+DFX*DXI+DFY*DYI+DFZ*DZI
                                                                                                   ;//         PVIR(J)=PVIR(J)+DFX*DXI+DFY*DYI+DFZ*DZI+DGX*DXJ+DGY*DYJ+DGZ*DZJ
                                                                                                   ;//         PVIR(K)=PVIR(K)+DGX*DXJ+DGY*DYJ+DGZ*DZJ
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
///       SECOND DERIVATIVE PART                                                                   ;//C       SECOND DERIVATIVE PART
          if (QSECD) {                                                                             ;//        IF (QSECD) THEN
///                                                                                                ;//C
            DDF=TWO*DDF*ST2R*(ONE-CST*STR*DA)                                                      ;//          DDF=TWO*DDF*ST2R*(ONE-CST*STR*DA)
///                                                                                                ;//C
            RI2RF=RIR*RIR*DF                                                                       ;//          RI2RF=RIR*RIR*DF
            RJ2RF=RJR*RJR*DF                                                                       ;//          RJ2RF=RJR*RJR*DF
            RIRJF=RIR*RJR*DF                                                                       ;//          RIRJF=RIR*RJR*DF
///                                                                                                ;//C
            DDXIXI=RI2RF*(CST*(DXIR*DXIR-ONE)-TWO*DXI*DTXI)+DDF*DTXI*DTXI                          ;//          DDXIXI=RI2RF*(CST*(DXIR*DXIR-ONE)-TWO*DXI*DTXI)+DDF*DTXI*DTXI
            DDYIYI=RI2RF*(CST*(DYIR*DYIR-ONE)-TWO*DYI*DTYI)+DDF*DTYI*DTYI                          ;//          DDYIYI=RI2RF*(CST*(DYIR*DYIR-ONE)-TWO*DYI*DTYI)+DDF*DTYI*DTYI
            DDZIZI=RI2RF*(CST*(DZIR*DZIR-ONE)-TWO*DZI*DTZI)+DDF*DTZI*DTZI                          ;//          DDZIZI=RI2RF*(CST*(DZIR*DZIR-ONE)-TWO*DZI*DTZI)+DDF*DTZI*DTZI
            DDXJXJ=RJ2RF*(CST*(DXJR*DXJR-ONE)-TWO*DXJ*DTXJ)+DDF*DTXJ*DTXJ                          ;//          DDXJXJ=RJ2RF*(CST*(DXJR*DXJR-ONE)-TWO*DXJ*DTXJ)+DDF*DTXJ*DTXJ
            DDYJYJ=RJ2RF*(CST*(DYJR*DYJR-ONE)-TWO*DYJ*DTYJ)+DDF*DTYJ*DTYJ                          ;//          DDYJYJ=RJ2RF*(CST*(DYJR*DYJR-ONE)-TWO*DYJ*DTYJ)+DDF*DTYJ*DTYJ
            DDZJZJ=RJ2RF*(CST*(DZJR*DZJR-ONE)-TWO*DZJ*DTZJ)+DDF*DTZJ*DTZJ                          ;//          DDZJZJ=RJ2RF*(CST*(DZJR*DZJR-ONE)-TWO*DZJ*DTZJ)+DDF*DTZJ*DTZJ
///                                                                                                ;//C
            DDXIXJ=RIRJF*(ONE-DXIR*DXIR-DXJR*DXJR+CST*DXIR*DXJR)+DDF                                //          DDXIXJ=RIRJF*(ONE-DXIR*DXIR-DXJR*DXJR+CST*DXIR*DXJR)+DDF
              *DTXI*DTXJ                                                                           ;//     1      *DTXI*DTXJ
            DDYIYJ=RIRJF*(ONE-DYIR*DYIR-DYJR*DYJR+CST*DYIR*DYJR)+DDF                                //          DDYIYJ=RIRJF*(ONE-DYIR*DYIR-DYJR*DYJR+CST*DYIR*DYJR)+DDF
              *DTYI*DTYJ                                                                           ;//     1      *DTYI*DTYJ
            DDZIZJ=RIRJF*(ONE-DZIR*DZIR-DZJR*DZJR+CST*DZIR*DZJR)+DDF                                //          DDZIZJ=RIRJF*(ONE-DZIR*DZIR-DZJR*DZJR+CST*DZIR*DZJR)+DDF
              *DTZI*DTZJ                                                                           ;//     1      *DTZI*DTZJ
///                                                                                                ;//C
            DDXIYI=RI2RF*(CST*DXIR*DYIR-DXI*DTYI-DYI*DTXI)+DDF*DTXI*DTYI                           ;//          DDXIYI=RI2RF*(CST*DXIR*DYIR-DXI*DTYI-DYI*DTXI)+DDF*DTXI*DTYI
            DDXIZI=RI2RF*(CST*DXIR*DZIR-DXI*DTZI-DZI*DTXI)+DDF*DTXI*DTZI                           ;//          DDXIZI=RI2RF*(CST*DXIR*DZIR-DXI*DTZI-DZI*DTXI)+DDF*DTXI*DTZI
            DDYIZI=RI2RF*(CST*DYIR*DZIR-DYI*DTZI-DZI*DTYI)+DDF*DTYI*DTZI                           ;//          DDYIZI=RI2RF*(CST*DYIR*DZIR-DYI*DTZI-DZI*DTYI)+DDF*DTYI*DTZI
///                                                                                                ;//C
            DDXJYJ=RJ2RF*(CST*DXJR*DYJR-DXJ*DTYJ-DYJ*DTXJ)+DDF*DTXJ*DTYJ                           ;//          DDXJYJ=RJ2RF*(CST*DXJR*DYJR-DXJ*DTYJ-DYJ*DTXJ)+DDF*DTXJ*DTYJ
            DDXJZJ=RJ2RF*(CST*DXJR*DZJR-DXJ*DTZJ-DZJ*DTXJ)+DDF*DTXJ*DTZJ                           ;//          DDXJZJ=RJ2RF*(CST*DXJR*DZJR-DXJ*DTZJ-DZJ*DTXJ)+DDF*DTXJ*DTZJ
            DDYJZJ=RJ2RF*(CST*DYJR*DZJR-DYJ*DTZJ-DZJ*DTYJ)+DDF*DTYJ*DTZJ                           ;//          DDYJZJ=RJ2RF*(CST*DYJR*DZJR-DYJ*DTZJ-DZJ*DTYJ)+DDF*DTYJ*DTZJ
///                                                                                                ;//C
            A=DXIR*DYIR+DXJR*DYJR                                                                  ;//          A=DXIR*DYIR+DXJR*DYJR
            DDXIYJ=RIRJF*(CST*DXIR*DYJR-A)+DDF*DTXI*DTYJ                                           ;//          DDXIYJ=RIRJF*(CST*DXIR*DYJR-A)+DDF*DTXI*DTYJ
            DDYIXJ=RIRJF*(CST*DYIR*DXJR-A)+DDF*DTYI*DTXJ                                           ;//          DDYIXJ=RIRJF*(CST*DYIR*DXJR-A)+DDF*DTYI*DTXJ
            A=DXIR*DZIR+DXJR*DZJR                                                                  ;//          A=DXIR*DZIR+DXJR*DZJR
            DDXIZJ=RIRJF*(CST*DXIR*DZJR-A)+DDF*DTXI*DTZJ                                           ;//          DDXIZJ=RIRJF*(CST*DXIR*DZJR-A)+DDF*DTXI*DTZJ
            DDZIXJ=RIRJF*(CST*DZIR*DXJR-A)+DDF*DTZI*DTXJ                                           ;//          DDZIXJ=RIRJF*(CST*DZIR*DXJR-A)+DDF*DTZI*DTXJ
            A=DYIR*DZIR+DYJR*DZJR                                                                  ;//          A=DYIR*DZIR+DYJR*DZJR
            DDYIZJ=RIRJF*(CST*DYIR*DZJR-A)+DDF*DTYI*DTZJ                                           ;//          DDYIZJ=RIRJF*(CST*DYIR*DZJR-A)+DDF*DTYI*DTZJ
            DDZIYJ=RIRJF*(CST*DZIR*DYJR-A)+DDF*DTZI*DTYJ                                           ;//          DDZIYJ=RIRJF*(CST*DZIR*DYJR-A)+DDF*DTZI*DTYJ
///                                                                                                ;//C
///                                                                                                ;//C
                                                                                                   ;//##IF MBOND
                                                                                                   ;//      IF (qMBSec) THEN
                                                                                                   ;//
                                                                                                   ;//         CALL MBSecA(heap(pDD1Sys(1)),heap(pDD1Vac(1)),I,J,K,
                                                                                                   ;//     &                   DDXIXI,DDYIYI,DDZIZI,DDXJXJ,DDYJYJ,DDZJZJ,
                                                                                                   ;//     &                   DDXIXJ,DDYIYJ,DDZIZJ,DDXIYI,DDXIZI,DDYIZI,
                                                                                                   ;//     &                   DDXJYJ,DDXJZJ,DDYJZJ,DDXIYJ,DDYIXJ,DDXIZJ,
                                                                                                   ;//     &                   DDZIXJ,DDYIZJ,DDZIYJ)
                                                                                                   ;//
                                                                                                   ;//      ELSE
                                                                                                   ;//##ENDIF ! MBOND
                                                                                                   ;//##IF DIMB
                                                                                                   ;//          IF(QCMPCT) THEN
                                                                                                   ;//             CALL EANCMP(I,J,K,DDXIXI,DDYIYI,DDZIZI,DDXJXJ,DDYJYJ,
                                                                                                   ;//     &                   DDZJZJ,DDXIXJ,DDYIYJ,DDZIZJ,DDXIYI,DDXIZI,
                                                                                                   ;//     &                   DDYIZI,DDXJYJ,DDXJZJ,DDYJZJ,DDXIYJ,DDYIXJ,
                                                                                                   ;//     &                   DDXIZJ,DDZIXJ,DDYIZJ,DDZIYJ,DD1,
                                                                                                   ;//     &                   HEAP(PINBCM),HEAP(PJNBCM))
                                                                                                   ;//          ELSE
                                                                                                   ;//##ENDIF ! DIMB
                                                                                                   ;//
            II=3*I-2                                                                               ;//          II=3*I-2
            JJ=3*J-2                                                                               ;//          JJ=3*J-2
            KK=3*K-2                                                                               ;//          KK=3*K-2
            IJTEST=(J <  I)                                                                        ;//          IJTEST=(J.LT.I)
            IKTEST=(K <  I)                                                                        ;//          IKTEST=(K.LT.I)
            JKTEST=(K <  J)                                                                        ;//          JKTEST=(K.LT.J)
///                                                                                                ;//C
            if (IKTEST) {                                                                          ;//          IF (IKTEST) THEN
              IADD=IUPT[KK]+II                                                                     ;//            IADD=IUPT(KK)+II
              DD1[IADD]=DD1[IADD]+DDXIXJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDXIXJ
              IADD=IUPT[KK+1]+II+1                                                                 ;//            IADD=IUPT(KK+1)+II+1
              DD1[IADD]=DD1[IADD]+DDYIYJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDYIYJ
              IADD=IUPT[KK+2]+II+2                                                                 ;//            IADD=IUPT(KK+2)+II+2
              DD1[IADD]=DD1[IADD]+DDZIZJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDZIZJ
              IADD=IUPT[KK]+II+1                                                                   ;//            IADD=IUPT(KK)+II+1
              DD1[IADD]=DD1[IADD]+DDYIXJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDYIXJ
              IADD=IUPT[KK+1]+II                                                                   ;//            IADD=IUPT(KK+1)+II
              DD1[IADD]=DD1[IADD]+DDXIYJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDXIYJ
              IADD=IUPT[KK]+II+2                                                                   ;//            IADD=IUPT(KK)+II+2
              DD1[IADD]=DD1[IADD]+DDZIXJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDZIXJ
              IADD=IUPT[KK+2]+II                                                                   ;//            IADD=IUPT(KK+2)+II
              DD1[IADD]=DD1[IADD]+DDXIZJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDXIZJ
              IADD=IUPT[KK+1]+II+2                                                                 ;//            IADD=IUPT(KK+1)+II+2
              DD1[IADD]=DD1[IADD]+DDZIYJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDZIYJ
              IADD=IUPT[KK+2]+II+1                                                                 ;//            IADD=IUPT(KK+2)+II+1
              DD1[IADD]=DD1[IADD]+DDYIZJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDYIZJ
            } else {                                                                               ;//          ELSE
              IADD=IUPT[II]+KK                                                                     ;//            IADD=IUPT(II)+KK
              DD1[IADD]=DD1[IADD]+DDXIXJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDXIXJ
              IADD=IUPT[II+1]+KK+1                                                                 ;//            IADD=IUPT(II+1)+KK+1
              DD1[IADD]=DD1[IADD]+DDYIYJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDYIYJ
              IADD=IUPT[II+2]+KK+2                                                                 ;//            IADD=IUPT(II+2)+KK+2
              DD1[IADD]=DD1[IADD]+DDZIZJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDZIZJ
              IADD=IUPT[II+1]+KK                                                                   ;//            IADD=IUPT(II+1)+KK
              DD1[IADD]=DD1[IADD]+DDYIXJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDYIXJ
              IADD=IUPT[II]+KK+1                                                                   ;//            IADD=IUPT(II)+KK+1
              DD1[IADD]=DD1[IADD]+DDXIYJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDXIYJ
              IADD=IUPT[II+2]+KK                                                                   ;//            IADD=IUPT(II+2)+KK
              DD1[IADD]=DD1[IADD]+DDZIXJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDZIXJ
              IADD=IUPT[II]+KK+2                                                                   ;//            IADD=IUPT(II)+KK+2
              DD1[IADD]=DD1[IADD]+DDXIZJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDXIZJ
              IADD=IUPT[II+2]+KK+1                                                                 ;//            IADD=IUPT(II+2)+KK+1
              DD1[IADD]=DD1[IADD]+DDZIYJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDZIYJ
              IADD=IUPT[II+1]+KK+2                                                                 ;//            IADD=IUPT(II+1)+KK+2
              DD1[IADD]=DD1[IADD]+DDYIZJ                                                           ;//            DD1(IADD)=DD1(IADD)+DDYIZJ
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if (IJTEST) {                                                                          ;//          IF (IJTEST) THEN
              IADD=IUPT[JJ]+II                                                                     ;//            IADD=IUPT(JJ)+II
              DD1[IADD]=DD1[IADD]-DDXIXJ-DDXIXI                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIXJ-DDXIXI
              IADD=IUPT[JJ+1]+II+1                                                                 ;//            IADD=IUPT(JJ+1)+II+1
              DD1[IADD]=DD1[IADD]-DDYIYJ-DDYIYI                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIYJ-DDYIYI
              IADD=IUPT[JJ+2]+II+2                                                                 ;//            IADD=IUPT(JJ+2)+II+2
              DD1[IADD]=DD1[IADD]-DDZIZJ-DDZIZI                                                    ;//            DD1(IADD)=DD1(IADD)-DDZIZJ-DDZIZI
              IADD=IUPT[JJ]+II+1                                                                   ;//            IADD=IUPT(JJ)+II+1
              DD1[IADD]=DD1[IADD]-DDXIYI-DDYIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIYI-DDYIXJ
              IADD=IUPT[JJ+1]+II                                                                   ;//            IADD=IUPT(JJ+1)+II
              DD1[IADD]=DD1[IADD]-DDXIYI-DDXIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIYI-DDXIYJ
              IADD=IUPT[JJ]+II+2                                                                   ;//            IADD=IUPT(JJ)+II+2
              DD1[IADD]=DD1[IADD]-DDXIZI-DDZIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIZI-DDZIXJ
              IADD=IUPT[JJ+2]+II                                                                   ;//            IADD=IUPT(JJ+2)+II
              DD1[IADD]=DD1[IADD]-DDXIZI-DDXIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIZI-DDXIZJ
              IADD=IUPT[JJ+1]+II+2                                                                 ;//            IADD=IUPT(JJ+1)+II+2
              DD1[IADD]=DD1[IADD]-DDYIZI-DDZIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIZI-DDZIYJ
              IADD=IUPT[JJ+2]+II+1                                                                 ;//            IADD=IUPT(JJ+2)+II+1
              DD1[IADD]=DD1[IADD]-DDYIZI-DDYIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIZI-DDYIZJ
            } else {                                                                               ;//          ELSE
              IADD=IUPT[II]+JJ                                                                     ;//            IADD=IUPT(II)+JJ
              DD1[IADD]=DD1[IADD]-DDXIXJ-DDXIXI                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIXJ-DDXIXI
              IADD=IUPT[II+1]+JJ+1                                                                 ;//            IADD=IUPT(II+1)+JJ+1
              DD1[IADD]=DD1[IADD]-DDYIYJ-DDYIYI                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIYJ-DDYIYI
              IADD=IUPT[II+2]+JJ+2                                                                 ;//            IADD=IUPT(II+2)+JJ+2
              DD1[IADD]=DD1[IADD]-DDZIZJ-DDZIZI                                                    ;//            DD1(IADD)=DD1(IADD)-DDZIZJ-DDZIZI
              IADD=IUPT[II+1]+JJ                                                                   ;//            IADD=IUPT(II+1)+JJ
              DD1[IADD]=DD1[IADD]-DDXIYI-DDYIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIYI-DDYIXJ
              IADD=IUPT[II]+JJ+1                                                                   ;//            IADD=IUPT(II)+JJ+1
              DD1[IADD]=DD1[IADD]-DDXIYI-DDXIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIYI-DDXIYJ
              IADD=IUPT[II+2]+JJ                                                                   ;//            IADD=IUPT(II+2)+JJ
              DD1[IADD]=DD1[IADD]-DDXIZI-DDZIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIZI-DDZIXJ
              IADD=IUPT[II]+JJ+2                                                                   ;//            IADD=IUPT(II)+JJ+2
              DD1[IADD]=DD1[IADD]-DDXIZI-DDXIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIZI-DDXIZJ
              IADD=IUPT[II+2]+JJ+1                                                                 ;//            IADD=IUPT(II+2)+JJ+1
              DD1[IADD]=DD1[IADD]-DDYIZI-DDZIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIZI-DDZIYJ
              IADD=IUPT[II+1]+JJ+2                                                                 ;//            IADD=IUPT(II+1)+JJ+2
              DD1[IADD]=DD1[IADD]-DDYIZI-DDYIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIZI-DDYIZJ
            }                                                                                      ;//          ENDIF
            if (JKTEST) {                                                                          ;//          IF (JKTEST) THEN
              IADD=IUPT[KK]+JJ                                                                     ;//            IADD=IUPT(KK)+JJ
              DD1[IADD]=DD1[IADD]-DDXIXJ-DDXJXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIXJ-DDXJXJ
              IADD=IUPT[KK+1]+JJ+1                                                                 ;//            IADD=IUPT(KK+1)+JJ+1
              DD1[IADD]=DD1[IADD]-DDYIYJ-DDYJYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIYJ-DDYJYJ
              IADD=IUPT[KK+2]+JJ+2                                                                 ;//            IADD=IUPT(KK+2)+JJ+2
              DD1[IADD]=DD1[IADD]-DDZIZJ-DDZJZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDZIZJ-DDZJZJ
              IADD=IUPT[KK]+JJ+1                                                                   ;//            IADD=IUPT(KK)+JJ+1
              DD1[IADD]=DD1[IADD]-DDXJYJ-DDYIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJYJ-DDYIXJ
              IADD=IUPT[KK+1]+JJ                                                                   ;//            IADD=IUPT(KK+1)+JJ
              DD1[IADD]=DD1[IADD]-DDXJYJ-DDXIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJYJ-DDXIYJ
              IADD=IUPT[KK]+JJ+2                                                                   ;//            IADD=IUPT(KK)+JJ+2
              DD1[IADD]=DD1[IADD]-DDXJZJ-DDZIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJZJ-DDZIXJ
              IADD=IUPT[KK+2]+JJ                                                                   ;//            IADD=IUPT(KK+2)+JJ
              DD1[IADD]=DD1[IADD]-DDXJZJ-DDXIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJZJ-DDXIZJ
              IADD=IUPT[KK+1]+JJ+2                                                                 ;//            IADD=IUPT(KK+1)+JJ+2
              DD1[IADD]=DD1[IADD]-DDYJZJ-DDZIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYJZJ-DDZIYJ
              IADD=IUPT[KK+2]+JJ+1                                                                 ;//            IADD=IUPT(KK+2)+JJ+1
              DD1[IADD]=DD1[IADD]-DDYJZJ-DDYIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYJZJ-DDYIZJ
            } else {                                                                               ;//          ELSE
              IADD=IUPT[JJ]+KK                                                                     ;//            IADD=IUPT(JJ)+KK
              DD1[IADD]=DD1[IADD]-DDXIXJ-DDXJXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXIXJ-DDXJXJ
              IADD=IUPT[JJ+1]+KK+1                                                                 ;//            IADD=IUPT(JJ+1)+KK+1
              DD1[IADD]=DD1[IADD]-DDYIYJ-DDYJYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYIYJ-DDYJYJ
              IADD=IUPT[JJ+2]+KK+2                                                                 ;//            IADD=IUPT(JJ+2)+KK+2
              DD1[IADD]=DD1[IADD]-DDZIZJ-DDZJZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDZIZJ-DDZJZJ
              IADD=IUPT[JJ+1]+KK                                                                   ;//            IADD=IUPT(JJ+1)+KK
              DD1[IADD]=DD1[IADD]-DDXJYJ-DDYIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJYJ-DDYIXJ
              IADD=IUPT[JJ]+KK+1                                                                   ;//            IADD=IUPT(JJ)+KK+1
              DD1[IADD]=DD1[IADD]-DDXJYJ-DDXIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJYJ-DDXIYJ
              IADD=IUPT[JJ+2]+KK                                                                   ;//            IADD=IUPT(JJ+2)+KK
              DD1[IADD]=DD1[IADD]-DDXJZJ-DDZIXJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJZJ-DDZIXJ
              IADD=IUPT[JJ]+KK+2                                                                   ;//            IADD=IUPT(JJ)+KK+2
              DD1[IADD]=DD1[IADD]-DDXJZJ-DDXIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDXJZJ-DDXIZJ
              IADD=IUPT[JJ+2]+KK+1                                                                 ;//            IADD=IUPT(JJ+2)+KK+1
              DD1[IADD]=DD1[IADD]-DDYJZJ-DDZIYJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYJZJ-DDZIYJ
              IADD=IUPT[JJ+1]+KK+2                                                                 ;//            IADD=IUPT(JJ+1)+KK+2
              DD1[IADD]=DD1[IADD]-DDYJZJ-DDYIZJ                                                    ;//            DD1(IADD)=DD1(IADD)-DDYJZJ-DDYIZJ
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
///         DIAGONAL TERMS                                                                         ;//C         DIAGONAL TERMS
            IADD=IUPT[II]+II                                                                       ;//          IADD=IUPT(II)+II
            DD1[IADD]=DD1[IADD]+DDXIXI                                                             ;//          DD1(IADD)=DD1(IADD)+DDXIXI
            IADD=IUPT[II+1]+II+1                                                                   ;//          IADD=IUPT(II+1)+II+1
            DD1[IADD]=DD1[IADD]+DDYIYI                                                             ;//          DD1(IADD)=DD1(IADD)+DDYIYI
            IADD=IUPT[II+2]+II+2                                                                   ;//          IADD=IUPT(II+2)+II+2
            DD1[IADD]=DD1[IADD]+DDZIZI                                                             ;//          DD1(IADD)=DD1(IADD)+DDZIZI
            IADD=IUPT[II]+II+1                                                                     ;//          IADD=IUPT(II)+II+1
            DD1[IADD]=DD1[IADD]+DDXIYI                                                             ;//          DD1(IADD)=DD1(IADD)+DDXIYI
            IADD=IUPT[II]+II+2                                                                     ;//          IADD=IUPT(II)+II+2
            DD1[IADD]=DD1[IADD]+DDXIZI                                                             ;//          DD1(IADD)=DD1(IADD)+DDXIZI
            IADD=IUPT[II+1]+II+2                                                                   ;//          IADD=IUPT(II+1)+II+2
            DD1[IADD]=DD1[IADD]+DDYIZI                                                             ;//          DD1(IADD)=DD1(IADD)+DDYIZI
///                                                                                                ;//C
            IADD=IUPT[KK]+KK                                                                       ;//          IADD=IUPT(KK)+KK
            DD1[IADD]=DD1[IADD]+DDXJXJ                                                             ;//          DD1(IADD)=DD1(IADD)+DDXJXJ
            IADD=IUPT[KK+1]+KK+1                                                                   ;//          IADD=IUPT(KK+1)+KK+1
            DD1[IADD]=DD1[IADD]+DDYJYJ                                                             ;//          DD1(IADD)=DD1(IADD)+DDYJYJ
            IADD=IUPT[KK+2]+KK+2                                                                   ;//          IADD=IUPT(KK+2)+KK+2
            DD1[IADD]=DD1[IADD]+DDZJZJ                                                             ;//          DD1(IADD)=DD1(IADD)+DDZJZJ
            IADD=IUPT[KK]+KK+1                                                                     ;//          IADD=IUPT(KK)+KK+1
            DD1[IADD]=DD1[IADD]+DDXJYJ                                                             ;//          DD1(IADD)=DD1(IADD)+DDXJYJ
            IADD=IUPT[KK]+KK+2                                                                     ;//          IADD=IUPT(KK)+KK+2
            DD1[IADD]=DD1[IADD]+DDXJZJ                                                             ;//          DD1(IADD)=DD1(IADD)+DDXJZJ
            IADD=IUPT[KK+1]+KK+2                                                                   ;//          IADD=IUPT(KK+1)+KK+2
            DD1[IADD]=DD1[IADD]+DDYJZJ                                                             ;//          DD1(IADD)=DD1(IADD)+DDYJZJ
///                                                                                                ;//C
            IADD=IUPT[JJ]+JJ                                                                       ;//          IADD=IUPT(JJ)+JJ
            DD1[IADD]=DD1[IADD]+DDXIXI+DDXJXJ+DDXIXJ+DDXIXJ                                        ;//          DD1(IADD)=DD1(IADD)+DDXIXI+DDXJXJ+DDXIXJ+DDXIXJ
            IADD=IUPT[JJ+1]+JJ+1                                                                   ;//          IADD=IUPT(JJ+1)+JJ+1
            DD1[IADD]=DD1[IADD]+DDYIYI+DDYJYJ+DDYIYJ+DDYIYJ                                        ;//          DD1(IADD)=DD1(IADD)+DDYIYI+DDYJYJ+DDYIYJ+DDYIYJ
            IADD=IUPT[JJ+2]+JJ+2                                                                   ;//          IADD=IUPT(JJ+2)+JJ+2
            DD1[IADD]=DD1[IADD]+DDZIZI+DDZJZJ+DDZIZJ+DDZIZJ                                        ;//          DD1(IADD)=DD1(IADD)+DDZIZI+DDZJZJ+DDZIZJ+DDZIZJ
            IADD=IUPT[JJ]+JJ+1                                                                     ;//          IADD=IUPT(JJ)+JJ+1
            DD1[IADD]=DD1[IADD]+DDXIYI+DDXJYJ+DDXIYJ+DDYIXJ                                        ;//          DD1(IADD)=DD1(IADD)+DDXIYI+DDXJYJ+DDXIYJ+DDYIXJ
            IADD=IUPT[JJ]+JJ+2                                                                     ;//          IADD=IUPT(JJ)+JJ+2
            DD1[IADD]=DD1[IADD]+DDXIZI+DDXJZJ+DDXIZJ+DDZIXJ                                        ;//          DD1(IADD)=DD1(IADD)+DDXIZI+DDXJZJ+DDXIZJ+DDZIXJ
            IADD=IUPT[JJ+1]+JJ+2                                                                   ;//          IADD=IUPT(JJ+1)+JJ+2
            DD1[IADD]=DD1[IADD]+DDYIZI+DDYJZJ+DDYIZJ+DDZIYJ                                        ;//          DD1(IADD)=DD1(IADD)+DDYIZI+DDYJZJ+DDYIZJ+DDZIYJ
                                                                                                   ;//##IF DIMB
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF ! DIMB
                                                                                                   ;//##IF MBOND
                                                                                                   ;//      ENDIF
                                                                                                   ;//##ENDIF ! MBOND
                                                                                                   ;//
          }                                                                                        ;//        ENDIF
                                                                                                   ;//
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF ! BLOCK
///                                                                                                ;//C
        }                                                                                          ;//   20 CONTINUE
///                                                                                                ;//C
//      IF(NWARN.GT.5 .AND. WRNLEV.GE.2) WRITE(OUTU,30) NWARN                                      ;//      IF(NWARN.GT.5 .AND. WRNLEV.GE.2) WRITE(OUTU,30) NWARN
//   30 FORMAT(' TOTAL OF',I6,' WARNINGS FROM EANGLE')                                             ;//   30 FORMAT(' TOTAL OF',I6,' WARNINGS FROM EANGLE')
///                                                                                                ;//C
        return                                                                                     ;//      RETURN
    }                                                                                               //      END
//                                                                                                ;//
#region * option: GENETIC
/// GENETIC                                                                       
/// http://www.charmm.org/documentation/c32b2/galgor.html                         
///                                                                               
///     Galgor: Commands which deal with Genetic Algorithm and Monte Carlo.       
///                                                                               
/// # Michal Vieth,H. Daigler, C.L. Brooks III -Dec-15-1997 Initial release.      
///                                                                               
///      The commands described in this node are associated with genetic          
/// algorithm module for conformational searches and docking of small ligands to  
/// rigid proteins. The full description of the GA features is presented          
/// in the paper "Rational approach to docking. Optimizing the search algorithm"  
#endregion
    public void EPHI(double EP,int[] IP,int[] JP,int[] KP,int[] LP,int[] ICP,int NPHI,              //      SUBROUTINE EPHI(EP,IP,JP,KP,LP,ICP,NPHI,CPC,CPD,CPB,CPCOS,
                     double[] CPC,int[] CPD,double[] CPB,double[] CPCOS,double[] CPSIN,             //     &                CPSIN,DX,DY,DZ,X,Y,Z,QCPW,CPW,
                     double[] DX, double[] DY, double[] DZ, double[] X, double[] Y, double[] Z,     //     &                QECONTX,ECONTX,ICONHP,ISKP,DD1,IUPT,QSECD)
                     bool QCPW, double[] CPW, bool QECONTX, double[] ECONTX,
                     int ICONHP, int[] ISKP, double[] DD1, int[] IUPT, bool QSECD)
    {
///-----------------------------------------------------------------------                         ;//C-----------------------------------------------------------------------
///     CALCULATES EITHER TORSION ANGLES OR IMPROPER TORSION                                       ;//C     CALCULATES EITHER TORSION ANGLES OR IMPROPER TORSION
///     ANGLES AND THEIR ENERGIES. FIRST DERIVATIVES ARE ADDED                                     ;//C     ANGLES AND THEIR ENERGIES. FIRST DERIVATIVES ARE ADDED
///     TO DX, DY, DZ AND IF QSECD SECOND DERIVATIVES TO DD1.                                      ;//C     TO DX, DY, DZ AND IF QSECD SECOND DERIVATIVES TO DD1.
///                                                                                                ;//C
///     ENERGY TERMS ARE EXPRESSED AS A FUNCTION OF PHI TO AVOID                                   ;//C     ENERGY TERMS ARE EXPRESSED AS A FUNCTION OF PHI TO AVOID
///     ALL PROBLEMS AS DIHEDRALS BECOME PLANAR.                                                   ;//C     ALL PROBLEMS AS DIHEDRALS BECOME PLANAR.
///     THE FUNCTIONAL FORMS ARE:                                                                  ;//C     THE FUNCTIONAL FORMS ARE:
///                                                                                                ;//C
///     E = K*(1+COS(n*Phi-Phi0))                                                                  ;//C     E = K*(1+COS(n*Phi-Phi0))
///     Where:                                                                                     ;//C     Where:
///     n IS A POSITIVE INTEGER COS(n*Phi) AND SIN(n*Phi)                                          ;//C     n IS A POSITIVE INTEGER COS(n*Phi) AND SIN(n*Phi)
///            ARE CALCULATED BY RECURRENCE TO AVOID LIMITATION ON n                               ;//C            ARE CALCULATED BY RECURRENCE TO AVOID LIMITATION ON n
///     K IS THE FORCE CONSTANT IN kcal/mol                                                        ;//C     K IS THE FORCE CONSTANT IN kcal/mol
///     Phi0/n IS A MAXIMUM IN ENERGY.                                                             ;//C     Phi0/n IS A MAXIMUM IN ENERGY.
///                                                                                                ;//C
///     FOR IMPROPER DIHEDRALS, THE ENERGY TERM IS GIVEN BY:                                       ;//C     FOR IMPROPER DIHEDRALS, THE ENERGY TERM IS GIVEN BY:
///     E = K*(Phi-phi0)**2                                                                        ;//C     E = K*(Phi-phi0)**2
///     WHERE                                                                                      ;//C     WHERE
///     K IS THE FORCE CONSTANT IN kcal/mol/rad^2                                                  ;//C     K IS THE FORCE CONSTANT IN kcal/mol/rad^2
///     Phi0 IS THE MINIMUM IN ENERGY.                                                             ;//C     Phi0 IS THE MINIMUM IN ENERGY.
///                                                                                                ;//C
///     If QCPW is specified, then the improper becomes                                            ;//C     If QCPW is specified, then the improper becomes
///     a flat bottom with quadratic walls                                                         ;//C     a flat bottom with quadratic walls
///     E= K*( MAX(0, ABS(Phi-phi0)-cpw )**2                                                       ;//C     E= K*( MAX(0, ABS(Phi-phi0)-cpw )**2
///                                                                                                ;//C
///     THE CONSTRAINTS ON DIHEDRALS CAN USE EITHER FORM.                                          ;//C     THE CONSTRAINTS ON DIHEDRALS CAN USE EITHER FORM.
///     FOR THE COSINE FORM, Phi0 IS PhiMin*n+PI. THIS DONE AT THE                                 ;//C     FOR THE COSINE FORM, Phi0 IS PhiMin*n+PI. THIS DONE AT THE 
///     PARSER LEVEL SO THAT THE GIVEN PhiMin BECOMES A MINIMUM.                                   ;//C     PARSER LEVEL SO THAT THE GIVEN PhiMin BECOMES A MINIMUM.
///                                                                                                ;//C
///     The parameters of the routine are:                                                         ;//C     The parameters of the routine are:
///                                                                                                ;//C
///     EP          <- Dihedral Energy                                                             ;//C     EP          <- Dihedral Energy
///     IP,JP,KP,LP(phi) -> atom number for the members of dihedrals                               ;//C     IP,JP,KP,LP(phi) -> atom number for the members of dihedrals
///     ICP(phi)    -> parameter number associated with dihedral (type)                            ;//C     ICP(phi)    -> parameter number associated with dihedral (type)
///     NPHI        ->  Number of dihedrals                                                        ;//C     NPHI        ->  Number of dihedrals
///     CPC(type)   -> K value as explained above                                                  ;//C     CPC(type)   -> K value as explained above
///     CPD(type)   -> if 0: flags Improper form. if CPD<0 flag for multiple                       ;//C     CPD(type)   -> if 0: flags Improper form. if CPD<0 flag for multiple
///                    dihedral n=-CPD. if CPD>0 last term n=CPD.                                  ;//C                    dihedral n=-CPD. if CPD>0 last term n=CPD.
///     CPB(type)   -> Phi0 value as explained above (warnings) (radians)                          ;//C     CPB(type)   -> Phi0 value as explained above (warnings) (radians)
///     CPCOS(type) -> Cos(Phi0)                                                                   ;//C     CPCOS(type) -> Cos(Phi0)
///     CPSIN(type) -> Sin(Phi0)                                                                   ;//C     CPSIN(type) -> Sin(Phi0)
///     DX,DY,DZ(atom) <-> Force matrices                                                          ;//C     DX,DY,DZ(atom) <-> Force matrices
///     X,Y,Z(atom) -> Coordinate matrices                                                         ;//C     X,Y,Z(atom) -> Coordinate matrices
///     QCPW        -> Flag indicating flat bottom impropers are in use                            ;//C     QCPW        -> Flag indicating flat bottom impropers are in use
///     CPW(type)   -> half width of flat bottom potential (degrees)                               ;//C     CPW(type)   -> half width of flat bottom potential (degrees)
///     QECONT      -> Flag for energy/atom statistics                                             ;//C     QECONT      -> Flag for energy/atom statistics
///     ECONT(atom) <- matrix of energy/atom                                                       ;//C     ECONT(atom) <- matrix of energy/atom
///     ICONHP      -> Flag to activate the skipping procedure                                     ;//C     ICONHP      -> Flag to activate the skipping procedure
///     ISKP(phi)   -> matrix of flag to skip dihedral. Skip if ISKIP.ne.0                         ;//C     ISKP(phi)   -> matrix of flag to skip dihedral. Skip if ISKIP.ne.0
///     DD1        <-> Second derivative matrix (upper triangle)                                   ;//C     DD1        <-> Second derivative matrix (upper triangle)
///     IUPT(atom)  -> Index function for DD1                                                      ;//C     IUPT(atom)  -> Index function for DD1
///     QSECD       -> Second derivative flag.                                                     ;//C     QSECD       -> Second derivative flag.
///                                                                                                ;//C
///                                                                                                ;//C
///     By Bernard R. Brooks    1981                                                               ;//C     By Bernard R. Brooks    1981
///                                                                                                ;//C
///     New formulation and comments by:                                                           ;//C     New formulation and comments by:
///                                                                                                ;//C
///           Arnaud Blondel    1994                                                               ;//C           Arnaud Blondel    1994
///                                                                                                ;//C
//##INCLUDE '~/charmm_fcm/impnon.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/impnon.fcm'
//##INCLUDE '~/charmm_fcm/dimens.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/dimens.fcm'
//##INCLUDE '~/charmm_fcm/exfunc.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/exfunc.fcm'
//##INCLUDE '~/charmm_fcm/number.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/number.fcm'
//##INCLUDE '~/charmm_fcm/econt.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/econt.fcm'
//##INCLUDE '~/charmm_fcm/cnst.fcm'                                                                ;//##INCLUDE '~/charmm_fcm/cnst.fcm'
                                                                                                   ;//##IF PARALLEL
                                                                                                   ;//##INCLUDE '~/charmm_fcm/parallel.fcm'
                                                                                                   ;//      LOGICAL NOPARS
                                                                                                   ;//##ENDIF
///                                                                                                ;//C
//      REAL*8 EP                                                                                  ;//      REAL*8 EP
//      INTEGER IP(*),JP(*),KP(*),LP(*),ICP(*)                                                     ;//      INTEGER IP(*),JP(*),KP(*),LP(*),ICP(*)
//      INTEGER NPHI                                                                               ;//      INTEGER NPHI
//      REAL*8 CPC(*),CPB(*),CPCOS(*),CPSIN(*)                                                     ;//      REAL*8 CPC(*),CPB(*),CPCOS(*),CPSIN(*)
//      INTEGER CPD(*)                                                                             ;//      INTEGER CPD(*)
//      REAL*8 DX(*),DY(*),DZ(*),X(*),Y(*),Z(*)                                                    ;//      REAL*8 DX(*),DY(*),DZ(*),X(*),Y(*),Z(*)
//      LOGICAL QCPW                                                                               ;//      LOGICAL QCPW
//      REAL*8 CPW(*)                                                                              ;//      REAL*8 CPW(*)
//      LOGICAL QECONTX                                                                            ;//      LOGICAL QECONTX
//      REAL*8 ECONTX(*)                                                                           ;//      REAL*8 ECONTX(*)
//      INTEGER ICONHP                                                                             ;//      INTEGER ICONHP
//      INTEGER ISKP(*)                                                                            ;//      INTEGER ISKP(*)
//      REAL*8 DD1(*)                                                                              ;//      REAL*8 DD1(*)
//      INTEGER IUPT(*)                                                                            ;//      INTEGER IUPT(*)
//      LOGICAL QSECD                                                                              ;//      LOGICAL QSECD
///                                                                                                ;//C
//##INCLUDE '~/charmm_fcm/block.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/block.fcm'
//##INCLUDE '~/charmm_fcm/lambda.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/lambda.fcm'
//##INCLUDE '~/charmm_fcm/mbond.fcm'                                                               ;//##INCLUDE '~/charmm_fcm/mbond.fcm'
//##INCLUDE '~/charmm_fcm/dimb.fcm'                                                                ;//##INCLUDE '~/charmm_fcm/dimb.fcm'
//##INCLUDE '~/charmm_fcm/consta.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/consta.fcm'
//##INCLUDE '~/charmm_fcm/heap.fcm'                                                                ;//##INCLUDE '~/charmm_fcm/heap.fcm'
//##INCLUDE '~/charmm_fcm/stream.fcm'                                                              ;//##INCLUDE '~/charmm_fcm/stream.fcm'
///                                                                                                ;//C
        double CPBIC,E1,DF1,DDF1,FX,FY,FZ,GX,GY,GZ,HX,HY,HZ;                        DDF1=double.NaN;//      REAL*8 CPBIC,E1,DF1,DDF1,FX,FY,FZ,GX,GY,GZ,HX,HY,HZ
        double AX,AY,AZ,BX,BY,BZ,RA2,RB2,RA2R,RB2R,RG2,RG,RGR,RGR2                                 ;//      REAL*8 AX,AY,AZ,BX,BY,BZ,RA2,RB2,RA2R,RB2R,RG2,RG,RGR,RGR2
        double RABR,CP,AP,SP,E,DF,DDF,CA,SA,ARG,APR                                                ;//      REAL*8 RABR,CP,AP,SP,E,DF,DDF,CA,SA,ARG,APR
        double GAA,GBB,FG,HG,FGA,HGB,FGRG2,HGRG2,DFRG3                                             ;//      REAL*8 GAA,GBB,FG,HG,FGA,HGB,FGRG2,HGRG2,DFRG3
        double DFX,DFY,DFZ,DHX,DHY,DHZ,DGX,DGY,DGZ                                                 ;//      REAL*8 DFX,DFY,DFZ,DHX,DHY,DHZ,DGX,DGY,DGZ
        double DTFX,DTFY,DTFZ,DTHX,DTHY,DTHZ,DTGX,DTGY,DTGZ                                        ;//      REAL*8 DTFX,DTFY,DTFZ,DTHX,DTHY,DTHZ,DTGX,DTGY,DTGZ
        double GAFX,GAFY,GAFZ,GBHX,GBHY,GBHZ                                                       ;//      REAL*8 GAFX,GAFY,GAFZ,GBHX,GBHY,GBHZ
        double FAGX,FAGY,FAGZ,HBGX,HBGY,HBGZ                                                       ;//      REAL*8 FAGX,FAGY,FAGZ,HBGX,HBGY,HBGZ
        double[] DDFGH = new double[45]                                                            ;//      REAL*8 DDFGH(45)
        int     NWARN,NWARNX,IPHI,I,J,K,L,IC,IPER,NPER                                             ;//      INTEGER NWARN,NWARNX,IPHI,I,J,K,L,IC,IPER,NPER
        int     II,JJ,KK,LL,IADD                                                                   ;//      INTEGER II,JJ,KK,LL,IADD
        bool    IJTEST,IKTEST,ILTEST,JKTEST,JLTEST,KLTEST                                          ;//      LOGICAL IJTEST,IKTEST,ILTEST,JKTEST,JLTEST,KLTEST
        bool    LREP,NOCONS;//,QAFIRST                                                             ;//      LOGICAL LREP,NOCONS,QAFIRST
      //string      SIDDNI,RIDDNI,RESDNI,ACDNI,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ                          ;//      CHARACTER*8 SIDDNI,RIDDNI,RESDNI,ACDNI,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ
      //string      SIDDNK,RIDDNK,RESDNK,ACDNK,SIDDNL,RIDDNL,RESDNL,ACDNL                          ;//      CHARACTER*8 SIDDNK,RIDDNK,RESDNK,ACDNK,SIDDNL,RIDDNL,RESDNL,ACDNL
///                                                                                                ;//C
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//      INTEGER IBL, JBL, KKK, LLL, KDOC
                                                                                                   ;//      REAL*8  COEF, DOCFI, DOCFJ, DOCFK, DOCFJ1, DOCFK1, DOCFL
                                                                                                   ;//##IF LDM
                                                                                                   ;//      REAL*8 UNSCALE, FALPHA
                                                                                                   ;//      REAL*8 DFORG                        !##LRST
                                                                                                   ;//##ENDIF ! LDM
                                                                                                   ;//##ENDIF ! BLOCK
///                                                                                                ;//C
        double  RXMIN,RXMIN2                                                                       ;//      REAL*8  RXMIN,RXMIN2
        RXMIN=0.005; RXMIN2=0.000025                                                               ;//      PARAMETER (RXMIN=0.005D0,RXMIN2=0.000025D0)
///                                                                                                ;//C
                                                                                                   ;//##IF GENETIC
                                                                                                   ;//##INCLUDE '~/charmm_fcm/galgor.fcm'
                                                                                                   ;//      INTEGER First
                                                                                                   ;//      First = 1
                                                                                                   ;//      If(qGA_Ener) First = Int(EP)
                                                                                                   ;//##ENDIF
                                                                                                   ;//
        EP=ZERO                                                                                    ;//      EP=ZERO
        NOCONS=(ICONHP >  0)                                                                       ;//      NOCONS=(ICONHP.GT.0)
        if(NPHI <= 0) return                                                                       ;//      IF(NPHI.LE.0) RETURN
        NWARN=0                                                                                    ;//      NWARN=0
        NWARNX=0                                                                                   ;//      NWARNX=0
//      QAFIRST= true                                                                              ;//      QAFIRST=.TRUE.
///                                                                                                ;//C
                                                                                                   ;//##IF PARALLEL (paraphi)
                                                                                                   ;//##IF PARAFULL (parfphi)
                                                                                                   ;//      DO 10 IPHI=MYNODP,NPHI,NUMNOD
                                                                                                   ;//##ELIF PARASCAL (parfphi)
                                                                                                   ;//      NOPARS=(ICONHP.GE.0)
                                                                                                   ;//##IF GENETIC
                                                                                                   ;//      DO 10 IPHI=First,NPHI
                                                                                                   ;//##ELSE
                                                                                                   ;//      DO 10 IPHI=1,NPHI
                                                                                                   ;//##ENDIF
                                                                                                   ;//        IF(NOPARS) THEN
                                                                                                   ;//          II=JPBLOCK(IP(IPHI))
                                                                                                   ;//          JJ=JPBLOCK(JP(IPHI))
                                                                                                   ;//          KK=JPBLOCK(KP(IPHI))
                                                                                                   ;//          LL=JPBLOCK(LP(IPHI))
                                                                                                   ;//          IA=JJ
                                                                                                   ;//          IB=KK
                                                                                                   ;//          IF(IA.EQ.IB) IB=LL
                                                                                                   ;//          IF(IA.EQ.IB) IB=II
                                                                                                   ;//          IF(II.NE.IA .AND. II.NE.IB) THEN
                                                                                                   ;//            CALL PSADDTOCL(IP(IPHI),JPMAT(IA,IB))
                                                                                                   ;//          ENDIF
                                                                                                   ;//          IF(LL.NE.IA .AND. LL.NE.IB) THEN
                                                                                                   ;//            CALL PSADDTOCL(LP(IPHI),JPMAT(IA,IB))
                                                                                                   ;//          ENDIF
                                                                                                   ;//          IF(JPMAT(IA,IB).NE.MYNOD) GOTO 160
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF (parfphi)
                                                                                                   ;//##ELSE (paraphi)
                                                                                                   ;//##IF GENETIC
                                                                                                   ;//      DO 10 IPHI=First,NPHI
                                                                                                   ;//##ELSE
        for(IPHI=1; IPHI<=NPHI; IPHI++) {                                                          ;//      DO 10 IPHI=1,NPHI
                                                                                                   ;//##ENDIF
                                                                                                   ;//##ENDIF (paraphi)
///                                                                                                ;//C
          I=IP[IPHI]                                                                               ;//        I=IP(IPHI)
          if(NOCONS) {                                                                             ;//        IF(NOCONS) THEN
            if(ISKP[IPHI] != 0) goto goto160                                                       ;//          IF(ISKP(IPHI).NE.0) GOTO 160
          }                                                                                        ;//        ENDIF
          J=JP[IPHI]                                                                               ;//        J=JP(IPHI)
          K=KP[IPHI]                                                                               ;//        K=KP(IPHI)
          L=LP[IPHI]                                                                               ;//        L=LP(IPHI)
          IC=ICP[IPHI]                                                                             ;//        IC=ICP(IPHI)
          if(IC == 0) goto goto160                                                                 ;//        IF(IC.EQ.0) GOTO 160
/// F=Ri-Rj, G=Rj-Rk, H-Rl-Rk.                                                                     ;//C F=Ri-Rj, G=Rj-Rk, H-Rl-Rk.
          FX=X[I]-X[J]                                                                             ;//        FX=X(I)-X(J)
          FY=Y[I]-Y[J]                                                                             ;//        FY=Y(I)-Y(J)
          FZ=Z[I]-Z[J]                                                                             ;//        FZ=Z(I)-Z(J)
          GX=X[J]-X[K]                                                                             ;//        GX=X(J)-X(K)
          GY=Y[J]-Y[K]                                                                             ;//        GY=Y(J)-Y(K)
          GZ=Z[J]-Z[K]                                                                             ;//        GZ=Z(J)-Z(K)
          HX=X[L]-X[K]                                                                             ;//        HX=X(L)-X(K)
          HY=Y[L]-Y[K]                                                                             ;//        HY=Y(L)-Y(K)
          HZ=Z[L]-Z[K]                                                                             ;//        HZ=Z(L)-Z(K)
/// A=F^G, B=H^G                                                                                   ;//C A=F^G, B=H^G
          AX=FY*GZ-FZ*GY                                                                           ;//        AX=FY*GZ-FZ*GY
          AY=FZ*GX-FX*GZ                                                                           ;//        AY=FZ*GX-FX*GZ
          AZ=FX*GY-FY*GX                                                                           ;//        AZ=FX*GY-FY*GX
          BX=HY*GZ-HZ*GY                                                                           ;//        BX=HY*GZ-HZ*GY
          BY=HZ*GX-HX*GZ                                                                           ;//        BY=HZ*GX-HX*GZ
          BZ=HX*GY-HY*GX                                                                           ;//        BZ=HX*GY-HY*GX
/// RG=|G|, RGR=1/|G|                                                                              ;//C RG=|G|, RGR=1/|G|
          RA2=AX*AX+AY*AY+AZ*AZ                                                                    ;//        RA2=AX*AX+AY*AY+AZ*AZ
          RB2=BX*BX+BY*BY+BZ*BZ                                                                    ;//        RB2=BX*BX+BY*BY+BZ*BZ
          RG2=GX*GX+GY*GY+GZ*GZ                                                                    ;//        RG2=GX*GX+GY*GY+GZ*GZ
          RG=SQRT(RG2)                                                                             ;//        RG=SQRT(RG2)
/// Warnings have been simplified.                                                                 ;//C Warnings have been simplified.
          if((RA2 <= RXMIN2) || (RB2 <= RXMIN2) || (RG <= RXMIN)) {                                ;//        IF((RA2.LE.RXMIN2).OR.(RB2.LE.RXMIN2).OR.(RG.LE.RXMIN)) THEN
            NWARN=NWARN+1                                                                          ;//          NWARN=NWARN+1
            if((NWARN <= 5  &&   WRNLEV >= 5)  ||  WRNLEV >= 6) {                                  ;//          IF((NWARN.LE.5 .AND. WRNLEV.GE.5) .OR. WRNLEV.GE.6) THEN
//            WRITE(OUTU,20) IPHI,I,J,K,L                                                          ;//            WRITE(OUTU,20) IPHI,I,J,K,L
//   20       FORMAT(' EPHI: WARNING.  dihedral',I5,' is almost linear.'/                          ;//   20       FORMAT(' EPHI: WARNING.  dihedral',I5,' is almost linear.'/
//     1        ' derivatives may be affected for atoms:',4I5)                                     ;//     1        ' derivatives may be affected for atoms:',4I5)
            }                                                                                      ;//          ENDIF
            goto goto160                                                                           ;//          GOTO 160
          }                                                                                        ;//        ENDIF
///                                                                                                ;//C
          RGR=ONE/RG                                                                               ;//        RGR=ONE/RG
          RA2R=ONE/RA2                                                                             ;//        RA2R=ONE/RA2
          RB2R=ONE/RB2                                                                             ;//        RB2R=ONE/RB2
          RABR=SQRT(RA2R*RB2R)                                                                     ;//        RABR=SQRT(RA2R*RB2R)
/// CP=cos(phi)                                                                                    ;//C CP=cos(phi)
          CP=(AX*BX+AY*BY+AZ*BZ)*RABR                                                              ;//        CP=(AX*BX+AY*BY+AZ*BZ)*RABR
/// SP=sin(phi), Note that sin(phi).G/|G|=B^A/(|A|.|B|)                                            ;//C SP=sin(phi), Note that sin(phi).G/|G|=B^A/(|A|.|B|)
/// which can be simplify to sin(phi)=|G|H.A/(|A|.|B|)                                             ;//C which can be simplify to sin(phi)=|G|H.A/(|A|.|B|)
          SP=RG*RABR*(AX*HX+AY*HY+AZ*HZ)                                                           ;//        SP=RG*RABR*(AX*HX+AY*HY+AZ*HZ)
///                                                                                                ;//C
/// Energy and derivative contributions.                                                           ;//C Energy and derivative contributions.
///                                                                                                ;//C
          if (CPD[IC] != 0) {                                                                      ;//        IF (CPD(IC).NE.0) THEN
///                                                                                                ;//C
/// Set up for the proper dihedrals.                                                               ;//C Set up for the proper dihedrals.
///                                                                                                ;//C
            E=ZERO                                                                                 ;//          E=ZERO
            DF=ZERO                                                                                ;//          DF=ZERO
            DDF=ZERO                                                                               ;//          DDF=ZERO
     goto30:                                                                                       ;//   30     CONTINUE
            IPER=CPD[IC]                                                                           ;//          IPER=CPD(IC)
            if (IPER >= 0) {                                                                       ;//          IF (IPER.GE.0) THEN
              LREP= false                                                                          ;//            LREP=.FALSE.
            } else {                                                                               ;//          ELSE
              LREP= true                                                                           ;//            LREP=.TRUE.
              IPER=-IPER                                                                           ;//            IPER=-IPER
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            E1=ONE                                                                                 ;//          E1=ONE
            DF1=ZERO                                                                               ;//          DF1=ZERO
///alculation of cos(n*phi-phi0) and sin(n*phi-phi0).                                              ;//Calculation of cos(n*phi-phi0) and sin(n*phi-phi0).
            for(NPER=1; NPER<=IPER; NPER++) {                                                      ;//          DO 50 NPER=1,IPER
               DDF1=E1*CP-DF1*SP                                                                   ;//             DDF1=E1*CP-DF1*SP
               DF1=E1*SP+DF1*CP                                                                    ;//             DF1=E1*SP+DF1*CP
               E1=DDF1                                                                             ;//             E1=DDF1
            } HDebug.Assert(double.IsNaN(DDF1) == false)                                           ;// 50       CONTINUE
            E1=E1*CPCOS[IC]+DF1*CPSIN[IC]                                                          ;//          E1=E1*CPCOS(IC)+DF1*CPSIN(IC)
            DF1=DF1*CPCOS[IC]-DDF1*CPSIN[IC]                                                       ;//          DF1=DF1*CPCOS(IC)-DDF1*CPSIN(IC)
            DF1=-IPER*DF1                                                                          ;//          DF1=-IPER*DF1
            DDF1=-IPER*IPER*E1                                                                     ;//          DDF1=-IPER*IPER*E1
            E1=ONE+E1                                                                              ;//          E1=ONE+E1
///                                                                                                ;//C
            if (IPER == 0) {                                                                       ;//          IF (IPER.EQ.0) THEN
               E1=ONE                                                                              ;//             E1=ONE
               DF1=ZERO                                                                            ;//             DF1=ZERO
               DDF1=ZERO                                                                           ;//             DDF1=ZERO
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            ARG=CPC[IC]                                                                            ;//          ARG=CPC(IC)
            E=E+ARG*E1                                                                             ;//          E=E+ARG*E1
            DF=DF+ARG*DF1                                                                          ;//          DF=DF+ARG*DF1
            DDF=DDF+ARG*DDF1                                                                       ;//          DDF=DDF+ARG*DDF1
///                                                                                                ;//C
            if(LREP) {                                                                             ;//          IF(LREP) THEN
              IC=IC+1                                                                              ;//            IC=IC+1
              goto goto30                                                                          ;//            GOTO 30
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
/// Set up for the improper dihedrals.                                                             ;//C Set up for the improper dihedrals.
///                                                                                                ;//C
          } else {                                                                                 ;//        ELSE
///                                                                                                ;//C
///alcul of cos(phi-phi0),sin(phi-phi0) and (Phi-Phi0).                                            ;//Calcul of cos(phi-phi0),sin(phi-phi0) and (Phi-Phi0).
            CA=CP*CPCOS[IC]+SP*CPSIN[IC]                                                           ;//          CA=CP*CPCOS(IC)+SP*CPSIN(IC)
            SA=SP*CPCOS[IC]-CP*CPSIN[IC]                                                           ;//          SA=SP*CPCOS(IC)-CP*CPSIN(IC)
            if (CA >  PTONE ) {                                                                    ;//          IF (CA.GT.PTONE ) THEN
                AP=ASIN(SA)                                                                        ;//              AP=ASIN(SA)
            } else {                                                                               ;//          ELSE
                AP=SIGN(ACOS(MAX(CA,MINONE)),SA)                                                   ;//              AP=SIGN(ACOS(MAX(CA,MINONE)),SA)
/// Warning is now triggered at deltaphi=84.26...deg (used to be 90).                              ;//C Warning is now triggered at deltaphi=84.26...deg (used to be 90).
                NWARNX=NWARNX+1                                                                    ;//              NWARNX=NWARNX+1
                if((NWARNX <= 5  &&   WRNLEV >= 5)  ||  WRNLEV >= 6) {                             ;//              IF((NWARNX.LE.5 .AND. WRNLEV.GE.5) .OR. WRNLEV.GE.6) THEN
                   APR=AP*RADDEG                                                                   ;//                 APR=AP*RADDEG
                   CPBIC=CPB[IC]*RADDEG                                                            ;//                 CPBIC=CPB(IC)*RADDEG
//                 WRITE(OUTU,80) IPHI,APR,CPBIC,I,J,K,L                                           ;//                 WRITE(OUTU,80) IPHI,APR,CPBIC,I,J,K,L
// 80              FORMAT(' EPHI: WARNING. bent improper torsion angle',                           ;// 80              FORMAT(' EPHI: WARNING. bent improper torsion angle',
//     A           ' is '//'far ','from minimum for;'/3X,' IPHI=',I5,                              ;//     A           ' is '//'far ','from minimum for;'/3X,' IPHI=',I5,
//     A           '  with deltaPHI=',F9.4,' MIN=',F9.4,' ATOMS:',4I5)                             ;//     A           '  with deltaPHI=',F9.4,' MIN=',F9.4,' ATOMS:',4I5)
                }                                                                                  ;//              ENDIF
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if(QCPW) {                                                                             ;//          IF(QCPW) THEN
              APR=CPW[IC]*DEGRAD                                                                   ;//            APR=CPW(IC)*DEGRAD
              if(ABS(AP) >  APR) {                                                                 ;//            IF(ABS(AP).GT.APR) THEN
                if(AP >  ZERO) {                                                                   ;//              IF(AP.GT.ZERO) THEN
                  AP=AP-APR                                                                        ;//                AP=AP-APR
                } else {                                                                           ;//              ELSE
                  AP=AP+APR                                                                        ;//                AP=AP+APR
                }                                                                                  ;//              ENDIF
                DDF=TWO*CPC[IC]                                                                    ;//              DDF=TWO*CPC(IC)
                DF=DDF*AP                                                                          ;//              DF=DDF*AP
                E=HALF*DF*AP                                                                       ;//              E=HALF*DF*AP
              } else {                                                                             ;//            ELSE
                DDF=ZERO                                                                           ;//              DDF=ZERO
                DF=ZERO                                                                            ;//              DF=ZERO
                E=ZERO                                                                             ;//              E=ZERO
              }                                                                                    ;//            ENDIF
            } else {                                                                               ;//          ELSE
              DDF=TWO*CPC[IC]                                                                      ;//            DDF=TWO*CPC(IC)
              DF=DDF*AP                                                                            ;//            DF=DDF*AP
              E=HALF*DF*AP                                                                         ;//            E=HALF*DF*AP
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
          }                                                                                        ;//        ENDIF
///                                                                                                ;//C
                                                                                                   ;//##IF BLOCK (big_block)
                                                                                                   ;//        IF (QBLOCK) THEN
                                                                                                   ;//          IBL=I4VAL(HEAP(IBLCKP),I)
                                                                                                   ;//          JBL=I4VAL(HEAP(IBLCKP),J)
                                                                                                   ;//          KKK=I4VAL(HEAP(IBLCKP),K)
                                                                                                   ;//          LLL=I4VAL(HEAP(IBLCKP),L)
                                                                                                   ;//##IF DOCK
                                                                                                   ;//c         three pairs (i,j), (k,j) and (k,l)
                                                                                                   ;//          DOCFI = 1.0
                                                                                                   ;//          DOCFJ = 1.0
                                                                                                   ;//          DOCFK = 1.0
                                                                                                   ;//          DOCFJ1 = 1.0
                                                                                                   ;//          DOCFL = 1.0
                                                                                                   ;//          DOCFK1 = 1.0
                                                                                                   ;//          IF(QDOCK) THEN
                                                                                                   ;//              KDOC  = (IBL - 1)*NBLOCK + JBL
                                                                                                   ;//              DOCFI = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (JBL - 1)*NBLOCK + IBL
                                                                                                   ;//              DOCFJ = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (KKK - 1)*NBLOCK + JBL
                                                                                                   ;//              DOCFK = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (JBL - 1)*NBLOCK + KKK
                                                                                                   ;//              DOCFJ1 = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (KKK - 1)*NBLOCK + LLL
                                                                                                   ;//              DOCFK1 = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//              KDOC  = (LLL - 1)*NBLOCK + KKK
                                                                                                   ;//              DOCFL = GTRR8(HEAP(BLDOCP),KDOC)
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! DOCK
                                                                                                   ;//          IF (IBL .EQ. JBL) JBL=KKK
                                                                                                   ;//          IF (IBL .EQ. JBL) JBL=LLL
                                                                                                   ;//          IF (JBL .LT. IBL) THEN
                                                                                                   ;//            KKK=JBL
                                                                                                   ;//            JBL=IBL
                                                                                                   ;//            IBL=KKK
                                                                                                   ;//          ENDIF
                                                                                                   ;//          KKK=IBL+JBL*(JBL-1)/2
                                                                                                   ;//          COEF=GTRR8(HEAP(BLCOED),KKK)
                                                                                                   ;//
                                                                                                   ;//##IF BANBA
                                                                                                   ;//          IF (QPRNTV) THEN
                                                                                                   ;//            IF (IBL.EQ.1 .OR. JBL.EQ.1 .OR. IBL.EQ.JBL) THEN
                                                                                                   ;//              IF(QNOIM .AND. (CPD(IC).EQ.0)) THEN
                                                                                                   ;//                CALL ASUMR8(E,HEAP(VBIMPR),JBL)
                                                                                                   ;//              ELSE IF(QNOPH.AND.(CPD(IC).NE.0)) THEN
                                                                                                   ;//                CALL ASUMR8(E,HEAP(VBTORS),JBL)
                                                                                                   ;//              ENDIF
                                                                                                   ;//            ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! BANBA                    
                                                                                                   ;//##IF LDM  
                                                                                                   ;//          IF(QLDM) THEN                          !##.not.LMC
                                                                                                   ;//          IF(QLDM.or.QLMC) THEN                  !##LMC
                                                                                                   ;//C           first row or diagonal elements exclude (1,1).
                                                                                                   ;//            UNSCALE = 0.0
                                                                                                   ;//##IF LDMGEN
                                                                                                   ;//            IF((IBL.EQ.1.AND.JBL.GE.LSTRT).or.
                                                                                                   ;//     &         (IBL.GE.LSTRT.AND.IBL.EQ.JBL)) UNSCALE = E
                                                                                                   ;//##ELSE
                                                                                                   ;//            IF(IBL.NE.1.AND.IBL.EQ.JBL) THEN 
                                                                                                   ;//               UNSCALE = E
                                                                                                   ;//            ELSE IF(IBL.EQ.1.AND.JBL.GE.2) THEN
                                                                                                   ;//               UNSCALE = E
                                                                                                   ;//            ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF    ! LDM
                                                                                                   ;//##IF BANBA
                                                                                                   ;//          IF ( QNOIM .AND. (CPD(IC).EQ.0) ) COEF=1.0
                                                                                                   ;//          IF ( QNOPH .AND. (CPD(IC).NE.0) ) COEF=1.0
                                                                                                   ;//##ENDIF ! BANBA
                                                                                                   ;//##IF DOCK
                                                                                                   ;//          IF(QDOCK) THEN
                                                                                                   ;//            E=E*COEF*(DOCFI+DOCFJ+DOCFK+DOCFJ1+
                                                                                                   ;//     &                 DOCFK1+DOCFL)/6.0
                                                                                                   ;//          ELSE    
                                                                                                   ;//##ENDIF
                                                                                                   ;//
                                                                                                   ;//            E=E*COEF
                                                                                                   ;//
                                                                                                   ;//##IF DOCK
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//          DFORG=DF               !##LRST   !##LDM
                                                                                                   ;//          DF=DF*COEF
                                                                                                   ;//          DDF=DDF*COEF
                                                                                                   ;//        ENDIF
                                                                                                   ;//C Set the energy.
                                                                                                   ;//##IF LDM  (ldm_5)
                                                                                                   ;//        IF(QLDM) THEN           !##.not.LMC
                                                                                                   ;//        IF(QLDM.or.QLMC) THEN !##LMC
                                                                                                   ;//##IF BANBA
                                                                                                   ;//           IF((.NOT.QNOIM.AND.CPD(IC).EQ.0).OR.
                                                                                                   ;//     &          (.NOT.QNOPH.AND.CPD(IC).NE.0))THEN
                                                                                                   ;//##ENDIF !BANBA
                                                                                                   ;//##IF LDMGEN
                                                                                                   ;//              IF((IBL.EQ.1.AND.JBL.GE.LSTRT).or.
                                                                                                   ;//     &             (IBL.GE.LSTRT.AND.IBL.EQ.JBL)) THEN 
                                                                                                   ;//##ELSE
                                                                                                   ;//              IF((IBL.EQ.JBL).OR.(IBL.EQ.1.AND.JBL.GE.2)) THEN
                                                                                                   ;//##ENDIF !LDMGEN
                                                                                                   ;//                 FALPHA = UNSCALE
                                                                                                   ;//                 LAGMUL = LAGMUL + FALPHA
                                                                                                   ;//                 CALL ASUMR8(FALPHA,HEAP(BIFLAM),JBL)
                                                                                                   ;//##IF LRST
                                                                                                   ;//                 IF(NRST.EQ.2)THEN
                                                                                                   ;//                    CALL ASUMR8(FALPHA,HEAP(BFRST),JBL)
                                                                                                   ;//                 ENDIF
                                                                                                   ;//##ENDIF !LRST
                                                                                                   ;//              ENDIF
                                                                                                   ;//##IF BANBA
                                                                                                   ;//           ENDIF
                                                                                                   ;//##ENDIF  !BANBA
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF  (ldm_5)
                                                                                                   ;//##ENDIF (big_block)
          EP=EP+E                                                                                  ;//        EP=EP+E
///                                                                                                ;//C
///brb...19-Jul-94 New ANAL terms                                                                  ;//Cbrb...19-Jul-94 New ANAL terms
//        if(QATERM) {                                                                             ;//        IF(QATERM) THEN
//          KK=ANSLCT[I]+ANSLCT[J]+ANSLCT[K]+ANSLCT[L]                                             ;//          KK=ANSLCT(I)+ANSLCT(J)+ANSLCT(K)+ANSLCT(L)
//          if(KK == 4  ||  (KK >= 1  &&      ! QAONLY)) {                                         ;//          IF(KK.EQ.4 .OR. (KK.GE.1 .AND. .NOT.QAONLY)) THEN
//            if(QAUNIT <  0) {                                                                    ;//            IF(QAUNIT.LT.0) THEN
//              II=OUTU                                                                            ;//              II=OUTU
//            } else {                                                                             ;//            ELSE
//              II=QAUNIT                                                                          ;//              II=QAUNIT
//            }                                                                                    ;//            ENDIF
///                                                                                                ;//C
//            if (CP >  PTONE ) {                                                                  ;//            IF (CP.GT.PTONE ) THEN
//              AP=ASIN(SP)                                                                        ;//              AP=ASIN(SP)
//            } else {                                                                             ;//            ELSE
//              AP=SIGN(ACOS(MAX(CP,MINONE)),SP)                                                   ;//              AP=SIGN(ACOS(MAX(CP,MINONE)),SP)
//            }                                                                                    ;//            ENDIF
///                                                                                                ;//C
//            if(PRNLEV >= 5) {                                                                    ;//            IF(PRNLEV.GE.5) THEN
//              if(QAFIRST) {                                                                      ;//              IF(QAFIRST) THEN
//                if(QLONGL) {                                                                     ;//                IF(QLONGL) THEN
//                  WRITE(II,243)                                                                  ;//                  WRITE(II,243)
//                } else {                                                                         ;//                ELSE
//                  WRITE(II,244)                                                                  ;//                  WRITE(II,244)
//                }                                                                                ;//                ENDIF
// 243           FORMAT('ANAL: DIHE: Index        Atom-I              ',                           ;// 243           FORMAT('ANAL: DIHE: Index        Atom-I              ',
//     &                '     Atom-J                   Atom-K         ',                           ;//     &                '     Atom-J                   Atom-K         ',
//     &                '          Atom-L          ',                                              ;//     &                '          Atom-L          ',
//     &                '        Dihedral       Energy   ',                                        ;//     &                '        Dihedral       Energy   ',
//     &                '      Force           Parameters')                                        ;//     &                '      Force           Parameters')
// 244           FORMAT('ANAL: DIHE: Index        Atom-I              ',                           ;// 244           FORMAT('ANAL: DIHE: Index        Atom-I              ',
//     &                '     Atom-J',/                                                            ;//     &                '     Atom-J',/
//     &                '                         Atom-K         ',                                ;//     &                '                         Atom-K         ',
//     &                '          Atom-L          ',/                                             ;//     &                '          Atom-L          ',/
//     &                '        Dihedral       Energy   ',                                        ;//     &                '        Dihedral       Energy   ',
//     &                '      Force           Parameters')                                        ;//     &                '      Force           Parameters')
//               QAFIRST= false                                                                    ;//               QAFIRST=.FALSE.
//              }                                                                                  ;//              ENDIF
//              CALL ATOMID(I,SIDDNI,RIDDNI,RESDNI,ACDNI)                                          ;//              CALL ATOMID(I,SIDDNI,RIDDNI,RESDNI,ACDNI)
//              CALL ATOMID(J,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ)                                          ;//              CALL ATOMID(J,SIDDNJ,RIDDNJ,RESDNJ,ACDNJ)
//              CALL ATOMID(K,SIDDNK,RIDDNK,RESDNK,ACDNK)                                          ;//              CALL ATOMID(K,SIDDNK,RIDDNK,RESDNK,ACDNK)
//              CALL ATOMID(L,SIDDNL,RIDDNL,RESDNL,ACDNL)                                          ;//              CALL ATOMID(L,SIDDNL,RIDDNL,RESDNL,ACDNL)
//              if(QLONGL) {                                                                       ;//              IF(QLONGL) THEN
//                 WRITE(II,245) IPHI,I,SIDDNI(1:idleng),RIDDNI(1:idleng),                         ;//                 WRITE(II,245) IPHI,I,SIDDNI(1:idleng),RIDDNI(1:idleng),
//     $                RESDNI(1:idleng),ACDNI(1:idleng),                                          ;//     $                RESDNI(1:idleng),ACDNI(1:idleng),
//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),                                       ;//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),
//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),                                          ;//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),
//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),                                       ;//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),
//     $                RESDNK(1:idleng),ACDNK(1:idleng),                                          ;//     $                RESDNK(1:idleng),ACDNK(1:idleng),
//     $                L,SIDDNL(1:idleng),RIDDNL(1:idleng),                                       ;//     $                L,SIDDNL(1:idleng),RIDDNL(1:idleng),
//     $                RESDNL(1:idleng),ACDNL(1:idleng),                                          ;//     $                RESDNL(1:idleng),ACDNL(1:idleng),
//     $                AP*RADDEG,E,DF,IC,CPB(IC)*RADDEG,                                          ;//     $                AP*RADDEG,E,DF,IC,CPB(IC)*RADDEG,
//     $                CPC(IC),CPD(IC)                                                            ;//     $                CPC(IC),CPD(IC)
//              } else {                                                                           ;//              ELSE
//                 WRITE(II,246) IPHI,I,SIDDNI(1:idleng),RIDDNI(1:idleng),                         ;//                 WRITE(II,246) IPHI,I,SIDDNI(1:idleng),RIDDNI(1:idleng),
//     $                RESDNI(1:idleng),ACDNI(1:idleng),                                          ;//     $                RESDNI(1:idleng),ACDNI(1:idleng),
//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),                                       ;//     $                J,SIDDNJ(1:idleng),RIDDNJ(1:idleng),
//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),                                          ;//     $                RESDNJ(1:idleng),ACDNJ(1:idleng),
//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),                                       ;//     $                K,SIDDNK(1:idleng),RIDDNK(1:idleng),
//     $                RESDNK(1:idleng),ACDNK(1:idleng),                                          ;//     $                RESDNK(1:idleng),ACDNK(1:idleng),
//     $                L,SIDDNL(1:idleng),RIDDNL(1:idleng),                                       ;//     $                L,SIDDNL(1:idleng),RIDDNL(1:idleng),
//     $                RESDNL(1:idleng),ACDNL(1:idleng),                                          ;//     $                RESDNL(1:idleng),ACDNL(1:idleng),
//     $                AP*RADDEG,E,DF,IC,CPB(IC)*RADDEG,                                          ;//     $                AP*RADDEG,E,DF,IC,CPB(IC)*RADDEG,
//     $                CPC(IC),CPD(IC)                                                            ;//     $                CPC(IC),CPD(IC)
//              }                                                                                  ;//              ENDIF
// 245          FORMAT('ANAL: DIHE>',2I5,4(1X,A),I5,4(1X,A),I5,4(1X,A),                            ;// 245          FORMAT('ANAL: DIHE>',2I5,4(1X,A),I5,4(1X,A),I5,4(1X,A),
//     &               I5,4(1X,A),3F15.6,I5,2F15.6,I5)                                             ;//     &               I5,4(1X,A),3F15.6,I5,2F15.6,I5)
// 246          FORMAT('ANAL: DIHE>',2I5,4(1X,A),I5,4(1X,A),/                                      ;// 246          FORMAT('ANAL: DIHE>',2I5,4(1X,A),I5,4(1X,A),/
//     &                              I5,4(1X,A),I5,4(1X,A),/                                      ;//     &                              I5,4(1X,A),I5,4(1X,A),/
//     &                              3F15.6,I5,2F15.6,I5)                                         ;//     &                              3F15.6,I5,2F15.6,I5)
//            }                                                                                    ;//            ENDIF
//          }                                                                                      ;//          ENDIF
//        }                                                                                        ;//        ENDIF
///                                                                                                ;//C
/// Contribution on atoms.                                                                         ;//C Contribution on atoms.
          if(QECONTX) {                                                                            ;//        IF(QECONTX) THEN
            E=E*PT25                                                                               ;//          E=E*PT25
            ECONTX[I]=ECONTX[I]+E                                                                  ;//          ECONTX(I)=ECONTX(I)+E
            ECONTX[J]=ECONTX[J]+E                                                                  ;//          ECONTX(J)=ECONTX(J)+E
            ECONTX[K]=ECONTX[K]+E                                                                  ;//          ECONTX(K)=ECONTX(K)+E
            ECONTX[L]=ECONTX[L]+E                                                                  ;//          ECONTX(L)=ECONTX(L)+E
          }                                                                                        ;//        ENDIF
///                                                                                                ;//C
/// Compute derivatives wrt catesian coordinates.                                                  ;//C Compute derivatives wrt catesian coordinates.
/// this section is for first derivatives only.                                                    ;//C this section is for first derivatives only.
///                                                                                                ;//C
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        IF (.NOT. NOFORC) THEN
                                                                                                   ;//##ENDIF ! BLOCK
/// GAA=-|G|/A^2, GBB=|G|/B^2, FG=F.G, HG=H.G                                                      ;//C GAA=-|G|/A^2, GBB=|G|/B^2, FG=F.G, HG=H.G
///  FGA=F.G/(|G|A^2), HGB=H.G/(|G|B^2)                                                            ;//C  FGA=F.G/(|G|A^2), HGB=H.G/(|G|B^2)
          FG=FX*GX+FY*GY+FZ*GZ                                                                     ;//        FG=FX*GX+FY*GY+FZ*GZ
          HG=HX*GX+HY*GY+HZ*GZ                                                                     ;//        HG=HX*GX+HY*GY+HZ*GZ
          FGA=FG*RA2R*RGR                                                                          ;//        FGA=FG*RA2R*RGR
          HGB=HG*RB2R*RGR                                                                          ;//        HGB=HG*RB2R*RGR
          GAA=-RA2R*RG                                                                             ;//        GAA=-RA2R*RG
          GBB=RB2R*RG                                                                              ;//        GBB=RB2R*RG
/// DTFi=d(phi)/dFi, DTGi=d(phi)/dGi, DTHi=d(phi)/dHi. (used in 2nd deriv)                         ;//C DTFi=d(phi)/dFi, DTGi=d(phi)/dGi, DTHi=d(phi)/dHi. (used in 2nd deriv)
          DTFX=GAA*AX                                                                              ;//        DTFX=GAA*AX
          DTFY=GAA*AY                                                                              ;//        DTFY=GAA*AY
          DTFZ=GAA*AZ                                                                              ;//        DTFZ=GAA*AZ
          DTGX=FGA*AX-HGB*BX                                                                       ;//        DTGX=FGA*AX-HGB*BX
          DTGY=FGA*AY-HGB*BY                                                                       ;//        DTGY=FGA*AY-HGB*BY
          DTGZ=FGA*AZ-HGB*BZ                                                                       ;//        DTGZ=FGA*AZ-HGB*BZ
          DTHX=GBB*BX                                                                              ;//        DTHX=GBB*BX
          DTHY=GBB*BY                                                                              ;//        DTHY=GBB*BY
          DTHZ=GBB*BZ                                                                              ;//        DTHZ=GBB*BZ
/// DFi=dE/dFi, DGi=dE/dGi, DHi=dE/dHi.                                                            ;//C DFi=dE/dFi, DGi=dE/dGi, DHi=dE/dHi.
          DFX=DF*DTFX                                                                              ;//        DFX=DF*DTFX
          DFY=DF*DTFY                                                                              ;//        DFY=DF*DTFY
          DFZ=DF*DTFZ                                                                              ;//        DFZ=DF*DTFZ
          DGX=DF*DTGX                                                                              ;//        DGX=DF*DTGX
          DGY=DF*DTGY                                                                              ;//        DGY=DF*DTGY
          DGZ=DF*DTGZ                                                                              ;//        DGZ=DF*DTGZ
          DHX=DF*DTHX                                                                              ;//        DHX=DF*DTHX
          DHY=DF*DTHY                                                                              ;//        DHY=DF*DTHY
          DHZ=DF*DTHZ                                                                              ;//        DHZ=DF*DTHZ
/// Distribute over Ri.                                                                            ;//C Distribute over Ri.
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        IF(QDOCK) THEN
                                                                                                   ;//           DX(I)=DX(I)+DFX*DOCFI
                                                                                                   ;//           DY(I)=DY(I)+DFY*DOCFI
                                                                                                   ;//           DZ(I)=DZ(I)+DFZ*DOCFI
                                                                                                   ;//           DX(J)=DX(J)-DFX*DOCFJ+DGX*DOCFJ1
                                                                                                   ;//           DY(J)=DY(J)-DFY*DOCFJ+DGY*DOCFJ1
                                                                                                   ;//           DZ(J)=DZ(J)-DFZ*DOCFJ+DGZ*DOCFJ1
                                                                                                   ;//           DX(K)=DX(K)-DHX*DOCFK1-DGX*DOCFK
                                                                                                   ;//           DY(K)=DY(K)-DHY*DOCFK1-DGY*DOCFK
                                                                                                   ;//           DZ(K)=DZ(K)-DHZ*DOCFK1-DGZ*DOCFK
                                                                                                   ;//           DX(L)=DX(L)+DHX*DOCFL
                                                                                                   ;//           DY(L)=DY(L)+DHY*DOCFL
                                                                                                   ;//           DZ(L)=DZ(L)+DHZ*DOCFL
                                                                                                   ;//        ELSE
                                                                                                   ;//##ENDIF
                                                                                                   ;//##ENDIF
             DX[I]=DX[I]+DFX                                                                       ;//           DX(I)=DX(I)+DFX
             DY[I]=DY[I]+DFY                                                                       ;//           DY(I)=DY(I)+DFY
             DZ[I]=DZ[I]+DFZ                                                                       ;//           DZ(I)=DZ(I)+DFZ
             DX[J]=DX[J]-DFX+DGX                                                                   ;//           DX(J)=DX(J)-DFX+DGX
             DY[J]=DY[J]-DFY+DGY                                                                   ;//           DY(J)=DY(J)-DFY+DGY
             DZ[J]=DZ[J]-DFZ+DGZ                                                                   ;//           DZ(J)=DZ(J)-DFZ+DGZ
             DX[K]=DX[K]-DHX-DGX                                                                   ;//           DX(K)=DX(K)-DHX-DGX
             DY[K]=DY[K]-DHY-DGY                                                                   ;//           DY(K)=DY(K)-DHY-DGY
             DZ[K]=DZ[K]-DHZ-DGZ                                                                   ;//           DZ(K)=DZ(K)-DHZ-DGZ
             DX[L]=DX[L]+DHX                                                                       ;//           DX(L)=DX(L)+DHX
             DY[L]=DY[L]+DHY                                                                       ;//           DY(L)=DY(L)+DHY
             DZ[L]=DZ[L]+DHZ                                                                       ;//           DZ(L)=DZ(L)+DHZ
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//##IF DOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//##ENDIF
                                                                                                   ;//##IF LDM
                                                                                                   ;//##IF LRST
                                                                                                   ;//            IF(RSTP)THEN
                                                                                                   ;//              IF((.NOT.QNOIM.AND.CPD(IC).EQ.0).OR.
                                                                                                   ;//     &           (.NOT.QNOPH.AND.CPD(IC).NE.0))THEN
                                                                                                   ;//                IF((IBL.EQ.1 .AND. JBL.GE.LSTRT).or.
                                                                                                   ;//     &             (IBL.GE.LSTRT .AND. IBL.EQ.JBL) ) THEN
                                                                                                   ;//                 DFX=DFORG*DTFX
                                                                                                   ;//                 DFY=DFORG*DTFY
                                                                                                   ;//                 DFZ=DFORG*DTFZ
                                                                                                   ;//                 DGX=DFORG*DTGX
                                                                                                   ;//                 DGY=DFORG*DTGY
                                                                                                   ;//                 DGZ=DFORG*DTGZ
                                                                                                   ;//                 DHX=DFORG*DTHX
                                                                                                   ;//                 DHY=DFORG*DTHY
                                                                                                   ;//                 DHZ=DFORG*DTHZ
                                                                                                   ;//                 CALL ASUMR8(DFX,HEAP(ENVDX),I)
                                                                                                   ;//                 CALL ASUMR8(DFY,HEAP(ENVDY),I)
                                                                                                   ;//                 CALL ASUMR8(DFZ,HEAP(ENVDZ),I)
                                                                                                   ;//C
                                                                                                   ;//                 CALL ASUMR8(-DFX+DGX,HEAP(ENVDX),J)
                                                                                                   ;//                 CALL ASUMR8(-DFY+DGY,HEAP(ENVDY),J)
                                                                                                   ;//                 CALL ASUMR8(-DFZ+DGZ,HEAP(ENVDZ),J)
                                                                                                   ;//C
                                                                                                   ;//                 CALL ASUMR8(-DHX-DGX,HEAP(ENVDX),K)
                                                                                                   ;//                 CALL ASUMR8(-DHY-DGY,HEAP(ENVDY),K)
                                                                                                   ;//                 CALL ASUMR8(-DHZ-DGZ,HEAP(ENVDZ),K)
                                                                                                   ;//C
                                                                                                   ;//                 CALL ASUMR8(DHX,HEAP(ENVDX),L)
                                                                                                   ;//                 CALL ASUMR8(DHY,HEAP(ENVDY),L)
                                                                                                   ;//                 CALL ASUMR8(DHZ,HEAP(ENVDZ),L)
                                                                                                   ;//                ENDIF 
                                                                                                   ;//              ENDIF 
                                                                                                   ;//            ENDIF 
                                                                                                   ;//##ENDIF ! LRST
                                                                                                   ;//##ENDIF ! LDM
///                                                                                                ;//C
                                                                                                   ;//##IF IPRESS
                                                                                                   ;//        IF(QIPRSS) THEN
                                                                                                   ;//          PVIR(I)=PVIR(I)+DFX*FX+DFY*FY+DFZ*FZ
                                                                                                   ;//          PVIR(J)=PVIR(J)+DFX*FX+DFY*FY+DFZ*FZ+DGX*GX+DGY*GY+DGZ*GZ
                                                                                                   ;//          PVIR(K)=PVIR(K)+DGX*GX+DGY*GY+DGZ*GZ+DHX*HX+DHY*HY+DHZ*HZ
                                                                                                   ;//          PVIR(L)=PVIR(L)+DHX*HX+DHY*HY+DHZ*HZ
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF
///                                                                                                ;//C
/// Second derivative part.                                                                        ;//C Second derivative part.
///                                                                                                ;//C
          if (QSECD) {                                                                             ;//        IF (QSECD) THEN
///                                                                                                ;//C
/// RGR2=1/G.G,FGRG2=(F.G)/(G.G),HGRG2=(H.G)/(G.G),DFRG3=(dE/dPhi)/|G|^3                           ;//C RGR2=1/G.G,FGRG2=(F.G)/(G.G),HGRG2=(H.G)/(G.G),DFRG3=(dE/dPhi)/|G|^3 
          RGR2=RGR*RGR                                                                             ;//        RGR2=RGR*RGR
          FGRG2=FG*RGR2                                                                            ;//        FGRG2=FG*RGR2
          HGRG2=HG*RGR2                                                                            ;//        HGRG2=HG*RGR2
          DFRG3=DF*RGR2*RGR                                                                        ;//        DFRG3=DF*RGR2*RGR
/// GAF=-G^A/A.A, GBH=-G^B/B.B, FAG=F^A/A.A, HBG=-H^B/B.B                                          ;//C GAF=-G^A/A.A, GBH=-G^B/B.B, FAG=F^A/A.A, HBG=-H^B/B.B
          GAFX=RA2R*(AY*GZ-AZ*GY)                                                                  ;//        GAFX=RA2R*(AY*GZ-AZ*GY)
          GAFY=RA2R*(AZ*GX-AX*GZ)                                                                  ;//        GAFY=RA2R*(AZ*GX-AX*GZ)
          GAFZ=RA2R*(AX*GY-AY*GX)                                                                  ;//        GAFZ=RA2R*(AX*GY-AY*GX)
          GBHX=RB2R*(BY*GZ-BZ*GY)                                                                  ;//        GBHX=RB2R*(BY*GZ-BZ*GY)
          GBHY=RB2R*(BZ*GX-BX*GZ)                                                                  ;//        GBHY=RB2R*(BZ*GX-BX*GZ)
          GBHZ=RB2R*(BX*GY-BY*GX)                                                                  ;//        GBHZ=RB2R*(BX*GY-BY*GX)
          FAGX=RA2R*(FY*AZ-FZ*AY)                                                                  ;//        FAGX=RA2R*(FY*AZ-FZ*AY)
          FAGY=RA2R*(FZ*AX-FX*AZ)                                                                  ;//        FAGY=RA2R*(FZ*AX-FX*AZ)
          FAGZ=RA2R*(FX*AY-FY*AX)                                                                  ;//        FAGZ=RA2R*(FX*AY-FY*AX)
          HBGX=RB2R*(BY*HZ-BZ*HY)                                                                  ;//        HBGX=RB2R*(BY*HZ-BZ*HY)
          HBGY=RB2R*(BZ*HX-BX*HZ)                                                                  ;//        HBGY=RB2R*(BZ*HX-BX*HZ)
          HBGZ=RB2R*(BX*HY-BY*HX)                                                                  ;//        HBGZ=RB2R*(BX*HY-BY*HX)
/// What are the indexes ?                                                                         ;//C What are the indexes ?
/// ddE/dX.dY= DDFGH(n)      Fx, Fy, Fz,|Gx, Gy, Gz,|Hx, Hy, Hz. X/                                ;//c ddE/dX.dY= DDFGH(n)      Fx, Fy, Fz,|Gx, Gy, Gz,|Hx, Hy, Hz. X/
///                          ----------------------------------- / Y                               ;//c                          ----------------------------------- / Y
///               n=          1   2   4 | 7  11  16 |22  29  37   Fx                               ;//c               n=          1   2   4 | 7  11  16 |22  29  37   Fx
///                               3   5 | 8  12  17 |23  30  38   Fy                               ;//c                               3   5 | 8  12  17 |23  30  38   Fy
///                                   6 | 9  13  18 |24  31  39   Fz                               ;//c                                   6 | 9  13  18 |24  31  39   Fz
///                                     ------------------------                                   ;//c                                     ------------------------
///                                      10  14  19 |25  32  40   Gx                               ;//c                                      10  14  19 |25  32  40   Gx
///                                          15  20 |26  33  41   Gy                               ;//c                                          15  20 |26  33  41   Gy
///                                              21 |27  34  42   Gz                               ;//c                                              21 |27  34  42   Gz
///                                                 ------------                                   ;//c                                                 ------------
///                                                  28  35  43   Hx                               ;//c                                                  28  35  43   Hx
///                                                      36  44   Hy                               ;//c                                                      36  44   Hy
///                                                          45   Hz                               ;//c                                                          45   Hz
///                                                                                                ;//C
/// ddE/dF.dF                                                                                      ;//C ddE/dF.dF
          DDFGH[1] =DDF*DTFX*DTFX+TWO*DFX*GAFX                                                     ;//        DDFGH(1) =DDF*DTFX*DTFX+TWO*DFX*GAFX
          DDFGH[2] =DDF*DTFX*DTFY+DFX*GAFY+DFY*GAFX                                                ;//        DDFGH(2) =DDF*DTFX*DTFY+DFX*GAFY+DFY*GAFX
          DDFGH[3] =DDF*DTFY*DTFY+TWO*DFY*GAFY                                                     ;//        DDFGH(3) =DDF*DTFY*DTFY+TWO*DFY*GAFY
          DDFGH[4] =DDF*DTFX*DTFZ+DFX*GAFZ+DFZ*GAFX                                                ;//        DDFGH(4) =DDF*DTFX*DTFZ+DFX*GAFZ+DFZ*GAFX
          DDFGH[5] =DDF*DTFY*DTFZ+DFY*GAFZ+DFZ*GAFY                                                ;//        DDFGH(5) =DDF*DTFY*DTFZ+DFY*GAFZ+DFZ*GAFY
          DDFGH[6] =DDF*DTFZ*DTFZ+TWO*DFZ*GAFZ                                                     ;//        DDFGH(6) =DDF*DTFZ*DTFZ+TWO*DFZ*GAFZ
/// ddE/dF.dG                                                                                      ;//C ddE/dF.dG
          DDFGH[7] =DDF*DTFX*DTGX+FAGX*DFX-FGRG2*DFX*GAFX                                          ;//        DDFGH(7) =DDF*DTFX*DTGX+FAGX*DFX-FGRG2*DFX*GAFX
          DDFGH[8] =DDF*DTFY*DTGX+FAGY*DFX-FGRG2*DFY*GAFX                                          ;//        DDFGH(8) =DDF*DTFY*DTGX+FAGY*DFX-FGRG2*DFY*GAFX
          DDFGH[9] =DDF*DTFZ*DTGX+FAGZ*DFX-FGRG2*DFZ*GAFX                                          ;//        DDFGH(9) =DDF*DTFZ*DTGX+FAGZ*DFX-FGRG2*DFZ*GAFX
          DDFGH[11]=DDF*DTFX*DTGY+FAGX*DFY-FGRG2*DFX*GAFY                                          ;//        DDFGH(11)=DDF*DTFX*DTGY+FAGX*DFY-FGRG2*DFX*GAFY
          DDFGH[12]=DDF*DTFY*DTGY+FAGY*DFY-FGRG2*DFY*GAFY                                          ;//        DDFGH(12)=DDF*DTFY*DTGY+FAGY*DFY-FGRG2*DFY*GAFY
          DDFGH[13]=DDF*DTFZ*DTGY+FAGZ*DFY-FGRG2*DFZ*GAFY                                          ;//        DDFGH(13)=DDF*DTFZ*DTGY+FAGZ*DFY-FGRG2*DFZ*GAFY
          DDFGH[16]=DDF*DTFX*DTGZ+FAGX*DFZ-FGRG2*DFX*GAFZ                                          ;//        DDFGH(16)=DDF*DTFX*DTGZ+FAGX*DFZ-FGRG2*DFX*GAFZ
          DDFGH[17]=DDF*DTFY*DTGZ+FAGY*DFZ-FGRG2*DFY*GAFZ                                          ;//        DDFGH(17)=DDF*DTFY*DTGZ+FAGY*DFZ-FGRG2*DFY*GAFZ
          DDFGH[18]=DDF*DTFZ*DTGZ+FAGZ*DFZ-FGRG2*DFZ*GAFZ                                          ;//        DDFGH(18)=DDF*DTFZ*DTGZ+FAGZ*DFZ-FGRG2*DFZ*GAFZ
/// ddE/dF.dH                                                                                      ;//C ddE/dF.dH
          DDFGH[22]=DDF*DTFX*DTHX                                                                  ;//        DDFGH(22)=DDF*DTFX*DTHX
          DDFGH[23]=DDF*DTFY*DTHX                                                                  ;//        DDFGH(23)=DDF*DTFY*DTHX
          DDFGH[24]=DDF*DTFZ*DTHX                                                                  ;//        DDFGH(24)=DDF*DTFZ*DTHX
          DDFGH[29]=DDF*DTFX*DTHY                                                                  ;//        DDFGH(29)=DDF*DTFX*DTHY
          DDFGH[30]=DDF*DTFY*DTHY                                                                  ;//        DDFGH(30)=DDF*DTFY*DTHY
          DDFGH[31]=DDF*DTFZ*DTHY                                                                  ;//        DDFGH(31)=DDF*DTFZ*DTHY
          DDFGH[37]=DDF*DTFX*DTHZ                                                                  ;//        DDFGH(37)=DDF*DTFX*DTHZ
          DDFGH[38]=DDF*DTFY*DTHZ                                                                  ;//        DDFGH(38)=DDF*DTFY*DTHZ
          DDFGH[39]=DDF*DTFZ*DTHZ                                                                  ;//        DDFGH(39)=DDF*DTFZ*DTHZ
/// ddE/dG.dG                                                                                      ;//C ddE/dG.dG
          DDFGH[10]=DDF*DTGX*DTGX-DFRG3*(GAFX*AX-GBHX*BX)                                           //        DDFGH(10)=DDF*DTGX*DTGX-DFRG3*(GAFX*AX-GBHX*BX)
             -TWO*FGRG2*DFX*FAGX+TWO*HGRG2*DHX*HBGX                                                ;//     A     -TWO*FGRG2*DFX*FAGX+TWO*HGRG2*DHX*HBGX
          DDFGH[14]=DDF*DTGX*DTGY                                                                   //        DDFGH(14)=DDF*DTGX*DTGY
            -HALF*DFRG3*(GAFX*AY+GAFY*AX-GBHX*BY-GBHY*BX)                                           //     A    -HALF*DFRG3*(GAFX*AY+GAFY*AX-GBHX*BY-GBHY*BX)
            -FGRG2*(DFX*FAGY+DFY*FAGX)+HGRG2*(DHX*HBGY+DHY*HBGX)                                   ;//     A    -FGRG2*(DFX*FAGY+DFY*FAGX)+HGRG2*(DHX*HBGY+DHY*HBGX)
          DDFGH[15]=DDF*DTGY*DTGY-DFRG3*(GAFY*AY-GBHY*BY)                                           //        DDFGH(15)=DDF*DTGY*DTGY-DFRG3*(GAFY*AY-GBHY*BY)
             -TWO*FGRG2*DFY*FAGY+TWO*HGRG2*DHY*HBGY                                                ;//     A     -TWO*FGRG2*DFY*FAGY+TWO*HGRG2*DHY*HBGY
          DDFGH[19]=DDF*DTGX*DTGZ                                                                   //        DDFGH(19)=DDF*DTGX*DTGZ
            -HALF*DFRG3*(GAFX*AZ+GAFZ*AX-GBHX*BZ-GBHZ*BX)                                           //     A    -HALF*DFRG3*(GAFX*AZ+GAFZ*AX-GBHX*BZ-GBHZ*BX)
            -FGRG2*(DFX*FAGZ+DFZ*FAGX)+HGRG2*(DHX*HBGZ+DHZ*HBGX)                                   ;//     A    -FGRG2*(DFX*FAGZ+DFZ*FAGX)+HGRG2*(DHX*HBGZ+DHZ*HBGX)
          DDFGH[20]=DDF*DTGY*DTGZ                                                                   //        DDFGH(20)=DDF*DTGY*DTGZ
            -HALF*DFRG3*(GAFY*AZ+GAFZ*AY-GBHY*BZ-GBHZ*BY)                                           //     A    -HALF*DFRG3*(GAFY*AZ+GAFZ*AY-GBHY*BZ-GBHZ*BY)
            -FGRG2*(DFY*FAGZ+DFZ*FAGY)+HGRG2*(DHY*HBGZ+DHZ*HBGY)                                   ;//     A    -FGRG2*(DFY*FAGZ+DFZ*FAGY)+HGRG2*(DHY*HBGZ+DHZ*HBGY)
          DDFGH[21]=DDF*DTGZ*DTGZ-DFRG3*(GAFZ*AZ-GBHZ*BZ)                                           //        DDFGH(21)=DDF*DTGZ*DTGZ-DFRG3*(GAFZ*AZ-GBHZ*BZ)
             -TWO*FGRG2*DFZ*FAGZ+TWO*HGRG2*DHZ*HBGZ                                                ;//     A     -TWO*FGRG2*DFZ*FAGZ+TWO*HGRG2*DHZ*HBGZ
/// ddE/dG.dH                                                                                      ;//C ddE/dG.dH
          DDFGH[25]=DDF*DTGX*DTHX-DHX*HBGX-HGRG2*GBHX*DHX                                          ;//        DDFGH(25)=DDF*DTGX*DTHX-DHX*HBGX-HGRG2*GBHX*DHX
          DDFGH[26]=DDF*DTGY*DTHX-DHY*HBGX-HGRG2*GBHY*DHX                                          ;//        DDFGH(26)=DDF*DTGY*DTHX-DHY*HBGX-HGRG2*GBHY*DHX
          DDFGH[27]=DDF*DTGZ*DTHX-DHZ*HBGX-HGRG2*GBHZ*DHX                                          ;//        DDFGH(27)=DDF*DTGZ*DTHX-DHZ*HBGX-HGRG2*GBHZ*DHX
          DDFGH[32]=DDF*DTGX*DTHY-DHX*HBGY-HGRG2*GBHX*DHY                                          ;//        DDFGH(32)=DDF*DTGX*DTHY-DHX*HBGY-HGRG2*GBHX*DHY
          DDFGH[33]=DDF*DTGY*DTHY-DHY*HBGY-HGRG2*GBHY*DHY                                          ;//        DDFGH(33)=DDF*DTGY*DTHY-DHY*HBGY-HGRG2*GBHY*DHY
          DDFGH[34]=DDF*DTGZ*DTHY-DHZ*HBGY-HGRG2*GBHZ*DHY                                          ;//        DDFGH(34)=DDF*DTGZ*DTHY-DHZ*HBGY-HGRG2*GBHZ*DHY
          DDFGH[40]=DDF*DTGX*DTHZ-DHX*HBGZ-HGRG2*GBHX*DHZ                                          ;//        DDFGH(40)=DDF*DTGX*DTHZ-DHX*HBGZ-HGRG2*GBHX*DHZ
          DDFGH[41]=DDF*DTGY*DTHZ-DHY*HBGZ-HGRG2*GBHY*DHZ                                          ;//        DDFGH(41)=DDF*DTGY*DTHZ-DHY*HBGZ-HGRG2*GBHY*DHZ
          DDFGH[42]=DDF*DTGZ*DTHZ-DHZ*HBGZ-HGRG2*GBHZ*DHZ                                          ;//        DDFGH(42)=DDF*DTGZ*DTHZ-DHZ*HBGZ-HGRG2*GBHZ*DHZ
/// ddE/dH.dH                                                                                      ;//C ddE/dH.dH
          DDFGH[28]=DDF*DTHX*DTHX+TWO*DHX*GBHX                                                     ;//        DDFGH(28)=DDF*DTHX*DTHX+TWO*DHX*GBHX
          DDFGH[35]=DDF*DTHX*DTHY+DHX*GBHY+DHY*GBHX                                                ;//        DDFGH(35)=DDF*DTHX*DTHY+DHX*GBHY+DHY*GBHX
          DDFGH[36]=DDF*DTHY*DTHY+TWO*DHY*GBHY                                                     ;//        DDFGH(36)=DDF*DTHY*DTHY+TWO*DHY*GBHY
          DDFGH[43]=DDF*DTHX*DTHZ+DHX*GBHZ+DHZ*GBHX                                                ;//        DDFGH(43)=DDF*DTHX*DTHZ+DHX*GBHZ+DHZ*GBHX
          DDFGH[44]=DDF*DTHY*DTHZ+DHY*GBHZ+DHZ*GBHY                                                ;//        DDFGH(44)=DDF*DTHY*DTHZ+DHY*GBHZ+DHZ*GBHY
          DDFGH[45]=DDF*DTHZ*DTHZ+TWO*DHZ*GBHZ                                                     ;//        DDFGH(45)=DDF*DTHZ*DTHZ+TWO*DHZ*GBHZ
                                                                                                   ;//##IF MBOND
                                                                                                   ;//      IF (qMBSec) THEN
                                                                                                   ;//
                                                                                                   ;//         CALL MBSecP(heap(pDD1Sys(1)),heap(pDD1Vac(1)),
                                                                                                   ;//     &                   I,J,K,L,DDFGH)
                                                                                                   ;//
                                                                                                   ;//      ELSE
                                                                                                   ;//##ENDIF ! MBOND
///                                                                                                ;//C
/// Now scatter ddE/(dFGH)^2 through (dFGH/dRiRjRkRl)^2                                            ;//C Now scatter ddE/(dFGH)^2 through (dFGH/dRiRjRkRl)^2
///                                                                                                ;//C
                                                                                                   ;//##IF DIMB
                                                                                                   ;//          IF (QCMPCT) THEN
                                                                                                   ;//             CALL EPHCMP(I,J,K,L,DDFGH,DD1,HEAP(PINBCM),HEAP(PJNBCM))
                                                                                                   ;//          ELSE
                                                                                                   ;//##ENDIF ! DIMB
                                                                                                   ;//
            II=3*I-2                                                                               ;//          II=3*I-2
            JJ=3*J-2                                                                               ;//          JJ=3*J-2
            KK=3*K-2                                                                               ;//          KK=3*K-2
            LL=3*L-2                                                                               ;//          LL=3*L-2
            IJTEST=(J <  I)                                                                        ;//          IJTEST=(J.LT.I)
            IKTEST=(K <  I)                                                                        ;//          IKTEST=(K.LT.I)
            JKTEST=(K <  J)                                                                        ;//          JKTEST=(K.LT.J)
            ILTEST=(L <  I)                                                                        ;//          ILTEST=(L.LT.I)
            JLTEST=(L <  J)                                                                        ;//          JLTEST=(L.LT.J)
            KLTEST=(L <  K)                                                                        ;//          KLTEST=(L.LT.K)
///                                                                                                ;//C
            IADD=IUPT[II]+II                                                                       ;//          IADD=IUPT(II)+II
            DD1[IADD]=DD1[IADD]+DDFGH[1]                                                           ;//          DD1(IADD)=DD1(IADD)+DDFGH(1)
            IADD=IUPT[II+1]+II+1                                                                   ;//          IADD=IUPT(II+1)+II+1
            DD1[IADD]=DD1[IADD]+DDFGH[3]                                                           ;//          DD1(IADD)=DD1(IADD)+DDFGH(3)
            IADD=IUPT[II+2]+II+2                                                                   ;//          IADD=IUPT(II+2)+II+2
            DD1[IADD]=DD1[IADD]+DDFGH[6]                                                           ;//          DD1(IADD)=DD1(IADD)+DDFGH(6)
            IADD=IUPT[II]+II+1                                                                     ;//          IADD=IUPT(II)+II+1
            DD1[IADD]=DD1[IADD]+DDFGH[2]                                                           ;//          DD1(IADD)=DD1(IADD)+DDFGH(2)
            IADD=IUPT[II]+II+2                                                                     ;//          IADD=IUPT(II)+II+2
            DD1[IADD]=DD1[IADD]+DDFGH[4]                                                           ;//          DD1(IADD)=DD1(IADD)+DDFGH(4)
            IADD=IUPT[II+1]+II+2                                                                   ;//          IADD=IUPT(II+1)+II+2
            DD1[IADD]=DD1[IADD]+DDFGH[5]                                                           ;//          DD1(IADD)=DD1(IADD)+DDFGH(5)
///                                                                                                ;//C
            IADD=IUPT[LL]+LL                                                                       ;//          IADD=IUPT(LL)+LL
            DD1[IADD]=DD1[IADD]+DDFGH[28]                                                          ;//          DD1(IADD)=DD1(IADD)+DDFGH(28)
            IADD=IUPT[LL+1]+LL+1                                                                   ;//          IADD=IUPT(LL+1)+LL+1
            DD1[IADD]=DD1[IADD]+DDFGH[36]                                                          ;//          DD1(IADD)=DD1(IADD)+DDFGH(36)
            IADD=IUPT[LL+2]+LL+2                                                                   ;//          IADD=IUPT(LL+2)+LL+2
            DD1[IADD]=DD1[IADD]+DDFGH[45]                                                          ;//          DD1(IADD)=DD1(IADD)+DDFGH(45)
            IADD=IUPT[LL]+LL+1                                                                     ;//          IADD=IUPT(LL)+LL+1
            DD1[IADD]=DD1[IADD]+DDFGH[35]                                                          ;//          DD1(IADD)=DD1(IADD)+DDFGH(35)
            IADD=IUPT[LL]+LL+2                                                                     ;//          IADD=IUPT(LL)+LL+2
            DD1[IADD]=DD1[IADD]+DDFGH[43]                                                          ;//          DD1(IADD)=DD1(IADD)+DDFGH(43)
            IADD=IUPT[LL+1]+LL+2                                                                   ;//          IADD=IUPT(LL+1)+LL+2
            DD1[IADD]=DD1[IADD]+DDFGH[44]                                                          ;//          DD1(IADD)=DD1(IADD)+DDFGH(44)
///                                                                                                ;//C
            IADD=IUPT[JJ]+JJ                                                                       ;//          IADD=IUPT(JJ)+JJ
            DD1[IADD]=DD1[IADD]+DDFGH[1]+DDFGH[10]-DDFGH[7]-DDFGH[7]                               ;//          DD1(IADD)=DD1(IADD)+DDFGH(1)+DDFGH(10)-DDFGH(7)-DDFGH(7)
            IADD=IUPT[JJ+1]+JJ+1                                                                   ;//          IADD=IUPT(JJ+1)+JJ+1
            DD1[IADD]=DD1[IADD]+DDFGH[3]+DDFGH[15]-DDFGH[12]-DDFGH[12]                             ;//          DD1(IADD)=DD1(IADD)+DDFGH(3)+DDFGH(15)-DDFGH(12)-DDFGH(12)
            IADD=IUPT[JJ+2]+JJ+2                                                                   ;//          IADD=IUPT(JJ+2)+JJ+2
            DD1[IADD]=DD1[IADD]+DDFGH[6]+DDFGH[21]-DDFGH[18]-DDFGH[18]                             ;//          DD1(IADD)=DD1(IADD)+DDFGH(6)+DDFGH(21)-DDFGH(18)-DDFGH(18)
            IADD=IUPT[JJ]+JJ+1                                                                     ;//          IADD=IUPT(JJ)+JJ+1
            DD1[IADD]=DD1[IADD]+DDFGH[2]+DDFGH[14]-DDFGH[11]-DDFGH[8]                              ;//          DD1(IADD)=DD1(IADD)+DDFGH(2)+DDFGH(14)-DDFGH(11)-DDFGH(8)
            IADD=IUPT[JJ]+JJ+2                                                                     ;//          IADD=IUPT(JJ)+JJ+2
            DD1[IADD]=DD1[IADD]+DDFGH[4]+DDFGH[19]-DDFGH[16]-DDFGH[9]                              ;//          DD1(IADD)=DD1(IADD)+DDFGH(4)+DDFGH(19)-DDFGH(16)-DDFGH(9)
            IADD=IUPT[JJ+1]+JJ+2                                                                   ;//          IADD=IUPT(JJ+1)+JJ+2
            DD1[IADD]=DD1[IADD]+DDFGH[5]+DDFGH[20]-DDFGH[17]-DDFGH[13]                             ;//          DD1(IADD)=DD1(IADD)+DDFGH(5)+DDFGH(20)-DDFGH(17)-DDFGH(13)
///                                                                                                ;//C
            IADD=IUPT[KK]+KK                                                                       ;//          IADD=IUPT(KK)+KK
            DD1[IADD]=DD1[IADD]+DDFGH[28]+DDFGH[10]+DDFGH[25]+DDFGH[25]                            ;//          DD1(IADD)=DD1(IADD)+DDFGH(28)+DDFGH(10)+DDFGH(25)+DDFGH(25)
            IADD=IUPT[KK+1]+KK+1                                                                   ;//          IADD=IUPT(KK+1)+KK+1
            DD1[IADD]=DD1[IADD]+DDFGH[36]+DDFGH[15]+DDFGH[33]+DDFGH[33]                            ;//          DD1(IADD)=DD1(IADD)+DDFGH(36)+DDFGH(15)+DDFGH(33)+DDFGH(33)
            IADD=IUPT[KK+2]+KK+2                                                                   ;//          IADD=IUPT(KK+2)+KK+2
            DD1[IADD]=DD1[IADD]+DDFGH[45]+DDFGH[21]+DDFGH[42]+DDFGH[42]                            ;//          DD1(IADD)=DD1(IADD)+DDFGH(45)+DDFGH(21)+DDFGH(42)+DDFGH(42)
            IADD=IUPT[KK]+KK+1                                                                     ;//          IADD=IUPT(KK)+KK+1
            DD1[IADD]=DD1[IADD]+DDFGH[35]+DDFGH[14]+DDFGH[32]+DDFGH[26]                            ;//          DD1(IADD)=DD1(IADD)+DDFGH(35)+DDFGH(14)+DDFGH(32)+DDFGH(26)
            IADD=IUPT[KK]+KK+2                                                                     ;//          IADD=IUPT(KK)+KK+2
            DD1[IADD]=DD1[IADD]+DDFGH[43]+DDFGH[19]+DDFGH[40]+DDFGH[27]                            ;//          DD1(IADD)=DD1(IADD)+DDFGH(43)+DDFGH(19)+DDFGH(40)+DDFGH(27)
            IADD=IUPT[KK+1]+KK+2                                                                   ;//          IADD=IUPT(KK+1)+KK+2
            DD1[IADD]=DD1[IADD]+DDFGH[44]+DDFGH[20]+DDFGH[41]+DDFGH[34]                            ;//          DD1(IADD)=DD1(IADD)+DDFGH(44)+DDFGH(20)+DDFGH(41)+DDFGH(34)
///                                                                                                ;//C
            if (IJTEST) {                                                                          ;//          IF (IJTEST) THEN
              IADD=IUPT[JJ]+II                                                                     ;//            IADD=IUPT(JJ)+II
              DD1[IADD]=DD1[IADD]+DDFGH[7]-DDFGH[1]                                                ;//            DD1(IADD)=DD1(IADD)+DDFGH(7)-DDFGH(1)
              IADD=IUPT[JJ+1]+II+1                                                                 ;//            IADD=IUPT(JJ+1)+II+1
              DD1[IADD]=DD1[IADD]+DDFGH[12]-DDFGH[3]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(12)-DDFGH(3)
              IADD=IUPT[JJ+2]+II+2                                                                 ;//            IADD=IUPT(JJ+2)+II+2
              DD1[IADD]=DD1[IADD]+DDFGH[18]-DDFGH[6]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(18)-DDFGH(6)
              IADD=IUPT[JJ]+II+1                                                                   ;//            IADD=IUPT(JJ)+II+1
              DD1[IADD]=DD1[IADD]+DDFGH[8]-DDFGH[2]                                                ;//            DD1(IADD)=DD1(IADD)+DDFGH(8)-DDFGH(2)
              IADD=IUPT[JJ+1]+II                                                                   ;//            IADD=IUPT(JJ+1)+II
              DD1[IADD]=DD1[IADD]+DDFGH[11]-DDFGH[2]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(11)-DDFGH(2)
              IADD=IUPT[JJ]+II+2                                                                   ;//            IADD=IUPT(JJ)+II+2
              DD1[IADD]=DD1[IADD]+DDFGH[9]-DDFGH[4]                                                ;//            DD1(IADD)=DD1(IADD)+DDFGH(9)-DDFGH(4)
              IADD=IUPT[JJ+2]+II                                                                   ;//            IADD=IUPT(JJ+2)+II
              DD1[IADD]=DD1[IADD]+DDFGH[16]-DDFGH[4]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(16)-DDFGH(4)
              IADD=IUPT[JJ+1]+II+2                                                                 ;//            IADD=IUPT(JJ+1)+II+2
              DD1[IADD]=DD1[IADD]+DDFGH[13]-DDFGH[5]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(13)-DDFGH(5)
              IADD=IUPT[JJ+2]+II+1                                                                 ;//            IADD=IUPT(JJ+2)+II+1
              DD1[IADD]=DD1[IADD]+DDFGH[17]-DDFGH[5]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(17)-DDFGH(5)
            } else {                                                                               ;//          ELSE
              IADD=IUPT[II]+JJ                                                                     ;//            IADD=IUPT(II)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[7]-DDFGH[1]                                                ;//            DD1(IADD)=DD1(IADD)+DDFGH(7)-DDFGH(1)
              IADD=IUPT[II+1]+JJ+1                                                                 ;//            IADD=IUPT(II+1)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[12]-DDFGH[3]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(12)-DDFGH(3)
              IADD=IUPT[II+2]+JJ+2                                                                 ;//            IADD=IUPT(II+2)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[18]-DDFGH[6]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(18)-DDFGH(6)
              IADD=IUPT[II+1]+JJ                                                                   ;//            IADD=IUPT(II+1)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[8]-DDFGH[2]                                                ;//            DD1(IADD)=DD1(IADD)+DDFGH(8)-DDFGH(2)
              IADD=IUPT[II]+JJ+1                                                                   ;//            IADD=IUPT(II)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[11]-DDFGH[2]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(11)-DDFGH(2)
              IADD=IUPT[II+2]+JJ                                                                   ;//            IADD=IUPT(II+2)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[9]-DDFGH[4]                                                ;//            DD1(IADD)=DD1(IADD)+DDFGH(9)-DDFGH(4)
              IADD=IUPT[II]+JJ+2                                                                   ;//            IADD=IUPT(II)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[16]-DDFGH[4]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(16)-DDFGH(4)
              IADD=IUPT[II+2]+JJ+1                                                                 ;//            IADD=IUPT(II+2)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[13]-DDFGH[5]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(13)-DDFGH(5)
              IADD=IUPT[II+1]+JJ+2                                                                 ;//            IADD=IUPT(II+1)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[17]-DDFGH[5]                                               ;//            DD1(IADD)=DD1(IADD)+DDFGH(17)-DDFGH(5)
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if (IKTEST) {                                                                          ;//          IF (IKTEST) THEN
              IADD=IUPT[KK]+II                                                                     ;//            IADD=IUPT(KK)+II
              DD1[IADD]=DD1[IADD]-DDFGH[7]-DDFGH[22]                                               ;//            DD1(IADD)=DD1(IADD)-DDFGH(7)-DDFGH(22)
              IADD=IUPT[KK+1]+II+1                                                                 ;//            IADD=IUPT(KK+1)+II+1
              DD1[IADD]=DD1[IADD]-DDFGH[12]-DDFGH[30]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(12)-DDFGH(30)
              IADD=IUPT[KK+2]+II+2                                                                 ;//            IADD=IUPT(KK+2)+II+2
              DD1[IADD]=DD1[IADD]-DDFGH[18]-DDFGH[39]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(18)-DDFGH(39)
              IADD=IUPT[KK]+II+1                                                                   ;//            IADD=IUPT(KK)+II+1
              DD1[IADD]=DD1[IADD]-DDFGH[8]-DDFGH[23]                                               ;//            DD1(IADD)=DD1(IADD)-DDFGH(8)-DDFGH(23)
              IADD=IUPT[KK+1]+II                                                                   ;//            IADD=IUPT(KK+1)+II
              DD1[IADD]=DD1[IADD]-DDFGH[11]-DDFGH[29]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(11)-DDFGH(29)
              IADD=IUPT[KK]+II+2                                                                   ;//            IADD=IUPT(KK)+II+2
              DD1[IADD]=DD1[IADD]-DDFGH[9]-DDFGH[24]                                               ;//            DD1(IADD)=DD1(IADD)-DDFGH(9)-DDFGH(24)
              IADD=IUPT[KK+2]+II                                                                   ;//            IADD=IUPT(KK+2)+II
              DD1[IADD]=DD1[IADD]-DDFGH[16]-DDFGH[37]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(16)-DDFGH(37)
              IADD=IUPT[KK+1]+II+2                                                                 ;//            IADD=IUPT(KK+1)+II+2
              DD1[IADD]=DD1[IADD]-DDFGH[13]-DDFGH[31]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(13)-DDFGH(31)
              IADD=IUPT[KK+2]+II+1                                                                 ;//            IADD=IUPT(KK+2)+II+1
              DD1[IADD]=DD1[IADD]-DDFGH[17]-DDFGH[38]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(17)-DDFGH(38)
            } else {                                                                               ;//          ELSE
              IADD=IUPT[II]+KK                                                                     ;//            IADD=IUPT(II)+KK
              DD1[IADD]=DD1[IADD]-DDFGH[7]-DDFGH[22]                                               ;//            DD1(IADD)=DD1(IADD)-DDFGH(7)-DDFGH(22)
              IADD=IUPT[II+1]+KK+1                                                                 ;//            IADD=IUPT(II+1)+KK+1
              DD1[IADD]=DD1[IADD]-DDFGH[12]-DDFGH[30]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(12)-DDFGH(30)
              IADD=IUPT[II+2]+KK+2                                                                 ;//            IADD=IUPT(II+2)+KK+2
              DD1[IADD]=DD1[IADD]-DDFGH[18]-DDFGH[39]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(18)-DDFGH(39)
              IADD=IUPT[II+1]+KK                                                                   ;//            IADD=IUPT(II+1)+KK
              DD1[IADD]=DD1[IADD]-DDFGH[8]-DDFGH[23]                                               ;//            DD1(IADD)=DD1(IADD)-DDFGH(8)-DDFGH(23)
              IADD=IUPT[II]+KK+1                                                                   ;//            IADD=IUPT(II)+KK+1
              DD1[IADD]=DD1[IADD]-DDFGH[11]-DDFGH[29]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(11)-DDFGH(29)
              IADD=IUPT[II+2]+KK                                                                   ;//            IADD=IUPT(II+2)+KK
              DD1[IADD]=DD1[IADD]-DDFGH[9]-DDFGH[24]                                               ;//            DD1(IADD)=DD1(IADD)-DDFGH(9)-DDFGH(24)
              IADD=IUPT[II]+KK+2                                                                   ;//            IADD=IUPT(II)+KK+2
              DD1[IADD]=DD1[IADD]-DDFGH[16]-DDFGH[37]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(16)-DDFGH(37)
              IADD=IUPT[II+2]+KK+1                                                                 ;//            IADD=IUPT(II+2)+KK+1
              DD1[IADD]=DD1[IADD]-DDFGH[13]-DDFGH[31]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(13)-DDFGH(31)
              IADD=IUPT[II+1]+KK+2                                                                 ;//            IADD=IUPT(II+1)+KK+2
              DD1[IADD]=DD1[IADD]-DDFGH[17]-DDFGH[38]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(17)-DDFGH(38)
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if (ILTEST) {                                                                          ;//          IF (ILTEST) THEN
              IADD=IUPT[LL]+II                                                                     ;//            IADD=IUPT(LL)+II
              DD1[IADD]=DD1[IADD]+DDFGH[22]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(22)
              IADD=IUPT[LL+1]+II+1                                                                 ;//            IADD=IUPT(LL+1)+II+1
              DD1[IADD]=DD1[IADD]+DDFGH[30]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(30)
              IADD=IUPT[LL+2]+II+2                                                                 ;//            IADD=IUPT(LL+2)+II+2
              DD1[IADD]=DD1[IADD]+DDFGH[39]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(39)
              IADD=IUPT[LL]+II+1                                                                   ;//            IADD=IUPT(LL)+II+1
              DD1[IADD]=DD1[IADD]+DDFGH[23]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(23)
              IADD=IUPT[LL+1]+II                                                                   ;//            IADD=IUPT(LL+1)+II
              DD1[IADD]=DD1[IADD]+DDFGH[29]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(29)
              IADD=IUPT[LL]+II+2                                                                   ;//            IADD=IUPT(LL)+II+2
              DD1[IADD]=DD1[IADD]+DDFGH[24]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(24)
              IADD=IUPT[LL+2]+II                                                                   ;//            IADD=IUPT(LL+2)+II
              DD1[IADD]=DD1[IADD]+DDFGH[37]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(37)
              IADD=IUPT[LL+1]+II+2                                                                 ;//            IADD=IUPT(LL+1)+II+2
              DD1[IADD]=DD1[IADD]+DDFGH[31]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(31)
              IADD=IUPT[LL+2]+II+1                                                                 ;//            IADD=IUPT(LL+2)+II+1
              DD1[IADD]=DD1[IADD]+DDFGH[38]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(38)
            } else {                                                                               ;//          ELSE
              IADD=IUPT[II]+LL                                                                     ;//            IADD=IUPT(II)+LL
              DD1[IADD]=DD1[IADD]+DDFGH[22]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(22)
              IADD=IUPT[II+1]+LL+1                                                                 ;//            IADD=IUPT(II+1)+LL+1
              DD1[IADD]=DD1[IADD]+DDFGH[30]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(30)
              IADD=IUPT[II+2]+LL+2                                                                 ;//            IADD=IUPT(II+2)+LL+2
              DD1[IADD]=DD1[IADD]+DDFGH[39]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(39)
              IADD=IUPT[II+1]+LL                                                                   ;//            IADD=IUPT(II+1)+LL
              DD1[IADD]=DD1[IADD]+DDFGH[23]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(23)
              IADD=IUPT[II]+LL+1                                                                   ;//            IADD=IUPT(II)+LL+1
              DD1[IADD]=DD1[IADD]+DDFGH[29]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(29)
              IADD=IUPT[II+2]+LL                                                                   ;//            IADD=IUPT(II+2)+LL
              DD1[IADD]=DD1[IADD]+DDFGH[24]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(24)
              IADD=IUPT[II]+LL+2                                                                   ;//            IADD=IUPT(II)+LL+2
              DD1[IADD]=DD1[IADD]+DDFGH[37]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(37)
              IADD=IUPT[II+2]+LL+1                                                                 ;//            IADD=IUPT(II+2)+LL+1
              DD1[IADD]=DD1[IADD]+DDFGH[31]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(31)
              IADD=IUPT[II+1]+LL+2                                                                 ;//            IADD=IUPT(II+1)+LL+2
              DD1[IADD]=DD1[IADD]+DDFGH[38]                                                        ;//            DD1(IADD)=DD1(IADD)+DDFGH(38)
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if (JKTEST) {                                                                          ;//          IF (JKTEST) THEN
              IADD=IUPT[KK]+JJ                                                                     ;//            IADD=IUPT(KK)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[7]+DDFGH[22]-DDFGH[10]-DDFGH[25]                           ;//            DD1(IADD)=DD1(IADD)+DDFGH(7)+DDFGH(22)-DDFGH(10)-DDFGH(25)
              IADD=IUPT[KK+1]+JJ+1                                                                 ;//            IADD=IUPT(KK+1)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[12]+DDFGH[30]-DDFGH[15]-DDFGH[33]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(12)+DDFGH(30)-DDFGH(15)-DDFGH(33)
              IADD=IUPT[KK+2]+JJ+2                                                                 ;//            IADD=IUPT(KK+2)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[18]+DDFGH[39]-DDFGH[21]-DDFGH[42]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(18)+DDFGH(39)-DDFGH(21)-DDFGH(42)
              IADD=IUPT[KK]+JJ+1                                                                   ;//            IADD=IUPT(KK)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[8]+DDFGH[23]-DDFGH[14]-DDFGH[26]                           ;//            DD1(IADD)=DD1(IADD)+DDFGH(8)+DDFGH(23)-DDFGH(14)-DDFGH(26)
              IADD=IUPT[KK+1]+JJ                                                                   ;//            IADD=IUPT(KK+1)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[11]+DDFGH[29]-DDFGH[14]-DDFGH[32]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(11)+DDFGH(29)-DDFGH(14)-DDFGH(32)
              IADD=IUPT[KK]+JJ+2                                                                   ;//            IADD=IUPT(KK)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[9]+DDFGH[24]-DDFGH[19]-DDFGH[27]                           ;//            DD1(IADD)=DD1(IADD)+DDFGH(9)+DDFGH(24)-DDFGH(19)-DDFGH(27)
              IADD=IUPT[KK+2]+JJ                                                                   ;//            IADD=IUPT(KK+2)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[16]+DDFGH[37]-DDFGH[19]-DDFGH[40]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(16)+DDFGH(37)-DDFGH(19)-DDFGH(40)
              IADD=IUPT[KK+1]+JJ+2                                                                 ;//            IADD=IUPT(KK+1)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[13]+DDFGH[31]-DDFGH[20]-DDFGH[34]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(13)+DDFGH(31)-DDFGH(20)-DDFGH(34)
              IADD=IUPT[KK+2]+JJ+1                                                                 ;//            IADD=IUPT(KK+2)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[17]+DDFGH[38]-DDFGH[20]-DDFGH[41]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(17)+DDFGH(38)-DDFGH(20)-DDFGH(41)
            } else {                                                                               ;//          ELSE
              IADD=IUPT[JJ]+KK                                                                     ;//            IADD=IUPT(JJ)+KK
              DD1[IADD]=DD1[IADD]+DDFGH[7]+DDFGH[22]-DDFGH[10]-DDFGH[25]                           ;//            DD1(IADD)=DD1(IADD)+DDFGH(7)+DDFGH(22)-DDFGH(10)-DDFGH(25)
              IADD=IUPT[JJ+1]+KK+1                                                                 ;//            IADD=IUPT(JJ+1)+KK+1
              DD1[IADD]=DD1[IADD]+DDFGH[12]+DDFGH[30]-DDFGH[15]-DDFGH[33]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(12)+DDFGH(30)-DDFGH(15)-DDFGH(33)
              IADD=IUPT[JJ+2]+KK+2                                                                 ;//            IADD=IUPT(JJ+2)+KK+2
              DD1[IADD]=DD1[IADD]+DDFGH[18]+DDFGH[39]-DDFGH[21]-DDFGH[42]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(18)+DDFGH(39)-DDFGH(21)-DDFGH(42)
              IADD=IUPT[JJ+1]+KK                                                                   ;//            IADD=IUPT(JJ+1)+KK
              DD1[IADD]=DD1[IADD]+DDFGH[8]+DDFGH[23]-DDFGH[14]-DDFGH[26]                           ;//            DD1(IADD)=DD1(IADD)+DDFGH(8)+DDFGH(23)-DDFGH(14)-DDFGH(26)
              IADD=IUPT[JJ]+KK+1                                                                   ;//            IADD=IUPT(JJ)+KK+1
              DD1[IADD]=DD1[IADD]+DDFGH[11]+DDFGH[29]-DDFGH[14]-DDFGH[32]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(11)+DDFGH(29)-DDFGH(14)-DDFGH(32)
              IADD=IUPT[JJ+2]+KK                                                                   ;//            IADD=IUPT(JJ+2)+KK
              DD1[IADD]=DD1[IADD]+DDFGH[9]+DDFGH[24]-DDFGH[19]-DDFGH[27]                           ;//            DD1(IADD)=DD1(IADD)+DDFGH(9)+DDFGH(24)-DDFGH(19)-DDFGH(27)
              IADD=IUPT[JJ]+KK+2                                                                   ;//            IADD=IUPT(JJ)+KK+2
              DD1[IADD]=DD1[IADD]+DDFGH[16]+DDFGH[37]-DDFGH[19]-DDFGH[40]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(16)+DDFGH(37)-DDFGH(19)-DDFGH(40)
              IADD=IUPT[JJ+2]+KK+1                                                                 ;//            IADD=IUPT(JJ+2)+KK+1
              DD1[IADD]=DD1[IADD]+DDFGH[13]+DDFGH[31]-DDFGH[20]-DDFGH[34]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(13)+DDFGH(31)-DDFGH(20)-DDFGH(34)
              IADD=IUPT[JJ+1]+KK+2                                                                 ;//            IADD=IUPT(JJ+1)+KK+2
              DD1[IADD]=DD1[IADD]+DDFGH[17]+DDFGH[38]-DDFGH[20]-DDFGH[41]                          ;//            DD1(IADD)=DD1(IADD)+DDFGH(17)+DDFGH(38)-DDFGH(20)-DDFGH(41)
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if (JLTEST) {                                                                          ;//          IF (JLTEST) THEN
              IADD=IUPT[LL]+JJ                                                                     ;//            IADD=IUPT(LL)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[25]-DDFGH[22]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(25)-DDFGH(22)
              IADD=IUPT[LL+1]+JJ+1                                                                 ;//            IADD=IUPT(LL+1)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[33]-DDFGH[30]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(33)-DDFGH(30)
              IADD=IUPT[LL+2]+JJ+2                                                                 ;//            IADD=IUPT(LL+2)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[42]-DDFGH[39]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(42)-DDFGH(39)
              IADD=IUPT[LL]+JJ+1                                                                   ;//            IADD=IUPT(LL)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[26]-DDFGH[23]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(26)-DDFGH(23)
              IADD=IUPT[LL+1]+JJ                                                                   ;//            IADD=IUPT(LL+1)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[32]-DDFGH[29]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(32)-DDFGH(29)
              IADD=IUPT[LL]+JJ+2                                                                   ;//            IADD=IUPT(LL)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[27]-DDFGH[24]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(27)-DDFGH(24)
              IADD=IUPT[LL+2]+JJ                                                                   ;//            IADD=IUPT(LL+2)+JJ
              DD1[IADD]=DD1[IADD]+DDFGH[40]-DDFGH[37]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(40)-DDFGH(37)
              IADD=IUPT[LL+1]+JJ+2                                                                 ;//            IADD=IUPT(LL+1)+JJ+2
              DD1[IADD]=DD1[IADD]+DDFGH[34]-DDFGH[31]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(34)-DDFGH(31)
              IADD=IUPT[LL+2]+JJ+1                                                                 ;//            IADD=IUPT(LL+2)+JJ+1
              DD1[IADD]=DD1[IADD]+DDFGH[41]-DDFGH[38]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(41)-DDFGH(38)
            } else {                                                                               ;//          ELSE
              IADD=IUPT[JJ]+LL                                                                     ;//            IADD=IUPT(JJ)+LL
              DD1[IADD]=DD1[IADD]+DDFGH[25]-DDFGH[22]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(25)-DDFGH(22)
              IADD=IUPT[JJ+1]+LL+1                                                                 ;//            IADD=IUPT(JJ+1)+LL+1
              DD1[IADD]=DD1[IADD]+DDFGH[33]-DDFGH[30]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(33)-DDFGH(30)
              IADD=IUPT[JJ+2]+LL+2                                                                 ;//            IADD=IUPT(JJ+2)+LL+2
              DD1[IADD]=DD1[IADD]+DDFGH[42]-DDFGH[39]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(42)-DDFGH(39)
              IADD=IUPT[JJ+1]+LL                                                                   ;//            IADD=IUPT(JJ+1)+LL
              DD1[IADD]=DD1[IADD]+DDFGH[26]-DDFGH[23]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(26)-DDFGH(23)
              IADD=IUPT[JJ]+LL+1                                                                   ;//            IADD=IUPT(JJ)+LL+1
              DD1[IADD]=DD1[IADD]+DDFGH[32]-DDFGH[29]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(32)-DDFGH(29)
              IADD=IUPT[JJ+2]+LL                                                                   ;//            IADD=IUPT(JJ+2)+LL
              DD1[IADD]=DD1[IADD]+DDFGH[27]-DDFGH[24]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(27)-DDFGH(24)
              IADD=IUPT[JJ]+LL+2                                                                   ;//            IADD=IUPT(JJ)+LL+2
              DD1[IADD]=DD1[IADD]+DDFGH[40]-DDFGH[37]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(40)-DDFGH(37)
              IADD=IUPT[JJ+2]+LL+1                                                                 ;//            IADD=IUPT(JJ+2)+LL+1
              DD1[IADD]=DD1[IADD]+DDFGH[34]-DDFGH[31]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(34)-DDFGH(31)
              IADD=IUPT[JJ+1]+LL+2                                                                 ;//            IADD=IUPT(JJ+1)+LL+2
              DD1[IADD]=DD1[IADD]+DDFGH[41]-DDFGH[38]                                              ;//            DD1(IADD)=DD1(IADD)+DDFGH(41)-DDFGH(38)
            }                                                                                      ;//          ENDIF
///                                                                                                ;//C
            if (KLTEST) {                                                                          ;//          IF (KLTEST) THEN
              IADD=IUPT[LL]+KK                                                                     ;//            IADD=IUPT(LL)+KK
              DD1[IADD]=DD1[IADD]-DDFGH[25]-DDFGH[28]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(25)-DDFGH(28)
              IADD=IUPT[LL+1]+KK+1                                                                 ;//            IADD=IUPT(LL+1)+KK+1
              DD1[IADD]=DD1[IADD]-DDFGH[33]-DDFGH[36]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(33)-DDFGH(36)
              IADD=IUPT[LL+2]+KK+2                                                                 ;//            IADD=IUPT(LL+2)+KK+2
              DD1[IADD]=DD1[IADD]-DDFGH[42]-DDFGH[45]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(42)-DDFGH(45)
              IADD=IUPT[LL]+KK+1                                                                   ;//            IADD=IUPT(LL)+KK+1
              DD1[IADD]=DD1[IADD]-DDFGH[26]-DDFGH[35]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(26)-DDFGH(35)
              IADD=IUPT[LL+1]+KK                                                                   ;//            IADD=IUPT(LL+1)+KK
              DD1[IADD]=DD1[IADD]-DDFGH[32]-DDFGH[35]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(32)-DDFGH(35)
              IADD=IUPT[LL]+KK+2                                                                   ;//            IADD=IUPT(LL)+KK+2
              DD1[IADD]=DD1[IADD]-DDFGH[27]-DDFGH[43]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(27)-DDFGH(43)
              IADD=IUPT[LL+2]+KK                                                                   ;//            IADD=IUPT(LL+2)+KK
              DD1[IADD]=DD1[IADD]-DDFGH[40]-DDFGH[43]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(40)-DDFGH(43)
              IADD=IUPT[LL+1]+KK+2                                                                 ;//            IADD=IUPT(LL+1)+KK+2
              DD1[IADD]=DD1[IADD]-DDFGH[34]-DDFGH[44]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(34)-DDFGH(44)
              IADD=IUPT[LL+2]+KK+1                                                                 ;//            IADD=IUPT(LL+2)+KK+1
              DD1[IADD]=DD1[IADD]-DDFGH[41]-DDFGH[44]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(41)-DDFGH(44)
            } else {                                                                               ;//          ELSE
              IADD=IUPT[KK]+LL                                                                     ;//            IADD=IUPT(KK)+LL
              DD1[IADD]=DD1[IADD]-DDFGH[25]-DDFGH[28]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(25)-DDFGH(28)
              IADD=IUPT[KK+1]+LL+1                                                                 ;//            IADD=IUPT(KK+1)+LL+1
              DD1[IADD]=DD1[IADD]-DDFGH[33]-DDFGH[36]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(33)-DDFGH(36)
              IADD=IUPT[KK+2]+LL+2                                                                 ;//            IADD=IUPT(KK+2)+LL+2
              DD1[IADD]=DD1[IADD]-DDFGH[42]-DDFGH[45]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(42)-DDFGH(45)
              IADD=IUPT[KK+1]+LL                                                                   ;//            IADD=IUPT(KK+1)+LL
              DD1[IADD]=DD1[IADD]-DDFGH[26]-DDFGH[35]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(26)-DDFGH(35)
              IADD=IUPT[KK]+LL+1                                                                   ;//            IADD=IUPT(KK)+LL+1
              DD1[IADD]=DD1[IADD]-DDFGH[32]-DDFGH[35]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(32)-DDFGH(35)
              IADD=IUPT[KK+2]+LL                                                                   ;//            IADD=IUPT(KK+2)+LL
              DD1[IADD]=DD1[IADD]-DDFGH[27]-DDFGH[43]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(27)-DDFGH(43)
              IADD=IUPT[KK]+LL+2                                                                   ;//            IADD=IUPT(KK)+LL+2
              DD1[IADD]=DD1[IADD]-DDFGH[40]-DDFGH[43]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(40)-DDFGH(43)
              IADD=IUPT[KK+2]+LL+1                                                                 ;//            IADD=IUPT(KK+2)+LL+1
              DD1[IADD]=DD1[IADD]-DDFGH[34]-DDFGH[44]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(34)-DDFGH(44)
              IADD=IUPT[KK+1]+LL+2                                                                 ;//            IADD=IUPT(KK+1)+LL+2
              DD1[IADD]=DD1[IADD]-DDFGH[41]-DDFGH[44]                                              ;//            DD1(IADD)=DD1(IADD)-DDFGH(41)-DDFGH(44)
            }                                                                                      ;//          ENDIF
                                                                                                   ;//
                                                                                                   ;//##IF DIMB
                                                                                                   ;//          ENDIF
                                                                                                   ;//##ENDIF ! DIMB
                                                                                                   ;//##IF MBOND
                                                                                                   ;//      ENDIF
                                                                                                   ;//##ENDIF ! MBOND
                                                                                                   ;//
          }                                                                                        ;//        ENDIF
                                                                                                   ;//##IF BLOCK
                                                                                                   ;//        ENDIF
                                                                                                   ;//##ENDIF ! BLOCK
///                                                                                                ;//C
          goto160:                                                                                 ;//  160   CONTINUE
        }                                                                                          ;//   10 CONTINUE
///                                                                                                ;//C
        NWARN=NWARN+NWARNX                                                                         ;//      NWARN=NWARN+NWARNX
//      if(NWARN >  5  &&   WRNLEV >= 2) WRITE(OUTU,170) NWARN                                     ;//      IF(NWARN.GT.5 .AND. WRNLEV.GE.2) WRITE(OUTU,170) NWARN
//  170 FORMAT(' TOTAL OF',I6,' WARNINGS FROM EPHI')                                               ;//  170 FORMAT(' TOTAL OF',I6,' WARNINGS FROM EPHI')
///                                                                                                ;//C
        return                                                                                     ;//      RETURN
    }                                                                                               //      END
}
}
}
