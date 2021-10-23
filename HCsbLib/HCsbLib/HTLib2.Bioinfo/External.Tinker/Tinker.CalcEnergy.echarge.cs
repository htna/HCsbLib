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
            /// c     ##  subroutine echarge  --  charge-charge potential energy  ##
            /// c     ##                                                          ##
            /// c     ##############################################################
            /// c
            /// c
            /// c     "echarge" calculates the charge-charge interaction energy
            /// c
            /// c
            /// c
            /// c
            /// c     #################################################################
            /// c     ##                                                             ##
            /// c     ##  subroutine echarge0d  --  double loop Ewald charge energy  ##
            /// c     ##                                                             ##
            /// c     #################################################################
            /// c
            /// c
            /// c     "echarge0d" calculates the charge-charge interaction energy
            /// c     using a particle mesh Ewald summation
            /// c
            /// c
            public static double echarge
                ( double[] coordi, Tinker.Prm.Charge chgi
                , double[] coordj, Tinker.Prm.Charge chgj
                )
            {
                throw new NotImplementedException();
                                                        /// unit.i
                const double coulomb = 332.063714;      //        parameter (coulomb=332.063714d0)
                                                        /// initprm.f
                                                        /// c
                                                        /// c     set default control parameters for charge-charge terms
                                                        /// c
                const double electric = coulomb;        //        electric = coulomb
                const double dielec = 1.0;              //        dielec = 1.0d0
                const double ebuffer = 0.0;             //        ebuffer = 0.0d0
                                                        //        c2scale = 0.0d0
                                                        //        c3scale = 0.0d0
                                                        //        c4scale = 1.0d0
                                                        //        c5scale = 1.0d0
                                                        //        neutnbr = .false.
                                                        //        neutcut = .false.
                                                        //////////////////////////////////////////////////////////////////////////////
                                                        /// c
                                                        /// c
                                                        /// c     zero out the Ewald charge interaction energy
                                                        /// c
                                                        //        ec = 0.0d0
                                                        //        if (nion .eq. 0)  return
                                                        /// c
                                                        /// c     perform dynamic allocation of some local arrays
                                                        /// c
                                                        //        allocate (cscale(n))
                                                        /// c
                                                        /// c     set array needed to scale connected atom interactions
                                                        /// c
                                                        //        do i = 1, n
                                                        //           cscale(i) = 1.0d0
                                                        //        end do
                                                        /// c
                                                        /// c     set conversion factor, cutoff and switching coefficients
                                                        /// c
                double f = electric / dielec;           //        f = electric / dielec
                                                        //        mode = 'EWALD'
                                                        //        call switch (mode)
                                                        /// c
                                                        /// c     compute the Ewald self-energy term over all the atoms
                                                        /// c
                                                        //        fs = -f * aewald / sqrtpi
                                                        //        do ii = 1, nion
                                                        //           e = fs * pchg(ii)**2
                                                        //           ec = ec + e
                                                        //        end do
                                                        /// c
                                                        /// c     compute the cell dipole boundary correction term
                                                        /// c
                                                        //        if (boundary .eq. 'VACUUM') then
                                                        //           xd = 0.0d0
                                                        //           yd = 0.0d0
                double fi = f * chgi.pch;               //           zd = 0.0d0
                                                        //           do ii = 1, nion
                                                        //              i = iion(ii)
                                                        //              xd = xd + pchg(ii)*x(i)
                                                        //              yd = yd + pchg(ii)*y(i)
                                                        //              zd = zd + pchg(ii)*z(i)
                                                        //           end do
                                                        //           e = (2.0d0/3.0d0) * f * (pi/volbox) * (xd*xd+yd*yd+zd*zd)
                                                        //           ec = ec + e
                                                        //        end if
                                                        /// c
                                                        /// c     compute the reciprocal space part of the Ewald summation
                                                        /// c
                                                        //        call ecrecip
                                                        /// c
                                                        /// c     compute the real space portion of the Ewald summation
                                                        /// c
                                                        //        do ii = 1, nion-1
                                                        //           i = iion(ii)
                                                        //           in = jion(ii)
                                                        //           usei = use(i)
                                                        //           xi = x(i)
                                                        //           yi = y(i)
                                                        //           zi = z(i)
                                                        //           fi = f * pchg(ii)
                                                        /// c
                                                        /// c     set interaction scaling coefficients for connected atoms
                                                        /// c
                                                        //           do j = 1, n12(in)
                                                        //              cscale(i12(j,in)) = c2scale
                                                        //           end do
                                                        //           do j = 1, n13(in)
                                                        //              cscale(i13(j,in)) = c3scale
                                                        //           end do
                                                        //           do j = 1, n14(in)
                                                        //              cscale(i14(j,in)) = c4scale
                                                        //           end do
                                                        //           do j = 1, n15(in)
                                                        //              cscale(i15(j,in)) = c5scale
                                                        //           end do
                                                        /// c
                                                        /// c     decide whether to compute the current interaction
                                                        /// c
                                                        //           do kk = ii+1, nion
                                                        //              k = iion(kk)
                                                        //              kn = jion(kk)
                                                        //              if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                        //              proceed = .true.
                                                        //              if (proceed)  proceed = (usei .or. use(k))
                                                        /// c
                                                        /// c     compute the energy contribution for this interaction
                                                        /// c
                                                        //              if (proceed) then
                double xr = coordi[0] - coordj[0];      //                 xr = xi - x(k)
                double yr = coordi[1] - coordj[1];      //                 yr = yi - y(k)
                double zr = coordi[2] - coordj[2];      //                 zr = zi - z(k)
                                                        //                 call image (xr,yr,zr)
                double r2 = xr*xr + yr*yr + zr*zr;      //                 r2 = xr*xr + yr*yr + zr*zr
                                                        //                 if (r2 .le. off2) then
                double r = Math.Sqrt(r2);               //                    r = sqrt(r2)
                double rb = r + ebuffer;                //                    rb = r + ebuffer
                double fik = fi * chgj.pch;             //                    fik = fi * pchg(kk)
                                                        //                    rew = aewald * r
                                                        //                    erfterm = erfc (rew)
                                                        //                    scale = cscale(kn)
                                                        //                    if (use_group)  scale = scale * fgrp
                                                        //                    scaleterm = scale - 1.0d0
                double e = fik / rb;                    //                    e = (fik/rb) * (erfterm+scaleterm)
                                                        /// c
                                                        /// c     increment the overall charge-charge energy component
                                                        /// c
                                                        //                    ec = ec + e
                                                        //                 end if
                                                        //              end if
                                                        //           end do
                                                        /// c
                                                        /// c     reset interaction scaling coefficients for connected atoms
                                                        /// c
                                                        //           do j = 1, n12(in)
                                                        //              cscale(i12(j,in)) = 1.0d0
                                                        //           end do
                                                        //           do j = 1, n13(in)
                                                        //              cscale(i13(j,in)) = 1.0d0
                                                        //           end do
                                                        //           do j = 1, n14(in)
                                                        //              cscale(i14(j,in)) = 1.0d0
                                                        //           end do
                                                        //           do j = 1, n15(in)
                                                        //              cscale(i15(j,in)) = 1.0d0
                                                        //           end do
                                                        //        end do
                                                        /// c
                                                        /// c     for periodic boundary conditions with large cutoffs
                                                        /// c     neighbors must be found by the replicates method
                                                        /// c
                //////////////////////////////////////////        if (.not. use_replica)  return
                                                        /// c
                                                        /// c     calculate real space portion involving other unit cells
                                                        /// c
                                                        //        do ii = 1, nion
                                                        //           i = iion(ii)
                                                        //           in = jion(ii)
                                                        //           usei = use(i)
                                                        //           xi = x(i)
                                                        //           yi = y(i)
                                                        //           zi = z(i)
                                                        //           fi = f * pchg(ii)
                                                        /// c
                                                        /// c     set interaction scaling coefficients for connected atoms
                                                        /// c
                                                        //           do j = 1, n12(in)
                                                        //              cscale(i12(j,in)) = c2scale
                                                        //           end do
                                                        //           do j = 1, n13(in)
                                                        //              cscale(i13(j,in)) = c3scale
                                                        //           end do
                                                        //           do j = 1, n14(in)
                                                        //              cscale(i14(j,in)) = c4scale
                                                        //           end do
                                                        //           do j = 1, n15(in)
                                                        //              cscale(i15(j,in)) = c5scale
                                                        //           end do
                                                        /// c
                                                        /// c     decide whether to compute the current interaction
                                                        /// c
                                                        //           do kk = ii, nion
                                                        //              k = iion(kk)
                                                        //              kn = jion(kk)
                                                        //              if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                        //              proceed = .true.
                                                        //              if (proceed)  proceed = (usei .or. use(k))
                                                        /// c
                                                        /// c     compute the energy contribution for this interaction
                                                        /// c
                                                        //              if (proceed) then
                                                        //                 do j = 1, ncell
                                                        //                    xr = xi - x(k)
                                                        //                    yr = yi - y(k)
                                                        //                    zr = zi - z(k)
                                                        //                    call imager (xr,yr,zr,j)
                                                        //                    r2 = xr*xr + yr*yr + zr*zr
                                                        //                    if (r2 .le. off2) then
                                                        //                       r = sqrt(r2)
                                                        //                       rb = r + ebuffer
                                                        //                       fik = fi * pchg(kk)
                                                        //                       rew = aewald * r
                                                        //                       erfterm = erfc (rew)
                                                        //                       scale = 1.0d0
                                                        //                       if (use_group)  scale = scale * fgrp
                                                        //                       if (use_polymer) then
                                                        //                          if (r2 .le. polycut2) then
                                                        //                             scale = scale * cscale(kn)
                                                        //                          end if
                                                        //                       end if
                                                        //                       scaleterm = scale - 1.0d0
                                                        //                       e = (fik/rb) * (erfterm+scaleterm)
                                                        /// c
                                                        /// c     increment the overall charge-charge energy component;
                                                        /// c     interaction of an atom with its own image counts half
                                                        /// c
                                                        //                       if (i .eq. k)  e = 0.5d0 * e
                                                        //                       ec = ec + e
                                                        //                    end if
                                                        //                 end do
                                                        //              end if
                                                        //           end do
                                                        /// c
                                                        /// c     reset interaction scaling coefficients for connected atoms
                                                        /// c
                                                        //           do j = 1, n12(in)
                                                        //              cscale(i12(j,in)) = 1.0d0
                                                        //           end do
                                                        //           do j = 1, n13(in)
                                                        //              cscale(i13(j,in)) = 1.0d0
                                                        //           end do
                                                        //           do j = 1, n14(in)
                                                        //              cscale(i14(j,in)) = 1.0d0
                                                        //           end do
                                                        //           do j = 1, n15(in)
                                                        //              cscale(i15(j,in)) = 1.0d0
                                                        //           end do
                                                        //        end do
                                                        /// c
                                                        /// c     perform deallocation of some local arrays
                                                        /// c
                                                        //        deallocate (cscale)
                                                        //        return
                                                        //        end
                                                        //  
                return e;
            }
        }                
    }
}
