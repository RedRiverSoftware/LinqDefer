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
using AutoMapper.QueryableExtensions;
using LinqDefer.RuntimeTypes;
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.FunctionalTests
{
    [TestClass]
    public class AutoMapperProjectToTests
    {
        [TestMethod]
        public void AutoMapper_projectto_expression_transformed_to_single_intermediate_type()
        {
            // configure automapper
            AutoMapper.Mapper.CreateMap<TestData, FormattedName>()
                .ForMember(f => f.DisplayName, m => m.MapFrom(t => string.Format("{0} {1}", t.FirstName, t.LastName)))
                .ForMember(f => f.IndexedName, m => m.MapFrom(t => string.Format("{0}, {1}", t.LastName.ToUpper(), t.FirstName)));
            AutoMapper.Mapper.AssertConfigurationIsValid();

            var query = TestData.Sample.AsQueryable()
                .Project().To<FormattedName>();

            // build queryable provider stub to monitor what's passed through to underlying provider
            var stubProvider = new StubQueryProviderProxy(query.Provider);
            var mockQueryable = MockQueryableHelper.CreateMockQueryable(query, stubProvider);

            // act: apply LinqDefer to the mocked query/provider stub
            var queryWithDefer = mockQueryable.Object.ThenDoDeferred();
            var results = queryWithDefer.ToArray();

            // assert: the underlying mocked provider was only asked for our intermediate type
            Assert.AreEqual(1, stubProvider.CreateQueryCalledTypes.Count);
            Assert.IsTrue(stubProvider.CreateQueryCalledTypes[0].Assembly.FullName == IntermediateTypeCache.AssemblyFullName);

            var expectedResults =
                TestData.Sample.Select(AutoMapper.Mapper.Map<TestData, FormattedName>).ToArray();

            AssertExtensions.AreCollectionsEquivalent(results, expectedResults);
        }
    }
}
