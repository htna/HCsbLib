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
        public static Vector[] LoadCoord(string coordpath)
        {
            List<Vector> coords = new List<Vector>();

            int i = 0;
            Vector iatom_coord = null;
            foreach(string line in HFile.ReadLines(coordpath))
            {
                int iatom  = i / 3;
                int icoord = i % 3;
                i++;

                double value = double.Parse(line);
                if(icoord == 0)
                {
                    HDebug.Assert(coords.Count == iatom);
                    iatom_coord = new double[3];
                    coords.Add(iatom_coord);
                }

                iatom_coord[icoord] = value;
                HDebug.Assert(coords[iatom][icoord] == iatom_coord[icoord]);
            }

            return coords.ToArray();
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

            //////////////////////////////////////////////////////////////////////
            // read hessian matrix
            string hesspath = args[0];
            if(HFile.Exists(hesspath))
            {
                System.Console.WriteLine("Cannot find hessian file: "+hesspath);
                return;
            }
            HessMatrix hess = LoadHess(hesspath);

            //////////////////////////////////////////////////////////////////////
            // read force vector
            string forcpath = args[1];
            if(HFile.Exists(forcpath))
            {
                System.Console.WriteLine("Cannot find force file: "+forcpath);
                return;
            }
            Vector[] forc = LoadForce(forcpath);

            //////////////////////////////////////////////////////////////////////
            // read xyz file (atom types and atom coordinates)
            string xyzpath = args[1];
            if(HFile.Exists(xyzpath))
            {
                System.Console.WriteLine("Cannot find tinker xyz file: "+xyzpath);
                return;
            }
            Tinker.Xyz xyz = Tinker.Xyz.FromFile(xyzpath, false);

            //////////////////////////////////////////////////////////////////////
            // threshold to make a non-zero block as a zero block
            double thres_zeroblk = 1.0E-07; // = 1.0E-07 (default)
            {
                string optionname = "ThresZeroBlock:";
                string[] sels = args.HSelectStartsWith(optionname);
                if(sels.Length >= 1)
                {
                    HDebug.Assert(sels.Length == 1);
                    string str = sels[0].Substring(optionname.Length);
                    double val;
                    if(double.TryParse(str, out val))
                        thres_zeroblk = val;
                }
            }

            //////////////////////////////////////////////////////////////////////
            // iterative coarse-graining block size to project waters into the protein
            int box_size = 20;// = 20 A (default);
            {
                string optionname = "CoarseBlockSize:";
                string[] sels = args.HSelectStartsWith(optionname);
                if(sels.Length >= 1)
                {
                    HDebug.Assert(sels.Length == 1);
                    string str = sels[0].Substring(optionname.Length);
                    int    val;
                    if(int.TryParse(str, out val))
                        box_size = val;
                }
            }
            //////////////////////////////////////////////////////////////////////
            // function that determines
            // ( protein atom indices
            // , list of water atom indices to project into protein iteratively)
            Tuple<int[], int[][]> GetIdxKeepListRemv(object[] atoms, Vector[] coords)
            {
                return HessForc.Coarse.GetIdxKeepListRemv_RemoveHOH
                    (atoms
                    , coords
                    , box_size: box_size
                    );
            };

            //////////////////////////////////////////////////////////////////////
            // coarse-graining option
            string[] coarse_options = new string[] { "pinv(D)" };

            //////////////////////////////////////////////////////////////////////
            // Hess/Force info class
            HessForc.Coarse.HessForcInfo hessforcinfo_prot_solv = new HessForc.Coarse.HessForcInfo
            {
                atoms  = xyz.atoms,
                mass   = null,
                coords = xyz.atoms.HListCoords(),
                hess   = hess,
                forc   = forc,
            };

            HessForc.Coarse.HessForcInfo hessforcinfo_prot_exsolv;
            hessforcinfo_prot_exsolv = HessForc.Coarse.GetCoarseHessForc
            (hessforcinfo_prot_solv
            , coords: hessforcinfo_prot_solv.coords
            , GetIdxKeepListRemv: GetIdxKeepListRemv
            , ila: null
            , thres_zeroblk: thres_zeroblk      // double.Epsilon //1.0E-07
            , options: coarse_options           // new string[] { "/D -> pinv(D)" } // new string[] { "pinv(D)" }
            );

        }
    }
}
