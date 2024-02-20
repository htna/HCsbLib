using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using TkFile  = Tinker.TkFile;
    using Element = Tinker.TkFile.Element;
    public partial class Tinker
    {
        public partial class Vel : IBinarySerializable
        {
            public Element[] elements;
            public Atom[]    atoms { get { return elements.HSelectByType<Element,Atom>().ToArray(); } }
            public Atom.Format    atoms_format
            {
                get
                {
                    Atom[] atoms = this.atoms;
                    Atom.Format format = atoms[0].format;
                    if(HDebug.IsDebuggerAttached)
                    {
                        foreach(var atom in atoms)
                            HDebug.Assert(format == atom.format);
                    }
                    return format;
                }
            }
            public Vel(Element[] elements)
            {
                this.elements = elements;
            }
            ///////////////////////////////////////////////////
            // IBinarySerializable
            public void BinarySerialize(HBinaryWriter writer)
            {
                writer.Write(atoms_format);
                writer.Write(elements.Length);
                for(int i=0; i<elements.Length; i++)
                {
                    Element element = elements[i];
                    switch(element)
                    {
                        case Header header:
                            writer.Write("Header");
                            header.BinarySerialize(writer);
                            break;
                        case Atom atom:
                            writer.Write("Atom");
                            atom.BinarySerialize(writer);
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }
            public Vel(HBinaryReader reader)
            {
                Atom.Format format; reader.Read(out format);
                int         length; reader.Read(out length);
                elements = new Element[length];
                for(int i=0; i<elements.Length; i++)
                {
                    string type; reader.Read(out type);
                    Element element;
                    switch(type)
                    {
                        case "Header":
                            element = new Header(reader);
                            HDebug.Assert(object.ReferenceEquals(format, (element as Header).format));
                            break;
                        case "Atom":
                            element = new Atom(reader);
                            HDebug.Assert(object.ReferenceEquals(format, (element as Atom).format));
                            break;
                        default:
                            throw new Exception();
                    }
                    elements[i] = element;
                }
            }
            /// 186  GNOMES, ROCK MONSTERS AND CHILI SAUCE
            ///   1  NH3  -11.020000  -12.540000  -24.210000    65     2     5     6     7
            ///   2  CT1  -12.460000  -12.650000  -24.060000    24     1     3     8     9
            ///   3  C    -12.800000  -14.020000  -23.500000    20     2     4    25
            public static Vel FromFile(string path, bool loadLatest)
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

                int   idxId_length;// = idxId[1] - idxId[0] + 1;
                {
                    int val; int.TryParse(line0, out val);
                    string[] tokens = line0.Split(new char[]{' ' }, StringSplitOptions.RemoveEmptyEntries);
                    idxId_length = line0.IndexOf(tokens[0]) + tokens[0].Length;
                }

                int[] idxId = new int[] { 0, idxId_length-1 };

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

                int[] idxDX;
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
                    idxDX = new int[] { idxX0, idxX1 };
                    nDecimalPlaces = idxX1 - idxDecimalPoint;
                }
                int idxX_length = idxDX[1] - idxDX[0] + 1;

                int[] idxDY        = new int[2]; idxDY       [0] = idxDX     [1] + 1; idxDY       [1] = idxDY       [0] + idxX_length  - 1;
                int[] idxDZ        = new int[2]; idxDZ       [0] = idxDY     [1] + 1; idxDZ       [1] = idxDZ       [0] + idxX_length  - 1;

                string formatId       = "                                            {0}";  // HSubEndStringCount
                string formatAtomType = "{0}                                            ";  // Substring
                string formatDXDYDZ   = "                               {0:0.######E+00}";//"                               {0:0."; for(int i=0; i<nDecimalPlaces; i++) formatDXDYDZ += "0"; formatDXDYDZ+="}";

                Atom.Format format = new Atom.Format
                {
                    idxId       = idxId      ,    formatId       = formatId      ,
                    idxAtomType = idxAtomType,    formatAtomType = formatAtomType,
                    idxDX       = idxDX      ,    formatDX       = formatDXDYDZ  ,
                    idxDY       = idxDY      ,    formatDY       = formatDXDYDZ  ,
                    idxDZ       = idxDZ      ,    formatDZ       = formatDXDYDZ  ,
                };

                return format;
            }
            public static Vel FromFile(string path, bool loadLatest, Atom.Format format)
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
            public void ToFile(string path, bool saveAsNext, Atom.Format format)//, bool autoAdjustCoord=false)
            {
                if(format == null)
                    format = atoms_format;

                List<Element> nelems = new List<Element>();
                foreach(var elem in elements)
                {
                    if(elem is Atom)
                    {
                        var atom = elem as Atom;
                        var natom = Atom.FromData(format, atom.Id, atom.AtomType.Trim(), atom.DX, atom.DY, atom.DZ);//, autoAdjustCoord);
                        nelems.Add(natom);
                    }
                    else if(elem is Header)
                    {
                        var header = elem as Header;
                        var nheader = Header.FromData(format, header.NumAtoms);
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
                        nelems.Add(elem);
                    }
                }

                TkFile.ElementsToFile(path, saveAsNext, nelems);
            }
            public static Vel FromAtoms(IList<Vel.Atom> atoms)
            {
                Atom.Format format = atoms.First().format;
                Header header = Header.FromData(format, atoms.Count);

                List<string> lines = new List<string>(atoms.Count+1);

                lines.Add(header.line);
                foreach(var atom in atoms)
                    lines.Add(atom.line);

                return FromLines(format, lines);
            }
            public static Vel FromLines(IList<string> lines)
            {
                Atom.Format format = new Atom.Format();
                return FromLines(format, lines);
            }
            public static Element[] GetElementsFromLines(Atom.Format format, IList<string> lines)
            {
                if(HDebug.Selftest())
                    #region MyRegion
                {
                    //Vel lxyz = FromLines(Selftest.selftest_xyz);
                    //Pdb lpdb = Pdb.FromLines(Selftest.selftest_pdb);
                    //lxyz.ListMatch(lpdb.atoms);
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
                elements[0] = new Header(format, lines[0]);
                for(int i=1; i<lines.Count; i++)
                    elements[i] = new Atom(format, lines[i]);

                return elements;
            }
            public static Vel FromLines(Atom.Format format, IList<string> lines)
            {
                Element[] elements = GetElementsFromLines(format, lines);
                Vel xyz = new Vel( elements );
                
                return xyz;
            }
            public static Vel FromCoords(Atom.Format format, Vel src, IList<Vector> coords)
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
                            Atom natom = Atom.FromCoord(format, atom, coord);
                            elements[i] = natom;
                            break;
                        default:
                            HDebug.Assert(false);
                            break;
                    }
                }

                Vel xyz = new Vel( elements );
                return xyz;
            }

            //public class Element : Tinker.TkFile.Element
            //{
            //    public readonly Format format;
            //    public Element(Format format, string line) : base(line) { this.format = format; }
            //}
            public class Header : Element, IBinarySerializable
            {
                ///////////////////////////////////////////////////
                // IBinarySerializable
                public new void BinarySerialize(HBinaryWriter writer)
                {
                    base.BinarySerialize(writer);
                    writer.Write(format);
                }
                public Header(HBinaryReader reader)
                    : base(reader)
                {
                    reader.Read(out format);
                }
                // IBinarySerializable
                ///////////////////////////////////////////////////

                public readonly Atom.Format format;
                public Header(Atom.Format format, string line) : base(line) { this.format = format                  ; }
                public override string type { get { return "Header"; } }
                ///  num atoms
                ///  0-5
                /// 
                ///  0        1         2         3         4         5         6         7         8
                ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///  ================================================================================
                ///    3138  HYDROLASE/HYDROLASE INHIBITOR           02-OCT-12   4HDB
                /// "  2521  HEME PROTEIN                            25-FEB-98   1A6G"
                public int NumAtoms { get { return GetInt(format.idxId).Value; } }
                public static Header FromData(Atom.Format format, int numatoms, string description="", string date="", string pdbid="")
                {
                    //if(HDebug.Selftest())
                    //{
                    //    var line0 = "  2521  HEME PROTEIN                            25-FEB-98   1A6G";
                    //    var line1 = FromData(2521, "HEME PROTEIN", "25-FEB-98", "1A6G").line;
                    //    HDebug.Exception(line0 == line1);
                    //}
                    string line = "";
                    line += string.Format("                    {0}", numatoms).HSubEndStringCount(format.idxId[1]-format.idxId[0]+1);
                    line += "  ";
                    if(description != null) line += (description + "                                        ").Substring(0, 40);
                    if(date        != null) line += (date        + "                                        ").Substring(0, 12);
                    if(pdbid       != null) line += (pdbid       + "                                        ").Substring(0,  4);
                    line = line.TrimEnd();
                    Header header = new Header(format, line);
                    HDebug.Assert(numatoms == header.NumAtoms);
                    return header;
                }
            }
            public class Atom : Element, IBinarySerializable, ICloneable
            {
                public class Format : IBinarySerializable
                {
                    public int[] idxId       = new int[]{ 0, 5};    public string formatId       = "                     {0}";  // HSubEndStringCount
                    public int[] idxAtomType = new int[]{ 8,10};    public string formatAtomType = "{0}                     ";  // Substring
                    public int[] idxDX       = new int[]{11,22};    public string formatDX       = "        {0:0.######E+00}";  // HSubEndStringCount
                    public int[] idxDY       = new int[]{23,34};    public string formatDY       = "        {0:0.######E+00}";  // HSubEndStringCount
                    public int[] idxDZ       = new int[]{35,46};    public string formatDZ       = "        {0:0.######E+00}";  // HSubEndStringCount

                    public int IdSize    { get { return (idxId[1] - idxId[0]); } }
                    public int CoordSize { get { return (idxDX[1] - idxDX[0]); } }

                    public Format(int IdSize=5, int CoordSize=11, string CoordFormat="{0:0.000000}")
                    {
                        UpdateFormat(IdSize, CoordSize, CoordFormat);
                    }
                    public void UpdateFormat(int IdSize=5, int CoordSize=11, string CoordFormat="{0:0.000000}")
                    {
                        int i = 0;
                        idxId       = new int[]{ i, i+IdSize    };    i+=3+IdSize   ;    formatId       = "                     {0}";  // HSubEndStringCount
                        idxAtomType = new int[]{ i, i+2         };    i+=3          ;    formatAtomType = "{0}                     ";  // Substring
                        idxDX       = new int[]{ i, i+CoordSize };    i+=1+CoordSize;    formatDX       = "            "+CoordFormat;  // HSubEndStringCount
                        idxDY       = new int[]{ i, i+CoordSize };    i+=1+CoordSize;    formatDY       = "            "+CoordFormat;  // HSubEndStringCount
                        idxDZ       = new int[]{ i, i+CoordSize };    i+=1+CoordSize;    formatDZ       = "            "+CoordFormat;  // HSubEndStringCount
                    }
                    ///////////////////////////////////////////////////
                    // IBinarySerializable
                    public void BinarySerialize(HBinaryWriter writer)
                    {
                        writer.Write(idxId      );
                        writer.Write(idxAtomType);
                        writer.Write(idxDX      );
                        writer.Write(idxDY      );
                        writer.Write(idxDZ      );
                    }
                    public Format(HBinaryReader reader)
                    {
                        reader.Read(out idxId      );
                        reader.Read(out idxAtomType);
                        reader.Read(out idxDX      );
                        reader.Read(out idxDY      );
                        reader.Read(out idxDZ      );
                    }
                    // IBinarySerializable
                    ///////////////////////////////////////////////////
                }

                ///////////////////////////////////////////////////
                // IBinarySerializable
                public new void BinarySerialize(HBinaryWriter writer)
                {
                    base.BinarySerialize(writer);
                    writer.Write(format);
                }
                public Atom(HBinaryReader reader)
                    : base(reader)
                {
                    reader.Read(out format);
                }
                // IBinarySerializable
                ///////////////////////////////////////////////////
                public Atom CloneAtom()
                {
                    Atom natom = new Atom(format, line);
                    return natom;
                }
                public object Clone()
                {
                    return CloneAtom();
                }
                ///////////////////////////////////////////////////
                public readonly Format format;
                public Atom(Format format, string line) : base(line) { this.format = format                  ; CheckFormat(format, line); }
                static void CheckFormat(Format format, string line)
                {
                    for(int i=1+format.idxId      [1]; i<format.idxAtomType[0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxAtomType[1]; i<format.idxDX      [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxDX      [1]; i<format.idxDY      [0]; i++) HDebug.Assert(line[i] == ' ');
                    for(int i=1+format.idxDY      [1]; i<format.idxDZ      [0]; i++) HDebug.Assert(line[i] == ' ');
                }
                public override string type { get { return "Atom"; } }
                ///  id  (atom type in prm)   x     y      z        (atom-id in prm)  bonds, ...
                ///  0-5 8-10                11-22  23-34  35-46    47-52             53-58(6), 59-64(6), ...
                ///   
                ///  0        1         2         3         4         5         6         7         8
                ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///  ================================================================================
                ///       1  NH3   -4.040000   15.048000   13.602000    65     2     5     6     7


                public int    Id         { get { return GetInt      (format.idxId       ).Value; } }
                public string AtomType   { get { return GetString   (format.idxAtomType )      ; } }
                public double DX         { get { return GetDoubleExp(format.idxDX       ).Value; } }
                public double DY         { get { return GetDoubleExp(format.idxDY       ).Value; } }
                public double DZ         { get { return GetDoubleExp(format.idxDZ       ).Value; } }

                public Vector VelXYZ   { get { return new double[3]{ DX, DY, DZ }; } }

                static bool FromData_Selftest = HDebug.IsDebuggerAttached;
                public static Atom FromData(Format format, int id, string atomtype, double dx, double dy, double dz)//, bool autoAdjustCoord=false)
                {
                    if(FromData_Selftest)
                    {
                        FromData_Selftest = false;
                        string line0, line1;
                        Format tformat = new Format
                        {
                            idxId       = new int[]{ 0, 5},  formatId       = "                     {0}",
                            idxAtomType = new int[]{ 8,10},  formatAtomType = "{0}                     ",
                            idxDX       = new int[]{11,26},  formatDX       = "        {0:0.######E+00}",
                            idxDY       = new int[]{27,42},  formatDY       = "        {0:0.######E+00}",
                            idxDZ       = new int[]{43,58},  formatDZ       = "        {0:0.######E+00}",
                        };
                        ///  012345678901234567890123456789012345678901234567890123456789012345678901234567890
                        ///  ================================================================================
                        /// "     1  NH3   -0.668723D+01   -0.405234D+00   -0.331187D+01"
                        /// "     2  CT1    0.353263D+01   -0.436092D+01   -0.106154D+02"
                        /// "     3  C      0.176526D+01    0.125528D+01    0.969232D+01"
                        line0 =             "     1  NH3   -0.668723D+01   -0.405234D+00   -0.331187D+01";
                        line1 = FromData(tformat, 1,"NH3", -0.668723E+01,  -0.405234E+00,  -0.331187E+01 ).line;
                        HDebug.Exception(line0 == line1);

                        line0 =             "     2  CT1    0.353263D-12   -0.436092D+12    0.106154D-01";
                        line1 = FromData(tformat, 2,"CT1",  0.353263E-12,  -0.436092E+12,   0.106154E-01 ).line;
                        HDebug.Exception(line0 == line1);

                        line0 =             "     3  C      0.176526D+01    0.125528D+01    0.969232D+01";
                        line1 = FromData(tformat, 3,"C",    0.176526E+01,   0.125528E+01,   0.969232E+01 ).line;
                        HDebug.Exception(line0 == line1);

                        line0 =             "     4  O     -0.618584D+01    0.435150D+00   -0.112307D+01";
                        line1 = FromData(tformat, 4,"O",   -0.618584E+01,   0.435150E+00,  -0.112307E+01 ).line;
                        HDebug.Exception(line0 == line1);
                    }

                    string line = "";
                    line += string.Format(format.formatId      ,       id).HSubEndStringCount(format.idxId      [1]-format.idxId      [0]+1);
                    line += "  ";
                    line += string.Format(format.formatAtomType, atomtype).Substring       (0,format.idxAtomType[1]-format.idxAtomType[0]+1);
                    line += FormatDXYZ   (format.formatDX      ,       dx).HSubEndStringCount(format.idxDX      [1]-format.idxDX      [0]+1);
                    line += FormatDXYZ   (format.formatDY      ,       dy).HSubEndStringCount(format.idxDY      [1]-format.idxDY      [0]+1);
                    line += FormatDXYZ   (format.formatDZ      ,       dz).HSubEndStringCount(format.idxDZ      [1]-format.idxDZ      [0]+1);

                    Atom atom = new Atom(format, line);
                    HDebug.Assert(id              == atom.Id              );
                    HDebug.Assert(atomtype.Trim() == atom.AtomType.Trim() );
                    // exceptional case that the atom is out of available number range
                    //if(autoAdjustCoord == false) { HDebug.AssertTolerance(0.000001, x-atom.X); } else { if(Math.Abs(x-atom.X) > 0.000001) return FromData(format, id, atomtype, x/2, y, z, atomid, bondedids, autoAdjustCoord); }
                    //if(autoAdjustCoord == false) { HDebug.AssertTolerance(0.000001, y-atom.Y); } else { if(Math.Abs(y-atom.Y) > 0.000001) return FromData(format, id, atomtype, x, y/2, z, atomid, bondedids, autoAdjustCoord); }
                    //if(autoAdjustCoord == false) { HDebug.AssertTolerance(0.000001, z-atom.Z); } else { if(Math.Abs(z-atom.Z) > 0.000001) return FromData(format, id, atomtype, x, y, z/2, atomid, bondedids, autoAdjustCoord); }
                    return atom;
                    static string FormatDXYZ(string format, double dv)
                    {
                        int format_blanks = 0;
                        while(format[0] == ' ')
                        {
                            format = format.Substring(1);
                            format_blanks++;
                        }
                        string str = string.Format(format, dv);
                        string[] tokens = str.Split('E');

                        int exp = int.Parse(tokens[1]);
                        string expstr = string.Format("{0:00}",exp); if(exp>=0) expstr = "+"+expstr;
                        HDebug.Assert(tokens[1] == expstr);

                        string nfrastr = tokens[0];
                        while(nfrastr.Replace("-","").Length < 7)
                            nfrastr = nfrastr + "0";
                        for(int i=0; i<format_blanks; i++)
                            nfrastr = " " + nfrastr;
                        HDebug.Assert(nfrastr.Contains("0.") == false);

                        if(nfrastr.Contains("1.")) { nfrastr = nfrastr.Replace("1.","0.1"); exp++; }
                        if(nfrastr.Contains("2.")) { nfrastr = nfrastr.Replace("2.","0.2"); exp++; }
                        if(nfrastr.Contains("3.")) { nfrastr = nfrastr.Replace("3.","0.3"); exp++; }
                        if(nfrastr.Contains("4.")) { nfrastr = nfrastr.Replace("4.","0.4"); exp++; }
                        if(nfrastr.Contains("5.")) { nfrastr = nfrastr.Replace("5.","0.5"); exp++; }
                        if(nfrastr.Contains("6.")) { nfrastr = nfrastr.Replace("6.","0.6"); exp++; }
                        if(nfrastr.Contains("7.")) { nfrastr = nfrastr.Replace("7.","0.7"); exp++; }
                        if(nfrastr.Contains("8.")) { nfrastr = nfrastr.Replace("8.","0.8"); exp++; }
                        if(nfrastr.Contains("9.")) { nfrastr = nfrastr.Replace("9.","0.9"); exp++; }
                        string nexpstr = string.Format("{0:00}",exp); if(exp>=0) nexpstr = "+"+nexpstr;

                        string nstr = nfrastr + "D" + nexpstr;
                        return nstr;
                    }
                }
                public static Atom FromCoord(Format format, Atom src, Vector vel)
                {
                    double dx = vel[0];
                    double dy = vel[1];
                    double dz = vel[2];
                    Atom dest = FromData(format, src.Id, src.AtomType, dx, dy, dz);
                    return dest;
                }
            }
        }
    }
}
