using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Stream = System.IO.Stream;
    public partial class PdbDatabase
	{
        //static PdbInfo GetPdbInfo(string pdbid)
        //{
        //    Pdb pdb = Pdb.FromPdbid(pdbid);
        //    return GetPdbInfo(pdbid, pdb);
        //}
        static PdbInfo GetPdbInfo(string pdbid, Stream stream)
        {
            Pdb pdb = Pdb.FromStream(stream);
            return GetPdbInfo(pdbid, pdb);
        }
        static PdbInfo GetPdbInfo(string pdbid, List<string> lines)
        {
            Pdb pdb = Pdb.FromLines(lines);
            return GetPdbInfo(pdbid, pdb);
        }
        //public static PdbInfo GetPdbInfo(string pdbid, string pdbpath)
        //{
        //    if(HFile.Exists(pdbpath) == false)
        //        Pdb.FromPdbid(pdbid, cachepath: pdbpath);
        //    Pdb pdb = Pdb.FromFile(pdbpath);
        //    return GetPdbInfo(pdbid, pdb);
        //}
        //static PdbInfo GetPdbInfo(Pdb pdb)
        //{
        //    return GetPdbInfo("", pdb);
        //}
        static PdbInfo GetPdbInfo(string pdbid, Pdb pdb)
        {
            PdbInfo pdbinfo = new PdbInfo
            {
                pdbid       = pdbid,
                keywords    = pdb.elements.ListType<Pdb.Keywds>().ListKeyword().ToArray(),
                numAtom     = pdb.atoms.Length,
                numAnisou   = pdb.anisous.Length,
                numModel    = pdb.elements.ListType<Pdb.Model>().Count,
                numChain    = pdb.atoms.ListChainID().HListCommonT().Count,
                numAltloc   = pdb.atoms.ListAltLoc().HListCommonT().Count,
                numResidue  = pdb.atoms.ListResSeq().HListCommonT().Count,
            };

            return pdbinfo;
        }
        public static PdbInfo[] GetPdbInfo()
        {
            string[] pdbids = GetPdbIds();
            return GetPdbInfo(pdbids);
        }
        public static PdbInfo[] GetPdbInfo(params string[] pdbids)
        {
            Dictionary<string,PdbInfo> pdbinfos = new Dictionary<string, PdbInfo>();
            int VER = 0;

            string cachepath = RootPath + @"cache\GetPdbInfo.data";
            if(HFile.Exists(cachepath))
            {
                HSerialize.Deserialize(cachepath, VER, out pdbinfos);
                if(pdbinfos == null)
                    pdbinfos = new Dictionary<string, PdbInfo>();
            }

            bool updated = false;
            for(int i=0; i<pdbids.Length; i++)
            {
                string pdbid = pdbids[i];

                if(pdbinfos.ContainsKey(pdbid) == false)
                    pdbinfos.Add(pdbid, null);

                if(pdbinfos[pdbid] != null)
                    continue;

                updated = true;
                //continue;

                List<string> pdblines = GetPdbLines(pdbid);
                if(pdblines == null)
                    pdblines = GetPdbLines(pdbid, forceToRedownload: true);
                HDebug.Assert(pdblines != null);
                if(pdblines == null)
                {
                    System.Console.WriteLine(pdbid + " is not processed");
                    continue;
                }

                PdbInfo pdbinfo = GetPdbInfo(pdbid, pdblines);
                pdbinfos[pdbid] = pdbinfo;

                if(i%10 == 0)
                    System.Console.WriteLine(pdbid + " is processed. There are "+(pdbids.Length-i).ToString()+" unprocessed pdbs.");

                if(i%200 == 0)
                {
                    HSerialize.Serialize(cachepath, VER, pdbinfos);
                    updated = false;
                    System.Console.WriteLine("serialize cache");
                }
            }
            //GetPdbInfo(pdbinfos);

            if(updated)
                HSerialize.Serialize(cachepath, VER, pdbinfos);

            return pdbinfos.HSelectByKeys(pdbids);
        }
        public static void BuildPdbInfos(Dictionary<string,PdbInfo> pdbinfos)
        {
            HDebug.Assert(pdbinfos != null);

            int countremained = pdbinfos.Count;
            foreach(string pdbid in pdbinfos.Keys.ToArray())
            //Parallel.ForEach(key_pdbids.Keys, delegate(string key)
            {
                countremained--;

                if(pdbinfos[pdbid] != null)
                    continue;

                List<string> pdblines = GetPdbLines(pdbid);
                if(pdblines == null)
                    pdblines = GetPdbLines(pdbid, forceToRedownload:true);
                HDebug.Assert(pdblines != null);
                if(pdblines == null)
                {
                    System.Console.WriteLine(pdbid + " is not processed");
                    continue;
                }

                PdbInfo pdbinfo = GetPdbInfo(pdbid, pdblines);
                pdbinfos[pdbid] = pdbinfo;

                System.Console.WriteLine(pdbid + " is processed. There are "+countremained+" unprocessed pdbs.");

                //Serializer.Serialize(pdbid_pdbinfo_path, pdbid_pdbinfo_VER, pdbid_pdbinfo);
            }
            //);
        }
    }
}
