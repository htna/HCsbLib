using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public static SMatrix3x3 elj2
            ( double[] coordi, Tinker.Prm.Vdw vdwi
            , double[] coordj, Tinker.Prm.Vdw vdwj
            )
        {
                                                                /// c
                                                                /// c     compute the Hessian elements for this interaction
                                                                /// c
                                                                //              if (proceed) then
                                                                //                 kt = jvdw(k)
            double xr = coordi[0] - coordj[0];                  //                 xr = xi - xred(k)
            double yr = coordi[1] - coordj[1];                  //                 yr = yi - yred(k)
            double zr = coordi[2] - coordj[2];                  //                 zr = zi - zred(k)
                                                                //                 call image (xr,yr,zr)
            double rik2 = (xr*xr + yr*yr + zr*zr);              //                 rik2 = xr*xr + yr*yr + zr*zr
                                                                /// c
                                                                /// c     check for an interaction distance less than the cutoff
                                                                /// c
                                                                //                 if (rik2 .le. off2) then
            double rv  =           vdwi.Rmin2   + vdwj.Rmin2;   //                    rv = radmin(kt,it)
            double eps = Math.Sqrt(vdwi.Epsilon * vdwj.Epsilon);//                    eps = epsilon(kt,it)
                                                                //                    if (iv14(k) .eq. i) then
                                                                //                       rv = radmin4(kt,it)
                                                                //                       eps = epsilon4(kt,it)
                                                                //                    end if
                                                                //                    eps = eps * vscale(k)
            double rik = Math.Sqrt(rik2);                       //                    rik = sqrt(rik2)
            double p6 = Math.Pow(rv,6) / Math.Pow(rik2,3);      //                    p6 = rv**6 / rik2**3
            double p12 = p6 * p6;                               //                    p12 = p6 * p6
            double de = eps * (p12-p6) * (-12.0/rik);           //                    de = eps * (p12-p6) * (-12.0d0/rik)
            double d2e = eps * (13.0*p12-7.0*p6) * (12.0/rik2); //                    d2e = eps * (13.0d0*p12-7.0d0*p6) * (12.0d0/rik2)
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
                    de = de / rik            ;                   //                    de = de / rik
                    d2e = (d2e-de) / rik2    ;                   //                    d2e = (d2e-de) / rik2
            double d2edx = d2e * xr         ;                   //                    d2edx = d2e * xr
            double d2edy = d2e * yr         ;                   //                    d2edy = d2e * yr
            double d2edz = d2e * zr         ;                   //                    d2edz = d2e * zr
            double term_1_1 = d2edx*xr + de ;                   //                    term(1,1) = d2edx*xr + de
            double term_1_2 = d2edx*yr      ;                   //                    term(1,2) = d2edx*yr
            double term_1_3 = d2edx*zr      ;                   //                    term(1,3) = d2edx*zr
            double term_2_1 = term_1_2      ;                   //                    term(2,1) = term(1,2)
            double term_2_2 = d2edy*yr + de ;                   //                    term(2,2) = d2edy*yr + de
            double term_2_3 = d2edy*zr      ;                   //                    term(2,3) = d2edy*zr
            double term_3_1 = term_1_3      ;                   //                    term(3,1) = term(1,3)
            double term_3_2 = term_2_3      ;                   //                    term(3,2) = term(2,3)
            double term_3_3 = d2edz*zr + de ;                   //                    term(3,3) = d2edz*zr + de

            return new SMatrix3x3
                ( term_1_1, term_1_2, term_1_3
                , term_2_1, term_2_2, term_2_3
                , term_3_1, term_3_2, term_3_3
                );
        }
    }
}
