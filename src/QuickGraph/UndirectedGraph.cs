using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph
{
    public delegate bool EdgeEqualityComparer<TVertex, TEdge>(TEdge edge, TVertex source, TVertex target)
        where TEdge : IEdge<TVertex>;

#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class UndirectedGraph<TVertex, TEdge> 
        : IMutableUndirectedGraph<TVertex,TEdge>
#if !SILVERLIGHT
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
        private readonly bool allowParallelEdges = true;
        private readonly VertexEdgeDictionary<TVertex, TEdge> adjacentEdges =
            new VertexEdgeDictionary<TVertex, TEdge>();
        private readonly EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer;
        private int edgeCount = 0;
        private int edgeCapacity = 4;

        public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer)
        {
            Contract.Requires(edgeEqualityComparer != null);

            this.allowParallelEdges = allowParallelEdges;
            this.edgeEqualityComparer = edgeEqualityComparer;
        }

        public UndirectedGraph(bool allowParallelEdges)
            :this(allowParallelEdges, EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>())
        {
            this.allowParallelEdges = allowParallelEdges;
        }

        public UndirectedGraph()
            :this(true)
        {}

        public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer
        {
            get
            {
                Contract.Ensures(Contract.Result<EdgeEqualityComparer<TVertex, TEdge>>() != null);
                return edgeEqualityComparer;
            }
        }

        public int EdgeCapacity
        {
            get => edgeCapacity;
            set => edgeCapacity = value;
        }

        public IEnumerable<TVertex> AdjacentVertices(TVertex v)
        {
            var adjacentEdges = AdjacentEdges(v);
            var adjacentVertices = new HashSet<TVertex>();
            foreach (TEdge edge in adjacentEdges)
            {
                adjacentVertices.Add(edge.Source);
                adjacentVertices.Add(edge.Target);
            }

            adjacentVertices.Remove(v);

            return adjacentVertices;
        }

        public static UndirectedGraph<TVertex, TEdge> LoadDot(string dotSource,
            Func<string, IDictionary<string, string>, TVertex> vertexFunc,
            Func<TVertex, TVertex, IDictionary<string, string>, TEdge> edgeFunc)
        {
            Func<bool, IMutableVertexAndEdgeSet<TVertex, TEdge>> createGraph = (allowParallelEdges) =>
                new UndirectedGraph<TVertex, TEdge>(allowParallelEdges);

            return (UndirectedGraph<TVertex, TEdge>)
                DotParserAdapter.LoadDot(dotSource, createGraph, vertexFunc, edgeFunc);
        }

        public BidirectionalGraph<TVertex, TEdge> ToBidirectionalGraph()
        {
            var newGraph = new BidirectionalGraph<TVertex, TEdge>();

            newGraph.AddVertexRange(Vertices);
            newGraph.AddEdgeRange(Edges);
            return newGraph;
        }
        #region IGraph<Vertex,Edge> Members
        public bool  IsDirected => false;

        public bool  AllowParallelEdges => allowParallelEdges;

        #endregion

        #region IMutableUndirected<Vertex,Edge> Members
        public event VertexAction<TVertex> VertexAdded;
        protected virtual void OnVertexAdded(TVertex args)
        {
            Contract.Requires(args != null);

            var eh = VertexAdded;
            if (eh != null)
                eh(args);
        }

        public int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            int count = 0;
            foreach (var v in vertices)
                if (AddVertex(v))
                    count++;
            return count;
        }

        public bool AddVertex(TVertex v)
        {
            if (ContainsVertex(v))
                return false;

            var edges = EdgeCapacity < 0 
                ? new EdgeList<TVertex, TEdge>() 
                : new EdgeList<TVertex, TEdge>(EdgeCapacity);
            adjacentEdges.Add(v, edges);
            OnVertexAdded(v);
            return true;
        }

        private IEdgeList<TVertex, TEdge> AddAndReturnEdges(TVertex v)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (!adjacentEdges.TryGetValue(v, out edges))
                adjacentEdges[v] = edges = EdgeCapacity < 0 
                    ? new EdgeList<TVertex, TEdge>() 
                    : new EdgeList<TVertex, TEdge>(EdgeCapacity);

            return edges;
        }

        public event VertexAction<TVertex> VertexRemoved;
        protected virtual void OnVertexRemoved(TVertex args)
        {
            Contract.Requires(args != null);

            var eh = VertexRemoved;
            if (eh != null)
                eh(args);
        }

        public bool RemoveVertex(TVertex v)
        {
            ClearAdjacentEdges(v);
            bool result = adjacentEdges.Remove(v);

            if (result)
                OnVertexRemoved(v);

            return result;
        }

        public int RemoveVertexIf(VertexPredicate<TVertex> pred)
        {
            var vertices = new List<TVertex>();
            foreach (var v in Vertices)
                if (pred(v))
                    vertices.Add(v);

            foreach (var v in vertices)
                RemoveVertex(v);
            return vertices.Count;
        }
        #endregion

        #region IMutableIncidenceGraph<Vertex,Edge> Members
        public int RemoveAdjacentEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
        {
            var outEdges = adjacentEdges[v];
            var edges = new List<TEdge>(outEdges.Count);
            foreach (var edge in outEdges)
                if (predicate(edge))
                    edges.Add(edge);

            RemoveEdges(edges);
            return edges.Count;
        }

        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(edgeCount >= 0);
        }

        public void ClearAdjacentEdges(TVertex v)
        {
            var edges = adjacentEdges[v].Clone();
            edgeCount -= edges.Count;

            foreach (var edge in edges)
            {
                IEdgeList<TVertex, TEdge> aEdges;
                if (adjacentEdges.TryGetValue(edge.Target, out aEdges))
                    aEdges.Remove(edge);
                if (adjacentEdges.TryGetValue(edge.Source, out aEdges))
                    aEdges.Remove(edge);
            }
        }
        #endregion

        #region IMutableGraph<Vertex,Edge> Members
        public void TrimEdgeExcess()
        {
            foreach (var edges in adjacentEdges.Values)
                edges.TrimExcess();
        }

        public void Clear()
        {
            adjacentEdges.Clear();
            edgeCount = 0;
        }
        #endregion

        #region IUndirectedGraph<Vertex,Edge> Members
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (Comparer<TVertex>.Default.Compare(source, target) > 0)
            {
                var temp = source;
                source = target;
                target = temp;
            }

            foreach (var e in AdjacentEdges(source))
            {
                if (edgeEqualityComparer(e, source, target))
                {
                    edge = e;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        public bool ContainsEdge(TVertex source, TVertex target)
        {
            TEdge edge;
            return TryGetEdge(source, target, out edge);
        }

        public TEdge AdjacentEdge(TVertex v, int index)
        {
            return adjacentEdges[v][index];
        }

        public bool IsVerticesEmpty => adjacentEdges.Count == 0;

        public int VertexCount => adjacentEdges.Count;

        public IEnumerable<TVertex> Vertices => adjacentEdges.Keys;


        [Pure]
        public bool ContainsVertex(TVertex vertex)
        {
            return adjacentEdges.ContainsKey(vertex);
        }
        #endregion

        #region IMutableEdgeListGraph<Vertex,Edge> Members
        public bool AddVerticesAndEdge(TEdge edge)
        {
            var sourceEdges = AddAndReturnEdges(edge.Source);
            var targetEdges = AddAndReturnEdges(edge.Target);

            if (!AllowParallelEdges)
            {
                if (ContainsEdgeBetweenVertices(sourceEdges, edge))
                    return false;
            }

            sourceEdges.Add(edge);
            if (!edge.IsSelfEdge<TVertex, TEdge>())
                targetEdges.Add(edge);
            edgeCount++;
            OnEdgeAdded(edge);

            return true;
        }

        public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (AddVerticesAndEdge(edge))
                    count++;
            return count;
        }

        public bool AddEdge(TEdge edge)
        {
            var sourceEdges = adjacentEdges[edge.Source];
            if (!AllowParallelEdges)
            {
                if (ContainsEdgeBetweenVertices(sourceEdges, edge))
                    return false;
            }

            sourceEdges.Add(edge);
            if (!edge.IsSelfEdge<TVertex, TEdge>())
            {
                var targetEdges = adjacentEdges[edge.Target];
                targetEdges.Add(edge);
            }
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

        public event EdgeAction<TVertex, TEdge> EdgeAdded;
        protected virtual void OnEdgeAdded(TEdge args)
        {
            var eh = EdgeAdded;
            if (eh != null)
                eh(args);
        }

        public bool RemoveEdge(TEdge edge)
        {
            bool removed = adjacentEdges[edge.Source].Remove(edge);
            if (removed)
            {
                if (!edge.IsSelfEdge<TVertex, TEdge>())
                    adjacentEdges[edge.Target].Remove(edge);
                edgeCount--;
                Contract.Assert(edgeCount >= 0);
                OnEdgeRemoved(edge);
                return true;
            }
            else
                return false;
        }

        public event EdgeAction<TVertex, TEdge> EdgeRemoved;
        protected virtual void OnEdgeRemoved(TEdge args)
        {
            var eh = EdgeRemoved;
            if (eh != null)
                eh(args);
        }

        public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
        {
            List<TEdge> edges = new List<TEdge>();
            foreach (var edge in Edges)
            {
                if (predicate(edge))
                    edges.Add(edge);
            }
            return RemoveEdges(edges);
        }

        public int RemoveEdges(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
            {
                if (RemoveEdge(edge))
                    count++;
            }
            return count;
        }
        #endregion

        #region IEdgeListGraph<Vertex,Edge> Members
        public bool IsEdgesEmpty => EdgeCount==0;

        public int EdgeCount => edgeCount;

        public IEnumerable<TEdge> Edges
        {
            get 
            {
                var edgeColors = new Dictionary<TEdge, GraphColor>(EdgeCount);
                foreach (var edges in adjacentEdges.Values)
                {
                    foreach(TEdge edge in edges)
                    {
                        GraphColor c;
                        if (edgeColors.TryGetValue(edge, out c))
                            continue;
                        edgeColors.Add(edge, GraphColor.Black);
                        yield return edge;
                    }
                }
            }
        }

        public bool ContainsEdge(TEdge edge)
        {
            var eqc = EdgeEqualityComparer;
            foreach (var e in AdjacentEdges(edge.Source))
                if (e.Equals(edge))
                    return true;
            return false;
        }

        private bool ContainsEdgeBetweenVertices(IEnumerable<TEdge> edges, TEdge edge)
        {
            Contract.Requires(edges != null);
            Contract.Requires(edge != null);

            var source = edge.Source;
            var target= edge.Target;
            foreach (var e in edges)
                if (EdgeEqualityComparer(e,source, target))
                    return true;
            return false;
        }
        #endregion

        #region IUndirectedGraph<Vertex,Edge> Members
        public IEnumerable<TEdge> AdjacentEdges(TVertex v)
        {
            return adjacentEdges[v];
        }

        public int AdjacentDegree(TVertex v)
        {
            return adjacentEdges[v].Count;
        }

        public bool IsAdjacentEdgesEmpty(TVertex v)
        {
            return adjacentEdges[v].Count == 0;
        }
        #endregion

        #region ICloneable Members
        private UndirectedGraph(
            VertexEdgeDictionary<TVertex, TEdge> adjacentEdges,
            EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer,
            int edgeCount,
            int edgeCapacity,
            bool allowParallelEdges
            )
        {
            Contract.Requires(adjacentEdges != null);
            Contract.Requires(edgeEqualityComparer != null);
            Contract.Requires(edgeCount >= 0);

            this.adjacentEdges = adjacentEdges;
            this.edgeEqualityComparer = edgeEqualityComparer;
            this.edgeCount = edgeCount;
            this.edgeCapacity = edgeCapacity;
            this.allowParallelEdges = allowParallelEdges;
        }

        [Pure]
        public UndirectedGraph<TVertex, TEdge> Clone()
        {
            return new UndirectedGraph<TVertex, TEdge>(
                adjacentEdges.Clone(),
                edgeEqualityComparer,
                edgeCount,
                edgeCapacity,
                allowParallelEdges
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
