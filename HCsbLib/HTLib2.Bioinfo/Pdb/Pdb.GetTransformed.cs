using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        public Pdb GetTransformed(Trans3 trans)
        {
            Element[] nelements = new Element[elements.Length];
            for(int i=0; i<elements.Length; i++)
            {
                if(typeof(Atom).IsInstanceOfType(elements[i]))
                {
                    Atom atom = (Atom)elements[i];
                    Vector pt = atom.coord;
                    pt = trans.DoTransform(pt);
                    atom = Atom.FromString(atom.GetUpdatedLine(pt));
                    nelements[i] = atom;
                }
                else if(typeof(Hetatm).IsInstanceOfType(elements[i]))
                {
                    Hetatm hetatm = (Hetatm)elements[i];
                    Vector pt = hetatm.coord;
                    pt = trans.DoTransform(pt);
                    hetatm = Hetatm.FromString(hetatm.GetUpdatedLine(pt));
                    nelements[i] = hetatm;
                }
                else
                {
                    nelements[i] = elements[i].UpdateElement();
                }
            }
            return new Pdb(nelements);
        }
    }
}
