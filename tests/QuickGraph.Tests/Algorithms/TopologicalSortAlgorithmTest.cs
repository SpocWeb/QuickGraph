﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Serialization;
using QuickGraph.Algorithms.TopologicalSort;
using System.Text;
using System.Collections.Generic;

namespace QuickGraph.Algorithms
{
    [TestClass]
    public partial class TopologicalSortAlgorithmTest
    {
        [DataTestMethod]
        [DynamicData(nameof(TestGraphFactory.GetAdjacencyGraphData), typeof(TestGraphFactory), DynamicDataSourceType.Method)]
        public void TopologicalSortAll(AdjacencyGraph<string, Edge<string>> g) => SortCyclic(g);

        
        public static void SortCyclic<TVertex,TEdge>(
            IVertexListGraph<TVertex, TEdge> g)
            where TEdge : IEdge<TVertex>
        {
            var topo = new TopologicalSortAlgorithm<TVertex, TEdge>(g);
            topo.Compute();
        }

        [TestMethod]
        //[DeploymentItem("GraphML/DCT8.graphml", "GraphML")]
        public void SortDCT8()
        {
            var g = TestGraphFactory.LoadGraph("GraphML/DCT8.graphml");
            var topo = new TopologicalSortAlgorithm<string, Edge<string>>(g);
            Assert.IsFalse(topo.AllowCyclicGraph);
            topo.Compute();
        }

        [TestMethod]
        public void OneTwo()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddEdge(new Edge<int>(1, 2));
            var t = new TopologicalSortAlgorithm<int, Edge<int>>(graph);
            var vertices = new List<int>(graph.VertexCount);
            t.Compute(vertices);
            Assert.AreEqual(2, vertices.Count);
        }

        // Trying to see if order of vertices affects the topological sort order.
        [TestMethod]
        public void TwoOne()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            // Deliberately adding 1 and then 2, before adding edge (2, 1).
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddEdge(new Edge<int>(2, 1));
            var t = new TopologicalSortAlgorithm<int, Edge<int>>(graph);
            var vertices = new List<int>(graph.VertexCount);
            t.Compute(vertices);
            Assert.AreEqual(2, vertices.Count);
        }

        [TestMethod]
        public void FacebookSeattleWordPuzzle()
        {
            /* A puzzle from Facebook Seattle opening party:
             * http://www.facebook.com/note.php?note_id=146727365346299
             You are given a list of relationships between the letters in a single word, all of which are in the form: 
            "The first occurrence of A comes before N occurrences of B." 
            You can safely assume that you have all such relationships except for any in which N would be 0. 
            Determine the original word, then go to http://www.facebook.com/seattle/[insert-word-here] to find the second part of the puzzle.

            The first occurrence of 'e' comes before 1 occurrence of 's'.
            The first occurrence of 'i' comes before 1 occurrence of 'n'.
            The first occurrence of 'i' comes before 1 occurrence of 'i'.
            The first occurrence of 'n' comes before 2 occurrences of 'e'.
            The first occurrence of 'e' comes before 1 occurrence of 'e'.
            The first occurrence of 'i' comes before 1 occurrence of 'v'.
            The first occurrence of 'n' comes before 1 occurrence of 'i'.
            The first occurrence of 'n' comes before 1 occurrence of 'v'.
            The first occurrence of 'i' comes before 1 occurrence of 's'.
            The first occurrence of 't' comes before 1 occurrence of 's'.
            The first occurrence of 'v' comes before 1 occurrence of 's'.
            The first occurrence of 'v' comes before 2 occurrences of 'e'.
            The first occurrence of 't' comes before 2 occurrences of 'e'.
            The first occurrence of 'i' comes before 2 occurrences of 'e'.
            The first occurrence of 'v' comes before 1 occurrence of 't'.
            The first occurrence of 'n' comes before 1 occurrence of 't'.
            The first occurrence of 'v' comes before 1 occurrence of 'i'.
            The first occurrence of 'i' comes before 1 occurrence of 't'.
            The first occurrence of 'n' comes before 1 occurrence of 's'. 
             */
            var graph = new AdjacencyGraph<Letter, Edge<Letter>>();

            //A more generalized algorithm would handle duplicate letters automatically.
            //This is the quick and dirty solution.
            var i1 = new Letter('i');
            var i2 = new Letter('i');
            var e1 = new Letter('e');
            var e2 = new Letter('e');

            var s = new Letter('s');
            var n = new Letter('n');
            var t = new Letter('t');
            var v = new Letter('v');

            graph.AddVertexRange(new List<Letter> {e1,e2,s,i1,i2,n,t,v});
            
            graph.AddEdge(new Edge<Letter>(e1, s));
            graph.AddEdge(new Edge<Letter>(i1, n));
            graph.AddEdge(new Edge<Letter>(i1, i2));
            graph.AddEdge(new Edge<Letter>(n, e1));
            graph.AddEdge(new Edge<Letter>(n, e2));
            graph.AddEdge(new Edge<Letter>(e1, e2));
            graph.AddEdge(new Edge<Letter>(i1, v));
            graph.AddEdge(new Edge<Letter>(n, e1));
            graph.AddEdge(new Edge<Letter>(n, v));
            graph.AddEdge(new Edge<Letter>(i1, s));
            graph.AddEdge(new Edge<Letter>(t, s));
            graph.AddEdge(new Edge<Letter>(v, s));
            graph.AddEdge(new Edge<Letter>(v, e1));
            graph.AddEdge(new Edge<Letter>(v, e2));
            graph.AddEdge(new Edge<Letter>(t, e1));
            graph.AddEdge(new Edge<Letter>(t, e2));
            graph.AddEdge(new Edge<Letter>(i1, e1));
            graph.AddEdge(new Edge<Letter>(i1, e2));
            graph.AddEdge(new Edge<Letter>(v, t));
            graph.AddEdge(new Edge<Letter>(n, t));
            graph.AddEdge(new Edge<Letter>(v, i2));
            graph.AddEdge(new Edge<Letter>(i1, t));
            graph.AddEdge(new Edge<Letter>(n, s));


            var sort = new TopologicalSortAlgorithm<Letter, Edge<Letter>>(graph);
            sort.Compute();

            StringBuilder builder = new StringBuilder();
            foreach (var item in sort.SortedVertices)
            {
                builder.Append(item.ToString());
            }
            var word = builder.ToString();

            Assert.AreEqual("invitees", word);
        }
    }

    public class Letter
    {
        public char Char;

        public Letter(char letter)
        {
            Char = letter;
        }

        public override string ToString()
        {
            return Char.ToString();
        }

    }
}
