using System;
using System.Collections.Generic;

using QuickGraph.Collections;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.TopologicalSort
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class SourceFirstTopologicalSortAlgorithm<TVertex, TEdge> :
        AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private IDictionary<TVertex, int> inDegrees = new Dictionary<TVertex, int>();
        private BinaryQueue<TVertex,int> heap;
        private IList<TVertex> sortedVertices = new List<TVertex>();

        public SourceFirstTopologicalSortAlgorithm(
            IVertexAndEdgeListGraph<TVertex,TEdge> visitedGraph
            )
            :base(visitedGraph)
        {
            heap = new BinaryQueue<TVertex,int>(e => inDegrees[e]);
        }

        public ICollection<TVertex> SortedVertices
        {
            get
            {
                return sortedVertices;
            }
        }

        public BinaryQueue<TVertex,int> Heap
        {
            get
            {
                return heap;
            }
        }

        public IDictionary<TVertex,int> InDegrees
        {
            get
            {
                return inDegrees;
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
                if (inDegrees[v] != 0)
                    throw new NonAcyclicGraphException();

                sortedVertices.Add(v);
                OnAddVertex(v);

                // update the count of it's adjacent vertices
                foreach (var e in VisitedGraph.OutEdges(v))
                {
                    if (e.Source.Equals(e.Target))
                        continue;

                    inDegrees[e.Target]--;
                    Contract.Assert(inDegrees[e.Target] >= 0);
                    heap.Update(e.Target);
                }
            }
        }

        private void InitializeInDegrees()
        {
            foreach (var v in VisitedGraph.Vertices)
            {
                inDegrees.Add(v, 0);         
            }

            foreach (var e in VisitedGraph.Edges)
            {
                if (e.Source.Equals(e.Target))
                    continue;
                inDegrees[e.Target]++;
            }

            foreach (var v in VisitedGraph.Vertices)
            {
                heap.Enqueue(v);
            }

        }
    }
}
