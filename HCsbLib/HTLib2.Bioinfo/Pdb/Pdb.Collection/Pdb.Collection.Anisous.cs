using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class PdbCollection
    {
        public static List<   int> ListSerial    (this IList<Pdb.Anisou> anisous)   { List<   int> list = new List<   int>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.serial    ); return list; }
        public static List<string> ListName      (this IList<Pdb.Anisou> anisous)   { List<string> list = new List<string>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.name      ); return list; }
        public static List<  char> ListAltLoc    (this IList<Pdb.Anisou> anisous)   { List<  char> list = new List<  char>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.altLoc    ); return list; }
        public static List<string> ListResName   (this IList<Pdb.Anisou> anisous)   { List<string> list = new List<string>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.resName   ); return list; }
        public static List<  char> ListChainID   (this IList<Pdb.Anisou> anisous)   { List<  char> list = new List<  char>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.chainID   ); return list; }
        public static List<   int> ListResSeq    (this IList<Pdb.Anisou> anisous)   { List<   int> list = new List<   int>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.resSeq    ); return list; }
        public static List<  char> ListICode     (this IList<Pdb.Anisou> anisous)   { List<  char> list = new List<  char>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.iCode     ); return list; }
        public static List<string> ListElement   (this IList<Pdb.Anisou> anisous)   { List<string> list = new List<string>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.element   ); return list; }
        public static List<string> ListCharge    (this IList<Pdb.Anisou> anisous)   { List<string> list = new List<string>(anisous.Count); foreach(Pdb.Anisou anisou in anisous) list.Add(anisou.charge    ); return list; }

        public static List<MatrixByArr> ListU(this IList<Pdb.Anisou> anious)
        {
            int size = anious.Count;
            MatrixByArr[] listu = new MatrixByArr[size];
            for(int i=0; i<size; i++)
                listu[i] = anious[i].U;
            return new List<MatrixByArr>(listu);
        }
        public static List<Anisou> ListBioAnisou(this IList<Pdb.Anisou> anisous)
        {
            int size = anisous.Count;
            Anisou[] bioanisou = new Anisou[size];
            for(int i=0; i<size; i++)
                bioanisou[i] = Anisou.FromMatrix(anisous[i].U);
            return new List<Anisou>(bioanisou);
        }
        public static List<Pdb.Anisou> SelectBySerial(this IList<Pdb.Anisou> anisous, IList<int> serials)
        {
            Dictionary<int, Pdb.Anisou> serial_anisou = new Dictionary<int, Pdb.Anisou>();
            foreach(Pdb.Anisou anisou in anisous)
                serial_anisou.Add(anisou.serial, anisou);

            List<Pdb.Anisou> select = new List<Pdb.Anisou>(serials.Count);
            foreach(int serial in serials)
                select.Add(serial_anisou[serial]);

            return select;
        }
    }
}
