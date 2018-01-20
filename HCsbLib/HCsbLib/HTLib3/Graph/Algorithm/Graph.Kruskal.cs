using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Graph
    {
        public class Kruskal
        {
            /// http://en.wikipedia.org/wiki/Kruskal's_algorithm
            /// KRUSKAL(G):
            /// 1 A = ∅
            /// 2 foreach v ∈ G.V:
            /// 3   MAKE-SET(v)
            /// 4 foreach (u, v) ordered by weight(u, v), increasing:
            /// 5    if FIND-SET(u) ≠ FIND-SET(v):
            /// 6       A = A ∪ {(u, v)}
            /// 7       UNION(u, v)
            /// 8 return A
            public static Node FindRoot(Dictionary<Node, Node> node2parent, Node node)
            {
                int iter = 0;
                while(iter < node2parent.Count)
                {
                    iter ++;
                    Node parent = node2parent[node];
                    Debug.Assert(parent != node);
                    if(parent == null)
                        return node;
                    node = parent;
                }
                Debug.Assert(false);
                return null;
            }
            public static Graph<Graph.Node,Graph.Edge> BuildMST(Graph graph, Func<Edge,double> edgecost)
            {
                ////////////////////////////////////////////////////
                // make set of independent trees
                // make mst graph in parallel
                Dictionary<Node, Node> node2parent = new Dictionary<Node, Node>();
                Graph<Graph.Node, Graph.Edge> mst = new Graph<Node, Edge>();
                foreach(Node node in graph.Nodes)
                {
                    node2parent.Add(node, null);
                    mst.AddNode(node);
                }
                ////////////////////////////////////////////////////
                // sort by edgecosts
                SortedList<double, Edge> edge2cost = new SortedList<double, Edge>();
                foreach(Edge edge in graph.Edges)
                    edge2cost.Add(edgecost(edge), edge);
                ////////////////////////////////////////////////////
                // loop
                foreach(Edge minedge in edge2cost.Values)
                {
                    // visit each edge in edge-list in the order of small cost
                    // get node of minedge
                    Debug.Assert(minedge.nodes.Length == 2);
                    Node node0 = minedge.nodes[0]; Node parent0 = FindRoot(node2parent, node0);
                    Node node1 = minedge.nodes[1]; Node parent1 = FindRoot(node2parent, node1);
                    // if both trees are the same, skip
                    if(parent0 == parent1)
                        continue;
                    // merge trees and assign the new tree to both nodes
                    ///  
                    ///         parent0
                    ///         .    \
                    ///        .     parent1
                    ///      node0      .
                    ///                  .
                    ///                 node1
                    node2parent[parent1] = parent0;
                    Debug.Assert(FindRoot(node2parent, node0) == FindRoot(node2parent, node1));
                    Graph<Graph.Node, Graph.Edge>.Edge mstedge = mst.AddEdge(node0, node1, minedge);
                    Debug.Assert(mstedge != null);
                }
                ////////////////////////////////////////////////////
                // check if all nodes has the same tree
                if(Debug.IsDebuggerAttached)
                {
                    Dictionary<Node, List<Node>> roots = new Dictionary<Node, List<Node>>();
                    foreach(Node node in graph.Nodes)
                    {
                        Node root = FindRoot(node2parent, node);
                        if(roots.ContainsKey(root) == false)
                            roots.Add(root, new List<Node>());
                        roots[root].Add(node);
                        //list_node_root.Add(new Tuple<Node, Node>(node, root));
                    }
                    /// Multiple tree can exist when there is no edge between vertex groups.
                    /// This could become possible when inf-vert is removed, since solvent verts are connected through inf-vert.
                }
                ////////////////////////////////////////////////////
                // return mst
                return mst;
            }

            //////////////////////////////////////////////////////////////////////////////////
            /// 
            /// Template functions
            /// 
            public static Graph<Graph.Node, Graph.Edge> BuildMST(Graph graph, Dictionary<Edge, double> edgecost)
            {
                Func<Edge, double> fnedgecost = delegate(Edge edge)
                {
                    return edgecost[edge];
                };
                return BuildMST(graph, fnedgecost);
            }
        }
    }
}
