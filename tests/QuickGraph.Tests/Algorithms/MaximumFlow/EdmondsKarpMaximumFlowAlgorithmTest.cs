
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.MaximumFlow;
using QuickGraph.Serialization;

namespace QuickGraph.Tests.Algorithms.MaximumFlow
{
    [TestClass]
    public partial class EdmondsKarpMaximumFlowAlgorithmTest
    {
        [DataTestMethod]
        [DynamicData(nameof(TestGraphFactory.GetAdjacencyGraphData), typeof(TestGraphFactory), DynamicDataSourceType.Method)]
        public void EdmondsKarpMaxFlowAll(AdjacencyGraph<string, Edge<string>> g)
        {
            if (g.VertexCount > 0)
                EdmondsKarpMaxFlow(g, (source, target) => new Edge<string>(source, target));
        }


        
        public static void EdmondsKarpMaxFlow<TVertex, TEdge>(IMutableVertexAndEdgeListGraph<TVertex, TEdge> g, 
            EdgeFactory<TVertex, TEdge> edgeFactory)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(g.VertexCount > 0);

            foreach (var source in g.Vertices)
                foreach (var sink in g.Vertices)
                {
                    if (source.Equals(sink)) continue;

                    RunMaxFlowAlgorithm(g, edgeFactory, source, sink);
                }
        }

        private static double RunMaxFlowAlgorithm<TVertex, TEdge>(IMutableVertexAndEdgeListGraph<TVertex, TEdge> g, EdgeFactory<TVertex, TEdge> edgeFactory, TVertex source, TVertex sink) where TEdge : IEdge<TVertex>
        {
            var reversedEdgeAugmentorAlgorithm = g.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            reversedEdgeAugmentorAlgorithm.AddReversedEdges();

            TryFunc<TVertex, TEdge> flowPredecessors;
            var flow = g.MaximumFlowEdmondsKarp(_ => 1,
                source, sink,
                out flowPredecessors,
                edgeFactory,
                reversedEdgeAugmentorAlgorithm
                );

            reversedEdgeAugmentorAlgorithm.RemoveReversedEdges();

            return flow;
        }

    }
}
