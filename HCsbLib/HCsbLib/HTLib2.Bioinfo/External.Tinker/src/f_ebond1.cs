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
///
///
///     ###################################################
///     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ################################################################
///     ##                                                            ##
///     ##  subroutine ebond1  --  bond stretch energy & derivatives  ##
///     ##                                                            ##
///     ################################################################
///
///
///     "ebond1" calculates the bond stretching energy and
///     first derivatives with respect to Cartesian coordinates
///
///
    public void ebond1()                                             //      subroutine ebond1
    {                                                                //      implicit none
                                                                     //      include 'sizes.i'
                                                                     //      include 'atoms.i'
                                                                     //      include 'bndpot.i'
                                                                     //      include 'bond.i'
                                                                     //      include 'bound.i'
                                                                     //      include 'deriv.i'
                                                                     //      include 'energi.i'
                                                                     //      include 'group.i'
                                                                     //      include 'usage.i'
                                                                     //      include 'virial.i'
      int    i,ia,ib                                                ;//      integer i,ia,ib
      double e,ideal,force                                          ;//      real*8 e,ideal,force
      double expterm,bde,fgrp                                       ;//      real*8 expterm,bde,fgrp
      double dt,dt2,deddt                                           ;//      real*8 dt,dt2,deddt
      double de,dedx,dedy,dedz                                      ;//      real*8 de,dedx,dedy,dedz
      double xab,yab,zab,rab                                        ;//      real*8 xab,yab,zab,rab
      double vxx,vyy,vzz                                            ;//      real*8 vxx,vyy,vzz
      double vyx,vzx,vzy                                            ;//      real*8 vyx,vzx,vzy
      bool   proceed                                                ;//      logical proceed
      e = fgrp = deddt = double.NaN;
///
///
///     zero out the bond energy and first derivatives
///
      eb = 0.0                                                      ;//      eb = 0.0d0
      for(i=0; i<n; i++) {                                          ;//      do i = 1, n
         deb[1,i] = 0.0                                             ;//         deb(1,i) = 0.0d0
         deb[2,i] = 0.0                                             ;//         deb(2,i) = 0.0d0
         deb[3,i] = 0.0                                             ;//         deb(3,i) = 0.0d0
      }                                                             ;//      end do
///
///     calculate the bond stretch energy and first derivatives
///
      for(i=1; i<=nbond; i++) {                                     ;//      do i = 1, nbond
         ia = ibnd[1,i]                                             ;//         ia = ibnd(1,i)
         ib = ibnd[2,i]                                             ;//         ib = ibnd(2,i)
         ideal = bl[i]                                              ;//         ideal = bl(i)
         force = bk[i]                                              ;//         force = bk(i)
///
///     decide whether to compute the current interaction
///
         proceed = true                                             ;//         proceed = .true.
         if (use_group)  groups(out proceed,out fgrp,ia,ib,0,0,0,0) ;//         if (use_group)  call groups (proceed,fgrp,ia,ib,0,0,0,0)
         if (proceed)  proceed = (use[ia] || use[ib])               ;//         if (proceed)  proceed = (use(ia) .or. use(ib))
///
///     compute the value of the bond length deviation
///
         if (proceed) {                                             ;//         if (proceed) then
            xab = x[ia] - x[ib]                                     ;//            xab = x(ia) - x(ib)
            yab = y[ia] - y[ib]                                     ;//            yab = y(ia) - y(ib)
            zab = z[ia] - z[ib]                                     ;//            zab = z(ia) - z(ib)
            if (use_polymer)  image (ref xab,ref yab,ref zab)       ;//            if (use_polymer)  call image (xab,yab,zab)
            rab = sqrt(xab*xab + yab*yab + zab*zab)                 ;//            rab = sqrt(xab*xab + yab*yab + zab*zab)
            dt = rab - ideal                                        ;//            dt = rab - ideal
///
///     harmonic potential uses Taylor expansion of Morse potential
///     through the fourth power of the bond length deviation
///
            if (bndtyp == "HARMONIC") {                             ;//            if (bndtyp .eq. 'HARMONIC') then
               dt2 = dt * dt                                        ;//               dt2 = dt * dt
               e = bndunit * force * dt2 * (1.0+cbnd*dt+qbnd*dt2)   ;//               e = bndunit * force * dt2 * (1.0d0+cbnd*dt+qbnd*dt2)
               deddt = 2.0 * bndunit * force * dt                    //               deddt = 2.0d0 * bndunit * force * dt
                           * (1.0+1.5*cbnd*dt+2.0*qbnd*dt2)         ;//     &                    * (1.0d0+1.5d0*cbnd*dt+2.0d0*qbnd*dt2)
///
///     Morse potential uses energy = BDE * (1 - e**(-alpha*dt))**2)
///     with the approximations alpha = sqrt(ForceConst/BDE) = -2
///     and BDE = Bond Dissociation Energy = ForceConst/alpha**2
///
            } else if (bndtyp == "MORSE") {                         ;//            else if (bndtyp .eq. 'MORSE') then
               expterm = exp(-2.0*dt)                               ;//               expterm = exp(-2.0d0*dt)
               bde = 0.25 * bndunit * force                         ;//               bde = 0.25d0 * bndunit * force
               e = bde * pow(1.0-expterm,2)                         ;//               e = bde * (1.0d0-expterm)**2
               deddt = 4.0 * bde * (1.0-expterm) * expterm          ;//               deddt = 4.0d0 * bde * (1.0d0-expterm) * expterm
            }                                                       ;//            end if
///
///     scale the interaction based on its group membership
///
            if (use_group) {                                        ;//            if (use_group) then
               e = e * fgrp                                         ;//               e = e * fgrp
               deddt = deddt * fgrp                                 ;//               deddt = deddt * fgrp
            }                                                       ;//            end if
///
///     compute chain rule terms needed for derivatives
///
            if (rab == 0.0) {                                       ;//            if (rab .eq. 0.0d0) then
               de = 0.0                                             ;//               de = 0.0d0
            } else {                                                ;//            else
               de = deddt / rab                                     ;//               de = deddt / rab
            }                                                       ;//            end if
            dedx = de * xab                                         ;//            dedx = de * xab
            dedy = de * yab                                         ;//            dedy = de * yab
            dedz = de * zab                                         ;//            dedz = de * zab
///
///     increment the total bond energy and first derivatives
///
            eb = eb + e                                             ;//            eb = eb + e
            deb[1,ia] = deb[1,ia] + dedx                            ;//            deb(1,ia) = deb(1,ia) + dedx
            deb[2,ia] = deb[2,ia] + dedy                            ;//            deb(2,ia) = deb(2,ia) + dedy
            deb[3,ia] = deb[3,ia] + dedz                            ;//            deb(3,ia) = deb(3,ia) + dedz
            deb[1,ib] = deb[1,ib] - dedx                            ;//            deb(1,ib) = deb(1,ib) - dedx
            deb[2,ib] = deb[2,ib] - dedy                            ;//            deb(2,ib) = deb(2,ib) - dedy
            deb[3,ib] = deb[3,ib] - dedz                            ;//            deb(3,ib) = deb(3,ib) - dedz
///
///     increment the internal virial tensor components
///
            vxx = xab * dedx                                        ;//            vxx = xab * dedx
            vyx = yab * dedx                                        ;//            vyx = yab * dedx
            vzx = zab * dedx                                        ;//            vzx = zab * dedx
            vyy = yab * dedy                                        ;//            vyy = yab * dedy
            vzy = zab * dedy                                        ;//            vzy = zab * dedy
            vzz = zab * dedz                                        ;//            vzz = zab * dedz
            vir[1,1] = vir[1,1] + vxx                               ;//            vir(1,1) = vir(1,1) + vxx
            vir[2,1] = vir[2,1] + vyx                               ;//            vir(2,1) = vir(2,1) + vyx
            vir[3,1] = vir[3,1] + vzx                               ;//            vir(3,1) = vir(3,1) + vzx
            vir[1,2] = vir[1,2] + vyx                               ;//            vir(1,2) = vir(1,2) + vyx
            vir[2,2] = vir[2,2] + vyy                               ;//            vir(2,2) = vir(2,2) + vyy
            vir[3,2] = vir[3,2] + vzy                               ;//            vir(3,2) = vir(3,2) + vzy
            vir[1,3] = vir[1,3] + vzx                               ;//            vir(1,3) = vir(1,3) + vzx
            vir[2,3] = vir[2,3] + vzy                               ;//            vir(2,3) = vir(2,3) + vzy
            vir[3,3] = vir[3,3] + vzz                               ;//            vir(3,3) = vir(3,3) + vzz
         }                                                          ;//         end if
      }                                                             ;//      end do
      return                                                        ;//      return
    }                                                                //      end
}
}
}
