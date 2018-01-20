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
///     ################################################################
///     ##                                                            ##
///     ##  subroutine eurey1  --  bond stretch energy & derivatives  ##
///     ##                                                            ##
///     ################################################################
///
///
///     "eurey1" calculates the Urey-Bradley interaction energy and
///     its first derivatives with respect to Cartesian coordinates
///
///
    public void eurey1() {                                          ;//      subroutine eurey1
                                                                    ;//      implicit none
                                                                    ;//      include 'sizes.i'
                                                                    ;//      include 'atoms.i'
                                                                    ;//      include 'bound.i'
                                                                    ;//      include 'deriv.i'
                                                                    ;//      include 'energi.i'
                                                                    ;//      include 'group.i'
                                                                    ;//      include 'urey.i'
                                                                    ;//      include 'urypot.i'
                                                                    ;//      include 'usage.i'
                                                                    ;//      include 'virial.i'
      int i,ia,ic                                                   ;//      integer i,ia,ic
      double e,de,ideal,force                                       ;//      real*8 e,de,ideal,force
      double dt,dt2,deddt,fgrp                                      ;//      real*8 dt,dt2,deddt,fgrp
      double dedx,dedy,dedz                                         ;//      real*8 dedx,dedy,dedz
      double vxx,vyy,vzz                                            ;//      real*8 vxx,vyy,vzz
      double vyx,vzx,vzy                                            ;//      real*8 vyx,vzx,vzy
      double xac,yac,zac,rac                                        ;//      real*8 xac,yac,zac,rac
      bool proceed                                                  ;//      logical proceed
      fgrp = double.NaN;
///
///
///     zero out the Urey-Bradley energy and first derivatives
///
      eub = 0.0e0                                                   ;//      eub = 0.0d0
      for(i=1; i<=n; i++) {                                         ;//      do i = 1, n
         deub[1,i] = 0.0e0                                          ;//         deub(1,i) = 0.0d0
         deub[2,i] = 0.0e0                                          ;//         deub(2,i) = 0.0d0
         deub[3,i] = 0.0e0                                          ;//         deub(3,i) = 0.0d0
      }                                                             ;//      end do
///
///     calculate the Urey-Bradley 1-3 energy and first derivatives
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
            e = ureyunit * force * dt2 * (1.0e0+cury*dt+qury*dt2)   ;//            e = ureyunit * force * dt2 * (1.0d0+cury*dt+qury*dt2)
            deddt = 2.0e0 * ureyunit * force * dt                    //            deddt = 2.0d0 * ureyunit * force * dt
                       * (1.0e0+1.5e0*cury*dt+2.0e0*qury*dt2)       ;//     &                 * (1.0d0+1.5d0*cury*dt+2.0d0*qury*dt2)
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
            de = deddt / rac                                        ;//            de = deddt / rac
            dedx = de * xac                                         ;//            dedx = de * xac
            dedy = de * yac                                         ;//            dedy = de * yac
            dedz = de * zac                                         ;//            dedz = de * zac
///
///     increment the total Urey-Bradley energy and first derivatives
///
            eub = eub + e                                           ;//            eub = eub + e
            deub[1,ia] = deub[1,ia] + dedx                          ;//            deub(1,ia) = deub(1,ia) + dedx
            deub[2,ia] = deub[2,ia] + dedy                          ;//            deub(2,ia) = deub(2,ia) + dedy
            deub[3,ia] = deub[3,ia] + dedz                          ;//            deub(3,ia) = deub(3,ia) + dedz
            deub[1,ic] = deub[1,ic] - dedx                          ;//            deub(1,ic) = deub(1,ic) - dedx
            deub[2,ic] = deub[2,ic] - dedy                          ;//            deub(2,ic) = deub(2,ic) - dedy
            deub[3,ic] = deub[3,ic] - dedz                          ;//            deub(3,ic) = deub(3,ic) - dedz
///
///     increment the internal virial tensor components
///
            vxx = xac * dedx                                        ;//            vxx = xac * dedx
            vyx = yac * dedx                                        ;//            vyx = yac * dedx
            vzx = zac * dedx                                        ;//            vzx = zac * dedx
            vyy = yac * dedy                                        ;//            vyy = yac * dedy
            vzy = zac * dedy                                        ;//            vzy = zac * dedy
            vzz = zac * dedz                                        ;//            vzz = zac * dedz
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
