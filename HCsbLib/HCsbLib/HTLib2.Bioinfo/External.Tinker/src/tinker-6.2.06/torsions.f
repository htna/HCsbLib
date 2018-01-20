c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ##########################################################
c     ##                                                      ##
c     ##  subroutine torsions  --  locate and store torsions  ##
c     ##                                                      ##
c     ##########################################################
c
c
c     "torsions" finds the total number of dihedral angles and
c     the numbers of the four atoms defining each dihedral angle
c
c
      subroutine torsions
      implicit none
      include 'sizes.i'
      include 'bond.i'
      include 'couple.i'
      include 'iounit.i'
      include 'tors.i'
      integer i,j,k
      integer ia,ib,ic,id
c
c
c     loop over all bonds, storing the atoms in each torsion
c
      ntors = 0
      do i = 1, nbond
         ib = ibnd(1,i)
         ic = ibnd(2,i)
         do j = 1, n12(ib)
            ia = i12(j,ib)
            if (ia .ne. ic) then
               do k = 1, n12(ic)
                  id = i12(k,ic)
                  if (id.ne.ib .and. id.ne.ia) then
                     ntors = ntors + 1
                     if (ntors .gt. maxtors) then
                        write (iout,10)
   10                   format (/,' TORSIONS  --  Too many Torsional',
     &                             ' Angles; Increase MAXTORS')
                        call fatal
                     end if
                     itors(1,ntors) = ia
                     itors(2,ntors) = ib
                     itors(3,ntors) = ic
                     itors(4,ntors) = id
                  end if
               end do
            end if
         end do
      end do
      return
      end
