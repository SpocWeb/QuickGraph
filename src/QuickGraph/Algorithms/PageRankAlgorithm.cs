using System;
using System.Collections.Generic;
using QuickGraph.Predicates;

namespace QuickGraph.Algorithms.Ranking
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class PageRankAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private IDictionary<TVertex,double> ranks = new Dictionary<TVertex,double>();

        private int maxIterations = 60;
        private double tolerance = 2 * double.Epsilon;
        private double damping = 0.85;

        public PageRankAlgorithm(IBidirectionalGraph<TVertex, TEdge> visitedGraph)
            :base(visitedGraph)
        {}

        public IDictionary<TVertex,double> Ranks
        {
            get
            {
                return ranks;
            }
        }

        public double Damping
        {
            get
            {
                return damping;
            }
            set
            {
                damping = value;
            }
        }

        public double Tolerance
        {
            get
            {
                return tolerance;
            }
            set
            {
                tolerance = value;
            }
        }

        public int MaxIteration
        {
            get
            {
                return maxIterations;
            }
            set
            {
                maxIterations = value;
            }
        }

        public void InitializeRanks()
        {
            ranks.Clear();
            foreach (var v in VisitedGraph.Vertices)
            {
                ranks.Add(v, 0);
            }
//            this.RemoveDanglingLinks();
        }
/*
        public void RemoveDanglingLinks()
        {
            VertexCollection danglings = new VertexCollection();
            do
            {
                danglings.Clear();

                // create filtered graph
                IVertexListGraph fg = new FilteredVertexListGraph(
                    this.VisitedGraph,
                    new InDictionaryVertexPredicate(this.ranks)
                    );

                // iterate over of the vertices in the rank map
                foreach (IVertex v in this.ranks.Keys)
                {
                    // if v does not have out-edge in the filtered graph, remove
                    if (fg.OutDegree(v) == 0)
                        danglings.Add(v);
                }

                // remove from ranks
                foreach (IVertex v in danglings)
                    this.ranks.Remove(v);
                // iterate until no dangling was removed
            } while (danglings.Count != 0);
        }
*/
        protected override void InternalCompute()
        {
            var cancelManager = Services.CancelManager;
            IDictionary<TVertex, double> tempRanks = new Dictionary<TVertex, double>();

            // create filtered graph
            FilteredBidirectionalGraph<
                TVertex,
                TEdge,
                IBidirectionalGraph<TVertex,TEdge>
                > fg = new FilteredBidirectionalGraph<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                VisitedGraph,
                new InDictionaryVertexPredicate<TVertex,double>(ranks).Test,
                e => true
                );

            int iter = 0;
            double error = 0;
            do
            {
                if (cancelManager.IsCancelling)
                    return;
                  
                // compute page ranks
                error = 0;
                foreach (KeyValuePair<TVertex,double> de in Ranks)
                {
                    if (cancelManager.IsCancelling)
                        return;

                    TVertex v = de.Key;
                    double rank = de.Value;
                    // compute ARi
                    double r = 0;
                    foreach (var e in fg.InEdges(v))
                    {
                        r += ranks[e.Source] / fg.OutDegree(e.Source);
                    }

                    // add sourceRank and store
                    double newRank = (1 - damping) + damping * r;
                    tempRanks[v] = newRank;
                    // compute deviation
                    error += Math.Abs(rank - newRank);
                }

                // swap ranks
                var temp = ranks;
                ranks = tempRanks;
                tempRanks = temp;

                iter++;
            } while (error > tolerance && iter < maxIterations);
            Console.WriteLine("{0}, {1}", iter, error);
        }

        public double GetRanksSum()
        {
            double sum = 0;
            foreach (double rank in ranks.Values)
            {
                sum += rank;
            }
            return sum;
        }

        public double GetRanksMean()
        {
            return GetRanksSum() / ranks.Count;
        }
    }
}
