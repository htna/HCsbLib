c
c
c     ################################################################
c     ##  COPYRIGHT (C) 2002 by Michael Schnieders & Jay W. Ponder  ##
c     ##                     All Rights Reserved                    ##
c     ################################################################
c
c     #############################################################
c     ##                                                         ##
c     ##  subroutine sktopt  --  send current optimization info  ##
c     ##                                                         ##
c     #############################################################
c
c
c     "sktopt" sends the current optimization info via a socket
c
c
      subroutine sktopt (ncycle,eopt)
      implicit none
      include 'sizes.i'
      include 'atoms.i'
      include 'deriv.i'
      include 'mpole.i'
      include 'polar.i'
      include 'potent.i'
      include 'socket.i'
      integer i,k,ncycle
      integer flag
      real*8 eopt
      real*8, allocatable :: px(:)
      real*8, allocatable :: py(:)
      real*8, allocatable :: pz(:)
c
c
c     check to see if the Server has been created
c
      runtyp = 2
      if (.not. skt_init)  call sktinit ()
      if (.not. use_socket)  return
c
c     save the current step number and energy
c
      cstep = ncycle
      cenergy = eopt
c
c     check to see if an update is needed
c
      flag = 1
      if (.not. skt_close)  call needupdate (flag)
      if (flag .eq. 0)  return
c
c     get the monitor for the update structure
c
      call getmonitor ()
c
c     load the coordinates and energy information
c
      call setcoordinates (n,x,y,z)
      call setstep (ncycle)
      call setenergy (eopt)
c
c     perform dynamic allocation of some local arrays
c
      allocate (px(n))
      allocate (py(n))
      allocate (pz(n))
c
c     load the gradient and induced dipole information
c
      do i = 1, n
         cdx(i) = desum(1,i)
         cdy(i) = desum(2,i)
         cdz(i) = desum(3,i)
         px(i) = 0.0d0
         py(i) = 0.0d0
         pz(i) = 0.0d0
      end do
      call setgradients (n,cdx,cdy,cdz)
      if (use_polar) then
          do i = 1, npole
             k = ipole(i)
             px(k) = uind(1,i)
             py(k) = uind(2,i)
             pz(k) = uind(3,i)
          end do
          call setinduced (n,px,py,pz)
      end if
c
c     perform deallocation of some local arrays
c
      deallocate (px)
      deallocate (py)
      deallocate (pz)
c
c     release the monitor for the system stucture
c
      call setupdated ()
      call releasemonitor ()
      return
      end
c
c
c     ##############################################################
c     ##                                                          ##
c     ##  subroutine sktdyn  --   send the current dynamics info  ##
c     ##                                                          ##
c     ##############################################################
c
c
c     "sktdyn" sends the current dynamics info via a socket
c
c
      subroutine sktdyn (istep,dt,epot)
      implicit none
      include 'sizes.i'
      include 'atoms.i'
      include 'moldyn.i'
      include 'mpole.i'
      include 'polar.i'
      include 'potent.i'
      include 'socket.i'
      integer i,k,istep
      integer flag
      real*8 dt,time,epot
      real*8, allocatable :: vx(:)
      real*8, allocatable :: vy(:)
      real*8, allocatable :: vz(:)
      real*8, allocatable :: ax(:)
      real*8, allocatable :: ay(:)
      real*8, allocatable :: az(:)
      real*8, allocatable :: px(:)
      real*8, allocatable :: py(:)
      real*8, allocatable :: pz(:)
c
c
c     check to see if the Java objects have been created
c
      runtyp = 1
      if (.not. skt_init)  call sktinit ()
      if (.not. use_socket)  return
c
c     save the current step number, time and energy
c
      cstep = istep
      cdt = dt
      cenergy = epot
c
c     check to see if we need to update the system info
c
      flag = 1
      if (.not. skt_close)  call needupdate (flag)
      if (flag .eq. 0)  return
c
c     get the monitor for the update structure
c
      call getmonitor ()
c
c     load the coordinated, time and energy information
c
      call setcoordinates (n,x,y,z)
      time = dble(istep) * dt
      call setmdtime (time)
      call setenergy (epot)
c
c     perform dynamic allocation of some local arrays
c
      allocate (vx(n))
      allocate (vy(n))
      allocate (vz(n))
      allocate (ax(n))
      allocate (ay(n))
      allocate (az(n))
      allocate (px(n))
      allocate (py(n))
      allocate (pz(n))
c
c     load the velocity and acceleration information
c
      do i = 1, n
         vx(i) = v(1,i)
         vy(i) = v(2,i)
         vz(i) = v(3,i)
         ax(i) = a(1,i)
         ay(i) = a(2,i)
         az(i) = a(3,i)
         px(i) = 0.0d0
         py(i) = 0.0d0
         pz(i) = 0.0d0
      end do
      call setvelocity (n,vx,vy,vz)
      call setacceleration (n,ax,ay,az)
      if (use_polar) then
          do i = 1, npole
             k = ipole(i)
             px(k) = uind(1,i)
             py(k) = uind(2,i)
             pz(k) = uind(3,i)
          end do
          call setinduced (n,px,py,pz)
      end if
c
c     perform deallocation of some local arrays
c
      deallocate (vx)
      deallocate (vy)
      deallocate (vz)
      deallocate (ax)
      deallocate (ay)
      deallocate (az)
      deallocate (px)
      deallocate (py)
      deallocate (pz)
c
c     release the monitor for the update stucture
c
      call setupdated ()
      call releasemonitor ()
      return
      end
c
c
c     ###############################################################
c     ##                                                           ##
c     ##  subroutine sktinit  --  initialize socket communication  ##
c     ##                                                           ##
c     ###############################################################
c
c
c     "sktinit" sets up socket communication with the graphical
c     user interface by starting a Java virtual machine, initiating
c     a server, and loading an object with system information
c
c
      subroutine sktinit
      implicit none
      include 'sizes.i'
      include 'atmtyp.i'
      include 'atoms.i'
      include 'charge.i'
      include 'couple.i'
      include 'files.i'
      include 'fields.i'
      include 'iounit.i'
      include 'inform.i'
      include 'keys.i'
      include 'polar.i'
      include 'socket.i'
      integer i
      integer flag
      real*8, allocatable :: b1(:)
      real*8, allocatable :: b2(:)
      real*8, allocatable :: b3(:)
      real*8, allocatable :: b4(:)
c
c
c     set initialization flag and test for socket usage
c
      skt_init = .true.
      use_socket = .true.
      call chksocket (flag)
      if (flag .eq. 0) then
         use_socket = .false.
         return
      end if
c
c     create the Java Virtual Machine
c
      call createjvm (flag)
      if (flag .eq. 0) then
         use_socket = .false.
         if (debug) then
            write (iout,10)
   10       format (/,' SKTINIT  --  Unable to Start Server for',
     &                 ' Java GUI Communication',
     &              /,' Check the LD_LIBRARY_PATH and CLASSPATH',
     &                 ' Environment Variables',/)
         end if
         return
      end if
c
c     create the TINKER system object
c
      call createsystem (n,nkey,flag)
      if (flag .eq. 0) then
         use_socket = .false.
         return
      end if
c
c     load the keyfile and coordinates information
c
      call setfile (filename)
      call setforcefield (forcefield)
      do i = 1, nkey
         call setkeyword (i,keyline(i))
      end do
      call setcoordinates (n,x,y,z)
c
c     perform dynamic allocation of some local arrays
c
      allocate (b1(n))
      allocate (b2(n))
      allocate (b3(n))
      allocate (b4(n))
c
c     load the atom connectivity information
c
      do i = 1, n
         b1(i) = i12(1,i)
         b2(i) = i12(2,i)
         b3(i) = i12(3,i)
         b4(i) = i12(4,i)
      end do
      call setconnectivity (n,b1,b2,b3,b4)
c
c     perform deallocation of some local arrays
c
      deallocate (b1)
      deallocate (b2)
      deallocate (b3)
      deallocate (b4)
c
c     load atom type information for the parameter set
c
      call setatomtypes (n,type)
      do i = 1, n
         call setname (i,name(i))
         call setstory (i,story(i))
      end do
      call setatomic (n,atomic)
      call setmass (n,mass)
      call setcharge (n,pchg)
c
c     create the TINKER server
c
      call createserver (flag)
      if (flag .eq. 0) then
         use_socket = .false.
         return
      end if
c
c     create the update object
c
      call createupdate (n,runtyp,npolar,flag)
      if (flag .eq. 0) then
         use_socket = .false.
         return
      end if
      return
      end
c
c
c     ###########################################################
c     ##                                                       ##
c     ##  subroutine sktkill  --  shutdown the server and JVM  ##
c     ##                                                       ##
c     ###########################################################
c
c
c     "sktkill" closes the server and Java virtual machine
c
c
      subroutine sktkill
      implicit none
      include 'sizes.i'
      include 'socket.i'
c
c
c     check to see if there is anything to close
c
      if (.not. use_socket)  return
      skt_close = .true.
c
c     load the final simulation results
c
      if (runtyp .eq. 1)  call sktdyn (cstep,cdt,cenergy)
      if (runtyp .eq. 2)  call sktopt (cstep,cenergy)
c
c     shutdown the TINKER server
c
      call destroyserver ()
c
c     shutdown the Java virtual machine
c
      call destroyjvm ()
      return
      end
