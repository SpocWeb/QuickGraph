using System;
using System.Collections.Generic;

using QuickGraph.Collections;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.TopologicalSort
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class UndirectedFirstTopologicalSortAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private IDictionary<TVertex, int> degrees = new Dictionary<TVertex, int>();
        private BinaryQueue<TVertex, int> heap;
        private IList<TVertex> sortedVertices = new List<TVertex>();
        private bool allowCyclicGraph = false;

        public UndirectedFirstTopologicalSortAlgorithm(
            IUndirectedGraph<TVertex, TEdge> visitedGraph
            )
            : base(visitedGraph)
        {
            heap = new BinaryQueue<TVertex, int>(e => degrees[e]);
        }

        public ICollection<TVertex> SortedVertices => sortedVertices;

        public BinaryQueue<TVertex, int> Heap => heap;

        public IDictionary<TVertex, int> Degrees => degrees;


        public bool AllowCyclicGraph
        {
            get => allowCyclicGraph;
            set => allowCyclicGraph = value;
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
            InitializeInDegrees();
            var cancelManager = Services.CancelManager;

            while (heap.Count != 0)
            {
                if (cancelManager.IsCancelling) return;

                TVertex v = heap.Dequeue();
                if (degrees[v] != 0 && !AllowCyclicGraph)
                    throw new NonAcyclicGraphException();

                sortedVertices.Add(v);
                OnAddVertex(v);

                // update the count of it's adjacent vertices
                foreach (var e in VisitedGraph.AdjacentEdges(v))
                {
                    if (e.Source.Equals(e.Target))
                        continue;

                    degrees[e.Target]--;
                    if (degrees[e.Target] < 0 && !AllowCyclicGraph)
                        throw new InvalidOperationException("Degree is negative, and cannot be");
                    if (heap.Contains(e.Target))
                        heap.Update(e.Target);
                }
            }
        }

        private void InitializeInDegrees()
        {
            foreach (var v in VisitedGraph.Vertices)
            {
                degrees.Add(v, VisitedGraph.AdjacentDegree(v));
                heap.Enqueue(v);
            }
        }
    }
}
