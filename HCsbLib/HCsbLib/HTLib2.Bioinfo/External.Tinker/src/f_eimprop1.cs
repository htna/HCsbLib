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
///   #################################################################     ;//c     #################################################################
///   ##                                                             ##     ;//c     ##                                                             ##
///   ##  subroutine eimprop1  --  impr. dihedral energy & gradient  ##     ;//c     ##  subroutine eimprop1  --  impr. dihedral energy & gradient  ##
///   ##                                                             ##     ;//c     ##                                                             ##
///   #################################################################     ;//c     #################################################################
///                                                                         ;//c
///                                                                         ;//c
///   "eimprop1" calculates improper dihedral energy and its                ;//c     "eimprop1" calculates improper dihedral energy and its
///   first derivatives with respect to Cartesian coordinates               ;//c     first derivatives with respect to Cartesian coordinates
///                                                                         ;//c
///                                                                         ;//c
    public void eimprop1() {                                                ;//      subroutine eimprop1
                                                                            ;//      implicit none
                                                                            ;//      include 'sizes.i'
                                                                            ;//      include 'atoms.i'
                                                                            ;//      include 'bound.i'
                                                                            ;//      include 'deriv.i'
                                                                            ;//      include 'energi.i'
                                                                            ;//      include 'group.i'
                                                                            ;//      include 'improp.i'
                                                                            ;//      include 'math.i'
                                                                            ;//      include 'torpot.i'
                                                                            ;//      include 'usage.i'
                                                                            ;//      include 'virial.i'
      int i,ia,ib,ic,id                                                     ;//      integer i,ia,ib,ic,id
      double e,dedphi                                                       ;//      real*8 e,dedphi
      double dt,fgrp                                                        ;//      real*8 dt,fgrp
      double ideal,force                                                    ;//      real*8 ideal,force
      double cosine,sine                                                    ;//      real*8 cosine,sine
      double rcb,angle                                                      ;//      real*8 rcb,angle
      double xt,yt,zt                                                       ;//      real*8 xt,yt,zt
      double xu,yu,zu                                                       ;//      real*8 xu,yu,zu
      double xtu,ytu,ztu                                                    ;//      real*8 xtu,ytu,ztu
      double rt2,ru2,rtru                                                   ;//      real*8 rt2,ru2,rtru
      double xia,yia,zia                                                    ;//      real*8 xia,yia,zia
      double xib,yib,zib                                                    ;//      real*8 xib,yib,zib
      double xic,yic,zic                                                    ;//      real*8 xic,yic,zic
      double xid,yid,zid                                                    ;//      real*8 xid,yid,zid
      double xba,yba,zba                                                    ;//      real*8 xba,yba,zba
      double xcb,ycb,zcb                                                    ;//      real*8 xcb,ycb,zcb
      double xdc,ydc,zdc                                                    ;//      real*8 xdc,ydc,zdc
      double xca,yca,zca                                                    ;//      real*8 xca,yca,zca
      double xdb,ydb,zdb                                                    ;//      real*8 xdb,ydb,zdb
      double dedxt,dedyt,dedzt                                              ;//      real*8 dedxt,dedyt,dedzt
      double dedxu,dedyu,dedzu                                              ;//      real*8 dedxu,dedyu,dedzu
      double dedxia,dedyia,dedzia                                           ;//      real*8 dedxia,dedyia,dedzia
      double dedxib,dedyib,dedzib                                           ;//      real*8 dedxib,dedyib,dedzib
      double dedxic,dedyic,dedzic                                           ;//      real*8 dedxic,dedyic,dedzic
      double dedxid,dedyid,dedzid                                           ;//      real*8 dedxid,dedyid,dedzid
      double vxx,vyy,vzz                                                    ;//      real*8 vxx,vyy,vzz
      double vyx,vzx,vzy                                                    ;//      real*8 vyx,vzx,vzy
      bool proceed                                                          ;//      logical proceed
      fgrp = double.NaN                                                     ;//c
///                                                                         ;//c
///   zero out energy and first derivative components                       ;//c     zero out energy and first derivative components
///                                                                         ;//c
      eid = 0.0e0                                                           ;//      eid = 0.0d0
      for(i=1; i<=n; i++) {                                                 ;//      do i = 1, n
         deid[1,i] = 0.0e0                                                  ;//         deid(1,i) = 0.0d0
         deid[2,i] = 0.0e0                                                  ;//         deid(2,i) = 0.0d0
         deid[3,i] = 0.0e0                                                  ;//         deid(3,i) = 0.0d0
      }                                                                     ;//      end do
///                                                                         ;//c
///   calculate the improper dihedral angle energy term                     ;//c     calculate the improper dihedral angle energy term
///                                                                         ;//c
      for(i=1; i<=niprop; i++) {                                            ;//      do i = 1, niprop
         ia = iiprop[1,i]                                                   ;//         ia = iiprop(1,i)
         ib = iiprop[2,i]                                                   ;//         ib = iiprop(2,i)
         ic = iiprop[3,i]                                                   ;//         ic = iiprop(3,i)
         id = iiprop[4,i]                                                   ;//         id = iiprop(4,i)
///                                                                         ;//c
///   decide whether to compute the current interaction                     ;//c     decide whether to compute the current interaction
///                                                                         ;//c
         proceed =  true                                                    ;//         proceed = .true.
         if (use_group) groups(out proceed,out fgrp,ia,ib,ic,id,0,0)        ;//         if (use_group)  call groups (proceed,fgrp,ia,ib,ic,id,0,0)
         if (proceed)  proceed = (use[ia]  ||  use[ib]  ||                   //         if (proceed)  proceed = (use(ia) .or. use(ib) .or.
                                    use[ic]  ||  use[id])                   ;//     &                              use(ic) .or. use(id))
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
               ideal = vprop[i]                                             ;//               ideal = vprop(i)
               force = kprop[i]                                             ;//               force = kprop(i)
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
///   calculate improper energy and master chain rule term                  ;//c     calculate improper energy and master chain rule term
///                                                                         ;//c
               e = idihunit * force * pow(dt,2)                             ;//               e = idihunit * force * dt**2
               dedphi = 2.0e0 * idihunit * force * dt                       ;//               dedphi = 2.0d0 * idihunit * force * dt
///                                                                         ;//c
///   scale the interaction based on its group membership                   ;//c     scale the interaction based on its group membership
///                                                                         ;//c
               if (use_group) {                                             ;//               if (use_group) then
                  e = e * fgrp                                              ;//                  e = e * fgrp
                  dedphi = dedphi * fgrp                                    ;//                  dedphi = dedphi * fgrp
               }                                                            ;//               end if
///                                                                         ;//c
///   chain rule terms for first derivative components                      ;//c     chain rule terms for first derivative components
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
               dedxt = dedphi * (yt*zcb - ycb*zt) / (rt2*rcb)               ;//               dedxt = dedphi * (yt*zcb - ycb*zt) / (rt2*rcb)
               dedyt = dedphi * (zt*xcb - zcb*xt) / (rt2*rcb)               ;//               dedyt = dedphi * (zt*xcb - zcb*xt) / (rt2*rcb)
               dedzt = dedphi * (xt*ycb - xcb*yt) / (rt2*rcb)               ;//               dedzt = dedphi * (xt*ycb - xcb*yt) / (rt2*rcb)
               dedxu = -dedphi * (yu*zcb - ycb*zu) / (ru2*rcb)              ;//               dedxu = -dedphi * (yu*zcb - ycb*zu) / (ru2*rcb)
               dedyu = -dedphi * (zu*xcb - zcb*xu) / (ru2*rcb)              ;//               dedyu = -dedphi * (zu*xcb - zcb*xu) / (ru2*rcb)
               dedzu = -dedphi * (xu*ycb - xcb*yu) / (ru2*rcb)              ;//               dedzu = -dedphi * (xu*ycb - xcb*yu) / (ru2*rcb)
///                                                                         ;//c
///   compute first derivative components for this angle                    ;//c     compute first derivative components for this angle
///                                                                         ;//c
               dedxia = zcb*dedyt - ycb*dedzt                               ;//               dedxia = zcb*dedyt - ycb*dedzt
               dedyia = xcb*dedzt - zcb*dedxt                               ;//               dedyia = xcb*dedzt - zcb*dedxt
               dedzia = ycb*dedxt - xcb*dedyt                               ;//               dedzia = ycb*dedxt - xcb*dedyt
               dedxib = yca*dedzt - zca*dedyt + zdc*dedyu - ydc*dedzu       ;//               dedxib = yca*dedzt - zca*dedyt + zdc*dedyu - ydc*dedzu
               dedyib = zca*dedxt - xca*dedzt + xdc*dedzu - zdc*dedxu       ;//               dedyib = zca*dedxt - xca*dedzt + xdc*dedzu - zdc*dedxu
               dedzib = xca*dedyt - yca*dedxt + ydc*dedxu - xdc*dedyu       ;//               dedzib = xca*dedyt - yca*dedxt + ydc*dedxu - xdc*dedyu
               dedxic = zba*dedyt - yba*dedzt + ydb*dedzu - zdb*dedyu       ;//               dedxic = zba*dedyt - yba*dedzt + ydb*dedzu - zdb*dedyu
               dedyic = xba*dedzt - zba*dedxt + zdb*dedxu - xdb*dedzu       ;//               dedyic = xba*dedzt - zba*dedxt + zdb*dedxu - xdb*dedzu
               dedzic = yba*dedxt - xba*dedyt + xdb*dedyu - ydb*dedxu       ;//               dedzic = yba*dedxt - xba*dedyt + xdb*dedyu - ydb*dedxu
               dedxid = zcb*dedyu - ycb*dedzu                               ;//               dedxid = zcb*dedyu - ycb*dedzu
               dedyid = xcb*dedzu - zcb*dedxu                               ;//               dedyid = xcb*dedzu - zcb*dedxu
               dedzid = ycb*dedxu - xcb*dedyu                               ;//               dedzid = ycb*dedxu - xcb*dedyu
///                                                                         ;//c
///   calculate improper dihedral energy and derivatives                    ;//c     calculate improper dihedral energy and derivatives
///                                                                         ;//c
               eid = eid + e                                                ;//               eid = eid + e
               deid[1,ia] = deid[1,ia] + dedxia                             ;//               deid(1,ia) = deid(1,ia) + dedxia
               deid[2,ia] = deid[2,ia] + dedyia                             ;//               deid(2,ia) = deid(2,ia) + dedyia
               deid[3,ia] = deid[3,ia] + dedzia                             ;//               deid(3,ia) = deid(3,ia) + dedzia
               deid[1,ib] = deid[1,ib] + dedxib                             ;//               deid(1,ib) = deid(1,ib) + dedxib
               deid[2,ib] = deid[2,ib] + dedyib                             ;//               deid(2,ib) = deid(2,ib) + dedyib
               deid[3,ib] = deid[3,ib] + dedzib                             ;//               deid(3,ib) = deid(3,ib) + dedzib
               deid[1,ic] = deid[1,ic] + dedxic                             ;//               deid(1,ic) = deid(1,ic) + dedxic
               deid[2,ic] = deid[2,ic] + dedyic                             ;//               deid(2,ic) = deid(2,ic) + dedyic
               deid[3,ic] = deid[3,ic] + dedzic                             ;//               deid(3,ic) = deid(3,ic) + dedzic
               deid[1,id] = deid[1,id] + dedxid                             ;//               deid(1,id) = deid(1,id) + dedxid
               deid[2,id] = deid[2,id] + dedyid                             ;//               deid(2,id) = deid(2,id) + dedyid
               deid[3,id] = deid[3,id] + dedzid                             ;//               deid(3,id) = deid(3,id) + dedzid
///                                                                         ;//c
///   increment the internal virial tensor components                       ;//c     increment the internal virial tensor components
///                                                                         ;//c
               vxx = xcb*(dedxic+dedxid) - xba*dedxia + xdc*dedxid          ;//               vxx = xcb*(dedxic+dedxid) - xba*dedxia + xdc*dedxid
               vyx = ycb*(dedxic+dedxid) - yba*dedxia + ydc*dedxid          ;//               vyx = ycb*(dedxic+dedxid) - yba*dedxia + ydc*dedxid
               vzx = zcb*(dedxic+dedxid) - zba*dedxia + zdc*dedxid          ;//               vzx = zcb*(dedxic+dedxid) - zba*dedxia + zdc*dedxid
               vyy = ycb*(dedyic+dedyid) - yba*dedyia + ydc*dedyid          ;//               vyy = ycb*(dedyic+dedyid) - yba*dedyia + ydc*dedyid
               vzy = zcb*(dedyic+dedyid) - zba*dedyia + zdc*dedyid          ;//               vzy = zcb*(dedyic+dedyid) - zba*dedyia + zdc*dedyid
               vzz = zcb*(dedzic+dedzid) - zba*dedzia + zdc*dedzid          ;//               vzz = zcb*(dedzic+dedzid) - zba*dedzia + zdc*dedzid
               vir[1,1] = vir[1,1] + vxx                                    ;//               vir(1,1) = vir(1,1) + vxx
               vir[2,1] = vir[2,1] + vyx                                    ;//               vir(2,1) = vir(2,1) + vyx
               vir[3,1] = vir[3,1] + vzx                                    ;//               vir(3,1) = vir(3,1) + vzx
               vir[1,2] = vir[1,2] + vyx                                    ;//               vir(1,2) = vir(1,2) + vyx
               vir[2,2] = vir[2,2] + vyy                                    ;//               vir(2,2) = vir(2,2) + vyy
               vir[3,2] = vir[3,2] + vzy                                    ;//               vir(3,2) = vir(3,2) + vzy
               vir[1,3] = vir[1,3] + vzx                                    ;//               vir(1,3) = vir(1,3) + vzx
               vir[2,3] = vir[2,3] + vzy                                    ;//               vir(2,3) = vir(2,3) + vzy
               vir[3,3] = vir[3,3] + vzz                                    ;//               vir(3,3) = vir(3,3) + vzz
            }                                                               ;//            end if
         }                                                                  ;//         end if
      }                                                                     ;//      end do
      return                                                                ;//      return
    }                                                                        //      end
}
}
}
