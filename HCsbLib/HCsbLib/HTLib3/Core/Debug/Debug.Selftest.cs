using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib3
{
    using StackTrace = System.Diagnostics.StackTrace;

    public partial class Debug
	{
        static HashSet<Func<bool>> SelftestFuncs = new HashSet<Func<bool>>();
		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void Selftest(Func<bool> SelftestFunc)
		{
            if(SelftestFuncs.Contains(SelftestFunc))
                return;
            SelftestFuncs.Add(SelftestFunc);
            Assert(SelftestFunc());
		}

        static HashSet<object> setSelftestDone = new HashSet<object>();
        public static bool SelftestDo()
        {
            if(Debug.IsDebuggerAttached == false)
                return false;
            StackTrace stackTrace = new StackTrace();
            var frame1 = stackTrace.GetFrame(1);
            var method = frame1.GetMethod();
            bool selftest_do = SelftestDo(method);
            return selftest_do;
        }
        private static bool SelftestDo(object func)
        {
            if(Debug.IsDebuggerAttached == false)
                return false;
            if(setSelftestDone.Contains(func))
                return false;
            setSelftestDone.Add(func);
            return true;
        }
	}
}
