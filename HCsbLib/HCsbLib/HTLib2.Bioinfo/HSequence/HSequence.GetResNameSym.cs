using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class HSequence
	{
        public static string GetResiSequence(IList<Pdb.Atom> atoms)
        {
            Dictionary<int, List<Pdb.Atom>> resis = atoms.GroupResSeq();
            int minSeq = resis.Keys.Min();
            int maxSeq = resis.Keys.Max();
            string seq = "";
            for(int ir=minSeq; ir<=maxSeq; ir++)
            {
                if(resis.ContainsKey(ir))
                {
                    char? sym = resis[ir].First().resNameSyn;
                    if(sym == '?' ) sym = 'X';
                    if(sym == null) sym = 'X';
                    seq = seq + sym;
                }
                else
                {
                    seq = seq + "-";
                }
            }
            return seq;
        }
    }
}
