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
///                                                                     ;//c
///                                                                     ;//c
///     ###################################################             ;//c     ###################################################
///     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##             ;//c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##             ;//c     ##              All Rights Reserved              ##
///     ###################################################             ;//c     ###################################################
///                                                                     ;//c
///     #############################################################   ;//c     #############################################################
///     ##                                                         ##   ;//c     ##                                                         ##
///     ##  subroutine eangle  --  angle bending potential energy  ##   ;//c     ##  subroutine eangle  --  angle bending potential energy  ##
///     ##                                                         ##   ;//c     ##                                                         ##
///     #############################################################   ;//c     #############################################################
///                                                                     ;//c
///                                                                     ;//c
///     "eangle" calculates the angle bending potential energy;         ;//c     "eangle" calculates the angle bending potential energy;
///     projected in-plane angles at trigonal centers, special          ;//c     projected in-plane angles at trigonal centers, special
///     linear or Fourier angle bending terms are optionally used       ;//c     linear or Fourier angle bending terms are optionally used
///                                                                     ;//c
///                                                                     ;//c
    public void eangle() {                                              ;//      subroutine eangle
                                                                        ;//      implicit none
                                                                        ;//      include 'sizes.i'
                                                                        ;//      include 'angle.i'
                                                                        ;//      include 'angpot.i'
                                                                        ;//      include 'atoms.i'
                                                                        ;//      include 'bound.i'
                                                                        ;//      include 'energi.i'
                                                                        ;//      include 'group.i'
                                                                        ;//      include 'math.i'
                                                                        ;//      include 'usage.i'
      int i,ia,ib,ic,id                                                 ;//      integer i,ia,ib,ic,id
      double e,ideal,force                                              ;//      real*8 e,ideal,force
      double fold,factor                                                ;//      real*8 fold,factor
      double dot,cosine                                                 ;//      real*8 dot,cosine
      double angle,fgrp                                                 ;//      real*8 angle,fgrp
      double dt,dt2,dt3,dt4                                             ;//      real*8 dt,dt2,dt3,dt4
      double xia,yia,zia                                                ;//      real*8 xia,yia,zia
      double xib,yib,zib                                                ;//      real*8 xib,yib,zib
      double xic,yic,zic                                                ;//      real*8 xic,yic,zic
      double xid,yid,zid                                                ;//      real*8 xid,yid,zid
      double xab,yab,zab                                                ;//      real*8 xab,yab,zab
      double xcb,ycb,zcb                                                ;//      real*8 xcb,ycb,zcb
      double xad,yad,zad                                                ;//      real*8 xad,yad,zad
      double xbd,ybd,zbd                                                ;//      real*8 xbd,ybd,zbd
      double xcd,ycd,zcd                                                ;//      real*8 xcd,ycd,zcd
      double xip,yip,zip                                                ;//      real*8 xip,yip,zip
      double xap,yap,zap                                                ;//      real*8 xap,yap,zap
      double xcp,ycp,zcp                                                ;//      real*8 xcp,ycp,zcp
      double rab2,rcb2                                                  ;//      real*8 rab2,rcb2
      double rap2,rcp2                                                  ;//      real*8 rap2,rcp2
      double xt,yt,zt                                                   ;//      real*8 xt,yt,zt
      double rt2,delta                                                  ;//      real*8 rt2,delta
      bool proceed                                                      ;//      logical proceed
      fgrp = e = double.NaN;
///                                                                     ;//c
///                                                                     ;//c
///     zero out the angle bending energy component                     ;//c     zero out the angle bending energy component
///                                                                     ;//c
      ea = 0.0e0                                                        ;//      ea = 0.0d0
///                                                                     ;//c
///     calculate the bond angle bending energy term                    ;//c     calculate the bond angle bending energy term
///                                                                     ;//c
      for(i=1;i<=nangle; i++) {                                         ;//      do i = 1, nangle
         ia = iang[1,i]                                                 ;//         ia = iang(1,i)
         ib = iang[2,i]                                                 ;//         ib = iang(2,i)
         ic = iang[3,i]                                                 ;//         ic = iang(3,i)
         id = iang[4,i]                                                 ;//         id = iang(4,i)
         ideal = anat[i]                                                ;//         ideal = anat(i)
         force = ak[i]                                                  ;//         force = ak(i)
///                                                                     ;//c
///     decide whether to compute the current interaction               ;//c     decide whether to compute the current interaction
///                                                                     ;//c
         proceed =  true                                                ;//         proceed = .true.
         if (angtyp[i]  ==  "IN-PLANE") {                               ;//         if (angtyp(i) .eq. 'IN-PLANE') then
            if (use_group) groups(out proceed,out fgrp,ia,ib,ic,id,0,0) ;//            if (use_group)  call groups (proceed,fgrp,ia,ib,ic,id,0,0)
            if (proceed)  proceed = (use[ia]  ||  use[ib]  ||            //            if (proceed)  proceed = (use(ia) .or. use(ib) .or.
                                       use[ic]  ||  use[id])            ;//     &                                 use(ic) .or. use(id))
         } else {                                                       ;//         else
            if (use_group) groups(out proceed,out fgrp,ia,ib,ic,0,0,0)  ;//            if (use_group)  call groups (proceed,fgrp,ia,ib,ic,0,0,0)
            if (proceed)  proceed = (use[ia]  ||  use[ib]  ||  use[ic]) ;//            if (proceed)  proceed = (use(ia) .or. use(ib) .or. use(ic))
         }                                                              ;//         end if
///                                                                     ;//c
///     get the coordinates of the atoms in the angle                   ;//c     get the coordinates of the atoms in the angle
///                                                                     ;//c
         if (proceed) {                                                 ;//         if (proceed) then
            xia = x[ia]                                                 ;//            xia = x(ia)
            yia = y[ia]                                                 ;//            yia = y(ia)
            zia = z[ia]                                                 ;//            zia = z(ia)
            xib = x[ib]                                                 ;//            xib = x(ib)
            yib = y[ib]                                                 ;//            yib = y(ib)
            zib = z[ib]                                                 ;//            zib = z(ib)
            xic = x[ic]                                                 ;//            xic = x(ic)
            yic = y[ic]                                                 ;//            yic = y(ic)
            zic = z[ic]                                                 ;//            zic = z(ic)
///                                                                     ;//c
///     compute the bond angle bending energy                           ;//c     compute the bond angle bending energy
///                                                                     ;//c
            if (angtyp[i]  !=  "IN-PLANE") {                            ;//            if (angtyp(i) .ne. 'IN-PLANE') then
               xab = xia - xib                                          ;//               xab = xia - xib
               yab = yia - yib                                          ;//               yab = yia - yib
               zab = zia - zib                                          ;//               zab = zia - zib
               xcb = xic - xib                                          ;//               xcb = xic - xib
               ycb = yic - yib                                          ;//               ycb = yic - yib
               zcb = zic - zib                                          ;//               zcb = zic - zib
               if (use_polymer) {                                       ;//               if (use_polymer) then
                  image(ref xab,ref yab,ref zab)                        ;//                  call image (xab,yab,zab)
                  image(ref xcb,ref ycb,ref zcb)                        ;//                  call image (xcb,ycb,zcb)
               }                                                        ;//               end if
               rab2 = xab*xab + yab*yab + zab*zab                       ;//               rab2 = xab*xab + yab*yab + zab*zab
               rcb2 = xcb*xcb + ycb*ycb + zcb*zcb                       ;//               rcb2 = xcb*xcb + ycb*ycb + zcb*zcb
               if (rab2 != 0.0e0  &&   rcb2 != 0.0e0) {                 ;//               if (rab2.ne.0.0d0 .and. rcb2.ne.0.0d0) then
                  dot = xab*xcb + yab*ycb + zab*zcb                     ;//                  dot = xab*xcb + yab*ycb + zab*zcb
                  cosine = dot / sqrt(rab2*rcb2)                        ;//                  cosine = dot / sqrt(rab2*rcb2)
                  cosine = min(1.0e0,max(-1.0e0,cosine))                ;//                  cosine = min(1.0d0,max(-1.0d0,cosine))
                  angle = radian * acos(cosine)                         ;//                  angle = radian * acos(cosine)
                  if (angtyp[i]  ==  "HARMONIC") {                      ;//                  if (angtyp(i) .eq. 'HARMONIC') then
                     dt = angle - ideal                                 ;//                     dt = angle - ideal
                     dt2 = dt * dt                                      ;//                     dt2 = dt * dt
                     dt3 = dt2 * dt                                     ;//                     dt3 = dt2 * dt
                     dt4 = dt2 * dt2                                    ;//                     dt4 = dt2 * dt2
                     e = angunit * force * dt2                           //                     e = angunit * force * dt2
                            * (1.0e0+cang*dt+qang*dt2+pang*dt3+sang*dt4);//     &                      * (1.0d0+cang*dt+qang*dt2+pang*dt3+sang*dt4)
                  } else if (angtyp[i]  ==  "LINEAR") {                 ;//                  else if (angtyp(i) .eq. 'LINEAR') then
                     factor = 2.0e0 * angunit * pow(radian,2)           ;//                     factor = 2.0d0 * angunit * radian**2
                     e = factor * force * (1.0e0+cosine)                ;//                     e = factor * force * (1.0d0+cosine)
                  } else if (angtyp[i]  ==  "FOURIER") {                ;//                  else if (angtyp(i) .eq. 'FOURIER') then
                     fold = afld[i]                                     ;//                     fold = afld(i)
                     factor = 2.0e0 * angunit * pow((radian/fold),2)    ;//                     factor = 2.0d0 * angunit * (radian/fold)**2
                     cosine = cos((fold*angle-ideal)/radian)            ;//                     cosine = cos((fold*angle-ideal)/radian)
                     e = factor * force * (1.0e0+cosine)                ;//                     e = factor * force * (1.0d0+cosine)
                  }                                                     ;//                  end if
///                                                                     ;//c
///     scale the interaction based on its group membership             ;//c     scale the interaction based on its group membership
///                                                                     ;//c
                  if (use_group)  e = e * fgrp                          ;//                  if (use_group)  e = e * fgrp
///                                                                     ;//c
///     increment the total bond angle bending energy                   ;//c     increment the total bond angle bending energy
///                                                                     ;//c
                  ea = ea + e                                           ;//                  ea = ea + e
               }                                                        ;//               end if
///                                                                     ;//c
///     compute the projected in-plane angle bend energy                ;//c     compute the projected in-plane angle bend energy
///                                                                     ;//c
            } else {                                                    ;//            else
               xid = x[id]                                              ;//               xid = x(id)
               yid = y[id]                                              ;//               yid = y(id)
               zid = z[id]                                              ;//               zid = z(id)
               xad = xia - xid                                          ;//               xad = xia - xid
               yad = yia - yid                                          ;//               yad = yia - yid
               zad = zia - zid                                          ;//               zad = zia - zid
               xbd = xib - xid                                          ;//               xbd = xib - xid
               ybd = yib - yid                                          ;//               ybd = yib - yid
               zbd = zib - zid                                          ;//               zbd = zib - zid
               xcd = xic - xid                                          ;//               xcd = xic - xid
               ycd = yic - yid                                          ;//               ycd = yic - yid
               zcd = zic - zid                                          ;//               zcd = zic - zid
               if (use_polymer) {                                       ;//               if (use_polymer) then
                  image(ref xad,ref yad,ref zad)                        ;//                  call image (xad,yad,zad)
                  image(ref xbd,ref ybd,ref zbd)                        ;//                  call image (xbd,ybd,zbd)
                  image(ref xcd,ref ycd,ref zcd)                        ;//                  call image (xcd,ycd,zcd)
               }                                                        ;//               end if
               xt = yad*zcd - zad*ycd                                   ;//               xt = yad*zcd - zad*ycd
               yt = zad*xcd - xad*zcd                                   ;//               yt = zad*xcd - xad*zcd
               zt = xad*ycd - yad*xcd                                   ;//               zt = xad*ycd - yad*xcd
               rt2 = xt*xt + yt*yt + zt*zt                              ;//               rt2 = xt*xt + yt*yt + zt*zt
               delta = -(xt*xbd + yt*ybd + zt*zbd) / rt2                ;//               delta = -(xt*xbd + yt*ybd + zt*zbd) / rt2
               xip = xib + xt*delta                                     ;//               xip = xib + xt*delta
               yip = yib + yt*delta                                     ;//               yip = yib + yt*delta
               zip = zib + zt*delta                                     ;//               zip = zib + zt*delta
               xap = xia - xip                                          ;//               xap = xia - xip
               yap = yia - yip                                          ;//               yap = yia - yip
               zap = zia - zip                                          ;//               zap = zia - zip
               xcp = xic - xip                                          ;//               xcp = xic - xip
               ycp = yic - yip                                          ;//               ycp = yic - yip
               zcp = zic - zip                                          ;//               zcp = zic - zip
               if (use_polymer) {                                       ;//               if (use_polymer) then
                  image(ref xap,ref yap,ref zap)                        ;//                  call image (xap,yap,zap)
                  image(ref xcp,ref ycp,ref zcp)                        ;//                  call image (xcp,ycp,zcp)
               }                                                        ;//               end if
               rap2 = xap*xap + yap*yap + zap*zap                       ;//               rap2 = xap*xap + yap*yap + zap*zap
               rcp2 = xcp*xcp + ycp*ycp + zcp*zcp                       ;//               rcp2 = xcp*xcp + ycp*ycp + zcp*zcp
               if (rap2 != 0.0e0  &&   rcp2 != 0.0e0) {                 ;//               if (rap2.ne.0.0d0 .and. rcp2.ne.0.0d0) then
                  dot = xap*xcp + yap*ycp + zap*zcp                     ;//                  dot = xap*xcp + yap*ycp + zap*zcp
                  cosine = dot / sqrt(rap2*rcp2)                        ;//                  cosine = dot / sqrt(rap2*rcp2)
                  cosine = min(1.0e0,max(-1.0e0,cosine))                ;//                  cosine = min(1.0d0,max(-1.0d0,cosine))
                  angle = radian * acos(cosine)                         ;//                  angle = radian * acos(cosine)
                  dt = angle - ideal                                    ;//                  dt = angle - ideal
                  dt2 = dt * dt                                         ;//                  dt2 = dt * dt
                  dt3 = dt2 * dt                                        ;//                  dt3 = dt2 * dt
                  dt4 = dt2 * dt2                                       ;//                  dt4 = dt2 * dt2
                  e = angunit * force * dt2                              //                  e = angunit * force * dt2
                         * (1.0e0+cang*dt+qang*dt2+pang*dt3+sang*dt4)   ;//     &                   * (1.0d0+cang*dt+qang*dt2+pang*dt3+sang*dt4)
///                                                                     ;//c
///     scale the interaction based on its group membership             ;//c     scale the interaction based on its group membership
///                                                                     ;//c
                  if (use_group)  e = e * fgrp                          ;//                  if (use_group)  e = e * fgrp
///                                                                     ;//c
///     increment the total bond angle bending energy                   ;//c     increment the total bond angle bending energy
///                                                                     ;//c
                  ea = ea + e                                           ;//                  ea = ea + e
               }                                                        ;//               end if
            }                                                           ;//            end if
         }                                                              ;//         end if
      }                                                                 ;//      end do
      return                                                            ;//      return
    }                                                                    //      end
}
}
}
