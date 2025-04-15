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
    public sealed class StronglyConnectedComponentsAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IVertexListGraph<TVertex, TEdge>>,
        IConnectedComponentAlgorithm<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private readonly IDictionary<TVertex, int> components;
        private readonly Dictionary<TVertex, int> discoverTimes;
        private readonly Dictionary<TVertex, TVertex> roots;
        private Stack<TVertex> stack;
        int componentCount;
        int dfsTime;
        private List<int> diffBySteps;
        private int step;
        private List<TVertex> vertices;
        List<BidirectionalGraph<TVertex, TEdge>> graphs;

        public StronglyConnectedComponentsAlgorithm(
            IVertexListGraph<TVertex, TEdge> g)
            : this(g, new Dictionary<TVertex, int>())
        { }

        public StronglyConnectedComponentsAlgorithm(
            IVertexListGraph<TVertex, TEdge> g,
            IDictionary<TVertex, int> components)
            : this(null, g, components)
        { }

        public StronglyConnectedComponentsAlgorithm(
            IAlgorithmComponent host,
            IVertexListGraph<TVertex, TEdge> g,
            IDictionary<TVertex, int> components)
            : base(host, g)
        {
            Contract.Requires(components != null);

            this.components = components;
            roots = new Dictionary<TVertex, TVertex>();
            discoverTimes = new Dictionary<TVertex, int>();
            stack = new Stack<TVertex>();
            componentCount = 0;
            dfsTime = 0;
        }

        public IDictionary<TVertex, int> Components
        {
            get
            {
                return components;
            }
        }

        public IDictionary<TVertex, TVertex> Roots
        {
            get
            {
                return roots;
            }
        }

        public IDictionary<TVertex, int> DiscoverTimes
        {
            get
            {
                return discoverTimes;
            }
        }

        public int ComponentCount
        {
            get
            {
                return componentCount;
            }
        }

        public List<TVertex> Vertices
        {
            get
            {
                return vertices;
            }
        }

        public int Steps
        {
            get
            {
                return step;
            }
        }
        public List<int> DiffBySteps
        {
            get
            {
                return diffBySteps;
            }
        }

        private void DiscoverVertex(TVertex v)
        {
            Roots[v] = v;
            Components[v] = int.MaxValue;

            // this.diffBySteps[step] = componentCount;
            diffBySteps.Add(componentCount);
            vertices.Add(v);
            step++;

            DiscoverTimes[v] = dfsTime++;
            stack.Push(v);
        }

        private void FinishVertex(TVertex v)
        {
            var roots = Roots;

            foreach (var e in VisitedGraph.OutEdges(v))
            {
                var w = e.Target;
                if (Components[w] == int.MaxValue)
                    roots[v] = MinDiscoverTime(roots[v], roots[w]);
            }

            if (this.roots[v].Equals(v))
            {
                var w = default(TVertex);
                do
                {
                    w = stack.Pop();
                    Components[w] = componentCount;


                    diffBySteps.Add(componentCount);
                    vertices.Add(w);
                    step++;
                }
                while (!w.Equals(v));
                ++componentCount;

            }
        }

        private TVertex MinDiscoverTime(TVertex u, TVertex v)
        {
            Contract.Requires(u != null);
            Contract.Requires(v != null);
            Contract.Ensures(DiscoverTimes[u] < DiscoverTimes[v]
                ? Contract.Result<TVertex>().Equals(u)
                : Contract.Result<TVertex>().Equals(v)
                );

            TVertex minVertex = discoverTimes[u] < discoverTimes[v] ? u : v;
            return minVertex;
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
            Contract.Ensures(ComponentCount >= 0);
            Contract.Ensures(VisitedGraph.VertexCount == 0 || ComponentCount > 0);
            Contract.Ensures(VisitedGraph.Vertices.All(v => Components.ContainsKey(v)));
            Contract.Ensures(VisitedGraph.VertexCount == Components.Count);
            Contract.Ensures(Components.Values.All(c => c <= ComponentCount));

            diffBySteps = new List<int>();
            vertices = new List<TVertex>();

            Components.Clear();
            Roots.Clear();
            DiscoverTimes.Clear();
            stack.Clear();
            componentCount = 0;
            dfsTime = 0;

            DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
            try
            {
                dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(
                    this,
                    VisitedGraph,
                    new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount)
                    );
                dfs.DiscoverVertex += DiscoverVertex;
                dfs.FinishVertex += FinishVertex;

                dfs.Compute();
            }
            finally
            {
                if (dfs != null)
                {
                    dfs.DiscoverVertex -= DiscoverVertex;
                    dfs.FinishVertex -= FinishVertex;
                }
            }
        }
    }
}