﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Predicates
{
    public sealed class ResidualEdgePredicate<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
		private readonly IDictionary<TEdge,double> residualCapacities;

        public ResidualEdgePredicate(
            IDictionary<TEdge,double> residualCapacities)
		{
            Contract.Requires(residualCapacities != null);

            this.residualCapacities = residualCapacities;
		}

		public IDictionary<TEdge,double> ResidualCapacities => residualCapacities;

        public bool Test(TEdge e)
		{
            Contract.Requires(e != null);
			return 0 < residualCapacities[e];
		}
    }
}
