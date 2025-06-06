﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace QuickGraph
{
    /// <summary>
    /// An struct based <see cref="IEdge&lt;TVertex&gt;"/> implementation.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay(EdgeExtensions.DebuggerDisplayUndirectedEdgeFormatString)]
    [StructLayout(LayoutKind.Auto)]
    public struct SEquatableUndirectedEdge<TVertex>
        : IUndirectedEdge<TVertex>
        , IEquatable<SEquatableUndirectedEdge<TVertex>>
    {
        private readonly TVertex source;
        private readonly TVertex target;

        /// <summary>
        /// Initializes a new instance of the <see cref="SEdge&lt;TVertex&gt;"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public SEquatableUndirectedEdge(TVertex source, TVertex target)
        {
            Contract.Requires(source != null);
            Contract.Requires(target != null);
            if (Comparer<TVertex>.Default.Compare(source, target) > 0)
            {
                throw new ArgumentException("source cannot be greater than target in SEquatableUndirectedEdge");
            }
            Contract.Ensures(Contract.ValueAtReturn(out this).Source.Equals(source));
            Contract.Ensures(Contract.ValueAtReturn(out this).Target.Equals(target));

            this.source = source;
            this.target = target;
        }

        /// <summary>
        /// Gets the source vertex
        /// </summary>
        /// <value></value>
        public TVertex Source => source;

        /// <summary>
        /// Gets the target vertex
        /// </summary>
        /// <value></value>
        public TVertex Target => target;

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                EdgeExtensions.UndirectedEdgeFormatString,
                Source,
                Target);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SEquatableUndirectedEdge<TVertex> other)
        {
            Contract.Ensures(
                Contract.Result<bool>() ==
                (Source.Equals(other.Source) &&
                Target.Equals(other.Target))
                );

            return
                source.Equals(other.source) &&
                target.Equals(other.target);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return
                obj is SEquatableUndirectedEdge<TVertex> edge &&
                Equals(edge);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(
                source.GetHashCode(), 
                target.GetHashCode());
        }
    }
}
