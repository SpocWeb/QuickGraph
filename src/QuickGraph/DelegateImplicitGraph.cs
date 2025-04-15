using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph
{
    /// <summary>
    /// A delegate-based implicit graph
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DelegateImplicitGraph<TVertex, TEdge>
        : IImplicitGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>, IEquatable<TEdge>
    {
        readonly TryFunc<TVertex, IEnumerable<TEdge>> tryGetOutEdges;

        public DelegateImplicitGraph(
            TryFunc<TVertex, IEnumerable<TEdge>> tryGetOutEdges)
        {
            Contract.Requires(tryGetOutEdges != null);

            this.tryGetOutEdges = tryGetOutEdges;
        }

        public TryFunc<TVertex, IEnumerable<TEdge>> TryGetOutEdgesFunc
        {
            get { return tryGetOutEdges; }
        }

        public bool IsOutEdgesEmpty(TVertex v)
        {
            foreach (var edge in OutEdges(v))
                return false;
            return true;
        }

        public int OutDegree(TVertex v)
        {
            return OutEdges(v).Count();
        }

        public IEnumerable<TEdge> OutEdges(TVertex v)
        {
            IEnumerable<TEdge> result;
            if (!tryGetOutEdges(v, out result))
                return Enumerable.Empty<TEdge>();
            return result;
        }

        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            return tryGetOutEdges(v, out edges);
        }

        public TEdge OutEdge(TVertex v, int index)
        {
            return OutEdges(v).ElementAt(index);
        }

        public bool IsDirected
        {
            get { return true; }
        }

        public bool AllowParallelEdges
        {
            get { return true; }
        }

        public bool ContainsVertex(TVertex vertex)
        {
            IEnumerable<TEdge> edges;
            return
                tryGetOutEdges(vertex, out edges);
        }
    }
}
