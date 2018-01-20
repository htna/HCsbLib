/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public static Universe BuildPdb(Pdb pdb)
        {
            return BuilderPdb.Build(pdb);
        }
        class BuilderPdb
        {
            public static Universe Build(Pdb pdb)
            {
                Tuple<string, Pdb.Atom, Pdb.Atom>[] lbonds = pdb.GetBonds();

                //Dictionary<int, List<Pdb.Atom>> ress = atoms.GroupResSeq();
                //List<int> listResSeq;
                //{
                //    listResSeq = new List<int>(ress.Keys);
                //    listResSeq = listResSeq.HSort();
                //    listResSeq = listResSeq.HGroupBySequence().HSelectLargest();
                //}
                //
                //Dictionary<Pdb.Atom, HashSet<Pdb.Atom>> bonds = new Dictionary<Pdb.Atom, HashSet<Pdb.Atom>>();
                //List<Dictionary<Pdb.Atom, HashSet<Pdb.Atom>>> ress_bonds = new List<Dictionary<Pdb.Atom, HashSet<Pdb.Atom>>>();
                //for(int i=0; i<listResSeq.Count; i++)
                //{
                //    Dictionary<Pdb.Atom, HashSet<Pdb.Atom>> res_bonds = Pdb.Struct.BuildBonds(ress[listResSeq[i]].ToArray());
                //    ress_bonds.Add(res_bonds);
                //
                //    foreach(Pdb.Atom atom in res_bonds.Keys)
                //        bonds.Add(atom, res_bonds[atom]);
                //
                //    if(i > 0)
                //    {
                //        Dictionary<Pdb.Atom, HashSet<Pdb.Atom>> prv_bonds = ress_bonds[i-1];
                //        Dictionary<Pdb.Atom, HashSet<Pdb.Atom>> cur_bonds = ress_bonds[i  ];
                //        Pdb.Atom prv_C = prv_bonds.Keys.ToArray().SelectByName("C").First();
                //        Pdb.Atom cur_N = cur_bonds.Keys.ToArray().SelectByName("N").First();
                //        HDebug.Assert(((new Vector(prv_C.coord)) - (new Vector(cur_N.coord))).Dist < 2);
                //        bonds[prv_C].Add(cur_N);
                //        bonds[cur_N].Add(prv_C);
                //    }
                //}

                int xxx = 0;
                
                
                //
                //
                //Dictionary<string, Atom> atoms   = new Dictionary<string, Atom>();
                //Dictionary<string, int>  atomids = new Dictionary<string, int>();
                //int atomId = 0;
                //foreach(Pdb.Atom pdbatom in pdb.atoms)
                //{
                //    string key = string.Format("{0}-{1:D3}", pdbatom.name, pdbatom.resSeq);
                //    if(atoms.ContainsKey(key) == false)
                //    {
                //        Atom atom = new Atom(atomids.Count);
                //        atom.AtomName    = pdbatom.name;
                //        //atom.AtomType
                //        atom.ResidueId   = pdbatom.resSeq;
                //        atom.ResidueName = pdbatom.resName;
                //        atoms  .Add(key, atom   );
                //        atomids.Add(key, atom.ID);
                //    }
                ////    Atom atom = new Atom();
                //}
                return null;
            }
        }
	}
}
*/