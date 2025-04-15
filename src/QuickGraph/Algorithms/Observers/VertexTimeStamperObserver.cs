using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <reference-ref
    ///     idref="boost"
    ///     />
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class VertexTimeStamperObserver<TVertex, TEdge> :
        IObserver<IVertexTimeStamperAlgorithm<TVertex>>
        where TEdge : IEdge<TVertex>
    {
        private readonly Dictionary<TVertex, int> discoverTimes;
        private readonly Dictionary<TVertex, int> _finishTimes;
        private int currentTime = 0;


        public VertexTimeStamperObserver()
            :this(new Dictionary<TVertex,int>(), new Dictionary<TVertex,int>())
        {}

        public VertexTimeStamperObserver(Dictionary<TVertex, int> discoverTimes)
        {
            Contract.Requires(discoverTimes != null);

            this.discoverTimes = discoverTimes;
            _finishTimes = null;
        }

        public VertexTimeStamperObserver(
            Dictionary<TVertex, int> discoverTimes,
            Dictionary<TVertex, int> finishTimes)
        {
            Contract.Requires(discoverTimes != null);
            Contract.Requires(finishTimes != null);

            this.discoverTimes = discoverTimes;
            _finishTimes = finishTimes;
        }

        public IDictionary<TVertex, int> DiscoverTimes => discoverTimes;

        public IDictionary<TVertex, int> FinishTimes => _finishTimes;

        public IDisposable Attach(IVertexTimeStamperAlgorithm<TVertex> algorithm)
        {
            algorithm.DiscoverVertex += DiscoverVertex;
            if (_finishTimes != null)
                algorithm.FinishVertex += FinishVertex;

            return new DisposableAction(
                () =>
                {
                    algorithm.DiscoverVertex -= DiscoverVertex;
                    if (_finishTimes != null)
                        algorithm.FinishVertex -= FinishVertex;
                });
        }

        void DiscoverVertex(TVertex v)
        {
            discoverTimes[v] = currentTime++;
        }

        void FinishVertex(TVertex v)
        {
            _finishTimes[v] = currentTime++;
        }
    }
}
