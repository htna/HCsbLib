using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph<NODE,EDGE>
    {
        public class Dijkstra
        {
            public class Elem : Graph.Dijkstra.Elem
            {
                public new Node   node { get { return (Node  )base.node; } }
                public new double dist { get { return (double)base.dist; } }
                public new Node   prev { get { return (Node  )base.prev; } }
                public new Edge   edge { get { return (Edge  )base.edge; } }
                public Elem(Graph.Dijkstra.Elem elem) : base(elem.node, elem.dist, elem.prev, elem.edge) { }
            }
            public static Elem[] BuildMinAlt(Graph<NODE,EDGE> graph, Dictionary<Node,double> source2initdist, Func<double,Node,Edge,Node,double> GetAlt, Func<Node,bool> IsTarget)
            {
                Func<double, Graph.Node, Graph.Edge, Graph.Node, double> fnGetAlt = null;
                if(GetAlt != null) fnGetAlt = delegate(double u_dist, Graph.Node u_node, Graph.Edge uv_edge, Graph.Node v_node)
                                              {
                                                  return GetAlt(u_dist, (Node)u_node, (Edge)uv_edge, (Node)v_node);
                                              };
                Func<Graph.Node, bool> fnIsTarget = null;
                if(IsTarget != null) fnIsTarget = delegate(Graph.Node node)
                                                  {
                                                      return IsTarget((Node)node);
                                                  };
                Dictionary<Graph.Node, double> lsource2initdist = new Dictionary<Graph.Node, double>();
                foreach(Node source in source2initdist.Keys)
                    lsource2initdist.Add(source, source2initdist[source]);
                Graph.Dijkstra.Elem[] elems = Graph.Dijkstra.BuildMinAlt(graph.graph, lsource2initdist, fnGetAlt, fnIsTarget);
                Elem[] elems2 = new Elem[elems.Length];
                for(int i=0; i<elems.Length; i++)
                    elems2[i] = new Elem(elems[i]);
                return elems2;
            }
            public static Elem[] BuildMinAlt(Graph<NODE,EDGE> graph, Node source, Func<double,Node,Edge,Node,double> GetAlt, Func<Node,bool> IsTarget)
            {
                Dictionary<Node, double> source2initdist = new Dictionary<Node, double>();
                source2initdist.Add(source, 0);
                return BuildMinAlt(graph, source2initdist, GetAlt, IsTarget);
            }
            public static Node[] GetPathNode(Elem[] elems, Node target)
            {
                return Graph.Dijkstra.GetPathNode(elems.HToType<Elem, Graph.Dijkstra.Elem>(), target).HToType<Graph.Node, Node>();
            }
            public static Edge[] GetPathEdge(Elem[] elems, Node target)
            {
                return Graph.Dijkstra.GetPathEdge(elems.HToType<Elem, Graph.Dijkstra.Elem>(), target).HToType<Graph.Edge, Edge>();
            }
            //////////////////////////////////////////////////////////////////////////////////
            /// 
            /// Template classes
            /// 
            public static Elem[] Build(Graph<NODE,EDGE> graph, Node source, Dictionary<Edge, double> edge_dist, Func<Node, bool> IsTarget)
            {
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] Build(Graph<NODE, EDGE> graph, Node source, Dictionary<Edge, double> edge_dist)
            {
                Func<Node, bool> IsTarget = null;
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] Build(Graph<NODE, EDGE> graph, Node source, Dictionary<Edge, double> edge_dist, IList<Node> targets)
            {
                return BuildMinSum(graph, source, edge_dist, targets);
            }
            public static Elem[] Build(Graph<NODE, EDGE> graph, Dictionary<Node, double> source2initdist, Dictionary<Edge, double> edge_dist)
            {
                Func<Node, bool> IsTarget = null;
                return BuildMinSum(graph, source2initdist, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinSum(Graph<NODE,EDGE> graph, Node source, Dictionary<Edge, double> edge_dist)
            {
                Func<Node, bool> IsTarget = null;
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinSum(Graph<NODE,EDGE> graph, Node source, Dictionary<Edge, double> edge_dist, Func<Node, bool> IsTarget)
            {
                Dictionary<Node, double> source2initdist = new Dictionary<Node, double>();
                source2initdist.Add(source, 0);
                return BuildMinSum(graph, source2initdist, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinSum(Graph<NODE, EDGE> graph, Dictionary<Node, double> source2initdist, Dictionary<Edge, double> edge_dist, Func<Node, bool> IsTarget)
            {
                /// dist(v2) := min[dist(v2), dist(v1)+dist(v1-v2)]
                Func<double, Node, Edge, Node, double> GetAlt = delegate(double u_dist, Node u_node, Edge uv_edge, Node v_node)
                {
                    double alt = u_dist + edge_dist[uv_edge];
                    return alt;
                };
                Func<Graph.Node, bool> fnIsTarget = null;
                if(IsTarget != null) fnIsTarget = delegate(Graph.Node node)
                                                  {
                                                      return IsTarget((Node)node);
                                                  };
                return BuildMinAlt(graph, source2initdist, GetAlt, IsTarget);
            }
            public static Elem[] BuildMinSum(Graph<NODE,EDGE> graph, Node source, Dictionary<Edge, double> edge_dist, IList<Node> targets)
            {
                Func<Node, bool> IsTarget = delegate(Node node)
                {
                    return targets.Contains(node);
                };
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinMax(Graph<NODE,EDGE> graph, Node source, Dictionary<Edge, double> edge_dist)
            {
                Func<Node, bool> IsTarget = null;
                Dictionary<Node, double> source2initdist = new Dictionary<Node, double>();
                source2initdist.Add(source, 0);
                return BuildMinMax(graph, source2initdist, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinMax(Graph<NODE, EDGE> graph, Dictionary<Node, double> source2initdist, Dictionary<Edge, double> edge_dist, Func<Node, bool> IsTarget)
            {
                /// dist(v2) := min[dist(v2), max[dist(v1), dist(v1-v2)]]
                Func<double, Node, Edge, Node, double> GetAlt = delegate(double u_dist, Node u_node, Edge uv_edge, Node v_node)
                {
                    double alt = Math.Max(u_dist, edge_dist[uv_edge]);
                    return alt;
                };
                Func<Graph.Node, bool> fnIsTarget = null;
                if(IsTarget != null) fnIsTarget = delegate(Graph.Node node)
                                                  {
                                                      return IsTarget((Node)node);
                                                  };
                return BuildMinAlt(graph, source2initdist, GetAlt, IsTarget);
            }
            public static Elem[] BuildMinMax(Graph<NODE,EDGE> graph, Node source, Dictionary<Edge, double> edge_dist, IList<Node> targets)
            {
                Func<Node, bool> IsTarget = delegate(Node node)
                {
                    return targets.Contains(node);
                };
                Dictionary<Node, double> source2initdist = new Dictionary<Node, double>();
                source2initdist.Add(source, 0);
                return BuildMinMax(graph, source2initdist, edge_dist, IsTarget);
            }
        }
        public class Kruskal
        {
            public static Graph<NODE,EDGE> BuildMST(Graph<NODE,EDGE> graph, Func<Edge, double> edgecost)
            {
                Func<Graph.Edge, double> fnedgecost = delegate(Graph.Edge edge)
                {
                    return edgecost((Edge)edge);
                };
                Graph<Graph.Node, Graph.Edge> mst = Graph.Kruskal.BuildMST(graph.graph, fnedgecost);
                Graph<NODE, EDGE> mst2 = new Graph<NODE, EDGE>();
                foreach(Graph.Node node in mst.GetNodeValues())
                {
                    NODE nodeval = graph.GetNodeValue((Node)node);
                    HDebug.Verify(mst2.AddNode(nodeval) != null);
                }
                foreach(Graph.Edge edge in mst.GetEdgeValues())
                {
                    EDGE edgeval = ((Edge)edge).value;
                    Node node0 = (Node)edge.nodes[0]; NODE nodeval0 = graph.GetNodeValue(node0); HDebug.Assert(mst2.GetNode(nodeval0) != null);
                    Node node1 = (Node)edge.nodes[1]; NODE nodeval1 = graph.GetNodeValue(node1); HDebug.Assert(mst2.GetNode(nodeval1) != null);
                    HDebug.Verify(mst2.AddEdge(nodeval0, nodeval1, edgeval) != null);
                }
                return mst2;
            }
            public static Graph<NODE, EDGE> BuildMST(Graph<NODE, EDGE> graph, Dictionary<Edge, double> edgecost)
            {
                Func<Edge, double> fnedgecost = delegate(Edge edge)
                {
                    return edgecost[(Edge)edge];
                };
                return BuildMST(graph, fnedgecost);
            }
        }
    }
}
