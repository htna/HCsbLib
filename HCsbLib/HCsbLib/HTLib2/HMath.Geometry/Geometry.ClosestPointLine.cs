using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static Vector ClosestPointOnLine(Vector A, Vector B, Vector P, bool segmentClamp)
        {
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=444154
            HDebug.AssertAnd(A.Size == B.Size, B.Size == P.Size);
            Vector AP = P - A;
            Vector AB = B - A;
            double ab2 = LinAlg.VtV(AB, AB);// AB.x*AB.x + AB.y*AB.y;
            if(ab2 == 0)
                return A.Clone();
            double ap_ab = LinAlg.VtV(AP, AB); //AP.x*AB.x + AP.y*AB.y;
            double t0 = ap_ab / ab2;
            double t = t0;
            if (segmentClamp)
            {
                if (t < 0.0) t = 0.0;
                else if (t > 1.0) t = 1.0;
            }
            Vector Closest = A + AB * t;
            HDebug.AssertTolerance(0.000000001, LinAlg.VtV(B-A, P-(A + AB * t0)));
            return Closest;
        }
        public class PClosestPointOnSegment
        {
            public Vector point;
            public double dist;
            //public Tuple<Vector,Vector> segment;
            public int iA { get { return iAB  ; } }
            public int iB { get { return iAB+1; } }
            public int iAB;
            public double GetDistPointSeg0(IList<Vector> ABs)
            {
                double accdist = 0;
                int lastB = 0;
                for(int i=0; i<iA; i++)
                {
                    accdist += (ABs[i] - ABs[i+1]).Dist;
                    lastB = i+1;
                }
                HDebug.Assert(lastB == iA);
                accdist += (ABs[iA] - point).Dist;
                return accdist;
            }
        }
        public static PClosestPointOnSegment ClosestPointOnSegment(IList<Vector> ABs, Vector Pt)
        {
            HDebug.Assert(ABs.Count >= 2);
            Vector closest       = ABs[0];
            //Tuple<Vector,Vector> closest_segment = new Tuple<Vector,Vector>(ABs[0], ABs[1]);
            double closest_dist2 = (closest-Pt).Dist2;
            double closest_dist  = Math.Sqrt(closest_dist2);
            int    closest_iAB   = 0;
            for(int i=1; i<ABs.Count; i++)
            {
                int iAB = i-1;
                Vector A = ABs[iAB  ];
                Vector B = ABs[iAB+1];
                Vector C = ClosestPointOnLine(A, B, Pt, true);
                double C_dist2 = (C-Pt).Dist2;
                if(C_dist2 < closest_dist2)
                {
                    closest       = C;
                    closest_dist2 = C_dist2;
                    closest_dist  = Math.Sqrt(C_dist2);
                    closest_iAB   = iAB;
                    //closest_segment = new Tuple<Vector, Vector>(A, B);
                }
            }
            HDebug.AssertTolerance(0.00000001, closest_dist - DistPointSegment(Pt, ABs));
            return new PClosestPointOnSegment
            {
                point   = closest,
                dist    = closest_dist,
                iAB     = closest_iAB,
                //segment = closest_segment,
            };
        }
        public static Vector[] ClosestPointOnLine(IList<Vector> ABs, IList<Vector> Ps)
        {
            HDebug.Assert(false);
            //Debug.AssertAnd(As.Count == Bs.Count, Bs.Count == Ps.Count);
            //Vector[] closests = new Vector[As.Count];
            //for(int i=0; i<As.Count; i++)
            //    closests[i] = ClosestPointOnLine(As[i], Bs[i], Ps[i], segmentClamp);
            //return closests;
            return null;
        }
        public static Vector[] ClosestPointOnLine(IList<Vector> As, IList<Vector> Bs, IList<Vector> Ps, bool segmentClamp)
        {
            HDebug.AssertAnd(As.Count == Bs.Count, Bs.Count == Ps.Count);
            Vector[] closests = new Vector[As.Count];
            for(int i=0; i<As.Count; i++)
                closests[i] = ClosestPointOnLine(As[i], Bs[i], Ps[i], segmentClamp);
            return closests;
        }
        public static List<Vector> ClosestPointOnLine2(IList<Vector> LineBase, IList<Vector> LineDirect, IList<Vector> Points, bool segmentClamp)
        {
            HDebug.AssertAnd(LineBase.Count == LineDirect.Count, LineDirect.Count == Points.Count);
            List<Vector> closests = new List<Vector>(LineBase.Count);
            for(int i=0; i<LineBase.Count; i++)
                closests.Add(ClosestPointOnLine(LineBase[i], LineBase[i]+LineDirect[i], Points[i], segmentClamp));
            HDebug.Assert(closests.Count == LineBase.Count);
            return closests;
        }
        //public static DoubleVector3[] ClosestPointOnLine(IList<DoubleVector3> As, IList<DoubleVector3> Bs, IList<DoubleVector3> Ps, bool segmentClamp)
        //{
        //    Debug.AssertAnd(As.Count == Bs.Count, Bs.Count == Ps.Count);
        //    DoubleVector3[] closests = new DoubleVector3[As.Count];
        //    for(int i=0; i<As.Count; i++)
        //        closests[i] = (DoubleVector3)ClosestPointOnLine(As[i].ToArray(), Bs[i].ToArray(), Ps[i].ToArray(), segmentClamp);
        //    return closests;
        //}
	}
}
