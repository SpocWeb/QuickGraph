using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace QuickGraph.Collections
{
    /// <summary>
    /// Disjoint-set implementation with path compression and union-by-rank optimizations.
    /// optimization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ForestDisjointSet<T>
        : IDisjointSet<T>
    {
#if DEBUG
        [DebuggerDisplay("{ID}:{Rank}->{Parent}")]
#endif
        class Element
        {
#if DEBUG
            public readonly int ID;
            static int nextID;
#endif
            public Element Parent;
            public int Rank;
            public readonly T Value;
            public Element(T value)
            {
#if DEBUG
                ID = nextID++;
#endif
                Parent = null;
                Rank = 0;
                Value = value;
            }
        }

        readonly Dictionary<T, Element> elements;
        int setCount;

        public ForestDisjointSet(int elementCapacity)
        {
            Contract.Requires(elementCapacity >= 0 && elementCapacity < int.MaxValue);
            elements = new Dictionary<T, Element>(elementCapacity);
            setCount = 0;
        }

        public ForestDisjointSet()
        {
            elements = new Dictionary<T, Element>();
            setCount = 0;
        }

        public int SetCount => setCount;

        public int ElementCount => elements.Count;

        /// <summary>
        /// Adds a new set
        /// </summary>
        /// <param name="value"></param>
        public void MakeSet(T value)
        {
            var element = new Element(value);
            elements.Add(value, element);
            setCount++;
        }

        [Pure]
        public bool Contains(T value)
        {
            return elements.ContainsKey(value);
        }

        public bool Union(T left, T right)
        {
            return Union(elements[left], elements[right]);
        }

        public T FindSet(T value)
        {
            return Find(elements[value]).Value;
        }

        public bool AreInSameSet(T left, T right)
        {
            return FindSet(left).Equals(FindSet(right));
        }

        [Pure]
        private static Element FindNoCompression(Element element)
        {
            Contract.Requires(element != null);
            Contract.Ensures(Contract.Result<Element>() != null);

            // find root,
            var current = element;
            while (current.Parent != null)
                current = current.Parent;

            return current;
        }

        /// <summary>
        /// Finds the parent element, and applies path compression
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Element Find(Element element)
        {
            Contract.Requires(element != null);
            Contract.Ensures(Contract.Result<Element>() != null);

            var root = FindNoCompression(element);            
            CompressPath(element, root);
            return root;
        }

        private static void CompressPath(Element element, Element root)
        {
            Contract.Requires(element != null);
            Contract.Requires(root != null);

            // path compression
            var current = element;
            while (current != root)
            {
                var temp = current;
                current = current.Parent;
                temp.Parent = root;
            }
        }

        private bool Union(Element left, Element right)
        {
            Contract.Requires(left != null);
            Contract.Requires(right != null);
            Contract.Ensures(
                Contract.Result<bool>() 
                ? Contract.OldValue(SetCount) - 1 == SetCount             
                : Contract.OldValue(SetCount) == SetCount);
            Contract.Ensures(FindNoCompression(left) == FindNoCompression(right));

            // shortcut when already unioned,
            if (left == right) return false;

            var leftRoot = Find(left);
            var rightRoot = Find(right);

            // union by rank
            if (leftRoot.Rank > rightRoot.Rank)
                rightRoot.Parent = leftRoot;
            else if (leftRoot.Rank < rightRoot.Rank)
                leftRoot.Parent = rightRoot;
            else if (leftRoot != rightRoot)
            {
                rightRoot.Parent = leftRoot;
                leftRoot.Rank = leftRoot.Rank + 1;
            }
            else
                return false; // do not update the setcount

            setCount--;
            return true;
        }

        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(setCount >= 0);
            Contract.Invariant(setCount <= elements.Count);
        }
    }
}
