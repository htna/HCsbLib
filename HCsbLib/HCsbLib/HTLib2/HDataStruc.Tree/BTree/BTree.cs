using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        public class Node
        {
            public T    value;
            public Node parent;
            public Node left;
            public Node right;
            public bool IsRoot { get { return (parent == null); } }
            public bool IsLeaf { get { return (left == null && right == null); } }

            public static Node New(T value, Node parent, Node left, Node right)
            {
                return new Node
                {
                    value  = value ,
                    parent = parent,
                    left   = left  ,
                    right  = right ,
                };
            }
            public override string ToString()
            {
                return "";
                //return id.ToString() + "," + nexts.Count + " edgs";
            }
            public Node MaxNode()
            {
                HDebug.Assert(parent != null);
                if(right == null)
                    return this;
                return right.MaxNode();
            }
            public Node MinNode()
            {
                HDebug.Assert(parent != null);
                if(left == null)
                    return this;
                return left.MinNode();
            }
        }


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

        ///////////////////////////////////////////////////////////////////////
        /// BST Insert
        ///////////////////////////////////////////////////////////////////////
        public Node BstInsert(T value)
        {
            return BstInsert(null, ref root, value);
        }
        Node BstInsert(Node parent, ref Node node, T value)
        {
            if(node == null)
            {
                node = Node.New(value, parent, null, null);
                return node;
            }
            if(comp.Compare(node.value, value) < 0)
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
        ///////////////////////////////////////////////////////////////////////
        public T BstDelete(T query)
        {
            return BstDelete(ref root, query);
        }
        T BstDelete(ref Node node, T query)
        {
            HDebug.Assert(node != null);
            int query_node = comp.Compare(query, node.value);
            if     (query_node <  0) return BstDelete(ref node.left , query);
            else if(query_node >  0) return BstDelete(ref node.right, query);
            else                     return BstDelete(ref node);
        }
        T BstDelete(ref Node node)
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
                T value = node.value;
                node = node.left;
                return value;
            }
            else if(node.left == null && node.right != null)
            {
                T value = node.value;
                node = node.right;
                return value;
            }
            else
            {
                T value = node.value;
                T pred_value = BstDeleteMax(ref node.left);
                node.value = pred_value;
                return value;
            }
        }
        T BstDeleteMax(ref Node node)
        {
            if(node.right != null)
                return BstDeleteMax(ref node.right);
            T value = node.value;
            node = node.left;
            return value;
        }
    }
}
