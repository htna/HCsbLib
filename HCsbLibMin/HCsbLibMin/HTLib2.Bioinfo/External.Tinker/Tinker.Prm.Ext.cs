using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Prm = Tinker.Prm;
    public static partial class TinkerStatic
    {
        public static IEnumerable<int> HEnumClass(this IEnumerable<Tinker.Prm.Vdw> vdws)
        {
            foreach(var vdw in vdws)
                yield return vdw.Class;
        }
        public static IEnumerable<double> HEnumRmin2(this IEnumerable<Tinker.Prm.Vdw> vdws)
        {
            foreach(var vdw in vdws)
                yield return vdw.Rmin2;
        }
        public static IEnumerable<double> HEnumEpsilon(this IEnumerable<Tinker.Prm.Vdw> vdws)
        {
            foreach(var vdw in vdws)
                yield return vdw.Epsilon;
        }
        public static Tinker.Prm.Atom SelectByXyzAtom(this IList<Tinker.Prm.Atom> prmatoms, Tinker.Xyz.Atom xyzatom, HPack<Dictionary<int, Tinker.Prm.Atom>> cache=null)
        {
            foreach(var prmatom in prmatoms)
                if(xyzatom.AtomId == prmatom.Id)
                    return prmatom;
            return null;
        }
        public static IList<Tinker.Prm.Atom> SelectByXyzAtom(this IList<Tinker.Prm.Atom> prmatoms, IList<Tinker.Xyz.Atom> xyzatoms)
        {
            Dictionary<int, Tinker.Prm.Atom> prm_id2atom = prmatoms.ToIdDictionary();
            Prm.Atom[] seles = new Prm.Atom[xyzatoms.Count];
            for(int i=0; i<seles.Length; i++)
            {
                var xyzatom = xyzatoms[i];
                seles[i] = prm_id2atom[xyzatom.AtomId];
            }
            return seles;
        }

        public static double[] ListMass(this IList<Tinker.Prm.Atom> prmatoms)
        {
            int size = prmatoms.Count;
            double[] masses = new double[size];
            for(int i=0; i<size; i++)
                masses[i] = prmatoms[i].Mass;
            return masses;
        }

        public static int FindId(this IList<Tinker.Prm.Biotype> biotypes, string atomname, string resiname)
        {
            string name0 = atomname.ToUpper();
            string resn0 = resiname.ToUpper();
            if(name0[0] == 'H' && '0' <= name0.Last() && name0.Last() <= '9')
                name0 = name0.Substring(0, name0.Length-1);

            List<Tinker.Prm.Biotype> matches = new List<Tinker.Prm.Biotype>();
            foreach(var biotype in biotypes)
            {
                string name1 = biotype.Name.ToUpper();
                string resn1 = biotype.Resn.ToUpper();
                if(resn0.StartsWith(resn1) == false && resn1.StartsWith(resn0) == false) continue;
                if(name0 != name1) continue;
                if(biotype.Id == -1) continue;
                matches.Add(biotype);
            }
            HDebug.Assert(matches.Count > 0);
            if(matches.Count == 1)
                return matches[0].Id;
            HDebug.Assert(false);
            return -1;
        }
        public static Dictionary<int,Tinker.Prm.Atom> ToIdDictionary(this IList<Tinker.Prm.Atom> atoms)
        {
            Dictionary<int, Tinker.Prm.Atom> dict = new Dictionary<int, Tinker.Prm.Atom>();
            foreach(var atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        public static Dictionary<int, Tinker.Prm.Charge> ToIdDictionary(this IList<Tinker.Prm.Charge> charges)
        {
            Dictionary<int, Tinker.Prm.Charge> dict = new Dictionary<int, Tinker.Prm.Charge>();
            foreach(var charge in charges)
                dict.Add(charge.Id, charge);
            return dict;
        }
        public static Dictionary<int, Tinker.Prm.Vdw> ToClassDictionary(this IList<Tinker.Prm.Vdw> vdws)
        {
            Dictionary<int, Tinker.Prm.Vdw> dict = new Dictionary<int, Tinker.Prm.Vdw>();
            foreach(var vdw in vdws)
                dict.Add(vdw.Class, vdw);
            return dict;
        }
        public static Dictionary<int, Tinker.Prm.Biotype> ToIdDictionary(this IList<Tinker.Prm.Biotype> biotypes)
        {
            Dictionary<int, Tinker.Prm.Biotype> dict = new Dictionary<int, Tinker.Prm.Biotype>();
            foreach(var biotype in biotypes)
                dict.Add(biotype.Id, biotype);
            return dict;
        }
        public static Dictionary<int, Tinker.Prm.Vdw14> ToClassDictionary(this IList<Tinker.Prm.Vdw14> vdw14s)
        {
            Dictionary<int, Tinker.Prm.Vdw14> dict = new Dictionary<int, Tinker.Prm.Vdw14>();
            foreach(var vdw14 in vdw14s)
                dict.Add(vdw14.Class, vdw14);
            return dict;
        }
        public static Dictionary<Tuple<int,int>,Prm.Bond> ToClassDictionary(this IList<Tinker.Prm.Bond> bonds)
        {
            Dictionary<Tuple<int, int>, Prm.Bond> dict = new Dictionary<Tuple<int, int>, Prm.Bond>();
            foreach(var bond in bonds)
            {
                try   { dict.Add(new Tuple<int,int>(bond.Class1,bond.Class2), bond); }
                catch {                                                              }
            }
            return dict;
        }
        public static Dictionary<Tuple<int, int, int>, Prm.Angle> ToClassDictionary(this IList<Tinker.Prm.Angle> angles)
        {
            Dictionary<Tuple<int, int, int>, Prm.Angle> dict = new Dictionary<Tuple<int, int, int>, Prm.Angle>();
            foreach(var angle in angles)
                dict.Add(new Tuple<int, int, int>(angle.Class1, angle.Class2, angle.Class3), angle);
            return dict;
        }
        public static Dictionary<Tuple<int, int, int>, Prm.Ureybrad> ToClassDictionary(this IList<Tinker.Prm.Ureybrad> ureybrads)
        {
            Dictionary<Tuple<int, int, int>, Prm.Ureybrad> dict = new Dictionary<Tuple<int, int, int>, Prm.Ureybrad>();
            foreach(var ureybrad in ureybrads)
                dict.Add(new Tuple<int, int, int>(ureybrad.Class1, ureybrad.Class2, ureybrad.Class3), ureybrad);
            return dict;
        }
        public static Dictionary<Tuple<int, int, int, int>, Prm.Improper> ToClassDictionary(this IList<Tinker.Prm.Improper> impropers)
        {
            Dictionary<Tuple<int, int, int, int>, Prm.Improper> dict = new Dictionary<Tuple<int, int, int, int>, Prm.Improper>();
            foreach(var improper in impropers)
                dict.Add(new Tuple<int, int, int, int>(improper.Class1, improper.Class2, improper.Class3, improper.Class4), improper);
            return dict;
        }
        public static Dictionary<Tuple<int, int, int, int>, Prm.Torsion> ToClassDictionary(this IList<Tinker.Prm.Torsion> torsions)
        {
            Dictionary<Tuple<int, int, int, int>, Prm.Torsion> dict = new Dictionary<Tuple<int, int, int, int>, Prm.Torsion>();
            foreach(var torsion in torsions)
            {
                Tuple<int, int, int, int> cls = new Tuple<int, int, int, int>(torsion.Class1, torsion.Class2, torsion.Class3, torsion.Class4);
                if(dict.ContainsKey(cls))
                {
                    //Debug.Assert(false);
                    continue;
                }
                dict.Add(cls, torsion);
            }
            return dict;
        }

        public static Prm CloneByUpdateSpring(this Prm prm, Func<Tinker.Prm.Element, int, double?> GetSpring)
        {
            string[] nlines = new string[prm.elements.Length];
            for(int i=0; i<nlines.Length; i++)
            {
                var elem = prm.elements[i];
                var type = elem.GetType().Name;
                switch(type)
                {
                    case "Element" : break;
                    case "Atom"    : break;
                    case "Vdw"     : break;
                    case "Vdw14"   : break;
                    case "Bond"    : { var val = elem as Prm.Bond    ; elem = Prm.Bond    .FromData(val.Class1, val.Class2,                         GetSpring(val, 0).Value, val.b0    ); } break;
                    case "Angle"   : { var val = elem as Prm.Angle   ; elem = Prm.Angle   .FromData(val.Class1, val.Class2, val.Class3,             GetSpring(val, 0).Value, val.Theta0); } break;
                    case "Ureybrad": { var val = elem as Prm.Ureybrad; elem = Prm.Ureybrad.FromData(val.Class1, val.Class2, val.Class3,             GetSpring(val, 0).Value, val.S0    ); } break;
                    case "Improper": { var val = elem as Prm.Improper; elem = Prm.Improper.FromData(val.Class1, val.Class2, val.Class3, val.Class4, GetSpring(val, 0).Value, val.psi0  ); } break;
                    case "Torsion" : { var val = elem as Prm.Torsion ; elem = Prm.Torsion .FromData(val.Class1, val.Class2, val.Class3, val.Class4
                                                                                                   ,GetSpring(val, 0), val.delta0, val.n0
                                                                                                   ,GetSpring(val, 1), val.delta1, val.n1
                                                                                                   ,GetSpring(val, 2), val.delta2, val.n2
                                                                                                   ); } break;
                    case "Charge" : break;
                    case "Biotype": break;

                    default: throw new NotImplementedException();
                }
                nlines[i] = elem.line;
            }

            return Prm.FromLines(nlines);
        }
    }
}
