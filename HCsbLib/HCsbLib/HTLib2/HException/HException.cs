using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    [Serializable]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(Exception))]    // [ComDefaultInterface(typeof(_Exception))]
    [ComVisible(true)]
    public class HException : Exception
    {
        int    line;
        string path;
        public HException
            ( [CallerLineNumber] int    line = 0
            , [CallerFilePath  ] string path = null
            )
        {
            this.line = line;
            this.path = path    ;
            HDebug.Assert(false);
        }
        public HException(string message
            , [CallerLineNumber] int    line = 0
            , [CallerFilePath  ] string path = null
            )
            : base(message)
        {
            this.line = line;
            this.path = path;
            HDebug.Assert(false);
        }
        protected HException(SerializationInfo info, StreamingContext context
            , [CallerLineNumber] int    line = 0
            , [CallerFilePath  ] string path = null
            )
            : base(info, context)
        {
            this.line = line;
            this.path = path;
            HDebug.Assert(false);
        }
        public HException(string message, Exception innerException
            , [CallerLineNumber] int    line = 0
            , [CallerFilePath  ] string path = null
            )
            : base(message, innerException)
        {
            this.line = line;
            this.path = path;
            HDebug.Assert(false);
        }
        public override string Message
        {
            get
            {
                string msg = base.Message;
                msg += string.Format(" [line {0} in {1}]", line, path);
                return msg;
            }
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
                {
                    if(HDebug.True)
                        throw new HException("AssertException");
                }
        }

        //[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void Exception(string message=null)
        {
            Assert(false);
            if(HDebug.True)
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
                    if(HDebug.True)
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
