using System;
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
    public sealed class VertexPredecessorPathRecorderObserver<TVertex, TEdge> :
        IObserver<IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private readonly IDictionary<TVertex, TEdge> vertexPredecessors;
        private readonly List<TVertex> endPathVertices = new List<TVertex>();

        public VertexPredecessorPathRecorderObserver()
            :this(new Dictionary<TVertex,TEdge>())
        {}

        public VertexPredecessorPathRecorderObserver(
            IDictionary<TVertex, TEdge> vertexPredecessors)
        {
            Contract.Requires(vertexPredecessors != null);
            this.vertexPredecessors = vertexPredecessors;
        }

        public IDictionary<TVertex, TEdge> VertexPredecessors
        {
            get { return vertexPredecessors; }
        }

        public ICollection<TVertex> EndPathVertices
        {
            get { return endPathVertices; }
        }

        public IDisposable Attach(IVertexPredecessorRecorderAlgorithm<TVertex, TEdge> algorithm)
        {
            algorithm.TreeEdge += TreeEdge;
            algorithm.FinishVertex += FinishVertex;
            return new DisposableAction(
                () =>
                {
                    algorithm.TreeEdge -= TreeEdge;
                    algorithm.FinishVertex -= FinishVertex;
                });
        }

        void TreeEdge(TEdge e)
        {
            VertexPredecessors[e.Target] = e;
        }

        void FinishVertex(TVertex v)
        {
            foreach (var edge in VertexPredecessors.Values)
            {
                if (edge.Source.Equals(v))
                    return;
            }
            endPathVertices.Add(v);
        }

        public IEnumerable<IEnumerable<TEdge>> AllPaths()
        {
            List<IEnumerable<TEdge>> es = new List<IEnumerable<TEdge>>();
            foreach (var v in EndPathVertices)
            {
                IEnumerable<TEdge> path;
                if (vertexPredecessors.TryGetPath(v, out path))
                    es.Add(path);
            }
            return es;
        }
    }
}
