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
///   ########################################################              ;//c     ########################################################
///   ##                                                    ##              ;//c     ##                                                    ##
///   ##  subroutine eimprop  --  improper dihedral energy  ##              ;//c     ##  subroutine eimprop  --  improper dihedral energy  ##
///   ##                                                    ##              ;//c     ##                                                    ##
///   ########################################################              ;//c     ########################################################
///                                                                         ;//c
///                                                                         ;//c
///   "eimprop" calculates the improper dihedral potential energy           ;//c     "eimprop" calculates the improper dihedral potential energy
///                                                                         ;//c
///                                                                         ;//c
    public void eimprop() {                                                 ;//      subroutine eimprop
                                                                            ;//      implicit none
                                                                            ;//      include 'sizes.i'
                                                                            ;//      include 'atoms.i'
                                                                            ;//      include 'bound.i'
                                                                            ;//      include 'energi.i'
                                                                            ;//      include 'group.i'
                                                                            ;//      include 'improp.i'
                                                                            ;//      include 'math.i'
                                                                            ;//      include 'torpot.i'
                                                                            ;//      include 'usage.i'
      int i,ia,ib,ic,id                                                     ;//      integer i,ia,ib,ic,id
      double e,dt,fgrp                                                      ;//      real*8 e,dt,fgrp
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
      bool proceed                                                          ;//      logical proceed
      fgrp = double.NaN                                                     ;//c
///                                                                         ;//c
///   zero out improper dihedral energy                                     ;//c     zero out improper dihedral energy
///                                                                         ;//c
      eid = 0.0e0                                                           ;//      eid = 0.0d0
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
///   calculate the improper dihedral energy                                ;//c     calculate the improper dihedral energy
///                                                                         ;//c
               e = idihunit * force * pow(dt,2)                             ;//               e = idihunit * force * dt**2
///                                                                         ;//c
///   scale the interaction based on its group membership                   ;//c     scale the interaction based on its group membership
///                                                                         ;//c
               if (use_group)  e = e * fgrp                                 ;//               if (use_group)  e = e * fgrp
///                                                                         ;//c
///   increment the total improper dihedral energy                          ;//c     increment the total improper dihedral energy
///                                                                         ;//c
               eid = eid + e                                                ;//               eid = eid + e
            }                                                               ;//            end if
         }                                                                  ;//         end if
      }                                                                     ;//      end do
      return                                                                ;//      return
    }                                                                        //      end
}
}
}
