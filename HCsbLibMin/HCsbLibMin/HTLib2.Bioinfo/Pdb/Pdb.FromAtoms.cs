using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public static Pdb FromAtoms(IList<Pdb.Atom> atoms)
        {
            IList<Pdb.Element> elements = atoms.HSelectByType<Pdb.Atom, Pdb.Element>();
            HDebug.Assert(atoms.Count == elements.Count);
            return new Pdb(elements);
        }
	}
}
