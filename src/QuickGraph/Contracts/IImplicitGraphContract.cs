﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph.Contracts
{
    [ContractClassFor(typeof(IImplicitGraph<,>))]
    abstract class IImplicitGraphContract<TVertex, TEdge> 
        : IImplicitGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [Pure]
        bool IImplicitGraph<TVertex, TEdge>.IsOutEdgesEmpty(TVertex v)
        {
            IImplicitGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(Contract.Result<bool>() == (ithis.OutDegree(v) == 0));

            return default(bool);
        }

        [Pure]
        int IImplicitGraph<TVertex, TEdge>.OutDegree(TVertex v)
        {
            IImplicitGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(Contract.Result<int>() == ithis.OutEdges(v).Count<TEdge>());

            return default(int);
        }

        [Pure]
        IEnumerable<TEdge> IImplicitGraph<TVertex, TEdge>.OutEdges(TVertex v)
        {
            IImplicitGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(Contract.Result<IEnumerable<TEdge>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<TEdge>>().All(e => e.Source.Equals(v)));

            return default(IEnumerable<TEdge>);
        }

        [Pure]
        bool IImplicitGraph<TVertex, TEdge>.TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            IImplicitGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Ensures(!Contract.Result<bool>() || 
                (Contract.ValueAtReturn(out edges) != null && 
                 Contract.ValueAtReturn(out edges).All(e => e.Source.Equals(v)))
                );

            edges = null;
            return default(bool);
        }

        [Pure]
        TEdge IImplicitGraph<TVertex, TEdge>.OutEdge(TVertex v, int index)
        {
            IImplicitGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(ithis.OutEdges(v).Any(e => e.Equals(Contract.Result<TEdge>())));

            return default(TEdge);
        }

        #region IImplicitVertexSet<TVertex> Members
        bool IImplicitVertexSet<TVertex>.ContainsVertex(TVertex vertex)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IGraph<TVertex,TEdge> Members

        public bool IsDirected => throw new NotImplementedException();

        public bool AllowParallelEdges => throw new NotImplementedException();

        #endregion
    }
}
