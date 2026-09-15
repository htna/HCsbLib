using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
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
