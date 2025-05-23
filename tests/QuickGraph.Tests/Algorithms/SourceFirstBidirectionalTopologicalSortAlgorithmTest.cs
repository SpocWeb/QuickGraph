﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Serialization;
using QuickGraph.Algorithms.TopologicalSort;
using System;

namespace QuickGraph.Algorithms
{
    [TestClass]
    public partial class SourceFirstBidirectionalTopologicalSortAlgorithmTest
    {
        [TestMethod]

        public void SortAll()
        {
            foreach (var g in TestGraphFactory.GetBidirectionalGraphs())
            {
                Sort(g, TopologicalSortDirection.Forward);
                Sort(g, TopologicalSortDirection.Backward);
            }
        }

        
        public static void Sort<TVertex, TEdge>(IBidirectionalGraph<TVertex, TEdge> g, TopologicalSortDirection direction)
            where TEdge : IEdge<TVertex>
        {
            var topo = new SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>(g, direction);
            try
            {
                topo.Compute();
                //How to 
            }
            catch (NonAcyclicGraphException x)
            {
                Console.WriteLine("Cyclic Graph: " + x);
            }
        }

        [TestMethod]
        public void SortAnotherOne()
        {
            var g = new BidirectionalGraph<int, Edge<int>>();

            g.AddVertexRange(new int[5] { 0, 1, 2, 3, 4 });
            g.AddEdge(new Edge<int>(0, 1));
            g.AddEdge(new Edge<int>(1, 2));
            g.AddEdge(new Edge<int>(1, 3));
            g.AddEdge(new Edge<int>(2, 3));
            g.AddEdge(new Edge<int>(3, 4));

            SourceFirstBidirectionalTopologicalSortAlgorithm<int, Edge<int>> topo;

            topo = new SourceFirstBidirectionalTopologicalSortAlgorithm<int, Edge<int>>(g, TopologicalSortDirection.Forward);
            topo.Compute();

            topo = new SourceFirstBidirectionalTopologicalSortAlgorithm<int, Edge<int>>(g, TopologicalSortDirection.Backward);
            topo.Compute();
        }

        [TestMethod]
        //[DeploymentItem("GraphML/DCT8.graphml", "GraphML")]
        public void SortDCT()
        {
            var g = TestGraphFactory.LoadBidirectionalGraph("GraphML/DCT8.graphml");

            SourceFirstBidirectionalTopologicalSortAlgorithm<string, Edge<string>> topo;

            topo = new SourceFirstBidirectionalTopologicalSortAlgorithm<string, Edge<string>>(g, TopologicalSortDirection.Forward);
            topo.Compute();

            topo = new SourceFirstBidirectionalTopologicalSortAlgorithm<string, Edge<string>>(g, TopologicalSortDirection.Backward);
            topo.Compute();
        }
    }
}
