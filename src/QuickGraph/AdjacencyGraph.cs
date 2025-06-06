﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph
{
    /// <summary>
    /// A mutable directed graph data structure efficient for sparse
    /// graph representation where out-edge need to be enumerated only.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class AdjacencyGraph<TVertex,TEdge> 
        : IVertexAndEdgeListGraph<TVertex,TEdge>
        , IEdgeListAndIncidenceGraph<TVertex,TEdge>
        , IMutableEdgeListGraph<TVertex,TEdge>
        , IMutableIncidenceGraph<TVertex,TEdge>
        , IMutableVertexListGraph<TVertex,TEdge>
        , IMutableVertexAndEdgeListGraph<TVertex,TEdge>
#if !SILVERLIGHT
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
        private readonly bool isDirected = true;
        private readonly bool allowParallelEdges;
        private readonly IVertexEdgeDictionary<TVertex, TEdge> vertexEdges;
        private int edgeCount = 0;
        private int edgeCapacity = -1;

        public AdjacencyGraph()
            :this(true)
        {}

        public AdjacencyGraph(bool allowParallelEdges)
            :this(allowParallelEdges,-1)
        {
        }

        public AdjacencyGraph(bool allowParallelEdges, int capacity)
            :this(allowParallelEdges, capacity, -1)
        {
        }

        public AdjacencyGraph(bool allowParallelEdges, int capacity, int edgeCapacity)
        {
            this.allowParallelEdges = allowParallelEdges;
            if (capacity > -1)
                vertexEdges = new VertexEdgeDictionary<TVertex, TEdge>(capacity);
            else
                vertexEdges = new VertexEdgeDictionary<TVertex, TEdge>();
            this.edgeCapacity = edgeCapacity;
        }

        public AdjacencyGraph(
            bool allowParallelEdges, 
            int capacity, 
            int edgeCapacity,
            Func<int, IVertexEdgeDictionary<TVertex, TEdge>> vertexEdgesDictionaryFactory)
        {
            Contract.Requires(vertexEdgesDictionaryFactory != null);
            this.allowParallelEdges = allowParallelEdges;
            vertexEdges = vertexEdgesDictionaryFactory(capacity);
            this.edgeCapacity = edgeCapacity;
        }

        public bool IsDirected => isDirected;

        public bool AllowParallelEdges
        {
            [Pure]
            get => allowParallelEdges;
        }

        public int EdgeCapacity
        {
            get => edgeCapacity;
            set => edgeCapacity = value;
        }

        public static Type EdgeType => typeof(TEdge);

        public bool IsVerticesEmpty => vertexEdges.Count == 0;

        public int VertexCount => vertexEdges.Count;

        public virtual IEnumerable<TVertex> Vertices => vertexEdges.Keys;

        [Pure]
        public bool ContainsVertex(TVertex v)
        {
            return TryGetEdges(v) != null;
        }

        public IEdgeList<TVertex, TEdge> TryGetEdges(TVertex v)
        {
            vertexEdges.TryGetValue(v, out var edges);
            return edges;
        }

        public bool IsOutEdgesEmpty(TVertex v)
        {
            return vertexEdges[v].Count == 0;
        }

        public int OutDegree(TVertex v)
        {
            return vertexEdges[v].Count;
        }

        public virtual IEnumerable<TEdge> OutEdges(TVertex v)
        {
            return vertexEdges[v];
        }

        public virtual bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            IEdgeList<TVertex, TEdge> list;
            if (vertexEdges.TryGetValue(v, out list))
            {
                edges = list;
                return true;
            }

            edges = null;
            return false;
        }

        public TEdge OutEdge(TVertex v, int index)
        {
            return vertexEdges[v][index];
        }

        /// <summary>
        /// Gets a value indicating whether this instance is edges empty.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is edges empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEdgesEmpty
        {
            [Pure]
            get => edgeCount == 0;
        }

        /// <summary>
        /// Gets the edge count.
        /// </summary>
        /// <value>The edge count.</value>
        public int EdgeCount => edgeCount;

        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(edgeCount >= 0);
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        public virtual IEnumerable<TEdge> Edges
        {
            [Pure]
            get
            {
                foreach (var edges in vertexEdges.Values)
                    foreach (var edge in edges)
                        yield return edge;
            }
        }

        [Pure]
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            IEnumerable<TEdge> outEdges;
            if (!TryGetOutEdges(source, out outEdges))
                return false;
            foreach (var outEdge in outEdges)
                if (outEdge.Target.Equals(target))
                    return true;
            return false;
        }

        [Pure]
        public bool ContainsEdge(TEdge edge)
        {
            IEdgeList<TVertex, TEdge> edges;
            return 
                vertexEdges.TryGetValue(edge.Source, out edges) &&
                edges.Contains(edge);
        }

        [Pure]
        public bool TryGetEdge(
            TVertex source,
            TVertex target,
            out TEdge edge)
        {
            IEdgeList<TVertex, TEdge> edgeList;
            if (vertexEdges.TryGetValue(source, out edgeList) &&
                edgeList.Count > 0)
            {
                foreach (var e in edgeList)
                {
                    if (e.Target.Equals(target))
                    {
                        edge = e;
                        return true;
                    }
                }
            }
            edge = default(TEdge);
            return false;
        }

        [Pure]
        public virtual bool TryGetEdges(
            TVertex source,
            TVertex target,
            out IEnumerable<TEdge> edges)
        {
            if (vertexEdges.TryGetValue(source, out var outEdges))
            {
                List<TEdge> list = new List<TEdge>(outEdges.Count);
                foreach (var edge in outEdges)
                    if (edge.Target.Equals(target))
                        list.Add(edge);

                edges = list;
                return true;
            }
            else
            {
                edges = null;
                return false;
            }
        }

        public virtual bool AddVertex(TVertex v)
        {
            if (ContainsVertex(v))
                return false;
            AddEdges(v);
            return true;
        }

        protected IEdgeList<TVertex, TEdge> GetEdges(TVertex v)
        {
            var edges = TryGetEdges(v);
            if (edges != null)
                return edges;
            return AddEdges(v);
        }

        protected EdgeList<TVertex, TEdge> AddEdges(TVertex v)
        {
            var edges = (EdgeCapacity > 0)
                ? new EdgeList<TVertex,TEdge>(EdgeCapacity)
                : new EdgeList<TVertex, TEdge>();
            vertexEdges.Add(v, edges);
            OnVertexAdded(v);
            return edges;
        }

        public virtual int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            int count = 0;
            foreach (var v in vertices)
                if (AddVertex(v))
                    count++;
            return count;
        }

        public event VertexAction<TVertex> VertexAdded;
        protected virtual void OnVertexAdded(TVertex args)
        {
            Contract.Requires(args != null);

            var eh = VertexAdded;
            if (eh != null)
                eh(args);
        }

        public virtual bool RemoveVertex(TVertex v)
        {
            if (!ContainsVertex(v))
                return false;
            // remove outedges
            {
                var edges = vertexEdges[v];
                if (EdgeRemoved != null) // lazily notify
                {
                    foreach (var edge in edges)
                        OnEdgeRemoved(edge);
                }
                edgeCount -= edges.Count;
                edges.Clear();
            }

            // iterage over edges and remove each edge touching the vertex
            foreach (var kv in vertexEdges)
            {
                if (kv.Key.Equals(v)) continue; // we've already 
                // collect edge to remove
                foreach(var edge in kv.Value.Clone())
                {
                    if (edge.Target.Equals(v))
                    {
                        kv.Value.Remove(edge);
                        OnEdgeRemoved(edge);
                        edgeCount--;
                    }
                }
            }

            Contract.Assert(edgeCount >= 0);
            vertexEdges.Remove(v);
            OnVertexRemoved(v);

            return true;
        }

        public event VertexAction<TVertex> VertexRemoved;
        protected virtual void OnVertexRemoved(TVertex args)
        {
            Contract.Requires(args != null);

            var eh = VertexRemoved;
            if (eh != null)
                eh(args);
        }

        public int RemoveVertexIf(VertexPredicate<TVertex> predicate)
        {
            var vertices = new VertexList<TVertex>();
            foreach (var v in Vertices)
                if (predicate(v))
                    vertices.Add(v);

            foreach (var v in vertices)
                RemoveVertex(v);

            return vertices.Count;
        }

        public virtual bool AddVerticesAndEdge(TEdge e)
        {
            AddVertex(e.Source);
            AddVertex(e.Target);
            return AddEdge(e);
        }

        /// <summary>
        /// Adds a range of edges to the graph
        /// </summary>
        /// <param name="edges"></param>
        /// <returns>the count edges that were added</returns>
        public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (AddVerticesAndEdge(edge))
                    count++;
            return count;
        }

        /// <summary>
        /// Adds the edge to the graph
        /// </summary>
        /// <param name="e">the edge to add</param>
        /// <returns>true if the edge was added; false if it was already part of the graph</returns>
        public virtual bool AddEdge(TEdge e)
        {
            Contract.Assert(e != null);
            if (e == null)
            {
                throw new ArgumentException("Edge can not be null.");
                //return false;
            }
            if (!AllowParallelEdges)
            {
                if (ContainsEdge(e.Source, e.Target))
                    return false;
            }

            var edges = GetEdges(e.Source);
            edges.Add(e);
            edgeCount++;

            OnEdgeAdded(e);

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

        public virtual bool RemoveEdge(TEdge e)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (vertexEdges.TryGetValue(e.Source, out edges) &&
                edges.Remove(e))
            {
                edgeCount--;
                Contract.Assert(edgeCount >= 0);
                OnEdgeRemoved(e);
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
            var edges = new EdgeList<TVertex, TEdge>();
            foreach (var edge in Edges)
                if (predicate(edge))
                    edges.Add(edge);

            foreach (var edge in edges)
            {
                OnEdgeRemoved(edge);
                vertexEdges[edge.Source].Remove(edge);
            }

            edgeCount -= edges.Count;

            return edges.Count;
        }

        public void ClearOutEdges(TVertex v)
        {
            var edges = vertexEdges[v];
            int count = edges.Count;
            if (EdgeRemoved != null) // call only if someone is listening
            {
                foreach (var edge in edges)
                    OnEdgeRemoved(edge);
            }
            edges.Clear();
            edgeCount -= count;
        }

        public int RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
        {
            var edges = vertexEdges[v];
            var edgeToRemove = new EdgeList<TVertex,TEdge>(edges.Count);
            foreach (var edge in edges)
                if (predicate(edge))
                    edgeToRemove.Add(edge);

            foreach (var edge in edgeToRemove)
            {
                edges.Remove(edge);
                OnEdgeRemoved(edge);
            }
            edgeCount -= edgeToRemove.Count;

            return edgeToRemove.Count;
        }

        public void TrimEdgeExcess()
        {
            foreach (var edges in vertexEdges.Values)
                edges.TrimExcess();
        }

        public void Clear()
        {
            vertexEdges.Clear();
            edgeCount = 0;
        }

        public static AdjacencyGraph<TVertex, TEdge> LoadDot(string dotSource,
            Func<string, IDictionary<string, string>, TVertex> vertexFunc,
            Func<TVertex, TVertex, IDictionary<string, string>, TEdge> edgeFunc)
        {
            Func<bool, IMutableVertexAndEdgeSet<TVertex, TEdge>> createGraph = (allowParallelEdges) =>
                new AdjacencyGraph<TVertex, TEdge>(allowParallelEdges);

            return (AdjacencyGraph<TVertex, TEdge>)
            DotParserAdapter.LoadDot(dotSource, createGraph, vertexFunc, edgeFunc);
        }

        #region ICloneable Members
        private AdjacencyGraph(
            IVertexEdgeDictionary<TVertex, TEdge> vertexEdges,
            int edgeCount,
            int edgeCapacity,
            bool allowParallelEdges
            )
        {
            Contract.Requires(vertexEdges != null);
            Contract.Requires(edgeCount >= 0);

            this.vertexEdges = vertexEdges;
            this.edgeCount = edgeCount;
            this.edgeCapacity = edgeCapacity;
            this.allowParallelEdges = allowParallelEdges;
        }

        [Pure]
        public AdjacencyGraph<TVertex, TEdge> Clone()
        {
            return new AdjacencyGraph<TVertex, TEdge>(
                vertexEdges.Clone(),
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
