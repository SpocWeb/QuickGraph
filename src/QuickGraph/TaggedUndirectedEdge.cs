﻿using System;
using System.Diagnostics;

namespace QuickGraph
{
    /// <summary>
    /// A tagged undirected edge.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    /// <typeparam name="TTag">Type type of the tag</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay(EdgeExtensions.DebuggerDisplayTaggedUndirectedEdgeFormatString)]
    public class TaggedUndirectedEdge<TVertex, TTag> 
        : UndirectedEdge<TVertex>
        , ITagged<TTag>
    {
        private TTag tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedUndirectedEdge&lt;TVertex, TTag&gt;"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="tag">the tag</param>
        public TaggedUndirectedEdge(TVertex source, TVertex target, TTag tag)
            :base(source, target)
        {
            this.tag = tag;
        }

        /// <summary>
        /// Raised when the tag is changed
        /// </summary>
        public event EventHandler TagChanged;

        void OnTagChanged(EventArgs e)
        {
            var eh = TagChanged;
            if (eh != null)
                eh(this, e);
        }

        /// <summary>
        /// Gets or sets the tag
        /// </summary>
        public TTag Tag
        {
            get => tag;
            set
            {
                if (!Equals(tag, value))
                {
                    tag = value;
                    OnTagChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                EdgeExtensions.TaggedUndirectedEdgeFormatString,
                Source,
                Target,
                Tag);
        }
    }
}
