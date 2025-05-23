﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
    /// <summary>
    /// A distance recorder for directed tree builder algorithms
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class VertexDistanceRecorderObserver<TVertex, TEdge>
        : IObserver<ITreeBuilderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private readonly IDistanceRelaxer distanceRelaxer;
        private readonly Func<TEdge, double> edgeWeights;
        private readonly IDictionary<TVertex, double> distances;

        public VertexDistanceRecorderObserver(Func<TEdge, double> edgeWeights)
            : this(edgeWeights, DistanceRelaxers.EdgeShortestDistance, new Dictionary<TVertex, double>())
        { }

        public VertexDistanceRecorderObserver(
            Func<TEdge, double> edgeWeights,
            IDistanceRelaxer distanceRelaxer,
            IDictionary<TVertex, double> distances)
        {
            Contract.Requires(edgeWeights != null);
            Contract.Requires(distanceRelaxer != null);
            Contract.Requires(distances != null);

            this.edgeWeights = edgeWeights;
            this.distanceRelaxer = distanceRelaxer;
            this.distances = distances;
        }

        public IDistanceRelaxer DistanceRelaxer => distanceRelaxer;

        public Func<TEdge, double> EdgeWeights => edgeWeights;

        public IDictionary<TVertex, double> Distances => distances;

        public IDisposable Attach(ITreeBuilderAlgorithm<TVertex, TEdge> algorithm)
        {
            algorithm.TreeEdge += TreeEdge;
            return new DisposableAction(() => algorithm.TreeEdge -= TreeEdge);
        }

        private void TreeEdge(TEdge edge)
        {
            var source = edge.Source;
            var target = edge.Target;

            double sourceDistance;
            if (!distances.TryGetValue(source, out sourceDistance))
                distances[source] = sourceDistance = distanceRelaxer.InitialDistance;
            distances[target] = DistanceRelaxer.Combine(sourceDistance, edgeWeights(edge));
        }
    }
}
