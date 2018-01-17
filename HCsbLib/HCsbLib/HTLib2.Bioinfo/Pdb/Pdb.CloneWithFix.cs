using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        public Pdb CloneWithFixResiToAla()
        {
            Atom[] atoms = this.atoms;
            int numfix = 0;
            Dictionary<Element, Atom> atom2natom = new Dictionary<Element, Atom>();
            {
                var resis = atoms.ListResidue();
                foreach(var resi in resis)
                {
                    if(resi.ResName == "ALA") continue;
                    string[] names = resi.atoms.ListName().HTrim().HSort();
                    if(names.HConcat("-") == "C-CA-CB-N-O")
                    {
                        numfix++;
                        foreach(var atom in resi.atoms)
                        {
                            Pdb.Atom natom = Atom.FromData
                                (
                                    serial     : atom.serial    ,
                                    name       : atom.name      ,
                                    resName    : "ALA"          ,
                                    chainID    : atom.chainID   ,
                                    resSeq     : atom.resSeq    ,
                                    x          : atom.x         ,
                                    y          : atom.y         ,
                                    z          : atom.z         ,
                                    altLoc     : atom.altLoc    ,
                                    iCode      : atom.iCode     ,
                                    occupancy  : atom.occupancy ,
                                    tempFactor : atom.tempFactor,
                                    element    : atom.element   ,
                                    charge     : atom.charge    
                                );
                            atom2natom.Add(atom, natom);
                        }
                    }
                }
            }
            //atoms.group();
            Element[] newelements = new Element[elements.Length];
            for(int i=0; i<elements.Length; i++)
            {
                Element element  = elements[i];
                newelements[i] = element.UpdateElement();
                if(atom2natom.ContainsKey(element))
                {
                    newelements[i] = atom2natom[element].UpdateElement();
                }
            }
            return new Pdb(newelements);
        }
    }
}
