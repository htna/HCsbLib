using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public Vector GetMasses(int dim=1)
        {
            Vector masses = atoms.ToArray().GetMasses(dim);

            if(HDebug.IsDebuggerAttached)
            {
                double[] tmasses = new double[size*dim];
                for(int i=0; i<size; i++)
                    for(int j=0; j<dim; j++)
                        tmasses[i*dim+j] = atoms[i].Mass;
                HDebug.AssertTolerance(0, masses - tmasses);
            }
            return masses;
        }
        public Matrix GetMassMatrix(int dim=1)
        {
            Matrix masses = atoms.ToArray().GetMassMatrix(dim);

            if(HDebug.IsDebuggerAttached)
            {
                Matrix tmasses = new double[size*dim, size*dim];
                for(int i=0; i<size; i++)
                    for(int j=0; j<dim; j++)
                        tmasses[i*dim+j, i*dim+j] = atoms[i].Mass;
                HDebug.AssertToleranceMatrix(0, masses - tmasses);
            }
            return masses;
        }
    }
}
