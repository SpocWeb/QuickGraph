using System;
using System.Collections.Generic;
using Microsoft.Glee.Drawing;

namespace QuickGraph.Glee
{
    public class GleeDefaultGraphPopulator<TVertex, TEdge>
        : GleeGraphPopulator<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        public GleeDefaultGraphPopulator(IEdgeListGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        { }

        private Dictionary<TVertex, string> vertexIds;
        protected override void OnStarted(EventArgs e)
        {
            base.OnStarted(e);
            vertexIds = new Dictionary<TVertex, string>(VisitedGraph.VertexCount);
        }

        protected override void OnFinished(EventArgs e)
        {
            vertexIds = null;
            base.OnFinished(e);
        }

        protected override Node AddNode(TVertex v)
        {
            string id = GetVertexId(v);
            vertexIds.Add(v, id);
            Node node = (Node)GleeGraph.AddNode(id);
            node.Attr.Shape = Shape.Box;
            node.Attr.Label = GetVertexLabel(id, v);
            return node;
        }

        protected virtual string GetVertexId(TVertex v)
        {
            return vertexIds.Count.ToString();
        }

        protected virtual string GetVertexLabel(string id, TVertex v)
        {
            return string.Format("{0}: {1}", id, v.ToString());
        }

        protected override Edge AddEdge(TEdge e)
        {
            return (Edge)GleeGraph.AddEdge(
                vertexIds[e.Source],
                vertexIds[e.Target]
                );
        }
    }
}
