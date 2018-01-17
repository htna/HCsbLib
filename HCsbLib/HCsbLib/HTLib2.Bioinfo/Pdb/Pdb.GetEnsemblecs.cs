using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        public List<List<Atom>> GetEnsemble()
        {
            List<List<Atom>> ensemble;
            {
                ensemble = new List<List<Atom>>();
                ensemble.Add(new List<Atom>());
                for(int i=0; i<elements.Length; i++)
                {
                    if(Atom  .IsAtom  (elements[i].line))   ensemble.Last().Add((Atom)elements[i]);
                    if(Endmdl.IsEndmdl(elements[i].line))   ensemble.Add(new List<Atom>());
                }
                if(ensemble.Last().Count == 0)
                    ensemble.Remove(ensemble.Last());
            }
            return ensemble;
        }
    }
}
