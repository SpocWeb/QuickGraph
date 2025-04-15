using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph.Algorithms.RankedShortestPath
{
    public abstract class RankedShortestPathAlgorithmBase<TVertex, TEdge, TGraph>
        : RootedAlgorithmBase<TVertex, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IGraph<TVertex, TEdge>
    {
        private readonly IDistanceRelaxer distanceRelaxer;
        private int shortestPathCount = 3;
        private List<IEnumerable<TEdge>> computedShortestPaths;

        public int ShortestPathCount
        {
            get { return shortestPathCount; }
            set
            {
                Contract.Requires(value > 1);
                Contract.Ensures(ShortestPathCount == value);

                shortestPathCount = value;
            }
        }

        public int ComputedShortestPathCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == ComputedShortestPaths.Count());

                return computedShortestPaths == null ? 0 : computedShortestPaths.Count;
            }
        }

        public IEnumerable<IEnumerable<TEdge>> ComputedShortestPaths
        {
            get
            {
                if (computedShortestPaths == null)
                    yield break;
                else
                    foreach (var path in computedShortestPaths)
                        yield return path;
            }
        }

        protected void AddComputedShortestPath(List<TEdge> path)
        {
            Contract.Requires(path != null);
            Contract.Requires(path.All(e => e != null));

            var pathArray = path.ToArray();
            computedShortestPaths.Add(pathArray);
            Console.WriteLine("found shortest path {0}", path.Count);
        }

        public IDistanceRelaxer DistanceRelaxer
        {
            get { return distanceRelaxer; }
        }

        protected RankedShortestPathAlgorithmBase(
            IAlgorithmComponent host, 
            TGraph visitedGraph,
            IDistanceRelaxer distanceRelaxer)
            : base(host, visitedGraph)
        {
            Contract.Requires(distanceRelaxer != null);

            this.distanceRelaxer = distanceRelaxer;
        }

        protected override void Initialize()
        {
            base.Initialize();
            computedShortestPaths = new List<IEnumerable<TEdge>>(ShortestPathCount);
        }
    }
}
