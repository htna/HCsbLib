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
            public static Tuple<int[], int[][]> GetIdxKeepListRemv_ResiWise(Universe.Atom[] atoms, Vector[] coords)
            {
                List<Universe.Atom[]> resis = atoms.GroupByResidue();
                var resi_ca_others = resis.HSplitByNames("CA").HToTuple();

                Universe.Atom[]   cas     = new Universe.Atom[resi_ca_others.Length];
                Universe.Atom[][] otherss = new Universe.Atom[resi_ca_others.Length][];
                for(int i=0; i<resi_ca_others.Length; i++)
                {
                    var ca_others = resi_ca_others[i];
                    HDebug.Assert(ca_others.Item1.Length == 1);
                    cas    [i] = ca_others.Item1[0];
                    otherss[i] = ca_others.Item2;
                    if(i >= 1)
                    {
                        HDebug.Assert(cas[i-1].ResiduePdbId < cas[i].ResiduePdbId);
                    }
                }

                return new Tuple<int[],int[][]>
                (
                    cas.ListIDs(),
                    otherss.ListIDs()
                );
            }
            public static Tuple<int[], int[][]> GetIdxKeepListRemv_lResiCluster(Universe.Atom[] atoms, Vector[] coords, params string[] nameToKeep)
            {
                double clus_width; int num_atom_merge;
                clus_width = 12;                        // each cluster contains about 10-13 residues, and 190-220 non-ca atoms
                clus_width = 12;                        // ResiCluster  | 1AON | 664 clusters | 357537x357537 -> 24045x24045 | coarse(114.3 min) | coarse graining avg of iter( 664), sec( 7.61), MemoryMb(3025), RemAtom(167), B-SetZero( 8552), B-NonZero( 3744), BDC-IgnAdd(74964)
                clus_width = 20;                        // ResiCluster  | 1AON | 277 clusters | 357270x357270 -> 24045x24045 | coarse( 83.6 min) | coarse graining avg of iter( 277), sec(16.08), MemoryMb(2374), RemAtom(401), B-SetZero( 7610), B-NonZero( 4625), BDC-IgnAdd(95151)
                clus_width = 20;                        // ResiCluster2 | 
                clus_width = 14; num_atom_merge=300;    // ResiCluster2 | 1AON | 480 clusters | 357015x357015 -> 24045x24045 | coarse( 83.5 min) | coarse graining avg of iter( 480), sec( 8.38), MemoryMb(2683), RemAtom(231), B-SetZero( 7151), B-NonZero( 3724), BDC-IgnAdd(63116)
                clus_width = 14; num_atom_merge=400;    // ResiCluster2 | 1AON | 384 clusters | 356691x356691 -> 24045x24045 | coarse( 75.1 min) | coarse graining avg of iter( 384), sec( 9.70), MemoryMb(2819), RemAtom(289), B-SetZero( 8531), B-NonZero( 4651), BDC-IgnAdd(81300)
                clus_width = 14; num_atom_merge=500;    // ResiCluster2 | 1AON | 289 clusters | 356397x356397 -> 24045x24045 | coarse( 77.8 min) | coarse graining avg of iter( 289), sec(13.84), MemoryMb(2868), RemAtom(385), B-SetZero(10645), B-NonZero( 6175), BDC-IgnAdd(112878)
                clus_width = 14; num_atom_merge=600;    // ResiCluster2 | 1AON | 221 clusters | 356088x356088 -> 24045x24045 | coarse( 79.5 min) | coarse graining avg of iter( 221), sec(19.21), MemoryMb(2800), RemAtom(503), B-SetZero(13053), B-NonZero( 8039), BDC-IgnAdd(156219)
                clus_width = 18; num_atom_merge=400;    // ResiCluster2 | 1AON | 319 clusters | 356733x356733 -> 24045x24045 | coarse( 74.0 min) | coarse graining avg of iter( 319), sec(11.97), MemoryMb(2521), RemAtom(348), B-SetZero( 7733), B-NonZero( 4556), BDC-IgnAdd(89570)
                clus_width = 18; num_atom_merge=600;    // ResiCluster2 | 1AON | 253 clusters | 356154x356154 -> 24045x24045 | coarse( 78.3 min) | coarse graining avg of iter( 253), sec(16.35), MemoryMb(2698), RemAtom(439), B-SetZero( 9254), B-NonZero( 5734), BDC-IgnAdd(115875)
                clus_width = 16; num_atom_merge=400;    // ResiCluster2 | 1AON | 352 clusters | 356700x356700 -> 24045x24045 | coarse( 83.2 min) | coarse graining avg of iter( 352), sec(11.96), MemoryMb(2674), RemAtom(316), B-SetZero( 8277), B-NonZero( 4645), BDC-IgnAdd(87222)
                clus_width = 18; num_atom_merge=500;    // ResiCluster2 | 1AON | 290 clusters | 356385x356385 -> 24045x24045 | coarse( 71.9 min) | coarse graining avg of iter( 290), sec(12.93), MemoryMb(2596), RemAtom(383), B-SetZero( 8327), B-NonZero( 5008), BDC-IgnAdd(99928)

                /// (14,400) is little bit more optimized for densely packed protein,
                ///          because each cluster is maintained in a manageable size.
                /// (18,500) is better for sparse but larger protein (such as GroEL or Proteasome, whose shape is similar to a container),
                ///          because 
                /// Assuming globular protein is being tested (such as Ribosome), (14,400) would be better
                /// However, if large structured x-ray proteins are tested (18,500) is better.
                /// 
                /// For computation time (t) by residue number (r) with 80 proteins whose residues number is in the range of [800,8000],
                ///       the outputs with (14,400) are fit to a non-linear curve "t = a1 * r ^ 1.23",
                /// while the outputs with (18,500) are fit to a non-linear curve "t = a2 * r ^ 1.15".
                /// This implies, for large superamolecule which is too large to minimize, (18,500) is the better choice than (14,400).
                clus_width = 18; num_atom_merge=500;
                                                    
                return GetIdxKeepListRemv_ResiCluster2(atoms, coords, clus_width, num_atom_merge, nameToKeep);
            }
            #region public static Tuple<int[],int[][]> GetIdxKeepListRemv_ResiCluster(Universe univ, Vector[] coords, double clus_width)
            /*
            public static Tuple<int[],int[][]> GetIdxKeepListRemv_ResiCluster(Universe univ, Vector[] coords, double clus_width)
            {
                Universe.Atom[] atoms = univ.atoms.ToArray();
                List<Universe.Atom[]> resis = atoms.GroupByResidue(univ);
                Tuple<Universe.Atom[], Universe.Atom[]>[] resi_ca_others = resis.HSplitByNames("CA").HToTuple();

                Vector coord_min = new double[3] { double.MaxValue, double.MaxValue, double.MaxValue };
                foreach(var coord in coords)
                {
                    coord_min[0] = Math.Min(coord_min[0], coord[0]);
                    coord_min[1] = Math.Min(coord_min[1], coord[1]);
                    coord_min[2] = Math.Min(coord_min[2], coord[2]);
                }


                List<List<Tuple<Universe.Atom[], Universe.Atom[]>>> clus_resis;
                {
                    Dictionary<Tuple<int, int, int>, List<Tuple<Universe.Atom[], Universe.Atom[]>>> clus_key_resis = new Dictionary<Tuple<int, int, int>, List<Tuple<Universe.Atom[], Universe.Atom[]>>>();
                    foreach(var ca_others in resi_ca_others)
                    {
                        int caId = ca_others.Item1[0].ID;
                        HDebug.Assert(atoms[caId].AtomName == "CA");
                        Vector dcoord = coords[caId] - coord_min;
                        int ix = (int)(dcoord[0] / clus_width);
                        int iy = (int)(dcoord[1] / clus_width);
                        int iz = (int)(dcoord[2] / clus_width);
                        Tuple<int,int,int> key = new Tuple<int, int, int>(ix, iy, iz);

                        if(clus_key_resis.ContainsKey(key) == false)
                            clus_key_resis.Add(key, new List<Tuple<Universe.Atom[], Universe.Atom[]>>());
                        clus_key_resis[key].Add(ca_others);
                    }

                    clus_resis = clus_key_resis.Values.ToList();
                    while(true)
                    {
                        // merge small clusters
                        int[] counts = clus_resis.HListCount().ToArray();
                        clus_resis = clus_resis.HSelectByIndex(counts.HIdxSorted()).ToList();
                        if((clus_resis[0].Count + clus_resis[1].Count) > ((clus_width*4)/3))
                            break;
                        clus_resis[0].AddRange(clus_resis[1]);
                        clus_resis[1] = null;
                        clus_resis.RemoveAt(1);
                    }
                    clus_resis.Reverse(); // reverse since the last one will be merged first.
                }

                Universe.Atom[] keeps = new Universe.Atom[resi_ca_others.Length];
                for(int i=0; i<resi_ca_others.Length; i++)
                {
                    var ca_others = resi_ca_others[i];
                    HDebug.Assert(ca_others.Item1.Length == 1);
                    keeps[i] = ca_others.Item1[0];
                }

                int max_num_remvs = int.MinValue;
                int max_num_resis = int.MinValue;
                List<Universe.Atom[]> remvss = new List<Universe.Atom[]>();
                foreach(var cresis in clus_resis)
                {
                    List<Universe.Atom> removs = new List<Universe.Atom>();
                    foreach(var ca_others in cresis)
                    {
                        Universe.Atom ca = ca_others.Item1[0];
                        Universe.Atom[] others = ca_others.Item2;
                        removs.AddRange(others);
                    }
                    remvss.Add(removs.ToArray());
                    max_num_remvs = Math.Max(max_num_remvs, removs.Count);
                    max_num_resis = Math.Max(max_num_resis, cresis.Count);
                }
                //System.Console.Write("MaxNumClus({0,4},{1,5})", max_num_resis, max_num_remvs);

                return new Tuple<int[],int[][]>
                (
                    keeps.ListIDs(),
                    remvss.ListIDs()
                );
            }
            */
            #endregion
            public static Dictionary<Tuple<int, int, int>, List<Tuple<Universe.Atom[], Universe.Atom[]>>> GetIdxKeepListRemv_GetClusters
                ( Universe.Atom[] atoms
                , Vector[] coords
                , double clus_width
                , Tuple<Universe.Atom[], Universe.Atom[]>[] resi_keeps_others
                )
            {
                // determine the coordinate of the corner atom
                Vector coord_min = new double[3] { double.MaxValue, double.MaxValue, double.MaxValue };
                foreach(var coord in coords)
                {
                    if(coord == null)
                        continue;
                    coord_min[0] = Math.Min(coord_min[0], coord[0]);
                    coord_min[1] = Math.Min(coord_min[1], coord[1]);
                    coord_min[2] = Math.Min(coord_min[2], coord[2]);
                }
                Vector coord_avg = coords.HRemoveAll(null).Average();

                // add residues into block (18x18x18)
                // if its Ca location is within the block
                Dictionary<Tuple<int, int, int>, List<Tuple<Universe.Atom[], Universe.Atom[]>>> clus_key_resis;
                clus_key_resis = new Dictionary<Tuple<int, int, int>, List<Tuple<Universe.Atom[], Universe.Atom[]>>>();
                int num_null = 0;
                foreach(var keeps_others in resi_keeps_others)
                {
                    if((keeps_others.Item1.Length == 0) && (keeps_others.Item2.Length == 0))
                        continue;

                    Vector coord = null;
                    if(keeps_others.Item1.Length == 1)
                    {
                        // the center as the keeping atom
                        int caId = keeps_others.Item1[0].ID;
                        coord = coords[caId];
                    }
                    else
                    {
                        // center as the average of all atoms' coordinates
                        List<Vector> lcoords = new List<Vector>();
                        foreach(var atom in keeps_others.Item1) lcoords.Add(coords[atom.ID]);
                        foreach(var atom in keeps_others.Item2) lcoords.Add(coords[atom.ID]);
                        lcoords = lcoords.HRemoveAll(null).ToList();
                        if(lcoords.Count != 0)
                            coord = lcoords.Average();
                    }

                    Tuple<int,int,int> key = null;
                    if(coord == null)
                    {
                        num_null++;
                        int idx_null = -((num_null * 10) / 500 + 1);
                        key = new Tuple<int, int, int>( idx_null, 0, 0);
                    }
                    else
                    {
                        Vector dcoord = coord - coord_min;
                        int ix = (int)(dcoord[0] / clus_width);
                        int iy = (int)(dcoord[1] / clus_width);
                        int iz = (int)(dcoord[2] / clus_width);
                        key = new Tuple<int, int, int>(ix, iy, iz);
                    }

                    if(clus_key_resis.ContainsKey(key) == false)
                        clus_key_resis.Add(key, new List<Tuple<Universe.Atom[], Universe.Atom[]>>());
                    clus_key_resis[key].Add(keeps_others);
                }
                return clus_key_resis;
            }
            public static Tuple<int[],int[][]> GetIdxKeepListRemv_ResiCluster2(Universe.Atom[] atoms, Vector[] coords, double clus_width, int num_atom_merge, params string[] nameToKeep)
            {
                if((nameToKeep == null) || (nameToKeep.Length == 0))
                    throw new HException();

                var split = atoms.HSplitByNames(nameToKeep);
                var keeps = split.match;
                return GetIdxKeepListRemv_ResiCluster2(atoms, coords, clus_width, num_atom_merge, keeps);
            }
            public static Tuple<int[], int[][]> GetIdxKeepListRemv_ResiCluster2(Universe.Atom[] atoms, Vector[] coords, double clus_width, int num_atom_merge, IList<Universe.Atom> keeps)
            {
                /// 1. group atoms (by residues) by blocks, whose dimensions are 18A
                ///    a. group atoms by residues
                ///    c. group residues whose Ca atoms are in a same block
                /// 2. sort blocks by its atom numbers
                ///    ex) 1,1,1,1,1, 1,1,2, 2,2, 2,3, 3, 3, 4, 4, 5
                /// 3. merge blocks into groups using threshold (500 atoms)
                ///    ex) [1,1,1,1,1], [1,1,2], [2,2], [2,3], [3], [3], [4], [4], [5]
                /// 4. reverse groups
                ///    ex) [5], [4], [4], [3], [3], [2,3], [2,2], [1,1,2], [1,1,1,1,1]
                /// 5. delete groups by the order of
                ///          9    8    7    6    5    4      3      2        1

                List<Universe.Atom> lkeeps = new List<Universe.Atom>();
                foreach(var keep in keeps)
                    if(coords[keep.ID] != null)
                        lkeeps.Add(keep);

                List<Universe.Atom[]> resis = atoms.GroupByResidue();
                Tuple<Universe.Atom[], Universe.Atom[]>[] resi_keeps_others = resis.HSplitByMatches(lkeeps.HToHashSet()).HToTuple();

                Dictionary<Tuple<int, int, int>, List<Tuple<Universe.Atom[], Universe.Atom[]>>> clus_key_resis;
                clus_key_resis = GetIdxKeepListRemv_GetClusters(atoms, coords, clus_width, resi_keeps_others);

                List<List<Tuple<Universe.Atom[], Universe.Atom[]>>> clus_resis;
                List<int> numresi;
                List<int> numatom;
                {
                    // determine #residues and #atoms in each block
                    clus_resis = clus_key_resis.Values.ToList();    // clusters(resis/CAs, residue atoms)
                    numresi = clus_resis.HListCount().ToList();     // number of resis
                    numatom = new List<int>();                      // number of atoms
                    {
                        foreach(var cresis in clus_resis)
                        {
                            IList<Universe.Atom[]> lresis = cresis.HListItem2();
                            var lnumatoms = lresis.HListCount();
                            numatom.Add(lnumatoms.Sum());
                        }
                    }

                    bool merge = true;
                    if(merge)
                    {
                        // sort by atom number
                        int[] idxsrt = numatom.HIdxSorted();
                        clus_resis = clus_resis.HSelectByIndex(idxsrt).ToList();
                        numresi    = numresi   .HSelectByIndex(idxsrt).ToList();
                        numatom    = numatom   .HSelectByIndex(idxsrt).ToList();

                        // merge clusters
                        for(int imrg=0; imrg<clus_resis.Count-1; )
                        {
                            // skip merging if number of atoms > threshold
                            if(numatom[imrg]                   > num_atom_merge) { imrg++; continue; }
                            if(numatom[imrg] + numatom[imrg+1] > num_atom_merge) { imrg++; continue; }
                            // merge clusters: add imrg+1 into imrg
                            clus_resis[imrg].AddRange(clus_resis[imrg+1]);
                            numresi   [imrg] +=       numresi   [imrg+1];
                            numatom   [imrg] +=       numatom   [imrg+1];
                            clus_resis[imrg+1] = null;
                            clus_resis.RemoveAt(imrg+1);
                            numresi   .RemoveAt(imrg+1);
                            numatom   .RemoveAt(imrg+1);
                        }

                        // reverse so
                        // from [1,1,1,1,1], [1,1,2], [2,2], [2,3], [3], [3], [4], [4], [5]
                        // to   [5], [4], [4], [3], [3], [2,3], [2,2], [1,1,2], [1,1,1,1,1]
                        // since the order of deleteing cluster is
                        //       9    8    7    6    5    4      3      2        1
                        clus_resis.Reverse();
                        numresi   .Reverse();
                        numatom   .Reverse();
                    }
                }

                if(HDebug.IsDebuggerAttached)
                {
                    HashSet<Universe.Atom> diffset = lkeeps.HToHashSet();
                    bool same = true;
                    for(int i=0; i<resi_keeps_others.Length; i++)
                    {
                        var keeps_others = resi_keeps_others[i];
                        foreach(var keep in keeps_others.Item1)
                        {
                            if(diffset.Contains(keep))
                                diffset.Remove(keep);
                            else
                                same = false;
                        }
                    }
                    if(diffset.Count != 0)
                        same = false;
                    HDebug.Assert(same == true);
                }

                int max_num_remvs = int.MinValue;
                int max_num_resis = int.MinValue;
                List<Universe.Atom[]> remvss = new List<Universe.Atom[]>();
                foreach(var cresis in clus_resis)
                {
                    List<Universe.Atom> removs = new List<Universe.Atom>();
                    foreach(var keeps_others in cresis)
                    {
                        //Universe.Atom ca = keeps_others.Item1[0];
                        Universe.Atom[] others = keeps_others.Item2;
                        removs.AddRange(others);
                    }
                    remvss.Add(removs.ToArray());
                    max_num_remvs = Math.Max(max_num_remvs, removs.Count);
                    max_num_resis = Math.Max(max_num_resis, cresis.Count);
                }
                //System.Console.Write("MaxNumClus({0,4},{1,5})", max_num_resis, max_num_remvs);

                foreach(var keep in lkeeps)
                {
                    HDebug.Exception(coords[keep.ID] != null, "keeping atom must be non-null");
                }

                return new Tuple<int[],int[][]>
                (
                    lkeeps.ListIDs(),
                    remvss.ListIDs()
                );
            }
        }
    }
}
