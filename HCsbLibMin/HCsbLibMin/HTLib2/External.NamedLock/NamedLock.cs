using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HTLib2
{
    public partial class NamedLock : IDisposable
    {
        /// using(new NamedLock("locking name"))
        /// {
        ///     ...
        /// }

        string name;
        public NamedLock(string name)
        {
            this.name = name;
            if(name != "nolock")
                Lock(name);
        }
        public virtual void Dispose()
        {
            if(name != "nolock")
                Unlock(name);
        }

        public static Dictionary<string, Mutex> _lstNamedLock = new Dictionary<string, Mutex>();
        public static void Lock(string name)
        {
            // refer "public static TResult LockedCall<TResult>(string name, Func<TResult> func)"
            while(true)
            {
                try
                {
                    var mutex = new Mutex(false, name);
                    mutex.WaitOne();
                    HDebug.Assert(_lstNamedLock.ContainsKey(name) == false);
                    lock(_lstNamedLock) _lstNamedLock.Add(name, mutex);
                    break;
                }
                catch(AbandonedMutexException)
                {
                    // repeat if the exception (by closing another program using this mutex is closed by ctrl-c) happens.
                }
            }
        }
        public static void Unlock(string name)
        {
            HDebug.Assert(_lstNamedLock.ContainsKey(name) == true);
            Mutex mutex = _lstNamedLock[name];
            try
            {
                mutex.ReleaseMutex();
            }
            catch(AbandonedMutexException)
            {
                HDebug.Assert(false);
            }

            //HDebug.Verify(_lstNamedLock.Remove(name));
            lock(_lstNamedLock)
            {
                bool verify = _lstNamedLock.Remove(name);
                HDebug.Assert(verify);
            }
        }
    }
}
