using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public static SVector3 elj1
            ( double[] coordi, Tinker.Prm.Vdw vdwi
            , double[] coordj, Tinker.Prm.Vdw vdwj
            )
        {
                                                                                        /// c
                                                                                        /// c     compute the energy contribution for this interaction
                                                                                        /// c
                                                                                        //              if (proceed) then
                                                                                        //                 kt = jvdw(k)
                double xr = coordi[0] - coordj[0];                                      //                 xr = xi - xred(k)
                double yr = coordi[1] - coordj[1];                                      //                 yr = yi - yred(k)
                double zr = coordi[2] - coordj[2];                                      //                 zr = zi - zred(k)
                                                                                        //                 call image (xr,yr,zr)
                double rik2 = xr*xr + yr*yr + zr*zr;                                    //                 rik2 = xr*xr + yr*yr + zr*zr
                                                                                        /// c
                                                                                        /// c     check for an interaction distance less than the cutoff
                                                                                        /// c
                                                                                        //                 if (rik2 .le. off2) then
                double rv  =           vdwi.Rmin2   + vdwj.Rmin2;                       //                    rv = radmin(kt,it)
                double eps = Math.Sqrt(vdwi.Epsilon * vdwj.Epsilon);                    //                    eps = epsilon(kt,it)
                                                                                        //                    if (iv14(k) .eq. i) then
                                                                                        //                       rv = radmin4(kt,it)
                                                                                        //                       eps = epsilon4(kt,it)
                                                                                        //                    end if
                                                                                        //                    eps = eps * vscale(k)
                double rik = Math.Sqrt(rik2);                                           //                    rik = sqrt(rik2)
                double p6 = Math.Pow(rv,6) / Math.Pow(rik2,3);                          //                    p6 = rv**6 / rik2**3
                double p12 = p6 * p6;                                                   //                    p12 = p6 * p6
                double e = eps * (p12-2.0*p6);                                          //                    e = eps * (p12-2.0d0*p6)
                double de = eps * (p12-p6) * (-12.0/rik);                               //                    de = eps * (p12-p6) * (-12.0d0/rik)
                                                                                        /// c
                                                                                        /// c     use energy switching if near the cutoff distance
                                                                                        /// c
                                                                                        //                    if (rik2 .gt. cut2) then
                                                                                        //                       rik3 = rik2 * rik
                                                                                        //                       rik4 = rik2 * rik2
                                                                                        //                       rik5 = rik2 * rik3
                                                                                        //                       taper = c5*rik5 + c4*rik4 + c3*rik3
                                                                                        //       &                          + c2*rik2 + c1*rik + c0
                                                                                        //                       dtaper = 5.0d0*c5*rik4 + 4.0d0*c4*rik3
                                                                                        //       &                           + 3.0d0*c3*rik2 + 2.0d0*c2*rik + c1
                                                                                        //                       de = e*dtaper + de*taper
                                                                                        //                       e = e * taper
                                                                                        //                    end if
            /// c                                                                       /// c
            /// c     scale the interaction based on its group membership               /// c     scale the interaction based on its group membership
            /// c                                                                       /// c
                                                                                        //                    if (use_group) then
                                                                                        //                       e = e * fgrp
                                                                                        //                       de = de * fgrp
                                                                                        //                    end if
            /// c                                                                       /// c
            /// c     find the chain rule terms for derivative components               /// c     find the chain rule terms for derivative components
            /// c                                                                       /// c
                double de = de / rik;                                                   //                    de = de / rik
                double dedx = de * xr;                                                  //                    dedx = de * xr
                double dedy = de * yr;                                                  //                    dedy = de * yr
                double dedz = de * zr;                                                  //                    dedz = de * zr
                                                                                        /// c
                                                                                        /// c     increment the total van der Waals energy and derivatives
                                                                                        /// c
                                                                                        //                    ev = ev + e
                                                                                        //                    if (i .eq. iv) then
                                                                                        //                       dev(1,i) = dev(1,i) + dedx
                                                                                        //                       dev(2,i) = dev(2,i) + dedy
                                                                                        //                       dev(3,i) = dev(3,i) + dedz
                                                                                        //                    else
                                                                                        //                       dev(1,i) = dev(1,i) + dedx*redi
                                                                                        //                       dev(2,i) = dev(2,i) + dedy*redi
                                                                                        //                       dev(3,i) = dev(3,i) + dedz*redi
                                                                                        //                       dev(1,iv) = dev(1,iv) + dedx*rediv
                                                                                        //                       dev(2,iv) = dev(2,iv) + dedy*rediv
                                                                                        //                       dev(3,iv) = dev(3,iv) + dedz*rediv
                                                                                        //                    end if
                                                                                        //                    if (k .eq. kv) then
                                                                                        //                       dev(1,k) = dev(1,k) - dedx
                                                                                        //                       dev(2,k) = dev(2,k) - dedy
                                                                                        //                       dev(3,k) = dev(3,k) - dedz
                                                                                        //                    else
                                                                                        //                       redk = kred(k)
                                                                                        //                       redkv = 1.0d0 - redk
                                                                                        //                       dev(1,k) = dev(1,k) - dedx*redk
                                                                                        //                       dev(2,k) = dev(2,k) - dedy*redk
                                                                                        //                       dev(3,k) = dev(3,k) - dedz*redk
                                                                                        //                       dev(1,kv) = dev(1,kv) - dedx*redkv
                                                                                        //                       dev(2,kv) = dev(2,kv) - dedy*redkv
                                                                                        //                       dev(3,kv) = dev(3,kv) - dedz*redkv
                                                                                        //                    end if
                                                                                        /// c
                                                                                        /// c     increment the internal virial tensor components
                                                                                        /// c
                                                                                        //                    vxx = xr * dedx
                                                                                        //                    vyx = yr * dedx
                                                                                        //                    vzx = zr * dedx
                                                                                        //                    vyy = yr * dedy
                                                                                        //                    vzy = zr * dedy
                                                                                        //                    vzz = zr * dedz
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
            return new SVector3(dedx, dedy, dedz);
        }
    }
}
