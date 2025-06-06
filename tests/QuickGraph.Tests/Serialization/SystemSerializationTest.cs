﻿using QuickGraph.Serialization;
using System.IO;
using QuickGraph.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QuickGraph.Tests.Serialization
{
    [TestClass]
    [Ignore("Serializer is deprecated!")]
    public class SystemSerializationTest
    {
        [TestMethod]
        [Ignore("Serializer is deprecated!")]
        [Obsolete("Serializer is deprecated!", true)]
        public void AdjacencyList()
        {
            var g = new AdjacencyGraph<int, Edge<int>>();
            //populate
            g.AddVertex(0);
            g.AddVertex(1);
            g.AddEdge(new Edge<int>(0, 1));

            var result = SerializeDeserialize<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(g);
            AssertGraphsEqual(g, result);
        }

        [TestMethod]
        [Ignore("Serializer is deprecated!")]
        [Obsolete("Serializer is deprecated!", true)]
        public void BidirectionalList()
        {
            var g = new BidirectionalGraph<int, Edge<int>>();
            //populate
            g.AddVertex(0);
            g.AddVertex(1);
            g.AddEdge(new Edge<int>(0, 1));

            var result = SerializeDeserialize<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(g);
            AssertGraphsEqual(g, result);
        }

        [TestMethod]
        [Ignore("Serializer is deprecated!")]
        [Obsolete("Serializer is deprecated!", true)]
        public void UndirectedGraph()
        {
            var g = new UndirectedGraph<int, Edge<int>>();
            //populate
            g.AddVertex(0);
            g.AddVertex(1);
            g.AddEdge(new Edge<int>(0, 1));

            var result = SerializeDeserialize<int, Edge<int>, UndirectedGraph<int, Edge<int>>>(g);
            AssertGraphsEqual(g, result);
        }

        [Obsolete("Serializer is deprecated!", true)]
        private static TGraph SerializeDeserialize<TVertex, TEdge, TGraph>(TGraph g)
            where TGraph : IGraph<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            Assert.IsNotNull(g);
            // serialize
            var stream = new MemoryStream();
            g.SerializeToBinary(stream);

            // deserialize
            stream.Position = 0;
            var result = stream.DeserializeFromBinary<TVertex, TEdge, TGraph>();
            Assert.IsNotNull(result);
            return result;
        }

        private static void AssertGraphsEqual(
            IEdgeListGraph<int, Edge<int>> g, 
            IEdgeListGraph<int, Edge<int>> result)
        {
            // check equal
            Assert.IsTrue(g.VertexCountEqual(result));
            Assert.IsTrue(g.EdgeCountEqual(result));
            foreach (var v in g.Vertices)
                Assert.IsTrue(result.ContainsVertex(v));
            //foreach (var e in g.Edges)
            //    Assert.IsTrue(result.ContainsEdge(e.Source, e.Target));
        }
    }
}
