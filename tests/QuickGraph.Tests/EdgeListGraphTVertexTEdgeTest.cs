// <copyright file="EdgeListGraphTVertexTEdgeTest.cs" company="Jonathan de Halleux">Copyright http://quickgraph.codeplex.com/</copyright>
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph
{
    /// <summary>This class contains parameterized unit tests for EdgeListGraph`2</summary>
    [TestClass]
    //[PexClass(typeof(EdgeListGraph<, >))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class EdgeListGraphTVertexTEdgeTest
    {
        /// <summary>Test stub for AddEdge(!1)</summary>
        
        public bool AddEdge<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target, TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.AddEdge(EdgeListGraph`2<!!0,!!1>, !!1)
            bool result = target.AddEdge(edge);
            return result;
        }

        /// <summary>Test stub for AddEdgeRange(IEnumerable`1&lt;!1&gt;)</summary>
        
        public int AddEdgeRange<TVertex,TEdge>(
            EdgeListGraph<TVertex, TEdge> target,
            IEnumerable<TEdge> edges
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.AddEdgeRange(EdgeListGraph`2<!!0,!!1>, IEnumerable`1<!!1>)
            int result = target.AddEdgeRange(edges);
            return result;
        }

        /// <summary>Test stub for AddVerticesAndEdge(!1)</summary>
        
        public bool AddVerticesAndEdge<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target, TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.AddVerticesAndEdge(EdgeListGraph`2<!!0,!!1>, !!1)
            bool result = target.AddVerticesAndEdge(edge);
            return result;
        }

        /// <summary>Test stub for AddVerticesAndEdgeRange(IEnumerable`1&lt;!1&gt;)</summary>
        
        public int AddVerticesAndEdgeRange<TVertex,TEdge>(
            EdgeListGraph<TVertex, TEdge> target,
            IEnumerable<TEdge> edges
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.AddVerticesAndEdgeRange(EdgeListGraph`2<!!0,!!1>, IEnumerable`1<!!1>)
            int result = target.AddVerticesAndEdgeRange(edges);
            return result;
        }

        /// <summary>Test stub for AllowParallelEdges</summary>
        
        public void AllowParallelEdgesGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.AllowParallelEdgesGet(EdgeListGraph`2<!!0,!!1>)
            bool result = target.AllowParallelEdges;
        }

        /// <summary>Test stub for Clear()</summary>
        
        public void Clear<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.Clear(EdgeListGraph`2<!!0,!!1>)
            target.Clear();
        }

        /// <summary>Test stub for Clone()</summary>
        
        public EdgeListGraph<TVertex, TEdge> Clone<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.Clone(EdgeListGraph`2<!!0,!!1>)
            EdgeListGraph<TVertex, TEdge> result = target.Clone();
            return result;
        }

        /// <summary>Test stub for .ctor()</summary>
        
        public EdgeListGraph<TVertex, TEdge> Constructor<TVertex,TEdge>()
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.Constructor()
            EdgeListGraph<TVertex, TEdge> target = new EdgeListGraph<TVertex, TEdge>();
            return target;
        }

        /// <summary>Test stub for .ctor(Boolean, Boolean)</summary>
        
        public EdgeListGraph<TVertex, TEdge> Constructor01<TVertex,TEdge>(bool isDirected, bool allowParralelEdges)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.Constructor01(Boolean, Boolean)
            EdgeListGraph<TVertex, TEdge> target
               = new EdgeListGraph<TVertex, TEdge>(isDirected, allowParralelEdges);
            return target;
        }

        /// <summary>Test stub for ContainsEdge(!1)</summary>
        
        public bool ContainsEdge<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target, TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.ContainsEdge(EdgeListGraph`2<!!0,!!1>, !!1)
            bool result = target.ContainsEdge(edge);
            return result;
        }

        /// <summary>Test stub for ContainsVertex(!0)</summary>
        
        public bool ContainsVertex<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target, TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.ContainsVertex(EdgeListGraph`2<!!0,!!1>, !!0)
            bool result = target.ContainsVertex(vertex);
            return result;
        }

        /// <summary>Test stub for EdgeCount</summary>
        
        public void EdgeCountGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.EdgeCountGet(EdgeListGraph`2<!!0,!!1>)
            int result = target.EdgeCount;
        }

        /// <summary>Test stub for Edges</summary>
        
        public void EdgesGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.EdgesGet(EdgeListGraph`2<!!0,!!1>)
            IEnumerable<TEdge> result = target.Edges;
        }

        /// <summary>Test stub for IsDirected</summary>
        
        public void IsDirectedGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.IsDirectedGet(EdgeListGraph`2<!!0,!!1>)
            bool result = target.IsDirected;
        }

        /// <summary>Test stub for IsEdgesEmpty</summary>
        
        public void IsEdgesEmptyGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.IsEdgesEmptyGet(EdgeListGraph`2<!!0,!!1>)
            bool result = target.IsEdgesEmpty;
        }

        /// <summary>Test stub for IsVerticesEmpty</summary>
        
        public void IsVerticesEmptyGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.IsVerticesEmptyGet(EdgeListGraph`2<!!0,!!1>)
            bool result = target.IsVerticesEmpty;
        }

        /// <summary>Test stub for RemoveEdge(!1)</summary>
        
        public bool RemoveEdge<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target, TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.RemoveEdge(EdgeListGraph`2<!!0,!!1>, !!1)
            bool result = target.RemoveEdge(edge);
            return result;
        }

        /// <summary>Test stub for RemoveEdgeIf(EdgePredicate`2&lt;!0,!1&gt;)</summary>
        
        public int RemoveEdgeIf<TVertex,TEdge>(
            EdgeListGraph<TVertex, TEdge> target,
            EdgePredicate<TVertex, TEdge> predicate
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.RemoveEdgeIf(EdgeListGraph`2<!!0,!!1>, EdgePredicate`2<!!0,!!1>)
            int result = target.RemoveEdgeIf(predicate);
            return result;
        }

        /// <summary>Test stub for VertexCount</summary>
        
        public void VertexCountGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.VertexCountGet(EdgeListGraph`2<!!0,!!1>)
            int result = target.VertexCount;
        }

        /// <summary>Test stub for Vertices</summary>
        
        public void VerticesGet<TVertex,TEdge>(EdgeListGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method EdgeListGraphTVertexTEdgeTest.VerticesGet(EdgeListGraph`2<!!0,!!1>)
            IEnumerable<TVertex> result = target.Vertices;
        }
    }
}
