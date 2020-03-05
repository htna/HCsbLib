using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HCsbLibExplsolv
{
    class Program
    {
        public static HessMatrix LoadHess(string hesspath)
        {
            HessMatrix hess = null;

            int numlines = 0;
            foreach(string line in HFile.ReadLines(hesspath))
            {
                numlines ++;
                if(numlines == 1)
                {
                    string[] tokens = line.Split().HRemoveAll("");
                    HDebug.Assert(tokens.Length == 1);
                    int size = int.Parse(tokens[0]);
                    HDebug.Assert(size%3 == 0);
                    hess = HessMatrixSparse.ZerosSparse(size, size);
                }
                else
                {
                    string[] tokens = line.Split().HRemoveAll("");
                    if(tokens.Length == 0)
                        continue;
                    HDebug.Assert(tokens.Length == 3);
                    int    i = int   .Parse(tokens[0]);
                    int    j = int   .Parse(tokens[1]);
                    double v = double.Parse(tokens[2]);
                    hess[i, j] = v;
                    hess[j, i] = v;
                }
            }

            return hess;
        }
        public static Vector[] LoadForce(string forcpath)
        {
            List<Vector> forc = new List<Vector>();

            int i = 0;
            Vector iatom_forc = null;
            foreach(string line in HFile.ReadLines(forcpath))
            {
                int iatom  = i / 3;
                int icoord = i % 3;
                i++;

                double value = double.Parse(line);
                if(icoord == 0)
                {
                    HDebug.Assert(forc.Count == iatom);
                    iatom_forc = new double[3];
                    forc.Add(iatom_forc);
                }

                iatom_forc[icoord] = value;
                HDebug.Assert(forc[iatom][icoord] == iatom_forc[icoord]);
            }

            return forc.ToArray();
        }
        public static void PrintUsage()
        {
            System.Console.WriteLine("HCsbLibExplsolv.exe   hessian-path  force-path  [options]");
            System.Console.WriteLine("    hessian-path: path of a file containing the Hessian matrix");
            System.Console.WriteLine("    force-path  : path of a file containing the force vector");
        }
        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                PrintUsage();
                return;
            }

            string hesspath = args[0];
            if(HFile.Exists(hesspath))
            {
                System.Console.WriteLine("Cannot find hessian-path: "+hesspath);
                return;
            }

            string forcpath = args[1];
            if(HFile.Exists(forcpath))
            {
                System.Console.WriteLine("Cannot find force-path: "+forcpath);
                return;
            }


        }
    }
}
