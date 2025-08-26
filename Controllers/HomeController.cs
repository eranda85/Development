using AmplifynTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static AmplifynTest.Models.GraphModel;

namespace AmplifynTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Graph()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CalculatePath(NodeSelection nodeSelection)
        {
            //Graph definition
            var graph = new List<Node>
            {
                new Node { Name = "A", Edges = new List<Edge> { new Edge{ To="B", Weight=4 }, new Edge{ To="C", Weight=6 } } },
                new Node { Name = "B", Edges = new List<Edge> { new Edge{ To="F", Weight=2 } } },
                new Node { Name = "C", Edges = new List<Edge> { new Edge{ To="A", Weight=6 }, new Edge{ To="D", Weight=8 } } },
                new Node { Name = "D", Edges = new List<Edge> { new Edge{ To="G", Weight=1 }, new Edge{ To="C", Weight=8 } } },
                new Node { Name = "E", Edges = new List<Edge> { new Edge{ To="B", Weight=2 }, new Edge{ To="F", Weight=3 }, new Edge{ To="D", Weight=4 }, new Edge{ To="I", Weight=8 } } },
                new Node { Name = "F", Edges = new List<Edge> { new Edge{ To="B", Weight=2 }, new Edge{ To="G", Weight=4 }, new Edge{ To="H", Weight=6 } } },
                new Node { Name = "G", Edges = new List<Edge> { new Edge{ To="E", Weight=4 }, new Edge{ To="H", Weight=5 }, new Edge{ To="I", Weight=5 } } },
                new Node { Name = "H", Edges = new List<Edge> { new Edge{ To="F", Weight=6 } } },
                new Node { Name = "I", Edges = new List<Edge>() }
            };

            // Calculate shortest path
            var result = ShortestPath(nodeSelection.fromNode, nodeSelection.toNode, graph);

            // Prepare result
            var finalresult = new
            {
                FromNode = nodeSelection.fromNode,
                ToNode = nodeSelection.toNode,
                Path = string.Join(", ", result.NodeNames),
                TotalDistance = result.Distance
            };

            // Return result as JSON
            return Json(finalresult);
        }

        public ShortestPathData ShortestPath(string fromNodeName, string toNodeName, List<Node> graph)
        {
            // Dijkstra's algorithm implementation
            var distances = new Dictionary<string, int>();
            var previous = new Dictionary<string, string>();

            // Priority queue to hold nodes to explore
            var pq = new SortedSet<(int distance, string node)>();

            // Initialize distances and previous nodes
            foreach (var node in graph)
            {
                distances[node.Name] = int.MaxValue;
                previous[node.Name] = null;
            }

            // Start from the source node
            distances[fromNodeName] = 0;
            pq.Add((0, fromNodeName));

            // Explore the graph
            while (pq.Count>0)
            {
                var (currentDistance, currentNode) = pq.Min;
                pq.Remove(pq.Min);

                // If we reached the destination node, break
                if (currentNode == toNodeName)
                    break;

                // Get the current node object
                var nodeObj = graph.FirstOrDefault(n => n.Name == currentNode);
                if (nodeObj == null) continue;

                // Explore neighbors
                foreach (var edge in nodeObj.Edges)
                {
                    int newDist = currentDistance + edge.Weight;

                    if (newDist < distances[edge.To])
                    {
                        pq.Remove((distances[edge.To], edge.To));
                        distances[edge.To] = newDist;
                        previous[edge.To] = currentNode;
                        pq.Add((newDist, edge.To));
                    }
                }
            }

            // Reconstruct the shortest path
            var path = new List<string>();
            string current = toNodeName;
            while (current != null)
            {
                path.Insert(0, current);
                current = previous[current];
            }

            return new ShortestPathData
            {
                NodeNames = path,
                Distance = distances[toNodeName] == int.MaxValue ? 0 : distances[toNodeName]
            };
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
