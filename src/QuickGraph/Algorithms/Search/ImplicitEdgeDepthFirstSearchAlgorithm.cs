using System;
using System.Collections.Generic;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Search
{
    /// <summary>
    /// A edge depth first search algorithm for implicit directed graphs
    /// </summary>
    /// <remarks>
    /// This is a variant of the classic DFS where the edges are color
    /// marked.
    /// </remarks>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2"
    ///     />
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class ImplicitEdgeDepthFirstSearchAlgorithm<TVertex, TEdge> :
        RootedAlgorithmBase<TVertex,IIncidenceGraph<TVertex,TEdge>>,
        ITreeBuilderAlgorithm<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        private int maxDepth = int.MaxValue;
        private IDictionary<TEdge,GraphColor> edgeColors = new Dictionary<TEdge,GraphColor>();

        public ImplicitEdgeDepthFirstSearchAlgorithm(IIncidenceGraph<TVertex, TEdge> visitedGraph)
            : this(null, visitedGraph)
        { }

        public ImplicitEdgeDepthFirstSearchAlgorithm(
            IAlgorithmComponent host,
            IIncidenceGraph<TVertex,TEdge> visitedGraph
            )
            :base(host, visitedGraph)
        {}

        /// <summary>
        /// Gets the vertex color map
        /// </summary>
        /// <value>
        /// Vertex color (<see cref="GraphColor"/>) dictionary
        /// </value>
        public IDictionary<TEdge, GraphColor> EdgeColors => edgeColors;

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
        /// Triggers the StartVertex event.
        /// </summary>
        /// <param name="v"></param>
        private void OnStartVertex(TVertex v)
        {
            var eh = StartVertex;
            if (eh != null)
                eh(v);
        }

        /// <summary>
        /// Invoked on the first edge of a test case
        /// </summary>
        public event EdgeAction<TVertex,TEdge> StartEdge;

        /// <summary>
        /// Triggers the StartEdge event.
        /// </summary>
        /// <param name="e"></param>
        private void OnStartEdge(TEdge e)
        {
            var eh = StartEdge;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EdgeEdgeAction<TVertex, TEdge> DiscoverTreeEdge;

        /// <summary>
        /// Triggers DiscoverEdge event
        /// </summary>
        /// <param name="se"></param>
        /// <param name="e"></param>
        private void OnDiscoverTreeEdge(TEdge se, TEdge e)
        {
            var eh = DiscoverTreeEdge;
            if (eh != null)
                eh(se, e);
        }

        /// <summary>
        /// Invoked on each edge as it becomes a member of the edges that form 
        /// the search tree. If you wish to record predecessors, do so at this 
        /// event point. 
        /// </summary>
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        /// <summary>
        /// Triggers the TreeEdge event.
        /// </summary>
        /// <param name="e"></param>
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
        /// Triggers the BackEdge event.
        /// </summary>
        /// <param name="e"></param>
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
        /// Triggers the ForwardOrCrossEdge event.
        /// </summary>
        /// <param name="e"></param>
        private void OnForwardOrCrossEdge(TEdge e)
        {
            var eh = ForwardOrCrossEdge;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// Invoked on a edge after all of its out edges have been added to 
        /// the search tree and all of the adjacent vertices have been 
        /// discovered (but before their out-edges have been examined). 
        /// </summary>
        public event EdgeAction<TVertex, TEdge> FinishEdge;

        /// <summary>
        /// Triggers the ForwardOrCrossEdge event.
        /// </summary>
        /// <param name="e"></param>
        private void OnFinishEdge(TEdge e)
        {
            var eh = FinishEdge;
            if (eh != null)
                eh(e);
        }

        

        protected override void  InternalCompute()
        {
            TVertex rootVertex;
            if (!TryGetRootVertex(out rootVertex))
                throw new InvalidOperationException("root vertex not set");

            // initialize algorithm
            Initialize();

            // start whith him:
            OnStartVertex(rootVertex);

            var cancelManager = Services.CancelManager;
            // process each out edge of v
            foreach (var e in VisitedGraph.OutEdges(rootVertex))
            {
                if (cancelManager.IsCancelling) return;

                if (!EdgeColors.ContainsKey(e))
                {
                    OnStartEdge(e);
                    Visit(e, 0);
                }
            }
        }

        /// <summary>
        /// Does a depth first search on the vertex u
        /// </summary>
        /// <param name="se">edge to explore</param>
        /// <param name="depth">current exploration depth</param>
        /// <exception cref="ArgumentNullException">se cannot be null</exception>
        private void Visit(TEdge se, int depth)
        {            
            Contract.Requires(se != null);
            Contract.Requires(depth >= 0);

            if (depth > maxDepth)
                return;

            // mark edge as gray
            EdgeColors[se] = GraphColor.Gray;
            // add edge to the search tree
            OnTreeEdge(se);

            var cancelManager = Services.CancelManager;
            // iterate over out-edges
            foreach (var e in VisitedGraph.OutEdges(se.Target))
            {
                if (cancelManager.IsCancelling) return;

                // check edge is not explored yet,
                // if not, explore it.
                GraphColor c;
                if (!EdgeColors.TryGetValue(e, out c))
                {
                    OnDiscoverTreeEdge(se, e);
                    Visit(e, depth + 1);
                }
                else
                {
                    if (c == GraphColor.Gray)
                        OnBackEdge(e);
                    else
                        OnForwardOrCrossEdge(e);
                }
            }

            // all out-edges have been explored
            EdgeColors[se] = GraphColor.Black;
            OnFinishEdge(se);
        }

        /// <summary>
        /// Initializes the algorithm before computation.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            EdgeColors.Clear();
        }
   }
}