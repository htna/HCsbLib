using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HTLib2
{
    public partial class NamedLock
    {
        public delegate void Func();
        public static void LockedCall(string name, Func func)
        {
            int result = LockedCall(name, delegate()
                {
                    func();
                    return 0;
                });
            HDebug.Assert(result == 0);
        }
        // locking interprocess (locking between another application too)
        public static TResult LockedCall<TResult>(string name, Func<TResult> func)
        {
            while(true)
            {
                try
                {
                    var mutex = new Mutex(false, name);
                    mutex.WaitOne();

                    TResult result = func();

                    mutex.ReleaseMutex();
                    return result;
                }
                catch(AbandonedMutexException)
                {
                    // repeat if the exception (by closing another program using this mutex is closed by ctrl-c) happens.
                }
            }
        }
        public delegate void FuncR <T1         >(ref T1 arg1                          );    public static void LockedCall<T1        >(string name, FuncR  <T1        > func, ref T1 arg1                          ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(ref arg1                    ); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncO <T1         >(out T1 arg1                          );    public static void LockedCall<T1        >(string name, FuncO  <T1        > func, out T1 arg1                          ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(out arg1                    ); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncRR<T1,T2      >(ref T1 arg1, ref T2 arg2             );    public static void LockedCall<T1,T2     >(string name, FuncRR <T1,T2     > func, ref T1 arg1, ref T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(ref arg1, ref arg2          ); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncRO<T1,T2      >(ref T1 arg1, out T2 arg2             );    public static void LockedCall<T1,T2     >(string name, FuncRO <T1,T2     > func, ref T1 arg1, out T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(ref arg1, out arg2          ); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncOR<T1,T2      >(out T1 arg1, ref T2 arg2             );    public static void LockedCall<T1,T2     >(string name, FuncOR <T1,T2     > func, out T1 arg1, ref T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(out arg1, ref arg2          ); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncOO<T1,T2      >(out T1 arg1, out T2 arg2             );    public static void LockedCall<T1,T2     >(string name, FuncOO <T1,T2     > func, out T1 arg1, out T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(out arg1, out arg2          ); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncOOO<T1,T2,T3  >(out T1 arg1, out T2 arg2, out T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncOOO<T1,T2,T3  > func, out T1 arg1, out T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(out arg1, out arg2, out arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncOOR<T1,T2,T3  >(out T1 arg1, out T2 arg2, ref T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncOOR<T1,T2,T3  > func, out T1 arg1, out T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(out arg1, out arg2, ref arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncORO<T1,T2,T3  >(out T1 arg1, ref T2 arg2, out T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncORO<T1,T2,T3  > func, out T1 arg1, ref T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(out arg1, ref arg2, out arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncORR<T1,T2,T3  >(out T1 arg1, ref T2 arg2, ref T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncORR<T1,T2,T3  > func, out T1 arg1, ref T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(out arg1, ref arg2, ref arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncROO<T1,T2,T3  >(ref T1 arg1, out T2 arg2, out T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncROO<T1,T2,T3  > func, ref T1 arg1, out T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(ref arg1, out arg2, out arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncROR<T1,T2,T3  >(ref T1 arg1, out T2 arg2, ref T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncROR<T1,T2,T3  > func, ref T1 arg1, out T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(ref arg1, out arg2, ref arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncRRO<T1,T2,T3  >(ref T1 arg1, ref T2 arg2, out T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncRRO<T1,T2,T3  > func, ref T1 arg1, ref T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(ref arg1, ref arg2, out arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }
        public delegate void FuncRRR<T1,T2,T3  >(ref T1 arg1, ref T2 arg2, ref T3 arg3);    public static void LockedCall<T1,T2,T3  >(string name, FuncRRR<T1,T2,T3  > func, ref T1 arg1, ref T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne();            func(ref arg1, ref arg2, ref arg3); mutex.ReleaseMutex();                } catch(AbandonedMutexException) { } } }

        public delegate U    FuncR  <T1,      U>(ref T1 arg1                          );    public static U    LockedCall<T1   ,U   >(string name, FuncR  <T1,      U> func, ref T1 arg1                          ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(ref arg1                    ); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncO  <T1,      U>(out T1 arg1                          );    public static U    LockedCall<T1   ,U   >(string name, FuncO  <T1,      U> func, out T1 arg1                          ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(out arg1                    ); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncRR <T1,T2   ,U>(ref T1 arg1, ref T2 arg2             );    public static U    LockedCall<T1,T2,U   >(string name, FuncRR <T1,T2   ,U> func, ref T1 arg1, ref T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(ref arg1, ref arg2          ); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncRO <T1,T2   ,U>(ref T1 arg1, out T2 arg2             );    public static U    LockedCall<T1,T2,U   >(string name, FuncRO <T1,T2   ,U> func, ref T1 arg1, out T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(ref arg1, out arg2          ); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncOR <T1,T2   ,U>(out T1 arg1, ref T2 arg2             );    public static U    LockedCall<T1,T2,U   >(string name, FuncOR <T1,T2   ,U> func, out T1 arg1, ref T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(out arg1, ref arg2          ); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncOO <T1,T2   ,U>(out T1 arg1, out T2 arg2             );    public static U    LockedCall<T1,T2,U   >(string name, FuncOO <T1,T2   ,U> func, out T1 arg1, out T2 arg2             ) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(out arg1, out arg2          ); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncOOO<T1,T2,T3,U>(out T1 arg1, out T2 arg2, out T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncOOO<T1,T2,T3,U> func, out T1 arg1, out T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(out arg1, out arg2, out arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncOOR<T1,T2,T3,U>(out T1 arg1, out T2 arg2, ref T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncOOR<T1,T2,T3,U> func, out T1 arg1, out T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(out arg1, out arg2, ref arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncORO<T1,T2,T3,U>(out T1 arg1, ref T2 arg2, out T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncORO<T1,T2,T3,U> func, out T1 arg1, ref T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(out arg1, ref arg2, out arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncORR<T1,T2,T3,U>(out T1 arg1, ref T2 arg2, ref T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncORR<T1,T2,T3,U> func, out T1 arg1, ref T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(out arg1, ref arg2, ref arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncROO<T1,T2,T3,U>(ref T1 arg1, out T2 arg2, out T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncROO<T1,T2,T3,U> func, ref T1 arg1, out T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(ref arg1, out arg2, out arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncROR<T1,T2,T3,U>(ref T1 arg1, out T2 arg2, ref T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncROR<T1,T2,T3,U> func, ref T1 arg1, out T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(ref arg1, out arg2, ref arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncRRO<T1,T2,T3,U>(ref T1 arg1, ref T2 arg2, out T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncRRO<T1,T2,T3,U> func, ref T1 arg1, ref T2 arg2, out T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(ref arg1, ref arg2, out arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
        public delegate U    FuncRRR<T1,T2,T3,U>(ref T1 arg1, ref T2 arg2, ref T3 arg3);    public static U    LockedCall<T1,T2,T3,U>(string name, FuncRRR<T1,T2,T3,U> func, ref T1 arg1, ref T2 arg2, ref T3 arg3) { while(true) { try { var mutex = new Mutex(false, name); mutex.WaitOne(); U result = func(ref arg1, ref arg2, ref arg3); mutex.ReleaseMutex(); return result; } catch(AbandonedMutexException) { } } }
    }
}
