namespace AmplifynTest.Models
{
    public class GraphModel
    {
        public class NodeSelection
        {
            public string fromNode { get; set; }
            public string toNode { get; set; }
        }
        public class Node
        {
            public string Name { get; set; }
            public List<Edge> Edges { get; set; }
        }

        public class Edge
        {
            public string To { get; set; }
            public int Weight { get; set; }
        }

        public class ShortestPathData
        {
            public List<string> NodeNames { get; set; }
            public int Distance { get; set; }
        }
    }
}
