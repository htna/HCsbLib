using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        public class Node<T>
        {
            public T       value;
            public Node<T> parent;
            public Node<T> left;
            public Node<T> right;
            public bool IsRoot     { get { return (parent == null); } }
            public bool IsLeaf     { get { return (left == null && right == null); } }
            public bool IsBalanced { get { return (Math.Abs(left.Height() - right.Height()) <= 1) } }

            public static Node<T> New(T value, Node<T> parent, Node<T> left, Node<T> right)
            {
                return new Node<T>
                {
                    value  = value ,
                    parent = parent,
                    left   = left  ,
                    right  = right ,
                };
            }
            public Node<T> MaxNode()
            {
                HDebug.Assert(parent != null);
                if(right == null)
                    return this;
                return right.MaxNode();
            }
            public Node<T> MinNode()
            {
                HDebug.Assert(parent != null);
                if(left == null)
                    return this;
                return left.MinNode();
            }
            public int Count()
            {
                return Count(this);

                int Count(Node<T> n)
                {
                    if(n == null)
                        return -1; // height of leaf is 0
                    int lc = Count(n.left);
                    int rc = Count(n.right);
                    return (1 + lc + rc);
                }
            }
            public int Height()
            {
                return Height(this);

                int Height(Node<T> n)
                {
                    if(n == null)
                        return 0;
                    int lh = Height(n.left);
                    int rh = Height(n.right);
                    return Math.Max(lh, rh) + 1;
                }
            }

            ///////////////////////////////////////////////////////////////////////
            /// ToString()
            ///////////////////////////////////////////////////////////////////////
            public override string ToString()
            {
                return ToString(this);
            }
            public static string ToString<T>(Node<T> node)
            {
                if(node == null)
                    return "()";
                StringBuilder sb = new StringBuilder();
                ToString(sb, node);
                if(node.IsLeaf)
                {
                    sb.Insert(0, "(");
                    sb.Append(")");
                }
                return sb.ToString();
            }
            static void ToString<T>(StringBuilder sb, Node<T> node)
            {
                if(node == null)
                {
                    sb.Append("_");
                    return;
                }
                else if(node.IsLeaf)
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
                    return;
                }
            }
        }
    }
}
