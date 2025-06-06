﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace QuickGraph
{
    /// <summary>
    /// An struct based <see cref="IUndirectedEdge&lt;TVertex&gt;"/> implementation.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay(EdgeExtensions.DebuggerDisplayUndirectedEdgeFormatString)]
    [StructLayout(LayoutKind.Auto)]
    public struct SUndirectedEdge<TVertex>
        : IUndirectedEdge<TVertex>
    {
        private readonly TVertex source;
        private readonly TVertex target;

        /// <summary>
        /// Initializes a new instance of the <see cref="SUndirectedEdge&lt;TVertex&gt;"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public SUndirectedEdge(TVertex source, TVertex target)
        {
            Contract.Requires(source != null);
            Contract.Requires(target != null);
            Contract.Requires(Comparer<TVertex>.Default.Compare(source, target) <= 0);
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
    }
}
