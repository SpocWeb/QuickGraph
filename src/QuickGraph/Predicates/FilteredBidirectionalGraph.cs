﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class FilteredBidirectionalGraph<TVertex, TEdge, TGraph> 
        : FilteredVertexListGraph<TVertex, TEdge, TGraph>
        , IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        public FilteredBidirectionalGraph(
            TGraph baseGraph,
            VertexPredicate<TVertex> vertexPredicate,
            EdgePredicate<TVertex, TEdge> edgePredicate
            )
            :base(baseGraph,vertexPredicate,edgePredicate)
        { }

        [Pure]
        public bool IsInEdgesEmpty(TVertex v)
        {
            return InDegree(v) == 0;
        }

        [Pure]
        public int InDegree(TVertex v)
        {
            int count = 0;
            foreach (var edge in BaseGraph.InEdges(v))
                if (TestEdge(edge))
                    count++;
            return count;
        }

        [Pure]
        public IEnumerable<TEdge> InEdges(TVertex v)
        {
            foreach (var edge in BaseGraph.InEdges(v))
                if (TestEdge(edge))
                    yield return edge;
        }

        [Pure]
        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            if (ContainsVertex(v))
            {
                edges = InEdges(v);
                return true;
            }
            else
            {
                edges = null;
                return false;
            }
        }

        [Pure]
        public int Degree(TVertex v)
        {
            return OutDegree(v) + InDegree(v);
        }

        public bool IsEdgesEmpty
        {
            get
            {
                foreach (var edge in BaseGraph.Edges)
                    if (TestEdge(edge))
                        return false;
                return true;
            }
        }

        public int EdgeCount
        {
            get
            {
                int count = 0;
                foreach (var edge in BaseGraph.Edges)
                    if (TestEdge(edge))
                        count++;
                return count;
            }
        }

        public IEnumerable<TEdge> Edges
        {
            get
            {
                foreach (var edge in BaseGraph.Edges)
                    if (TestEdge(edge))
                        yield return edge;
            }
        }

        [Pure]
        public bool ContainsEdge(TEdge edge)
        {
            if (!TestEdge(edge))
                return false;
            return BaseGraph.ContainsEdge(edge);
        }

        [Pure]
        public TEdge InEdge(TVertex v, int index)
        {
            throw new NotSupportedException();
        }
    }
}
