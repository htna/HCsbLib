using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static partial class HessCoarseResiIter
        {
            public static Tuple<int[], int[][]> GetIdxKeepListRemv_ResiCluster_SymrcmBlockWise
                ( Universe.Atom[] atoms
                , Vector[] coords
                , HessMatrix hess
                , double clus_width
                , int num_atom_merge
                , double? symrcm_filter_blckwise_interact
                , params string[] nameToKeep
                )
            {
                if((nameToKeep == null) || (nameToKeep.Length == 0))
                    throw new HException();

                var split = atoms.HSplitByNames(nameToKeep);
                var keeps = split.match;
                return GetIdxKeepListRemv_ResiCluster_SymrcmBlockWise(atoms, coords, hess, clus_width, num_atom_merge, symrcm_filter_blckwise_interact, keeps);
            }
            public static Tuple<int[], int[][]> GetIdxKeepListRemv_ResiCluster_SymrcmBlockWise
                ( Universe.Atom[] atoms
                , Vector[] coords
                , HessMatrix hess
                , double clus_width
                , int num_atom_merge
                , double? symrcm_filter_blckwise_interact
                , IList<Universe.Atom> keeps
                )
            {
                /// summarizing the above procedures:
                /// 1. determine k-by-k-by-k blocks each of which is composed of protein residues
                /// 2. for each block whose atom number is smaller than 0.5*block_threshold,
                ///    find its neighboring block that has the largest contact with the small block.
                ///    if total number of their atoms does not surpass 1.5*block_threshold, then merge them.
                ///    otherwise, repeat finding another neighboring block with the next largest contact
                ///                  and checking if it can be merged with the small block.
                ///    if none of the neighbors can be merged, skip merging this small block.
                /// 3. build block by block contact matrix
                /// 4. reshuffle the blocks using the reversed Cuthill-McKee algorithm (symrcm in matlab)
                //  1. build blocks
                //     a. select corner where
                //        minx = min{x1, x2, x3, ..., xn}
                //        miny = min{y1, y2, y3, ..., yn}
                //        minz = min{z1, z2, z3, ..., zn}
                //     b. group atoms by residues
                //     c. add residue to a block whose index is <i,j,k> where
                //        i = (residue-ca.x - minx)/block size
                //        j = (residue-ca.y - miny)/block size
                //        k = (residue-ca.z - minz)/block size
                //  2. merge blocks
                //     a. enumerate each blocks (the order of enumeration is determined by dictionary data structure
                //                               implemented in C# and its keys <i,j,k>. I do not manually enumerate them.
                //                               Sorry. I incorrectly explained this in the meeting.)
                //  [REP] 1. for each block BBB
                //        2. if block BBB is small, such that (number of atoms in BBB < block_threshold/2), then
                //           a. list neighboring blocks of BBB (totally, 3*3*3-1=26 neighbors)
                //  [rep]    b. select a neighbor in the list, whose contact area with BBB is largest,
                //              where the contact area is determined as (number of atom in the neighbor satisfying dist(atom,BBB) < 4A )
                //           c. if (number of atoms of the merged block (the neighbor + BBB) < block_threshold*1.5)
                //              1. then merge them
                //                 and go to step 2.a.1 [REP]
                //              2. otherwise repeat from 2.a.2.b [rep]
                //           e. if there is no neighbor to be merged, remain this block BBB as non-merged.
                //  3. build block contact matrix
                //     a. if m is the number of blocks,
                //           make m-by-m matrix
                //     b. define contact value between block BB and CC by (max Hess[atom in BB, atom in CC]).
                //     b. set m[BB,CC] = 1 if (contact value between blocks BB and CC < tolerance-threshold=1)
                //            m[BB,CC] = 0 otherwise <= the reason of ignoring this contact is because
                //                                      this interaction will be removed in (line 7 of Algorithm 1)
                //  4. reshuffle the atoms in blocks using "symrcm in matlab"

                #region capture of computation speeds and its summary
                //options.Add("ssNMA-symrcm-blockwise-8-129-001");     //  8 -> 108 -> *1.2=129 -> *1.5=194   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-8-129-001 : hess(  7.7 sec), coarse(  4.3 min), mode(    0.0 sec): coarse graining avg of iter( 111), sec( 1.35), MemoryMb( 305), RemAtom(130), B-SetZero( 3350), B-NonZero( 1459), BDC-IgnAdd( 31407)
                //options.Add("ssNMA-symrcm-blockwise-9-157-001");     //  9 -> 131 -> *1.2=157 -> *1.5=235   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.5 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  91), sec( 1.59), MemoryMb( 318), RemAtom(159), B-SetZero( 3394), B-NonZero( 1606), BDC-IgnAdd( 35626)
                //options.Add("ssNMA-symrcm-blockwise-10-189-001");    // 10 -> 157 -> *1.2=189 -> *1.5=283   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.6 sec), coarse(  2.8 min), mode(    0.0 sec): coarse graining avg of iter(  79), sec( 1.96), MemoryMb( 356), RemAtom(183), B-SetZero( 3633), B-NonZero( 1807), BDC-IgnAdd( 44519)
                //options.Add("ssNMA-symrcm-blockwise-11-227-001");    // 11 -> 189 -> *1.2=227 -> *1.5=339   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-11-227-001: hess(  7.5 sec), coarse(  2.9 min), mode(    0.0 sec): coarse graining avg of iter(  70), sec( 2.35), MemoryMb( 402), RemAtom(207), B-SetZero( 3872), B-NonZero( 1993), BDC-IgnAdd( 48423)
                //options.Add("ssNMA-symrcm-blockwise-12-270-001");    // 12 -> 225 -> *1.2=270 -> *1.5=406   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-12-270-001: hess(  7.5 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  53), sec( 3.59), MemoryMb( 443), RemAtom(273), B-SetZero( 4309), B-NonZero( 2458), BDC-IgnAdd( 72160)
                //options.Add("ssNMA-symrcm-blockwise-13-321-001");    // 13 -> 268 -> *1.2=321 -> *1.5=482   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-13-321-001: hess(  7.5 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter(  47), sec( 3.84), MemoryMb( 434), RemAtom(308), B-SetZero( 4067), B-NonZero( 2464), BDC-IgnAdd( 70300)
                //options.Add("ssNMA-symrcm-blockwise-14-380-001");    // 14 -> 317 -> *1.2=380 -> *1.5=570   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-14-380-001: hess(  7.5 sec), coarse(  3.4 min), mode(    0.0 sec): coarse graining avg of iter(  39), sec( 5.08), MemoryMb( 470), RemAtom(371), B-SetZero( 4246), B-NonZero( 2796), BDC-IgnAdd( 79872)
                //options.Add("ssNMA-symrcm-blockwise-15-447-001");    // 15 -> 372 -> *1.2=447 -> *1.5=670   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-15-447-001: hess(  7.5 sec), coarse(  3.9 min), mode(    0.0 sec): coarse graining avg of iter(  33), sec( 6.92), MemoryMb( 493), RemAtom(439), B-SetZero( 4624), B-NonZero( 3157), BDC-IgnAdd(108344)
                //options.Add("ssNMA-symrcm-blockwise-16-523-001");    // 16 -> 436 -> *1.2=523 -> *1.5=784   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-16-523-001: hess(  7.6 sec), coarse(  4.4 min), mode(    0.0 sec): coarse graining avg of iter(  26), sec( 9.96), MemoryMb( 537), RemAtom(557), B-SetZero( 4719), B-NonZero( 3726), BDC-IgnAdd(127072)
                //options.Add("ssNMA-symrcm-blockwise-17-609-001");    // 17 -> 507 -> *1.2=609 -> *1.5=913   //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-17-609-001: hess(  7.5 sec), coarse(  4.7 min), mode(    0.0 sec): coarse graining avg of iter(  24), sec(11.40), MemoryMb( 559), RemAtom(604), B-SetZero( 4767), B-NonZero( 3873), BDC-IgnAdd(143710)
                //options.Add("ssNMA-symrcm-blockwise-18-705-001");    // 18 -> 588 -> *1.2=705 -> *1.5=1058  //  pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-18-705-001: hess(  7.5 sec), coarse(  5.1 min), mode(    0.0 sec): coarse graining avg of iter(  22), sec(13.70), MemoryMb( 660), RemAtom(659), B-SetZero( 4632), B-NonZero( 4030), BDC-IgnAdd(147021)
                //
                // in my computer
                //  K:\bin\SimulCoarseSsnma\AnyCPU_Release>SimulCoarseSsnma.exe
                //  total number of pdbs to test: 80
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-18-500-001                 : hess(  5.9 sec), coarse(  2.2 min), mode(    0.0 sec): coarse graining avg of iter(  28), sec( 4.45), MemoryMb( 337), RemAtom(390), B-SetZero( 2763), B-NonZero( 2761), BDC-IgnAdd( 56026)
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  5.6 sec), coarse(  1.6 min), mode(    0.0 sec): coarse graining avg of iter(  71), sec( 1.19), MemoryMb( 348), RemAtom(153), B-SetZero( 2107), B-NonZero( 1267), BDC-IgnAdd( 21764)
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-blockwise-10-189-001: hess(  5.6 sec), coarse(  1.7 min), mode(    0.0 sec): coarse graining avg of iter(  53), sec( 1.77), MemoryMb( 266), RemAtom(206), B-SetZero( 2338), B-NonZero( 1555), BDC-IgnAdd( 31123)
                //   pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-18-500-001                 : hess(  6.5 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  27), sec( 6.54), MemoryMb( 381), RemAtom(426), B-SetZero( 3868), B-NonZero( 3362), BDC-IgnAdd( 82751)
                //   pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.4 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  77), sec( 1.61), MemoryMb( 361), RemAtom(149), B-SetZero( 3200), B-NonZero( 1609), BDC-IgnAdd( 35001)
                //   pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.4 sec), coarse(  2.2 min), mode(    0.0 sec): coarse graining avg of iter(  65), sec( 1.91), MemoryMb( 384), RemAtom(177), B-SetZero( 3360), B-NonZero( 1779), BDC-IgnAdd( 40433)
                //   pdbid(3JUX), resi(  813), atoms(  13264) :   ssNMA-18-500-001                 : hess(  6.4 sec), coarse(  2.9 min), mode(    0.0 sec): coarse graining avg of iter(  31), sec( 5.40), MemoryMb( 401), RemAtom(401), B-SetZero( 3393), B-NonZero( 2956), BDC-IgnAdd( 71749)
                //   pdbid(3JUX), resi(  813), atoms(  13264) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.4 sec), coarse(  2.2 min), mode(    0.0 sec): coarse graining avg of iter(  83), sec( 1.41), MemoryMb( 405), RemAtom(150), B-SetZero( 2630), B-NonZero( 1420), BDC-IgnAdd( 29102)
                //   pdbid(3JUX), resi(  813), atoms(  13264) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.4 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  73), sec( 1.53), MemoryMb( 359), RemAtom(170), B-SetZero( 2651), B-NonZero( 1480), BDC-IgnAdd( 28833)
                //   pdbid(1I8Q), resi(  814), atoms(  12936) :   ssNMA-18-500-001                 : hess(  6.7 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  29), sec( 6.27), MemoryMb( 409), RemAtom(418), B-SetZero( 3780), B-NonZero( 3297), BDC-IgnAdd( 82719)
                //   pdbid(1I8Q), resi(  814), atoms(  12936) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.8 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  82), sec( 1.54), MemoryMb( 401), RemAtom(147), B-SetZero( 3164), B-NonZero( 1594), BDC-IgnAdd( 32422)
                //   pdbid(1I8Q), resi(  814), atoms(  12936) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.8 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  67), sec( 2.13), MemoryMb( 412), RemAtom(180), B-SetZero( 3408), B-NonZero( 1906), BDC-IgnAdd( 43947)
                //   pdbid(3G2N), resi(  815), atoms(  13216) :   ssNMA-18-500-001                 : hess(  7.1 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  30), sec( 6.39), MemoryMb( 426), RemAtom(413), B-SetZero( 3735), B-NonZero( 3071), BDC-IgnAdd( 89619)
                //   pdbid(3G2N), resi(  815), atoms(  13216) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.0 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  82), sec( 1.57), MemoryMb( 343), RemAtom(151), B-SetZero( 3239), B-NonZero( 1500), BDC-IgnAdd( 36468)
                //   pdbid(3G2N), resi(  815), atoms(  13216) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.9 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  65), sec( 2.16), MemoryMb( 368), RemAtom(190), B-SetZero( 3618), B-NonZero( 1784), BDC-IgnAdd( 49040)
                //   pdbid(1ILE), resi(  821), atoms(  13287) :   ssNMA-18-500-001                 : hess(  6.8 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter(  30), sec( 6.14), MemoryMb( 419), RemAtom(415), B-SetZero( 3595), B-NonZero( 3207), BDC-IgnAdd( 80513)
                //   pdbid(1ILE), resi(  821), atoms(  13287) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.8 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  81), sec( 1.54), MemoryMb( 392), RemAtom(153), B-SetZero( 2909), B-NonZero( 1557), BDC-IgnAdd( 31304)
                //   pdbid(1ILE), resi(  821), atoms(  13287) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.0 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  68), sec( 1.83), MemoryMb( 403), RemAtom(183), B-SetZero( 2912), B-NonZero( 1681), BDC-IgnAdd( 34580)
                //   pdbid(2E1R), resi(  828), atoms(  12987) :   ssNMA-18-500-001                 : hess(  6.1 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  31), sec( 4.59), MemoryMb( 409), RemAtom(392), B-SetZero( 3007), B-NonZero( 2848), BDC-IgnAdd( 57987)
                //   pdbid(2E1R), resi(  828), atoms(  12987) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.1 sec), coarse(  1.9 min), mode(    0.0 sec): coarse graining avg of iter(  78), sec( 1.33), MemoryMb( 351), RemAtom(155), B-SetZero( 2344), B-NonZero( 1348), BDC-IgnAdd( 24919)
                //   pdbid(2E1R), resi(  828), atoms(  12987) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.2 sec), coarse(  1.9 min), mode(    0.0 sec): coarse graining avg of iter(  66), sec( 1.59), MemoryMb( 352), RemAtom(184), B-SetZero( 2486), B-NonZero( 1520), BDC-IgnAdd( 28073)
                //   pdbid(3PSI), resi(  843), atoms(  13685) :   ssNMA-18-500-001                 : hess(  6.2 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  34), sec( 3.35), MemoryMb( 380), RemAtom(377), B-SetZero( 2096), B-NonZero( 2156), BDC-IgnAdd( 38821)
                //   pdbid(3PSI), resi(  843), atoms(  13685) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.2 sec), coarse(  1.5 min), mode(    0.0 sec): coarse graining avg of iter(  90), sec( 0.86), MemoryMb( 338), RemAtom(142), B-SetZero( 1522), B-NonZero(  920), BDC-IgnAdd( 13467)
                //   pdbid(3PSI), resi(  843), atoms(  13685) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.2 sec), coarse(  1.5 min), mode(    0.0 sec): coarse graining avg of iter(  73), sec( 1.12), MemoryMb( 347), RemAtom(175), B-SetZero( 1719), B-NonZero( 1094), BDC-IgnAdd( 18098)
                //   pdbid(1NO3), resi(  851), atoms(  13548) :   ssNMA-18-500-001                 : hess(  7.3 sec), coarse(  3.4 min), mode(    0.0 sec): coarse graining avg of iter(  31), sec( 6.43), MemoryMb( 428), RemAtom(409), B-SetZero( 3924), B-NonZero( 3102), BDC-IgnAdd( 91354)
                //   pdbid(1NO3), resi(  851), atoms(  13548) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.3 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  81), sec( 1.73), MemoryMb( 432), RemAtom(156), B-SetZero( 3426), B-NonZero( 1577), BDC-IgnAdd( 40638)
                //   pdbid(1NO3), resi(  851), atoms(  13548) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.3 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  74), sec( 1.77), MemoryMb( 430), RemAtom(171), B-SetZero( 3165), B-NonZero( 1576), BDC-IgnAdd( 39515)
                //   pdbid(2Z5N), resi(  855), atoms(  13715) :   ssNMA-18-500-001                 : hess(  6.6 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  32), sec( 4.69), MemoryMb( 414), RemAtom(401), B-SetZero( 3089), B-NonZero( 2866), BDC-IgnAdd( 57340)
                //   pdbid(2Z5N), resi(  855), atoms(  13715) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.7 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  84), sec( 1.26), MemoryMb( 470), RemAtom(153), B-SetZero( 2305), B-NonZero( 1304), BDC-IgnAdd( 21978)
                //   pdbid(2Z5N), resi(  855), atoms(  13715) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.7 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  71), sec( 1.56), MemoryMb( 457), RemAtom(181), B-SetZero( 2352), B-NonZero( 1470), BDC-IgnAdd( 26008)
                //   pdbid(1QBB), resi(  858), atoms(  13357) :   ssNMA-18-500-001                 : hess(  7.3 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  29), sec( 6.60), MemoryMb( 430), RemAtom(431), B-SetZero( 3629), B-NonZero( 3101), BDC-IgnAdd( 85561)
                //   pdbid(1QBB), resi(  858), atoms(  13357) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.5 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  81), sec( 1.60), MemoryMb( 489), RemAtom(154), B-SetZero( 3098), B-NonZero( 1480), BDC-IgnAdd( 37533)
                //   pdbid(1QBB), resi(  858), atoms(  13357) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.3 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  68), sec( 1.98), MemoryMb( 485), RemAtom(183), B-SetZero( 3190), B-NonZero( 1641), BDC-IgnAdd( 43851)
                //   pdbid(3ND2), resi(  861), atoms(  13267) :   ssNMA-18-500-001                 : hess(  6.6 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  33), sec( 4.39), MemoryMb( 402), RemAtom(375), B-SetZero( 3083), B-NonZero( 2800), BDC-IgnAdd( 57000)
                //   pdbid(3ND2), resi(  861), atoms(  13267) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.6 sec), coarse(  1.9 min), mode(    0.0 sec): coarse graining avg of iter(  79), sec( 1.31), MemoryMb( 437), RemAtom(157), B-SetZero( 2390), B-NonZero( 1390), BDC-IgnAdd( 22728)
                //   pdbid(3ND2), resi(  861), atoms(  13267) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.6 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  64), sec( 1.76), MemoryMb( 414), RemAtom(193), B-SetZero( 2625), B-NonZero( 1621), BDC-IgnAdd( 29275)
                //   pdbid(3S1S), resi(  862), atoms(  13782) :   ssNMA-18-500-001                 : hess(  7.1 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  30), sec( 5.86), MemoryMb( 414), RemAtom(430), B-SetZero( 3563), B-NonZero( 3034), BDC-IgnAdd( 77300)
                //   pdbid(3S1S), resi(  862), atoms(  13782) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.0 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  89), sec( 1.39), MemoryMb( 423), RemAtom(145), B-SetZero( 2885), B-NonZero( 1418), BDC-IgnAdd( 28302)
                //   pdbid(3S1S), resi(  862), atoms(  13782) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.0 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  69), sec( 1.89), MemoryMb( 419), RemAtom(187), B-SetZero( 3080), B-NonZero( 1663), BDC-IgnAdd( 35503)
                //   pdbid(3WAK), resi(  864), atoms(  13827) :   ssNMA-18-500-001                 : hess(  7.1 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter(  31), sec( 5.92), MemoryMb( 401), RemAtom(418), B-SetZero( 3337), B-NonZero( 2928), BDC-IgnAdd( 71720)
                //   pdbid(3WAK), resi(  864), atoms(  13827) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.1 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  83), sec( 1.55), MemoryMb( 391), RemAtom(156), B-SetZero( 2955), B-NonZero( 1507), BDC-IgnAdd( 31732)
                //   pdbid(3WAK), resi(  864), atoms(  13827) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.0 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  69), sec( 2.01), MemoryMb( 380), RemAtom(187), B-SetZero( 3223), B-NonZero( 1732), BDC-IgnAdd( 41013)
                //   pdbid(2DQM), resi(  867), atoms(  13758) :   ssNMA-18-500-001                 : hess(  7.2 sec), coarse(  3.6 min), mode(    0.0 sec): coarse graining avg of iter(  31), sec( 6.74), MemoryMb( 492), RemAtom(415), B-SetZero( 3902), B-NonZero( 3227), BDC-IgnAdd( 96046)
                //   pdbid(2DQM), resi(  867), atoms(  13758) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.2 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  87), sec( 1.58), MemoryMb( 503), RemAtom(148), B-SetZero( 3204), B-NonZero( 1537), BDC-IgnAdd( 35150)
                //   pdbid(2DQM), resi(  867), atoms(  13758) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.2 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  67), sec( 2.15), MemoryMb( 517), RemAtom(192), B-SetZero( 3474), B-NonZero( 1810), BDC-IgnAdd( 43915)
                //   pdbid(1FIY), resi(  873), atoms(  13839) :   ssNMA-18-500-001                 : hess(  7.1 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  31), sec( 6.11), MemoryMb( 408), RemAtom(418), B-SetZero( 3615), B-NonZero( 3043), BDC-IgnAdd( 80286)
                //   pdbid(1FIY), resi(  873), atoms(  13839) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.1 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  82), sec( 1.59), MemoryMb( 451), RemAtom(158), B-SetZero( 2923), B-NonZero( 1497), BDC-IgnAdd( 35220)
                //   pdbid(1FIY), resi(  873), atoms(  13839) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.1 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  71), sec( 2.04), MemoryMb( 468), RemAtom(182), B-SetZero( 3237), B-NonZero( 1721), BDC-IgnAdd( 44328)
                //   pdbid(2OAJ), resi(  875), atoms(  13746) :   ssNMA-18-500-001                 : hess(  7.0 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter(  30), sec( 6.20), MemoryMb( 409), RemAtom(429), B-SetZero( 3491), B-NonZero( 3043), BDC-IgnAdd( 78526)
                //   pdbid(2OAJ), resi(  875), atoms(  13746) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.0 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  84), sec( 1.58), MemoryMb( 434), RemAtom(153), B-SetZero( 3102), B-NonZero( 1501), BDC-IgnAdd( 33478)
                //   pdbid(2OAJ), resi(  875), atoms(  13746) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.0 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  69), sec( 2.05), MemoryMb( 448), RemAtom(186), B-SetZero( 3394), B-NonZero( 1804), BDC-IgnAdd( 42567)
                //   pdbid(3ZIM), resi(  886), atoms(  14488) :   ssNMA-18-500-001                 : hess(  7.3 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  34), sec( 5.28), MemoryMb( 456), RemAtom(400), B-SetZero( 3400), B-NonZero( 2747), BDC-IgnAdd( 70296)
                //   pdbid(3ZIM), resi(  886), atoms(  14488) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.3 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  89), sec( 1.39), MemoryMb( 440), RemAtom(152), B-SetZero( 2782), B-NonZero( 1360), BDC-IgnAdd( 26459)
                //   pdbid(3ZIM), resi(  886), atoms(  14488) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.3 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  80), sec( 1.66), MemoryMb( 433), RemAtom(170), B-SetZero( 3023), B-NonZero( 1518), BDC-IgnAdd( 34463)
                //   pdbid(4FYS), resi(  909), atoms(  14470) :   ssNMA-18-500-001                 : hess(  7.7 sec), coarse(  4.3 min), mode(    0.0 sec): coarse graining avg of iter(  32), sec( 7.75), MemoryMb( 455), RemAtom(423), B-SetZero( 4587), B-NonZero( 3486), BDC-IgnAdd(114485)
                //   pdbid(4FYS), resi(  909), atoms(  14470) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.7 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  90), sec( 1.87), MemoryMb( 520), RemAtom(150), B-SetZero( 3870), B-NonZero( 1687), BDC-IgnAdd( 43472)
                //   pdbid(4FYS), resi(  909), atoms(  14470) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.8 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  78), sec( 2.18), MemoryMb( 511), RemAtom(173), B-SetZero( 3945), B-NonZero( 1857), BDC-IgnAdd( 48642)
                //   pdbid(3FAH), resi(  907), atoms(  13566) :   ssNMA-18-500-001                 : hess(  7.8 sec), coarse(  4.2 min), mode(    0.0 sec): coarse graining avg of iter(  30), sec( 8.19), MemoryMb( 457), RemAtom(421), B-SetZero( 4600), B-NonZero( 3596), BDC-IgnAdd(111559)
                //   pdbid(3FAH), resi(  907), atoms(  13566) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.4 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  81), sec( 2.10), MemoryMb( 553), RemAtom(156), B-SetZero( 4090), B-NonZero( 1872), BDC-IgnAdd( 52403)
                //   pdbid(3FAH), resi(  907), atoms(  13566) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.4 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter(  73), sec( 2.43), MemoryMb( 519), RemAtom(173), B-SetZero( 4178), B-NonZero( 1964), BDC-IgnAdd( 59481)
                //   pdbid(2QIZ), resi(  943), atoms(  15239) :   ssNMA-18-500-001                 : hess(  8.0 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter(  35), sec( 5.22), MemoryMb( 405), RemAtom(408), B-SetZero( 3114), B-NonZero( 2792), BDC-IgnAdd( 59596)
                //   pdbid(2QIZ), resi(  943), atoms(  15239) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  8.1 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  97), sec( 1.30), MemoryMb( 448), RemAtom(147), B-SetZero( 2538), B-NonZero( 1322), BDC-IgnAdd( 24290)
                //   pdbid(2QIZ), resi(  943), atoms(  15239) :   ssNMA-symrcm-blockwise-10-189-001: hess(  8.2 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  80), sec( 1.57), MemoryMb( 433), RemAtom(178), B-SetZero( 2635), B-NonZero( 1465), BDC-IgnAdd( 27829)
                //   pdbid(2XVL), resi(  944), atoms(  14860) :   ssNMA-18-500-001                 : hess(  8.4 sec), coarse(  4.8 min), mode(    0.0 sec): coarse graining avg of iter(  33), sec( 8.54), MemoryMb( 550), RemAtom(421), B-SetZero( 5200), B-NonZero( 3678), BDC-IgnAdd(117688)
                //   pdbid(2XVL), resi(  944), atoms(  14860) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  8.4 sec), coarse(  3.4 min), mode(    0.0 sec): coarse graining avg of iter(  96), sec( 1.93), MemoryMb( 586), RemAtom(144), B-SetZero( 4222), B-NonZero( 1767), BDC-IgnAdd( 45937)
                //   pdbid(2XVL), resi(  944), atoms(  14860) :   ssNMA-symrcm-blockwise-10-189-001: hess(  8.4 sec), coarse(  3.5 min), mode(    0.0 sec): coarse graining avg of iter(  74), sec( 2.67), MemoryMb( 537), RemAtom(188), B-SetZero( 4696), B-NonZero( 2178), BDC-IgnAdd( 61833)
                //   pdbid(3OG2), resi(  986), atoms(  15064) :   ssNMA-18-500-001                 : hess(  8.3 sec), coarse(  4.7 min), mode(    0.0 sec): coarse graining avg of iter(  33), sec( 8.24), MemoryMb( 458), RemAtom(426), B-SetZero( 4910), B-NonZero( 3565), BDC-IgnAdd(116402)
                //   pdbid(3OG2), resi(  986), atoms(  15064) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  8.2 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  91), sec( 1.97), MemoryMb( 495), RemAtom(154), B-SetZero( 4052), B-NonZero( 1784), BDC-IgnAdd( 45692)
                //   pdbid(3OG2), resi(  986), atoms(  15064) :   ssNMA-symrcm-blockwise-10-189-001: hess(  8.7 sec), coarse(  3.5 min), mode(    0.0 sec): coarse graining avg of iter(  81), sec( 2.36), MemoryMb( 485), RemAtom(173), B-SetZero( 4324), B-NonZero( 1943), BDC-IgnAdd( 56613)
                //   pdbid(3W5B), resi(  997), atoms(  15486) :   ssNMA-18-500-001                 : hess(  7.7 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  34), sec( 5.21), MemoryMb( 436), RemAtom(426), B-SetZero( 2902), B-NonZero( 2819), BDC-IgnAdd( 56974)
                //   pdbid(3W5B), resi(  997), atoms(  15486) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.6 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  93), sec( 1.41), MemoryMb( 535), RemAtom(155), B-SetZero( 2496), B-NonZero( 1395), BDC-IgnAdd( 23893)
                //   pdbid(3W5B), resi(  997), atoms(  15486) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.7 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  85), sec( 1.59), MemoryMb( 501), RemAtom(170), B-SetZero( 2600), B-NonZero( 1513), BDC-IgnAdd( 26728)
                //   pdbid(3POY), resi( 1005), atoms(  15446) :   ssNMA-18-500-001                 : hess(  8.0 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  36), sec( 4.80), MemoryMb( 403), RemAtom(401), B-SetZero( 3207), B-NonZero( 2774), BDC-IgnAdd( 55596)
                //   pdbid(3POY), resi( 1005), atoms(  15446) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.9 sec), coarse(  2.5 min), mode(    0.0 sec): coarse graining avg of iter(  94), sec( 1.40), MemoryMb( 475), RemAtom(153), B-SetZero( 2768), B-NonZero( 1414), BDC-IgnAdd( 24269)
                //   pdbid(3POY), resi( 1005), atoms(  15446) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.9 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  76), sec( 1.74), MemoryMb( 403), RemAtom(190), B-SetZero( 2802), B-NonZero( 1553), BDC-IgnAdd( 27755)
                //   pdbid(3EJU), resi( 1016), atoms(  16229) :   ssNMA-18-500-001                 : hess(  8.7 sec), coarse(  4.8 min), mode(    0.0 sec): coarse graining avg of iter(  35), sec( 8.02), MemoryMb( 473), RemAtom(434), B-SetZero( 4829), B-NonZero( 3362), BDC-IgnAdd(109826)
                //   pdbid(3EJU), resi( 1016), atoms(  16229) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  8.7 sec), coarse(  3.4 min), mode(    0.0 sec): coarse graining avg of iter(  95), sec( 1.97), MemoryMb( 631), RemAtom(160), B-SetZero( 4049), B-NonZero( 1696), BDC-IgnAdd( 44527)
                //   pdbid(3EJU), resi( 1016), atoms(  16229) :   ssNMA-symrcm-blockwise-10-189-001: hess(  8.7 sec), coarse(  3.7 min), mode(    0.0 sec): coarse graining avg of iter(  81), sec( 2.55), MemoryMb( 633), RemAtom(187), B-SetZero( 4462), B-NonZero( 1940), BDC-IgnAdd( 58043)
                //   pdbid(1T9Y), resi( 1016), atoms(  15605) :   ssNMA-18-500-001                 : hess(  7.6 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  37), sec( 4.79), MemoryMb( 436), RemAtom(394), B-SetZero( 3074), B-NonZero( 2557), BDC-IgnAdd( 62245)
                //   pdbid(1T9Y), resi( 1016), atoms(  15605) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.6 sec), coarse(  2.3 min), mode(    0.0 sec): coarse graining avg of iter(  96), sec( 1.28), MemoryMb( 523), RemAtom(151), B-SetZero( 2365), B-NonZero( 1236), BDC-IgnAdd( 23243)
                //   pdbid(1T9Y), resi( 1016), atoms(  15605) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.6 sec), coarse(  2.4 min), mode(    0.0 sec): coarse graining avg of iter(  85), sec( 1.50), MemoryMb( 486), RemAtom(171), B-SetZero( 2559), B-NonZero( 1369), BDC-IgnAdd( 26558)
                //   pdbid(1UG9), resi( 1019), atoms(  14659) :   ssNMA-18-500-001                 : hess(  7.7 sec), coarse(  4.0 min), mode(    0.0 sec): coarse graining avg of iter(  30), sec( 7.76), MemoryMb( 424), RemAtom(454), B-SetZero( 4376), B-NonZero( 3807), BDC-IgnAdd( 96984)
                //   pdbid(1UG9), resi( 1019), atoms(  14659) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.6 sec), coarse(  2.9 min), mode(    0.0 sec): coarse graining avg of iter(  85), sec( 1.85), MemoryMb( 552), RemAtom(160), B-SetZero( 3551), B-NonZero( 1770), BDC-IgnAdd( 38878)
                //   pdbid(1UG9), resi( 1019), atoms(  14659) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.7 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  70), sec( 2.40), MemoryMb( 554), RemAtom(194), B-SetZero( 3841), B-NonZero( 2016), BDC-IgnAdd( 48747)
                //   pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-18-500-001                 : hess(  8.0 sec), coarse(  4.1 min), mode(    0.0 sec): coarse graining avg of iter(  35), sec( 6.76), MemoryMb( 463), RemAtom(414), B-SetZero( 4250), B-NonZero( 3226), BDC-IgnAdd( 90483)
                //   pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  8.1 sec), coarse(  2.9 min), mode(    0.0 sec): coarse graining avg of iter(  91), sec( 1.72), MemoryMb( 455), RemAtom(159), B-SetZero( 3394), B-NonZero( 1606), BDC-IgnAdd( 35626)
                //   pdbid(4KWU), resi( 1030), atoms(  15528) :   ssNMA-symrcm-blockwise-10-189-001: hess(  8.0 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  79), sec( 2.12), MemoryMb( 458), RemAtom(183), B-SetZero( 3633), B-NonZero( 1807), BDC-IgnAdd( 44519)
                //   pdbid(3W5M), resi( 1030), atoms(  15549) :   ssNMA-18-500-001                 : hess(  8.2 sec), coarse(  4.1 min), mode(    0.0 sec): coarse graining avg of iter(  36), sec( 6.59), MemoryMb( 430), RemAtom(403), B-SetZero( 4517), B-NonZero( 3377), BDC-IgnAdd( 97395)
                //   pdbid(3W5M), resi( 1030), atoms(  15549) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  8.2 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter(  88), sec( 1.97), MemoryMb( 570), RemAtom(164), B-SetZero( 3674), B-NonZero( 1766), BDC-IgnAdd( 41828)
                //   pdbid(3W5M), resi( 1030), atoms(  15549) :   ssNMA-symrcm-blockwise-10-189-001: hess(  8.2 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  70), sec( 2.60), MemoryMb( 531), RemAtom(207), B-SetZero( 4024), B-NonZero( 2036), BDC-IgnAdd( 52685)
                //   pdbid(1WXR), resi( 1035), atoms(  15300) :   ssNMA-18-500-001                 : hess(  7.8 sec), coarse(  3.8 min), mode(    0.0 sec): coarse graining avg of iter(  35), sec( 6.22), MemoryMb( 441), RemAtom(407), B-SetZero( 4363), B-NonZero( 3368), BDC-IgnAdd( 77753)
                //   pdbid(1WXR), resi( 1035), atoms(  15300) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.9 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  90), sec( 1.80), MemoryMb( 560), RemAtom(158), B-SetZero( 3361), B-NonZero( 1717), BDC-IgnAdd( 31581)
                //   pdbid(1WXR), resi( 1035), atoms(  15300) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.8 sec), coarse(  2.9 min), mode(    0.0 sec): coarse graining avg of iter(  79), sec( 2.02), MemoryMb( 523), RemAtom(180), B-SetZero( 3565), B-NonZero( 1862), BDC-IgnAdd( 36336)
                //   pdbid(1ST6), resi( 1049), atoms(  16289) :   ssNMA-18-500-001                 : hess(  7.9 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  37), sec( 3.94), MemoryMb( 402), RemAtom(411), B-SetZero( 2321), B-NonZero( 2425), BDC-IgnAdd( 43878)
                //   pdbid(1ST6), resi( 1049), atoms(  16289) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.6 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter( 107), sec( 0.98), MemoryMb( 510), RemAtom(142), B-SetZero( 1625), B-NonZero( 1002), BDC-IgnAdd( 15330)
                //   pdbid(1ST6), resi( 1049), atoms(  16289) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.6 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  86), sec( 1.25), MemoryMb( 441), RemAtom(177), B-SetZero( 1714), B-NonZero( 1157), BDC-IgnAdd( 19497)
                //   pdbid(2FHB), resi( 1052), atoms(  15825) :   ssNMA-18-500-001                 : hess(  8.3 sec), coarse(  4.0 min), mode(    0.0 sec): coarse graining avg of iter(  37), sec( 6.21), MemoryMb( 411), RemAtom(399), B-SetZero( 4169), B-NonZero( 3200), BDC-IgnAdd( 85699)
                //   pdbid(2FHB), resi( 1052), atoms(  15825) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  8.4 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  96), sec( 1.67), MemoryMb( 559), RemAtom(153), B-SetZero( 3323), B-NonZero( 1541), BDC-IgnAdd( 34728)
                //   pdbid(2FHB), resi( 1052), atoms(  15825) :   ssNMA-symrcm-blockwise-10-189-001: hess(  8.4 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  82), sec( 2.08), MemoryMb( 501), RemAtom(180), B-SetZero( 3727), B-NonZero( 1790), BDC-IgnAdd( 43362)
                //   pdbid(3S5K), resi( 1053), atoms(  17342) :   ssNMA-18-500-001                 : hess(  9.3 sec), coarse(  4.0 min), mode(    0.0 sec): coarse graining avg of iter(  38), sec( 6.07), MemoryMb( 470), RemAtom(428), B-SetZero( 3474), B-NonZero( 2941), BDC-IgnAdd( 79250)
                //   pdbid(3S5K), resi( 1053), atoms(  17342) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  9.2 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter( 104), sec( 1.52), MemoryMb( 552), RemAtom(156), B-SetZero( 2726), B-NonZero( 1417), BDC-IgnAdd( 29967)
                //   pdbid(3S5K), resi( 1053), atoms(  17342) :   ssNMA-symrcm-blockwise-10-189-001: hess(  9.2 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  90), sec( 1.78), MemoryMb( 507), RemAtom(180), B-SetZero( 2797), B-NonZero( 1499), BDC-IgnAdd( 33846)
                //   pdbid(2PO4), resi( 1094), atoms(  16930) :   ssNMA-18-500-001                 : hess(  9.0 sec), coarse(  4.3 min), mode(    0.0 sec): coarse graining avg of iter(  38), sec( 6.60), MemoryMb( 478), RemAtom(416), B-SetZero( 4016), B-NonZero( 3115), BDC-IgnAdd( 94555)
                //   pdbid(2PO4), resi( 1094), atoms(  16930) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  9.0 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter( 102), sec( 1.63), MemoryMb( 599), RemAtom(155), B-SetZero( 3193), B-NonZero( 1521), BDC-IgnAdd( 33602)
                //   pdbid(2PO4), resi( 1094), atoms(  16930) :   ssNMA-symrcm-blockwise-10-189-001: hess(  9.0 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  84), sec( 2.14), MemoryMb( 551), RemAtom(188), B-SetZero( 3505), B-NonZero( 1739), BDC-IgnAdd( 43438)
                //   pdbid(4KF7), resi( 1096), atoms(  17066) :   ssNMA-18-500-001                 : hess(  9.2 sec), coarse(  4.1 min), mode(    0.0 sec): coarse graining avg of iter(  38), sec( 6.23), MemoryMb( 473), RemAtom(420), B-SetZero( 3832), B-NonZero( 3104), BDC-IgnAdd( 83878)
                //   pdbid(4KF7), resi( 1096), atoms(  17066) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  9.3 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter( 102), sec( 1.54), MemoryMb( 626), RemAtom(156), B-SetZero( 2946), B-NonZero( 1429), BDC-IgnAdd( 30598)
                //   pdbid(4KF7), resi( 1096), atoms(  17066) :   ssNMA-symrcm-blockwise-10-189-001: hess(  9.2 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  85), sec( 1.94), MemoryMb( 580), RemAtom(187), B-SetZero( 3148), B-NonZero( 1617), BDC-IgnAdd( 37317)
                //   pdbid(3VA7), resi( 1130), atoms(  17610) :   ssNMA-18-500-001                 : hess(  9.7 sec), coarse(  4.7 min), mode(    0.0 sec): coarse graining avg of iter(  40), sec( 6.75), MemoryMb( 467), RemAtom(412), B-SetZero( 4460), B-NonZero( 3299), BDC-IgnAdd( 93162)
                //   pdbid(3VA7), resi( 1130), atoms(  17610) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  9.6 sec), coarse(  3.8 min), mode(    0.0 sec): coarse graining avg of iter( 109), sec( 1.85), MemoryMb( 666), RemAtom(151), B-SetZero( 3593), B-NonZero( 1620), BDC-IgnAdd( 37396)
                //   pdbid(3VA7), resi( 1130), atoms(  17610) :   ssNMA-symrcm-blockwise-10-189-001: hess(  9.5 sec), coarse(  3.6 min), mode(    0.0 sec): coarse graining avg of iter(  92), sec( 2.15), MemoryMb( 627), RemAtom(179), B-SetZero( 3678), B-NonZero( 1737), BDC-IgnAdd( 42041)
                //   pdbid(3SHF), resi( 1133), atoms(  17931) :   ssNMA-18-500-001                 : hess(  9.4 sec), coarse(  4.2 min), mode(    0.0 sec): coarse graining avg of iter(  41), sec( 5.84), MemoryMb( 461), RemAtom(409), B-SetZero( 3783), B-NonZero( 3106), BDC-IgnAdd( 71591)
                //   pdbid(3SHF), resi( 1133), atoms(  17931) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  9.4 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter( 112), sec( 1.52), MemoryMb( 571), RemAtom(149), B-SetZero( 3051), B-NonZero( 1537), BDC-IgnAdd( 26427)
                //   pdbid(3SHF), resi( 1133), atoms(  17931) :   ssNMA-symrcm-blockwise-10-189-001: hess(  9.2 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter(  88), sec( 2.04), MemoryMb( 522), RemAtom(190), B-SetZero( 3221), B-NonZero( 1767), BDC-IgnAdd( 35025)
                //   pdbid(2HPM), resi( 1143), atoms(  18323) :   ssNMA-18-500-001                 : hess(  9.1 sec), coarse(  3.5 min), mode(    0.0 sec): coarse graining avg of iter(  42), sec( 4.77), MemoryMb( 425), RemAtom(409), B-SetZero( 3105), B-NonZero( 2617), BDC-IgnAdd( 55196)
                //   pdbid(2HPM), resi( 1143), atoms(  18323) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  9.1 sec), coarse(  2.9 min), mode(    0.0 sec): coarse graining avg of iter( 113), sec( 1.31), MemoryMb( 521), RemAtom(152), B-SetZero( 2462), B-NonZero( 1248), BDC-IgnAdd( 21729)
                //   pdbid(2HPM), resi( 1143), atoms(  18323) :   ssNMA-symrcm-blockwise-10-189-001: hess(  9.1 sec), coarse(  2.9 min), mode(    0.0 sec): coarse graining avg of iter(  95), sec( 1.66), MemoryMb( 482), RemAtom(180), B-SetZero( 2677), B-NonZero( 1428), BDC-IgnAdd( 28029)
                //   pdbid(1MWH), resi( 1256), atoms(  19798) :   ssNMA-18-500-001                 : hess( 10.7 sec), coarse(  5.8 min), mode(    0.0 sec): coarse graining avg of iter(  43), sec( 7.78), MemoryMb( 538), RemAtom(431), B-SetZero( 4992), B-NonZero( 3504), BDC-IgnAdd(106185)
                //   pdbid(1MWH), resi( 1256), atoms(  19798) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 10.8 sec), coarse(  4.2 min), mode(    0.0 sec): coarse graining avg of iter( 119), sec( 1.90), MemoryMb( 579), RemAtom(155), B-SetZero( 3790), B-NonZero( 1666), BDC-IgnAdd( 37431)
                //   pdbid(1MWH), resi( 1256), atoms(  19798) :   ssNMA-symrcm-blockwise-10-189-001: hess( 10.8 sec), coarse(  4.3 min), mode(    0.0 sec): coarse graining avg of iter( 101), sec( 2.30), MemoryMb( 523), RemAtom(183), B-SetZero( 3912), B-NonZero( 1840), BDC-IgnAdd( 45674)
                //   pdbid(3UMM), resi( 1284), atoms(  19622) :   ssNMA-18-500-001                 : hess( 11.0 sec), coarse(  6.7 min), mode(    0.0 sec): coarse graining avg of iter(  43), sec( 9.06), MemoryMb( 524), RemAtom(426), B-SetZero( 5689), B-NonZero( 3726), BDC-IgnAdd(135604)
                //   pdbid(3UMM), resi( 1284), atoms(  19622) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 11.0 sec), coarse(  5.0 min), mode(    0.0 sec): coarse graining avg of iter( 122), sec( 2.19), MemoryMb( 689), RemAtom(150), B-SetZero( 4516), B-NonZero( 1806), BDC-IgnAdd( 51253)
                //   pdbid(3UMM), resi( 1284), atoms(  19622) :   ssNMA-symrcm-blockwise-10-189-001: hess( 11.1 sec), coarse(  4.7 min), mode(    0.0 sec): coarse graining avg of iter( 104), sec( 2.48), MemoryMb( 663), RemAtom(176), B-SetZero( 4495), B-NonZero( 1916), BDC-IgnAdd( 53872)
                //   pdbid(1WYG), resi( 1297), atoms(  20119) :   ssNMA-18-500-001                 : hess( 11.5 sec), coarse(  6.1 min), mode(    0.0 sec): coarse graining avg of iter(  44), sec( 7.98), MemoryMb( 526), RemAtom(427), B-SetZero( 4966), B-NonZero( 3362), BDC-IgnAdd(113216)
                //   pdbid(1WYG), resi( 1297), atoms(  20119) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 11.5 sec), coarse(  4.5 min), mode(    0.0 sec): coarse graining avg of iter( 122), sec( 1.97), MemoryMb( 596), RemAtom(154), B-SetZero( 3986), B-NonZero( 1679), BDC-IgnAdd( 43112)
                //   pdbid(1WYG), resi( 1297), atoms(  20119) :   ssNMA-symrcm-blockwise-10-189-001: hess( 11.5 sec), coarse(  4.8 min), mode(    0.0 sec): coarse graining avg of iter( 107), sec( 2.45), MemoryMb( 604), RemAtom(175), B-SetZero( 4390), B-NonZero( 1935), BDC-IgnAdd( 55319)
                //   pdbid(2NP0), resi( 1304), atoms(  21413) :   ssNMA-18-500-001                 : hess( 11.7 sec), coarse(  5.6 min), mode(    0.0 sec): coarse graining avg of iter(  49), sec( 6.50), MemoryMb( 516), RemAtom(410), B-SetZero( 4494), B-NonZero( 3144), BDC-IgnAdd( 80793)
                //   pdbid(2NP0), resi( 1304), atoms(  21413) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 11.5 sec), coarse(  4.3 min), mode(    0.0 sec): coarse graining avg of iter( 131), sec( 1.70), MemoryMb( 560), RemAtom(153), B-SetZero( 3556), B-NonZero( 1509), BDC-IgnAdd( 31073)
                //   pdbid(2NP0), resi( 1304), atoms(  21413) :   ssNMA-symrcm-blockwise-10-189-001: hess( 11.5 sec), coarse(  4.2 min), mode(    0.0 sec): coarse graining avg of iter( 112), sec( 1.99), MemoryMb( 562), RemAtom(179), B-SetZero( 3652), B-NonZero( 1665), BDC-IgnAdd( 33926)
                //   pdbid(4JZA), resi( 1611), atoms(  25931) :   ssNMA-18-500-001                 : hess( 13.2 sec), coarse(  5.9 min), mode(    0.0 sec): coarse graining avg of iter(  60), sec( 5.61), MemoryMb( 526), RemAtom(405), B-SetZero( 4078), B-NonZero( 2757), BDC-IgnAdd( 68697)
                //   pdbid(4JZA), resi( 1611), atoms(  25931) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 13.5 sec), coarse(  5.0 min), mode(    0.0 sec): coarse graining avg of iter( 161), sec( 1.55), MemoryMb( 670), RemAtom(151), B-SetZero( 3202), B-NonZero( 1318), BDC-IgnAdd( 25636)
                //   pdbid(4JZA), resi( 1611), atoms(  25931) :   ssNMA-symrcm-blockwise-10-189-001: hess( 13.4 sec), coarse(  5.2 min), mode(    0.0 sec): coarse graining avg of iter( 140), sec( 1.95), MemoryMb( 668), RemAtom(173), B-SetZero( 3569), B-NonZero( 1518), BDC-IgnAdd( 32961)
                //   pdbid(2VSE), resi( 1645), atoms(  26369) :   ssNMA-18-500-001                 : hess( 13.7 sec), coarse(  6.7 min), mode(    0.0 sec): coarse graining avg of iter(  61), sec( 6.19), MemoryMb( 581), RemAtom(405), B-SetZero( 4842), B-NonZero( 3110), BDC-IgnAdd( 70214)
                //   pdbid(2VSE), resi( 1645), atoms(  26369) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 13.7 sec), coarse(  5.9 min), mode(    0.0 sec): coarse graining avg of iter( 158), sec( 1.94), MemoryMb( 759), RemAtom(156), B-SetZero( 4043), B-NonZero( 1664), BDC-IgnAdd( 29883)
                //   pdbid(2VSE), resi( 1645), atoms(  26369) :   ssNMA-symrcm-blockwise-10-189-001: hess( 14.2 sec), coarse(  6.0 min), mode(    0.0 sec): coarse graining avg of iter( 128), sec( 2.49), MemoryMb( 729), RemAtom(193), B-SetZero( 4528), B-NonZero( 1967), BDC-IgnAdd( 38086)
                //   pdbid(1XD4), resi( 1648), atoms(  27138) :   ssNMA-18-500-001                 : hess( 13.2 sec), coarse(  5.4 min), mode(    0.0 sec): coarse graining avg of iter(  64), sec( 4.69), MemoryMb( 537), RemAtom(398), B-SetZero( 3523), B-NonZero( 2596), BDC-IgnAdd( 53007)
                //   pdbid(1XD4), resi( 1648), atoms(  27138) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 13.5 sec), coarse(  4.8 min), mode(    0.0 sec): coarse graining avg of iter( 162), sec( 1.48), MemoryMb( 646), RemAtom(157), B-SetZero( 2700), B-NonZero( 1283), BDC-IgnAdd( 20383)
                //   pdbid(1XD4), resi( 1648), atoms(  27138) :   ssNMA-symrcm-blockwise-10-189-001: hess( 13.3 sec), coarse(  4.6 min), mode(    0.0 sec): coarse graining avg of iter( 135), sec( 1.77), MemoryMb( 578), RemAtom(188), B-SetZero( 2832), B-NonZero( 1429), BDC-IgnAdd( 24432)
                //   pdbid(3B8C), resi( 1666), atoms(  26042) :   ssNMA-18-500-001                 : hess( 13.0 sec), coarse(  5.6 min), mode(    0.0 sec): coarse graining avg of iter(  59), sec( 5.38), MemoryMb( 537), RemAtom(413), B-SetZero( 3423), B-NonZero( 2872), BDC-IgnAdd( 62461)
                //   pdbid(3B8C), resi( 1666), atoms(  26042) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 12.8 sec), coarse(  4.7 min), mode(    0.0 sec): coarse graining avg of iter( 154), sec( 1.56), MemoryMb( 674), RemAtom(158), B-SetZero( 2541), B-NonZero( 1347), BDC-IgnAdd( 22718)
                //   pdbid(3B8C), resi( 1666), atoms(  26042) :   ssNMA-symrcm-blockwise-10-189-001: hess( 13.5 sec), coarse(  4.9 min), mode(    0.0 sec): coarse graining avg of iter( 134), sec( 1.88), MemoryMb( 623), RemAtom(181), B-SetZero( 2759), B-NonZero( 1519), BDC-IgnAdd( 26983)
                //   pdbid(2JE8), resi( 1678), atoms(  27030) :   ssNMA-18-500-001                 : hess( 14.4 sec), coarse(  8.2 min), mode(    0.0 sec): coarse graining avg of iter(  62), sec( 7.57), MemoryMb( 611), RemAtom(408), B-SetZero( 5785), B-NonZero( 3404), BDC-IgnAdd( 95941)
                //   pdbid(2JE8), resi( 1678), atoms(  27030) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 14.4 sec), coarse(  6.8 min), mode(    0.0 sec): coarse graining avg of iter( 165), sec( 2.13), MemoryMb( 827), RemAtom(153), B-SetZero( 4574), B-NonZero( 1724), BDC-IgnAdd( 38810)
                //   pdbid(2JE8), resi( 1678), atoms(  27030) :   ssNMA-symrcm-blockwise-10-189-001: hess( 14.6 sec), coarse(  6.9 min), mode(    0.0 sec): coarse graining avg of iter( 150), sec( 2.44), MemoryMb( 812), RemAtom(169), B-SetZero( 4866), B-NonZero( 1872), BDC-IgnAdd( 45144)
                //   pdbid(2EAB), resi( 1769), atoms(  26341) :   ssNMA-18-500-001                 : hess( 14.9 sec), coarse(  8.7 min), mode(    0.0 sec): coarse graining avg of iter(  57), sec( 8.81), MemoryMb( 688), RemAtom(431), B-SetZero( 6134), B-NonZero( 3687), BDC-IgnAdd(119997)
                //   pdbid(2EAB), resi( 1769), atoms(  26341) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 14.8 sec), coarse(  6.9 min), mode(    0.0 sec): coarse graining avg of iter( 154), sec( 2.36), MemoryMb( 696), RemAtom(159), B-SetZero( 4910), B-NonZero( 1859), BDC-IgnAdd( 47528)
                //   pdbid(2EAB), resi( 1769), atoms(  26341) :   ssNMA-symrcm-blockwise-10-189-001: hess( 14.9 sec), coarse(  7.1 min), mode(    0.0 sec): coarse graining avg of iter( 131), sec( 2.91), MemoryMb( 611), RemAtom(187), B-SetZero( 5338), B-NonZero( 2106), BDC-IgnAdd( 58634)
                //   pdbid(4KNH), resi( 1771), atoms(  28314) :   ssNMA-18-500-001                 : hess( 14.9 sec), coarse(  6.1 min), mode(    0.0 sec): coarse graining avg of iter(  65), sec( 5.23), MemoryMb( 578), RemAtom(408), B-SetZero( 3880), B-NonZero( 2640), BDC-IgnAdd( 63269)
                //   pdbid(4KNH), resi( 1771), atoms(  28314) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 15.0 sec), coarse(  5.3 min), mode(    0.0 sec): coarse graining avg of iter( 178), sec( 1.47), MemoryMb( 778), RemAtom(149), B-SetZero( 3033), B-NonZero( 1244), BDC-IgnAdd( 22943)
                //   pdbid(4KNH), resi( 1771), atoms(  28314) :   ssNMA-symrcm-blockwise-10-189-001: hess( 15.0 sec), coarse(  5.2 min), mode(    0.0 sec): coarse graining avg of iter( 144), sec( 1.86), MemoryMb( 714), RemAtom(184), B-SetZero( 3222), B-NonZero( 1410), BDC-IgnAdd( 29064)
                //   pdbid(2B3Y), resi( 1776), atoms(  27742) :   ssNMA-18-500-001                 : hess( 15.4 sec), coarse(  8.8 min), mode(    0.0 sec): coarse graining avg of iter(  60), sec( 8.36), MemoryMb( 653), RemAtom(432), B-SetZero( 5725), B-NonZero( 3546), BDC-IgnAdd(115545)
                //   pdbid(2B3Y), resi( 1776), atoms(  27742) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 15.4 sec), coarse(  7.0 min), mode(    0.0 sec): coarse graining avg of iter( 173), sec( 2.07), MemoryMb( 826), RemAtom(150), B-SetZero( 4380), B-NonZero( 1649), BDC-IgnAdd( 40408)
                //   pdbid(2B3Y), resi( 1776), atoms(  27742) :   ssNMA-symrcm-blockwise-10-189-001: hess( 15.3 sec), coarse(  7.1 min), mode(    0.0 sec): coarse graining avg of iter( 147), sec( 2.55), MemoryMb( 768), RemAtom(176), B-SetZero( 4808), B-NonZero( 1909), BDC-IgnAdd( 49244)
                //   pdbid(1HKB), resi( 1798), atoms(  28294) :   ssNMA-18-500-001                 : hess( 15.0 sec), coarse(  6.8 min), mode(    0.0 sec): coarse graining avg of iter(  64), sec( 6.05), MemoryMb( 614), RemAtom(414), B-SetZero( 4317), B-NonZero( 3000), BDC-IgnAdd( 74280)
                //   pdbid(1HKB), resi( 1798), atoms(  28294) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 15.3 sec), coarse(  5.9 min), mode(    0.0 sec): coarse graining avg of iter( 169), sec( 1.76), MemoryMb( 820), RemAtom(156), B-SetZero( 3418), B-NonZero( 1451), BDC-IgnAdd( 29508)
                //   pdbid(1HKB), resi( 1798), atoms(  28294) :   ssNMA-symrcm-blockwise-10-189-001: hess( 15.0 sec), coarse(  5.8 min), mode(    0.0 sec): coarse graining avg of iter( 147), sec( 2.03), MemoryMb( 752), RemAtom(180), B-SetZero( 3481), B-NonZero( 1597), BDC-IgnAdd( 32443)
                //   pdbid(1Z3H), resi( 1839), atoms(  29912) :   ssNMA-18-500-001                 : hess( 15.6 sec), coarse(  6.5 min), mode(    0.0 sec): coarse graining avg of iter(  68), sec( 5.37), MemoryMb( 596), RemAtom(412), B-SetZero( 3997), B-NonZero( 2930), BDC-IgnAdd( 54761)
                //   pdbid(1Z3H), resi( 1839), atoms(  29912) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 15.6 sec), coarse(  5.8 min), mode(    0.0 sec): coarse graining avg of iter( 184), sec( 1.57), MemoryMb( 707), RemAtom(152), B-SetZero( 2939), B-NonZero( 1374), BDC-IgnAdd( 20715)
                //   pdbid(1Z3H), resi( 1839), atoms(  29912) :   ssNMA-symrcm-blockwise-10-189-001: hess( 15.6 sec), coarse(  6.0 min), mode(    0.0 sec): coarse graining avg of iter( 158), sec( 1.94), MemoryMb( 673), RemAtom(177), B-SetZero( 3292), B-NonZero( 1579), BDC-IgnAdd( 26982)
                //   pdbid(4L3T) : xyz format is incorrect. !!
                //   pdbid(4L3T) : xyz format is incorrect. !!
                //   pdbid(4L3T) : xyz format is incorrect. !!
                //   pdbid(2OKX), resi( 1908), atoms(  29290) :   ssNMA-18-500-001                 : hess( 16.5 sec), coarse( 10.1 min), mode(    0.0 sec): coarse graining avg of iter(  65), sec( 8.91), MemoryMb( 657), RemAtom(421), B-SetZero( 6421), B-NonZero( 3685), BDC-IgnAdd(121805)
                //   pdbid(2OKX), resi( 1908), atoms(  29290) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 16.6 sec), coarse(  8.2 min), mode(    0.0 sec): coarse graining avg of iter( 166), sec( 2.58), MemoryMb( 887), RemAtom(164), B-SetZero( 5269), B-NonZero( 1928), BDC-IgnAdd( 49787)
                //   pdbid(2OKX), resi( 1908), atoms(  29290) :   ssNMA-symrcm-blockwise-10-189-001: hess( 16.7 sec), coarse(  8.4 min), mode(    0.0 sec): coarse graining avg of iter( 147), sec( 3.05), MemoryMb( 838), RemAtom(186), B-SetZero( 5678), B-NonZero( 2138), BDC-IgnAdd( 59011)
                //   pdbid(2Q1F), resi( 1912), atoms(  30236) :   ssNMA-18-500-001                 : hess( 15.7 sec), coarse(  8.1 min), mode(    0.0 sec): coarse graining avg of iter(  69), sec( 6.63), MemoryMb( 640), RemAtom(410), B-SetZero( 5138), B-NonZero( 3182), BDC-IgnAdd( 82151)
                //   pdbid(2Q1F), resi( 1912), atoms(  30236) :   ssNMA-symrcm-blockwise-9-157-001 : hess( 15.6 sec), coarse(  7.0 min), mode(    0.0 sec): coarse graining avg of iter( 176), sec( 2.03), MemoryMb( 861), RemAtom(160), B-SetZero( 3997), B-NonZero( 1570), BDC-IgnAdd( 32361)
                //   pdbid(2Q1F), resi( 1912), atoms(  30236) :   ssNMA-symrcm-blockwise-10-189-001: hess( 15.9 sec), coarse(  6.9 min), mode(    0.0 sec): coarse graining avg of iter( 150), sec( 2.43), MemoryMb( 778), RemAtom(188), B-SetZero( 4199), B-NonZero( 1734), BDC-IgnAdd( 37566)
                //   pdbid(3H09), resi( 1925), atoms(  29300) :   ssNMA-18-500-001                 : hess( 15.7 sec), coarse(  7.9 min), mode(    0.0 sec): coarse graining avg of iter(  66), sec( 6.78), MemoryMb( 643), RemAtom(414), B-SetZero( 5290), B-NonZero( 3340), BDC-IgnAdd( 79153)
                //   pdbid(3H09), resi( 1925), atoms(  29300) :   ssNMA-symrcm-blockwise-9-157-001 ^C
                //  K:\bin\SimulCoarseSsnma\AnyCPU_Release>
                //  
                //  in ts4-stat
                //  total number of pdbs to test: 80
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-18-500-001                 : hess(  7.2 sec), coarse(  2.8 min), mode(    0.0 sec): coarse graining avg of iter(  28), sec( 5.71), MemoryMb( 337), RemAtom(390), B-SetZero( 2763), B-NonZero( 2761), BDC-IgnAdd( 56026)
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  7.5 sec), coarse(* 4.0 min), mode(    0.0 sec): coarse graining avg of iter(  71), sec( 1.64), MemoryMb( 349), RemAtom(153), B-SetZero( 2107), B-NonZero( 1267), BDC-IgnAdd( 21764)
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-blockwise-10-189-001: hess(  7.8 sec), coarse(  2.2 min), mode(    0.0 sec): coarse graining avg of iter(  53), sec( 2.35), MemoryMb( 267), RemAtom(206), B-SetZero( 2338), B-NonZero( 1555), BDC-IgnAdd( 31123)
                //  
                //  in reliatn-wn1 
                //  total number of pdbs to test: 80
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-18-500-001                 : hess(  5.4 sec), coarse(  1.9 min), mode(    0.0 sec): coarse graining avg of iter(  28), sec( 3.90), MemoryMb( 337), RemAtom(390), B-SetZero( 2763), B-NonZero( 2761), BDC-IgnAdd( 56026)
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  5.6 sec), coarse(* 3.1 min), mode(    0.0 sec): coarse graining avg of iter(  71), sec( 1.10), MemoryMb( 229), RemAtom(153), B-SetZero( 2107), B-NonZero( 1267), BDC-IgnAdd( 21764)
                //   pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-blockwise-10-189-001: hess(  5.3 sec), coarse(  1.5 min), mode(    0.0 sec): coarse graining avg of iter(  53), sec( 1.61), MemoryMb( 275), RemAtom(206), B-SetZero( 2338), B-NonZero( 1555), BDC-IgnAdd( 31123)
                //   pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-18-500-001                 : hess(  6.0 sec), coarse(  2.7 min), mode(    0.0 sec): coarse graining avg of iter(  27), sec( 5.85), MemoryMb( 404), RemAtom(426), B-SetZero( 3868), B-NonZero( 3362), BDC-IgnAdd( 82751)
                //   pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.0 sec), coarse(  2.1 min), mode(    0.0 sec): coarse graining avg of iter(  77), sec( 1.47), MemoryMb( 322), RemAtom(149), B-SetZero( 3200), B-NonZero( 1609), BDC-IgnAdd( 35001)
                //   pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.0 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  65), sec( 1.75), MemoryMb( 339), RemAtom(177), B-SetZero( 3360), B-NonZero( 1779), BDC-IgnAdd( 40433)
                //   pdbid(3JUX), resi(  813), atoms(  13264) :   ssNMA-18-500-001                 : hess(  6.1 sec), coarse(  2.6 min), mode(    0.0 sec): coarse graining avg of iter(  31), sec( 4.80), MemoryMb( 402), RemAtom(401), B-SetZero( 3393), B-NonZero( 2956), BDC-IgnAdd( 71749)
                //   pdbid(3JUX), resi(  813), atoms(  13264) :   ssNMA-symrcm-blockwise-9-157-001 : hess(  6.1 sec), coarse(  2.0 min), mode(    0.0 sec): coarse graining avg of iter(  83), sec( 1.30), MemoryMb( 403), RemAtom(150), B-SetZero( 2630), B-NonZero( 1420), BDC-IgnAdd( 29102)
                //   pdbid(3JUX), resi(  813), atoms(  13264) :   ssNMA-symrcm-blockwise-10-189-001: hess(  6.1 sec), coarse(  1.9 min), mode(    0.0 sec): coarse graining avg of iter(  73), sec( 1.40), MemoryMb( 405), RemAtom(170), B-SetZero( 2651), B-NonZero( 1480), BDC-IgnAdd( 28833)
                //  
                //  there are some case that ssNMA-symrcm-blockwise-9-157-001 work slower than ssNMA-18-500-001
                //  But in general (ssNMA-symrcm-blockwise-9-157-001 )
                //             and (ssNMA-symrcm-blockwise-10-189-001) have the similar performance
                //  
                //  therefore,     (ssNMA-symrcm-blockwise-10-189-001) could be a bit better choice
                // 
                // 
                // 
                // 
                //  pdbid | #resi, #atom | ssNMA-18-500-001                   | ssNMA-symrcm-blockwise-9-157-001   | ssNMA-symrcm-blockwise-10-189-001  |
                //  =====================================================================================================================================
                //   3UFH |  800,  11720 | hess(  5.9 sec), coarse(  2.2 min) | hess(  5.6 sec), coarse(  1.6 min) | hess(  5.6 sec), coarse(  1.7 min) |
                //   2WAN |  809,  12318 | hess(  6.5 sec), coarse(  3.0 min) | hess(  6.4 sec), coarse(  2.3 min) | hess(  6.4 sec), coarse(  2.2 min) |
                //   3JUX |  813,  13264 | hess(  6.4 sec), coarse(  2.9 min) | hess(  6.4 sec), coarse(  2.2 min) | hess(  6.4 sec), coarse(  2.0 min) |
                //   1I8Q |  814,  12936 | hess(  6.7 sec), coarse(  3.1 min) | hess(  6.8 sec), coarse(  2.3 min) | hess(  6.8 sec), coarse(  2.6 min) |
                //   3G2N |  815,  13216 | hess(  7.1 sec), coarse(  3.3 min) | hess(  7.0 sec), coarse(  2.4 min) | hess(  6.9 sec), coarse(  2.5 min) |
                //   1ILE |  821,  13287 | hess(  6.8 sec), coarse(  3.2 min) | hess(  6.8 sec), coarse(  2.3 min) | hess(  7.0 sec), coarse(  2.3 min) |
                //   2E1R |  828,  12987 | hess(  6.1 sec), coarse(  2.5 min) | hess(  6.1 sec), coarse(  1.9 min) | hess(  6.2 sec), coarse(  1.9 min) |
                //   3PSI |  843,  13685 | hess(  6.2 sec), coarse(  2.0 min) | hess(  6.2 sec), coarse(  1.5 min) | hess(  6.2 sec), coarse(  1.5 min) |
                //   1NO3 |  851,  13548 | hess(  7.3 sec), coarse(  3.4 min) | hess(  7.3 sec), coarse(  2.6 min) | hess(  7.3 sec), coarse(  2.4 min) |
                //   2Z5N |  855,  13715 | hess(  6.6 sec), coarse(  2.6 min) | hess(  6.7 sec), coarse(  2.0 min) | hess(  6.7 sec), coarse(  2.0 min) |
                //   1QBB |  858,  13357 | hess(  7.3 sec), coarse(  3.3 min) | hess(  7.5 sec), coarse(  2.4 min) | hess(  7.3 sec), coarse(  2.4 min) |
                //   3ND2 |  861,  13267 | hess(  6.6 sec), coarse(  2.5 min) | hess(  6.6 sec), coarse(  1.9 min) | hess(  6.6 sec), coarse(  2.0 min) |
                //   3S1S |  862,  13782 | hess(  7.1 sec), coarse(  3.0 min) | hess(  7.0 sec), coarse(  2.3 min) | hess(  7.0 sec), coarse(  2.4 min) |
                //   3WAK |  864,  13827 | hess(  7.1 sec), coarse(  3.2 min) | hess(  7.1 sec), coarse(  2.4 min) | hess(  7.0 sec), coarse(  2.5 min) |
                //   2DQM |  867,  13758 | hess(  7.2 sec), coarse(  3.6 min) | hess(  7.2 sec), coarse(  2.5 min) | hess(  7.2 sec), coarse(  2.6 min) |
                //   1FIY |  873,  13839 | hess(  7.1 sec), coarse(  3.3 min) | hess(  7.1 sec), coarse(  2.4 min) | hess(  7.1 sec), coarse(  2.6 min) |
                //   2OAJ |  875,  13746 | hess(  7.0 sec), coarse(  3.2 min) | hess(  7.0 sec), coarse(  2.4 min) | hess(  7.0 sec), coarse(  2.6 min) |
                //   3ZIM |  886,  14488 | hess(  7.3 sec), coarse(  3.1 min) | hess(  7.3 sec), coarse(  2.3 min) | hess(  7.3 sec), coarse(  2.4 min) |
                //   4FYS |  909,  14470 | hess(  7.7 sec), coarse(  4.3 min) | hess(  7.7 sec), coarse(  3.1 min) | hess(  7.8 sec), coarse(  3.1 min) |
                //   3FAH |  907,  13566 | hess(  7.8 sec), coarse(  4.2 min) | hess(  7.4 sec), coarse(  3.1 min) | hess(  7.4 sec), coarse(  3.2 min) |
                //   2QIZ |  943,  15239 | hess(  8.0 sec), coarse(  3.2 min) | hess(  8.1 sec), coarse(  2.4 min) | hess(  8.2 sec), coarse(  2.3 min) |
                //   2XVL |  944,  14860 | hess(  8.4 sec), coarse(  4.8 min) | hess(  8.4 sec), coarse(  3.4 min) | hess(  8.4 sec), coarse(  3.5 min) |
                //   3OG2 |  986,  15064 | hess(  8.3 sec), coarse(  4.7 min) | hess(  8.2 sec), coarse(  3.3 min) | hess(  8.7 sec), coarse(  3.5 min) |
                //   3W5B |  997,  15486 | hess(  7.7 sec), coarse(  3.1 min) | hess(  7.6 sec), coarse(  2.5 min) | hess(  7.7 sec), coarse(  2.5 min) |
                //   3POY | 1005,  15446 | hess(  8.0 sec), coarse(  3.0 min) | hess(  7.9 sec), coarse(  2.5 min) | hess(  7.9 sec), coarse(  2.4 min) |
                //   3EJU | 1016,  16229 | hess(  8.7 sec), coarse(  4.8 min) | hess(  8.7 sec), coarse(  3.4 min) | hess(  8.7 sec), coarse(  3.7 min) |
                //   1T9Y | 1016,  15605 | hess(  7.6 sec), coarse(  3.1 min) | hess(  7.6 sec), coarse(  2.3 min) | hess(  7.6 sec), coarse(  2.4 min) |
                //   1UG9 | 1019,  14659 | hess(  7.7 sec), coarse(  4.0 min) | hess(  7.6 sec), coarse(  2.9 min) | hess(  7.7 sec), coarse(  3.0 min) |
                //   4KWU | 1030,  15528 | hess(  8.0 sec), coarse(  4.1 min) | hess(  8.1 sec), coarse(  2.9 min) | hess(  8.0 sec), coarse(  3.1 min) |
                //   3W5M | 1030,  15549 | hess(  8.2 sec), coarse(  4.1 min) | hess(  8.2 sec), coarse(  3.2 min) | hess(  8.2 sec), coarse(  3.3 min) |
                //   1WXR | 1035,  15300 | hess(  7.8 sec), coarse(  3.8 min) | hess(  7.9 sec), coarse(  3.0 min) | hess(  7.8 sec), coarse(  2.9 min) |
                //   1ST6 | 1049,  16289 | hess(  7.9 sec), coarse(  2.6 min) | hess(  7.6 sec), coarse(  2.0 min) | hess(  7.6 sec), coarse(  2.0 min) |
                //   2FHB | 1052,  15825 | hess(  8.3 sec), coarse(  4.0 min) | hess(  8.4 sec), coarse(  3.0 min) | hess(  8.4 sec), coarse(  3.1 min) |
                //   3S5K | 1053,  17342 | hess(  9.3 sec), coarse(  4.0 min) | hess(  9.2 sec), coarse(  3.0 min) | hess(  9.2 sec), coarse(  3.0 min) |
                //   2PO4 | 1094,  16930 | hess(  9.0 sec), coarse(  4.3 min) | hess(  9.0 sec), coarse(  3.1 min) | hess(  9.0 sec), coarse(  3.3 min) |
                //   4KF7 | 1096,  17066 | hess(  9.2 sec), coarse(  4.1 min) | hess(  9.3 sec), coarse(  3.0 min) | hess(  9.2 sec), coarse(  3.0 min) |
                //   3VA7 | 1130,  17610 | hess(  9.7 sec), coarse(  4.7 min) | hess(  9.6 sec), coarse(  3.8 min) | hess(  9.5 sec), coarse(  3.6 min) |
                //   3SHF | 1133,  17931 | hess(  9.4 sec), coarse(  4.2 min) | hess(  9.4 sec), coarse(  3.2 min) | hess(  9.2 sec), coarse(  3.3 min) |
                //   2HPM | 1143,  18323 | hess(  9.1 sec), coarse(  3.5 min) | hess(  9.1 sec), coarse(  2.9 min) | hess(  9.1 sec), coarse(  2.9 min) |
                //   1MWH | 1256,  19798 | hess( 10.7 sec), coarse(  5.8 min) | hess( 10.8 sec), coarse(  4.2 min) | hess( 10.8 sec), coarse(  4.3 min) |
                //   3UMM | 1284,  19622 | hess( 11.0 sec), coarse(  6.7 min) | hess( 11.0 sec), coarse(  5.0 min) | hess( 11.1 sec), coarse(  4.7 min) |
                //   1WYG | 1297,  20119 | hess( 11.5 sec), coarse(  6.1 min) | hess( 11.5 sec), coarse(  4.5 min) | hess( 11.5 sec), coarse(  4.8 min) |
                //   2NP0 | 1304,  21413 | hess( 11.7 sec), coarse(  5.6 min) | hess( 11.5 sec), coarse(  4.3 min) | hess( 11.5 sec), coarse(  4.2 min) |
                //   4JZA | 1611,  25931 | hess( 13.2 sec), coarse(  5.9 min) | hess( 13.5 sec), coarse(  5.0 min) | hess( 13.4 sec), coarse(  5.2 min) |
                //   2VSE | 1645,  26369 | hess( 13.7 sec), coarse(  6.7 min) | hess( 13.7 sec), coarse(  5.9 min) | hess( 14.2 sec), coarse(  6.0 min) |
                //   1XD4 | 1648,  27138 | hess( 13.2 sec), coarse(  5.4 min) | hess( 13.5 sec), coarse(  4.8 min) | hess( 13.3 sec), coarse(  4.6 min) |
                //   3B8C | 1666,  26042 | hess( 13.0 sec), coarse(  5.6 min) | hess( 12.8 sec), coarse(  4.7 min) | hess( 13.5 sec), coarse(  4.9 min) |
                //   2JE8 | 1678,  27030 | hess( 14.4 sec), coarse(  8.2 min) | hess( 14.4 sec), coarse(  6.8 min) | hess( 14.6 sec), coarse(  6.9 min) |
                //   2EAB | 1769,  26341 | hess( 14.9 sec), coarse(  8.7 min) | hess( 14.8 sec), coarse(  6.9 min) | hess( 14.9 sec), coarse(  7.1 min) |
                //   4KNH | 1771,  28314 | hess( 14.9 sec), coarse(  6.1 min) | hess( 15.0 sec), coarse(  5.3 min) | hess( 15.0 sec), coarse(  5.2 min) |
                //   2B3Y | 1776,  27742 | hess( 15.4 sec), coarse(  8.8 min) | hess( 15.4 sec), coarse(  7.0 min) | hess( 15.3 sec), coarse(  7.1 min) |
                //   1HKB | 1798,  28294 | hess( 15.0 sec), coarse(  6.8 min) | hess( 15.3 sec), coarse(  5.9 min) | hess( 15.0 sec), coarse(  5.8 min) |
                //   1Z3H | 1839,  29912 | hess( 15.6 sec), coarse(  6.5 min) | hess( 15.6 sec), coarse(  5.8 min) | hess( 15.6 sec), coarse(  6.0 min) |
                //   2OKX | 1908,  29290 | hess( 16.5 sec), coarse( 10.1 min) | hess( 16.6 sec), coarse(  8.2 min) | hess( 16.7 sec), coarse(  8.4 min) |
                //   2Q1F | 1912,  30236 | hess( 15.7 sec), coarse(  8.1 min) | hess( 15.6 sec), coarse(  7.0 min) | hess( 15.9 sec), coarse(  6.9 min) |
                //  =====================================================================================================================================
                #endregion
                /// pdbid | #resi, #atom | ssNMA-18-500-001                   | ssNMA-symrcm-blockwise-9-157-001   | ssNMA-symrcm-blockwise-10-189-001  |
                /// =====================================================================================================================================
                /// avg in my computer   |                        4.4927 min  | better                 3.5091 min  | little better   0.0509+3.5091 min  |
                /// -------------------------------------------------------------------------------------------------------------------------------------
                /// in ts4-stat          |                                    |                                    |                                    |
                ///  3UFH |  800,  11720 |                  coarse(  2.8 min) |                < coarse(* 4.0 min) |                  coarse(  2.2 min) |
                /// -------------------------------------------------------------------------------------------------------------------------------------
                /// in reliatn-wn1       |                                    |                                    |                                    |
                ///  3UFH |  800,  11720 |                  coarse(  1.9 min) |                < coarse(* 3.1 min) |                  coarse(  1.5 min) |
                ///  2WAN |  809,  12318 |                  coarse(  2.7 min) |                > coarse(  2.1 min) |                  coarse(  2.0 min) |
                ///  3JUX |  813,  13264 |                  coarse(  2.6 min) |                > coarse(  2.0 min) |                  coarse(  1.9 min) |
                /// =====================================================================================================================================
                /// In my computer, [ ssNMA-symrcm-blockwise-9-157-001 (coarse 3.5091 min       ) ] is slightly better than
                ///                 [ ssNMA-symrcm-blockwise-10-189-001(coarse 3.5091 min+0.0509) ] .
                /// However, there are cases that [ ssNMA-symrcm-blockwise-9-157-001                       ] is several times slower than
                ///                               [ ssNMA-symrcm-blockwise-10-189-001 and ssNMA-18-500-001 ] .
                /// Therefore, ssNMA-symrcm-blockwise-10-189-001 is more safer choice !!
                /// =====================================================================================================================================


                List<Universe.Atom[]> resis = atoms.GroupByResidue();
                Tuple<Universe.Atom[], Universe.Atom[]>[] resi_keeps_others = resis.HSplitByMatches(keeps.HToHashSet()).HToTuple();

                //clus_width = 1;// pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  2 -> max_size   23 -> fitting to       ->  | width num-atom | ...    |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  3 -> max_size   43 ->  - 1.51976       ->  ========================================
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  4 -> max_size   44 ->  +12.7401    x   ->  |  1,   10.6 | 21,  887.3 | 41, 6232.0 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  5 -> max_size   50 ->  - 0.679447  x^2 ->  |  2,   22.0 | 22, 1008.7 | 42, 6702.2 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  6 -> max_size   71 ->  + 0.0994375 x^3 ->  |  3,   33.3 | 23, 1141.9 | 43, 7196.0 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  7 -> max_size   88 ->                  ->  |  4,   44.9 | 24, 1287.5 | 44, 7714.1 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  8 -> max_size  101 ->                  ->  |  5,   57.6 | 25, 1446.0 | 45, 8257.2 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width  9 -> max_size  129 ->                  ->  |  6,   71.9 | 26, 1618.1 | 46, 8825.7 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 10 -> max_size  160 ->                  ->  |  7,   88.5 | 27, 1804.4 | 47, 9420.3 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 11 -> max_size  186 ->                  ->  |  8,  107.8 | 28, 2005.4 | 48,10041.6 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 12 -> max_size  219 ->                  ->  |  9,  130.6 | 29, 2221.7 | 49,10690.1 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 13 -> max_size  251 ->                  ->  | 10,  157.4 | 30, 2454.0 | 50,11366.6 |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 14 -> max_size  315 ->                  ->  | 11,  188.8 | 31, 2702.8 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 15 -> max_size  395 ->                  ->  | 12,  225.3 | 32, 2968.8 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 16 -> max_size  434 ->                  ->  | 13,  267.7 | 33, 3252.5 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 17 -> max_size  561 ->                  ->  | 14,  316.5 | 34, 3554.5 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 18 -> max_size  596 ->                  ->  | 15,  372.3 | 35, 3875.4 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 19 -> max_size  688 ->                  ->  | 16,  435.7 | 36, 4215.9 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 20 -> max_size  761 ->                  ->  | 17,  507.2 | 37, 4576.5 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 21 -> max_size  827 ->                  ->  | 18,  587.6 | 38, 4957.8 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 22 -> max_size  949 ->                  ->  | 19,  677.3 | 39, 5360.5 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 23 -> max_size 1206 ->                  ->  | 20,  777.0 | 40, 5785.0 |            |
                                 // pdbid(4KWU), resi( 1030), atoms(  15528) : clus_width 24 -> max_size 1298 ->                  ->  ========================================
                Dictionary<Tuple<int, int, int>, List<Tuple<Universe.Atom[], Universe.Atom[]>>> clus_key_resis;
                clus_key_resis = GetIdxKeepListRemv_GetClusters(atoms, coords, clus_width, resi_keeps_others);

                int maxsize = 0;
                Dictionary<Tuple<int, int, int>, Tuple<int,int>> clus_key_size = new Dictionary<Tuple<int, int, int>, Tuple<int, int>>();
                foreach(var key in clus_key_resis.Keys)
                {
                    var clus = clus_key_resis[key];
                    int num_resi = clus.Count;
                    int num_atom = 0;
                    foreach(var resi in clus)
                        num_atom += resi.Item2.Length;
                    clus_key_size.Add(key, new Tuple<int, int>(num_resi, num_atom));
                    maxsize = Math.Max(maxsize, num_atom);
                }

                // merge clusters
                //num_atom_merge = 100;
                foreach(var key in clus_key_size.Keys.ToArray())
                {
                    if(clus_key_size.ContainsKey(key) == false)
                        continue;
                    int num_atom = clus_key_size[key].Item2;

                    if(num_atom > num_atom_merge*0.5)
                        // skip if block size is large enough
                        continue;

                    Tuple<int, int, int> keyto;

                    // find neighbor clusters (dx,dy,dz,num_atoms)
                    List<Tuple<Tuple<int, int, int>, Tuple<int, int>>> lst_dkey_numatom_numinter = new List<Tuple<Tuple<int, int, int>, Tuple<int, int>>>();
                    for(int dx=-1; dx<=1; dx++) for(int dy=-1; dy<=1; dy++) for(int dz=-1; dz<=1; dz++)
                    {
                        if(dx==0 && dy==0 && dz==0) continue;
                        keyto = new Tuple<int, int, int>(key.Item1+dx, key.Item2+dy, key.Item3+dz);
                        if(clus_key_size.ContainsKey(keyto) == false) continue;

                        int numatom = clus_key_size[keyto].Item2;
                        int numinter = 0;
                        foreach(var keyresi in clus_key_resis[key]) foreach(var keyatom in keyresi.Item2)
                        {
                            foreach(var keytoresi in clus_key_resis[keyto]) foreach(var keytoatom in keytoresi.Item2)
                            {
                                if(hess.HasBlock(keyatom.ID, keytoatom.ID) == false) continue;
                                double dist = (coords[keyatom.ID] - coords[keytoatom.ID]).Dist;
                                if(dist < 4)
                                    numinter++;
                            }
                        }
                        lst_dkey_numatom_numinter.Add(new Tuple<Tuple<int, int, int>, Tuple<int, int>>
                            ( new Tuple<int, int, int>(dx, dy, dz)
                            , new Tuple<int, int>(numatom, numinter)
                            ));
                    }

                    keyto = null;
                    {
                        int[] idxsort = lst_dkey_numatom_numinter.HListItem2().HListItem2().HIdxSorted().HReverse();
                        lst_dkey_numatom_numinter = lst_dkey_numatom_numinter.HSelectByIndex(idxsort).ToList();
                        foreach(var dkey_numatom_numinter in lst_dkey_numatom_numinter)
                        {
                            int dkey_numatom = dkey_numatom_numinter.Item2.Item1;
                            if(dkey_numatom == 0)
                                continue;
                    
                            if(num_atom + dkey_numatom > num_atom_merge*1.5)
                                continue;
                    
                            // find key-to mergt
                            keyto = key.HAdd(dkey_numatom_numinter.Item1);
                    
                            clus_key_resis[keyto].AddRange(clus_key_resis[key]);
                            clus_key_size [keyto] = clus_key_size[keyto].HAdd(clus_key_size[key]);
                            clus_key_resis.Remove(key);
                            clus_key_size .Remove(key);
                            break;
                        }
                    }

                    // merge with its neighbor cluster whose atoms are the least
                    // in the order of diff-1(6 neighbors as up,down,left,right,front,back),
                    //                 diff-2(8 neighbors, 2 combination of 6 neighbors like up-left, up-front)
                    //                 diff-3(12 neighbors, 3 combinations of 6 neighbors)
                    //keyto = null;
                    //List<Tuple<int, int, int, int>> all_dkey_numatom = new List<Tuple<int, int, int, int>>();
                    //foreach(int diff in diff_dkey_numatom.Keys.ToArray().HSort())
                    //{
                    //    all_dkey_numatom.AddRange(diff_dkey_numatom[diff]);
                    //    if(diff_dkey_numatom[diff].Count == 0)
                    //        continue;
                    //
                    //    var min_dkey_numatom = diff_dkey_numatom[diff].HSortByItem4().First();
                    //    if(num_atom + min_dkey_numatom.Item4 > num_atom_merge*1.5)
                    //        continue;
                    //
                    //    // find key-to mergt
                    //    keyto = key.HAdd(min_dkey_numatom.HItem123());
                    //
                    //    clus_key_resis[keyto].AddRange(clus_key_resis[key]);
                    //    clus_key_size [keyto] = clus_key_size[keyto].HAdd(clus_key_size[key]);
                    //    clus_key_resis.Remove(key);
                    //    clus_key_size .Remove(key);
                    //    break;
                    //}
                    //
                    if(keyto == null)
                    {
                    //    // if merge is not done in the above
                    //    // merge with the smallest neighbor among all
                    //    if(all_dkey_numatom.Count != 0)
                    //    {
                    //        var min_dkey_numatom = all_dkey_numatom.HSortByItem4().First();
                    //        if(num_atom + min_dkey_numatom.Item4 > num_atom_merge*1.5)
                    //            continue;
                    //
                    //        keyto = key.HAdd(min_dkey_numatom.HItem123());
                    //
                    //        clus_key_resis[keyto].AddRange(clus_key_resis[key]);
                    //        clus_key_size[keyto] = clus_key_size[keyto].HAdd(clus_key_size[key]);
                    //        clus_key_resis.Remove(key);
                    //        clus_key_size.Remove(key);
                    //    }
                    }
                }

                // determine block-wise interactions from hessian matrix
                Tuple<int, int>[] bwi; // block-wise interaction
                Dictionary<int, Tuple<int, int, int>> blk_key;
                {
                    blk_key = new Dictionary<int, Tuple<int, int, int>>();
                    Dictionary<Tuple<int, int, int>, int> key_blk = new Dictionary<Tuple<int,int,int>,int>();
                    foreach(var key in clus_key_resis.Keys)
                    {
                        int blk = blk_key.Count;
                        blk_key.Add(blk, key);
                        key_blk.Add(key, blk);
                    }

                    Dictionary<int, int> atom_blk = new Dictionary<int, int>();
                    foreach(var key in clus_key_resis.Keys)
                    {
                        foreach(var keeps_others in clus_key_resis[key])
                        {
                            foreach(var atom in keeps_others.Item1) atom_blk.Add(atom.ID, key_blk[key]);
                            foreach(var atom in keeps_others.Item2) atom_blk.Add(atom.ID, key_blk[key]);
                        }
                    }

                    double thres_zeroblk = 1;
                    if(symrcm_filter_blckwise_interact != null) thres_zeroblk = symrcm_filter_blckwise_interact.Value;
                    HashSet<Tuple<int, int>> lbwi = new HashSet<Tuple<int, int>>();
                    foreach(Tuple<int, int, MatrixByArr> bc_br_bval in hess.EnumBlocks_dep())
                    {
                        double val = bc_br_bval.Item3.HAbsMax();
                        if(val < thres_zeroblk) continue;

                        int atom_c = bc_br_bval.Item1; HDebug.Assert(atom_c == atoms[atom_c].ID);
                        int atom_r = bc_br_bval.Item2; HDebug.Assert(atom_r == atoms[atom_r].ID);
                        //if((coords[atom_c] - coords[atom_r]).Dist > 5) continue;

                        int blk_c = atom_blk[atom_c];
                        int blk_r = atom_blk[atom_r];
                        int min = Math.Min(blk_c, blk_r);
                        int max = Math.Max(blk_c, blk_r);
                        lbwi.Add(new Tuple<int, int>(min, max));
                    }
                    bwi = lbwi.ToArray();
                }

                // get re-indexing information
                int[] reidxs;
                {
                    int[]    cols = bwi.HListItem1().ToArray();
                    int[]    rows = bwi.HListItem2().ToArray();
                    Matlab.Execute("clear");
                    Matlab.PutVector("cols", cols);
                    Matlab.PutVector("rows", rows);
                    Matlab.Execute("vals = ones(size(cols));");
                    Matlab.Execute("mat = sparse(cols+1, rows+1, vals);");
                    Matlab.Execute("mat = (mat + mat');");
                    Matlab.Execute("reidxs=symrcm(mat);");
                    //Matlab.Execute("figure; spy(mat);");
                    //Matlab.Execute("figure; spy(mat(reidxs,reidxs));");
                    reidxs = Matlab.GetVectorInt("reidxs-1");
                    Matlab.Execute("clear");
                }

                //  // collect keeping atoms
                //  List<Universe.Atom> keeps = new List<Universe.Atom>();
                //  for(int i=0; i<resi_keeps_others.Length; i++)
                //  {
                //      var keeps_others = resi_keeps_others[i];
                //      if(keeps_others.Item1.Length == 0)
                //          continue;
                //      HDebug.Assert(keeps_others.Item1.Length == 1);
                //      keeps.Add(keeps_others.Item1[0]);
                //  }

                // collect removing atoms in order by re-indexing
                List<Universe.Atom[]> remvss = new List<Universe.Atom[]>();
                {
                    List<Universe.Atom> removs = new List<Universe.Atom>();
                    foreach(int blk in reidxs)
                    {
                        Tuple<int, int, int> key = blk_key[blk];

                        List<Universe.Atom> blk_others = new List<Universe.Atom>();
                        foreach(var keeps_others in clus_key_resis[key])
                            blk_others.AddRange(keeps_others.Item2);

                        remvss.Add(blk_others.ToArray());
                    }
                }

                return new Tuple<int[],int[][]>
                (
                    keeps.ListIDs(),
                    remvss.ListIDs()
                );
            }
            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // /////////////////////////////////////////////////// BlockWise
            /// pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-18-500-001                 : hess(  5.5 sec), coarse(  2.1 min), mode(    0.0 sec): coarse graining avg of iter(  28), sec( 4.34), MemoryMb( 334), RemAtom(390), B-SetZero( 2763), B-NonZero( 2761), BDC-IgnAdd( 56026)
            // /////////////////////////////////////////////////// SymrcmResiWise
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-150-001    : hess(  5.6 sec), coarse(  4.6 min), mode(    0.0 sec): coarse graining avg of iter(  77), sec( 2.28), MemoryMb( 260), RemAtom(141), B-SetZero( 3499), B-NonZero( 2054), BDC-IgnAdd( 54002)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-140-001    : hess(  5.7 sec), coarse(  4.5 min), mode(    0.0 sec): coarse graining avg of iter(  83), sec( 2.11), MemoryMb( 263), RemAtom(131), B-SetZero( 3413), B-NonZero( 1960), BDC-IgnAdd( 50239)
            /// pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-130-001    : hess(  6.2 sec), coarse(  3.1 min), mode(    0.0 sec): coarse graining avg of iter(  90), sec( 1.92), MemoryMb( 260), RemAtom(121), B-SetZero( 3264), B-NonZero( 1839), BDC-IgnAdd( 45910)
            /// pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-120-001    : hess(  5.8 sec), coarse(  2.8 min), mode(    0.0 sec): coarse graining avg of iter(  98), sec( 1.59), MemoryMb( 265), RemAtom(111), B-SetZero( 3099), B-NonZero( 1704), BDC-IgnAdd( 37588)
            /// pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-110-001    : hess(  5.9 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter( 108), sec( 1.51), MemoryMb( 265), RemAtom(101), B-SetZero( 2997), B-NonZero( 1614), BDC-IgnAdd( 35880)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-100-001    : hess(  6.6 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter( 119), sec( 1.43), MemoryMb( 274), RemAtom(91), B-SetZero( 2832), B-NonZero( 1480), BDC-IgnAdd( 33726)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-090-001    : hess(  6.5 sec), coarse(  3.2 min), mode(    0.0 sec): coarse graining avg of iter( 132), sec( 1.27), MemoryMb( 260), RemAtom(82), B-SetZero( 2667), B-NonZero( 1374), BDC-IgnAdd( 28063)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-080-001    : hess(  6.6 sec), coarse(  3.3 min), mode(    0.0 sec): coarse graining avg of iter( 152), sec( 1.14), MemoryMb( 274), RemAtom(71), B-SetZero( 2441), B-NonZero( 1220), BDC-IgnAdd( 23812)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-070-001    : hess(  6.6 sec), coarse(  3.5 min), mode(    0.0 sec): coarse graining avg of iter( 177), sec( 1.02), MemoryMb( 277), RemAtom(61), B-SetZero( 2225), B-NonZero( 1083), BDC-IgnAdd( 19011)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-060-001    : hess(  7.4 sec), coarse(  3.6 min), mode(    0.0 sec): coarse graining avg of iter( 210), sec( 0.86), MemoryMb( 280), RemAtom(52), B-SetZero( 1969), B-NonZero(  934), BDC-IgnAdd( 15284)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-050-001    : hess(  7.1 sec), coarse(  4.0 min), mode(    0.0 sec): coarse graining avg of iter( 260), sec( 0.75), MemoryMb( 271), RemAtom(42), B-SetZero( 1723), B-NonZero(  796), BDC-IgnAdd( 12267)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-resiwise-040-001    : hess(  7.4 sec), coarse(  4.5 min), mode(    0.0 sec): coarse graining avg of iter( 343), sec( 0.60), MemoryMb( 282), RemAtom(31), B-SetZero( 1413), B-NonZero(  625), BDC-IgnAdd(  8103)
            // /////////////////////////////////////////////////// SymrcmAtomWise
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-150-001    : hess(  6.5 sec), coarse(  8.6 min), mode(    0.0 sec): coarse graining avg of iter(  73), sec( 4.83), MemoryMb( 307), RemAtom(149), B-SetZero( 5589), B-NonZero( 3656), BDC-IgnAdd(133852)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-140-001    : hess(  6.1 sec), coarse( 11.4 min), mode(    0.0 sec): coarse graining avg of iter(  78), sec( 4.30), MemoryMb( 302), RemAtom(140), B-SetZero( 5506), B-NonZero( 3554), BDC-IgnAdd(118861)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-130-001    : hess(  6.1 sec), coarse( 15.0 min), mode(    0.0 sec): coarse graining avg of iter(  84), sec( 4.17), MemoryMb( 301), RemAtom(130), B-SetZero( 5222), B-NonZero( 3313), BDC-IgnAdd(114433)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-120-001    : hess(  5.9 sec), coarse(  5.9 min), mode(    0.0 sec): coarse graining avg of iter(  91), sec( 3.72), MemoryMb( 317), RemAtom(120), B-SetZero( 5120), B-NonZero( 3191), BDC-IgnAdd(106996)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-110-001    : hess(  6.3 sec), coarse(  6.0 min), mode(    0.0 sec): coarse graining avg of iter( 100), sec( 3.40), MemoryMb( 313), RemAtom(109), B-SetZero( 4786), B-NonZero( 2915), BDC-IgnAdd( 89590)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-100-001    : hess(  6.5 sec), coarse(  6.0 min), mode(    0.0 sec): coarse graining avg of iter( 110), sec( 3.11), MemoryMb( 314), RemAtom(99), B-SetZero( 4623), B-NonZero( 2764), BDC-IgnAdd( 83944)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-090-001    : hess(  6.6 sec), coarse(  6.0 min), mode(    0.0 sec): coarse graining avg of iter( 122), sec( 2.78), MemoryMb( 319), RemAtom(89), B-SetZero( 4277), B-NonZero( 2508), BDC-IgnAdd( 73938)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-080-001    : hess(  6.8 sec), coarse(  6.0 min), mode(    0.0 sec): coarse graining avg of iter( 137), sec( 2.44), MemoryMb( 307), RemAtom(79), B-SetZero( 4009), B-NonZero( 2285), BDC-IgnAdd( 65009)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-070-001    : hess(  7.1 sec), coarse(  5.9 min), mode(    0.0 sec): coarse graining avg of iter( 156), sec( 2.08), MemoryMb( 316), RemAtom(70), B-SetZero( 3745), B-NonZero( 2091), BDC-IgnAdd( 53641)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-060-001    : hess(  7.4 sec), coarse(  6.5 min), mode(    0.0 sec): coarse graining avg of iter( 182), sec( 1.92), MemoryMb( 326), RemAtom(60), B-SetZero( 3406), B-NonZero( 1868), BDC-IgnAdd( 45213)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-050-001    : hess(  7.5 sec), coarse(  6.3 min), mode(    0.0 sec): coarse graining avg of iter( 219), sec( 1.53), MemoryMb( 332), RemAtom(49), B-SetZero( 3021), B-NonZero( 1609), BDC-IgnAdd( 37170)
            //  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm-atomwise-040-001    : hess(  7.7 sec), coarse(  5.6 min), mode(    0.0 sec): coarse graining avg of iter( 273), sec( 1.07), MemoryMb( 325), RemAtom(40), B-SetZero( 2593), B-NonZero( 1357), BDC-IgnAdd( 29549)
            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
            public static Tuple<int[],int[][]> GetIdxKeepListRemv_ResiCluster_SymrcmResiWise(Universe.Atom[] atoms, Vector[] coords, HessMatrix hess, int num_atom_merge, double thres_zeroblk, params string[] nameToKeep)
            {
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //  18-500-001 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.4 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  28), sec( 6.23), MemoryMb( 336), RemAtom(390), B-SetZero( 2763), B-NonZero( 2761), BDC-IgnAdd( 56026)
                //                 pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess( 12.4 sec), coarse(  4.5 min), mode(    0.0 sec): coarse graining avg of iter(  27), sec( 9.60), MemoryMb( 382), RemAtom(426), B-SetZero( 3868), B-NonZero( 3362), BDC-IgnAdd( 82751)
                //  
                //  symrcm-400 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.6 sec), coarse(  6.1 min), mode(    0.0 sec): coarse graining avg of iter(  28), sec(12.69), MemoryMb( 330), RemAtom(390), B-SetZero( 5316), B-NonZero( 4402), BDC-IgnAdd(174094)
                //  symrcm-300 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  9.0 sec), coarse(  5.0 min), mode(    0.0 sec): coarse graining avg of iter(  38), sec( 7.57), MemoryMb( 260), RemAtom(287), B-SetZero( 4712), B-NonZero( 3495), BDC-IgnAdd(115697)
                //  symrcm-200 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  9.2 sec), coarse(  4.1 min), mode(    0.0 sec): coarse graining avg of iter(  57), sec( 4.01), MemoryMb( 236), RemAtom(191), B-SetZero( 4025), B-NonZero( 2582), BDC-IgnAdd( 71874)
                //  symrcm-150 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.9 sec), coarse(  3.9 min), mode(    0.0 sec): coarse graining avg of iter(  78), sec( 2.72), MemoryMb( 227), RemAtom(140), B-SetZero( 3517), B-NonZero( 2062), BDC-IgnAdd( 52767)
                //  symrcm-120 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.5 sec), coarse(  3.7 min), mode(    0.0 sec): coarse graining avg of iter(  98), sec( 2.00), MemoryMb( 224), RemAtom(111), B-SetZero( 3112), B-NonZero( 1713), BDC-IgnAdd( 39226)
                //                 pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess( 10.0 sec), coarse(  5.4 min), mode(    0.0 sec): coarse graining avg of iter( 104), sec( 2.87), MemoryMb( 272), RemAtom(110), B-SetZero( 4506), B-NonZero( 2109), BDC-IgnAdd( 63944)
                //  symrcm-110 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.6 sec), coarse(  3.7 min), mode(    0.0 sec): coarse graining avg of iter( 107), sec( 1.85), MemoryMb( 224), RemAtom(102), B-SetZero( 2977), B-NonZero( 1612), BDC-IgnAdd( 35300)
                //                 pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess(  9.9 sec), coarse(  5.3 min), mode(    0.0 sec): coarse graining avg of iter( 114), sec( 2.50), MemoryMb( 266), RemAtom(100), B-SetZero( 4279), B-NonZero( 1956), BDC-IgnAdd( 55397)
                //  
                /// symrcm-100 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.7 sec), coarse(  3.6 min), mode(    0.0 sec): coarse graining avg of iter( 119), sec( 1.59), MemoryMb( 220), RemAtom( 91), B-SetZero( 2772), B-NonZero( 1450), BDC-IgnAdd( 31097)
                ///                pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess( 13.1 sec), coarse(  5.5 min), mode(    0.0 sec): coarse graining avg of iter( 126), sec( 2.28), MemoryMb( 272), RemAtom(91), B-SetZero( 4067), B-NonZero( 1828), BDC-IgnAdd( 51730)
                //
                //  symrcm- 90 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  9.0 sec), coarse(  3.7 min), mode(    0.0 sec): coarse graining avg of iter( 133), sec( 1.43), MemoryMb( 222), RemAtom( 82), B-SetZero( 2638), B-NonZero( 1360), BDC-IgnAdd( 28587)
                //                 pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess( 10.0 sec), coarse(  5.3 min), mode(    0.0 sec): coarse graining avg of iter( 141), sec( 2.01), MemoryMb( 271), RemAtom( 81), B-SetZero( 3815), B-NonZero( 1677), BDC-IgnAdd( 45330)
                //  symrcm- 80 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.4 sec), coarse(  3.7 min), mode(    0.0 sec): coarse graining avg of iter( 152), sec( 1.22), MemoryMb( 221), RemAtom( 71), B-SetZero( 2427), B-NonZero( 1214), BDC-IgnAdd( 23865)
                //                 pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess(  9.8 sec), coarse(  5.2 min), mode(    0.0 sec): coarse graining avg of iter( 162), sec( 1.66), MemoryMb( 271), RemAtom( 71), B-SetZero( 3462), B-NonZero( 1476), BDC-IgnAdd( 35909)
                //  symrcm- 70 =>  
                //                 pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess( 10.0 sec), coarse(  5.2 min), mode(    0.0 sec): coarse graining avg of iter( 187), sec( 1.42), MemoryMb( 274), RemAtom( 61), B-SetZero( 3175), B-NonZero( 1313), BDC-IgnAdd( 30681)
                //  
                //  symrcm- 50 =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  9.5 sec), coarse(  4.3 min), mode(    0.0 sec): coarse graining avg of iter( 263), sec( 0.76), MemoryMb( 229), RemAtom( 41), B-SetZero( 1708), B-NonZero(  786), BDC-IgnAdd( 12315)
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if((nameToKeep == null) || (nameToKeep.Length == 0))
                    throw new HException();

                List<Universe.Atom[]> resis = atoms.GroupByResidue();
                Tuple<Universe.Atom[], Universe.Atom[]>[] resi_keeps_others = resis.HSplitByNames(nameToKeep).HToTuple();

                // determine residue-wise interactions from hessian matrix
                Tuple<int, int>[] rwi; // residue-wise interaction
                {
                    Dictionary<Universe.Atom, int> atom_resi = new Dictionary<Universe.Atom, int>();
                    for(int resi=0; resi<resis.Count; resi++)
                        foreach(var atom in resis[resi])
                            atom_resi.Add(atom, resi);

                    HashSet<Tuple<int, int>> lrwi = new HashSet<Tuple<int, int>>();
                    foreach(Tuple<int, int, MatrixByArr> bc_br_bval in hess.EnumBlocks_dep())
                    {
                        double val = bc_br_bval.Item3.HAbsMax();
                        if(val < thres_zeroblk) continue;

                        int bc = bc_br_bval.Item1; var atom_c = atoms[bc]; int resi_c = atom_resi[atom_c];
                        int br = bc_br_bval.Item2; var atom_r = atoms[br]; int resi_r = atom_resi[atom_r];
                        int min = Math.Min(resi_c, resi_r);
                        int max = Math.Max(resi_c, resi_r);
                        lrwi.Add(new Tuple<int, int>(min, max));
                    }
                    rwi = lrwi.ToArray();
                }
                GC.Collect();

                // get re-indexing information
                int[] reidxs;
                {
                    int[]    cols = rwi.HListItem1().ToArray();
                    int[]    rows = rwi.HListItem2().ToArray();
                    Matlab.Execute("clear");
                    Matlab.PutVector("cols", cols);
                    Matlab.PutVector("rows", rows);
                    Matlab.Execute("vals = ones(size(cols));");
                    Matlab.Execute("mat = sparse(cols+1, rows+1, vals);");
                    Matlab.Execute("mat = (mat + mat');");
                    Matlab.Execute("reidxs=symrcm(mat);");
                    //Matlab.Execute("figure; spy(mat);");
                    //Matlab.Execute("figure; spy(mat(reidxs,reidxs));");
                    reidxs = Matlab.GetVectorInt("reidxs-1");
                    Matlab.Execute("clear");
                }

                // collect keeping atoms
                List<Universe.Atom> keeps = new List<Universe.Atom>();
                for(int i=0; i<resi_keeps_others.Length; i++)
                {
                    var keeps_others = resi_keeps_others[i];
                    if(keeps_others.Item1.Length == 0)
                        continue;
                    HDebug.Assert(keeps_others.Item1.Length == 1);
                    keeps.Add(keeps_others.Item1[0]);
                }

                // collect removing atoms in order by re-indexing
                List<Universe.Atom[]> remvss = new List<Universe.Atom[]>();
                {
                    List<Universe.Atom> removs = new List<Universe.Atom>();
                    foreach(int reidx in reidxs)
                    {
                        int cnt_removs     = removs.Count;
                        int cnt_resi_reidx = resi_keeps_others[reidx].Item2.Length;
                        if(cnt_removs + cnt_resi_reidx >= num_atom_merge)
                        {
                            remvss.Add(removs.ToArray());
                            removs = new List<Universe.Atom>();
                        }
                        removs.AddRange(resi_keeps_others[reidx].Item2);
                        HDebug.Assert(removs.Count == cnt_removs + cnt_resi_reidx);
                    }
                    if(removs.Count > 0)
                    {
                        remvss.Add(removs.ToArray());
                        removs = new List<Universe.Atom>();
                    }
                    if(removs.Count != 0)
                        throw new Exception();
                }

                return new Tuple<int[],int[][]>
                (
                    keeps.ListIDs(),
                    remvss.ListIDs()
                );
            }

        
            public static Tuple<int[],int[][]> GetIdxKeepListRemv_ResiCluster_SymrcmAtomWise(Universe.Atom[] atoms, Vector[] coords, HessMatrix hess, int num_atom_merge, double thres_zeroblk, params string[] nameToKeep)
            {
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // when "if(val < thres_zeroblk) continue;" is *NOT* applied
                //  18-500-001           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  8.4 sec), coarse(  3.0 min), mode(    0.0 sec): coarse graining avg of iter(  28), sec( 6.23), MemoryMb( 336), RemAtom(390), B-SetZero( 2763), B-NonZero( 2761), BDC-IgnAdd( 56026)
                //                           pdbid(2WAN), resi(  809), atoms(  12318) :   ssNMA-symrcm      : hess( 12.4 sec), coarse(  4.5 min), mode(    0.0 sec): coarse graining avg of iter(  27), sec( 9.60), MemoryMb( 382), RemAtom(426), B-SetZero( 3868), B-NonZero( 3362), BDC-IgnAdd( 82751)
                //  
                //  symrcm-300           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.2 sec), coarse(  7.8 min), mode(    0.0 sec): coarse graining avg of iter(  37), sec(12.30), MemoryMb( 351), RemAtom(295), B-SetZero( 7228), B-NonZero( 5997), BDC-IgnAdd(253998)
                //  symrcm-200           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.1 sec), coarse(  6.7 min), mode(    0.0 sec): coarse graining avg of iter(  55), sec( 7.04), MemoryMb( 321), RemAtom(198), B-SetZero( 6399), B-NonZero( 4659), BDC-IgnAdd(171524)
                //  symrcm-100           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess( 10.2 sec), coarse(  5.4 min), mode(    0.0 sec): coarse graining avg of iter( 110), sec( 2.58), MemoryMb( 299), RemAtom( 99), B-SetZero( 4540), B-NonZero( 2779), BDC-IgnAdd( 80389)
                /// symrcm- 80           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.4 sec), coarse(  4.9 min), mode(    0.0 sec): coarse graining avg of iter( 137), sec( 1.92), MemoryMb( 300), RemAtom( 79), B-SetZero( 4038), B-NonZero( 2360), BDC-IgnAdd( 62472)
                /// symrcm- 70           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.4 sec), coarse(  4.9 min), mode(    0.0 sec): coarse graining avg of iter( 156), sec( 1.66), MemoryMb( 304), RemAtom( 70), B-SetZero( 3743), B-NonZero( 2133), BDC-IgnAdd( 54648)
                /// symrcm- 60           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.6 sec), coarse(  4.9 min), mode(    0.0 sec): coarse graining avg of iter( 182), sec( 1.40), MemoryMb( 304), RemAtom( 60), B-SetZero( 3358), B-NonZero( 1886), BDC-IgnAdd( 45893)
                /// symrcm- 50           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.3 sec), coarse(  4.9 min), mode(    0.0 sec): coarse graining avg of iter( 219), sec( 1.13), MemoryMb( 307), RemAtom( 49), B-SetZero( 2984), B-NonZero( 1636), BDC-IgnAdd( 36582)
                //  symrcm- 40           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.3 sec), coarse(  5.4 min), mode(    0.0 sec): coarse graining avg of iter( 273), sec( 0.97), MemoryMb( 312), RemAtom( 40), B-SetZero( 2566), B-NonZero( 1378), BDC-IgnAdd( 29234)
                //  symrcm- 25           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.0 sec), coarse(  6.2 min), mode(    0.0 sec): coarse graining avg of iter( 437), sec( 0.64), MemoryMb( 324), RemAtom( 24), B-SetZero( 1810), B-NonZero(  949), BDC-IgnAdd( 16736)
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // when "if(val < thres_zeroblk) continue;" is *APPLIED*
                //  symrcm-100           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.3 sec), coarse(  5.5 min), mode(    0.0 sec): coarse graining avg of iter( 110), sec( 2.75), MemoryMb( 302), RemAtom(99), B-SetZero( 4623), B-NonZero( 2764), BDC-IgnAdd( 83944)
                //  symrcm- 80           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.1 sec), coarse(  5.0 min), mode(    0.0 sec): coarse graining avg of iter( 137), sec( 1.97), MemoryMb( 302), RemAtom(79), B-SetZero( 4009), B-NonZero( 2285), BDC-IgnAdd( 65009)
                /// symrcm- 70           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.6 sec), coarse(  4.9 min), mode(    0.0 sec): coarse graining avg of iter( 156), sec( 1.66), MemoryMb( 302), RemAtom(70), B-SetZero( 3745), B-NonZero( 2091), BDC-IgnAdd( 53641)
                //  symrcm- 60           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  6.2 sec), coarse(  5.0 min), mode(    0.0 sec): coarse graining avg of iter( 182), sec( 1.43), MemoryMb( 306), RemAtom(60), B-SetZero( 3406), B-NonZero( 1868), BDC-IgnAdd( 45213)
                //  symrcm- 50           =>  pdbid(3UFH), resi(  800), atoms(  11720) :   ssNMA-symrcm      : hess(  7.3 sec), coarse(  5.2 min), mode(    0.0 sec): coarse graining avg of iter( 219), sec( 1.20), MemoryMb( 309), RemAtom(49), B-SetZero( 3021), B-NonZero( 1609), BDC-IgnAdd( 37170)
                // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                if((nameToKeep == null) || (nameToKeep.Length == 0))
                    throw new HException();

                List<Universe.Atom[]> resis = atoms.GroupByResidue();
                Tuple<Universe.Atom[], Universe.Atom[]>[] resi_keeps_others = resis.HSplitByNames(nameToKeep).HToTuple();

                // collect keeping atoms
                List<Universe.Atom> keeps = new List<Universe.Atom>();
                for(int i=0; i<resi_keeps_others.Length; i++)
                {
                    var keeps_others = resi_keeps_others[i];
                    if(keeps_others.Item1.Length == 0)
                        continue;
                    HDebug.Assert(keeps_others.Item1.Length == 1);
                    keeps.Add(keeps_others.Item1[0]);
                }

                // determine atom-wise interactions from hessian matrix
                List<Tuple<int, int>> awi; // atom-wise interaction
                {
                    awi = new List<Tuple<int, int>>();
                    HashSet<Universe.Atom> hkeeps = keeps.HToHashSet();
                    foreach(Tuple<int, int, MatrixByArr> bc_br_bval in hess.EnumBlocks_dep())
                    {
                        double val = bc_br_bval.Item3.HAbsMax();
                        if(val < thres_zeroblk) continue;

                        int bc = bc_br_bval.Item1; var atom_c = atoms[bc]; if(hkeeps.Contains(atom_c)) continue;
                        int br = bc_br_bval.Item2; var atom_r = atoms[br]; if(hkeeps.Contains(atom_r)) continue;
                        int min = Math.Min(atom_c.ID, atom_r.ID);
                        int max = Math.Max(atom_c.ID, atom_r.ID);
                        awi.Add(new Tuple<int, int>(min, max));
                    }
                }

                // get re-indexing information
                int[] reidxs;
                {
                    int[]    cols = awi.HListItem1().ToArray();
                    int[]    rows = awi.HListItem2().ToArray();
                    Matlab.Execute("clear");
                    Matlab.PutVector("cols", cols);
                    Matlab.PutVector("rows", rows);
                    Matlab.Execute("vals = ones(size(cols));");
                    Matlab.Execute("mat = sparse(cols+1, rows+1, vals);");
                    Matlab.Execute("mat = (mat + mat');");
                    Matlab.Execute("reidxs=symrcm(mat);");
                    //Matlab.Execute("figure; spy(mat);");
                    //Matlab.Execute("figure; spy(mat(reidxs,reidxs));");
                    reidxs = Matlab.GetVectorInt("reidxs-1");
                    Matlab.Execute("clear");

                    // remove keeping atom indixes
                    HashSet<int> keps = keeps.ListIDs().HToHashSet();
                    for(int i=0; i<reidxs.Length; i++)
                    {
                        if(keps.Contains(reidxs[i]))
                            reidxs[i] = -1;
                    }
                }

                // collect removing atoms in order by re-indexing
                List<Universe.Atom[]> remvss = new List<Universe.Atom[]>();
                {
                    List<Universe.Atom> removs = new List<Universe.Atom>();
                    foreach(int reidx in reidxs)
                    {
                        if(reidx == -1)
                            continue;
                        if(removs.Count >= num_atom_merge)
                        {
                            remvss.Add(removs.ToArray());
                            removs = new List<Universe.Atom>();
                        }
                        var atom = atoms[reidx];
                        removs.Add(atoms[reidx]);
                    }
                    if(removs.Count > 0)
                    {
                        remvss.Add(removs.ToArray());
                        removs = new List<Universe.Atom>();
                    }
                    if(removs.Count != 0)
                        throw new Exception();
                }

                return new Tuple<int[],int[][]>
                (
                    keeps.ListIDs(),
                    remvss.ListIDs()
                );
            }
        }
    }
}
