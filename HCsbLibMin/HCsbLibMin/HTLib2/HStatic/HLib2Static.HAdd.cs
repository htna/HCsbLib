using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static bool HLinkedList_selftest = HDebug.IsDebuggerAttached;

        [System.Diagnostics.Conditional("DEBUG")]
        public static void HLinkedList_SelfTest()
        {
            if(HLinkedList_selftest == false)
                return;

            HLinkedList_selftest = false;

            LinkedList<int> list = new LinkedList<int>();
            list.AddLast(3);
            list.AddLast(4);
            list.AddLast(5);
            list.AddLast(6);
            int val;

            val = 3;
            foreach(var item in list)
            {
                HDebug.Assert(val == item);
                val ++;
            }

            list.HAddFirstRange(new int[] { 0, 1, 2 });

            val = 0;
            foreach(var item in list)
            {
                HDebug.Assert(val == item);
                val ++;
            }

            list.HAddLastRange(new int[] { 7, 8, 9 });

            val = 0;
            foreach(var item in list)
            {
                HDebug.Assert(val == item);
                val ++;
            }
        }

        public static void HAddLastRange<T>(this LinkedList<T> list, IEnumerable<T> collection)
        {
            HLinkedList_SelfTest();

            foreach(var item in collection)
                list.AddLast(item);
        }

        public static void HAddFirstRange<T>(this LinkedList<T> list, IEnumerable<T> collection)
        {
            HLinkedList_SelfTest();

            LinkedListNode<T> node = null;
            foreach(var item in collection)
            {
                if(node == null)
                {
                    node = list.AddFirst(item);
                }
                else
                {
                    node = list.AddAfter(node, item);
                }
            }
        }
    }
}
