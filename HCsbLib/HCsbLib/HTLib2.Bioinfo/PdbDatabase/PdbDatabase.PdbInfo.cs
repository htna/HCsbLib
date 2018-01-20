using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using PdbInfo = PdbDatabase.PdbInfo;

    public partial class PdbDatabase
	{
        [Serializable]
        public class PdbInfo
        {
            public string pdbid;
            public string[] keywords;
            public int numAnisou; // number of anisous.
            public int numAtom;   // number of atoms
            public int numModel;  // 
            public int numChain;
            public int numAltloc;
            public int numResidue;

            public override string ToString()
            {
                string str = pdbid;
                str += string.Format(" - "+numAtom+" atom");
                str += string.Format(", "+numResidue+" res");
                str += string.Format(", "+numChain+" chain");
                str += string.Format(", "+numModel+" mdl");
                str += string.Format(" - ");
                foreach(string keyword in keywords)
                    str += ", " + keyword;
                return str;
            }

            public string[] GetKeywordTokens()
            {
                List<string> tokens = new List<string>();
                foreach(string keyword in keywords)
                {
                    string[] ltokens = keyword.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    ltokens = ltokens.HToUpper().ToArray();
                    tokens.AddRange(ltokens);
                }
                return (new HashSet<string>(tokens)).ToArray();
            }
        }
    }
    public static partial class PdbDatabaseStatic
	{
        public static PdbInfo[]     ListAnisou(this PdbInfo[]     infos) { int[] idx = infos.ListNumAnisou().HIndexGre(0); return infos.HSelectByIndex(idx).ToArray(); }
        //public static List<PdbInfo> ListAnisou(this List<PdbInfo> infos) { List<PdbInfo> sel = new List<PdbInfo>(); foreach(PdbInfo info in infos) if(info.numAnisou > 0) sel.Add(info); return sel; }

        public static int[] ListNumAnisou (this PdbInfo[] infos) { int[] list = new int[infos.Length]; for(int i=0; i<infos.Length; i++) list[i] =infos[i].numAnisou ; return list; }
        public static int[] ListNumAtom   (this PdbInfo[] infos) { int[] list = new int[infos.Length]; for(int i=0; i<infos.Length; i++) list[i] =infos[i].numAtom   ; return list; }
        public static int[] ListNumModel  (this PdbInfo[] infos) { int[] list = new int[infos.Length]; for(int i=0; i<infos.Length; i++) list[i] =infos[i].numModel  ; return list; }
        public static int[] ListNumChain  (this PdbInfo[] infos) { int[] list = new int[infos.Length]; for(int i=0; i<infos.Length; i++) list[i] =infos[i].numChain  ; return list; }
        public static int[] ListNumAltloc (this PdbInfo[] infos) { int[] list = new int[infos.Length]; for(int i=0; i<infos.Length; i++) list[i] =infos[i].numAltloc ; return list; }
        public static int[] ListNumResidue(this PdbInfo[] infos) { int[] list = new int[infos.Length]; for(int i=0; i<infos.Length; i++) list[i] =infos[i].numResidue; return list; }

        public static string[] ListPdbid(this PdbInfo[] infos)
        {
            string[] pdbids = new string[infos.Length];
            for(int i=0; i<infos.Length; i++)
                pdbids[i] = infos[i].pdbid;
            return pdbids;
        }

        public static PdbInfo[] ListNoKeyword(this PdbInfo[] infos, params string[] keywords)
        {
            keywords = keywords.HToUpper().ToArray();
            List<PdbInfo> select = new List<PdbInfo>();
            for(int i=0; i<infos.Length; i++)
            {
                string[] tokens = infos[i].GetKeywordTokens();
                IEnumerable<string> common = keywords.Intersect(tokens);
                if(common.Count() == 0)
                    select.Add(infos[i]);
            }
            return select.ToArray();
        }

        public static Dictionary<string,List<PdbInfo>> ListKeywordToInfo(this IEnumerable<PdbInfo> infos)
        {
            Dictionary<string, List<PdbDatabase.PdbInfo>> keywd_info = new Dictionary<string, List<PdbDatabase.PdbInfo>>();
            foreach(PdbDatabase.PdbInfo info in infos)
            {
                foreach(string keyword in info.keywords)
                {
                    if(keywd_info.ContainsKey(keyword) == false)
                        keywd_info.Add(keyword, new List<PdbDatabase.PdbInfo>());
                    keywd_info[keyword].Add(info);
                }
            }
            return keywd_info;
        }
    }
}
