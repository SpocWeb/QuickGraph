﻿using QuickGraph.Graphviz.Dot;

namespace QuickGraph.Graphviz
{
    public sealed class FormatEdgeEventArgs<TVertex,TEdge> 
        : EdgeEventArgs<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly GraphvizEdge edgeFormatter;

        internal FormatEdgeEventArgs(TEdge e, GraphvizEdge edgeFormatter)
			: base(e)
        {
#if CONTRACTS_BUG
            Contract.Requires(edgeFormatter != null);
#endif
            this.edgeFormatter = edgeFormatter;
        }

        /// <summary>
        /// Edge formatter
        /// </summary>
        public GraphvizEdge EdgeFormatter => edgeFormatter;
    }

    public delegate void FormatEdgeAction<TVertex, TEdge>(
        object sender, 
        FormatEdgeEventArgs<TVertex,TEdge> e)
        where TEdge : IEdge<TVertex>;
}
