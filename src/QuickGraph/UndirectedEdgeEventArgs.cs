using System;

namespace QuickGraph
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class UndirectedEdgeEventArgs<TVertex, TEdge>
        : EdgeEventArgs<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly bool reversed;

        public UndirectedEdgeEventArgs(TEdge edge, bool reversed)
            :base(edge)
        {
            this.reversed = reversed;
        }

        public bool Reversed
        {
            get { return reversed; }
        }

        public TVertex Source
        {
            get { return reversed ? Edge.Target : Edge.Source; }
        }

        public TVertex Target
        {
            get { return reversed ? Edge.Source : Edge.Target; }
        }
    }

    public delegate void UndirectedEdgeAction<TVertex, TEdge>(
        object sender,
        UndirectedEdgeEventArgs<TVertex, TEdge> e)
        where TEdge : IEdge<TVertex>;
}
