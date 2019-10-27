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
            internal Node(T value, Node prev, Node next)
            {
                this.value = value;
                this._prev = prev ;
                this._next = next ;
            }
        }

        Node head;
        Node tail;
        BTree.AvlTree<Node> avl;
        Comparison<T> comp;
        int nodecomp(Node x, Node y) { return comp(x.value, y.value); }

        public LinkedAvlTree(Comparison<T> comp)
        {
            this.head = null;
            this.tail = null;
            this.comp = comp;
            this.avl  = BTree.AvlTree<Node>.NewAvlTree(nodecomp);
        }
        public static LinkedAvlTree<T> New(Comparison<T> comp)
        {
            return new LinkedAvlTree<T>(comp);
        }
        public bool Contains(T query)
        {
            return (Search(query) != null);
        }
        public Node Search(T query)
        {
            var nodequery = new Node(query, null, null);;

            var value = avl.Search(nodequery);

            if(value == null)
                return null;
            return value.Value.value;
        }
        //  public bool Insert(T value)
        //  {
        //      Node node = new Node(value, null, null);
        //      avl.Insert(node);
        //  
        //      Node<AvlNodeInfo> node = AvlInsert(value);
        //      if(node == null)
        //          return false;
        //      return true;
        //  }
        //  public bool[] InsertRange(params T[] values)
        //  {
        //      return InsertRange(values as IEnumerable<T>);
        //  }
        //  public bool[] InsertRange(IEnumerable<T> values)
        //  {
        //      List<Node<AvlNodeInfo>> inserteds = AvlInsertRange(values);
        //      bool[] results = new bool[inserteds.Count];
        //      for(int i=0; i<results.Length; i++)
        //          results[i] = (inserteds[i] != null);
        //      return results;
        //  }
    }
}
