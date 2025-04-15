using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace QuickGraph.Predicates
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class ReversedResidualEdgePredicate<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly IDictionary<TEdge,double> residualCapacities;
        private readonly IDictionary<TEdge,TEdge> reversedEdges;

        public ReversedResidualEdgePredicate(
            IDictionary<TEdge, double> residualCapacities,
            IDictionary<TEdge, TEdge> reversedEdges)
        {
            Contract.Requires(residualCapacities != null);
            Contract.Requires(reversedEdges != null);
            
            this.residualCapacities = residualCapacities;
            this.reversedEdges = reversedEdges;
        }

        /// <summary>
        /// Residual capacities map
        /// </summary>
        public IDictionary<TEdge,double> ResidualCapacities => residualCapacities;

        /// <summary>
        /// Reversed edges map
        /// </summary>
        public IDictionary<TEdge,TEdge> ReversedEdges => reversedEdges;

        public bool Test(TEdge e)
        {
            Contract.Requires(e != null);
            return 0 < residualCapacities[reversedEdges[e]];
        }
    }
}
