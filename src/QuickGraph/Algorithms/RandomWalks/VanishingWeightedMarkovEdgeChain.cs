﻿using System;
using System.Collections.Generic;
namespace QuickGraph.Algorithms.RandomWalks
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class VanishingWeightedMarkovEdgeChain<TVertex, TEdge> :
        WeightedMarkovEdgeChainBase<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
		private double factor;

		public VanishingWeightedMarkovEdgeChain(IDictionary<TEdge,double> weights)
            :this(weights,0.2)
        {}

		public VanishingWeightedMarkovEdgeChain(IDictionary<TEdge,double> weights, double factor)
			:base(weights)
		{
			this.factor = factor;
		}

		public double Factor
		{
			get => factor;
            set => factor = value;
        }

        public override bool TryGetSuccessor(IImplicitGraph<TVertex,TEdge> g, TVertex u, out TEdge successor)
        {
            if (!g.IsOutEdgesEmpty(u))
            {
                // get outweight
                double outWeight = GetOutWeight(g, u);
                // get succesor
                TEdge s;
                if (TryGetSuccessor(g, u, Rand.NextDouble() * outWeight, out s))
                {
                    // update probabilities
                    Weights[s] *= Factor;

                    // normalize
                    foreach (TEdge e in g.OutEdges(u))
                    {
                        checked
                        {
                            Weights[e] /= outWeight;
                        }
                    }

                    successor = s;
                    return true;
                }
            }

            successor = default(TEdge);
            return false;
		}

        public override bool TryGetSuccessor(IEnumerable<TEdge> edges, TVertex u, out TEdge successor)
        {
            // get outweight
            double outWeight = GetWeights(edges);
            // get succesor
            TEdge s;
            if (TryGetSuccessor(edges, Rand.NextDouble() * outWeight, out s))
            {
                // update probabilities
                Weights[s] *= Factor;

                // normalize
                foreach (var e in edges)
                {
                    checked
                    {
                        Weights[e] /= outWeight;
                    }
                }


                successor = s;
                return true;
            }

            successor = default(TEdge);
            return false;
        }
    }
}
