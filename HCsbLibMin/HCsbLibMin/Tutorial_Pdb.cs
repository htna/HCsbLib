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
        public class Tutorial_Pdb
        {
            public static void Main(string pathbase, string pdbbase, string[] args)
            {
                // load a pdb file
                Pdb pdb = Pdb.FromFile(pdbbase+"1l2y.pdb");

                // get coordinates
                List<Vector> coords = new List<Vector>();
                foreach(var atom in pdb.atoms)
                {
                    if(atom.name.Trim() == "CA")
                        coords.Add(atom.coord);
                    else
                        coords.Add(atom.coord);
                }

                // update coordinates
                Vector move = new double[] { 1, 2, 3 };
                for(int i=0; i<coords.Count; i++)
                {
                    coords[i] += move;
                }

                // save a new pdb file with the new coordinates
                Pdb npdb = pdb.CloneUpdateCoord(coords);
                npdb.ToFile("1l2y_moved.pdb");


                // initialize matlab cache/temporary directory
                Matlab.Register(@"C:\temp\");

                // send a matrix to matlab
                // console-mode matlab will be automatically launched
                Matrix mat = new double[,] { { 1, 2 }, { 2, 1 } };
                Matlab.PutMatrix("mat", mat);

                // calculate eigenvalues and eigenvectors
                Matlab.Execute("[V,D] = eig(mat);");
                Matlab.Execute("D = diag(D);");

                // get eigenvalues and eigenvectors
                Matrix eigvecs = Matlab.GetMatrix("V");
                Vector eigvals = Matlab.GetVector("D");

                // clear values in matlab
                Matlab.Execute("clear;");
            }
        }
    }
}
