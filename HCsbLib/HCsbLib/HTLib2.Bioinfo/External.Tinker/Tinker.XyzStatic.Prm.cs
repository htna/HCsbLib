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
        // Prm.Vdw
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

        // Prm.Vdw14
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

        // Prm.Charge
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

        // Prm.Atom
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

        // Prm.Atom.Mass
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
    }
}
