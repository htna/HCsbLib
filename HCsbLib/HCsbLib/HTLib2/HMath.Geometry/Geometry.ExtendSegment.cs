using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static Vector[] ExtendSegmentBeginEnd(IList<Vector> segment, double extlength_begin, double extlength_end)
        {
            HDebug.Exception(extlength_begin>0, "must be (extlength_begin > 0)");
            HDebug.Exception(extlength_end  >0, "must be (extlength_end > 0)"  );
            Vector[] extsegment = segment.HCloneVectors().ToArray();
            extsegment = ExtendSegmentBegin(extsegment, extlength_begin);
            extsegment = ExtendSegmentEnd  (extsegment, extlength_end  );
            return extsegment;
        }
        public static Vector[] ExtendSegmentBegin(IList<Vector> segment, double extlength)
        {
            List<Vector> extsegment = segment.HCloneVectors().ToList();

            Vector vec10 = (segment[0] - segment[1]).UnitVector();
            extsegment.Insert(0, extsegment[0] + vec10*extlength);
            
            if(HDebug.IsDebuggerAttached)
            {
                Vector vec01 = (extsegment[0]-extsegment[1]).UnitVector();
                Vector vec12 = (extsegment[1]-extsegment[2]).UnitVector();
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(vec01, vec12)-1);
            }
            return extsegment.ToArray();
        }
        public static Vector[] ExtendSegmentEnd(IList<Vector> segment, double extlength)
        {
            List<Vector> extsegment = segment.HCloneVectors().ToList();

            int cnt = segment.Count;
            Vector vec89 = (segment[cnt-1] - segment[cnt-2]).UnitVector();
            extsegment.Add(extsegment[cnt-1] + vec89*extlength);

            if(HDebug.IsDebuggerAttached)
            {
                int extcnt = extsegment.Count;
                Vector extvec78 = (extsegment[extcnt-3]-extsegment[extcnt-2]).UnitVector();
                Vector extvec89 = (extsegment[extcnt-2]-extsegment[extcnt-1]).UnitVector();
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(extvec78, extvec89)-1);
            }
            return extsegment.ToArray();
        }
    }
}
