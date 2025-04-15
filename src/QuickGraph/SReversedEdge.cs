using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace QuickGraph
{
    /// <summary>
    /// A reversed edge
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("{Source}<-{Target}")]
    public struct SReversedEdge<TVertex, TEdge> 
        : IEdge<TVertex>
        , IEquatable<SReversedEdge<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private readonly TEdge originalEdge;
        public SReversedEdge(TEdge originalEdge)
        {
            Contract.Requires(originalEdge != null);

            this.originalEdge = originalEdge;
        }

        public TEdge OriginalEdge => originalEdge;

        public TVertex Source => OriginalEdge.Target;

        public TVertex Target => OriginalEdge.Source;

        [Pure]
        public override bool Equals(object obj)
        {
            if (!(obj is SReversedEdge<TVertex, TEdge> edge))
                return false;

            return Equals(edge);
        }

        [Pure]
        public override int GetHashCode()
        {
            return OriginalEdge.GetHashCode() ^ 16777619;
        }

        [Pure]
        public override string ToString()
        {
            return string.Format("R({0})", OriginalEdge);
        }

        [Pure]
        public bool Equals(SReversedEdge<TVertex, TEdge> other)
        {
            return OriginalEdge.Equals(other.OriginalEdge);
        }
    }
}
