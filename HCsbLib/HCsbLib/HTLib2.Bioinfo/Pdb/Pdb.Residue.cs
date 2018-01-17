using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Pdb.Atom;
    using Residue = Pdb.Residue;

    public partial class Pdb
    {
        [Serializable]
        public class Residue
        {
            public char       ChainID;
            public string     ResName;
            public int        ResSeq;
            public List<Atom> atoms;
            public override string ToString() { return ResSeq+": "+ResName+", "+atoms.Count+" atoms"; }

            public static IEqualityComparer<Residue> GetEqualityComparerByName() { return new EqualityComparerByName(); }
            public class EqualityComparerByName : IEqualityComparer<Residue>
            {
                public bool Equals(Residue x, Residue y) { return (x.ResName == y.ResName); }
                public int GetHashCode(Residue obj) { return obj.GetHashCode(); }
            }
        }
    }

    public static partial class PdbStatic
    {
        public static string[] HListResName(this IList<Residue> resis)
        {
            string[] list = resis.Select(delegate(Residue resi) { return resi.ResName; }).ToArray();
            return list;
        }
        public static int[] HListResSeq(this IList<Residue> resis)
        {
            int[] list = resis.Select(delegate(Residue resi) { return resi.ResSeq; }).ToArray();
            return list;
        }
        public static Tuple<string, int>[] HListResNameSeq(this IList<Residue> resis)
        {
            string[] resnames = resis.HListResName();
            int   [] resseqs  = resis.HListResSeq ();
            Tuple<string, int>[] resnameseqs = resnames.HToIListTuple(resseqs);

            Tuple<string, int>[] list = new Tuple<string, int>[resis.Count];
            for(int i=0; i<resis.Count; i++)
                list[i] = new Tuple<string,int>(resis[i].ResName, resis[i].ResSeq);
            return list;
        }
        public static Residue SelectByResSeq(this IList<Residue> resis, int ResSet)
        {
            foreach(Residue resi in resis)
            {
                if(resi.ResSeq == ResSet)
                    return resi;
            }
            HDebug.Assert(false);
            return null;
        }
        public static Residue[] ListResidue(this IList<Atom> atoms)
        {
            List<Residue> residues = new List<Residue>();
            Dictionary<char, List<Atom>> chain2atoms = atoms.GroupChainID();
            foreach(char ChainID in chain2atoms.Keys)
            {
                int? prv_resseq = null;
                Dictionary<int,List<Atom>> resi2atoms = chain2atoms[ChainID].GroupResSeq();
                foreach(int ResSeq in resi2atoms.Keys)
                {
                    string ResName = resi2atoms[ResSeq][0].resName;
                    Residue residue = new Residue{  ChainID = ChainID,
                                                    ResName = ResName,
                                                    ResSeq  = ResSeq,
                                                    atoms   = resi2atoms[ResSeq]
                                                 };
                    residues.Add(residue);

                    if(prv_resseq != null)
                        HDebug.Assert(prv_resseq < residue.ResSeq);
                    prv_resseq = residue.ResSeq;
                }
            }
            return residues.ToArray();
        }
    }
}
