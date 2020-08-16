using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Xyz     = Tinker.Xyz;
    using Prm     = Tinker.Prm;
    using HOH     = TinkerStatic.SockInSolvLayer_HOH;
    public static partial class TinkerStatic
    {
        public static Xyz Merge( (Xyz xyz1, Xyz xyz2) xyzs, Xyz.Atom.Format format=null)
        {
            return null;
        }
        public static Xyz Merge(IEnumerable<Xyz.Atom> atoms1, IEnumerable<Xyz.Atom> atoms2, Xyz.Atom.Format format=null)
        {
            List<Xyz.Atom> natoms1;
            {
                HDebug.Assert(atoms1.HEnumId().HToHashSet().Count == atoms1.Count());

                List<Tuple<int, int>> idsFromTo = new List<Tuple<int, int>>();
                int nid = 1;
                foreach(int id in atoms1.HEnumId().HEnumSorted())
                {
                    idsFromTo.Add(new Tuple<int, int>(id, nid));
                    nid ++;
                }

                natoms1 = Xyz.CloneByReindex(atoms1, idsFromTo, format);
            }

            List<Xyz.Atom> natoms2;
            {
                List<Tuple<int, int>> idsFromTo = new List<Tuple<int, int>>();
                int nid = natoms1.HEnumId().Max() + 1;
                foreach(int id in atoms2.HEnumId().HEnumSorted())
                {
                    idsFromTo.Add(new Tuple<int, int>(id, nid));
                    nid ++;
                }

                natoms2 = Xyz.CloneByReindex(atoms2, idsFromTo, format);
            }

            Xyz.Header header;
            {
                int maxid = natoms2.Last().Id;
                HDebug.Assert(maxid >= natoms1.HEnumId().Max());
                HDebug.Assert(maxid >= natoms2.HEnumId().Max());

                //if(HDebug.IsDebuggerAttached)
                {
                    int maxidsize = 0;
                    int lmaxid = maxid;
                    while(lmaxid > 0)
                    {
                        lmaxid /= 10;
                        maxidsize ++;
                    }
                    if(maxidsize > format.IdSize)
                        throw new Exception();
                }

                header = Xyz.Header.FromData(format, maxid);
            }

            List<Tinker.TkFile.Element> nelements;
            {
                nelements = new List<Tinker.TkFile.Element>();
                nelements.Add(header);
                nelements.AddRange(natoms1);
                nelements.AddRange(natoms2);
            }

            return new Xyz { elements = nelements.ToArray() };
        }
        public static Xyz SockInSolvBox(Xyz prot, Xyz solvbox, Xyz.Atom.Format format=null)
        {
            return null;
        }
        internal class SockInSolvLayer_HOH
        {
            public Xyz.Atom O;
            public Xyz.Atom H1;
            public Xyz.Atom H2;
        }
        internal static KDTree.KDTree<HOH> HToKdTree(this IEnumerable<HOH> hohs)
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
        internal static List<HOH> HToListHOH(this Xyz solvs)
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
                HDebug.Assert(false);
            }

            foreach(int id in id_atom.Keys)
                HDebug.Assert(id_atom[id] == null);

            return list;
        }
        public static Xyz SockInSolvLayer(Xyz prot, Xyz solvbox, double thickSolvLayer, Prm prm, Xyz.Atom.Format format=null)
        {
            Xyz.Atom[] prot_atoms = prot.atoms;
            KDTree.KDTree<Xyz.Atom> prot_kdtree = prot_atoms.HToKDTree();

            Dictionary<int,Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
            Dictionary<int,Prm.Vdw > prm_cls2vdw = prm.vdws .ToClassDictionary();

            List<HOH> hohs = solvbox.HToListHOH();
            int cntOutSolvLayer = 0;
            int cntCrash        = 0;
            int cntSelected     = 0;
            for(int i=0; i<hohs.Count; i++)
            {
                HOH hoh = hohs[i];

                Xyz.Atom closeO = prot_kdtree.nearest(hoh.O.Coord);
                double distO = (closeO.Coord, hoh.O.Coord).Dist();

                if(distO > thickSolvLayer)
                {
                    cntOutSolvLayer ++;
                    hohs[i] = null;
                    continue;
                }
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
            public Xyz SockInSolvBox(Xyz solvbox, Atom.Format format=null)
            {
                return null;
            }
            public Xyz SockInSolvLayer(Xyz solvbox, double thickSolvLayer, Prm prm, Atom.Format format=null)
            {
                return TinkerStatic.SockInSolvLayer(this, solvbox, thickSolvLayer, prm, format);
            }
        }
    }
}
