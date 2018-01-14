using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public static void ToFileAnimated( string filepath
                                         , IList<Atom> atoms
                                         , IList<Vector> velocities
                                         , IList<double> stepTimes = null
                                         , IList<Vector> coords = null
                                         , IList<double> bfactors = null
                                         , IList<Element> headers = null
                                         )
        {
            if(coords == null)
                coords = atoms.ListCoord();
            if(stepTimes == null)
                stepTimes = new double[]{   0.0, -0.1, -0.2, -0.3, -0.4, -0.5, -0.6, -0.7, -0.8, -0.9, -1.0,
                                           -1.0, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1,  0.0,
                                            0.0,  0.1,  0.2,  0.3,  0.4,  0.5,  0.6,  0.7,  0.8,  0.9,  1.0,
                                            1.0,  0.9,  0.8,  0.7,  0.6,  0.5,  0.4,  0.3,  0.2,  0.1,  0.0,
                                       };

            bool append = false;
            for(int i=0; i<stepTimes.Count; i++)
            {
                List<Vector> lcoords = coords.PwAdd(velocities, stepTimes[i]);
                List<string> lheaders = null;
                if((append == false) && (headers != null))
                    lheaders = headers.ToLines();
                Pdb.ToFile(filepath, atoms, coords: lcoords, append: append, headers: lheaders, bfactors: bfactors);
                append = true;
            }
        }
    }
}
