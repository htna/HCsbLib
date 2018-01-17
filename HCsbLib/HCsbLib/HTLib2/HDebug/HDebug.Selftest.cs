using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    using StackTrace = System.Diagnostics.StackTrace;

    public partial class HDebug
	{
        //static HashSet<Func<bool>> SelftestFuncs = new HashSet<Func<bool>>();
		//[System.Diagnostics.Conditional("DEBUG")]
		//[System.Diagnostics.DebuggerHiddenAttribute()]
        //public static void SelftestRun(Func<bool> SelftestFunc)
		//{
        //    if(SelftestFuncs.Contains(SelftestFunc))
        //        return;
        //    SelftestFuncs.Add(SelftestFunc);
        //    Assert(SelftestFunc());
		//}

        static HashSet<object> setSelftestDone = new HashSet<object>();
        public static bool Selftest()
        {
            if(HDebug.IsDebuggerAttached == false)
                return false;
            StackTrace stackTrace = new StackTrace();
            var frame1 = stackTrace.GetFrame(1);
            var method = frame1.GetMethod();
            bool selftest_do = Selftest(method);
            return selftest_do;
        }
        public static bool Selftest(object func)
        {
            if(HDebug.IsDebuggerAttached == false)
                return false;
            if(setSelftestDone.Contains(func))
                return false;
            setSelftestDone.Add(func);
            return true;
        }
	}
}
