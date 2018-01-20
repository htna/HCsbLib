using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Graph
    {
        public class Dijkstra
        {
            /// http://en.wikipedia.org/wiki/Dijkstra's_algorithm
            ///  1  function Dijkstra(Graph, source):
            ///  2      for each vertex v in Graph:                                // Initializations
            ///  3          dist[v] := infinity ;                                  // Unknown distance function from 
            ///  4                                                                 // source to v
            ///  5          previous[v] := undefined ;                             // Previous node in optimal path
            ///  6      end for                                                    // from source
            ///  7      
            ///  8      dist[source] := 0 ;                                        // Distance from source to source
            ///  9      Q := the set of all nodes in Graph ;                       // All nodes in the graph are
            /// 10                                                                 // unoptimized - thus are in Q
            /// 11      while Q is not empty:                                      // The main loop
            /// 12          u := vertex in Q with smallest distance in dist[] ;    // Source node in first case
            /// 13          remove u from Q ;
            /// 14          if dist[u] = infinity:
            /// 15              break ;                                            // all remaining vertices are
            /// 16          end if                                                 // inaccessible from source
            /// 17          
            /// 18          for each neighbor v of u:                              // where v has not yet been 
            /// 19                                                                 // removed from Q.
            /// 20              alt := dist[u] + dist_between(u, v) ;
            /// 21              if alt < dist[v]:                                  // Relax (u,v,a)
            /// 22                  dist[v] := alt ;
            /// 23                  previous[v] := u ;
            /// 24                  decrease-key v in Q;                           // Reorder v in the Queue
            /// 25              end if
            /// 26          end for
            /// 27      end while
            /// 28      return dist;
            /// 29  endfunction
            public class Elem : IComparer<Elem>
            {
                public readonly Graph.Node node;
                public readonly double     dist;
                public readonly Graph.Node prev;
                public readonly Graph.Edge edge;
                public Elem(Graph.Node node, double dist, Graph.Node prev, Graph.Edge edge)
                {
                    this.node = node;
                    this.dist = dist;
                    this.prev = prev;
                    this.edge = edge;
                    if(edge != null)
                    {
                        Debug.Assert(edge.nodes.Length == 2);
                        Debug.Assert(edge.nodes.Contains(node));
                        Debug.Assert(edge.nodes.Contains(prev));
                    }
                }
                public override string ToString()
                {
                    return node.ToString() + string.Format(" => dist({0})", dist);
                }
                public int Compare(Elem x, Elem y)
                {
                    if(x == y) return 0;
                    if(x.node == y.node) { Debug.Assert(false); return 0; }
                    if(x.dist < y.dist) return -1;
                    if(x.dist > y.dist) return  1;
                    if(x.node.id < y.node.id) return -1;
                    if(x.node.id > y.node.id) return  1;
                    Debug.Assert(false);
                    return 0;
                }
            }
            public static Elem[] BuildMinAlt(Graph graph, Dictionary<Graph.Node,double> source2initdist, Func<double, Graph.Node, Graph.Edge, Graph.Node, double> GetAlt, Func<Graph.Node, bool> IsTarget)
            {
                /// dist(v2) := min[dist(v2), alt[dist(v1), dist(v1-v2)]]
                int maxid = 0;
                foreach(Graph.Node node in graph.Nodes) maxid = Math.Max(maxid, node.id);
                int size = maxid+1;
                //double maxdist = edge_dist.Values.Max() + 100;

                Elem[] elems = new Elem[size];
                Dictionary<Graph.Node, Graph.Node> prev = new Dictionary<Graph.Node, Graph.Node>();
                foreach(Graph.Node node in graph.Nodes)
                    elems[node.id] = new Elem(node, double.PositiveInfinity, null, null);

                foreach(Graph.Node source in source2initdist.Keys)
                {
                    double initdist = source2initdist[source];
                    Debug.Assert(initdist>=0);
                    Debug.Assert(elems[source.id] != null, elems[source.id].node == source);
                    elems[source.id] = new Elem(source, initdist, null, null);
                }

                SortedSet<Elem> Q = new SortedSet<Elem>(new Elem(null,double.NaN,null,null));
                foreach(Graph.Node node in graph.Nodes)
                    Q.Add(elems[node.id]);
                
                while(Q.Count > 0)
                {
                    Elem u = Q.Min;
                    Q.Remove(u);

                    if((IsTarget != null) && (IsTarget(u.node)))
                        return elems;

                    if(double.IsInfinity(u.dist))
                    //if(u.dist > maxdist)
                    {
                        elems[u.node.id] = new Elem(u.node, double.PositiveInfinity, null, null);
                        continue;
                    }

                    foreach(Graph.Node v_node in u.node.nexts.Keys)
                    {
                        Graph.Edge uv_edge = u.node.nexts[v_node];
                        double alt = GetAlt(u.dist, u.node, uv_edge, v_node);
                        Debug.Assert(alt >= u.dist);
                        if(alt < elems[v_node.id].dist)
                        {
                            Q.Remove(elems[v_node.id]);
                            elems[v_node.id] = new Elem(v_node, alt, u.node, uv_edge);
                            Q.Add(elems[v_node.id]);
                        }
                    }
                }

                return elems;
            }

            /// Now we can read the shortest path from source to target by reverse iteration:
            /// 1  S := empty sequence
            /// 2  u := target
            /// 3  while previous[u] is defined:                                   // Construct the shortest path with a stack S
            /// 4      insert u at the beginning of S                              // Push the vertex into the stack
            /// 5      u := previous[u]                                            // Traverse from target to source
            /// 6  end while ;
            public static Graph.Node[] GetPathNode(Elem[] elems, Graph.Node target)
            {
                List<Graph.Node> S = new List<Graph.Node>();
                Elem u = elems[target.id];
                while(u != null)
                {
                    S.Insert(0, u.node);
                    u = (u.prev != null) ? elems[u.prev.id] : null;
                }
                return S.ToArray();
            }
            public static Graph.Edge[] GetPathEdge(Elem[] elems, Graph.Node target)
            {
                List<Graph.Edge> S = new List<Graph.Edge>();
                Elem u = elems[target.id];
                while(u != null)
                {
                    if(u.edge != null)
                    {
                        S.Insert(0, u.edge);
                    }
                    else
                        Debug.Assert(u.prev == null);
                    u = (u.prev != null) ? elems[u.prev.id] : null;
                }
                return S.ToArray();
            }

            //////////////////////////////////////////////////////////////////////////////////
            /// 
            /// Template functions
            /// 
            public static Elem[] Build(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist)
            {
                Func<Graph.Node, bool> IsTarget = null;
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] Build(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist, Func<Graph.Node, bool> IsTarget)
            {
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] Build(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist, IList<Graph.Node> targets)
            {
                return BuildMinSum(graph, source, edge_dist, targets);
            }
            public static Elem[] BuildMinSum(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist)
            {
                Func<Graph.Node, bool> IsTarget = null;
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinSum(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist, Func<Graph.Node, bool> IsTarget)
            {
                /// dist(v2) := min[dist(v2), dist(v1)+dist(v1-v2)]
                Func<double, Graph.Node, Graph.Edge, Graph.Node, double> GetAlt = delegate(double u_dist, Graph.Node u_node, Graph.Edge uv_edge, Graph.Node v_node)
                {
                    double alt = u_dist + edge_dist[uv_edge];
                    return alt;
                };
                Dictionary<Graph.Node, double> source2initdist = new Dictionary<Graph.Node, double>();
                source2initdist.Add(source, 0);
                return BuildMinAlt(graph, source2initdist, GetAlt, IsTarget);
            }
            public static Elem[] BuildMinSum(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist, IList<Graph.Node> targets)
            {
                Func<Graph.Node, bool> IsTarget = delegate(Graph.Node node)
                {
                    return targets.Contains(node);
                };
                return BuildMinSum(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinMax(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist)
            {
                Func<Graph.Node, bool> IsTarget = null;
                return BuildMinMax(graph, source, edge_dist, IsTarget);
            }
            public static Elem[] BuildMinMax(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist, Func<Graph.Node, bool> IsTarget/*=null*/)
            {
                /// dist(v2) := min[dist(v2), max[dist(v1), dist(v1-v2)]]
                Func<double, Graph.Node, Graph.Edge, Graph.Node, double> GetAlt = delegate(double u_dist, Graph.Node u_node, Graph.Edge uv_edge, Graph.Node v_node)
                {
                    double alt = Math.Max(u_dist, edge_dist[uv_edge]);
                    return alt;
                };
                Dictionary<Graph.Node, double> source2initdist = new Dictionary<Graph.Node, double>();
                source2initdist.Add(source, 0);
                return BuildMinAlt(graph, source2initdist, GetAlt, IsTarget);
            }
            public static Elem[] BuildMinMax(Graph graph, Graph.Node source, Dictionary<Graph.Edge, double> edge_dist, IList<Graph.Node> targets)
            {
                Func<Graph.Node, bool> IsTarget = delegate(Graph.Node node)
                {
                    return targets.Contains(node);
                };
                return BuildMinMax(graph, source, edge_dist, IsTarget);
            }
        }
    }
}
