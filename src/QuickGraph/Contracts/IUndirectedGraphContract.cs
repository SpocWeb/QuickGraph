﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
    [ContractClassFor(typeof(IUndirectedGraph<,>))]
    abstract class IUndirectedGraphContract<TVertex, TEdge>
        : IUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        #region IVertexSet<TVertex> Members

        bool IVertexSet<TVertex>.IsVerticesEmpty => throw new NotImplementedException();

        int IVertexSet<TVertex>.VertexCount => throw new NotImplementedException();

        IEnumerable<TVertex> IVertexSet<TVertex>.Vertices => throw new NotImplementedException();

        bool IImplicitVertexSet<TVertex>.ContainsVertex(TVertex vertex)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraph<TVertex,TEdge> Members

        bool IGraph<TVertex, TEdge>.IsDirected => throw new NotImplementedException();

        bool IGraph<TVertex, TEdge>.AllowParallelEdges => throw new NotImplementedException();

        #endregion

        #region IEdgeSet<TVertex,TEdge> Members

        bool IEdgeSet<TVertex, TEdge>.IsEdgesEmpty => throw new NotImplementedException();

        int IEdgeSet<TVertex, TEdge>.EdgeCount => throw new NotImplementedException();

        IEnumerable<TEdge> IEdgeSet<TVertex, TEdge>.Edges => throw new NotImplementedException();

        bool IEdgeSet<TVertex, TEdge>.ContainsEdge(TEdge edge)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IImplicitUndirectedGraph<TVertex,TEdge> Members
        [Pure]
        EdgeEqualityComparer<TVertex, TEdge> IImplicitUndirectedGraph<TVertex, TEdge>.EdgeEqualityComparer => throw new NotImplementedException();

        IEnumerable<TEdge> IImplicitUndirectedGraph<TVertex, TEdge>.AdjacentEdges(TVertex v)
        {
            throw new NotImplementedException();
        }

        int IImplicitUndirectedGraph<TVertex, TEdge>.AdjacentDegree(TVertex v)
        {
            throw new NotImplementedException();
        }

        bool IImplicitUndirectedGraph<TVertex, TEdge>.IsAdjacentEdgesEmpty(TVertex v)
        {
            throw new NotImplementedException();
        }

        TEdge IImplicitUndirectedGraph<TVertex, TEdge>.AdjacentEdge(TVertex v, int index)
        {
            throw new NotImplementedException();
        }

        bool IImplicitUndirectedGraph<TVertex, TEdge>.ContainsEdge(TVertex source, TVertex target)
        {
            throw new NotImplementedException();
        }

        bool IImplicitUndirectedGraph<TVertex, TEdge>.TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
