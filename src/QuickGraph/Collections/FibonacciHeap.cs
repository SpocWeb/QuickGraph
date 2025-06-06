﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph.Collections
{
    /// <summary>
    /// Specifies the order in which a Heap will Dequeue items.
    /// </summary>
    public enum HeapDirection
    {
        /// <summary>
        /// Items are Dequeued in Increasing order from least to greatest.
        /// </summary>
        Increasing,
        /// <summary>
        /// Items are Dequeued in Decreasing order, from greatest to least.
        /// </summary>
        Decreasing
    }
    
    internal static class LambdaHelpers
    {
        /// <summary>
        /// Performs an action on each item in a list, used to shortcut a "foreach" loop
        /// </summary>
        /// <typeparam name="T">Type contained in List</typeparam>
        /// <param name="collection">List to enumerate over</param>
        /// <param name="action">Lambda Function to be performed on all elements in List</param>
        internal static void ForEach<T>(IList<T> collection, Action<T> action)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                action(collection[i]);
            }
        }
        /// <summary>
        /// Performs an action on each item in a list, used to shortcut a "foreach" loop
        /// </summary>
        /// <typeparam name="T">Type contained in List</typeparam>
        /// <param name="collection">List to enumerate over</param>
        /// <param name="action">Lambda Function to be performed on all elements in List</param>
        public static void ForEach<T>(IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        public static Stack<T> ToStack<T>(IEnumerable<T> collection)
        {
            Stack<T> newStack = new Stack<T>();
            ForEach(collection, x => newStack.Push(x));
            return newStack;
        }
    }

    public sealed class FibonacciHeapLinkedList<TPriority, TValue> 
        : IEnumerable<FibonacciHeapCell<TPriority, TValue>>
    {
        FibonacciHeapCell<TPriority, TValue> first;
        FibonacciHeapCell<TPriority, TValue> last;

        public FibonacciHeapCell<TPriority, TValue> First => first;

        internal FibonacciHeapLinkedList()
        {
            first = null;
            last = null; 
        }

        internal void MergeLists(FibonacciHeapLinkedList<TPriority, TValue> list)
        {
            Contract.Requires(list != null);

            if (list.First != null)
            {
                if (last != null)
                {
                    last.Next = list.first;
                }
                list.first.Previous = last;
                last = list.last;
                if (first == null)
                {
                    first = list.first;
                }
            }
        }

        internal void AddLast(FibonacciHeapCell<TPriority, TValue> node)
        {
            Contract.Requires(node != null);

            if (last != null)
            {
                last.Next = node;
            }
            node.Previous = last;
            last = node;
            if (first == null)
            {
                first = node;
            }
        }

        internal void Remove(FibonacciHeapCell<TPriority, TValue> node)
        {
            Contract.Requires(node != null);

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            else if (first == node)
            {
                first = node.Next;
            }

            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
            else if (last == node)
            {
                last = node.Previous;
            }

            node.Next = null;
            node.Previous = null;
        }

        #region IEnumerable<FibonacciHeapNode<T,K>> Members

        public IEnumerator<FibonacciHeapCell<TPriority, TValue>> GetEnumerator()
        {
            var current = first;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public sealed class FibonacciHeapCell<TPriority, TValue>
    {
        /// <summary>
        /// Determines of a Node has had a child cut from it before
        /// </summary>
        public bool Marked;
        /// <summary>
        /// Determines the depth of a node
        /// </summary>
        public int Degree;
        public TPriority Priority;
        public TValue Value;
        public bool Removed;
        public FibonacciHeapLinkedList<TPriority, TValue> Children;
        public FibonacciHeapCell<TPriority, TValue> Parent;
        public FibonacciHeapCell<TPriority, TValue> Next;
        public FibonacciHeapCell<TPriority, TValue> Previous;

        public KeyValuePair<TPriority, TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TPriority, TValue>(Priority, Value);
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public sealed class FibonacciHeap<TPriority, TValue> 
        : IEnumerable<KeyValuePair<TPriority, TValue>>
    {
        public FibonacciHeap()
            : this(HeapDirection.Increasing, Comparer<TPriority>.Default.Compare)
        { }

        public FibonacciHeap(HeapDirection Direction)
            : this(Direction, Comparer<TPriority>.Default.Compare)
        { }
        
        public FibonacciHeap(HeapDirection Direction, Comparison<TPriority> priorityComparison)            
        {
            nodes = new FibonacciHeapLinkedList<TPriority, TValue>();
            degreeToNode = new Dictionary<int, FibonacciHeapCell<TPriority, TValue>>();
            DirectionMultiplier = (short)(Direction == HeapDirection.Increasing ? 1 : -1);
            direction = Direction;
            priorityComparsion = priorityComparison;
            count = 0;
        }

        readonly FibonacciHeapLinkedList<TPriority, TValue> nodes;
        FibonacciHeapCell<TPriority, TValue> next;
        private readonly short DirectionMultiplier;  //Used to control the direction of the heap, set to 1 if the Heap is increasing, -1 if it's decreasing
                                          //We use the approach to avoid unnessecary branches
        private readonly Dictionary<int, FibonacciHeapCell<TPriority, TValue>> degreeToNode;
        private readonly Comparison<TPriority> priorityComparsion;
        private readonly HeapDirection direction;
        public HeapDirection Direction => direction;
        private int count;
        public int Count => count;
        //Draws the current heap in a string.  Marked Nodes have a * Next to them

        struct NodeLevel
        {
            public readonly FibonacciHeapCell<TPriority, TValue> Node;
            public readonly int Level;
            public NodeLevel(FibonacciHeapCell<TPriority, TValue> node, int level)
            {
                Node = node;
                Level = level;
            }
        }

        public Comparison<TPriority> PriorityComparison => priorityComparsion;

        public string DrawHeap()
        {
            var lines = new List<string>();
            var lineNum = 0;
            var columnPosition = 0;
            var list = new List<NodeLevel>();
            foreach (var node in nodes) list.Add(new NodeLevel(node, 0));
            list.Reverse();
            var stack = new Stack<NodeLevel>(list);
            while (stack.Count > 0)
            {
                var currentcell = stack.Pop();
                lineNum = currentcell.Level;
                if (lines.Count <= lineNum)
                    lines.Add(string.Empty);
                var currentLine = lines[lineNum];
                currentLine = currentLine.PadRight(columnPosition, ' ');
                var nodeString = currentcell.Node.Priority.ToString() + (currentcell.Node.Marked ? "*" : "") + " ";
                currentLine += nodeString;
                if (currentcell.Node.Children != null && currentcell.Node.Children.First != null)
                {
                    var children = new List<FibonacciHeapCell<TPriority, TValue>>(currentcell.Node.Children);
                    children.Reverse();
                    children.ForEach(x => stack.Push(new NodeLevel(x, currentcell.Level + 1)));
                }
                else
                {
                    columnPosition += nodeString.Length;
                }
                lines[lineNum] = currentLine;
            }
            return string.Join(Environment.NewLine, lines.ToArray());
        }

        public FibonacciHeapCell<TPriority, TValue> Enqueue(TPriority Priority, TValue Value)
        {
            var newNode =
                new FibonacciHeapCell<TPriority, TValue>
                {
                    Priority = Priority,
                    Value = Value,
                    Marked = false,
                    Children = new FibonacciHeapLinkedList<TPriority, TValue>(),
                    Degree = 1,
                    Next = null,
                    Previous = null,
                    Parent = null,
                    Removed = false
                };

            //We don't do any book keeping or maintenance of the heap on Enqueue,
            //We just add this node to the end of the list of Heaps, updating the Next if required
            nodes.AddLast(newNode);
            if (next == null || 
                (priorityComparsion(newNode.Priority, next.Priority) * DirectionMultiplier) < 0)
            {
                next = newNode;
            }
            count++;
            return newNode;            
        }

        public void Delete(FibonacciHeapCell<TPriority, TValue> node)
        {
            Contract.Requires(node != null);

            ChangeKeyInternal(node, default(TPriority), true);
            Dequeue();            
        }

        public void ChangeKey(FibonacciHeapCell<TPriority, TValue> node, TPriority newKey)
        {            
            Contract.Requires(node != null);

            ChangeKeyInternal(node, newKey, false);            
        }

        private void ChangeKeyInternal(
            FibonacciHeapCell<TPriority, TValue> node, 
            TPriority NewKey, bool deletingNode)
        {
            Contract.Requires(node != null);

            var delta = Math.Sign(priorityComparsion(node.Priority, NewKey));
            if (delta == 0)
                return;
            if (delta == DirectionMultiplier || deletingNode)
            {
                //New value is in the same direciton as the heap
                node.Priority = NewKey;
                var parentNode = node.Parent;
                if (parentNode != null && ((priorityComparsion(NewKey, node.Parent.Priority) * DirectionMultiplier) < 0 || deletingNode))
                {
                    node.Marked = false;
                    parentNode.Children.Remove(node);
                    UpdateNodesDegree(parentNode);
                    node.Parent = null;
                    nodes.AddLast(node);
                    //This loop is the cascading cut, we continue to cut
                    //ancestors of the node reduced until we hit a root 
                    //or we found an unmarked ancestor
                    while (parentNode.Marked && parentNode.Parent != null)
                    {
                        parentNode.Parent.Children.Remove(parentNode);
                        UpdateNodesDegree(parentNode);
                        parentNode.Marked = false;
                        nodes.AddLast(parentNode);
                        var currentParent = parentNode;
                        parentNode = parentNode.Parent;
                        currentParent.Parent = null;
                    }
                    if (parentNode.Parent != null)
                    {
                        //We mark this node to note that it's had a child
                        //cut from it before
                        parentNode.Marked = true;
                    }
                }
                //Update next
                if (deletingNode || (priorityComparsion(NewKey, next.Priority) * DirectionMultiplier) < 0)
                {
                    next = node;
                }
            }
            else
            {
                //New value is in opposite direction of Heap, cut all children violating heap condition
                node.Priority = NewKey;
                if (node.Children != null)
                {
                    List<FibonacciHeapCell<TPriority, TValue>> toupdate = null;
                    foreach (var child in node.Children)
                    {
                        if ((priorityComparsion(node.Priority, child.Priority) * DirectionMultiplier) > 0)
                        {
                            if (toupdate == null)
                                toupdate = new List<FibonacciHeapCell<TPriority, TValue>>();
                            toupdate.Add(child);
                        }
                    }

                    if (toupdate != null)
                        foreach (var child in toupdate)
                        {
                            node.Marked = true;
                            node.Children.Remove(child);
                            child.Parent = null;
                            child.Marked = false;
                            nodes.AddLast(child);
                            UpdateNodesDegree(node);
                        }
                }
                UpdateNext();
            }
        }

        static int Max<T>(IEnumerable<T> values, Converter<T, int> converter)
        {
            Contract.Requires(values != null);
            Contract.Requires(converter != null);

            int max = int.MinValue;
            foreach (var value in values)
            {
                int v = converter(value);
                if (max < v)
                    max = v;
            }
            return max;
        }

        /// <summary>
        /// Updates the degree of a node, cascading to update the degree of the
        /// parents if nessecary
        /// </summary>
        /// <param name="parentNode"></param>
        private void UpdateNodesDegree(
            FibonacciHeapCell<TPriority, TValue> parentNode)
        {
            Contract.Requires(parentNode != null);

            var oldDegree = parentNode.Degree;
            parentNode.Degree = 
                parentNode.Children.First != null
                ? Max(parentNode.Children, x => x.Degree) + 1 
                : 1;
            FibonacciHeapCell<TPriority, TValue> degreeMapValue;
            if (oldDegree != parentNode.Degree)
            {
                if (degreeToNode.TryGetValue(oldDegree, out degreeMapValue) && degreeMapValue == parentNode)
                {
                    degreeToNode.Remove(oldDegree);
                }
                else if (parentNode.Parent != null)
                {
                    UpdateNodesDegree(parentNode.Parent);
                }
            }
        }

        public KeyValuePair<TPriority, TValue> Dequeue()
        {
            if (count == 0)
                throw new InvalidOperationException();

            var result = new KeyValuePair<TPriority, TValue>(
                next.Priority,
                next.Value);

            nodes.Remove(next);
            next.Next = null;
            next.Parent = null;
            next.Previous = null;
            next.Removed = true;
            FibonacciHeapCell<TPriority, TValue> currentDegreeNode;
            if (degreeToNode.TryGetValue(next.Degree, out currentDegreeNode))
            {
                if (currentDegreeNode == next)
                {
                    degreeToNode.Remove(next.Degree);
                }
            }
            Contract.Assert(next.Children != null);
            foreach (var child in next.Children)
            {
                child.Parent = null;
            }
            nodes.MergeLists(next.Children);
            next.Children = null;
            count--;
            UpdateNext();

            return result;
        }

        /// <summary>
        /// Updates the Next pointer, maintaining the heap
        /// by folding duplicate heap degrees into eachother
        /// Takes O(lg(N)) time amortized
        /// </summary>
        private void UpdateNext()
        {
            CompressHeap();
            var node = nodes.First;
            next = nodes.First;
            while (node != null)
            {
                if ((priorityComparsion(node.Priority, next.Priority) * DirectionMultiplier) < 0)
                {
                    next = node;
                }
                node = node.Next;
            }
        }

        private void CompressHeap()
        {
            var node = nodes.First;
            FibonacciHeapCell<TPriority, TValue> currentDegreeNode;
            while (node != null)
            {
                var nextNode = node.Next;
                while (degreeToNode.TryGetValue(node.Degree, out currentDegreeNode) && currentDegreeNode != node)
                {
                    degreeToNode.Remove(node.Degree);
                    if ((priorityComparsion(currentDegreeNode.Priority, node.Priority) * DirectionMultiplier) <= 0)
                    {
                        if (node == nextNode)
                        {
                            nextNode = node.Next;
                        }
                        ReduceNodes(currentDegreeNode, node);
                        node = currentDegreeNode;
                    }
                    else
                    {
                        if (currentDegreeNode == nextNode)
                        {
                            nextNode = currentDegreeNode.Next;
                        }
                        ReduceNodes(node, currentDegreeNode);
                    }
                }
                degreeToNode[node.Degree] = node;
                node = nextNode;
            }
        }

        /// <summary>
        /// Given two nodes, adds the child node as a child of the parent node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        private void ReduceNodes(
            FibonacciHeapCell<TPriority, TValue> parentNode, 
            FibonacciHeapCell<TPriority, TValue> childNode)
        {
            Contract.Requires(parentNode != null);
            Contract.Requires(childNode != null);

            nodes.Remove(childNode);
            parentNode.Children.AddLast(childNode);
            childNode.Parent = parentNode;
            childNode.Marked = false;
            if (parentNode.Degree == childNode.Degree)
            {
                parentNode.Degree += 1;
            }
        }

        public bool IsEmpty => nodes.First == null;

        public FibonacciHeapCell<TPriority, TValue> Top => next;

        public void Merge(FibonacciHeap<TPriority, TValue> other)
        {      
            Contract.Requires(other != null);

            if (other.Direction != Direction)
            {
                throw new Exception("Error: Heaps must go in the same direction when merging");
            }
            nodes.MergeLists(other.nodes);
            if ((priorityComparsion(other.Top.Priority, next.Priority) * DirectionMultiplier) < 0)
            {
                next = other.next;
            }
            count += other.Count;
        }

        public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
        {
            var tempHeap = new FibonacciHeap<TPriority, TValue>(Direction, priorityComparsion);
            var nodeStack = new Stack<FibonacciHeapCell<TPriority, TValue>>();
            LambdaHelpers.ForEach(nodes, x => nodeStack.Push(x));
            while (nodeStack.Count > 0)
            {
                var topNode = nodeStack.Peek();
                tempHeap.Enqueue(topNode.Priority, topNode.Value);
                nodeStack.Pop();
                LambdaHelpers.ForEach(topNode.Children, x => nodeStack.Push(x));
            }
            while (!tempHeap.IsEmpty)
            {
                yield return tempHeap.Top.ToKeyValuePair();
                tempHeap.Dequeue();
            }
        }
        public IEnumerable<KeyValuePair<TPriority, TValue>> GetDestructiveEnumerator()
        {
            while (!IsEmpty)
            {
                yield return Top.ToKeyValuePair();
                Dequeue();
            }
        }

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}