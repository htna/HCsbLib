using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Linq;

namespace HTLib2
{
    public static class HGC
    {
        public static int MaxGeneration                                                                             { get { return GC.MaxGeneration; } }

        public static void AddMemoryPressure(long bytesAllocated)                                                   {        GC.AddMemoryPressure(bytesAllocated)                                               ;}
        public static void CancelFullGCNotification()                                                               {        GC.CancelFullGCNotification()                                                      ;}
        public static void Collect()                                                                                {        GC.Collect()                                                                       ;}
        public static void Collect(int generation)                                                                  {        GC.Collect(generation)                                                             ;}
        public static void Collect(int generation, GCCollectionMode mode)                                           {        GC.Collect(generation, mode)                                                       ;}
        public static int CollectionCount(int generation)                                                           { return GC.CollectionCount(generation)                                                     ;}
        public static int GetGeneration(object obj)                                                                 { return GC.GetGeneration(obj)                                                              ;}
        public static int GetGeneration(WeakReference wo)                                                           { return GC.GetGeneration(wo)                                                               ;}
        public static long GetTotalMemory(bool forceFullCollection)                                                 { return GC.GetTotalMemory(forceFullCollection)                                             ;}
        public static void KeepAlive(object obj)                                                                    {        GC.KeepAlive(obj)                                                                  ;}
        public static void RegisterForFullGCNotification(int maxGenerationThreshold, int largeObjectHeapThreshold)  {        GC.RegisterForFullGCNotification(maxGenerationThreshold, largeObjectHeapThreshold) ;}
        public static void RemoveMemoryPressure(long bytesAllocated)                                                {        GC.RemoveMemoryPressure(bytesAllocated)                                            ;}
        public static void ReRegisterForFinalize(object obj)                                                        {        GC.ReRegisterForFinalize(obj)                                                      ;}
        public static void SuppressFinalize(object obj)                                                             {        GC.SuppressFinalize(obj)                                                           ;}
        public static GCNotificationStatus WaitForFullGCApproach()                                                  { return GC.WaitForFullGCApproach()                                                         ;}
        public static GCNotificationStatus WaitForFullGCApproach(int millisecondsTimeout)                           { return GC.WaitForFullGCApproach(millisecondsTimeout)                                      ;}
        public static GCNotificationStatus WaitForFullGCComplete()                                                  { return GC.WaitForFullGCComplete()                                                         ;}
        public static GCNotificationStatus WaitForFullGCComplete(int millisecondsTimeout)                           { return GC.WaitForFullGCComplete(millisecondsTimeout)                                      ;}
        public static void WaitForPendingFinalizers()                                                               {        GC.WaitForPendingFinalizers()                                                      ;}

        public static void Test()
        {
            // http://csharp.2000things.com/tag/garbage-collection/

            object bob = new object();
                          Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 0
            GC.Collect(); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 1
            GC.Collect(); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2
            GC.Collect(); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2

            bob = new object();
                           Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 0
            GC.Collect(0); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 1
            GC.Collect(0); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 1
            GC.Collect(0); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 1

            GC.Collect(1); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2
            GC.Collect(1); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2
            GC.Collect(1); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2

            GC.Collect(2); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2
            GC.Collect(2); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2
            GC.Collect(2); Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));    // Bob is in generation 2
        }
    }
}
