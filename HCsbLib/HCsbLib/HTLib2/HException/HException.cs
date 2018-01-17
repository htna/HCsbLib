using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace HTLib2
{
    [Serializable]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(_Exception))]
    [ComVisible(true)]
    public class HException : Exception
    {
        public HException()
        {
            HDebug.Assert(false);
        }
        public HException(string message)
            : base(message)
        {
            HDebug.Assert(false);
        }
        protected HException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            HDebug.Assert(false);
        }
        public HException(string message, Exception innerException)
            : base(message, innerException)
        {
            HDebug.Assert(false);
        }
    }
    public partial class HDebug
	{
        [System.Diagnostics.Conditional("DEBUG")]
        //[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertException(params bool[] conditions)
        {
            Depreciated("call HDebug.Exception()");
            AssertAnd(conditions);
            foreach(bool cond in conditions)
                if(cond == false)
                    throw new HException("AssertException");
        }

        //[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void Exception(string message=null)
        {
            Assert(false);
            throw new HException(message);
        }
        public static void Exception(Exception exception)
        {
            throw exception;
        }
        //[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void Exception(string message, params bool[] conds)
        {
            foreach(bool cond in conds)
            {
                if(cond == false)
                {
                    Assert(false);
                    throw new HException(message);
                }
            }
        }
        public static void Exception(bool cond1,                                                 string message=null) { Exception(message, cond1                              ); }
        public static void Exception(bool cond1, bool cond2,                                     string message=null) { Exception(message, cond1, cond2                       ); }
        public static void Exception(bool cond1, bool cond2, bool cond3,                         string message=null) { Exception(message, cond1, cond2, cond3                ); }
        public static void Exception(bool cond1, bool cond2, bool cond3, bool cond4,             string message = null) { Exception(message, cond1, cond2, cond3, cond4       ); }
        public static void Exception(bool cond1, bool cond2, bool cond3, bool cond4, bool cond5, string message = null) { Exception(message, cond1, cond2, cond3, cond4, cond5); }
    }
}
