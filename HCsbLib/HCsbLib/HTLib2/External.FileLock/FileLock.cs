using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HTLib2
{
    public partial class FileLock : IDisposable
    {
        /// using(new FileLock(500, 2, @"c:\temp\filelock"))
        /// {
        ///     ...
        /// }

        HFile.FileLock lockedfile = null;
        public FileLock(int sleepInMilliseconds=500, int numprocesss=1, string lockpathbase=@"c:\temp\filelock")
        {
            lockedfile = GetLockedFile(sleepInMilliseconds, numprocesss, lockpathbase);
            if(lockedfile == null)
                throw new Exception();
        }
        public virtual void Dispose()
        {
            lockedfile.Release();
            lockedfile = null;
        }

        /// HFile.FileLock lockedfile = FileLock.GetLockedFile();
        /// 
        /// ...
        /// 
        /// if(lockedfile != null)
        /// {
        ///     lockedfile.Release();
        ///     lockedfile = null;
        /// }
        public static HFile.FileLock GetLockedFile(int sleepInMilliseconds=500, int numprocesss=1, string lockpathbase=@"c:\temp\filelock")
        {
            HFile.FileLock lockedfile = null;
            {
                int lockcount = 0;
                while(true)
                {
                    string filename = lockpathbase + "_" + lockcount;
                    lockedfile = HFile.LockFile(filename);
                    if(lockedfile != null)
                        break;
                    System.Threading.Thread.Sleep(sleepInMilliseconds);
                    lockcount = (lockcount + 1) % numprocesss;
                }
            }
            return lockedfile;
        }
    }
}
