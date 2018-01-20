using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Namd
    {
        public static Pdb CopyPdbOccupancyForMergedPsf(Pdb copyto, Pdb copyfrom1, Pdb copyfrom2)
        {
            KDTreeDLL.KDTree<Pdb.Atom> kdtree = new KDTreeDLL.KDTree<Pdb.Atom>(3);
            foreach(var atom in copyfrom1.atoms) kdtree.insert(atom.coord, atom);
            foreach(var atom in copyfrom2.atoms) kdtree.insert(atom.coord, atom);

            Pdb pdb = copyto.Clone();

            List<string> nlines = new List<string>();
            for(int i=0; i<pdb.elements.Length; i++)
            {
                var elemi = pdb.elements[i];
                if((elemi is Pdb.Atom) == false)
                {
                    nlines.Add(elemi.line);
                }
                else
                {
                    var atomi = elemi as Pdb.Atom;
                    var atomx = kdtree.nearest(atomi.coord);
                    HDebug.Assert(atomi.coord[0] == atomx.coord[0]);
                    HDebug.Assert(atomi.coord[1] == atomx.coord[1]);
                    HDebug.Assert(atomi.coord[2] == atomx.coord[2]);
                    /// 55 - 60        Real(6.2)     occupancy    Occupancy.
                    char[] line = atomi.line.ToCharArray();
                    line[55-1] = atomx.line[55-1];
                    line[56-1] = atomx.line[56-1];
                    line[57-1] = atomx.line[57-1];
                    line[58-1] = atomx.line[58-1];
                    line[59-1] = atomx.line[59-1];
                    line[60-1] = atomx.line[60-1];
                    string nline = line.HToString();
                    nlines.Add(nline);
                    Pdb.Atom natomi = Pdb.Atom.FromString(nline);
                    pdb.elements[i] = natomi;
                }
            }
            return pdb;
        }
    }
}
