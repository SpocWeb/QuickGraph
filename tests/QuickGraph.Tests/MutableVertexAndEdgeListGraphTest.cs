﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph
{
    [TestClass]
    public partial class MutableVertexAndEdgeListGraphTest
    {
        
        public void AddVertexOnly(IMutableVertexAndEdgeListGraph<string, Edge<string>> g, string v)
        {
            int vertexCount = g.VertexCount;
            g.AddVertex(v);
            Assert.AreEqual(vertexCount + 1, g.VertexCount);
            Assert.IsTrue(g.ContainsVertex(v));
            VerifyCounts(g);
        }

        
        public void AddAndRemoveVertex(IMutableVertexAndEdgeListGraph<int, Edge<int>> g, int v)
        {
            int vertexCount = g.VertexCount;
            g.AddVertex(v);
            Assert.AreEqual(vertexCount + 1, g.VertexCount);
            Assert.IsTrue(g.ContainsVertex(v));
            g.RemoveVertex(v);
            Assert.AreEqual(vertexCount, g.VertexCount);
            Assert.IsFalse(g.ContainsVertex(v));
            //VerifyCounts(g);
        }

        
        public void AddVertexAddEdgesAndRemoveTargetVertex(IMutableVertexAndEdgeListGraph<string, Edge<string>> g, string v1, string v2)
        {
            int vertexCount = g.VertexCount;
            int edgeCount = g.EdgeCount;

            g.AddVertex(v1);
            g.AddVertex(v2);
            Assert.AreEqual(vertexCount + 2, g.VertexCount);
            Assert.IsTrue(g.ContainsVertex(v1));
            Assert.IsTrue(g.ContainsVertex(v2));

            g.AddEdge(new Edge<string>(v1, v2));
            Assert.AreEqual(edgeCount + 1, g.EdgeCount);

            g.RemoveVertex(v2);
            Assert.AreEqual(vertexCount + 1, g.VertexCount);
            Assert.AreEqual(edgeCount, g.EdgeCount);
            Assert.IsTrue(g.ContainsVertex(v1));
            Assert.IsFalse(g.ContainsVertex(v2));
            VerifyCounts(g);
        }

        
        public void AddVertexAddEdgesAndRemoveSourceVertex(IMutableVertexAndEdgeListGraph<string, Edge<string>> g, string v1, string v2)
        {
            int vertexCount = g.VertexCount;
            int edgeCount = g.EdgeCount;

            g.AddVertex(v1);
            g.AddVertex(v2);
            Assert.AreEqual(vertexCount + 2, g.VertexCount);
            Assert.IsTrue(g.ContainsVertex(v1));
            Assert.IsTrue(g.ContainsVertex(v2));

            g.AddEdge(new Edge<string>(v1, v2));
            Assert.AreEqual(edgeCount + 1, g.EdgeCount);

            g.RemoveVertex(v1);
            Assert.AreEqual(vertexCount + 1, g.VertexCount);
            Assert.AreEqual(edgeCount, g.EdgeCount);
            Assert.IsTrue(g.ContainsVertex(v2));
            Assert.IsFalse(g.ContainsVertex(v1));
            VerifyCounts(g);
        }

        private static void 
            VerifyCounts(IMutableVertexAndEdgeListGraph<string, Edge<string>> g)
        {
            int i = 0;
            foreach (string v in g.Vertices)
                i++;
            Assert.AreEqual(g.VertexCount, i);

            i = 0;
            foreach (string v in g.Vertices)
                foreach (Edge<string> e in g.OutEdges(v))
                    i++;
            Assert.AreEqual(g.EdgeCount, i);

            i = 0;
            foreach (Edge<string> e in g.Edges)
                i++;
            Assert.AreEqual(g.EdgeCount, i);
        }
    }
}
