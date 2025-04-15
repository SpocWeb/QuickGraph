using Microsoft.Glee.Drawing;
using System.Diagnostics.Contracts;

namespace QuickGraph.Glee
{
    public sealed class GleeIndentifiableGraphPopulator<TVertex,TEdge>
        : GleeGraphPopulator<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly VertexIdentity<TVertex> vertexIdentities;
        public GleeIndentifiableGraphPopulator(IEdgeListGraph<TVertex, TEdge> visitedGraph, VertexIdentity<TVertex> vertexIdentities)
            : base(visitedGraph)
        {
            Contract.Requires(vertexIdentities != null);

            this.vertexIdentities = vertexIdentities;
        }

        protected override Node AddNode(TVertex v)
        {
            return (Node)GleeGraph.AddNode(vertexIdentities(v));
        }

        protected override Edge AddEdge(TEdge e)
        {
            return (Edge)GleeGraph.AddEdge(
                vertexIdentities(e.Source),
                vertexIdentities(e.Target));
        }
    }
}
