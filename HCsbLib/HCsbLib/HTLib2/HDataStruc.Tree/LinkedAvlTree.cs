using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    /// <summary>
    /// AVL tree whose nodes are linked as a linked-list in increasing order
    /// </summary>
    public partial class LinkedAvlTree<T>
    {
        public class Node
        {
            public T    value;
            public Node prev { get { return _prev; } }
            public Node next { get { return _next; } }

            Node _prev;
            Node _next;
            Node(T value, Node prev, Node next)
            {
                this.value = value;
                this._prev = prev ;
                this._next = next ;
            }
        }

        Node _head;
        Node _tail;
        //BTree.AvlTree
    }
}
