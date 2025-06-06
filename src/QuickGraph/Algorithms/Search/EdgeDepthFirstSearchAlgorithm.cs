﻿using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Search
{
    /// <summary>
    /// A edge depth first search algorithm for directed graphs
    /// </summary>
    /// <remarks>
    /// This is a variant of the classic DFS algorithm where the
    /// edges are color marked instead of the vertices.
    /// </remarks>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2"
    ///     />
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class EdgeDepthFirstSearchAlgorithm<TVertex, TEdge> :
        RootedAlgorithmBase<TVertex,IEdgeListAndIncidenceGraph<TVertex, TEdge>>,
        IEdgeColorizerAlgorithm<TVertex,TEdge>,
        IEdgePredecessorRecorderAlgorithm<TVertex,TEdge>,
        ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private IDictionary<TEdge,GraphColor> colors;
        private int maxDepth = int.MaxValue;

        public EdgeDepthFirstSearchAlgorithm(IEdgeListAndIncidenceGraph<TVertex, TEdge> g)
            :this(g, new Dictionary<TEdge, GraphColor>())
        {
        }

        public EdgeDepthFirstSearchAlgorithm(
            IEdgeListAndIncidenceGraph<TVertex, TEdge> visitedGraph,
            IDictionary<TEdge, GraphColor> colors
            )
            :this(null, visitedGraph, colors)
        {}

        public EdgeDepthFirstSearchAlgorithm(
            IAlgorithmComponent host,
            IEdgeListAndIncidenceGraph<TVertex, TEdge> visitedGraph,
            IDictionary<TEdge, GraphColor> colors
            )
            :base(host, visitedGraph)
        {
            Contract.Requires(colors != null);

            this.colors = colors;
        }

        public IDictionary<TEdge, GraphColor> EdgeColors => colors;

        public int MaxDepth
        {
            get => maxDepth;
            set => maxDepth = value;
        }

        public event EdgeAction<TVertex,TEdge> InitializeEdge;
        private void OnInitializeEdge(TEdge e)
        {
            var eh = InitializeEdge;
            if (eh != null)
                eh(e);
        }

        public event VertexAction<TVertex> StartVertex;
        private void OnStartVertex(TVertex v)
        {
            var eh = StartVertex;
            if (eh != null)
                eh(v);
        }

        public event EdgeAction<TVertex, TEdge> StartEdge;
        private void OnStartEdge(TEdge e)
        {
            var eh = StartEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeEdgeAction<TVertex, TEdge> DiscoverTreeEdge;
        private void OnDiscoverTreeEdge(TEdge e, TEdge targetEge)
        {
            var eh = DiscoverTreeEdge;
            if (eh != null)
                eh(e, targetEge);
        }

        public event EdgeAction<TVertex, TEdge> ExamineEdge;
        private void OnExamineEdge(TEdge e)
        {
            var eh = ExamineEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex, TEdge> TreeEdge;
        private void OnTreeEdge(TEdge e)
        {
            var eh = TreeEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex, TEdge> BackEdge;
        private void OnBackEdge(TEdge e)
        {
            var eh = BackEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex, TEdge> ForwardOrCrossEdge;
        private void OnForwardOrCrossEdge(TEdge e)
        {
            var eh = ForwardOrCrossEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex,TEdge> FinishEdge;
        private void OnFinishEdge(TEdge e)
        {
            var eh = FinishEdge;
            if (eh != null)
                eh(e);
        }
        
        protected override void  InternalCompute()
        {
            Initialize();
            var cancelManager = Services.CancelManager;
            if (cancelManager.IsCancelling)
                return;

            // start whith him:
            TVertex rootVertex;
            if (TryGetRootVertex(out rootVertex))
            {
                OnStartVertex(rootVertex);

                // process each out edge of v
                foreach (var e in VisitedGraph.OutEdges(rootVertex))
                {
                    if (cancelManager.IsCancelling)
                        return;
                    if (EdgeColors[e] == GraphColor.White)
                    {
                        OnStartEdge(e);
                        Visit(e, 0);
                    }
                }
            }

            // process the rest of the graph edges
            foreach (var e in VisitedGraph.Edges)
            {
                if (cancelManager.IsCancelling)
                    return;
                if (EdgeColors[e] == GraphColor.White)
                {
                    OnStartEdge(e);
                    Visit(e, 0);
                }
            }
        }

        protected override void Initialize()
        {
            // put all vertex to white
            var cancelManager = Services.CancelManager;
            foreach (var e in VisitedGraph.Edges)
            {
                if (cancelManager.IsCancelling)
                    return;
                EdgeColors[e] = GraphColor.White;
                OnInitializeEdge(e);
            }
        }

        public void Visit(TEdge se, int depth)
        {
            if (depth > maxDepth)
                return;
            var cancelManager = Services.CancelManager;

            // mark edge as gray
            EdgeColors[se] = GraphColor.Gray;
            // add edge to the search tree
            OnTreeEdge(se);

            // iterate over out-edges
            foreach (var e in VisitedGraph.OutEdges(se.Target))
            {
                if (cancelManager.IsCancelling) return;

                // check edge is not explored yet,
                // if not, explore it.
                if (EdgeColors[e] == GraphColor.White)
                {
                    OnDiscoverTreeEdge(se, e);
                    Visit(e, depth + 1);
                }
                else if (EdgeColors[e] == GraphColor.Gray)
                {
                    // edge is being explored
                    OnBackEdge(e);
                }
                else
                    // edge is black
                    OnForwardOrCrossEdge(e);
            }

            // all out-edges have been explored
            EdgeColors[se] = GraphColor.Black;
            OnFinishEdge(se);
        }
    }
}
