using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class ModeStatic
    {
        public static void ToPdbFileAnimated
            ( this Mode mode
            , string filepath
            , Pdb pdb
            , IList<double> stepTimes = null
            , double scale = 1
            , int? modelidx = null
            )
        {
            Pdb.ToFileAnimated
                ( filepath
                , pdb.atoms
                , mode.GetEigvecsOfAtoms(scale)
                , stepTimes
                , null
                , null
                , null
                , modelidx
                );
        }
    }
}
