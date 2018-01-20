using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static Matrix GetHessStem(IList<Vector> coords)
        {
            return STeM.GetHessCa(coords);
        }
    }
}
