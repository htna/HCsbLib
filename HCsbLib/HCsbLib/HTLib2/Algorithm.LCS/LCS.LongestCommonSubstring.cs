using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace HTLib2
{
    public partial class LCS
    {
        public static int LongestCommonSubstring(string str1, string str2)
        {
            /// source: http://en.wikibooks.org/wiki/Algorithm_implementation/Strings/Longest_common_substring
            /// 
            if(String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2))
                return 0;

            int[,] num = new int[str1.Length, str2.Length];
            int maxlen = 0;

            for(int i = 0; i < str1.Length; i++)
            {
                for(int j = 0; j < str2.Length; j++)
                {
                    if(str1[i] != str2[j])
                        num[i, j] = 0;
                    else
                    {
                        if((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if(num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                        }
                    }
                }
            }
            return maxlen;
        }
        public static int LongestCommonSubstring<T>( IList<T> str1, IList<T> str2
                                                   , IEqualityComparer<T> comparer
                                                   , HPack<int> maxBegin1 = null
                                                   , HPack<int> maxBegin2 = null
                                                   , HPack<List<T>> maxSequence1 = null
                                                   , HPack<List<T>> maxSequence2 = null
                                                   )
        {
            Func<T, T, bool> funcEquals = delegate(T x, T y)
            {
                return comparer.Equals(x, y);
            };
            return LongestCommonSubstring( str1, str2
                                         , maxBegin1: maxBegin1
                                         , maxBegin2: maxBegin2
                                         , maxSequence1: maxSequence1
                                         , maxSequence2: maxSequence2
                                         , funcEquals: funcEquals
                                         );
        }
        public static int LongestCommonSubstring<T>( IList<T> str1, IList<T> str2
                                                   , HPack<int> maxBegin1 = null
                                                   , HPack<int> maxBegin2 = null
                                                   , HPack<List<T>> maxSequence1 = null
                                                   , HPack<List<T>> maxSequence2 = null
                                                   , Func<T, T, bool> funcEquals = null
                                                   //, IEqualityComparer<T> comparer = null
                                                   )
        {
            if(funcEquals == null)
            {
                funcEquals = delegate(T x, T y)
                {
                    return EqualityComparer<T>.Default.Equals(x, y);
                };
            }

            if(str1 == null || str1.Count == 0 || str2 == null || str2.Count == 0)
                return 0;

            int[,] num = new int[str1.Count, str2.Count];
            int maxlen = 0;
            int maxlenIdx1 = -1;
            int maxlenIdx2 = -1;

            for(int i = 0; i < str1.Count; i++)
            {
                for(int j = 0; j < str2.Count; j++)
                {
                    if(funcEquals(str1[i], str2[j]) == false)
                        num[i, j] = 0;
                    else
                    {
                        if((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if(num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                            maxlenIdx1 = i;
                            maxlenIdx2 = j;
                        }
                    }
                }
            }
            if(maxBegin1    != null) maxBegin1.value    = maxlenIdx1-maxlen+1;
            if(maxBegin2    != null) maxBegin2.value    = maxlenIdx2-maxlen+1;
            if(maxSequence1 != null) maxSequence1.value = str1.HTake(maxlenIdx1+1-maxlen, maxlen);
            if(maxSequence2 != null) maxSequence2.value = str2.HTake(maxlenIdx2+1-maxlen, maxlen);
            return maxlen;
        }
        public static int LongestCommonSubstring<T>( IList<T>[] strs
                                                   //, Pack<int[]> maxBegins = null
                                                   , HPack<List<T>[]> maxSequences = null
                                                   , IEqualityComparer<T> comparer = null
                                                   )
        {
            List<T>[] lstrs = new List<T>[strs.Length];
            for(int i=0; i<strs.Length; i++)
                lstrs[i] = new List<T>(strs[i]);

            bool repeat = true;
            while(repeat)
            {
                repeat = false;
                for(int i=1; i<lstrs.Length; i++)
                {
                    IList<T> str1 = lstrs[0];
                    IList<T> str2 = lstrs[i];
                    HPack<List<T>> maxSequence1 = new HPack<List<T>>();
                    HPack<List<T>> maxSequence2 = new HPack<List<T>>();
                    LCS.LongestCommonSubstring( str1, str2
                                              , maxSequence1: maxSequence1
                                              , maxSequence2: maxSequence2
                                              , comparer: comparer
                                              );
                    if(str1.Count != maxSequence1.value.Count) repeat = true; // sequence length is changed
                    if(str2.Count != maxSequence2.value.Count) repeat = true; // sequence length is changed
                    lstrs[0] = maxSequence1.value;
                    lstrs[i] = maxSequence2.value;
                }
                for(int i=1; i<lstrs.Length; i++)
                    if(lstrs[0].Count != lstrs[i].Count)
                        repeat = true;
            }
            if(maxSequences != null)
                maxSequences.value = lstrs;
            return lstrs[0].Count;
        }
    }
}
