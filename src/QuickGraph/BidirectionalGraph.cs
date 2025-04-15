using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using QuickGraph.Contracts;
using QuickGraph.Collections;

namespace QuickGraph
{
    /// <summary>
    /// A mutable directed graph data structure efficient for sparse
    /// graph representation where out-edge and in-edges need to be enumerated. Requires
    /// twice as much memory as the adjacency graph.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class BidirectionalGraph<TVertex, TEdge> 
        : IVertexAndEdgeListGraph<TVertex, TEdge>
        , IEdgeListAndIncidenceGraph<TVertex, TEdge>
        , IMutableEdgeListGraph<TVertex, TEdge>
        , IMutableIncidenceGraph<TVertex, TEdge>
        , IMutableVertexListGraph<TVertex, TEdge>
        , IBidirectionalGraph<TVertex,TEdge>
        , IMutableBidirectionalGraph<TVertex,TEdge>
        , IMutableVertexAndEdgeListGraph<TVertex, TEdge>
#if !SILVERLIGHT
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
        private readonly bool isDirected = true;
        private readonly bool allowParallelEdges;
        private readonly IVertexEdgeDictionary<TVertex, TEdge> vertexOutEdges;
        private readonly IVertexEdgeDictionary<TVertex, TEdge> vertexInEdges;
        private int edgeCount = 0;
        private int edgeCapacity = -1;

        public BidirectionalGraph()
            :this(true)
        {}

        public BidirectionalGraph(bool allowParallelEdges)
            :this(allowParallelEdges,-1)
        {}

        public BidirectionalGraph(bool allowParallelEdges, int vertexCapacity)
        {
            this.allowParallelEdges = allowParallelEdges;
            if (vertexCapacity > -1)
            {
                vertexInEdges = new VertexEdgeDictionary<TVertex, TEdge>(vertexCapacity);
                vertexOutEdges = new VertexEdgeDictionary<TVertex, TEdge>(vertexCapacity);
            }
            else
            {
                vertexInEdges = new VertexEdgeDictionary<TVertex, TEdge>();
                vertexOutEdges = new VertexEdgeDictionary<TVertex, TEdge>();
            }
        }

        public BidirectionalGraph(
            bool allowParallelEdges, 
            int capacity, 
            Func<int, IVertexEdgeDictionary<TVertex, TEdge>> vertexEdgesDictionaryFactory)
        {
            Contract.Requires(vertexEdgesDictionaryFactory != null);
            this.allowParallelEdges = allowParallelEdges;
            vertexInEdges = vertexEdgesDictionaryFactory(capacity);
            vertexOutEdges = vertexEdgesDictionaryFactory(capacity);
        }
 
        public static Type EdgeType
        {
            [Pure]
            get { return typeof(TEdge); }
        }

        public int EdgeCapacity
        {
            [Pure]
            get { return edgeCapacity; }
            set { edgeCapacity = value; }
        }

        public bool IsDirected
        {
            [Pure]
            get { return isDirected; }
        }

        public bool AllowParallelEdges
        {
            [Pure]
            get { return allowParallelEdges; }
        }

        public bool IsVerticesEmpty
        {
            [Pure]
            get { return vertexOutEdges.Count == 0; }
        }

        public int VertexCount
        {
            [Pure]
            get { return vertexOutEdges.Count; }
        }

        public virtual IEnumerable<TVertex> Vertices
        {
            [Pure]
            get { return vertexOutEdges.Keys; }
        }

        [Pure]
        public bool ContainsVertex(TVertex v)
        {
            return vertexOutEdges.ContainsKey(v);
        }

        [Pure]
        public bool IsOutEdgesEmpty(TVertex v)
        {
            return vertexOutEdges[v].Count == 0;
        }

        [Pure]
        public int OutDegree(TVertex v)
        {
            return vertexOutEdges[v].Count;
        }

        [Pure]
        public IEnumerable<TEdge> OutEdges(TVertex v)
        {
            return vertexOutEdges[v];
        }

        [Pure]
        public bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            IEdgeList<TVertex, TEdge> list;
            if (vertexInEdges.TryGetValue(v, out list))
            {
                edges = list;
                return true;
            }

            edges = null;
            return false;
        }

        [Pure]
        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            IEdgeList<TVertex, TEdge> list;
            if (vertexOutEdges.TryGetValue(v, out list))
            {
                edges = list;
                return true;
            }

            edges = null;
            return false;
        }

        [Pure]
        public TEdge OutEdge(TVertex v, int index)
        {
            return vertexOutEdges[v][index];
        }

        [Pure]
        public bool IsInEdgesEmpty(TVertex v)
        {
            return vertexInEdges[v].Count == 0;
        }

        [Pure]
        public int InDegree(TVertex v)
        {
            return vertexInEdges[v].Count;
        }

        [Pure]
        public IEnumerable<TEdge> InEdges(TVertex v)
        {
            return vertexInEdges[v];
        }

        [Pure]
        public TEdge InEdge(TVertex v, int index)
        {
            return vertexInEdges[v][index];
        }

        [Pure]
        public int Degree(TVertex v)
        {
            return OutDegree(v) + InDegree(v);
        }

        public bool IsEdgesEmpty
        {
            get { return edgeCount == 0; }
        }

        public int EdgeCount
        {
            get 
            {
                return edgeCount; 
            }
        }

        public virtual IEnumerable<TEdge> Edges
        {
            get
            {
                foreach (var edges in vertexOutEdges.Values)
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
        public bool TryGetEdge(
            TVertex source,
            TVertex target,
            out TEdge edge)
        {
            IEdgeList<TVertex, TEdge> edgeList;
            if (vertexOutEdges.TryGetValue(source, out edgeList) &&
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
        public bool TryGetEdges(
            TVertex source,
            TVertex target,
            out IEnumerable<TEdge> edges)
        {
            IEdgeList<TVertex, TEdge> edgeList;
            if (vertexOutEdges.TryGetValue(source, out edgeList))
            {
                List<TEdge> list = new List<TEdge>(edgeList.Count);
                foreach (var edge in edgeList)
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

        [Pure]
        public bool ContainsEdge(TEdge edge)
        {
            IEdgeList<TVertex, TEdge> outEdges;
            return vertexOutEdges.TryGetValue(edge.Source, out outEdges) &&
                outEdges.Contains(edge);
        }

        public virtual bool AddVertex(TVertex v)
        {
            if (ContainsVertex(v))
                return false;

            if (EdgeCapacity > 0)
            {
                vertexOutEdges.Add(v, new EdgeList<TVertex, TEdge>(EdgeCapacity));
                vertexInEdges.Add(v, new EdgeList<TVertex, TEdge>(EdgeCapacity));
            }
            else
            {
                vertexOutEdges.Add(v, new EdgeList<TVertex, TEdge>());
                vertexInEdges.Add(v, new EdgeList<TVertex, TEdge>());
            }
            OnVertexAdded(v);
            return true;
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
            var eh = VertexAdded;
            if (eh != null)
                eh(args);
        }

        public virtual bool RemoveVertex(TVertex v)
        {
            if (!ContainsVertex(v))
                return false;

            // collect edges to remove
            var edgesToRemove = new EdgeList<TVertex, TEdge>();
            foreach (var outEdge in OutEdges(v))
            {
                vertexInEdges[outEdge.Target].Remove(outEdge);
                edgesToRemove.Add(outEdge);
            }
            foreach (var inEdge in InEdges(v))
            {
                // might already have been removed
                if(vertexOutEdges[inEdge.Source].Remove(inEdge))
                    edgesToRemove.Add(inEdge);
            }

            // notify users
            if (EdgeRemoved != null)
            {
                foreach(TEdge edge in edgesToRemove)
                    OnEdgeRemoved(edge);
            }

            vertexOutEdges.Remove(v);
            vertexInEdges.Remove(v);
            edgeCount -= edgesToRemove.Count;
            OnVertexRemoved(v);

            return true;
        }

        public event VertexAction<TVertex> VertexRemoved;
        protected virtual void OnVertexRemoved(TVertex args)
        {
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

        public virtual bool AddEdge(TEdge e)
        {
            if (!AllowParallelEdges)
            {
                if (ContainsEdge(e.Source, e.Target))
                    return false;
            }
            vertexOutEdges[e.Source].Add(e);
            vertexInEdges[e.Target].Add(e);
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

        public virtual bool AddVerticesAndEdge(TEdge e)
        {
            AddVertex(e.Source);
            AddVertex(e.Target);
            return AddEdge(e);
        }

        public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (AddVerticesAndEdge(edge))
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
            if (vertexOutEdges[e.Source].Remove(e))
            {
                vertexInEdges[e.Target].Remove(e);
                edgeCount--;
                Contract.Assert(edgeCount >= 0);

                OnEdgeRemoved(e);
                return true;
            }
            else
            {
                return false;
            }
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
                RemoveEdge(edge);
            return edges.Count;
        }

        public int RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
        {
            var edges = new EdgeList<TVertex, TEdge>();
            foreach (var edge in OutEdges(v))
                if (predicate(edge))
                    edges.Add(edge);
            foreach (var edge in edges)
                RemoveEdge(edge);
            return edges.Count;
        }

        public int RemoveInEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
        {
            var edges = new EdgeList<TVertex, TEdge>();
            foreach (var edge in InEdges(v))
                if (predicate(edge))
                    edges.Add(edge);
            foreach (var edge in edges)
                RemoveEdge(edge);
            return edges.Count;
        }

        public void ClearOutEdges(TVertex v)
        {
            var outEdges = vertexOutEdges[v];
            foreach (var edge in outEdges)
            {
                vertexInEdges[edge.Target].Remove(edge);
                OnEdgeRemoved(edge);
            }

            edgeCount -= outEdges.Count;
            outEdges.Clear();
        }

#if DEEP_INVARIANT
        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(this.edgeCount >= 0);
            Contract.Invariant(Enumerable.Sum(this.vertexInEdges.Values, ie => ie.Count) == this.edgeCount);
            Contract.Invariant(this.vertexInEdges.Count == this.vertexOutEdges.Count);
            Contract.Invariant(Enumerable.All(this.vertexInEdges, kv => this.vertexOutEdges.ContainsKey(kv.Key)));
            Contract.Invariant(Enumerable.All(this.vertexOutEdges, kv => this.vertexInEdges.ContainsKey(kv.Key)));
        }
#endif

        public void ClearInEdges(TVertex v)
        {
            var inEdges = vertexInEdges[v];
            foreach (var edge in inEdges)
            {
                vertexOutEdges[edge.Source].Remove(edge);
                OnEdgeRemoved(edge);
            }

            edgeCount -= inEdges.Count;
            inEdges.Clear();
        }

        public void ClearEdges(TVertex v)
        {
            ClearOutEdges(v);
            ClearInEdges(v);
        }

        public void TrimEdgeExcess()
        {
            foreach (var edges in vertexInEdges.Values)
                edges.TrimExcess();
            foreach (var edges in vertexOutEdges.Values)
                edges.TrimExcess();
        }

        public void Clear()
        {
            vertexOutEdges.Clear();
            vertexInEdges.Clear();
            edgeCount = 0;
        }

        public void MergeVertex(TVertex v, EdgeFactory<TVertex, TEdge> edgeFactory)
        {
            Contract.Requires(GraphContract.InVertexSet(this, v));
            Contract.Requires(edgeFactory != null);

            // storing edges in local array
            var inedges = vertexInEdges[v];
            var outedges = vertexOutEdges[v];

            // remove vertex
            RemoveVertex(v);

            // add edges from each source to each target
            foreach (var source in inedges)
            {
                //is it a self edge
                if (source.Source.Equals(v))
                    continue;
                foreach (var target in outedges)
                {
                    if (v.Equals(target.Target))
                        continue;
                    // we add an new edge
                    AddEdge(edgeFactory(source.Source, target.Target));
                }
            }
        }

        public void MergeVertexIf(VertexPredicate<TVertex> vertexPredicate, EdgeFactory<TVertex, TEdge> edgeFactory)
        {
            Contract.Requires(vertexPredicate != null);
            Contract.Requires(edgeFactory != null);

            // storing vertices to merge
            var mergeVertices = new VertexList<TVertex>(VertexCount / 4);
            foreach (var v in Vertices)
                if (vertexPredicate(v))
                    mergeVertices.Add(v);

            // applying merge recursively
            foreach (var v in mergeVertices)
                MergeVertex(v, edgeFactory);
        }

        public static BidirectionalGraph<TVertex, TEdge> LoadDot(string dotSource,
            Func<string, IDictionary<string, string>, TVertex> vertexFunc,
            Func<TVertex, TVertex, IDictionary<string, string>, TEdge> edgeFunc)
        {
            Func<bool, IMutableVertexAndEdgeSet<TVertex, TEdge>> createGraph = (allowParallelEdges) =>
                new BidirectionalGraph<TVertex, TEdge>(allowParallelEdges);

            return (BidirectionalGraph<TVertex, TEdge>)
                DotParserAdapter.LoadDot(dotSource, createGraph, vertexFunc, edgeFunc);
        }

        #region ICloneable Members

        /// <summary>
        /// Copy constructor that creates sufficiently deep copy of the graph.
        /// </summary>
        /// <param name="other"></param>
        public BidirectionalGraph(BidirectionalGraph<TVertex, TEdge> other)
        {
            Contract.Requires(other != null);

            vertexInEdges = other.vertexInEdges.Clone();
            vertexOutEdges = other.vertexOutEdges.Clone();
            edgeCount = other.edgeCount;
            edgeCapacity = other.edgeCapacity;
            allowParallelEdges = other.allowParallelEdges;
        }


        private BidirectionalGraph(
            IVertexEdgeDictionary<TVertex, TEdge> vertexInEdges,
            IVertexEdgeDictionary<TVertex, TEdge> vertexOutEdges,
            int edgeCount,
            int edgeCapacity,
            bool allowParallelEdges
            )
        {
            Contract.Requires(vertexInEdges != null);
            Contract.Requires(vertexOutEdges != null);
            Contract.Requires(edgeCount >= 0);

            this.vertexInEdges = vertexInEdges;
            this.vertexOutEdges = vertexOutEdges;
            this.edgeCount = edgeCount;
            this.edgeCapacity = edgeCapacity;
            this.allowParallelEdges = allowParallelEdges;
        }

        public BidirectionalGraph<TVertex, TEdge> Clone()
        {
            return new BidirectionalGraph<TVertex, TEdge>(this);
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
