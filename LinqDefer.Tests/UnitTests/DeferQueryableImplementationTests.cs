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
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.UnitTests
{
    [TestClass]
    public class DeferQueryableImplementationTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Execute_scalar_query_gives_notsupportedexception()
        {
            var expected = TestData.Sample.Length;
            var query = TestData.Sample.AsQueryable()
                .Select(d => new string(d.FirstName.Reverse().ToArray()));

            query = DeferProvider.Wrap(query, LinqDeferConfiguration.Default);

            var result = query.Count();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Direct_execute_untyped_scalar_query_gives_notsupportedexception()
        {
            var expected = TestData.Sample.Select(d => d.FirstName + "!").ToArray();
            var query = TestData.Sample.AsQueryable()
                .Select(d => d.FirstName + "!");

            var nonTypedQuery = DeferProvider.Wrap((IQueryable)query, LinqDeferConfiguration.Default);

            nonTypedQuery.Provider.Execute(nonTypedQuery.Expression);
        }

        [TestMethod]
        public void Create_untyped_query_works()
        {
            var expected = TestData.Sample.Select(d => d.FirstName + "!").ToArray();
            var query = TestData.Sample.AsQueryable()
                .Select(d => d.FirstName + "!");

            var nonTypedQuery = DeferProvider.Wrap((IQueryable)query, LinqDeferConfiguration.Default);

            var results = nonTypedQuery.GetEnumerator().ToArray();
            var stringArrResults = results.Cast<string>().ToArray();

            AssertExtensions.AreCollectionsEquivalent(expected, stringArrResults);
        }

        [TestMethod]
        public void More_queryable_work_after_deferred_works()
        {
            var expected = TestData.Sample.Select(d => d.FirstName + "!").ToArray();
            var query = TestData.Sample.AsQueryable()
                .Select(d => new string(d.FirstName.Reverse().ToArray()));

            query = DeferProvider.Wrap(query, LinqDeferConfiguration.Default)
                .Select(d => new string(d.Reverse().ToArray()) + "!");

            var results = query.ToArray();

            AssertExtensions.AreCollectionsEquivalent(expected, results);
        }

        [TestMethod]
        public void Query_elementtype_is_correct()
        {
            var expected = TestData.Sample.Select(d => d.FirstName + "!").ToArray();
            var query = TestData.Sample.AsQueryable()
                .Select(d => new string(d.FirstName.Reverse().ToArray()));

            query = DeferProvider.Wrap(query, LinqDeferConfiguration.Default);

            Assert.AreEqual(typeof(string), query.ElementType);
        }

        [TestMethod]
        public void Manual_enumeration_of_query_works()
        {
            var expected = TestData.Sample.Select(d => new string(d.FirstName.Reverse().ToArray())).ToArray();
            var query = TestData.Sample.AsQueryable()
                .Select(d => new string(d.FirstName.Reverse().ToArray()));

            query = DeferProvider.Wrap(query, LinqDeferConfiguration.Default);
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
