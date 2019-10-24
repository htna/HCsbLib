using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree
    {
        public class Node
        {
            public Node parent;
            public Node left;
            public Node right;

            public int Count()
            {
                return Count(this);

            }
            int Count(Node n)
            {
                if(n == null)
                    return 0;
                int lc = Count(n.left);
                int rc = Count(n.right);
                return (1 + lc + rc);
            }
            public int Height()
            {
                return Height(this);
            }
            int Height(Node n)
            {
                if(n == null)
                    return -1; // height of leaf is 0
                int lh = Height(n.left);
                int rh = Height(n.right);
                return Math.Max(lh, rh) + 1;
            }
        }

        public Node root;
        public BTree()
        {
            root = null;
        }

        //  public override string ToString()
        //  {
        //      StringBuilder sb = new StringBuilder();
        //      Node.ToString(sb, root);
        //      return sb.ToString();
        //  }
        //  
        //  public int Count()
        //  {
        //      return Count(root);
        //  }
        //  
        //  int Count(Node node)
        //  {
        //      if(node == null)
        //          return 0;
        //      int l = Count(node.left);
        //      int r = Count(node.right);
        //      return (1+l+r);
        //  }
    }
}
