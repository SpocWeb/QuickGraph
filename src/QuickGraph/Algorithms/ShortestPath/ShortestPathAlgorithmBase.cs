using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.ShortestPath
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class ShortestPathAlgorithmBase<TVertex, TEdge, TGraph>
        : RootedAlgorithmBase<TVertex,TGraph>
        , ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexSet<TVertex>
    {
        private readonly Func<TEdge, double> weights;
        private readonly IDistanceRelaxer distanceRelaxer;
        private Dictionary<TVertex, GraphColor> vertexColors;
        private Dictionary<TVertex, double> distances;

        protected ShortestPathAlgorithmBase(
            IAlgorithmComponent host,
            TGraph visitedGraph,
            Func<TEdge, double> weights
            )
            :this(host, visitedGraph, weights, DistanceRelaxers.ShortestDistance)
        {}

        protected ShortestPathAlgorithmBase(
            IAlgorithmComponent host,
            TGraph visitedGraph,
            Func<TEdge, double> weights,
            IDistanceRelaxer distanceRelaxer
            )
            :base(host, visitedGraph)
        {
            Contract.Requires(weights != null);
            Contract.Requires(distanceRelaxer != null);

            this.weights = weights;
            this.distanceRelaxer = distanceRelaxer;
        }

        public Dictionary<TVertex, GraphColor> VertexColors => vertexColors;

        public GraphColor GetVertexColor(TVertex vertex)
        {
            Contract.Assert(distances != null);

            return
                vertexColors[vertex];
        }

        public bool TryGetDistance(TVertex vertex, out double distance)
        {
            Contract.Requires(vertex != null);
            Contract.Assert(distances != null);

            return distances.TryGetValue(vertex, out distance);
        }

        public Dictionary<TVertex, double> Distances => distances;

        protected Func<TVertex, double> DistancesIndexGetter()
        {
            return AlgorithmExtensions.GetIndexer(distances);
        }

        public Func<TEdge, double> Weights => weights;

        public IDistanceRelaxer DistanceRelaxer => distanceRelaxer;

        protected override void Initialize()
        {
            base.Initialize();
            vertexColors = new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount);
            distances = new Dictionary<TVertex, double>(VisitedGraph.VertexCount);
        }

        /// <summary>
        /// Invoked when the distance label for the target vertex is decreased. 
        /// The edge that participated in the last relaxation for vertex v is 
        /// an edge in the shortest paths tree.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        /// <summary>
        /// Raises the <see cref="TreeEdge"/> event.
        /// </summary>
        /// <param name="e">edge that raised the event</param>
        protected virtual void OnTreeEdge(TEdge e)
        {
            var eh = TreeEdge;
            if (eh != null)
                eh(e);
        }

        protected bool Relax(TEdge e)
        {
            Contract.Requires(e != null);

            var source = e.Source;
            var target = e.Target;
            double du = distances[source];
            double dv = distances[target];
            double we = Weights(e);

            var relaxer = DistanceRelaxer;
            var duwe = relaxer.Combine(du, we);
            if (relaxer.Compare(duwe, dv) < 0)
            {
                distances[target] = duwe;
                return true;
            }
            else
                return false;
        }
    }
}
