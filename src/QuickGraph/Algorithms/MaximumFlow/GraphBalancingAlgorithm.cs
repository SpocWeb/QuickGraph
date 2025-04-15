using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.MaximumFlow
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class GraphBalancerAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private IMutableBidirectionalGraph<TVertex,TEdge> visitedGraph;
        private VertexFactory<TVertex> vertexFactory;
        private EdgeFactory<TVertex,TEdge> edgeFactory;

        private TVertex source;
        private TVertex sink;

        private TVertex balancingSource;
        private TEdge balancingSourceEdge;

        private TVertex balancingSink;
        private TEdge balancingSinkEdge;

        private IDictionary<TEdge,double> capacities = new Dictionary<TEdge,double>();
        private Dictionary<TEdge,int> preFlow = new Dictionary<TEdge,int>();
        private List<TVertex> surplusVertices = new List<TVertex>();
        private List<TEdge> surplusEdges = new List<TEdge>();
        private List<TVertex> deficientVertices = new List<TVertex>();
        private List<TEdge> deficientEdges = new List<TEdge>();
        private bool balanced = false;

        public GraphBalancerAlgorithm(
            IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
            TVertex source,
            TVertex sink,
            VertexFactory<TVertex> vertexFactory,
            EdgeFactory<TVertex,TEdge> edgeFactory
            )
        {
            Contract.Requires(visitedGraph != null);
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);
            Contract.Requires(source != null);
            Contract.Requires(visitedGraph.ContainsVertex(source));
            Contract.Requires(sink != null);
            Contract.Requires(visitedGraph.ContainsVertex(sink));

            this.visitedGraph = visitedGraph;
            this.vertexFactory = vertexFactory;
            this.edgeFactory = edgeFactory;
            this.source = source;
            this.sink = sink;

            // setting capacities = u(e) = +infty
            foreach (var edge in VisitedGraph.Edges)
                capacities.Add(edge, double.MaxValue);

            // setting preflow = l(e) = 1
            foreach (var edge in VisitedGraph.Edges)
                preFlow.Add(edge, 1);
        }

        public GraphBalancerAlgorithm(
            IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
            VertexFactory<TVertex> vertexFactory,
            EdgeFactory<TVertex,TEdge> edgeFactory,
            TVertex source,
            TVertex sink,
            IDictionary<TEdge,double> capacities)
        {
            Contract.Requires(visitedGraph != null);
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);
            Contract.Requires(source != null);
            Contract.Requires(visitedGraph.ContainsVertex(source));
            Contract.Requires(sink != null);
            Contract.Requires(visitedGraph.ContainsVertex(sink));
            Contract.Requires(capacities != null);

            this.visitedGraph = visitedGraph;
            this.source = source;
            this.sink = sink;
            this.capacities = capacities;

            // setting preflow = l(e) = 1
            foreach (var edge in VisitedGraph.Edges)
                preFlow.Add(edge, 1);
        }

        public IMutableBidirectionalGraph<TVertex, TEdge> VisitedGraph
        {
            get
            {
                return visitedGraph;
            }
        }

        public VertexFactory<TVertex> VertexFactory
        {
            get { return vertexFactory;}
        }

        public EdgeFactory<TVertex,TEdge> EdgeFactory
        {
            get { return edgeFactory;}
        }

        public bool Balanced
        {
            get
            {
                return balanced;
            }
        }

        public TVertex Source
        {
            get
            {
                return source;
            }
        }
        public TVertex Sink
        {
            get
            {
                return sink;
            }
        }
        public TVertex BalancingSource
        {
            get
            {
                return balancingSource;
            }
        }
        public TEdge BalancingSourceEdge
        {
            get
            {
                return balancingSourceEdge;
            }
        }
        public TVertex BalancingSink
        {
            get
            {
                return balancingSink;
            }
        }
        public TEdge BalancingSinkEdge
        {
            get
            {
                return balancingSinkEdge;
            }
        }
        public ICollection<TVertex> SurplusVertices
        {
            get
            {
                return surplusVertices;
            }
        }
        public ICollection<TEdge> SurplusEdges
        {
            get
            {
                return surplusEdges;
            }
        }
        public ICollection<TVertex> DeficientVertices
        {
            get
            {
                return deficientVertices;
            }
        }
        public ICollection<TEdge> DeficientEdges
        {
            get
            {
                return deficientEdges;
            }
        }
        public IDictionary<TEdge,double> Capacities
        {
            get
            {
                return capacities;
            }
        }

        public event VertexAction<TVertex> BalancingSourceAdded;
        private void OnBalancingSourceAdded()
        {
            var eh = BalancingSourceAdded;
            if (eh != null)
                eh(source);
        }
        public event VertexAction<TVertex> BalancingSinkAdded;
        private void OnBalancingSinkAdded()
        {
            var eh = BalancingSinkAdded;
            if (eh != null)
                eh(sink);
        }
        public event EdgeAction<TVertex,TEdge> EdgeAdded;
        private void OnEdgeAdded(TEdge edge)
        {
            Contract.Requires(edge != null);

            var eh = EdgeAdded;
            if (eh != null)
                eh(edge);
        }
        public event VertexAction<TVertex> SurplusVertexAdded;
        private void OnSurplusVertexAdded(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            var eh = SurplusVertexAdded;
            if (eh != null)
                eh(vertex);
        }
        public event VertexAction<TVertex> DeficientVertexAdded;
        private void OnDeficientVertexAdded(TVertex vertex)
        {
            Contract.Requires(vertex != null);

            var eh = DeficientVertexAdded;
            if (eh != null)
                eh(vertex);
        }

        public int GetBalancingIndex(TVertex v)
        {
            Contract.Requires(v != null);

            int bi = 0;
            foreach (var edge in VisitedGraph.OutEdges(v))
            {
                int pf = preFlow[edge];
                bi += pf;
            }
            foreach (var edge in VisitedGraph.InEdges(v))
            {
                int pf = preFlow[edge];
                bi -= pf;
            }
            return bi;
        }

        public void Balance()
        {
            if (Balanced)
                throw new InvalidOperationException("Graph already balanced");

            // step 0
            // create new source, new sink
            balancingSource = VertexFactory();
            visitedGraph.AddVertex(balancingSource);
            OnBalancingSourceAdded();

            balancingSink = VertexFactory();
            visitedGraph.AddVertex(balancingSink);
            OnBalancingSinkAdded();

            // step 1
            balancingSourceEdge = EdgeFactory(BalancingSource, Source);
            VisitedGraph.AddEdge(BalancingSourceEdge);
            capacities.Add(balancingSourceEdge, double.MaxValue);
            preFlow.Add(balancingSourceEdge, 0);
            OnEdgeAdded(balancingSourceEdge);

            balancingSinkEdge = EdgeFactory(Sink, BalancingSink);
            VisitedGraph.AddEdge(balancingSinkEdge);
            capacities.Add(balancingSinkEdge, double.MaxValue);
            preFlow.Add(balancingSinkEdge, 0);
            OnEdgeAdded(balancingSinkEdge);

            // step 2
            // for each surplus vertex v, add (source -> v)
            foreach (var v in VisitedGraph.Vertices)
            {
                if (v.Equals(balancingSource))
                    continue;
                if (v.Equals(balancingSink))
                    continue;
                if (v.Equals(source))
                    continue;
                if (v.Equals(sink))
                    continue;

                int balacingIndex = GetBalancingIndex(v);
                if (balacingIndex == 0)
                    continue;

                if (balacingIndex < 0)
                {
                    // surplus vertex
                    TEdge edge = EdgeFactory(BalancingSource, v);
                    VisitedGraph.AddEdge(edge);
                    surplusEdges.Add(edge);
                    surplusVertices.Add(v);
                    preFlow.Add(edge, 0);
                    capacities.Add(edge, -balacingIndex);
                    OnSurplusVertexAdded(v);
                    OnEdgeAdded(edge);
                }
                else
                {
                    // deficient vertex
                    TEdge edge = EdgeFactory(v, BalancingSink);
                    deficientEdges.Add(edge);
                    deficientVertices.Add(v);
                    preFlow.Add(edge, 0);
                    capacities.Add(edge, balacingIndex);
                    OnDeficientVertexAdded(v);
                    OnEdgeAdded(edge);
                }
            }

            balanced = true;
        }

        public void UnBalance()
        {
            if (!Balanced)
                throw new InvalidOperationException("Graph is not balanced");
            foreach (var edge in surplusEdges)
            {
                VisitedGraph.RemoveEdge(edge);
                capacities.Remove(edge);
                preFlow.Remove(edge);
            }
            foreach (var edge in deficientEdges)
            {
                VisitedGraph.RemoveEdge(edge);
                capacities.Remove(edge);
                preFlow.Remove(edge);
            }

            capacities.Remove(BalancingSinkEdge);
            capacities.Remove(BalancingSourceEdge);
            preFlow.Remove(BalancingSinkEdge);
            preFlow.Remove(BalancingSourceEdge);
            VisitedGraph.RemoveEdge(BalancingSourceEdge);
            VisitedGraph.RemoveEdge(BalancingSinkEdge);
            VisitedGraph.RemoveVertex(BalancingSource);
            VisitedGraph.RemoveVertex(BalancingSink);

            balancingSource = default(TVertex);
            balancingSink = default(TVertex);
            balancingSourceEdge = default(TEdge);
            balancingSinkEdge = default(TEdge);

            surplusEdges.Clear();
            deficientEdges.Clear();
            surplusVertices.Clear();
            deficientVertices.Clear();

            balanced = false;
        }
    }
}
