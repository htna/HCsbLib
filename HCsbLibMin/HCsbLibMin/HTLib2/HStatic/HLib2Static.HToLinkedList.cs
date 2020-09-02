using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static LinkedList<T> HToLinkedList<T>(this IEnumerable<List<T>> listss)
        {
            LinkedList<T> llist = new LinkedList<T>();
            foreach(var list in listss)
                foreach(T item in list)
                    llist.AddLast(item);
            return llist;
        }
        public static LinkedList<T> HToLinkedList<T>(this IEnumerable<IList<T>> listss)
        {
            LinkedList<T> llist = new LinkedList<T>();
            foreach(var list in listss)
                foreach(T item in list)
                    llist.AddLast(item);
            return llist;
        }
        public static LinkedList<T> HToLinkedList<T>(this IEnumerable<IEnumerable<T>> listss)
        {
            LinkedList<T> llist = new LinkedList<T>();
            foreach(var list in listss)
                foreach(T item in list)
                    llist.AddLast(item);
            return llist;
        }
    }
}
