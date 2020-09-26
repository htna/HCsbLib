using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Xyz     = Tinker.Xyz;
    using Prm     = Tinker.Prm;
    public static partial class TinkerStatic
    {
        public static Xyz Merge( (Xyz xyz1, Xyz xyz2) xyzs, Xyz.Atom.Format format)
        {
            return null;
        }
        public static Xyz Merge(IEnumerable<Xyz.Atom> atoms1, IEnumerable<Xyz.Atom> atoms2, Xyz.Atom.Format format)
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
    }
}
