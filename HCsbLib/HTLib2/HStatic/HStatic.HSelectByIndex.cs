using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        //public static List<T> SelectByIndex<T>(this List<T> values, IList<int> indexes)
        //{
        //    List<T> lvalues = new List<T>(indexes.Count);
        //    for(int i=0; i<indexes.Count; i++)
        //        lvalues.Add(values[indexes[i]]);
        //    return lvalues;
        //}
        public static U[] HSelectByIndex<T,U>(this IList<T> list, IList<int> idx) 
        {
            List<U> select = new List<U>(idx.Count);
            for(int i=0; i<idx.Count; i++)
            {
                object obj = list[idx[i]];
                select.Add((U)obj);
            }
            return select.ToArray();
        }
        public static bool HSelectByIndex_selftest = HDebug.IsDebuggerAttached;
        public static T[] HSelectByIndex<T>
            ( this IList<T> list
            , IDictionary<int, int> idx
            , T defnull //=default(T)
            )
        {
            if(HSelectByIndex_selftest)
                #region selftest
            {
                HSelectByIndex_selftest = false;
                string[] _list = new string[] { "3", "1", "0", "2", "x", };
                Dictionary<int, int> _idx = new Dictionary<int, int>();
                _idx.Add(0, 2);
                _idx.Add(1, 1);
                _idx.Add(3, 0);
                string[] _out = _list.HSelectByIndex(_idx, "-");
                HDebug.Assert(_out.Length == 4);
                HDebug.Assert(_out[0] == "0");
                HDebug.Assert(_out[1] == "1");
                HDebug.Assert(_out[2] == "-");
                HDebug.Assert(_out[3] == "3");
            }
                #endregion

            int leng = idx.Keys.Max()+1;
            T[]    select = new T   [leng];
            bool[] assign = new bool[leng];
            for(int i=0; i<leng; i++)
            {
                select[i] = defnull;
                assign[i] = false;
            }
            foreach(var i_idxi in idx)
            {
                int i    = i_idxi.Key;
                int idxi = i_idxi.Value; HDebug.Assert(idxi == idx[i]);
                if(assign[i] == true)
                    throw new HException("multi-index");
                assign[i] = true;
                select[i] = list[idxi];
            }
            return select;
        }
        public static T[] HSelectByIndex<T>(this IList<T> list, IList<int> idx)
        {
            List<T> select = new List<T>(idx.Count);
            for(int i=0; i<idx.Count; i++)
                select.Add(list[idx[i]]);
            return select.ToArray();
        }
        public static T[,] HSelectByIndex<T>(this IList<T> list, int[,] idx)
        {
            int leng0 = idx.GetLength(0);
            int leng1 = idx.GetLength(1);
            T[,] select = new T[leng0, leng1];
            for(int i=0; i<leng0; i++)
                for(int j=0; j<leng1; j++)
                    select[i, j] = list[idx[i, j]];
            return select;
        }
        public static T[][] HSelectByIndex<T>(this IList<T> list, int[][] idx)
        {
            int leng0 = idx.Length;
            T[][] select = new T[leng0][];
            for(int i=0; i<leng0; i++)
            {
                if(idx[i] != null)
                {
                    int leng1 = idx[i].Length;
                    select[i] = new T[leng1];
                    for(int j=0; j<leng1; j++)
                        select[i][j] = list[idx[i][j]];
                }
            }
            return select;
        }

        public static Tuple<T        > HSelectByIndex<T>(this IList<T> list, Tuple<int                > idx) { return new Tuple<T        >(list[idx.Item1]                                                                    ); }
        public static Tuple<T,T      > HSelectByIndex<T>(this IList<T> list, Tuple<int,int            > idx) { return new Tuple<T,T      >(list[idx.Item1], list[idx.Item2]                                                   ); }
        public static Tuple<T,T,T    > HSelectByIndex<T>(this IList<T> list, Tuple<int,int,int        > idx) { return new Tuple<T,T,T    >(list[idx.Item1], list[idx.Item2], list[idx.Item3]                                  ); }
        public static Tuple<T,T,T,T  > HSelectByIndex<T>(this IList<T> list, Tuple<int,int,int,int    > idx) { return new Tuple<T,T,T,T  >(list[idx.Item1], list[idx.Item2], list[idx.Item3], list[idx.Item4]                 ); }
        public static Tuple<T,T,T,T,T> HSelectByIndex<T>(this IList<T> list, Tuple<int,int,int,int,int> idx) { return new Tuple<T,T,T,T,T>(list[idx.Item1], list[idx.Item2], list[idx.Item3], list[idx.Item4], list[idx.Item5]); }

        public static Tuple<T        >[] HSelectByIndex<T>(this IList<T> list, Tuple<int                >[] idx) { int leng=idx.Length; var select=new Tuple<T        >[leng]; for(int i=0; i<leng; i++) if(idx[i] != null) select[i] = list.HSelectByIndex(idx[i]); return select; }
        public static Tuple<T,T      >[] HSelectByIndex<T>(this IList<T> list, Tuple<int,int            >[] idx) { int leng=idx.Length; var select=new Tuple<T,T      >[leng]; for(int i=0; i<leng; i++) if(idx[i] != null) select[i] = list.HSelectByIndex(idx[i]); return select; }
        public static Tuple<T,T,T    >[] HSelectByIndex<T>(this IList<T> list, Tuple<int,int,int        >[] idx) { int leng=idx.Length; var select=new Tuple<T,T,T    >[leng]; for(int i=0; i<leng; i++) if(idx[i] != null) select[i] = list.HSelectByIndex(idx[i]); return select; }
        public static Tuple<T,T,T,T  >[] HSelectByIndex<T>(this IList<T> list, Tuple<int,int,int,int    >[] idx) { int leng=idx.Length; var select=new Tuple<T,T,T,T  >[leng]; for(int i=0; i<leng; i++) if(idx[i] != null) select[i] = list.HSelectByIndex(idx[i]); return select; }
        public static Tuple<T,T,T,T,T>[] HSelectByIndex<T>(this IList<T> list, Tuple<int,int,int,int,int>[] idx) { int leng=idx.Length; var select=new Tuple<T,T,T,T,T>[leng]; for(int i=0; i<leng; i++) if(idx[i] != null) select[i] = list.HSelectByIndex(idx[i]); return select; }
    }
}
