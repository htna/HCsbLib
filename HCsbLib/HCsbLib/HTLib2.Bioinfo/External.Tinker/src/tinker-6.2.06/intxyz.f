c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     #############################################################
c     ##                                                         ##
c     ##  program intxyz  --  internal to Cartesian coordinates  ##
c     ##                                                         ##
c     #############################################################
c
c
c     "intxyz" takes as input an internal coordinates file,
c     converts to and then writes out Cartesian coordinates
c
c
      program intxyz
      implicit none
      include 'files.i'
      include 'iounit.i'
      include 'titles.i'
      integer ixyz,freeunit
      character*120 xyzfile
c
c
c     get and read the internal coordinates file;
c     conversion to Cartesians is done in "getint"
c
      call initial
      call getint
      write (iout,10)  title(1:ltitle)
   10 format (/,' Title :  ',a)
c
c     write out the Cartesian coordinates file
c
      ixyz = freeunit ()
      xyzfile = filename(1:leng)//'.xyz'
      call version (xyzfile,'new')
      open (unit=ixyz,file=xyzfile,status='new')
      call prtxyz (ixyz)
      close (unit=ixyz)
c
c     perform any final tasks before program exit
c
      call final
      end
