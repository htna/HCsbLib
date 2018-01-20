c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1993  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ###############################################################
c     ##                                                           ##
c     ##  subroutine eangang2  --  angle-angle Hessian; numerical  ##
c     ##                                                           ##
c     ###############################################################
c
c
c     "eangang2" calculates the angle-angle potential energy
c     second derivatives with respect to Cartesian coordinates
c     using finite difference methods
c
c
      subroutine eangang2 (i)
      implicit none
      include 'sizes.i'
      include 'angang.i'
      include 'angle.i'
      include 'atoms.i'
      include 'deriv.i'
      include 'group.i'
      include 'hessn.i'
      integer i,j,k,iangang
      integer ia,ib,ic,id,ie
      real*8 eps,fgrp
      real*8 old,term
      real*8, allocatable :: d0(:,:)
      logical proceed
      logical twosided
c
c
c     set stepsize for derivatives and default group weight
c
      eps = 1.0d-5
      fgrp = 1.0d0
      twosided = .false.
      if (n .le. 50)  twosided = .true.
c
c     perform dynamic allocation of some local arrays
c
      allocate (d0(3,n))
c
c     compute numerical angle-angle Hessian for current atom
c
      do iangang = 1, nangang
         j = iaa(1,iangang)
         k = iaa(2,iangang)
         ia = iang(1,j)
         ib = iang(2,j)
         ic = iang(3,j)
         id = iang(1,k)
         ie = iang(3,k)
c
c     decide whether to compute the current interaction
c
         proceed = (i.eq.ia .or. i.eq.ib .or. i.eq.ic
     &                 .or. i.eq.id .or. i.eq.ie)
         if (proceed .and. use_group)
     &      call groups (proceed,fgrp,ia,ib,ic,id,ie,0)
c
c     eliminate any duplicate atoms in the pair of angles
c
         if (proceed) then
            if (id.eq.ia .or. id.eq.ic)  then
               id = ie
               ie = 0
            else if (ie.eq.ia .or. ie.eq.ic) then
               ie = 0
            end if
            term = fgrp / eps
c
c     find first derivatives for the base structure
c
            if (.not. twosided) then
               call eangang2a (iangang)
               do j = 1, 3
                  d0(j,ia) = deaa(j,ia)
                  d0(j,ib) = deaa(j,ib)
                  d0(j,ic) = deaa(j,ic)
                  d0(j,id) = deaa(j,id)
                  if (ie .ne. 0)  d0(j,ie) = deaa(j,ie)
               end do
            end if
c
c     find numerical x-components via perturbed structures
c
            old = x(i)
            if (twosided) then
               x(i) = x(i) - 0.5d0*eps
               call eangang2a (iangang)
               do j = 1, 3
                  d0(j,ia) = deaa(j,ia)
                  d0(j,ib) = deaa(j,ib)
                  d0(j,ic) = deaa(j,ic)
                  d0(j,id) = deaa(j,id)
                  if (ie .ne. 0)  d0(j,ie) = deaa(j,ie)
               end do
            end if
            x(i) = x(i) + eps
            call eangang2a (iangang)
            x(i) = old
            do j = 1, 3
               hessx(j,ia) = hessx(j,ia) + term*(deaa(j,ia)-d0(j,ia))
               hessx(j,ib) = hessx(j,ib) + term*(deaa(j,ib)-d0(j,ib))
               hessx(j,ic) = hessx(j,ic) + term*(deaa(j,ic)-d0(j,ic))
               hessx(j,id) = hessx(j,id) + term*(deaa(j,id)-d0(j,id))
               if (ie .ne. 0)
     &            hessx(j,ie) = hessx(j,ie) + term*(deaa(j,ie)-d0(j,ie))
            end do
c
c     find numerical y-components via perturbed structures
c
            old = y(i)
            if (twosided) then
               y(i) = y(i) - 0.5d0*eps
               call eangang2a (iangang)
               do j = 1, 3
                  d0(j,ia) = deaa(j,ia)
                  d0(j,ib) = deaa(j,ib)
                  d0(j,ic) = deaa(j,ic)
                  d0(j,id) = deaa(j,id)
                  if (ie .ne. 0)  d0(j,ie) = deaa(j,ie)
               end do
            end if
            y(i) = y(i) + eps
            call eangang2a (iangang)
            y(i) = old
            do j = 1, 3
               hessy(j,ia) = hessy(j,ia) + term*(deaa(j,ia)-d0(j,ia))
               hessy(j,ib) = hessy(j,ib) + term*(deaa(j,ib)-d0(j,ib))
               hessy(j,ic) = hessy(j,ic) + term*(deaa(j,ic)-d0(j,ic))
               hessy(j,id) = hessy(j,id) + term*(deaa(j,id)-d0(j,id))
               if (ie .ne. 0)
     &            hessy(j,ie) = hessy(j,ie) + term*(deaa(j,ie)-d0(j,ie))
            end do
c
c     find numerical z-components via perturbed structures
c
            old = z(i)
            if (twosided) then
               z(i) = z(i) - 0.5d0*eps
               call eangang2a (iangang)
               do j = 1, 3
                  d0(j,ia) = deaa(j,ia)
                  d0(j,ib) = deaa(j,ib)
                  d0(j,ic) = deaa(j,ic)
                  d0(j,id) = deaa(j,id)
                  if (ie .ne. 0)  d0(j,ie) = deaa(j,ie)
               end do
            end if
            z(i) = z(i) + eps
            call eangang2a (iangang)
            z(i) = old
            do j = 1, 3
               hessz(j,ia) = hessz(j,ia) + term*(deaa(j,ia)-d0(j,ia))
               hessz(j,ib) = hessz(j,ib) + term*(deaa(j,ib)-d0(j,ib))
               hessz(j,ic) = hessz(j,ic) + term*(deaa(j,ic)-d0(j,ic))
               hessz(j,id) = hessz(j,id) + term*(deaa(j,id)-d0(j,id))
               if (ie .ne. 0)
     &            hessz(j,ie) = hessz(j,ie) + term*(deaa(j,ie)-d0(j,ie))
            end do
         end if
      end do
c
c     perform deallocation of some local arrays
c
      deallocate (d0)
      return
      end
c
c
c     ################################################################
c     ##                                                            ##
c     ##  subroutine eangang2a  --  angle-angle interaction derivs  ##
c     ##                                                            ##
c     ################################################################
c
c
c     "eangang2a" calculates the angle-angle first derivatives for
c     a single interaction with respect to Cartesian coordinates;
c     used in computation of finite difference second derivatives
c
c
      subroutine eangang2a (i)
      implicit none
      include 'sizes.i'
      include 'angang.i'
      include 'angle.i'
      include 'angpot.i'
      include 'atoms.i'
      include 'bound.i'
      include 'deriv.i'
      include 'math.i'
      integer i,j,k
      integer ia,ib,ic,id,ie
      real*8 angle,term
      real*8 dot,cosine
      real*8 dt1,deddt1
      real*8 dt2,deddt2
      real*8 xia,yia,zia
      real*8 xib,yib,zib
      real*8 xic,yic,zic
      real*8 xid,yid,zid
      real*8 xie,yie,zie
      real*8 xab,yab,zab
      real*8 xcb,ycb,zcb
      real*8 xdb,ydb,zdb
      real*8 xeb,yeb,zeb
      real*8 rab2,rcb2
      real*8 rdb2,reb2
      real*8 xp,yp,zp,rp
      real*8 xq,yq,zq,rq
      real*8 terma,termc
      real*8 termd,terme
      real*8 dedxia,dedyia,dedzia
      real*8 dedxib,dedyib,dedzib
      real*8 dedxic,dedyic,dedzic
      real*8 dedxid,dedyid,dedzid
      real*8 dedxie,dedyie,dedzie
c
c
c     set the coordinates of the involved atoms
c
      j = iaa(1,i)
      k = iaa(2,i)
      ia = iang(1,j)
      ib = iang(2,j)
      ic = iang(3,j)
      id = iang(1,k)
      ie = iang(3,k)
      xia = x(ia)
      yia = y(ia)
      zia = z(ia)
      xib = x(ib)
      yib = y(ib)
      zib = z(ib)
      xic = x(ic)
      yic = y(ic)
      zic = z(ic)
      xid = x(id)
      yid = y(id)
      zid = z(id)
      xie = x(ie)
      yie = y(ie)
      zie = z(ie)
c
c     zero out the first derivative components
c
      deaa(1,ia) = 0.0d0
      deaa(2,ia) = 0.0d0
      deaa(3,ia) = 0.0d0
      deaa(1,ib) = 0.0d0
      deaa(2,ib) = 0.0d0
      deaa(3,ib) = 0.0d0
      deaa(1,ic) = 0.0d0
      deaa(2,ic) = 0.0d0
      deaa(3,ic) = 0.0d0
      deaa(1,id) = 0.0d0
      deaa(2,id) = 0.0d0
      deaa(3,id) = 0.0d0
      deaa(1,ie) = 0.0d0
      deaa(2,ie) = 0.0d0
      deaa(3,ie) = 0.0d0
c
c     compute the values of the two bond angles
c
      xab = xia - xib
      yab = yia - yib
      zab = zia - zib
      xcb = xic - xib
      ycb = yic - yib
      zcb = zic - zib
      xdb = xid - xib
      ydb = yid - yib
      zdb = zid - zib
      xeb = xie - xib
      yeb = yie - yib
      zeb = zie - zib
      if (use_polymer) then
         call image (xab,yab,zab)
         call image (xcb,ycb,zcb)
         call image (xdb,ydb,zdb)
         call image (xeb,yeb,zeb)
      end if
      rab2 = xab*xab + yab*yab + zab*zab
      rcb2 = xcb*xcb + ycb*ycb + zcb*zcb
      rdb2 = xdb*xdb + ydb*ydb + zdb*zdb
      reb2 = xeb*xeb + yeb*yeb + zeb*zeb
      xp = ycb*zab - zcb*yab
      yp = zcb*xab - xcb*zab
      zp = xcb*yab - ycb*xab
      xq = yeb*zdb - zeb*ydb
      yq = zeb*xdb - xeb*zdb
      zq = xeb*ydb - yeb*xdb
      rp = sqrt(xp*xp + yp*yp + zp*zp)
      rq = sqrt(xq*xq + yq*yq + zq*zq)
      if (rp*rq .ne. 0.0d0) then
         dot = xab*xcb + yab*ycb + zab*zcb
         cosine = dot / sqrt(rab2*rcb2)
         cosine = min(1.0d0,max(-1.0d0,cosine))
         angle = radian * acos(cosine)
         dt1 = angle - anat(j)
         dot = xdb*xeb + ydb*yeb + zdb*zeb
         cosine = dot / sqrt(rdb2*reb2)
         cosine = min(1.0d0,max(-1.0d0,cosine))
         angle = radian * acos(cosine)
         dt2 = angle - anat(k)
c
c     get the energy and master chain rule terms for derivatives
c
         term = radian * aaunit * kaa(i)
         deddt1 = term * dt2
         deddt2 = term * dt1
c
c     find chain rule terms for the first bond angle deviation
c
         terma = -deddt1 / (rab2*rp)
         termc = deddt1 / (rcb2*rp)
         dedxia = terma * (yab*zp-zab*yp)
         dedyia = terma * (zab*xp-xab*zp)
         dedzia = terma * (xab*yp-yab*xp)
         dedxic = termc * (ycb*zp-zcb*yp)
         dedyic = termc * (zcb*xp-xcb*zp)
         dedzic = termc * (xcb*yp-ycb*xp)
c
c     find chain rule terms for the second bond angle deviation
c
         termd = -deddt2 / (rdb2*rq)
         terme = deddt2 / (reb2*rq)
         dedxid = termd * (ydb*zq-zdb*yq)
         dedyid = termd * (zdb*xq-xdb*zq)
         dedzid = termd * (xdb*yq-ydb*xq)
         dedxie = terme * (yeb*zq-zeb*yq)
         dedyie = terme * (zeb*xq-xeb*zq)
         dedzie = terme * (xeb*yq-yeb*xq)
c
c     get the central atom derivative terms by difference
c
         dedxib = -dedxia - dedxic - dedxid - dedxie
         dedyib = -dedyia - dedyic - dedyid - dedyie
         dedzib = -dedzia - dedzic - dedzid - dedzie
c
c     set the angle-angle interaction first derivatives
c
         deaa(1,ia) = deaa(1,ia) + dedxia
         deaa(2,ia) = deaa(2,ia) + dedyia
         deaa(3,ia) = deaa(3,ia) + dedzia
         deaa(1,ib) = deaa(1,ib) + dedxib
         deaa(2,ib) = deaa(2,ib) + dedyib
         deaa(3,ib) = deaa(3,ib) + dedzib
         deaa(1,ic) = deaa(1,ic) + dedxic
         deaa(2,ic) = deaa(2,ic) + dedyic
         deaa(3,ic) = deaa(3,ic) + dedzic
         deaa(1,id) = deaa(1,id) + dedxid
         deaa(2,id) = deaa(2,id) + dedyid
         deaa(3,id) = deaa(3,id) + dedzid
         deaa(1,ie) = deaa(1,ie) + dedxie
         deaa(2,ie) = deaa(2,ie) + dedyie
         deaa(3,ie) = deaa(3,ie) + dedzie
      end if
      return
      end
