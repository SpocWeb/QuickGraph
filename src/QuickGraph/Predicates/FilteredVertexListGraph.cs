﻿using System;
using System.Collections.Generic;

namespace QuickGraph.Predicates
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class FilteredVertexListGraph<TVertex, TEdge, Graph> 
        : FilteredIncidenceGraph<TVertex,TEdge,Graph>
        , IVertexListGraph<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
        where Graph : IVertexListGraph<TVertex,TEdge>
    {
        public FilteredVertexListGraph(
            Graph baseGraph,
            VertexPredicate<TVertex> vertexPredicate,
            EdgePredicate<TVertex, TEdge> edgePredicate
            )
            :base(baseGraph,vertexPredicate,edgePredicate)
        { }

        public bool IsVerticesEmpty
        {
            get 
            {
                foreach (var v in BaseGraph.Vertices)
                    if (VertexPredicate(v))
                        return false;
                return true;
            }
        }

        public int VertexCount
        {
            get 
            {
                int count = 0;
                foreach (var v in BaseGraph.Vertices)
                    if (VertexPredicate(v))
                        count++;
                return count;
            }
        }

        public IEnumerable<TVertex> Vertices
        {
            get 
            {
                foreach (var v in BaseGraph.Vertices)
                    if (VertexPredicate(v))
                        yield return v;
            }
        }
    }
}
