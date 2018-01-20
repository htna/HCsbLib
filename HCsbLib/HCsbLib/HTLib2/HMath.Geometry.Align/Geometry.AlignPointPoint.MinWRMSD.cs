using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public partial class AlignPointPoint
        {
            public partial class MinRMSD
            {
                public static Trans3 GetTrans( IList<Vector> source
                                             , IList<Vector> target
                                             , IList<double> weight
                                             )
                {
                    Trans3 trans = ICP3.OptimalTransformWeighted(source, target, weight);
                    return trans;
                }
            }
        }
	}
}
