using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.MaximumFlow
{
    public abstract class GraphAugmentorAlgorithmBase<TVertex,TEdge,TGraph> 
        : AlgorithmBase<TGraph>
        , IDisposable
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
    {
        private bool augmented = false;
        private readonly List<TEdge> augmentedEdges = new List<TEdge>();
        private readonly VertexFactory<TVertex> vertexFactory;
        private readonly EdgeFactory<TVertex, TEdge> edgeFactory;

        private TVertex superSource = default(TVertex);
        private TVertex superSink = default(TVertex);

        protected GraphAugmentorAlgorithmBase(
            IAlgorithmComponent host,
            TGraph visitedGraph,
            VertexFactory<TVertex> vertexFactory,
            EdgeFactory<TVertex,TEdge> edgeFactory
            )
            :base(host, visitedGraph)
        {
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);

            this.vertexFactory = vertexFactory;
            this.edgeFactory = edgeFactory;
        }

        public VertexFactory<TVertex> VertexFactory
        {
            get { return vertexFactory; }
        }

        public EdgeFactory<TVertex, TEdge> EdgeFactory
        {
            get { return edgeFactory; }
        }

        public TVertex SuperSource
        {
            get { return superSource; }
        }

        public TVertex SuperSink
        {
            get { return superSink; }
        }

        public bool Augmented
        {
            get { return augmented; }
        }

        public ICollection<TEdge> AugmentedEdges
        {
            get { return augmentedEdges; }
        }

        public event VertexAction<TVertex> SuperSourceAdded;
        private void OnSuperSourceAdded(TVertex v)
        {
            Contract.Requires(v != null);
            var eh = SuperSourceAdded;
            if (eh != null)
                eh(v);
        }

        public event VertexAction<TVertex> SuperSinkAdded;
        private void OnSuperSinkAdded(TVertex v)
        {
            Contract.Requires(v != null);
            var eh = SuperSinkAdded;
            if (eh != null)
                eh(v);
        }

        public event EdgeAction<TVertex, TEdge> EdgeAdded;
        private void OnEdgeAdded(TEdge e)
        {
            Contract.Requires(e != null);
            var eh = EdgeAdded;
            if (eh != null)
                eh(e);
        }


        protected override void InternalCompute()
        {
            if (Augmented)
                throw new InvalidOperationException("Graph already augmented");

            superSource = VertexFactory();
            VisitedGraph.AddVertex(superSource);
            OnSuperSourceAdded(SuperSource);

            superSink = VertexFactory();
            VisitedGraph.AddVertex(superSink);
            OnSuperSinkAdded(SuperSink);

            AugmentGraph();
            augmented = true;
        }

        public virtual void Rollback()
        {
            if (!Augmented)
                return;

            augmented = false;
            VisitedGraph.RemoveVertex(SuperSource);
            VisitedGraph.RemoveVertex(SuperSink);
            superSource = default(TVertex);
            superSink = default(TVertex);
            augmentedEdges.Clear();
        }

        public void Dispose()
        {
            Rollback();
        }

        protected abstract void AugmentGraph();

        protected void AddAugmentedEdge(TVertex source, TVertex target)
        {
            TEdge edge = EdgeFactory(source, target);
            augmentedEdges.Add(edge);
            VisitedGraph.AddEdge(edge);
            OnEdgeAdded(edge);
        }
    }
}
