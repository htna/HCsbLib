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

        //  bool useMatlabLock = false;
        //  {
        //      HFile.FileLock matlablock = null;
        //      if(useMatlabLock)
        //      {
        //          while(true)
        //          {
        //              matlablock = HFile.LockFile(@"c:\temp\matlablock");
        //              if(matlablock != null)
        //                  break;
        //              System.Threading.Thread.Sleep(100);
        //          }
        //      }
        //
        //      ...
        //
        //      if(matlablock != null)
        //      {
        //          matlablock.Release();
        //          matlablock = null;
        //      }
        //  }
        //
        //
        /// HFile.FileLock matlablock = GetLockFile();
        /// ...
        /// if(matlablock != null)
        /// {
        ///     matlablock.Release();
        ///     matlablock = null;
        /// }
        public static HFile.FileLock GetLockFile(int numprocesss=1, string lockpathbase=@"c:\temp\matlablock_$$lockcount$$")
        {
            HFile.FileLock matlablock = null;
            //if(useMatlabLock)
            {
                int lockcount = 0;
                while(true)
                {
                    matlablock = HFile.LockFile(@"c:\temp\matlablock"+lockcount);
                    if(matlablock != null)
                        break;
                    System.Threading.Thread.Sleep(500);
                    lockcount = (lockcount + 1) % numprocesss;
                }
            }
            return matlablock;
        }
	}
}
