#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Exploration
{
    public sealed class CloneableVertexGraphExplorerAlgorithm<TVertex,TEdge> 
        : RootedAlgorithmBase<TVertex, IMutableVertexAndEdgeSet<TVertex, TEdge>>
        , ITreeBuilderAlgorithm<TVertex,TEdge>
        where TVertex : ICloneable, IComparable<TVertex>
        where TEdge : IEdge<TVertex>
    {
        private readonly IList<ITransitionFactory<TVertex, TEdge>> transitionFactories = new List<ITransitionFactory<TVertex, TEdge>>();

        private readonly Queue<TVertex> unexploredVertices = new Queue<TVertex>();

        private VertexPredicate<TVertex> addVertexPredicate = v => true;
        private VertexPredicate<TVertex> exploreVertexPredicate = v => true;
        private EdgePredicate<TVertex, TEdge> addEdgePredicate = e => true;
        private Predicate<CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>> finishedPredicate =
            new DefaultFinishedPredicate().Test;
        private bool finishedSuccessfully;

        public CloneableVertexGraphExplorerAlgorithm(
            IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph
            )
            : this(null, visitedGraph)
        { }

        public CloneableVertexGraphExplorerAlgorithm(
            IAlgorithmComponent host,
            IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph
            )
            :base(host, visitedGraph)
        {}

        public IList<ITransitionFactory<TVertex, TEdge>> TransitionFactories => transitionFactories;

        public VertexPredicate<TVertex> AddVertexPredicate
        {
            get => addVertexPredicate;
            set => addVertexPredicate = value;
        }

        public VertexPredicate<TVertex> ExploreVertexPredicate
        {
            get => exploreVertexPredicate;
            set => exploreVertexPredicate = value;
        }

        public EdgePredicate<TVertex, TEdge> AddEdgePredicate
        {
            get => addEdgePredicate;
            set => addEdgePredicate = value;
        }

        public Predicate<CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>> FinishedPredicate
        {
            get => finishedPredicate;
            set => finishedPredicate = value;
        }

        public IEnumerable<TVertex> UnexploredVertices => unexploredVertices;

        public bool FinishedSuccessfully => finishedSuccessfully;

        public event VertexAction<TVertex> DiscoverVertex;
        private void OnDiscoverVertex(TVertex v)
        {
            Contract.Requires(v != null);

            VisitedGraph.AddVertex(v);
            unexploredVertices.Enqueue(v);

            var eh = DiscoverVertex;
            if (eh != null)
                eh(v);
        }
        public event EdgeAction<TVertex,TEdge> TreeEdge;
        private void OnTreeEdge(TEdge e)
        {
            Contract.Requires(e != null);

            var eh = TreeEdge;
            if (eh != null)
                eh(e);
        }
        public event EdgeAction<TVertex, TEdge> BackEdge;
        private void OnBackEdge(TEdge e)
        {
            Contract.Requires(e != null);
            var eh = BackEdge;
            if (eh != null)
                eh(e);
        }
        public event EdgeAction<TVertex, TEdge> EdgeSkipped;
        private void OnEdgeSkipped(TEdge e)
        {
            Contract.Requires(e != null);
            var eh = EdgeSkipped;
            if (eh != null)
                eh(e);
        }

        protected override void  InternalCompute()
        {
            TVertex rootVertex;
            if (!TryGetRootVertex(out rootVertex))
                throw new InvalidOperationException("RootVertex is not specified");

            VisitedGraph.Clear();
            unexploredVertices.Clear();
            finishedSuccessfully = false;

            if (!AddVertexPredicate(rootVertex))
                throw new ArgumentException("StartVertex does not satisfy AddVertexPredicate");
            OnDiscoverVertex(rootVertex);

            while (unexploredVertices.Count > 0)
            {
                // are we done yet ?
                if (!FinishedPredicate(this))
                {
                    finishedSuccessfully = false;
                    return;
                }

                TVertex current = unexploredVertices.Dequeue();
                TVertex clone = (TVertex)current.Clone();

                // let's make sure we want to explore this one
                if (!ExploreVertexPredicate(clone))
                    continue;

                foreach (ITransitionFactory<TVertex, TEdge> transitionFactory in TransitionFactories)
                {
                    GenerateFromTransitionFactory(clone, transitionFactory);
                }
            }

            finishedSuccessfully = true;
        }

        private void GenerateFromTransitionFactory(
            TVertex current,
            ITransitionFactory<TVertex, TEdge> transitionFactory
            )
        {
            if (!transitionFactory.IsValid(current))
                return;

            foreach (var transition in transitionFactory.Apply(current))
            {
                if (    
                    !AddVertexPredicate(transition.Target)
                 || !AddEdgePredicate(transition))
                {
                    OnEdgeSkipped(transition);
                    continue;
                }

                bool backEdge = VisitedGraph.ContainsVertex(transition.Target);
                if (!backEdge)
                    OnDiscoverVertex(transition.Target);

                VisitedGraph.AddEdge(transition);
                if (backEdge)
                    OnBackEdge(transition);
                else
                    OnTreeEdge(transition);
            }
        }

        public sealed class DefaultFinishedPredicate
        {
            private int maxVertexCount = 1000;
            private int maxEdgeCount = 1000;

            public DefaultFinishedPredicate()
            { }

            public DefaultFinishedPredicate(
                int maxVertexCount,
                int maxEdgeCount)
            {
                this.maxVertexCount = maxVertexCount;
                this.maxEdgeCount = maxEdgeCount;
            }

            public int MaxVertexCount
            {
                get => maxVertexCount;
                set => maxVertexCount = value;
            }

            public int MaxEdgeCount
            {
                get => maxEdgeCount;
                set => maxEdgeCount = value;
            }

            public bool Test(CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge> t)
            {
                if (t.VisitedGraph.VertexCount > MaxVertexCount)
                    return false;
                if (t.VisitedGraph.EdgeCount > MaxEdgeCount)
                    return false;
                return true;
            }
        }
    }
}
#endif