using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib3
{
	using DEBUG = System.Diagnostics.Debug;

    public partial class Debug
	{
		public static Random rand = new Random();
		public static readonly Debug debug = new Debug();

		[System.Diagnostics.Conditional("DEBUG")]
//		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void InitValues<T>(IList<T> values, T initvalue)
		{
            for(int i=0; i<values.Count; i++)
                values[i] = initvalue;
		}

		[System.Diagnostics.Conditional("DEBUG")]
//		[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void InitValues<T>(T[,] values, T initvalue)
		{
            for(int i0=0; i0<values.GetLength(0); i0++)
                for(int i1=0; i1<values.GetLength(1); i1++)
                    values[i0, i1] = initvalue;
		}

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void Assert(params bool[] conditions)
		{
			AssertAnd(conditions);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void AssertTolerant(double tolerance, params double[] values)
		{
            for(int i=0; i<values.Length; i++)
                if(Math.Abs(values[i]) > tolerance)
                {
                    System.Diagnostics.Debug.Assert(false);
                    return;
                }
        }

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertTolerantIf(bool condition, double tolerance, params double[] values)
		{
            if(condition)
                AssertTolerant(tolerance, values);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertTolerant(double tolerance, params double[][] values)
		{
			for(int i=0; i<values.Length; i++)
				for(int j=0; j<values[i].Length; j++)
                    if(Math.Abs(values[i][j]) > tolerance)
                    {
            			System.Diagnostics.Debug.Assert(false);
                        return;
                    }
		}

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertTolerant(double tolerance, double[,] values)
        {
            for(int c=0; c<values.GetLength(0); c++)
                for(int r=0; r<values.GetLength(1); r++)
                    if(Math.Abs(values[c, r]) > tolerance)
                    {
                        System.Diagnostics.Debug.Assert(false);
                        return;
                    }
        }
        //////////////////////////////////////////////
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void Verify(bool condition)
		{
			System.Diagnostics.Debug.Assert(condition);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void AssertOr(params bool[] conditions)
		{
			foreach(bool condition in conditions)
			{
				if(condition == true)
				{
					return;
				}
			}
			System.Diagnostics.Debug.Assert(false);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void AssertAnd(params bool[] conditions)
		{
			bool success = true;
			foreach(bool condition in conditions)
			{
				if(condition == false)
				{
					success = false;
				}
			}
			System.Diagnostics.Debug.Assert(success);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void AssertXor(params bool[] conditions)
		{
			int numsuccess = 0;
			foreach(bool condition in conditions)
			{
				if(condition == true)
				{
					numsuccess++;
				}
			}
			System.Diagnostics.Debug.Assert(numsuccess == 1);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void AssertIf(bool condition, params bool[] asserts)
		{
			if(condition)
			{
				bool assert = true;
				for(int i=0; i<asserts.Length; i++)
					assert = assert && asserts[i];
				Assert(assert);
			}
		}

		static public bool IsDebuggerAttached
		{
			get
			{
				return System.Diagnostics.Debugger.IsAttached;
			}
		}
		static public bool IsDebuggerAttachedWithProb(double prob)
		{
			if(System.Diagnostics.Debugger.IsAttached)
			{
				Debug.Assert(0<=prob, prob<=1);
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

		public static class Trace
		{
			public static bool AutoFlush          { set{ System.Diagnostics.Trace.AutoFlush   = value; } get{ return System.Diagnostics.Trace.AutoFlush  ; } }
			public static int  IndentLevel        { set{ System.Diagnostics.Trace.IndentLevel = value; } get{ return System.Diagnostics.Trace.IndentLevel; } }
			public static int  IndentSize         { set{ System.Diagnostics.Trace.IndentSize  = value; } get{ return System.Diagnostics.Trace.IndentSize ; } }
			public static System.Diagnostics.CorrelationManager      CorrelationManager {                get{ return System.Diagnostics.Trace.CorrelationManager; } }
			public static System.Diagnostics.TraceListenerCollection Listeners          {                get{ return System.Diagnostics.Trace.Listeners         ; } }

			[System.Diagnostics.Conditional("TRACE")]	public static void Flush()    { System.Diagnostics.Trace.Flush();    }
			[System.Diagnostics.Conditional("TRACE")]	public static void Indent()   { System.Diagnostics.Trace.Indent();   }
			[System.Diagnostics.Conditional("TRACE")]	public static void Unindent() { System.Diagnostics.Trace.Unindent(); }
														public static void Refresh()  { System.Diagnostics.Trace.Refresh();  }

			[System.Diagnostics.Conditional("TRACE")]	public static void Write(object value)                                          { System.Diagnostics.Trace.Write(value);                              }
			[System.Diagnostics.Conditional("TRACE")]	public static void Write(string message)										{ System.Diagnostics.Trace.Write(message);                            }
			[System.Diagnostics.Conditional("TRACE")]	public static void Write(object value, string category)							{ System.Diagnostics.Trace.Write(value, category);                    }
			[System.Diagnostics.Conditional("TRACE")]	public static void Write(string message, string category)						{ System.Diagnostics.Trace.Write(message, category);                  }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, object value)						{ System.Diagnostics.Trace.WriteIf(condition, value);                 }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, string message)						{ System.Diagnostics.Trace.WriteIf(condition, message);               }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, object value, string category)		{ System.Diagnostics.Trace.WriteIf(condition, value, category);       }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, string message, string category)		{ System.Diagnostics.Trace.WriteIf(condition, message, category);     }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(object value)										{ System.Diagnostics.Trace.WriteLine(value);                          }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(string message)									{ System.Diagnostics.Trace.WriteLine(message);                        }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(object value, string category)						{ System.Diagnostics.Trace.WriteLine(value, category);                }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(string message, string category)					{ System.Diagnostics.Trace.WriteLine(message, category);              }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, object value)					{ System.Diagnostics.Trace.WriteLineIf(condition, value);             }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, string message)					{ System.Diagnostics.Trace.WriteLineIf(condition, message);           }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, object value, string category)	{ System.Diagnostics.Trace.WriteLineIf(condition, value, category);   }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, string message, string category)	{ System.Diagnostics.Trace.WriteLineIf(condition, message, category); }
		}
		public static class TraceFile
		{
			static System.IO.StreamWriter writer = System.IO.File.CreateText("TRACE.TXT");
			//[System.Diagnostics.Conditional("TRACE")]	public static void Write(object value)                                          { writer.Write(value);                              writer.Flush(); }
			[System.Diagnostics.Conditional("TRACE")]	public static void Write(string message)										{ writer.Write(message);                            writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void Write(object value, string category)							{ writer.Write(value, category);                    writer.Flush(); }
			[System.Diagnostics.Conditional("TRACE")]	public static void Write(string message, string category)						{ writer.Write(message, category);                  writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, object value)						{ writer.WriteIf(condition, value);                 writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, string message)						{ writer.WriteIf(condition, message);               writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, object value, string category)		{ writer.WriteIf(condition, value, category);       writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteIf(bool condition, string message, string category)		{ writer.WriteIf(condition, message, category);     writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(object value)										{ writer.WriteLine(value);                          writer.Flush(); }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(string message)									{ writer.WriteLine(message);                        writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(object value, string category)						{ writer.WriteLine(value, category);                writer.Flush(); }
			[System.Diagnostics.Conditional("TRACE")]	public static void WriteLine(string message, string category)					{ writer.WriteLine(message, category);              writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, object value)					{ writer.WriteLineIf(condition, value);             writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, string message)					{ writer.WriteLineIf(condition, message);           writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, object value, string category)	{ writer.WriteLineIf(condition, value, category);   writer.Flush(); }
			//[System.Diagnostics.Conditional("TRACE")]	public static void WriteLineIf(bool condition, string message, string category)	{ writer.WriteLineIf(condition, message, category); writer.Flush(); }
		}
	}
}
