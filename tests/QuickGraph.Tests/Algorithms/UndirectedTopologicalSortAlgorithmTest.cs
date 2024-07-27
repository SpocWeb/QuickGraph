using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph.Algorithms.TopologicalSort;

namespace QuickGraph.Algorithms
{
    [TestClass]
    public partial class UndirectedTopologicalSortAlgorithmTest
    {
        
        public void Compute(IUndirectedGraph<string, Edge<string>> g)
        {
            UndirectedTopologicalSortAlgorithm<string, Edge<string>> topo =
                new UndirectedTopologicalSortAlgorithm<string, Edge<string>>(g);
            topo.AllowCyclicGraph = true;
            topo.Compute();

            Display(topo);
        }

        private void Display(UndirectedTopologicalSortAlgorithm<string, Edge<string>> topo)
        {
            int index = 0;
            foreach (string v in topo.SortedVertices)
                Console.WriteLine("{0}: {1}", index++, v);
        }
    }
}
