/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo.External
{
    public partial class Gromacs
    {
        public partial class Topol
        {
            List<List<string>> linegroups;
            List<List<Element>> groups;

            public List<Atom> GetAtoms()
            {
                foreach(List<Element> group in groups)
                {
                    if(typeof(Atom).IsInstanceOfType(group[0]))
                    {
                        List<Atom> atoms = new List<Atom>();
                        for(int i=0; i<group.Count; i++)
                            atoms.Add((Atom)(group[i]));
                        return atoms;
                    }
                }
                return null;
            }
        }
    }
}
*/