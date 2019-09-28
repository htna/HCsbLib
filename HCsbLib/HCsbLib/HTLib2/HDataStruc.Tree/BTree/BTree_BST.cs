using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        ///////////////////////////////////////////////////////////////////////
        /// BST Insert
        /// 
        /// 1. Insert value into BST
        /// 2. Return the inserted node
        ///////////////////////////////////////////////////////////////////////
        static bool BstInsert_selftest = true;
        public Node<T> BstInsert(T value)
        {
            if(BstInsert_selftest)
            {
                BstInsert_selftest = false;
                Comparison<int> _compare = delegate(int a, int b) { return a - b; };
                BTree<int> _bst = new BTree<int>(_compare);
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
        Node<T> BstInsert(Node<T> parent, ref Node<T> node, T value)
        {
            if(node == null)
            {
                node = Node<T>.New(value, parent, null, null);
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

        ///////////////////////////////////////////////////////////////////////
        /// BST Delete
        /// 
        /// 1. Delete node whose value is same to query
        /// 2. Return the value in the deleted node
        ///////////////////////////////////////////////////////////////////////
        public T BstDelete(T query)
        {
            return BstDelete(ref root, query);
        }
        T BstDelete(ref Node<T> node, T query)
        {
            // find node to delete
            HDebug.Assert(node != null);
            int query_node = compare(query, node.value);
            if     (query_node <  0) return BstDelete(ref node.left , query);
            else if(query_node >  0) return BstDelete(ref node.right, query);
            else                     return BstDelete(ref node);
        }
        T BstDelete(ref Node<T> node)
        {
            if(node.left == null && node.right == null)
            {
                // delete a leaf
                T value = node.value;
                node = null;
                return value;
            }
            else if(node.left != null && node.right == null)
            {
                // has left child
                T value = node.value;
                node = node.left;
                return value;
            }
            else if(node.left == null && node.right != null)
            {
                // has right child
                T value = node.value;
                node = node.right;
                return value;
            }
            else
            {
                // has both left and right children
                T value = node.value;
                T pred_value = BstDeleteMax(ref node.left);
                node.value = pred_value;
                return value;
            }
        }
        T BstDeleteMax(ref Node<T> node)
        {
            if(node.right != null)
                return BstDeleteMax(ref node.right);
            T value = node.value;
            node = node.left;
            return value;
        }
    }
}
