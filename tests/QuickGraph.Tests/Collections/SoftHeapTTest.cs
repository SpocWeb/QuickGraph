﻿// <copyright file="SoftHeapTKeyTValueTest.cs" company="MSIT">Copyright © MSIT 2008</copyright>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace QuickGraph.Collections
{
    /// <summary>This class contains parameterized unit tests for SoftHeap`2</summary>
    [TestClass]
    //[PexClass(typeof(SoftHeap<,>))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class SoftHeapTKeyTValueTest
    {
        public void Add(int[] keys)
        {
            keys.ShouldAllBe(k => k < int.MaxValue);
            Assert.IsTrue(keys.Length > 0);

            var target = new SoftHeap<int, int>(1/4.0, int.MaxValue);
            Console.WriteLine("expected error rate: {0}", target.ErrorRate);
            foreach (var key in keys)
            {
                var count = target.Count;
                target.Add(key, key + 1);
                Assert.AreEqual(count + 1, target.Count);
            }

            int lastMin = int.MaxValue;
            int error = 0;
            while (target.Count > 0)
            {
                var kv = target.DeleteMin();
                if (lastMin < kv.Key)
                    error++;
                lastMin = kv.Key;
                Assert.AreEqual(kv.Key + 1, kv.Value);
            }

            Console.WriteLine("error rate: {0}", error / (double)keys.Length);
            Assert.IsTrue(error / (double)keys.Length <= target.ErrorRate);
        }
    }
}
