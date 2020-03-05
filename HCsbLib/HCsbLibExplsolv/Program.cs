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

        public static void SaveHess(string hesspath, HessMatrix hess)
        {
            int colVecSize = hess.ColSize;
            int rowVecSize = hess.ColSize;
            HDebug.Assert(colVecSize == rowVecSize);

            string idxFormat;
            {
                int digit = 0;
                int size = Math.Max(colVecSize, rowVecSize);
                while(size > 0)
                {
                    digit ++;
                    size /= 10;
                }
                idxFormat = "{0,"+digit+"}";
                //idxFormat = string.Format(idxFormat, 125);
            }

            List<string> lines = new List<string>();
            for(int c=0; c<colVecSize; c++)
            {
                for(int r=c; r<rowVecSize; r++)
                {
                    double v = hess[c,r];
                    string line
                        = " "
                        + string.Format(idxFormat, c)
                        + " "
                        + string.Format(idxFormat, r)
                        + " "
                        + string.Format("{0,16:0.0000000000}", v)
                        ;
                    lines.Add(line);
                }
            }

            HFile.WriteAllLines(hesspath, lines);
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
            if(args.Length < 4)
            {
                PrintUsage();
                return;
            }

            string inPathHess  = args[0];
            string inPathForc  = args[1];
            string inPathXyz   = args[2];
            string outPathHess = args[3];
            string outPathForc = args[4];
            string outPathXyz  = args[5];

            //////////////////////////////////////////////////////////////////////
            // read hessian matrix
            if(HFile.Exists(inPathHess))
            {
                System.Console.WriteLine("Cannot find hessian file: "+inPathHess);
                return;
            }
            HessMatrix hess = LoadHess(inPathHess);

            //////////////////////////////////////////////////////////////////////
            // read force vector
            if(HFile.Exists(inPathForc))
            {
                System.Console.WriteLine("Cannot find force file: "+inPathForc);
                return;
            }
            Vector[] forc = LoadForce(inPathForc);

            //////////////////////////////////////////////////////////////////////
            // read xyz file (atom types and atom coordinates)
            if(HFile.Exists(inPathXyz))
            {
                System.Console.WriteLine("Cannot find tinker xyz file: "+inPathXyz);
                return;
            }
            Tinker.Xyz xyz = Tinker.Xyz.FromFile(inPathXyz, false);

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

            //////////////////////////////////////////////////////////////////////
            // coarse-graining Hess/Force
            HessForc.Coarse.HessForcInfo hessforcinfo_prot_exsolv;
            hessforcinfo_prot_exsolv = HessForc.Coarse.GetCoarseHessForc
            (hessforcinfo_prot_solv
            , coords: hessforcinfo_prot_solv.coords
            , GetIdxKeepListRemv: GetIdxKeepListRemv
            , ila: null
            , thres_zeroblk: thres_zeroblk      // double.Epsilon //1.0E-07
            , options: coarse_options           // new string[] { "/D -> pinv(D)" } // new string[] { "pinv(D)" }
            );


            SaveHess(outPathHess, hessforcinfo_prot_exsolv.hess);
        }
    }
}
