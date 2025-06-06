﻿using System;
using System.Collections.Generic;

using QuickGraph.Algorithms.Search;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.TopologicalSort
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class TopologicalSortAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IVertexListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private IList<TVertex> vertices = new List<TVertex>();
        private bool allowCyclicGraph = false;

        public TopologicalSortAlgorithm(IVertexListGraph<TVertex, TEdge> g)
            : this(g, new List<TVertex>())
        { }

        public TopologicalSortAlgorithm(
            IVertexListGraph<TVertex, TEdge> g,
            IList<TVertex> vertices)
            : base(g)
        {
            Contract.Requires(vertices != null);

            this.vertices = vertices;
        }


        public IList<TVertex> SortedVertices => vertices;

        public bool AllowCyclicGraph => allowCyclicGraph;

        private void BackEdge(TEdge args)
        {
            if (!AllowCyclicGraph)
                throw new NonAcyclicGraphException();
        }

        private void VertexFinished(TVertex v)
        {
            vertices.Insert(0, v);
        }

        public event VertexAction<TVertex> DiscoverVertex;
        public event VertexAction<TVertex> FinishVertex;

        protected override void InternalCompute()
        {
            DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
            try
            {
                dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(
                    this,
                    VisitedGraph,
                    new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount)
                    );
                dfs.BackEdge += BackEdge;
                dfs.FinishVertex += VertexFinished;
                dfs.DiscoverVertex += DiscoverVertex;
                dfs.FinishVertex += FinishVertex;

                dfs.Compute();
            }
            finally
            {
                if (dfs != null)
                {
                    dfs.BackEdge -= BackEdge;
                    dfs.FinishVertex -= VertexFinished;
                    dfs.DiscoverVertex -= DiscoverVertex;
                    dfs.FinishVertex -= FinishVertex;
                }
            }
        }

        public void Compute(IList<TVertex> vertices)
        {
            this.vertices = vertices;
            this.vertices.Clear();
            Compute();
        }
    }
}