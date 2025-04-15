﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace QuickGraph
{
    /// <summary>
    /// Directed graph representation using a Compressed Sparse Row representation
    /// (http://www.cs.utk.edu/~dongarra/etemplates/node373.html)
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public sealed class CompressedSparseRowGraph<TVertex>
        : IVertexSet<TVertex>
        , IEdgeSet<TVertex, SEquatableEdge<TVertex>>
        , IVertexListGraph<TVertex, SEquatableEdge<TVertex>>
#if !SILVERLIGHT
        , ICloneable
#endif
    {
#if !SILVERLIGHT
        [Serializable]
#endif
        struct Range
        {
            public readonly int Start;
            public readonly int End;
            public Range(int start, int end)
            {
                Contract.Requires(start >= 0);
                Contract.Requires(start <= end);
                Contract.Ensures(Contract.ValueAtReturn(out this).Start == start);
                Contract.Ensures(Contract.ValueAtReturn(out this).End == end);

                Start = start;
                End = end;
            }

            public int Length
            {
                get {
                    Contract.Ensures(Contract.Result<int>() >= 0);

                    return End - Start;
                }
            }
        }

        readonly Dictionary<TVertex, Range> outEdgeStartRanges;
        readonly TVertex[] outEdges;

        private CompressedSparseRowGraph(
            Dictionary<TVertex, Range> outEdgeStartRanges,
            TVertex[] outEdges
            )
        {
            Contract.Requires(outEdgeStartRanges != null);
            Contract.Requires(outEdges != null);

            this.outEdgeStartRanges = outEdgeStartRanges;
            this.outEdges = outEdges;
        }

        public static CompressedSparseRowGraph<TVertex> FromGraph<TEdge>(
            IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph
            )
            where TEdge : IEdge<TVertex>
        {
            Contract.Requires(visitedGraph != null);
            Contract.Ensures(Contract.Result<CompressedSparseRowGraph<TVertex>>() != null);

            var outEdgeStartRanges = new Dictionary<TVertex, Range>(visitedGraph.VertexCount);
            var outEdges = new TVertex[visitedGraph.EdgeCount];

            int start = 0;
            int end = 0;
            int index = 0;
            foreach (var vertex in visitedGraph.Vertices)
            {
                end = start + visitedGraph.OutDegree(vertex);
                var range = new Range(start, end);
                outEdgeStartRanges.Add(vertex, range);
                foreach (var edge in visitedGraph.OutEdges(vertex))
                    outEdges[index++] = edge.Target;
                Contract.Assert(index == end);
            }
            Contract.Assert(index == outEdges.Length);

            return new CompressedSparseRowGraph<TVertex>(
                outEdgeStartRanges,
                outEdges);
        }

        public bool IsVerticesEmpty
        {
            get { return outEdgeStartRanges.Count > 0; }
        }

        public int VertexCount
        {
            get { return outEdgeStartRanges.Count; }
        }

        public IEnumerable<TVertex> Vertices
        {
            get { return outEdgeStartRanges.Keys; }
        }

        public bool ContainsVertex(TVertex vertex)
        {
            return outEdgeStartRanges.ContainsKey(vertex);
        }

        public int EdgeCount
        {
            get { return outEdges.Length; }
        }

        public bool IsEdgesEmpty
        {
            get { return outEdges.Length > 0; }
        }

        public IEnumerable<SEquatableEdge<TVertex>> Edges
        {
            get 
            {
                foreach (var kv in outEdgeStartRanges)
                {
                    var source = kv.Key;
                    var range = kv.Value;
                    for (int i = range.Start; i < range.End; ++i)
                    {
                        var target = outEdges[i];
                        yield return new SEquatableEdge<TVertex>(source, target);
                    }
                }
            }
        }

        public bool ContainsEdge(SEquatableEdge<TVertex> edge)
        {
            return ContainsEdge(edge.Source, edge.Target);
        }

        public bool ContainsEdge(TVertex source, TVertex target)
        {
            Range range;
            if (outEdgeStartRanges.TryGetValue(source, out range))
            {
                for (int i = range.Start; i < range.End; ++i)
                    if (outEdges[i].Equals(target))
                        return true;
            }

            return false;
        }

        public bool TryGetEdges(
            TVertex source, 
            TVertex target, 
            out IEnumerable<SEquatableEdge<TVertex>> edges)
        {
            if (ContainsEdge(source, target))
            {
                edges = new SEquatableEdge<TVertex>[] { new SEquatableEdge<TVertex>(source, target) };
                return true;
            }

            edges = null;
            return false;
        }

        public bool TryGetEdge(
            TVertex source, 
            TVertex target, 
            out SEquatableEdge<TVertex> edge)
        {
            if (ContainsEdge(source, target))
            {
                edge = new SEquatableEdge<TVertex>(source, target);
                return true;
            }

            edge = default(SEquatableEdge<TVertex>);
            return false;
        }

        public bool IsOutEdgesEmpty(TVertex v)
        {
            return outEdgeStartRanges[v].Length == 0;
        }

        public int OutDegree(TVertex v)
        {
            return outEdgeStartRanges[v].Length;
        }

        public IEnumerable<SEquatableEdge<TVertex>> OutEdges(TVertex v)
        {
            var range = outEdgeStartRanges[v];
            for(int i = range.Start;i<range.End;++i)
                yield return new SEquatableEdge<TVertex>(v, outEdges[i]);
        }

        public bool TryGetOutEdges(TVertex v, out IEnumerable<SEquatableEdge<TVertex>> edges)
        {
            var range = outEdgeStartRanges[v];
            if (range.Length > 0)
            {
                edges = OutEdges(v);
                return false;
            }

            edges = null;
            return false;
        }

        public SEquatableEdge<TVertex> OutEdge(TVertex v, int index)
        {
            var range = outEdgeStartRanges[v];
            var targetIndex = range.Start + index;
            Contract.Assert(targetIndex < range.End);
            return new SEquatableEdge<TVertex>(v, outEdges[targetIndex]);
        }

        public bool IsDirected
        {
            get { return true; }
        }

        public bool AllowParallelEdges
        {
            get { return false; }
        }

        public CompressedSparseRowGraph<TVertex> Clone()
        {
            var ranges = new Dictionary<TVertex, Range>(outEdgeStartRanges);
            var edges = (TVertex[])outEdges.Clone();
            return new CompressedSparseRowGraph<TVertex>(ranges, edges);
        }

#if !SILVERLIGHT
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif
    }
}
