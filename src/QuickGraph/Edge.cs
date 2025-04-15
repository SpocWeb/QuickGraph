﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
    /// <summary>
    /// The default <see cref="IEdge&lt;TVertex&gt;"/> implementation.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("{Source}->{Target}")]
    public class Edge<TVertex> 
        : IEdge<TVertex>
    {
        private readonly TVertex source;
        private readonly TVertex target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge&lt;TVertex&gt;"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public Edge(TVertex source, TVertex target)
        {
            Contract.Requires(source != null);
            Contract.Requires(target != null);
            Contract.Ensures(Source.Equals(source));
            Contract.Ensures(Target.Equals(target));

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
            return Source + "->" + Target;
        }
    }
}
