using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class Anisou : ICloneable
    {
        public MatrixByArr U;
        public Vector[] eigvecs; // eigvecs of U
        public double[] eigvals; // eigvals of U
        public Vector[] axes { get { return eigvecs; } }
        public double[] rads
        {
            get
            {
                double[] rads = new double[3];
                for(int i=0; i<3; i++)
                    rads[i] = Math.Sign(eigvals[i])*Math.Sqrt(Math.Abs(eigvals[i]));
                return rads;
            }
        }

        public double bfactor { get { double bfactor = (U[0, 0] + U[1, 1] + U[2, 2]); return bfactor; } }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        public Anisou Clone()
        {
            Anisou anis  = new Anisou();
            anis.U       = this.U;
            anis.eigvecs = this.eigvecs;
            anis.eigvals = this.eigvals;
            return anis;
        }
    }
}
