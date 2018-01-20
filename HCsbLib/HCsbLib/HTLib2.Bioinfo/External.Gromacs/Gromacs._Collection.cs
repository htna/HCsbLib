using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class GromacsCollection
    {
        public static List<double> ListMass(this IList<Gromacs.Top.Atom> atoms)
        {
            List<double> mass = new List<double>(atoms.Count);
            for(int i=0; i<atoms.Count; i++)
                mass.Add(atoms[i].mass);
            return mass;
        }
    }
}
