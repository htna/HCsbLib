using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Pymol
	{
        //File.AppendAllLines(pmlpath, "quit");
        //string curdirectory = System.Environment.CurrentDirectory;
        //string pmldirectory = pmlpath.Substring(0, pmlpath.LastIndexOf('\\')+1);
        //System.Environment.CurrentDirectory = pmldirectory;
        //System.Diagnostics.Process pymol = System.Diagnostics.Process.Start(@"C:\Program Files (x86)\PyMOL\PyMOL\PymolWin.exe ", "\""+pmlpath+"\"");
        //pymol.WaitForExit();
        //System.Environment.CurrentDirectory = curdirectory;
        //File.WriteAllLines(pmlpath, lines);
        public static void Run(params string[] args)
        {
            System.Diagnostics.Process pymol;
            if(args.Length == 0)
            {
                pymol = System.Diagnostics.Process.Start(@"C:\Program Files (x86)\PyMOL\PyMOL\PymolWin.exe");
            }
            else
            {
                string arguments = "";
                for(int i=0; i<args.Length; i++)
                    arguments = arguments + "\"" + args[i] + "\", ";
                arguments = arguments.Substring(0, arguments.Length-2);

                pymol = System.Diagnostics.Process.Start(@"C:\Program Files (x86)\PyMOL\PyMOL\PymolWin.exe", arguments);
            }
            pymol.WaitForExit();
        }
        public static void SavePse( string psepath
                                  , bool cartoon
                                  , bool line
                                  , string pngpath // null if not save png
                                  , params string[] pdbpaths)
        {
            string currpath = HEnvironment.CurrentDirectory;
            HEnvironment.CurrentDirectory = HFile.GetFileInfo(psepath).Directory.FullName;
            {
                Random rand = new Random();
                string psename = HFile.GetFileInfo(psepath).Name;

                int count = pdbpaths.Length;
                string[] pdblnames = new string[count];
                string[] pdblpaths = new string[count];
                for(int i=0; i<count; i++)
                {
                    var info = HFile.GetFileInfo(pdbpaths[i]);
                    pdblnames[i] = info.Name.Replace(".pdb", "").Replace(".PDB","");
                    pdblpaths[i] = psepath+info.Name+rand.NextInt(99999).ToString(".rnd00000")+".pdb";
                    HFile.Copy(pdbpaths[i], pdblpaths[i]);
                }

                string pmlpath = psepath+rand.NextInt(99999).ToString(".rnd00000")+".pml";
                List<string> pmltext = new List<string>();
                for(int i=0; i<count; i++)
                    pmltext.Add("load $$path$$, $$name$$;".Replace("$$path$$", pdblpaths[i]).Replace("$$name$$",pdblnames[i]));
                if(cartoon) pmltext.Add("show cartoon;"); else pmltext.Add("hide cartoon");
                if(line) pmltext.Add("show lines;"); else pmltext.Add("hide lines");
                pmltext.Add("reset;");
                pmltext.Add("save $$psepath$$;".Replace("$$psepath$$", psepath));
                if(pngpath != null) pmltext.Add("png $$pngpath$$;".Replace("$$pngpath$$", pngpath));
                pmltext.Add("quit;");

                HFile.WriteAllLines(pmlpath, pmltext);
            
                System.Diagnostics.Process pymol;
                pymol = System.Diagnostics.Process.Start(@"C:\Program Files (x86)\PyMOL\PyMOL\PymolWin.exe", pmlpath);
                pymol.WaitForExit();

                for(int i=0; i<count; i++)
                    HFile.Delete(pdblpaths[i]);
                HFile.Delete(pmlpath);
            }
            HEnvironment.CurrentDirectory = currpath;
        }
    }
}
