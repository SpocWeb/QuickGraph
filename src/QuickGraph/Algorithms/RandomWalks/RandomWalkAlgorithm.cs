﻿using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.RandomWalks
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class RandomWalkAlgorithm<TVertex, TEdge> 
        : ITreeBuilderAlgorithm<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        private IImplicitGraph<TVertex,TEdge> visitedGraph;
        private EdgePredicate<TVertex,TEdge> endPredicate;
        private IEdgeChain<TVertex,TEdge> edgeChain;

        public RandomWalkAlgorithm(IImplicitGraph<TVertex,TEdge> visitedGraph)
            :this(visitedGraph,new NormalizedMarkovEdgeChain<TVertex,TEdge>())
        {}

        public RandomWalkAlgorithm(
            IImplicitGraph<TVertex,TEdge> visitedGraph,
            IEdgeChain<TVertex,TEdge> edgeChain
            )
        {
            Contract.Requires(visitedGraph != null);
            Contract.Requires(edgeChain != null);

            this.visitedGraph = visitedGraph;
            this.edgeChain = edgeChain;
        }

        public IImplicitGraph<TVertex,TEdge> VisitedGraph => visitedGraph;

        public IEdgeChain<TVertex,TEdge> EdgeChain
        {
            get => edgeChain;
            set
            {
                Contract.Requires(value != null);

                edgeChain = value;
            }
        }

        public EdgePredicate<TVertex,TEdge> EndPredicate
        {
            get => endPredicate;
            set => endPredicate = value;
        }

        public event VertexAction<TVertex> StartVertex;
        private void OnStartVertex(TVertex v)
        {
            var eh = StartVertex;
            if (eh != null)
                eh(v);
        }

        public event VertexAction<TVertex> EndVertex;
        private void OnEndVertex(TVertex v)
        {
            var eh = EndVertex;
            if (eh != null)
                eh(v);
        }

        public event EdgeAction<TVertex,TEdge> TreeEdge;
        private void OnTreeEdge(TEdge e)
        {
            var eh = TreeEdge;
            if (eh != null)
                eh(e);
        }

        private bool TryGetSuccessor(TVertex u, out TEdge successor)
        {
            return EdgeChain.TryGetSuccessor(VisitedGraph, u, out successor);
        }

        public void Generate(TVertex root)
        {
            Contract.Requires(root != null);

            Generate(root, 100);
        }

        public void Generate(TVertex root, int walkCount)
        {
            Contract.Requires(root != null);

            int count = 0;
            TEdge e = default(TEdge);
            TVertex v = root;

            OnStartVertex(root);
            while (count < walkCount && TryGetSuccessor(v, out e))
            {
                // if dead end stop
                if (e==null)
                    break;
                // if end predicate, test
                if (endPredicate != null && endPredicate(e))
                    break;
                OnTreeEdge(e);
                v = e.Target;
                // upgrade count
                ++count;
            }
            OnEndVertex(v);
        }

    }
}
