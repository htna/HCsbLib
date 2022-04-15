using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
	{
#if DEBUG
        public static bool True  { get { return _True ; } }  static bool _True  = true ;
        public static bool False { get { return _False; } }  static bool _False = false;
#else
        public const bool True  = true ;
        public const bool False = false;
#endif
    }
}
