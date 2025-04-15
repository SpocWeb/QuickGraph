using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using QuickGraph.Serialization;

namespace QuickGraph.Algorithms.Condensation
{
    [TestClass]
    
    public class WeaklyConnectedCondensationGraphAlgorithmTest
    {
        [DataTestMethod]
        [DynamicData(nameof(TestGraphFactory.GetAdjacencyGraphData), typeof(TestGraphFactory), DynamicDataSourceType.Method)]
        public void WeaklyConnectedCondensatAll(AdjacencyGraph<string, Edge<string>> g) => WeaklyConnectedCondensate(g);

        
        public static void WeaklyConnectedCondensate<TVertex, TEdge>(IVertexAndEdgeListGraph<TVertex, TEdge> g)
            where TEdge : IEdge<TVertex>
        {
            var algo = new CondensationGraphAlgorithm<TVertex,TEdge, AdjacencyGraph<TVertex,TEdge>>(g);
            algo.StronglyConnected = false;
            algo.Compute();
            CheckVertexCount(g, algo);
            CheckEdgeCount(g, algo);
            CheckComponentCount(g, algo);
        }

        private static void CheckVertexCount<TVertex, TEdge>(IVertexAndEdgeListGraph<TVertex, TEdge> g,
            CondensationGraphAlgorithm<TVertex,TEdge, AdjacencyGraph<TVertex,TEdge>> algo)
            where TEdge : IEdge<TVertex>
        {
            int count = 0;
            foreach (var vertices in algo.CondensedGraph.Vertices)
                count += vertices.VertexCount;
            Assert.AreEqual(g.VertexCount, count, "VertexCount does not match");
        }

        private static void CheckEdgeCount<TVertex,TEdge>(IVertexAndEdgeListGraph<TVertex,TEdge> g,
            CondensationGraphAlgorithm<TVertex,TEdge, AdjacencyGraph<TVertex,TEdge>> algo)
            where TEdge : IEdge<TVertex>
        {
            // check edge count
            int count = 0;
            foreach (var edges in algo.CondensedGraph.Edges)
                count += edges.Edges.Count;
            foreach (var vertices in algo.CondensedGraph.Vertices)
                count += vertices.EdgeCount;
            Assert.AreEqual(g.EdgeCount, count, "EdgeCount does not match");
        }


        private static void CheckComponentCount<TVertex,TEdge>(IVertexAndEdgeListGraph<TVertex,TEdge> g,
            CondensationGraphAlgorithm<TVertex,TEdge, AdjacencyGraph<TVertex,TEdge>> algo)
            where TEdge : IEdge<TVertex>
        {
            // check number of vertices = number of storngly connected components
            int components = g.WeaklyConnectedComponents(new Dictionary<TVertex, int>());
            Assert.AreEqual(components, algo.CondensedGraph.VertexCount, "ComponentCount does not match");
        }
    }
}
