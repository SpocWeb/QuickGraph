﻿using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace QuickGraph
{
    /// <summary>
    /// A tagged edge as value type.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TTag"></typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("{Source}->{Target}:{Tag}")]
    public struct STaggedEdge<TVertex, TTag>
        : IEdge<TVertex>
        , ITagged<TTag>
    {
        readonly TVertex source;
        readonly TVertex target;
        TTag tag;

        public STaggedEdge(TVertex source, TVertex target, TTag tag)
        {
            Contract.Requires(source != null);
            Contract.Requires(target != null);

            this.source = source;
            this.target = target;
            this.tag = tag;
            TagChanged = null;
        }

        public TVertex Source => source;

        public TVertex Target => target;

        public event EventHandler TagChanged;

        void OnTagChanged(EventArgs e)
        {
            var eh = TagChanged;
            if (eh != null)
                eh(this, e);
        }

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
            return string.Format("{0}->{1}:{2}", Source, Target, Tag);
        }

    }
}
