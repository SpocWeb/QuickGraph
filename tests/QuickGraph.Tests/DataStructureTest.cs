using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickGraph.Tests
{
    [TestClass]
    public class DataStructureTest
    {
        [TestMethod]
        public void DisplayLinkedList()
        {
            var target = new LinkedList<int>();
            target.AddFirst(0);
            target.AddFirst(1);
        }
    }
}
