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
    }
}
