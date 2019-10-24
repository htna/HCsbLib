using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
        where T : class
    {
        ///////////////////////////////////////////////////////////////////////
        /// BST Search
        ///////////////////////////////////////////////////////////////////////
        static bool BstSearch_selftest = true;
        public T BstSearch(T query)
        {
            if(BstSearch_selftest)
            {
                BstSearch_selftest = false;
                Comparison<object> _compare = delegate(object a, object b) { return (int)a - (int)b; };
                BTree<object> _bst = new BTree<object>(_compare);
                _bst.BstInsertRange(new object[] { 10, 5, 20, 2, 7, 4, 6, 30, 3, 25 });
                HDebug.Assert(_bst.ToString() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,(25,30,_)))");
                HDebug.Assert((_bst.BstSearch(10) != null) ==  true);
                HDebug.Assert((_bst.BstSearch(25) != null) ==  true);
                HDebug.Assert((_bst.BstSearch( 4) != null) ==  true);
                HDebug.Assert((_bst.BstSearch( 7) != null) ==  true);
                HDebug.Assert((_bst.BstSearch( 0) != null) == false);
                HDebug.Assert((_bst.BstSearch( 9) != null) == false);
                HDebug.Assert((_bst.BstSearch(15) != null) == false);
                HDebug.Assert((_bst.BstSearch(50) != null) == false);
            }

            Node node = BstSearch(root, query);
            if(node == null)
                return default(T);
            return node.value;
        }
        Node BstSearch(Node node, T query)
        {
            if(node == null)
                return null;
            int query_node = compare(query, node.value);
            if     (query_node <  0) return BstSearch(node.left , query);
            else if(query_node >  0) return BstSearch(node.right, query);
            else                     return node;
        }
        ///////////////////////////////////////////////////////////////////////
        /// BST Insert
        /// 
        /// 1. Insert value into BST
        /// 2. Return the inserted node
        ///////////////////////////////////////////////////////////////////////
        static bool BstInsert_selftest = true;
        public Node BstInsert(T value)
        {
            if(BstInsert_selftest)
            {
                BstInsert_selftest = false;
                Comparison<object> _compare = delegate(object a, object b) { return (int)a - (int)b; };
                BTree<object> _bst = new BTree<object>(_compare);
                                    HDebug.Assert(_bst.ToString() == "()"                                             );
                _bst.BstInsert(10); HDebug.Assert(_bst.ToString() == "(10)"                                           );
                _bst.BstInsert( 5); HDebug.Assert(_bst.ToString() == "(5,10,_)"                                       );
                _bst.BstInsert(20); HDebug.Assert(_bst.ToString() == "(5,10,20)"                                      );
                _bst.BstInsert( 2); HDebug.Assert(_bst.ToString() == "((2,5,_),10,20)"                                );
                _bst.BstInsert( 7); HDebug.Assert(_bst.ToString() == "((2,5,7),10,20)"                                );
                _bst.BstInsert( 4); HDebug.Assert(_bst.ToString() == "(((_,2,4),5,7),10,20)"                          );
                _bst.BstInsert( 6); HDebug.Assert(_bst.ToString() == "(((_,2,4),5,(6,7,_)),10,20)"                    );
                _bst.BstInsert(30); HDebug.Assert(_bst.ToString() == "(((_,2,4),5,(6,7,_)),10,(_,20,30))"             );
                _bst.BstInsert( 3); HDebug.Assert(_bst.ToString() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,30))"       );
                _bst.BstInsert(25); HDebug.Assert(_bst.ToString() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,(25,30,_)))");
            }
            return BstInsert(null, ref root, value);
        }
        Node BstInsert(Node parent, ref Node node, T value)
        {
            if(node == null)
            {
                node = Node.New(value, parent, null, null);
                return node;
            }
            if(compare(node.value, value) < 0)
            {
                return BstInsert(node, ref node.right, value);
            }
            else
            {
                return BstInsert(node, ref node.left, value);
            }
        }
        public IEnumerable<Node> BstInsertRange(IEnumerable<T> values)
        {
            List<Node> nodes = new List<Node>();
            foreach(T value in values)
                nodes.Add(BstInsert(value));
            return nodes;
        }

        ///////////////////////////////////////////////////////////////////////
        /// BST Delete
        /// 
        /// 1. Delete node whose value is same to query
        /// 2. Return the value in the deleted node
        ///////////////////////////////////////////////////////////////////////
        public (T value, Node node_updated) BstDelete(T query)
        {
            return BstDelete(ref root, query);
        }
        (T value, Node node_updated) BstDelete(ref Node node, T query)
        {
            // find node to delete
            HDebug.Assert(node != null);
            int query_node = compare(query, node.value);
            if     (query_node <  0) return BstDelete(ref node.left , query);
            else if(query_node >  0) return BstDelete(ref node.right, query);
            else                     return BstDelete(ref node);
        }
        (T value, Node node_updated) BstDelete(ref Node node)
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
                // 4. delete pred; since (*pred).right == null, make pred = (*pred).left
                pred = pred.left;

                return (value, pred);
            }
        }
    }
}
