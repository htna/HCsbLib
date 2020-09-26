using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Xyz     = Tinker.Xyz;
    using Prm     = Tinker.Prm;
    using HOH     = TinkerStatic.TinkerXyzAtom_HOH;
    public static partial class TinkerStatic
    {
        public class TinkerXyzAtom_HOH
        {
            public Xyz.Atom O;
            public Xyz.Atom H1;
            public Xyz.Atom H2;
        }
        public static KDTree.KDTree<HOH> HToKdTree(this IEnumerable<HOH> hohs)
        {
            KDTree.KDTree<HOH> kdtree = new KDTree.KDTree<HOH>(3);
            foreach(var hoh in hohs)
            {
                if(hoh == null)
                    continue;
                kdtree.insert(hoh.O.Coord, hoh);
            }
            return kdtree;
        }
        public static List<HOH> HSelectListHOH
            ( this Xyz solvs
            , bool assertHasNonHOH = true
            )
        {
            Dictionary<int, Xyz.Atom> id_atom = solvs.atoms.HToDictionaryIdAtom();
            List<HOH> list = new List<HOH>();
            foreach(var id in id_atom.Keys.ToArray())
            {
                Xyz.Atom atom = id_atom[id];
                if(atom == null)
                    continue;
                if(atom.AtomType == "HT ")
                    continue;
                if(atom.AtomType == "OT ")
                {
                    Xyz.Atom O = atom;
                    HDebug.Assert(atom.BondedIds.Length == 2);
                    int H1_id = atom.BondedId1.Value;
                    int H2_id = atom.BondedId2.Value;
                    Xyz.Atom H1 = id_atom[H1_id]; HDebug.Assert(H1.AtomType == "HT ");
                    Xyz.Atom H2 = id_atom[H2_id]; HDebug.Assert(H2.AtomType == "HT ");
                    HOH hoh = new HOH
                    {
                        O = O,
                        H1 = H1,
                        H2 = H2,
                    };
                    list.Add(hoh);

                    id_atom[id] = null;
                    id_atom[H1_id] = null;
                    id_atom[H2_id] = null;
                    continue;
                }
                if(assertHasNonHOH)
                    HDebug.Assert(false);
            }

            foreach(int id in id_atom.Keys)
                HDebug.Assert(id_atom[id] == null);

            return list;
        }
        public static Xyz SockInSolv(Xyz prot, Xyz solvbox, double? thickSolvLayer, Prm prm, Xyz.Atom.Format format)
        {
            Xyz.Atom[] prot_atoms = prot.atoms;
            KDTree.KDTree<Xyz.Atom> prot_kdtree = prot_atoms.HToKDTree();

            Dictionary<int,Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
            Dictionary<int,Prm.Vdw > prm_cls2vdw = prm.vdws .ToClassDictionary();

            List<HOH> hohs = solvbox.HSelectListHOH();
            int cntOutSolvLayer = 0;
            int cntCrash        = 0;
            int cntSelected     = 0;
            for(int i=0; i<hohs.Count; i++)
            {
                HOH hoh = hohs[i];

                Xyz.Atom closeO = prot_kdtree.nearest(hoh.O.Coord);
                double distO = (closeO.Coord, hoh.O.Coord).Dist();

                // check thickness
                if(thickSolvLayer != null)
                {
                    if(distO > thickSolvLayer)
                    {
                        cntOutSolvLayer ++;
                        hohs[i] = null;
                        continue;
                    }
                }

                // check collision
                Xyz.Atom closeH1 = prot_kdtree.nearest(hoh.H1.Coord);
                Xyz.Atom closeH2 = prot_kdtree.nearest(hoh.H2.Coord);
                double distH1 = (closeH1.Coord, hoh.H1.Coord).Dist();
                double distH2 = (closeH2.Coord, hoh.H2.Coord).Dist();
                
                double vdwCloseO  = hoh.O .GetPrmVdw(prm_id2atom, prm_cls2vdw).Rmin2 + closeO .GetPrmVdw(prm_id2atom, prm_cls2vdw).Rmin2;
                double vdwCloseH1 = hoh.H1.GetPrmVdw(prm_id2atom, prm_cls2vdw).Rmin2 + closeH1.GetPrmVdw(prm_id2atom, prm_cls2vdw).Rmin2;
                double vdwCloseH2 = hoh.H2.GetPrmVdw(prm_id2atom, prm_cls2vdw).Rmin2 + closeH2.GetPrmVdw(prm_id2atom, prm_cls2vdw).Rmin2;

                bool crashO  = (distO  < vdwCloseO *0.9);
                bool crashH1 = (distH1 < vdwCloseH1*0.9);
                bool crashH2 = (distH2 < vdwCloseH2*0.9);

                if(crashO || crashH1 || crashH2)
                {
                    //  if( crashO &&  crashH1 &&  crashH2) { }
                    //  if( crashO &&  crashH1 && !crashH2) { }
                    //  if( crashO && !crashH1 &&  crashH2) { }
                    //  if( crashO && !crashH1 && !crashH2) { }
                    //  if(!crashO &&  crashH1 &&  crashH2) { }
                    //  if(!crashO &&  crashH1 && !crashH2) { }
                    //  if(!crashO && !crashH1 &&  crashH2) { }
                    //  if(!crashO && !crashH1 && !crashH2) { }
                    cntCrash ++;
                    hohs[i] = null;
                    continue;
                }

                cntSelected ++;
            }
            //hohs = hohs.HEnumNonNull().ToList();

            List<Xyz.Atom> solv_atoms = new List<Xyz.Atom>(cntSelected*3);
            foreach(HOH hoh in hohs)
            {
                if(hoh == null)
                    continue;
                solv_atoms.Add(hoh.O );
                solv_atoms.Add(hoh.H1);
                solv_atoms.Add(hoh.H2);
            }
            HDebug.Assert(solv_atoms.Count == cntSelected*3);

            Xyz prot_solv = Merge(prot_atoms,solv_atoms, format);

            return prot_solv;
        }
    }
    public partial class Tinker
    {
        public partial class Xyz
        {
            public Xyz SockInSolv(Xyz solvbox, double? thickSolvLayer, Prm prm, Atom.Format format)
            {
                return TinkerStatic.SockInSolv(this, solvbox, thickSolvLayer, prm, format);
            }
        }
    }
}
