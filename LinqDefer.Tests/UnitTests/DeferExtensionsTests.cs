//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.UnitTests
{
    [TestClass]
    public class DeferExtensionsTests
    {
        [TestMethod]
        public void Execute_scalar_query_works()
        {
            var expected = TestData.Sample.Length;
            var query = TestData.Sample.AsQueryable()
                .Select(d => new string(d.FirstName.Reverse().ToArray()))
                .ThenDoDeferred();

            var result = query.Count();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void More_queryable_work_after_deferred_works()
        {
            var expected = TestData.Sample.Select(d => d.FirstName + "!").ToArray();
            var query = TestData.Sample.AsQueryable()
                .Select(d => new string(d.FirstName.Reverse().ToArray()))
                .ThenDoDeferred()
                .Select(d => new string(d.Reverse().ToArray()) + "!");

            var results = query.ToArray();

            AssertExtensions.AreCollectionsEquivalent(expected, results);
        }

        [TestMethod]
        public void Manual_enumeration_of_query_works()
        {
            var expected = TestData.Sample.Select(d => new string(d.FirstName.Reverse().ToArray())).ToArray();
            var query = TestData.Sample.AsQueryable()
                .Select(d => new string(d.FirstName.Reverse().ToArray()))
                .ThenDoDeferred();
            var enumerable = (IEnumerable)query;

            var results = new List<string>();
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                results.Add((string)enumerator.Current);
            }

            AssertExtensions.AreCollectionsEquivalent(expected, results);
        }
    }
}
