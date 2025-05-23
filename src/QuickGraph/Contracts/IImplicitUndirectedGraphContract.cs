﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph.Contracts
{
    [ContractClassFor(typeof(IImplicitUndirectedGraph<,>))]
    abstract class IImplicitUndirectedGraphContract<TVertex, TEdge> 
        : IImplicitUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        #region IImplicitUndirectedGraph<TVertex,TEdge> Members
        [Pure]
        EdgeEqualityComparer<TVertex, TEdge> IImplicitUndirectedGraph<TVertex, TEdge>.EdgeEqualityComparer
        {
            get
            {
                Contract.Ensures(Contract.Result<EdgeEqualityComparer<TVertex, TEdge>>() != null);
                return null;
            }
        }

        [Pure]
        IEnumerable<TEdge> IImplicitUndirectedGraph<TVertex, TEdge>.AdjacentEdges(TVertex v)
        {
            IImplicitUndirectedGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(Contract.Result<IEnumerable<TEdge>>() != null);
            Contract.Ensures(
                Contract.Result<IEnumerable<TEdge>>().All(edge => 
                        edge != null && 
                        ithis.ContainsEdge(edge.Source, edge.Target) && 
                        (edge.Source.Equals(v) || edge.Target.Equals(v))
                    )
                );

            return default(IEnumerable<TEdge>);
        }

        [Pure]
        int IImplicitUndirectedGraph<TVertex, TEdge>.AdjacentDegree(TVertex v)
        {
            IImplicitUndirectedGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(Contract.Result<int>() == ithis.AdjacentDegree(v));

            return default(int);
        }

        [Pure]
        bool IImplicitUndirectedGraph<TVertex, TEdge>.IsAdjacentEdgesEmpty(TVertex v)
        {
            IImplicitUndirectedGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(Contract.Result<bool>() == (ithis.AdjacentDegree(v) == 0));

            return default(bool);
        }

        [Pure]
        TEdge IImplicitUndirectedGraph<TVertex, TEdge>.AdjacentEdge(TVertex v, int index)
        {
            IImplicitUndirectedGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(v != null);
            Contract.Requires(ithis.ContainsVertex(v));
            Contract.Ensures(Contract.Result<TEdge>() != null);
            Contract.Ensures(
                Contract.Result<TEdge>().Source.Equals(v)
                || Contract.Result<TEdge>().Target.Equals(v));

            return default(TEdge);
        }

        [Pure]
        bool IImplicitUndirectedGraph<TVertex, TEdge>.TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            IImplicitUndirectedGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(source != null);
            Contract.Requires(target != null);

            edge = default(TEdge);
            return default(bool);
        }

        [Pure]
        bool IImplicitUndirectedGraph<TVertex, TEdge>.ContainsEdge(TVertex source, TVertex target)
        {
            IImplicitUndirectedGraph<TVertex, TEdge> ithis = this;
            Contract.Requires(source != null);
            Contract.Requires(target != null);
            Contract.Ensures(Contract.Result<bool>() == ithis.AdjacentEdges(source).Any(e => e.Target.Equals(target) || e.Source.Equals(target)));

            return default(bool);
        }
        #endregion

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
