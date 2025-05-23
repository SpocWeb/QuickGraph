﻿using System;
using System.IO;
using QuickGraph.Algorithms.Condensation;

namespace QuickGraph.Graphviz
{
    public class CondensatedGraphRenderer<TVertex,TEdge,TGraph> :
        GraphRendererBase<TGraph, CondensedEdge<TVertex, TEdge, TGraph>>
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>, new()
    {
        public CondensatedGraphRenderer(
            IVertexAndEdgeListGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> visitedGraph)
            :base(visitedGraph)
        {}

        protected override void Initialize()
        {
            base.Initialize();
            Graphviz.FormatVertex += Graphviz_FormatVertex;
            Graphviz.FormatEdge += Graphviz_FormatEdge;
        }


        void Graphviz_FormatVertex(object sender, FormatVertexEventArgs<TGraph> e)
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("{0}-{1}", e.Vertex.VertexCount, e.Vertex.EdgeCount);
            foreach (var v in e.Vertex.Vertices)
                sw.WriteLine("  {0}", v);
            foreach(TEdge edge in e.Vertex.Edges)
                sw.WriteLine("  {0}", edge);
            e.VertexFormatter.Label = Graphviz.Escape(sw.ToString());
        }

        void Graphviz_FormatEdge(object sender, FormatEdgeEventArgs<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> e)
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("{0}", e.Edge.Edges.Count);
            foreach (var edge in e.Edge.Edges)
                sw.WriteLine("  {0}", edge);
            e.EdgeFormatter.Label.Value = Graphviz.Escape(sw.ToString());
        }
    }
}
