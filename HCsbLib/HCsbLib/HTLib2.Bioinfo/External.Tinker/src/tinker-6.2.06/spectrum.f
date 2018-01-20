c
c
c     ###################################################
c     ##  COPYRIGHT (C)  2000  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     #################################################################
c     ##                                                             ##
c     ##  program spectrum  --  power spectrum from autocorrelation  ##
c     ##                                                             ##
c     #################################################################
c
c
c     "spectrum" computes a power spectrum over a wavelength range
c     from the velocity autocorrelation as a function of time
c
c
      program spectrum
      implicit none
      include 'files.i'
      include 'iounit.i'
      include 'math.i'
      include 'units.i'
      integer maxvel,maxfreq
      parameter (maxvel=100000)
      parameter (maxfreq=5000)
      integer i,k,nsamp
      integer ivel,nvel
      integer freeunit
      real*8 factor,aver
      real*8 norm,step
      real*8 freq,time
      real*8 vel(0:maxvel)
      real*8 intense(maxfreq)
      logical exist,done
      character*120 velfile
      character*120 record
      character*120 string
c
c
c     perform the standard initialization functions
c
      call initial
c
c     try to get a filename from the command line arguments
c
      call nextarg (velfile,exist)
      if (exist) then
         call basefile (velfile)
         call suffix (velfile,'vel','old')
         inquire (file=velfile,exist=exist)
      end if
c
c     ask for the velocity autocorrelation data filename
c
      do while (.not. exist)
         write (iout,10)
   10    format (/,' Enter Name of Velocity Autocorrelation',
     &              ' File :  ',$)
         read (input,20)  velfile
   20    format (a120)
         call basefile (velfile)
         call suffix (velfile,'vel','old')
         inquire (file=velfile,exist=exist)
      end do
c
c     get the time step between autocorrelation data points
c
      step = -1.0d0
      call nextarg (string,exist)
      if (exist)  read (string,*,err=30,end=30)  step
   30 continue
      if (step .le. 0.0d0) then
         write (iout,40)
   40    format (/,' Enter Time Step for Autocorrelation Data',
     &              ' [1.0 fs] :  ',$)
         read (input,50)  step
   50    format (f20.0)
      end if
      if (step .le. 0.0d0)  step = 1.0d0
      step = 0.001d0 * step
c
c     open the velocity autocorrelation data file
c
      ivel = freeunit ()
      open (unit=ivel,file=velfile,status='old')
      rewind (unit=ivel)
c
c     read through file headers to the start of the data
c
      done = .false.
      do while (.not. done)
         read (ivel,60)  record
   60    format (a120)
         if (record(4:13) .eq. 'Separation') then
            done = .true.
            read (ivel,70)
   70       format ()
         end if
      end do
c
c     read the velocity autocorrelation as a function of time
c
      do i = 1, maxvel
         read (ivel,80,err=90,end=90)  record
   80    format (a120)
         read (record,*)  k,nsamp,aver,norm
         nvel = k
         vel(k) = norm
      end do
   90 continue
c
c     compute the power spectrum via discrete Fourier transform
c
      factor = 2.0d0 * pi * lightspd
      do i = 1, maxfreq
         freq = factor * dble(i)
         intense(i) = 0.0d0
         do k = 0, nvel
            time = step * dble(k)
            intense(i) = intense(i) + vel(k)*cos(freq*time)
         end do
         intense(i) = 1000.0d0 * step * intense(i)
      end do
c
c     print the power spectrum intensity at each wavelength
c
      write (iout,100)
  100 format (/,' Power Spectrum from Velocity Autocorrelation :',
     &        //,4x,'Frequency (cm-1)',10x,'Intensity',/)
      do i = 1, maxfreq
         write (iout,110)  dble(i),intense(i)
  110    format (3x,f12.2,8x,f16.6)
      end do
      end
