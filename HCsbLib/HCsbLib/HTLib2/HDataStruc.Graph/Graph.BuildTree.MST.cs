using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph
    {
        public static double BuildTreeMST_costfunc(Node from, Node to)
        {
            HDebug.Assert(from.nexts.ContainsKey(to));
            return 1;
        }
        public Tree<Node> BuildTreeMST(Node root, Func<Node,Node,double> costfunc=null)
        {
            Tree<Node> tree = new Tree<Node>();
            SortedList<double,List<Tuple<Node,Node>>> probing = new SortedList<double, List<Tuple<Node, Node>>>();
            probing.Add(0, new List<Tuple<Node,Node>>());
            probing[0].Add(new Tuple<Node, Node>(null, root));
            double[] costs = new double[nodes.Count];
            for(int i=0; i<costs.Length; i++)
                costs[i] = double.NaN;
            if(costfunc == null)
                costfunc = BuildTreeMST_costfunc;

            while(probing.Count > 0)
            {
                var first = probing.ElementAt(0);
                double cost = first.Key;
                Node   from = first.Value[0].Item1;
                Node   to   = first.Value[0].Item2;
                first.Value.RemoveAt(0);
                if(first.Value.Count == 0)
                    probing.Remove(cost);

                if(double.IsNaN(costs[to.id]) == false)
                    continue; // to is already in tree
                HDebug.Assert(double.IsNaN(cost) == false);
                tree.AddChild(from, to);
                costs[to.id] = cost;

                foreach(Node node in to.nexts.Keys)
                {
                    double nodecost = cost + costfunc(to, node);
                    if(probing.ContainsKey(nodecost) == false)
                        probing.Add(nodecost, new List<Tuple<Node, Node>>());
                    probing[nodecost].Add(new Tuple<Node, Node>(to, node));
                }
            }

            return tree;
        }
    }
}
