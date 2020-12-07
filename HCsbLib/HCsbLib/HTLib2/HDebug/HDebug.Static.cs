using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
	{
#if DEBUG
        public static bool True  { get { return true ; } }
        public static bool False { get { return false; } }
#else
        public const bool True  = true ;
        public const bool False = false;
#endif
    }
}
