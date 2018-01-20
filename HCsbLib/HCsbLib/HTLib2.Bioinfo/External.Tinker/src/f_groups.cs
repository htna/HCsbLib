using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Tinker
{
public partial class Src
{
///
///
///     ###################################################
///     ##  COPYRIGHT (C)  1997  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ###############################################################
///     ##                                                           ##
///     ##  subroutine groups  --  group membership of set of atoms  ##
///     ##                                                           ##
///     ###############################################################
///
///
///     "groups" tests a set of atoms to see if all are members
///     of a single atom group or a pair of atom groups; if so,
///     then the correct intra- or intergroup weight is assigned
///
///
    public void groups( out bool proceed, out double weigh
                      , int ia, int ib, int ic
                      , int id, int ie, int ig) {                       ;//      subroutine groups (proceed,weigh,ia,ib,ic,id,ie,ig)
                                                                        ;//      implicit none
                                                                        ;//      include 'sizes.i'
                                                                        ;//      include 'group.i'
                                                                        ;//      integer ia,ib,ic
                                                                        ;//      integer id,ie,ig
      int iga,igb,igc                                                   ;//      integer iga,igb,igc
      int igd,ige,igg                                                   ;//      integer igd,ige,igg
      int nset                                                          ;//      integer nset
      int gmax,gmin                                                     ;//      integer gmax,gmin
                                                                        ;//      real*8 weigh
                                                                        ;//      logical proceed
///
///
///     determine the number of atoms in the set to be compared
///
      nset = 0                                                          ;//      nset = 0
      weigh = 0.0e0                                                     ;//      weigh = 0.0d0
      if (ig  !=  0) {                                                  ;//      if (ig .ne. 0) then
         nset = 6                                                       ;//         nset = 6
      } else if (ie  !=  0) {                                           ;//      else if (ie .ne. 0) then
         nset = 5                                                       ;//         nset = 5
      } else if (id  !=  0) {                                           ;//      else if (id .ne. 0) then
         nset = 4                                                       ;//         nset = 4
      } else if (ic  !=  0) {                                           ;//      else if (ic .ne. 0) then
         nset = 3                                                       ;//         nset = 3
      } else if (ib  !=  0) {                                           ;//      else if (ib .ne. 0) then
         nset = 2                                                       ;//         nset = 2
      } else if (ia  !=  0) {                                           ;//      else if (ia .ne. 0) then
         nset = 1                                                       ;//         nset = 1
      }                                                                 ;//      end if
///
///     check group membership for a set containing one atom
///
      if (nset  ==  1) {                                                ;//      if (nset .eq. 1) then
         iga = grplist[ia]                                              ;//         iga = grplist(ia)
         weigh = wgrp[iga,iga]                                          ;//         weigh = wgrp(iga,iga)
///
///     check group membership for a set containing two atoms
///
      } else if (nset  ==  2) {                                         ;//      else if (nset .eq. 2) then
         iga = grplist[ia]                                              ;//         iga = grplist(ia)
         igb = grplist[ib]                                              ;//         igb = grplist(ib)
         weigh = wgrp[iga,igb]                                          ;//         weigh = wgrp(iga,igb)
///
///     check group membership for a set containing three atoms
///
      } else if (nset  ==  3) {                                         ;//      else if (nset .eq. 3) then
         iga = grplist[ia]                                              ;//         iga = grplist(ia)
         igb = grplist[ib]                                              ;//         igb = grplist(ib)
         igc = grplist[ic]                                              ;//         igc = grplist(ic)
         if (iga == igb  ||  igb == igc) {                              ;//         if (iga.eq.igb .or. igb.eq.igc) then
            weigh = wgrp[iga,igc]                                       ;//            weigh = wgrp(iga,igc)
         } else if (iga  ==  igc) {                                     ;//         else if (iga .eq. igc) then
            weigh = wgrp[iga,igb]                                       ;//            weigh = wgrp(iga,igb)
         }                                                              ;//         end if
///
///     check group membership for a set containing four atoms
///
      } else if (nset  ==  4) {                                         ;//      else if (nset .eq. 4) then
         iga = grplist[ia]                                              ;//         iga = grplist(ia)
         igb = grplist[ib]                                              ;//         igb = grplist(ib)
         igc = grplist[ic]                                              ;//         igc = grplist(ic)
         igd = grplist[id]                                              ;//         igd = grplist(id)
         gmin = min(iga,igb,igc,igd)                                    ;//         gmin = min(iga,igb,igc,igd)
         gmax = max(iga,igb,igc,igd)                                    ;//         gmax = max(iga,igb,igc,igd)
         if ((iga == gmin  ||  iga == gmax)  &&                          //         if ((iga.eq.gmin .or. iga.eq.gmax) .and.
             (igb == gmin  ||  igb == gmax)  &&                          //     &       (igb.eq.gmin .or. igb.eq.gmax) .and.
             (igc == gmin  ||  igc == gmax)  &&                          //     &       (igc.eq.gmin .or. igc.eq.gmax) .and.
             (igd == gmin  ||  igd == gmax))  weigh = wgrp[gmin,gmax]   ;//     &       (igd.eq.gmin .or. igd.eq.gmax))  weigh = wgrp(gmin,gmax)
///
///     check group membership for a set containing five atoms
///
      } else if (nset  ==  5) {                                         ;//      else if (nset .eq. 5) then
         iga = grplist[ia]                                              ;//         iga = grplist(ia)
         igb = grplist[ib]                                              ;//         igb = grplist(ib)
         igc = grplist[ic]                                              ;//         igc = grplist(ic)
         igd = grplist[id]                                              ;//         igd = grplist(id)
         ige = grplist[ie]                                              ;//         ige = grplist(ie)
         gmin = min(iga,igb,igc,igd,ige)                                ;//         gmin = min(iga,igb,igc,igd,ige)
         gmax = max(iga,igb,igc,igd,ige)                                ;//         gmax = max(iga,igb,igc,igd,ige)
         if ((iga == gmin  ||  iga == gmax)  &&                          //         if ((iga.eq.gmin .or. iga.eq.gmax) .and.
             (igb == gmin  ||  igb == gmax)  &&                          //     &       (igb.eq.gmin .or. igb.eq.gmax) .and.
             (igc == gmin  ||  igc == gmax)  &&                          //     &       (igc.eq.gmin .or. igc.eq.gmax) .and.
             (igd == gmin  ||  igd == gmax)  &&                          //     &       (igd.eq.gmin .or. igd.eq.gmax) .and.
             (ige == gmin  ||  ige == gmax))  weigh = wgrp[gmin,gmax]   ;//     &       (ige.eq.gmin .or. ige.eq.gmax))  weigh = wgrp(gmin,gmax)
///
///     check group membership for a set containing five atoms
///
      } else if (nset  ==  6) {                                         ;//      else if (nset .eq. 6) then
         iga = grplist[ia]                                              ;//         iga = grplist(ia)
         igb = grplist[ib]                                              ;//         igb = grplist(ib)
         igc = grplist[ic]                                              ;//         igc = grplist(ic)
         igd = grplist[id]                                              ;//         igd = grplist(id)
         ige = grplist[ie]                                              ;//         ige = grplist(ie)
         igg = grplist[ig]                                              ;//         igg = grplist(ig)
         gmin = min(iga,igb,igc,igd,ige,igg)                            ;//         gmin = min(iga,igb,igc,igd,ige,igg)
         gmax = max(iga,igb,igc,igd,ige,igg)                            ;//         gmax = max(iga,igb,igc,igd,ige,igg)
         if ((iga == gmin  ||  iga == gmax)  &&                          //         if ((iga.eq.gmin .or. iga.eq.gmax) .and.
             (igb == gmin  ||  igb == gmax)  &&                          //     &       (igb.eq.gmin .or. igb.eq.gmax) .and.
             (igc == gmin  ||  igc == gmax)  &&                          //     &       (igc.eq.gmin .or. igc.eq.gmax) .and.
             (igd == gmin  ||  igd == gmax)  &&                          //     &       (igd.eq.gmin .or. igd.eq.gmax) .and.
             (ige == gmin  ||  ige == gmax)  &&                          //     &       (ige.eq.gmin .or. ige.eq.gmax) .and.
             (igg == gmin  ||  igg == gmax))  weigh = wgrp[gmin,gmax]   ;//     &       (igg.eq.gmin .or. igg.eq.gmax))  weigh = wgrp(gmin,gmax)
      }                                                                 ;//      end if
///
///     interaction will be used if its group has nonzero weight
///
      if (weigh  ==  0.0e0) {                                           ;//      if (weigh .eq. 0.0d0) then
         proceed =  false                                               ;//         proceed = .false.
      } else {                                                          ;//      else
         proceed =  true                                                ;//         proceed = .true.
      }                                                                 ;//      end if
      return                                                            ;//      return
    }                                                                    //      end
}
}
}
