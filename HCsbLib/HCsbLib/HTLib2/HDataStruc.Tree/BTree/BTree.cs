using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BSTree<T>
    {
        protected Node root;
        Comparison<T>  compare;
        public BSTree
            ( Comparison<T> comp // = delegate(int a, int b) { return a - b; }
            )
        {
            this.root = null;
            this.compare = comp;
        }
        public Node Root
        {
            get { return root; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Node.ToString(sb, root);
            return sb.ToString();
        }

        public int Count()
        {
            return Count(root);
        }

        int Count(Node node)
        {
            if(node == null)
                return 0;
            int l = Count(node.left);
            int r = Count(node.right);
            return (1+l+r);
        }
    }
}
