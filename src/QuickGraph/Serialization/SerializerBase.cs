namespace QuickGraph.Serialization
{
    public abstract class SerializerBase<TVertex,TEdge>
        where TEdge :IEdge<TVertex>
    {
        public bool EmitDocumentDeclaration {get;set;}
    }
}
