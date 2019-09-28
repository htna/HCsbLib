using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        protected Node root;
        IComparer<T>   comp;
        public BTree(IComparer<T> comp)
        {
            this.root = null;
            this.comp = comp;
        }
        public Node Root
        {
            get { return root; }
        }

        public override string ToString()
        {
            if(root == null)
                return "()";
            StringBuilder sb = new StringBuilder();
            ToString(sb, root);
            return sb.ToString();
        }
        void ToString(StringBuilder sb, Node node)
        {
            if(node == null)
                return;
            if(node.IsLeaf)
            {
                sb.Append(node.value);
                return;
            }
            else
            {
                sb.Append("(");
                ToString(sb, node.left);
                sb.Append(",");
                sb.Append(node.value);
                sb.Append(",");
                ToString(sb, node.right);
                sb.Append(")");
            }
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
