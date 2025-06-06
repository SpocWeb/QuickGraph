﻿using System;
using System.Linq;
using QuickGraph.Data;
using QuickGraph.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph.Tests.Data
{
    [TestClass]
    public class StoreTest
    {
        [TestMethod]
        public void DataSetTopologicalSortOfTables()
        {
            // typed dataset
            var store = new Store();
            // extract the graph
            var g = store.ToGraph(); 
            // topological sort of the tables (reversed)
            foreach(var table in g.TopologicalSort().Reverse())
                Console.WriteLine(table.TableName);
        }
        [TestMethod]
        public void DataSetGraphviz()
        {
            // typed dataset
            var store = new Store();
            // extract the graph
            var g = store.ToGraph();
            // rendering to dot
            Console.WriteLine(g.ToGraphviz());
        }

    }
}
