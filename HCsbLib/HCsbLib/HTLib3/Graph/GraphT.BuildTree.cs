using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Graph<NODE,EDGE>
    {
        //public HTLib2.Tree<Tuple<NODE,EDGE>> BuildTree(NODE rootval)
        //{
        //    HTLib2.Tree<Tuple<NODE,EDGE>> tree = new HTLib2.Tree<Tuple<NODE,EDGE>>();
        //    BuildTreeRec(tree, null, rootval);
        //
        //    return tree;
        //}
        //void BuildTreeRec(HTLib2.Tree<Tuple<NODE,EDGE>> tree, HTLib2.Tree.Node treeparent, NODE nodeval)
        //{
        //    Node parent = null;
        //    if(treeparent != null)
        //    {
        //        Tuple<NODE,EDGE> parentval = tree.GetValue(treeparent);
        //        parent = GetNode(parentval);
        //    }
        //    // add node-val
        //    Debug.Assert(tree.FindNode(nodeval) == null);
        //    HTLib2.Tree.Node treenode   = tree.AddChild(treeparent, nodeval);
        //    // recursion with children
        //    Node node = GetNode(nodeval);
        //    foreach(Node child in node.nexts.Keys)
        //    {
        //        if(child == parent)
        //            continue;
        //        NODE childval = GetValue(child);
        //        BuildTreeRec(tree, treenode, childval);
        //    }
        //}
        public HTLib2.Tree<Tuple<NODE, EDGE>> BuildTree(NODE rootval, EDGE nulledgeval)
        {
            Node root = GetNode(rootval);
            HTLib2.Tree<Tuple<Node, Edge>> tree = BuildTree(root);
            HTLib2.Tree<Tuple<Node, Edge>>.Node treeroot = (HTLib2.Tree<Tuple<Node, Edge>>.Node)tree.Root;
            HTLib2.Tree<Tuple<NODE,EDGE>> tree2 = new HTLib2.Tree<Tuple<NODE,EDGE>>();
            HTLib2.Tree<Tuple<NODE,EDGE>>.Node tree2root = tree2.AddChild((HTLib2.Tree.Node)null, new Tuple<NODE,EDGE>(rootval,nulledgeval));
            CopyTreeRec(tree, treeroot, tree2, tree2root);
            return tree2;
        }
        void CopyTreeRec(HTLib2.Tree<Tuple<Node,Edge>> tree, HTLib2.Tree<Tuple<Node,Edge>>.Node treenode, HTLib2.Tree<Tuple<NODE,EDGE>> tree2, HTLib2.Tree<Tuple<NODE,EDGE>>.Node treenode2)
        {
            Debug.Assert(treenode2.children.Count == 0);
            foreach(HTLib2.Tree<Tuple<Node,Edge>>.Node treechild in treenode.children)
            {
                Node node = (treechild as HTLib2.Tree<Tuple<Node,Edge>>.Node).value.Item1; NODE nodeval = GetValue(node);
                Edge edge = (treechild as HTLib2.Tree<Tuple<Node,Edge>>.Node).value.Item2; EDGE edgeval = GetValue(edge);
                HTLib2.Tree<Tuple<NODE,EDGE>>.Node treechild2 = (tree2.AddChild(treenode2, new Tuple<NODE,EDGE>(nodeval, edgeval)) as HTLib2.Tree<Tuple<NODE,EDGE>>.Node);
                CopyTreeRec(tree, treechild, tree2, treechild2);
            }
        }
        public HTLib2.Tree<Tuple<Node, Edge>> BuildTree(Node root)
        {
            HashSet<Node> within = new HashSet<Node>(Nodes);
            Debug.Assert(within.Contains(root));

            HTLib2.Tree<Tuple<Node, Edge>> tree = new HTLib2.Tree<Tuple<Node, Edge>>();
            Tuple<Node, Edge> rootval = new Tuple<Node, Edge>(root, null);
            BuildTreeRec(null, rootval, within, tree);
            return tree;
        }
        void BuildTreeRec(Tuple<Node, Edge> parent, Tuple<Node, Edge> node, HashSet<Node> within, HTLib2.Tree<Tuple<Node, Edge>> tree)
        {
            if(parent != null)
                Debug.Assert(within.Contains(parent.Item1) == false);
            Debug.Assert(within.Contains(node.Item1));

            HTLib2.Tree.Node treenode = tree.AddChild(parent, node);
            within.Remove(node.Item1);
            foreach(Node childnode in node.Item1.nexts.Keys)
            {
                if(within.Contains(childnode) == false)
                    continue;
                Edge childedge = node.Item1.nexts[childnode] as Edge;
                Tuple<Node, Edge> child = new Tuple<Node, Edge>(childnode, childedge);
                BuildTreeRec(node, child, within, tree);
            }
        }
    }
}
