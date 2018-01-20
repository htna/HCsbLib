/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo.External
{
    public partial class Gromacs
    {
        public partial class Topol
        {
            static bool selftest = Debug.IsDebuggerAttached;
            public static void SelfTest(string rootpath, string[] args)
            {
                if(selftest == false)
                    return;
                selftest = false;

                string filepath = rootpath + @"\Bioinfo\External.Gromacs\Selftest\topol.top";
                Topol topol = FromFile(filepath);
            }
        }
    }
}
*/