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
                    case "Header":
                        HDebug.Assert(elements[ie] is Xyz.Header);
                        headers.Add(elements[ie] as Xyz.Header);
                        break;
                    case "Atom":
                        HDebug.Assert(elements[ie] is Xyz.Atom);
                        atoms.Add(elements[ie] as Xyz.Atom);
                        break;
                    default:
                        HDebug.Assert(false);
                        unknowns.Add(elements[ie]);
                        break;
                }
            }

            return (headers, atoms, unknowns);
        }
        public static bool IsInter12(this Tinker.Xyz.Atom atom0, Tinker.Xyz.Atom atom1)
        {
            bool inter12 = atom0.BondedIds.Contains(atom1.Id);
            HDebug.Assert( atom1.BondedIds.Contains(atom0.Id) == inter12);
            return inter12;
        }
        public static bool IsInter123(this Tinker.Xyz.Atom atom0, Tinker.Xyz.Atom atom1)
        {
            var atom0bonds = atom0.BondedIds;
            var atom1bonds = atom1.BondedIds;
            //return (atom0bonds.Intersect(atom1bonds).Count() > 0);
            foreach(int id0 in atom0bonds)
                foreach(int id1 in atom1bonds)
                    if(id0 == id1)
                        return true;
            return false;
        }
        public static bool IsInter1234
            ( this Tinker.Xyz.Atom atom0
            , Tinker.Xyz.Atom atom1
            , Dictionary<int, Tinker.Xyz.Atom> id2atom  // = atoms.HToType<object, Tinker.Xyz.Atom>().ToIdDictionary();
            )
        {
            int id1 = atom0.Id;
            var id2s = atom0.BondedIds;
            var id3s = atom1.BondedIds;
            int id4 = atom1.Id;
            foreach(int id2 in id2s)
            {
                foreach(int id3 in id2atom[id2].BondedIds)
                {
                    if(id3s.Contains(id3))
                        return true;
                }
            }
            return false;
        }
        public static Dictionary<int, Tinker.Xyz.Atom> ToIdDictionary(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            Dictionary<int, Tinker.Xyz.Atom> dict = new Dictionary<int, Tinker.Xyz.Atom>();
            foreach(Tinker.Xyz.Atom atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        public static Tinker.Xyz.Atom[] HSelectCorrectAtomType(this IList<Tinker.Xyz.Atom> atoms)
        {
            List<Tinker.Xyz.Atom> sels = new List<Tinker.Xyz.Atom>();
            foreach(var atom in atoms)
                if(atom.AtomType.Trim().Length != 0)
                    sels.Add(atom);
            return sels.ToArray();
        }
        public static Vector[] HListCoords(this IList<Tinker.Xyz.Atom> atoms)
        {
            Vector[] coords = new Vector[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                coords[i] = atoms[i].Coord;
            return coords;
        }
        public static IList<int> HListId(this IList<Tinker.Xyz.Atom> atoms)
        {
            int[] ids = new int[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                ids[i] = atoms[i].Id;
            return ids;
        }
        public static IEnumerable<int> HEnumId(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            foreach(var atom in atoms)
                yield return atom.Id;
        }
        public static IEnumerable<Tinker.Prm.Vdw> HEnumPrmVdw(this IEnumerable<Tinker.Xyz.Atom> atoms, Tinker.Prm prm)
        {
            Dictionary<int,Tinker.Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
            Dictionary<int,Tinker.Prm.Vdw > prm_cls2vdw = prm.vdws .ToClassDictionary();
            return HEnumPrmVdw(atoms, prm_id2atom, prm_cls2vdw);
        }
        public static IEnumerable<Tinker.Prm.Vdw> HEnumPrmVdw
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            , Dictionary<int,Tinker.Prm.Atom> prm_id2atom
            , Dictionary<int,Tinker.Prm.Vdw > prm_cls2vdw
            )
        {
            foreach(var atom in atoms)
                yield return atom.GetPrmVdw(prm_id2atom, prm_cls2vdw);
        }
        public static IEnumerable<Tinker.Prm.Vdw14> HEnumPrmVdw14(this IEnumerable<Tinker.Xyz.Atom> atoms, Tinker.Prm prm)
        {
            Dictionary<int,Tinker.Prm.Atom > prm_id2atom   = prm.atoms .ToIdDictionary();
            Dictionary<int,Tinker.Prm.Vdw14> prm_cls2vdw14 = prm.vdw14s.ToClassDictionary();
            return HEnumPrmVdw14(atoms, prm_id2atom, prm_cls2vdw14);
        }
        public static IEnumerable<Tinker.Prm.Vdw14> HEnumPrmVdw14
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            , Dictionary<int,Tinker.Prm.Atom > prm_id2atom
            , Dictionary<int,Tinker.Prm.Vdw14> prm_cls2vdw14
            )
        {
            foreach(var atom in atoms)
                yield return atom.GetPrmVdw14(prm_id2atom, prm_cls2vdw14);
        }

        public static IEnumerable<Tinker.Prm.Charge> HEnumPrmCharge(this IEnumerable<Tinker.Xyz.Atom> atoms, Tinker.Prm prm)
        {
            Dictionary<int,Tinker.Prm.Charge> prm_id2charge = prm.charges.ToIdDictionary();
            return HEnumPrmCharge(atoms, prm_id2charge);
        }
        public static IEnumerable<Tinker.Prm.Charge> HEnumPrmCharge
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            , Dictionary<int,Tinker.Prm.Charge> prm_id2charge
            )
        {
            foreach(var atom in atoms)
                yield return atom.GetPrmCharge(prm_id2charge);
        }

        public static Dictionary<Tinker.Xyz.Atom, Tinker.Prm.Vdw> HToDictionaryAtomPrmVdw(this IEnumerable<Tinker.Xyz.Atom> atoms, Tinker.Prm prm)
        {
            Dictionary<int,Tinker.Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
            Dictionary<int,Tinker.Prm.Vdw > prm_cls2vdw = prm.vdws .ToClassDictionary();
            return HToDictionaryAtomPrmVdw(atoms, prm_id2atom, prm_cls2vdw);
        }
        public static Dictionary<Tinker.Xyz.Atom, Tinker.Prm.Vdw> HToDictionaryAtomPrmVdw
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            , Dictionary<int,Tinker.Prm.Atom> prm_id2atom
            , Dictionary<int,Tinker.Prm.Vdw > prm_cls2vdw
            )
        {
            Dictionary<Tinker.Xyz.Atom, Tinker.Prm.Vdw> dict = new Dictionary<Tinker.Xyz.Atom, Tinker.Prm.Vdw>();
            foreach(var atom in atoms)
                dict.Add(atom, atom.GetPrmVdw(prm_id2atom, prm_cls2vdw));
            return dict;
        }
        public static IEnumerable<double> HEnumMass(this IEnumerable<Tinker.Xyz.Atom> atoms, Tinker.Prm prm)
        {
            Dictionary<int,Tinker.Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
            return HEnumMass(atoms, prm_id2atom);
        }
        public static IEnumerable<double> HEnumMass
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            , Dictionary<int,Tinker.Prm.Atom> prm_id2atom
            )
        {
            foreach(var atom in atoms)
                yield return atom.GetMass(prm_id2atom);
        }

        public static IEnumerable<Tinker.Prm.Atom> HEnumPrmAtom(this IEnumerable<Tinker.Xyz.Atom> atoms, Tinker.Prm prm)
        {
            Dictionary<int,Tinker.Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
            return HEnumPrmAtom(atoms, prm_id2atom);
        }
        public static IEnumerable<Tinker.Prm.Atom> HEnumPrmAtom
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            , Dictionary<int,Tinker.Prm.Atom> prm_id2atom
            )
        {
            foreach(var atom in atoms)
                yield return atom.GetPrmAtom(prm_id2atom);
        }

        public static Dictionary<Tinker.Xyz.Atom, double> HToDictionaryAtomMass
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            , Dictionary<int,Tinker.Prm.Atom> prm_id2atom
            )
        {
            Dictionary<Tinker.Xyz.Atom, double> dict = new Dictionary<Tinker.Xyz.Atom, double>();
            foreach(var atom in atoms)
                dict.Add(atom, atom.GetMass(prm_id2atom));
            return dict;
        }
        public static Dictionary<int, Tinker.Xyz.Atom> HToDictionaryIdAtom
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            )
        {
            Dictionary<int, Tinker.Xyz.Atom> dict = new Dictionary<int, Tinker.Xyz.Atom>();
            foreach(var atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        public class CDivideHeavyHydro
        {
            public IList<Tuple<int,Tinker.Xyz.Atom,Tinker.Prm.Atom>> lstHydrogenIdxAtmPrm;
            public IList<Tuple<int,Tinker.Xyz.Atom,Tinker.Prm.Atom>> lstHeavyIdxAtmPrm   ;
        }
        public static CDivideHeavyHydro DivideHeavyHydro(this IList<Tinker.Xyz.Atom> atoms, Tinker.Prm prm)
        {
            Tinker.Prm.Atom[] prmatoms = prm.atoms.SelectByXyzAtom(atoms).ToArray();
            HDebug.Assert(atoms.Count == prmatoms.Length);
            List<Tuple<int,Tinker.Xyz.Atom,Tinker.Prm.Atom>> lstHydrogenIdxAtmPrm = new List<Tuple<int, Tinker.Xyz.Atom, Tinker.Prm.Atom>>();
            List<Tuple<int,Tinker.Xyz.Atom,Tinker.Prm.Atom>> lstHeavyIdxAtmPrm    = new List<Tuple<int, Tinker.Xyz.Atom, Tinker.Prm.Atom>>();
            for(int idx=0; idx<atoms.Count; idx++)
            {
                var prmatom = prmatoms[idx];
                var xyzatom = atoms[idx];
                HDebug.Assert(xyzatom.AtomId == prmatom.Id);
                if(prmatom.IsHydrogen)
                {
                    lstHydrogenIdxAtmPrm.Add(new Tuple<int, Tinker.Xyz.Atom, Tinker.Prm.Atom>(idx, xyzatom, prmatom));
                }
                else
                {
                    lstHeavyIdxAtmPrm.Add(new Tuple<int, Tinker.Xyz.Atom, Tinker.Prm.Atom>(idx, xyzatom, prmatom));
                }
            }
            HDebug.Assert(Enumerable.Union(lstHydrogenIdxAtmPrm, lstHeavyIdxAtmPrm).Count() == atoms.Count);
            return new CDivideHeavyHydro
            {
                lstHydrogenIdxAtmPrm = lstHydrogenIdxAtmPrm,
                lstHeavyIdxAtmPrm    = lstHeavyIdxAtmPrm   
            };
        }
        public static KDTree.KDTree<Tinker.Xyz.Atom> HToKDTree(this IList<Tinker.Xyz.Atom> atoms)
        {
            KDTree.KDTree<Tinker.Xyz.Atom> kdtree = new KDTree.KDTree<Tinker.Xyz.Atom>(3);
            foreach(var atom in atoms)
                kdtree.insert(atom.Coord, atom);
            return kdtree;
        }
        public static IEnumerable<string> EnumLine(this IEnumerable<Element> elems)
        {
            foreach(var elem in elems)
                yield return elem.line;
        }
        public static IEnumerable<Tinker.Xyz.Atom> HSelectByAtomType(this IEnumerable<Tinker.Xyz.Atom> atoms, string AtomType)
        {
            foreach(var atom in atoms)
            {
                if(atom.AtomType == AtomType)
                    yield return atom;
            }
        }
        public static KDTreeDLL.KDTree<Tinker.Xyz.Atom> HToKDTreeByCoord(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            KDTreeDLL.KDTree<Tinker.Xyz.Atom> kdtree = new KDTreeDLL.KDTree<Tinker.Xyz.Atom>(3);
            foreach(var atom in atoms)
                kdtree.insert(atom.Coord, atom);
            return kdtree;
        }
    }
}
