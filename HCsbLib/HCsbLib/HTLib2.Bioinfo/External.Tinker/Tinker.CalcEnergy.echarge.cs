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
                                                    /// c     set array needed to scale connected atom interactions
                                                    /// c
                                                    //        do i = 1, n
                                                    //           cscale(i) = 1.0d0
                                                    //        end do
                                                    /// c
                                                    /// c     set conversion factor, cutoff and switching coefficients
                                                    /// c
            double f = electric / dielec;           //        f = electric / dielec
                                                    //        mode = 'CHARGE'
                                                    //        call switch (mode)
                                                    //////////////////////////////////////////////////////////////////////////////
                                                    /// c
                                                    /// c     calculate the charge interaction energy term
                                                    /// c
                                                    //        do ii = 1, nion-1
                                                    //           i = iion(ii)
                                                    //           in = jion(ii)
                                                    //           ic = kion(ii)
                                                    //           xic = x(ic)
                                                    //           yic = y(ic)
                                                    //           zic = z(ic)
                                                    //           xi = x(i) - xic
                                                    //           yi = y(i) - yic
                                                    //           zi = z(i) - zic
            double fi = f * chgi.pch;               //           fi = f * pchg(ii)
                                                    //           usei = (use(i) .or. use(ic))
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
                                                    /// c     compute the energy contribution for this interaction
                                                    /// c
                                                    //              if (proceed) then
                                                    //                 xc = xic - x(kc)
                                                    //                 yc = yic - y(kc)
                                                    //                 zc = zic - z(kc)
                                                    //                 if (use_bounds)  call image (xc,yc,zc)
                                                    //                 rc2 = xc*xc + yc*yc + zc*zc
                                                    //                 if (rc2 .le. off2) then
            double xr = coordi[0] - coordj[0];      //                    xr = xc + xi - x(k) + x(kc)
            double yr = coordi[1] - coordj[1];      //                    yr = yc + yi - y(k) + y(kc)
            double zr = coordi[2] - coordj[2];      //                    zr = zc + zi - z(k) + z(kc)
            double r2 = xr*xr + yr*yr + zr*zr;      //                    r2 = xr*xr + yr*yr + zr*zr
            double r = Math.Sqrt(r2);               //                    r = sqrt(r2)
            double rb = r + ebuffer;                //                    rb = r + ebuffer
            double fik = fi * chgj.pch * 1;         //                    fik = fi * pchg(kk) * cscale(kn)
            double e = fik / rb;                    //                    e = fik / rb
                                                    /// c
                                                    /// c     use shifted energy switching if near the cutoff distance
                                                    /// c
                                                    //                    shift = fik / (0.5d0*(off+cut))
                                                    //                    e = e - shift
                                                    //                    if (rc2 .gt. cut2) then
                                                    //                       rc = sqrt(rc2)
                                                    //                       rc3 = rc2 * rc
                                                    //                       rc4 = rc2 * rc2
                                                    //                       rc5 = rc2 * rc3
                                                    //                       rc6 = rc3 * rc3
                                                    //                       rc7 = rc3 * rc4
                                                    //                       taper = c5*rc5 + c4*rc4 + c3*rc3
                                                    //       &                          + c2*rc2 + c1*rc + c0
                                                    //                       trans = fik * (f7*rc7 + f6*rc6 + f5*rc5 + f4*rc4
                                                    //       &                               + f3*rc3 + f2*rc2 + f1*rc + f0)
                                                    //                       e = e*taper + trans
                                                    //                    end if
                                                    /// c
                                                    /// c     scale the interaction based on its group membership
                                                    /// c
                                                    //                    if (use_group)  e = e * fgrp
                                                    /// c
                                                    /// c     increment the overall charge-charge energy component
                                                    /// c
                                                    //                    ec = ec + e
                                                    //                 end if
                                                    //              end if
            return e;
        }
    }
}
