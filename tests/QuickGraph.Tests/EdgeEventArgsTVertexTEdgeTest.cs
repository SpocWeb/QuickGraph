// <copyright file="EdgeEventArgsTVertexTEdgeTest.cs" company="Jonathan de Halleux">Copyright http://quickgraph.codeplex.com/</copyright>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph
{
    /// <summary>This class contains parameterized unit tests for EdgeEventArgs`2</summary>
    [TestClass]
    //[PexClass(typeof(EdgeEventArgs<, >))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class EdgeEventArgsTVertexTEdgeTest
    {
        /// <summary>Test stub for .ctor(!1)</summary>
        
        public static EdgeEventArgs<TVertex, TEdge> Constructor<TVertex,TEdge>(TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeEventArgsTVertexTEdgeTest.Constructor(!!1)
            EdgeEventArgs<TVertex, TEdge> target = new EdgeEventArgs<TVertex, TEdge>(edge);
            return target;
        }

        /// <summary>Test stub for Edge</summary>
        
        public void EdgeGet<TVertex,TEdge>(EdgeEventArgs<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeEventArgsTVertexTEdgeTest.EdgeGet(EdgeEventArgs`2<!!0,!!1>)
            TEdge result = target.Edge;
        }
    }
}
