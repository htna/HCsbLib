using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class PdbCollection
	{
        public static Pdb.Element[] UpdateElement(this Pdb.Element[] elements) { return UpdateElement(new List<Pdb.Element>(elements)).ToArray(); }
        public static List<Pdb.Element> UpdateElement(this List<Pdb.Element> elements)
        {
            elements = elements.HClone();
            int? iatom_lastserial = 0;
            for(int i=0; i<elements.Count; i++)
            {
                elements[i] = elements[i].UpdateElement();
                if(elements[i] is Pdb.IAtom)
                {
                    Pdb.IAtom iatom = elements[i] as Pdb.IAtom;
                    bool hexserial = false;
                    if(iatom_lastserial >= 99999)
                    {
                        if(iatom.Serial(true) >= 99999)
                            hexserial = true;
                    }
                    iatom.hexserial = hexserial;
                    iatom_lastserial = iatom.Serial();
                }
            }
            return elements;
        }
        public static List<ELEM> ListType<ELEM>(this IList<Pdb.Element> elements)
            where ELEM : Pdb.Element
        {
            List<ELEM> elems = new List<ELEM>();
            foreach(Pdb.Element element in elements)
                if(typeof(ELEM).IsInstanceOfType(element))
                    elems.Add((ELEM)element);
            return elems;
        }
    }
}
