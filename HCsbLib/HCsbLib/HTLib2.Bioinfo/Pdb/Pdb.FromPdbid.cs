using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public static Pdb FromPdbidCache(string pdbid, string cachepath)
		{
            Pdb pdb;
            if(HFile.Exists(cachepath))
            {
                pdb = FromPdbid(pdbid);
                pdb.ToFile(cachepath);
            }
            pdb = FromFile(cachepath);
            return pdb;
        }
        public static Pdb FromPdbid
            ( string pdbid
            , string cachebase  // null or @"K:\PdbUnzippeds\$PDBID$.pdb"
            , bool? download    // null or false
            )
        {
            if(cachebase == null) cachebase = @"D:\ProtDataBank\pdb\$PDBID$.pdb";
            if(download  == null) download  = false;

            string pdbpath = cachebase.Replace("$PDBID$", pdbid);

            if(HFile.Exists(pdbpath))
            {
                var pdb = Pdb.FromFile(pdbpath);
                var last = pdb.elements.Last();
                if(last is Pdb.End)        return pdb;
                if(last.line.Length == 80) return pdb;

                if(download.Value)
                {   // redownload
                    pdb = Pdb.FromPdbid(pdbid);
                    pdb.ToFile(pdbpath);
                }
                return pdb;
            }
            else if(download.Value)
            {
                Pdb pdb = Pdb.FromPdbid(pdbid);
                if(pdb != null)
                {
                    pdb.ToFile(pdbpath);
                    return pdb;
                }
            }
            return null;
        }
        public static Pdb FromPdbid(string pdbid, string exception_handling=null)
		{
            //Pdb pdb = FromFile(cachepath);
            //webClient.DownloadFile("http://mysite.com/myfile.txt", @"c:\myfile.txt");

            string address = string.Format(@"http://www.pdb.org/pdb/files/{0}.pdb", pdbid.ToUpper());
            System.Net.WebClient webClient = new System.Net.WebClient();
            Pdb pdb;
            try
            {
                Stream stream = webClient.OpenRead(address);
                pdb = FromStream(stream);
                stream.Close();
            }
            catch(Exception e)
            {
                switch(exception_handling)
                {
                    case "silent":
                        break;
                    case "rethrow":
                        throw e;
                    case null:
                    default:
                        System.Console.Error.WriteLine("error while downloading {0}", pdbid);
                        System.Console.Error.WriteLine(e.Message);
                        System.Console.Error.WriteLine(e);
                        break;
                }
                pdb = null;
            }
            webClient.Dispose();
            return pdb;
        }
        public static Pdb FromPdbidChainAltlocResis(string pdbIdChain, bool removeSymbolAltloc)
        {
            /// "pdbid:chains:altlocs:residues"
            /// ex: "1A6G:A::1-151"
            string[] tokens = pdbIdChain.Split(':');
            
            Pdb.Atom[] atoms;
            {
                string pdbid = tokens[0];
                Pdb pdb = FromPdbid(pdbid);
                atoms = pdb.atoms;
            }

            if(tokens.Length >= 2)
            {
                char[] chains = tokens[1].ToCharArray();
                atoms = atoms.SelectByChainID(chains).ToArray();
            }

            if((tokens.Length >= 3) && (tokens[2].Trim().Length >= 1))
            {
                char[] altlocs = tokens[2].ToCharArray();
                HDebug.Assert(altlocs.Length >= 1);
                HDebug.ToDo();
            }
            else
            {
                atoms = atoms.SelectByAltLoc().ToArray();
            }

            if((tokens.Length >= 4) && (tokens[3].Trim().Length >= 1))
            {
                /// "1,2,5,10,15-100,113,115-160"
                List<int> resis = new List<int>();
                string   resis0 = tokens[3].Trim();
                string[] resis1 = resis0.Split(',');
                foreach(string resis2 in resis1)
                {
                    string[] resis3 = resis2.Trim().Split('-').HTrim().HRemoveAll("");
                    int[]    resis4 = resis3.HParseInt();
                    if(resis4.Length == 1)
                    {
                        int resi = resis4[0];
                        HDebug.Assert(resis.Contains(resi) == false);
                        resis.Add(resi);
                        continue;
                    }
                    HDebug.Assert(resis4.Length == 2);
                    //if(resis4.Length == 2)
                    {
                        HDebug.Assert(resis4[0] <= resis4[1]);
                        for(int resi=resis4[0]; resi<=resis4[1]; resi++)
                        {
                            HDebug.Assert(resis.Contains(resi) == false);
                            resis.Add(resi);
                        }
                        continue;
                    }
                }
                atoms = atoms.SelectByResSeq(resis.ToArray()).ToArray();
            }

            if(removeSymbolAltloc)
                atoms = atoms.CloneByRemoveSymbolAltLoc().ToArray();

            return Pdb.FromAtoms(atoms);
        }
    }
}
