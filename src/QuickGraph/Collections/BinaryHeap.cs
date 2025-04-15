using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Diagnostics.Contracts;

namespace QuickGraph.Collections
{
    /// <summary>
    /// Binary heap
    /// </summary>
    /// <remarks>
    /// Indexing rules:
    /// 
    /// parent index: index ¡ 1)/2
    /// left child: 2 * index + 1
    /// right child: 2 * index + 2
    /// 
    /// Reference:
    /// http://dotnetslackers.com/Community/files/folders/data-structures-and-algorithms/entry28722.aspx
    /// </remarks>
    /// <typeparam name="TValue">type of the value</typeparam>
    /// <typeparam name="TPriority">type of the priority metric</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class BinaryHeap<TPriority, TValue> 
        : IEnumerable<KeyValuePair<TPriority, TValue>>
    {
        readonly Comparison<TPriority> priorityComparsion;
        KeyValuePair<TPriority, TValue>[] items;
        int count;
        int version;

        const int DefaultCapacity = 16;

        public BinaryHeap()
            : this(DefaultCapacity, Comparer<TPriority>.Default.Compare)
        { }

        public BinaryHeap(Comparison<TPriority> priorityComparison)
            : this(DefaultCapacity, priorityComparison)
        { }

        public BinaryHeap(int capacity, Comparison<TPriority> priorityComparison)
        {
            Contract.Requires(capacity >= 0);
            Contract.Requires(priorityComparison != null);

            items = new KeyValuePair<TPriority, TValue>[capacity];
            priorityComparsion = priorityComparison;
        }

        public Comparison<TPriority> PriorityComparison => priorityComparsion;

        public int Capacity => items.Length;

        public int Count => count;

        public void Add(TPriority priority, TValue value)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine("Add({0}, {1})", priority, value);
#endif
            version++;
            ResizeArray();
            items[count++] = new KeyValuePair<TPriority, TValue>(priority, value);
            MinHeapifyDown(count - 1);
#if BINARY_HEAP_DEBUG
            Console.WriteLine("Add: {0}", ToString2());
#endif
        }

        // TODO: MinHeapifyDown is really MinHeapifyUp.  Do the renaming
        private void MinHeapifyDown(int start)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine("MinHeapifyDown");
#endif
            int current = start;
            int parent = (current - 1) / 2;
            while (current > 0 && Less(current, parent))
            {
                Swap(current, parent);
                current = parent;
                parent = (current - 1) / 2;
            }
        }

        public TValue[] ToValueArray()
        {
            var values = new TValue[items.Length];
            for (int i = 0; i < values.Length; ++i)
                values[i] = items[i].Value;
            return values;
        }

        public KeyValuePair<TPriority, TValue>[] ToPriorityValueArray()
        {
            var array = new KeyValuePair<TPriority, TValue>[items.Length];
            for (int i = 0; i < array.Length; ++i)
                array[i] = items[i];
            return array;
        }

        public bool IsConsistent()
        {
            int wrong = -1;

            for (int i = 0; i < count; i++)
            {
                var l = 2 * i + 1;
                var r = 2 * i + 2;
                if (l < count && !LessOrEqual(i, l))
                    wrong = i;
                if (r < count && !LessOrEqual(i, r))
                    wrong = i;
            }

            var correct = wrong == -1;
            return correct;
        }

        private string EntryToString(int i)
        {
            if (i < 0 || i >= count)
                return "null";

            var kvp = items[i];
            var k = kvp.Key;
            var v = kvp.Value;

            var str = "";
            str += k.ToString();
            str += " ";
            str += v == null ? "null" : v.ToString();
            return str;
        }

        public string ToString2()
        {
            var status = IsConsistent();
            var str = status.ToString() + ": ";

            for (int i = 0; i < items.Length; i++)
            {
                str += EntryToString(i);
                str += ", ";
            }
            return str;
        }

        public string ToStringTree()
        {
            var status = IsConsistent();
            var str = "Consistent? " + status.ToString();

            for (int i = 0; i < count; i++)
            {
                var l = 2 * i + 1;
                var r = 2 * i + 2;

                var s = "index";
                s += i.ToString();
                s += ": ";
                s += EntryToString(i);
                s += " -> ";
                s += EntryToString(l);
                s += " and ";
                s += EntryToString(r);

                str += "\r\n" + s;
            }
            return str;
        }

        private void ResizeArray()
        {
            if (count == items.Length)
            {
                var newItems = new KeyValuePair<TPriority, TValue>[count * 2 + 1];
                Array.Copy(items, newItems, count);
                items = newItems;
            }
        }

        public KeyValuePair<TPriority, TValue> Minimum()
        {
            if (count == 0)
                throw new InvalidOperationException();
            return items[0];
        }

        public KeyValuePair<TPriority, TValue> RemoveMinimum()
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine("RemoveMinimum");
#endif
            if (count == 0)
                throw new InvalidOperationException("heap is empty");

            // shortcut for heap with 1 element.
            if (count == 1)
            {
                version++;
                return items[--count];
            }
            Swap(0, count - 1);
            count--;
            MinHeapifyUp(0);
            return items[count];
        }

        /// <summary>
        /// Removes element at a certain index.  
        /// TODO: RemoveAt is wrong.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Obsolete("BinaryHeap.RemoveAt is wrong. Fix it before using it.")]
        public KeyValuePair<TPriority, TValue> RemoveAt(int index)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine("RemoveAt({0})", index);
#endif
            if (count == 0)
                throw new InvalidOperationException("heap is empty");
            if (index < 0 | index >= count)
                throw new ArgumentOutOfRangeException("index");

            version++;
            // shortcut for heap with 1 element.
            if (count == 1)
                return items[--count];

            Swap(index, count - 1);
            count--;
            MinHeapifyUp(index);

            return items[count];
        }

        // TODO: MinHeapifyUp is really MinHeapifyDown.  Do the renaming
        private void MinHeapifyUp(int index)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine("MinHeapifyUp");
#endif
            while (true)
            {
                var left = 2 * index + 1;
                var right = 2 * index + 2;
                var smallest = index;
                if (left < count && Less(left, smallest))
                    smallest = left;
                if (right < count && Less(right, smallest))
                    smallest = right;
                
                if (smallest == index)
                    break;

                Swap(smallest, index);
                index = smallest;
            }
        }

        public int IndexOf(TValue value)
        {
            for (int i = 0; i < count; i++)
            {
                if (Equals(value, items[i].Value))
                    return i;
            }
            return -1;
        }

        public bool MinimumUpdate(TPriority priority, TValue value)
        {
            // find index
            var index = IndexOf(value);

            if (index >= 0)
            {
                if (priorityComparsion(priority, items[index].Key) <= 0)
                {
                    Update(priority, value);
                    return true;
                }
                return false;
            }

            // not in collection
            Add(priority, value);
            return true;
        }

        public void Update(TPriority priority, TValue value)
        {
#if BINARY_HEAP_DEBUG
            Console.WriteLine("Update({0}, {1})", priority, value);
#endif
            // find index
            var index = IndexOf(value);
            
            // if it exists, update, else add
            if (index > -1)
            {
                var neww = priority;
                var oldd = items[index].Key;
                items[index] = new KeyValuePair<TPriority,TValue>(neww, value);

                if (priorityComparsion(neww, oldd) > 0)
                    MinHeapifyUp(index);
                else if (priorityComparsion(neww, oldd) < 0)
                    MinHeapifyDown(index);
            }
            else
            {
                Add(priority, value);
            }
        }

        [Pure]
        private bool LessOrEqual(int i, int j)
        {
            Contract.Requires(
                i >= 0 & i < count &
                j >= 0 & j < count &
                i != j);

            return priorityComparsion(items[i].Key, items[j].Key) <= 0;
        }

        [Pure]
        private bool Less(int i, int j)
        {
            Contract.Requires(
                i >= 0 & i < count &
                j >= 0 & j < count);

            return priorityComparsion(items[i].Key, items[j].Key) < 0;
        }

        private void Swap(int i, int j)
        {
            Contract.Requires(
                i >= 0 && i < count &&
                j >= 0 && j < count);

            if (i == j)
            {
                return;
            }

            var kv = items[i];
            items[i] = items[j];
            items[j] = kv;
        }

#if DEEP_INVARIANT
        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(this.items != null);
            Contract.Invariant(
                this.count > -1 &
                this.count <= this.items.Length);
            Contract.Invariant(
                EnumerableContract.All(0, this.count, index =>
                {
                    var left = 2 * index + 1;
                    var right = 2 * index + 2;
                    return  (left >= count || this.LessOrEqual(index, left)) &&
                            (right >= count || this.LessOrEqual(index, right));
                })
            );
        }
#endif

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        struct Enumerator :
            IEnumerator<KeyValuePair<TPriority, TValue>>
        {
            BinaryHeap<TPriority, TValue> owner;
            KeyValuePair<TPriority, TValue>[] items;
            readonly int count;
            readonly int version;
            int index;

            public Enumerator(BinaryHeap<TPriority, TValue> owner)
            {
                this.owner = owner;
                items = owner.items;
                count = owner.count;
                version = owner.version;
                index = -1;
            }

            public KeyValuePair<TPriority, TValue> Current
            {
                get
                {
                    if (version != owner.version)
                        throw new InvalidOperationException();
                    if (index < 0 | index == count)
                        throw new InvalidOperationException();
                    Contract.Assert(index <= count);
                    return items[index];
                }
            }

            void IDisposable.Dispose()
            {
                owner = null;
                items = null;
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (version != owner.version)
                    throw new InvalidOperationException();
                return ++index < count;
            }

            public void Reset()
            {
                if (version != owner.version)
                    throw new InvalidOperationException();
                index = -1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

    }
}
