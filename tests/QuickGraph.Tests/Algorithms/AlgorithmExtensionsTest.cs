﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Algorithms;

using QuickGraph.Serialization;

namespace QuickGraph.Tests.Algorithms
{
    [TestClass]
    public partial class AlgorithmExtensionsTest
    {
        [TestMethod]
        public void AdjacencyGraphRoots()
        {
            var g = new AdjacencyGraph<string, Edge<string>>();
            g.AddVertex("A");
            g.AddVertex("B");
            g.AddVertex("C");

            g.AddEdge(new Edge<string>("A", "B"));
            g.AddEdge(new Edge<string>("B", "C"));

            var roots = g.Roots().ToList();
            Assert.AreEqual(1, roots.Count);
            Assert.AreEqual("A", roots[0]);
        }

        [TestMethod]
        public void AllAdjacencyGraphRoots()
        {
            foreach (var g in TestGraphFactory.GetAdjacencyGraphs())
                Roots(g);
        }

        
        public void Roots<T>(IVertexAndEdgeListGraph<T, Edge<T>> g)
        {
            var roots = new HashSet<T>(g.Roots());
            foreach (var edge in g.Edges)
                Assert.IsFalse(roots.Contains(edge.Target));
        }
    }
}
