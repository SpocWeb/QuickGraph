// <copyright file="FibonacciHeapFactory.cs" company="MSIT">Copyright © MSIT 2007</copyright>

namespace QuickGraph.Collections
{
    public static partial class FibonacciHeapFactory
    {
        public static FibonacciHeap<int, int> Create()
        {
            FibonacciHeap<int, int> fibonacciHeap
               = new FibonacciHeap<int, int>();
            return fibonacciHeap;
        }
    }
}
