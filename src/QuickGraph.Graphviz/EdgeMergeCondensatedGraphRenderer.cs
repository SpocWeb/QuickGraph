﻿using System;
using System.IO;
using QuickGraph.Algorithms.Condensation;

namespace QuickGraph.Graphviz
{
    public class EdgeMergeCondensatedGraphRenderer<TVertex, TEdge> :
        GraphRendererBase<TVertex, MergedEdge<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        public EdgeMergeCondensatedGraphRenderer(
            IVertexAndEdgeListGraph<TVertex, MergedEdge<TVertex, TEdge>> visitedGraph)
            :base(visitedGraph)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            this.Graphviz.FormatVertex += Graphviz_FormatVertex;
            this.Graphviz.FormatEdge += Graphviz_FormatEdge;
        }

        void Graphviz_FormatEdge(object sender, FormatEdgeEventArgs<TVertex, MergedEdge<TVertex, TEdge>> e)
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("{0}", e.Edge.Edges.Count);
            foreach (var edge in e.Edge.Edges)
                sw.WriteLine("  {0}", edge);
            e.EdgeFormatter.Label.Value = this.Graphviz.Escape(sw.ToString());
        }

        static void Graphviz_FormatVertex(object sender, FormatVertexEventArgs<TVertex> e)
        {
            e.VertexFormatter.Label = e.Vertex.ToString();
        }
    }
}
