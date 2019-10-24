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
                int lc = (right == null) ? 0 : left .Count();
                int rc = (right == null) ? 0 : right.Count();
                return (1 + lc + rc);
            }
            public int Height()
            {
                int lh = (right == null) ? 0 : left .Height();
                int rh = (right == null) ? 0 : right.Height();
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
