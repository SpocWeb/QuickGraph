// <copyright file="ForestDisjointSetFactory.cs" company="MSIT">Copyright © MSIT 2007</copyright>

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph.Collections
{
    public static partial class ForestDisjointSetFactory
    {
        public static ForestDisjointSet<int> Create(int[] elements, int[] unions)
        {
            var ds = new ForestDisjointSet<int>();
            for (int i = 0; i < elements.Length; ++i)
                ds.MakeSet(i);
            for (int i = 0; i+1 < unions.Length; i+=2)
            {
                Assert.IsTrue(ds.Contains(unions[i]));
                Assert.IsTrue(ds.Contains(unions[i+1]));
                ds.Union(unions[i], unions[i+1]);
            }
            return ds;
        }
    }
}
