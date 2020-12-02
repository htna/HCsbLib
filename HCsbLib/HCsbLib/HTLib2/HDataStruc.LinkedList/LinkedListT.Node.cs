using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class LinkedList<T>
    {
        public class Node
        {
            public T    value;
            public Node next ;
            public Node(T value, Node next)
            {
                this.value = value;
                this.next  = next ;
            }
        }
        public class DNode
        {
            public T     value;
            public DNode next ;
            public DNode prev ;
            public DNode(T value, DNode next, DNode prev)
            {
                this.value = value;
                this.next  = next ;
                this.prev  = prev ;
            }
        }
    }
}
