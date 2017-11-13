using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
	{
        static Dictionary<string,object> _watch = (IsDebuggerAttached) ? (new Dictionary<string, object>()) : (null);

        public static CWatch Watch = new CWatch();

        public struct CWatch
        {
            public object this[string key]
            {
                get
                {
                    if(_watch == null)
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
