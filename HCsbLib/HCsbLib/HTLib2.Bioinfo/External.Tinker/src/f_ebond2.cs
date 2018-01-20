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
///     ##  subroutine ebond2  --  atom-by-atom bond stretch Hessian  ##
///     ##                                                            ##
///     ################################################################
///
///
///     "ebond2" calculates second derivatives of the bond
///     stretching energy for a single atom at a time
///
///
    public void ebond2(int i) {                                     ;//      subroutine ebond2 (i)
                                                                    ;//      implicit none
                                                                    ;//      include 'sizes.i'
                                                                    ;//      include 'atmlst.i'
                                                                    ;//      include 'atoms.i'
                                                                    ;//      include 'bndpot.i'
                                                                    ;//      include 'bond.i'
                                                                    ;//      include 'bound.i'
                                                                    ;//      include 'couple.i'
                                                                    ;//      include 'group.i'
                                                                    ;//      include 'hessn.i'
      int       j,k,ia,ib                                           ;//      integer i,j,k,ia,ib
      double ideal,force,fgrp                                       ;//      real*8 ideal,force,fgrp
      double xab,yab,zab                                            ;//      real*8 xab,yab,zab
      double rab,rab2                                               ;//      real*8 rab,rab2
      double expterm,bde                                            ;//      real*8 expterm,bde
      double dt,dt2,term                                            ;//      real*8 dt,dt2,term
      double termx,termy,termz                                      ;//      real*8 termx,termy,termz
      double de,deddt,d2eddt2                                       ;//      real*8 de,deddt,d2eddt2
      double[,] d2e = new double[3+1,3+1]                           ;//      real*8 d2e(3,3)
      bool proceed                                                  ;//      logical proceed
      d2eddt2 = fgrp = deddt = double.NaN;
///
///
///     compute the Hessian elements of the bond stretch energy
///
      ia = i                                                        ;//      ia = i
      for(k=1; k<=n12[ia]; k++) {                                   ;//      do k = 1, n12(ia)
         j = bndlist[k,ia]                                          ;//         j = bndlist(k,ia)
         if (ibnd[1,j] ==   ia) {                                   ;//         if (ibnd(1,j) .eq. ia) then
            ib = ibnd[2,j]                                          ;//            ib = ibnd(2,j)
         } else {                                                   ;//         else
            ib = ibnd[1,j]                                          ;//            ib = ibnd(1,j)
         }                                                          ;//         end if
         ideal = bl[j]                                              ;//         ideal = bl(j)
         force = bk[j]                                              ;//         force = bk(j)
///
///     decide whether to compute the current interaction
///
         proceed = true                                             ;//         proceed = .true.
         if (use_group) groups(out proceed,out fgrp,ia,ib,0,0,0,0)  ;//         if (use_group)  call groups (proceed,fgrp,ia,ib,0,0,0,0)
///
///     compute the value of the bond length deviation
///
         if (proceed) {                                             ;//         if (proceed) then
            xab = x[ia] - x[ib]                                     ;//            xab = x(ia) - x(ib)
            yab = y[ia] - y[ib]                                     ;//            yab = y(ia) - y(ib)
            zab = z[ia] - z[ib]                                     ;//            zab = z(ia) - z(ib)
            if (use_polymer)  image(ref xab,ref yab,ref zab)        ;//            if (use_polymer)  call image (xab,yab,zab)
            rab2 = xab*xab + yab*yab + zab*zab                      ;//            rab2 = xab*xab + yab*yab + zab*zab
            rab = sqrt(rab2)                                        ;//            rab = sqrt(rab2)
            dt = rab - ideal                                        ;//            dt = rab - ideal
///
///     harmonic potential uses Taylor expansion of Morse potential
///     through the fourth power of the bond length deviation
///
            if (bndtyp ==   "HARMONIC") {                           ;//            if (bndtyp .eq. 'HARMONIC') then
               dt2 = dt * dt                                        ;//               dt2 = dt * dt
               deddt = 2.0 * bndunit * force * dt                    //               deddt = 2.0d0 * bndunit * force * dt
                          * (1.0+1.5*cbnd*dt+2.0*qbnd*dt2)          ;//     &                    * (1.0d0+1.5d0*cbnd*dt+2.0d0*qbnd*dt2)
               d2eddt2 = 2.0 * bndunit * force                       //               d2eddt2 = 2.0d0 * bndunit * force
                            * (1.0+3.0*cbnd*dt+6.0*qbnd*dt2)        ;//     &                      * (1.0d0+3.0d0*cbnd*dt+6.0d0*qbnd*dt2)
///
///     Morse potential uses energy = BDE * (1 - e**(-alpha*dt))**2)
///     with the approximations alpha = sqrt(ForceConst/BDE) = -2
///     and BDE = Bond Dissociation Energy = ForceConst/alpha**2
///
            } else if (bndtyp  ==  "MORSE") {                       ;//            else if (bndtyp .eq. 'MORSE') then
               expterm = exp(-2.0*dt)                               ;//               expterm = exp(-2.0d0*dt)
               bde = 0.25 * bndunit * force                         ;//               bde = 0.25d0 * bndunit * force
               deddt = 4.0 * bde * (1.0-expterm) * expterm          ;//               deddt = 4.0d0 * bde * (1.0d0-expterm) * expterm
               d2eddt2 = -8.0 * bde * (1.0-2.0*expterm) * expterm   ;//               d2eddt2 = -8.0d0 * bde * (1.0d0-2.0d0*expterm) * expterm
            }                                                       ;//            end if
///
///     scale the interaction based on its group membership
///
            if (use_group) {                                        ;//            if (use_group) then
               deddt = deddt * fgrp                                 ;//               deddt = deddt * fgrp
               d2eddt2 = d2eddt2 * fgrp                             ;//               d2eddt2 = d2eddt2 * fgrp
            }                                                       ;//            end if
///
///     set the chain rule terms for the Hessian elements
///
            if (rab2  ==  0.0) {                                    ;//            if (rab2 .eq. 0.0d0) then
               de = 0.0                                             ;//               de = 0.0d0
               term = 0.0                                           ;//               term = 0.0d0
            } else {                                                ;//            else
               de = deddt / rab                                     ;//               de = deddt / rab
               term = (d2eddt2-de) / rab2                           ;//               term = (d2eddt2-de) / rab2
            }                                                       ;//            end if
            termx = term * xab                                      ;//            termx = term * xab
            termy = term * yab                                      ;//            termy = term * yab
            termz = term * zab                                      ;//            termz = term * zab
            d2e[1,1] = termx*xab + de                               ;//            d2e(1,1) = termx*xab + de
            d2e[1,2] = termx*yab                                    ;//            d2e(1,2) = termx*yab
            d2e[1,3] = termx*zab                                    ;//            d2e(1,3) = termx*zab
            d2e[2,1] = d2e[1,2]                                     ;//            d2e(2,1) = d2e(1,2)
            d2e[2,2] = termy*yab + de                               ;//            d2e(2,2) = termy*yab + de
            d2e[2,3] = termy*zab                                    ;//            d2e(2,3) = termy*zab
            d2e[3,1] = d2e[1,3]                                     ;//            d2e(3,1) = d2e(1,3)
            d2e[3,2] = d2e[2,3]                                     ;//            d2e(3,2) = d2e(2,3)
            d2e[3,3] = termz*zab + de                               ;//            d2e(3,3) = termz*zab + de
///
///     increment diagonal and non-diagonal Hessian elements
///
            for(j=1; j<=3; j++) {                                   ;//            do j = 1, 3
               hessx[j,ia] = hessx[j,ia] + d2e[1,j]                 ;//               hessx(j,ia) = hessx(j,ia) + d2e(1,j)
               hessy[j,ia] = hessy[j,ia] + d2e[2,j]                 ;//               hessy(j,ia) = hessy(j,ia) + d2e(2,j)
               hessz[j,ia] = hessz[j,ia] + d2e[3,j]                 ;//               hessz(j,ia) = hessz(j,ia) + d2e(3,j)
               hessx[j,ib] = hessx[j,ib] - d2e[1,j]                 ;//               hessx(j,ib) = hessx(j,ib) - d2e(1,j)
               hessy[j,ib] = hessy[j,ib] - d2e[2,j]                 ;//               hessy(j,ib) = hessy(j,ib) - d2e(2,j)
               hessz[j,ib] = hessz[j,ib] - d2e[3,j]                 ;//               hessz(j,ib) = hessz(j,ib) - d2e(3,j)
            }                                                       ;//            end do
         }                                                          ;//         end if
      }                                                             ;//      end do
      return                                                        ;//      return
    }                                                                //      end
}
}
}
