// <copyright file="AdjacencyGraphTVertexTEdgeTest.cs" company="Jonathan de Halleux">Copyright http://quickgraph.codeplex.com/</copyright>
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph
{
    /// <summary>This class contains parameterized unit tests for AdjacencyGraph`2</summary>
    [TestClass]
    [Ignore("Explicit test - run manually and individually")]
    //[PexClass(typeof(AdjacencyGraph<, >))]
    //[PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
    //[PexGenericArguments(typeof(int), typeof(Edge<int>))]
    public partial class AdjacencyGraphTVertexTEdgeTest
    {
        /// <summary>Test stub for AddEdge(!1)</summary>
        
        public static bool AddEdge<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TEdge e)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.AddEdge(AdjacencyGraph`2<!!0,!!1>, !!1)
            try
            {
                bool result = target.AddEdge(e);
                return result;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
        }

        /// <summary>Test stub for AddEdgeRange(IEnumerable`1&lt;!1&gt;)</summary>
        
        public int AddEdgeRange<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target,
            TEdge[] edges
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.AddEdgeRange(AdjacencyGraph`2<!!0,!!1>, !!1[])
            int result = target.AddEdgeRange((IEnumerable<TEdge>)edges);
            return result;
        }

        /// <summary>Test stub for AddVertex(!0)</summary>
        
        public bool AddVertex<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TVertex v)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.AddVertex(AdjacencyGraph`2<!!0,!!1>, !!0)
            bool result = target.AddVertex(v);
            return result;
        }

        /// <summary>Test stub for AddVertexRange(IEnumerable`1&lt;!0&gt;)</summary>
        
        public int AddVertexRange<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target,
            TVertex[] vertices
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.AddVertexRange(AdjacencyGraph`2<!!0,!!1>, !!0[])
            int result = target.AddVertexRange((IEnumerable<TVertex>)vertices);
            return result;
        }

        /// <summary>Test stub for AddVerticesAndEdge(!1)</summary>
        public bool AddVerticesAndEdge<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TEdge e)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.AddVerticesAndEdge(AdjacencyGraph`2<!!0,!!1>, !!1)
            bool result = target.AddVerticesAndEdge(e);
            return result;
        }

        /// <summary>Test stub for AddVerticesAndEdgeRange(IEnumerable`1&lt;!1&gt;)</summary>
        
        public int AddVerticesAndEdgeRange<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target,
            TEdge[] edges
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.AddVerticesAndEdgeRange(AdjacencyGraph`2<!!0,!!1>, !!1[])
            int result = target.AddVerticesAndEdgeRange((IEnumerable<TEdge>)edges);
            return result;
        }

        /// <summary>Test stub for AllowParallelEdges</summary>
        public void AllowParallelEdgesGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.AllowParallelEdgesGet(AdjacencyGraph`2<!!0,!!1>)
            bool result = target.AllowParallelEdges;
        }

        /// <summary>Test stub for Clear()</summary>
        
        public void Clear<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.Clear(AdjacencyGraph`2<!!0,!!1>)
            target.Clear();
        }

        /// <summary>Test stub for ClearOutEdges(!0)</summary>
        
        public void ClearOutEdges<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TVertex v)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.ClearOutEdges(AdjacencyGraph`2<!!0,!!1>, !!0)
            target.ClearOutEdges(v);
        }

        /// <summary>Test stub for Clone()</summary>
        
        public AdjacencyGraph<TVertex, TEdge> Clone<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.Clone(AdjacencyGraph`2<!!0,!!1>)
            AdjacencyGraph<TVertex, TEdge> result = target.Clone();
            return result;
        }

        /// <summary>Test stub for .ctor()</summary>
        
        public AdjacencyGraph<TVertex, TEdge> Constructor<TVertex,TEdge>()
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.Constructor()
            AdjacencyGraph<TVertex, TEdge> target = new AdjacencyGraph<TVertex, TEdge>();
            return target;
        }

        /// <summary>Test stub for .ctor(Boolean)</summary>
        
        public AdjacencyGraph<TVertex, TEdge> Constructor01<TVertex,TEdge>(bool allowParallelEdges)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.Constructor01(Boolean)
            AdjacencyGraph<TVertex, TEdge> target
               = new AdjacencyGraph<TVertex, TEdge>(allowParallelEdges);
            return target;
        }

        /// <summary>Test stub for .ctor(Boolean, Int32)</summary>
        
        public AdjacencyGraph<TVertex, TEdge> Constructor02<TVertex,TEdge>(bool allowParallelEdges, int capacity)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.Constructor02(Boolean, Int32)
            AdjacencyGraph<TVertex, TEdge> target
               = new AdjacencyGraph<TVertex, TEdge>(allowParallelEdges, capacity);
            return target;
        }

        /// <summary>Test stub for .ctor(Boolean, Int32, Int32)</summary>
        
        public AdjacencyGraph<TVertex, TEdge> Constructor03<TVertex,TEdge>(
            bool allowParallelEdges,
            int capacity,
            int edgeCapacity
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.Constructor03(Boolean, Int32, Int32)
            AdjacencyGraph<TVertex, TEdge> target = new AdjacencyGraph<TVertex, TEdge>
                (allowParallelEdges, capacity, edgeCapacity);
            return target;
        }

        /// <summary>Test stub for ContainsEdge(!0, !0)</summary>
        
        public bool ContainsEdge<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target01,
            TVertex source,
            TVertex target
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.ContainsEdge(AdjacencyGraph`2<!!0,!!1>, !!0, !!0)
            bool result = target01.ContainsEdge(source, target);
            return result;
        }

        /// <summary>Test stub for ContainsEdge(!1)</summary>
        
        public bool ContainsEdge01<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TEdge edge)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.ContainsEdge01(AdjacencyGraph`2<!!0,!!1>, !!1)
            bool result = target.ContainsEdge(edge);
            return result;
        }

        /// <summary>Test stub for ContainsVertex(!0)</summary>
        
        public bool ContainsVertex<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TVertex v)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.ContainsVertex(AdjacencyGraph`2<!!0,!!1>, !!0)
            bool result = target.ContainsVertex(v);
            return result;
        }

        /// <summary>Test stub for EdgeCapacity</summary>
        
        public void EdgeCapacityGetSet<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target,
            int value
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.EdgeCapacityGetSet(AdjacencyGraph`2<!!0,!!1>)
            target.EdgeCapacity = value;
            int result = target.EdgeCapacity;
            Assert.AreEqual((object)value, (object)result);
        }

        /// <summary>Test stub for EdgeCount</summary>
        
        public void EdgeCountGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.EdgeCountGet(AdjacencyGraph`2<!!0,!!1>)
            int result = target.EdgeCount;
        }

        /// <summary>Test stub for EdgeType</summary>
        
        public void EdgeTypeGet<TVertex,TEdge>()
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.EdgeTypeGet()
            Type result = AdjacencyGraph<TVertex, TEdge>.EdgeType;
        }

        /// <summary>Test stub for Edges</summary>
        
        public void EdgesGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.EdgesGet(AdjacencyGraph`2<!!0,!!1>)
            IEnumerable<TEdge> result = target.Edges;
        }

        /// <summary>Test stub for IsDirected</summary>
        
        public void IsDirectedGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.IsDirectedGet(AdjacencyGraph`2<!!0,!!1>)
            bool result = target.IsDirected;
        }

        /// <summary>Test stub for IsEdgesEmpty</summary>
        
        public void IsEdgesEmptyGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.IsEdgesEmptyGet(AdjacencyGraph`2<!!0,!!1>)
            bool result = target.IsEdgesEmpty;
        }

        /// <summary>Test stub for IsOutEdgesEmpty(!0)</summary>
        
        public bool IsOutEdgesEmpty<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TVertex v)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.IsOutEdgesEmpty(AdjacencyGraph`2<!!0,!!1>, !!0)
            bool result = target.IsOutEdgesEmpty(v);
            return result;
        }

        /// <summary>Test stub for IsVerticesEmpty</summary>
        
        public void IsVerticesEmptyGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.IsVerticesEmptyGet(AdjacencyGraph`2<!!0,!!1>)
            bool result = target.IsVerticesEmpty;
        }

        /// <summary>Test stub for OutDegree(!0)</summary>
        
        public int OutDegree<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TVertex v)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.OutDegree(AdjacencyGraph`2<!!0,!!1>, !!0)
            int result = target.OutDegree(v);
            return result;
        }

        /// <summary>Test stub for OutEdge(!0, Int32)</summary>
        
        public TEdge OutEdge<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target,
            TVertex v,
            int index
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.OutEdge(AdjacencyGraph`2<!!0,!!1>, !!0, Int32)
            TEdge result = target.OutEdge(v, index);
            return result;
        }

        /// <summary>Test stub for OutEdges(!0)</summary>
        
        public IEnumerable<TEdge> OutEdges<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TVertex v)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.OutEdges(AdjacencyGraph`2<!!0,!!1>, !!0)
            IEnumerable<TEdge> result = target.OutEdges(v);
            return result;
        }

        /// <summary>Test stub for RemoveEdge(!1)</summary>
        
        public bool RemoveEdge<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TEdge e)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.RemoveEdge(AdjacencyGraph`2<!!0,!!1>, !!1)
            bool result = target.RemoveEdge(e);
            return result;
        }

        /// <summary>Test stub for RemoveVertex(!0)</summary>
        
        public bool RemoveVertex<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target, TVertex v)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.RemoveVertex(AdjacencyGraph`2<!!0,!!1>, !!0)
            bool result = target.RemoveVertex(v);
            return result;
        }

        /// <summary>Test stub for TrimEdgeExcess()</summary>
        
        public void TrimEdgeExcess<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.TrimEdgeExcess(AdjacencyGraph`2<!!0,!!1>)
            target.TrimEdgeExcess();
        }

        /// <summary>Test stub for TryGetEdge(!0, !0, !1&amp;)</summary>
        
        public bool TryGetEdge<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target01,
            TVertex source,
            TVertex target,
            out TEdge edge
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.TryGetEdge(AdjacencyGraph`2<!!0,!!1>, !!0, !!0, !!1&)
            bool result = target01.TryGetEdge(source, target, out edge);
            return result;
        }

        /// <summary>Test stub for TryGetEdges(!0, !0, IEnumerable`1&lt;!1&gt;&amp;)</summary>
        
        public bool TryGetEdges<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target01,
            TVertex source,
            TVertex target,
            out IEnumerable<TEdge> edges
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.TryGetEdges(AdjacencyGraph`2<!!0,!!1>, !!0, !!0, IEnumerable`1<!!1>&)
            bool result = target01.TryGetEdges(source, target, out edges);
            return result;
        }

        /// <summary>Test stub for TryGetOutEdges(!0, IEnumerable`1&lt;!1&gt;&amp;)</summary>
        
        public bool TryGetOutEdges<TVertex,TEdge>(
            AdjacencyGraph<TVertex, TEdge> target,
            TVertex v,
            out IEnumerable<TEdge> edges
        )
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.TryGetOutEdges(AdjacencyGraph`2<!!0,!!1>, !!0, IEnumerable`1<!!1>&)
            bool result = target.TryGetOutEdges(v, out edges);
            return result;
        }

        /// <summary>Test stub for VertexCount</summary>
        
        public void VertexCountGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.VertexCountGet(AdjacencyGraph`2<!!0,!!1>)
            int result = target.VertexCount;
        }

        /// <summary>Test stub for Vertices</summary>
        
        public void VerticesGet<TVertex,TEdge>(AdjacencyGraph<TVertex, TEdge> target)
            where TEdge : IEdge<TVertex>
        {
            // TODO: add assertions to method AdjacencyGraphTVertexTEdgeTest.VerticesGet(AdjacencyGraph`2<!!0,!!1>)
            IEnumerable<TVertex> result = target.Vertices;
        }
    }
}
