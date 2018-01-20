using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public MinimizeTNMImpl MinimizeTNM
        {
            get
            {
                return new MinimizeTNMImpl(this);
            }
        }
        public partial class MinimizeTNMImpl
        {
            Universe univ;
            public MinimizeTNMImpl(Universe univ)
            {
                this.univ = univ;
            }

            int size { get { return univ.size; } }
        }
    }
}
