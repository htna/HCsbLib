using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
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
        /// c     ##  subroutine echarge2  --  atomwise charge-charge Hessian  ##
        /// c     ##                                                           ##
        /// c     ###############################################################
        /// c
        /// c
        /// c     "echarge2" calculates second derivatives of the
        /// c     charge-charge interaction energy for a single atom
        /// c
        /// c
        /// c
        /// c
        /// c     ################################################################
        /// c     ##                                                            ##
        /// c     ##  subroutine echarge2a  --  charge Hessian via double loop  ##
        /// c     ##                                                            ##
        /// c     ################################################################
        /// c
        /// c
        /// c     "echarge2a" calculates second derivatives of the charge-charge
        /// c     interaction energy for a single atom using a pairwise double loop
        /// c
        /// c
        public static SMatrix3x3 echarge2
            ( double[] coordi, Tinker.Prm.Charge chgi
            , double[] coordj, Tinker.Prm.Charge chgj
            )
        {
            throw new NotImplementedException();
                                                                /// unit.i
            const double coulomb = 332.063714;                  //        parameter (coulomb=332.063714d0)
                                                                /// initprm.f
                                                                /// c
                                                                /// c     set default control parameters for charge-charge terms
                                                                /// c
            const double electric = coulomb;                    //        electric = coulomb
            const double dielec = 1.0;                          //        dielec = 1.0d0
            const double ebuffer = 0.0;                         //        ebuffer = 0.0d0
                                                                //        c2scale = 0.0d0
                                                                //        c3scale = 0.0d0
                                                                //        c4scale = 1.0d0
                                                                //        c5scale = 1.0d0
                                                                //        neutnbr = .false.
                                                                //        neutcut = .false.
                                                                //////////////////////////////////////////////////////////////////////////////
                                                                /// c
                                                                /// c
                                                                /// c     first see if the atom of interest carries a charge
                                                                /// c
                                                                //        do k = 1, nion
                                                                //           if (iion(k) .eq. i) then
            double fi = electric * chgi.pch / dielec;           //              fi = electric * pchg(k) / dielec
                                                                //              in = jion(k)
                                                                //              goto 10
                                                                //           end if
                                                                //        end do
                                                                //        return
                                                                //     10 continue
                                                                /// c
                                                                /// c     store the coordinates of the atom of interest
                                                                /// c
            double xi = coordi[0];                              //        xi = x(i)
            double yi = coordi[1];                              //        yi = y(i)
            double zi = coordi[2];                              //        zi = z(i)
                                                                /// c
                                                                /// c     perform dynamic allocation of some local arrays
                                                                /// c
                                                                //        allocate (cscale(n))
                                                                /// c
                                                                /// c     set array needed to scale connected atom interactions
                                                                /// c
                                                                //        do j = 1, nion
                                                                //           cscale(iion(j)) = 1.0d0
                                                                //        end do
                                                                //        do j = 1, n12(in)
                                                                //           cscale(i12(j,in)) = c2scale
                                                                //        end do
                                                                //        do j = 1, n13(in)
                                                                //           cscale(i13(j,in)) = c3scale
                                                                //        end do
                                                                //        do j = 1, n14(in)
                                                                //           cscale(i14(j,in)) = c4scale
                                                                //        end do
                                                                //        do j = 1, n15(in)
                                                                //           cscale(i15(j,in)) = c5scale
                                                                //        end do
                                                                /// c
                                                                /// c     set cutoff distances and switching function coefficients
                                                                /// c
                                                                //        mode = 'CHARGE'
                                                                //        call switch (mode)
                                                                /// c
                                                                /// c     calculate the charge interaction energy Hessian elements
                                                                /// c
                                                                //        do kk = 1, nion
                                                                //           k = iion(kk)
                                                                //           kn = jion(kk)
                                                                //           proceed = .true.
                                                                //           if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                                //           if (proceed)  proceed = (kn .ne. i)
                                                                /// c
                                                                /// c     compute the energy contribution for this interaction
                                                                /// c
                                                                //           if (proceed) then
            double xr = xi - coordj[0];                         //              xr = xi - x(k)
            double yr = yi - coordj[1];                         //              yr = yi - y(k)
            double zr = zi - coordj[2];                         //              zr = zi - z(k)
                                                                //              if (use_bounds)  call image (xr,yr,zr)
            double r2 = xr*xr + yr*yr + zr*zr;                  //              r2 = xr*xr + yr*yr + zr*zr
                                                                //              if (r2 .le. off2) then
            double r = Math.Sqrt(r2);                           //                 r = sqrt(r2)
            double rb = r + ebuffer;                            //                 rb = r + ebuffer
            double rb2 = rb * rb;                               //                 rb2 = rb * rb
            double fik = fi * chgj.pch * 1;                     //                 fik = fi * pchg(kk) * cscale(kn)
                                                                /// c
                                                                /// c     compute chain rule terms for Hessian matrix elements
                                                                /// c
            double de = -fik / rb2;                             //                 de = -fik / rb2
            double d2e = -2.0 * de/rb;                          //                 d2e = -2.0d0 * de/rb
                                                                /// c
                                                                /// c     use shifted energy switching if near the cutoff distance
                                                                /// c
                                                                //                 if (r2 .gt. cut2) then
                                                                //                    e = fik / r
                                                                //                    shift = fik / (0.5d0*(off+cut))
                                                                //                    e = e - shift
                                                                //                    r3 = r2 * r
                                                                //                    r4 = r2 * r2
                                                                //                    r5 = r2 * r3
                                                                //                    r6 = r3 * r3
                                                                //                    r7 = r3 * r4
                                                                //                    taper = c5*r5 + c4*r4 + c3*r3 + c2*r2 + c1*r + c0
                                                                //                    dtaper = 5.0d0*c5*r4 + 4.0d0*c4*r3
                                                                //       &                        + 3.0d0*c3*r2 + 2.0d0*c2*r + c1
                                                                //                    d2taper = 20.0d0*c5*r3 + 12.0d0*c4*r2
                                                                //       &                         + 6.0d0*c3*r + 2.0d0*c2
                                                                //                    trans = fik * (f7*r7 + f6*r6 + f5*r5 + f4*r4
                                                                //       &                            + f3*r3 + f2*r2 + f1*r + f0)
                                                                //                    dtrans = fik * (7.0d0*f7*r6 + 6.0d0*f6*r5
                                                                //       &                            + 5.0d0*f5*r4 + 4.0d0*f4*r3
                                                                //       &                            + 3.0d0*f3*r2 + 2.0d0*f2*r + f1)
                                                                //                    d2trans = fik * (42.0d0*f7*r5 + 30.0d0*f6*r4
                                                                //       &                             + 20.0d0*f5*r3 + 12.0d0*f4*r2
                                                                //       &                             + 6.0d0*f3*r + 2.0d0*f2)
                                                                //                    d2e = e*d2taper + 2.0d0*de*dtaper
                                                                //       &                     + d2e*taper + d2trans
                                                                //                    de = e*dtaper + de*taper + dtrans
                                                                //                 end if
                                                                /// c
                                                                /// c     scale the interaction based on its group membership
                                                                /// c
                                                                //                 if (use_group) then
                                                                //                    de = de * fgrp
                                                                //                    d2e = d2e * fgrp
                                                                //                 end if
                                                                /// c
                                                                /// c     form the individual Hessian element components
                                                                /// c
                   de = de / r;                                 //                 de = de / r
                   d2e = (d2e-de) / r2;                         //                 d2e = (d2e-de) / r2
            double d2edx = d2e * xr;                            //                 d2edx = d2e * xr
            double d2edy = d2e * yr;                            //                 d2edy = d2e * yr
            double d2edz = d2e * zr;                            //                 d2edz = d2e * zr
            double term_1_1 = d2edx*xr + de;                    //                 term(1,1) = d2edx*xr + de
            double term_1_2 = d2edx*yr;                         //                 term(1,2) = d2edx*yr
            double term_1_3 = d2edx*zr;                         //                 term(1,3) = d2edx*zr
            double term_2_1 = term_1_2;                         //                 term(2,1) = term(1,2)
            double term_2_2 = d2edy*yr + de;                    //                 term(2,2) = d2edy*yr + de
            double term_2_3 = d2edy*zr;                         //                 term(2,3) = d2edy*zr
            double term_3_1 = term_1_3;                         //                 term(3,1) = term(1,3)
            double term_3_2 = term_2_3;                         //                 term(3,2) = term(2,3)
            double term_3_3 = d2edz*zr + de;                    //                 term(3,3) = d2edz*zr + de
                                                                /// c
                                                                /// c     increment diagonal and non-diagonal Hessian elements
                                                                /// c
                                                                //                 do j = 1, 3
                                                                //                    hessx(j,i) = hessx(j,i) + term(1,j)
                                                                //                    hessy(j,i) = hessy(j,i) + term(2,j)
                                                                //                    hessz(j,i) = hessz(j,i) + term(3,j)
                                                                //                    hessx(j,k) = hessx(j,k) - term(1,j)
                                                                //                    hessy(j,k) = hessy(j,k) - term(2,j)
                                                                //                    hessz(j,k) = hessz(j,k) - term(3,j)
                                                                //                 end do
                                                                //              end if
                                                                //           end if
                                                                //        end do
                                                                /// c
                                                                /// c     for periodic boundary conditions with large cutoffs
                                                                /// c     neighbors must be found by the replicates method
                                                                /// c
                                                                //        if (.not. use_replica)  return
                                                                /// c
                                                                /// c     calculate interaction energy with other unit cells
                                                                /// c
                                                                //        do kk = 1, nion
                                                                //           k = iion(kk)
                                                                //           kn = jion(kk)
                                                                //           proceed = .true.
                                                                //           if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                                /// c
                                                                /// c     compute the energy contribution for this interaction
                                                                /// c
                                                                //           if (proceed) then
                                                                //              do jcell = 1, ncell
                                                                //                 xr = xi - x(k)
                                                                //                 yr = yi - y(k)
                                                                //                 zr = zi - z(k)
                                                                //                 call imager (xr,yr,zr,jcell)
                                                                //                 r2 = xr*xr + yr*yr + zr*zr
                                                                //                 if (r2 .le. off2) then
                                                                //                    r = sqrt(r2)
                                                                //                    rb = r + ebuffer
                                                                //                    rb2 = rb * rb
                                                                //                    fik = fi * pchg(kk)
                                                                //                    if (use_polymer) then
                                                                //                       if (r2 .le. polycut2)  fik = fik * cscale(kn)
                                                                //                    end if
                                                                /// c
                                                                /// c     compute chain rule terms for Hessian matrix elements
                                                                /// c
                                                                //                    de = -fik / rb2
                                                                //                    d2e = -2.0d0 * de/rb
                                                                /// c
                                                                /// c     use shifted energy switching if near the cutoff distance
                                                                /// c
                                                                //                    if (r2 .gt. cut2) then
                                                                //                       e = fik / r
                                                                //                       shift = fik / (0.5d0*(off+cut))
                                                                //                       e = e - shift
                                                                //                       r3 = r2 * r
                                                                //                       r4 = r2 * r2
                                                                //                       r5 = r2 * r3
                                                                //                       r6 = r3 * r3
                                                                //                       r7 = r3 * r4
                                                                //                       taper = c5*r5 + c4*r4 + c3*r3 + c2*r2 + c1*r + c0
                                                                //                       dtaper = 5.0d0*c5*r4 + 4.0d0*c4*r3
                                                                //       &                           + 3.0d0*c3*r2 + 2.0d0*c2*r + c1
                                                                //                       d2taper = 20.0d0*c5*r3 + 12.0d0*c4*r2
                                                                //       &                            + 6.0d0*c3*r + 2.0d0*c2
                                                                //                       trans = fik * (f7*r7 + f6*r6 + f5*r5 + f4*r4
                                                                //       &                               + f3*r3 + f2*r2 + f1*r + f0)
                                                                //                       dtrans = fik * (7.0d0*f7*r6 + 6.0d0*f6*r5
                                                                //       &                               + 5.0d0*f5*r4 + 4.0d0*f4*r3
                                                                //       &                               + 3.0d0*f3*r2 + 2.0d0*f2*r + f1)
                                                                //                       d2trans = fik * (42.0d0*f7*r5 + 30.0d0*f6*r4
                                                                //       &                                + 20.0d0*f5*r3 + 12.0d0*f4*r2
                                                                //       &                                + 6.0d0*f3*r + 2.0d0*f2)
                                                                //                       d2e = e*d2taper + 2.0d0*de*dtaper
                                                                //       &                        + d2e*taper + d2trans
                                                                //                       de = e*dtaper + de*taper + dtrans
                                                                //                    end if
                                                                /// c
                                                                /// c     scale the interaction based on its group membership
                                                                /// c
                                                                //                    if (use_group) then
                                                                //                       de = de * fgrp
                                                                //                       d2e = d2e * fgrp
                                                                //                    end if
                                                                /// c
                                                                /// c     form the individual Hessian element components
                                                                /// c
                                                                //                    de = de / r
                                                                //                    d2e = (d2e-de) / r2
                                                                //                    d2edx = d2e * xr
                                                                //                    d2edy = d2e * yr
                                                                //                    d2edz = d2e * zr
                                                                //                    term(1,1) = d2edx*xr + de
                                                                //                    term(1,2) = d2edx*yr
                                                                //                    term(1,3) = d2edx*zr
                                                                //                    term(2,1) = term(1,2)
                                                                //                    term(2,2) = d2edy*yr + de
                                                                //                    term(2,3) = d2edy*zr
                                                                //                    term(3,1) = term(1,3)
                                                                //                    term(3,2) = term(2,3)
                                                                //                    term(3,3) = d2edz*zr + de
                                                                /// c
                                                                /// c     increment diagonal and non-diagonal Hessian elements
                                                                /// c
                                                                //                    do j = 1, 3
                                                                //                       hessx(j,i) = hessx(j,i) + term(1,j)
                                                                //                       hessy(j,i) = hessy(j,i) + term(2,j)
                                                                //                       hessz(j,i) = hessz(j,i) + term(3,j)
                                                                //                       hessx(j,k) = hessx(j,k) - term(1,j)
                                                                //                       hessy(j,k) = hessy(j,k) - term(2,j)
                                                                //                       hessz(j,k) = hessz(j,k) - term(3,j)
                                                                //                    end do
                                                                //                 end if
                                                                //              end do
                                                                //           end if
                                                                //        end do
                                                                /// c
                                                                /// c     perform deallocation of some local arrays
                                                                /// c
                                                                //        deallocate (cscale)
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
