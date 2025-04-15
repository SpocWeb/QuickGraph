using System;
using QuickGraph.Graphviz.Dot;


namespace QuickGraph.Graphviz
{
    /// <summary>
    /// A clustered graph event argument.
    /// </summary>
    public class FormatClusterEventArgs<TVertex, TEdge> : EventArgs where TEdge : IEdge<TVertex>
    {
        private readonly IVertexAndEdgeListGraph<TVertex,TEdge> cluster;
        private readonly GraphvizGraph graphFormat;

        public FormatClusterEventArgs(IVertexAndEdgeListGraph<TVertex,TEdge> cluster, GraphvizGraph graphFormat)
        {
            if (cluster == null)
                throw new ArgumentNullException("cluster");
            this.cluster = cluster;
            this.graphFormat = graphFormat;
        }

        public IVertexAndEdgeListGraph<TVertex,TEdge> Cluster => cluster;

        public GraphvizGraph GraphFormat => graphFormat;
    }

    public delegate void FormatClusterEventHandler<TVertex, TEdge>(
        object sender,
        FormatClusterEventArgs<TVertex,TEdge> e)
        where TEdge: IEdge<TVertex>;

}
