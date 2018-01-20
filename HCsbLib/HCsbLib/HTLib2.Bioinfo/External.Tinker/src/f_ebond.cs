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
///     ###########################################################
///     ##                                                       ##
///     ##  subroutine ebond  --  bond stretch potential energy  ##
///     ##                                                       ##
///     ###########################################################
///
///
///     "ebond" calculates the bond stretching energy
///
///
    public void ebond() {                                            //      subroutine ebond
                                                                    ;//      implicit none
                                                                    ;//      include 'sizes.i'
                                                                    ;//      include 'atoms.i'
                                                                    ;//      include 'bndpot.i'
                                                                    ;//      include 'bond.i'
                                                                    ;//      include 'bound.i'
                                                                    ;//      include 'energi.i'
                                                                    ;//      include 'group.i'
                                                                    ;//      include 'usage.i'
      int i,ia,ib                                                   ;//      integer i,ia,ib
      double e,ideal,force                                          ;//      real*8 e,ideal,force
      double expterm,bde                                            ;//      real*8 expterm,bde
      double dt,dt2,fgrp                                            ;//      real*8 dt,dt2,fgrp
      double xab,yab,zab,rab                                        ;//      real*8 xab,yab,zab,rab
      bool proceed                                                  ;//      logical proceed
      e = fgrp = double.NaN;
///
///
///     zero out the bond stretching energy
///
      eb = 0.0e0                                                    ;//      eb = 0.0d0
///
///     calculate the bond stretching energy term
///
      for(i=1; i<=nbond; i++) {                                     ;//      do i = 1, nbond
         ia = ibnd[1,i]                                             ;//         ia = ibnd(1,i)
         ib = ibnd[2,i]                                             ;//         ib = ibnd(2,i)
         ideal = bl[i]                                              ;//         ideal = bl(i)
         force = bk[i]                                              ;//         force = bk(i)
///
///     decide whether to compute the current interaction
///
         proceed =  true                                            ;//proceed = .true.
         if (use_group)  groups(out proceed,out fgrp,ia,ib,0,0,0,0) ;//if (use_group)  call groups (proceed,fgrp,ia,ib,0,0,0,0)
         if (proceed)  proceed = (use[ia]  ||  use[ib])             ;//if (proceed)  proceed = (use(ia) .or. use(ib))
///
///     compute the value of the bond length deviation
///
         if (proceed) {                                             ;//         if (proceed) then
            xab = x[ia] - x[ib]                                     ;//            xab = x(ia) - x(ib)
            yab = y[ia] - y[ib]                                     ;//            yab = y(ia) - y(ib)
            zab = z[ia] - z[ib]                                     ;//            zab = z(ia) - z(ib)
            if (use_polymer) image(ref xab,ref yab,ref zab)         ;//            if (use_polymer)  call image (xab,yab,zab)
            rab = sqrt(xab*xab + yab*yab + zab*zab)                 ;//            rab = sqrt(xab*xab + yab*yab + zab*zab)
            dt = rab - ideal                                        ;//            dt = rab - ideal
///
///     harmonic potential uses Taylor expansion of Morse potential
///     through the fourth power of the bond length deviation
///
            if (bndtyp  ==  "HARMONIC") {                           ;//            if (bndtyp .eq. 'HARMONIC') then
               dt2 = dt * dt                                        ;//               dt2 = dt * dt
               e = bndunit * force * dt2 * (1.0e0+cbnd*dt+qbnd*dt2) ;//               e = bndunit * force * dt2 * (1.0d0+cbnd*dt+qbnd*dt2)
///
///     Morse potential uses energy = BDE * (1 - e**(-alpha*dt))**2)
///     with the approximations alpha = sqrt(ForceConst/BDE) = -2
///     and BDE = Bond Dissociation Energy = ForceConst/alpha**2
///
            } else if (bndtyp  ==  "MORSE") {                       ;//            else if (bndtyp .eq. 'MORSE') then
               expterm = exp(-2.0e0*dt)                             ;//               expterm = exp(-2.0d0*dt)
               bde = 0.25e0 * bndunit * force                       ;//               bde = 0.25d0 * bndunit * force
               e = bde * pow((1.0e0-expterm),2)                     ;//               e = bde * (1.0d0-expterm)**2
            }                                                       ;//            end if
///
///     scale the interaction based on its group membership
///
            if (use_group)  e = e * fgrp                            ;//            if (use_group)  e = e * fgrp
///
///     increment the total bond stretching energy
///
            eb = eb + e                                             ;//            eb = eb + e
         }                                                          ;//         end if
      }                                                             ;//      end do
      return                                                        ;//      return
    }                                                                //      end
}
}
}
