using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace Tutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            // load a pdb file
            Pdb pdb = Pdb.FromFile("1l2y.pdb");

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
        }
    }
}
