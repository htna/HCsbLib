using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class AvlTree<T>
    {
        protected Node root;
        Comparison<T>     compare;
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
