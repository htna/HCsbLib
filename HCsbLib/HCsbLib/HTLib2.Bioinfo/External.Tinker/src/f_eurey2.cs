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
///     ##  subroutine eurey2  --  atom-by-atom Urey-Bradley Hessian  ##
///     ##                                                            ##
///     ################################################################
///
///
///     "eurey2" calculates second derivatives of the Urey-Bradley
///     interaction energy for a single atom at a time
///
///
    public void eurey2(int i) {                                 ;//      subroutine eurey2 (i)
                                                                ;//      implicit none
                                                                ;//      include 'sizes.i'
                                                                ;//      include 'atoms.i'
                                                                ;//      include 'bound.i'
                                                                ;//      include 'couple.i'
                                                                ;//      include 'group.i'
                                                                ;//      include 'hessn.i'
                                                                ;//      include 'urey.i'
                                                                ;//      include 'urypot.i'
      int j,ia,ic                                               ;//      integer i,j,ia,ic,iurey
      double ideal,force,fgrp                                   ;//      real*8 ideal,force,fgrp
      double xac,yac,zac                                        ;//      real*8 xac,yac,zac
      double rac,rac2                                           ;//      real*8 rac,rac2
      double dt,dt2,term                                        ;//      real*8 dt,dt2,term
      double termx,termy,termz                                  ;//      real*8 termx,termy,termz
      double de,deddt,d2eddt2                                   ;//      real*8 de,deddt,d2eddt2
      double[,] d2e = new double[1+3,1+3]                       ;//      real*8 d2e(3,3)
      bool proceed                                              ;//      logical proceed
      fgrp = double.NaN;
///
///
///     compute the Hessian elements of the Urey-Bradley energy
///
      foreach(int iurey in dolist(1, nurey)) {                  ;//      do iurey = 1, nurey
         ia = iury[1,iurey]                                     ;//         ia = iury(1,iurey)
         ic = iury[3,iurey]                                     ;//         ic = iury(3,iurey)
         ideal = ul[iurey]                                      ;//         ideal = ul(iurey)
         force = uk[iurey]                                      ;//         force = uk(iurey)
///
///     decide whether to compute the current interaction
///
         proceed = (i == ia  ||  i == ic)                       ;//         proceed = (i.eq.ia .or. i.eq.ic)
         if (proceed  &&   use_group)                            //         if (proceed .and. use_group)
            groups(out proceed,out fgrp,ia,ic,0,0,0,0)          ;//     &      call groups (proceed,fgrp,ia,ic,0,0,0,0)
///
///     compute the value of the 1-3 distance deviation
///
         if (proceed) {                                         ;//         if (proceed) then
            if (i  ==  ic) {                                    ;//            if (i .eq. ic) then
               ic = ia                                          ;//               ic = ia
               ia = i                                           ;//               ia = i
            }                                                   ;//            end if
            xac = x[ia] - x[ic]                                 ;//            xac = x(ia) - x(ic)
            yac = y[ia] - y[ic]                                 ;//            yac = y(ia) - y(ic)
            zac = z[ia] - z[ic]                                 ;//            zac = z(ia) - z(ic)
            if (use_polymer) image(ref xac,ref yac,ref zac)     ;//            if (use_polymer)  call image (xac,yac,zac)
            rac2 = xac*xac + yac*yac + zac*zac                  ;//            rac2 = xac*xac + yac*yac + zac*zac
            rac = sqrt(rac2)                                    ;//            rac = sqrt(rac2)
            dt = rac - ideal                                    ;//            dt = rac - ideal
            dt2 = dt * dt                                       ;//            dt2 = dt * dt
            deddt = 2.0e0 * ureyunit * force * dt                //            deddt = 2.0d0 * ureyunit * force * dt
                       * (1.0e0+1.5e0*cury*dt+2.0e0*qury*dt2)   ;//     &                 * (1.0d0+1.5d0*cury*dt+2.0d0*qury*dt2)
            d2eddt2 = 2.0e0 * ureyunit * force                   //            d2eddt2 = 2.0d0 * ureyunit * force
                         * (1.0e0+3.0e0*cury*dt+6.0e0*qury*dt2) ;//     &                   * (1.0d0+3.0d0*cury*dt+6.0d0*qury*dt2)
///
///     scale the interaction based on its group membership
///
            if (use_group) {                                    ;//            if (use_group) then
               deddt = deddt * fgrp                             ;//               deddt = deddt * fgrp
               d2eddt2 = d2eddt2 * fgrp                         ;//               d2eddt2 = d2eddt2 * fgrp
            }                                                   ;//            end if
///
///     set the chain rule terms for the Hessian elements
///
            de = deddt / rac                                    ;//            de = deddt / rac
            term = (d2eddt2-de) / rac2                          ;//            term = (d2eddt2-de) / rac2
            termx = term * xac                                  ;//            termx = term * xac
            termy = term * yac                                  ;//            termy = term * yac
            termz = term * zac                                  ;//            termz = term * zac
            d2e[1,1] = termx*xac + de                           ;//            d2e(1,1) = termx*xac + de
            d2e[1,2] = termx*yac                                ;//            d2e(1,2) = termx*yac
            d2e[1,3] = termx*zac                                ;//            d2e(1,3) = termx*zac
            d2e[2,1] = d2e[1,2]                                 ;//            d2e(2,1) = d2e(1,2)
            d2e[2,2] = termy*yac + de                           ;//            d2e(2,2) = termy*yac + de
            d2e[2,3] = termy*zac                                ;//            d2e(2,3) = termy*zac
            d2e[3,1] = d2e[1,3]                                 ;//            d2e(3,1) = d2e(1,3)
            d2e[3,2] = d2e[2,3]                                 ;//            d2e(3,2) = d2e(2,3)
            d2e[3,3] = termz*zac + de                           ;//            d2e(3,3) = termz*zac + de
///
///     increment diagonal and non-diagonal Hessian elements
///
            for(j=1; j<=3; j++) {                               ;//            do j = 1, 3
               hessx[j,ia] = hessx[j,ia] + d2e[1,j]             ;//               hessx(j,ia) = hessx(j,ia) + d2e(1,j)
               hessy[j,ia] = hessy[j,ia] + d2e[2,j]             ;//               hessy(j,ia) = hessy(j,ia) + d2e(2,j)
               hessz[j,ia] = hessz[j,ia] + d2e[3,j]             ;//               hessz(j,ia) = hessz(j,ia) + d2e(3,j)
               hessx[j,ic] = hessx[j,ic] - d2e[1,j]             ;//               hessx(j,ic) = hessx(j,ic) - d2e(1,j)
               hessy[j,ic] = hessy[j,ic] - d2e[2,j]             ;//               hessy(j,ic) = hessy(j,ic) - d2e(2,j)
               hessz[j,ic] = hessz[j,ic] - d2e[3,j]             ;//               hessz(j,ic) = hessz(j,ic) - d2e(3,j)
            }                                                   ;//            end do
         }                                                      ;//         end if
      }                                                         ;//      end do
      return                                                    ;//      return
    }                                                            //      end
}
}
}
