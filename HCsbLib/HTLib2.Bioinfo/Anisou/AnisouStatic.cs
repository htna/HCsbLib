using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class AnisouStatic
    {
        public static MatrixByArr[] GetUs(this IList<Anisou> anisous, double scale=1)
        {
            MatrixByArr[] Us = new MatrixByArr[anisous.Count];
            for(int i=0; i<Us.Length; i++)
                Us[i] = anisous[i].U.CloneT() * scale;
            return Us;
        }
    }
}
