c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ################################################################
c     ##                                                            ##
c     ##  subroutine echarge3  --  charge-charge energy & analysis  ##
c     ##                                                            ##
c     ################################################################
c
c
c     "echarge3" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms
c
c
      subroutine echarge3
      implicit none
      include 'sizes.i'
      include 'cutoff.i'
      include 'warp.i'
c
c
c     choose the method for summing over pairwise interactions
c
      if (use_smooth) then
         call echarge3g
      else if (use_ewald) then
         if (use_clist) then
            call echarge3f
         else if (use_lights) then
            call echarge3e
         else
            call echarge3d
         end if
      else if (use_clist) then
         call echarge3c
      else if (use_lights) then
         call echarge3b
      else
         call echarge3a
      end if
      return
      end
c
c
c     #################################################################
c     ##                                                             ##
c     ##  subroutine echarge3a  --  charge analysis via double loop  ##
c     ##                                                             ##
c     #################################################################
c
c
c     "echarge3a" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms using a pairwise
c     double loop
c
c
      subroutine echarge3a
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'bound.i'
      include 'cell.i'
      include 'charge.i'
      include 'chgpot.i'
      include 'couple.i'
      include 'energi.i'
      include 'group.i'
      include 'inform.i'
      include 'inter.i'
      include 'iounit.i'
      include 'molcul.i'
      include 'shunt.i'
      include 'usage.i'
      integer i,j,k
      integer ii,kk
      integer in,kn
      integer ic,kc
      real*8 e,fgrp
      real*8 r,r2,rb
      real*8 f,fi,fik
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 xc,yc,zc
      real*8 xic,yic,zic
      real*8 shift,taper,trans
      real*8 rc,rc2,rc3,rc4
      real*8 rc5,rc6,rc7
      real*8, allocatable :: cscale(:)
      logical proceed,usei
      logical header,huge
      character*6 mode
c
c
c     zero out the charge interaction energy and partitioning
c
      nec = 0
      ec = 0.0d0
      do i = 1, n
         aec(i) = 0.0d0
      end do
      if (nion .eq. 0)  return
      header = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (cscale(n))
c
c     set array needed to scale connected atom interactions
c
      do i = 1, n
         cscale(i) = 1.0d0
      end do
c
c     set conversion factor, cutoff and switching coefficients
c
      f = electric / dielec
      mode = 'CHARGE'
      call switch (mode)
c
c     compute and partition the charge interaction energy
c
      do ii = 1, nion-1
         i = iion(ii)
         in = jion(ii)
         ic = kion(ii)
         xic = x(ic)
         yic = y(ic)
         zic = z(ic)
         xi = x(i) - xic
         yi = y(i) - yic
         zi = z(i) - zic
         fi = f * pchg(ii)
         usei = (use(i) .or. use(ic))
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     decide whether to compute the current interaction
c
         do kk = ii+1, nion
            k = iion(kk)
            kn = jion(kk)
            kc = kion(kk)
            proceed = .true.
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            if (proceed)  proceed = (usei .or. use(k) .or. use(kc))
            if (proceed)  proceed = (cscale(kn) .ne. 0.0d0)
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               xc = xic - x(kc)
               yc = yic - y(kc)
               zc = zic - z(kc)
               if (use_bounds)  call image (xc,yc,zc)
               rc2 = xc*xc + yc*yc + zc*zc
               if (rc2 .le. off2) then
                  xr = xc + xi - x(k) + x(kc)
                  yr = yc + yi - y(k) + y(kc)
                  zr = zc + zi - z(k) + z(kc)
                  r2 = xr*xr + yr*yr + zr*zr
                  r = sqrt(r2)
                  rb = r + ebuffer
                  fik = fi * pchg(kk) * cscale(kn)
                  e = fik / rb
c
c     use shifted energy switching if near the cutoff distance
c
                  shift = fik / (0.5d0*(off+cut))
                  e = e - shift
                  if (rc2 .gt. cut2) then
                     rc = sqrt(rc2)
                     rc3 = rc2 * rc
                     rc4 = rc2 * rc2
                     rc5 = rc2 * rc3
                     rc6 = rc3 * rc3
                     rc7 = rc3 * rc4
                     taper = c5*rc5 + c4*rc4 + c3*rc3
     &                          + c2*rc2 + c1*rc + c0
                     trans = fik * (f7*rc7 + f6*rc6 + f5*rc5 + f4*rc4
     &                               + f3*rc3 + f2*rc2 + f1*rc + f0)
                     e = e*taper + trans
                  end if
c
c     scale the interaction based on its group membership
c
                  if (use_group)  e = e * fgrp
c
c     increment the overall charge-charge energy component
c
                  nec = nec + 1
                  ec = ec + e
                  aec(i) = aec(i) + 0.5d0*e
                  aec(k) = aec(k) + 0.5d0*e
c
c     increment the total intermolecular energy
c
                  if (molcule(i) .ne. molcule(k)) then
                     einter = einter + e
                  end if
c
c     print a message if the energy of this interaction is large
c
                  huge = (abs(e) .gt. 100.0d0)
                  if (debug .or. (verbose.and.huge)) then
                     if (header) then
                        header = .false.
                        write (iout,10)
   10                   format (/,' Individual Charge-Charge',
     &                             ' Interactions :',
     &                          //,' Type',14x,'Atom Names',
     &                             17x,'Charges',5x,'Distance',
     &                             6x,'Energy',/)
                     end if
                     write (iout,20)  i,name(i),k,name(k),
     &                                pchg(ii),pchg(kk),r,e
   20                format (' Charge',4x,2(i7,'-',a3),8x,
     &                          2f7.2,f11.4,f12.4)
                  end if
               end if
            end if
         end do
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     for periodic boundary conditions with large cutoffs
c     neighbors must be found by the replicates method
c
      if (.not. use_replica)  return
c
c     calculate interaction energy with other unit cells
c
      do ii = 1, nion
         i = iion(ii)
         in = jion(ii)
         ic = kion(ii)
         usei = (use(i) .or. use(ic))
         xic = x(ic)
         yic = y(ic)
         zic = z(ic)
         xi = x(i) - xic
         yi = y(i) - yic
         zi = z(i) - zic
         fi = f * pchg(ii)
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     decide whether to compute the current interaction
c
         do kk = ii, nion
            k = iion(kk)
            kn = jion(kk)
            kc = kion(kk)
            proceed = .true.
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            if (proceed)  proceed = (usei .or. use(k) .or. use(kc))
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               do j = 1, ncell
                  xc = xic - x(kc)
                  yc = yic - y(kc)
                  zc = zic - z(kc)
                  call imager (xc,yc,zc,j)
                  rc2 = xc*xc + yc*yc + zc*zc
                  if (rc2 .le. off2) then
                     xr = xc + xi - x(k) + x(kc)
                     yr = yc + yi - y(k) + y(kc)
                     zr = zc + zi - z(k) + z(kc)
                     r2 = xr*xr + yr*yr + zr*zr
                     r = sqrt(r2)
                     rb = r + ebuffer
                     fik = fi * pchg(kk)
                     if (use_polymer) then
                        if (r2 .le. polycut2)  fik = fik * cscale(kn)
                     end if
                     e = fik / rb
c
c     use shifted energy switching if near the cutoff distance
c
                     shift = fik / (0.5d0*(off+cut))
                     e = e - shift
                     if (rc2 .gt. cut2) then
                        rc = sqrt(rc2)
                        rc3 = rc2 * rc
                        rc4 = rc2 * rc2
                        rc5 = rc2 * rc3
                        rc6 = rc3 * rc3
                        rc7 = rc3 * rc4
                        taper = c5*rc5 + c4*rc4 + c3*rc3
     &                             + c2*rc2 + c1*rc + c0
                        trans = fik * (f7*rc7 + f6*rc6 + f5*rc5 + f4*rc4
     &                                  + f3*rc3 + f2*rc2 + f1*rc + f0)
                        e = e*taper + trans
                     end if
c
c     scale the interaction based on its group membership
c
                     if (use_group)  e = e * fgrp
c
c     increment the overall charge-charge energy component
c
                     if (i .eq. k)  e = 0.5d0 * e
                     if (e .ne. 0.0d0)  nec = nec + 1
                     ec = ec + e
                     aec(i) = aec(i) + 0.5d0*e
                     aec(k) = aec(k) + 0.5d0*e
c
c     increment the total intermolecular energy
c
                     einter = einter + e
c
c     print a message if the energy of this interaction is large
c
                     huge = (abs(e) .gt. 100.0d0)
                     if ((debug.and.e.ne.0.0d0)
     &                     .or. (verbose.and.huge)) then
                        if (header) then
                           header = .false.
                           write (iout,30)
   30                      format (/,' Individual Charge-Charge',
     &                                ' Interactions :',
     &                             //,' Type',14x,'Atom Names',
     &                                17x,'Charges',5x,'Distance',
     &                                6x,'Energy',/)
                        end if
                        write (iout,40)  i,name(i),k,name(k),
     &                                   pchg(ii),pchg(kk),r,e
   40                   format (' Charge',4x,2(i7,'-',a3),1x,
     &                             '(XTAL)',1x,2f7.2,f11.4,f12.4)
                     end if
                  end if
               end do
            end if
         end do
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (cscale)
      return
      end
c
c
c     ##################################################################
c     ##                                                              ##
c     ##  subroutine echarge3b  --  method of lights charge analysis  ##
c     ##                                                              ##
c     ##################################################################
c
c
c     "echarge3b" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms using the method
c     of lights
c
c
      subroutine echarge3b
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'bound.i'
      include 'boxes.i'
      include 'cell.i'
      include 'charge.i'
      include 'chgpot.i'
      include 'couple.i'
      include 'energi.i'
      include 'group.i'
      include 'inform.i'
      include 'inter.i'
      include 'iounit.i'
      include 'light.i'
      include 'molcul.i'
      include 'shunt.i'
      include 'usage.i'
      integer i,j,k,ii,kk
      integer in,ic,kn,kc
      integer kgy,kgz,kmap
      integer start,stop
      integer ikmin,ikmax
      real*8 e,fgrp
      real*8 r,r2,rb
      real*8 f,fi,fik
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 xc,yc,zc
      real*8 xic,yic,zic
      real*8 shift,taper,trans
      real*8 rc,rc2,rc3,rc4
      real*8 rc5,rc6,rc7
      real*8, allocatable :: cscale(:)
      real*8, allocatable :: xsort(:)
      real*8, allocatable :: ysort(:)
      real*8, allocatable :: zsort(:)
      logical proceed,usei
      logical prime,repeat
      logical header,huge
      character*6 mode
c
c
c     zero out the charge interaction energy and partitioning
c
      nec = 0
      ec = 0.0d0
      do i = 1, n
         aec(i) = 0.0d0
      end do
      if (nion .eq. 0)  return
      header = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (cscale(n))
      allocate (xsort(8*n))
      allocate (ysort(8*n))
      allocate (zsort(8*n))
c
c     set array needed to scale connected atom interactions
c
      do i = 1, n
         cscale(i) = 1.0d0
      end do
c
c     set conversion factor, cutoff and switching coefficients
c
      f = electric / dielec
      mode = 'CHARGE'
      call switch (mode)
c
c     transfer the interaction site coordinates to sorting arrays
c
      do i = 1, nion
         k = kion(i)
         xsort(i) = x(k)
         ysort(i) = y(k)
         zsort(i) = z(k)
      end do
c
c     use the method of lights to generate neighbors
c
      call lights (off,nion,xsort,ysort,zsort)
c
c     loop over all atoms computing the interactions
c
      do ii = 1, nion
         i = iion(ii)
         in = jion(ii)
         ic = kion(ii)
         usei = (use(i) .or. use(ic))
         xic = xsort(rgx(ii))
         yic = ysort(rgy(ii))
         zic = zsort(rgz(ii))
         xi = x(i) - x(ic)
         yi = y(i) - y(ic)
         zi = z(i) - z(ic)
         fi = f * pchg(ii)
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     loop over method of lights neighbors of current atom
c
         if (kbx(ii) .le. kex(ii)) then
            repeat = .false.
            start = kbx(ii) + 1
            stop = kex(ii)
         else
            repeat = .true.
            start = 1
            stop = kex(ii)
         end if
   10    continue
         do j = start, stop
            kk = locx(j)
            kgy = rgy(kk)
            if (kby(ii) .le. key(ii)) then
               if (kgy.lt.kby(ii) .or. kgy.gt.key(ii))  goto 50
            else
               if (kgy.lt.kby(ii) .and. kgy.gt.key(ii))  goto 50
            end if
            kgz = rgz(kk)
            if (kbz(ii) .le. kez(ii)) then
               if (kgz.lt.kbz(ii) .or. kgz.gt.kez(ii))  goto 50
            else
               if (kgz.lt.kbz(ii) .and. kgz.gt.kez(ii))  goto 50
            end if
            kmap = kk - ((kk-1)/nion)*nion
            k = iion(kmap)
            kn = jion(kmap)
            kc = kion(kmap)
            prime = (kk .le. nion)
c
c     decide whether to compute the current interaction
c
            proceed = .true.
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            if (proceed)  proceed = (usei .or. use(k) .or. use(kc))
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               xc = xic - xsort(j)
               yc = yic - ysort(kgy)
               zc = zic - zsort(kgz)
               if (use_bounds) then
                  if (abs(xc) .gt. xcell2)  xc = xc - sign(xcell,xc)
                  if (abs(yc) .gt. ycell2)  yc = yc - sign(ycell,yc)
                  if (abs(zc) .gt. zcell2)  zc = zc - sign(zcell,zc)
                  if (monoclinic) then
                     xc = xc + zc*beta_cos
                     zc = zc * beta_sin
                  else if (triclinic) then
                     xc = xc + yc*gamma_cos + zc*beta_cos
                     yc = yc*gamma_sin + zc*beta_term
                     zc = zc * gamma_term
                  end if
               end if
               rc2 = xc*xc + yc*yc + zc*zc
               if (rc2 .le. off2) then
                  xr = xc + xi - x(k) + x(kc)
                  yr = yc + yi - y(k) + y(kc)
                  zr = zc + zi - z(k) + z(kc)
                  r2 = xr*xr + yr*yr + zr*zr
                  r = sqrt(r2)
                  rb = r + ebuffer
                  fik = fi * pchg(kmap)
                  if (prime)  fik = fik * cscale(kn)
                  if (use_polymer) then
                     if (r2 .gt. polycut2)  fik = fi * pchg(kmap)
                  end if
                  e = fik / rb
c
c     use shifted energy switching if near the cutoff distance
c
                  shift = fik / (0.5d0*(off+cut))
                  e = e - shift
                  if (rc2 .gt. cut2) then
                     rc = sqrt(rc2)
                     rc3 = rc2 * rc
                     rc4 = rc2 * rc2
                     rc5 = rc2 * rc3
                     rc6 = rc3 * rc3
                     rc7 = rc3 * rc4
                     taper = c5*rc5 + c4*rc4 + c3*rc3
     &                          + c2*rc2 + c1*rc + c0
                     trans = fik * (f7*rc7 + f6*rc6 + f5*rc5 + f4*rc4
     &                               + f3*rc3 + f2*rc2 + f1*rc + f0)
                     e = e*taper + trans
                  end if
c
c     scale the interaction based on its group membership
c
                  if (use_group)  e = e * fgrp
c
c     increment the overall charge-charge energy component
c
                  if (e .ne. 0.0d0)  nec = nec + 1
                  ec = ec + e
                  aec(i) = aec(i) + 0.5d0*e
                  aec(k) = aec(k) + 0.5d0*e
c
c     increment the total intermolecular energy
c
                  if (.not.prime .or. molcule(i).ne.molcule(k)) then
                     einter = einter + e
                  end if
c
c     print a message if the energy of this interaction is large
c
                  huge = (abs(e) .gt. 100.0d0)
                  if ((debug.and.e.ne.0.0d0)
     &                  .or. (verbose.and.huge)) then
                     if (header) then
                        header = .false.
                        write (iout,20)
   20                   format (/,' Individual Charge-Charge',
     &                             ' Interactions :',
     &                          //,' Type',14x,'Atom Names',
     &                             17x,'Charges',5x,'Distance',
     &                             6x,'Energy',/)
                     end if
                     ikmin = min(i,k)
                     ikmax = max(i,k)
                     if (prime) then
                        write (iout,30)  ikmin,name(ikmin),ikmax,
     &                                   name(ikmax),pchg(ii),
     &                                   pchg(kmap),r,e
   30                   format (' Charge',4x,2(i7,'-',a3),8x,
     &                             2f7.2,f11.4,f12.4)
                     else
                        write (iout,40)  ikmin,name(ikmin),ikmax,
     &                                   name(ikmax),pchg(ii),
     &                                   pchg(kmap),r,e
   40                   format (' Charge',4x,2(i7,'-',a3),1x,
     &                             '(XTAL)',1x,2f7.2,f11.4,f12.4)
                     end if
                  end if
               end if
            end if
   50       continue
         end do
         if (repeat) then
            repeat = .false.
            start = kbx(ii) + 1
            stop = nlight
            goto 10
         end if
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (cscale)
      deallocate (xsort)
      deallocate (ysort)
      deallocate (zsort)
      return
      end
c
c
c     ###############################################################
c     ##                                                           ##
c     ##  subroutine echarge3c  --  neighbor list charge analysis  ##
c     ##                                                           ##
c     ###############################################################
c
c
c     "echarge3c" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms using a pairwise
c     neighbor list
c
c
      subroutine echarge3c
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'bound.i'
      include 'charge.i'
      include 'chgpot.i'
      include 'couple.i'
      include 'energi.i'
      include 'group.i'
      include 'inform.i'
      include 'inter.i'
      include 'iounit.i'
      include 'molcul.i'
      include 'neigh.i'
      include 'shunt.i'
      include 'usage.i'
      integer i,j,k
      integer ii,kk,kkk
      integer in,kn
      integer ic,kc
      real*8 e,fgrp
      real*8 r,r2,rb
      real*8 f,fi,fik
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 xc,yc,zc
      real*8 xic,yic,zic
      real*8 shift,taper,trans
      real*8 rc,rc2,rc3,rc4
      real*8 rc5,rc6,rc7
      real*8, allocatable :: cscale(:)
      logical proceed,usei
      logical header,huge
      character*6 mode
c
c
c     zero out the charge interaction energy and partitioning
c
      nec = 0
      ec = 0.0d0
      do i = 1, n
         aec(i) = 0.0d0
      end do
      if (nion .eq. 0)  return
      header = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (cscale(n))
c
c     set array needed to scale connected atom interactions
c
      do i = 1, n
         cscale(i) = 1.0d0
      end do
c
c     set conversion factor, cutoff and switching coefficients
c
      f = electric / dielec
      mode = 'CHARGE'
      call switch (mode)
c
c     compute and partition the charge interaction energy
c
      do ii = 1, nion-1
         i = iion(ii)
         in = jion(ii)
         ic = kion(ii)
         xic = x(ic)
         yic = y(ic)
         zic = z(ic)
         xi = x(i) - xic
         yi = y(i) - yic
         zi = z(i) - zic
         fi = f * pchg(ii)
         usei = (use(i) .or. use(ic))
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     decide whether to compute the current interaction
c
         do kkk = 1, nelst(ii)
            kk = elst(kkk,ii)
            k = iion(kk)
            kn = jion(kk)
            kc = kion(kk)
            proceed = .true.
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            if (proceed)  proceed = (usei .or. use(k) .or. use(kc))
            if (proceed)  proceed = (cscale(kn) .ne. 0.0d0)
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               xc = xic - x(kc)
               yc = yic - y(kc)
               zc = zic - z(kc)
               if (use_bounds)  call image (xc,yc,zc)
               rc2 = xc*xc + yc*yc + zc*zc
               if (rc2 .le. off2) then
                  xr = xc + xi - x(k) + x(kc)
                  yr = yc + yi - y(k) + y(kc)
                  zr = zc + zi - z(k) + z(kc)
                  r2 = xr*xr + yr*yr + zr*zr
                  r = sqrt(r2)
                  rb = r + ebuffer
                  fik = fi * pchg(kk) * cscale(kn)
                  e = fik / rb
c
c     use shifted energy switching if near the cutoff distance
c
                  shift = fik / (0.5d0*(off+cut))
                  e = e - shift
                  if (rc2 .gt. cut2) then
                     rc = sqrt(rc2)
                     rc3 = rc2 * rc
                     rc4 = rc2 * rc2
                     rc5 = rc2 * rc3
                     rc6 = rc3 * rc3
                     rc7 = rc3 * rc4
                     taper = c5*rc5 + c4*rc4 + c3*rc3
     &                          + c2*rc2 + c1*rc + c0
                     trans = fik * (f7*rc7 + f6*rc6 + f5*rc5 + f4*rc4
     &                               + f3*rc3 + f2*rc2 + f1*rc + f0)
                     e = e*taper + trans
                  end if
c
c     scale the interaction based on its group membership
c
                  if (use_group)  e = e * fgrp
c
c     increment the overall charge-charge energy component
c
                  nec = nec + 1
                  ec = ec + e
                  aec(i) = aec(i) + 0.5d0*e
                  aec(k) = aec(k) + 0.5d0*e
c
c     increment the total intermolecular energy
c
                  if (molcule(i) .ne. molcule(k)) then
                     einter = einter + e
                  end if
c
c     print a message if the energy of this interaction is large
c
                  huge = (abs(e) .gt. 100.0d0)
                  if (debug .or. (verbose.and.huge)) then
                     if (header) then
                        header = .false.
                        write (iout,10)
   10                   format (/,' Individual Charge-Charge',
     &                             ' Interactions :',
     &                          //,' Type',14x,'Atom Names',
     &                             17x,'Charges',5x,'Distance',
     &                             6x,'Energy',/)
                     end if
                     write (iout,20)  i,name(i),k,name(k),
     &                                pchg(ii),pchg(kk),r,e
   20                format (' Charge',4x,2(i7,'-',a3),8x,
     &                          2f7.2,f11.4,f12.4)
                  end if
               end if
            end if
         end do
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (cscale)
      return
      end
c
c
c     ################################################################
c     ##                                                            ##
c     ##  subroutine echarge3d  --  Ewald charge analysis via loop  ##
c     ##                                                            ##
c     ################################################################
c
c
c     "echarge3d" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms using a particle
c     mesh Ewald summation
c
c
      subroutine echarge3d
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'bound.i'
      include 'boxes.i'
      include 'cell.i'
      include 'charge.i'
      include 'chgpot.i'
      include 'couple.i'
      include 'energi.i'
      include 'ewald.i'
      include 'group.i'
      include 'inform.i'
      include 'inter.i'
      include 'iounit.i'
      include 'math.i'
      include 'molcul.i'
      include 'shunt.i'
      include 'usage.i'
      integer i,j,k
      integer ii,kk
      integer in,kn
      real*8 e,efix
      real*8 eintra
      real*8 f,fi,fik
      real*8 fs,fgrp
      real*8 r,r2,rb,rew
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 xd,yd,zd
      real*8 erfc,erfterm
      real*8 scale,scaleterm
      real*8, allocatable :: cscale(:)
      logical proceed,usei
      logical header,huge
      character*6 mode
      external erfc
c
c
c     zero out the Ewald summation energy and partitioning
c
      nec = 0
      ec = 0.0d0
      do i = 1, n
         aec(i) = 0.0d0
      end do
      if (nion .eq. 0)  return
      header = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (cscale(n))
c
c     set array needed to scale connected atom interactions
c
      do i = 1, n
         cscale(i) = 1.0d0
      end do
c
c     zero out the intramolecular portion of the Ewald energy
c
      eintra = 0.0d0
c
c     set conversion factor, cutoff and switching coefficients
c
      f = electric / dielec
      mode = 'EWALD'
      call switch (mode)
c
c     compute the Ewald self-energy term over all the atoms
c
      fs = -f * aewald / sqrtpi
      do ii = 1, nion
         e = fs * pchg(ii)**2
         ec = ec + e
      end do
c
c     compute the cell dipole boundary correction term
c
      if (boundary .eq. 'VACUUM') then
         xd = 0.0d0
         yd = 0.0d0
         zd = 0.0d0
         do ii = 1, nion
            i = iion(ii)
            xd = xd + pchg(ii)*x(i)
            yd = yd + pchg(ii)*y(i)
            zd = zd + pchg(ii)*z(i)
         end do
         e = (2.0d0/3.0d0) * f * (pi/volbox) * (xd*xd+yd*yd+zd*zd)
         ec = ec + e
      end if
c
c     compute the reciprocal space part of the Ewald summation
c
      call ecrecip
c
c     compute the real space portion of the Ewald summation
c
      do ii = 1, nion-1
         i = iion(ii)
         in = jion(ii)
         usei = use(i)
         xi = x(i)
         yi = y(i)
         zi = z(i)
         fi = f * pchg(ii)
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     decide whether to compute the current interaction
c
         do kk = ii+1, nion
            k = iion(kk)
            kn = jion(kk)
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            proceed = .true.
            if (proceed)  proceed = (usei .or. use(k))
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               xr = xi - x(k)
               yr = yi - y(k)
               zr = zi - z(k)
c
c     find energy for interactions within real space cutoff
c
               call image (xr,yr,zr)
               r2 = xr*xr + yr*yr + zr*zr
               if (r2 .le. off2) then
                  r = sqrt(r2)
                  rb = r + ebuffer
                  fik = fi * pchg(kk)
                  rew = aewald * r
                  erfterm = erfc (rew)
                  scale = cscale(kn)
                  if (use_group)  scale = scale * fgrp
                  scaleterm = scale - 1.0d0
                  e = (fik/rb) * (erfterm+scaleterm)
c
c     increment the overall charge-charge energy component
c
                  nec = nec + 1
                  ec = ec + e
                  aec(i) = aec(i) + 0.5d0*e
                  aec(k) = aec(k) + 0.5d0*e
c
c     increment the total intramolecular energy
c
                  efix = (fik/rb) * scale
                  if (molcule(i) .eq. molcule(k)) then
                     eintra = eintra + efix
                  end if
c
c     print a message if the energy of this interaction is large
c
                  huge = (abs(efix) .gt. 100.0d0)
                  if (debug .or. (verbose.and.huge)) then
                     if (header) then
                        header = .false.
                        write (iout,10)
   10                   format (/,' Individual Real Space Ewald',
     &                             ' Charge-Charge Interactions :',
     &                          //,' Type',14x,'Atom Names',
     &                             17x,'Charges',5x,'Distance',
     &                             6x,'Energy',/)
                     end if
                     write (iout,20)  i,name(i),k,name(k),
     &                                pchg(ii),pchg(kk),r,efix
   20                format (' Charge',4x,2(i7,'-',a3),8x,
     &                          2f7.2,f11.4,f12.4)
                  end if
               end if
            end if
         end do
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     intermolecular energy is total minus intramolecular part
c
      einter = einter + ec - eintra
c
c     for periodic boundary conditions with large cutoffs
c     neighbors must be found by the replicates method
c
      if (.not. use_replica)  return
c
c     calculate real space portion involving other unit cells
c
      do ii = 1, nion
         i = iion(ii)
         in = jion(ii)
         usei = use(i)
         xi = x(i)
         yi = y(i)
         zi = z(i)
         fi = f * pchg(ii)
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     decide whether to compute the current interaction
c
         do kk = ii, nion
            k = iion(kk)
            kn = jion(kk)
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            proceed = .true.
            if (proceed)  proceed = (usei .or. use(k))
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               do j = 1, ncell
                  xr = xi - x(k)
                  yr = yi - y(k)
                  zr = zi - z(k)
                  call imager (xr,yr,zr,j)
                  r2 = xr*xr + yr*yr + zr*zr
                  if (r2 .le. off2) then
                     r = sqrt(r2)
                     rb = r + ebuffer
                     fik = fi * pchg(kk)
                     rew = aewald * r
                     erfterm = erfc (rew)
                     scale = 1.0d0
                     if (use_group)  scale = scale * fgrp
                     if (use_polymer) then
                        if (r2 .le. polycut2) then
                           scale = scale * cscale(kn)
                        end if
                     end if
                     scaleterm = scale - 1.0d0
                     e = (fik/rb) * (erfterm+scaleterm)
c
c     increment the overall charge-charge energy component
c
                     if (i .eq. k)  e = 0.5d0 * e
                     if (e .ne. 0.0d0)  nec = nec + 1
                     ec = ec + e
                     aec(i) = aec(i) + 0.5d0*e
                     aec(k) = aec(k) + 0.5d0*e
c
c     print a message if the energy of this interaction is large
c
                     efix = (fik/rb) * scale
                     huge = (abs(efix) .gt. 100.0d0)
                     if ((debug.and.e.ne.0.0d0)
     &                     .or. (verbose.and.huge)) then
                        if (header) then
                           header = .false.
                           write (iout,30)
   30                      format (/,' Individual Real Space Ewald',
     &                                ' Charge-Charge Interactions :',
     &                             //,' Type',14x,'Atom Names',
     &                                17x,'Charges',5x,'Distance',
     &                                6x,'Energy',/)
                        end if
                        write (iout,40)  i,name(i),k,name(k),
     &                                   pchg(ii),pchg(kk),r,efix
   40                   format (' Charge',4x,2(i7,'-',a3),1x,
     &                             '(XTAL)',1x,2f7.2,f11.4,f12.4)
                     end if
                  end if
               end do
            end if
         end do
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (cscale)
      return
      end
c
c
c     ##################################################################
c     ##                                                              ##
c     ##  subroutine echarge3e  --  Ewald charge analysis via lights  ##
c     ##                                                              ##
c     ##################################################################
c
c
c     "echarge3e" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms using a particle
c     mesh Ewald summation and the method of lights
c
c
      subroutine echarge3e
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'bound.i'
      include 'boxes.i'
      include 'cell.i'
      include 'charge.i'
      include 'chgpot.i'
      include 'couple.i'
      include 'energi.i'
      include 'ewald.i'
      include 'group.i'
      include 'inform.i'
      include 'inter.i'
      include 'iounit.i'
      include 'light.i'
      include 'math.i'
      include 'molcul.i'
      include 'shunt.i'
      include 'usage.i'
      integer i,j,k
      integer ii,kk,in,kn
      integer kgy,kgz,kmap
      integer start,stop
      real*8 e,efix
      real*8 eintra
      real*8 f,fi,fik
      real*8 fs,fgrp
      real*8 r,r2,rb,rew
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 xd,yd,zd
      real*8 erfc,erfterm
      real*8 scale,scaleterm
      real*8, allocatable :: cscale(:)
      real*8, allocatable :: xsort(:)
      real*8, allocatable :: ysort(:)
      real*8, allocatable :: zsort(:)
      logical proceed,usei
      logical prime,repeat
      logical header,huge
      character*6 mode
      external erfc
c
c
c     zero out the Ewald summation energy and partitioning
c
      nec = 0
      ec = 0.0d0
      do i = 1, n
         aec(i) = 0.0d0
      end do
      if (nion .eq. 0)  return
      header = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (cscale(n))
      allocate (xsort(8*n))
      allocate (ysort(8*n))
      allocate (zsort(8*n))
c
c     set array needed to scale connected atom interactions
c
      do i = 1, n
         cscale(i) = 1.0d0
      end do
c
c     zero out the intramolecular portion of the Ewald energy
c
      eintra = 0.0d0
c
c     set conversion factor, cutoff and switching coefficients
c
      f = electric / dielec
      mode = 'EWALD'
      call switch (mode)
c
c     compute the Ewald self-energy term over all the atoms
c
      fs = -f * aewald / sqrtpi
      do ii = 1, nion
         e = fs * pchg(ii)**2
         ec = ec + e
      end do
c
c     compute the cell dipole boundary correction term
c
      if (boundary .eq. 'VACUUM') then
         xd = 0.0d0
         yd = 0.0d0
         zd = 0.0d0
         do ii = 1, nion
            i = iion(ii)
            xd = xd + pchg(ii)*x(i)
            yd = yd + pchg(ii)*y(i)
            zd = zd + pchg(ii)*z(i)
         end do
         e = (2.0d0/3.0d0) * f * (pi/volbox) * (xd*xd+yd*yd+zd*zd)
         ec = ec + e
      end if
c
c     compute the reciprocal space part of the Ewald summation
c
      call ecrecip
c
c     compute the real space portion of the Ewald summation;
c     transfer the interaction site coordinates to sorting arrays
c
      do i = 1, nion
         k = iion(i)
         xsort(i) = x(k)
         ysort(i) = y(k)
         zsort(i) = z(k)
      end do
c
c     use the method of lights to generate neighbors
c
      call lights (off,nion,xsort,ysort,zsort)
c
c     loop over all atoms computing the interactions
c
      do ii = 1, nion
         i = iion(ii)
         in = jion(ii)
         xi = xsort(rgx(ii))
         yi = ysort(rgy(ii))
         zi = zsort(rgz(ii))
         fi = f * pchg(ii)
         usei = use(i)
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     loop over method of lights neighbors of current atom
c
         if (kbx(ii) .le. kex(ii)) then
            repeat = .false.
            start = kbx(ii) + 1
            stop = kex(ii)
         else
            repeat = .true.
            start = 1
            stop = kex(ii)
         end if
   10    continue
         do j = start, stop
            kk = locx(j)
            kgy = rgy(kk)
            if (kby(ii) .le. key(ii)) then
               if (kgy.lt.kby(ii) .or. kgy.gt.key(ii))  goto 50
            else
               if (kgy.lt.kby(ii) .and. kgy.gt.key(ii))  goto 50
            end if
            kgz = rgz(kk)
            if (kbz(ii) .le. kez(ii)) then
               if (kgz.lt.kbz(ii) .or. kgz.gt.kez(ii))  goto 50
            else
               if (kgz.lt.kbz(ii) .and. kgz.gt.kez(ii))  goto 50
            end if
            kmap = kk - ((kk-1)/nion)*nion
            k = iion(kmap)
            kn = jion(kmap)
            prime = (kk .le. nion)
c
c     decide whether to compute the current interaction
c
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            proceed = .true.
            if (proceed)  proceed = (usei .or. use(k))
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               xr = xi - xsort(j)
               yr = yi - ysort(kgy)
               zr = zi - zsort(kgz)
c
c     find energy for interactions within real space cutoff
c
               if (use_bounds) then
                  if (abs(xr) .gt. xcell2)  xr = xr - sign(xcell,xr)
                  if (abs(yr) .gt. ycell2)  yr = yr - sign(ycell,yr)
                  if (abs(zr) .gt. zcell2)  zr = zr - sign(zcell,zr)
                  if (monoclinic) then
                     xr = xr + zr*beta_cos
                     zr = zr * beta_sin
                  else if (triclinic) then
                     xr = xr + yr*gamma_cos + zr*beta_cos
                     yr = yr*gamma_sin + zr*beta_term
                     zr = zr * gamma_term
                  end if
               end if
               r2 = xr*xr + yr*yr + zr*zr
               if (r2 .le. off2) then
                  r = sqrt(r2)
                  rb = r + ebuffer
                  rew = aewald * r
                  erfterm = erfc (rew)
                  scale = 1.0d0
                  if (prime)  scale = cscale(kn)
                  if (use_group)  scale = scale * fgrp
                  fik = fi * pchg(kmap)
                  if (use_polymer) then
                     if (r2 .gt. polycut2)  fik = fi * pchg(kmap)
                  end if
                  scaleterm = scale - 1.0d0
                  e = (fik/rb) * (erfterm+scaleterm)
c
c     increment the overall charge-charge energy component
c
                  if (e .ne. 0.0d0)  nec = nec + 1
                  ec = ec + e
                  aec(i) = aec(i) + 0.5d0*e
                  aec(k) = aec(k) + 0.5d0*e
c
c     increment the total intramolecular energy
c
                  efix = (fik/rb) * scale
                  if (prime .and. molcule(i).eq.molcule(k)) then
                     eintra = eintra + efix
                  end if
c
c     print a message if the energy of this interaction is large
c
                  huge = (abs(efix) .gt. 100.0d0)
                  if (debug .or. (verbose.and.huge)) then
                     if (header) then
                        header = .false.
                        write (iout,20)
   20                   format (/,' Individual Real Space Ewald',
     &                             ' Charge-Charge Interactions :',
     &                          //,' Type',14x,'Atom Names',
     &                             17x,'Charges',5x,'Distance',
     &                             6x,'Energy',/)
                     end if
                     if (prime) then
                        write (iout,30)  i,name(i),k,name(k),pchg(ii),
     &                                   pchg(kmap),r,efix
   30                   format (' Charge',4x,2(i7,'-',a3),8x,
     &                             2f7.2,f11.4,f12.4)
                     else
                        write (iout,40)  i,name(i),k,name(k),pchg(ii),
     &                                   pchg(kmap),r,efix
   40                   format (' Charge',4x,2(i7,'-',a3),1x,
     &                             '(XTAL)',1x,2f7.2,f11.4,f12.4)
                     end if
                  end if
               end if
            end if
   50       continue
         end do
         if (repeat) then
            repeat = .false.
            start = kbx(ii) + 1
            stop = nlight
            goto 10
         end if
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     intermolecular energy is total minus intramolecular part
c
      einter = einter + ec - eintra
c
c     perform deallocation of some local arrays
c
      deallocate (cscale)
      deallocate (xsort)
      deallocate (ysort)
      deallocate (zsort)
      return
      end
c
c
c     ################################################################
c     ##                                                            ##
c     ##  subroutine echarge3f  --  Ewald charge analysis via list  ##
c     ##                                                            ##
c     ################################################################
c
c
c     "echarge3f" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms using a particle
c     mesh Ewald summation and a pairwise neighbor list
c
c
      subroutine echarge3f
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'bound.i'
      include 'boxes.i'
      include 'charge.i'
      include 'chgpot.i'
      include 'couple.i'
      include 'energi.i'
      include 'ewald.i'
      include 'group.i'
      include 'inform.i'
      include 'inter.i'
      include 'iounit.i'
      include 'math.i'
      include 'molcul.i'
      include 'neigh.i'
      include 'shunt.i'
      include 'usage.i'
      integer i,j,k
      integer ii,kk,kkk
      integer in,kn
      integer nect
      real*8 e,efix
      real*8 eintra
      real*8 f,fi,fik
      real*8 fs,fgrp
      real*8 r,r2,rb,rew
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 xd,yd,zd
      real*8 erfc,erfterm
      real*8 scale,scaleterm
      real*8 ect,eintrat
      real*8, allocatable :: cscale(:)
      real*8, allocatable :: aect(:)
      logical proceed,usei
      logical header,huge
      character*6 mode
      external erfc
c
c
c     zero out the Ewald summation energy and partitioning
c
      nec = 0
      ec = 0.0d0
      do i = 1, n
         aec(i) = 0.0d0
      end do
      if (nion .eq. 0)  return
      header = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (cscale(n))
      allocate (aect(n))
c
c     set array needed to scale connected atom interactions
c
      do i = 1, n
         cscale(i) = 1.0d0
      end do
c
c     zero out the intramolecular portion of the Ewald energy
c
      eintra = 0.0d0
c
c     set conversion factor, cutoff and switching coefficients
c
      f = electric / dielec
      mode = 'EWALD'
      call switch (mode)
c
c     compute the Ewald self-energy term over all the atoms
c
      fs = -f * aewald / sqrtpi
      do ii = 1, nion
         e = fs * pchg(ii)**2
         ec = ec + e
      end do
c
c     compute the cell dipole boundary correction term
c
      if (boundary .eq. 'VACUUM') then
         xd = 0.0d0
         yd = 0.0d0
         zd = 0.0d0
         do ii = 1, nion
            i = iion(ii)
            xd = xd + pchg(ii)*x(i)
            yd = yd + pchg(ii)*y(i)
            zd = zd + pchg(ii)*z(i)
         end do
         e = (2.0d0/3.0d0) * f * (pi/volbox) * (xd*xd+yd*yd+zd*zd)
         ec = ec + e
      end if
c
c     compute the reciprocal space part of the Ewald summation
c
      call ecrecip
c
c     initialize local variables for OpenMP calculation
c
      ect = ec
      eintrat = eintra
      nect = nec
      do i = 1, n
         aect(i) = aec(i)
      end do
c
c     set OpenMP directives for the major loop structure
c
!$OMP PARALLEL default(private) shared(nion,iion,jion,use,
!$OMP& x,y,z,f,pchg,nelst,elst,n12,n13,n14,n15,i12,i13,i14,
!$OMP& i15,c2scale,c3scale,c4scale,c5scale,use_group,off2,
!$OMP& aewald,molcule,ebuffer,name,verbose,debug,header,iout)
!$OMP& firstprivate(cscale) shared(ect,eintrat,nect,aect)
!$OMP DO reduction(+:ect,eintrat,nect,aect)
!$OMP& schedule(dynamic)
c
c     compute the real space portion of the Ewald summation
c
      do ii = 1, nion
         i = iion(ii)
         in = jion(ii)
         usei = use(i)
         xi = x(i)
         yi = y(i)
         zi = z(i)
         fi = f * pchg(ii)
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     decide whether to compute the current interaction
c
         do kkk = 1, nelst(ii)
            kk = elst(kkk,ii)
            k = iion(kk)
            kn = jion(kk)
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            proceed = .true.
            if (proceed)  proceed = (usei .or. use(k))
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               xr = xi - x(k)
               yr = yi - y(k)
               zr = zi - z(k)
c
c     find energy for interactions within real space cutoff
c
               call image (xr,yr,zr)
               r2 = xr*xr + yr*yr + zr*zr
               if (r2 .le. off2) then
                  r = sqrt(r2)
                  rb = r + ebuffer
                  fik = fi * pchg(kk)
                  rew = aewald * r
                  erfterm = erfc (rew)
                  scale = cscale(kn)
                  if (use_group)  scale = scale * fgrp
                  scaleterm = scale - 1.0d0
                  e = (fik/rb) * (erfterm+scaleterm)
c
c     increment the overall charge-charge energy component
c
                  nect = nect + 1
                  ect = ect + e
                  aect(i) = aect(i) + 0.5d0*e
                  aect(k) = aect(k) + 0.5d0*e
c
c     increment the total intramolecular energy
c
                  efix = (fik/rb) * scale
                  if (molcule(i) .eq. molcule(k)) then
                     eintrat = eintrat + efix
                  end if
c
c     print a message if the energy of this interaction is large
c
                  huge = (abs(efix) .gt. 100.0d0)
                  if (debug .or. (verbose.and.huge)) then
                     if (header) then
                        header = .false.
                        write (iout,10)
   10                   format (/,' Individual Real Space Ewald',
     &                             ' Charge-Charge Interactions :',
     &                          //,' Type',14x,'Atom Names',
     &                             17x,'Charges',5x,'Distance',
     &                             6x,'Energy',/)
                     end if
                     write (iout,20)  i,name(i),k,name(k),
     &                                pchg(ii),pchg(kk),r,efix
   20                format (' Charge',4x,2(i7,'-',a3),8x,
     &                          2f7.2,f11.4,f12.4)
                  end if
               end if
            end if
         end do
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     end OpenMP directives for the major loop structure
c
!$OMP END DO
!$OMP END PARALLEL
c
c     add local copies to global variables for OpenMP calculation
c
      ec = ect
      eintra = eintrat
      nec = nect
      do i = 1, n
         aec(i) = aect(i)
      end do
c
c     intermolecular energy is total minus intramolecular part
c
      einter = einter + ec - eintra
c
c     perform deallocation of some local arrays
c
      deallocate (cscale)
      deallocate (aect)
      return
      end
c
c
c     ###############################################################
c     ##                                                           ##
c     ##  subroutine echarge3g  --  charge analysis for smoothing  ##
c     ##                                                           ##
c     ###############################################################
c
c
c     "echarge3g" calculates the charge-charge interaction energy
c     and partitions the energy among the atoms for use with
c     potential smoothing methods
c
c
      subroutine echarge3g
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'charge.i'
      include 'chgpot.i'
      include 'couple.i'
      include 'energi.i'
      include 'group.i'
      include 'inform.i'
      include 'inter.i'
      include 'iounit.i'
      include 'molcul.i'
      include 'usage.i'
      include 'warp.i'
      integer i,j,k
      integer ii,kk
      integer in,kn
      real*8 e,fgrp
      real*8 r,r2,rb,rb2
      real*8 f,fi,fik
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 erf,wterm,width
      real*8 width2,width3
      real*8, allocatable :: cscale(:)
      logical proceed,usei
      logical header,huge
      external erf
c
c
c     zero out the charge interaction energy and partitioning
c
      nec = 0
      ec = 0.0d0
      do i = 1, n
         aec(i) = 0.0d0
      end do
      if (nion .eq. 0)  return
      header = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (cscale(n))
c
c     set array needed to scale connected atom interactions
c
      do i = 1, n
         cscale(i) = 1.0d0
      end do
c
c     set the energy units conversion factor
c
      f = electric / dielec
c
c     set the extent of smoothing to be performed
c
      width = deform * diffc
      if (use_dem) then
         if (width .gt. 0.0d0)  width = 0.5d0 / sqrt(width)
      else if (use_gda) then
         wterm = sqrt(3.0d0/(2.0d0*diffc))
      end if
      width2 = width * width
      width3 = width * width2
c
c     compute and partition the charge interaction energy
c
      do ii = 1, nion-1
         i = iion(ii)
         in = jion(ii)
         usei = (use(i))
         xi = x(i)
         yi = y(i)
         zi = z(i)
         fi = f * pchg(ii)
c
c     set interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = c2scale
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = c3scale
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = c4scale
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = c5scale
         end do
c
c     decide whether to compute the current interaction
c
         do kk = ii+1, nion
            k = iion(kk)
            kn = jion(kk)
            proceed = .true.
            if (use_group)  call groups (proceed,fgrp,i,k,0,0,0,0)
            if (proceed)  proceed = (usei .or. use(k))
            if (proceed)  proceed = (cscale(kn) .ne. 0.0d0)
c
c     compute the energy contribution for this interaction
c
            if (proceed) then
               xr = xi - x(k)
               yr = yi - y(k)
               zr = zi - z(k)
               r2 = xr*xr + yr*yr + zr*zr
               r = sqrt(r2)
               rb = r + ebuffer
               fik = fi * pchg(kk) * cscale(kn)
               e = fik / rb
c
c     transform the potential function via smoothing
c
               if (use_dem) then
                  if (width .gt. 0.0d0) then
                     e = e * erf(width*rb)
                  end if
               else if (use_gda) then
                  width = m2(i) + m2(k)
                  if (width .gt. 0.0d0) then
                     width = wterm / sqrt(width)
                     e = e * erf(width*rb)
                  end if
               else if (use_tophat) then
                  if (width .gt. rb) then
                     rb2 = rb * rb
                     e = fik * (3.0d0*width2-rb2) / (2.0d0*width3)
                  end if
               else if (use_stophat) then
                  e = fik / (rb+width)
               end if
c
c     scale the interaction based on its group membership
c
               if (use_group)  e = e * fgrp
c
c     increment the overall charge-charge energy component
c
               nec = nec + 1
               ec = ec + e
               aec(i) = aec(i) + 0.5d0*e
               aec(k) = aec(k) + 0.5d0*e
c
c     increment the total intermolecular energy
c
               if (molcule(i) .ne. molcule(k)) then
                  einter = einter + e
               end if
c
c     print a message if the energy of this interaction is large
c
               huge = (abs(e) .gt. 100.0d0)
               if (debug .or. (verbose.and.huge)) then
                  if (header) then
                     header = .false.
                     write (iout,10)
   10                format (/,' Individual Charge-Charge',
     &                          ' Interactions :',
     &                       //,' Type',14x,'Atom Names',
     &                          17x,'Charges',5x,'Distance',
     &                          6x,'Energy',/)
                  end if
                  write (iout,20)  i,name(i),k,name(k),pchg(ii),
     &                             pchg(kk),r,e
   20             format (' Charge',4x,2(i7,'-',a3),8x,
     &                       2f7.2,f11.4,f12.4)
               end if
            end if
         end do
c
c     reset interaction scaling coefficients for connected atoms
c
         do j = 1, n12(in)
            cscale(i12(j,in)) = 1.0d0
         end do
         do j = 1, n13(in)
            cscale(i13(j,in)) = 1.0d0
         end do
         do j = 1, n14(in)
            cscale(i14(j,in)) = 1.0d0
         end do
         do j = 1, n15(in)
            cscale(i15(j,in)) = 1.0d0
         end do
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (cscale)
      return
      end
