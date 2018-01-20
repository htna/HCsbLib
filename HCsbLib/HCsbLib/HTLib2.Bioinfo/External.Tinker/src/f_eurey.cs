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
///     ##  COPYRIGHT (C)  1993  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ###########################################################
///     ##                                                       ##
///     ##  subroutine eurey  --  Urey-Bradley potential energy  ##
///     ##                                                       ##
///     ###########################################################
///
///
///     "eurey" calculates the Urey-Bradley 1-3 interaction energy
///
///
    public void eurey() {                                           ;//      subroutine eurey
                                                                    ;//      implicit none
                                                                    ;//      include 'sizes.i'
                                                                    ;//      include 'atoms.i'
                                                                    ;//      include 'bound.i'
                                                                    ;//      include 'energi.i'
                                                                    ;//      include 'group.i'
                                                                    ;//      include 'urey.i'
                                                                    ;//      include 'urypot.i'
                                                                    ;//      include 'usage.i'
      int i,ia,ic                                                   ;//      integer i,ia,ic
      double e,ideal,force                                          ;//      real*8 e,ideal,force
      double dt,dt2,fgrp                                            ;//      real*8 dt,dt2,fgrp
      double xac,yac,zac,rac                                        ;//      real*8 xac,yac,zac,rac
      bool proceed                                                  ;//      logical proceed
      fgrp = double.NaN;
///
///
///     zero out the Urey-Bradley interaction energy
///
      eub = 0.0e0                                                   ;//      eub = 0.0d0
///
///     calculate the Urey-Bradley 1-3 energy term
///
      for(i=1; i<=nurey; i++) {                                     ;//      do i = 1, nurey
         ia = iury[1,i]                                             ;//         ia = iury(1,i)
         ic = iury[3,i]                                             ;//         ic = iury(3,i)
         ideal = ul[i]                                              ;//         ideal = ul(i)
         force = uk[i]                                              ;//         force = uk(i)
///
///     decide whether to compute the current interaction
///
         proceed =  true                                            ;//         proceed = .true.
         if (use_group) groups(out proceed,out fgrp,ia,ic,0,0,0,0)  ;//         if (use_group)  call groups (proceed,fgrp,ia,ic,0,0,0,0)
         if (proceed)  proceed = (use[ia]  ||  use[ic])             ;//         if (proceed)  proceed = (use(ia) .or. use(ic))
///
///     compute the value of the 1-3 distance deviation
///
         if (proceed) {                                             ;//         if (proceed) then
            xac = x[ia] - x[ic]                                     ;//            xac = x(ia) - x(ic)
            yac = y[ia] - y[ic]                                     ;//            yac = y(ia) - y(ic)
            zac = z[ia] - z[ic]                                     ;//            zac = z(ia) - z(ic)
            if (use_polymer) image(ref xac,ref yac,ref zac)         ;//            if (use_polymer)  call image (xac,yac,zac)
            rac = sqrt(xac*xac + yac*yac + zac*zac)                 ;//            rac = sqrt(xac*xac + yac*yac + zac*zac)
            dt = rac - ideal                                        ;//            dt = rac - ideal
            dt2 = dt * dt                                           ;//            dt2 = dt * dt
///
///     calculate the Urey-Bradley energy for this interaction
///
            e = ureyunit * force * dt2 * (1.0e0+cury*dt+qury*dt2)   ;//            e = ureyunit * force * dt2 * (1.0d0+cury*dt+qury*dt2)
///
///     scale the interaction based on its group membership
///
            if (use_group)  e = e * fgrp                            ;//            if (use_group)  e = e * fgrp
///
///     increment the total Urey-Bradley energy
///
            eub = eub + e                                           ;//            eub = eub + e
         }                                                          ;//         end if
      }                                                             ;//      end do
      return                                                        ;//      return
    }                                                                //      end
}
}
}
