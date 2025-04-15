using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Algorithms;

namespace QuickGraph.Tests.Regression
{
    [TestClass]
    public class ShortestPathBellmanFordTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Repro12901()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            int vertex = 1;
            graph.AddVerticesAndEdge(new Edge<int>(vertex, vertex));
            var pathFinder = graph.ShortestPathsBellmanFord(_ => -1.0, vertex);
            IEnumerable<Edge<int>> path;
            pathFinder(vertex, out path);
        }
    }
}
