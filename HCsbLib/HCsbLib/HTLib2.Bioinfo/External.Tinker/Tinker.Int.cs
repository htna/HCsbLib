/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    public partial class Tinker
    {
        public partial class Int
        {
            public Element[] elements;
            public static Int FromFile(string path, bool loadLatest)
            {
                string[] lines = TkFile.LinesFromFile(path, loadLatest);
                return FromLines(lines);
            }
            public void ToFile(string path, bool saveAsNext)
            {
                TkFile.ElementsToFile(path, saveAsNext, elements);
            }
            public static Int FromLines(IList<string> lines)
            {
                if(HDebug.Selftest())
                    #region MyRegion
                {
                }
                    #endregion

                if(lines == null)
                    return null;
                if(lines.Count == 0)
                    return null;

                Element[] elements = new Element[lines.Count];
                elements[0] = new Header(lines[0]);
                for(int i=1; i<lines.Count; i++)
                    elements[i] = new Atom(lines[i]);

                Int intrnl = new Int();
                intrnl.elements = elements;

                return intrnl;
            }
            public Int CloneByReindex()
            {
                Atom[] atoms = elements.OfType<Atom>().ToArray();
                Tuple<int,int>[] idFromTo = new Tuple<int, int>[atoms.Length];
                for(int i=0; i<atoms.Length; i++)
                    idFromTo[i] = new Tuple<int, int>(atoms[i].Id, i+1);
                return CloneByReindex(idFromTo);
            }
            public Int CloneByReindex(Tuple<int, int>[] idFromTo)
            {
                HDebug.ToDo("check!!");
                List<Element> nelements = new List<Element>();
                Dictionary<int,int> from2to = idFromTo.HToDictionaryWithKey1Value2();
                foreach(Element element in elements)
                {
                    Element nelement = null;
                    if(element.type == "Header")
                    {
                        Header header   = element as Header;
                        string name     = header.Name;
                        int    numatoms = from2to.Values.Max();
                        nelement = Header.FromData(numatoms, name);
                    }
                    else
                    {
                        Atom atom = element as Atom;
                        if(atom.Ids.Length < 4)
                        {
                            if(from2to[atom.Id] != atom.Id)
                                throw new Exception("indexes of first three should not change");
                            nelement = new Atom(element.line);
                        }
                        else
                        {
                            int Id         = from2to[atom.Id        ];
                            int IdBond     = from2to[atom.IdBond    ];
                            int IdAngle    = from2to[atom.IdAngle   ];
                            int IdDihedral = from2to[atom.IdDihedral];
                            nelement = Atom.FromData( Id        , atom.AtomType, atom.AtomId
                                                    , IdBond    , atom.Leng
                                                    , IdAngle   , atom.Angle
                                                    , IdDihedral, atom.Dihedral, atom.Chirality
                                                    );
                        }
                    }
                    if(nelement == null)
                        throw new NotImplementedException();
                    nelements.Add(nelement);
                }

                Int intrnl = new Int { elements = nelements.ToArray() };
                return intrnl;
            }


            /// http://chembytes.wikidot.com/tnk-tut02
            /// 
            ///      8  Ethane
            ///      1  C       1
            ///      2  C       1     1   1.60000
            ///      3  H       5     1   1.10000     2  109.4700
            ///      4  H       5     1   1.10000     2  109.4700     3  109.4700     1
            ///      5  H       5     1   1.10000     2  109.4700     3  109.4700    -1
            ///      6  H       5     2   1.10000     1  109.4700     3   10.0000     0
            ///      7  H       5     2   1.10000     1  109.4700     6  109.4700     1
            ///      8  H       5     2   1.10000     1  109.4700     6  109.4700    -1
            ///      ...
            ///   6548  OC     79  6534   1.26876  6533  117.1108  6535  120.3980     1
            /// 
            /// 0        1         2         3         4         5         6         7         8
            /// 012345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// ==================================================================================
            ///      8  Ethane
            ///   6548  OC     79  6534   1.26876  6533  117.1108  6535  120.3980     1
            ///   6351  CT1    25  6350   1.54768  6345  114.6924  6344  174.6729     0
            /// 
            /// The z-matrix should be red as following:
            /// 1.The first line, says that there are eight atoms in the system, and that the name of the
            ///   molecule is Ethane.
            /// 2.Second line set the origin of the system, i.e., from which atom start to build the molecule:
            ///   first atom is atom 1, which is a carbon atom, C, and its atom type for the MM3 force field
            ///   (see file ethane.key) is 1.
            /// 3.Third line says that the second atom in the molecule is a carbon atom, C, having MM3's atom
            ///   type 1, bonded to atom 1 placed at a distance of 1.6 Angstroms from the atom 1.
            /// 4.Fourth line says that the third atom in the structure is an hydrogen atoms, H, having MM3's
            ///   atom type 5, bonded to atom 1, placed at a distance of 1.1 Angstroms from atom 1 and forming
            ///   an angle of 109.47 degrees with atom 2.
            /// 5.Fifth line says that the fourth atom in the structure is an hydrogen atoms, H, having MM3's
            ///   atom type 5, bonded to atom 1, placed at a distance of 1.1 Angstroms from atom 1, and forming
            ///   an angle of 109.47 degrees with atom 2. Also, the dihedral angles between the planes defined
            ///   by atoms 1 2 3 and 2 3 4 is 109.47 degrees. The last column indicates the chirality flag (to
            ///   be honest I have never found this number in any other z-matrix, like those used in gaussian).
            ///   The structure of this line is then repeated for all the remain atoms.
            /// 
            /// Z-matrices are particularly useful for conformational search, allowing to vary the geometry
            ///  of the molecules changing bonds, angles or torsional angles in a systematic way wondering to
            ///  rewrite the atomic coordinates for each geometry. As example, think to to sample the torsion
            ///  of the two methyl groups in the ethane with respect the central bond. you can easily change
            ///  the dihedral angles in the z-matrix, rather than write many cartesian coordinate files.

            public class Header : Element
            {
                public Header(string line) : base(line) { }
                public override string type { get { return "Header"; } }
                /// 1.The first line, says that there are eight atoms in the system, and that the name of the
                ///   molecule is Ethane.
                /// 
                /// 0         1         2         3         4         5         6         7         8
                /// 012345678901234567890123456789012345678901234567890123456789012345678901234567890
                /// ==================================================================================
                ///      8  Ethane
                public int    NumAtoms { get { return GetInt(idxNumAtoms).Value; } } static readonly int[] idxNumAtoms = new int[] { 0, 5 };
                public string Name     { get { return line.Substring(6).Trim(); } }
                public static Header FromData(int numatoms, string name)
                {
                    throw new NotImplementedException();
                }
            }
            public class Atom : Element
            {
                public Atom(string line) : base(line) { }
                public override string type { get { return "Atom"; } }
                /// 1.The first line, says that there are eight atoms in the system, and that the name of the
                ///   molecule is Ethane.
                /// 
                /// 0         1         2         3         4         5         6         7         8
                /// 012345678901234567890123456789012345678901234567890123456789012345678901234567890
                /// ==================================================================================
                ///      1  C       1
                ///      2  C       1     1   1.60000
                ///      3  H       5     1   1.10000     2  109.4700
                ///      4  H       5     1   1.10000     2  109.4700     3  109.4700     1
                ///   6548  OC     79  6534   1.26876  6533  117.1108  6535  120.3980     1
                ///   6351  CT1    25  6350   1.54768  6345  114.6924  6344  174.6729     0
                public int    Id         { get { return GetInt   (idxId        ).Value; } } static readonly int[] idxId         = new int[] { 0, 5};
                public string AtomType   { get { return GetString(idxAtomType  )      ; } } static readonly int[] idxAtomType   = new int[] { 6,10};
                public int    AtomId     { get { return GetInt   (idxAtomId    ).Value; } } static readonly int[] idxAtomId     = new int[] {11,16};
                public int    IdBond     { get { return GetInt   (idxIdBond    ).Value; } } static readonly int[] idxIdBond     = new int[] {17,22};
                public double Leng       { get { return GetDouble(idxLeng      ).Value; } } static readonly int[] idxLeng       = new int[] {23,32};
                public int    IdAngle    { get { return GetInt   (idxIdAngle   ).Value; } } static readonly int[] idxIdAngle    = new int[] {33,38};
                public double Angle      { get { return GetDouble(idxAngle     ).Value; } } static readonly int[] idxAngle      = new int[] {39,48};
                public int    IdDihedral { get { return GetInt   (idxIdDihedral).Value; } } static readonly int[] idxIdDihedral = new int[] {49,54};
                public double Dihedral   { get { return GetDouble(idxDihedral  ).Value; } } static readonly int[] idxDihedral   = new int[] {55,64};
                public int    Chirality  { get { return GetInt   (idxChirality ).Value; } } static readonly int[] idxChirality  = new int[] {65,70};

                public int[]  Ids
                {
                    get
                    {
                        List<int> ids = new List<int>();
                        int? id0 = GetInt(idxId        ); if(id0 != null) ids.Add(id0.Value);
                        int? id1 = GetInt(idxIdBond    ); if(id1 != null) ids.Add(id1.Value);
                        int? id2 = GetInt(idxIdAngle   ); if(id2 != null) ids.Add(id2.Value);
                        int? id3 = GetInt(idxIdDihedral); if(id3 != null) ids.Add(id3.Value);
                        HDebug.AssertIf(true       , id0 != null);
                        HDebug.AssertIf(id0 != null, id1 != null);
                        HDebug.AssertIf(id1 != null, id2 != null);
                        HDebug.AssertIf(id2 != null, id3 != null);
                        return ids.ToArray();
                    }
                }
                public static Header FromData(int Id        , string AtomType ,int AtomId
                                            , int IdBond    , double Leng
                                            , int IdAngle   , double Angle
                                            , int IdDihedral, double Dihedral, int Chirality
                                            )
                {
                    string line = "                                                                       ";
                    line = UpdateLine(line, Id        , "           {0}", idxId        );
                    line = UpdateLine(line, AtomType  , "           {0}", idxAtomType  );
                    line = UpdateLine(line, AtomId    , "           {0}", idxAtomId    );
                    line = UpdateLine(line, IdBond    , "           {0}", idxIdBond    );
                    line = UpdateLine(line, Leng      , "     {0.00000}", idxLeng      );
                    line = UpdateLine(line, IdAngle   , "           {0}", idxIdAngle   );
                    line = UpdateLine(line, Angle     , "      {0.0000}", idxAngle     );
                    line = UpdateLine(line, IdDihedral, "           {0}", idxIdDihedral);
                    line = UpdateLine(line, Dihedral  , "      {0.0000}", idxDihedral  );
                    line = UpdateLine(line, Chirality , "           {0}", idxChirality );
                    return new Header(line);
                }
            }
        }
    }
}
*/