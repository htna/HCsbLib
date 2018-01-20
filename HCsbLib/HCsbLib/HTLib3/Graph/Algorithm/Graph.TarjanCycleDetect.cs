using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Graph
    {
        public class TarjanCycleDetect
        {
            // http://en.wikipedia.org/wiki/Tarjan%27s_strongly_connected_components_algorithm
            // http://stackoverflow.com/questions/6643076/tarjan-cycle-detection-help-c-sharp#sca
            class tempclass
            {
                Dictionary<Graph.Node, int> cache = new Dictionary<Graph.Node, int>();
                public int this[Graph.Node n]
                {
                    get { if(cache.ContainsKey(n) == false) return -1; return cache[n]; }
                    set { if(cache.ContainsKey(n) == false) cache.Add(n,-1); cache[n] = value; }
                }
            }

            List<List<Graph.Node>> StronglyConnectedComponents = new List<List<Graph.Node>>();
            Stack<Graph.Node> S = new Stack<Graph.Node>();
            int index;
            tempclass indexs = new tempclass();
            tempclass lowlinks = new tempclass();
            //private static DepGraph dg;
            public List<List<Graph.Node>> DetectCycle(Graph g)
            {
                StronglyConnectedComponents = new List<List<Graph.Node>>();
                index = 0;
                S = new Stack<Graph.Node>();
                //dg = g;
                foreach(Graph.Node v in g.Nodes)
                {
                    if(indexs[v] < 0)
                    {
                        strongconnect(v);
                    }
                }
                return StronglyConnectedComponents;
            }

            private void strongconnect(Graph.Node v)
            {
                indexs[v]   = index;
                lowlinks[v] = index;
                index++;
                S.Push(v);

                foreach(Graph.Node w in v.nexts.Keys)
                {
                    if(indexs[w] < 0)
                    {
                        strongconnect(w);
                        lowlinks[v] = Math.Min(lowlinks[v], lowlinks[w]);
                    }
                    else if(S.Contains(w))
                    {
                        lowlinks[v] = Math.Min(lowlinks[v], indexs[w]);
                    }
                }

                if(lowlinks[v] == indexs[v])
                {
                    List<Graph.Node> scc = new List<Graph.Node>();
                    Graph.Node w;
                    do
                    {
                        w = S.Pop();
                        scc.Add(w);
                    } while(v != w);
                    StronglyConnectedComponents.Add(scc);
                }
            }
        }
    }
}
