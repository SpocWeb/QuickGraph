using System;
using System.Diagnostics;

namespace QuickGraph
{
    /// <summary>
    /// An equatable term edge implementation
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("{Source}->{Target}")]
    public class EquatableTermEdge<TVertex> 
        : TermEdge<TVertex>
        , IEquatable<EquatableTermEdge<TVertex>>
    {
        public EquatableTermEdge(TVertex source, TVertex target, int sourceTerminal, int targetTerminal)
            : base(source, target, sourceTerminal, targetTerminal)
        { }

        public EquatableTermEdge(TVertex source, TVertex target)
            : base(source, target)
        { }

        public bool Equals(EquatableTermEdge<TVertex> other)
        {
            return
                (object)other != null &&
                Source.Equals(other.Source) &&
                Target.Equals(other.Target) &&
                SourceTerminal.Equals(other.SourceTerminal) &&
                TargetTerminal.Equals(other.TargetTerminal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EquatableTermEdge<TVertex>);
        }

        public override int GetHashCode()
        {
            return
                HashCodeHelper.Combine(Source.GetHashCode(), Target.GetHashCode(),
                                       SourceTerminal.GetHashCode(), TargetTerminal.GetHashCode());
        }
    }
}
