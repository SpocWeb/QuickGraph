﻿using System;
using System.Collections.Generic;

namespace QuickGraph.Collections
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class VertexList<TVertex>
        : List<TVertex>
#if !SILVERLIGHT
        , ICloneable
#endif
    {
        public VertexList()
        { }

        public VertexList(int capacity)
            : base(capacity)
        { }

        public VertexList(VertexList<TVertex> other)
            : base(other)
        { }

        public VertexList<TVertex> Clone()
        {
            return new VertexList<TVertex>(this);
        }

#if !SILVERLIGHT
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif
    }
}
