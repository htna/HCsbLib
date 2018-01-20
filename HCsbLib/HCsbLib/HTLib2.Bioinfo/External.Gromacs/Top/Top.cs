using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public partial class Top
        {
            // http://manual.gromacs.org/online/top.html

            public LineElement[] elements;

            public List<Atom> GetAtoms()
            {
                List<Atom> atoms = new List<Atom>();
                foreach(LineElement element in elements)
                    if(element is Atom)
                        atoms.Add((Atom)element);
                return atoms;
            }
            //public Dictionary<int,Atom>    atoms;
            //public List<Bond>    bonds;
            //public List<Pair>    pairs;
            //public List<Angle>   angles;
        }
    }
}
