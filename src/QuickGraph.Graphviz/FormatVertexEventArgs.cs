﻿using System;
using QuickGraph.Graphviz.Dot;

namespace QuickGraph.Graphviz
{
    public sealed class FormatVertexEventArgs<TVertex> 
        : VertexEventArgs<TVertex>
    {
        private readonly GraphvizVertex vertexFormatter;

        internal FormatVertexEventArgs(TVertex v, GraphvizVertex vertexFormatter)
			: base(v)
        {
#if CONTRACTS_BUG
            Contract.Requires(vertexFormatter != null);
#endif
            this.vertexFormatter = vertexFormatter;
        }

        public GraphvizVertex VertexFormatter => vertexFormatter;
    }

    public delegate void FormatVertexEventHandler<TVertex>(
        object sender,
        FormatVertexEventArgs<TVertex> e);
}
