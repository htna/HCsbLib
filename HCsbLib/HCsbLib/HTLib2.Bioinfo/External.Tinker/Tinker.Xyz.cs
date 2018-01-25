﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    public static partial class TinkerStatic
    {
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
    }
    public partial class Tinker
    {
        public partial class Xyz
        {
            public Element[] elements;
            public Atom[]    atoms { get { return elements.HSelectByType<Element,Atom>().ToArray(); } }
            /// 186  GNOMES, ROCK MONSTERS AND CHILI SAUCE
            ///   1  NH3  -11.020000  -12.540000  -24.210000    65     2     5     6     7
            ///   2  CT1  -12.460000  -12.650000  -24.060000    24     1     3     8     9
            ///   3  C    -12.800000  -14.020000  -23.500000    20     2     4    25
            public static Xyz FromFile(string path, bool loadLatest)
            {
                Atom.Format format = new Atom.Format();
                return FromFile(path, loadLatest, format);
            }
            public static Xyz FromFile(string path, bool loadLatest, Atom.Format format)
            {
                string[] lines = TkFile.LinesFromFile(path, loadLatest);
                return FromLines(format, lines);
            }
            public void ToFile(string path, bool saveAsNext)
            {
                TkFile.ElementsToFile(path, saveAsNext, elements);
            }
            public void ToFile(string path, bool saveAsNext, Atom.Format format)
            {
                List<Element> nelems = new List<Element>();
                foreach(var elem in elements)
                {
                    if(elem is Atom)
                    {
                        var atom = elem as Atom;
                        var natom = Atom.FromData(format, atom.Id, atom.AtomType.Trim(), atom.X, atom.Y, atom.Z, atom.AtomId, atom.BondedIds);
                        nelems.Add(natom);
                    }
                    else
                    {
                        nelems.Add(elem);
                    }
                }

                TkFile.ElementsToFile(path, saveAsNext, nelems);
            }
            public static Xyz FromLines(IList<string> lines)
            {
                Atom.Format format = new Atom.Format();
                return FromLines(format, lines);
            }
            public static Xyz FromLines(Atom.Format format, IList<string> lines)
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
                elements[0] = new Header(lines[0]);
                for(int i=1; i<lines.Count; i++)
                    elements[i] = new Atom(format, lines[i]);

                Xyz xyz = new Xyz();
                xyz.elements = elements;
                
                return xyz;
            }
            public static Xyz FromCoords(Xyz src, IList<Vector> coords)
            {
                Element[] elements = src.elements.HClone();
                for(int i=0; i<elements.Length; i++)
                {
                    switch(elements[i].type)
                    {
                        case "Header":
                            HDebug.Assert((elements[0] as Header).NumAtoms == coords.Count);
                            break;
                        case "Atom":
                            Atom atom  = (elements[i] as Atom);
                            Vector coord = coords[atom.Id-1];
                            Atom natom = Atom.FromCoord(atom, coord);
                            elements[i] = natom;
                            break;
                        default:
                            HDebug.Assert(false);
                            break;
                    }
                }

                Xyz xyz = new Xyz { elements = elements };
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
            public class Header : Element
            {
                public Header(string line) : base(line) { }
                public override string type { get { return "Header"; } }
                ///  num atoms
                ///  0-5
                /// 
                ///  0        1         2         3         4         5         6         7         8
                ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///  ================================================================================
                ///    3138  HYDROLASE/HYDROLASE INHIBITOR           02-OCT-12   4HDB
                /// "  2521  HEME PROTEIN                            25-FEB-98   1A6G"
                public int NumAtoms { get { return GetInt(idxNumAtoms).Value; } } static readonly int[] idxNumAtoms = new int[] { 0, 5 };
                public static Header FromData(int numatoms, string description="", string date="", string pdbid="")
                {
                    if(HDebug.Selftest())
                    {
                        var line0 = "  2521  HEME PROTEIN                            25-FEB-98   1A6G";
                        var line1 = FromData(2521, "HEME PROTEIN", "25-FEB-98", "1A6G").line;
                        HDebug.Exception(line0 == line1);
                    }
                    string line = "";
                    line += string.Format("                    {0}", numatoms).HSubEndStringCount(idxNumAtoms[1]-idxNumAtoms[0]+1);
                    line += "  ";
                    if(description != null) line += (description + "                                        ").Substring(0, 40);
                    if(date        != null) line += (date        + "                                        ").Substring(0, 12);
                    if(pdbid       != null) line += (pdbid       + "                                        ").Substring(0,  4);
                    line = line.TrimEnd();
                    Header header = new Header(line);
                    HDebug.Assert(numatoms == header.NumAtoms);
                    return header;
                }
            }
            public class Atom : Element
            {
                public class Format
                {
                    public int[] idxId       = new int[]{ 0, 5};    public string formatId       = "                     {0}";  // HSubEndStringCount
                    public int[] idxAtomType = new int[]{ 8,10};    public string formatAtomType = "{0}                     ";  // Substring
                    public int[] idxX        = new int[]{11,22};    public string formatX        = "            {0:0.000000}";  // HSubEndStringCount
                    public int[] idxY        = new int[]{23,34};    public string formatY        = "            {0:0.000000}";  // HSubEndStringCount
                    public int[] idxZ        = new int[]{35,46};    public string formatZ        = "            {0:0.000000}";  // HSubEndStringCount
                    public int[] idxAtomId   = new int[]{47,52};    public string formatAtomId   = "                     {0}";  // HSubEndStringCount
                    public int[] idxBondedId = new int[]{53,58};    public string formatBondedId = "                     {0}";  // HSubEndStringCount
                }
                Format format;
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

                public Atom(Format format, string line) : base(line) { CheckFormat(format, line); this.format = format; }
                public Atom(               string line) : base(line) { CheckFormat(format, line); this.format = defformat_digit06; }
                static void CheckFormat(Format format, string line)
                {
                    for(int i=1+format.idxId      [1]; i<format.idxAtomType[0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxAtomType[1]; i<format.idxX       [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxX       [1]; i<format.idxY       [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxY       [1]; i<format.idxZ       [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxZ       [1]; i<format.idxAtomId  [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxAtomId  [1]; i<format.idxBondedId[0]; i++) HDebug.Assert(line[i] == ' ');
                }
                public override string type { get { return "Atom"; } }
                ///  id  (atom type in prm)   x     y      z        (atom-id in prm)  bonds, ...
                ///  0-5 8-10                11-22  23-34  35-46    47-52             53-58(6), 59-64(6), ...
                ///   
                ///  0        1         2         3         4         5         6         7         8
                ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///  ================================================================================
                ///       1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7

                public int    Id         { get { return GetInt   (format.idxId       ).Value; } }
                public string AtomType   { get { return GetString(format.idxAtomType )      ; } }
                public double X          { get { return GetDouble(format.idxX        ).Value; } }
                public double Y          { get { return GetDouble(format.idxY        ).Value; } }
                public double Z          { get { return GetDouble(format.idxZ        ).Value; } }
                public int    AtomId     { get { return GetInt   (format.idxAtomId   ).Value; } }
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
                        case 1: return GetTokenInt( 6);
                        case 2: return GetTokenInt( 7);
                        case 3: return GetTokenInt( 8);
                        case 4: return GetTokenInt( 9);
                        case 5: return GetTokenInt(10);
                        case 6: return GetTokenInt(11);
                        case 7: return GetTokenInt(12);
                        case 8: return GetTokenInt(13);
                        case 9: return GetTokenInt(14);
                    }
                    throw new NotImplementedException();
                }

                public Vector Coord    { get { return new double[3]{ X, Y, Z }; } }
                public int[]  BondedIds
                {
                    get
                    {
                        List<int> bondeds = new List<int>();
                        for(int idx=1; idx<20; idx++)
                        {
                            int? bonded = GetBondedId(idx);
                            if(bonded == null)
                                break;
                            bondeds.Add(bonded.Value);
                        }
                        return bondeds.ToArray();
                    }
                }

                public static Atom FromData(int id, string atomtype, double x, double y, double z, int atomid, params int[] bondedids)
                {
                    if(HDebug.Selftest())
                    {
                        string line0, line1;
                        ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                        ///  ================================================================================
                        /// "     1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7"
                        /// "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7"
                        line0 = "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7";
                        line1 = FromData(1, "NH3", -7.403641, 7.761010, 19.275393, 65,    2, 5, 6, 7).line;
                        HDebug.Exception(line0 == line1);
                        line0 = "    41  O      0.845971   11.532886   21.390802    74    40";
                        line1 = FromData(41, "O", 0.845971, 11.532886, 21.390802, 74, 40).line;
                        HDebug.Exception(line0 == line1);
                    }
                    return FromData(defformat_digit06, id, atomtype, x, y, z, atomid, bondedids);
                }
                public static Atom FromData(Format format, int id, string atomtype, double x, double y, double z, int atomid, params int[] bondedids)
                {
                    if(HDebug.Selftest())
                    {
                        string line0, line1;
                        ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                        ///  ================================================================================
                        /// "     1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7"
                        /// "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7"
                        line0 =                       "     1  NH3   -7.403641    7.761010   19.275393    65     2     5     6     7";
                        line1 = FromData(defformat_digit06, 1,"NH3", -7.403641,   7.761010,  19.275393,   65,    2,    5,    6,    7).line;
                        HDebug.Exception(line0 == line1);
                        line0 =                        "    41  O      0.845971   11.532886   21.390802    74    40";
                        line1 = FromData(defformat_digit06, 41,"O",    0.845971,  11.532886,  21.390802,   74,   40).line;
                        HDebug.Exception(line0 == line1);

                        line0 =                            "     13  OT         -85.4401110000        -18.6572660000         -9.9272310000   101     14     15";
                        line1 = FromData(_defformat_digit10_id7, 13, "OT",       -85.4401110000,       -18.6572660000,        -9.9272310000,  101,    14,    15).line;
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
                    Atom atom = new Atom(format, line);
                    HDebug.Assert(id              == atom.Id              );
                    HDebug.Assert(atomtype.Trim() == atom.AtomType.Trim() );
                    HDebug.Assert(atomid          == atom.AtomId          );
                    HDebug.Assert(bondedids.HToVectorT() == atom.BondedIds);
                    HDebug.AssertTolerance(0.000001, x-atom.X);
                    HDebug.AssertTolerance(0.000001, y-atom.Y);
                    HDebug.AssertTolerance(0.000001, z-atom.Z);
                    return atom;
                }
                public static Atom FromCoord(Atom src, Vector coord)
                {
                    return FromCoord(src, coord, defformat_digit06);
                }
                public static Atom FromCoord(Atom src, Vector coord, Atom.Format format)
                {
                    double x = coord[0];
                    double y = coord[1];
                    double z = coord[2];
                    Atom dest = FromData(format, src.Id, src.AtomType, x, y, z, src.AtomId, src.BondedIds);
                    return dest;
                }
            }
        }
    }
}