using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    using Xyz     = Tinker.Xyz;
    public static partial class TinkerStatic
    {
        public static (List<Xyz.Header> headers, List<Xyz.Atom> atoms, List<Element> unknown) GroupByHeaderAtom(this IList<Element> elements)
        {
            List<Xyz.Header> headers  = new List<Xyz.Header>();
            List<Xyz.Atom  > atoms    = new List<Xyz.Atom  >();
            List<Element   > unknowns = new List<Element   >();

            for(int ie=0; ie<elements.Count; ie++)
            {
                switch(elements[ie].type)
                {
                    case Xyz.Header.type:
                        headers.Add(elements[ie].Header);
                        break;
                    case Xyz.Atom.type:
                        atoms.Add(elements[ie].Atom);
                        break;
                    default:
                        HDebug.Assert(false);
                        unknowns.Add(elements[ie]);
                        break;
                }
            }

            return (headers, atoms, unknowns);
        }
    }
}
