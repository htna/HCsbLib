using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Top = Gromacs.Top;
    public static partial class GromacsStatic
    {
        public static List<T            > ListType<T>  (this IList<Top.LineElement> elements) where T : Top.LineElement { return elements.HSelectByTypeDeprec((T)null); }

        public static List<Top.Atom     > ListAtom     (this IList<Top.LineElement> elements) { return elements.HSelectByTypeDeprec((Top.Atom     )null); }
        public static List<Top.Bond     > ListBond     (this IList<Top.LineElement> elements) { return elements.HSelectByTypeDeprec((Top.Bond     )null); }
        public static List<Top.Atomtypes> ListAtomtypes(this IList<Top.LineElement> elements) { return elements.HSelectByTypeDeprec((Top.Atomtypes)null); }

        public static List<Top.Atom> SelectMatchToPdb(this IList<Top.Atom> atoms, IList<Pdb.Atom> pdbatoms)
        {
            Dictionary<int, Pdb.Atom> dict = new Dictionary<int, Pdb.Atom>(pdbatoms.Count);
            foreach(Pdb.Atom pdbatom in pdbatoms)
                dict.Add(pdbatom.serial, pdbatom);

            List<Top.Atom> select = new List<Top.Atom>();
            foreach(Top.Atom atom in atoms)
                if(dict.ContainsKey(atom.cgnr))
                {
                    Pdb.Atom pdbatom = dict[atom.cgnr];
                    if((atom.residu == pdbatom.resName) && (atom.resnr == pdbatom.resSeq))
                    {
                        HDebug.AssertOr(atom.atom.Trim() == pdbatom.name.Trim()
                                      , atom.atom.Trim() == (pdbatom.name.Substring(1)+pdbatom.name[0]).Trim()  // (top) HG21 <=> 1HG2 (pdb)
                                      );
                        select.Add(atom);
                    }
                }

            HDebug.Assert(pdbatoms.Count == select.Count);
            return select;
        }
        public static List<T> SelectSourceExtTop<T>(this IList<T> elements)
            where T : Top.LineElement
        {
            List<T> select = new List<T>();
            foreach(T element in elements)
            {
                if(element.source is string)
                    if(((string)element.source).ToLower().EndsWith(".top"))
                        select.Add(element);
            }
            return select;
        }
        public static Dictionary<int, Top.Atom> ToDictionary(this IList<Top.Atom> atoms)
        {
            Dictionary<int, Top.Atom> dict = new Dictionary<int, Top.Atom>(atoms.Count);
            foreach(Top.Atom atom in atoms)
                dict.Add(atom.cgnr, atom);
            return dict;
        }
        public static Dictionary<string, Top.Atomtypes> ToDictionary(this IList<Top.Atomtypes> atomtypes)
        {
            Dictionary<string, Top.Atomtypes> dict = new Dictionary<string, Top.Atomtypes>(atomtypes.Count);
            foreach(Top.Atomtypes atomtype in atomtypes)
                dict.Add(atomtype.name, atomtype);
            return dict;
        }
        //public static Dictionary<string, T> ToDictionary<T>(this IList<T> elements)
        //    where T : Top.IStringKey
        //{
        //    Dictionary<string, T> dict = new Dictionary<string, T>(elements.Count);
        //    foreach(T element in elements)
        //    {
        //        string key = element.GetStringKey();
        //        if(dict.ContainsKey(key))
        //        {
        //            if(element is IEquatable<T> && ((IEquatable<T>)element).Equals(dict[key]))
        //                continue;
        //            Debug.Assert(false);
        //        }
        //        dict.Add(key, element);
        //    }
        //    return dict;
        //}
        //public static List<string[]> ToListTypes<T>(this IList<T> elements)
        //    where T : Top.IStringTypes
        //{
        //    List<string[]> types = new List<string[]>(elements.Count);
        //    for(int i=0; i<elements.Count; i++)
        //        types.Add(elements[i].GetStringTypes());
        //    return types;
        //}
        //public static Dictionary<string, List<T>> ToDictionaryList<T>(this IList<T> elements)
        //    where T : Top.IStringKey
        //{
        //    Dictionary<string, List<T>> dict = new Dictionary<string, List<T>>(elements.Count);
        //    foreach(T element in elements)
        //    {
        //        string key = element.GetStringKey();
        //        if(dict.ContainsKey(key) == false)
        //        {
        //            dict.Add(key, new List<T>());
        //            dict[key].Add(element);
        //        }
        //        else
        //        {
        //            if(element is IEquatable<T>)
        //                foreach(T val in dict[key])
        //                    Debug.Assert(((IEquatable<T>)element).Equals(dict[key]) == false); // do not exist the same here here
        //            dict[key].Add(element);
        //        }
        //    }
        //    return dict;
        //}
    }
}
