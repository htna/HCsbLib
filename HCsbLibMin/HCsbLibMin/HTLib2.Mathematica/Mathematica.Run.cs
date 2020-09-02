using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Wolfram.NETLink;

// C:\Program Files\Wolfram Research\Mathematica\7.0\SystemFiles\Links\NETLink

namespace HTLib2
{
	public partial class Mathematica
	{
        public static void Run(string scriptpath, bool waitForExit)
        {
            string currpath = HEnvironment.CurrentDirectory;
            HEnvironment.CurrentDirectory = HFile.GetFileInfo(scriptpath).Directory.FullName;
            {
                System.Diagnostics.Process mathm;
                string argument = "-script \""+HFile.GetFileInfo(scriptpath).Name+"\"";
                mathm = System.Diagnostics.Process.Start(@"C:\Program Files\Wolfram Research\Mathematica\8.0\math.exe", argument);
                if(waitForExit)
                    mathm.WaitForExit();
            }
            HEnvironment.CurrentDirectory = currpath;
        }
	}
}
