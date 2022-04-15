using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public partial class CalcEnergy
        {
            /// c
            /// c
            /// c     ###################################################
            /// c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
            /// c     ##              All Rights Reserved              ##
            /// c     ###################################################
            /// c
            /// c     ###############################################################
            /// c     ##                                                           ##
            /// c     ##  subroutine elj2  --  atom-by-atom Lennard-Jones Hessian  ##
            /// c     ##                                                           ##
            /// c     ###############################################################
            /// c
            /// c
            /// c     "elj2" calculates the Lennard-Jones 6-12 van der Waals second
            /// c     derivatives for a single atom at a time
            /// c
            /// c
            /// c
            /// c
            /// c     ###############################################################
            /// c     ##                                                           ##
            /// c     ##  subroutine elj2a  --  double loop Lennard-Jones Hessian  ##
            /// c     ##                                                           ##
            /// c     ###############################################################
            /// c
            /// c
            /// c     "elj2a" calculates the Lennard-Jones 6-12 van der Waals second
            /// c     derivatives using a double loop over relevant atom pairs
            /// c
            /// c
            public static SMatrix3x3 elj2
                ( double[] coordi, Tinker.Prm.Vdw vdwi
                , double[] coordj, Tinker.Prm.Vdw vdwj
                )
            {
#pragma warning disable CS0162 // Rethrow to preserve stack details
                throw new NotImplementedException();
                                                                                    /// c
                                                                                    /// c
                                                                                    /// c     perform dynamic allocation of some local arrays
                                                                                    /// c
                                                                                    //        allocate (iv14(n))
                                                                                    //        allocate (vscale(n))
                                                                                    /// c
                                                                                    /// c     set arrays needed to scale connected atom interactions
                                                                                    /// c
                                                                                    //        do i = 1, n
                                                                                    //           vscale(i) = 1.0d0
                                                                                    //           iv14(i) = 0
                                                                                    //        end do
                                                                                    /// c
                                                                                    /// c     set the coefficients for the switching function
                                                                                    /// c
                                                                                    //        mode = 'VDW'
                                                                                    //        call switch (mode)
                                                                                    /// c
                                                                                    /// c     check to see if the atom of interest is a vdw site
                                                                                    /// c
                                                                                    //        nlist = 0
                                                                                    //        do k = 1, nvdw
                                                                                    //           if (ivdw(k) .eq. iatom) then
                                                                                    //              nlist = nlist + 1
                                                                                    //              list(nlist) = iatom
                                                                                    //              goto 10
                                                                                    //           end if
                                                                                    //        end do
                                                                                    //        return
                                                                                    //     10 continue
                                                                                    /// c
                                                                                    /// c     determine the atoms involved via reduction factors
                                                                                    /// c
                                                                                    //        nlist = 1
                                                                                    //        list(nlist) = iatom
                                                                                    //        do k = 1, n12(iatom)
                                                                                    //           i = i12(k,iatom)
                                                                                    //           if (ired(i) .eq. iatom) then
                                                                                    //              nlist = nlist + 1
                                                                                    //              list(nlist) = i
                                                                                    //           end if
                                                                                    //        end do
                                                                                    /// c
                                                                                    /// c     find van der Waals Hessian elements for involved atoms
                                                                                    /// c
                                                                                    //        do ii = 1, nlist
                                                                                    //           i = list(ii)
                                                                                    //           iv = ired(i)
                                                                                    //           redi = kred(i)
                                                                                    //           if (i .ne. iv) then
                                                                                    //              rediv = 1.0d0 - redi
                                                                                    //              redi2 = redi * redi
                                                                                    //              rediv2 = rediv * rediv
                                                                                    //              rediiv = redi * rediv
                                                                                    //           end if
                                                                                    //           it = jvdw(i)
                                                                                    //           xi = xred(i)
                                                                                    //           yi = yred(i)
                                                                                    //           zi = zred(i)
                                                                                    /// c
                                                                                    /// c     set interaction scaling coefficients for connected atoms
                                                                                    /// c
                                                                                    //           do j = 1, n12(i)
                                                                                    //              vscale(i12(j,i)) = v2scale
                                                                                    //           end do
                                                                                    //           do j = 1, n13(i)
                                                                                    //              vscale(i13(j,i)) = v3scale
                                                                                    //           end do
                                                                                    //           do j = 1, n14(i)
                                                                                    //              vscale(i14(j,i)) = v4scale
                                                                                    //              iv14(i14(j,i)) = i
                                                                                    //           end do
                                                                                    //           do j = 1, n15(i)
                                                                                    //              vscale(i15(j,i)) = v5scale
                                                                                    //           end do
                                                                                    /// c
                                                                                    /// c     decide whether to compute the current interaction
                                                                                    /// c
                                                                                    //           do kk = 1, nvdw
                                                                                    //              k = ivdw(kk)
                                                                                    //              kv = ired(k)
                                                                                    //              proceed = .true.
                                                                                    //              if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                                                    //              if (proceed)  proceed = (k .ne. i)
                                                                                    /// c
                                                                                    /// c     compute the Hessian elements for this interaction
                                                                                    /// c
                                                                                    //              if (proceed) then
                                                                                    //                 kt = jvdw(k)
                double xr = coordi[0] - coordj[0];                                  //                 xr = xi - xred(k)
                double yr = coordi[1] - coordj[1];                                  //                 yr = yi - yred(k)
                double zr = coordi[2] - coordj[2];                                  //                 zr = zi - zred(k)
                                                                                    //                 call image (xr,yr,zr)
                double rik2 = (xr*xr + yr*yr + zr*zr);                              //                 rik2 = xr*xr + yr*yr + zr*zr
                                                                                    /// c
                                                                                    /// c     check for an interaction distance less than the cutoff
                                                                                    /// c
                                                                                    //                 if (rik2 .le. off2) then
                double rv  =           vdwi.Rmin2   + vdwj.Rmin2;                   //                    rv = radmin(kt,it)
                double eps = Math.Sqrt(vdwi.Epsilon * vdwj.Epsilon);                //                    eps = epsilon(kt,it)
                                                                                    //                    if (iv14(k) .eq. i) then
                                                                                    //                       rv = radmin4(kt,it)
                                                                                    //                       eps = epsilon4(kt,it)
                                                                                    //                    end if
                       eps = eps * 1;                                               //                    eps = eps * vscale(k)
                double rik = Math.Sqrt(rik2);                                       //                    rik = sqrt(rik2)
                double p6 = Math.Pow(rv,6) / Math.Pow(rik2,3);                      //                    p6 = rv**6 / rik2**3
                double p12 = p6 * p6;                                               //                    p12 = p6 * p6
                double de = eps * (p12-p6) * (-12.0/rik);                           //                    de = eps * (p12-p6) * (-12.0d0/rik)
                double d2e = eps * (13.0*p12-7.0*p6) * (12.0/rik2);                 //                    d2e = eps * (13.0d0*p12-7.0d0*p6) * (12.0d0/rik2)
                                                                                    /// c
                                                                                    /// c     use energy switching if near the cutoff distance
                                                                                    /// c
                                                                                    //                    if (rik2 .gt. cut2) then
                                                                                    //                       e = eps * (p12-2.0d0*p6)
                                                                                    //                       rik3 = rik2 * rik
                                                                                    //                       rik4 = rik2 * rik2
                                                                                    //                       rik5 = rik2 * rik3
                                                                                    //                       taper = c5*rik5 + c4*rik4 + c3*rik3
                                                                                    //       &                          + c2*rik2 + c1*rik + c0
                                                                                    //                       dtaper = 5.0d0*c5*rik4 + 4.0d0*c4*rik3
                                                                                    //       &                           + 3.0d0*c3*rik2 + 2.0d0*c2*rik + c1
                                                                                    //                       d2taper = 20.0d0*c5*rik3 + 12.0d0*c4*rik2
                                                                                    //       &                            + 6.0d0*c3*rik + 2.0d0*c2
                                                                                    //                       d2e = e*d2taper + 2.0d0*de*dtaper + d2e*taper
                                                                                    //                       de = e*dtaper + de*taper
                                                                                    //                    end if
                                                                                    /// c
                                                                                    /// c     scale the interaction based on its group membership
                                                                                    /// c
                                                                                    //                    if (use_group) then
                                                                                    //                       de = de * fgrp
                                                                                    //                       d2e = d2e * fgrp
                                                                                    //                    end if
                                                                                    /// c
                                                                                    /// c     get chain rule terms for van der Waals Hessian elements
                                                                                    /// c
                       de = de / rik            ;                                   //                    de = de / rik
                       d2e = (d2e-de) / rik2    ;                                   //                    d2e = (d2e-de) / rik2
                double d2edx = d2e * xr         ;                                   //                    d2edx = d2e * xr
                double d2edy = d2e * yr         ;                                   //                    d2edy = d2e * yr
                double d2edz = d2e * zr         ;                                   //                    d2edz = d2e * zr
                double term_1_1 = d2edx*xr + de ;                                   //                    term(1,1) = d2edx*xr + de
                double term_1_2 = d2edx*yr      ;                                   //                    term(1,2) = d2edx*yr
                double term_1_3 = d2edx*zr      ;                                   //                    term(1,3) = d2edx*zr
                double term_2_1 = term_1_2      ;                                   //                    term(2,1) = term(1,2)
                double term_2_2 = d2edy*yr + de ;                                   //                    term(2,2) = d2edy*yr + de
                double term_2_3 = d2edy*zr      ;                                   //                    term(2,3) = d2edy*zr
                double term_3_1 = term_1_3      ;                                   //                    term(3,1) = term(1,3)
                double term_3_2 = term_2_3      ;                                   //                    term(3,2) = term(2,3)
                double term_3_3 = d2edz*zr + de ;                                   //                    term(3,3) = d2edz*zr + de
                                                                                    /// c
                                                                                    /// c     increment diagonal and non-diagonal Hessian elements
                                                                                    /// c
                                                                                    //                    if (i .eq. iatom) then
                                                                                    //                       if (i.eq.iv .and. k.eq.kv) then
                                                                                    //                          do j = 1, 3
                                                                                    //                             hessx(j,i) = hessx(j,i) + term(1,j)
                                                                                    //                             hessy(j,i) = hessy(j,i) + term(2,j)
                                                                                    //                             hessz(j,i) = hessz(j,i) + term(3,j)
                                                                                    //                             hessx(j,k) = hessx(j,k) - term(1,j)
                                                                                    //                             hessy(j,k) = hessy(j,k) - term(2,j)
                                                                                    //                             hessz(j,k) = hessz(j,k) - term(3,j)
                                                                                    //                          end do
                                                                                    //                       else if (k .eq. kv) then
                                                                                    //                          do j = 1, 3
                                                                                    //                             hessx(j,i) = hessx(j,i) + term(1,j)*redi2
                                                                                    //                             hessy(j,i) = hessy(j,i) + term(2,j)*redi2
                                                                                    //                             hessz(j,i) = hessz(j,i) + term(3,j)*redi2
                                                                                    //                             hessx(j,k) = hessx(j,k) - term(1,j)*redi
                                                                                    //                             hessy(j,k) = hessy(j,k) - term(2,j)*redi
                                                                                    //                             hessz(j,k) = hessz(j,k) - term(3,j)*redi
                                                                                    //                             hessx(j,iv) = hessx(j,iv) + term(1,j)*rediiv
                                                                                    //                             hessy(j,iv) = hessy(j,iv) + term(2,j)*rediiv
                                                                                    //                             hessz(j,iv) = hessz(j,iv) + term(3,j)*rediiv
                                                                                    //                          end do
                                                                                    //                       else if (i .eq. iv) then
                                                                                    //                          redk = kred(k)
                                                                                    //                          redkv = 1.0d0 - redk
                                                                                    //                          do j = 1, 3
                                                                                    //                             hessx(j,i) = hessx(j,i) + term(1,j)
                                                                                    //                             hessy(j,i) = hessy(j,i) + term(2,j)
                                                                                    //                             hessz(j,i) = hessz(j,i) + term(3,j)
                                                                                    //                             hessx(j,k) = hessx(j,k) - term(1,j)*redk
                                                                                    //                             hessy(j,k) = hessy(j,k) - term(2,j)*redk
                                                                                    //                             hessz(j,k) = hessz(j,k) - term(3,j)*redk
                                                                                    //                             hessx(j,kv) = hessx(j,kv) - term(1,j)*redkv
                                                                                    //                             hessy(j,kv) = hessy(j,kv) - term(2,j)*redkv
                                                                                    //                             hessz(j,kv) = hessz(j,kv) - term(3,j)*redkv
                                                                                    //                          end do
                                                                                    //                       else
                                                                                    //                          redk = kred(k)
                                                                                    //                          redkv = 1.0d0 - redk
                                                                                    //                          redik = redi * redk
                                                                                    //                          redikv = redi * redkv
                                                                                    //                          do j = 1, 3
                                                                                    //                             hessx(j,i) = hessx(j,i) + term(1,j)*redi2
                                                                                    //                             hessy(j,i) = hessy(j,i) + term(2,j)*redi2
                                                                                    //                             hessz(j,i) = hessz(j,i) + term(3,j)*redi2
                                                                                    //                             hessx(j,k) = hessx(j,k) - term(1,j)*redik
                                                                                    //                             hessy(j,k) = hessy(j,k) - term(2,j)*redik
                                                                                    //                             hessz(j,k) = hessz(j,k) - term(3,j)*redik
                                                                                    //                             hessx(j,iv) = hessx(j,iv) + term(1,j)*rediiv
                                                                                    //                             hessy(j,iv) = hessy(j,iv) + term(2,j)*rediiv
                                                                                    //                             hessz(j,iv) = hessz(j,iv) + term(3,j)*rediiv
                                                                                    //                             hessx(j,kv) = hessx(j,kv) - term(1,j)*redikv
                                                                                    //                             hessy(j,kv) = hessy(j,kv) - term(2,j)*redikv
                                                                                    //                             hessz(j,kv) = hessz(j,kv) - term(3,j)*redikv
                                                                                    //                          end do
                                                                                    //                       end if
                                                                                    //                    else if (iv .eq. iatom) then
                                                                                    //                       if (k .eq. kv) then
                                                                                    //                          do j = 1, 3
                                                                                    //                             hessx(j,i) = hessx(j,i) + term(1,j)*rediiv
                                                                                    //                             hessy(j,i) = hessy(j,i) + term(2,j)*rediiv
                                                                                    //                             hessz(j,i) = hessz(j,i) + term(3,j)*rediiv
                                                                                    //                             hessx(j,k) = hessx(j,k) - term(1,j)*rediv
                                                                                    //                             hessy(j,k) = hessy(j,k) - term(2,j)*rediv
                                                                                    //                             hessz(j,k) = hessz(j,k) - term(3,j)*rediv
                                                                                    //                             hessx(j,iv) = hessx(j,iv) + term(1,j)*rediv2
                                                                                    //                             hessy(j,iv) = hessy(j,iv) + term(2,j)*rediv2
                                                                                    //                             hessz(j,iv) = hessz(j,iv) + term(3,j)*rediv2
                                                                                    //                          end do
                                                                                    //                       else
                                                                                    //                          redk = kred(k)
                                                                                    //                          redkv = 1.0d0 - redk
                                                                                    //                          redivk = rediv * redk
                                                                                    //                          redivkv = rediv * redkv
                                                                                    //                          do j = 1, 3
                                                                                    //                             hessx(j,i) = hessx(j,i) + term(1,j)*rediiv
                                                                                    //                             hessy(j,i) = hessy(j,i) + term(2,j)*rediiv
                                                                                    //                             hessz(j,i) = hessz(j,i) + term(3,j)*rediiv
                                                                                    //                             hessx(j,k) = hessx(j,k) - term(1,j)*redivk
                                                                                    //                             hessy(j,k) = hessy(j,k) - term(2,j)*redivk
                                                                                    //                             hessz(j,k) = hessz(j,k) - term(3,j)*redivk
                                                                                    //                             hessx(j,iv) = hessx(j,iv) + term(1,j)*rediv2
                                                                                    //                             hessy(j,iv) = hessy(j,iv) + term(2,j)*rediv2
                                                                                    //                             hessz(j,iv) = hessz(j,iv) + term(3,j)*rediv2
                                                                                    //                             hessx(j,kv) = hessx(j,kv) - term(1,j)*redivkv
                                                                                    //                             hessy(j,kv) = hessy(j,kv) - term(2,j)*redivkv
                                                                                    //                             hessz(j,kv) = hessz(j,kv) - term(3,j)*redivkv
                                                                                    //                          end do
                                                                                    //                       end if
                                                                                    //                    end if
                                                                                    //                 end if
                                                                                    //              end if
                                                                                    //           end do
                                                                                    /// c
                                                                                    /// c     reset interaction scaling coefficients for connected atoms
                                                                                    /// c
                                                                                    //           do j = 1, n12(i)
                                                                                    //              vscale(i12(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //           do j = 1, n13(i)
                                                                                    //              vscale(i13(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //           do j = 1, n14(i)
                                                                                    //              vscale(i14(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //           do j = 1, n15(i)
                                                                                    //              vscale(i15(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //        end do
                                                                                    /// c
                                                                                    /// c     for periodic boundary conditions with large cutoffs
                                                                                    /// c     neighbors must be found by the replicates method
                                                                                    /// c
                //////////////////////////////////////////////////////////////////////        if (.not. use_replica)  return
                                                                                    /// c
                                                                                    /// c     calculate interaction energy with other unit cells
                                                                                    /// c
                                                                                    //        do ii = 1, nlist
                                                                                    //           i = list(ii)
                                                                                    //           iv = ired(i)
                                                                                    //           redi = kred(i)
                                                                                    //           if (i .ne. iv) then
                                                                                    //              rediv = 1.0d0 - redi
                                                                                    //              redi2 = redi * redi
                                                                                    //              rediv2 = rediv * rediv
                                                                                    //              rediiv = redi * rediv
                                                                                    //           end if
                                                                                    //           it = jvdw(i)
                                                                                    //           xi = xred(i)
                                                                                    //           yi = yred(i)
                                                                                    //           zi = zred(i)
                                                                                    /// c
                                                                                    /// c     set interaction scaling coefficients for connected atoms
                                                                                    /// c
                                                                                    //           do j = 1, n12(i)
                                                                                    //              vscale(i12(j,i)) = v2scale
                                                                                    //           end do
                                                                                    //           do j = 1, n13(i)
                                                                                    //              vscale(i13(j,i)) = v3scale
                                                                                    //           end do
                                                                                    //           do j = 1, n14(i)
                                                                                    //              vscale(i14(j,i)) = v4scale
                                                                                    //              iv14(i14(j,i)) = i
                                                                                    //           end do
                                                                                    //           do j = 1, n15(i)
                                                                                    //              vscale(i15(j,i)) = v5scale
                                                                                    //           end do
                                                                                    /// c
                                                                                    /// c     decide whether to compute the current interaction
                                                                                    /// c
                                                                                    //           do kk = 1, nvdw
                                                                                    //              k = ivdw(kk)
                                                                                    //              kv = ired(k)
                                                                                    //              proceed = .true.
                                                                                    //              if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                                                    /// c
                                                                                    /// c     compute the Hessian elements for this interaction
                                                                                    /// c
                                                                                    //              if (proceed) then
                                                                                    //                 kt = jvdw(k)
                                                                                    //                 do jcell = 1, ncell
                                                                                    //                    xr = xi - xred(k)
                                                                                    //                    yr = yi - yred(k)
                                                                                    //                    zr = zi - zred(k)
                                                                                    //                    call imager (xr,yr,zr,jcell)
                                                                                    //                    rik2 = xr*xr + yr*yr + zr*zr
                                                                                    /// c
                                                                                    /// c     check for an interaction distance less than the cutoff
                                                                                    /// c
                                                                                    //                    if (rik2 .le. off2) then
                                                                                    //                       rv = radmin(kt,it)
                                                                                    //                       eps = epsilon(kt,it)
                                                                                    //                       if (use_polymer) then
                                                                                    //                          if (rik2 .le. polycut2) then
                                                                                    //                             if (iv14(k) .eq. i) then
                                                                                    //                                rv = radmin4(kt,it)
                                                                                    //                                eps = epsilon4(kt,it)
                                                                                    //                             end if
                                                                                    //                             eps = eps * vscale(k)
                                                                                    //                          end if
                                                                                    //                       end if
                                                                                    //                       rik = sqrt(rik2)
                                                                                    //                       p6 = rv**6 / rik2**3
                                                                                    //                       p12 = p6 * p6
                                                                                    //                       de = eps * (p12-p6) * (-12.0d0/rik)
                                                                                    //                       d2e = eps * (13.0d0*p12-7.0d0*p6) * (12.0d0/rik2)
                                                                                    /// c
                                                                                    /// c     use energy switching if near the cutoff distance
                                                                                    /// c
                                                                                    //                       if (rik2 .gt. cut2) then
                                                                                    //                          e = eps * (p12-2.0d0*p6)
                                                                                    //                          rik3 = rik2 * rik
                                                                                    //                          rik4 = rik2 * rik2
                                                                                    //                          rik5 = rik2 * rik3
                                                                                    //                          taper = c5*rik5 + c4*rik4 + c3*rik3
                                                                                    //       &                             + c2*rik2 + c1*rik + c0
                                                                                    //                          dtaper = 5.0d0*c5*rik4 + 4.0d0*c4*rik3
                                                                                    //       &                           + 3.0d0*c3*rik2 + 2.0d0*c2*rik + c1
                                                                                    //                          d2taper = 20.0d0*c5*rik3 + 12.0d0*c4*rik2
                                                                                    //       &                             + 6.0d0*c3*rik + 2.0d0*c2
                                                                                    //                          d2e = e*d2taper + 2.0d0*de*dtaper + d2e*taper
                                                                                    //                          de = e*dtaper + de*taper
                                                                                    //                       end if
                                                                                    /// c
                                                                                    /// c     scale the interaction based on its group membership
                                                                                    /// c
                                                                                    //                       if (use_group) then
                                                                                    //                          de = de * fgrp
                                                                                    //                          d2e = d2e * fgrp
                                                                                    //                       end if
                                                                                    /// c
                                                                                    /// c     get chain rule terms for van der Waals Hessian elements
                                                                                    /// c
                                                                                    //                       de = de / rik
                                                                                    //                       d2e = (d2e-de) / rik2
                                                                                    //                       d2edx = d2e * xr
                                                                                    //                       d2edy = d2e * yr
                                                                                    //                       d2edz = d2e * zr
                                                                                    //                       term(1,1) = d2edx*xr + de
                                                                                    //                       term(1,2) = d2edx*yr
                                                                                    //                       term(1,3) = d2edx*zr
                                                                                    //                       term(2,1) = term(1,2)
                                                                                    //                       term(2,2) = d2edy*yr + de
                                                                                    //                       term(2,3) = d2edy*zr
                                                                                    //                       term(3,1) = term(1,3)
                                                                                    //                       term(3,2) = term(2,3)
                                                                                    //                       term(3,3) = d2edz*zr + de
                                                                                    /// c
                                                                                    /// c     increment diagonal and non-diagonal Hessian elements
                                                                                    /// c
                                                                                    //                       if (i .eq. iatom) then
                                                                                    //                          if (i.eq.iv .and. k.eq.kv) then
                                                                                    //                             do j = 1, 3
                                                                                    //                                hessx(j,i) = hessx(j,i) + term(1,j)
                                                                                    //                                hessy(j,i) = hessy(j,i) + term(2,j)
                                                                                    //                                hessz(j,i) = hessz(j,i) + term(3,j)
                                                                                    //                                hessx(j,k) = hessx(j,k) - term(1,j)
                                                                                    //                                hessy(j,k) = hessy(j,k) - term(2,j)
                                                                                    //                                hessz(j,k) = hessz(j,k) - term(3,j)
                                                                                    //                             end do
                                                                                    //                          else if (k .eq. kv) then
                                                                                    //                             do j = 1, 3
                                                                                    //                                hessx(j,i) = hessx(j,i) + term(1,j)*redi2
                                                                                    //                                hessy(j,i) = hessy(j,i) + term(2,j)*redi2
                                                                                    //                                hessz(j,i) = hessz(j,i) + term(3,j)*redi2
                                                                                    //                                hessx(j,k) = hessx(j,k) - term(1,j)*redi
                                                                                    //                                hessy(j,k) = hessy(j,k) - term(2,j)*redi
                                                                                    //                                hessz(j,k) = hessz(j,k) - term(3,j)*redi
                                                                                    //                                hessx(j,iv) = hessx(j,iv)
                                                                                    //       &                                         + term(1,j)*rediiv
                                                                                    //                                hessy(j,iv) = hessy(j,iv)
                                                                                    //       &                                         + term(2,j)*rediiv
                                                                                    //                                hessz(j,iv) = hessz(j,iv)
                                                                                    //       &                                         + term(3,j)*rediiv
                                                                                    //                             end do
                                                                                    //                          else if (i .eq. iv) then
                                                                                    //                             redk = kred(k)
                                                                                    //                             redkv = 1.0d0 - redk
                                                                                    //                             do j = 1, 3
                                                                                    //                                hessx(j,i) = hessx(j,i) + term(1,j)
                                                                                    //                                hessy(j,i) = hessy(j,i) + term(2,j)
                                                                                    //                                hessz(j,i) = hessz(j,i) + term(3,j)
                                                                                    //                                hessx(j,k) = hessx(j,k) - term(1,j)*redk
                                                                                    //                                hessy(j,k) = hessy(j,k) - term(2,j)*redk
                                                                                    //                                hessz(j,k) = hessz(j,k) - term(3,j)*redk
                                                                                    //                                hessx(j,kv) = hessx(j,kv)
                                                                                    //       &                                         - term(1,j)*redkv
                                                                                    //                                hessy(j,kv) = hessy(j,kv)
                                                                                    //       &                                         - term(2,j)*redkv
                                                                                    //                                hessz(j,kv) = hessz(j,kv)
                                                                                    //       &                                         - term(3,j)*redkv
                                                                                    //                             end do
                                                                                    //                          else
                                                                                    //                             redk = kred(k)
                                                                                    //                             redkv = 1.0d0 - redk
                                                                                    //                             redik = redi * redk
                                                                                    //                             redikv = redi * redkv
                                                                                    //                             do j = 1, 3
                                                                                    //                                hessx(j,i) = hessx(j,i) + term(1,j)*redi2
                                                                                    //                                hessy(j,i) = hessy(j,i) + term(2,j)*redi2
                                                                                    //                                hessz(j,i) = hessz(j,i) + term(3,j)*redi2
                                                                                    //                                hessx(j,k) = hessx(j,k) - term(1,j)*redik
                                                                                    //                                hessy(j,k) = hessy(j,k) - term(2,j)*redik
                                                                                    //                                hessz(j,k) = hessz(j,k) - term(3,j)*redik
                                                                                    //                                hessx(j,iv) = hessx(j,iv)
                                                                                    //       &                                         + term(1,j)*rediiv
                                                                                    //                                hessy(j,iv) = hessy(j,iv)
                                                                                    //       &                                         + term(2,j)*rediiv
                                                                                    //                                hessz(j,iv) = hessz(j,iv)
                                                                                    //       &                                         + term(3,j)*rediiv
                                                                                    //                                hessx(j,kv) = hessx(j,kv)
                                                                                    //       &                                         - term(1,j)*redikv
                                                                                    //                                hessy(j,kv) = hessy(j,kv)
                                                                                    //       &                                         - term(2,j)*redikv
                                                                                    //                                hessz(j,kv) = hessz(j,kv)
                                                                                    //       &                                         - term(3,j)*redikv
                                                                                    //                             end do
                                                                                    //                          end if
                                                                                    //                       else if (iv .eq. iatom) then
                                                                                    //                          if (k .eq. kv) then
                                                                                    //                             do j = 1, 3
                                                                                    //                                hessx(j,i) = hessx(j,i) + term(1,j)*rediiv
                                                                                    //                                hessy(j,i) = hessy(j,i) + term(2,j)*rediiv
                                                                                    //                                hessz(j,i) = hessz(j,i) + term(3,j)*rediiv
                                                                                    //                                hessx(j,k) = hessx(j,k) - term(1,j)*rediv
                                                                                    //                                hessy(j,k) = hessy(j,k) - term(2,j)*rediv
                                                                                    //                                hessz(j,k) = hessz(j,k) - term(3,j)*rediv
                                                                                    //                                hessx(j,iv) = hessx(j,iv)
                                                                                    //       &                                         + term(1,j)*rediv2
                                                                                    //                                hessy(j,iv) = hessy(j,iv)
                                                                                    //       &                                         + term(2,j)*rediv2
                                                                                    //                                hessz(j,iv) = hessz(j,iv)
                                                                                    //       &                                         + term(3,j)*rediv2
                                                                                    //                             end do
                                                                                    //                          else
                                                                                    //                             redk = kred(k)
                                                                                    //                             redkv = 1.0d0 - redk
                                                                                    //                             redivk = rediv * redk
                                                                                    //                             redivkv = rediv * redkv
                                                                                    //                             do j = 1, 3
                                                                                    //                                hessx(j,i) = hessx(j,i) + term(1,j)*rediiv
                                                                                    //                                hessy(j,i) = hessy(j,i) + term(2,j)*rediiv
                                                                                    //                                hessz(j,i) = hessz(j,i) + term(3,j)*rediiv
                                                                                    //                                hessx(j,k) = hessx(j,k) - term(1,j)*redivk
                                                                                    //                                hessy(j,k) = hessy(j,k) - term(2,j)*redivk
                                                                                    //                                hessz(j,k) = hessz(j,k) - term(3,j)*redivk
                                                                                    //                                hessx(j,iv) = hessx(j,iv)
                                                                                    //       &                                         + term(1,j)*rediv2
                                                                                    //                                hessy(j,iv) = hessy(j,iv)
                                                                                    //       &                                         + term(2,j)*rediv2
                                                                                    //                                hessz(j,iv) = hessz(j,iv)
                                                                                    //       &                                         + term(3,j)*rediv2
                                                                                    //                                hessx(j,kv) = hessx(j,kv)
                                                                                    //       &                                         - term(1,j)*redivkv
                                                                                    //                                hessy(j,kv) = hessy(j,kv)
                                                                                    //       &                                         - term(2,j)*redivkv
                                                                                    //                                hessz(j,kv) = hessz(j,kv)
                                                                                    //       &                                         - term(3,j)*redivkv
                                                                                    //                             end do
                                                                                    //                          end if
                                                                                    //                       end if
                                                                                    //                    end if
                                                                                    //                 end do
                                                                                    //              end if
                                                                                    //           end do
                                                                                    /// c
                                                                                    /// c     reset interaction scaling coefficients for connected atoms
                                                                                    /// c
                                                                                    //           do j = 1, n12(i)
                                                                                    //              vscale(i12(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //           do j = 1, n13(i)
                                                                                    //              vscale(i13(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //           do j = 1, n14(i)
                                                                                    //              vscale(i14(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //           do j = 1, n15(i)
                                                                                    //              vscale(i15(j,i)) = 1.0d0
                                                                                    //           end do
                                                                                    //        end do
                                                                                    /// c
                                                                                    /// c     perform deallocation of some local arrays
                                                                                    /// c
                                                                                    //        deallocate (iv14)
                                                                                    //        deallocate (vscale)
                                                                                    //        return
                                                                                    //        end
                return new SMatrix3x3
                    ( term_1_1, term_1_2, term_1_3
                    , term_2_1, term_2_2, term_2_3
                    , term_3_1, term_3_2, term_3_3
                    );
            }
        }
    }
}
