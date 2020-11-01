/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    /// <summary>
    /// AVL tree whose nodes are linked as a linked-list in increasing order
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    /// Binar Search Tree
    ///////////////////////////////////////////////////////////////////////
    public class LinkedBST<T>
    {
        public struct RetT
        {
            public T value;
            public static RetT New(T val) { return new RetT { value = val }; }
        }

        public class Node : BTree.Node
        {
            public T    value ;
            public Node parent;
            public Node left  ;
            public Node right ;
            public Node prev  ;
            public Node next  ;
        }

        Node          root;
        int           cont;
        Comparison<T> comp;
        public Comparison<T> Comp { get { return comp; } }
        public void ChangeComp(Comparison<T> comp)
        {
            this.comp = comp;
            HDebug.Assert(Validate());
        }

        public LinkedBST(Comparison<T> comp)
        {
            this.root = null;
            this.cont = 0;
            this.comp = comp;
        }

        public bool  IsEmpty ()        { return (root == null); }
        public bool  Contains(T query) { return (Search(query) != null); }
        public T?    Search  (T query) { Node<T> node = BstSearchWithParent(    root, null, query, _comp).ret; if(node == null) return null; return RetT.New(node.value); }
        public void  Insert  (T value) { Node<T> node = BstInsert          (ref root      , value, _comp)    ; if(node == null) return null; return RetT.New(node.value); }
        public void  Delete  (T query) { var     del  = BstDelete          (ref root      , query, _comp)    ; if(del  == null) return null ; return RetT.New(del.Value.value); }
        public bool  Validate()
        {
            if(BstValidateConnection(root) == false) return false;
            if(BstValidateOrder(root, _comp) == false) return false;
            return true;
        }

        public Node SearchNode(T query)
        {
            Node node = root;
            while(node != null)
            {
                int cmp = comp(query, node.value);
                if(cmp == 0)
                {
                    return node;
                }
                else if(cmp < 0)
                {
                    // query < node
                    node = node.left;
                }
                else
                {
                    // node < query
                    node = node.right;
                }
            }
            return null;
        }
        ///////////////////////////////////////////////////////////////////////
        /// BST Insert
        /// 
        /// 1. Insert value into BST
        /// 2. Return the inserted node
        ///////////////////////////////////////////////////////////////////////
        static bool InsertNode_selftest = true;
        public Node InsertNode(T value)
        {
            if(InsertNode_selftest)
            {
                InsertNode_selftest = false;
                Comparison<int> _compare = delegate(int a, int b) { return a - b; };
                //Node<int> _root = null;
                //BstInsert(ref _root, 10, _compare); HDebug.Assert(_root.ToStringSimple() == "(10)"                                           );
                //BstInsert(ref _root,  5, _compare); HDebug.Assert(_root.ToStringSimple() == "(5,10,_)"                                       );
                //BstInsert(ref _root, 20, _compare); HDebug.Assert(_root.ToStringSimple() == "(5,10,20)"                                      );
                //BstInsert(ref _root,  2, _compare); HDebug.Assert(_root.ToStringSimple() == "((2,5,_),10,20)"                                );
                //BstInsert(ref _root,  7, _compare); HDebug.Assert(_root.ToStringSimple() == "((2,5,7),10,20)"                                );
                //BstInsert(ref _root,  4, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,4),5,7),10,20)"                          );
                //BstInsert(ref _root,  6, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,4),5,(6,7,_)),10,20)"                    );
                //BstInsert(ref _root, 30, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,4),5,(6,7,_)),10,(_,20,30))"             );
                //BstInsert(ref _root,  3, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,30))"       );
                //BstInsert(ref _root, 25, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,(25,30,_)))");
            }

            if(root == null)
            {
                root = new Node
                {
                    value  = value,
                    parent = null,
                    left   = null,
                    right  = null,
                    prev   = null,
                    next   = null,
                };
                return root;
            }
            else
            {
                Node p = root;
                while(true)
                {
                    int cmp = comp(value, p.value);
                    if(cmp < 0)
                    {
                        if(p.left == null)
                        {
                            Node node = new Node
                            {
                                value  = value,
                                parent = p,
                                left   = null,
                                right  = null,
                                prev   = p.prev,
                                next   = p,
                            };
                            p.prev.next = node;
                            p.prev      = node;
                            p.left      = node;
                            return node;
                        }
                        else
                        {
                            p = p.left;
                        }
                    }
                    else
                    {
                        if(p.right == null)
                        {
                            Node node = new Node
                            {
                                value  = value,
                                parent = p,
                                left   = null,
                                right  = null,
                                prev   = p,
                                next   = p.next,
                            };
                            p.next.prev = node;
                            p.next      = node;
                            p.right     = node;
                            return node;
                        }
                        else
                        {
                            p = p.right;
                        }
                    }
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////
        /// BST Delete
        /// 
        /// 1. Delete node whose value is same to query
        /// 2. Return the value in the deleted node
        ///////////////////////////////////////////////////////////////////////
        (T value, Node deleted_parent)? DeleteNode(ref Node root, T query)
        {
            return DeleteNodeImpl(ref root, query);
        }
        (T value, Node deleted_parent)? DeleteNodeImpl(ref Node node, T query)
        {
            // find node to delete
            HDebug.Assert(node != null);
            int query_node = comp(query, node.value);
            if     (query_node <  0) return DeleteNodeImpl(ref node.left , query);
            else if(query_node >  0) return DeleteNodeImpl(ref node.right, query);
            else if(query_node == 0) return DeleteNodeImpl(ref node);
            else                     return null;
        }
        (T value, Node deleted_parent) DeleteNodeImpl(ref Node node)
        {
            if(node.left == null && node.right == null)
            {
                // delete a leaf
                T    value  = node.value;
                Node parent = node.parent;
                node = null;
                return (value, parent);
            }
            else if(node.left != null && node.right == null)
            {
                // has left child
                T    value  = node.value;
                Node parent = node.parent;
                node = node.left;
                node.parent = parent;
                return (value, parent);
            }
            else if(node.left == null && node.right != null)
            {
                // has right child
                T    value  = node.value;
                Node parent = node.parent;
                node = node.right;
                node.parent = parent;
                return (value, parent);
            }
            else
            {
                // has both left and right children
                // 1. find predecessor reference
                ref Node Pred(ref Node lnode)
                {
                    if(lnode.right == null)
                        return ref lnode;
                    return ref Pred(ref lnode.right);
                };
                ref Node pred = ref Pred(ref node.left);

                // 2. backup value to return
                T value = node.value;
                // 3. copy pred.value to node
                node.value = pred.value;
                // 4. node updated
                Node pred_parent = pred.parent;
                // 4. delete pred; since (*pred).right == null, make pred = (*pred).left

                pred = pred.left;
                if(pred != null)
                    pred.parent = pred_parent;

                return (value, pred_parent);
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////
    /// Validate connections
    ///////////////////////////////////////////////////////////////////////
    static bool BstValidateConnection<T>(Node<T> root)
    {
        if(root.parent != null)
            return false;
        if(root.ValidateConnection() == false)
            return false;
        return true;
    }
    ///////////////////////////////////////////////////////////////////////
    /// Validate order
    ///////////////////////////////////////////////////////////////////////
    static bool BstValidateOrder<T>(Node<T> root, Comparison<T> compare)
    {
        if(root == null)
            return true;
        int    count = root.Count();
        Node<T> node = root.MinNode();
        Node<T> next = node.Successor();
        if(next == null)
            return true;
        int num_compare = 0;
        while(next != null)
        {
            if(compare(node.value, next.value) > 0)
                return false;
            num_compare ++;
            node = next;
            next = next.Successor();
        }
        if(num_compare != count-1)
            return false;
        return true;
    }
    ///////////////////////////////////////////////////////////////////////
    /// BST Search
    ///////////////////////////////////////////////////////////////////////
    static bool BstSearch_selftest = true;
    //public T BstSearch(T query)
    //{
    //    Node node = BstSearch(root, query);
    //    if(node == null)
    //        return default(T);
    //    return node.value;
    //}
    static Node<T> BstSearch<T>(Node<T> node, T query, Comparison<T> compare)
    {
        (Node<T> ret, Node<T> ret_parent) = BstSearchWithParent(node, null, query, compare);
        return ret;
    }
    static (Node<T> ret, Node<T> ret_parent) BstSearchWithParent<T>(Node<T> node, Node<T> node_parent, T query, Comparison<T> compare)
    {
        if(BstSearch_selftest)
        {
            BstSearch_selftest = false;
            Comparison<int> _compare = delegate(int a, int b) { return a - b; };
            Node<int> _root = null;
            BstInsertRange(ref _root, new int[] { 10, 5, 20, 2, 7, 4, 6, 30, 3, 25 }, _compare);
            HDebug.Assert(_root.ToString() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,(25,30,_)))");
            HDebug.Assert((BstSearchWithParent(_root, null, 10, _compare).ret != null) ==  true);
            HDebug.Assert((BstSearchWithParent(_root, null, 25, _compare).ret != null) ==  true);
            HDebug.Assert((BstSearchWithParent(_root, null,  4, _compare).ret != null) ==  true);
            HDebug.Assert((BstSearchWithParent(_root, null,  7, _compare).ret != null) ==  true);
            HDebug.Assert((BstSearchWithParent(_root, null,  0, _compare).ret != null) == false);
            HDebug.Assert((BstSearchWithParent(_root, null,  9, _compare).ret != null) == false);
            HDebug.Assert((BstSearchWithParent(_root, null, 15, _compare).ret != null) == false);
            HDebug.Assert((BstSearchWithParent(_root, null, 50, _compare).ret != null) == false);
        }

        if(node == null)
            return (null, node_parent);
        int query_node = compare(query, node.value);
        if     (query_node <  0) return BstSearchWithParent(node.left , node, query, compare);
        else if(query_node >  0) return BstSearchWithParent(node.right, node, query, compare);
        else                     return (node, node_parent);
    }

}
*/