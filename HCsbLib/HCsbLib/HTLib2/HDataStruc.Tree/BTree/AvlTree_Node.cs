using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class AvlTree<T>
    {
        public class Node : BTree<T>.Node
        {
            public new Node parent { get { return (Node)base.parent; } set { base.parent = value; } }
            public new Node left   { get { return (Node)base.left  ; } set { base.left   = value; } }
            public new Node right  { get { return (Node)base.right ; } set { base.right  = value; } }
            int left_height;
            int right_height;

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
            public new Node MaxNode()       { return (Node)base.MaxNode   (); }
            public new Node MinNode()       { return (Node)base.MinNode   (); }
            public new int  Count()         { return       base.Count     (); }
            public new int  Height()
            {
                int height = Math.Max(left_height, right_height)+1;
                HDebug.Assert(height == base.Height());
                return height;
            }
            public new bool IsBalanced()
            {
                bool isbalanced = Math.Abs(left_height - right_height) <= 1;
                HDebug.Assert(isbalanced == base.IsBalanced());
                return isbalanced;
            }

            ///////////////////////////////////////////////////////////////////////
            /// ToString()
            ///////////////////////////////////////////////////////////////////////
            //  public override string ToString()
            //  {
            //      StringBuilder sb = new StringBuilder();
            //      BTree<T>.Node.ToString(sb, this);
            //      sb.Insert(0, "val:"+value+", cnt:"+Count().ToString()+", ");
            //      return sb.ToString();
            //  }
            //  public new string ToStringSimple()
            //  {
            //      StringBuilder sb = new StringBuilder();
            //      ToString(sb, this);
            //      return sb.ToString();
            //  }
        }
    }
}
