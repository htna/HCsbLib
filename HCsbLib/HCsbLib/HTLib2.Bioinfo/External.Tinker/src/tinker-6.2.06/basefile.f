c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1996  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ################################################################
c     ##                                                            ##
c     ##  subroutine basefile  --  get base prefix from a filename  ##
c     ##                                                            ##
c     ################################################################
c
c
c     "basefile" extracts from an input filename the portion
c     consisting of any directory name and the base filename
c
c
      subroutine basefile (string)
      implicit none
      include 'ascii.i'
      include 'files.i'
      integer i,k,trimtext
      character*1 letter
      character*120 string
c
c
c     store the input filename and find its full length
c
      filename = string
      leng = trimtext (string)
c
c     count the number of characters prior to any extension
c
      k = leng
      do i = 1, leng
         letter = string(i:i)
         if (letter .eq. '/')  k = leng
c        if (letter .eq. '\')  k = leng
         if (ichar(letter) .eq. backslash)  k = leng
         if (letter .eq. ']')  k = leng
         if (letter .eq. ':')  k = leng
         if (letter .eq. '~')  k = leng
         if (letter .eq. '.')  k = i - 1
      end do
      leng = min(leng,k)
c
c     find the length of any directory name prefix
c
      k = 0
      do i = leng, 1, -1
         letter = string(i:i)
         if (letter .eq. '/')  k = i
c        if (letter .eq. '\')  k = i
         if (ichar(letter) .eq. backslash)  k = i
         if (letter .eq. ']')  k = i
         if (letter .eq. ':')  k = i
         if (letter .eq. '~')  k = i
c        if (letter .eq. '.')  k = i
         if (k .ne. 0)  goto 10
      end do
   10 continue
      ldir = k
c
c     read and store the keywords from the keyfile
c
      call getkey
c
c     get the information level and output style
c
      call control
      return
      end
