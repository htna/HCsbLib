using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Tree
    {
        public class Node
        {
            public Node          parent;
            public HashSet<Node> children;
            public bool IsRoot { get { return (parent == null); } }
            public bool IsLeaf { get { return (children.Count == 0); } }

            public static Node New(Node parent)
            {
                return new Node(parent);
            }
            public Node(Node parent)
            {
                this.parent   = parent;
                this.children = new HashSet<Node>();
            }
            public override string ToString()
            {
                return "";
                //return id.ToString() + "," + nexts.Count + " edgs";
            }
        }

        protected Node root;
        public Tree()
        {
            this.root = null;
        }
        public Node Root
        {
            get { return root; }
        }
        public Node AddChild(Tree.Node parent)
        {
            return AddChild(parent, Node.New);
        }
        public Node AddChild(Tree.Node parent, Func<Node, Node> nodebuilder)
        {
            Node child = nodebuilder(parent);
            if(parent == null)
            {
                root = child;
                return root;
            }
            parent.children.Add(child);
            return child;
        }
        public List<Node> ListDescendents(Node node, bool includeSelf)
        {
            List<Node> descendents = new List<Node>();
            if(includeSelf)
                descendents.Add(node);
            ListDescendentsRec(node, descendents);
            return descendents;
        }
        void ListDescendentsRec(Node node, List<Node> descendents)
        {
            foreach(Node child in node.children)
            {
                descendents.Add(child);
                ListDescendentsRec(child, descendents);
            }
        }
        public List<Node> NodesFromRootTo(Node node)
        {
            List<Node> nodes = new List<Node>();
            while(node != null)
            {
                nodes.Insert(0, node);
                node = node.parent;
            }
            return nodes;
        }
        static bool DistFromRoot_selftest = HDebug.IsDebuggerAttached;
        public int DistFromRoot(Node node)
        {
            if(DistFromRoot_selftest)
            {
                DistFromRoot_selftest = false;
                HDebug.Assert(DistFromRoot(root) == 0);
            }
            return NodesFromRootTo(node).Count-1;
        }
    }
}
