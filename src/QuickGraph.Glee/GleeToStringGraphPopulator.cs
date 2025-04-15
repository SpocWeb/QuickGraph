using System;

namespace QuickGraph.Glee
{
    public sealed class GleeToStringGraphPopulator<TVertex,TEdge> : GleeDefaultGraphPopulator<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly IFormatProvider formatProvider;
        private readonly string format;

        public GleeToStringGraphPopulator(
            IEdgeListGraph<TVertex, TEdge> visitedGraph,
            IFormatProvider formatProvider,
            string format
            )
            :base(visitedGraph)
        {
            this.formatProvider = formatProvider;
            if (string.IsNullOrEmpty(format))
                this.format = "{0}";
            else
                this.format = format;
        }

        public IFormatProvider FormatProvider => formatProvider;

        public string Format => format;

        protected override string GetVertexId(TVertex v)
        {
            return string.Format(formatProvider, format, v);
        }
    }
}
