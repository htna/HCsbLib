using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static Vector CenterOfMass(IList<Vector> coords, IList<double> masses, params int[] sele)
        {
            HDebug.Exception(coords.Count == masses.Count, "coords.Count != masses.Count");
            Vector sum = new double[3];
            double wei = 0;
            int leng = coords.Count;
            if(sele != null) foreach(int i in sele   ) { wei += masses[i]; sum += masses[i] * coords[i]; }
            else             for(int i=0; i<leng; i++) { wei += masses[i]; sum += masses[i] * coords[i]; }
            Vector cent = sum/wei;
            return cent;
        }
    }
}
