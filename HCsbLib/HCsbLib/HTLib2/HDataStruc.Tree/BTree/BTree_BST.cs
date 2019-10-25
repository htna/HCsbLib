using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree
    {
        ///////////////////////////////////////////////////////////////////////
        /// Binar Search Tree
        ///////////////////////////////////////////////////////////////////////
        public class BST<T>
        {
            Node<T> root;
            Comparison<T> comp;

            public static BST<T> NewBST<T>(Comparison<T> comp)
            {
                return new BST<T>
                {
                    root = null,
                    comp = comp,
                };
            }
            public (T,Node) Search(T query) { Node<T> node = BstSearch(root, query, comp); return (node.value,node); }
            public    Node  Insert(T value) { return BstInsert(ref root, value, comp); }
            public  T       Delete(T query) { return BstDelete(ref root, query, comp).value; }
            public    void  Balance()       { DSW(ref root); }
        }
        public static BST<T> NewBST<T>(Comparison<T> comp)
        {
            return BST<T>.NewBST(comp);
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
            if(BstSearch_selftest)
            {
                BstSearch_selftest = false;
                Comparison<int> _compare = delegate(int a, int b) { return a - b; };
                Node<int> _root = null;
                BstInsertRange(ref _root, new int[] { 10, 5, 20, 2, 7, 4, 6, 30, 3, 25 }, _compare);
                HDebug.Assert(_root.ToString() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,(25,30,_)))");
                HDebug.Assert((BstSearch(_root, 10, _compare) != null) ==  true);
                HDebug.Assert((BstSearch(_root, 25, _compare) != null) ==  true);
                HDebug.Assert((BstSearch(_root,  4, _compare) != null) ==  true);
                HDebug.Assert((BstSearch(_root,  7, _compare) != null) ==  true);
                HDebug.Assert((BstSearch(_root,  0, _compare) != null) == false);
                HDebug.Assert((BstSearch(_root,  9, _compare) != null) == false);
                HDebug.Assert((BstSearch(_root, 15, _compare) != null) == false);
                HDebug.Assert((BstSearch(_root, 50, _compare) != null) == false);
            }

            if(node == null)
                return null;
            int query_node = compare(query, node.value);
            if     (query_node <  0) return BstSearch(node.left , query, compare);
            else if(query_node >  0) return BstSearch(node.right, query, compare);
            else                     return node;
        }
        ///////////////////////////////////////////////////////////////////////
        /// BST Insert
        /// 
        /// 1. Insert value into BST
        /// 2. Return the inserted node
        ///////////////////////////////////////////////////////////////////////
        static bool BstInsert_selftest = true;
        static Node<T> BstInsert<T>(ref Node<T> root, T value, Comparison<T> compare)
        {
            HDebug.Assert(root == null || root.IsRoot());
            return BstInsert(null, ref root, value, compare);
        }
        static Node<T> BstInsert<T>(Node<T> parent, ref Node<T> node, T value, Comparison<T> compare)
        {
            if(BstInsert_selftest)
            {
                BstInsert_selftest = false;
                Comparison<int> _compare = delegate(int a, int b) { return a - b; };
                Node<int> _root = null;
                BstInsert(ref _root, 10, _compare); HDebug.Assert(_root.ToStringSimple() == "(10)"                                           );
                BstInsert(ref _root,  5, _compare); HDebug.Assert(_root.ToStringSimple() == "(5,10,_)"                                       );
                BstInsert(ref _root, 20, _compare); HDebug.Assert(_root.ToStringSimple() == "(5,10,20)"                                      );
                BstInsert(ref _root,  2, _compare); HDebug.Assert(_root.ToStringSimple() == "((2,5,_),10,20)"                                );
                BstInsert(ref _root,  7, _compare); HDebug.Assert(_root.ToStringSimple() == "((2,5,7),10,20)"                                );
                BstInsert(ref _root,  4, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,4),5,7),10,20)"                          );
                BstInsert(ref _root,  6, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,4),5,(6,7,_)),10,20)"                    );
                BstInsert(ref _root, 30, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,4),5,(6,7,_)),10,(_,20,30))"             );
                BstInsert(ref _root,  3, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,30))"       );
                BstInsert(ref _root, 25, _compare); HDebug.Assert(_root.ToStringSimple() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,(25,30,_)))");
            }

            if(node == null)
            {
                node = Node<T>.New(value, parent, null, null);
                return node;
            }
            if(compare(node.value, value) < 0)
            {
                return BstInsert(node, ref node.right, value, compare);
            }
            else
            {
                return BstInsert(node, ref node.left, value, compare);
            }
        }
        static IEnumerable<Node<T>> BstInsertRange<T>(ref Node<T> root, IEnumerable<T> values, Comparison<T> compare)
        {
            List<Node<T>> nodes = new List<Node<T>>();
            foreach(T value in values)
                nodes.Add(BstInsert(ref root, value, compare));
            return nodes;
        }

        ///////////////////////////////////////////////////////////////////////
        /// BST Delete
        /// 
        /// 1. Delete node whose value is same to query
        /// 2. Return the value in the deleted node
        ///////////////////////////////////////////////////////////////////////
        static (T value, Node<T> node_updated) BstDelete<T>(ref Node<T> root, T query, Comparison<T> compare)
        {
            return BstDeleteImpl(ref root, query, compare);
        }
        static (T value, Node<T> node_updated) BstDeleteImpl<T>(ref Node<T> node, T query, Comparison<T> compare)
        {
            // find node to delete
            HDebug.Assert(node != null);
            int query_node = compare(query, node.value);
            if     (query_node <  0) return BstDeleteImpl(ref node.left , query, compare);
            else if(query_node >  0) return BstDeleteImpl(ref node.right, query, compare);
            else                     return BstDeleteImpl(ref node);
        }
        static (T value, Node<T> node_updated) BstDeleteImpl<T>(ref Node<T> node)
        {
            if(node.left == null && node.right == null)
            {
                // delete a leaf
                T value = node.value;
                node = null;
                return (value, null);
            }
            else if(node.left != null && node.right == null)
            {
                // has left child
                T value = node.value;
                node = node.left;
                return (value, node);
            }
            else if(node.left == null && node.right != null)
            {
                // has right child
                T value = node.value;
                node = node.right;
                return (value, node);
            }
            else
            {
                // has both left and right children
                // 1. find predecessor reference
                ref Node<T> Pred(ref Node<T> lnode)
                {
                    if(lnode.right == null)
                        return ref lnode;
                    return ref Pred(ref lnode.right);
                };
                ref Node<T> pred = ref Pred(ref node.left);

                // 2. backup value to return
                T value = node.value;
                // 3. copy pred.value to node
                node.value = pred.value;
                // 4. delete pred; since (*pred).right == null, make pred = (*pred).left
                pred = pred.left;

                return (value, pred);
            }
        }
    }
}
