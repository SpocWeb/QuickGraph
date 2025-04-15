using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class FilteredVertexAndEdgeListGraph<TVertex, TEdge, TGraph> :
        FilteredVertexListGraph<TVertex, TEdge, TGraph>,
        IVertexAndEdgeListGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        public FilteredVertexAndEdgeListGraph(
            TGraph baseGraph,
            VertexPredicate<TVertex> vertexPredicate,
            EdgePredicate<TVertex, TEdge> edgePredicate
            )
            :base(baseGraph,vertexPredicate,edgePredicate)
        { }

        public bool IsEdgesEmpty => EdgeCount == 0;

        public int EdgeCount
        {
            get
            {
                int count = 0;
                foreach (var edge in BaseGraph.Edges)
                {
                    if (
                           VertexPredicate(edge.Source)
                        && VertexPredicate(edge.Target)
                        && EdgePredicate(edge))
                        count++;
                }
                return count;
            }
        }

        public IEnumerable<TEdge> Edges
        {
            get
            {
                foreach(TEdge edge in BaseGraph.Edges)
                {
                    if (
                           VertexPredicate(edge.Source)
                        && VertexPredicate(edge.Target)
                        && EdgePredicate(edge))
                        yield return edge;
                }
            }
        }

        [Pure]
        public bool ContainsEdge(TEdge edge)
        {
            foreach (var e in Edges)
                if (Equals(edge, e))
                    return true;
            return false;
        }
    }
}
