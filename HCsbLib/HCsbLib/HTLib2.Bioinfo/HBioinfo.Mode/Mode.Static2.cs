using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class ModeStatic
    {
        public static int Size(this IList<Mode> modes)
        {
            int? size = null;
            foreach(Mode mode in modes)
            {
                if(mode == null) continue;
                if(size == null) size = mode.size;
                else HDebug.Assert(size == mode.size);
            }
            HDebug.Assert(size != null);
            return size.Value;
        }
    }
}
