c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ############################################################
c     ##                                                        ##
c     ##  subroutine extra3  --  user defined extra potentials  ##
c     ##                                                        ##
c     ############################################################
c
c
c     "extra3" calculates any additional user defined potential
c     contribution and also partitions the energy among the atoms
c
c
      subroutine extra3
      implicit none
      include 'sizes.i'
      include 'action.i'
      include 'analyz.i'
      include 'atoms.i'
      include 'energi.i'
      integer i
c
c
c     zero out the energy due to extra potential terms
c
      nex = 0
      ex = 0.0d0
      do i = 1, n
         aex(i) = 0.0d0
      end do
c
c     add any user-defined extra potentials and partitioning
c
c     e = ......
c     nex = nex + 1
c     ex = ex + e
c     aex(i) = ......
c
      return
      end
