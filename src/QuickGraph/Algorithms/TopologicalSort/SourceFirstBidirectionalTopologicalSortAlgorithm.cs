using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickGraph.Collections;

namespace QuickGraph.Algorithms.TopologicalSort
{
    public enum TopologicalSortDirection
    {
        Forward,
        Backward
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private IDictionary<TVertex, int> predCounts = new Dictionary<TVertex, int>();
        private BinaryQueue<TVertex, int> heap;
        private IList<TVertex> sortedVertices = new List<TVertex>();
        private TopologicalSortDirection direction = TopologicalSortDirection.Forward;

        public SourceFirstBidirectionalTopologicalSortAlgorithm(
            IBidirectionalGraph<TVertex, TEdge> visitedGraph
            )
            : this(visitedGraph, TopologicalSortDirection.Forward)
        {
        }

        public SourceFirstBidirectionalTopologicalSortAlgorithm(
            IBidirectionalGraph<TVertex, TEdge> visitedGraph,
            TopologicalSortDirection direction
            )
            : base(visitedGraph)
        {
            this.direction = direction;
            heap = new BinaryQueue<TVertex, int>(e => predCounts[e]);
        }

        public ICollection<TVertex> SortedVertices
        {
            get
            {
                return sortedVertices;
            }
        }

        public BinaryQueue<TVertex, int> Heap
        {
            get
            {
                return heap;
            }
        }

        public IDictionary<TVertex, int> InDegrees
        {
            get
            {
                return predCounts;
            }
        }

        public event VertexAction<TVertex> AddVertex;
        private void OnAddVertex(TVertex v)
        {
            var eh = AddVertex;
            if (eh != null)
                eh(v);
        }

        public void Compute(IList<TVertex> vertices)
        {
            Contract.Requires(vertices != null);

            sortedVertices = vertices;
            Compute();
        }


        protected override void InternalCompute()
        {
            var cancelManager = Services.CancelManager;
            InitializeInDegrees();

            while (heap.Count != 0)
            {
                if (cancelManager.IsCancelling) break;

                TVertex v = heap.Dequeue();
                if (predCounts[v] != 0)
                    throw new NonAcyclicGraphException();

                sortedVertices.Add(v);
                OnAddVertex(v);

                // update the count of its successor vertices
                var succEdges = (direction == TopologicalSortDirection.Forward) ? VisitedGraph.OutEdges(v) : VisitedGraph.InEdges(v);
                foreach (var e in succEdges)
                {
                    if (e.Source.Equals(e.Target))
                        continue;
                    var succ = (direction == TopologicalSortDirection.Forward) ? e.Target : e.Source;
                    predCounts[succ]--;
                    Contract.Assert(predCounts[succ] >= 0);
                    heap.Update(succ);
                }
            }
        }

        private void InitializeInDegrees()
        {
            foreach (var v in VisitedGraph.Vertices)
            {
                predCounts.Add(v, 0);
            }

            foreach (var e in VisitedGraph.Edges)
            {
                if (e.Source.Equals(e.Target))
                    continue;
                var succ = (direction == TopologicalSortDirection.Forward) ? e.Target : e.Source;
                predCounts[succ]++;
            }

            foreach (var v in VisitedGraph.Vertices)
            {
                heap.Enqueue(v);
            }
        }
    }
}
