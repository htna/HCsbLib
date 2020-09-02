using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    using Env = HTLib2.HEnvironment;
    public partial class Matlab
	{
        /// using(new Matlab.NamedLock(""))
        /// {
        ///     ...
        /// }
        public class NamedLock : HTLib2.NamedLock
        {
            public readonly string name;
            public static string GetName(string name)
            {
                name = "nolock";
                return name;
                //name = "LOCK";
                //if(Env.IsWow64BitProcess)
                //    return "matlab64 - "+name;
                //else
                //    return "matlab32 - "+name;
            }
            public NamedLock(string name)
                : base(GetName(name))
            {
                this.name = name;
                Matlab.Execute("clear;");
            }
            public override void Dispose()
            {
                Matlab.Execute("clear;");
                base.Dispose();
            }
        }
	}
}
