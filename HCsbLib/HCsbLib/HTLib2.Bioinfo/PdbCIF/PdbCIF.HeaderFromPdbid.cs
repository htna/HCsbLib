using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2.Bioinfo
{
	public partial class PdbCIF
	{
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
                lines = HFile.ReadAllLines(stream);
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
