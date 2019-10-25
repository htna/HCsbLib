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
        }
        public class Node<T> : Node
        {
            public T       value ;
            public Node<T> parent;
            public Node<T> left  ;
            public Node<T> right ;

            public static Node<T> New<T>(T value, Node<T> parent, Node<T> left, Node<T> right)
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

            public bool IsRoot()
            {
                return (parent == null);
            }
            public bool IsLeaf()
            {
                return (left == null && right == null);
            }
            public int Count()
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

            public bool IsBalanced()
            {
                return IsBalanced(this).balanced;

                (bool balanced, int height) IsBalanced(Node<T> n)
                {
                    if(n == null)
                        return (true, -1);
                    var lb = IsBalanced(n.left );
                    var rb = IsBalanced(n.right);
                    int height = Math.Max(lb.height, rb.height) + 1;
                    int diff   = left.Height() - right.Height();
                    bool balanced = lb.balanced && rb.balanced && (Math.Abs(diff) <= 1);
                    return (balanced, height);
                }
            }

            public ref Node<T> GetThisRef(ref Node<T> root)
            {
                if(this == root        ) return ref root;
                if(this == parent.left ) return ref parent.left ;
                if(this == parent.right) return ref parent.right;
                HDebug.Assert(false);
                throw new Exception();
            }
            ///////////////////////////////////////////////////////////////////////
            /// ToString()
            ///////////////////////////////////////////////////////////////////////
            public override string ToString()
            {
                return ToStringSimple();
            }
            public string ToStringDetail()
            {
                StringBuilder sb = new StringBuilder();
                ToString(sb, this);
                sb.Insert(0, "val:" + value + ", cnt:" + Count().ToString() + ", ");
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
                if (node == null)
                {
                    sb.Append("()");
                }
                else
                {
                    ToStringRec(sb, node);
                    if (node.IsLeaf())
                    {
                        sb.Insert(0, "(");
                        sb.Append(")");
                    }
                }
            }
            static void ToStringRec<T>(StringBuilder sb, Node<T> node)
            {
                if (node == null)
                {
                    sb.Append("_");
                    return;
                }
                else if (node.IsLeaf())
                {
                    sb.Append(node.value.ToString());
                    return;
                }
                else
                {
                    sb.Append("(");
                    ToStringRec(sb, node.left);
                    sb.Append(",");
                    sb.Append(node.value.ToString());
                    sb.Append(",");
                    ToStringRec(sb, node.right);
                    sb.Append(")");
                    return;
                }
            }
        }
    }
}
