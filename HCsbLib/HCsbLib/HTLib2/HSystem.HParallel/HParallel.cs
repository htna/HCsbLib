using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace HTLib2
{
    public static class HParallel
	{
        public static ParallelLoopResult For(int  fromInclusive, int  toExclusive, Action<int, ParallelLoopState> body)                                                                                                                                            { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, body); }
        public static ParallelLoopResult For(int  fromInclusive, int  toExclusive, Action<int> body)                                                                                                                                                               { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, body); }
        public static ParallelLoopResult For(long fromInclusive, long toExclusive, Action<long, ParallelLoopState> body)                                                                                                                                           { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, body); }
        public static ParallelLoopResult For(long fromInclusive, long toExclusive, Action<long> body)                                                                                                                                                              { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, body); }
        public static ParallelLoopResult For(int  fromInclusive, int  toExclusive, ParallelOptions parallelOptions, Action<int, ParallelLoopState> body)                                                                                                           { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, parallelOptions, body); }
        public static ParallelLoopResult For(int  fromInclusive, int  toExclusive, ParallelOptions parallelOptions, Action<int> body)                                                                                                                              { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, parallelOptions, body); }
        public static ParallelLoopResult For(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Action<long, ParallelLoopState> body)                                                                                                          { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, parallelOptions, body); }
        public static ParallelLoopResult For(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Action<long> body)                                                                                                                             { return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, parallelOptions, body); }
        public static ParallelLoopResult For<TLocal>(int  fromInclusive, int  toExclusive, Func<TLocal> localInit, Func<int, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                                                                 { return System.Threading.Tasks.Parallel.For<TLocal>(fromInclusive, toExclusive, localInit, body, localFinally); }
        public static ParallelLoopResult For<TLocal>(long fromInclusive, long toExclusive, Func<TLocal> localInit, Func<long, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                                                                { return System.Threading.Tasks.Parallel.For<TLocal>(fromInclusive, toExclusive, localInit, body, localFinally); }
        public static ParallelLoopResult For<TLocal>(int  fromInclusive, int  toExclusive, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<int, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                                { return System.Threading.Tasks.Parallel.For<TLocal>(fromInclusive, toExclusive, parallelOptions, localInit, body, localFinally); }
        public static ParallelLoopResult For<TLocal>(long fromInclusive, long toExclusive, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<long, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                               { return System.Threading.Tasks.Parallel.For<TLocal>(fromInclusive, toExclusive, parallelOptions, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, ParallelLoopState, long> body)                                                                                                                              { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, body); }
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, ParallelLoopState> body)                                                                                                                                    { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, body); }
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)                                                                                                                                                       { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, body); }
        public static ParallelLoopResult ForEach<TSource>(OrderablePartitioner<TSource> source, Action<TSource, ParallelLoopState, long> body)                                                                                                                     { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, body); }
        public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource, ParallelLoopState> body)                                                                                                                                    { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, body); }
        public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource> body)                                                                                                                                                       { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, body); }
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState, long> body)                                                                                             { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, parallelOptions, body); }
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState> body)                                                                                                   { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, parallelOptions, body); }
        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)                                                                                                                      { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, parallelOptions, body); }
        public static ParallelLoopResult ForEach<TSource>(OrderablePartitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState, long> body)                                                                                    { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, parallelOptions, body); }
        public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource, ParallelLoopState> body)                                                                                                   { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, parallelOptions, body); }
        public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)                                                                                                                      { return System.Threading.Tasks.Parallel.ForEach<TSource>(source, parallelOptions, body); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)                                                   { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                                                         { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(OrderablePartitioner<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)                                          { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(Partitioner<TSource> source, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                                                         { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)                  { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, parallelOptions, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                        { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, parallelOptions, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(OrderablePartitioner<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)         { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, parallelOptions, localInit, body, localFinally); }
        public static ParallelLoopResult ForEach<TSource, TLocal>(Partitioner<TSource> source, ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)                        { return System.Threading.Tasks.Parallel.ForEach<TSource, TLocal>(source, parallelOptions, localInit, body, localFinally); }
        public static void Invoke(params Action[] actions)                                                                                                                                                                                                         {        System.Threading.Tasks.Parallel.Invoke(actions); }
        public static void Invoke(ParallelOptions parallelOptions, params Action[] actions)                                                                                                                                                                        {        System.Threading.Tasks.Parallel.Invoke(parallelOptions, actions); }

        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, int optionMaxDegreeOfParallelism, Action<TSource> body)
        {
            return System.Threading.Tasks.Parallel.ForEach<TSource>
                (
                    source,
                    new ParallelOptions { MaxDegreeOfParallelism = optionMaxDegreeOfParallelism },
                    body
                );
        }

        public static ParallelLoopResult For(int fromInclusive, int toExclusive, int increment, Action<int> body)
        {
            List<int> idxs = new List<int>();
            for(int i=fromInclusive; i<toExclusive; i+=increment)
                idxs.Add(i);
            return ForEach(idxs, body);
        }

        //  string prefix = "prefix: ";
        //  Action<int> body = delegate(int i, object lprefix) {
        //      System.Console.WriteLine((string)base + i.ToString());
        //  }
        //  For<object>(0, 10, body, prefix);
        public static ParallelLoopResult ForEach<TSource, TParam>(Partitioner<TSource> source, Action<TSource, TParam> body, TParam param)
        {
            Action<TSource> lbody = delegate(TSource src)
            {
                body(src, param);
            };
            return System.Threading.Tasks.Parallel.ForEach<TSource>(source, lbody);
        }
        public static ParallelLoopResult For<TParam>(int fromInclusive, int toExclusive, Action<int, TParam> body, TParam param)
        {
            Action<int> lbody = delegate(int src)
            {
                body(src, param);
            };
            return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, lbody);
        }
        public static ParallelLoopResult For<TParam>(long fromInclusive, long toExclusive, Action<long, TParam> body, TParam param)
        {
            Action<long> lbody = delegate(long src)
            {
                body(src, param);
            };
            return System.Threading.Tasks.Parallel.For(fromInclusive, toExclusive, lbody);
        }

        public static void Sleep(int millisecondsTimeout) { System.Threading.Thread.Sleep(millisecondsTimeout); }
        public static void Sleep(TimeSpan        timeout) { System.Threading.Thread.Sleep(            timeout); }

        static List<Task> lstInvokeTask = new List<Task>();
        public static int NumInvokeTask { get { return lstInvokeTask.Count; } }
        public static void InvokeTask(Action action, Action finishaction=null, uint? maxParallelInvokeTask=null)
        {
            if(maxParallelInvokeTask == 0)
                maxParallelInvokeTask = 1;
            Task task = null;
            Action taskaction = delegate()
            {
                HDebug.Assert(task != null);
                action();
                if(finishaction != null)
                    finishaction();

                lock(lstInvokeTask)
                {
                    bool removed = lstInvokeTask.Remove(task);
                    HDebug.Assert(removed);
                }
            };
            task = new Task(taskaction);

            while(true)
            {
                lock(lstInvokeTask)
                {
                    if((maxParallelInvokeTask == null) || (lstInvokeTask.Count < maxParallelInvokeTask))
                    {
                        lstInvokeTask.Add(task);
                        break;
                    }
                }
                Sleep(100);
            }
            task.Start();
        }
    }
}
