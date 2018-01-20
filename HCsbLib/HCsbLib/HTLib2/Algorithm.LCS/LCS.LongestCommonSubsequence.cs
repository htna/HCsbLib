using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace HTLib2
{
    public partial class LCS
    {
        public class OLcs<T>
        {
            public IList<object>            _seq1;
            public IList<object>            _seq2;
            public Func<object,object,bool> _equals;
            public Tuple<int,int,int>[,]    LCS_src1_src2_leng;
            public void BackTrack
                ( int curr1, int curr2
                , int max_llcs_length
                , out List<T>   lcs     
                , List<int> lcsidx1 = null
                , List<int> lcsidx2 = null
                )
            {
                List<object>              llcs;
                List<int>                 llcsidx1;
                List<int>                 llcsidx2;
                List<Tuple<Oper, object>> loper1to2;
                List<Tuple<Oper, object>> loper2to1;
                LongestCommonSubsequence_BackTrack
                    ( _seq1, _seq2, _equals, LCS_src1_src2_leng
                    , curr1, curr2, max_llcs_length
                    , out llcs, out llcsidx1, out llcsidx2, out loper1to2, out loper2to1
                    );
                lcs = llcs.OfType<T>().ToList();
                if(lcsidx1 != null) { lcsidx1.Clear(); lcsidx1.AddRange(llcsidx1); }
                if(lcsidx2 != null) { lcsidx2.Clear(); lcsidx2.AddRange(llcsidx2); }
            }
            public T[]              lcs;
            public int[]            lcsidx1;
            public int[]            lcsidx2;
            public Tuple<Oper, T>[] oper1to2;
            public Tuple<Oper, T>[] oper2to1;
            public int              Length { get { return lcs.Length; } }
        }
        static bool LongestCommonSubsequence_selftest = true;
        public static OLcs<T> LongestCommonSubsequence<T>( IList<T> seq1, IList<T> seq2
                                                         , Func<T,T,bool> equals
                                                         )
        {
            Func<object, object, bool> objequals = delegate(object v1, object v2) { return equals((T)v1,(T)v2); };
            Tuple<int,int,int>[,] LCS_src1_src2_leng;
            object[] lcs    ;
            int[]    lcsidx1;
            int[]    lcsidx2;
            Tuple<Oper, object>[] oper1to2;
            Tuple<Oper, object>[] oper2to1;

            object[] objseq1 = seq1.OfType<object>().ToArray();
            object[] objseq2 = seq2.OfType<object>().ToArray();

            bool succ = LongestCommonSubsequenceImpl
            ( objseq1, objseq2
            , objequals
            , out LCS_src1_src2_leng
            , out lcs
            , out lcsidx1
            , out lcsidx2
            , out oper1to2
            , out oper2to1
            );

            if(succ == false)
                return null;
            return new OLcs<T>
            {
                _seq1              = objseq1,
                _seq2              = objseq2,
                _equals            = objequals,
                LCS_src1_src2_leng = LCS_src1_src2_leng,
                lcs      = lcs     .OfType<T>().ToArray(),
                lcsidx1  = lcsidx1 ,
                lcsidx2  = lcsidx2 ,
                oper1to2 = oper1to2
                            .Select(delegate(Tuple<Oper, object> op) { return new Tuple<Oper,T>(op.Item1, (T)op.Item2); })
                            .ToArray(),
                oper2to1 = oper2to1
                            .Select(delegate(Tuple<Oper, object> op) { return new Tuple<Oper,T>(op.Item1, (T)op.Item2); })
                            .ToArray(),
            };
        }
        public enum Oper
        {
            Match,
            Delete,
            Insert,
        };
        public static T[] DoOper<T>(IList<T> seq1, IList<Tuple<Oper, T>> oper1to2, Func<T, T, bool> equals)
        {
            seq1 = new List<T>(seq1);
            List<T> seq2 = new List<T>();
            foreach(var oper in oper1to2)
            {
                switch(oper.Item1)
                {
                    case Oper.Match : HDebug.Assert(equals(seq1[0], oper.Item2)); seq1.RemoveAt(0); seq2.Add(oper.Item2); break;
                    case Oper.Insert: seq2.Add(oper.Item2); break;
                    case Oper.Delete: HDebug.Assert(equals(seq1[0], oper.Item2)); seq1.RemoveAt(0); break;
                    default: throw new Exception();
                }
            }
            return seq2.ToArray();
        }
        static bool LongestCommonSubsequenceImpl
            ( IList<object> seq1, IList<object> seq2
            , Func<object,object,bool> equals
            , out Tuple<int,int,int>[,] LCS_src1_src2_leng
            , out object[]              lcs
            , out int[]                 lcsidx1
            , out int[]                 lcsidx2
            , out Tuple<Oper, object>[] oper1to2
            , out Tuple<Oper, object>[] oper2to1
            )
        {
            /// LCS(X[i],Y[j]) = | empty                                            if i = 0 or j = 0
            ///                  | (LCS(X[i-1],Y[j-1],xi)                           if xi = yi
            ///                  | longest(LCS(X[i], Y[j-1]), LCS(X[i-1],Y[j]))     if xi != yj
            if(LongestCommonSubsequence_selftest)
            {
                LongestCommonSubsequence_selftest = false;
                Func<char,char,bool> charcomp = delegate(char a, char b) { return (a==b); };
                OLcs<char> tlcs;
                char[] tseq1;
                char[] tseq2;
                {
                    /// AGCAT
                    ///  | |
                    ///  G AR
                    tseq1 = "AGCAT".ToArray();
                    tseq2 = "GAR"  .ToArray();
                    tlcs = LongestCommonSubsequence(tseq1, tseq2, charcomp);
                    HDebug.Assert(tlcs.lcs.HToString() == "GA");
                    HDebug.Assert(tseq1.HSelectByIndex(tlcs.lcsidx1).HToString() == "GA");
                    HDebug.Assert(tseq2.HSelectByIndex(tlcs.lcsidx2).HToString() == "GA");
                    HDebug.Assert(DoOper(tseq1, tlcs.oper1to2, charcomp).HToString() == tseq2.HToString());
                    HDebug.Assert(DoOper(tseq2, tlcs.oper2to1, charcomp).HToString() == tseq1.HToString());
                }
                {
                    ///  XYG AR
                    ///  | | |
                    /// AX GCAT
                    tseq1 = "XYGAR" .ToArray();
                    tseq2 = "AXGCAT".ToArray();
                    tlcs = LongestCommonSubsequence(tseq1, tseq2, charcomp);
                    HDebug.Assert(tlcs.lcs.HToString() == "XGA");
                    HDebug.Assert(tseq1.HSelectByIndex(tlcs.lcsidx1).HToString() == "XGA");
                    HDebug.Assert(tseq2.HSelectByIndex(tlcs.lcsidx2).HToString() == "XGA");
                    HDebug.Assert(DoOper(tseq1, tlcs.oper1to2, charcomp).HToString() == tseq2.HToString());
                    HDebug.Assert(DoOper(tseq2, tlcs.oper2to1, charcomp).HToString() == tseq1.HToString());
                }
            }

            int size1 = seq1.Count;
            int size2 = seq2.Count;
            Tuple<int,int,int>[,] LCS = new Tuple<int, int, int>[size1+1, size2+1];

            for(int i=0; i<size1+1; i++)
            {
                for(int j=0; j<size2+1; j++)
                {
                    if(i==0 || j==0)
                    {
                        if     (i==0 && j==0) LCS[i, j] = new Tuple<int, int, int>( -1,  -1, 0);
                        else if(i==0 && j!=0) LCS[i, j] = new Tuple<int, int, int>(  0, j-1, 0);
                        else if(i!=0 && j==0) LCS[i, j] = new Tuple<int, int, int>(i-1,   0, 0);
                        else throw new HException("HTLib2.LCS.LongestCommonSubsequenceImpl - 1");
                    }
                    else if(equals(seq1[i-1], seq2[j-1]) == true)
                    //else if(seq1[i-1] == seq2[j-1])
                    {
                        LCS[i, j] = new Tuple<int, int, int>(i-1, j-1, LCS[i-1, j-1].Item3+1);
                    }
                    else
                    {
                        if(LCS[i, j-1].Item3 >= LCS[i-1, j].Item3)
                        {
                            LCS[i, j] = new Tuple<int, int, int>(i, j-1, LCS[i, j-1].Item3);
                        }
                        else
                        {
                            LCS[i, j] = new Tuple<int, int, int>(i-1, j, LCS[i-1, j].Item3);
                        }
                    }
                }
            }

            List<object>              llcs     ;
            List<int>                 llcsidx1 ;
            List<int>                 llcsidx2 ;
            List<Tuple<Oper, object>> loper1to2;
            List<Tuple<Oper, object>> loper2to1;

            //List<Tuple<T,int,int>> lcs = new List<Tuple<T, int, int>>();
            int curr1 = size1;
            int curr2 = size2;
            int max_llcs_length = int.MaxValue;
            LongestCommonSubsequence_BackTrack
            ( seq1, seq2, equals
            , LCS
            , curr1, curr2, max_llcs_length
            , out llcs, out llcsidx1, out llcsidx2, out loper1to2, out loper2to1
            );

            LCS_src1_src2_leng = LCS;
            lcs      = llcs     .ToArray();
            lcsidx1  = llcsidx1 .ToArray();
            lcsidx2  = llcsidx2 .ToArray();
            oper1to2 = loper1to2.ToArray();
            oper2to1 = loper2to1.ToArray();
            return true;
        }
        public class OTrack
        {
            public List<object>              llcs     ;
            public List<int>                 llcsidx1 ;
            public List<int>                 llcsidx2 ;
            public List<Tuple<Oper, object>> loper1to2;
            public List<Tuple<Oper, object>> loper2to1;
        }
        public static void LongestCommonSubsequence_BackTrack
            ( IList<object> seq1, IList<object> seq2
            , Func<object,object,bool> equals
            , Tuple<int,int,int>[,] LCS
            , int curr1
            , int curr2
            , int max_llcs_length
            , out List<object>              llcs     
            , out List<int>                 llcsidx1 
            , out List<int>                 llcsidx2 
            , out List<Tuple<Oper, object>> loper1to2
            , out List<Tuple<Oper, object>> loper2to1
            )
        {
            llcs      = new List<object>();
            llcsidx1  = new List<int>   ();
            llcsidx2  = new List<int>   ();
            loper1to2 = new List<Tuple<Oper, object>>();
            loper2to1 = new List<Tuple<Oper, object>>();

            //List<Tuple<T,int,int>> lcs = new List<Tuple<T, int, int>>();
            while(!(curr1 == 0 && curr2 == 0))
            {
                if(llcs.Count >= max_llcs_length)
                    break;
                int prev1 = LCS[curr1, curr2].Item1;
                int prev2 = LCS[curr1, curr2].Item2;
                if(prev1+1==curr1 && prev2+1==curr2)
                {
                    HDebug.Assert(equals(seq1[curr1-1], seq2[curr2-1]) == true);
                    //Debug.Assert(seq1[curr1-1] == seq2[curr2-1]);
                    //lcs.Insert(0, new Tuple<object, int, int>(
                    //    seq1[curr1-1],
                    //    curr1-1,
                    //    curr2-1
                    //    ));
                    llcs     .Insert(0, seq1[prev1]);
                    llcsidx1 .Insert(0,      prev1 );
                    llcsidx2 .Insert(0,      prev2 );
                    loper1to2.Insert(0, new Tuple<Oper, object>(Oper.Match, seq1[prev1]));
                    loper2to1.Insert(0, new Tuple<Oper, object>(Oper.Match, seq1[prev1]));
                }
                else if(prev1+1==curr1 && prev2  ==curr2)
                {
                    loper1to2.Insert(0, new Tuple<Oper, object>(Oper.Delete, seq1[prev1]));
                    loper2to1.Insert(0, new Tuple<Oper, object>(Oper.Insert, seq1[prev1]));
                }
                else if(prev1  ==curr1 && prev2+1==curr2)
                {
                    loper1to2.Insert(0, new Tuple<Oper, object>(Oper.Insert, seq2[prev2]));
                    loper2to1.Insert(0, new Tuple<Oper, object>(Oper.Delete, seq2[prev2]));
                }
                else
                {
                    throw new HException("HTLib2.LCS.LongestCommonSubsequenceImpl - 2");
                }

                curr1 = prev1;
                curr2 = prev2;
            }
        }
    }
}
