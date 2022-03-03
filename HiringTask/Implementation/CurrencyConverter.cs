using HiringTask.Interfaces;
using HiringTask.Models;

namespace HiringTask.Implementation
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private IList<Node> Nodes;
        public void ClearConfiguration()
        {
            Nodes = new List<Node>();
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            var nodeStart = GetNode(fromCurrency);
            var nodeEnd = GetNode(toCurrency);

            var result = amount * DFSSearch(nodeStart, nodeEnd);
            foreach (var node in Nodes)
            {
                node.IsVisited = false;
            }
            return result;
        }

        public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
        {
            ClearConfiguration();
            foreach (var conversionRate in conversionRates)
            {
                var node1 = GetOrAddNode(conversionRate.Item1);
                var node2 = GetOrAddNode(conversionRate.Item2);
                GetOrAddEdge(node1, node2, conversionRate.Item3);
                GetOrAddEdge(node2, node1, 1 / conversionRate.Item3);
            }
        }
        private double DFSSearch(Node startNode, Node endNode)
        {

            startNode.IsVisited = true;
            if (startNode == endNode)
            {
                return 1;
            }
            foreach (var connection in startNode.Edges.Where(x => !x.EndNode.IsVisited).ToList())
            {
                return DFSSearch(connection.EndNode, endNode) * connection.Amount;
            }
            return 0;
        }
        private double BFSSearch(Node startNode, Node endNode)
        {
            return 0;
        }

        private Node GetOrAddNode(string name)
        {
            var node = Nodes.FirstOrDefault(x => x.Name == name);
            if (node is null)
            {
                node = new Node() { Name = name };
                Nodes.Add(node);
            }
            return node;
        }
        private Node GetNode(string name)
        {
            var node = Nodes?.FirstOrDefault(x => x.Name == name) ?? null;
            if (node is null)
            {
                throw new ArgumentException("There is no currency with this name");
            }
            return node!;
        }
        private Edge GetOrAddEdge(Node node1, Node node2, double amount)
        {
            var edge = node1.Edges.FirstOrDefault(x => x.StartNode.Name == node1.Name && x.EndNode.Name == node2.Name);
            if (edge == null)
            {
                edge = new Edge
                {
                    StartNode = node1,
                    EndNode = node2,
                    Amount = amount
                };
                node1.Edges.Add(edge);
            }
            return edge;
        }

        //public string Print()
        //{
        //    string s = "";
        //    foreach (var node in Nodes)
        //    {
        //        foreach(var edge in node.Edges)
        //        {
        //            s += (edge.StartNode.Name + " " + edge.EndNode.Name + " " + edge.Amount);
        //            s += '\n';
        //            Console.WriteLine(edge.StartNode.Name+" "+edge.EndNode.Name +" "+edge.Amount);
        //        }
        //        s += '\n';
        //        Console.WriteLine();

        //    }
        //    return s;
        //}
    }
}
