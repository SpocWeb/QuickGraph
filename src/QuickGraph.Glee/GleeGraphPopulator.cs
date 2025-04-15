using QuickGraph.Algorithms;
using Microsoft.Glee.Drawing;

namespace QuickGraph.Glee
{
    public abstract class GleeGraphPopulator<TVertex,TEdge> :
        AlgorithmBase<IEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        protected GleeGraphPopulator(IEdgeListGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        { }

        private Graph gleeGraph;
        public Graph GleeGraph => gleeGraph;

        #region Events
        public event GleeVertexNodeEventHandler<TVertex> NodeAdded;
        protected virtual void OnNodeAdded(GleeVertexEventArgs<TVertex> e)
        {
            GleeVertexNodeEventHandler<TVertex> eh = NodeAdded;
            if (eh != null)
                eh(this, e);
        }

        public event GleeEdgeEventHandler<TVertex, TEdge> EdgeAdded;
        protected virtual void OnEdgeAdded(GleeEdgeEventArgs<TVertex, TEdge> e)
        {
            var eh = EdgeAdded;
            if (eh != null)
                eh(this, e);
        }
        #endregion

        protected override void InternalCompute()
        {
            gleeGraph = new Graph("");

            foreach (var v in VisitedGraph.Vertices)
            {
                Node node = AddNode(v);
                node.UserData = v;
                OnNodeAdded(new GleeVertexEventArgs<TVertex>(v, node));
            }

            foreach (var e in VisitedGraph.Edges)
            {
                Edge edge = AddEdge(e);
                edge.UserData = e;
                OnEdgeAdded(new GleeEdgeEventArgs<TVertex,TEdge>(e, edge));
            }
        }

        protected abstract Node AddNode(TVertex v);

        protected abstract Edge AddEdge(TEdge e);
    }
}
