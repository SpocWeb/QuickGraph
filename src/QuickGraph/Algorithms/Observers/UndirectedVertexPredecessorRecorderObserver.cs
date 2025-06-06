﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <reference-ref
    ///     idref="boost"
    ///     />
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge> :
        IObserver<IUndirectedTreeBuilderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private readonly IDictionary<TVertex, TEdge> vertexPredecessors;

        public UndirectedVertexPredecessorRecorderObserver()
            :this(new Dictionary<TVertex,TEdge>())
        {}

        public UndirectedVertexPredecessorRecorderObserver(
            IDictionary<TVertex, TEdge> vertexPredecessors)
        {
            Contract.Requires(vertexPredecessors != null);

            this.vertexPredecessors = vertexPredecessors;
        }

        public IDictionary<TVertex, TEdge> VertexPredecessors => vertexPredecessors;

        public IDisposable Attach(IUndirectedTreeBuilderAlgorithm<TVertex, TEdge> algorithm)
        {
            algorithm.TreeEdge += TreeEdge;
            return new DisposableAction(() => algorithm.TreeEdge -= TreeEdge);
        }

        void TreeEdge(object sender, UndirectedEdgeEventArgs<TVertex,TEdge> e)
        {
            vertexPredecessors[e.Target] = e.Edge;
        }

        public bool TryGetPath(TVertex vertex, out IEnumerable<TEdge> path)
        {
            return VertexPredecessors.TryGetPath(vertex, out path);
        }
    }
}
