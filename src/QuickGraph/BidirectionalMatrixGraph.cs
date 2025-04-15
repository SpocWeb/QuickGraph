using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class BidirectionalMatrixGraph<TEdge> 
        : IBidirectionalGraph<int, TEdge>
        , IMutableEdgeListGraph<int, TEdge>
#if !SILVERLIGHT
        , ICloneable
#endif
        where TEdge : IEdge<int>
    {
        private readonly int vertexCount;
        private int edgeCount;
        private readonly TEdge[,] edges;

        public BidirectionalMatrixGraph(int vertexCount)        
        {
            Contract.Requires(vertexCount > 0);

            this.vertexCount = vertexCount;
            edgeCount = 0;
            edges = new TEdge[vertexCount, vertexCount];
        }

        #region IGraph
        public bool AllowParallelEdges => false;

        public bool IsDirected => true;

        #endregion

        #region IVertexListGraph
        public int VertexCount => vertexCount;

        public bool IsVerticesEmpty => VertexCount == 0;

        #endregion

        #region IEdgeListGraph
        public int EdgeCount => edgeCount;

        public bool IsEdgesEmpty => EdgeCount == 0;

        public IEnumerable<TEdge> Edges
        {
            get
            {
                for (int i = 0; i < VertexCount; ++i)
                {
                    for (int j = 0; j < VertexCount; ++j)
                    {
                        TEdge e = edges[i, j];
                        if (e != null)
                            yield return e;
                    }
                }
            }
        }
        #endregion

        #region IBidirectionalGraph<int,Edge> Members
        [Pure]
        public bool IsInEdgesEmpty(int v)
        {
            for (int i = 0; i < VertexCount; ++i)
                if (edges[i, v] != null)
                    return false;
            return true;
        }

        [Pure]
        public int InDegree(int v)
        {
            int count = 0;
            for (int i = 0; i < VertexCount; ++i)
                if (edges[i, v] != null)
                    count++;
            return count;
        }

        [Pure]
        public IEnumerable<TEdge> InEdges(int v)
        {
            for (int i = 0; i < VertexCount; ++i)
            {
                TEdge e = edges[i, v];
                if (e != null)
                    yield return e;
            }
        }

        [Pure]
        public bool TryGetInEdges(int v, out IEnumerable<TEdge> edges)
        {
            Contract.Ensures(Contract.Result<bool>() == (0 <= v && v > VertexCount));
            Contract.Ensures(
                Contract.Result<bool>() == 
                (Contract.ValueAtReturn<IEnumerable<TEdge>>(out edges) != null));

            if (v > -1 && v < vertexCount)
            {
                edges = InEdges(v);
                return true;
            }
            edges = null;
            return false;
        }

        [Pure]
        public TEdge InEdge(int v, int index)
        {
            int count = 0;
            for (int i = 0; i < VertexCount; ++i)
            {
                TEdge e = edges[i, v];
                if (e != null)
                {
                    if (count == index)
                        return e;
                    count++;
                }
            }
            throw new ArgumentOutOfRangeException("index");
        }

        [Pure]
        public int Degree(int v)
        {
            return InDegree(v) + OutDegree(v);
        }

        #endregion

        #region IIncidenceGraph<int,Edge> Members

        public bool ContainsEdge(int source, int target)
        {
            return edges[source, target] != null;
        }

        public bool TryGetEdge(int source, int target, out TEdge edge)
        {
            edge = edges[source, target];
            return edge != null;
        }

        public bool TryGetEdges(int source, int target, out IEnumerable<TEdge> edges)
        {
            TEdge edge;
            if (TryGetEdge(source, target, out edge))
            {
                edges = new TEdge[] { edge };
                return true;
            }
            else
            {
                edges = null;
                return false;
            }
        }

        #endregion

        #region IImplicitGraph<int,Edge> Members

        [Pure]
        public bool IsOutEdgesEmpty(int v)
        {
            for (int j = 0; j < vertexCount; ++j)
                if (edges[v, j] != null)
                    return false;
            return true;
        }

        [Pure]
        public int OutDegree(int v)
        {
            int count = 0;
            for (int j = 0; j < vertexCount; ++j)
                if (edges[v, j] != null)
                    count++;
            return count;
        }

        [Pure]
        public IEnumerable<TEdge> OutEdges(int v)
        {
            for (int j = 0; j < vertexCount; ++j)
            {
                TEdge e = edges[v, j];
                if (e != null)
                    yield return e;
            }
        }

        [Pure]
        public bool TryGetOutEdges(int v, out IEnumerable<TEdge> edges)
        {
            if (v > -1 && v < vertexCount)
            {
                edges = OutEdges(v);
                return true;
            }
            edges = null;
            return false;
        }

        [Pure]
        public TEdge OutEdge(int v, int index)
        {
            int count = 0;
            for (int j = 0; j < vertexCount; ++j)
            {
                TEdge e = edges[v, j];
                if (e != null)
                {
                    if (count==index)
                        return e;
                    count++;
                }
            }
            throw new ArgumentOutOfRangeException("index");
        }

        #endregion

        #region IVertexSet<int,Edge> Members

        public IEnumerable<int> Vertices
        {
            get 
            {
                for (int i = 0; i < VertexCount; ++i)
                    yield return i;
            }
        }

        [Pure]
        public bool ContainsVertex(int vertex)
        {
            return vertex >= 0 && vertex < VertexCount;
        }

        #endregion

        #region IEdgeListGraph<int,Edge> Members
        [Pure]
        public bool ContainsEdge(TEdge edge)
        {
            TEdge e = edges[edge.Source, edge.Target];
            return e!=null && 
                e.Equals(edge);
        }

        #endregion

        #region IMutableBidirectionalGraph<int,Edge> Members

        public int RemoveInEdgeIf(int v, EdgePredicate<int, TEdge> edgePredicate)
        {
            Contract.Requires(0 <= v && v < VertexCount);

            int count = 0;
            for (int i = 0; i < VertexCount; ++i)
            {
                TEdge e = edges[i, v];
                if (e != null && edgePredicate(e))
                {
                    RemoveEdge(e);
                    count++;
                }
            }
            return count;
        }

        public void ClearInEdges(int v)
        {
            Contract.Requires(0 <= v && v < VertexCount);

            for (int i = 0; i < VertexCount; ++i)
            {
                TEdge e = edges[i, v];
                if (e != null)
                    RemoveEdge(e);
            }
        }

        public void ClearEdges(int v)
        {
            Contract.Requires(0 <= v && v < VertexCount);

            ClearInEdges(v);
            ClearOutEdges(v);
        }

        #endregion

        #region IMutableIncidenceGraph<int,Edge> Members

        public int RemoveOutEdgeIf(int v, EdgePredicate<int, TEdge> predicate)
        {
            Contract.Requires(0 <= v && v < VertexCount);

            int count = 0;
            for (int j = 0; j < VertexCount; ++j)
            {
                TEdge e = edges[v, j];
                if (e != null && predicate(e))
                {
                    RemoveEdge(e);
                    count++;
                }
            }
            return count;
        }

        public void ClearOutEdges(int v)
        {
            Contract.Requires(0 <= v && v < VertexCount);

            for (int j = 0; j < VertexCount; ++j)
            {
                TEdge e = edges[v, j];
                if (e != null)
                    RemoveEdge(e);
            }
        }

        #endregion

        #region IMutableGraph<int,Edge> Members
        public void Clear()
        {
            for(int i = 0;i<vertexCount;++i)
                for(int j = 0;j<vertexCount;++j)
                    edges[i,j] = default(TEdge);
            edgeCount = 0;
        }
        #endregion

        #region IMutableEdgeListGraph<int,Edge> Members
        public bool AddEdge(TEdge edge)
        {
            if (edges[edge.Source, edge.Target]!=null)
                throw new ParallelEdgeNotAllowedException();
            edges[edge.Source,edge.Target] = edge;
            edgeCount++;
            OnEdgeAdded(edge);
            return true;
        }

        public int AddEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (AddEdge(edge))
                    count++;
            return count;
        }

        public event EdgeAction<int, TEdge> EdgeAdded;
        protected virtual void OnEdgeAdded(TEdge args)
        {
            var eh = EdgeAdded;
            if (eh != null)
                eh(args);
        }

        public bool RemoveEdge(TEdge edge)
        {
            TEdge e = edges[edge.Source, edge.Target];
            edges[edge.Source, edge.Target] = default(TEdge);
            if (!e.Equals(default(TEdge)))
            {
                edgeCount--;
                OnEdgeRemoved(edge);
                return true;
            }
            else
                return false;
        }

        public event EdgeAction<int, TEdge> EdgeRemoved;
        protected virtual void OnEdgeRemoved(TEdge args)
        {
            var eh = EdgeRemoved;
            if (eh != null)
                eh(args);
        }

        public int RemoveEdgeIf(EdgePredicate<int, TEdge> predicate)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ICloneable Members
        private BidirectionalMatrixGraph(
            int vertexCount,
            int edgeCount,
            TEdge[,] edges)
        {
            Contract.Requires(vertexCount > 0);
            Contract.Requires(edgeCount >= 0);
            Contract.Requires(edges != null);
            Contract.Requires(vertexCount == edges.GetLength(0));
            Contract.Requires(vertexCount == edges.GetLength(1));

            this.vertexCount = vertexCount;
            this.edgeCount = edgeCount;
            this.edges = edges;
        }

        public BidirectionalMatrixGraph<TEdge> Clone()
        {
            return new BidirectionalMatrixGraph<TEdge>(
                vertexCount,
                edgeCount,
                (TEdge[,])edges.Clone()
                );
        }

#if !SILVERLIGHT
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif
        #endregion
    }
}
