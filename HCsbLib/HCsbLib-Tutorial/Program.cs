using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace Tutorial
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string pathbase = Environment.CurrentDirectory + @"\test\";
            HDirectory.CreateDirectory(pathbase);

            Tutorial_Mathematica.Main(args);
            Tutorial_Main0.Main(args);
        }
    }
}
