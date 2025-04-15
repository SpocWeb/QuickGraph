#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.Exploration
{
    public sealed class TransitionFactoryImplicitGraph<TVertex,TEdge> 
        : IImplicitGraph<TVertex,TEdge>
        where TVertex : ICloneable
        where TEdge : IEdge<TVertex>
    {
        private readonly VertexEdgeDictionary<TVertex, TEdge> vertedEdges =
            new VertexEdgeDictionary<TVertex, TEdge>();
        private readonly List<ITransitionFactory<TVertex, TEdge>> transitionFactories
            = new List<ITransitionFactory<TVertex, TEdge>>();
        private VertexPredicate<TVertex> successorVertexPredicate
            = v => true;
        private EdgePredicate<TVertex, TEdge> successorEdgePredicate
            = e => true;

        public TransitionFactoryImplicitGraph()
        {}

        public IList<ITransitionFactory<TVertex, TEdge>> TransitionFactories => transitionFactories;

        public VertexPredicate<TVertex> SuccessorVertexPredicate
        {
            get => successorVertexPredicate;
            set => successorVertexPredicate = value;
        }

        public EdgePredicate<TVertex, TEdge> SuccessorEdgePredicate
        {
            get => successorEdgePredicate;
            set => successorEdgePredicate = value;
        }

        [Pure]
        public bool IsOutEdgesEmpty(TVertex v)
        {
            return OutDegree(v) == 0;
        }

        [Pure]
        public int OutDegree(TVertex v)
        {
            int i = 0;
            foreach(TEdge edge in OutEdges(v))
                i++;
            return i;
        }

        [Pure]
        public bool ContainsVertex(TVertex vertex)
        {
            return vertedEdges.ContainsKey(vertex);
        }

        [Pure]
        public IEnumerable<TEdge> OutEdges(TVertex v)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (!vertedEdges.TryGetValue(v, out edges))
            {
                edges = new EdgeList<TVertex, TEdge>();
                foreach (ITransitionFactory<TVertex, TEdge> transitionFactory
                    in TransitionFactories)
                {
                    if (!transitionFactory.IsValid(v))
                        continue;

                    foreach (var edge in transitionFactory.Apply(v))
                    {
                        if (SuccessorVertexPredicate(edge.Target) &&
                            SuccessorEdgePredicate(edge))
                            edges.Add(edge);
                    }
                }
                vertedEdges[v] = edges;
            }
            return edges;
        }

        [Pure]
        public bool TryGetOutEdges(TVertex v, out IEnumerable<TEdge> edges)
        {
            edges = OutEdges(v);
            return true;
        }

        [Pure]
        public TEdge OutEdge(TVertex v, int index)
        {
            int i = 0;
            foreach (var e in OutEdges(v))
                if (i++ == index)
                    return e;
            throw new ArgumentOutOfRangeException("index");
        }

        public bool IsDirected => true;

        public bool AllowParallelEdges => true;
    }
}
#endif