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
            public bool IsRoot { get { return (parent == null); } }
            public bool IsLeaf { get { return (left == null && right == null); } }

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
                        return 0;
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
                        return -1; // height of leaf is 0
                    int lh = Height(n.left);
                    int rh = Height(n.right);
                    return Math.Max(lh, rh) + 1;
                }
            }
            public bool IsBalanced()
            {
                return IsBalanced(this);

                bool IsBalanced(Node<T> n)
                {
                    if(n == null)
                        return true;
                    int diff = left.Height() - right.Height();
                    if(Math.Abs(diff) > 1          ) return false;
                    if(IsBalanced(n.left ) == false) return false;
                    if(IsBalanced(n.right) == false) return false;
                    return true;
                }
            }

            ///////////////////////////////////////////////////////////////////////
            /// ToString()
            ///////////////////////////////////////////////////////////////////////
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                ToString(sb, this);
                sb.Insert(0, "val:"+value+", cnt:"+Count().ToString()+", ");
                return sb.ToString();
            }
            public string ToStringSimple()
            {
                StringBuilder sb = new StringBuilder();
                ToString(sb, this);
                return sb.ToString();
            }
            public static void ToString<T>(StringBuilder sb, Node<T> node)
            {
                if(node == null)
                {
                    sb.Append("()");
                }
                else
                {
                    ToStringRec(sb, node);
                    if(node.IsLeaf)
                    {
                        sb.Insert(0, "(");
                        sb.Append(")");
                    }
                }
            }
            static void ToStringRec<T>(StringBuilder sb, Node<T> node)
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
                    ToStringRec(sb, node.left);
                    sb.Append(",");
                    sb.Append(node.value);
                    sb.Append(",");
                    ToStringRec(sb, node.right);
                    sb.Append(")");
                    return;
                }
            }
        }
    }
}
