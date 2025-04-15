namespace QuickGraph.Algorithms
{
    public interface IVertexTimeStamperAlgorithm<TVertex>
    {
        event VertexAction<TVertex> DiscoverVertex;
        event VertexAction<TVertex> FinishVertex;
    }
}
