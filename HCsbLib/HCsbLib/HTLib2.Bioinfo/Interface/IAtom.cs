using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public interface IAtom
    {
        double x { get; }
        double y { get; }
        double z { get; }
        double[] coord { get; }

        string  type { get; }
        double? pch  { get; }
        double? rmin2{ get; }
        double? eps  { get; }
    }
}
