// <copyright file="EdgeTVertexTest.cs" company="Jonathan de Halleux">Copyright http://quickgraph.codeplex.com/</copyright>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph
{
    /// <summary>This class contains parameterized unit tests for Edge`1</summary>
    [TestClass]
    //[PexClass(typeof(Edge<>))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class EdgeTVertexTest
    {
        /// <summary>Test stub for .ctor(!0, !0)</summary>
        
        public Edge<TVertex> Constructor<TVertex>(TVertex source, TVertex target)
        {
            // TODO: add assertions to method EdgeTVertexTest.Constructor(!!0, !!0)
            Edge<TVertex> target01 = new Edge<TVertex>(source, target);
            return target01;
        }

        /// <summary>Test stub for Source</summary>
        
        public void SourceGet<TVertex>(Edge<TVertex> target)
        {
            // TODO: add assertions to method EdgeTVertexTest.SourceGet(Edge`1<!!0>)
            TVertex result = target.Source;
        }

        /// <summary>Test stub for Target</summary>
        
        public void TargetGet<TVertex>(Edge<TVertex> target)
        {
            // TODO: add assertions to method EdgeTVertexTest.TargetGet(Edge`1<!!0>)
            TVertex result = target.Target;
        }

        /// <summary>Test stub for ToString()</summary>
        
        public string ToString<TVertex>(Edge<TVertex> target)
        {
            // TODO: add assertions to method EdgeTVertexTest.ToString(Edge`1<!!0>)
            string result = target.ToString();
            return result;
        }
    }
}
