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
                public int height;
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
            /// AVL Insert
            /// 
            /// 1. Insert value into BST
            /// 2. Rebalance
            /// 3. Return the inserted node
            ///////////////////////////////////////////////////////////////////////
            static bool AvlInsert_selftest = true;
            public Node<AvlNodeInfo> Insert(T value)
            {
                HDebug.Assert(root == null || root.IsRoot());
                AvlNodeInfo avlvalue = new AvlNodeInfo
                {
                    value  = value,
                    height = 0,
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
                    UpdateParentBalance(node, 0);
                    return node;
                }
            }
            static int GetHeight(Node<AvlNodeInfo> node)
            {
                if(node == null)
                    return -1;
                return node.value.height;
            }
            ref Node<AvlNodeInfo> GetNodeRef(Node<AvlNodeInfo> node)
            {
                if(node == root)
                {
                    return ref root;
                }

                var parent = node.parent;
                if(parent.left  == node) return ref parent.left ;
                if(parent.right == node) return ref parent.right;
                HDebug.Assert(false);
                throw new Exception();
            }
            void UpdateParentBalance(Node<AvlNodeInfo> node, int node_bf)
            {
                HDebug.Assert(node != null);
                Node<AvlNodeInfo> parent = node.parent;
                if(parent == null)
                    return;

                int parent_left_height  = GetHeight(parent.left );
                int parent_right_height = GetHeight(parent.right);
                parent.value.height = Math.Max(parent_left_height, parent_right_height) + 1;
                int parent_bf = parent_right_height - parent_left_height;

                if(parent_bf <= -2)
                {
                    ref Node<AvlNodeInfo> parent_ref = ref GetNodeRef(parent);
                    HDebug.Assert(parent_bf == -2);
                    if(node_bf == -1)
                    {
                        BTree.RotateRight<AvlNodeInfo>(ref parent_ref);
                    }
                    else if(node_bf == 1)
                    {
                        throw new NotImplementedException();
                    }
                    else
                        throw new NotImplementedException();
                }
                else if(parent_bf >= 2)
                {
                    throw new NotImplementedException();
                }

                UpdateParentBalance(parent, parent_bf);
            }
        
            //  public BTree
            //      ( Comparison<T> comp // = delegate(int a, int b) { return a - b; }
            //      )
            //  {
            //      this.root = null;
            //      this.compare = comp;
            //  }
            //  public Node<T> Root
            //  {
            //      get { return root; }
            //  }
            //  
            //  public override string ToString()
            //  {
            //      StringBuilder sb = new StringBuilder();
            //      Node<T>.ToString(sb, root);
            //      return sb.ToString();
            //  }
            //  
            //  public int Count()
            //  {
            //      return Count(root);
            //  }
            //  
            //  int Count(Node<T> node)
            //  {
            //      if(node == null)
            //          return 0;
            //      int l = Count(node.left);
            //      int r = Count(node.right);
            //      return (1+l+r);
            //  }
        
            //  static bool Insert_selftest = true;
            //  public Node Insert(T value)
            //  {
            //      if(Insert_selftest)
            //      {
            //          Insert_selftest = false;
            //          //  Comparison<object> _compare = delegate(object a, object b) { return (int)a - (int)b; };
            //          //  BTree<object> _bst = new BTree<object>(_compare);
            //          //                      HDebug.Assert(_bst.ToString() == "()"                                             );
            //          //  _bst.BstInsert(10); HDebug.Assert(_bst.ToString() == "(10)"                                           );
            //          //  _bst.BstInsert( 5); HDebug.Assert(_bst.ToString() == "(5,10,_)"                                       );
            //          //  _bst.BstInsert(20); HDebug.Assert(_bst.ToString() == "(5,10,20)"                                      );
            //          //  _bst.BstInsert( 2); HDebug.Assert(_bst.ToString() == "((2,5,_),10,20)"                                );
            //          //  _bst.BstInsert( 7); HDebug.Assert(_bst.ToString() == "((2,5,7),10,20)"                                );
            //          //  _bst.BstInsert( 4); HDebug.Assert(_bst.ToString() == "(((_,2,4),5,7),10,20)"                          );
            //          //  _bst.BstInsert( 6); HDebug.Assert(_bst.ToString() == "(((_,2,4),5,(6,7,_)),10,20)"                    );
            //          //  _bst.BstInsert(30); HDebug.Assert(_bst.ToString() == "(((_,2,4),5,(6,7,_)),10,(_,20,30))"             );
            //          //  _bst.BstInsert( 3); HDebug.Assert(_bst.ToString() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,30))"       );
            //          //  _bst.BstInsert(25); HDebug.Assert(_bst.ToString() == "(((_,2,(3,4,_)),5,(6,7,_)),10,(_,20,(25,30,_)))");
            //      }
            //      BTree<T>.Node _root = root;
            //      return BTree<T>.BstInsert(null, ref _root, value, compare);
            //  }
            //  //public static Node BstInsert(Node parent, ref Node node, T value, Comparison<T> compare)
        }
    }
}
