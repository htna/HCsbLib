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
///     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ################################################################
///     ##                                                            ##
///     ##  subroutine image  --  compute the minimum image distance  ##
///     ##                                                            ##
///     ################################################################
///
///
///     "image" takes the components of pairwise distance between
///     two points in a periodic box and converts to the components
///     of the minimum image distance
///
///
    public void image(ref double xr, ref double yr, ref double zr) {    ;//      subroutine image (xr,yr,zr)
                                                                        ;//      implicit none
                                                                        ;//      include 'sizes.i'
                                                                        ;//      include 'boxes.i'
                                                                        ;//      include 'cell.i'
                                                                        ;//      real*8 xr,yr,zr
///
///
///     for orthogonal lattice, find the desired image directly
///
      if (orthogonal) {                                     ;//      if (orthogonal) then
         while (abs(xr)  >   xcell2) {                      ;//         do while (abs(xr) .gt. xcell2)
            xr = xr - sign(xcell,xr)                        ;//            xr = xr - sign(xcell,xr)
         }                                                  ;//         end do
         while (abs(yr)  >   ycell2) {                      ;//         do while (abs(yr) .gt. ycell2)
            yr = yr - sign(ycell,yr)                        ;//            yr = yr - sign(ycell,yr)
         }                                                  ;//         end do
         while (abs(zr)  >   zcell2) {                      ;//         do while (abs(zr) .gt. zcell2)
            zr = zr - sign(zcell,zr)                        ;//            zr = zr - sign(zcell,zr)
         }                                                  ;//         end do
///
///     for monoclinic lattice, convert "xr" and "zr" to
///     fractional coordinates, find desired image and then
///     translate fractional coordinates back to Cartesian
///
      } else if (monoclinic) {                              ;//      else if (monoclinic) then
         zr = zr / beta_sin                                 ;//         zr = zr / beta_sin
         xr = xr - zr*beta_cos                              ;//         xr = xr - zr*beta_cos
         while (abs(xr)  >   xcell2) {                      ;//         do while (abs(xr) .gt. xcell2)
            xr = xr - sign(xcell,xr)                        ;//            xr = xr - sign(xcell,xr)
         }                                                  ;//         end do
         while (abs(yr)  >   ycell2) {                      ;//         do while (abs(yr) .gt. ycell2)
            yr = yr - sign(ycell,yr)                        ;//            yr = yr - sign(ycell,yr)
         }                                                  ;//         end do
         while (abs(zr)  >   zcell2) {                      ;//         do while (abs(zr) .gt. zcell2)
            zr = zr - sign(zcell,zr)                        ;//            zr = zr - sign(zcell,zr)
         }                                                  ;//         end do
         xr = xr + zr*beta_cos                              ;//         xr = xr + zr*beta_cos
         zr = zr * beta_sin                                 ;//         zr = zr * beta_sin
///
///     for triclinic lattice, convert pairwise components to
///     fractional coordinates, find desired image and then
///     translate fractional coordinates back to Cartesian
///
      } else if (triclinic) {                               ;//      else if (triclinic) then
         zr = zr / gamma_term                               ;//         zr = zr / gamma_term
         yr = (yr - zr*beta_term) / gamma_sin               ;//         yr = (yr - zr*beta_term) / gamma_sin
         xr = xr - yr*gamma_cos - zr*beta_cos               ;//         xr = xr - yr*gamma_cos - zr*beta_cos
         while (abs(xr)  >   xcell2) {                      ;//         do while (abs(xr) .gt. xcell2)
            xr = xr - sign(xcell,xr)                        ;//            xr = xr - sign(xcell,xr)
         }                                                  ;//         end do
         while (abs(yr)  >   ycell2) {                      ;//         do while (abs(yr) .gt. ycell2)
            yr = yr - sign(ycell,yr)                        ;//            yr = yr - sign(ycell,yr)
         }                                                  ;//         end do
         while (abs(zr)  >   zcell2) {                      ;//         do while (abs(zr) .gt. zcell2)
            zr = zr - sign(zcell,zr)                        ;//            zr = zr - sign(zcell,zr)
         }                                                  ;//         end do
         xr = xr + yr*gamma_cos + zr*beta_cos               ;//         xr = xr + yr*gamma_cos + zr*beta_cos
         yr = yr*gamma_sin + zr*beta_term                   ;//         yr = yr*gamma_sin + zr*beta_term
         zr = zr * gamma_term                               ;//         zr = zr * gamma_term
///
///     for truncated octahedron, use orthogonal box equations,
///     then perform extra tests to remove corner pieces
///
      } else if (octahedron) {                              ;//      else if (octahedron) then
         while (abs(xr)  >   xbox2) {                       ;//         do while (abs(xr) .gt. xbox2)
            xr = xr - sign(xbox,xr)                         ;//            xr = xr - sign(xbox,xr)
         }                                                  ;//         end do
         while (abs(yr)  >   ybox2) {                       ;//         do while (abs(yr) .gt. ybox2)
            yr = yr - sign(ybox,yr)                         ;//            yr = yr - sign(ybox,yr)
         }                                                  ;//         end do
         while (abs(zr)  >   zbox2) {                       ;//         do while (abs(zr) .gt. zbox2)
            zr = zr - sign(zbox,zr)                         ;//            zr = zr - sign(zbox,zr)
         }                                                  ;//         end do
         if (abs(xr)+abs(yr)+abs(zr)  >   box34) {          ;//         if (abs(xr)+abs(yr)+abs(zr) .gt. box34) then
            xr = xr - sign(xbox2,xr)                        ;//            xr = xr - sign(xbox2,xr)
            yr = yr - sign(ybox2,yr)                        ;//            yr = yr - sign(ybox2,yr)
            zr = zr - sign(zbox2,zr)                        ;//            zr = zr - sign(zbox2,zr)
         }                                                  ;//         end if
      }                                                     ;//      end if
      return                                                ;//      return
    }                                                        //      end
///
///
///     ###############################################################
///     ##                                                           ##
///     ##  subroutine imager  --  replicate minimum image distance  ##
///     ##                                                           ##
///     ###############################################################
///
///
///     "imager" takes the components of pairwise distance between
///     two points in the same or neighboring periodic boxes and
///     converts to the components of the minimum image distance
///
///
    public void imager(ref double xr, ref double yr, ref double zr, int i) {    ;//      subroutine imager (xr,yr,zr,i)
                                                                                ;//      implicit none
                                                                                ;//      include 'sizes.i'
                                                                                ;//      include 'boxes.i'
                                                                                ;//      include 'cell.i'
                                                                                ;//      integer i
                                                                                ;//      real*8 xr,yr,zr
      double xsize,ysize,zsize                                                  ;//      real*8 xsize,ysize,zsize
      double xsize2,ysize2,zsize2                                               ;//      real*8 xsize2,ysize2,zsize2
      double xmove,ymove,zmove                                                  ;//      real*8 xmove,ymove,zmove
///
///
///     set dimensions for either single box or replicated cell
///
      if (i  >=  0) {                                               ;//      if (i .ge. 0) then
         xsize = xcell                                              ;//         xsize = xcell
         ysize = ycell                                              ;//         ysize = ycell
         zsize = zcell                                              ;//         zsize = zcell
         xsize2 = xcell2                                            ;//         xsize2 = xcell2
         ysize2 = ycell2                                            ;//         ysize2 = ycell2
         zsize2 = zcell2                                            ;//         zsize2 = zcell2
      } else {                                                      ;//      else
         xsize = xbox                                               ;//         xsize = xbox
         ysize = ybox                                               ;//         ysize = ybox
         zsize = zbox                                               ;//         zsize = zbox
         xsize2 = xbox2                                             ;//         xsize2 = xbox2
         ysize2 = ybox2                                             ;//         ysize2 = ybox2
         zsize2 = zbox2                                             ;//         zsize2 = zbox2
      }                                                             ;//      end if
///
///     compute the distance to translate along each cell axis
///
      if (i  <=  0) {                                               ;//      if (i .le. 0) then
         xmove = 0.0e0                                              ;//         xmove = 0.0d0
         ymove = 0.0e0                                              ;//         ymove = 0.0d0
         zmove = 0.0e0                                              ;//         zmove = 0.0d0
      } else {                                                      ;//      else
         xmove = icell[1,i] * xbox                                  ;//         xmove = icell(1,i) * xbox
         ymove = icell[2,i] * ybox                                  ;//         ymove = icell(2,i) * ybox
         zmove = icell[3,i] * zbox                                  ;//         zmove = icell(3,i) * zbox
      }                                                             ;//      end if
///
///     for orthogonal lattice, find the desired image directly
///
      if (orthogonal) {                                             ;//      if (orthogonal) then
         xr = xr + xmove                                            ;//         xr = xr + xmove
         while (abs(xr)  >   xsize2) {                              ;//         do while (abs(xr) .gt. xsize2)
            xr = xr - sign(xsize,xr)                                ;//            xr = xr - sign(xsize,xr)
         }                                                          ;//         end do
         yr = yr + ymove                                            ;//         yr = yr + ymove
         while (abs(yr)  >   ysize2) {                              ;//         do while (abs(yr) .gt. ysize2)
            yr = yr - sign(ysize,yr)                                ;//            yr = yr - sign(ysize,yr)
         }                                                          ;//         end do
         zr = zr + zmove                                            ;//         zr = zr + zmove
         while (abs(zr)  >   zsize2) {                              ;//         do while (abs(zr) .gt. zsize2)
            zr = zr - sign(zsize,zr)                                ;//            zr = zr - sign(zsize,zr)
         }                                                          ;//         end do
///
///     for monoclinic lattice, convert "xr" and "zr" to
///     fractional coordinates, find desired image and then
///     translate fractional coordinates back to Cartesian
///
      } else if (monoclinic) {                                      ;//      else if (monoclinic) then
         zr = zr / beta_sin                                         ;//         zr = zr / beta_sin
         xr = xr - zr*beta_cos                                      ;//         xr = xr - zr*beta_cos
         xr = xr + xmove                                            ;//         xr = xr + xmove
         while (abs(xr)  >   xsize2) {                              ;//         do while (abs(xr) .gt. xsize2)
            xr = xr - sign(xsize,xr)                                ;//            xr = xr - sign(xsize,xr)
         }                                                          ;//         end do
         yr = yr + ymove                                            ;//         yr = yr + ymove
         while (abs(yr)  >   ysize2) {                              ;//         do while (abs(yr) .gt. ysize2)
            yr = yr - sign(ysize,yr)                                ;//            yr = yr - sign(ysize,yr)
         }                                                          ;//         end do
         zr = zr + zmove                                            ;//         zr = zr + zmove
         while (abs(zr)  >   zsize2) {                              ;//         do while (abs(zr) .gt. zsize2)
            zr = zr - sign(zsize,zr)                                ;//            zr = zr - sign(zsize,zr)
         }                                                          ;//         end do
         xr = xr + zr*beta_cos                                      ;//         xr = xr + zr*beta_cos
         zr = zr * beta_sin                                         ;//         zr = zr * beta_sin
///
///     for triclinic lattice, convert pairwise components to
///     fractional coordinates, find desired image and then
///     translate fractional coordinates back to Cartesian
///
      } else if (triclinic) {                                       ;//      else if (triclinic) then
         zr = zr / gamma_term                                       ;//         zr = zr / gamma_term
         yr = (yr - zr*beta_term) / gamma_sin                       ;//         yr = (yr - zr*beta_term) / gamma_sin
         xr = xr - yr*gamma_cos - zr*beta_cos                       ;//         xr = xr - yr*gamma_cos - zr*beta_cos
         xr = xr + xmove                                            ;//         xr = xr + xmove
         while (abs(xr)  >   xsize2) {                              ;//         do while (abs(xr) .gt. xsize2)
            xr = xr - sign(xsize,xr)                                ;//            xr = xr - sign(xsize,xr)
         }                                                          ;//         end do
         yr = yr + ymove                                            ;//         yr = yr + ymove
         while (abs(yr)  >   ysize2) {                              ;//         do while (abs(yr) .gt. ysize2)
            yr = yr - sign(ysize,yr)                                ;//            yr = yr - sign(ysize,yr)
         }                                                          ;//         end do
         zr = zr + zmove                                            ;//         zr = zr + zmove
         while (abs(zr)  >   zsize2) {                              ;//         do while (abs(zr) .gt. zsize2)
            zr = zr - sign(zsize,zr)                                ;//            zr = zr - sign(zsize,zr)
         }                                                          ;//         end do
         xr = xr + yr*gamma_cos + zr*beta_cos                       ;//         xr = xr + yr*gamma_cos + zr*beta_cos
         yr = yr*gamma_sin + zr*beta_term                           ;//         yr = yr*gamma_sin + zr*beta_term
         zr = zr * gamma_term                                       ;//         zr = zr * gamma_term
///
///     for truncated octahedron, use orthogonal box equations,
///     then perform extra tests to remove corner pieces
///
      } else if (octahedron) {                                      ;//      else if (octahedron) then
         while (abs(xr)  >   xbox2) {                               ;//         do while (abs(xr) .gt. xbox2)
            xr = xr - sign(xbox,xr)                                 ;//            xr = xr - sign(xbox,xr)
         }                                                          ;//         end do
         while (abs(yr)  >   ybox2) {                               ;//         do while (abs(yr) .gt. ybox2)
            yr = yr - sign(ybox,yr)                                 ;//            yr = yr - sign(ybox,yr)
         }                                                          ;//         end do
         while (abs(zr)  >   zbox2) {                               ;//         do while (abs(zr) .gt. zbox2)
            zr = zr - sign(zbox,zr)                                 ;//            zr = zr - sign(zbox,zr)
         }                                                          ;//         end do
         if (abs(xr)+abs(yr)+abs(zr)  >   box34) {                  ;//         if (abs(xr)+abs(yr)+abs(zr) .gt. box34) then
            xr = xr - sign(xbox2,xr)                                ;//            xr = xr - sign(xbox2,xr)
            yr = yr - sign(ybox2,yr)                                ;//            yr = yr - sign(ybox2,yr)
            zr = zr - sign(zbox2,zr)                                ;//            zr = zr - sign(zbox2,zr)
         }                                                          ;//         end if
      }                                                             ;//      end if
      return                                                        ;//      return
    }                                                               //      end
}
}
}
