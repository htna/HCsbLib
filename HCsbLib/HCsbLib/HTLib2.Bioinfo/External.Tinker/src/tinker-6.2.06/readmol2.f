c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1995  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     #################################################################
c     ##                                                             ##
c     ##  subroutine readmol2  --  read in a Sybyl MOL2 format file  ##
c     ##                                                             ##
c     #################################################################
c
c
c     "readmol2" gets a set of Sybyl MOL2 coordinates from
c     an external disk file
c
c
      subroutine readmol2 (isyb)
      implicit none
      include 'sizes.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'couple.i'
      include 'files.i'
      include 'iounit.i'
      include 'titles.i'
      integer i,j,k,m,ia,ib,isyb
      integer nbond,number
      integer next,trimtext
      logical exist,opened
      character*10 atmnam
      character*120 sybylfile
      character*120 record
      character*120 string
c
c
c     open the input file if it has not already been done
c
      inquire (unit=isyb,opened=opened)
      if (.not. opened) then
         sybylfile = filename(1:leng)//'.mol2'
         call version (sybylfile,'old')
         inquire (file=sybylfile,exist=exist)
         if (exist) then
            open (unit=isyb,file=sybylfile,status='old')
            rewind (unit=isyb)
         else
            write (iout,10)
   10       format (/,' READMOL2  --  Unable to Find the TRIPOS',
     &                 ' Sybyl MOL2 File')
            call fatal
         end if
      end if
c
c     get title line and get the number of atoms and bonds
c
      do i = 1, 1000000
         read (isyb,20)  record
   20    format (a120)
         next = 1
         call gettext (record,string,next)
         call upcase (string)
         if (string .eq. '@<TRIPOS>MOLECULE') then
            read (isyb,30)  title
   30       format (a120)
            ltitle = trimtext (title)
            read (isyb,40)  record
   40       format (a120)
            read (record,*)  n,nbond
            goto 50
         end if
      end do
   50 continue
c
c     check for too few or too many total atoms in the file
c
      if (n .le. 0) then
         write (iout,60)
   60    format (/,' READMOL2  --  The Coordinate File Does Not',
     &              ' Contain Any Atoms')
         call fatal
      else if (n .gt. maxatm) then
         write (iout,70)  maxatm
   70    format (/,' READMOL2  --  The Maximum of',i8,' Atoms',
     &              ' has been Exceeded')
         call fatal
      end if
c
c     read the atom names and coordinates
c
      do i = 1, 1000000
         read (isyb,80)  record
   80    format (a120)
         next = 1
         call gettext (record,string,next)
         call upcase (string)
         if (string .eq. '@<TRIPOS>ATOM') then
            do j = 1, n
               read (isyb,90)  record
   90          format (a120)
               read (record,*)  number
               next = 1
               call getword (record,atmnam,next)
               string = record(next:120)
               read (string,*)  x(j),y(j),z(j)
               call getword (record,atmnam,next)
               name(j) = atmnam(1:3)
               do k = 1, 3
                  if (atmnam(k:k) .eq. '.') then
                     do m = k, 3
                        name(j)(m:m) = ' '
                     end do
                  end if
               end do
               type(j) = 0
            end do
            goto 100
         end if
      end do
  100 continue
c
c     read the bond list to get attached atom lists
c
      do i = 1, 1000000
         read (isyb,110)  record
  110    format (a120)
         next = 1
         call gettext (record,string,next)
         call upcase (string)
         if (string .eq. '@<TRIPOS>BOND') then
            do j = 1, nbond
               read (isyb,120)  record
  120          format (a120)
               read (record,*)  number,ia,ib
               n12(ia) = n12(ia) + 1
               i12(n12(ia),ia) = ib
               n12(ib) = n12(ib) + 1
               i12(n12(ib),ib) = ia
            end do
            goto 130
         end if
      end do
  130 continue
c
c     for each atom, sort its list of attached atoms
c
      do i = 1, n
         call sort (n12(i),i12(1,i))
      end do
      if (.not. opened)  close (unit=isyb)
      return
      end
