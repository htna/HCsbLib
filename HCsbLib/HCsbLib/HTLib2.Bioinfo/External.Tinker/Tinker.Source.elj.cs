using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public static double elj
            ( double[] coordi, Tinker.Prm.Vdw vdwi
            , double[] coordj, Tinker.Prm.Vdw vdwj
            )
        {
            throw new NotImplementedException();
                                                                    /// c
                                                                    /// c     compute the energy contribution for this interaction
                                                                    /// c
                                                                    //              if (proceed) then
                                                                    //                 kt = jvdw(k)
                                                                    //                 do j = 1, ncell
            double xr = coordi[0] - coordj[0];                      //                    xr = xi - xred(k)
            double yr = coordi[1] - coordj[1];                      //                    yr = yi - yred(k)
            double zr = coordi[2] - coordj[2];                      //                    zr = zi - zred(k)
                                                                    //                    call imager (xr,yr,zr,j)
            double rik2 = xr*xr + yr*yr + zr*zr;                    //                    rik2 = xr*xr + yr*yr + zr*zr
                                                                    /// c
                                                                    /// c     check for an interaction distance less than the cutoff
                                                                    /// c
                                                                    //                    if (rik2 .le. off2) then
            double rv  =           vdwi.Rmin2   + vdwj.Rmin2;       //                       rv = radmin(kt,it)
            double eps = Math.Sqrt(vdwi.Epsilon * vdwj.Epsilon);    //                       eps = epsilon(kt,it)
                                                                    //                       if (use_polymer) then
                                                                    //                          if (rik2 .le. polycut2) then
                                                                    //                             if (iv14(k) .eq. i) then
                                                                    //                                rv = radmin4(kt,it)
                                                                    //                                eps = epsilon4(kt,it)
                                                                    //                             end if
                                                                    //                             eps = eps * vscale(k)
                                                                    //                          end if
                                                                    //                       end if
            double p6 = Math.Pow(rv,6) / Math.Pow(rik2,3);          //                       p6 = rv**6 / rik2**3
            double p12 = p6 * p6;                                   //                       p12 = p6 * p6
            double e = eps * (p12 - 2.0*p6);                        //                       e = eps * (p12 - 2.0d0*p6)
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
            return e;
        }
    }
}
