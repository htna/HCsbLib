using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    using T = System.Int32;
    public static partial class BTree
    {
        public static AvlTree NewAvlTree(Comparison<T> comp=null)
        {
            return AvlTree.NewAvlTree(comp);
        }
        ///////////////////////////////////////////////////////////////////////
        /// AVL Tree
        ///////////////////////////////////////////////////////////////////////
        public class AvlTree
        {
            public struct AvlNodeInfo
            {
                public T   value;
                public int left_height;
                public int right_height;
                public int height { get { return Math.Max(left_height, right_height) + 1; } }
                public int bf     { get { return right_height - left_height; } }
                public override string ToString() { return value.ToString(); }
            }
            Node<AvlNodeInfo> root;
            Comparison<T> comp;
            public int avlcomp(AvlNodeInfo x, AvlNodeInfo y) { return comp(x.value, y.value); }

            public static AvlTree NewAvlTree(Comparison<T> comp)
            {
                if(comp == null)
                {
                    var compr = Comparer<T>.Default;
                    comp = delegate(T x, T y)
                    {
                        return compr.Compare(x,y);
                    };
                }

                return new AvlTree
                {
                    root = null,
                    comp = comp,
                };
            }

            public override string ToString()
            {
                string str = root.ToStringSimple();
                return str;
            }
            
            public (T,Node) Search(T query) { var node = BstSearch<AvlNodeInfo>(root, new AvlNodeInfo{value = query}, avlcomp); return (node.value.value, node); }
            //public      T  Delete(T query) { return BstDelete(ref root, query, comp).value; }
            //public    void Balance()       { DSW(ref root); }

            ///////////////////////////////////////////////////////////////////////
            /// AVL Validate
            ///////////////////////////////////////////////////////////////////////
            public bool Validate()
            {
                if(root == null)
                    return true;

                if(ValidateBalance() == null) return false;
                if(root.ValidateConnection() == false) return false;
                if(BTree.BstValidateOrder(root, avlcomp) == false) return false;
                return true;
            }
            bool ValidateBalance()
            {
                if(ValidateBalance(root) == null)
                    return false;
                return true;

                int? ValidateBalance(Node<AvlNodeInfo> node)
                {
                    if(node == null)
                        return -1;
                    int? lh = ValidateBalance(node.left);
                    int? rh = ValidateBalance(node.right);
                    if(lh == null) return null;
                    if(rh == null) return null;
                    if(lh.Value != node.value.left_height ) { HDebug.Assert(false); return null; }
                    if(rh.Value != node.value.right_height) { HDebug.Assert(false); return null; }
                    if(Math.Abs(rh.Value - lh.Value) >= 2 ) { HDebug.Assert(false); return null; }
                    int h = Math.Max(lh.Value, rh.Value) + 1;
                    return h;
                }
            }

            ///////////////////////////////////////////////////////////////////////
            /// AVL Insert
            /// 
            /// 1. Insert value into BST
            /// 2. Rebalance
            /// 3. Return the inserted node
            ///////////////////////////////////////////////////////////////////////
            static bool Insert_selftest = HDebug.IsDebuggerAttached;
            public static void InsertSelftest()
            {
                if(Insert_selftest == false)
                    return;

                Insert_selftest = false;
                {
                    var avltree = BTree.NewAvlTree();
                    HDebug.Assert(avltree.Validate()); 
                    avltree.Insert( 1); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(1)");
                    avltree.Insert( 2); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(_,1,2)");
                    avltree.Insert( 3); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(1,2,3)");
                }
                {
                    var avltree = BTree.NewAvlTree();
                    HDebug.Assert(avltree.Validate()); 
                    avltree.Insert( 4); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(4)");
                    avltree.Insert( 3); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(3,4,_)");
                    avltree.Insert( 9); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(3,4,9)");
                    avltree.Insert( 2); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((2,3,_),4,9)");
                    avltree.Insert(11); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((2,3,_),4,(_,9,11))");
                    avltree.Insert( 0); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(_,9,11))");
                    avltree.Insert(15); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(9,11,15))");
                    avltree.Insert(17); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(9,11,(_,15,17)))");
                    avltree.Insert(14); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(9,11,(14,15,17)))");
                    avltree.Insert(12); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((0,2,3),4,((9,11,12),14,(_,15,17)))");
                }
                {
                    var avltree = BTree.NewAvlTree();
                    HDebug.Assert(avltree.Validate()); 
                    avltree.Insert( 4); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(4)");
                    avltree.Insert( 3); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(3,4,_)");
                    avltree.Insert( 9); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(3,4,9)");
                    avltree.Insert( 2); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((2,3,_),4,9)");
                    avltree.Insert(11); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((2,3,_),4,(_,9,11))");
                    avltree.Insert(-1); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((-1,2,3),4,(_,9,11))");
                    avltree.Insert(15); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "((-1,2,3),4,(9,11,15))");
                    avltree.Insert( 0); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(((_,-1,0),2,3),4,(9,11,15))");
                    avltree.Insert(-2); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(((-2,-1,0),2,3),4,(9,11,15))");
                    avltree.Insert( 1); HDebug.Assert(avltree.Validate()); HDebug.Assert(avltree.ToString() == "(((-2,-1,_),0,(1,2,3)),4,(9,11,15))");
                }
            }
            public Node<AvlNodeInfo>[] InsertRange(params T[] values)
            {
                return _InsertRange(values);
            }
            public Node<AvlNodeInfo>[] InsertRange(IEnumerable<T> values)
            {
                return _InsertRange(values);
            }
            Node<AvlNodeInfo>[] _InsertRange(IEnumerable<T> values)
            {
                List<Node<AvlNodeInfo>> inserteds = new List<Node<AvlNodeInfo>>();
                foreach(var value in values)
                {
                    Node<AvlNodeInfo> node = Insert(value);
                    inserteds.Add(node);
                }
                return inserteds.ToArray();
            }
            public Node<AvlNodeInfo> Insert(T value)
            {
                HDebug.Assert(root == null || root.IsRoot());
                AvlNodeInfo avlvalue = new AvlNodeInfo
                {
                    value  = value,
                    left_height  = -1, // height of null node is -1
                    right_height = -1, // height of null node is -1
                };

                if(root == null)
                {
                    Node<AvlNodeInfo> node = BstInsert<AvlNodeInfo>(null, ref root, avlvalue, avlcomp);
                    HDebug.Assert(root == node);
                    HDebug.Assert(root.left  == null);
                    HDebug.Assert(root.right == null);
                    HDebug.Assert(root.value.height == 0);
                    return node;
                }
                else
                {
                    Node<AvlNodeInfo> node = BstInsert<AvlNodeInfo>(null, ref root, avlvalue, avlcomp);
                    HDebug.Assert(node.value.height == 0);
                    UpdateBalance(node, ref root);
                    return node;
                }
            }
            //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
            //static void UpdateParentHeight(Node<AvlNodeInfo> node)
            //{
            //    Node<AvlNodeInfo> parent = node.parent;
            //    HDebug.Assert(parent != null);
            //    HDebug.Assert((node == parent.left ) || (node == parent.right));
            //
            //    if(node == parent.left ) parent.value.left_height  = node.value.height;
            //    if(node == parent.right) parent.value.right_height = node.value.height;
            //}
            ///////////////////////////////////////////////////////////////////////
            /// AVL Update height
            /// AVL Update Balance (rebalance)
            ///////////////////////////////////////////////////////////////////////
            static void UpdateHeight(Node<AvlNodeInfo> node)
            {
                node.value.left_height  = (node.left  == null) ? -1 : node.left .value.height;
                node.value.right_height = (node.right == null) ? -1 : node.right.value.height;
            }
            static void UpdateBalance(Node<AvlNodeInfo> node, ref Node<AvlNodeInfo> root)
            {
                Node<AvlNodeInfo> parent = node.parent;
                while(parent != null)
                {
                    (node, parent) = UpdateBalance(node, parent, ref root);
                    node   = parent;
                    parent = node.parent;
                }
            }
            // update balance of (node, parent), and return (redetermined node, redetermined parent)
            static (Node<AvlNodeInfo> nnode, Node<AvlNodeInfo> nparent) UpdateBalance(Node<AvlNodeInfo> node, Node<AvlNodeInfo> parent, ref Node<AvlNodeInfo> root)
            {
                HDebug.Assert(node   != null);
                HDebug.Assert(parent != null);
                HDebug.Assert(parent == node.parent);

                UpdateHeight(parent);

                int   node_bf =   node.value.bf;
                int parent_bf = parent.value.bf;

                Node<AvlNodeInfo> nnode   = null;
                Node<AvlNodeInfo> nparent = null;

                if((Math.Abs(node_bf) <= 1) && (Math.Abs(parent_bf) <= 1))
                {
                    nnode   = node  ;
                    nparent = parent;
                }
                else if((node_bf == -1) && (parent_bf == -2))
                {
                    //HDebug.Assert(parent_bf == -2);
                    ref Node<AvlNodeInfo> parent_ref = ref parent.GetThisRef(ref root);
                    BTree.RotateRight<AvlNodeInfo>(ref parent_ref);
                    UpdateHeight(parent);
                    UpdateHeight(node  );
                    HDebug.Assert(parent.parent == node);

                    nnode   = parent;
                    nparent = node;
                }
                else if((node_bf == 1) && (parent_bf == -2))
                {
                    //HDebug.Assert(parent_bf == -2);
                    ref Node<AvlNodeInfo> node_ref = ref node.GetThisRef(ref root);
                    BTree.RotateLeft<AvlNodeInfo>(ref node_ref);
                    ref Node<AvlNodeInfo> parent_ref = ref parent.GetThisRef(ref root);
                    BTree.RotateRight<AvlNodeInfo>(ref parent_ref);
                    HDebug.Assert(node.parent == parent.parent);
                    UpdateHeight(parent);
                    UpdateHeight(node  );
                    UpdateHeight(node.parent);

                    nnode   = node;
                    nparent = node.parent;
                }
                else if((node_bf == 1) && (parent_bf == 2))
                {
                    //HDebug.Assert(parent_bf == 2);
                    ref Node<AvlNodeInfo> parent_ref = ref parent.GetThisRef(ref root);
                    BTree.RotateLeft<AvlNodeInfo>(ref parent_ref);
                    UpdateHeight(parent);
                    UpdateHeight(node  );
                    HDebug.Assert(parent.parent == node);

                    nnode   = parent;
                    nparent = node;
                }
                else if((node_bf == -1) && (parent_bf == 2))
                {
                    //HDebug.Assert(parent_bf == 2);
                    ref Node<AvlNodeInfo> node_ref = ref node.GetThisRef(ref root);
                    BTree.RotateRight<AvlNodeInfo>(ref node_ref);
                    ref Node<AvlNodeInfo> parent_ref = ref parent.GetThisRef(ref root);
                    BTree.RotateLeft<AvlNodeInfo>(ref parent_ref);
                    HDebug.Assert(node.parent == parent.parent);
                    UpdateHeight(parent);
                    UpdateHeight(node  );
                    UpdateHeight(node.parent);

                    nnode   = node;
                    nparent = node.parent;
                }
                else
                {
                    throw new NotImplementedException();
                }

                HDebug.Assert(nnode   != null);
                HDebug.Assert(nparent != null);
                HDebug.Assert(nnode.parent == nparent);
                return (nnode, nparent);
            }
            ///////////////////////////////////////////////////////////////////////
            /// AVL Delete
            /// 
            /// 1. Delete node from BST
            /// 2. Rebalance
            /// 3. Return the deleted value
            ///////////////////////////////////////////////////////////////////////
            static bool Delete_selftest = HDebug.IsDebuggerAttached;
            public static void DeleteSelftest()
            {
                if(Delete_selftest == false)
                    return;

                Delete_selftest = false;
                {
                    var avltree = BTree.NewAvlTree();
                    HDebug.Assert(avltree.Validate()); 
                    avltree.InsertRange( 10, 5, 17, 2, 7, 20, 3 );
                    HDebug.Assert(avltree.Validate());
                    HDebug.Assert(avltree.ToString() == "(((_,2,3),5,7),10,(_,17,20))");
                    avltree.Delete(10);
                }
            }
            public T Delete(T query)
            {
                AvlNodeInfo avlquery = new AvlNodeInfo
                {
                    value  = query,
                };
                (AvlNodeInfo value, Node<AvlNodeInfo> deleted_parent) = BstDelete<AvlNodeInfo>(ref root, avlquery, avlcomp);

                HDebug.Assert(deleted_parent.left == null || deleted_parent.right == null);
                Node<AvlNodeInfo> deleted_sibling = (deleted_parent.left != null) ? deleted_parent.left : deleted_parent.right;
                UpdateBalance(deleted_sibling, ref root);

                return value.value;
            }
        }
    }
}
