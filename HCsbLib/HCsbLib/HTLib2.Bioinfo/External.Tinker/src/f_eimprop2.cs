using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Tinker
{
public partial class Src
{
///                                                                         ;//c
///                                                                         ;//c
///   ###################################################                   ;//c     ###################################################
///   ##  COPYRIGHT (C)  1993  by  Jay William Ponder  ##                   ;//c     ##  COPYRIGHT (C)  1993  by  Jay William Ponder  ##
///   ##              All Rights Reserved              ##                   ;//c     ##              All Rights Reserved              ##
///   ###################################################                   ;//c     ###################################################
///                                                                         ;//c
///   ###############################################################       ;//c     ###############################################################
///   ##                                                           ##       ;//c     ##                                                           ##
///   ##  subroutine eimprop2  --  atomwise imp. dihedral Hessian  ##       ;//c     ##  subroutine eimprop2  --  atomwise imp. dihedral Hessian  ##
///   ##                                                           ##       ;//c     ##                                                           ##
///   ###############################################################       ;//c     ###############################################################
///                                                                         ;//c
///                                                                         ;//c
///   "eimprop2" calculates second derivatives of the improper              ;//c     "eimprop2" calculates second derivatives of the improper
///   dihedral angle energy for a single atom                               ;//c     dihedral angle energy for a single atom
///                                                                         ;//c
///                                                                         ;//c
    public void eimprop2(int i) {                                           ;//      subroutine eimprop2 (i)
                                                                            ;//      implicit none
                                                                            ;//      include 'sizes.i'
                                                                            ;//      include 'atoms.i'
                                                                            ;//      include 'bound.i'
                                                                            ;//      include 'group.i'
                                                                            ;//      include 'hessn.i'
                                                                            ;//      include 'improp.i'
                                                                            ;//      include 'math.i'
                                                                            ;//      include 'torpot.i'
                                                                            ;//      integer i,kiprop
      int ia,ib,ic,id                                                       ;//      integer ia,ib,ic,id
      double ideal,force                                                    ;//      real*8 ideal,force
      double angle,dt,fgrp                                                  ;//      real*8 angle,dt,fgrp
      double dedphi,d2edphi2                                                ;//      real*8 dedphi,d2edphi2
      double sine,cosine                                                    ;//      real*8 sine,cosine
      double xia,yia,zia                                                    ;//      real*8 xia,yia,zia
      double xib,yib,zib                                                    ;//      real*8 xib,yib,zib
      double xic,yic,zic                                                    ;//      real*8 xic,yic,zic
      double xid,yid,zid                                                    ;//      real*8 xid,yid,zid
      double xba,yba,zba                                                    ;//      real*8 xba,yba,zba
      double xcb,ycb,zcb                                                    ;//      real*8 xcb,ycb,zcb
      double xdc,ydc,zdc                                                    ;//      real*8 xdc,ydc,zdc
      double xca,yca,zca                                                    ;//      real*8 xca,yca,zca
      double xdb,ydb,zdb                                                    ;//      real*8 xdb,ydb,zdb
      double xt,yt,zt,xu,yu,zu                                              ;//      real*8 xt,yt,zt,xu,yu,zu
      double xtu,ytu,ztu                                                    ;//      real*8 xtu,ytu,ztu
      double rt2,ru2,rtru,rcb                                               ;//      real*8 rt2,ru2,rtru,rcb
      double dphidxt,dphidyt,dphidzt                                        ;//      real*8 dphidxt,dphidyt,dphidzt
      double dphidxu,dphidyu,dphidzu                                        ;//      real*8 dphidxu,dphidyu,dphidzu
      double dphidxia,dphidyia,dphidzia                                     ;//      real*8 dphidxia,dphidyia,dphidzia
      double dphidxib,dphidyib,dphidzib                                     ;//      real*8 dphidxib,dphidyib,dphidzib
      double dphidxic,dphidyic,dphidzic                                     ;//      real*8 dphidxic,dphidyic,dphidzic
      double dphidxid,dphidyid,dphidzid                                     ;//      real*8 dphidxid,dphidyid,dphidzid
      double xycb2,xzcb2,yzcb2                                              ;//      real*8 xycb2,xzcb2,yzcb2
      double rcbxt,rcbyt,rcbzt,rcbt2                                        ;//      real*8 rcbxt,rcbyt,rcbzt,rcbt2
      double rcbxu,rcbyu,rcbzu,rcbu2                                        ;//      real*8 rcbxu,rcbyu,rcbzu,rcbu2
      double dphidxibt,dphidyibt,dphidzibt                                  ;//      real*8 dphidxibt,dphidyibt,dphidzibt
      double dphidxibu,dphidyibu,dphidzibu                                  ;//      real*8 dphidxibu,dphidyibu,dphidzibu
      double dphidxict,dphidyict,dphidzict                                  ;//      real*8 dphidxict,dphidyict,dphidzict
      double dphidxicu,dphidyicu,dphidzicu                                  ;//      real*8 dphidxicu,dphidyicu,dphidzicu
      double dxiaxia,dyiayia,dziazia                                        ;//      real*8 dxiaxia,dyiayia,dziazia
      double dxibxib,dyibyib,dzibzib                                        ;//      real*8 dxibxib,dyibyib,dzibzib
      double dxicxic,dyicyic,dziczic                                        ;//      real*8 dxicxic,dyicyic,dziczic
      double dxidxid,dyidyid,dzidzid                                        ;//      real*8 dxidxid,dyidyid,dzidzid
      double dxiayia,dxiazia,dyiazia                                        ;//      real*8 dxiayia,dxiazia,dyiazia
      double dxibyib,dxibzib,dyibzib                                        ;//      real*8 dxibyib,dxibzib,dyibzib
      double dxicyic,dxiczic,dyiczic                                        ;//      real*8 dxicyic,dxiczic,dyiczic
      double dxidyid,dxidzid,dyidzid                                        ;//      real*8 dxidyid,dxidzid,dyidzid
      double dxiaxib,dxiayib,dxiazib                                        ;//      real*8 dxiaxib,dxiayib,dxiazib
      double dyiaxib,dyiayib,dyiazib                                        ;//      real*8 dyiaxib,dyiayib,dyiazib
      double dziaxib,dziayib,dziazib                                        ;//      real*8 dziaxib,dziayib,dziazib
      double dxiaxic,dxiayic,dxiazic                                        ;//      real*8 dxiaxic,dxiayic,dxiazic
      double dyiaxic,dyiayic,dyiazic                                        ;//      real*8 dyiaxic,dyiayic,dyiazic
      double dziaxic,dziayic,dziazic                                        ;//      real*8 dziaxic,dziayic,dziazic
      double dxiaxid,dxiayid,dxiazid                                        ;//      real*8 dxiaxid,dxiayid,dxiazid
      double dyiaxid,dyiayid,dyiazid                                        ;//      real*8 dyiaxid,dyiayid,dyiazid
      double dziaxid,dziayid,dziazid                                        ;//      real*8 dziaxid,dziayid,dziazid
      double dxibxic,dxibyic,dxibzic                                        ;//      real*8 dxibxic,dxibyic,dxibzic
      double dyibxic,dyibyic,dyibzic                                        ;//      real*8 dyibxic,dyibyic,dyibzic
      double dzibxic,dzibyic,dzibzic                                        ;//      real*8 dzibxic,dzibyic,dzibzic
      double dxibxid,dxibyid,dxibzid                                        ;//      real*8 dxibxid,dxibyid,dxibzid
      double dyibxid,dyibyid,dyibzid                                        ;//      real*8 dyibxid,dyibyid,dyibzid
      double dzibxid,dzibyid,dzibzid                                        ;//      real*8 dzibxid,dzibyid,dzibzid
      double dxicxid,dxicyid,dxiczid                                        ;//      real*8 dxicxid,dxicyid,dxiczid
      double dyicxid,dyicyid,dyiczid                                        ;//      real*8 dyicxid,dyicyid,dyiczid
      double dzicxid,dzicyid,dziczid                                        ;//      real*8 dzicxid,dzicyid,dziczid
      bool proceed                                                          ;//      logical proceed
      fgrp = double.NaN                                                     ;//c
///                                                                         ;//c
///   compute Hessian elements for the improper dihedral angles             ;//c     compute Hessian elements for the improper dihedral angles
///                                                                         ;//c
      foreach(var kiprop in dolist(1, niprop)) {                            ;//      do kiprop = 1, niprop
         ia = iiprop[1,kiprop]                                              ;//         ia = iiprop(1,kiprop)
         ib = iiprop[2,kiprop]                                              ;//         ib = iiprop(2,kiprop)
         ic = iiprop[3,kiprop]                                              ;//         ic = iiprop(3,kiprop)
         id = iiprop[4,kiprop]                                              ;//         id = iiprop(4,kiprop)
///                                                                         ;//c
///   decide whether to compute the current interaction                     ;//c     decide whether to compute the current interaction
///                                                                         ;//c
         proceed = (i == ia  ||  i == ib  ||  i == ic  ||  i == id)         ;//         proceed = (i.eq.ia .or. i.eq.ib .or. i.eq.ic .or. i.eq.id)
         if (proceed  &&   use_group)                                        //         if (proceed .and. use_group)
            groups(out proceed,out fgrp,ia,ib,ic,id,0,0)                    ;//     &      call groups (proceed,fgrp,ia,ib,ic,id,0,0)
///                                                                         ;//c
///   compute the value of the improper dihedral angle                      ;//c     compute the value of the improper dihedral angle
///                                                                         ;//c
         if (proceed) {                                                     ;//         if (proceed) then
            xia = x[ia]                                                     ;//            xia = x(ia)
            yia = y[ia]                                                     ;//            yia = y(ia)
            zia = z[ia]                                                     ;//            zia = z(ia)
            xib = x[ib]                                                     ;//            xib = x(ib)
            yib = y[ib]                                                     ;//            yib = y(ib)
            zib = z[ib]                                                     ;//            zib = z(ib)
            xic = x[ic]                                                     ;//            xic = x(ic)
            yic = y[ic]                                                     ;//            yic = y(ic)
            zic = z[ic]                                                     ;//            zic = z(ic)
            xid = x[id]                                                     ;//            xid = x(id)
            yid = y[id]                                                     ;//            yid = y(id)
            zid = z[id]                                                     ;//            zid = z(id)
            xba = xib - xia                                                 ;//            xba = xib - xia
            yba = yib - yia                                                 ;//            yba = yib - yia
            zba = zib - zia                                                 ;//            zba = zib - zia
            xcb = xic - xib                                                 ;//            xcb = xic - xib
            ycb = yic - yib                                                 ;//            ycb = yic - yib
            zcb = zic - zib                                                 ;//            zcb = zic - zib
            xdc = xid - xic                                                 ;//            xdc = xid - xic
            ydc = yid - yic                                                 ;//            ydc = yid - yic
            zdc = zid - zic                                                 ;//            zdc = zid - zic
            if (use_polymer) {                                              ;//            if (use_polymer) then
               image(ref xba,ref yba,ref zba)                               ;//               call image (xba,yba,zba)
               image(ref xcb,ref ycb,ref zcb)                               ;//               call image (xcb,ycb,zcb)
               image(ref xdc,ref ydc,ref zdc)                               ;//               call image (xdc,ydc,zdc)
            }                                                               ;//            end if
            xt = yba*zcb - ycb*zba                                          ;//            xt = yba*zcb - ycb*zba
            yt = zba*xcb - zcb*xba                                          ;//            yt = zba*xcb - zcb*xba
            zt = xba*ycb - xcb*yba                                          ;//            zt = xba*ycb - xcb*yba
            xu = ycb*zdc - ydc*zcb                                          ;//            xu = ycb*zdc - ydc*zcb
            yu = zcb*xdc - zdc*xcb                                          ;//            yu = zcb*xdc - zdc*xcb
            zu = xcb*ydc - xdc*ycb                                          ;//            zu = xcb*ydc - xdc*ycb
            xtu = yt*zu - yu*zt                                             ;//            xtu = yt*zu - yu*zt
            ytu = zt*xu - zu*xt                                             ;//            ytu = zt*xu - zu*xt
            ztu = xt*yu - xu*yt                                             ;//            ztu = xt*yu - xu*yt
            rt2 = xt*xt + yt*yt + zt*zt                                     ;//            rt2 = xt*xt + yt*yt + zt*zt
            ru2 = xu*xu + yu*yu + zu*zu                                     ;//            ru2 = xu*xu + yu*yu + zu*zu
            rtru = sqrt(rt2 * ru2)                                          ;//            rtru = sqrt(rt2 * ru2)
            if (rtru  !=  0.0e0) {                                          ;//            if (rtru .ne. 0.0d0) then
               rcb = sqrt(xcb*xcb + ycb*ycb + zcb*zcb)                      ;//               rcb = sqrt(xcb*xcb + ycb*ycb + zcb*zcb)
               cosine = (xt*xu + yt*yu + zt*zu) / rtru                      ;//               cosine = (xt*xu + yt*yu + zt*zu) / rtru
               sine = (xcb*xtu + ycb*ytu + zcb*ztu) / (rcb*rtru)            ;//               sine = (xcb*xtu + ycb*ytu + zcb*ztu) / (rcb*rtru)
               cosine = min(1.0e0,max(-1.0e0,cosine))                       ;//               cosine = min(1.0d0,max(-1.0d0,cosine))
               angle = radian * acos(cosine)                                ;//               angle = radian * acos(cosine)
               if (sine  <   0.0e0)  angle = -angle                         ;//               if (sine .lt. 0.0d0)  angle = -angle
///                                                                         ;//c
///   set the improper dihedral parameters for this angle                   ;//c     set the improper dihedral parameters for this angle
///                                                                         ;//c
               ideal = vprop[kiprop]                                        ;//               ideal = vprop(kiprop)
               force = kprop[kiprop]                                        ;//               force = kprop(kiprop)
               if (abs(angle+ideal)  <   abs(angle-ideal))                   //               if (abs(angle+ideal) .lt. abs(angle-ideal))
                  ideal = -ideal                                            ;//     &            ideal = -ideal
               dt = angle - ideal                                           ;//               dt = angle - ideal
               while (dt  >   180.0e0) {                                    ;//               do while (dt .gt. 180.0d0)
                  dt = dt - 360.0e0                                         ;//                  dt = dt - 360.0d0
               }                                                            ;//               end do
               while (dt  <   -180.0e0) {                                   ;//               do while (dt .lt. -180.0d0)
                  dt = dt + 360.0e0                                         ;//                  dt = dt + 360.0d0
               }                                                            ;//               end do
               dt = dt / radian                                             ;//               dt = dt / radian
///                                                                         ;//c
///   calculate the improper torsion master chain rule terms                ;//c     calculate the improper torsion master chain rule terms
///                                                                         ;//c
               dedphi = 2.0e0 * idihunit * force * dt                       ;//               dedphi = 2.0d0 * idihunit * force * dt
               d2edphi2 = 2.0e0 * idihunit * force                          ;//               d2edphi2 = 2.0d0 * idihunit * force
///                                                                         ;//c
///   scale the interaction based on its group membership                   ;//c     scale the interaction based on its group membership
///                                                                         ;//c
               if (use_group) {                                             ;//               if (use_group) then
                  dedphi = dedphi * fgrp                                    ;//                  dedphi = dedphi * fgrp
                  d2edphi2 = d2edphi2 * fgrp                                ;//                  d2edphi2 = d2edphi2 * fgrp
               }                                                            ;//               end if
///                                                                         ;//c
///   abbreviations for first derivative chain rule terms                   ;//c     abbreviations for first derivative chain rule terms
///                                                                         ;//c
               xca = xic - xia                                              ;//               xca = xic - xia
               yca = yic - yia                                              ;//               yca = yic - yia
               zca = zic - zia                                              ;//               zca = zic - zia
               xdb = xid - xib                                              ;//               xdb = xid - xib
               ydb = yid - yib                                              ;//               ydb = yid - yib
               zdb = zid - zib                                              ;//               zdb = zid - zib
               if (use_polymer) {                                           ;//               if (use_polymer) then
                  image(ref xca,ref yca,ref zca)                            ;//                  call image (xca,yca,zca)
                  image(ref xdb,ref ydb,ref zdb)                            ;//                  call image (xdb,ydb,zdb)
               }                                                            ;//               end if
               dphidxt = (yt*zcb - ycb*zt) / (rt2*rcb)                      ;//               dphidxt = (yt*zcb - ycb*zt) / (rt2*rcb)
               dphidyt = (zt*xcb - zcb*xt) / (rt2*rcb)                      ;//               dphidyt = (zt*xcb - zcb*xt) / (rt2*rcb)
               dphidzt = (xt*ycb - xcb*yt) / (rt2*rcb)                      ;//               dphidzt = (xt*ycb - xcb*yt) / (rt2*rcb)
               dphidxu = -(yu*zcb - ycb*zu) / (ru2*rcb)                     ;//               dphidxu = -(yu*zcb - ycb*zu) / (ru2*rcb)
               dphidyu = -(zu*xcb - zcb*xu) / (ru2*rcb)                     ;//               dphidyu = -(zu*xcb - zcb*xu) / (ru2*rcb)
               dphidzu = -(xu*ycb - xcb*yu) / (ru2*rcb)                     ;//               dphidzu = -(xu*ycb - xcb*yu) / (ru2*rcb)
///                                                                         ;//c
///   abbreviations for second derivative chain rule terms                  ;//c     abbreviations for second derivative chain rule terms
///                                                                         ;//c
               xycb2 = xcb*xcb + ycb*ycb                                    ;//               xycb2 = xcb*xcb + ycb*ycb
               xzcb2 = xcb*xcb + zcb*zcb                                    ;//               xzcb2 = xcb*xcb + zcb*zcb
               yzcb2 = ycb*ycb + zcb*zcb                                    ;//               yzcb2 = ycb*ycb + zcb*zcb
               rcbxt = -2.0e0 * rcb * dphidxt                               ;//               rcbxt = -2.0d0 * rcb * dphidxt
               rcbyt = -2.0e0 * rcb * dphidyt                               ;//               rcbyt = -2.0d0 * rcb * dphidyt
               rcbzt = -2.0e0 * rcb * dphidzt                               ;//               rcbzt = -2.0d0 * rcb * dphidzt
               rcbt2 = rcb * rt2                                            ;//               rcbt2 = rcb * rt2
               rcbxu = 2.0e0 * rcb * dphidxu                                ;//               rcbxu = 2.0d0 * rcb * dphidxu
               rcbyu = 2.0e0 * rcb * dphidyu                                ;//               rcbyu = 2.0d0 * rcb * dphidyu
               rcbzu = 2.0e0 * rcb * dphidzu                                ;//               rcbzu = 2.0d0 * rcb * dphidzu
               rcbu2 = rcb * ru2                                            ;//               rcbu2 = rcb * ru2
               dphidxibt = yca*dphidzt - zca*dphidyt                        ;//               dphidxibt = yca*dphidzt - zca*dphidyt
               dphidxibu = zdc*dphidyu - ydc*dphidzu                        ;//               dphidxibu = zdc*dphidyu - ydc*dphidzu
               dphidyibt = zca*dphidxt - xca*dphidzt                        ;//               dphidyibt = zca*dphidxt - xca*dphidzt
               dphidyibu = xdc*dphidzu - zdc*dphidxu                        ;//               dphidyibu = xdc*dphidzu - zdc*dphidxu
               dphidzibt = xca*dphidyt - yca*dphidxt                        ;//               dphidzibt = xca*dphidyt - yca*dphidxt
               dphidzibu = ydc*dphidxu - xdc*dphidyu                        ;//               dphidzibu = ydc*dphidxu - xdc*dphidyu
               dphidxict = zba*dphidyt - yba*dphidzt                        ;//               dphidxict = zba*dphidyt - yba*dphidzt
               dphidxicu = ydb*dphidzu - zdb*dphidyu                        ;//               dphidxicu = ydb*dphidzu - zdb*dphidyu
               dphidyict = xba*dphidzt - zba*dphidxt                        ;//               dphidyict = xba*dphidzt - zba*dphidxt
               dphidyicu = zdb*dphidxu - xdb*dphidzu                        ;//               dphidyicu = zdb*dphidxu - xdb*dphidzu
               dphidzict = yba*dphidxt - xba*dphidyt                        ;//               dphidzict = yba*dphidxt - xba*dphidyt
               dphidzicu = xdb*dphidyu - ydb*dphidxu                        ;//               dphidzicu = xdb*dphidyu - ydb*dphidxu
///                                                                         ;//c
///   chain rule terms for first derivative components                      ;//c     chain rule terms for first derivative components
///                                                                         ;//c
               dphidxia = zcb*dphidyt - ycb*dphidzt                         ;//               dphidxia = zcb*dphidyt - ycb*dphidzt
               dphidyia = xcb*dphidzt - zcb*dphidxt                         ;//               dphidyia = xcb*dphidzt - zcb*dphidxt
               dphidzia = ycb*dphidxt - xcb*dphidyt                         ;//               dphidzia = ycb*dphidxt - xcb*dphidyt
               dphidxib = dphidxibt + dphidxibu                             ;//               dphidxib = dphidxibt + dphidxibu
               dphidyib = dphidyibt + dphidyibu                             ;//               dphidyib = dphidyibt + dphidyibu
               dphidzib = dphidzibt + dphidzibu                             ;//               dphidzib = dphidzibt + dphidzibu
               dphidxic = dphidxict + dphidxicu                             ;//               dphidxic = dphidxict + dphidxicu
               dphidyic = dphidyict + dphidyicu                             ;//               dphidyic = dphidyict + dphidyicu
               dphidzic = dphidzict + dphidzicu                             ;//               dphidzic = dphidzict + dphidzicu
               dphidxid = zcb*dphidyu - ycb*dphidzu                         ;//               dphidxid = zcb*dphidyu - ycb*dphidzu
               dphidyid = xcb*dphidzu - zcb*dphidxu                         ;//               dphidyid = xcb*dphidzu - zcb*dphidxu
               dphidzid = ycb*dphidxu - xcb*dphidyu                         ;//               dphidzid = ycb*dphidxu - xcb*dphidyu
///                                                                         ;//c
///   chain rule terms for second derivative components                     ;//c     chain rule terms for second derivative components
///                                                                         ;//c
               dxiaxia = rcbxt*dphidxia                                     ;//               dxiaxia = rcbxt*dphidxia
               dxiayia = rcbxt*dphidyia - zcb*rcb/rt2                       ;//               dxiayia = rcbxt*dphidyia - zcb*rcb/rt2
               dxiazia = rcbxt*dphidzia + ycb*rcb/rt2                       ;//               dxiazia = rcbxt*dphidzia + ycb*rcb/rt2
               dxiaxic = rcbxt*dphidxict + xcb*xt/rcbt2                     ;//               dxiaxic = rcbxt*dphidxict + xcb*xt/rcbt2
               dxiayic = rcbxt*dphidyict - dphidzt                           //               dxiayic = rcbxt*dphidyict - dphidzt
                            - (xba*zcb*xcb+zba*yzcb2)/rcbt2                 ;//     &                      - (xba*zcb*xcb+zba*yzcb2)/rcbt2
               dxiazic = rcbxt*dphidzict + dphidyt                           //               dxiazic = rcbxt*dphidzict + dphidyt
                            + (xba*ycb*xcb+yba*yzcb2)/rcbt2                 ;//     &                      + (xba*ycb*xcb+yba*yzcb2)/rcbt2
               dxiaxid = 0.0e0                                              ;//               dxiaxid = 0.0d0
               dxiayid = 0.0e0                                              ;//               dxiayid = 0.0d0
               dxiazid = 0.0e0                                              ;//               dxiazid = 0.0d0
               dyiayia = rcbyt*dphidyia                                     ;//               dyiayia = rcbyt*dphidyia
               dyiazia = rcbyt*dphidzia - xcb*rcb/rt2                       ;//               dyiazia = rcbyt*dphidzia - xcb*rcb/rt2
               dyiaxib = rcbyt*dphidxibt - dphidzt                           //               dyiaxib = rcbyt*dphidxibt - dphidzt
                            - (yca*zcb*ycb+zca*xzcb2)/rcbt2                 ;//     &                      - (yca*zcb*ycb+zca*xzcb2)/rcbt2
               dyiaxic = rcbyt*dphidxict + dphidzt                           //               dyiaxic = rcbyt*dphidxict + dphidzt
                            + (yba*zcb*ycb+zba*xzcb2)/rcbt2                 ;//     &                      + (yba*zcb*ycb+zba*xzcb2)/rcbt2
               dyiayic = rcbyt*dphidyict + ycb*yt/rcbt2                     ;//               dyiayic = rcbyt*dphidyict + ycb*yt/rcbt2
               dyiazic = rcbyt*dphidzict - dphidxt                           //               dyiazic = rcbyt*dphidzict - dphidxt
                            - (yba*xcb*ycb+xba*xzcb2)/rcbt2                 ;//     &                      - (yba*xcb*ycb+xba*xzcb2)/rcbt2
               dyiaxid = 0.0e0                                              ;//               dyiaxid = 0.0d0
               dyiayid = 0.0e0                                              ;//               dyiayid = 0.0d0
               dyiazid = 0.0e0                                              ;//               dyiazid = 0.0d0
               dziazia = rcbzt*dphidzia                                     ;//               dziazia = rcbzt*dphidzia
               dziaxib = rcbzt*dphidxibt + dphidyt                           //               dziaxib = rcbzt*dphidxibt + dphidyt
                            + (zca*ycb*zcb+yca*xycb2)/rcbt2                 ;//     &                      + (zca*ycb*zcb+yca*xycb2)/rcbt2
               dziayib = rcbzt*dphidyibt - dphidxt                           //               dziayib = rcbzt*dphidyibt - dphidxt
                            - (zca*xcb*zcb+xca*xycb2)/rcbt2                 ;//     &                      - (zca*xcb*zcb+xca*xycb2)/rcbt2
               dziaxic = rcbzt*dphidxict - dphidyt                           //               dziaxic = rcbzt*dphidxict - dphidyt
                            - (zba*ycb*zcb+yba*xycb2)/rcbt2                 ;//     &                      - (zba*ycb*zcb+yba*xycb2)/rcbt2
               dziayic = rcbzt*dphidyict + dphidxt                           //               dziayic = rcbzt*dphidyict + dphidxt
                            + (zba*xcb*zcb+xba*xycb2)/rcbt2                 ;//     &                      + (zba*xcb*zcb+xba*xycb2)/rcbt2
               dziazic = rcbzt*dphidzict + zcb*zt/rcbt2                     ;//               dziazic = rcbzt*dphidzict + zcb*zt/rcbt2
               dziaxid = 0.0e0                                              ;//               dziaxid = 0.0d0
               dziayid = 0.0e0                                              ;//               dziayid = 0.0d0
               dziazid = 0.0e0                                              ;//               dziazid = 0.0d0
               dxibxic = -xcb*dphidxib/(rcb*rcb)                             //               dxibxic = -xcb*dphidxib/(rcb*rcb)
                   - (yca*(zba*xcb+yt)-zca*(yba*xcb-zt))/rcbt2               //     &             - (yca*(zba*xcb+yt)-zca*(yba*xcb-zt))/rcbt2
                   - 2.0e0*(yt*zba-yba*zt)*dphidxibt/rt2                     //     &             - 2.0d0*(yt*zba-yba*zt)*dphidxibt/rt2
                   - (zdc*(ydb*xcb+zu)-ydc*(zdb*xcb-yu))/rcbu2               //     &             - (zdc*(ydb*xcb+zu)-ydc*(zdb*xcb-yu))/rcbu2
                   + 2.0e0*(yu*zdb-ydb*zu)*dphidxibu/ru2                    ;//     &             + 2.0d0*(yu*zdb-ydb*zu)*dphidxibu/ru2
               dxibyic = -ycb*dphidxib/(rcb*rcb) + dphidzt + dphidzu         //               dxibyic = -ycb*dphidxib/(rcb*rcb) + dphidzt + dphidzu
                   - (yca*(zba*ycb-xt)+zca*(xba*xcb+zcb*zba))/rcbt2          //     &             - (yca*(zba*ycb-xt)+zca*(xba*xcb+zcb*zba))/rcbt2
                   - 2.0e0*(zt*xba-zba*xt)*dphidxibt/rt2                     //     &             - 2.0d0*(zt*xba-zba*xt)*dphidxibt/rt2
                   + (zdc*(xdb*xcb+zcb*zdb)+ydc*(zdb*ycb+xu))/rcbu2          //     &             + (zdc*(xdb*xcb+zcb*zdb)+ydc*(zdb*ycb+xu))/rcbu2
                   + 2.0e0*(zu*xdb-zdb*xu)*dphidxibu/ru2                    ;//     &             + 2.0d0*(zu*xdb-zdb*xu)*dphidxibu/ru2
               dxibxid = rcbxu*dphidxibu + xcb*xu/rcbu2                     ;//               dxibxid = rcbxu*dphidxibu + xcb*xu/rcbu2
               dxibyid = rcbyu*dphidxibu - dphidzu                           //               dxibyid = rcbyu*dphidxibu - dphidzu
                            - (ydc*zcb*ycb+zdc*xzcb2)/rcbu2                 ;//     &                      - (ydc*zcb*ycb+zdc*xzcb2)/rcbu2
               dxibzid = rcbzu*dphidxibu + dphidyu                           //               dxibzid = rcbzu*dphidxibu + dphidyu
                            + (zdc*ycb*zcb+ydc*xycb2)/rcbu2                 ;//     &                      + (zdc*ycb*zcb+ydc*xycb2)/rcbu2
               dyibzib = ycb*dphidzib/(rcb*rcb)                              //               dyibzib = ycb*dphidzib/(rcb*rcb)
                   - (xca*(xca*xcb+zcb*zca)+yca*(ycb*xca+zt))/rcbt2          //     &             - (xca*(xca*xcb+zcb*zca)+yca*(ycb*xca+zt))/rcbt2
                   - 2.0e0*(xt*zca-xca*zt)*dphidzibt/rt2                     //     &             - 2.0d0*(xt*zca-xca*zt)*dphidzibt/rt2
                   + (ydc*(xdc*ycb-zu)+xdc*(xdc*xcb+zcb*zdc))/rcbu2          //     &             + (ydc*(xdc*ycb-zu)+xdc*(xdc*xcb+zcb*zdc))/rcbu2
                   + 2.0e0*(xu*zdc-xdc*zu)*dphidzibu/ru2                    ;//     &             + 2.0d0*(xu*zdc-xdc*zu)*dphidzibu/ru2
               dyibxic = -xcb*dphidyib/(rcb*rcb) - dphidzt - dphidzu         //               dyibxic = -xcb*dphidyib/(rcb*rcb) - dphidzt - dphidzu
                   + (xca*(zba*xcb+yt)+zca*(zba*zcb+ycb*yba))/rcbt2          //     &             + (xca*(zba*xcb+yt)+zca*(zba*zcb+ycb*yba))/rcbt2
                   - 2.0e0*(yt*zba-yba*zt)*dphidyibt/rt2                     //     &             - 2.0d0*(yt*zba-yba*zt)*dphidyibt/rt2
                   - (zdc*(zdb*zcb+ycb*ydb)+xdc*(zdb*xcb-yu))/rcbu2          //     &             - (zdc*(zdb*zcb+ycb*ydb)+xdc*(zdb*xcb-yu))/rcbu2
                   + 2.0e0*(yu*zdb-ydb*zu)*dphidyibu/ru2                    ;//     &             + 2.0d0*(yu*zdb-ydb*zu)*dphidyibu/ru2
               dyibyic = -ycb*dphidyib/(rcb*rcb)                             //               dyibyic = -ycb*dphidyib/(rcb*rcb)
                   - (zca*(xba*ycb+zt)-xca*(zba*ycb-xt))/rcbt2               //     &             - (zca*(xba*ycb+zt)-xca*(zba*ycb-xt))/rcbt2
                   - 2.0e0*(zt*xba-zba*xt)*dphidyibt/rt2                     //     &             - 2.0d0*(zt*xba-zba*xt)*dphidyibt/rt2
                   - (xdc*(zdb*ycb+xu)-zdc*(xdb*ycb-zu))/rcbu2               //     &             - (xdc*(zdb*ycb+xu)-zdc*(xdb*ycb-zu))/rcbu2
                   + 2.0e0*(zu*xdb-zdb*xu)*dphidyibu/ru2                    ;//     &             + 2.0d0*(zu*xdb-zdb*xu)*dphidyibu/ru2
               dyibxid = rcbxu*dphidyibu + dphidzu                           //               dyibxid = rcbxu*dphidyibu + dphidzu
                            + (xdc*zcb*xcb+zdc*yzcb2)/rcbu2                 ;//     &                      + (xdc*zcb*xcb+zdc*yzcb2)/rcbu2
               dyibyid = rcbyu*dphidyibu + ycb*yu/rcbu2                     ;//               dyibyid = rcbyu*dphidyibu + ycb*yu/rcbu2
               dyibzid = rcbzu*dphidyibu - dphidxu                           //               dyibzid = rcbzu*dphidyibu - dphidxu
                            - (zdc*xcb*zcb+xdc*xycb2)/rcbu2                 ;//     &                      - (zdc*xcb*zcb+xdc*xycb2)/rcbu2
               dzibxic = -xcb*dphidzib/(rcb*rcb) + dphidyt + dphidyu         //               dzibxic = -xcb*dphidzib/(rcb*rcb) + dphidyt + dphidyu
                   - (xca*(yba*xcb-zt)+yca*(zba*zcb+ycb*yba))/rcbt2          //     &             - (xca*(yba*xcb-zt)+yca*(zba*zcb+ycb*yba))/rcbt2
                   - 2.0e0*(yt*zba-yba*zt)*dphidzibt/rt2                     //     &             - 2.0d0*(yt*zba-yba*zt)*dphidzibt/rt2
                   + (ydc*(zdb*zcb+ycb*ydb)+xdc*(ydb*xcb+zu))/rcbu2          //     &             + (ydc*(zdb*zcb+ycb*ydb)+xdc*(ydb*xcb+zu))/rcbu2
                   + 2.0e0*(yu*zdb-ydb*zu)*dphidzibu/ru2                    ;//     &             + 2.0d0*(yu*zdb-ydb*zu)*dphidzibu/ru2
               dzibzic = -zcb*dphidzib/(rcb*rcb)                             //               dzibzic = -zcb*dphidzib/(rcb*rcb)
                   - (xca*(yba*zcb+xt)-yca*(xba*zcb-yt))/rcbt2               //     &             - (xca*(yba*zcb+xt)-yca*(xba*zcb-yt))/rcbt2
                   - 2.0e0*(xt*yba-xba*yt)*dphidzibt/rt2                     //     &             - 2.0d0*(xt*yba-xba*yt)*dphidzibt/rt2
                   - (ydc*(xdb*zcb+yu)-xdc*(ydb*zcb-xu))/rcbu2               //     &             - (ydc*(xdb*zcb+yu)-xdc*(ydb*zcb-xu))/rcbu2
                   + 2.0e0*(xu*ydb-xdb*yu)*dphidzibu/ru2                    ;//     &             + 2.0d0*(xu*ydb-xdb*yu)*dphidzibu/ru2
               dzibxid = rcbxu*dphidzibu - dphidyu                           //               dzibxid = rcbxu*dphidzibu - dphidyu
                            - (xdc*ycb*xcb+ydc*yzcb2)/rcbu2                 ;//     &                      - (xdc*ycb*xcb+ydc*yzcb2)/rcbu2
               dzibyid = rcbyu*dphidzibu + dphidxu                           //               dzibyid = rcbyu*dphidzibu + dphidxu
                            + (ydc*xcb*ycb+xdc*xzcb2)/rcbu2                 ;//     &                      + (ydc*xcb*ycb+xdc*xzcb2)/rcbu2
               dzibzid = rcbzu*dphidzibu + zcb*zu/rcbu2                     ;//               dzibzid = rcbzu*dphidzibu + zcb*zu/rcbu2
               dxicxid = rcbxu*dphidxicu - xcb*(zdb*ycb-ydb*zcb)/rcbu2      ;//               dxicxid = rcbxu*dphidxicu - xcb*(zdb*ycb-ydb*zcb)/rcbu2
               dxicyid = rcbyu*dphidxicu + dphidzu                           //               dxicyid = rcbyu*dphidxicu + dphidzu
                            + (ydb*zcb*ycb+zdb*xzcb2)/rcbu2                 ;//     &                      + (ydb*zcb*ycb+zdb*xzcb2)/rcbu2
               dxiczid = rcbzu*dphidxicu - dphidyu                           //               dxiczid = rcbzu*dphidxicu - dphidyu
                            - (zdb*ycb*zcb+ydb*xycb2)/rcbu2                 ;//     &                      - (zdb*ycb*zcb+ydb*xycb2)/rcbu2
               dyicxid = rcbxu*dphidyicu - dphidzu                           //               dyicxid = rcbxu*dphidyicu - dphidzu
                            - (xdb*zcb*xcb+zdb*yzcb2)/rcbu2                 ;//     &                      - (xdb*zcb*xcb+zdb*yzcb2)/rcbu2
               dyicyid = rcbyu*dphidyicu - ycb*(xdb*zcb-zdb*xcb)/rcbu2      ;//               dyicyid = rcbyu*dphidyicu - ycb*(xdb*zcb-zdb*xcb)/rcbu2
               dyiczid = rcbzu*dphidyicu + dphidxu                           //               dyiczid = rcbzu*dphidyicu + dphidxu
                            + (zdb*xcb*zcb+xdb*xycb2)/rcbu2                 ;//     &                      + (zdb*xcb*zcb+xdb*xycb2)/rcbu2
               dzicxid = rcbxu*dphidzicu + dphidyu                           //               dzicxid = rcbxu*dphidzicu + dphidyu
                            + (xdb*ycb*xcb+ydb*yzcb2)/rcbu2                 ;//     &                      + (xdb*ycb*xcb+ydb*yzcb2)/rcbu2
               dzicyid = rcbyu*dphidzicu - dphidxu                           //               dzicyid = rcbyu*dphidzicu - dphidxu
                            - (ydb*xcb*ycb+xdb*xzcb2)/rcbu2                 ;//     &                      - (ydb*xcb*ycb+xdb*xzcb2)/rcbu2
               dziczid = rcbzu*dphidzicu - zcb*(ydb*xcb-xdb*ycb)/rcbu2      ;//               dziczid = rcbzu*dphidzicu - zcb*(ydb*xcb-xdb*ycb)/rcbu2
               dxidxid = rcbxu*dphidxid                                     ;//               dxidxid = rcbxu*dphidxid
               dxidyid = rcbxu*dphidyid + zcb*rcb/ru2                       ;//               dxidyid = rcbxu*dphidyid + zcb*rcb/ru2
               dxidzid = rcbxu*dphidzid - ycb*rcb/ru2                       ;//               dxidzid = rcbxu*dphidzid - ycb*rcb/ru2
               dyidyid = rcbyu*dphidyid                                     ;//               dyidyid = rcbyu*dphidyid
               dyidzid = rcbyu*dphidzid + xcb*rcb/ru2                       ;//               dyidzid = rcbyu*dphidzid + xcb*rcb/ru2
               dzidzid = rcbzu*dphidzid                                     ;//               dzidzid = rcbzu*dphidzid
///                                                                         ;//c
///   get some second derivative chain rule terms by difference             ;//c     get some second derivative chain rule terms by difference
///                                                                         ;//c
               dxiaxib = -dxiaxia - dxiaxic - dxiaxid                       ;//               dxiaxib = -dxiaxia - dxiaxic - dxiaxid
               dxiayib = -dxiayia - dxiayic - dxiayid                       ;//               dxiayib = -dxiayia - dxiayic - dxiayid
               dxiazib = -dxiazia - dxiazic - dxiazid                       ;//               dxiazib = -dxiazia - dxiazic - dxiazid
               dyiayib = -dyiayia - dyiayic - dyiayid                       ;//               dyiayib = -dyiayia - dyiayic - dyiayid
               dyiazib = -dyiazia - dyiazic - dyiazid                       ;//               dyiazib = -dyiazia - dyiazic - dyiazid
               dziazib = -dziazia - dziazic - dziazid                       ;//               dziazib = -dziazia - dziazic - dziazid
               dxibxib = -dxiaxib - dxibxic - dxibxid                       ;//               dxibxib = -dxiaxib - dxibxic - dxibxid
               dxibyib = -dyiaxib - dxibyic - dxibyid                       ;//               dxibyib = -dyiaxib - dxibyic - dxibyid
               dxibzib = -dxiazib - dzibxic - dzibxid                       ;//               dxibzib = -dxiazib - dzibxic - dzibxid
               dxibzic = -dziaxib - dxibzib - dxibzid                       ;//               dxibzic = -dziaxib - dxibzib - dxibzid
               dyibyib = -dyiayib - dyibyic - dyibyid                       ;//               dyibyib = -dyiayib - dyibyic - dyibyid
               dyibzic = -dziayib - dyibzib - dyibzid                       ;//               dyibzic = -dziayib - dyibzib - dyibzid
               dzibzib = -dziazib - dzibzic - dzibzid                       ;//               dzibzib = -dziazib - dzibzic - dzibzid
               dzibyic = -dyiazib - dyibzib - dzibyid                       ;//               dzibyic = -dyiazib - dyibzib - dzibyid
               dxicxic = -dxiaxic - dxibxic - dxicxid                       ;//               dxicxic = -dxiaxic - dxibxic - dxicxid
               dxicyic = -dyiaxic - dyibxic - dxicyid                       ;//               dxicyic = -dyiaxic - dyibxic - dxicyid
               dxiczic = -dziaxic - dzibxic - dxiczid                       ;//               dxiczic = -dziaxic - dzibxic - dxiczid
               dyicyic = -dyiayic - dyibyic - dyicyid                       ;//               dyicyic = -dyiayic - dyibyic - dyicyid
               dyiczic = -dziayic - dzibyic - dyiczid                       ;//               dyiczic = -dziayic - dzibyic - dyiczid
               dziczic = -dziazic - dzibzic - dziczid                       ;//               dziczic = -dziazic - dzibzic - dziczid
///                                                                         ;//c
///   increment diagonal and off-diagonal Hessian elements                  ;//c     increment diagonal and off-diagonal Hessian elements
///                                                                         ;//c
               if (i  ==  ia) {                                             ;//               if (i .eq. ia) then
                  hessx[1,ia] = hessx[1,ia] + dedphi*dxiaxia                 //                  hessx(1,ia) = hessx(1,ia) + dedphi*dxiaxia
                                   + d2edphi2*dphidxia*dphidxia             ;//     &                             + d2edphi2*dphidxia*dphidxia
                  hessy[1,ia] = hessy[1,ia] + dedphi*dxiayia                 //                  hessy(1,ia) = hessy(1,ia) + dedphi*dxiayia
                                   + d2edphi2*dphidxia*dphidyia             ;//     &                             + d2edphi2*dphidxia*dphidyia
                  hessz[1,ia] = hessz[1,ia] + dedphi*dxiazia                 //                  hessz(1,ia) = hessz(1,ia) + dedphi*dxiazia
                                   + d2edphi2*dphidxia*dphidzia             ;//     &                             + d2edphi2*dphidxia*dphidzia
                  hessx[2,ia] = hessx[2,ia] + dedphi*dxiayia                 //                  hessx(2,ia) = hessx(2,ia) + dedphi*dxiayia
                                   + d2edphi2*dphidxia*dphidyia             ;//     &                             + d2edphi2*dphidxia*dphidyia
                  hessy[2,ia] = hessy[2,ia] + dedphi*dyiayia                 //                  hessy(2,ia) = hessy(2,ia) + dedphi*dyiayia
                                   + d2edphi2*dphidyia*dphidyia             ;//     &                             + d2edphi2*dphidyia*dphidyia
                  hessz[2,ia] = hessz[2,ia] + dedphi*dyiazia                 //                  hessz(2,ia) = hessz(2,ia) + dedphi*dyiazia
                                   + d2edphi2*dphidyia*dphidzia             ;//     &                             + d2edphi2*dphidyia*dphidzia
                  hessx[3,ia] = hessx[3,ia] + dedphi*dxiazia                 //                  hessx(3,ia) = hessx(3,ia) + dedphi*dxiazia
                                   + d2edphi2*dphidxia*dphidzia             ;//     &                             + d2edphi2*dphidxia*dphidzia
                  hessy[3,ia] = hessy[3,ia] + dedphi*dyiazia                 //                  hessy(3,ia) = hessy(3,ia) + dedphi*dyiazia
                                   + d2edphi2*dphidyia*dphidzia             ;//     &                             + d2edphi2*dphidyia*dphidzia
                  hessz[3,ia] = hessz[3,ia] + dedphi*dziazia                 //                  hessz(3,ia) = hessz(3,ia) + dedphi*dziazia
                                   + d2edphi2*dphidzia*dphidzia             ;//     &                             + d2edphi2*dphidzia*dphidzia
                  hessx[1,ib] = hessx[1,ib] + dedphi*dxiaxib                 //                  hessx(1,ib) = hessx(1,ib) + dedphi*dxiaxib
                                   + d2edphi2*dphidxia*dphidxib             ;//     &                             + d2edphi2*dphidxia*dphidxib
                  hessy[1,ib] = hessy[1,ib] + dedphi*dyiaxib                 //                  hessy(1,ib) = hessy(1,ib) + dedphi*dyiaxib
                                   + d2edphi2*dphidyia*dphidxib             ;//     &                             + d2edphi2*dphidyia*dphidxib
                  hessz[1,ib] = hessz[1,ib] + dedphi*dziaxib                 //                  hessz(1,ib) = hessz(1,ib) + dedphi*dziaxib
                                   + d2edphi2*dphidzia*dphidxib             ;//     &                             + d2edphi2*dphidzia*dphidxib
                  hessx[2,ib] = hessx[2,ib] + dedphi*dxiayib                 //                  hessx(2,ib) = hessx(2,ib) + dedphi*dxiayib
                                   + d2edphi2*dphidxia*dphidyib             ;//     &                             + d2edphi2*dphidxia*dphidyib
                  hessy[2,ib] = hessy[2,ib] + dedphi*dyiayib                 //                  hessy(2,ib) = hessy(2,ib) + dedphi*dyiayib
                                   + d2edphi2*dphidyia*dphidyib             ;//     &                             + d2edphi2*dphidyia*dphidyib
                  hessz[2,ib] = hessz[2,ib] + dedphi*dziayib                 //                  hessz(2,ib) = hessz(2,ib) + dedphi*dziayib
                                   + d2edphi2*dphidzia*dphidyib             ;//     &                             + d2edphi2*dphidzia*dphidyib
                  hessx[3,ib] = hessx[3,ib] + dedphi*dxiazib                 //                  hessx(3,ib) = hessx(3,ib) + dedphi*dxiazib
                                   + d2edphi2*dphidxia*dphidzib             ;//     &                             + d2edphi2*dphidxia*dphidzib
                  hessy[3,ib] = hessy[3,ib] + dedphi*dyiazib                 //                  hessy(3,ib) = hessy(3,ib) + dedphi*dyiazib
                                   + d2edphi2*dphidyia*dphidzib             ;//     &                             + d2edphi2*dphidyia*dphidzib
                  hessz[3,ib] = hessz[3,ib] + dedphi*dziazib                 //                  hessz(3,ib) = hessz(3,ib) + dedphi*dziazib
                                   + d2edphi2*dphidzia*dphidzib             ;//     &                             + d2edphi2*dphidzia*dphidzib
                  hessx[1,ic] = hessx[1,ic] + dedphi*dxiaxic                 //                  hessx(1,ic) = hessx(1,ic) + dedphi*dxiaxic
                                   + d2edphi2*dphidxia*dphidxic             ;//     &                             + d2edphi2*dphidxia*dphidxic
                  hessy[1,ic] = hessy[1,ic] + dedphi*dyiaxic                 //                  hessy(1,ic) = hessy(1,ic) + dedphi*dyiaxic
                                   + d2edphi2*dphidyia*dphidxic             ;//     &                             + d2edphi2*dphidyia*dphidxic
                  hessz[1,ic] = hessz[1,ic] + dedphi*dziaxic                 //                  hessz(1,ic) = hessz(1,ic) + dedphi*dziaxic
                                   + d2edphi2*dphidzia*dphidxic             ;//     &                             + d2edphi2*dphidzia*dphidxic
                  hessx[2,ic] = hessx[2,ic] + dedphi*dxiayic                 //                  hessx(2,ic) = hessx(2,ic) + dedphi*dxiayic
                                   + d2edphi2*dphidxia*dphidyic             ;//     &                             + d2edphi2*dphidxia*dphidyic
                  hessy[2,ic] = hessy[2,ic] + dedphi*dyiayic                 //                  hessy(2,ic) = hessy(2,ic) + dedphi*dyiayic
                                   + d2edphi2*dphidyia*dphidyic             ;//     &                             + d2edphi2*dphidyia*dphidyic
                  hessz[2,ic] = hessz[2,ic] + dedphi*dziayic                 //                  hessz(2,ic) = hessz(2,ic) + dedphi*dziayic
                                   + d2edphi2*dphidzia*dphidyic             ;//     &                             + d2edphi2*dphidzia*dphidyic
                  hessx[3,ic] = hessx[3,ic] + dedphi*dxiazic                 //                  hessx(3,ic) = hessx(3,ic) + dedphi*dxiazic
                                   + d2edphi2*dphidxia*dphidzic             ;//     &                             + d2edphi2*dphidxia*dphidzic
                  hessy[3,ic] = hessy[3,ic] + dedphi*dyiazic                 //                  hessy(3,ic) = hessy(3,ic) + dedphi*dyiazic
                                   + d2edphi2*dphidyia*dphidzic             ;//     &                             + d2edphi2*dphidyia*dphidzic
                  hessz[3,ic] = hessz[3,ic] + dedphi*dziazic                 //                  hessz(3,ic) = hessz(3,ic) + dedphi*dziazic
                                   + d2edphi2*dphidzia*dphidzic             ;//     &                             + d2edphi2*dphidzia*dphidzic
                  hessx[1,id] = hessx[1,id] + dedphi*dxiaxid                 //                  hessx(1,id) = hessx(1,id) + dedphi*dxiaxid
                                   + d2edphi2*dphidxia*dphidxid             ;//     &                             + d2edphi2*dphidxia*dphidxid
                  hessy[1,id] = hessy[1,id] + dedphi*dyiaxid                 //                  hessy(1,id) = hessy(1,id) + dedphi*dyiaxid
                                   + d2edphi2*dphidyia*dphidxid             ;//     &                             + d2edphi2*dphidyia*dphidxid
                  hessz[1,id] = hessz[1,id] + dedphi*dziaxid                 //                  hessz(1,id) = hessz(1,id) + dedphi*dziaxid
                                   + d2edphi2*dphidzia*dphidxid             ;//     &                             + d2edphi2*dphidzia*dphidxid
                  hessx[2,id] = hessx[2,id] + dedphi*dxiayid                 //                  hessx(2,id) = hessx(2,id) + dedphi*dxiayid
                                   + d2edphi2*dphidxia*dphidyid             ;//     &                             + d2edphi2*dphidxia*dphidyid
                  hessy[2,id] = hessy[2,id] + dedphi*dyiayid                 //                  hessy(2,id) = hessy(2,id) + dedphi*dyiayid
                                   + d2edphi2*dphidyia*dphidyid             ;//     &                             + d2edphi2*dphidyia*dphidyid
                  hessz[2,id] = hessz[2,id] + dedphi*dziayid                 //                  hessz(2,id) = hessz(2,id) + dedphi*dziayid
                                   + d2edphi2*dphidzia*dphidyid             ;//     &                             + d2edphi2*dphidzia*dphidyid
                  hessx[3,id] = hessx[3,id] + dedphi*dxiazid                 //                  hessx(3,id) = hessx(3,id) + dedphi*dxiazid
                                   + d2edphi2*dphidxia*dphidzid             ;//     &                             + d2edphi2*dphidxia*dphidzid
                  hessy[3,id] = hessy[3,id] + dedphi*dyiazid                 //                  hessy(3,id) = hessy(3,id) + dedphi*dyiazid
                                   + d2edphi2*dphidyia*dphidzid             ;//     &                             + d2edphi2*dphidyia*dphidzid
                  hessz[3,id] = hessz[3,id] + dedphi*dziazid                 //                  hessz(3,id) = hessz(3,id) + dedphi*dziazid
                                   + d2edphi2*dphidzia*dphidzid             ;//     &                             + d2edphi2*dphidzia*dphidzid
               } else if (i  ==  ib) {                                       ;//               else if (i .eq. ib) then
                  hessx[1,ib] = hessx[1,ib] + dedphi*dxibxib                 //                  hessx(1,ib) = hessx(1,ib) + dedphi*dxibxib
                                   + d2edphi2*dphidxib*dphidxib             ;//     &                             + d2edphi2*dphidxib*dphidxib
                  hessy[1,ib] = hessy[1,ib] + dedphi*dxibyib                 //                  hessy(1,ib) = hessy(1,ib) + dedphi*dxibyib
                                   + d2edphi2*dphidxib*dphidyib             ;//     &                             + d2edphi2*dphidxib*dphidyib
                  hessz[1,ib] = hessz[1,ib] + dedphi*dxibzib                 //                  hessz(1,ib) = hessz(1,ib) + dedphi*dxibzib
                                   + d2edphi2*dphidxib*dphidzib             ;//     &                             + d2edphi2*dphidxib*dphidzib
                  hessx[2,ib] = hessx[2,ib] + dedphi*dxibyib                 //                  hessx(2,ib) = hessx(2,ib) + dedphi*dxibyib
                                   + d2edphi2*dphidxib*dphidyib             ;//     &                             + d2edphi2*dphidxib*dphidyib
                  hessy[2,ib] = hessy[2,ib] + dedphi*dyibyib                 //                  hessy(2,ib) = hessy(2,ib) + dedphi*dyibyib
                                   + d2edphi2*dphidyib*dphidyib             ;//     &                             + d2edphi2*dphidyib*dphidyib
                  hessz[2,ib] = hessz[2,ib] + dedphi*dyibzib                 //                  hessz(2,ib) = hessz(2,ib) + dedphi*dyibzib
                                   + d2edphi2*dphidyib*dphidzib             ;//     &                             + d2edphi2*dphidyib*dphidzib
                  hessx[3,ib] = hessx[3,ib] + dedphi*dxibzib                 //                  hessx(3,ib) = hessx(3,ib) + dedphi*dxibzib
                                   + d2edphi2*dphidxib*dphidzib             ;//     &                             + d2edphi2*dphidxib*dphidzib
                  hessy[3,ib] = hessy[3,ib] + dedphi*dyibzib                 //                  hessy(3,ib) = hessy(3,ib) + dedphi*dyibzib
                                   + d2edphi2*dphidyib*dphidzib             ;//     &                             + d2edphi2*dphidyib*dphidzib
                  hessz[3,ib] = hessz[3,ib] + dedphi*dzibzib                 //                  hessz(3,ib) = hessz(3,ib) + dedphi*dzibzib
                                   + d2edphi2*dphidzib*dphidzib             ;//     &                             + d2edphi2*dphidzib*dphidzib
                  hessx[1,ia] = hessx[1,ia] + dedphi*dxiaxib                 //                  hessx(1,ia) = hessx(1,ia) + dedphi*dxiaxib
                                   + d2edphi2*dphidxib*dphidxia             ;//     &                             + d2edphi2*dphidxib*dphidxia
                  hessy[1,ia] = hessy[1,ia] + dedphi*dxiayib                 //                  hessy(1,ia) = hessy(1,ia) + dedphi*dxiayib
                                   + d2edphi2*dphidyib*dphidxia             ;//     &                             + d2edphi2*dphidyib*dphidxia
                  hessz[1,ia] = hessz[1,ia] + dedphi*dxiazib                 //                  hessz(1,ia) = hessz(1,ia) + dedphi*dxiazib
                                   + d2edphi2*dphidzib*dphidxia             ;//     &                             + d2edphi2*dphidzib*dphidxia
                  hessx[2,ia] = hessx[2,ia] + dedphi*dyiaxib                 //                  hessx(2,ia) = hessx(2,ia) + dedphi*dyiaxib
                                   + d2edphi2*dphidxib*dphidyia             ;//     &                             + d2edphi2*dphidxib*dphidyia
                  hessy[2,ia] = hessy[2,ia] + dedphi*dyiayib                 //                  hessy(2,ia) = hessy(2,ia) + dedphi*dyiayib
                                   + d2edphi2*dphidyib*dphidyia             ;//     &                             + d2edphi2*dphidyib*dphidyia
                  hessz[2,ia] = hessz[2,ia] + dedphi*dyiazib                 //                  hessz(2,ia) = hessz(2,ia) + dedphi*dyiazib
                                   + d2edphi2*dphidzib*dphidyia             ;//     &                             + d2edphi2*dphidzib*dphidyia
                  hessx[3,ia] = hessx[3,ia] + dedphi*dziaxib                 //                  hessx(3,ia) = hessx(3,ia) + dedphi*dziaxib
                                   + d2edphi2*dphidxib*dphidzia             ;//     &                             + d2edphi2*dphidxib*dphidzia
                  hessy[3,ia] = hessy[3,ia] + dedphi*dziayib                 //                  hessy(3,ia) = hessy(3,ia) + dedphi*dziayib
                                   + d2edphi2*dphidyib*dphidzia             ;//     &                             + d2edphi2*dphidyib*dphidzia
                  hessz[3,ia] = hessz[3,ia] + dedphi*dziazib                 //                  hessz(3,ia) = hessz(3,ia) + dedphi*dziazib
                                   + d2edphi2*dphidzib*dphidzia             ;//     &                             + d2edphi2*dphidzib*dphidzia
                  hessx[1,ic] = hessx[1,ic] + dedphi*dxibxic                 //                  hessx(1,ic) = hessx(1,ic) + dedphi*dxibxic
                                   + d2edphi2*dphidxib*dphidxic             ;//     &                             + d2edphi2*dphidxib*dphidxic
                  hessy[1,ic] = hessy[1,ic] + dedphi*dyibxic                 //                  hessy(1,ic) = hessy(1,ic) + dedphi*dyibxic
                                   + d2edphi2*dphidyib*dphidxic             ;//     &                             + d2edphi2*dphidyib*dphidxic
                  hessz[1,ic] = hessz[1,ic] + dedphi*dzibxic                 //                  hessz(1,ic) = hessz(1,ic) + dedphi*dzibxic
                                   + d2edphi2*dphidzib*dphidxic             ;//     &                             + d2edphi2*dphidzib*dphidxic
                  hessx[2,ic] = hessx[2,ic] + dedphi*dxibyic                 //                  hessx(2,ic) = hessx(2,ic) + dedphi*dxibyic
                                   + d2edphi2*dphidxib*dphidyic             ;//     &                             + d2edphi2*dphidxib*dphidyic
                  hessy[2,ic] = hessy[2,ic] + dedphi*dyibyic                 //                  hessy(2,ic) = hessy(2,ic) + dedphi*dyibyic
                                   + d2edphi2*dphidyib*dphidyic             ;//     &                             + d2edphi2*dphidyib*dphidyic
                  hessz[2,ic] = hessz[2,ic] + dedphi*dzibyic                 //                  hessz(2,ic) = hessz(2,ic) + dedphi*dzibyic
                                   + d2edphi2*dphidzib*dphidyic             ;//     &                             + d2edphi2*dphidzib*dphidyic
                  hessx[3,ic] = hessx[3,ic] + dedphi*dxibzic                 //                  hessx(3,ic) = hessx(3,ic) + dedphi*dxibzic
                                   + d2edphi2*dphidxib*dphidzic             ;//     &                             + d2edphi2*dphidxib*dphidzic
                  hessy[3,ic] = hessy[3,ic] + dedphi*dyibzic                 //                  hessy(3,ic) = hessy(3,ic) + dedphi*dyibzic
                                   + d2edphi2*dphidyib*dphidzic             ;//     &                             + d2edphi2*dphidyib*dphidzic
                  hessz[3,ic] = hessz[3,ic] + dedphi*dzibzic                 //                  hessz(3,ic) = hessz(3,ic) + dedphi*dzibzic
                                   + d2edphi2*dphidzib*dphidzic             ;//     &                             + d2edphi2*dphidzib*dphidzic
                  hessx[1,id] = hessx[1,id] + dedphi*dxibxid                 //                  hessx(1,id) = hessx(1,id) + dedphi*dxibxid
                                   + d2edphi2*dphidxib*dphidxid             ;//     &                             + d2edphi2*dphidxib*dphidxid
                  hessy[1,id] = hessy[1,id] + dedphi*dyibxid                 //                  hessy(1,id) = hessy(1,id) + dedphi*dyibxid
                                   + d2edphi2*dphidyib*dphidxid             ;//     &                             + d2edphi2*dphidyib*dphidxid
                  hessz[1,id] = hessz[1,id] + dedphi*dzibxid                 //                  hessz(1,id) = hessz(1,id) + dedphi*dzibxid
                                   + d2edphi2*dphidzib*dphidxid             ;//     &                             + d2edphi2*dphidzib*dphidxid
                  hessx[2,id] = hessx[2,id] + dedphi*dxibyid                 //                  hessx(2,id) = hessx(2,id) + dedphi*dxibyid
                                   + d2edphi2*dphidxib*dphidyid             ;//     &                             + d2edphi2*dphidxib*dphidyid
                  hessy[2,id] = hessy[2,id] + dedphi*dyibyid                 //                  hessy(2,id) = hessy(2,id) + dedphi*dyibyid
                                   + d2edphi2*dphidyib*dphidyid             ;//     &                             + d2edphi2*dphidyib*dphidyid
                  hessz[2,id] = hessz[2,id] + dedphi*dzibyid                 //                  hessz(2,id) = hessz(2,id) + dedphi*dzibyid
                                   + d2edphi2*dphidzib*dphidyid             ;//     &                             + d2edphi2*dphidzib*dphidyid
                  hessx[3,id] = hessx[3,id] + dedphi*dxibzid                 //                  hessx(3,id) = hessx(3,id) + dedphi*dxibzid
                                   + d2edphi2*dphidxib*dphidzid             ;//     &                             + d2edphi2*dphidxib*dphidzid
                  hessy[3,id] = hessy[3,id] + dedphi*dyibzid                 //                  hessy(3,id) = hessy(3,id) + dedphi*dyibzid
                                   + d2edphi2*dphidyib*dphidzid             ;//     &                             + d2edphi2*dphidyib*dphidzid
                  hessz[3,id] = hessz[3,id] + dedphi*dzibzid                 //                  hessz(3,id) = hessz(3,id) + dedphi*dzibzid
                                   + d2edphi2*dphidzib*dphidzid             ;//     &                             + d2edphi2*dphidzib*dphidzid
               } else if (i  ==  ic) {                                      ;//               else if (i .eq. ic) then
                  hessx[1,ic] = hessx[1,ic] + dedphi*dxicxic                 //                  hessx(1,ic) = hessx(1,ic) + dedphi*dxicxic
                                   + d2edphi2*dphidxic*dphidxic             ;//     &                             + d2edphi2*dphidxic*dphidxic
                  hessy[1,ic] = hessy[1,ic] + dedphi*dxicyic                 //                  hessy(1,ic) = hessy(1,ic) + dedphi*dxicyic
                                   + d2edphi2*dphidxic*dphidyic             ;//     &                             + d2edphi2*dphidxic*dphidyic
                  hessz[1,ic] = hessz[1,ic] + dedphi*dxiczic                 //                  hessz(1,ic) = hessz(1,ic) + dedphi*dxiczic
                                   + d2edphi2*dphidxic*dphidzic             ;//     &                             + d2edphi2*dphidxic*dphidzic
                  hessx[2,ic] = hessx[2,ic] + dedphi*dxicyic                 //                  hessx(2,ic) = hessx(2,ic) + dedphi*dxicyic
                                   + d2edphi2*dphidxic*dphidyic             ;//     &                             + d2edphi2*dphidxic*dphidyic
                  hessy[2,ic] = hessy[2,ic] + dedphi*dyicyic                 //                  hessy(2,ic) = hessy(2,ic) + dedphi*dyicyic
                                   + d2edphi2*dphidyic*dphidyic             ;//     &                             + d2edphi2*dphidyic*dphidyic
                  hessz[2,ic] = hessz[2,ic] + dedphi*dyiczic                 //                  hessz(2,ic) = hessz(2,ic) + dedphi*dyiczic
                                   + d2edphi2*dphidyic*dphidzic             ;//     &                             + d2edphi2*dphidyic*dphidzic
                  hessx[3,ic] = hessx[3,ic] + dedphi*dxiczic                 //                  hessx(3,ic) = hessx(3,ic) + dedphi*dxiczic
                                   + d2edphi2*dphidxic*dphidzic             ;//     &                             + d2edphi2*dphidxic*dphidzic
                  hessy[3,ic] = hessy[3,ic] + dedphi*dyiczic                 //                  hessy(3,ic) = hessy(3,ic) + dedphi*dyiczic
                                   + d2edphi2*dphidyic*dphidzic             ;//     &                             + d2edphi2*dphidyic*dphidzic
                  hessz[3,ic] = hessz[3,ic] + dedphi*dziczic                 //                  hessz(3,ic) = hessz(3,ic) + dedphi*dziczic
                                   + d2edphi2*dphidzic*dphidzic             ;//     &                             + d2edphi2*dphidzic*dphidzic
                  hessx[1,ia] = hessx[1,ia] + dedphi*dxiaxic                 //                  hessx(1,ia) = hessx(1,ia) + dedphi*dxiaxic
                                   + d2edphi2*dphidxic*dphidxia             ;//     &                             + d2edphi2*dphidxic*dphidxia
                  hessy[1,ia] = hessy[1,ia] + dedphi*dxiayic                 //                  hessy(1,ia) = hessy(1,ia) + dedphi*dxiayic
                                   + d2edphi2*dphidyic*dphidxia             ;//     &                             + d2edphi2*dphidyic*dphidxia
                  hessz[1,ia] = hessz[1,ia] + dedphi*dxiazic                 //                  hessz(1,ia) = hessz(1,ia) + dedphi*dxiazic
                                   + d2edphi2*dphidzic*dphidxia             ;//     &                             + d2edphi2*dphidzic*dphidxia
                  hessx[2,ia] = hessx[2,ia] + dedphi*dyiaxic                 //                  hessx(2,ia) = hessx(2,ia) + dedphi*dyiaxic
                                   + d2edphi2*dphidxic*dphidyia             ;//     &                             + d2edphi2*dphidxic*dphidyia
                  hessy[2,ia] = hessy[2,ia] + dedphi*dyiayic                 //                  hessy(2,ia) = hessy(2,ia) + dedphi*dyiayic
                                   + d2edphi2*dphidyic*dphidyia             ;//     &                             + d2edphi2*dphidyic*dphidyia
                  hessz[2,ia] = hessz[2,ia] + dedphi*dyiazic                 //                  hessz(2,ia) = hessz(2,ia) + dedphi*dyiazic
                                   + d2edphi2*dphidzic*dphidyia             ;//     &                             + d2edphi2*dphidzic*dphidyia
                  hessx[3,ia] = hessx[3,ia] + dedphi*dziaxic                 //                  hessx(3,ia) = hessx(3,ia) + dedphi*dziaxic
                                   + d2edphi2*dphidxic*dphidzia             ;//     &                             + d2edphi2*dphidxic*dphidzia
                  hessy[3,ia] = hessy[3,ia] + dedphi*dziayic                 //                  hessy(3,ia) = hessy(3,ia) + dedphi*dziayic
                                   + d2edphi2*dphidyic*dphidzia             ;//     &                             + d2edphi2*dphidyic*dphidzia
                  hessz[3,ia] = hessz[3,ia] + dedphi*dziazic                 //                  hessz(3,ia) = hessz(3,ia) + dedphi*dziazic
                                   + d2edphi2*dphidzic*dphidzia             ;//     &                             + d2edphi2*dphidzic*dphidzia
                  hessx[1,ib] = hessx[1,ib] + dedphi*dxibxic                 //                  hessx(1,ib) = hessx(1,ib) + dedphi*dxibxic
                                   + d2edphi2*dphidxic*dphidxib             ;//     &                             + d2edphi2*dphidxic*dphidxib
                  hessy[1,ib] = hessy[1,ib] + dedphi*dxibyic                 //                  hessy(1,ib) = hessy(1,ib) + dedphi*dxibyic
                                   + d2edphi2*dphidyic*dphidxib             ;//     &                             + d2edphi2*dphidyic*dphidxib
                  hessz[1,ib] = hessz[1,ib] + dedphi*dxibzic                 //                  hessz(1,ib) = hessz(1,ib) + dedphi*dxibzic
                                   + d2edphi2*dphidzic*dphidxib             ;//     &                             + d2edphi2*dphidzic*dphidxib
                  hessx[2,ib] = hessx[2,ib] + dedphi*dyibxic                 //                  hessx(2,ib) = hessx(2,ib) + dedphi*dyibxic
                                   + d2edphi2*dphidxic*dphidyib             ;//     &                             + d2edphi2*dphidxic*dphidyib
                  hessy[2,ib] = hessy[2,ib] + dedphi*dyibyic                 //                  hessy(2,ib) = hessy(2,ib) + dedphi*dyibyic
                                   + d2edphi2*dphidyic*dphidyib             ;//     &                             + d2edphi2*dphidyic*dphidyib
                  hessz[2,ib] = hessz[2,ib] + dedphi*dyibzic                 //                  hessz(2,ib) = hessz(2,ib) + dedphi*dyibzic
                                   + d2edphi2*dphidzic*dphidyib             ;//     &                             + d2edphi2*dphidzic*dphidyib
                  hessx[3,ib] = hessx[3,ib] + dedphi*dzibxic                 //                  hessx(3,ib) = hessx(3,ib) + dedphi*dzibxic
                                   + d2edphi2*dphidxic*dphidzib             ;//     &                             + d2edphi2*dphidxic*dphidzib
                  hessy[3,ib] = hessy[3,ib] + dedphi*dzibyic                 //                  hessy(3,ib) = hessy(3,ib) + dedphi*dzibyic
                                   + d2edphi2*dphidyic*dphidzib             ;//     &                             + d2edphi2*dphidyic*dphidzib
                  hessz[3,ib] = hessz[3,ib] + dedphi*dzibzic                 //                  hessz(3,ib) = hessz(3,ib) + dedphi*dzibzic
                                   + d2edphi2*dphidzic*dphidzib             ;//     &                             + d2edphi2*dphidzic*dphidzib
                  hessx[1,id] = hessx[1,id] + dedphi*dxicxid                 //                  hessx(1,id) = hessx(1,id) + dedphi*dxicxid
                                   + d2edphi2*dphidxic*dphidxid             ;//     &                             + d2edphi2*dphidxic*dphidxid
                  hessy[1,id] = hessy[1,id] + dedphi*dyicxid                 //                  hessy(1,id) = hessy(1,id) + dedphi*dyicxid
                                   + d2edphi2*dphidyic*dphidxid             ;//     &                             + d2edphi2*dphidyic*dphidxid
                  hessz[1,id] = hessz[1,id] + dedphi*dzicxid                 //                  hessz(1,id) = hessz(1,id) + dedphi*dzicxid
                                   + d2edphi2*dphidzic*dphidxid             ;//     &                             + d2edphi2*dphidzic*dphidxid
                  hessx[2,id] = hessx[2,id] + dedphi*dxicyid                 //                  hessx(2,id) = hessx(2,id) + dedphi*dxicyid
                                   + d2edphi2*dphidxic*dphidyid             ;//     &                             + d2edphi2*dphidxic*dphidyid
                  hessy[2,id] = hessy[2,id] + dedphi*dyicyid                 //                  hessy(2,id) = hessy(2,id) + dedphi*dyicyid
                                   + d2edphi2*dphidyic*dphidyid             ;//     &                             + d2edphi2*dphidyic*dphidyid
                  hessz[2,id] = hessz[2,id] + dedphi*dzicyid                 //                  hessz(2,id) = hessz(2,id) + dedphi*dzicyid
                                   + d2edphi2*dphidzic*dphidyid             ;//     &                             + d2edphi2*dphidzic*dphidyid
                  hessx[3,id] = hessx[3,id] + dedphi*dxiczid                 //                  hessx(3,id) = hessx(3,id) + dedphi*dxiczid
                                   + d2edphi2*dphidxic*dphidzid             ;//     &                             + d2edphi2*dphidxic*dphidzid
                  hessy[3,id] = hessy[3,id] + dedphi*dyiczid                 //                  hessy(3,id) = hessy(3,id) + dedphi*dyiczid
                                   + d2edphi2*dphidyic*dphidzid             ;//     &                             + d2edphi2*dphidyic*dphidzid
                  hessz[3,id] = hessz[3,id] + dedphi*dziczid                 //                  hessz(3,id) = hessz(3,id) + dedphi*dziczid
                                   + d2edphi2*dphidzic*dphidzid             ;//     &                             + d2edphi2*dphidzic*dphidzid
               } else if (i  ==  id) {                                       //               else if (i .eq. id) then
                  hessx[1,id] = hessx[1,id] + dedphi*dxidxid                 //                  hessx(1,id) = hessx(1,id) + dedphi*dxidxid
                                   + d2edphi2*dphidxid*dphidxid             ;//     &                             + d2edphi2*dphidxid*dphidxid
                  hessy[1,id] = hessy[1,id] + dedphi*dxidyid                 //                  hessy(1,id) = hessy(1,id) + dedphi*dxidyid
                                   + d2edphi2*dphidxid*dphidyid             ;//     &                             + d2edphi2*dphidxid*dphidyid
                  hessz[1,id] = hessz[1,id] + dedphi*dxidzid                 //                  hessz(1,id) = hessz(1,id) + dedphi*dxidzid
                                   + d2edphi2*dphidxid*dphidzid             ;//     &                             + d2edphi2*dphidxid*dphidzid
                  hessx[2,id] = hessx[2,id] + dedphi*dxidyid                 //                  hessx(2,id) = hessx(2,id) + dedphi*dxidyid
                                   + d2edphi2*dphidxid*dphidyid             ;//     &                             + d2edphi2*dphidxid*dphidyid
                  hessy[2,id] = hessy[2,id] + dedphi*dyidyid                 //                  hessy(2,id) = hessy(2,id) + dedphi*dyidyid
                                   + d2edphi2*dphidyid*dphidyid             ;//     &                             + d2edphi2*dphidyid*dphidyid
                  hessz[2,id] = hessz[2,id] + dedphi*dyidzid                 //                  hessz(2,id) = hessz(2,id) + dedphi*dyidzid
                                   + d2edphi2*dphidyid*dphidzid             ;//     &                             + d2edphi2*dphidyid*dphidzid
                  hessx[3,id] = hessx[3,id] + dedphi*dxidzid                 //                  hessx(3,id) = hessx(3,id) + dedphi*dxidzid
                                   + d2edphi2*dphidxid*dphidzid             ;//     &                             + d2edphi2*dphidxid*dphidzid
                  hessy[3,id] = hessy[3,id] + dedphi*dyidzid                 //                  hessy(3,id) = hessy(3,id) + dedphi*dyidzid
                                   + d2edphi2*dphidyid*dphidzid             ;//     &                             + d2edphi2*dphidyid*dphidzid
                  hessz[3,id] = hessz[3,id] + dedphi*dzidzid                 //                  hessz(3,id) = hessz(3,id) + dedphi*dzidzid
                                   + d2edphi2*dphidzid*dphidzid             ;//     &                             + d2edphi2*dphidzid*dphidzid
                  hessx[1,ia] = hessx[1,ia] + dedphi*dxiaxid                 //                  hessx(1,ia) = hessx(1,ia) + dedphi*dxiaxid
                                   + d2edphi2*dphidxid*dphidxia             ;//     &                             + d2edphi2*dphidxid*dphidxia
                  hessy[1,ia] = hessy[1,ia] + dedphi*dxiayid                 //                  hessy(1,ia) = hessy(1,ia) + dedphi*dxiayid
                                   + d2edphi2*dphidyid*dphidxia             ;//     &                             + d2edphi2*dphidyid*dphidxia
                  hessz[1,ia] = hessz[1,ia] + dedphi*dxiazid                 //                  hessz(1,ia) = hessz(1,ia) + dedphi*dxiazid
                                   + d2edphi2*dphidzid*dphidxia             ;//     &                             + d2edphi2*dphidzid*dphidxia
                  hessx[2,ia] = hessx[2,ia] + dedphi*dyiaxid                 //                  hessx(2,ia) = hessx(2,ia) + dedphi*dyiaxid
                                   + d2edphi2*dphidxid*dphidyia             ;//     &                             + d2edphi2*dphidxid*dphidyia
                  hessy[2,ia] = hessy[2,ia] + dedphi*dyiayid                 //                  hessy(2,ia) = hessy(2,ia) + dedphi*dyiayid
                                   + d2edphi2*dphidyid*dphidyia             ;//     &                             + d2edphi2*dphidyid*dphidyia
                  hessz[2,ia] = hessz[2,ia] + dedphi*dyiazid                 //                  hessz(2,ia) = hessz(2,ia) + dedphi*dyiazid
                                   + d2edphi2*dphidzid*dphidyia             ;//     &                             + d2edphi2*dphidzid*dphidyia
                  hessx[3,ia] = hessx[3,ia] + dedphi*dziaxid                 //                  hessx(3,ia) = hessx(3,ia) + dedphi*dziaxid
                                   + d2edphi2*dphidxid*dphidzia             ;//     &                             + d2edphi2*dphidxid*dphidzia
                  hessy[3,ia] = hessy[3,ia] + dedphi*dziayid                 //                  hessy(3,ia) = hessy(3,ia) + dedphi*dziayid
                                   + d2edphi2*dphidyid*dphidzia             ;//     &                             + d2edphi2*dphidyid*dphidzia
                  hessz[3,ia] = hessz[3,ia] + dedphi*dziazid                 //                  hessz(3,ia) = hessz(3,ia) + dedphi*dziazid
                                   + d2edphi2*dphidzid*dphidzia             ;//     &                             + d2edphi2*dphidzid*dphidzia
                  hessx[1,ib] = hessx[1,ib] + dedphi*dxibxid                 //                  hessx(1,ib) = hessx(1,ib) + dedphi*dxibxid
                                   + d2edphi2*dphidxid*dphidxib             ;//     &                             + d2edphi2*dphidxid*dphidxib
                  hessy[1,ib] = hessy[1,ib] + dedphi*dxibyid                 //                  hessy(1,ib) = hessy(1,ib) + dedphi*dxibyid
                                   + d2edphi2*dphidyid*dphidxib             ;//     &                             + d2edphi2*dphidyid*dphidxib
                  hessz[1,ib] = hessz[1,ib] + dedphi*dxibzid                 //                  hessz(1,ib) = hessz(1,ib) + dedphi*dxibzid
                                   + d2edphi2*dphidzid*dphidxib             ;//     &                             + d2edphi2*dphidzid*dphidxib
                  hessx[2,ib] = hessx[2,ib] + dedphi*dyibxid                 //                  hessx(2,ib) = hessx(2,ib) + dedphi*dyibxid
                                   + d2edphi2*dphidxid*dphidyib             ;//     &                             + d2edphi2*dphidxid*dphidyib
                  hessy[2,ib] = hessy[2,ib] + dedphi*dyibyid                 //                  hessy(2,ib) = hessy(2,ib) + dedphi*dyibyid
                                   + d2edphi2*dphidyid*dphidyib             ;//     &                             + d2edphi2*dphidyid*dphidyib
                  hessz[2,ib] = hessz[2,ib] + dedphi*dyibzid                 //                  hessz(2,ib) = hessz(2,ib) + dedphi*dyibzid
                                   + d2edphi2*dphidzid*dphidyib             ;//     &                             + d2edphi2*dphidzid*dphidyib
                  hessx[3,ib] = hessx[3,ib] + dedphi*dzibxid                 //                  hessx(3,ib) = hessx(3,ib) + dedphi*dzibxid
                                   + d2edphi2*dphidxid*dphidzib             ;//     &                             + d2edphi2*dphidxid*dphidzib
                  hessy[3,ib] = hessy[3,ib] + dedphi*dzibyid                 //                  hessy(3,ib) = hessy(3,ib) + dedphi*dzibyid
                                   + d2edphi2*dphidyid*dphidzib             ;//     &                             + d2edphi2*dphidyid*dphidzib
                  hessz[3,ib] = hessz[3,ib] + dedphi*dzibzid                 //                  hessz(3,ib) = hessz(3,ib) + dedphi*dzibzid
                                   + d2edphi2*dphidzid*dphidzib             ;//     &                             + d2edphi2*dphidzid*dphidzib
                  hessx[1,ic] = hessx[1,ic] + dedphi*dxicxid                 //                  hessx(1,ic) = hessx(1,ic) + dedphi*dxicxid
                                   + d2edphi2*dphidxid*dphidxic             ;//     &                             + d2edphi2*dphidxid*dphidxic
                  hessy[1,ic] = hessy[1,ic] + dedphi*dxicyid                 //                  hessy(1,ic) = hessy(1,ic) + dedphi*dxicyid
                                   + d2edphi2*dphidyid*dphidxic             ;//     &                             + d2edphi2*dphidyid*dphidxic
                  hessz[1,ic] = hessz[1,ic] + dedphi*dxiczid                 //                  hessz(1,ic) = hessz(1,ic) + dedphi*dxiczid
                                   + d2edphi2*dphidzid*dphidxic             ;//     &                             + d2edphi2*dphidzid*dphidxic
                  hessx[2,ic] = hessx[2,ic] + dedphi*dyicxid                 //                  hessx(2,ic) = hessx(2,ic) + dedphi*dyicxid
                                   + d2edphi2*dphidxid*dphidyic             ;//     &                             + d2edphi2*dphidxid*dphidyic
                  hessy[2,ic] = hessy[2,ic] + dedphi*dyicyid                 //                  hessy(2,ic) = hessy(2,ic) + dedphi*dyicyid
                                   + d2edphi2*dphidyid*dphidyic             ;//     &                             + d2edphi2*dphidyid*dphidyic
                  hessz[2,ic] = hessz[2,ic] + dedphi*dyiczid                 //                  hessz(2,ic) = hessz(2,ic) + dedphi*dyiczid
                                   + d2edphi2*dphidzid*dphidyic             ;//     &                             + d2edphi2*dphidzid*dphidyic
                  hessx[3,ic] = hessx[3,ic] + dedphi*dzicxid                 //                  hessx(3,ic) = hessx(3,ic) + dedphi*dzicxid
                                   + d2edphi2*dphidxid*dphidzic             ;//     &                             + d2edphi2*dphidxid*dphidzic
                  hessy[3,ic] = hessy[3,ic] + dedphi*dzicyid                 //                  hessy(3,ic) = hessy(3,ic) + dedphi*dzicyid
                                   + d2edphi2*dphidyid*dphidzic             ;//     &                             + d2edphi2*dphidyid*dphidzic
                  hessz[3,ic] = hessz[3,ic] + dedphi*dziczid                 //                  hessz(3,ic) = hessz(3,ic) + dedphi*dziczid
                                   + d2edphi2*dphidzid*dphidzic             ;//     &                             + d2edphi2*dphidzid*dphidzic
               }                                                            ;//               end if
            }                                                               ;//            end if
         }                                                                  ;//         end if
      }                                                                     ;//      end do
      return                                                                ;//      return
    }                                                                        //      end
}
}
}
