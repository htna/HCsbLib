using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfoStatic
    {
        public struct GetResiPdbatomItem
        {
            public Tinker.Xyz.Atom  xyzatom         ; public int      xyzid { get { return xyzatom.Id  ; } }
            public int              NearResi        ; public int      Item1 { get { return NearResi    ; } }
            public Pdb.Atom         NearAtom        ; public Pdb.Atom Item2 { get { return NearAtom    ; } }
            public Pdb.Atom         NearConnAtom    ; public Pdb.Atom Item3 { get { return NearConnAtom; } }
            public GetResiPdbatomItem(Tinker.Xyz.Atom xyzatom, int NearResi, Pdb.Atom NearAtom, Pdb.Atom NearConnAtom)
            {
                this.xyzatom      = xyzatom     ;
                this.NearResi     = NearResi    ;
                this.NearAtom     = NearAtom    ;
                this.NearConnAtom = NearConnAtom;
            }
            public static implicit operator (int NearResi, Pdb.Atom NearAtom, Pdb.Atom NearConnAtom)(GetResiPdbatomItem item)
		    {
		    	return (item.NearResi, item.NearAtom, item.NearConnAtom);
		    }
            public override string ToString()
            {
                Pdb.Atom pdbatom = (NearAtom != null) ? NearAtom : NearConnAtom;
                if(pdbatom == null)
                    return "null";

                int    id      = xyzatom.Id;
                string name    = pdbatom.name;
                string resName = pdbatom.resName;
                int    resSeq  = pdbatom.resSeq;
                double x       = xyzatom.X;
                double y       = xyzatom.Y;
                double z       = xyzatom.Z;

                string str = string.Format("{0,3}, {1}, {2}{3,3:000} ({4,6:0.000},{5,6:0.000},{6,6:0.000})", id, name, resName, resSeq, x, y, z);
                return str;
            }
        }
        public static Dictionary<int, (int NearResi, Pdb.Atom NearAtom, Pdb.Atom NearConnAtom)> HToDictionaryTuple3(this List<GetResiPdbatomItem> dict)
        {
            Dictionary<int, ValueTuple<int, Pdb.Atom, Pdb.Atom>> res = new Dictionary<int, (int, Pdb.Atom, Pdb.Atom)>();
            foreach(var item in dict)
            {
                res.Add(item.xyzid, (item.NearResi, item.NearAtom, item.NearConnAtom));
            }
            return res;
        }
        public static List<GetResiPdbatomItem> GetResiPdbatom
            ( Tinker.Xyz xyz
            , string prot_xyz_path// = @"..\prot.xyz"
            , string prot_pdb_path// = @"..\prot.pdb"
            )
        {
            List<GetResiPdbatomItem> xyzid_NearResi_NearAtom_NearConnAtom = null;
            {
                // check if prot.xyz is the same to the protein in prot_solv.xyz
                var xyz0 = Tinker.Xyz.FromFile(prot_xyz_path, false);
                List<int> bonds  = new List<int>(10);
                List<int> bonds0 = new List<int>(10);
                bool sameprot = true;
                for(int i=0; i<xyz0.atoms.Length; i++)
                {
                    var atom  = xyz.atoms[i];
                    var atom0 = xyz0.atoms[i];
                    if(atom.Id        != atom0.Id       ) { HDebug.Assert(false); sameprot = false; break; }
                    if(atom.AtomId    != atom0.AtomId   ) { HDebug.Assert(false); sameprot = false; break; }
                    if(atom.AtomType  != atom0.AtomType ) { HDebug.Assert(false); sameprot = false; break; }

                    atom .GetBondedIds(ref bonds );
                    atom0.GetBondedIds(ref bonds0);
                    if(bonds.Count    != bonds0.Count   ) { HDebug.Assert(false); sameprot = false; break; }
                    for(int j=0; j<bonds.Count; j++)
                        if(bonds[j]   != bonds0[j]      ) { HDebug.Assert(false); sameprot = false; break; }

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
                    xyzid_NearResi_NearAtom_NearConnAtom = new List<GetResiPdbatomItem>();

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
                            xyzid_NearResi_NearAtom_NearConnAtom.Add(new GetResiPdbatomItem(atom, near.resSeq, near, null));
                        }
                        else
                        {
                            // atom must be hydrogen
                            HDebug.Assert(atom.BondedIds.Length == 1);
                            var conn = xyzid_xyzatom[atom.BondedId1.Value];
                            var connnear = kdtree.nearest(conn.Coord);
                            var conndist = (connnear.coord - conn.Coord).Dist;
                            HDebug.Assert(conndist < 0.1);

                            xyzid_NearResi_NearAtom_NearConnAtom.Add(new GetResiPdbatomItem(atom, connnear.resSeq, null, connnear));
                        }
                    }
                }
            }
            return xyzid_NearResi_NearAtom_NearConnAtom;
        }
    }
}
