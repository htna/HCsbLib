using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph
    {
        public Tree<Pair<Node,Edge>> BuildTree(Node root)
        {
            HashSet<Node> within = new HashSet<Node>(nodes);
            HDebug.Assert(within.Contains(root));

            Tree<Pair<Node,Edge>> tree = new Tree<Pair<Node, Edge>>();
            Pair<Node,Edge> rootval = new Pair<Node,Edge>(root,null);
            BuildTreeRec(null, rootval, within, tree);
            return tree;
        }
        void BuildTreeRec(Pair<Node,Edge> parent, Pair<Node,Edge> node, HashSet<Node> within, Tree<Pair<Node, Edge>> tree)
        {
            if(parent != null)
                HDebug.Assert(within.Contains(parent.Item1) == false);
            HDebug.Assert(within.Contains(node.Item1));

            Tree.Node treenode = tree.AddChild(parent, node);
            within.Remove(node.Item1);
            foreach(Node childnode in node.Item1.nexts.Keys)
            {
                if(within.Contains(childnode) == false)
                    continue;
                Edge childedge = node.Item1.nexts[childnode];
                Pair<Node,Edge> child = new Pair<Node,Edge>(childnode, childedge);
                BuildTreeRec(node, child, within, tree);
            }
        }
    }
}
