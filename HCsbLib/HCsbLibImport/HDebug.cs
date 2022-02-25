using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace HCsbLibImport
{
    using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
    {
        public static readonly HDebug debug = new HDebug();

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void Assert(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition);
        }

        //static public bool IsDebuggerAttached
        //{
        //    get
        //    {
        //        return System.Diagnostics.Debugger.IsAttached;
        //    }
        //}
#if DEBUG
        public const bool IsDebuggerAttached = true;
#else
        public const bool IsDebuggerAttached = false;
#endif
    }
}
