c
c
c     ###################################################
c     ##  COPYRIGHT (C)  1992  by  Jay William Ponder  ##
c     ##              All Rights Reserved              ##
c     ###################################################
c
c     ##############################################################
c     ##                                                          ##
c     ##  cutoff.i  --  cutoff distances for energy interactions  ##
c     ##                                                          ##
c     ##############################################################
c
c
c     vdwcut      cutoff distance for van der Waals interactions
c     chgcut      cutoff distance for charge-charge interactions
c     dplcut      cutoff distance for dipole-dipole interactions
c     mpolecut    cutoff distance for atomic multipole interactions
c     vdwtaper    distance at which van der Waals switching begins
c     chgtaper    distance at which charge-charge switching begins
c     dpltaper    distance at which dipole-dipole switching begins
c     mpoletaper  distance at which atomic multipole switching begins
c     ewaldcut    cutoff distance for direct space Ewald summation
c     usolvcut    cutoff distance for dipole solver preconditioner
c     use_ewald   logical flag governing use of Ewald summation
c     use_lights  logical flag governing use of method of lights
c     use_list    logical flag governing use of any neighbor lists
c     use_vlist   logical flag governing use of vdw neighbor list
c     use_clist   logical flag governing use of charge neighbor list
c     use_mlist   logical flag governing use of multipole neighbor list
c     use_ulist   logical flag governing use of preconditioner list
c
c
      real*8 vdwcut,chgcut
      real*8 dplcut,mpolecut
      real*8 vdwtaper,chgtaper
      real*8 dpltaper,mpoletaper
      real*8 ewaldcut,usolvcut
      logical use_ewald,use_lights
      logical use_list,use_vlist
      logical use_clist,use_mlist
      logical use_ulist
      common /cutoff/ vdwcut,chgcut,dplcut,mpolecut,vdwtaper,chgtaper,
     &                dpltaper,mpoletaper,ewaldcut,usolvcut,use_ewald,
     &                use_lights,use_list,use_vlist,use_clist,use_mlist,
     &                use_ulist
