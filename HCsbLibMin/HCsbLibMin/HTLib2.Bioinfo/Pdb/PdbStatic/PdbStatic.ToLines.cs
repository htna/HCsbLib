using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using IAtom = Pdb.IAtom;
    public static partial class PdbStatic
    {
        public static List<string> ToLines(this IList<Pdb.IAtom> elements)
        {
            List<string> lines = new List<string>(elements.Count);
            foreach(Pdb.Element element in elements)
                lines.Add(element.line);
            return lines;
        }
        public static List<string> ToLines(this IList<Pdb.Element> elements)
        {
            List<string> lines = new List<string>(elements.Count);
            foreach(Pdb.Element element in elements)
                lines.Add(element.line);
            return lines;
        }
        //public static string[] ToLines(this IList<Pdb.Element> elements)
        //{
        //    string[] lines = new string[elements.Count];
        //    for(int i=0; i<lines.Length; i++)
        //        lines[i] = elements[i].line;
        //    return lines;
        //}
    }
}
