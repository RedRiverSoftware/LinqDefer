//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.Utilities
{
    public static class AssertExtensions
    {
        public static void AreCollectionsEquivalent<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            var arr1 = first.ToArray();
            var arr2 = second.ToArray();

            Assert.AreEqual(arr1.Length, arr2.Length);

            for (int i = 0; i < arr1.Length; i++)
            {
                Assert.AreEqual(arr1[i], arr2[i]);
            }
        }
    }
}
