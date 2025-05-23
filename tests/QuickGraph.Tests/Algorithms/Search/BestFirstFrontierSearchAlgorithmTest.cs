﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms;
using QuickGraph.Serialization;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;

namespace QuickGraph.Tests.Algorithms.Search
{
    [TestClass]
    public partial class BestFirstFrontierSearchAlgorithmTest
    {
        [TestMethod]
        public void KrokFFig2Example()
        {
            var g = new BidirectionalGraph<char, SEquatableEdge<char>>();
            g.AddVerticesAndEdge(new SEquatableEdge<char>('A', 'C'));
            g.AddVerticesAndEdge(new SEquatableEdge<char>('A', 'B'));
            g.AddVerticesAndEdge(new SEquatableEdge<char>('B', 'E'));
            g.AddVerticesAndEdge(new SEquatableEdge<char>('B', 'D'));
            g.AddVerticesAndEdge(new SEquatableEdge<char>('E', 'F'));
            g.AddVerticesAndEdge(new SEquatableEdge<char>('E', 'G'));

            RunSearch(g);
        }

        [TestMethod]
        public void BestFirstFrontierSearchAllGraphs()
        {
            foreach (var g in TestGraphFactory.GetBidirectionalGraphs())
                RunSearch(g);
        }

        
        public static void RunSearch<TVertex, TEdge>(
            IBidirectionalGraph<TVertex, TEdge> g)
            where TEdge : IEdge<TVertex>
        {
            if (g.VertexCount == 0) return;

            Func<TEdge, double> edgeWeights = _ => 1;
            var distanceRelaxer = DistanceRelaxers.ShortestDistance;

            var search = new BestFirstFrontierSearchAlgorithm<TVertex, TEdge>(
                null,
                g,
                edgeWeights,
                distanceRelaxer);
            var root = g.Vertices.First();
            var target = g.Vertices.Last();
            var recorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>();

            using (recorder.Attach(search))
                search.Compute(root, target);

            if (recorder.VertexPredecessors.ContainsKey(target))
            {
                Console.WriteLine("cost: {0}", recorder.VertexPredecessors[target]);
                IEnumerable<TEdge> path;
                Assert.IsTrue(recorder.TryGetPath(target, out path));
            }
#if DEBUG
            Console.WriteLine("operator max count: {0}", search.OperatorMaxCount);
#endif
        }

        [TestMethod]
        public void CompareBestFirstFrontierSearchAllGraphs()
        {
            foreach (var g in TestGraphFactory.GetBidirectionalGraphs())
            {
                if (g.VertexCount == 0) continue;

                var root = g.Vertices.First();
                foreach(var v in g.Vertices)
                    if(!root.Equals(v))
                        CompareSearch(g, root, v);
            }
        }
        
        
        public static void CompareSearch<TVertex, TEdge>(
            IBidirectionalGraph<TVertex, TEdge> g,
            TVertex root, TVertex target)
            where TEdge: IEdge<TVertex>
        {
            Func<TEdge, double> edgeWeights = _ => 1;
            var distanceRelaxer = DistanceRelaxers.ShortestDistance;

            var search = new BestFirstFrontierSearchAlgorithm<TVertex, TEdge>(
                null, 
                g, 
                edgeWeights, 
                distanceRelaxer);
            var recorder = new VertexDistanceRecorderObserver<TVertex, TEdge>(edgeWeights);
            using(recorder.Attach(search))
                search.Compute(root, target);

            var dijkstra = new DijkstraShortestPathAlgorithm<TVertex, TEdge>(g, edgeWeights, distanceRelaxer);
            var dijRecorder = new VertexDistanceRecorderObserver<TVertex, TEdge>(edgeWeights);
            using (dijRecorder.Attach(dijkstra))
                dijkstra.Compute(root);

            var fvp = recorder.Distances;
            var dvp = dijRecorder.Distances;
            double cost;
            if (dvp.TryGetValue(target, out cost))
            {
                Assert.IsTrue(fvp.ContainsKey(target), "target {0} not found, should be {1}", target, cost);
                Assert.AreEqual(dvp[target], fvp[target]);
            }
        }
    }
}
