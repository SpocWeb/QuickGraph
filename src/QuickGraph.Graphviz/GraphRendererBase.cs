using QuickGraph.Graphviz.Dot;

namespace QuickGraph.Graphviz
{
    public abstract class GraphRendererBase<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly GraphvizAlgorithm<TVertex, TEdge> graphviz;

        public GraphRendererBase(
            IEdgeListGraph<TVertex, TEdge> visitedGraph)
        {
            graphviz = new GraphvizAlgorithm<TVertex, TEdge>(visitedGraph);
            Initialize();
        }

        protected virtual void Initialize()        
        {
            graphviz.CommonVertexFormat.Style = GraphvizVertexStyle.Filled;
            graphviz.CommonVertexFormat.FillColor = System.Drawing.Color.LightYellow;
            graphviz.CommonVertexFormat.Font = new System.Drawing.Font("Tahoma", 8.25F);
            graphviz.CommonVertexFormat.Shape = GraphvizVertexShape.Box;

            graphviz.CommonEdgeFormat.Font = new System.Drawing.Font("Tahoma", 8.25F);
        }

        public GraphvizAlgorithm<TVertex, TEdge> Graphviz
        {
            get { return graphviz; }
        }

        public IEdgeListGraph<TVertex, TEdge> VisitedGraph
        {
            get { return graphviz.VisitedGraph; }
        }

        public string Generate(IDotEngine dot, string fileName)
        {
            return graphviz.Generate(dot, fileName);
        }
    }
}
