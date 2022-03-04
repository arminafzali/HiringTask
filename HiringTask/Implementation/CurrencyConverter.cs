using HiringTask.Interfaces;
using HiringTask.Models;

namespace HiringTask.Implementation
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private IList<Node> Nodes;
        private IList<Task> Tasks;

        public CurrencyConverter()
        {
            Tasks = new List<Task>();
            Nodes = new List<Node>();
        }

        public void ClearConfiguration()
        {
            Task.WaitAll(Tasks.ToArray());
            Tasks.Clear();
            Nodes.Clear();
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            Task.WaitAll(Tasks.ToArray());
            Tasks.Clear();
            var t = new Task<double>(() => {
                var nodeStart = GetNode(fromCurrency);
                var nodeEnd = GetNode(toCurrency);

                var result = amount * BFSSearch(nodeStart, nodeEnd);
                ClearAfterAlgorithm();
                return result;
            });
            Tasks.Add(t);
            t.Start();

            return t.Result;
            
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
            if (startNode==endNode) 
            {
                return 1;
            }
            var queue = new Queue<Node>();
            startNode.IsVisited = true;
            queue.Enqueue(startNode);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var node in current.Edges.Select(x => x.EndNode).ToList())
                {
                    if (!node.IsVisited)
                    {
                        node.IsVisited = true;
                        queue.Enqueue(node);
                        node.PrevNode = current;
                        if(node== endNode)
                        {
                            queue.Clear();
                            break;
                        }
                    }
                }
            }

            var amount= TraceRoute(endNode);

            //GetOrAddEdge(startNode, endNode, 1 / amount);
            //GetOrAddEdge(endNode, startNode, amount);

            return amount;

        }
        private double TraceRoute(Node endNode)
        {
            var amount = 1.0;
            var node = endNode;
            if (node.PrevNode is null)
            {
                throw new Exception("There is no way");
            }
            while (node.PrevNode is not null)
            {
                var multi = node.Edges.FirstOrDefault(x => x.EndNode == node.PrevNode)?.Amount ?? 1;
                amount *= 1 / multi;
                node = node.PrevNode;
            }
            return amount;
        }

        private Node GetOrAddNode(string name)
        {
            var node = Nodes.FirstOrDefault(x => x.Name == name);
            if (node is null)
            {
                node = new Node() { Name = name };
                node.PrevNode = null;
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
        private void ClearAfterAlgorithm()
        {
            foreach (var node in Nodes)
            {
                node.IsVisited = false;
                node.PrevNode = null;
            }
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
