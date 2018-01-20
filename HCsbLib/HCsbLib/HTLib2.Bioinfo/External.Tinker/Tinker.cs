using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        static string tinkerpath =
            // @"C:\Program Files\tinker-6.2.01\bin-win64\";
            @"C:\Program Files\Tinker\bin-win64-8.2.1\";

        public static void SetTinkerPath(string tinkerpath)
        {
            Tinker.tinkerpath = tinkerpath;
        }
    }
}
