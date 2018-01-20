using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class PdbDatabase
	{
//        public static Dictionary<string, HashSet<string>> GetKeywords()
//        {
//            Dictionary<string, HashSet<string>> keywords;
//            int VER = 0;
//
//            string cachepath = RootPath + "cache.GetKeywords";
//            if(File.Exists(cachepath))
//            {
//                Serializer.Deserialize(cachepath, VER, out keywords);
//            }
//            else
//            {
//                string[] pdbids = GetPdbIds();
//                keywords = GetKeywords(pdbids);
//                Serializer.Serialize(cachepath, VER, keywords);
//            }
//
//            return keywords;
//        }
//        public static Dictionary<string, HashSet<string>> GetKeywords(IList<string> pdbids)
//        {
//            Dictionary<string, HashSet<string>> keywords = new Dictionary<string, HashSet<string>>();
//
//            //foreach(string pdbid in pdbids)
//            for(int i=0; i<pdbids.Count; i++)
//            {
//                string pdbid = pdbids[i];
//                string pdbpath = GetPdbPath(pdbid);
//                if(File.Exists(pdbpath) == false)
//                    continue;
//                Pdb pdb = Pdb.FromFile(pdbpath);
//
//                PdbInfo pdbinfo = GetPdbInfo(pdb);
//
//
//
//
//                List<string> keywds = pdb.elements.ListType<Pdb.Keywds>().ListKeyword();
//                foreach(string keywd in keywds)
//                {
//                    if(keywords.ContainsKey(keywd) == false)
//                        keywords.Add(keywd, new HashSet<string>());
//                    keywords[keywd].Add(pdbid);
//                }
//
//                if(i%10 == 0)
//                    System.Console.WriteLine(i);
//            }
//
//            return keywords;
//        }
    }
}
