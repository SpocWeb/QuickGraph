using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph
{
    [TestClass]
    public partial class UndirectedGraphTest
    {
        
        public static void IsAdjacentEdgesEmpty<T,E>(IUndirectedGraph<T, E> g)
            where E : IEdge<T>
        {
            foreach (T v in g.Vertices)
            {
                Assert.AreEqual(
                    g.IsAdjacentEdgesEmpty(v),
                    g.AdjacentDegree(v) == 0);
            }
        }
    }
}
