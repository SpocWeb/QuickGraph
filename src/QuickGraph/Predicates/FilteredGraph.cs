﻿using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class FilteredGraph<TVertex, TEdge, TGraph> : IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IGraph<TVertex,TEdge>
    {
        private readonly TGraph baseGraph;
        private readonly EdgePredicate<TVertex,TEdge> edgePredicate;
        private readonly VertexPredicate<TVertex> vertexPredicate;

        public FilteredGraph(
            TGraph baseGraph,
            VertexPredicate<TVertex> vertexPredicate,
            EdgePredicate<TVertex, TEdge> edgePredicate
            )
        {
            Contract.Requires(baseGraph != null);
            Contract.Requires(vertexPredicate != null);
            Contract.Requires(edgePredicate != null);

            this.baseGraph = baseGraph;
            this.vertexPredicate = vertexPredicate;
            this.edgePredicate = edgePredicate;
        }

        /// <summary>
        /// Underlying filtered graph
        /// </summary>
        public TGraph BaseGraph => baseGraph;

        /// <summary>
        /// Edge predicate used to filter the edges
        /// </summary>
        public EdgePredicate<TVertex, TEdge> EdgePredicate => edgePredicate;

        public VertexPredicate<TVertex> VertexPredicate => vertexPredicate;

        protected bool TestEdge(TEdge edge)
        {
            return VertexPredicate(edge.Source)
                    && VertexPredicate(edge.Target)
                    && EdgePredicate(edge);
        }

        public bool IsDirected => BaseGraph.IsDirected;

        public bool AllowParallelEdges => baseGraph.AllowParallelEdges;
    }
}
