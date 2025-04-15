using System;
using System.Collections.Generic;
using QuickGraph.Algorithms;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph.Tests.Algorithms
{
    [TestClass]
    public class InducedPathsAlgorithmTests
    {
        private static List<List<int>> InducedPaths(int[] verticies, int[] edges)
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();

            graph.AddVertexRange(verticies);

            var graphEdges = new Edge<int>[edges.Length / 2];
            for (int i = 0; i < edges.Length; i += 2)
            {
                graphEdges[i / 2] = new Edge<int>(edges[i], edges[i + 1]);
            }
            graph.AddEdgeRange(graphEdges);

            return InducedPathAlgorithm.findInducedPaths(graph);
        }
        
        private static List<List<int>> ToListPaths(int[] paths, int len)
        {
            var res = new List<List<int>> { };

            for (int i = 0; i < paths.Length; i += len)
            {
                var list = new List<int> { };

                for (int j = 0; j < len; j++)
                {
                    list.Add(paths[i + j]);
                }

                res.Add(list);
            }

            return res;
        }

        private static List<List<T>> ListSort<T>(List<List<T>> paths, Func<T, T, int> minus = null)
        {
            var res = paths.Select(x => x.OrderBy(y => y).ToList()).ToList();
            res.Sort((ts, list) => CompareTwoLists(ts, list, minus));
            return res;
        }

        public static int CompareTwoLists<T>(IReadOnlyList<T> list1, IReadOnlyList<T> list2)
            => CompareTwoLists(list1, list2, null);

        public static int CompareTwoLists<T>(IReadOnlyList<T> list1, IReadOnlyList<T> list2, Func<T, T, int> minus)// = null)
        {
            minus ??= Comparer<T>.Default.Compare;
            for (int i = list1.Count; --i >= 0; )
            {
                var compare = minus(list1[i], list2[i]);
                if (compare != 0)
                {
                    return compare;
                }
            }

            return 0;
        }

        public static bool CompareLists<T>(List<List<T>> list1, List<List<T>> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            list1 = ListSort(list1);
            list2 = ListSort(list2);

            for (var i = list1.Count; --i >= 0;)
            {
                if (list1[i].Count != list2[i].Count || CompareTwoLists(list1[i], list2[i]) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        [TestMethod]
        public void InducedPathsOnePath()
        {
            var res = InducedPaths(new int[] { 1, 2, 3, 4, 5 },
                new int[] { 1, 2,  2, 3,  3, 4,  5, 4  });
            var paths = ToListPaths(new int[] { 1, 2, 3, 4, 5 }, 5);

            Assert.IsTrue(CompareLists(res, paths));
        }

        [TestMethod]
        public void InducedPathsFullGraph()
        {
            var edges = new int[] { 1, 2,  1, 3,  1, 4,  1, 5,  2, 3, 2, 4, 2, 5,
                            3, 4,  3, 5,  4, 5 };
            var res = InducedPaths(new int[] { 1, 2, 3, 4, 5 }, edges);
            var paths = new List<List<int>> {};

            for (int i = 0; i < edges.Length; i += 2)
            {
                paths.Add(new List<int> { edges[i], edges[i + 1] });
            }

            Assert.IsTrue(CompareLists(res, paths));
        }

        [TestMethod]
        public void InducedPathsEmptyGraph()
        {
            var res = InducedPaths(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                new int[] { });
            var paths = new List<List<int>> {  };

            Assert.IsTrue(CompareLists(res, paths));
        }

        [TestMethod]
        public void InducedPathsCrossedPaths()
        {
            var res = InducedPaths(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                new int[] { 1, 2,  2, 3,  3, 4,  4, 5,
                            6, 7,  7, 3,  3, 8,  8, 9 });
            var paths = ToListPaths (new int[] { 1, 2, 3, 4, 5,  1, 2, 3, 8, 9,  1, 2, 3, 6, 7,
                6, 7, 3, 4, 5,  6, 7, 3, 8, 9,  5, 4, 3, 8, 9 }, 5);

            Assert.IsTrue(CompareLists(res, paths));
        }

        [TestMethod]
        public void InducedPathsPetersenGraph()
        {
            var res = InducedPaths(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                new int[] { 1, 2,  1, 5,  1, 6,  2, 3,  2, 7,  3, 4,  3, 8,  4, 5,  4, 9,
                            5, 10,  6, 8,  6, 9,  7, 9,  7, 10,  8, 10 });

            Assert.IsTrue(res.Count == 60);
        }

        [TestMethod]
        public void InducedPathsCircle()
        {
            var res = InducedPaths(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                new int[] { 0, 1,  1, 2,  2, 3,  3, 4,
                            4, 5,  5, 6,  6, 7,  7, 0 });
            var paths = new List<List<int>> { };
            var path = new List<int> { 0, 1, 2, 3, 4, 5, 6 };

            for (int i = 0; i < 8; i++)
            {
                paths.Add(path);
                path = path.Select(x => (x + 1) % 8).ToList();
            }

            Assert.IsTrue(CompareLists(res, paths));
        }
    }
}