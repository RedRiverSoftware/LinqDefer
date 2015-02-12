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
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.FunctionalTests
{
    [TestClass]
    public class SimpleObjectCompositionTests
    {
        [TestMethod]
        public void Format_name_with_string_format()
        {
            var sourceData = TestData.Sample;

            var query = sourceData.AsQueryable()
                .Select(n => string.Format("{0}, {1}", n.LastName, n.FirstName))
                .ThenDoDeferred();

            var results = query.ToArray();

            var expected = sourceData.Select(n => string.Format("{0}, {1}", n.LastName, n.FirstName)).ToArray();

            AssertExtensions.AreCollectionsEquivalent(expected, results);
        }
    }
}
