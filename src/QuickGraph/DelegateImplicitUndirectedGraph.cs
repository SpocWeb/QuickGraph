using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
    /// <summary>
    /// A functional implicit undirected graph
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DelegateImplicitUndirectedGraph<TVertex, TEdge>
        : IImplicitUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        readonly TryFunc<TVertex, IEnumerable<TEdge>> tryGetAdjacentEdges;
        readonly bool allowParallelEdges;
        readonly EdgeEqualityComparer<TVertex, TEdge> edgeEquality =
            EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();

        public DelegateImplicitUndirectedGraph(
            TryFunc<TVertex, IEnumerable<TEdge>> tryGetAdjacenyEdges,
            bool allowParallelEdges)
        {
            Contract.Requires(tryGetAdjacenyEdges != null);

            tryGetAdjacentEdges = tryGetAdjacenyEdges;
            this.allowParallelEdges = allowParallelEdges;
        }

        public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer => edgeEquality;

        public TryFunc<TVertex, IEnumerable<TEdge>> TryGetAdjacencyEdgesFunc => tryGetAdjacentEdges;

        public bool IsAdjacentEdgesEmpty(TVertex v)
        {
            foreach (var edge in AdjacentEdges(v))
                return false;
            return true;
        }

        public int AdjacentDegree(TVertex v)
        {
            return AdjacentEdges(v).Count();
        }

        public IEnumerable<TEdge> AdjacentEdges(TVertex v)
        {
            IEnumerable<TEdge> result;
            if (!tryGetAdjacentEdges(v, out result))
                return Enumerable.Empty<TEdge>();
            return result;
        }

        public bool TryGetAdjacentEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            return tryGetAdjacentEdges(v, out edges);
        }

        public TEdge AdjacentEdge(TVertex v, int index)
        {
            return AdjacentEdges(v).ElementAt(index);
        }

        public bool IsDirected => false;

        public bool AllowParallelEdges => allowParallelEdges;

        public bool ContainsVertex(TVertex vertex)
        {
            IEnumerable<TEdge> edges;
            return
                tryGetAdjacentEdges(vertex, out edges);
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            IEnumerable<TEdge> edges;
            if (TryGetAdjacentEdges(source, out edges))
                foreach (var e in edges)
                    if (edgeEquality(e, source, target))
                    {
                        edge = e;
                        return true;
                    }

            edge = default(TEdge);
            return false;
        }

        public bool ContainsEdge(TVertex source, TVertex target)
        {
            TEdge edge;
            return TryGetEdge(source, target, out edge);
        }
    }
}
