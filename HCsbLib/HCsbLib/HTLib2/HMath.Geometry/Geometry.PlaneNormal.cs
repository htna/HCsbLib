using System;
using System.Collections.Generic;
using System.Text;

// References
// - http://www.geometryalgorithms.com
// - http://www.gamedev.net/community/forums/topic.asp?topic_id=444154


namespace HTLib2
{
    public partial class Geometry
    {
        public static Vector PlaneNormal(Vector pt1, Vector pt2, Vector pt3)
        {
            Vector a = pt2 - pt1;
            Vector b = pt3 - pt1;
            Vector n = LinAlg.CrossProd(a, b);
            return n.UnitVector();
        }
    }
}
