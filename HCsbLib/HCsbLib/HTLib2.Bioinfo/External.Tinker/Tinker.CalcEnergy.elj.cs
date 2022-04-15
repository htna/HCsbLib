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
            /// c     ##############################################################
            /// c     ##                                                          ##
            /// c     ##  subroutine elj  --  Lennard-Jones van der Waals energy  ##
            /// c     ##                                                          ##
            /// c     ##############################################################
            /// c
            /// c
            /// c     "elj" calculates the Lennard-Jones 6-12 van der Waals energy
            /// c
            /// c
            /// c
            /// c
            /// c     ##################################################################
            /// c     ##                                                              ##
            /// c     ##  subroutine elj0a  --  double loop Lennard-Jones vdw energy  ##
            /// c     ##                                                              ##
            /// c     ##################################################################
            /// c
            /// c
            /// c     "elj0a" calculates the Lennard-Jones 6-12 van der Waals energy
            /// c     using a pairwise double loop
            /// c
            /// c
            public static double elj
                ( double[] coordi, Tinker.Prm.Vdw vdwi
                , double[] coordj, Tinker.Prm.Vdw vdwj
                )
            {
#pragma warning disable CS0162 // Rethrow to preserve stack details
                throw new NotImplementedException();
                                                                        /// c
                                                                        /// c
                                                                        /// c     zero out the van der Waals energy contribution
                                                                        /// c
                                                                        //        ev = 0.0d0
                                                                        /// c
                                                                        /// c     perform dynamic allocation of some local arrays
                                                                        /// c
                                                                        //        allocate (iv14(n))
                                                                        //        allocate (xred(n))
                                                                        //        allocate (yred(n))
                                                                        //        allocate (zred(n))
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
                                                                        /// c     apply any reduction factor to the atomic coordinates
                                                                        /// c
                                                                        //        do k = 1, nvdw
                                                                        //           i = ivdw(k)
                                                                        //           iv = ired(i)
                                                                        //           rdn = kred(i)
                                                                        //           xred(i) = rdn*(x(i)-x(iv)) + x(iv)
                                                                        //           yred(i) = rdn*(y(i)-y(iv)) + y(iv)
                                                                        //           zred(i) = rdn*(z(i)-z(iv)) + z(iv)
                                                                        //        end do
                                                                        /// c
                                                                        /// c     find the van der Waals energy via double loop search
                                                                        /// c
                                                                        //        do ii = 1, nvdw-1
                                                                        //           i = ivdw(ii)
                                                                        //           iv = ired(i)
                                                                        //           it = jvdw(i)
                                                                        //           xi = xred(i)
                                                                        //           yi = yred(i)
                                                                        //           zi = zred(i)
                                                                        //           usei = (use(i) .or. use(iv))
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
                                                                        //           do kk = ii+1, nvdw
                                                                        //              k = ivdw(kk)
                                                                        //              kv = ired(k)
                                                                        //              proceed = .true.
                                                                        //              if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                                        //              if (proceed)  proceed = (usei .or. use(k) .or. use(kv))
                                                                        /// c
                                                                        /// c     compute the energy contribution for this interaction
                                                                        /// c
                                                                        //              if (proceed) then
                                                                        //                 kt = jvdw(k)
                double xr = coordi[0] - coordj[0];                      //                 xr = xi - xred(k)
                double yr = coordi[1] - coordj[1];                      //                 yr = yi - yred(k)
                double zr = coordi[2] - coordj[2];                      //                 zr = zi - zred(k)
                                                                        //                 call image (xr,yr,zr)
                double rik2 = xr*xr + yr*yr + zr*zr;                    //                 rik2 = xr*xr + yr*yr + zr*zr
                                                                        /// c
                                                                        /// c     check for an interaction distance less than the cutoff
                                                                        /// c
                                                                        //                 if (rik2 .le. off2) then
                double rv  =           vdwi.Rmin2   + vdwj.Rmin2;       //                    rv = radmin(kt,it)
                double eps = Math.Sqrt(vdwi.Epsilon * vdwj.Epsilon);    //                    eps = epsilon(kt,it)
                                                                        //                    if (iv14(k) .eq. i) then
                                                                        //                       rv = radmin4(kt,it)
                                                                        //                       eps = epsilon4(kt,it)
                                                                        //                    end if
                       eps = eps * 1;                                   //                    eps = eps * vscale(k)
                double p6 = Math.Pow(rv,6) / Math.Pow(rik2,3);          //                    p6 = rv**6 / rik2**3
                double p12 = p6 * p6;                                   //                    p12 = p6 * p6
                double e = eps * (p12 - 2.0*p6);                        //                    e = eps * (p12 - 2.0d0*p6)
                                                                        /// c
                                                                        /// c     use energy switching if near the cutoff distance
                                                                        /// c
                                                                        //                    if (rik2 .gt. cut2) then
                                                                        //                       rik = sqrt(rik2)
                                                                        //                       rik3 = rik2 * rik
                                                                        //                       rik4 = rik2 * rik2
                                                                        //                       rik5 = rik2 * rik3
                                                                        //                       taper = c5*rik5 + c4*rik4 + c3*rik3
                                                                        //       &                          + c2*rik2 + c1*rik + c0
                                                                        //                       e = e * taper
                                                                        //                    end if
                                                                        /// c
                                                                        /// c     scale the interaction based on its group membership
                                                                        /// c
                                                                        //                    if (use_group)  e = e * fgrp
                                                                        /// c
                                                                        /// c     increment the overall van der Waals energy component
                                                                        /// c
                                                                        //                    ev = ev + e
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
                //////////////////////////////////////////////////////////        if (.not. use_replica)  return
                                                                        /// c
                                                                        /// c     calculate interaction energy with other unit cells
                                                                        /// c
                                                                        //        do ii = 1, nvdw
                                                                        //           i = ivdw(ii)
                                                                        //           iv = ired(i)
                                                                        //           it = jvdw(i)
                                                                        //           xi = xred(i)
                                                                        //           yi = yred(i)
                                                                        //           zi = zred(i)
                                                                        //           usei = (use(i) .or. use(iv))
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
                                                                        //           do kk = ii, nvdw
                                                                        //              k = ivdw(kk)
                                                                        //              kv = ired(k)
                                                                        //              proceed = .true.
                                                                        //              if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                                        //              if (proceed)  proceed = (usei .or. use(k) .or. use(kv))
                                                                        /// c
                                                                        /// c     compute the energy contribution for this interaction
                                                                        /// c
                                                                        //              if (proceed) then
                                                                        //                 kt = jvdw(k)
                                                                        //                 do j = 1, ncell
                                                                        //                    xr = xi - xred(k)
                                                                        //                    yr = yi - yred(k)
                                                                        //                    zr = zi - zred(k)
                                                                        //                    call imager (xr,yr,zr,j)
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
                                                                        //                       p6 = rv**6 / rik2**3
                                                                        //                       p12 = p6 * p6
                                                                        //                       e = eps * (p12 - 2.0d0*p6)
                                                                        /// c
                                                                        /// c     use energy switching if near the cutoff distance
                                                                        /// c
                                                                        //                       if (rik2 .gt. cut2) then
                                                                        //                          rik = sqrt(rik2)
                                                                        //                          rik3 = rik2 * rik
                                                                        //                          rik4 = rik2 * rik2
                                                                        //                          rik5 = rik2 * rik3
                                                                        //                          taper = c5*rik5 + c4*rik4 + c3*rik3
                                                                        //       &                             + c2*rik2 + c1*rik + c0
                                                                        //                          e = e * taper
                                                                        //                       end if
                                                                        /// c
                                                                        /// c     scale the interaction based on its group membership
                                                                        /// c
                                                                        //                       if (use_group)  e = e * fgrp
                                                                        /// c
                                                                        /// c     increment the overall van der Waals energy component;
                                                                        /// c     interaction of an atom with its own image counts half
                                                                        /// c
                                                                        //                       if (i .eq. k)  e = 0.5d0 * e
                                                                        //                       ev = ev + e
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
                                                                        //        deallocate (xred)
                                                                        //        deallocate (yred)
                                                                        //        deallocate (zred)
                                                                        //        deallocate (vscale)
                                                                        //        return
                                                                        //        end
                return e;
            }
        }
    }
}
