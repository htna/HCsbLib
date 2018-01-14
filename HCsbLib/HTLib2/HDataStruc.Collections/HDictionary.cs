using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
//using System.Linq;
//using System.Text;
using System.Collections.Generic;

namespace HTLib2
{
    [Serializable]
    public class HDictionary : System.Collections.Generic.Dictionary<string, HDynamic>
    {
        public HDictionary()
        {
            //HDebug.Depreciated();
            if(HDebug.Selftest())
            {
                HDebug.Assert(false); // depreciated
            }
        }

        public void Add(string key, object value)
        {
            base.Add(key, new HDynamic(value));
        }
		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
        public HDictionary(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
        }
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            base.GetObjectData(info, context);
		}
    }
}
