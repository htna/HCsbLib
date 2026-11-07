using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace HTLib2
{
    using static HTLib2.Geometry;
    using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
    {
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertException
            ( bool condition
            , string message = null
            , [CallerLineNumber] int    line = 0
            , [CallerFilePath  ] string path = null
            )
        {
            if(condition == true)
                return;

            System.Diagnostics.Debug.Assert(false);
            throw new HException(message, line, path);
        }
    }
}
