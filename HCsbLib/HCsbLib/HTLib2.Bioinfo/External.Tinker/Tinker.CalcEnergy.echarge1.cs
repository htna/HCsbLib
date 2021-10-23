using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        /// c     ###################################################
        /// c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
        /// c     ##              All Rights Reserved              ##
        /// c     ###################################################
        /// c
        /// c     ##############################################################
        /// c     ##                                                          ##
        /// c     ##  subroutine echarge1  --  charge-charge energy & derivs  ##
        /// c     ##                                                          ##
        /// c     ##############################################################
        /// c
        /// c
        /// c     "echarge1" calculates the charge-charge interaction energy
        /// c     and first derivatives with respect to Cartesian coordinates
        /// c
        /// c
        /// c
        /// c
        /// c     #################################################################
        /// c     ##                                                             ##
        /// c     ##  subroutine echarge1c  --  charge derivs via neighbor list  ##
        /// c     ##                                                             ##
        /// c     #################################################################
        /// c
        /// c
        /// c     "echarge1c" calculates the charge-charge interaction energy
        /// c     and first derivatives with respect to Cartesian coordinates
        /// c     using a pairwise neighbor list
        /// c
        /// c
        /// c
        public static SVector3 echarge1
            ( double[] coordi, Tinker.Prm.Charge chgi
            , double[] coordj, Tinker.Prm.Charge chgj
            )
        {
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
                                                    /// c     zero out the charge interaction energy and derivatives
                                                    /// c
                                                    //        ec = 0.0d0
                                                    //        do i = 1, n
                                                    //           dec(1,i) = 0.0d0
                                                    //           dec(2,i) = 0.0d0
                                                    //           dec(3,i) = 0.0d0
                                                    //        end do
                                                    //        if (nion .eq. 0)  return
                                                    /// c
                                                    /// c     set array needed to scale connected atom interactions
                                                    /// c
                                                    //        do i = 1, n
                                                    //           cscale(i) = 1.0d0
                                                    //        end do
                                                    /// c
                                                    /// c     set conversion factor, cutoff and switching coefficients
                                                    /// c
            const double f = electric / dielec;     //        f = electric / dielec
                                                    //        mode = 'CHARGE'
                                                    //        call switch (mode)
                                                    /// c
                                                    /// c     compute the charge interaction energy and first derivatives
                                                    /// c
                                                    //        do ii = 1, nion
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
                                                    /// c     decide whether to compute the current interaction
                                                    /// c
                                                    //           do kkk = 1, nelst(ii)
                                                    //              kk = elst(kkk,ii)
                                                    //              k = iion(kk)
                                                    //              kn = jion(kk)
                                                    //              kc = kion(kk)
                                                    //              proceed = .true.
                                                    //              if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
                                                    //              if (proceed)  proceed = (usei .or. use(k) .or. use(kc))
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
            double rb2 = rb * rb;                   //                    rb2 = rb * rb
            double fik = fi * chgj.pch * 1;         //                    fik = fi * pchg(kk) * cscale(kn)
            double e = fik / rb ;                   //                    e = fik / rb
            double de = -fik / rb2 ;                //                    de = -fik / rb2
                                                    //                    dc = 0.0d0
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
                                                    //                       dtaper = 5.0d0*c5*rc4 + 4.0d0*c4*rc3
                                                    //       &                           + 3.0d0*c3*rc2 + 2.0d0*c2*rc + c1
                                                    //                       trans = fik * (f7*rc7 + f6*rc6 + f5*rc5 + f4*rc4
                                                    //       &                               + f3*rc3 + f2*rc2 + f1*rc + f0)
                                                    //                       dtrans = fik * (7.0d0*f7*rc6 + 6.0d0*f6*rc5
                                                    //       &                               + 5.0d0*f5*rc4 + 4.0d0*f4*rc3
                                                    //       &                             + 3.0d0*f3*rc2 + 2.0d0*f2*rc + f1)
                                                    //                       dc = (e*dtaper + dtrans) / rc
                                                    //                       de = de * taper
                                                    //                       e = e*taper + trans
                                                    //                    end if
                                                    /// c
                                                    /// c     scale the interaction based on its group membership
                                                    /// c
                                                    //                    if (use_group) then
                                                    //                       e = e * fgrp
                                                    //                       de = de * fgrp
                                                    //                       dc = dc * fgrp
                                                    //                    end if
                                                    /// c
                                                    /// c     form the chain rule terms for derivative expressions
                                                    /// c
                   de = de / r;                     //                    de = de / r
            double dedx = de * xr;                  //                    dedx = de * xr
            double dedy = de * yr;                  //                    dedy = de * yr
            double dedz = de * zr;                  //                    dedz = de * zr
                                                    //                    dedxc = dc * xc
                                                    //                    dedyc = dc * yc
                                                    //                    dedzc = dc * zc
                                                    /// c
                                                    /// c     increment the overall energy and derivative expressions
                                                    /// c
                                                    //                    ec = ec + e
                                                    //                    dec(1,i) = dec(1,i) + dedx
                                                    //                    dec(2,i) = dec(2,i) + dedy
                                                    //                    dec(3,i) = dec(3,i) + dedz
                                                    //                    dec(1,ic) = dec(1,ic) + dedxc
                                                    //                    dec(2,ic) = dec(2,ic) + dedyc
                                                    //                    dec(3,ic) = dec(3,ic) + dedzc
                                                    //                    dec(1,k) = dec(1,k) - dedx
                                                    //                    dec(2,k) = dec(2,k) - dedy
                                                    //                    dec(3,k) = dec(3,k) - dedz
                                                    //                    dec(1,kc) = dec(1,kc) - dedxc
                                                    //                    dec(2,kc) = dec(2,kc) - dedyc
                                                    //                    dec(3,kc) = dec(3,kc) - dedzc
                                                    /// c
                                                    /// c     increment the internal virial tensor components
                                                    /// c
                                                    //                    vxx = xr*dedx + xc*dedxc
                                                    //                    vyx = yr*dedx + yc*dedxc
                                                    //                    vzx = zr*dedx + zc*dedxc
                                                    //                    vyy = yr*dedy + yc*dedyc
                                                    //                    vzy = zr*dedy + zc*dedyc
                                                    //                    vzz = zr*dedz + zc*dedzc
                                                    //                    vir(1,1) = vir(1,1) + vxx
                                                    //                    vir(2,1) = vir(2,1) + vyx
                                                    //                    vir(3,1) = vir(3,1) + vzx
                                                    //                    vir(1,2) = vir(1,2) + vyx
                                                    //                    vir(2,2) = vir(2,2) + vyy
                                                    //                    vir(3,2) = vir(3,2) + vzy
                                                    //                    vir(1,3) = vir(1,3) + vzx
                                                    //                    vir(2,3) = vir(2,3) + vzy
                                                    //                    vir(3,3) = vir(3,3) + vzz
                                                    /// c
                                                    /// c     increment the total intermolecular energy
                                                    /// c
                                                    //                    if (molcule(i) .ne. molcule(k)) then
                                                    //                       einter = einter + e
                                                    //                    end if
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
                                                    /// c     perform deallocation of some local arrays
                                                    /// c
                                                    //        deallocate (cscale)
                                                    //        return
                                                    //        end
            return new SVector3 (dedx, dedy, dedz);
        }
    }
}
