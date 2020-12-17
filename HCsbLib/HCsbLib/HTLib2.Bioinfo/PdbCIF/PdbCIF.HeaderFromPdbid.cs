using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2.Bioinfo
{
	public partial class PdbCIF
	{
        public static string[] HeaderFromPdbid
            ( string pdbid
            , string cachebase  // null or @"K:\PdbUnzippeds\$PDBID$.pdb"
            , bool? download    // null or false
            )
        {
            if(cachebase == null) cachebase = @"D:\ProtDataBank\header\$PDBID$.cif";
            if(download  == null) download  = false;

            string cifpath = cachebase.Replace("$PDBID$", pdbid);

            if(HFile.Exists(cifpath))
            {
                return HFile.ReadAllLines(cifpath);
            }
            else if(download.Value)
            {
                string[] lines = HeaderFromPdbid(pdbid);
                if(lines != null)
                {
                    HFile.WriteAllLines(cifpath, lines);
                    return lines;
                }
            }
            return null;
        }
        public static string[] HeaderFromPdbid(string pdbid)
		{
            //Pdb pdb = FromFile(cachepath);
            //webClient.DownloadFile("http://mysite.com/myfile.txt", @"c:\myfile.txt");

            string address = string.Format(@"https://files.rcsb.org/header/{0}.cif", pdbid.ToUpper());
            System.Net.WebClient webClient = new System.Net.WebClient();
            string[] lines;
            try
            {
                Stream stream = webClient.OpenRead(address);
                {
                    StreamReader streamreader = new StreamReader(stream);
                    List<string> _lines = new List<string>();
                    while(true)
                    {
                        string line = streamreader.ReadLine();
                        if(line == null)
                            break;
                        _lines.Add(line);
                    }
                    lines = _lines.ToArray();
                }
                stream.Close();
            }
            catch(Exception e)
            {
                System.Console.Error.WriteLine("error while downloading {0}", pdbid);
                System.Console.Error.WriteLine(e.Message);
                System.Console.Error.WriteLine(e);
                lines = null;
            }
            webClient.Dispose();
            return lines;
        }
    }
}
