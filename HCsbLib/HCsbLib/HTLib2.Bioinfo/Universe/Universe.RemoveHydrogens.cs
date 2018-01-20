using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Universe
    {
        public void RemoveHydrogens(ITextLogger logger)
        {
            foreach(Atom atom in new List<Atom>(atoms.ToArray()))
            {
                if(atom.IsHydrogen() == false)
                    continue;
                if(atom.Bonds.Count != 1)
                    logger.Log("warning in removing hydrogen: {" + atom + "} has '!=1' number of bonded atom(s)");
                // isolate the atom, and remove it
                Atom bonded;
                {
                    List<Atom> bondeds = new List<Atom>(atom.Bonds[0].atoms);
                    bondeds.Remove(atom);
                    HDebug.Assert(bondeds.Count == 1);
                    bonded = bondeds[0];
                }
                atom.Isolate(this);
                // assign its partial charge and mass to its bonded heavy atom
                bonded.Charge += atom.Charge;
                bonded.Mass   += atom.Mass;
                // 
                atoms.Remove(atom);
            }
        }
    }
}
