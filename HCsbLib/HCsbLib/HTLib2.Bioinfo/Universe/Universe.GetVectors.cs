using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public Vector[] GetVectorsZero()
        {
            Vector[] forces = new Vector[size];
            for(int i=0; i<forces.Length; i++)
                forces[i] = new double[3];
            return forces;
        }
        public Vector[] GetVectorsRandom()
        {
            Random rand = new Random();
            Vector[] forces = new Vector[size];
            Vector meanforce = new double[3];
            for(int i=0; i<forces.Length; i++)
            {
                double fx = rand.NextDouble();
                double fy = rand.NextDouble();
                double fz = rand.NextDouble();
                Vector force = new Vector(fx, fy, fz);
                forces[i] = force;
                meanforce += force;
            }
            meanforce /= forces.Length;
            for(int i=0; i<forces.Length; i++)
                forces[i] -= meanforce;
            return forces;
        }
    }
}
