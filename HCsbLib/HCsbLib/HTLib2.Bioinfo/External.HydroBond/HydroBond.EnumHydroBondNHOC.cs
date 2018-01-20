using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom  = Universe.Atom;
    using Atoms = Universe.Atoms;
    public partial class HydroBond
	{
        public static IEnumerable<Tuple<Atom, Pdb.Atom>[]> EnumHydroBondNHOC(Atoms univatoms)
        {
            List<Tuple<Atom, Pdb.Atom>[]> lstNH = new List<Tuple<Atom, Pdb.Atom>[]>();
            List<Tuple<Atom, Pdb.Atom>[]> lstOC = new List<Tuple<Atom, Pdb.Atom>[]>();
            for(int i=0; i<univatoms.Count; i++)
            {
                var uatom = univatoms[i];
                var patom = uatom.sources_PdbAtom();
                if(patom == null) continue;
                string name = patom.name.Trim();

                if(univatoms[i].Inter12.Count != 1) continue;
                int bond_ID = univatoms[i].Inter12.First().ID;
                HDebug.Assert(univatoms[i].Inter12.First() == univatoms[bond_ID]);
                var bond_uatom = univatoms[bond_ID];
                var bond_patom = bond_uatom.sources_PdbAtom();
                if(bond_patom == null) continue;
                string bond_name = bond_patom.name.Trim();

                if(bond_name[0] == 'N' && name[0] == 'H')
                {
                    lstNH.Add(new Tuple<Atom, Pdb.Atom>[]{
                        new Tuple<Atom, Pdb.Atom>(bond_uatom, bond_patom),
                        new Tuple<Atom, Pdb.Atom>(     uatom,      patom),
                    });
                    continue;
                }
                if(name[0] == 'O' && bond_name[0] == 'C')
                {
                    lstOC.Add(new Tuple<Atom, Pdb.Atom>[]{
                        new Tuple<Atom, Pdb.Atom>(     uatom,      patom),
                        new Tuple<Atom, Pdb.Atom>(bond_uatom, bond_patom),
                    });
                    continue;
                }
            }

            foreach(var NH in lstNH)
            {
                foreach(var OC in lstOC)
                {
                    yield return new Tuple<Atom, Pdb.Atom>[]
                    {
                        NH[0], NH[1], OC[0], OC[1]
                    };
                }
            }
        }
    }
}
