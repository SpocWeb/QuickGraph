using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
    [ContractClassFor(typeof(IEdgeListGraph<,>))]
    abstract class IEdgeListGraphContract<TVertex, TEdge>
        : IEdgeListGraph<TVertex, TEdge>
      where TEdge : IEdge<TVertex>  
    {
        #region IGraph<TVertex,TEdge> Members
        bool IGraph<TVertex, TEdge>.IsDirected => throw new NotImplementedException();

        bool IGraph<TVertex, TEdge>.AllowParallelEdges => throw new NotImplementedException();

        #endregion

        #region IEdgeSet<TVertex,TEdge> Members

        bool IEdgeSet<TVertex, TEdge>.IsEdgesEmpty => throw new NotImplementedException();

        int IEdgeSet<TVertex, TEdge>.EdgeCount => throw new NotImplementedException();

        System.Collections.Generic.IEnumerable<TEdge> IEdgeSet<TVertex, TEdge>.Edges => throw new NotImplementedException();

        bool IEdgeSet<TVertex, TEdge>.ContainsEdge(TEdge edge)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IVertexSet<TVertex> Members

        public bool IsVerticesEmpty => throw new NotImplementedException();

        public int VertexCount => throw new NotImplementedException();

        public System.Collections.Generic.IEnumerable<TVertex> Vertices => throw new NotImplementedException();

        #endregion

        #region IImplicitVertexSet<TVertex> Members

        public bool ContainsVertex(TVertex vertex) {
          throw new NotImplementedException();
        }

        #endregion
    }
}
