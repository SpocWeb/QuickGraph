using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph.Algorithms.ConnectedComponents
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class WeaklyConnectedComponentsAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IVertexListGraph<TVertex, TEdge>>,
        IConnectedComponentAlgorithm<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private readonly IDictionary<TVertex, int> components;
        private readonly Dictionary<int, int> componentEquivalences = new Dictionary<int, int>();
        private int componentCount = 0;
        private int currentComponent = 0;
        private List<BidirectionalGraph<TVertex, TEdge>> graphs;

        public WeaklyConnectedComponentsAlgorithm(IVertexListGraph<TVertex, TEdge> visitedGraph)
            : this(visitedGraph, new Dictionary<TVertex, int>())
        { }

        public WeaklyConnectedComponentsAlgorithm(
            IVertexListGraph<TVertex, TEdge> visitedGraph,
            IDictionary<TVertex, int> components)
            : this(null, visitedGraph, components)
        { }

        public WeaklyConnectedComponentsAlgorithm(
            IAlgorithmComponent host,
            IVertexListGraph<TVertex, TEdge> visitedGraph,
            IDictionary<TVertex, int> components)
            : base(host, visitedGraph)
        {
            Contract.Requires(components != null);

            this.components = components;
        }

        public IDictionary<TVertex, int> Components
        {
            get { return components; }
        }

        public int ComponentCount
        {
            get { return componentCount; }
        }

        protected override void Initialize()
        {
            componentCount = 0;
            currentComponent = 0;
            componentEquivalences.Clear();
            components.Clear();
        }

        public List<BidirectionalGraph<TVertex, TEdge>> Graphs
        {
            get
            {
                int i;
                graphs = new List<BidirectionalGraph<TVertex, TEdge>>(componentCount + 1);
                for (i = 0; i < componentCount; i++)
                {
                    graphs.Add(new BidirectionalGraph<TVertex, TEdge>());
                }
                foreach (TVertex componentName in components.Keys)
                {
                    graphs[components[componentName]].AddVertex(componentName);
                }

                foreach (TVertex vertex in VisitedGraph.Vertices)
                {
                    foreach (TEdge edge in VisitedGraph.OutEdges(vertex))
                    {

                        if (components[vertex] == components[edge.Target])
                        {
                            graphs[components[vertex]].AddEdge(edge);
                        }
                    }
                }
                return graphs;
            }

        }

        protected override void InternalCompute()
        {
            Contract.Ensures(0 <= ComponentCount && ComponentCount <= VisitedGraph.VertexCount);
            Contract.Ensures(VisitedGraph.Vertices.All(v => 0 <= Components[v] && Components[v] < ComponentCount));

            // shortcut for empty graph
            if (VisitedGraph.IsVerticesEmpty)
                return;

            var dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(VisitedGraph);
            try
            {
                dfs.StartVertex += dfs_StartVertex;
                dfs.TreeEdge += dfs_TreeEdge;
                dfs.ForwardOrCrossEdge += dfs_ForwardOrCrossEdge;

                dfs.Compute();
            }
            finally
            {
                dfs.StartVertex -= dfs_StartVertex;
                dfs.TreeEdge -= dfs_TreeEdge;
                dfs.ForwardOrCrossEdge -= dfs_ForwardOrCrossEdge;
            }

            // updating component numbers
            foreach (var v in VisitedGraph.Vertices)
            {
                int component = components[v];
                int equivalent = GetComponentEquivalence(component);
                if (component != equivalent)
                    components[v] = equivalent;
            }
            componentEquivalences.Clear();
        }

        void dfs_StartVertex(TVertex v)
        {
            // we are looking on a new tree
            currentComponent = componentEquivalences.Count;
            componentEquivalences.Add(currentComponent, currentComponent);
            componentCount++;
            components.Add(v, currentComponent);
        }

        void dfs_TreeEdge(TEdge e)
        {
            // new edge, we store with the current component number
            components.Add(e.Target, currentComponent);
        }

        private int GetComponentEquivalence(int component)
        {
            int equivalent = component;
            int temp = componentEquivalences[equivalent];
            bool compress = false;
            while (temp != equivalent)
            {
                equivalent = temp;
                temp = componentEquivalences[equivalent];
                compress = true;
            }

            // path compression
            if (compress)
            {
                int c = component;
                temp = componentEquivalences[c];
                while (temp != equivalent)
                {
                    temp = componentEquivalences[c];
                    componentEquivalences[c] = equivalent;
                }
            }

            return equivalent;
        }

        void dfs_ForwardOrCrossEdge(TEdge e)
        {
            // we have touched another tree, updating count and current component
            int otherComponent = GetComponentEquivalence(components[e.Target]);
            if (otherComponent != currentComponent)
            {
                componentCount--;
                Contract.Assert(componentCount > 0);
                if (currentComponent > otherComponent)
                {
                    componentEquivalences[currentComponent] = otherComponent;
                    currentComponent = otherComponent;
                }
                else
                {
                    componentEquivalences[otherComponent] = currentComponent;
                }
            }
        }
    }
}