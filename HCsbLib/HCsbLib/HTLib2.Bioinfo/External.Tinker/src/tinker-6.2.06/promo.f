c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1990  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ###############################################################
c     ##                                                           ##
c     ##  subroutine promo  --  version info and copywrite notice  ##
c     ##                                                           ##
c     ###############################################################
c
c
c     "promo" writes a short message containing information
c     about the TINKER version number and the copyright notice
c
c
      subroutine promo
      implicit none
      include 'iounit.i'
c
c
c     print out the informational header message
c
      write (iout,10)
   10 format (/,5x,70('#'),
     &        /,3x,74('#'),
     &        /,2x,'###',70x,'###',
     &        /,1x,'###',12x,'TINKER  ---  Software Tools for',
     &           ' Molecular Design',12x,'###',
     &        /,1x,'##',74x,'##',
     &        /,1x,'##',24x,'Version 6.2  February 2013',24x,'##',
     &        /,1x,'##',74x,'##',
     &        /,1x,'##',15x,'Copyright (c)  Jay William Ponder',
     &           '  1990-2013',15x,'##',
     &        /,1x,'###',27x,'All Rights Reserved',26x,'###',
     &        /,2x,'###',70x,'###',
     &        /,3x,74('#'),
     &        /,5x,70('#'),/)
      return
      end
