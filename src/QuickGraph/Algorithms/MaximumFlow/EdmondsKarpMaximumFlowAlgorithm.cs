﻿using System;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Predicates;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.MaximumFlow
{
    /// <summary>
    /// Edmond and Karp maximum flow algorithm for directed graph with positive capacities and flows.
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <remarks>
    /// <remarks>
    /// Will throw an exception in <see cref="ReversedEdgeAugmentorAlgorithm{TVertex, TEdge}.AddReversedEdges"/> if TEdge is a value type,
    /// e.g. <see cref="SEdge{TVertex}"/>.
    /// <seealso href="https://github.com/YaccConstructor/QuickGraph/issues/183#issue-377613647"/>.
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class EdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge>
        : MaximumFlowAlgorithm<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        public EdmondsKarpMaximumFlowAlgorithm(
            IMutableVertexAndEdgeListGraph<TVertex, TEdge> g,
            Func<TEdge, double> capacities,
            EdgeFactory<TVertex, TEdge> edgeFactory,
            ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> reversedEdgeAugmentorAlgorithm
            )
            : this(null, g, capacities, edgeFactory, reversedEdgeAugmentorAlgorithm)
        { }

		public EdmondsKarpMaximumFlowAlgorithm(
            IAlgorithmComponent host,
            IMutableVertexAndEdgeListGraph<TVertex, TEdge> g,
			Func<TEdge,double> capacities,
            EdgeFactory<TVertex, TEdge> edgeFactory,
            ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> reversedEdgeAugmentorAlgorithm

            )
            : base(host, g, capacities, edgeFactory)
		{
		    ReversedEdges = reversedEdgeAugmentorAlgorithm.ReversedEdges;
		}
	
		private IVertexListGraph<TVertex,TEdge> ResidualGraph
		{
			get
			{
				return new FilteredVertexListGraph<
                        TVertex,
                        TEdge,
                        IVertexListGraph<TVertex,TEdge>
                        >(
        					VisitedGraph,
                            v => true,
				        	new ResidualEdgePredicate<TVertex,TEdge>(ResidualCapacities).Test
    					);
			}
		}

	
		private void Augment(
			TVertex source,
			TVertex sink
			)
		{
            Contract.Requires(source != null);
            Contract.Requires(sink != null);

			TEdge e;
			TVertex u;

			// find minimum residual capacity along the augmenting path
			double delta = double.MaxValue;
            u = sink;
            do
			{
                e = Predecessors[u];
                delta = Math.Min(delta, ResidualCapacities[e]);
                u = e.Source;
			} while (!u.Equals(source));

			// push delta units of flow along the augmenting path
            u = sink;
            do 
			{
                e = Predecessors[u];
                ResidualCapacities[e] -= delta;
                if (ReversedEdges != null && ReversedEdges.ContainsKey(e))
                {
                    ResidualCapacities[ReversedEdges[e]] += delta;
                }
				u = e.Source;
			} while (!u.Equals(source));
		}
    
		/// <summary>
		/// Computes the maximum flow between Source and Sink.
		/// </summary>
		/// <returns></returns>
        protected override void InternalCompute()
        {
            if (Source == null)
                throw new InvalidOperationException("Source is not specified");
            if (Sink == null)
                throw new InvalidOperationException("Sink is not specified");


            if (Services.CancelManager.IsCancelling)
                return;

            var g = VisitedGraph;
            foreach (var u in g.Vertices)
                foreach (var e in g.OutEdges(u))
                {
                    var capacity = Capacities(e);
                    if (capacity < 0)
                        throw new InvalidOperationException("negative edge capacity");
                    ResidualCapacities[e] = capacity;
                }

            VertexColors[Sink] = GraphColor.Gray;
            while (VertexColors[Sink] != GraphColor.White)
            {
                var vis = new VertexPredecessorRecorderObserver<TVertex, TEdge>(
                    Predecessors
                    );
                var queue = new Collections.Queue<TVertex>();
                var bfs = new BreadthFirstSearchAlgorithm<TVertex, TEdge>(
                    ResidualGraph,
                    queue,
                    VertexColors
                    );
                using (vis.Attach(bfs))
                    bfs.Compute(Source);

                if (VertexColors[Sink] != GraphColor.White)
                    Augment(Source, Sink);
            } // while

            MaxFlow = 0;
            foreach (var e in g.OutEdges(Source))
                MaxFlow += (Capacities(e) - ResidualCapacities[e]);


           
        }
	}

}