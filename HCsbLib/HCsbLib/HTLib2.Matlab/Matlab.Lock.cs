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

        /// var matlablock = new Matlab.FileLock();
        /// {
        ///     ...
        /// }
        /// matlablock.Release();
        /// matlablock = null;
        /// 
        /// using(new Matlab.FileLock())
        /// {
        ///     ...
        /// }
        public class LockByFile : HTLib2.LockByFile
        {
            public LockByFile(int sleepInMilliseconds=500, int numprocesss=1, string lockpathbase=@"c:\temp\matlablock")
                : base(sleepInMilliseconds, numprocesss, lockpathbase)
            {
            }
            public override void Dispose()
            {
                base.Dispose();
            }
        }

        public static HFile.FileLock GetLockFile(int sleepInMilliseconds=500, int numprocesss=1, string lockpathbase=@"c:\temp\matlablock")
        {
            return HTLib2.LockByFile.GetLockedFile(sleepInMilliseconds, numprocesss, lockpathbase);

            //  HFile.FileLock matlablock = null;
            //  //if(useMatlabLock)
            //  {
            //      int lockcount = 0;
            //      while(true)
            //      {
            //          string filename = lockpathbase + "_" + lockcount;
            //          matlablock = HFile.LockFile(@"c:\temp\matlablock"+lockcount);
            //          if(matlablock != null)
            //              break;
            //          System.Threading.Thread.Sleep(sleepInMilliseconds);
            //          lockcount = (lockcount + 1) % numprocesss;
            //      }
            //  }
            //  return matlablock;
        }
	}
}
