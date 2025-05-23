// <copyright file="TaggedEdgeTVertexTTagTest.cs" company="Jonathan de Halleux">Copyright http://quickgraph.codeplex.com/</copyright>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph
{
    /// <summary>This class contains parameterized unit tests for TaggedEdge`2</summary>
    [TestClass]
    //[PexClass(typeof(TaggedEdge<, >))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TaggedEdgeTVertexTTagTest
    {
        /// <summary>Test stub for .ctor(!0, !0, !1)</summary>
        
        public TaggedEdge<TVertex, TTag> Constructor<TVertex,TTag>(
            TVertex source,
            TVertex target,
            TTag tag
        )
        {
            // TODO: add assertions to method TaggedEdgeTVertexTTagTest.Constructor(!!0, !!0, !!1)
            TaggedEdge<TVertex, TTag> target01
               = new TaggedEdge<TVertex, TTag>(source, target, tag);
            return target01;
        }

        /// <summary>Test stub for Tag</summary>
        
        public void TagGetSet<TVertex,TTag>(TaggedEdge<TVertex, TTag> target, TTag value)
        {
            // TODO: add assertions to method TaggedEdgeTVertexTTagTest.TagGetSet(TaggedEdge`2<!!0,!!1>)
            target.Tag = value;
            TTag result = target.Tag;
            Assert.AreEqual(value, result);
        }
    }
}
