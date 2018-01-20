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
///     ###############################################################     ;//c     ###############################################################
///     ##                                                           ##     ;//c     ##                                                           ##
///     ##  subroutine eangle2  --  atom-by-atom angle bend Hessian  ##     ;//c     ##  subroutine eangle2  --  atom-by-atom angle bend Hessian  ##
///     ##                                                           ##     ;//c     ##                                                           ##
///     ###############################################################     ;//c     ###############################################################
///                                                                         ;//c
///                                                                         ;//c
///     "eangle2" calculates second derivatives of the angle bending        ;//c     "eangle2" calculates second derivatives of the angle bending
///     energy for a single atom using a mixture of analytical and          ;//c     energy for a single atom using a mixture of analytical and
///     finite difference methods; projected in-plane angles at trigonal    ;//c     finite difference methods; projected in-plane angles at trigonal
///     centers, special linear or Fourier angle bending terms are          ;//c     centers, special linear or Fourier angle bending terms are
///     optionally used                                                     ;//c     optionally used
///                                                                         ;//c
///                                                                         ;//c
    public void eangle2(int i) {                                            ;//      subroutine eangle2 (i)
                                                                            ;//      implicit none
                                                                            ;//      include 'sizes.i'
                                                                            ;//      include 'angle.i'
                                                                            ;//      include 'angpot.i'
                                                                            ;//      include 'atoms.i'
                                                                            ;//      include 'deriv.i'
                                                                            ;//      include 'group.i'
                                                                            ;//      include 'hessn.i'
      int j,k                                                               ;//      integer i,j,k
      int ia,ib,ic,id                                                       ;//      integer ia,ib,ic,id
      double eps,fgrp                                                       ;//      real*8 eps,fgrp
      double old,term                                                       ;//      real*8 old,term
      double[,] d0                                                          ;//      real*8, allocatable :: d0(:,:)
      bool proceed                                                          ;//      logical proceed
      bool twosided                                                         ;//      logical twosided
      ia = ib = ic = id = -1;
///                                                                         ;//c
///                                                                         ;//c
///     compute analytical angle bending Hessian elements                   ;//c     compute analytical angle bending Hessian elements
///                                                                         ;//c
      eangle2a(i)                                                           ;//      call eangle2a (i)
///                                                                         ;//c
///     set stepsize for derivatives and default group weight               ;//c     set stepsize for derivatives and default group weight
///                                                                         ;//c
      eps = 1.0e-5                                                          ;//      eps = 1.0d-5
      fgrp = 1.0e0                                                          ;//      fgrp = 1.0d0
      twosided =  false                                                     ;//      twosided = .false.
      if (n  <=  50)  twosided =  true                                      ;//      if (n .le. 50)  twosided = .true.
///                                                                         ;//c
///     perform dynamic allocation of some local arrays                     ;//c     perform dynamic allocation of some local arrays
///                                                                         ;//c
      d0 = new double[1+3,1+n]                                              ;//      allocate (d0(3,n))
///                                                                         ;//c
///     compute numerical in-plane bend Hessian for current atom            ;//c     compute numerical in-plane bend Hessian for current atom
///                                                                         ;//c
      for(k=1; k<=nangle; k++) {                                            ;//      do k = 1, nangle
         proceed =  false                                                   ;//         proceed = .false.
         if (angtyp[k]  ==  "IN-PLANE") {                                   ;//         if (angtyp(k) .eq. 'IN-PLANE') then
            ia = iang[1,k]                                                  ;//            ia = iang(1,k)
            ib = iang[2,k]                                                  ;//            ib = iang(2,k)
            ic = iang[3,k]                                                  ;//            ic = iang(3,k)
            id = iang[4,k]                                                  ;//            id = iang(4,k)
            proceed = (i == ia  ||  i == ib  ||  i == ic  ||  i == id)      ;//            proceed = (i.eq.ia .or. i.eq.ib .or. i.eq.ic .or. i.eq.id)
            if (proceed  &&   use_group)                                    ;//            if (proceed .and. use_group)
               groups(out proceed,out fgrp,ia,ib,ic,id,0,0)                 ;//     &         call groups (proceed,fgrp,ia,ib,ic,id,0,0)
         }                                                                  ;//         end if
         if (proceed) {                                                     ;//         if (proceed) then
            term = fgrp / eps                                               ;//            term = fgrp / eps
///                                                                         ;//c
///     find first derivatives for the base structure                       ;//c     find first derivatives for the base structure
///                                                                         ;//c
            if ( !    twosided) {                                           ;//            if (.not. twosided) then
               eangle2b(k)                                                  ;//               call eangle2b (k)
               for(j=1; j<=3; j++) {                                        ;//               do j = 1, 3
                  d0[j,ia] = dea[j,ia]                                      ;//                  d0(j,ia) = dea(j,ia)
                  d0[j,ib] = dea[j,ib]                                      ;//                  d0(j,ib) = dea(j,ib)
                  d0[j,ic] = dea[j,ic]                                      ;//                  d0(j,ic) = dea(j,ic)
                  d0[j,id] = dea[j,id]                                      ;//                  d0(j,id) = dea(j,id)
               }                                                            ;//               end do
            }                                                               ;//            end if
///                                                                         ;//c
///     find numerical x-components via perturbed structures                ;//c     find numerical x-components via perturbed structures
///                                                                         ;//c
            old = x[i]                                                      ;//            old = x(i)
            if (twosided) {                                                 ;//            if (twosided) then
               x[i] = x[i] - 0.5e0*eps                                      ;//               x(i) = x(i) - 0.5d0*eps
               eangle2b(k)                                                  ;//               call eangle2b (k)
               for(j=1; j<=3; j++) {                                        ;//               do j = 1, 3
                  d0[j,ia] = dea[j,ia]                                      ;//                  d0(j,ia) = dea(j,ia)
                  d0[j,ib] = dea[j,ib]                                      ;//                  d0(j,ib) = dea(j,ib)
                  d0[j,ic] = dea[j,ic]                                      ;//                  d0(j,ic) = dea(j,ic)
                  d0[j,id] = dea[j,id]                                      ;//                  d0(j,id) = dea(j,id)
               }                                                            ;//               end do
            }                                                               ;//            end if
            x[i] = x[i] + eps                                               ;//            x(i) = x(i) + eps
            eangle2b(k)                                                     ;//            call eangle2b (k)
            x[i] = old                                                      ;//            x(i) = old
            for(j=1; j<=3; j++) {                                           ;//            do j = 1, 3
               hessx[j,ia] = hessx[j,ia] + term*(dea[j,ia]-d0[j,ia])        ;//               hessx(j,ia) = hessx(j,ia) + term*(dea(j,ia)-d0(j,ia))
               hessx[j,ib] = hessx[j,ib] + term*(dea[j,ib]-d0[j,ib])        ;//               hessx(j,ib) = hessx(j,ib) + term*(dea(j,ib)-d0(j,ib))
               hessx[j,ic] = hessx[j,ic] + term*(dea[j,ic]-d0[j,ic])        ;//               hessx(j,ic) = hessx(j,ic) + term*(dea(j,ic)-d0(j,ic))
               hessx[j,id] = hessx[j,id] + term*(dea[j,id]-d0[j,id])        ;//               hessx(j,id) = hessx(j,id) + term*(dea(j,id)-d0(j,id))
            }                                                               ;//            end do
///                                                                         ;//c
///     find numerical y-components via perturbed structures                ;//c     find numerical y-components via perturbed structures
///                                                                         ;//c
            old = y[i]                                                      ;//            old = y(i)
            if (twosided) {                                                 ;//            if (twosided) then
               y[i] = y[i] - 0.5e0*eps                                      ;//               y(i) = y(i) - 0.5d0*eps
               eangle2b(k)                                                  ;//               call eangle2b (k)
               for(j=1; j<=3; j++) {                                        ;//               do j = 1, 3
                  d0[j,ia] = dea[j,ia]                                      ;//                  d0(j,ia) = dea(j,ia)
                  d0[j,ib] = dea[j,ib]                                      ;//                  d0(j,ib) = dea(j,ib)
                  d0[j,ic] = dea[j,ic]                                      ;//                  d0(j,ic) = dea(j,ic)
                  d0[j,id] = dea[j,id]                                      ;//                  d0(j,id) = dea(j,id)
               }                                                            ;//               end do
            }                                                               ;//            end if
            y[i] = y[i] + eps                                               ;//            y(i) = y(i) + eps
            eangle2b(k)                                                     ;//            call eangle2b (k)
            y[i] = old                                                      ;//            y(i) = old
            for(j=1; j<=3; j++) {                                           ;//            do j = 1, 3
               hessy[j,ia] = hessy[j,ia] + term*(dea[j,ia]-d0[j,ia])        ;//               hessy(j,ia) = hessy(j,ia) + term*(dea(j,ia)-d0(j,ia))
               hessy[j,ib] = hessy[j,ib] + term*(dea[j,ib]-d0[j,ib])        ;//               hessy(j,ib) = hessy(j,ib) + term*(dea(j,ib)-d0(j,ib))
               hessy[j,ic] = hessy[j,ic] + term*(dea[j,ic]-d0[j,ic])        ;//               hessy(j,ic) = hessy(j,ic) + term*(dea(j,ic)-d0(j,ic))
               hessy[j,id] = hessy[j,id] + term*(dea[j,id]-d0[j,id])        ;//               hessy(j,id) = hessy(j,id) + term*(dea(j,id)-d0(j,id))
            }                                                               ;//            end do
///                                                                         ;//c
///     find numerical z-components via perturbed structures                ;//c     find numerical z-components via perturbed structures
///                                                                         ;//c
            old = z[i]                                                      ;//            old = z(i)
            if (twosided) {                                                 ;//            if (twosided) then
               z[i] = z[i] - 0.5e0*eps                                      ;//               z(i) = z(i) - 0.5d0*eps
               eangle2b(k)                                                  ;//               call eangle2b (k)
               for(j=1; j<=3; j++) {                                        ;//               do j = 1, 3
                  d0[j,ia] = dea[j,ia]                                      ;//                  d0(j,ia) = dea(j,ia)
                  d0[j,ib] = dea[j,ib]                                      ;//                  d0(j,ib) = dea(j,ib)
                  d0[j,ic] = dea[j,ic]                                      ;//                  d0(j,ic) = dea(j,ic)
                  d0[j,id] = dea[j,id]                                      ;//                  d0(j,id) = dea(j,id)
               }                                                            ;//               end do
            }                                                               ;//            end if
            z[i] = z[i] + eps                                               ;//            z(i) = z(i) + eps
            eangle2b(k)                                                     ;//            call eangle2b (k)
            z[i] = old                                                      ;//            z(i) = old
            for(j=1; j<=3; j++) {                                           ;//            do j = 1, 3
               hessz[j,ia] = hessz[j,ia] + term*(dea[j,ia]-d0[j,ia])        ;//               hessz(j,ia) = hessz(j,ia) + term*(dea(j,ia)-d0(j,ia))
               hessz[j,ib] = hessz[j,ib] + term*(dea[j,ib]-d0[j,ib])        ;//               hessz(j,ib) = hessz(j,ib) + term*(dea(j,ib)-d0(j,ib))
               hessz[j,ic] = hessz[j,ic] + term*(dea[j,ic]-d0[j,ic])        ;//               hessz(j,ic) = hessz(j,ic) + term*(dea(j,ic)-d0(j,ic))
               hessz[j,id] = hessz[j,id] + term*(dea[j,id]-d0[j,id])        ;//               hessz(j,id) = hessz(j,id) + term*(dea(j,id)-d0(j,id))
            }                                                               ;//            end do
         }                                                                  ;//         end if
      }                                                                     ;//      end do
///                                                                         ;//c
///     perform deallocation of some local arrays                           ;//c     perform deallocation of some local arrays
///                                                                         ;//c
//    deallocate (d0)                                                       ;//      deallocate (d0)
      return                                                                ;//      return
    }                                                                        //      end
///                                                                         ;//c
///                                                                         ;//c
///     ##################################################################  ;//c     ##################################################################
///     ##                                                              ##  ;//c     ##                                                              ##
///     ##  subroutine eangle2a  --  angle bending Hessian; analytical  ##  ;//c     ##  subroutine eangle2a  --  angle bending Hessian; analytical  ##
///     ##                                                              ##  ;//c     ##                                                              ##
///     ##################################################################  ;//c     ##################################################################
///                                                                         ;//c
///                                                                         ;//c
///     "eangle2a" calculates bond angle bending potential energy           ;//c     "eangle2a" calculates bond angle bending potential energy
///     second derivatives with respect to Cartesian coordinates            ;//c     second derivatives with respect to Cartesian coordinates
///                                                                         ;//c
///                                                                         ;//c
    public void eangle2a(int iatom) {                                       ;//      subroutine eangle2a (iatom)
                                                                            ;//      implicit none
                                                                            ;//      include 'sizes.i'
                                                                            ;//      include 'angle.i'
                                                                            ;//      include 'angpot.i'
                                                                            ;//      include 'atoms.i'
                                                                            ;//      include 'bound.i'
                                                                            ;//      include 'group.i'
                                                                            ;//      include 'hessn.i'
                                                                            ;//      include 'math.i'
      int i                                                                 ;//      integer i,iatom
      int ia,ib,ic                                                          ;//      integer ia,ib,ic
      double ideal,force                                                    ;//      real*8 ideal,force
      double fold,factor,dot                                                ;//      real*8 fold,factor,dot
      double cosine,sine                                                    ;//      real*8 cosine,sine
      double angle,fgrp                                                     ;//      real*8 angle,fgrp
      double dt,dt2,dt3,dt4                                                 ;//      real*8 dt,dt2,dt3,dt4
      double xia,yia,zia                                                    ;//      real*8 xia,yia,zia
      double xib,yib,zib                                                    ;//      real*8 xib,yib,zib
      double xic,yic,zic                                                    ;//      real*8 xic,yic,zic
      double xab,yab,zab                                                    ;//      real*8 xab,yab,zab
      double xcb,ycb,zcb                                                    ;//      real*8 xcb,ycb,zcb
      double rab2,rcb2                                                      ;//      real*8 rab2,rcb2
      double xpo,ypo,zpo                                                    ;//      real*8 xpo,ypo,zpo
      double xp,yp,zp,rp,rp2                                                ;//      real*8 xp,yp,zp,rp,rp2
      double xrab,yrab,zrab                                                 ;//      real*8 xrab,yrab,zrab
      double xrcb,yrcb,zrcb                                                 ;//      real*8 xrcb,yrcb,zrcb
      double xabp,yabp,zabp                                                 ;//      real*8 xabp,yabp,zabp
      double xcbp,ycbp,zcbp                                                 ;//      real*8 xcbp,ycbp,zcbp
      double deddt,d2eddt2                                                  ;//      real*8 deddt,d2eddt2
      double terma,termc                                                    ;//      real*8 terma,termc
      double ddtdxia,ddtdyia,ddtdzia                                        ;//      real*8 ddtdxia,ddtdyia,ddtdzia
      double ddtdxib,ddtdyib,ddtdzib                                        ;//      real*8 ddtdxib,ddtdyib,ddtdzib
      double ddtdxic,ddtdyic,ddtdzic                                        ;//      real*8 ddtdxic,ddtdyic,ddtdzic
      double dxiaxia,dxiayia,dxiazia                                        ;//      real*8 dxiaxia,dxiayia,dxiazia
      double dxibxib,dxibyib,dxibzib                                        ;//      real*8 dxibxib,dxibyib,dxibzib
      double dxicxic,dxicyic,dxiczic                                        ;//      real*8 dxicxic,dxicyic,dxiczic
      double dyiayia,dyiazia,dziazia                                        ;//      real*8 dyiayia,dyiazia,dziazia
      double dyibyib,dyibzib,dzibzib                                        ;//      real*8 dyibyib,dyibzib,dzibzib
      double dyicyic,dyiczic,dziczic                                        ;//      real*8 dyicyic,dyiczic,dziczic
      double dxibxia,dxibyia,dxibzia                                        ;//      real*8 dxibxia,dxibyia,dxibzia
      double dyibxia,dyibyia,dyibzia                                        ;//      real*8 dyibxia,dyibyia,dyibzia
      double dzibxia,dzibyia,dzibzia                                        ;//      real*8 dzibxia,dzibyia,dzibzia
      double dxibxic,dxibyic,dxibzic                                        ;//      real*8 dxibxic,dxibyic,dxibzic
      double dyibxic,dyibyic,dyibzic                                        ;//      real*8 dyibxic,dyibyic,dyibzic
      double dzibxic,dzibyic,dzibzic                                        ;//      real*8 dzibxic,dzibyic,dzibzic
      double dxiaxic,dxiayic,dxiazic                                        ;//      real*8 dxiaxic,dxiayic,dxiazic
      double dyiaxic,dyiayic,dyiazic                                        ;//      real*8 dyiaxic,dyiayic,dyiazic
      double dziaxic,dziayic,dziazic                                        ;//      real*8 dziaxic,dziayic,dziazic
      bool proceed,linear                                                   ;//      logical proceed,linear
      d2eddt2 = fgrp = deddt = double.NaN;
///                                                                         ;//c
///                                                                         ;//c
///     calculate the bond angle bending energy term                        ;//c     calculate the bond angle bending energy term
///                                                                         ;//c
      for(i=1; i<=nangle; i++) {                                            ;//       do i = 1, nangle
         ia = iang[1,i]                                                     ;//          ia = iang(1,i)
         ib = iang[2,i]                                                     ;//          ib = iang(2,i)
         ic = iang[3,i]                                                     ;//          ic = iang(3,i)
         ideal = anat[i]                                                    ;//          ideal = anat(i)
         force = ak[i]                                                      ;//          force = ak(i)
///                                                                         ;//c
///     decide whether to compute the current interaction                   ;//c     decide whether to compute the current interaction
///                                                                         ;//c
         proceed = (iatom == ia  ||  iatom == ib  ||  iatom == ic)          ;//         proceed = (iatom.eq.ia .or. iatom.eq.ib .or. iatom.eq.ic)
         if (proceed  &&   use_group)                                        //         if (proceed .and. use_group)
            groups(out proceed,out fgrp,ia,ib,ic,0,0,0)                     ;//     &      call groups (proceed,fgrp,ia,ib,ic,0,0,0)
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
///     compute the bond angle bending Hessian elements                     ;//c     compute the bond angle bending Hessian elements
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
                  dot = xab*xcb + yab*ycb + zab*zcb                         ;//                  dot = xab*xcb + yab*ycb + zab*zcb
                  cosine = dot / sqrt(rab2*rcb2)                            ;//                  cosine = dot / sqrt(rab2*rcb2)
                  cosine = min(1.0e0,max(-1.0e0,cosine))                    ;//                  cosine = min(1.0d0,max(-1.0d0,cosine))
                  angle = radian * acos(cosine)                             ;//                  angle = radian * acos(cosine)
///                                                                         ;//c
///     get the master chain rule terms for derivatives                     ;//c     get the master chain rule terms for derivatives
///                                                                         ;//c
                  if (angtyp[i]  ==  "HARMONIC") {                          ;//                  if (angtyp(i) .eq. 'HARMONIC') then
                     dt = angle - ideal                                     ;//                     dt = angle - ideal
                     dt2 = dt * dt                                          ;//                     dt2 = dt * dt
                     dt3 = dt2 * dt                                         ;//                     dt3 = dt2 * dt
                     dt4 = dt3 * dt                                         ;//                     dt4 = dt3 * dt
                     deddt = angunit * force * dt * radian                   //                     deddt = angunit * force * dt * radian
                               * (2.0e0 + 3.0e0*cang*dt + 4.0e0*qang*dt2     //     &                         * (2.0d0 + 3.0d0*cang*dt + 4.0d0*qang*dt2
                                   + 5.0e0*pang*dt3 + 6.0e0*sang*dt4)       ;//     &                             + 5.0d0*pang*dt3 + 6.0d0*sang*dt4)
                     d2eddt2 = angunit * force * pow(radian,2)               //                     d2eddt2 = angunit * force * radian**2
                              * (2.0e0 + 6.0e0*cang*dt + 12.0e0*qang*dt2     //     &                        * (2.0d0 + 6.0d0*cang*dt + 12.0d0*qang*dt2
                                  + 20.0e0*pang*dt3 + 30.0e0*sang*dt4)      ;//     &                            + 20.0d0*pang*dt3 + 30.0d0*sang*dt4)
                  } else if (angtyp[i]  ==  "LINEAR") {                     ;//                  else if (angtyp(i) .eq. 'LINEAR') then
                     factor = 2.0e0 * angunit * pow(radian,2)               ;//                     factor = 2.0d0 * angunit * radian**2
                     sine = sqrt(1.0e0-cosine*cosine)                       ;//                     sine = sqrt(1.0d0-cosine*cosine)
                     deddt = -factor * force * sine                         ;//                     deddt = -factor * force * sine
                     d2eddt2 = -factor * force * cosine                     ;//                     d2eddt2 = -factor * force * cosine
                  } else if (angtyp[i]  ==  "FOURIER") {                    ;//                  else if (angtyp(i) .eq. 'FOURIER') then
                     fold = afld[i]                                         ;//                     fold = afld(i)
                     factor = 2.0e0 * angunit * (pow(radian,2)/fold)        ;//                     factor = 2.0d0 * angunit * (radian**2/fold)
                     cosine = cos((fold*angle-ideal)/radian)                ;//                     cosine = cos((fold*angle-ideal)/radian)
                     sine = sin((fold*angle-ideal)/radian)                  ;//                     sine = sin((fold*angle-ideal)/radian)
                     deddt = -factor * force * sine                         ;//                     deddt = -factor * force * sine
                     d2eddt2 = -factor * force * fold * cosine              ;//                     d2eddt2 = -factor * force * fold * cosine
                  }                                                         ;//                  end if
///                                                                         ;//c
///     scale the interaction based on its group membership                 ;//c     scale the interaction based on its group membership
///                                                                         ;//c
                  if (use_group) {                                          ;//                  if (use_group) then
                     deddt = deddt * fgrp                                   ;//                     deddt = deddt * fgrp
                     d2eddt2 = d2eddt2 * fgrp                               ;//                     d2eddt2 = d2eddt2 * fgrp
                  }                                                         ;//                  end if
///                                                                         ;//c
///     construct an orthogonal direction for linear angles                 ;//c     construct an orthogonal direction for linear angles
///                                                                         ;//c
                  linear =  false                                           ;//                  linear = .false.
                  if (rp  <   0.000001e0) {                                 ;//                  if (rp .lt. 0.000001d0) then
                     linear =  true                                         ;//                     linear = .true.
                     if (xab != 0.0e0  &&   yab != 0.0e0) {                 ;//                     if (xab.ne.0.0d0 .and. yab.ne.0.0d0) then
                        xp = -yab                                           ;//                        xp = -yab
                        yp = xab                                            ;//                        yp = xab
                        zp = 0.0e0                                          ;//                        zp = 0.0d0
                     } else if (xab == 0.0e0  &&   yab == 0.0e0) {          ;//                     else if (xab.eq.0.0d0 .and. yab.eq.0.0d0) then
                        xp = 1.0e0                                          ;//                        xp = 1.0d0
                        yp = 0.0e0                                          ;//                        yp = 0.0d0
                        zp = 0.0e0                                          ;//                        zp = 0.0d0
                     } else if (xab != 0.0e0  &&   yab == 0.0e0) {          ;//                     else if (xab.ne.0.0d0 .and. yab.eq.0.0d0) then
                        xp = 0.0e0                                          ;//                        xp = 0.0d0
                        yp = 1.0e0                                          ;//                        yp = 1.0d0
                        zp = 0.0e0                                          ;//                        zp = 0.0d0
                     } else if (xab == 0.0e0  &&   yab != 0.0e0) {          ;//                     else if (xab.eq.0.0d0 .and. yab.ne.0.0d0) then
                        xp = 1.0e0                                          ;//                        xp = 1.0d0
                        yp = 0.0e0                                          ;//                        yp = 0.0d0
                        zp = 0.0e0                                          ;//                        zp = 0.0d0
                     }                                                      ;//                     end if
                     rp = sqrt(xp*xp + yp*yp + zp*zp)                       ;//                     rp = sqrt(xp*xp + yp*yp + zp*zp)
                  }                                                         ;//                  end if
///                                                                         ;//c
///     first derivatives of bond angle with respect to coordinates         ;//c     first derivatives of bond angle with respect to coordinates
///                                                                         ;//c
   goto10:                                                                  ;//   10             continue
                  terma = -1.0e0 / (rab2*rp)                                ;//                  terma = -1.0d0 / (rab2*rp)
                  termc = 1.0e0 / (rcb2*rp)                                 ;//                  termc = 1.0d0 / (rcb2*rp)
                  ddtdxia = terma * (yab*zp-zab*yp)                         ;//                  ddtdxia = terma * (yab*zp-zab*yp)
                  ddtdyia = terma * (zab*xp-xab*zp)                         ;//                  ddtdyia = terma * (zab*xp-xab*zp)
                  ddtdzia = terma * (xab*yp-yab*xp)                         ;//                  ddtdzia = terma * (xab*yp-yab*xp)
                  ddtdxic = termc * (ycb*zp-zcb*yp)                         ;//                  ddtdxic = termc * (ycb*zp-zcb*yp)
                  ddtdyic = termc * (zcb*xp-xcb*zp)                         ;//                  ddtdyic = termc * (zcb*xp-xcb*zp)
                  ddtdzic = termc * (xcb*yp-ycb*xp)                         ;//                  ddtdzic = termc * (xcb*yp-ycb*xp)
                  ddtdxib = -ddtdxia - ddtdxic                              ;//                  ddtdxib = -ddtdxia - ddtdxic
                  ddtdyib = -ddtdyia - ddtdyic                              ;//                  ddtdyib = -ddtdyia - ddtdyic
                  ddtdzib = -ddtdzia - ddtdzic                              ;//                  ddtdzib = -ddtdzia - ddtdzic
///                                                                         ;//c
///     abbreviations used in defining chain rule terms                     ;//c     abbreviations used in defining chain rule terms
///                                                                         ;//c
                  xrab = 2.0e0 * xab / rab2                                 ;//                  xrab = 2.0d0 * xab / rab2
                  yrab = 2.0e0 * yab / rab2                                 ;//                  yrab = 2.0d0 * yab / rab2
                  zrab = 2.0e0 * zab / rab2                                 ;//                  zrab = 2.0d0 * zab / rab2
                  xrcb = 2.0e0 * xcb / rcb2                                 ;//                  xrcb = 2.0d0 * xcb / rcb2
                  yrcb = 2.0e0 * ycb / rcb2                                 ;//                  yrcb = 2.0d0 * ycb / rcb2
                  zrcb = 2.0e0 * zcb / rcb2                                 ;//                  zrcb = 2.0d0 * zcb / rcb2
                  rp2 = 1.0e0 / (rp*rp)                                     ;//                  rp2 = 1.0d0 / (rp*rp)
                  xabp = (yab*zp-zab*yp) * rp2                              ;//                  xabp = (yab*zp-zab*yp) * rp2
                  yabp = (zab*xp-xab*zp) * rp2                              ;//                  yabp = (zab*xp-xab*zp) * rp2
                  zabp = (xab*yp-yab*xp) * rp2                              ;//                  zabp = (xab*yp-yab*xp) * rp2
                  xcbp = (ycb*zp-zcb*yp) * rp2                              ;//                  xcbp = (ycb*zp-zcb*yp) * rp2
                  ycbp = (zcb*xp-xcb*zp) * rp2                              ;//                  ycbp = (zcb*xp-xcb*zp) * rp2
                  zcbp = (xcb*yp-ycb*xp) * rp2                              ;//                  zcbp = (xcb*yp-ycb*xp) * rp2
///                                                                         ;//c
///     chain rule terms for second derivative components                   ;//c     chain rule terms for second derivative components
///                                                                         ;//c
                  dxiaxia = terma*(xab*xcb-dot) + ddtdxia*(xcbp-xrab)       ;//                  dxiaxia = terma*(xab*xcb-dot) + ddtdxia*(xcbp-xrab)
                  dxiayia = terma*(zp+yab*xcb) + ddtdxia*(ycbp-yrab)        ;//                  dxiayia = terma*(zp+yab*xcb) + ddtdxia*(ycbp-yrab)
                  dxiazia = terma*(zab*xcb-yp) + ddtdxia*(zcbp-zrab)        ;//                  dxiazia = terma*(zab*xcb-yp) + ddtdxia*(zcbp-zrab)
                  dyiayia = terma*(yab*ycb-dot) + ddtdyia*(ycbp-yrab)       ;//                  dyiayia = terma*(yab*ycb-dot) + ddtdyia*(ycbp-yrab)
                  dyiazia = terma*(xp+zab*ycb) + ddtdyia*(zcbp-zrab)        ;//                  dyiazia = terma*(xp+zab*ycb) + ddtdyia*(zcbp-zrab)
                  dziazia = terma*(zab*zcb-dot) + ddtdzia*(zcbp-zrab)       ;//                  dziazia = terma*(zab*zcb-dot) + ddtdzia*(zcbp-zrab)
                  dxicxic = termc*(dot-xab*xcb) - ddtdxic*(xabp+xrcb)       ;//                  dxicxic = termc*(dot-xab*xcb) - ddtdxic*(xabp+xrcb)
                  dxicyic = termc*(zp-ycb*xab) - ddtdxic*(yabp+yrcb)        ;//                  dxicyic = termc*(zp-ycb*xab) - ddtdxic*(yabp+yrcb)
                  dxiczic = -termc*(yp+zcb*xab) - ddtdxic*(zabp+zrcb)       ;//                  dxiczic = -termc*(yp+zcb*xab) - ddtdxic*(zabp+zrcb)
                  dyicyic = termc*(dot-yab*ycb) - ddtdyic*(yabp+yrcb)       ;//                  dyicyic = termc*(dot-yab*ycb) - ddtdyic*(yabp+yrcb)
                  dyiczic = termc*(xp-zcb*yab) - ddtdyic*(zabp+zrcb)        ;//                  dyiczic = termc*(xp-zcb*yab) - ddtdyic*(zabp+zrcb)
                  dziczic = termc*(dot-zab*zcb) - ddtdzic*(zabp+zrcb)       ;//                  dziczic = termc*(dot-zab*zcb) - ddtdzic*(zabp+zrcb)
                  dxiaxic = terma*(yab*yab+zab*zab) - ddtdxia*xabp          ;//                  dxiaxic = terma*(yab*yab+zab*zab) - ddtdxia*xabp
                  dxiayic = -terma*xab*yab - ddtdxia*yabp                   ;//                  dxiayic = -terma*xab*yab - ddtdxia*yabp
                  dxiazic = -terma*xab*zab - ddtdxia*zabp                   ;//                  dxiazic = -terma*xab*zab - ddtdxia*zabp
                  dyiaxic = -terma*xab*yab - ddtdyia*xabp                   ;//                  dyiaxic = -terma*xab*yab - ddtdyia*xabp
                  dyiayic = terma*(xab*xab+zab*zab) - ddtdyia*yabp          ;//                  dyiayic = terma*(xab*xab+zab*zab) - ddtdyia*yabp
                  dyiazic = -terma*yab*zab - ddtdyia*zabp                   ;//                  dyiazic = -terma*yab*zab - ddtdyia*zabp
                  dziaxic = -terma*xab*zab - ddtdzia*xabp                   ;//                  dziaxic = -terma*xab*zab - ddtdzia*xabp
                  dziayic = -terma*yab*zab - ddtdzia*yabp                   ;//                  dziayic = -terma*yab*zab - ddtdzia*yabp
                  dziazic = terma*(xab*xab+yab*yab) - ddtdzia*zabp          ;//                  dziazic = terma*(xab*xab+yab*yab) - ddtdzia*zabp
///                                                                         ;//c
///     get some second derivative chain rule terms by difference           ;//c     get some second derivative chain rule terms by difference
///                                                                         ;//c
                  dxibxia = -dxiaxia - dxiaxic                              ;//                  dxibxia = -dxiaxia - dxiaxic
                  dxibyia = -dxiayia - dyiaxic                              ;//                  dxibyia = -dxiayia - dyiaxic
                  dxibzia = -dxiazia - dziaxic                              ;//                  dxibzia = -dxiazia - dziaxic
                  dyibxia = -dxiayia - dxiayic                              ;//                  dyibxia = -dxiayia - dxiayic
                  dyibyia = -dyiayia - dyiayic                              ;//                  dyibyia = -dyiayia - dyiayic
                  dyibzia = -dyiazia - dziayic                              ;//                  dyibzia = -dyiazia - dziayic
                  dzibxia = -dxiazia - dxiazic                              ;//                  dzibxia = -dxiazia - dxiazic
                  dzibyia = -dyiazia - dyiazic                              ;//                  dzibyia = -dyiazia - dyiazic
                  dzibzia = -dziazia - dziazic                              ;//                  dzibzia = -dziazia - dziazic
                  dxibxic = -dxicxic - dxiaxic                              ;//                  dxibxic = -dxicxic - dxiaxic
                  dxibyic = -dxicyic - dxiayic                              ;//                  dxibyic = -dxicyic - dxiayic
                  dxibzic = -dxiczic - dxiazic                              ;//                  dxibzic = -dxiczic - dxiazic
                  dyibxic = -dxicyic - dyiaxic                              ;//                  dyibxic = -dxicyic - dyiaxic
                  dyibyic = -dyicyic - dyiayic                              ;//                  dyibyic = -dyicyic - dyiayic
                  dyibzic = -dyiczic - dyiazic                              ;//                  dyibzic = -dyiczic - dyiazic
                  dzibxic = -dxiczic - dziaxic                              ;//                  dzibxic = -dxiczic - dziaxic
                  dzibyic = -dyiczic - dziayic                              ;//                  dzibyic = -dyiczic - dziayic
                  dzibzic = -dziczic - dziazic                              ;//                  dzibzic = -dziczic - dziazic
                  dxibxib = -dxibxia - dxibxic                              ;//                  dxibxib = -dxibxia - dxibxic
                  dxibyib = -dxibyia - dxibyic                              ;//                  dxibyib = -dxibyia - dxibyic
                  dxibzib = -dxibzia - dxibzic                              ;//                  dxibzib = -dxibzia - dxibzic
                  dyibyib = -dyibyia - dyibyic                              ;//                  dyibyib = -dyibyia - dyibyic
                  dyibzib = -dyibzia - dyibzic                              ;//                  dyibzib = -dyibzia - dyibzic
                  dzibzib = -dzibzia - dzibzic                              ;//                  dzibzib = -dzibzia - dzibzic
///                                                                         ;//c
///     increment diagonal and off-diagonal Hessian elements                ;//c     increment diagonal and off-diagonal Hessian elements
///                                                                         ;//c
                  if (ia  ==  iatom) {                                      ;//                  if (ia .eq. iatom) then
                     hessx[1,ia] = hessx[1,ia] + deddt*dxiaxia               //                     hessx(1,ia) = hessx(1,ia) + deddt*dxiaxia
                                        + d2eddt2*ddtdxia*ddtdxia           ;//     &                                  + d2eddt2*ddtdxia*ddtdxia
                     hessx[2,ia] = hessx[2,ia] + deddt*dxiayia               //                     hessx(2,ia) = hessx(2,ia) + deddt*dxiayia
                                        + d2eddt2*ddtdxia*ddtdyia           ;//     &                                  + d2eddt2*ddtdxia*ddtdyia
                     hessx[3,ia] = hessx[3,ia] + deddt*dxiazia               //                     hessx(3,ia) = hessx(3,ia) + deddt*dxiazia
                                        + d2eddt2*ddtdxia*ddtdzia           ;//     &                                  + d2eddt2*ddtdxia*ddtdzia
                     hessy[1,ia] = hessy[1,ia] + deddt*dxiayia               //                     hessy(1,ia) = hessy(1,ia) + deddt*dxiayia
                                        + d2eddt2*ddtdyia*ddtdxia           ;//     &                                  + d2eddt2*ddtdyia*ddtdxia
                     hessy[2,ia] = hessy[2,ia] + deddt*dyiayia               //                     hessy(2,ia) = hessy(2,ia) + deddt*dyiayia
                                        + d2eddt2*ddtdyia*ddtdyia           ;//     &                                  + d2eddt2*ddtdyia*ddtdyia
                     hessy[3,ia] = hessy[3,ia] + deddt*dyiazia               //                     hessy(3,ia) = hessy(3,ia) + deddt*dyiazia
                                        + d2eddt2*ddtdyia*ddtdzia           ;//     &                                  + d2eddt2*ddtdyia*ddtdzia
                     hessz[1,ia] = hessz[1,ia] + deddt*dxiazia               //                     hessz(1,ia) = hessz(1,ia) + deddt*dxiazia
                                        + d2eddt2*ddtdzia*ddtdxia           ;//     &                                  + d2eddt2*ddtdzia*ddtdxia
                     hessz[2,ia] = hessz[2,ia] + deddt*dyiazia               //                     hessz(2,ia) = hessz(2,ia) + deddt*dyiazia
                                        + d2eddt2*ddtdzia*ddtdyia           ;//     &                                  + d2eddt2*ddtdzia*ddtdyia
                     hessz[3,ia] = hessz[3,ia] + deddt*dziazia               //                     hessz(3,ia) = hessz(3,ia) + deddt*dziazia
                                        + d2eddt2*ddtdzia*ddtdzia           ;//     &                                  + d2eddt2*ddtdzia*ddtdzia
                     hessx[1,ib] = hessx[1,ib] + deddt*dxibxia               //                     hessx(1,ib) = hessx(1,ib) + deddt*dxibxia
                                        + d2eddt2*ddtdxia*ddtdxib           ;//     &                                  + d2eddt2*ddtdxia*ddtdxib
                     hessx[2,ib] = hessx[2,ib] + deddt*dyibxia               //                     hessx(2,ib) = hessx(2,ib) + deddt*dyibxia
                                        + d2eddt2*ddtdxia*ddtdyib           ;//     &                                  + d2eddt2*ddtdxia*ddtdyib
                     hessx[3,ib] = hessx[3,ib] + deddt*dzibxia               //                     hessx(3,ib) = hessx(3,ib) + deddt*dzibxia
                                        + d2eddt2*ddtdxia*ddtdzib           ;//     &                                  + d2eddt2*ddtdxia*ddtdzib
                     hessy[1,ib] = hessy[1,ib] + deddt*dxibyia               //                     hessy(1,ib) = hessy(1,ib) + deddt*dxibyia
                                        + d2eddt2*ddtdyia*ddtdxib           ;//     &                                  + d2eddt2*ddtdyia*ddtdxib
                     hessy[2,ib] = hessy[2,ib] + deddt*dyibyia               //                     hessy(2,ib) = hessy(2,ib) + deddt*dyibyia
                                        + d2eddt2*ddtdyia*ddtdyib           ;//     &                                  + d2eddt2*ddtdyia*ddtdyib
                     hessy[3,ib] = hessy[3,ib] + deddt*dzibyia               //                     hessy(3,ib) = hessy(3,ib) + deddt*dzibyia
                                        + d2eddt2*ddtdyia*ddtdzib           ;//     &                                  + d2eddt2*ddtdyia*ddtdzib
                     hessz[1,ib] = hessz[1,ib] + deddt*dxibzia               //                     hessz(1,ib) = hessz(1,ib) + deddt*dxibzia
                                        + d2eddt2*ddtdzia*ddtdxib           ;//     &                                  + d2eddt2*ddtdzia*ddtdxib
                     hessz[2,ib] = hessz[2,ib] + deddt*dyibzia               //                     hessz(2,ib) = hessz(2,ib) + deddt*dyibzia
                                        + d2eddt2*ddtdzia*ddtdyib           ;//     &                                  + d2eddt2*ddtdzia*ddtdyib
                     hessz[3,ib] = hessz[3,ib] + deddt*dzibzia               //                     hessz(3,ib) = hessz(3,ib) + deddt*dzibzia
                                        + d2eddt2*ddtdzia*ddtdzib           ;//     &                                  + d2eddt2*ddtdzia*ddtdzib
                     hessx[1,ic] = hessx[1,ic] + deddt*dxiaxic               //                     hessx(1,ic) = hessx(1,ic) + deddt*dxiaxic
                                        + d2eddt2*ddtdxia*ddtdxic           ;//     &                                  + d2eddt2*ddtdxia*ddtdxic
                     hessx[2,ic] = hessx[2,ic] + deddt*dxiayic               //                     hessx(2,ic) = hessx(2,ic) + deddt*dxiayic
                                        + d2eddt2*ddtdxia*ddtdyic           ;//     &                                  + d2eddt2*ddtdxia*ddtdyic
                     hessx[3,ic] = hessx[3,ic] + deddt*dxiazic               //                     hessx(3,ic) = hessx(3,ic) + deddt*dxiazic
                                        + d2eddt2*ddtdxia*ddtdzic           ;//     &                                  + d2eddt2*ddtdxia*ddtdzic
                     hessy[1,ic] = hessy[1,ic] + deddt*dyiaxic               //                     hessy(1,ic) = hessy(1,ic) + deddt*dyiaxic
                                        + d2eddt2*ddtdyia*ddtdxic           ;//     &                                  + d2eddt2*ddtdyia*ddtdxic
                     hessy[2,ic] = hessy[2,ic] + deddt*dyiayic               //                     hessy(2,ic) = hessy(2,ic) + deddt*dyiayic
                                        + d2eddt2*ddtdyia*ddtdyic           ;//     &                                  + d2eddt2*ddtdyia*ddtdyic
                     hessy[3,ic] = hessy[3,ic] + deddt*dyiazic               //                     hessy(3,ic) = hessy(3,ic) + deddt*dyiazic
                                        + d2eddt2*ddtdyia*ddtdzic           ;//     &                                  + d2eddt2*ddtdyia*ddtdzic
                     hessz[1,ic] = hessz[1,ic] + deddt*dziaxic               //                     hessz(1,ic) = hessz(1,ic) + deddt*dziaxic
                                        + d2eddt2*ddtdzia*ddtdxic           ;//     &                                  + d2eddt2*ddtdzia*ddtdxic
                     hessz[2,ic] = hessz[2,ic] + deddt*dziayic               //                     hessz(2,ic) = hessz(2,ic) + deddt*dziayic
                                        + d2eddt2*ddtdzia*ddtdyic           ;//     &                                  + d2eddt2*ddtdzia*ddtdyic
                     hessz[3,ic] = hessz[3,ic] + deddt*dziazic               //                     hessz(3,ic) = hessz(3,ic) + deddt*dziazic
                                        + d2eddt2*ddtdzia*ddtdzic           ;//     &                                  + d2eddt2*ddtdzia*ddtdzic
                  } else if (ib  ==  iatom) {                               ;//                  else if (ib .eq. iatom) then
                     hessx[1,ib] = hessx[1,ib] + deddt*dxibxib               //                     hessx(1,ib) = hessx(1,ib) + deddt*dxibxib
                                        + d2eddt2*ddtdxib*ddtdxib           ;//     &                                  + d2eddt2*ddtdxib*ddtdxib
                     hessx[2,ib] = hessx[2,ib] + deddt*dxibyib               //                     hessx(2,ib) = hessx(2,ib) + deddt*dxibyib
                                        + d2eddt2*ddtdxib*ddtdyib           ;//     &                                  + d2eddt2*ddtdxib*ddtdyib
                     hessx[3,ib] = hessx[3,ib] + deddt*dxibzib               //                     hessx(3,ib) = hessx(3,ib) + deddt*dxibzib
                                        + d2eddt2*ddtdxib*ddtdzib           ;//     &                                  + d2eddt2*ddtdxib*ddtdzib
                     hessy[1,ib] = hessy[1,ib] + deddt*dxibyib               //                     hessy(1,ib) = hessy(1,ib) + deddt*dxibyib
                                        + d2eddt2*ddtdyib*ddtdxib           ;//     &                                  + d2eddt2*ddtdyib*ddtdxib
                     hessy[2,ib] = hessy[2,ib] + deddt*dyibyib               //                     hessy(2,ib) = hessy(2,ib) + deddt*dyibyib
                                        + d2eddt2*ddtdyib*ddtdyib           ;//     &                                  + d2eddt2*ddtdyib*ddtdyib
                     hessy[3,ib] = hessy[3,ib] + deddt*dyibzib               //                     hessy(3,ib) = hessy(3,ib) + deddt*dyibzib
                                        + d2eddt2*ddtdyib*ddtdzib           ;//     &                                  + d2eddt2*ddtdyib*ddtdzib
                     hessz[1,ib] = hessz[1,ib] + deddt*dxibzib               //                     hessz(1,ib) = hessz(1,ib) + deddt*dxibzib
                                        + d2eddt2*ddtdzib*ddtdxib           ;//     &                                  + d2eddt2*ddtdzib*ddtdxib
                     hessz[2,ib] = hessz[2,ib] + deddt*dyibzib               //                     hessz(2,ib) = hessz(2,ib) + deddt*dyibzib
                                        + d2eddt2*ddtdzib*ddtdyib           ;//     &                                  + d2eddt2*ddtdzib*ddtdyib
                     hessz[3,ib] = hessz[3,ib] + deddt*dzibzib               //                     hessz(3,ib) = hessz(3,ib) + deddt*dzibzib
                                        + d2eddt2*ddtdzib*ddtdzib           ;//     &                                  + d2eddt2*ddtdzib*ddtdzib
                     hessx[1,ia] = hessx[1,ia] + deddt*dxibxia               //                     hessx(1,ia) = hessx(1,ia) + deddt*dxibxia
                                        + d2eddt2*ddtdxib*ddtdxia           ;//     &                                  + d2eddt2*ddtdxib*ddtdxia
                     hessx[2,ia] = hessx[2,ia] + deddt*dxibyia               //                     hessx(2,ia) = hessx(2,ia) + deddt*dxibyia
                                        + d2eddt2*ddtdxib*ddtdyia           ;//     &                                  + d2eddt2*ddtdxib*ddtdyia
                     hessx[3,ia] = hessx[3,ia] + deddt*dxibzia               //                     hessx(3,ia) = hessx(3,ia) + deddt*dxibzia
                                        + d2eddt2*ddtdxib*ddtdzia           ;//     &                                  + d2eddt2*ddtdxib*ddtdzia
                     hessy[1,ia] = hessy[1,ia] + deddt*dyibxia               //                     hessy(1,ia) = hessy(1,ia) + deddt*dyibxia
                                        + d2eddt2*ddtdyib*ddtdxia           ;//     &                                  + d2eddt2*ddtdyib*ddtdxia
                     hessy[2,ia] = hessy[2,ia] + deddt*dyibyia               //                     hessy(2,ia) = hessy(2,ia) + deddt*dyibyia
                                        + d2eddt2*ddtdyib*ddtdyia           ;//     &                                  + d2eddt2*ddtdyib*ddtdyia
                     hessy[3,ia] = hessy[3,ia] + deddt*dyibzia               //                     hessy(3,ia) = hessy(3,ia) + deddt*dyibzia
                                        + d2eddt2*ddtdyib*ddtdzia           ;//     &                                  + d2eddt2*ddtdyib*ddtdzia
                     hessz[1,ia] = hessz[1,ia] + deddt*dzibxia               //                     hessz(1,ia) = hessz(1,ia) + deddt*dzibxia
                                        + d2eddt2*ddtdzib*ddtdxia           ;//     &                                  + d2eddt2*ddtdzib*ddtdxia
                     hessz[2,ia] = hessz[2,ia] + deddt*dzibyia               //                     hessz(2,ia) = hessz(2,ia) + deddt*dzibyia
                                        + d2eddt2*ddtdzib*ddtdyia           ;//     &                                  + d2eddt2*ddtdzib*ddtdyia
                     hessz[3,ia] = hessz[3,ia] + deddt*dzibzia               //                     hessz(3,ia) = hessz(3,ia) + deddt*dzibzia
                                        + d2eddt2*ddtdzib*ddtdzia           ;//     &                                  + d2eddt2*ddtdzib*ddtdzia
                     hessx[1,ic] = hessx[1,ic] + deddt*dxibxic               //                     hessx(1,ic) = hessx(1,ic) + deddt*dxibxic
                                        + d2eddt2*ddtdxib*ddtdxic           ;//     &                                  + d2eddt2*ddtdxib*ddtdxic
                     hessx[2,ic] = hessx[2,ic] + deddt*dxibyic               //                     hessx(2,ic) = hessx(2,ic) + deddt*dxibyic
                                        + d2eddt2*ddtdxib*ddtdyic           ;//     &                                  + d2eddt2*ddtdxib*ddtdyic
                     hessx[3,ic] = hessx[3,ic] + deddt*dxibzic               //                     hessx(3,ic) = hessx(3,ic) + deddt*dxibzic
                                        + d2eddt2*ddtdxib*ddtdzic           ;//     &                                  + d2eddt2*ddtdxib*ddtdzic
                     hessy[1,ic] = hessy[1,ic] + deddt*dyibxic               //                     hessy(1,ic) = hessy(1,ic) + deddt*dyibxic
                                        + d2eddt2*ddtdyib*ddtdxic           ;//     &                                  + d2eddt2*ddtdyib*ddtdxic
                     hessy[2,ic] = hessy[2,ic] + deddt*dyibyic               //                     hessy(2,ic) = hessy(2,ic) + deddt*dyibyic
                                        + d2eddt2*ddtdyib*ddtdyic           ;//     &                                  + d2eddt2*ddtdyib*ddtdyic
                     hessy[3,ic] = hessy[3,ic] + deddt*dyibzic               //                     hessy(3,ic) = hessy(3,ic) + deddt*dyibzic
                                        + d2eddt2*ddtdyib*ddtdzic           ;//     &                                  + d2eddt2*ddtdyib*ddtdzic
                     hessz[1,ic] = hessz[1,ic] + deddt*dzibxic               //                     hessz(1,ic) = hessz(1,ic) + deddt*dzibxic
                                        + d2eddt2*ddtdzib*ddtdxic           ;//     &                                  + d2eddt2*ddtdzib*ddtdxic
                     hessz[2,ic] = hessz[2,ic] + deddt*dzibyic               //                     hessz(2,ic) = hessz(2,ic) + deddt*dzibyic
                                        + d2eddt2*ddtdzib*ddtdyic           ;//     &                                  + d2eddt2*ddtdzib*ddtdyic
                     hessz[3,ic] = hessz[3,ic] + deddt*dzibzic               //                     hessz(3,ic) = hessz(3,ic) + deddt*dzibzic
                                        + d2eddt2*ddtdzib*ddtdzic           ;//     &                                  + d2eddt2*ddtdzib*ddtdzic
                  } else if (ic  ==  iatom) {                               ;//                  else if (ic .eq. iatom) then
                     hessx[1,ic] = hessx[1,ic] + deddt*dxicxic               //                     hessx(1,ic) = hessx(1,ic) + deddt*dxicxic
                                        + d2eddt2*ddtdxic*ddtdxic           ;//     &                                  + d2eddt2*ddtdxic*ddtdxic
                     hessx[2,ic] = hessx[2,ic] + deddt*dxicyic               //                     hessx(2,ic) = hessx(2,ic) + deddt*dxicyic
                                        + d2eddt2*ddtdxic*ddtdyic           ;//     &                                  + d2eddt2*ddtdxic*ddtdyic
                     hessx[3,ic] = hessx[3,ic] + deddt*dxiczic               //                     hessx(3,ic) = hessx(3,ic) + deddt*dxiczic
                                        + d2eddt2*ddtdxic*ddtdzic           ;//     &                                  + d2eddt2*ddtdxic*ddtdzic
                     hessy[1,ic] = hessy[1,ic] + deddt*dxicyic               //                     hessy(1,ic) = hessy(1,ic) + deddt*dxicyic
                                        + d2eddt2*ddtdyic*ddtdxic           ;//     &                                  + d2eddt2*ddtdyic*ddtdxic
                     hessy[2,ic] = hessy[2,ic] + deddt*dyicyic               //                     hessy(2,ic) = hessy(2,ic) + deddt*dyicyic
                                        + d2eddt2*ddtdyic*ddtdyic           ;//     &                                  + d2eddt2*ddtdyic*ddtdyic
                     hessy[3,ic] = hessy[3,ic] + deddt*dyiczic               //                     hessy(3,ic) = hessy(3,ic) + deddt*dyiczic
                                        + d2eddt2*ddtdyic*ddtdzic           ;//     &                                  + d2eddt2*ddtdyic*ddtdzic
                     hessz[1,ic] = hessz[1,ic] + deddt*dxiczic               //                     hessz(1,ic) = hessz(1,ic) + deddt*dxiczic
                                        + d2eddt2*ddtdzic*ddtdxic           ;//     &                                  + d2eddt2*ddtdzic*ddtdxic
                     hessz[2,ic] = hessz[2,ic] + deddt*dyiczic               //                     hessz(2,ic) = hessz(2,ic) + deddt*dyiczic
                                        + d2eddt2*ddtdzic*ddtdyic           ;//     &                                  + d2eddt2*ddtdzic*ddtdyic
                     hessz[3,ic] = hessz[3,ic] + deddt*dziczic               //                     hessz(3,ic) = hessz(3,ic) + deddt*dziczic
                                        + d2eddt2*ddtdzic*ddtdzic           ;//     &                                  + d2eddt2*ddtdzic*ddtdzic
                     hessx[1,ib] = hessx[1,ib] + deddt*dxibxic               //                     hessx(1,ib) = hessx(1,ib) + deddt*dxibxic
                                        + d2eddt2*ddtdxic*ddtdxib           ;//     &                                  + d2eddt2*ddtdxic*ddtdxib
                     hessx[2,ib] = hessx[2,ib] + deddt*dyibxic               //                     hessx(2,ib) = hessx(2,ib) + deddt*dyibxic
                                        + d2eddt2*ddtdxic*ddtdyib           ;//     &                                  + d2eddt2*ddtdxic*ddtdyib
                     hessx[3,ib] = hessx[3,ib] + deddt*dzibxic               //                     hessx(3,ib) = hessx(3,ib) + deddt*dzibxic
                                        + d2eddt2*ddtdxic*ddtdzib           ;//     &                                  + d2eddt2*ddtdxic*ddtdzib
                     hessy[1,ib] = hessy[1,ib] + deddt*dxibyic               //                     hessy(1,ib) = hessy(1,ib) + deddt*dxibyic
                                        + d2eddt2*ddtdyic*ddtdxib           ;//     &                                  + d2eddt2*ddtdyic*ddtdxib
                     hessy[2,ib] = hessy[2,ib] + deddt*dyibyic               //                     hessy(2,ib) = hessy(2,ib) + deddt*dyibyic
                                        + d2eddt2*ddtdyic*ddtdyib           ;//     &                                  + d2eddt2*ddtdyic*ddtdyib
                     hessy[3,ib] = hessy[3,ib] + deddt*dzibyic               //                     hessy(3,ib) = hessy(3,ib) + deddt*dzibyic
                                        + d2eddt2*ddtdyic*ddtdzib           ;//     &                                  + d2eddt2*ddtdyic*ddtdzib
                     hessz[1,ib] = hessz[1,ib] + deddt*dxibzic               //                     hessz(1,ib) = hessz(1,ib) + deddt*dxibzic
                                        + d2eddt2*ddtdzic*ddtdxib           ;//     &                                  + d2eddt2*ddtdzic*ddtdxib
                     hessz[2,ib] = hessz[2,ib] + deddt*dyibzic               //                     hessz(2,ib) = hessz(2,ib) + deddt*dyibzic
                                        + d2eddt2*ddtdzic*ddtdyib           ;//     &                                  + d2eddt2*ddtdzic*ddtdyib
                     hessz[3,ib] = hessz[3,ib] + deddt*dzibzic               //                     hessz(3,ib) = hessz(3,ib) + deddt*dzibzic
                                        + d2eddt2*ddtdzic*ddtdzib           ;//     &                                  + d2eddt2*ddtdzic*ddtdzib
                     hessx[1,ia] = hessx[1,ia] + deddt*dxiaxic               //                     hessx(1,ia) = hessx(1,ia) + deddt*dxiaxic
                                        + d2eddt2*ddtdxic*ddtdxia           ;//     &                                  + d2eddt2*ddtdxic*ddtdxia
                     hessx[2,ia] = hessx[2,ia] + deddt*dyiaxic               //                     hessx(2,ia) = hessx(2,ia) + deddt*dyiaxic
                                        + d2eddt2*ddtdxic*ddtdyia           ;//     &                                  + d2eddt2*ddtdxic*ddtdyia
                     hessx[3,ia] = hessx[3,ia] + deddt*dziaxic               //                     hessx(3,ia) = hessx(3,ia) + deddt*dziaxic
                                        + d2eddt2*ddtdxic*ddtdzia           ;//     &                                  + d2eddt2*ddtdxic*ddtdzia
                     hessy[1,ia] = hessy[1,ia] + deddt*dxiayic               //                     hessy(1,ia) = hessy(1,ia) + deddt*dxiayic
                                        + d2eddt2*ddtdyic*ddtdxia           ;//     &                                  + d2eddt2*ddtdyic*ddtdxia
                     hessy[2,ia] = hessy[2,ia] + deddt*dyiayic               //                     hessy(2,ia) = hessy(2,ia) + deddt*dyiayic
                                        + d2eddt2*ddtdyic*ddtdyia           ;//     &                                  + d2eddt2*ddtdyic*ddtdyia
                     hessy[3,ia] = hessy[3,ia] + deddt*dziayic               //                     hessy(3,ia) = hessy(3,ia) + deddt*dziayic
                                        + d2eddt2*ddtdyic*ddtdzia           ;//     &                                  + d2eddt2*ddtdyic*ddtdzia
                     hessz[1,ia] = hessz[1,ia] + deddt*dxiazic               //                     hessz(1,ia) = hessz(1,ia) + deddt*dxiazic
                                        + d2eddt2*ddtdzic*ddtdxia           ;//     &                                  + d2eddt2*ddtdzic*ddtdxia
                     hessz[2,ia] = hessz[2,ia] + deddt*dyiazic               //                     hessz(2,ia) = hessz(2,ia) + deddt*dyiazic
                                        + d2eddt2*ddtdzic*ddtdyia           ;//     &                                  + d2eddt2*ddtdzic*ddtdyia
                     hessz[3,ia] = hessz[3,ia] + deddt*dziazic               //                     hessz(3,ia) = hessz(3,ia) + deddt*dziazic
                                        + d2eddt2*ddtdzic*ddtdzia           ;//     &                                  + d2eddt2*ddtdzic*ddtdzia
                  }                                                         ;//                  end if
///                                                                         ;//c
///     construct a second orthogonal direction for linear angles           ;//c     construct a second orthogonal direction for linear angles
///                                                                         ;//c
                  if (linear) {                                             ;//                  if (linear) then
                     linear =  false                                        ;//                     linear = .false.
                     xpo = xp                                               ;//                     xpo = xp
                     ypo = yp                                               ;//                     ypo = yp
                     zpo = zp                                               ;//                     zpo = zp
                     xp = ypo*zab - zpo*yab                                 ;//                     xp = ypo*zab - zpo*yab
                     yp = zpo*xab - xpo*zab                                 ;//                     yp = zpo*xab - xpo*zab
                     zp = xpo*yab - ypo*xab                                 ;//                     zp = xpo*yab - ypo*xab
                     rp = sqrt(xp*xp + yp*yp + zp*zp)                       ;//                     rp = sqrt(xp*xp + yp*yp + zp*zp)
                     goto goto10                                            ;//                     goto 10
                  }                                                         ;//                  end if
               }                                                            ;//               end if
            }                                                               ;//            end if
         }                                                                  ;//         end if
      }                                                                     ;//      end do
      return                                                                ;//      return
    }                                                                        //      end
///                                                                         ;//c
///                                                                         ;//c
///     #################################################################   ;//c     #################################################################
///     ##                                                             ##   ;//c     ##                                                             ##
///     ##  subroutine eangle2b  --  in-plane bend Hessian; numerical  ##   ;//c     ##  subroutine eangle2b  --  in-plane bend Hessian; numerical  ##
///     ##                                                             ##   ;//c     ##                                                             ##
///     #################################################################   ;//c     #################################################################
///                                                                         ;//c
///                                                                         ;//c
///     "eangle2b" computes projected in-plane bending first derivatives    ;//c     "eangle2b" computes projected in-plane bending first derivatives
///     for a single angle with respect to Cartesian coordinates;           ;//c     for a single angle with respect to Cartesian coordinates;
///     used in computation of finite difference second derivatives         ;//c     used in computation of finite difference second derivatives
///                                                                         ;//c
///                                                                          //c
    public void eangle2b(int i) {                                           ;//      subroutine eangle2b (i)
                                                                            ;//      implicit none
                                                                            ;//      include 'sizes.i'
                                                                            ;//      include 'angle.i'
                                                                            ;//      include 'angpot.i'
                                                                            ;//      include 'atoms.i'
                                                                            ;//      include 'bound.i'
                                                                            ;//      include 'deriv.i'
                                                                            ;//      include 'math.i'
      int ia,ib,ic,id                                                       ;//      integer i,ia,ib,ic,id
      double ideal,force                                                    ;//      real*8 ideal,force
      double dot,cosine,angle                                               ;//      real*8 dot,cosine,angle
      double dt,dt2,dt3,dt4                                                 ;//      real*8 dt,dt2,dt3,dt4
      double deddt,term                                                     ;//      real*8 deddt,term
      double terma,termc                                                    ;//      real*8 terma,termc
      double xia,yia,zia                                                    ;//      real*8 xia,yia,zia
      double xib,yib,zib                                                    ;//      real*8 xib,yib,zib
      double xic,yic,zic                                                    ;//      real*8 xic,yic,zic
      double xid,yid,zid                                                    ;//      real*8 xid,yid,zid
      double xad,yad,zad                                                    ;//      real*8 xad,yad,zad
      double xbd,ybd,zbd                                                    ;//      real*8 xbd,ybd,zbd
      double xcd,ycd,zcd                                                    ;//      real*8 xcd,ycd,zcd
      double xip,yip,zip                                                    ;//      real*8 xip,yip,zip
      double xap,yap,zap                                                    ;//      real*8 xap,yap,zap
      double xcp,ycp,zcp                                                    ;//      real*8 xcp,ycp,zcp
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
///                                                                         ;//c
///                                                                         ;//c
///     set the atom numbers and parameters for this angle                  ;//c     set the atom numbers and parameters for this angle
///                                                                         ;//c
      ia = iang[1,i]                                                        ;//      ia = iang(1,i)
      ib = iang[2,i]                                                        ;//      ib = iang(2,i)
      ic = iang[3,i]                                                        ;//      ic = iang(3,i)
      id = iang[4,i]                                                        ;//      id = iang(4,i)
      ideal = anat[i]                                                       ;//      ideal = anat(i)
      force = ak[i]                                                         ;//      force = ak(i)
///                                                                         ;//c
///     get the coordinates of the atoms in the angle                       ;//c     get the coordinates of the atoms in the angle
///                                                                         ;//c
      xia = x[ia]                                                           ;//      xia = x(ia)
      yia = y[ia]                                                           ;//      yia = y(ia)
      zia = z[ia]                                                           ;//      zia = z(ia)
      xib = x[ib]                                                           ;//      xib = x(ib)
      yib = y[ib]                                                           ;//      yib = y(ib)
      zib = z[ib]                                                           ;//      zib = z(ib)
      xic = x[ic]                                                           ;//      xic = x(ic)
      yic = y[ic]                                                           ;//      yic = y(ic)
      zic = z[ic]                                                           ;//      zic = z(ic)
      xid = x[id]                                                           ;//      xid = x(id)
      yid = y[id]                                                           ;//      yid = y(id)
      zid = z[id]                                                           ;//      zid = z(id)
///                                                                         ;//c
///     zero out the first derivative components                            ;//c     zero out the first derivative components
///                                                                         ;//c
      dea[1,ia] = 0.0e0                                                     ;//      dea(1,ia) = 0.0d0
      dea[2,ia] = 0.0e0                                                     ;//      dea(2,ia) = 0.0d0
      dea[3,ia] = 0.0e0                                                     ;//      dea(3,ia) = 0.0d0
      dea[1,ib] = 0.0e0                                                     ;//      dea(1,ib) = 0.0d0
      dea[2,ib] = 0.0e0                                                     ;//      dea(2,ib) = 0.0d0
      dea[3,ib] = 0.0e0                                                     ;//      dea(3,ib) = 0.0d0
      dea[1,ic] = 0.0e0                                                     ;//      dea(1,ic) = 0.0d0
      dea[2,ic] = 0.0e0                                                     ;//      dea(2,ic) = 0.0d0
      dea[3,ic] = 0.0e0                                                     ;//      dea(3,ic) = 0.0d0
      dea[1,id] = 0.0e0                                                     ;//      dea(1,id) = 0.0d0
      dea[2,id] = 0.0e0                                                     ;//      dea(2,id) = 0.0d0
      dea[3,id] = 0.0e0                                                     ;//      dea(3,id) = 0.0d0
///                                                                         ;//c
///     compute the projected in-plane angle gradient                       ;//c     compute the projected in-plane angle gradient
///                                                                         ;//c
      xad = xia - xid                                                       ;//      xad = xia - xid
      yad = yia - yid                                                       ;//      yad = yia - yid
      zad = zia - zid                                                       ;//      zad = zia - zid
      xbd = xib - xid                                                       ;//      xbd = xib - xid
      ybd = yib - yid                                                       ;//      ybd = yib - yid
      zbd = zib - zid                                                       ;//      zbd = zib - zid
      xcd = xic - xid                                                       ;//      xcd = xic - xid
      ycd = yic - yid                                                       ;//      ycd = yic - yid
      zcd = zic - zid                                                       ;//      zcd = zic - zid
      if (use_polymer) {                                                    ;//      if (use_polymer) then
         image(ref xad,ref yad,ref zad)                                     ;//         call image (xad,yad,zad)
         image(ref xbd,ref ybd,ref zbd)                                     ;//         call image (xbd,ybd,zbd)
         image(ref xcd,ref ycd,ref zcd)                                     ;//         call image (xcd,ycd,zcd)
      }                                                                     ;//      end if
      xt = yad*zcd - zad*ycd                                                ;//      xt = yad*zcd - zad*ycd
      yt = zad*xcd - xad*zcd                                                ;//      yt = zad*xcd - xad*zcd
      zt = xad*ycd - yad*xcd                                                ;//      zt = xad*ycd - yad*xcd
      rt2 = xt*xt + yt*yt + zt*zt                                           ;//      rt2 = xt*xt + yt*yt + zt*zt
      delta = -(xt*xbd + yt*ybd + zt*zbd) / rt2                             ;//      delta = -(xt*xbd + yt*ybd + zt*zbd) / rt2
      xip = xib + xt*delta                                                  ;//      xip = xib + xt*delta
      yip = yib + yt*delta                                                  ;//      yip = yib + yt*delta
      zip = zib + zt*delta                                                  ;//      zip = zib + zt*delta
      xap = xia - xip                                                       ;//      xap = xia - xip
      yap = yia - yip                                                       ;//      yap = yia - yip
      zap = zia - zip                                                       ;//      zap = zia - zip
      xcp = xic - xip                                                       ;//      xcp = xic - xip
      ycp = yic - yip                                                       ;//      ycp = yic - yip
      zcp = zic - zip                                                       ;//      zcp = zic - zip
      if (use_polymer) {                                                    ;//      if (use_polymer) then
         image(ref xap,ref yap,ref zap)                                     ;//         call image (xap,yap,zap)
         image(ref xcp,ref ycp,ref zcp)                                     ;//         call image (xcp,ycp,zcp)
      }                                                                     ;//      end if
      rap2 = xap*xap + yap*yap + zap*zap                                    ;//      rap2 = xap*xap + yap*yap + zap*zap
      rcp2 = xcp*xcp + ycp*ycp + zcp*zcp                                    ;//      rcp2 = xcp*xcp + ycp*ycp + zcp*zcp
      if (rap2 != 0.0e0  &&   rcp2 != 0.0e0) {                              ;//      if (rap2.ne.0.0d0 .and. rcp2.ne.0.0d0) then
         xm = ycp*zap - zcp*yap                                             ;//         xm = ycp*zap - zcp*yap
         ym = zcp*xap - xcp*zap                                             ;//         ym = zcp*xap - xcp*zap
         zm = xcp*yap - ycp*xap                                             ;//         zm = xcp*yap - ycp*xap
         rm = sqrt(xm*xm + ym*ym + zm*zm)                                   ;//         rm = sqrt(xm*xm + ym*ym + zm*zm)
         rm = max(rm,0.000001e0)                                            ;//         rm = max(rm,0.000001d0)
         dot = xap*xcp + yap*ycp + zap*zcp                                  ;//         dot = xap*xcp + yap*ycp + zap*zcp
         cosine = dot / sqrt(rap2*rcp2)                                     ;//         cosine = dot / sqrt(rap2*rcp2)
         cosine = min(1.0e0,max(-1.0e0,cosine))                             ;//         cosine = min(1.0d0,max(-1.0d0,cosine))
         angle = radian * acos(cosine)                                      ;//         angle = radian * acos(cosine)
///                                                                         ;//c
///     get the master chain rule term for derivatives                      ;//c     get the master chain rule term for derivatives
///                                                                         ;//c
         dt = angle - ideal                                                 ;//         dt = angle - ideal
         dt2 = dt * dt                                                      ;//         dt2 = dt * dt
         dt3 = dt2 * dt                                                     ;//         dt3 = dt2 * dt
         dt4 = dt2 * dt2                                                    ;//         dt4 = dt2 * dt2
         deddt = angunit * force * dt * radian                               //         deddt = angunit * force * dt * radian
                   * (2.0e0 + 3.0e0*cang*dt + 4.0e0*qang*dt2                 //     &             * (2.0d0 + 3.0d0*cang*dt + 4.0d0*qang*dt2
                        + 5.0e0*pang*dt3 + 6.0e0*sang*dt4)                  ;//     &                  + 5.0d0*pang*dt3 + 6.0d0*sang*dt4)
///                                                                         ;//c
///     chain rule terms for first derivative components                    ;//c     chain rule terms for first derivative components
///                                                                         ;//c
         terma = -deddt / (rap2*rm)                                         ;//         terma = -deddt / (rap2*rm)
         termc = deddt / (rcp2*rm)                                          ;//         termc = deddt / (rcp2*rm)
         dedxia = terma * (yap*zm-zap*ym)                                   ;//         dedxia = terma * (yap*zm-zap*ym)
         dedyia = terma * (zap*xm-xap*zm)                                   ;//         dedyia = terma * (zap*xm-xap*zm)
         dedzia = terma * (xap*ym-yap*xm)                                   ;//         dedzia = terma * (xap*ym-yap*xm)
         dedxic = termc * (ycp*zm-zcp*ym)                                   ;//         dedxic = termc * (ycp*zm-zcp*ym)
         dedyic = termc * (zcp*xm-xcp*zm)                                   ;//         dedyic = termc * (zcp*xm-xcp*zm)
         dedzic = termc * (xcp*ym-ycp*xm)                                   ;//         dedzic = termc * (xcp*ym-ycp*xm)
         dedxip = -dedxia - dedxic                                          ;//         dedxip = -dedxia - dedxic
         dedyip = -dedyia - dedyic                                          ;//         dedyip = -dedyia - dedyic
         dedzip = -dedzia - dedzic                                          ;//         dedzip = -dedzia - dedzic
///                                                                         ;//c
///     chain rule components for the projection of the central atom        ;//c     chain rule components for the projection of the central atom
///                                                                         ;//c
         delta2 = 2.0e0 * delta                                             ;//         delta2 = 2.0d0 * delta
         ptrt2 = (dedxip*xt + dedyip*yt + dedzip*zt) / rt2                  ;//         ptrt2 = (dedxip*xt + dedyip*yt + dedzip*zt) / rt2
         term = (zcd*ybd-ycd*zbd) + delta2*(yt*zcd-zt*ycd)                  ;//         term = (zcd*ybd-ycd*zbd) + delta2*(yt*zcd-zt*ycd)
         dpdxia = delta*(ycd*dedzip-zcd*dedyip) + term*ptrt2                ;//         dpdxia = delta*(ycd*dedzip-zcd*dedyip) + term*ptrt2
         term = (xcd*zbd-zcd*xbd) + delta2*(zt*xcd-xt*zcd)                  ;//         term = (xcd*zbd-zcd*xbd) + delta2*(zt*xcd-xt*zcd)
         dpdyia = delta*(zcd*dedxip-xcd*dedzip) + term*ptrt2                ;//         dpdyia = delta*(zcd*dedxip-xcd*dedzip) + term*ptrt2
         term = (ycd*xbd-xcd*ybd) + delta2*(xt*ycd-yt*xcd)                  ;//         term = (ycd*xbd-xcd*ybd) + delta2*(xt*ycd-yt*xcd)
         dpdzia = delta*(xcd*dedyip-ycd*dedxip) + term*ptrt2                ;//         dpdzia = delta*(xcd*dedyip-ycd*dedxip) + term*ptrt2
         term = (yad*zbd-zad*ybd) + delta2*(zt*yad-yt*zad)                  ;//         term = (yad*zbd-zad*ybd) + delta2*(zt*yad-yt*zad)
         dpdxic = delta*(zad*dedyip-yad*dedzip) + term*ptrt2                ;//         dpdxic = delta*(zad*dedyip-yad*dedzip) + term*ptrt2
         term = (zad*xbd-xad*zbd) + delta2*(xt*zad-zt*xad)                  ;//         term = (zad*xbd-xad*zbd) + delta2*(xt*zad-zt*xad)
         dpdyic = delta*(xad*dedzip-zad*dedxip) + term*ptrt2                ;//         dpdyic = delta*(xad*dedzip-zad*dedxip) + term*ptrt2
         term = (xad*ybd-yad*xbd) + delta2*(yt*xad-xt*yad)                  ;//         term = (xad*ybd-yad*xbd) + delta2*(yt*xad-xt*yad)
         dpdzic = delta*(yad*dedxip-xad*dedyip) + term*ptrt2                ;//         dpdzic = delta*(yad*dedxip-xad*dedyip) + term*ptrt2
///                                                                         ;//c
///     compute derivative components for this interaction                  ;//c     compute derivative components for this interaction
///                                                                         ;//c
         dedxia = dedxia + dpdxia                                           ;//         dedxia = dedxia + dpdxia
         dedyia = dedyia + dpdyia                                           ;//         dedyia = dedyia + dpdyia
         dedzia = dedzia + dpdzia                                           ;//         dedzia = dedzia + dpdzia
         dedxib = dedxip                                                    ;//         dedxib = dedxip
         dedyib = dedyip                                                    ;//         dedyib = dedyip
         dedzib = dedzip                                                    ;//         dedzib = dedzip
         dedxic = dedxic + dpdxic                                           ;//         dedxic = dedxic + dpdxic
         dedyic = dedyic + dpdyic                                           ;//         dedyic = dedyic + dpdyic
         dedzic = dedzic + dpdzic                                           ;//         dedzic = dedzic + dpdzic
         dedxid = -dedxia - dedxib - dedxic                                 ;//         dedxid = -dedxia - dedxib - dedxic
         dedyid = -dedyia - dedyib - dedyic                                 ;//         dedyid = -dedyia - dedyib - dedyic
         dedzid = -dedzia - dedzib - dedzic                                 ;//         dedzid = -dedzia - dedzib - dedzic
///                                                                         ;//c
///     set the in-plane angle bending first derivatives                    ;//c     set the in-plane angle bending first derivatives
///                                                                         ;//c
         dea[1,ia] = dedxia                                                 ;//         dea(1,ia) = dedxia
         dea[2,ia] = dedyia                                                 ;//         dea(2,ia) = dedyia
         dea[3,ia] = dedzia                                                 ;//         dea(3,ia) = dedzia
         dea[1,ib] = dedxib                                                 ;//         dea(1,ib) = dedxib
         dea[2,ib] = dedyib                                                 ;//         dea(2,ib) = dedyib
         dea[3,ib] = dedzib                                                 ;//         dea(3,ib) = dedzib
         dea[1,ic] = dedxic                                                 ;//         dea(1,ic) = dedxic
         dea[2,ic] = dedyic                                                 ;//         dea(2,ic) = dedyic
         dea[3,ic] = dedzic                                                 ;//         dea(3,ic) = dedzic
         dea[1,id] = dedxid                                                 ;//         dea(1,id) = dedxid
         dea[2,id] = dedyid                                                 ;//         dea(2,id) = dedyid
         dea[3,id] = dedzid                                                 ;//         dea(3,id) = dedzid
      }                                                                     ;//      end if
      return                                                                ;//      return
    }                                                                        //      end
}
}
}
