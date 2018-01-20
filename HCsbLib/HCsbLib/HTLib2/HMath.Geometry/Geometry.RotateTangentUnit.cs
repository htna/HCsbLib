using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static Vector RotateTangentUnit(Vector pt, Vector axis1, Vector axis2)
        {
            Vector pt_on_axis   = ClosestPointOnLine(axis1, axis2, pt, false);
            Vector direct_pt    = (pt - pt_on_axis).UnitVector();
            Vector direct_axis  = (axis2 - axis1).UnitVector();
            Vector direct_tanpt = LinAlg.CrossProd(direct_axis, direct_pt).UnitVector();
            if(HDebug.IsDebuggerAttached)
            {
                Vector dbg_axis = LinAlg.CrossProd(direct_pt, direct_tanpt).UnitVector();
                HDebug.AssertTolerance(0.00000001, (direct_axis - dbg_axis).Dist);
            }
            return direct_tanpt;
        }
    }
}
