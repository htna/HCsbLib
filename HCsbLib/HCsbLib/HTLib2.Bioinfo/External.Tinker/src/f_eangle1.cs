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
///     ###################################################                 ;//c     ###################################################
///     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##                 ;//c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##                 ;//c     ##              All Rights Reserved              ##
///     ###################################################                 ;//c     ###################################################
///                                                                         ;//c
///     #################################################################   ;//c     #################################################################
///     ##                                                             ##   ;//c     ##                                                             ##
///     ##  subroutine eangle1  --  angle bend energy and derivatives  ##   ;//c     ##  subroutine eangle1  --  angle bend energy and derivatives  ##
///     ##                                                             ##   ;//c     ##                                                             ##
///     #################################################################   ;//c     #################################################################
///                                                                         ;//c
///                                                                         ;//c
///     "eangle1" calculates the angle bending potential energy and         ;//c     "eangle1" calculates the angle bending potential energy and
///     the first derivatives with respect to Cartesian coordinates;        ;//c     the first derivatives with respect to Cartesian coordinates;
///     projected in-plane angles at trigonal centers, special linear       ;//c     projected in-plane angles at trigonal centers, special linear
///     or Fourier angle bending terms are optionally used                  ;//c     or Fourier angle bending terms are optionally used
///                                                                         ;//c
///                                                                         ;//c
    public void eangle1() {                                                 ;//      subroutine eangle1
                                                                            ;//      implicit none
                                                                            ;//      include 'sizes.i'
                                                                            ;//      include 'angle.i'
                                                                            ;//      include 'angpot.i'
                                                                            ;//      include 'atoms.i'
                                                                            ;//      include 'bound.i'
                                                                            ;//      include 'deriv.i'
                                                                            ;//      include 'energi.i'
                                                                            ;//      include 'group.i'
                                                                            ;//      include 'math.i'
                                                                            ;//      include 'usage.i'
                                                                            ;//      include 'virial.i'
      int i,ia,ib,ic,id                                                     ;//      integer i,ia,ib,ic,id
      double e,ideal,force                                                  ;//      real*8 e,ideal,force
      double fold,factor,dot                                                ;//      real*8 fold,factor,dot
      double cosine,sine                                                    ;//      real*8 cosine,sine
      double angle,fgrp                                                     ;//      real*8 angle,fgrp
      double dt,dt2,dt3,dt4                                                 ;//      real*8 dt,dt2,dt3,dt4
      double deddt,term                                                     ;//      real*8 deddt,term
      double terma,termc                                                    ;//      real*8 terma,termc
      double xia,yia,zia                                                    ;//      real*8 xia,yia,zia
      double xib,yib,zib                                                    ;//      real*8 xib,yib,zib
      double xic,yic,zic                                                    ;//      real*8 xic,yic,zic
      double xid,yid,zid                                                    ;//      real*8 xid,yid,zid
      double xab,yab,zab                                                    ;//      real*8 xab,yab,zab
      double xcb,ycb,zcb                                                    ;//      real*8 xcb,ycb,zcb
      double xp,yp,zp,rp                                                    ;//      real*8 xp,yp,zp,rp
      double xad,yad,zad                                                    ;//      real*8 xad,yad,zad
      double xbd,ybd,zbd                                                    ;//      real*8 xbd,ybd,zbd
      double xcd,ycd,zcd                                                    ;//      real*8 xcd,ycd,zcd
      double xip,yip,zip                                                    ;//      real*8 xip,yip,zip
      double xap,yap,zap                                                    ;//      real*8 xap,yap,zap
      double xcp,ycp,zcp                                                    ;//      real*8 xcp,ycp,zcp
      double rab2,rcb2                                                      ;//      real*8 rab2,rcb2
      double rap2,rcp2                                                      ;//      real*8 rap2,rcp2
      double xt,yt,zt                                                       ;//      real*8 xt,yt,zt
      double rt2,ptrt2                                                      ;//      real*8 rt2,ptrt2
      double xm,ym,zm,rm                                                    ;//      real*8 xm,ym,zm,rm
      double delta,delta2                                                   ;//      real*8 delta,delta2
      double dedxia,dedyia,dedzia                                           ;//      real*8 dedxia,dedyia,dedzia
      double dedxib,dedyib,dedzib                                           ;//      real*8 dedxib,dedyib,dedzib
      double dedxic,dedyic,dedzic                                           ;//      real*8 dedxic,dedyic,dedzic
      double dedxid,dedyid,dedzid                                           ;//      real*8 dedxid,dedyid,dedzid
      double dedxip,dedyip,dedzip                                           ;//      real*8 dedxip,dedyip,dedzip
      double dpdxia,dpdyia,dpdzia                                           ;//      real*8 dpdxia,dpdyia,dpdzia
      double dpdxic,dpdyic,dpdzic                                           ;//      real*8 dpdxic,dpdyic,dpdzic
      double vxx,vyy,vzz                                                    ;//      real*8 vxx,vyy,vzz
      double vyx,vzx,vzy                                                    ;//      real*8 vyx,vzx,vzy
      bool proceed                                                          ;//      logical proceed
      fgrp = deddt = e = double.NaN;
///                                                                         ;//c
///                                                                         ;//c
///     zero out energy and first derivative components                     ;//c     zero out energy and first derivative components
///                                                                         ;//c
      ea = 0.0e0                                                            ;//      ea = 0.0d0
      for(i=1; i<=n; i++) {                                                 ;//      do i = 1, n
         dea[1,i] = 0.0e0                                                   ;//         dea(1,i) = 0.0d0
         dea[2,i] = 0.0e0                                                   ;//         dea(2,i) = 0.0d0
         dea[3,i] = 0.0e0                                                   ;//         dea(3,i) = 0.0d0
      }                                                                     ;//      end do
///                                                                         ;//c
///     calculate the bond angle bending energy term                        ;//c     calculate the bond angle bending energy term
///                                                                         ;//c
      for(i=1; i<=nangle; i++) {                                            ;//      do i = 1, nangle
         ia = iang[1,i]                                                     ;//         ia = iang(1,i)
         ib = iang[2,i]                                                     ;//         ib = iang(2,i)
         ic = iang[3,i]                                                     ;//         ic = iang(3,i)
         id = iang[4,i]                                                     ;//         id = iang(4,i)
         ideal = anat[i]                                                    ;//         ideal = anat(i)
         force = ak[i]                                                      ;//         force = ak(i)
///                                                                         ;//c
///     decide whether to compute the current interaction                   ;//c     decide whether to compute the current interaction
///                                                                         ;//c
         proceed =  true                                                    ;//         proceed = .true.
         if (angtyp[i]  ==  "IN-PLANE") {                                   ;//         if (angtyp(i) .eq. 'IN-PLANE') then
            if (use_group) groups(out proceed,out fgrp,ia,ib,ic,id,0,0)     ;//            if (use_group)  call groups (proceed,fgrp,ia,ib,ic,id,0,0)
            if (proceed)  proceed = (use[ia]  ||  use[ib]  ||                //            if (proceed)  proceed = (use(ia) .or. use(ib) .or.
                                       use[ic]  ||  use[id])                ;//     &                                 use(ic) .or. use(id))
         } else {                                                           ;//         else
            if (use_group) groups(out proceed,out fgrp,ia,ib,ic,0,0,0)      ;//            if (use_group)  call groups (proceed,fgrp,ia,ib,ic,0,0,0)
            if (proceed)  proceed = (use[ia]  ||  use[ib]  ||  use[ic])     ;//            if (proceed)  proceed = (use(ia) .or. use(ib) .or. use(ic))
         }                                                                  ;//         end if
///                                                                         ;//c
///     get the coordinates of the atoms in the angle                       ;//c     get the coordinates of the atoms in the angle
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
///                                                                         ;//c
///     compute the bond angle bending energy and gradient                  ;//c     compute the bond angle bending energy and gradient
///                                                                         ;//c
            if (angtyp[i]  !=  "IN-PLANE") {                                ;//            if (angtyp(i) .ne. 'IN-PLANE') then
               xab = xia - xib                                              ;//               xab = xia - xib
               yab = yia - yib                                              ;//               yab = yia - yib
               zab = zia - zib                                              ;//               zab = zia - zib
               xcb = xic - xib                                              ;//               xcb = xic - xib
               ycb = yic - yib                                              ;//               ycb = yic - yib
               zcb = zic - zib                                              ;//               zcb = zic - zib
               if (use_polymer) {                                           ;//               if (use_polymer) then
                  image(ref xab,ref yab,ref zab)                            ;//                  call image (xab,yab,zab)
                  image(ref xcb,ref ycb,ref zcb)                            ;//                  call image (xcb,ycb,zcb)
               }                                                            ;//               end if
               rab2 = xab*xab + yab*yab + zab*zab                           ;//               rab2 = xab*xab + yab*yab + zab*zab
               rcb2 = xcb*xcb + ycb*ycb + zcb*zcb                           ;//               rcb2 = xcb*xcb + ycb*ycb + zcb*zcb
               if (rab2 != 0.0e0  &&   rcb2 != 0.0e0) {                     ;//               if (rab2.ne.0.0d0 .and. rcb2.ne.0.0d0) then
                  xp = ycb*zab - zcb*yab                                    ;//                  xp = ycb*zab - zcb*yab
                  yp = zcb*xab - xcb*zab                                    ;//                  yp = zcb*xab - xcb*zab
                  zp = xcb*yab - ycb*xab                                    ;//                  zp = xcb*yab - ycb*xab
                  rp = sqrt(xp*xp + yp*yp + zp*zp)                          ;//                  rp = sqrt(xp*xp + yp*yp + zp*zp)
                  rp = max(rp,0.000001e0)                                   ;//                  rp = max(rp,0.000001d0)
                  dot = xab*xcb + yab*ycb + zab*zcb                         ;//                  dot = xab*xcb + yab*ycb + zab*zcb
                  cosine = dot / sqrt(rab2*rcb2)                            ;//                  cosine = dot / sqrt(rab2*rcb2)
                  cosine = min(1.0e0,max(-1.0e0,cosine))                    ;//                  cosine = min(1.0d0,max(-1.0d0,cosine))
                  angle = radian * acos(cosine)                             ;//                  angle = radian * acos(cosine)
///                                                                         ;//c
///     get the energy and master chain rule term for derivatives           ;//c     get the energy and master chain rule term for derivatives
///                                                                         ;//c
                  if (angtyp[i]  ==  "HARMONIC") {                          ;//                  if (angtyp(i) .eq. 'HARMONIC') then
                     dt = angle - ideal                                     ;//                     dt = angle - ideal
                     dt2 = dt * dt                                          ;//                     dt2 = dt * dt
                     dt3 = dt2 * dt                                         ;//                     dt3 = dt2 * dt
                     dt4 = dt2 * dt2                                        ;//                     dt4 = dt2 * dt2
                     e = angunit * force * dt2                               //                     e = angunit * force * dt2
                            * (1.0e0+cang*dt+qang*dt2+pang*dt3+sang*dt4)    ;//     &                      * (1.0d0+cang*dt+qang*dt2+pang*dt3+sang*dt4)
                     deddt = angunit * force * dt * radian                   //                     deddt = angunit * force * dt * radian
                               * (2.0e0 + 3.0e0*cang*dt + 4.0e0*qang*dt2     //     &                         * (2.0d0 + 3.0d0*cang*dt + 4.0d0*qang*dt2
                                    + 5.0e0*pang*dt3 + 6.0e0*sang*dt4)      ;//     &                              + 5.0d0*pang*dt3 + 6.0d0*sang*dt4)
                  } else if (angtyp[i]  ==  "LINEAR") {                     ;//                  else if (angtyp(i) .eq. 'LINEAR') then
                     factor = 2.0e0 * angunit * pow(radian,2)               ;//                     factor = 2.0d0 * angunit * radian**2
                     sine = sqrt(1.0e0-cosine*cosine)                       ;//                     sine = sqrt(1.0d0-cosine*cosine)
                     e = factor * force * (1.0e0+cosine)                    ;//                     e = factor * force * (1.0d0+cosine)
                     deddt = -factor * force * sine                         ;//                     deddt = -factor * force * sine
                  } else if (angtyp[i]  ==  "FOURIER") {                    ;//                  else if (angtyp(i) .eq. 'FOURIER') then
                     fold = afld[i]                                         ;//                     fold = afld(i)
                     factor = 2.0e0 * angunit * pow((radian/fold),2)        ;//                     factor = 2.0d0 * angunit * (radian/fold)**2
                     cosine = cos((fold*angle-ideal)/radian)                ;//                     cosine = cos((fold*angle-ideal)/radian)
                     sine = sin((fold*angle-ideal)/radian)                  ;//                     sine = sin((fold*angle-ideal)/radian)
                     e = factor * force * (1.0e0+cosine)                    ;//                     e = factor * force * (1.0d0+cosine)
                     deddt = -factor * force * fold * sine                  ;//                     deddt = -factor * force * fold * sine
                  }                                                         ;//                  end if
///                                                                         ;//c
///     scale the interaction based on its group membership                 ;//c     scale the interaction based on its group membership
///                                                                         ;//c
                  if (use_group) {                                          ;//                  if (use_group) then
                     e = e * fgrp                                           ;//                     e = e * fgrp
                     deddt = deddt * fgrp                                   ;//                     deddt = deddt * fgrp
                  }                                                         ;//                  end if
///                                                                         ;//c
///     compute derivative components for this interaction                  ;//c     compute derivative components for this interaction
///                                                                         ;//c
                  terma = -deddt / (rab2*rp)                                ;//                  terma = -deddt / (rab2*rp)
                  termc = deddt / (rcb2*rp)                                 ;//                  termc = deddt / (rcb2*rp)
                  dedxia = terma * (yab*zp-zab*yp)                          ;//                  dedxia = terma * (yab*zp-zab*yp)
                  dedyia = terma * (zab*xp-xab*zp)                          ;//                  dedyia = terma * (zab*xp-xab*zp)
                  dedzia = terma * (xab*yp-yab*xp)                          ;//                  dedzia = terma * (xab*yp-yab*xp)
                  dedxic = termc * (ycb*zp-zcb*yp)                          ;//                  dedxic = termc * (ycb*zp-zcb*yp)
                  dedyic = termc * (zcb*xp-xcb*zp)                          ;//                  dedyic = termc * (zcb*xp-xcb*zp)
                  dedzic = termc * (xcb*yp-ycb*xp)                          ;//                  dedzic = termc * (xcb*yp-ycb*xp)
                  dedxib = -dedxia - dedxic                                 ;//                  dedxib = -dedxia - dedxic
                  dedyib = -dedyia - dedyic                                 ;//                  dedyib = -dedyia - dedyic
                  dedzib = -dedzia - dedzic                                 ;//                  dedzib = -dedzia - dedzic
///                                                                         ;//c
///     increment the total bond angle energy and derivatives               ;//c     increment the total bond angle energy and derivatives
///                                                                         ;//c
                  ea = ea + e                                               ;//                  ea = ea + e
                  dea[1,ia] = dea[1,ia] + dedxia                            ;//                  dea(1,ia) = dea(1,ia) + dedxia
                  dea[2,ia] = dea[2,ia] + dedyia                            ;//                  dea(2,ia) = dea(2,ia) + dedyia
                  dea[3,ia] = dea[3,ia] + dedzia                            ;//                  dea(3,ia) = dea(3,ia) + dedzia
                  dea[1,ib] = dea[1,ib] + dedxib                            ;//                  dea(1,ib) = dea(1,ib) + dedxib
                  dea[2,ib] = dea[2,ib] + dedyib                            ;//                  dea(2,ib) = dea(2,ib) + dedyib
                  dea[3,ib] = dea[3,ib] + dedzib                            ;//                  dea(3,ib) = dea(3,ib) + dedzib
                  dea[1,ic] = dea[1,ic] + dedxic                            ;//                  dea(1,ic) = dea(1,ic) + dedxic
                  dea[2,ic] = dea[2,ic] + dedyic                            ;//                  dea(2,ic) = dea(2,ic) + dedyic
                  dea[3,ic] = dea[3,ic] + dedzic                            ;//                  dea(3,ic) = dea(3,ic) + dedzic
///                                                                         ;//c
///     increment the internal virial tensor components                     ;//c     increment the internal virial tensor components
///                                                                         ;//c
                  vxx = xab*dedxia + xcb*dedxic                             ;//                  vxx = xab*dedxia + xcb*dedxic
                  vyx = yab*dedxia + ycb*dedxic                             ;//                  vyx = yab*dedxia + ycb*dedxic
                  vzx = zab*dedxia + zcb*dedxic                             ;//                  vzx = zab*dedxia + zcb*dedxic
                  vyy = yab*dedyia + ycb*dedyic                             ;//                  vyy = yab*dedyia + ycb*dedyic
                  vzy = zab*dedyia + zcb*dedyic                             ;//                  vzy = zab*dedyia + zcb*dedyic
                  vzz = zab*dedzia + zcb*dedzic                             ;//                  vzz = zab*dedzia + zcb*dedzic
                  vir[1,1] = vir[1,1] + vxx                                 ;//                  vir(1,1) = vir(1,1) + vxx
                  vir[2,1] = vir[2,1] + vyx                                 ;//                  vir(2,1) = vir(2,1) + vyx
                  vir[3,1] = vir[3,1] + vzx                                 ;//                  vir(3,1) = vir(3,1) + vzx
                  vir[1,2] = vir[1,2] + vyx                                 ;//                  vir(1,2) = vir(1,2) + vyx
                  vir[2,2] = vir[2,2] + vyy                                 ;//                  vir(2,2) = vir(2,2) + vyy
                  vir[3,2] = vir[3,2] + vzy                                 ;//                  vir(3,2) = vir(3,2) + vzy
                  vir[1,3] = vir[1,3] + vzx                                 ;//                  vir(1,3) = vir(1,3) + vzx
                  vir[2,3] = vir[2,3] + vzy                                 ;//                  vir(2,3) = vir(2,3) + vzy
                  vir[3,3] = vir[3,3] + vzz                                 ;//                  vir(3,3) = vir(3,3) + vzz
               }                                                            ;//               end if
///                                                                         ;//c
///     compute the projected in-plane angle energy and gradient            ;//c     compute the projected in-plane angle energy and gradient
///                                                                         ;//c
            } else {                                                        ;//            else
               xid = x[id]                                                  ;//               xid = x(id)
               yid = y[id]                                                  ;//               yid = y(id)
               zid = z[id]                                                  ;//               zid = z(id)
               xad = xia - xid                                              ;//               xad = xia - xid
               yad = yia - yid                                              ;//               yad = yia - yid
               zad = zia - zid                                              ;//               zad = zia - zid
               xbd = xib - xid                                              ;//               xbd = xib - xid
               ybd = yib - yid                                              ;//               ybd = yib - yid
               zbd = zib - zid                                              ;//               zbd = zib - zid
               xcd = xic - xid                                              ;//               xcd = xic - xid
               ycd = yic - yid                                              ;//               ycd = yic - yid
               zcd = zic - zid                                              ;//               zcd = zic - zid
               if (use_polymer) {                                           ;//               if (use_polymer) then
                  image(ref xad,ref yad,ref zad)                            ;//                  call image (xad,yad,zad)
                  image(ref xbd,ref ybd,ref zbd)                            ;//                  call image (xbd,ybd,zbd)
                  image(ref xcd,ref ycd,ref zcd)                            ;//                  call image (xcd,ycd,zcd)
               }                                                            ;//               end if
               xt = yad*zcd - zad*ycd                                       ;//               xt = yad*zcd - zad*ycd
               yt = zad*xcd - xad*zcd                                       ;//               yt = zad*xcd - xad*zcd
               zt = xad*ycd - yad*xcd                                       ;//               zt = xad*ycd - yad*xcd
               rt2 = xt*xt + yt*yt + zt*zt                                  ;//               rt2 = xt*xt + yt*yt + zt*zt
               delta = -(xt*xbd + yt*ybd + zt*zbd) / rt2                    ;//               delta = -(xt*xbd + yt*ybd + zt*zbd) / rt2
               xip = xib + xt*delta                                         ;//               xip = xib + xt*delta
               yip = yib + yt*delta                                         ;//               yip = yib + yt*delta
               zip = zib + zt*delta                                         ;//               zip = zib + zt*delta
               xap = xia - xip                                              ;//               xap = xia - xip
               yap = yia - yip                                              ;//               yap = yia - yip
               zap = zia - zip                                              ;//               zap = zia - zip
               xcp = xic - xip                                              ;//               xcp = xic - xip
               ycp = yic - yip                                              ;//               ycp = yic - yip
               zcp = zic - zip                                              ;//               zcp = zic - zip
               if (use_polymer) {                                           ;//               if (use_polymer) then
                  image(ref xap,ref yap,ref zap)                            ;//                  call image (xap,yap,zap)
                  image(ref xcp,ref ycp,ref zcp)                            ;//                  call image (xcp,ycp,zcp)
               }                                                            ;//               end if
               rap2 = xap*xap + yap*yap + zap*zap                           ;//               rap2 = xap*xap + yap*yap + zap*zap
               rcp2 = xcp*xcp + ycp*ycp + zcp*zcp                           ;//               rcp2 = xcp*xcp + ycp*ycp + zcp*zcp
               if (rap2 != 0.0e0  &&   rcp2 != 0.0e0) {                     ;//               if (rap2.ne.0.0d0 .and. rcp2.ne.0.0d0) then
                  xm = ycp*zap - zcp*yap                                    ;//                  xm = ycp*zap - zcp*yap
                  ym = zcp*xap - xcp*zap                                    ;//                  ym = zcp*xap - xcp*zap
                  zm = xcp*yap - ycp*xap                                    ;//                  zm = xcp*yap - ycp*xap
                  rm = sqrt(xm*xm + ym*ym + zm*zm)                          ;//                  rm = sqrt(xm*xm + ym*ym + zm*zm)
                  rm = max(rm,0.000001e0)                                   ;//                  rm = max(rm,0.000001d0)
                  dot = xap*xcp + yap*ycp + zap*zcp                         ;//                  dot = xap*xcp + yap*ycp + zap*zcp
                  cosine = dot / sqrt(rap2*rcp2)                            ;//                  cosine = dot / sqrt(rap2*rcp2)
                  cosine = min(1.0e0,max(-1.0e0,cosine))                    ;//                  cosine = min(1.0d0,max(-1.0d0,cosine))
                  angle = radian * acos(cosine)                             ;//                  angle = radian * acos(cosine)
///                                                                         ;//c
///     get the energy and master chain rule term for derivatives           ;//c     get the energy and master chain rule term for derivatives
///                                                                         ;//c
                  dt = angle - ideal                                        ;//                  dt = angle - ideal
                  dt2 = dt * dt                                             ;//                  dt2 = dt * dt
                  dt3 = dt2 * dt                                            ;//                  dt3 = dt2 * dt
                  dt4 = dt2 * dt2                                           ;//                  dt4 = dt2 * dt2
                  e = angunit * force * dt2                                  //                  e = angunit * force * dt2
                         * (1.0e0+cang*dt+qang*dt2+pang*dt3+sang*dt4)       ;//     &                   * (1.0d0+cang*dt+qang*dt2+pang*dt3+sang*dt4)
                  deddt = angunit * force * dt * radian                      //                  deddt = angunit * force * dt * radian
                            * (2.0e0 + 3.0e0*cang*dt + 4.0e0*qang*dt2        //     &                      * (2.0d0 + 3.0d0*cang*dt + 4.0d0*qang*dt2
                                 + 5.0e0*pang*dt3 + 6.0e0*sang*dt4)         ;//     &                           + 5.0d0*pang*dt3 + 6.0d0*sang*dt4)
///                                                                         ;//c
///     scale the interaction based on its group membership                 ;//c     scale the interaction based on its group membership
///                                                                         ;//c
                  if (use_group) {                                          ;//                  if (use_group) then
                     e = e * fgrp                                           ;//                     e = e * fgrp
                     deddt = deddt * fgrp                                   ;//                     deddt = deddt * fgrp
                  }                                                         ;//                  end if
///                                                                         ;//c
///     chain rule terms for first derivative components                    ;//c     chain rule terms for first derivative components
///                                                                         ;//c
                  terma = -deddt / (rap2*rm)                                ;//                  terma = -deddt / (rap2*rm)
                  termc = deddt / (rcp2*rm)                                 ;//                  termc = deddt / (rcp2*rm)
                  dedxia = terma * (yap*zm-zap*ym)                          ;//                  dedxia = terma * (yap*zm-zap*ym)
                  dedyia = terma * (zap*xm-xap*zm)                          ;//                  dedyia = terma * (zap*xm-xap*zm)
                  dedzia = terma * (xap*ym-yap*xm)                          ;//                  dedzia = terma * (xap*ym-yap*xm)
                  dedxic = termc * (ycp*zm-zcp*ym)                          ;//                  dedxic = termc * (ycp*zm-zcp*ym)
                  dedyic = termc * (zcp*xm-xcp*zm)                          ;//                  dedyic = termc * (zcp*xm-xcp*zm)
                  dedzic = termc * (xcp*ym-ycp*xm)                          ;//                  dedzic = termc * (xcp*ym-ycp*xm)
                  dedxip = -dedxia - dedxic                                 ;//                  dedxip = -dedxia - dedxic
                  dedyip = -dedyia - dedyic                                 ;//                  dedyip = -dedyia - dedyic
                  dedzip = -dedzia - dedzic                                 ;//                  dedzip = -dedzia - dedzic
///                                                                         ;//c
///     chain rule components for the projection of the central atom        ;//c     chain rule components for the projection of the central atom
///                                                                         ;//c
                  delta2 = 2.0e0 * delta                                    ;//                  delta2 = 2.0d0 * delta
                  ptrt2 = (dedxip*xt + dedyip*yt + dedzip*zt) / rt2         ;//                  ptrt2 = (dedxip*xt + dedyip*yt + dedzip*zt) / rt2
                  term = (zcd*ybd-ycd*zbd) + delta2*(yt*zcd-zt*ycd)         ;//                  term = (zcd*ybd-ycd*zbd) + delta2*(yt*zcd-zt*ycd)
                  dpdxia = delta*(ycd*dedzip-zcd*dedyip) + term*ptrt2       ;//                  dpdxia = delta*(ycd*dedzip-zcd*dedyip) + term*ptrt2
                  term = (xcd*zbd-zcd*xbd) + delta2*(zt*xcd-xt*zcd)         ;//                  term = (xcd*zbd-zcd*xbd) + delta2*(zt*xcd-xt*zcd)
                  dpdyia = delta*(zcd*dedxip-xcd*dedzip) + term*ptrt2       ;//                  dpdyia = delta*(zcd*dedxip-xcd*dedzip) + term*ptrt2
                  term = (ycd*xbd-xcd*ybd) + delta2*(xt*ycd-yt*xcd)         ;//                  term = (ycd*xbd-xcd*ybd) + delta2*(xt*ycd-yt*xcd)
                  dpdzia = delta*(xcd*dedyip-ycd*dedxip) + term*ptrt2       ;//                  dpdzia = delta*(xcd*dedyip-ycd*dedxip) + term*ptrt2
                  term = (yad*zbd-zad*ybd) + delta2*(zt*yad-yt*zad)         ;//                  term = (yad*zbd-zad*ybd) + delta2*(zt*yad-yt*zad)
                  dpdxic = delta*(zad*dedyip-yad*dedzip) + term*ptrt2       ;//                  dpdxic = delta*(zad*dedyip-yad*dedzip) + term*ptrt2
                  term = (zad*xbd-xad*zbd) + delta2*(xt*zad-zt*xad)         ;//                  term = (zad*xbd-xad*zbd) + delta2*(xt*zad-zt*xad)
                  dpdyic = delta*(xad*dedzip-zad*dedxip) + term*ptrt2       ;//                  dpdyic = delta*(xad*dedzip-zad*dedxip) + term*ptrt2
                  term = (xad*ybd-yad*xbd) + delta2*(yt*xad-xt*yad)         ;//                  term = (xad*ybd-yad*xbd) + delta2*(yt*xad-xt*yad)
                  dpdzic = delta*(yad*dedxip-xad*dedyip) + term*ptrt2       ;//                  dpdzic = delta*(yad*dedxip-xad*dedyip) + term*ptrt2
///                                                                         ;//c
///     compute derivative components for this interaction                  ;//c     compute derivative components for this interaction
///                                                                         ;//c
                  dedxia = dedxia + dpdxia                                  ;//                  dedxia = dedxia + dpdxia
                  dedyia = dedyia + dpdyia                                  ;//                  dedyia = dedyia + dpdyia
                  dedzia = dedzia + dpdzia                                  ;//                  dedzia = dedzia + dpdzia
                  dedxib = dedxip                                           ;//                  dedxib = dedxip
                  dedyib = dedyip                                           ;//                  dedyib = dedyip
                  dedzib = dedzip                                           ;//                  dedzib = dedzip
                  dedxic = dedxic + dpdxic                                  ;//                  dedxic = dedxic + dpdxic
                  dedyic = dedyic + dpdyic                                  ;//                  dedyic = dedyic + dpdyic
                  dedzic = dedzic + dpdzic                                  ;//                  dedzic = dedzic + dpdzic
                  dedxid = -dedxia - dedxib - dedxic                        ;//                  dedxid = -dedxia - dedxib - dedxic
                  dedyid = -dedyia - dedyib - dedyic                        ;//                  dedyid = -dedyia - dedyib - dedyic
                  dedzid = -dedzia - dedzib - dedzic                        ;//                  dedzid = -dedzia - dedzib - dedzic
///                                                                         ;//c
///     increment the total bond angle energy and derivatives               ;//c     increment the total bond angle energy and derivatives
///                                                                         ;//c
                  ea = ea + e                                               ;//                  ea = ea + e
                  dea[1,ia] = dea[1,ia] + dedxia                            ;//                  dea(1,ia) = dea(1,ia) + dedxia
                  dea[2,ia] = dea[2,ia] + dedyia                            ;//                  dea(2,ia) = dea(2,ia) + dedyia
                  dea[3,ia] = dea[3,ia] + dedzia                            ;//                  dea(3,ia) = dea(3,ia) + dedzia
                  dea[1,ib] = dea[1,ib] + dedxib                            ;//                  dea(1,ib) = dea(1,ib) + dedxib
                  dea[2,ib] = dea[2,ib] + dedyib                            ;//                  dea(2,ib) = dea(2,ib) + dedyib
                  dea[3,ib] = dea[3,ib] + dedzib                            ;//                  dea(3,ib) = dea(3,ib) + dedzib
                  dea[1,ic] = dea[1,ic] + dedxic                            ;//                  dea(1,ic) = dea(1,ic) + dedxic
                  dea[2,ic] = dea[2,ic] + dedyic                            ;//                  dea(2,ic) = dea(2,ic) + dedyic
                  dea[3,ic] = dea[3,ic] + dedzic                            ;//                  dea(3,ic) = dea(3,ic) + dedzic
                  dea[1,id] = dea[1,id] + dedxid                            ;//                  dea(1,id) = dea(1,id) + dedxid
                  dea[2,id] = dea[2,id] + dedyid                            ;//                  dea(2,id) = dea(2,id) + dedyid
                  dea[3,id] = dea[3,id] + dedzid                            ;//                  dea(3,id) = dea(3,id) + dedzid
///                                                                         ;//c
///     increment the internal virial tensor components                     ;//c     increment the internal virial tensor components
///                                                                         ;//c
                  vxx = xad*dedxia + xbd*dedxib + xcd*dedxic                ;//                  vxx = xad*dedxia + xbd*dedxib + xcd*dedxic
                  vyx = yad*dedxia + ybd*dedxib + ycd*dedxic                ;//                  vyx = yad*dedxia + ybd*dedxib + ycd*dedxic
                  vzx = zad*dedxia + zbd*dedxib + zcd*dedxic                ;//                  vzx = zad*dedxia + zbd*dedxib + zcd*dedxic
                  vyy = yad*dedyia + ybd*dedyib + ycd*dedyic                ;//                  vyy = yad*dedyia + ybd*dedyib + ycd*dedyic
                  vzy = zad*dedyia + zbd*dedyib + zcd*dedyic                ;//                  vzy = zad*dedyia + zbd*dedyib + zcd*dedyic
                  vzz = zad*dedzia + zbd*dedzib + zcd*dedzic                ;//                  vzz = zad*dedzia + zbd*dedzib + zcd*dedzic
                  vir[1,1] = vir[1,1] + vxx                                 ;//                  vir(1,1) = vir(1,1) + vxx
                  vir[2,1] = vir[2,1] + vyx                                 ;//                  vir(2,1) = vir(2,1) + vyx
                  vir[3,1] = vir[3,1] + vzx                                 ;//                  vir(3,1) = vir(3,1) + vzx
                  vir[1,2] = vir[1,2] + vyx                                 ;//                  vir(1,2) = vir(1,2) + vyx
                  vir[2,2] = vir[2,2] + vyy                                 ;//                  vir(2,2) = vir(2,2) + vyy
                  vir[3,2] = vir[3,2] + vzy                                 ;//                  vir(3,2) = vir(3,2) + vzy
                  vir[1,3] = vir[1,3] + vzx                                 ;//                  vir(1,3) = vir(1,3) + vzx
                  vir[2,3] = vir[2,3] + vzy                                 ;//                  vir(2,3) = vir(2,3) + vzy
                  vir[3,3] = vir[3,3] + vzz                                 ;//                  vir(3,3) = vir(3,3) + vzz
               }                                                            ;//               end if
            }                                                               ;//            end if
         }                                                                  ;//         end if
      }                                                                     ;//      end do
      return                                                                ;//      return
    }                                                                        //      end
}
}
}
