using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HCsbLib
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string pdbpath = @"C:\1l2y.pdb";
            Tutorial_Pdb.Main(pdbpath, args);
        }
    }
}
