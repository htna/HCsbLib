using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class HLib2Static
    {
        public static HDynamic HToHDynamic(this object obj)
        {
            return new HDynamic(obj);
        }
    }
}
