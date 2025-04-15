using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Collections
{
    public sealed class SoftHeap<TKey, TValue>
        : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        sealed class Cell
        {
            public readonly TKey Key;
            public readonly TValue Value;
            public Cell Next = null;

            public Cell(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        sealed class Node
        {
            public TKey CKey;
            public readonly int Rank;
            public Node Next;
            public Node Child;
            public Cell IL;
            public Cell ILTail;

            public Node(Cell cell)
            {
                Rank = 0;
                CKey = cell.Key;
                IL = cell;
                ILTail = cell;
            }

            public Node(TKey cKey, int rank, Node next, Node child, Cell il, Cell iltail)
            {
                CKey = cKey;
                Rank = rank;
                Next = next;
                Child = child;
                IL = il;
                ILTail = iltail;
            }
        }

        sealed class Head
        {
            public Node Queue;
            public Head Next;
            public Head Prev;
            public Head SuffixMin;
            public int Rank;
        }

        readonly Comparison<TKey> comparison;
        readonly TKey keyMaxValue;
        readonly double errorRate;
        readonly Head header;
        readonly Head tail;
        int count;
        readonly int r;

        public SoftHeap(double maximumErrorRate, TKey keyMaxValue)
            : this(maximumErrorRate, keyMaxValue, Comparer<TKey>.Default.Compare)
        { }

        public SoftHeap(double maximumErrorRate, TKey keyMaxValue, Comparison<TKey> comparison)
        {
            Contract.Requires(comparison != null);
            Contract.Requires(0 < maximumErrorRate && maximumErrorRate <= 0.5);

            this.comparison = comparison;
            this.keyMaxValue = keyMaxValue;
            header = new Head();
            tail = new Head();
            tail.Rank = int.MaxValue;
            header.Next = tail;
            tail.Prev = header;
            errorRate = maximumErrorRate;
            r = 2 + 2 * (int)Math.Ceiling(Math.Log(1.0 / errorRate, 2.0));
            count = 0;
        }

        public int MinRank
        {
            get { return r; }
        }

        public Comparison<TKey> Comparison
        {
            get { return comparison; }
        }

        public double ErrorRate
        {
            get { return errorRate; }
        }

        public int Count
        {
            get { return count; }
        }

        public TKey KeyMaxValue
        {
            get { return keyMaxValue; }
        }

        public void Add(TKey key, TValue value)
        {
            Contract.Requires(Comparison(key, KeyMaxValue) < 0);

            var l = new Cell(key, value);
            var q = new Node(l);

            Meld(q);
            count++;
        }

        private void Meld(Node q)
        {
            Contract.Requires(q != null);

            Head toHead = header.Next;
            while (q.Rank > toHead.Rank)
            {
                Contract.Assert(toHead.Next != null);
                toHead = toHead.Next;
            }
            Head prevHead = toHead.Prev;
            while (q.Rank == toHead.Rank)
            {
                Node top, bottom;
                if (comparison(toHead.Queue.CKey, q.CKey) > 0)
                {
                    top = q;
                    bottom = toHead.Queue;
                }
                else
                {
                    top = toHead.Queue;
                    bottom = q;
                }
                q = new Node(top.CKey, top.Rank + 1, bottom, top, top.IL, top.ILTail);
                toHead = toHead.Next;
            }

            Head h;
            if (prevHead == toHead.Prev)
                h = new Head();
            else
                h = prevHead.Next;
            h.Queue = q;
            h.Rank = q.Rank;
            h.Prev = prevHead;
            h.Next = toHead;
            prevHead.Next = h;
            toHead.Prev = h;

            FixMinLinst(h);
        }

        private void FixMinLinst(Head h)
        {
            Contract.Requires(h != null);

            Head tmpmin;
            if (h.Next == tail)
                tmpmin = h;
            else
                tmpmin = h.Next.SuffixMin;

            while (h != header)
            {
                if (comparison(tmpmin.Queue.CKey, h.Queue.CKey) > 0)
                    tmpmin = h;

                h.SuffixMin = tmpmin;
                h = h.Prev;
            }
        }

        private Node Shift(Node v)
        {
            Contract.Requires(v != null);

            v.IL = null;
            v.ILTail = null;
            if (v.Next == null && v.Child == null)
            {
                v.CKey = keyMaxValue;
                return v;
            }

            v.Next = Shift(v.Next);
            // restore heap ordering that might be broken by shifting
            if (comparison(v.Next.CKey, v.Child.CKey) > 0)
            {
                var tmp = v.Child;
                v.Child = v.Next;
                v.Next = tmp;
            }

            v.IL = v.Next.IL;
            v.ILTail = v.Next.ILTail;
            v.CKey = v.Next.CKey;

            // softening the heap
            if (v.Rank > r &&
                (v.Rank % 2 == 1 || v.Child.Rank < v.Rank - 1))
            {
                v.Next = Shift(v.Next);
                // restore heap ordering that might be broken by shifting
                if (comparison(v.Next.CKey, v.Child.CKey) > 0)
                {
                    var tmp = v.Child;
                    v.Child = v.Next;
                    v.Next = tmp;
                }

                if (comparison(v.Next.CKey, keyMaxValue) != 0 && v.Next.IL != null)
                {
                    v.Next.ILTail.Next = v.IL;
                    v.IL = v.Next.IL;
                    if (v.ILTail == null)
                        v.ILTail = v.Next.ILTail;
                    v.CKey = v.Next.CKey;
                }
            }  // end second shift

            if (comparison(v.Child.CKey, keyMaxValue) == 0)
            {
                if (comparison(v.Next.CKey, keyMaxValue) == 0)
                {
                    v.Child = null;
                    v.Next = null;
                }
                else
                {
                    v.Child = v.Next.Child;
                    v.Next = v.Next.Next;
                }
            }

            return v;
        } // Shift

        public KeyValuePair<TKey, TValue> DeleteMin()
        {
            if (count == 0)
                throw new InvalidOperationException();

            var h = header.Next.SuffixMin;
            while (h.Queue.IL == null)
            {
                var tmp = h.Queue;
                int childCount = 0;
                while (tmp.Next != null)
                {
                    tmp = tmp.Next;
                    childCount++;
                }

                if (childCount < h.Rank / 2)
                {
                    h.Prev.Next = h.Next;
                    h.Next.Prev = h.Prev;
                    FixMinLinst(h.Prev);
                    tmp = h.Queue;
                    while (tmp.Next != null)
                    {
                        Meld(tmp.Child);
                        tmp = tmp.Next;
                    }
                }
                else
                {
                    h.Queue = Shift(h.Queue);
                    if (comparison(h.Queue.CKey, keyMaxValue) == 0)
                    {
                        h.Prev.Next = h.Next;
                        h.Next.Prev = h.Prev;
                        h = h.Prev;
                    }

                    FixMinLinst(h);
                }

                h = header.Next.SuffixMin;
            } // end of outer while loop

            var min = h.Queue.IL.Key;
            var value = h.Queue.IL.Value;
            h.Queue.IL = h.Queue.IL.Next;
            if (h.Queue.IL == null)
                h.Queue.ILTail = null;

            count--;
            return new KeyValuePair<TKey, TValue>(min, value);
        }

        [ContractInvariantMethod]
        void Invariant()
        {
            Contract.Invariant(count > -1);
            Contract.Invariant(header != null);
            Contract.Invariant(tail != null);
        }

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        struct Enumerator
            : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            readonly SoftHeap<TKey, TValue> owner;
            readonly KeyValuePair<TKey, TValue> current;

            public Enumerator(SoftHeap<TKey, TValue> owner)
            {
                Contract.Requires(owner != null);
                this.owner = owner;
                current = new KeyValuePair<TKey, TValue>();
            }

            public bool MoveNext()
            {
                // TODO
                return false;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return current; }
            }

            public void Dispose()
            { }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
        #endregion
    }
}
