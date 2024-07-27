using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Serialization;

namespace QuickGraph.Tests
{
    [TestClass]
    public partial class DegreeTest
    {
        [TestMethod]
        public void DegreeSumEqualsTwiceEdgeCountAll()
        {
            foreach (var g in TestGraphFactory.GetBidirectionalGraphs())
                this.DegreeSumEqualsTwiceEdgeCount(g);
        }

        
        public void DegreeSumEqualsTwiceEdgeCount<TVertex, TEdge>(
            IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            int edgeCount = graph.EdgeCount;
            int degCount = 0;
            foreach (var v in graph.Vertices)
                degCount += graph.Degree(v);

            Assert.AreEqual(edgeCount * 2, degCount);
        }

        [TestMethod]
        public void InDegreeSumEqualsEdgeCountAll()
        {
            foreach (var g in TestGraphFactory.GetBidirectionalGraphs())
                this.InDegreeSumEqualsEdgeCount(g);
        }

        
        public void InDegreeSumEqualsEdgeCount<TVertex,TEdge>(
             IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            int edgeCount = graph.EdgeCount;
            int degCount = 0;
            foreach (var v in graph.Vertices)
                degCount += graph.InDegree(v);

            Assert.AreEqual(edgeCount, degCount);
        }

        [TestMethod]
        public void OutDegreeSumEqualsEdgeCountAll()
        {
            foreach (var g in TestGraphFactory.GetBidirectionalGraphs())
                this.OutDegreeSumEqualsEdgeCount(g);
        }

        
        public void OutDegreeSumEqualsEdgeCount<TVertex,TEdge>(
             IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            int edgeCount = graph.EdgeCount;
            int degCount = 0;
            foreach (var v in graph.Vertices)
                degCount += graph.OutDegree(v);

            Assert.AreEqual(edgeCount, degCount);
        }

    }
}
