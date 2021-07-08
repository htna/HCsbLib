using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    public static partial class TinkerStatic
    {
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
        public static IEnumerable<Element> HEnumElement(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            foreach(var atom in atoms)
                yield return atom.elem;
        }
        public static IEnumerable<Element> HEnumElement(this IEnumerable<Tinker.Xyz.Header> headers)
        {
            foreach(var header in headers)
                yield return header.elem;
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
        public static IEnumerable<string> EnumLine(this IEnumerable<Tinker.Xyz.Atom> atoms)
        {
            foreach(var atom in atoms)
                yield return atom.line;
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
    public partial class Tinker
    {
        //[Serializable]
        public partial class Xyz : IBinarySerializable
        {
            public Element[] elements;
            public Atom[]    atoms { get { return elements.HSelectByType<Element,Atom>().ToArray(); } }
            public Atom.Format    atoms_format
            {
                get
                {
                    Atom.Format format = elements[0].format;
                    if(HDebug.IsDebuggerAttached)
                    {
                        foreach(var element in elements)
                            HDebug.Assert(format == element.format);
                    }
                    return format;
                }
            }
            public Xyz(Element[] elements)
            {
                this.elements = elements;
            }
            ///////////////////////////////////////////////////
            // IBinarySerializable
            public void BinarySerialize(HBinaryWriter writer)
            {
                writer.Write(atoms_format);
                string[] lines = elements.EnumLine().ToArray();
                writer.Write(lines);
            }
            public Xyz(HBinaryReader reader)
            {
                Atom.Format format; reader.Read(out format);
                string[]    lines;  reader.Read(out lines );
                
                elements = GetElementsFromLines(format, lines);
            }
            /// 186  GNOMES, ROCK MONSTERS AND CHILI SAUCE
            ///   1  NH3  -11.020000  -12.540000  -24.210000    65     2     5     6     7
            ///   2  CT1  -12.460000  -12.650000  -24.060000    24     1     3     8     9
            ///   3  C    -12.800000  -14.020000  -23.500000    20     2     4    25
            public static Xyz FromFile(string path, bool loadLatest)
            {
                Atom.Format format = new Atom.Format();
                return FromFile(path, loadLatest, format);
            }
            public static Atom.Format DetermineFormat(string path, bool loadLatest)
            {
                string[] lines = TkFile.LinesFromFile(path, loadLatest);
                return DetermineFormat(lines);
            }
            public static Atom.Format DetermineFormat(string[] lines)
            {
                string line0 = lines[0];
                string line1 = lines[1];

                int[] idxId = new int[] { 0, line0.Length-1 };
                int   idxId_length = idxId[1] - idxId[0] + 1;

                int[] idxAtomType  = null;
                {
                    for(int i=0; i<line1.Length; i++)
                    {
                        char ch = line1[i];
                        if((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
                        {
                            idxAtomType = new int[2] { i, i+2 };
                            break;
                        }
                    }
                    HDebug.Exception(idxAtomType != null);
                }

                int[] idxX;
                int   nDecimalPlaces = -1;
                {
                    int idxX0 = idxAtomType[1]+1;
                    int i = idxX0;
                    int idxDecimalPoint = -1;
                    while(line1[i] == ' ') i++;
                    while(line1[i] != ' ')
                    {
                        if(line1[i] == '.')
                            idxDecimalPoint = i;
                        i++;
                    }
                    int idxX1 = i-1;
                    idxX = new int[] { idxX0, idxX1 };
                    nDecimalPlaces = idxX1 - idxDecimalPoint;
                }
                int idxX_length = idxX[1] - idxX[0] + 1;

                int[] idxY        = new int[2]; idxY       [0] = idxX     [1] + 1; idxY       [1] = idxY       [0] + idxX_length  - 1;
                int[] idxZ        = new int[2]; idxZ       [0] = idxY     [1] + 1; idxZ       [1] = idxZ       [0] + idxX_length  - 1;
                int[] idxAtomId   = new int[2]; idxAtomId  [0] = idxZ     [1] + 1; idxAtomId  [1] = idxAtomId  [0] + 5;
                int[] idxBondedId = new int[2]; idxBondedId[0] = idxAtomId[1] + 1; idxBondedId[1] = idxBondedId[0] + idxId_length - 1;

                string formatId       = "                                            {0}";  // HSubEndStringCount
                string formatAtomType = "{0}                                            ";  // Substring
              //string formatX        = "                               {0:0.0000000000}";  // HSubEndStringCount
              //string formatY        = "                               {0:0.0000000000}";  // HSubEndStringCount
              //string formatZ        = "                               {0:0.0000000000}";  // HSubEndStringCount
                string formatXYZ      = "                               {0:0."; for(int i=0; i<nDecimalPlaces; i++) formatXYZ += "0"; formatXYZ+="}";
                string formatAtomId   = "                                            {0}";  // HSubEndStringCount
                string formatBondedId = "                                            {0}";  // HSubEndStringCount

                Atom.Format format = new Atom.Format
                {
                    idxId       = idxId      ,    formatId       = formatId      ,
                    idxAtomType = idxAtomType,    formatAtomType = formatAtomType,
                    idxX        = idxX       ,    formatX        = formatXYZ     ,
                    idxY        = idxY       ,    formatY        = formatXYZ     ,
                    idxZ        = idxZ       ,    formatZ        = formatXYZ     ,
                    idxAtomId   = idxAtomId  ,    formatAtomId   = formatAtomId  ,
                    idxBondedId = idxBondedId,    formatBondedId = formatBondedId,
                };

                return format;
            }
            public static Xyz FromFile(string path, bool loadLatest, Atom.Format format)
            {
                string[] lines = TkFile.LinesFromFile(path, loadLatest);
                if(format == null)
                    format = DetermineFormat(lines);
                return FromLines(format, lines);
            }
            public void ToFile(string path, bool saveAsNext)
            {
                TkFile.ElementsToFile(path, saveAsNext, elements);
            }
            public void ToFile(string path, bool saveAsNext, Atom.Format format, bool autoAdjustCoord=false)
            {
                if(format == null)
                    format = atoms_format;

                List<Element> nelems = new List<Element>();
                foreach(var elem in elements)
                {
                    if(elem.type == Atom.type)
                    {
                        Atom     atom = elem.Atom;
                        Element natom = Atom.ElementFromData(format, atom.Id, atom.AtomType.Trim(), atom.X, atom.Y, atom.Z, atom.AtomId, atom.BondedIds, autoAdjustCoord);
                        nelems.Add(natom);
                    }
                    else if(elem.type == Header.type)
                    {
                        Header   header = elem.Header;
                        Element nheader = Header.ElementFromData(format, header.NumAtoms);
                        //{
                        //    string line = "";
                        //    line += string.Format("                    {0}", header.NumAtoms).HSubEndStringCount(format.idxId[1]-format.idxId[0]+1);
                        //    line += "  ";
                        //    header.line = line;
                        //}
                        nelems.Add(nheader);
                    }
                    else
                    {
                        Element    nelem = new Element(elem.line, elem.type, format);
                        nelems.Add(nelem);
                    }
                }

                TkFile.ElementsToFile(path, saveAsNext, nelems);
            }
            public static Xyz FromAtoms(IList<Xyz.Atom> atoms)
            {
                Element header = Header.ElementFromData(atoms.Count);

                List<string> lines = new List<string>(atoms.Count+1);

                lines.Add(header.line);
                foreach(var atom in atoms)
                    lines.Add(atom.line);

                return FromLines(lines);
            }
            public static Xyz FromLines(IList<string> lines)
            {
                Atom.Format format = new Atom.Format();
                return FromLines(format, lines);
            }
            public static Element[] GetElementsFromLines(Atom.Format format, IList<string> lines)
            {
                if(HDebug.Selftest())
                    #region MyRegion
                {
                    Xyz lxyz = FromLines(Selftest.selftest_xyz);
                    Pdb lpdb = Pdb.FromLines(Selftest.selftest_pdb);
                    lxyz.ListMatch(lpdb.atoms);
                }
                    #endregion
                if(lines == null)
                {
                    HDebug.Assert(false);
                    return null;
                }
                if(lines.Count == 0)
                    return null;

                Element[] elements = new Element[lines.Count];
                elements[0] = Header.ElementFromLine(format, lines[0]);
                for(int i=1; i<lines.Count; i++)
                    elements[i] = Atom.ElementFromLine(format, lines[i]);

                return elements;
            }
            public static Xyz FromLines(Atom.Format format, IList<string> lines)
            {
                Element[] elements = GetElementsFromLines(format, lines);
                Xyz xyz = new Xyz( elements );
                
                return xyz;
            }
            public static Xyz FromCoords(Xyz src, IList<Vector> coords)
            {
                Element[] elements = src.elements.HClone();
                for(int i=0; i<elements.Length; i++)
                {
                    switch(elements[i].type)
                    {
                        case Header.type:
                            HDebug.Assert(elements[0].Header.NumAtoms == coords.Count);
                            break;
                        case Atom.type:
                            Atom atom    = elements[i].Atom;
                            Vector coord = coords[atom.Id-1];
                            Element nelem =  Atom.ElementFromCoord(atom, coord);
                            elements[i] = nelem;
                            break;
                        default:
                            HDebug.Assert(false);
                            break;
                    }
                }

                Xyz xyz = new Xyz( elements );
                return xyz;
            }

            public static Tuple<int, int>[] IdxXyzToPdb(IList<Xyz.Atom> xyzatoms, IList<Pdb.Atom> pdbatoms, double tolDist2=0.00001)
            {
                KDTree.KDTree<int[]> kdtree = new KDTree.KDTree<int[]>(3);
                for(int i=0; i<pdbatoms.Count; i++)
                    kdtree.insert(pdbatoms[i].coord, new int[] { i });
                List<Tuple<int, int>> xyz2pdb = new List<Tuple<int, int>>();
                for(int xyzidx=0; xyzidx<xyzatoms.Count; xyzidx++)
                {
                    Vector xyzcoord = xyzatoms[xyzidx].Coord;
                    int pdbidx = kdtree.nearest(xyzcoord)[0];
                    Vector pdbcoord = pdbatoms[pdbidx].coord;
                    double dist2 = (xyzcoord-pdbcoord).Dist2;
                    if(dist2 < tolDist2)
                        xyz2pdb.Add(new Tuple<int, int>(xyzidx, pdbidx));
                }
                return xyz2pdb.ToArray();
                
                // Vector[] xyzcoords;
                // {
                //     xyzcoords = new Vector[xyzatoms.Count];
                //     for(int i=0; i<xyzatoms.Count; i++) xyzcoords[i] = xyzatoms[i].Coord;
                // }
                // Vector[] pdbcoords;
                // {
                //     pdbcoords = new Vector[pdbatoms.Count];
                //     for(int i=0; i<pdbatoms.Count; i++) pdbcoords[i] = pdbatoms[i].coord;
                // }
                // List<Tuple<int, int>> xyz2pdb = new List<Tuple<int, int>>();
                // for(int xyzidx=0; xyzidx<xyzatoms.Count; xyzidx++)
                // {
                //     double bestdist2 = double.PositiveInfinity;
                //     int bestpdbidx = -1;
                //     for(int j=0; j<pdbatoms.Count; j++)
                //     {
                //         double dist2 = (xyzcoords[xyzidx] - pdbcoords[j]).Dist2;
                //         if(dist2 < bestdist2)
                //         {
                //             bestpdbidx = j;
                //             bestdist2 = dist2;
                //         }
                //     }
                //     //Debug.Assert(bestdist2 < 0.00001);
                //     if(bestdist2 < tolDist2)
                //     {
                //         xyz2pdb.Add(new Tuple<int, int>(xyzidx, bestpdbidx));
                //     }
                // }
                // return xyz2pdb.ToArray();
            }
            public Tuple<Atom,Pdb.Atom>[] ListMatch(IList<Pdb.Atom> pdbatoms)
            {
                Xyz.Atom[] xyzatoms = this.atoms;
                //Pdb.Atom[] pdbatoms = pdb.atoms;
                Tuple<int, int>[] xyz2pdb = IdxXyzToPdb(xyzatoms, pdbatoms);

                Tuple<Atom, Pdb.Atom>[] matches = new Tuple<Atom, Pdb.Atom>[xyz2pdb.Length];
                for(int idx=0; idx<xyz2pdb.Length; idx++)
                {
                    int xyzidx = xyz2pdb[idx].Item1;
                    int pdbidx = xyz2pdb[idx].Item2;
                    Xyz.Atom xyzatm = xyzatoms[xyzidx];
                    Pdb.Atom pdbatm = pdbatoms[pdbidx];

                    string xyzi_name = xyzatm.AtomType.Trim();
                    string pdbj_name = pdbatm.name.Trim();
                    if(pdbj_name[0]>='0' && pdbj_name[0]<='9') HDebug.Assert(xyzi_name[0] == pdbj_name[1]);
                    else HDebug.Assert(xyzi_name[0] == pdbj_name[0]);

                    matches[idx] = new Tuple<Atom, Pdb.Atom>(xyzatm, pdbatm);
                }
                return matches;
            }
            public static bool UpdateUnivCoords(string xyzpath, Universe univ)
            {
                Xyz xyz0 = Xyz.FromFile(xyzpath, false);
                Xyz xyz1 = Xyz.FromFile(xyzpath, true);
                Dictionary<int, Tinker.Xyz.Atom> xyz1_id2atom = xyz1.atoms.ToIdDictionary();

                Tuple<Atom, Pdb.Atom>[] matches = xyz0.ListMatch(univ.pdb.atoms);
                
                Vector[] newcoords = new Vector[univ.size];
                foreach(Tuple<Atom, Pdb.Atom> match in matches)
                {
                    Atom     xyz0atom = match.Item1;
                    Pdb.Atom pdb0atom = match.Item2;

                    int      univid   = univ.pdb.atoms.IndexOfAtom(pdb0atom.name, pdb0atom.resSeq);
                    HDebug.Assert(univid != -1);
                    if(newcoords[univid] != null)
                    {
                        // do not change because there is mis-match
                        HDebug.Assert(false);
                        return false;
                    }

                    int      xyzid    = xyz0atom.Id;
                    Atom     xyz1atom = xyz1_id2atom[xyzid];
                    newcoords[univid] = xyz1atom.Coord;
                }
                for(int i=0; i<newcoords.Length; i++)
                    if(newcoords == null)
                    {
                        // do not change because there is missing match
                        HDebug.Assert(false);
                        return false;
                    }

                univ.SetCoords(newcoords);
                return true;
            }
            //public class Element : Tinker.TkFile.Element
            //{
            //    public readonly Format format;
            //    public Element(Format format, string line) : base(line) { this.format = format; }
            //}
            public struct Header
            {
                public readonly Element elem;
                public string           line   { get { return elem.line  ; } }
                public Atom.Format      format { get { return elem.format; } }
                public Header(Element elem) { this.elem = elem; }
                public const string type = "Header";
                ///  num atoms
                ///  0-5
                /// 
                ///  0        1         2         3         4         5         6         7         8
                ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///  ================================================================================
                ///    3138  HYDROLASE/HYDROLASE INHIBITOR           02-OCT-12   4HDB
                /// "  2521  HEME PROTEIN                            25-FEB-98   1A6G"
                public int NumAtoms { get { return elem.GetInt(elem.format.idxId).Value; } }
                public static Element ElementFromLine(Atom.Format format, string line) {                                                     return new Element(line, Header.type, format); }
                public static Element ElementFromLine(                    string line) { Atom.Format format = Atom.Format.defformat_digit06; return new Element(line, Header.type, format); }
                public static Element ElementFromData(int numatoms, string description="", string date="", string pdbid="")
                {
                    return ElementFromData(Atom.Format.defformat_digit06, numatoms, description, date, pdbid);
                }
                public static Element ElementFromData(Atom.Format format, int numatoms, string description="", string date="", string pdbid="")
                {
                    if(HDebug.Selftest())
                    {
                        var line0 = "  2521  HEME PROTEIN                            25-FEB-98   1A6G";
                        var line1 = ElementFromData(2521, "HEME PROTEIN", "25-FEB-98", "1A6G").line;
                        HDebug.Exception(line0 == line1);
                    }
                    string line = "";
                    line += string.Format("                    {0}", numatoms).HSubEndStringCount(format.idxId[1]-format.idxId[0]+1);
                    line += "  ";
                    if(description != null) line += (description + "                                        ").Substring(0, 40);
                    if(date        != null) line += (date        + "                                        ").Substring(0, 12);
                    if(pdbid       != null) line += (pdbid       + "                                        ").Substring(0,  4);
                    line = line.TrimEnd();
                    Element elem = new Element(line, Header.type, format);
                    HDebug.Assert(numatoms == elem.Header.NumAtoms);
                    return elem;
                }
            }
            //[Serializable]
            public class Atom
            {
                //[Serializable]
                public class Format : IBinarySerializable
                {
                    public int[] idxId       = new int[]{ 0, 5};    public string formatId       = "                     {0}";  // HSubEndStringCount
                    public int[] idxAtomType = new int[]{ 8,10};    public string formatAtomType = "{0}                     ";  // Substring
                    public int[] idxX        = new int[]{11,22};    public string formatX        = "            {0:0.000000}";  // HSubEndStringCount
                    public int[] idxY        = new int[]{23,34};    public string formatY        = "            {0:0.000000}";  // HSubEndStringCount
                    public int[] idxZ        = new int[]{35,46};    public string formatZ        = "            {0:0.000000}";  // HSubEndStringCount
                    public int[] idxAtomId   = new int[]{47,52};    public string formatAtomId   = "                     {0}";  // HSubEndStringCount
                    public int[] idxBondedId = new int[]{53,58};    public string formatBondedId = "                     {0}";  // HSubEndStringCount

                    public int IdSize    { get { return (idxId[1] - idxId[0]); } }
                    public int CoordSize { get { return (idxX [1] - idxX [0]); } }

                    public Format(int IdSize=5, int CoordSize=11, string CoordFormat="{0:0.000000}")
                    {
                        UpdateFormat(IdSize, CoordSize, CoordFormat);
                    }
                    public void UpdateFormat(int IdSize=5, int CoordSize=11, string CoordFormat="{0:0.000000}")
                    {
                        //IdSize    =  5- 0;
                        //CoordSize = 22-11;
                        int i = 0;
                        idxId       = new int[]{ i, i+IdSize    };    i+=3+IdSize   ;    formatId       = "                     {0}";  // HSubEndStringCount
                        idxAtomType = new int[]{ i, i+2         };    i+=3          ;    formatAtomType = "{0}                     ";  // Substring
                        idxX        = new int[]{ i, i+CoordSize };    i+=1+CoordSize;    formatX        = "            "+CoordFormat;  // HSubEndStringCount
                        idxY        = new int[]{ i, i+CoordSize };    i+=1+CoordSize;    formatY        = "            "+CoordFormat;  // HSubEndStringCount
                        idxZ        = new int[]{ i, i+CoordSize };    i+=1+CoordSize;    formatZ        = "            "+CoordFormat;  // HSubEndStringCount
                        idxAtomId   = new int[]{ i, i+5         };    i+=6          ;    formatAtomId   = "                     {0}";  // HSubEndStringCount
                        idxBondedId = new int[]{ i, i+IdSize    };    i+=1+CoordSize;    formatBondedId = "                     {0}";  // HSubEndStringCount
                    }
                    ///////////////////////////////////////////////////
                    // IBinarySerializable
                    public void BinarySerialize(HBinaryWriter writer)
                    {
                        writer.Write(idxId      );
                        writer.Write(idxAtomType);
                        writer.Write(idxX       );
                        writer.Write(idxY       );
                        writer.Write(idxZ       );
                        writer.Write(idxAtomId  );
                        writer.Write(idxBondedId);
                    }
                    public Format(HBinaryReader reader)
                    {
                        reader.Read(out idxId      );
                        reader.Read(out idxAtomType);
                        reader.Read(out idxX       );
                        reader.Read(out idxY       );
                        reader.Read(out idxZ       );
                        reader.Read(out idxAtomId  );
                        reader.Read(out idxBondedId);
                    }
                    // IBinarySerializable
                    ///////////////////////////////////////////////////

                    public static Format defformat_digit06 = new Format
                    {
                        ///  id  (atom type in prm)   x     y      z        (atom-id in prm)  bonds, ...
                        ///  0-5 8-10                11-22  23-34  35-46    47-52             53-58(6), 59-64(6), ...
                        ///   
                        ///  0        1         2         3         4         5         6         7         8
                        ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                        ///  ================================================================================
                        ///       1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7
                        idxId        = new int[]{ 0, 5},
                        idxAtomType  = new int[]{ 8,10},
                        idxX         = new int[]{11,22},
                        idxY         = new int[]{23,34},
                        idxZ         = new int[]{35,46},
                        idxAtomId    = new int[]{47,52},
                        idxBondedId  = new int[]{53,58},

                        formatId       = "                     {0}",  // HSubEndStringCount
                        formatAtomType = "{0}                     ",  // Substring
                        formatX        = "            {0:0.000000}",  // HSubEndStringCount
                        formatY        = "            {0:0.000000}",  // HSubEndStringCount
                        formatZ        = "            {0:0.000000}",  // HSubEndStringCount
                        formatAtomId   = "                     {0}",  // HSubEndStringCount
                        formatBondedId = "                     {0}",  // HSubEndStringCount
                    };

                    public static Format defformat_digit10_id6 = new Format // default
                    {
                        ///  0         1         2         3         4         5         6         7         8         9
                        ///  01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567
                        ///  ==================================================================================================
                        /// "     3  C           -3.3566390770         -2.5341116969        -12.3096330214    20     2     4    20"
                        idxId        = new int[]{ 0-0, 6-1},
                        idxAtomType  = new int[]{ 9-1,11-1},
                        idxX         = new int[]{12-1,33-1},
                        idxY         = new int[]{34-1,55-1},
                        idxZ         = new int[]{56-1,77-1},
                        idxAtomId    = new int[]{78-1,83-1},
                        idxBondedId  = new int[]{84-1,90-2},
                
                        formatId       = "                          {0}",  // HSubEndStringCount
                        formatAtomType = "{0}                          ",  // Substring
                        formatX        = "             {0:0.0000000000}",  // HSubEndStringCount
                        formatY        = "             {0:0.0000000000}",  // HSubEndStringCount
                        formatZ        = "             {0:0.0000000000}",  // HSubEndStringCount
                        formatAtomId   = "                          {0}",  // HSubEndStringCount
                        formatBondedId = "                          {0}",  // HSubEndStringCount
                    };
                    public static Format _defformat_digit10_id7 = new Format // not-default
                    {
                        ///  0         1         2         3         4         5         6         7         8         9
                        ///  01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567
                        ///  ==================================================================================================
                        /// "     13  OT         -85.4401110000        -18.6572660000         -9.9272310000   101     14     15"
                        idxId        = new int[]{ 0, 6},
                        idxAtomType  = new int[]{ 9,11},
                        idxX         = new int[]{12,33},
                        idxY         = new int[]{34,55},
                        idxZ         = new int[]{56,77},
                        idxAtomId    = new int[]{78,83},
                        idxBondedId  = new int[]{84,90},

                        formatId       = "                          {0}",  // HSubEndStringCount
                        formatAtomType = "{0}                          ",  // Substring
                        formatX        = "             {0:0.0000000000}",  // HSubEndStringCount
                        formatY        = "             {0:0.0000000000}",  // HSubEndStringCount
                        formatZ        = "             {0:0.0000000000}",  // HSubEndStringCount
                        formatAtomId   = "                          {0}",  // HSubEndStringCount
                        formatBondedId = "                          {0}",  // HSubEndStringCount
                    };
                    public static Format _defformat_digit2310_id6 = new Format // not-default
                    {
                        ///  0         1         2         3         4         5         6         7         8         9         10        11
                        ///  0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901
                        ///  ==============================================================================================================
                        /// "     1  NH3         -15.5539038615           0.5823770912           8.8675305868    65     2     5     6     7"
                        idxId        = new int[]{ 0, 5},
                        idxAtomType  = new int[]{ 8,10},
                        idxX         = new int[]{11,33},
                        idxY         = new int[]{34,56},
                        idxZ         = new int[]{57,79},
                        idxAtomId    = new int[]{80,85},
                        idxBondedId  = new int[]{86,91},

                        formatId       = "                           {0}",  // HSubEndStringCount
                        formatAtomType = "{0}                           ",  // Substring
                        formatX        = "              {0:0.0000000000}",  // HSubEndStringCount
                        formatY        = "              {0:0.0000000000}",  // HSubEndStringCount
                        formatZ        = "              {0:0.0000000000}",  // HSubEndStringCount
                        formatAtomId   = "                           {0}",  // HSubEndStringCount
                        formatBondedId = "                           {0}",  // HSubEndStringCount
                    };
                }

                public readonly Element elem;
                public string           line   { get { return elem.line  ; } }
                public Format           format { get { return elem.format; } }
                public Atom(Element elem) { this.elem = elem; }
                //public readonly Format format;
                static void CheckFormat(Format format, string line)
                {
                    for(int i=1+format.idxId      [1]; i<format.idxAtomType[0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxAtomType[1]; i<format.idxX       [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxX       [1]; i<format.idxY       [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxY       [1]; i<format.idxZ       [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxZ       [1]; i<format.idxAtomId  [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxAtomId  [1]; i<format.idxBondedId[0]; i++) HDebug.Assert(line[i] == ' ');
                }
                public const string type = "Atom";
                ///  id  (atom type in prm)   x     y      z        (atom-id in prm)  bonds, ...
                ///  0-5 8-10                11-22  23-34  35-46    47-52             53-58(6), 59-64(6), ...
                ///   
                ///  0        1         2         3         4         5         6         7         8
                ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///  ================================================================================
                ///       1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7

                public int    Id         { get { return elem.GetInt   (elem.format.idxId       ).Value; } }
                public string AtomType   { get { return elem.GetString(elem.format.idxAtomType )      ; } }
                public double X          { get { return elem.GetDouble(elem.format.idxX        ).Value; } }
                public double Y          { get { return elem.GetDouble(elem.format.idxY        ).Value; } }
                public double Z          { get { return elem.GetDouble(elem.format.idxZ        ).Value; } }
                public int    AtomId     { get { return elem.GetInt   (elem.format.idxAtomId   ).Value; } }
                public int?   BondedId1  { get { return GetBondedId(1); } }
                public int?   BondedId2  { get { return GetBondedId(2); } }
                public int?   BondedId3  { get { return GetBondedId(3); } }
                public int?   BondedId4  { get { return GetBondedId(4); } }
                public int?   BondedId5  { get { return GetBondedId(5); } }
                public int?   BondedId6  { get { return GetBondedId(6); } }
                public int?   BondedId7  { get { return GetBondedId(7); } }
                public int?   BondedId8  { get { return GetBondedId(8); } }
                public int?   BondedId9  { get { return GetBondedId(9); } }

                static int[] GetIdxBondedId(Format format, int idx)
                {
                    int idx0 = format.idxBondedId[0];
                    int idx1 = format.idxBondedId[1];
                    int gap  = idx1 - idx0 + 1;
                    HDebug.Assert(idx >= 0);
                    return new int[] { idx0 + gap * idx, idx1 + gap * idx };
                }
                int? GetBondedId(int idx)
                {
                    //return GetInt(GetIdxBondedId(idx));
                    switch(idx)
                    {
                        case 1: return elem.GetInt(GetIdxBondedId(elem.format,  0));
                        case 2: return elem.GetInt(GetIdxBondedId(elem.format,  1));
                        case 3: return elem.GetInt(GetIdxBondedId(elem.format,  2));
                        case 4: return elem.GetInt(GetIdxBondedId(elem.format,  3));
                        case 5: return elem.GetInt(GetIdxBondedId(elem.format,  4));
                        case 6: return elem.GetInt(GetIdxBondedId(elem.format,  5));
                        case 7: return elem.GetInt(GetIdxBondedId(elem.format,  6));
                        case 8: return elem.GetInt(GetIdxBondedId(elem.format,  7));
                        case 9: return elem.GetInt(GetIdxBondedId(elem.format,  8));
                    }
                    throw new NotImplementedException();
                }

                public Vector Coord    { get { return new double[3]{ X, Y, Z }; } }
                public void GetBondedIds(ref List<int> bondeds)
                {
                    {
                        bondeds.Clear();
                        for(int idx=1; idx<20; idx++)
                        {
                            int? bonded = GetBondedId(idx);
                            if(bonded == null)
                                break;
                            bondeds.Add(bonded.Value);
                        }
                    }
                }
                public int[]  BondedIds
                {
                    get
                    {
                        List<int> bondeds = new List<int>();
                        GetBondedIds(ref bondeds);
                        return bondeds.ToArray();
                    }
                }

                public static Element ElementFromLine(Format format, string line) {                                         ; CheckFormat(format, line); return new Element(line, Atom.type, format); }
                public static Element ElementFromLine(               string line) { Format format = Format.defformat_digit06; CheckFormat(format, line); return new Element(line, Atom.type, format); }
                public static Element ElementFromData(int id, string atomtype, double x, double y, double z, int atomid, int[] bondedids, bool autoAdjustCoord=false)
                {
                    if(HDebug.Selftest())
                    {
                        string line0, line1;
                        ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                        ///  ================================================================================
                        /// "     1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7"
                        /// "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7"
                        line0 =    "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7";
                        line1 = ElementFromData(1,"NH3", -7.403641,   7.761010,  19.275393,   65,
                                                                                  new int[] { 2,    5,    6,    7 } ).line;
                        HDebug.Exception(line0 == line1);
                        line0 =     "    41  O      0.845971   11.532886   21.390802    74    40";
                        line1 = ElementFromData(41, "O",   0.845971,  11.532886,  21.390802,   74,
                                                                                  new int[] { 40 } ).line;
                        HDebug.Exception(line0 == line1);
                    }
                    return ElementFromData(Format.defformat_digit06, id, atomtype, x, y, z, atomid, bondedids, autoAdjustCoord);
                }
                public static Element ElementFromData(Format format, int id, string atomtype, double x, double y, double z, int atomid, int[] bondedids, bool autoAdjustCoord=false)
                {
                    if(HDebug.Selftest())
                    {
                        string line0, line1;
                        ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                        ///  ================================================================================
                        /// "     1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7"
                        /// "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7"
                        line0 =                              "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7";
                        line1 = ElementFromData(Format.defformat_digit06, 1,"NH3", -7.403641,   7.761010,  19.275393,   65,
                                                                                                            new int[] { 2,    5,    6,    7 } ).line;
                        HDebug.Exception(line0 == line1);
                        line0 =                               "    41  O      0.845971   11.532886   21.390802    74    40";
                        line1 = ElementFromData(Format.defformat_digit06, 41,"O",    0.845971,  11.532886,  21.390802,   74
                                                                                                            ,new int[]{ 40 } ).line;
                        HDebug.Exception(line0 == line1);

                        line0 =                                   "     13  OT         -85.4401110000        -18.6572660000         -9.9272310000   101     14     15";
                        line1 = ElementFromData(Format._defformat_digit10_id7, 13, "OT",      -85.4401110000,       -18.6572660000,        -9.9272310000,  101,
                                                                                                                                                new int[] { 14,    15 } ).line;
                        HDebug.Exception(line0 == line1);
                    }

                    string line = "";
                    line += string.Format(format.formatId      ,       id).HSubEndStringCount(format.idxId      [1]-format.idxId      [0]+1);
                    line += "  ";
                    line += string.Format(format.formatAtomType, atomtype).Substring       (0,format.idxAtomType[1]-format.idxAtomType[0]+1);
                    line += string.Format(format.formatX       ,        x).HSubEndStringCount(format.idxX       [1]-format.idxX       [0]+1);
                    line += string.Format(format.formatY       ,        y).HSubEndStringCount(format.idxY       [1]-format.idxY       [0]+1);
                    line += string.Format(format.formatZ       ,        z).HSubEndStringCount(format.idxZ       [1]-format.idxZ       [0]+1);
                    line += string.Format(format.formatAtomId  , atomid  ).HSubEndStringCount(format.idxAtomId  [1]-format.idxAtomId  [0]+1);
                    for(int i=0; i<bondedids.Length; i++)
                    {
                        int[] idxBondedId = GetIdxBondedId(format, i);
                        string bond = string.Format(format.formatBondedId, bondedids[i]).HSubEndStringCount(idxBondedId[1]-idxBondedId[0]+1);
                        if(bond[0] != ' ') bond = " "+bond;
                        line += bond;
                    }
                    Element elem = new Element(line, Atom.type, format);
                    HDebug.Assert(id                     == elem.Atom.Id              );
                    HDebug.Assert(atomtype.Trim()        == elem.Atom.AtomType.Trim() );
                    HDebug.Assert(atomid                 == elem.Atom.AtomId          );
                    HDebug.Assert(bondedids.HToVectorT() == elem.Atom.BondedIds);
                    // exceptional case that the atom is out of available number range
                    if(autoAdjustCoord == false) { HDebug.AssertTolerance(0.000001, x-elem.Atom.X); } else { if(Math.Abs(x-elem.Atom.X) > 0.000001) return ElementFromData(format, id, atomtype, x/2, y, z, atomid, bondedids, autoAdjustCoord); }
                    if(autoAdjustCoord == false) { HDebug.AssertTolerance(0.000001, y-elem.Atom.Y); } else { if(Math.Abs(y-elem.Atom.Y) > 0.000001) return ElementFromData(format, id, atomtype, x, y/2, z, atomid, bondedids, autoAdjustCoord); }
                    if(autoAdjustCoord == false) { HDebug.AssertTolerance(0.000001, z-elem.Atom.Z); } else { if(Math.Abs(z-elem.Atom.Z) > 0.000001) return ElementFromData(format, id, atomtype, x, y, z/2, atomid, bondedids, autoAdjustCoord); }
                    return elem;
                }
                public static Element ElementFromCoord(Atom src, Vector coord)
                {
                    return ElementFromCoord(src, coord, Format.defformat_digit06);
                }
                public static Element ElementFromCoord(Atom src, Vector coord, Format format)
                {
                    double x = coord[0];
                    double y = coord[1];
                    double z = coord[2];
                    Element elem = ElementFromData(format, src.Id, src.AtomType, x, y, z, src.AtomId, src.BondedIds);
                    return  elem;
                }

                //  public Prm.Charge GetCharge(Prm prm)
                //  {
                //      Dictionary(int, Prm.Charge) prm_id2charge = prm.charges.ToIdDictionary();
                //      Prm.Charge prm_charge = prm_id2charge[AtomId];
                //      return prm_charge;
                //  }
                public Prm.Charge GetPrmCharge(Prm prm)
                {
                    return GetPrmCharge(prm.charges);
                }
                public Prm.Charge GetPrmCharge(Prm.Charge[] prm_charges)
                {
                    Dictionary<int, Prm.Charge> prm_id2charge = prm_charges.ToIdDictionary();
                    return GetPrmCharge(prm_id2charge);
                }
                public Prm.Charge GetPrmCharge(Dictionary<int, Prm.Charge> prm_id2charge)
                {
                    Prm.Charge prm_charge = prm_id2charge[AtomId];
                    return prm_charge;
                }

                //  public double GetMass(Prm prm)
                //  {
                //      Dictionary(int,Prm.Atom) prm_id2atom = prm.atoms.ToIdDictionary();
                //      Prm.Atom prm_atom = prm_id2atom  [AtomId];
                //      double mass = prm_atom.Mass;
                //      return mass;
                //  }
                public double GetMass(Prm prm)
                {
                    return GetMass(prm.atoms);
                }
                public double GetMass(Prm.Atom[] prm_atoms)
                {
                    Dictionary<int,Prm.Atom> prm_id2atom = prm_atoms.ToIdDictionary();
                    return GetMass(prm_id2atom);
                }
                public double GetMass(Dictionary<int, Prm.Atom> prm_id2atom)
                {
                    Prm.Atom prm_atom = prm_id2atom[AtomId];
                    double mass = prm_atom.Mass;
                    return mass;
                }
                public Prm.Atom GetPrmAtom(Prm prm)
                {
                    return GetPrmAtom(prm.atoms);
                }
                public Prm.Atom GetPrmAtom(Prm.Atom[] prm_atoms)
                {
                    Dictionary<int,Prm.Atom> prm_id2atom = prm_atoms.ToIdDictionary();
                    return GetPrmAtom(prm_id2atom);
                }
                public Prm.Atom GetPrmAtom(Dictionary<int, Prm.Atom> prm_id2atom)
                {
                    Prm.Atom prm_atom = prm_id2atom[AtomId];
                    return prm_atom;
                }

                //  public Prm.Vdw GetVdw(Prm prm)
                //  {
                //      Dictionary<int,Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
                //      Dictionary<int,Prm.Vdw > prm_cls2vdw = prm.vdws .ToClassDictionary();
                //      Prm.Atom   prm_atom = prm_id2atom  [this.AtomId];
                //      Prm.Vdw    prm_vdw  = prm_cls2vdw[prm_atom.Class];
                //      return prm_vdw;
                //  }
                public Prm.Vdw GetPrmVdw(Prm prm)
                {
                    Dictionary<int,Prm.Atom> prm_id2atom = prm.atoms.ToIdDictionary();
                    Dictionary<int,Prm.Vdw > prm_cls2vdw = prm.vdws .ToClassDictionary();
                    return GetPrmVdw(prm_id2atom, prm_cls2vdw);
                }
                public Prm.Vdw GetPrmVdw
                    ( Dictionary<int, Prm.Atom> prm_id2atom
                    , Dictionary<int, Prm.Vdw > prm_cls2vdw
                    )
                {
                    Prm.Atom   prm_atom = prm_id2atom  [this.AtomId];
                    Prm.Vdw    prm_vdw  = prm_cls2vdw[prm_atom.Class];
                    return prm_vdw;
                }
            }
        }
    }
}
