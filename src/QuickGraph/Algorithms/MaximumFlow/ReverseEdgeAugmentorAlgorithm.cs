using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.MaximumFlow
{
    public static class ReversedEdgeAugmentorAlgorithm
    {
        public static ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> CreateReversedEdgeAugmentorAlgorithm<TVertex, TEdge>(
            this IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            EdgeFactory<TVertex, TEdge> edgeFactory, IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => null;//new ReversedEdgeAugmentorAlgorithm<TVertex, TEdge>(graph, edgeFactory, host);
    }
    /// <summary> Routines to add and remove auxiliary edges when using <see cref="EdmondsKarpMaximumFlowAlgorithm{TVertex, TEdge}"/> 
    /// or <see cref="MaximumBipartiteMatchingAlgorithm{TVertex, TEdge}.InternalCompute()"/>. 
    /// Remember to call <see cref="RemoveReversedEdges()"/> to remove auxiliary edges.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    /// <typeparam name="TEdge">The type of edge.</typeparam>
    /// <remarks>
    /// Will throw an exception in <see cref="ReversedEdgeAugmentorAlgorithm{TVertex, TEdge}.AddReversedEdges"/> if TEdge is a value type,
    /// e.g. <see cref="SEdge{TVertex}"/>.
    /// <seealso href="https://github.com/YaccConstructor/QuickGraph/issues/183#issue-377613647"/>.
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class ReversedEdgeAugmentorAlgorithm<TVertex, TEdge>
        : IDisposable
        where TEdge : IEdge<TVertex>
    {
        private readonly IMutableVertexAndEdgeListGraph<TVertex,TEdge> visitedGraph;
        private readonly EdgeFactory<TVertex, TEdge> edgeFactory;
        private IList<TEdge> augmentedEgdes = new List<TEdge>();
        private Dictionary<TEdge,TEdge> reversedEdges = new Dictionary<TEdge,TEdge>();
        private bool augmented = false;
         ReversedEdgeAugmentorAlgorithm(IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            EdgeFactory<TVertex, TEdge> edgeFactory, IAlgorithmComponent host = null)
        {
            Contract.Requires(visitedGraph != null);
            Contract.Requires(edgeFactory != null);

            this.visitedGraph = visitedGraph;
            this.edgeFactory = edgeFactory;
        }

        public IMutableVertexAndEdgeListGraph<TVertex,TEdge> VisitedGraph
        {
            get
            {
                return visitedGraph;
            }
        }

        public EdgeFactory<TVertex, TEdge> EdgeFactory
        {
            get { return edgeFactory; }
        }

        public ICollection<TEdge> AugmentedEdges
        {
            get
            {
                return augmentedEgdes;
            }
        }

        public Dictionary<TEdge,TEdge> ReversedEdges
        {
            get
            {
                return reversedEdges;
            }
        }

        public bool Augmented
        {
            get
            {
                return augmented;
            }
        }

        public event EdgeAction<TVertex,TEdge> ReversedEdgeAdded;
        private void OnReservedEdgeAdded(TEdge e)
        {
            var eh = ReversedEdgeAdded;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// Adds auxiliary edges to <see cref="VisitedGraph"/> to store residual flows.
        /// </summary>
        /// <remarks>
        /// Will throw an exception if TEdge is a value type, e.g. <see cref="SEdge{TVertex}"/>.
        /// <seealso href="https://github.com/YaccConstructor/QuickGraph/issues/183#issue-377613647"/>.
        /// </remarks>
        public void AddReversedEdges()
        {
            if (Augmented)
                throw new InvalidOperationException("Graph already augmented");
            // step 1, find edges that need reversing
            IList<TEdge> notReversedEdges = new List<TEdge>();
            foreach (var edge in VisitedGraph.Edges)
            {
                // if reversed already found, continue
                if (reversedEdges.ContainsKey(edge))
                    continue;

                TEdge reversedEdge = FindReversedEdge(edge);
                if (reversedEdge != null)
                {
                    // setup edge
                    reversedEdges[edge] = reversedEdge;
                    // setup reversed if needed
                    if (!reversedEdges.ContainsKey(reversedEdge))
                        reversedEdges[reversedEdge] = edge;
                    continue;
                }

                // this edge has no reverse
                notReversedEdges.Add(edge);
            }

            // step 2, go over each not reversed edge, add reverse
            foreach (var edge in notReversedEdges)
            {
                if (reversedEdges.ContainsKey(edge))
                    continue;

                // already been added
                TEdge reversedEdge = FindReversedEdge(edge);
                if (reversedEdge != null)
                {
                    reversedEdges[edge] = reversedEdge;
                    continue;
                }

                // need to create one
                reversedEdge = edgeFactory(edge.Target, edge.Source);
                if (!VisitedGraph.AddEdge(reversedEdge))
                    throw new InvalidOperationException("We should not be here");
                augmentedEgdes.Add(reversedEdge);
                reversedEdges[edge] = reversedEdge;
                reversedEdges[reversedEdge] = edge;
                OnReservedEdgeAdded(reversedEdge);
            }

            augmented = true;
        }

        public void RemoveReversedEdges()
        {
            if (!Augmented)
                throw new InvalidOperationException("Graph is not yet augmented");

            foreach (var edge in augmentedEgdes)
                VisitedGraph.RemoveEdge(edge);

            augmentedEgdes.Clear();
            reversedEdges.Clear();

            augmented = false;
        }

        private TEdge FindReversedEdge(TEdge edge)
        {
            foreach (var redge in VisitedGraph.OutEdges(edge.Target))
                if (redge.Target.Equals(edge.Source))
                    return redge;
            return default(TEdge);
        }

        void IDisposable.Dispose()
        {
            if(Augmented)
                RemoveReversedEdges();
        }
    }
}
