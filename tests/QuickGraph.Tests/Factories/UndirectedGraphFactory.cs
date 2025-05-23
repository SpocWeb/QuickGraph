// <copyright file="UndirectedGraphFactory.cs" company="MSIT">Copyright © MSIT 2007</copyright>

namespace QuickGraph
{
    public static partial class UndirectedGraphFactory
    {
        public static UndirectedGraph<int, SEdge<int>> Create(bool allowParallelEdges)
        {
            UndirectedGraph<int, SEdge<int>> undirectedGraph
               = new UndirectedGraph<int, SEdge<int>>(allowParallelEdges);

            return undirectedGraph;
            // TODO: Edit factory method of UndirectedGraph`2<Int32,SEdge`1<Int32>>
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
