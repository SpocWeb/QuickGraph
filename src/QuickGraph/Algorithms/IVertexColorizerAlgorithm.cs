namespace QuickGraph.Algorithms
{
    public interface IVertexColorizerAlgorithm<TVertex>
    {
        GraphColor GetVertexColor(TVertex v);
    }
}
