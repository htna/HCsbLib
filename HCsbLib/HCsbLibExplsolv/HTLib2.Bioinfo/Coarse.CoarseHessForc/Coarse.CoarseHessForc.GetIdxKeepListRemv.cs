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

                //////////////////////////////////////////////////////////////////////////////////////////////
                // 1. find left bottom corner of solvent sphere
                Vector center = coords.Average();
                Vector min    = new double[]{ double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };
                double radius = 0;
                foreach(var coord in coords)
                {
                    radius = Math.Max(radius, (center - coord).Dist);
                    min[0] = Math.Min(min[0], coord[0]);
                    min[1] = Math.Min(min[1], coord[1]);
                    min[2] = Math.Min(min[2], coord[2]);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////
                // 2. determine protein and solvent atoms
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

                //////////////////////////////////////////////////////////////////////////////////////////////
                // 3. put solvent atoms in blocks whose size is box_size x box_size x box_size
                //
                // bx_by_bz_atoms[i,j,k] is the (i-th, j-th, k-th) box that contains solvent atoms within the box
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

                //////////////////////////////////////////////////////////////////////////////////////////////
                // 3. put protein atom indices into idxProt
                //    and solvent atom indices of each box into idxListRemv.
                //    idxListRemv[k-1] is the first atoms to be removed
                //    idxListRemv[k-2] is the second atoms to be removed
                //    ..
                //    idxListRemv[  1] is the last second atoms to be removed
                //    idxListRemv[  0] is the last atoms to be removed
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

                //////////////////////////////////////////////////////////////////////////////////////////////
                // 4. return indices of atoms to keep (protein)
                //       and a set of indices of atoms to remove iteratively (waters)
                return new Tuple<int[], int[][]>
                    ( idxProt.ToArray()
                    , idxListRemv.ToArray()
                    );
            }
        }
    }
}
