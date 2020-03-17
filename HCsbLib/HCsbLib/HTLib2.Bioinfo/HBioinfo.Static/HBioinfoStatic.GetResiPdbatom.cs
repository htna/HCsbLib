using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfoStatic
    {
        public static Dictionary<int, ValueTuple<int, Pdb.Atom, Pdb.Atom>> GetResiPdbatom
            ( Tinker.Xyz xyz
            , string prot_xyz_path// = @"..\prot.xyz"
            , string prot_pdb_path// = @"..\prot.pdb"
            )
        {
            Dictionary<int, ValueTuple<int, Pdb.Atom, Pdb.Atom>> xyzid_NearResi_NearAtom_NearConnAtom = null;
            {
                // check if prot.xyz is the same to the protein in prot_solv.xyz
                var xyz0 = Tinker.Xyz.FromFile(prot_xyz_path, false);
                bool sameprot = true;
                for(int i=0; i<xyz0.atoms.Length; i++)
                {
                    var atom  = xyz.atoms[i];
                    var atom0 = xyz0.atoms[i];
                    if(atom.Id        != atom0.Id       ) { HDebug.Assert(false); sameprot = false; }
                    if(atom.AtomId    != atom0.AtomId   ) { HDebug.Assert(false); sameprot = false; }
                    if(atom.AtomType  != atom0.AtomType ) { HDebug.Assert(false); sameprot = false; }

                    int[] bonds  = atom .BondedIds;
                    int[] bonds0 = atom0.BondedIds;
                    if(bonds.Length   != bonds0.Length  ) { HDebug.Assert(false); sameprot = false; }
                    for(int j=0; j<Math.Min(bonds.Length, bonds0.Length); j++)
                        if(bonds[j]   != bonds0[j]      ) { HDebug.Assert(false); sameprot = false; }

                    //if(atom.BondedId1 != atom0.BondedId1) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId2 != atom0.BondedId2) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId3 != atom0.BondedId3) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId4 != atom0.BondedId4) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId5 != atom0.BondedId5) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId6 != atom0.BondedId6) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId7 != atom0.BondedId7) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId8 != atom0.BondedId8) { HDebug.Assert(false); sameprot = false; }
                    //if(atom.BondedId9 != atom0.BondedId9) { HDebug.Assert(false); sameprot = false; }

                    if(sameprot == false)
                        break;
                }

                if(sameprot)
                {
                    xyzid_NearResi_NearAtom_NearConnAtom = new Dictionary<int, ValueTuple<int, Pdb.Atom, Pdb.Atom>>();

                    var pdb0 = Pdb.FromFile(prot_pdb_path);
                    KDTreeDLL.KDTree<Pdb.Atom> kdtree = new KDTreeDLL.KDTree<Pdb.Atom>(3);
                    foreach(var atom in pdb0.atoms)
                        kdtree.insert(atom.coord, atom);

                    Dictionary<int, Tinker.Xyz.Atom> xyzid_xyzatom = new Dictionary<int, Tinker.Xyz.Atom>();
                    foreach(var atom in xyz0.atoms)
                        xyzid_xyzatom.Add(atom.Id, atom);

                    foreach(var atom in xyz0.atoms)
                    {
                        var near = kdtree.nearest(atom.Coord);
                        var dist = (near.coord - atom.Coord).Dist;
                        if(dist < 0.1)
                        {
                            xyzid_NearResi_NearAtom_NearConnAtom.Add(atom.Id, new ValueTuple<int, Pdb.Atom, Pdb.Atom>(near.resSeq, near, null));
                        }
                        else
                        {
                            // atom must be hydrogen
                            HDebug.Assert(atom.BondedIds.Length == 1);
                            var conn = xyzid_xyzatom[atom.BondedId1.Value];
                            var connnear = kdtree.nearest(conn.Coord);
                            var conndist = (connnear.coord - conn.Coord).Dist;
                            HDebug.Assert(conndist < 0.1);

                            xyzid_NearResi_NearAtom_NearConnAtom.Add(atom.Id, new ValueTuple<int, Pdb.Atom, Pdb.Atom>(connnear.resSeq, null, connnear));
                        }
                    }
                }
            }
            return xyzid_NearResi_NearAtom_NearConnAtom;
        }
    }
}
