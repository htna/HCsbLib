using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

/// Introduction
///     There are a few problems I've come across where synchronizing a particular
///     "name" might be useful. One of the apps I work on makes heavy use of Lucene.NET;
///     each page shows the results of a couple of queries. It doesn't make a whole ton
///     of sense to run multiple, identical queries against Lucene at the same time, so
///     I generate a key for each query, lock against that key, and let the first thread
///     do the actual work while the others sit around sipping coffee.
///
/// Using the code
///     It's pretty simple to use this class. Create a new instance of NamedLock<string>,
///     then call the Lock function within a using statement.
///
/// NamedLock<string> locker = new NamedLock<string>();
/// var url = "http://services.digg.com...";
/// using (locker.Lock(url))
/// {
///     //Do something synchronized
///     var xml = new XmlDocument();
///     xml.Load(url);
/// }

namespace HTLib2
{
    public partial class NamedLock<T>
    {
        public static void SelfTest()
        {
            //NamedLock<string> locker = new NamedLock<string>();
            //using(locker.Lock("selftest"))
            //{
            //    long iter = 0;
            //    while(true)
            //    {
            //        iter ++;
            //        System.Console.WriteLine("Locking..."+(iter));
            //    }
            //}
            while(true)
            {
                // Wait a few seconds if contended, in case another instance
                // of the program is still in the process of shutting down.
                bool processed = false;
                try
                {
                    var mutex = new Mutex(false, "oreilly.com OneAtATimeDemo");
                    mutex.WaitOne();
                    {
                        for(long iter=0; iter<100000; iter++)
                        {
                            System.Console.WriteLine("Locking..."+(iter));
                        }
                        processed = true;
                    }
                    mutex.ReleaseMutex();
                }
                catch(System.Threading.AbandonedMutexException)
                {
                }
                if(processed)
                    break;
            }
            //mutex.Dispose();
        }
    }
}