using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class Vectors
	{
        public static Vectors Clone(Vector[] vectors)
        {
            Vectors result = new Vectors(vectors.Length);
            for(int i=0; i<vectors.Length; i++)
                result[i] = vectors[i].Clone();
            return result;
        }
    }
}
