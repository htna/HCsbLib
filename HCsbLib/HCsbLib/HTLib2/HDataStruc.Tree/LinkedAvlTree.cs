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
#pragma warning disable 414
        static int _debug = 0;
#pragma warning restore 414

        // Linked-List Node
        public class LLNode
        {
            public T    value;
            public LLNode prev { get { return _prev; } }
            public LLNode next { get { return _next; } }

            internal LLNode _prev;
            internal LLNode _next;
            internal LLNode(T value, LLNode prev, LLNode next)
            {
                this.value = value;
                this._prev = prev ;
                this._next = next ;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        LLNode head;
        LLNode tail;
        BTree.AvlTree<LLNode> avl;
        Comparison<T> comp;
        int nodecomp(LLNode x, LLNode y) { return comp(x.value, y.value); }

        public LinkedAvlTree(Comparison<T> comp)
        {
            this.head = null;
            this.tail = null;
            this.comp = comp;
            this.avl  = BTree.AvlTree<LLNode>.NewAvlTree(nodecomp);
        }
        public static LinkedAvlTree<T> New(Comparison<T> comp)
        {
            return new LinkedAvlTree<T>(comp);
        }
        public LLNode GetHead()
        {
            return head;
        }
        public LLNode GetTail()
        {
            return tail;
        }
        public bool IsEmpty()
        {
            if(avl.IsEmpty() == true)
            {
                HDebug.Assert(head == null);
                HDebug.Assert(tail == null);
                return true;
            }
            else
            {
                HDebug.Assert(head != null);
                HDebug.Assert(tail != null);
                return false;
            }
        }
        public bool Contains(T query)
        {
            return (Search(query) != null);
        }
        public LLNode Search(T query)
        {
            var nodequery = new LLNode(query, null, null);

            var value = avl.Search(nodequery);

            if(value == null)
                return null;
            return value.Value.value;
        }
        public (LLNode value, LLNode parent_value) SearchWithParent(T query)
        {
            var nodequery = new LLNode(query, null, null);

            var (val, parent_val) = avl.SearchWithParent(nodequery);

            LLNode value        = null; if(val        != null) value        = val       .Value.value;
            LLNode parent_value = null; if(parent_val != null) parent_value = parent_val.Value.value;

            return (value, parent_value);
        }
        public (LLNode value, (LLNode left_value, LLNode right_value)) SearchRange(T query, bool doassert=true)
        {
            var nodequery = new LLNode(query, null, null); ;

            var (val, parent_val) = avl.SearchWithParent(nodequery);

            LLNode value, left_value, right_value;
            if(val == null)
            {
                HDebug.Assert(parent_val != null);
                LLNode parent_value = parent_val.Value.value;
                int cmp = comp(query, parent_value.value);
                HDebug.Assert(cmp != 0);
                if(cmp < 0)
                {
                    //HDebug.Assert(false);
                    //  parent->prev < query:null < parent)
                    //return (null, (parent_value.prev, parent_value));
                    value = null;
                    left_value = parent_value.prev;
                    right_value = parent_value;
                }
                else
                {
                    //HDebug.Assert(false);
                    //  parent < query:null < parent->next
                    //return (null, (parent_value, parent_value.next));
                    value = null;
                    left_value = parent_value;
                    right_value = parent_value.next;
                }
            }
            else
            {
                value       = val       .Value.value;
                left_value  = value.prev;
                right_value = value.next;
            }

            if(doassert && HDebug.IsDebuggerAttached)
            {
                if(left_value != null && left_value.value != null &&       value != null) HDebug.Assert(comp(left_value.value,       value.value) <= 0);
                if(     value != null &&      value.value != null && right_value != null) HDebug.Assert(comp(     value.value, right_value.value) <= 0);
                if(left_value != null && left_value.value != null && right_value != null) HDebug.Assert(comp(left_value.value, right_value.value) <= 0);
            }
            return (value, (left_value, right_value));
        }
        public LLNode Insert(T value)
        {
            LLNode node = new LLNode(value, null, null);

            if(avl.IsEmpty() == true)
            {
                var avlnode = avl.AvlInsert(node);
                HDebug.Assert(avlnode.value.value == node);

                node._prev = null;
                node._next = null;
                head = tail = node;
            }
            else
            {
                var avlnode = avl.AvlInsert(node);
                HDebug.Assert(avlnode.value.value == node);

                var avlnode_successor = avlnode.Successor();
                if (avlnode_successor == null)
                {
                    // added to tail
                    HDebug.Assert(avl.AvlSearch(tail).Successor().value.value == node);
                    HDebug.Assert(nodecomp(tail, node) < 0);
                    tail._next = node;
                    node._prev = tail;
                    tail = node;
                }
                else if (avlnode_successor.value.value == head)
                {
                    // added to head
                    HDebug.Assert(nodecomp(node, head) < 0);
                    head._prev = node;
                    node._next = head;
                    head = node;
                }
                else
                {
                    LLNode node_next = avlnode_successor.value.value;
                    LLNode node_prev = node_next.prev;
                    HDebug.Assert(node_prev.next == node_next);
                    HDebug.Assert(node_next.prev == node_prev);
                    HDebug.Assert(nodecomp(node_prev, node_next) < 0);
                    HDebug.Assert(nodecomp(node_prev, node     ) < 0);
                    HDebug.Assert(nodecomp(node     , node_next) < 0);
                    node._next = node_next;
                    node._prev = node_prev;
                    node_prev._next = node;
                    node_next._prev = node;
                }
            }
            return node;
        }
        public List<LLNode> InsertRange(params T[] values)
        {
            return InsertRange(values as IEnumerable<T>);
        }
        public List<LLNode> InsertRange(IEnumerable<T> values)
        {
            List<LLNode> nodes = new List<LLNode>();
            foreach(var value in values)
            {
                LLNode node = Insert(value);
                nodes.Add(node);
            }
            return nodes;
        }
        public LLNode Delete(T query)
        {
            var nodequery = new LLNode(query, null, null);

            BTree.AvlTree<LLNode>.RetT? del = avl.Delete(nodequery);
            if(del == null)
                return null;

            LLNode node = del.Value.value;

            return Delete_UpdateHeadTail(node);
        }
        internal LLNode Delete_UpdateHeadTail(LLNode node)
        {
            if(avl.IsEmpty())
            {
                HDebug.Assert(node == head);
                HDebug.Assert(node == tail);
                head = tail = null;
            }
            else if(node == head)
            {
                head = node.next;
                head._prev = null;
                node._next = null;
            }
            else if(node == tail)
            {
                tail = node.prev;
                tail._next = null;
                node._prev = null;
            }
            else
            {
                LLNode node_prev = node.prev;
                LLNode node_next = node.next;
                node_prev._next = node_next;
                node_next._prev = node_prev;
                node._next = null;
                node._prev = null;
            }

            if(HDebug.IsDebuggerAttached)
            {
                if(head != null) HDebug.Assert(head.prev == null);
                if(tail != null) HDebug.Assert(tail.next == null);
            }
            HDebug.Assert(node.prev == null);
            HDebug.Assert(node.next == null);
            return node;
        }
        public bool Validate()
        {
            return Validate(comp);
        }
        public bool Validate(Comparison<T> comp_validate)
        {
            int nodecomp_validate(LLNode x, LLNode y) { return comp_validate(x.value, y.value); }

            // check AVL validate
            if(avl.Validate(nodecomp_validate) == false) return false;

            // check linked-list validate
            LLNode n = head;
            while(n != null)
            {
                LLNode n_next = n.next;
                if(n_next != null && (nodecomp_validate(n, n_next) <= 0) == false)
                    return false;
                n = n_next;
            }

            return true;
        }
    }
}
