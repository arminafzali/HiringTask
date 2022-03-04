namespace HiringTask.Models
{
    public class Node
    {
        public string Name { get; set; }
        public IList<Edge> Edges { get; set; } = new List<Edge>();
        public bool IsVisited { get; set; }
        public Node? PrevNode { get; set; }
        public static bool operator == (Node node1, Node node2)
        {
            if (node1.Name == node2.Name)
                return true;
            return false;
        }
        public static bool operator != (Node node1, Node node2)
        {
            if (node1.Name == node2.Name)
                return false;
            return true;
        }
    }
}
