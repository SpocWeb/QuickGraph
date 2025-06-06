using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.Search
{   
    /// <summary>
    /// A depth first search algorithm for implicit directed graphs
    /// </summary>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2"
    ///     />
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class ImplicitDepthFirstSearchAlgorithm<TVertex, TEdge> :
        RootedAlgorithmBase<TVertex,IIncidenceGraph<TVertex,TEdge>>,
        IVertexPredecessorRecorderAlgorithm<TVertex,TEdge>,
        IVertexTimeStamperAlgorithm<TVertex>,
        ITreeBuilderAlgorithm<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        private int maxDepth = int.MaxValue;
        private IDictionary<TVertex, GraphColor> vertexColors = new Dictionary<TVertex, GraphColor>();

        public ImplicitDepthFirstSearchAlgorithm(
            IIncidenceGraph<TVertex, TEdge> visitedGraph)
            : this(null, visitedGraph)
        { }

        public ImplicitDepthFirstSearchAlgorithm(
            IAlgorithmComponent host,
            IIncidenceGraph<TVertex,TEdge> visitedGraph)
            :base(host, visitedGraph)
        {}

        /// <summary>
        /// Gets the vertex color map
        /// </summary>
        /// <value>
        /// Vertex color (<see cref="GraphColor"/>) dictionary
        /// </value>
        public IDictionary<TVertex,GraphColor> VertexColors => vertexColors;

        /// <summary>
        /// Gets or sets the maximum exploration depth, from
        /// the start vertex.
        /// </summary>
        /// <remarks>
        /// Defaulted at <c>int.MaxValue</c>.
        /// </remarks>
        /// <value>
        /// Maximum exploration depth.
        /// </value>
        public int MaxDepth
        {
            get => maxDepth;
            set => maxDepth = value;
        }

        /// <summary>
        /// Invoked on the source vertex once before the start of the search. 
        /// </summary>
        public event VertexAction<TVertex> StartVertex;

        /// <summary>
        /// Raises the <see cref="StartVertex"/> event.
        /// </summary>
        /// <param name="v">vertex that raised the event</param>
        private void OnStartVertex(TVertex v)
        {
            var eh = StartVertex;
            if (eh != null)
                eh(v);
        }

        /// <summary>
        /// Invoked when a vertex is encountered for the first time. 
        /// </summary>
        public event VertexAction<TVertex> DiscoverVertex;


        /// <summary>
        /// Raises the <see cref="DiscoverVertex"/> event.
        /// </summary>
        /// <param name="v">vertex that raised the event</param>
        private void OnDiscoverVertex(TVertex v)
        {
            var eh = DiscoverVertex;
            if (eh != null)
                eh(v);
        }

        /// <summary>
        /// Invoked on every out-edge of each vertex after it is discovered. 
        /// </summary>
        public event EdgeAction<TVertex,TEdge> ExamineEdge;


        /// <summary>
        /// Raises the <see cref="ExamineEdge"/> event.
        /// </summary>
        /// <param name="e">edge that raised the event</param>
        private void OnExamineEdge(TEdge e)
        {
            var eh = ExamineEdge;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// Invoked on each edge as it becomes a member of the edges that form 
        /// the search tree. If you wish to record predecessors, do so at this 
        /// event point. 
        /// </summary>
        public event EdgeAction<TVertex, TEdge> TreeEdge;


        /// <summary>
        /// Raises the <see cref="TreeEdge"/> event.
        /// </summary>
        /// <param name="e">edge that raised the event</param>
        private void OnTreeEdge(TEdge e)
        {
            var eh = TreeEdge;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// Invoked on the back edges in the graph. 
        /// </summary>
        public event EdgeAction<TVertex, TEdge> BackEdge;


        /// <summary>
        /// Raises the <see cref="BackEdge"/> event.
        /// </summary>
        /// <param name="e">edge that raised the event</param>
        private void OnBackEdge(TEdge e)
        {
            var eh = BackEdge;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// Invoked on forward or cross edges in the graph. 
        /// (In an undirected graph this method is never called.) 
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ForwardOrCrossEdge;


        /// <summary>
        /// Raises the <see cref="ForwardOrCrossEdge"/> event.
        /// </summary>
        /// <param name="e">edge that raised the event</param>
        private void OnForwardOrCrossEdge(TEdge e)
        {
            var eh = ForwardOrCrossEdge;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// Invoked on a vertex after all of its out edges have been added to 
        /// the search tree and all of the adjacent vertices have been 
        /// discovered (but before their out-edges have been examined). 
        /// </summary>
        public event VertexAction<TVertex> FinishVertex;

        /// <summary>
        /// Raises the <see cref="FinishVertex"/> event.
        /// </summary>
        /// <param name="v">vertex that raised the event</param>
        private void OnFinishVertex(TVertex v)
        {
            var eh = FinishVertex;
            if (eh != null)
                eh(v);
        }

        protected override void InternalCompute()
        {
            TVertex rootVertex;
            if (!TryGetRootVertex(out rootVertex))
                throw new InvalidOperationException("root vertex not set");

            Initialize();
            Visit(rootVertex, 0);
        }

        protected override void Initialize()
        {
            base.Initialize();

            VertexColors.Clear();
        }

        private void Visit(TVertex u, int depth)
        {
            if (depth > MaxDepth)
                return;

            VertexColors[u] = GraphColor.Gray;
            OnDiscoverVertex(u);

            var cancelManager = Services.CancelManager;
            foreach (var e in VisitedGraph.OutEdges(u))
            {
                if (cancelManager.IsCancelling) return;

                OnExamineEdge(e);
                TVertex v = e.Target;

                GraphColor c;
                if (!VertexColors.TryGetValue(v, out c))
                {
                    OnTreeEdge(e);
                    Visit(v, depth + 1);
                }
                else
                {
                    if (c == GraphColor.Gray)
                    {
                        OnBackEdge(e);
                    }
                    else
                    {
                        OnForwardOrCrossEdge(e);
                    }
                }
            }

            VertexColors[u] = GraphColor.Black;
            OnFinishVertex(u);
        }
   }
}