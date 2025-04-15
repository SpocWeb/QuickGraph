using System;
using System.Collections.Generic;

using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.ConnectedComponents
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class ConnectedComponentsAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>,
        IConnectedComponentAlgorithm<TVertex,TEdge,IUndirectedGraph<TVertex,TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private IDictionary<TVertex, int> components;
        private int componentCount=0;

        public ConnectedComponentsAlgorithm(IUndirectedGraph<TVertex, TEdge> g)
            :this(g, new Dictionary<TVertex, int>())
        { }

        public ConnectedComponentsAlgorithm(
            IUndirectedGraph<TVertex, TEdge> visitedGraph,
            IDictionary<TVertex, int> components)
            : this(null, visitedGraph, components)
        { }

        public ConnectedComponentsAlgorithm(
            IAlgorithmComponent host,
            IUndirectedGraph<TVertex, TEdge> visitedGraph,
            IDictionary<TVertex, int> components)
            :base(host, visitedGraph)
        {
            Contract.Requires(components != null);

            this.components = components;
        }

        public IDictionary<TVertex,int> Components
        {
            get
            {
                return components;
            }
        }

        public int ComponentCount
        {
            get { return componentCount; }
        }

        private void StartVertex(TVertex v)
        {
            ++componentCount;
        }

        private void DiscoverVertex(TVertex v)
        {
            Components[v] = componentCount;
        }

        protected override void InternalCompute()
        {
            components.Clear();
            if (VisitedGraph.VertexCount == 0)
            {
                componentCount = 0;
                return;
            }

            componentCount = -1;
            UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
            try
            {
                dfs = new UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge>(
                    this,
                    VisitedGraph,
                    new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount)
                    );

                dfs.StartVertex += StartVertex;
                dfs.DiscoverVertex += DiscoverVertex;
                dfs.Compute();
                ++componentCount;
            }
            finally
            {
                if (dfs != null)
                {
                    dfs.StartVertex -= StartVertex;
                    dfs.DiscoverVertex -= DiscoverVertex;
                }
            }
        }
    }
}
