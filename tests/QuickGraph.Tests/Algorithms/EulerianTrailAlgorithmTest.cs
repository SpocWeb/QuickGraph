using System;
using System.Collections.Generic;

using QuickGraph.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;

namespace QuickGraph.Algorithms
{
    [TestClass]
    public class EulerianTrailAlgorithmTest
    {
        //[TestMethod]
        //[Ignore]
        public void EulerianTrailAll()
        {
            IEnumerable<AdjacencyGraph<string, Edge<string>>> graphs = TestGraphFactory.GetAdjacencyGraphs();
            foreach (var g in graphs)
            {
                ComputeTrail(g, (s, t) => new Edge<string>(s, t));
            }
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs))]
        public void EulerianTrail(AdjacencyGraph<string, Edge<string>> g)
        {
            Console.WriteLine(g.ToString());
            ComputeTrail(g, (s, t) => new Edge<string>(s, t));
        }

        public static void ComputeTrail<TVertex,TEdge>(
            IMutableVertexAndEdgeListGraph<TVertex,TEdge> g,
            Func<TVertex, TVertex, TEdge> edgeCreator)
            where TEdge : IEdge<TVertex>
        {
            if (g.VertexCount == 0)
                return;

            int oddCount = 0;
            foreach (var v in g.Vertices)
                if (g.OutDegree(v) % 2 == 0)
                    oddCount++;

            int circuitCount = EulerianTrailAlgorithm<TVertex,TEdge>.ComputeEulerianPathCount(g);
            if (circuitCount == 0)
                return;

            var trail = new EulerianTrailAlgorithm<TVertex,TEdge>(g);
            trail.AddTemporaryEdges((s, t) => edgeCreator(s, t));
            trail.Compute();
            var trails = trail.Trails();
            trail.RemoveTemporaryEdges();

            Console.WriteLine("trails: {0}", trails.Count);
            //int index = 0;
            //foreach (var t in trails)
            //{
            //    Console.WriteLine("trail {0}", index++);
            //    foreach (TEdge edge in t)
            //        Console.WriteLine("\t{0}", t);
            //}

            // lets make sure all the edges are in the trail
            var edgeColors = new Dictionary<TEdge, GraphColor>(g.EdgeCount);
            foreach (var edge in g.Edges)
                edgeColors.Add(edge, GraphColor.White);
            foreach (var t in trails)
                foreach (var edge in t)
                    edgeColors.ContainsKey(edge).ShouldBeTrue();

        }
    }
}
