c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     #############################################################
c     ##                                                         ##
c     ##  program xyzedit  --  editing of Cartesian coordinates  ##
c     ##                                                         ##
c     #############################################################
c
c
c     "xyzedit" provides for modification and manipulation
c     of the contents of a Cartesian coordinates file
c
c
      program xyzedit
      implicit none
      include 'sizes.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'bound.i'
      include 'boxes.i'
      include 'couple.i'
      include 'cutoff.i'
      include 'files.i'
      include 'iounit.i'
      include 'math.i'
      include 'molcul.i'
      include 'titles.i'
      include 'units.i'
      include 'usage.i'
      integer i,j,k,m
      integer it,ixyz
      integer init,stop
      integer natom,atmnum
      integer nmode,mode
      integer offset,origin
      integer oldtype,newtype
      integer nlist,nmolecule
      integer freeunit
      integer, allocatable :: list(:)
      integer, allocatable :: keep(:)
      real*8 xi,yi,zi
      real*8 xr,yr,zr
      real*8 xran,yran,zran
      real*8 xorig,yorig,zorig
      real*8 xcm,ycm,zcm
      real*8 ri,rij,dij
      real*8 phi,theta,psi
      real*8 cphi,ctheta,cpsi
      real*8 sphi,stheta,spsi
      real*8 dist2,cut2
      real*8 random,norm,weigh
      real*8 a(3,3)
      real*8, allocatable :: rad(:)
      logical write
      character*120 xyzfile
      character*120 record
      external merge
c
c
c     initialize various constants and the write flag
c
      call initial
      offset = 0
      write = .false.
c
c     read in the coordinates and force field definition
c
      call getxyz
      call field
      call katom
c
c     perform dynamic allocation of some local arrays
c
      allocate (list(n))
      allocate (keep(n))
      allocate (rad(n))
c
c     present a list of possible coordinate modifications
c
      write (iout,10)
   10 format (/,' The TINKER XYZ Editing Facility can Provide :',
     &        //,4x,'(1) Offset the Numbers of the Current Atoms',
     &        /,4x,'(2) Deletion of Individual Specified Atoms',
     &        /,4x,'(3) Deletion of Specified Types of Atoms',
     &        /,4x,'(4) Deletion of Atoms outside Cutoff Range',
     &        /,4x,'(5) Insertion of Individual Specified Atoms',
     &        /,4x,'(6) Replace Old Atom Type with a New Type',
     &        /,4x,'(7) Assign Connectivities based on Distance',
     &        /,4x,'(8) Convert Units from Bohrs to Angstroms',
     &        /,4x,'(9) Invert thru Origin to give Mirror Image',
     &        /,3x,'(10) Translate All Atoms by an X,Y,Z-Vector',
     &        /,3x,'(11) Translate Center of Mass to the Origin',
     &        /,3x,'(12) Translate a Specified Atom to the Origin',
     &        /,3x,'(13) Translate and Rotate to Inertial Frame',
     &        /,3x,'(14) Move to Specified Rigid Body Coordinates',
     &        /,3x,'(15) Move Stray Molecules into Periodic Box',
     &        /,3x,'(16) Delete Molecules outside of Periodic Box',
     &        /,3x,'(17) Create and Fill a Periodic Boundary Box',
     &        /,3x,'(18) Soak Current Molecule in Box of Solvent',
     &        /,3x,'(19) Append a Second XYZ File to Current One')
c
c     get the desired type of coordinate file modification
c
   20 continue
      nmode = 19
      mode = -1
      do while (mode.lt.0 .or. mode.gt.nmode)
         mode = 0
         write (iout,30)
   30    format (/,' Number of the Desired Choice [<CR>=Exit] :  ',$)
         read (input,40,err=20,end=50)  mode
   40    format (i10)
   50    continue
      end do
c
c     get the offset value to be used in atom renumbering
c
      if (mode .eq. 1) then
   60    continue
         write (iout,70)
   70    format (/,' Offset used to Renumber the Current Atoms :  ',$)
         read (input,80,err=60)  offset
   80    format (i10)
         write = .true.
      end if
c
c     remove a specified list of individual atoms
c
      if (mode .eq. 2) then
         nlist = 0
         do i = 1, n
            list(i) = 0
         end do
         write (iout,90)
   90    format (/,' Numbers of the Atoms to be Removed :  ',$)
         read (input,100)  record
  100    format (a120)
         read (record,*,err=110,end=110)  (list(i),i=1,n)
  110    continue
         do while (list(nlist+1) .ne. 0)
            nlist = nlist + 1
         end do
         do i = 1, nlist
            if (list(i) .gt. n)  list(i) = n
            if (list(i) .lt. -n)  list(i) = -n
         end do
         call sort4 (nlist,list)
         do i = nlist, 1, -1
            if (i .gt. 1) then
               if (list(i-1) .lt. 0) then
                  do j = abs(list(i)), abs(list(i-1)), -1
                     call delete (j)
                  end do
               else if (list(i) .gt. 0) then
                  call delete (list(i))
               end if
            else if (list(i) .gt. 0) then
               call delete (list(i))
            end if
         end do
         write = .true.
         goto 20
      end if
c
c     remove all atoms with any of a specified list of atom types
c
      if (mode .eq. 3) then
         nlist = 0
         do i = 1, n
            list(i) = 0
         end do
         write (iout,120)
  120    format (/,' Atom Types to be Removed :  ',$)
         read (input,130)  record
  130    format (a120)
         read (record,*,err=140,end=140)  (list(i),i=1,n)
  140    continue
         do while (list(nlist+1) .ne. 0)
            nlist = nlist + 1
         end do
         natom = n
         do i = natom, 1, -1
            it = type(i)
            do j = 1, nlist
               if (list(j) .eq. it) then
                  call delete (i)
                  goto 150
               end if
            end do
  150       continue
         end do
         write = .true.
         goto 20
      end if
c
c     remove all atoms that are inactive and lie outside all cutoffs
c
      if (mode .eq. 4) then
         call active
         call cutoffs
         nlist = 0
         do i = 1, n
            keep(i) = 0
         end do
         cut2 = 0.0d0
         if (vdwcut .le. 1000.0d0)  cut2 = max(vdwcut**2,cut2)
         if (chgcut .le. 1000.0d0)  cut2 = max(chgcut**2,cut2)
         if (dplcut .le. 1000.0d0)  cut2 = max(dplcut**2,cut2)
         if (mpolecut .le. 1000.0d0)  cut2 = max(mpolecut**2,cut2)
         if (cut2 .eq. 0.0d0)  cut2 = 1.0d16
         do i = 1, n
            if (.not. use(i)) then
               do j = 1, n12(i)
                  keep(i12(j,i)) = i
               end do
               do j = 1, n13(i)
                  keep(i13(j,i)) = i
               end do
               do j = 1, n14(i)
                  keep(i14(j,i)) = i
               end do
               xi = x(i)
               yi = y(i)
               zi = z(i)
               do j = 1, n
                  if (use(j)) then
                     if (keep(j) .eq. i)  goto 160
                     dist2 = (x(j)-xi)**2 + (y(j)-yi)**2 + (z(j)-zi)**2
                     if (dist2 .le. cut2)  goto 160
                  end if
               end do
               nlist = nlist + 1
               list(nlist) = i
  160          continue
            end if
         end do
         do i = nlist, 1, -1
            call delete (list(i))
         end do
         write = .true.
         goto 20
      end if
c
c     insert a specified list of individual atoms
c
      if (mode .eq. 5) then
         nlist = 0
         do i = 1, n
            list(i) = 0
         end do
         write (iout,170)
  170    format (/,' Numbers of the Atoms to be Inserted :  ',$)
         read (input,180)  record
  180    format (a120)
         read (record,*,err=190,end=190)  (list(i),i=1,n)
  190    continue
         do while (list(nlist+1) .ne. 0)
            nlist = nlist + 1
         end do
         call sort4 (nlist,list)
         do i = nlist, 1, -1
            if (i .gt. 1) then
               if (list(i-1) .lt. 0) then
                  do j = abs(list(i-1)), abs(list(i))
                     call insert (j)
                  end do
               else if (list(i) .gt. 0) then
                  call insert (list(i))
               end if
            else if (list(i) .gt. 0) then
               call insert (list(i))
            end if
         end do
         write = .true.
         goto 20
      end if
c
c     get an old atom type and new atom type for replacement
c
      if (mode .eq. 6) then
  200    continue
         oldtype = 0
         newtype = 0
         write (iout,210)
  210    format (/,' Numbers of the Old and New Atom Types :  ',$)
         read (input,220)  record
  220    format (a120)
         read (record,*,err=200,end=200)  oldtype,newtype
         do i = 1, n
            if (type(i) .eq. oldtype)  type(i) = newtype
         end do
         call katom
         write = .true.
         goto 20
      end if
c
c     assign atom connectivities based on interatomic distances
c
      if (mode .eq. 7) then
         call unitcell
         call lattice
         do i = 1, n
            rad(i) = 0.77d0
            atmnum = atomic(i)
            if (atmnum .eq. 0)  rad(i) = 0.00d0
            if (atmnum .eq. 1)  rad(i) = 0.37d0
            if (atmnum .eq. 2)  rad(i) = 0.32d0
            if (atmnum .eq. 6)  rad(i) = 0.77d0
            if (atmnum .eq. 7)  rad(i) = 0.75d0
            if (atmnum .eq. 8)  rad(i) = 0.73d0
            if (atmnum .eq. 9)  rad(i) = 0.71d0
            if (atmnum .eq. 10)  rad(i) = 0.69d0
            if (atmnum .eq. 14)  rad(i) = 1.11d0
            if (atmnum .eq. 15)  rad(i) = 1.06d0
            if (atmnum .eq. 16)  rad(i) = 1.02d0
            if (atmnum .eq. 17)  rad(i) = 0.99d0
            if (atmnum .eq. 18)  rad(i) = 0.97d0
            if (atmnum .eq. 35)  rad(i) = 1.14d0
            if (atmnum .eq. 36)  rad(i) = 1.10d0
            if (atmnum .eq. 53)  rad(i) = 1.33d0
            if (atmnum .eq. 54)  rad(i) = 1.30d0
            rad(i) = 1.1d0 * rad(i)
         end do
         do i = 1, n
            n12(i) = 0
         end do
         do i = 1, n-1
            xi = x(i)
            yi = y(i)
            zi = z(i)
            ri = rad(i)
            do j = i+1, n
               xr = x(j) - xi
               yr = y(j) - yi
               zr = z(j) - zi
               rij = ri + rad(j)
               dij = sqrt(xr*xr + yr*yr + zr*zr)
               if (dij .lt. rij) then
                  n12(i) = n12(i) + 1
                  i12(n12(i),i) = j
                  n12(j) = n12(j) + 1
                  i12(n12(j),j) = i
               end if
            end do
         end do
         do i = 1, n
            call sort (n12(i),i12(1,i))
         end do
         write = .true.
         goto 20
      end if
c
c     convert the coordinate units from Bohrs to Angstroms
c
      if (mode .eq. 8) then
         do i = 1, n
            x(i) = x(i) * bohr
            y(i) = y(i) * bohr
            z(i) = z(i) * bohr
         end do
         write = .true.
         goto 20
      end if
c
c     get mirror image by inverting coordinates through origin
c
      if (mode .eq. 9) then
         do i = 1, n
            x(i) = -x(i)
            y(i) = -y(i)
            z(i) = -z(i)
         end do
         write = .true.
         goto 20
      end if
c
c     translate the entire system by a specified x,y,z-vector
c
      if (mode .eq. 10) then
         xr = 0.0d0
         yr = 0.0d0
         zr = 0.0d0
         write (iout,230)
  230    format (/,' Enter Translation Vector Components :  ',$)
         read (input,240)  record
  240    format (a120)
         read (record,*,err=250,end=250)  xr,yr,zr
  250    continue
         do i = 1, n
            x(i) = x(i) + xr
            y(i) = y(i) + yr
            z(i) = z(i) + zr
         end do
         write = .true.
         goto 20
      end if
c
c     translate the center of mass to the coordinate origin
c
      if (mode .eq. 11) then
         xcm = 0.0d0
         ycm = 0.0d0
         zcm = 0.0d0
         norm = 0.0d0
         do i = 1, n
            weigh = mass(i)
            xcm = xcm + x(i)*weigh
            ycm = ycm + y(i)*weigh
            zcm = zcm + z(i)*weigh
            norm = norm + weigh
         end do
         xcm = xcm / norm
         ycm = ycm / norm
         zcm = zcm / norm
         do i = 1, n
            x(i) = x(i) - xcm
            y(i) = y(i) - ycm
            z(i) = z(i) - zcm
         end do
         write = .true.
         goto 20
      end if
c
c     translate to place a specified atom at the origin
c
      if (mode .eq. 12) then
         write (iout,260)
  260    format (/,' Number of the Atom to Move to the Origin :  ',$)
         read (input,270)  origin
  270    format (i10)
         xorig = x(origin)
         yorig = y(origin)
         zorig = z(origin)
         do i = 1, n
            x(i) = x(i) - xorig
            y(i) = y(i) - yorig
            z(i) = z(i) - zorig
         end do
         write = .true.
         goto 20
      end if
c
c     translate and rotate into standard orientation
c
      if (mode .eq. 13) then
         call inertia (2)
         write = .true.
         goto 20
      end if
c
c     translate and rotate to specified rigid body coordinates
c
      if (mode .eq. 14) then
         xcm = 0.0d0
         ycm = 0.0d0
         zcm = 0.0d0
         phi = 0.0d0
         theta = 0.0d0
         psi = 0.0d0
         write (iout,280)
  280    format (/,' Enter Rigid Body Coordinates :  ',$)
         read (input,290)  record
  290    format (a120)
         read (record,*,err=300,end=300)  xcm,ycm,zcm,phi,theta,psi
  300    continue
         call inertia (2)
         phi = phi / radian
         theta = theta / radian
         psi = psi / radian
         cphi = cos(phi)
         sphi = sin(phi)
         ctheta = cos(theta)
         stheta = sin(theta)
         cpsi = cos(psi)
         spsi = sin(psi)
         a(1,1) = ctheta * cphi
         a(2,1) = spsi*stheta*cphi - cpsi*sphi
         a(3,1) = cpsi*stheta*cphi + spsi*sphi
         a(1,2) = ctheta * sphi
         a(2,2) = spsi*stheta*sphi + cpsi*cphi
         a(3,2) = cpsi*stheta*sphi - spsi*cphi
         a(1,3) = -stheta
         a(2,3) = ctheta * spsi
         a(3,3) = ctheta * cpsi
         do i = 1, n
            xorig = x(i)
            yorig = y(i)
            zorig = z(i)
            x(i) = a(1,1)*xorig + a(2,1)*yorig + a(3,1)*zorig + xcm
            y(i) = a(1,2)*xorig + a(2,2)*yorig + a(3,2)*zorig + ycm
            z(i) = a(1,3)*xorig + a(2,3)*yorig + a(3,3)*zorig + zcm
         end do
         write = .true.
         goto 20
      end if
c
c     move stray molecules back into original periodic box
c
      if (mode .eq. 15) then
         call unitcell
         if (use_bounds) then
            call lattice
            call molecule
            call bounds
            write = .true.
         end if
         goto 20
      end if
c
c     remove molecules to trim periodic box to a smaller size
c
      if (mode .eq. 16) then
         xbox = 0.0d0
         ybox = 0.0d0
         zbox = 0.0d0
         do while (xbox .eq. 0.0d0)
            write (iout,310)
  310       format (/,' Enter Periodic Box Dimensions (X,Y,Z) :  ',$)
            read (input,320)  record
  320       format (a120)
            read (record,*,err=330,end=330)  xbox,ybox,zbox
  330       continue
            if (ybox .eq. 0.0d0)  ybox = xbox
            if (zbox .eq. 0.0d0)  zbox = xbox
         end do
         call lattice
         call molecule
         do i = 1, n
            list(i) = 1
         end do
         do i = 1, nmol
            init = imol(1,i)
            stop = imol(2,i)
            xcm = 0.0d0
            ycm = 0.0d0
            zcm = 0.0d0
            do j = init, stop
               k = kmol(j)
               weigh = mass(k)
               xcm = xcm + x(k)*weigh
               ycm = ycm + y(k)*weigh
               zcm = zcm + z(k)*weigh
            end do
            weigh = molmass(i)
            xcm = xcm / weigh
            ycm = ycm / weigh
            zcm = zcm / weigh
            if (abs(xcm).gt.xbox2 .or. abs(ycm).gt.ybox2
     &                .or. abs(zcm).gt.zbox2) then
               do j = init, stop
                  k = kmol(j)
                  list(k) = 0
               end do
            end if
         end do
         k = 0
         do i = 1, n
            if (list(i) .ne. 0) then
               k = k + 1
               keep(k) = i
               list(i) = k
            end if
         end do
         n = k
         do k = 1, n
            i = keep(k)
            name(k) = name(i)
            x(k) = x(i)
            y(k) = y(i)
            z(k) = z(i)
            type(k) = type(i)
            n12(k) = n12(i)
            do j = 1, n12(k)
               i12(j,k) = list(i12(j,i))
            end do
         end do
         write = .true.
      end if
c
c     create a random box full of the current coordinates file
c
      if (mode .eq. 17) then
         write (iout,340)
  340    format (/,' Enter Number of Molecules in Box :  ',$)
         read (input,350)  nmolecule
  350    format (i10)
         xbox = 0.0d0
         ybox = 0.0d0
         zbox = 0.0d0
         do while (xbox .eq. 0.0d0)
            write (iout,360)
  360       format (/,' Enter Periodic Box Dimensions (X,Y,Z) :  ',$)
            read (input,370)  record
  370       format (a120)
            read (record,*,err=380,end=380)  xbox,ybox,zbox
  380       continue
            if (ybox .eq. 0.0d0)  ybox = xbox
            if (zbox .eq. 0.0d0)  zbox = xbox
         end do
         ixyz = freeunit ()
         xyzfile = filename(1:leng)//'.xyz'
         call version (xyzfile,'new')
         open (unit=ixyz,file=xyzfile,status='new')
         do k = 1, nmolecule
            offset = (k-1) * n
            xran = xbox * random ()
            yran = ybox * random ()
            zran = zbox * random ()
            do i = 1, n
               j = i + offset
               name(j) = name(i)
               type(j) = type(i)
               x(j) = x(i) + xran
               y(j) = y(i) + yran
               z(j) = z(i) + zran
               n12(j) = n12(i)
               do m = 1, n12(i)
                  i12(m,j) = i12(m,i) + offset
               end do
            end do
         end do
         n = nmolecule * n
         call prtxyz (ixyz)
         close (unit=ixyz)
         write (iout,390)  xyzfile
  390    format (/,' New Coordinates written to :  ',a)
         write = .false.
      end if
c
c     solvate the current system by insertion into a solvent box
c
      if (mode .eq. 18) then
         call soak
         write = .true.
      end if
c
c     append a second file to the current coordinates file
c
      if (mode .eq. 19) then
         call makeref (1)
         call getxyz
         call merge (1)
         write = .true.
         goto 20
      end if
c
c     perform deallocation of some local arrays
c
      deallocate (list)
      deallocate (keep)
      deallocate (rad)
c
c     write out the new coordinates file in its new format
c
      if (write) then
         ixyz = freeunit ()
         xyzfile = filename(1:leng)//'.xyz'
         call version (xyzfile,'new')
         open (unit=ixyz,file=xyzfile,status='new')
         if (offset .eq. 0) then
            call prtxyz (ixyz)
         else
            if (ltitle .eq. 0) then
               write (ixyz,400)  n
  400          format (i6)
            else
               write (ixyz,410)  n,title(1:ltitle)
  410          format (i6,2x,a)
            end if
            do i = 1, n
               write (ixyz,420)  i+offset,name(i),x(i),y(i),z(i),
     &                           type(i),(i12(j,i)+offset,j=1,n12(i))
  420          format (i6,2x,a3,3f12.6,5i6)
            end do
         end if
         close (unit=ixyz)
         write (iout,430)  xyzfile
  430    format (/,' New Coordinates written to File :  ',a)
      end if
c
c     perform any final tasks before program exit
c
      call final
      end
c
c
c     ##############################################################
c     ##                                                          ##
c     ##  subroutine soak  --  place a solute into a solvent box  ##
c     ##                                                          ##
c     ##############################################################
c
c
c     "soak" takes a currently defined solute system and places
c     it into a solvent box, with removal of any solvent molecules
c     that overlap the solute
c
c
      subroutine soak
      implicit none
      include 'sizes.i'
      include 'atoms.i'
      include 'bound.i'
      include 'iounit.i'
      include 'molcul.i'
      include 'refer.i'
      integer i,k,isolv
      integer ntot,freeunit
      real*8 xi,yi,zi
      real*8 xr,yr,zr,rik2
      real*8 close,close2
      logical, allocatable :: remove(:)
      character*120 solvfile
      external merge
c
c
c     make a copy of the solute coordinates and connectivities
c
      call makeref (1)
c
c     read the coordinates for the solvent box
c
   10 continue
      write (iout,20)
   20 format (/,' Enter Name of Solvent Box Coordinates :  ',$)
      read (input,30)  solvfile
   30 format (a120)
      call suffix (solvfile,'xyz','old')
      isolv = freeunit ()
      open (unit=isolv,file=solvfile,status='old',err=10)
      rewind (unit=isolv)
      call readxyz (isolv)
      close (unit=isolv)
c
c     combine solute and solvent into a single coordinate set
c
      call merge (1)
c
c     count number of molecules and set lattice parameters
c
      call molecule
      call unitcell
      call lattice
c
c     perform dynamic allocation of some local arrays
c
      allocate (remove(nmol))
c
c     initialize the list of solvent molecules to be deleted
c
      do i = 1, nmol
         remove(i) = .false.
      end do
c
c     search for close contacts between solute and solvent
c
      close = 1.5d0
      close2 = close * close
      do i = 1, nref(1)
         xi = x(i)
         yi = y(i)
         zi = z(i)
         do k = nref(1)+1, n
            xr = x(k) - xi
            yr = y(k) - yi
            zr = z(k) - zi
            call image (xr,yr,zr)
            rik2 = xr*xr + yr*yr + zr*zr
            if (rik2 .lt. close2)  remove(molcule(k)) = .true.
         end do
      end do
c
c     remove solvent molecules that are too close to the solute
c
      ntot = n
      do i = ntot, nref(1)+1, -1
         if (remove(molcule(i)))  call delete (i)
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (remove)
      return
      end
