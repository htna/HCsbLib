using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public List<Tuple<int, int>> GetIndex(Pdb pdb, char selAltLoc='A', bool includeHydrogen=true)
        {
            List<Tuple<int,int>> idxs = new List<Tuple<int, int>>();
                                       // (idx in univ.atoms, idx in pdb)
            List<int> lidxs = new List<int>();
            for(int i=0; i<atoms.Count; i++)
            {
                Atom atm0 = atoms[i];

                if(includeHydrogen == false && atm0.IsHydrogen())
                    continue;
                lidxs.Clear();
                for(int j=0; j<pdb.atoms.Length; j++)
                {
                    Pdb.Atom atm1 = pdb.atoms[j];
                    if(atm0.AtomName     != atm1.name  ) continue;
                    if(atm0.ResiduePdbId != atm1.resSeq) continue;
                    lidxs.Add(j);
                }
                if(lidxs.Count == 1)
                {
                    int j=lidxs.First();
                    Pdb.Atom atm1 = pdb.atoms[j];
                    HDebug.Assert(atm1.altLoc == ' ' || atm1.altLoc == 'A');
                    idxs.Add(new Tuple<int, int>(i, j));
                }
                else if(lidxs.Count >= 2)
                {
                    // get index of the matching altLoc
                    int sel = -1;
                    foreach(int lidx in lidxs)
                    {
                        if(pdb.atoms[lidx].altLoc == selAltLoc)                     sel = lidx;
                        else if(pdb.atoms[lidx].altLoc == '1' && selAltLoc == 'A')  sel = lidx;
                        else if(pdb.atoms[lidx].altLoc == '2' && selAltLoc == 'B')  sel = lidx;
                        else if(pdb.atoms[lidx].altLoc == '3' && selAltLoc == 'C')  sel = lidx;
                    }
                    // put it into idxs
                    HDebug.Assert(sel != -1);
                    int j = sel;
                    Pdb.Atom atm1 = pdb.atoms[j];
                    HDebug.Assert(atm1.altLoc == selAltLoc);
                    idxs.Add(new Tuple<int, int>(i, j));
                }
            }

            return idxs;
        }
        public List<int> GetIndexHeavyAtoms()
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<size; i++)
                if(atoms[i].IsHydrogen() == false)
                    idxs.Add(i);
            return idxs;
        }
	}
}
