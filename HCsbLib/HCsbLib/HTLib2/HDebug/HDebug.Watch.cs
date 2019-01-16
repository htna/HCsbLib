using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
	{
        static Dictionary<string,object> _watch = (IsDebuggerAttached) ? (new Dictionary<string, object>()) : (null);
        static object _watch0 = null;
        static object _watch1 = null;
        static object _watch2 = null;
        static object _watch3 = null;
        static object _watch4 = null;
        static object _watch5 = null;
        static object _watch6 = null;
        static object _watch7 = null;
        static object _watch8 = null;
        static object _watch9 = null;

        public static CWatch Watch = new CWatch();

        public struct CWatch
        {
            public object this[string key]
            {
                get
                {
                    if(_watch == null)
                        return null;
                    if(_watch.ContainsKey(key) == false)
                        return null;
                    return _watch[key];
                }
                set
                {
                    if(_watch == null)
                        return;
                    if(_watch.ContainsKey(key) == false)
                        _watch.Add(key, null);
                    _watch[key] = value;
                }
            }
        }
	}
}
