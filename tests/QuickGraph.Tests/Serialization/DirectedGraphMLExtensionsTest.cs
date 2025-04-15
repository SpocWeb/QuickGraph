using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Serialization;
using System.Diagnostics;

namespace QuickGraph.Tests.Serialization
{
    [TestClass]
    public partial class DirectedGraphMLExtensionsTest
    {
        [TestMethod]
        public void SimpleGraph()
        {
            int[][] edges = { new int[]{ 1, 2, 3 }, 
                              new int[]{ 2, 3, 1 } };
            edges.ToAdjacencyGraph()
                .ToDirectedGraphML()
                .WriteXml("simple.dgml");

            if (Debugger.IsAttached)
            { 
                Process.Start("simple.dgml");
            }

            edges.ToAdjacencyGraph()
                .ToDirectedGraphML()
                .WriteXml(Console.Out);
        }

        [DataTestMethod]
        [DynamicData(nameof(TestGraphFactory.GetAdjacencyGraphData), typeof(TestGraphFactory), DynamicDataSourceType.Method)]
        public void ToDirectedGraphML(AdjacencyGraph<string, Edge<string>> g)
        {
            var dg = g.ToDirectedGraphML();
            Assert.IsNotNull(g);
            Assert.AreEqual(dg.Nodes.Length, g.VertexCount);
            Assert.AreEqual(dg.Links.Length, g.EdgeCount);
        }
    }
}
