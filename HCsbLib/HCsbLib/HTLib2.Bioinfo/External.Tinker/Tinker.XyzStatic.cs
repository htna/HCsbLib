﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<int, Tinker.Xyz.Atom> ToIdDictionary(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            Dictionary<int, Tinker.Xyz.Atom> dict = new Dictionary<int, Tinker.Xyz.Atom>();
            foreach(Tinker.Xyz.Atom atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tinker.Xyz.Atom[] HSelectCorrectAtomType(this IList<Tinker.Xyz.Atom> atoms)
        {
            List<Tinker.Xyz.Atom> sels = new List<Tinker.Xyz.Atom>();
            foreach(var atom in atoms)
                if(atom.AtomType.Trim().Length != 0)
                    sels.Add(atom);
            return sels.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector[] HListCoords(this IList<Tinker.Xyz.Atom> atoms)
        {
            Vector[] coords = new Vector[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                coords[i] = atoms[i].Coord;
            return coords;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<int> HListId(this IList<Tinker.Xyz.Atom> atoms)
        {
            int[] ids = new int[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                ids[i] = atoms[i].Id;
            return ids;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> HEnumId(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            foreach(var atom in atoms)
                yield return atom.Id;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<int, Tinker.Xyz.Atom> HToDictionaryIdAtom
            ( this IEnumerable<Tinker.Xyz.Atom> atoms
            )
        {
            Dictionary<int, Tinker.Xyz.Atom> dict = new Dictionary<int, Tinker.Xyz.Atom>();
            foreach(var atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<int, int> HToDictionaryIdIndex
            ( this IList<Tinker.Xyz.Atom> atoms
            )
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for(int idx=0; idx<atoms.Count(); idx++)
            {
                int id = atoms[idx].Id;
                dict.Add(id, idx);
            }
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HIsHydrogen(this Tinker.Xyz.Atom atom, Tinker.Prm prm)
        {
            return HIsHydrogen(atom, prm.atoms);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HIsHydrogen(this Tinker.Xyz.Atom atom, Tinker.Prm.Atom[] prm_atoms)
        {
            Dictionary<int,Tinker.Prm.Atom> prm_id2atom = prm_atoms.ToIdDictionary();
            return HIsHydrogen(atom, prm_id2atom);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HIsHydrogen(this Tinker.Xyz.Atom atom, Dictionary<int, Tinker.Prm.Atom> prm_id2atom)
        {
            Tinker.Prm.Atom prm_atom = prm_id2atom[atom.AtomId];
            return prm_atom.IsHydrogen;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KDTree.KDTree<Tinker.Xyz.Atom> HToKDTree(this IList<Tinker.Xyz.Atom> atoms)
        {
            KDTree.KDTree<Tinker.Xyz.Atom> kdtree = new KDTree.KDTree<Tinker.Xyz.Atom>(3);
            foreach(var atom in atoms)
                kdtree.insert(atom.Coord, atom);
            return kdtree;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> EnumLine(this IEnumerable<Element> elems)
        {
            foreach(var elem in elems)
                yield return elem.line;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Tinker.Xyz.Atom> HSelectByAtomType(this IEnumerable<Tinker.Xyz.Atom> atoms, string AtomType)
        {
            foreach(var atom in atoms)
            {
                if(atom.AtomType == AtomType)
                    yield return atom;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KDTreeDLL.KDTree<Tinker.Xyz.Atom> HToKDTreeByCoord(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            KDTreeDLL.KDTree<Tinker.Xyz.Atom> kdtree = new KDTreeDLL.KDTree<Tinker.Xyz.Atom>(3);
            foreach(var atom in atoms)
                kdtree.insert(atom.Coord, atom);
            return kdtree;
        }
    }
}
