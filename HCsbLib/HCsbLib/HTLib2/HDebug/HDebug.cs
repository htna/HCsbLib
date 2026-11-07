using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
    {
        public static Random rand = new Random();
        public static readonly HDebug debug = new HDebug();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//      [System.Diagnostics.Conditional("DEBUG")]
//      [System.Diagnostics.DebuggerHiddenAttribute()]
        public static bool Check(bool istrue)
        {
            if(IsDebuggerAttached == false)
                return false;
            return istrue;
        }

        [System.Diagnostics.Conditional("DEBUG")]
//      [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void SetEpsilon(IList<double> values)
        {
            for(int i=0; i<values.Count; i++)
                values[i] = double.Epsilon;
        }

        [System.Diagnostics.Conditional("DEBUG")]
//      [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void SetEpsilon(double[,] values)
        {
            for(int i0=0; i0<values.GetLength(0); i0++)
                for(int i1=0; i1<values.GetLength(1); i1++)
                    values[i0,i1] = double.Epsilon;
        }

        static bool selftest_IsEqualsAll = true;
        public static bool IsEqualsAll<T>(params T[] values)
            where T : IEquatable<T>
        {
            if(IsDebuggerAttached && selftest_IsEqualsAll)
                #region selftest
                {
                    selftest_IsEqualsAll = false;
                    HDebug.Assert(IsEqualsAll(1, 1, 1, 1, 1));
                    HDebug.Assert(IsEqualsAll(1, 1, 1, 1, 2) == false);
                    HDebug.Assert(IsEqualsAll(2, 1, 1, 1, 1) == false);
                }
                #endregion
            for(int i=1; i<values.Length; i++)
            {
                if(values[0].Equals(values[i]) == false)
                    return false;
            }
            return true;
        }

        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void Verify(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition);
        }

        static Dictionary<int, Dictionary<string, bool>> _ConditionalAssert = new Dictionary<int, Dictionary<string, bool>>();
        public bool this[string name]
        {
            get
            {
                int hash = name.GetHashCode();
                if(_ConditionalAssert.ContainsKey(hash) == false)
                    return false;
                if(_ConditionalAssert[hash].ContainsKey(name) == false)
                    return false;
                return _ConditionalAssert[hash][name];
            }
            set
            {
                int hash = name.GetHashCode();
                if(_ConditionalAssert.ContainsKey(hash) == false)
                    return;
                if(_ConditionalAssert[hash].ContainsKey(name) == false)
                    return;
                _ConditionalAssert[hash][name] = value;
            }
        }

        //static public bool IsDebuggerAttached
        //{
        //    get
        //    {
        //        return System.Diagnostics.Debugger.IsAttached;
        //    }
        //}
#if DEBUG
        public static bool IsDebuggerAttached { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return true; } }
        //public const bool IsDebuggerAttached = true;
#else
        public static bool IsDebuggerAttached { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return false; } }
        //public const bool IsDebuggerAttached = false;
#endif

        static public bool IsDebuggerAttachedWithProb(double prob)
        {
            if(System.Diagnostics.Debugger.IsAttached)
            {
                HDebug.Assert(0<=prob, prob<=1);
                double nrand = rand.NextDouble();
                return (nrand < prob);
            }
            return false;
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        static public void Break()
        {
            System.Diagnostics.Debugger.Break();
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        static public void Break(params bool[] conditions)
        {
            BreakOr(conditions);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        static public void BreakAnd(params bool[] conditions)
        {
            if(conditions.Length >= 1)
            {
                bool dobreak = true;
                foreach(bool condition in conditions)
                    dobreak = dobreak && condition;
                if(dobreak)
                    System.Diagnostics.Debugger.Break();
            }
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        static public void BreakOr(params bool[] conditions)
        {
            if(conditions.Length >= 1)
            {
                bool dobreak = false;
                foreach(bool condition in conditions)
                    dobreak = dobreak || condition;
                if(dobreak)
                    System.Diagnostics.Debugger.Break();
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        static public void ToDo(params string[] todos)
        {
            foreach(string todo in todos)
                System.Console.Error.WriteLine("TODO: " + todo);
            Break();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        static public void Depreciated(params string[] messages)
        {
            foreach(string message in messages)
                System.Console.Error.WriteLine("Depreciated: " + message);
            Break();
        }

        public static class Trace
        {
            public static bool AutoFlush          { set{ System.Diagnostics.Trace.AutoFlush   = value; } get{ return System.Diagnostics.Trace.AutoFlush  ; } }
            public static int  IndentLevel        { set{ System.Diagnostics.Trace.IndentLevel = value; } get{ return System.Diagnostics.Trace.IndentLevel; } }
            public static int  IndentSize         { set{ System.Diagnostics.Trace.IndentSize  = value; } get{ return System.Diagnostics.Trace.IndentSize ; } }
            public static System.Diagnostics.CorrelationManager      CorrelationManager {                get{ return System.Diagnostics.Trace.CorrelationManager; } }
            public static System.Diagnostics.TraceListenerCollection Listeners          {                get{ return System.Diagnostics.Trace.Listeners         ; } }

            [System.Diagnostics.Conditional("TRACE")]    public static void Flush()    { System.Diagnostics.Trace.Flush();    }
            [System.Diagnostics.Conditional("TRACE")]    public static void Indent()   { System.Diagnostics.Trace.Indent();   }
            [System.Diagnostics.Conditional("TRACE")]    public static void Unindent() { System.Diagnostics.Trace.Unindent(); }
                                                        public static void Refresh()  { System.Diagnostics.Trace.Refresh();  }

            [System.Diagnostics.Conditional("TRACE")]    public static void Write(object value)                                          { System.Diagnostics.Trace.Write(value);                              }
            [System.Diagnostics.Conditional("TRACE")]    public static void Write(string message)                                        { System.Diagnostics.Trace.Write(message);                            }
            [System.Diagnostics.Conditional("TRACE")]    public static void Write(object value, string category)                            { System.Diagnostics.Trace.Write(value, category);                    }
            [System.Diagnostics.Conditional("TRACE")]    public static void Write(string message, string category)                        { System.Diagnostics.Trace.Write(message, category);                  }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, object value)                        { System.Diagnostics.Trace.WriteIf(condition, value);                 }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, string message)                        { System.Diagnostics.Trace.WriteIf(condition, message);               }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, object value, string category)        { System.Diagnostics.Trace.WriteIf(condition, value, category);       }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, string message, string category)        { System.Diagnostics.Trace.WriteIf(condition, message, category);     }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(object value)                                        { System.Diagnostics.Trace.WriteLine(value);                          }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(string message)                                    { System.Diagnostics.Trace.WriteLine(message);                        }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(object value, string category)                        { System.Diagnostics.Trace.WriteLine(value, category);                }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(string message, string category)                    { System.Diagnostics.Trace.WriteLine(message, category);              }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, object value)                    { System.Diagnostics.Trace.WriteLineIf(condition, value);             }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, string message)                    { System.Diagnostics.Trace.WriteLineIf(condition, message);           }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, object value, string category)    { System.Diagnostics.Trace.WriteLineIf(condition, value, category);   }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, string message, string category)    { System.Diagnostics.Trace.WriteLineIf(condition, message, category); }
        }
        public static class TraceFile
        {
            static System.IO.StreamWriter writer = System.IO.File.CreateText("TRACE.TXT");
            //[System.Diagnostics.Conditional("TRACE")]    public static void Write(object value)                                          { writer.Write(value);                              writer.Flush(); }
            [System.Diagnostics.Conditional("TRACE")]    public static void Write(string message)                                        { writer.Write(message);                            writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void Write(object value, string category)                            { writer.Write(value, category);                    writer.Flush(); }
            [System.Diagnostics.Conditional("TRACE")]    public static void Write(string message, string category)                        { writer.Write(message, category);                  writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, object value)                        { writer.WriteIf(condition, value);                 writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, string message)                        { writer.WriteIf(condition, message);               writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, object value, string category)        { writer.WriteIf(condition, value, category);       writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteIf(bool condition, string message, string category)        { writer.WriteIf(condition, message, category);     writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(object value)                                        { writer.WriteLine(value);                          writer.Flush(); }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(string message)                                    { writer.WriteLine(message);                        writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(object value, string category)                        { writer.WriteLine(value, category);                writer.Flush(); }
            [System.Diagnostics.Conditional("TRACE")]    public static void WriteLine(string message, string category)                    { writer.WriteLine(message, category);              writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, object value)                    { writer.WriteLineIf(condition, value);             writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, string message)                    { writer.WriteLineIf(condition, message);           writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, object value, string category)    { writer.WriteLineIf(condition, value, category);   writer.Flush(); }
            //[System.Diagnostics.Conditional("TRACE")]    public static void WriteLineIf(bool condition, string message, string category)    { writer.WriteLineIf(condition, message, category); writer.Flush(); }
        }
    }
}
