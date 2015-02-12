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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.UnitTests
{
    [TestClass]
    public class DeferPostProcessEnumeratorTests
    {
        [TestMethod]
        public void Basic_postprocessing_works()
        {
            var sourceData = TestData.Sample.Select(s => s.FirstName).ToArray();
            var expected = sourceData.Select(d => new string(d.Reverse().ToArray()));

            Expression<Func<string, string>> process = (s) => new string(s.Reverse().ToArray());

            var processingEnum = new DeferPostProcessEnumerator<string>(sourceData.GetEnumerator(), process);

            var results = processingEnum.ToArray();
            var resultsAsString = results.Cast<string>().ToArray();

            AssertExtensions.AreCollectionsEquivalent(resultsAsString, expected);
        }

        [TestMethod]
        public void Reset_enumerator_works()
        {
            var sourceData = TestData.Sample.Select(s => s.FirstName).ToArray();
            var expected = sourceData.Select(d => new string(d.Reverse().ToArray()));

            Expression<Func<string, string>> process = (s) => new string(s.Reverse().ToArray());

            var processingEnum = new DeferPostProcessEnumerator<string>(sourceData.GetEnumerator(), process);

            processingEnum.MoveNext();
            processingEnum.Reset();

            var results = processingEnum.ToArray();
            var resultsAsString = results.Cast<string>().ToArray();

            AssertExtensions.AreCollectionsEquivalent(resultsAsString, expected);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Current_when_ended_throws_invalidoperationexception()
        {
            var sourceData = TestData.Sample.Select(s => s.FirstName).ToArray();
            var expected = sourceData.Select(d => new string(d.Reverse().ToArray()));

            Expression<Func<string, string>> process = (s) => new string(s.Reverse().ToArray());

            var processingEnum = new DeferPostProcessEnumerator<string>(sourceData.GetEnumerator(), process);

            while (processingEnum.MoveNext()) { }

            var finalInvalidItem = processingEnum.Current;
        }
    }
}
