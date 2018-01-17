using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        public Pdb GetNoAltloc()
        {
            Pdb pdb = this.Clone();
            
            char[] toremove = pdb.atoms.ListAltLoc().HListCommonT().ToArray();
            toremove = toremove.HRemoveAll(' ');
            if(toremove.Length >= 1)
                toremove = toremove.HRemoveAll(toremove[0]);

            for(int i=0; i<pdb.elements.Length; i++)
            {
                char? altLoc = null;
                if(pdb.elements[i] is Pdb.Atom  ) altLoc = (pdb.elements[i] as Pdb.Atom  ).altLoc;
                if(pdb.elements[i] is Pdb.Hetatm) altLoc = (pdb.elements[i] as Pdb.Hetatm).altLoc;

                if((altLoc != null) && toremove.Contains(altLoc.Value))
                    pdb.elements[i] = null;
            }

            pdb = new Pdb(pdb.elements);
            return pdb;
        }
    }
}
