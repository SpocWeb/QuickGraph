using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms;
using QuickGraph.Serialization.DirectedGraphML;

namespace QuickGraph.Serialization
{
    public sealed class DirectedGraphMLAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IVertexAndEdgeListGraph<TVertex,TEdge>>
        where TEdge : IEdge<TVertex>
    {
        readonly VertexIdentity<TVertex> vertexIdentities;
        readonly EdgeIdentity<TVertex, TEdge> edgeIdentities;
        DirectedGraph directedGraph;

        public DirectedGraphMLAlgorithm(
            IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            VertexIdentity<TVertex> vertexIdentities,
            EdgeIdentity<TVertex, TEdge> edgeIdentities)
            :base(visitedGraph)
        {
            Contract.Requires(vertexIdentities != null);
            this.vertexIdentities = vertexIdentities;
            this.edgeIdentities = edgeIdentities;
        }

        public DirectedGraph DirectedGraph
        {
            get { return directedGraph; }
        }

        protected override void InternalCompute()
        {
            var cancelManager = Services.CancelManager;
            directedGraph = new DirectedGraph();

            var nodes = new List<DirectedGraphNode>(VisitedGraph.VertexCount);
            foreach (var vertex in VisitedGraph.Vertices)
            {
                if (cancelManager.IsCancelling) return;

                var node = new DirectedGraphNode { Id = vertexIdentities(vertex) };
                OnFormatNode(vertex, node);
                nodes.Add(node);
            }
            directedGraph.Nodes = nodes.ToArray();

            var links = new List<DirectedGraphLink>(VisitedGraph.EdgeCount);
            foreach (var edge in VisitedGraph.Edges)
            {
                if (cancelManager.IsCancelling) return;

                var link = new DirectedGraphLink
                {
                    Label = edgeIdentities(edge),
                    Source = vertexIdentities(edge.Source),
                    Target = vertexIdentities(edge.Target)
                };
                OnFormatEdge(edge, link);
                links.Add(link);
            }
            directedGraph.Links = links.ToArray();

            OnFormatGraph();
        }

        /// <summary>
        /// Raised when the graph is about to be returned
        /// </summary>
        public event Action<IVertexAndEdgeListGraph<TVertex, TEdge>, DirectedGraph> FormatGraph;

        private void OnFormatGraph()
        {
            var eh = FormatGraph;
            if (eh != null)
                eh(VisitedGraph, DirectedGraph);
        }

        /// <summary>
        /// Raised when a new link is added to the graph
        /// </summary>
        public event Action<TEdge, DirectedGraphLink> FormatEdge;

        private void OnFormatEdge(TEdge edge, DirectedGraphLink link)
        {
            Contract.Requires(edge != null);
            Contract.Requires(link != null);

            var eh = FormatEdge;
            if (eh != null)
                eh(edge, link);
        }

        /// <summary>
        /// Raised when a new node is added to the graph
        /// </summary>
        public event Action<TVertex, DirectedGraphNode> FormatNode;

        private void OnFormatNode(TVertex vertex, DirectedGraphNode node)
        {
            Contract.Requires(node != null);
            var eh = FormatNode;
            if (eh != null)
                eh(vertex, node);
        }
    }
}
