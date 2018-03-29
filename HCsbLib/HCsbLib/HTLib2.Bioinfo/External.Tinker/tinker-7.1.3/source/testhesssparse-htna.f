c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ####################################################################
c     ##                                                                ##
c     ##  program testhesssparse -- Hessian matrix test; cart. version  ##
c     ##                                                                ##
c     ####################################################################
c
c
c     "testhesssparse" computes sparse
c     Hessian matrices of the potential energy function with respect
c     to Cartesian coordinates
c     -- modified from testhess.f by htna
c
c
      program testhesssparse
      use sizes
      use atoms
      use files
      use hescut
      use inform
      use iounit
      use usage
      implicit none
      integer i,j,k,m
      integer ii,jj
      integer ixyz,ihes
      integer index,maxnum
      integer next,frame
      integer freeunit
      integer*8 maxhess, maxhess0
      integer errhindex, errh
      integer, allocatable :: hindex(:)
      integer, allocatable :: hinit(:,:)
      integer, allocatable :: hstop(:,:)
      real*8 energy,e,old,eps,eps0
      real*8 diff,delta,sum
      real*8, allocatable :: h(:)
      real*8, allocatable :: g(:,:)
      real*8, allocatable :: g0(:,:)
      real*8, allocatable :: hdiag(:,:)
      real*8, allocatable :: nhess(:,:,:,:)
      logical doanalyt
c     logical donumer
c     logical dograd,dofull
      logical exist,query
      logical identical
      character*1 answer
      character*1 axis(3)
      character*120 xyzfile
      character*120 hessfile
      character*120 record
      character*120 string
      external energy
      data axis  / 'X','Y','Z' /
c
c
c     set up the structure and mechanics calculation
c
      call initial
      call getxyz
      call mechanic
c
c     set difference threshhold via the energy precision
c
      delta = 1.0d-4
      if (digits .ge. 6)  delta = 1.0d-6
      if (digits .ge. 8)  delta = 1.0d-8
c
c     decide whether to do an analytical Hessian calculation
c
      doanalyt = .true.
cc    call nextarg (answer,exist)
cc    if (.not. exist) then
cc       write (iout,10)
cc 10    format (/,' Compute Analytical Hessian Matrix [Y] :  ',$)
cc       read (input,20)  record
cc 20    format (a120)
cc       next = 1
cc       call gettext (record,answer,next)
cc    end if
cc    call upcase (answer)
cc    if (answer .eq. 'N')  doanalyt = .false.
c
c     reopen the coordinates file and read the first structure
c
      frame = 0
      ixyz = freeunit ()
      xyzfile = filename
      call suffix (xyzfile,'xyz','old')
      open (unit=ixyz,file=xyzfile,status ='old')
      rewind (unit=ixyz)
      call readxyz (ixyz)
c
c     perform dynamic allocation of some local arrays
c     decide the size of sparse Hessian matrix : < 3*n*(3*n-1)/2
c
      allocate (hinit(3,n))
      allocate (hstop(3,n))
      allocate (g(3,n))
      allocate (g0(3,n))
      allocate (hdiag(3,n))
      if (n .le. maxnum)  allocate (nhess(3,n,3,n))
      maxhess0 = 3*n
      maxhess0 = maxhess0 * (maxhess0 -1)
      maxhess0 = maxhess0 / 2
      maxhess  = maxhess0
      do
         allocate (hindex(maxhess), stat=errhindex)
         allocate (h(maxhess)     , stat=errh     )
         if((errhindex .eq. 0) .and. (errh .eq. 0)) exit
         if(errhindex  .eq. 0) deallocate (hindex)
         if(errh       .eq. 0) deallocate (h)

   10    continue
         write (iout,11)  n, maxhess0
   11    format (/,' Select Smaller Size for Sparse Hessian Matrix '
     &             ' for ',i6,' Atoms [ <',i,'=3n(3n-1)/2 ] :  ',$)
         read (input,12,err=10)  maxhess
   12    format (i20)

         if (maxhess .ge. maxhess0) maxhess = maxhess0
         if (maxhess .le. 0       ) maxhess = maxhess0
      end do
c
c     perform analysis for each successive coordinate structure
c
      do while (.not. abort)
         frame = frame + 1
         if (frame .gt. 1) then
            write (iout,120)  frame
  120       format (/,' Analysis for Archive Structure :',8x,i8)
         end if
c
c     get the analytical Hessian matrix elements
c
         identical = .true.
         if (doanalyt) then
            if (verbose) then
               write (iout,130)
  130          format ()
            end if
CC          hesscut = 0.0d0
            call hessian (h,hinit,hstop,hindex,hdiag)
            call hessian (h,hinit,hstop,hindex,hdiag)
            call hessian (h,hinit,hstop,hindex,hdiag)
         end if
c
c     write out the diagonal Hessian elements for each atom
c
         if (doanalyt) then
            if (digits .ge. 8) then
               write (iout,230)
  230          format (/,' Diagonal Hessian Elements for Each Atom :',
     &                    //,6x,'Atom',21x,'X',19x,'Y',19x,'Z',/)
            else if (digits .ge. 6) then
               write (iout,240)
  240          format (/,' Diagonal Hessian Elements for Each Atom :',
     &                    //,6x,'Atom',19x,'X',17x,'Y',17x,'Z',/)
            else
               write (iout,250)
  250          format (/,' Diagonal Hessian Elements for Each Atom :',
     &                    //,6x,'Atom',17x,'X',15x,'Y',15x,'Z',/)
            end if
            do i = 1, n
               if (digits .ge. 8) then
                  write (iout,260)  i,(hdiag(j,i),j=1,3)
  260             format (i10,5x,3f20.8)
               else if (digits .ge. 6) then
                  write (iout,270)  i,(hdiag(j,i),j=1,3)
  270             format (i10,5x,3f18.6)
               else
                  write (iout,280)  i,(hdiag(j,i),j=1,3)
  280             format (i10,5x,3f16.4)
               end if
            end do
         end if
c
c     write out the Hessian trace as sum of diagonal elements
c
         if (doanalyt) then
            sum = 0.0d0
            do i = 1, n
               do j = 1, 3
                  sum = sum + hdiag(j,i)
               end do
            end do
            if (digits .ge. 8) then
               write (iout,290)  sum
  290          format (/,' Sum of Diagonal Hessian Elements :',6x,f20.8)
            else if (digits .ge. 6) then
               write (iout,300)  sum
  300          format (/,' Sum of Diagonal Hessian Elements :',6x,f18.6)
            else
               write (iout,310)  sum
  310          format (/,' Sum of Diagonal Hessian Elements :',6x,f16.4)
            end if
         end if
c
c     write out the full matrix of analytical Hessian elements
c
         if (doanalyt) then
            ihes = freeunit ()
            hessfile = filename(1:leng)//'.hes'
            call version (hessfile,'new')
            open (unit=ihes,file=hessfile,status='new')
            write (iout,360)  hessfile
  360       format (/,' Hessian Sparse Matrix written to File :  ',a40)
            write (ihes,370)
  370       format (/,' Diagonal Hessian Elements  (3 per Atom)',/)
            if (digits .ge. 8) then
               write (ihes,380)  ((hdiag(j,i),j=1,3),i=1,n)
  380          format (4f16.8)
            else if (digits .ge. 6) then
               write (ihes,390)  ((hdiag(j,i),j=1,3),i=1,n)
  390          format (5f14.6)
            else
               write (ihes,400)  ((hdiag(j,i),j=1,3),i=1,n)
  400          format (6f12.4)
            end if
            do i = 1, n
               do j = 1, 3
                  if (hinit(j,i) .le. hstop(j,i)) then
                     write (ihes,410)  i,axis(j)
  410                format (/,' Off-diagonal sparse Hessian'
     &                         ' Indexes/Elements for Atom',i6,1x,a1,/)
                     if (digits .ge. 8) then
                        write (ihes,420)
     &                        (hindex(k), h(k), k=hinit(j,i),hstop(j,i))
  420                   format (i6, f16.8, i6, f16.8, i6, f16.8,
     &                          i6, f16.8, i6, f16.8)
                     else if (digits .ge. 6) then
                        write (ihes,430)
     &                        (hindex(k), h(k), k=hinit(j,i),hstop(j,i))
  430                   format (i6, f14.6, i6, f14.6, i6, f14.6,
     &                          i6, f14.6, i6, f14.6)
                     else
                        write (ihes,440)
     &                        (hindex(k), h(k), k=hinit(j,i),hstop(j,i))
  440                   format (i6, f12.4, i6, f12.4, i6, f12.4,
     &                          i6, f12.4, i6, f12.4)
                     end if
                  end if
               end do
            end do
            close (unit=ihes)
         end if
c
c     attempt to read next structure from the coordinate file
c
         call readxyz (ixyz)
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (hindex)
      deallocate (hinit)
      deallocate (hstop)
      deallocate (h)
      deallocate (g)
      deallocate (g0)
      deallocate (hdiag)
      if (n .le. maxnum)  deallocate (nhess)
c
c     perform any final tasks before program exit
c
      call final
      end program testhesssparse
