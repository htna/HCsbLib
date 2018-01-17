using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph<NODE,EDGE>
    {
        public static Graph<NODE,int> Build(IList<Tuple<NODE,NODE>> edges)
        {
            Graph<NODE,int> graph = new Graph<NODE,int>();
            for(int iedge=0; iedge<edges.Count; iedge++)
            {
                NODE vnode1 = edges[iedge].Item1;
                NODE vnode2 = edges[iedge].Item2;
                graph.AddEdge(vnode1, vnode2, iedge);
            }
            return graph;
        }
        public static Graph<NODE, EDGE> Build(Dictionary<Tuple<NODE, NODE>, EDGE> edges)
        {
            Graph<NODE, EDGE> graph = new Graph<NODE, EDGE>();
            foreach(var edge in edges)
            {
                NODE vnode1 = edge.Key.Item1;
                NODE vnode2 = edge.Key.Item2;
                EDGE vedge  = edge.Value;
                graph.AddEdge(vnode1, vnode2, vedge);
            }
            return graph;
        }
        public static Graph<int, int> Build(int[,] edges)
        {
            Graph<int, int> graph = new Graph<int, int>();
            for(int iedge=0; iedge<edges.GetLength(0); iedge++)
            {
                int inode1 = edges[iedge, 0];
                int inode2 = edges[iedge, 1];
                graph.AddEdge(inode1, inode2, iedge);
            }
            return graph;

            //Dictionary<int, Graph<int,int>.Node> value2node = new Dictionary<int, Graph<int,int>.Node>();
            //List<Graph<int,int>.Edge> graph_edges = new List<Graph<int,int>.Edge>();
            //
            //for(int i=0; i<edges.GetLength(0); i++)
            //{
            //    int value0 = edges[i, 0];
            //    int value1 = edges[i, 1];
            //    if(value2node.ContainsKey(value0) == false) value2node.Add(value0, new Graph<int,int>.Node(value0));
            //    if(value2node.ContainsKey(value1) == false) value2node.Add(value1, new Graph<int,int>.Node(value1));
            //    Graph<int,int>.Node node0 = value2node[value0];
            //    Graph<int,int>.Node node1 = value2node[value1];
            //    bool valid = (node0.next.ContainsKey(node1) == false)
            //              && (node1.next.ContainsKey(node0) == false);
            //    //Debug.Assert(valid);
            //    if(valid)
            //    {
            //        Graph<int,int>.Edge edge = new Graph<int, int>.Edge(i, node0, node1);
            //        graph_edges.Add(edge);
            //        node0.next.Add(node1, edge);
            //        node1.next.Add(node0, edge);
            //    }
            //}
            //
            //Graph<int,int> graph = new Graph<int,int>();
            //graph.nodes = value2node.Values.ToArray();
            //for(int i=0; i<graph.nodes.Length; i++)
            //    graph.nodes[i].id = i;
            //graph.edges = graph_edges.ToArray();
            //for(int i=0; i<graph.edges.Length; i++)
            //    graph.edges[i].id = i;
            //
            //return graph;
        }
    }
}
