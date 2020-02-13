using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Coarse
    {
        public static partial class CoarseHessForcExt
        {
            public static Tuple<int[], int[][]> GetIdxKeepListRemv_RemoveHOH(object[] atoms, Vector[] coords)
            {
                Tinker.Xyz.Atom[] xyzatoms = atoms as Tinker.Xyz.Atom[];
                Dictionary<Tinker.Xyz.Atom, int> xyzatom_idx = xyzatoms.HToDictionaryAsValueIndex();
                Dictionary<int, Tinker.Xyz.Atom> ID_xyzatom = xyzatoms.ToIdDictionary();

                Vector center = coords.Average();
                Vector min    = new double[]{ double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };
                double radius = 0;                                      // 41.58
                foreach(var coord in coords)
                {
                    radius = Math.Max(radius, (center - coord).Dist);
                    min[0] = Math.Min(min[0], coord[0]);
                    min[1] = Math.Min(min[1], coord[1]);
                    min[2] = Math.Min(min[2], coord[2]);
                }
                {
                    double atomdensity
                        = coords.Length                                 // number of atoms
                        / (4.0/3.0 * Math.PI * radius*radius*radius);   // volume : 4/3 Pi r^3
                    double cutoff = 12;

                    // when reducing a single atom
                    double reduceatm_numAtom       = Math.Pow(cutoff*2, 3) * atomdensity;
                    double reduceatm_hesssize_byte = Math.Pow(reduceatm_numAtom*3, 2) * 8;
                    double reduceatm_hesssize_Gb   = reduceatm_hesssize_byte / 1000 / 1000 / 1000;

                    // when reducing a box
                    {                                                                         //atomdensity //              // 0.112// 0.112//
                        double box_size = 12;                                                               // 10   // 20   // 16   // 18   //
                        double box_numatom = Math.Pow(box_size, 3) * atomdensity;                           // 112  // 896  // 458  // 653  //
                        double reducebox_numAtom       = Math.Pow(cutoff*2 + box_size, 3) * atomdensity;    // 4404 // 9545 // 7171 // 8302 //
                        double reducebox_hesssize_byte = Math.Pow(reducebox_numAtom*3, 2) * 8;              //      //      //      //      //
                        double reducebox_hesssize_Gb   = reducebox_hesssize_byte / 1000 / 1000 / 1000;      // 1.4  // 6.6  // 3.7  // 5.0  //



                        // n=  100; H=rand(n*3); H=H+H'; tic; pinv(H); toc  =>    0.025914 seconds
                        // n= 1000; H=rand(n*3); H=H+H'; tic; pinv(H); toc  =>    5.922386 seconds
                        // n= 5000; H=rand(n*3); H=H+H'; tic; pinv(H); toc  =>  788.346814 seconds,  13.1391 minutes
                        // n= 7000; H=rand(n*3); H=H+H'; tic; pinv(H); toc  => 2117.486240 seconds,  35.2833 minutes
                        // n=16000; H=rand(n*3); H=H+H'; tic; pinv(H); toc  =>

                        /// box size | atoms in box | atoms in invD | size invD (Gb) | invD time | total invD time
                        /// 10       | 112          | 4404          | 1.4            | ~ 13 mins | n/122 * 13 = 0.106557 n
                        /// 16       | 458          | 7171          | 3.7            | 
                    }
                }

                // determine protein and solvent atoms
                HashSet<Tinker.Xyz.Atom> solv = new HashSet<Tinker.Xyz.Atom>();
                foreach(var xyzatom in xyzatoms)
                {
                    if(xyzatom.AtomType == "OT ")
                    {
                        solv.Add(xyzatom);
                        HDebug.Assert(xyzatom.BondedIds.Length == 2);
                        foreach(var ID in xyzatom.BondedIds)
                        {
                            var hydrogen = ID_xyzatom[ID];
                            solv.Add(hydrogen);
                        }
                    }
                }
                HashSet<Tinker.Xyz.Atom> prot = xyzatoms.HSetDiff(solv).HToHashSet();
                HDebug.Assert(xyzatoms.Length == prot.Count + solv.Count);

                HDictionary3<int, int, int, HashSet<Tinker.Xyz.Atom>> bx_by_bz_atoms = new HDictionary3<int, int, int, HashSet<Tinker.Xyz.Atom>>();
                int maxbx = 0;
                int maxby = 0;
                int maxbz = 0;
                foreach(var atom in solv)
                {
                    if(atom.AtomType == "OT ")
                    {
                        int box_size = 20;
                        int bx = (int)(atom.X - min[0]) / box_size; HDebug.Assert(bx >= 0); maxbx = Math.Max(maxbx, bx);
                        int by = (int)(atom.Y - min[1]) / box_size; HDebug.Assert(by >= 0); maxby = Math.Max(maxby, by);
                        int bz = (int)(atom.Z - min[2]) / box_size; HDebug.Assert(bz >= 0); maxbz = Math.Max(maxbz, bz);
                        if(bx_by_bz_atoms.ContainsKey(bx, by, bz) == false)
                            bx_by_bz_atoms.Add(bx, by, bz, new HashSet<Tinker.Xyz.Atom>());

                        var blk_atoms = bx_by_bz_atoms[bx, by, bz];
                        blk_atoms.Add(atom);
                        foreach(var ID in atom.BondedIds)
                        {
                            var hydrogen = ID_xyzatom[ID];
                            blk_atoms.Add(hydrogen);
                        }
                    }
                }

                List<int[]> idxListRemv = new List<int[]>();
                List<int>   idxProt = new List<int>();
                {
                    foreach(var atom in prot)
                    {
                        int idx = xyzatom_idx[atom];
                        idxProt.Add(idx);
                    }
                    int count = 0;
                    for(int bx=0; bx<=maxbx; bx++)
                        for(int by=0; by<=maxby; by++)
                            for(int bz=0; bz<=maxbz; bz++)
                            {
                                if(bx_by_bz_atoms.ContainsKey(bx, by, bz) == true)
                                {
                                    List<int> idxRemv = new List<int>();
                                    var blk_atoms = bx_by_bz_atoms[bx, by, bz];
                                    foreach(var atom in blk_atoms)
                                    {
                                        int idx = xyzatom_idx[atom];
                                        idxRemv.Add(idx);
                                        count++;
                                    }
                                    idxListRemv.Insert(0, idxRemv.ToArray());
                                }
                            }
                }

                return new Tuple<int[], int[][]>
                    ( idxProt.ToArray()
                    , idxListRemv.ToArray()
                    );
            }
        }
    }
}
