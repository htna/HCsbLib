//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Runtime.CompilerServices;


namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static bool HBinarySearchSelftest_selftest = HDebug.IsDebuggerAttached;

		[System.Diagnostics.Conditional("DEBUG")]
		//[System.Diagnostics.DebuggerHiddenAttribute()]
        public static void HBinarySearchSelftest()
        {
            if(HBinarySearchSelftest_selftest == false)
                return;
            HBinarySearchSelftest_selftest = false;

            List<int> list = new List<int>(new int[] {1,3,5,7,9});

            (bool found, int idx_leq, int idx_geq) find;

            find = list.HBinarySearch(1); HDebug.Assert(find.found == true && find.idx_leq == 0 && find.idx_geq == 0);
            find = list.HBinarySearch(3); HDebug.Assert(find.found == true && find.idx_leq == 1 && find.idx_geq == 1);
            find = list.HBinarySearch(5); HDebug.Assert(find.found == true && find.idx_leq == 2 && find.idx_geq == 2);
            find = list.HBinarySearch(7); HDebug.Assert(find.found == true && find.idx_leq == 3 && find.idx_geq == 3);
            find = list.HBinarySearch(9); HDebug.Assert(find.found == true && find.idx_leq == 4 && find.idx_geq == 4);

            find = list.HBinarySearch( 0); HDebug.Assert(find.found == false && find.idx_leq == -1 && find.idx_geq ==  0);
            find = list.HBinarySearch( 2); HDebug.Assert(find.found == false && find.idx_leq ==  0 && find.idx_geq ==  1);
            find = list.HBinarySearch( 4); HDebug.Assert(find.found == false && find.idx_leq ==  1 && find.idx_geq ==  2);
            find = list.HBinarySearch( 6); HDebug.Assert(find.found == false && find.idx_leq ==  2 && find.idx_geq ==  3);
            find = list.HBinarySearch( 8); HDebug.Assert(find.found == false && find.idx_leq ==  3 && find.idx_geq ==  4);
            find = list.HBinarySearch(10); HDebug.Assert(find.found == false && find.idx_leq ==  4 && find.idx_geq ==  5);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (bool found, int idx_leq, int idx_geq) HBinarySearch<T>(this List<T> list, T item)
        {
            HBinarySearchSelftest();
            /// Returns:
            ///     if item is found, return the zero-based index of item in the sorted System.Collections.Generic.List`1, ;
            ///     if there is no larger element, return the bitwise complement of System.Collections.Generic.List`1.Count.
            ///     otherwise, return a negative number that is the bitwise complement of the index of the next element that is larger than item or
            int idx = list.BinarySearch(item);

            if(idx >= 0)
                // found
                return (true, idx, idx);

            idx = ~idx;
            if(idx == 0)
                // item is smaller than all
                return (false, -1, 0);
            if(idx == list.Count)
                // item is larger than all
                return (false, list.Count-1, list.Count);
            // item is in-between idx-1, 
            return (false, idx-1, idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (bool found, int idx_leq, int idx_geq) HBinarySearch<T>(this List<T> list, T item, IComparer<T> comparer)
        {
            HBinarySearchSelftest();
            /// Returns:
            ///     if item is found, return the zero-based index of item in the sorted System.Collections.Generic.List`1, ;
            ///     if there is no larger element, return the bitwise complement of System.Collections.Generic.List`1.Count.
            ///     otherwise, return a negative number that is the bitwise complement of the index of the next element that is larger than item or
            int idx = list.BinarySearch(item, comparer);

            if(idx >= 0)
                // found
                return (true, idx, idx);

            idx = ~idx;
            if(idx == 0)
                // item is smaller than all
                return (false, -1, 0);
            if(idx == list.Count)
                // item is larger than all
                return (false, list.Count-1, list.Count);
            // item is in-between idx-1, 
            return (false, idx-1, idx);
        }
    }
}
