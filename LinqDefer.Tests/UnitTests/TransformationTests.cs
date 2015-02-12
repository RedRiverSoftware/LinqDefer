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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LinqDefer.Exceptions;
using LinqDefer.ExpressionAnalysers;
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqDefer.Tests.UnitTests
{
    [TestClass]
    public class TransformationTests
    {
        [TestMethod]
        [ExpectedException(typeof(LinqDeferException))]
        public void Throws_exception_if_select_not_final_clause()
        {
            // arrange: analyser which says underlying provider shouldn't handle any expressions,
            // stub provider to record which type is requested to be retrieved by that provider
            Mock<IExpressionAnalyser> mockAnalyser;
            var config = MockQueryableHelper.CreateConfigWithMockRejectingAnalyser(out mockAnalyser);

            var query = TestData.Sample.AsQueryable()
                .Select(d => DateTime.Now)
                .Where(d => (d.Ticks % 10) == 0);

            var stubProvider = new StubQueryProviderProxy(query.Provider);
            var mockQueryable = MockQueryableHelper.CreateMockQueryable(query, stubProvider);

            // act: apply LinqDefer to the mocked query/provider stub
            mockQueryable.Object.ThenDoDeferred(config);
        }

        [TestMethod]
        public void Passes_through_nothing_if_no_source_expressions()
        {
            // arrange: analyser which says underlying provider shouldn't handle any expressions,
            // stub provider to record which type is requested to be retrieved by that provider
            Mock<IExpressionAnalyser> mockAnalyser;
            var config = MockQueryableHelper.CreateConfigWithMockRejectingAnalyser(out mockAnalyser);

            var query = TestData.Sample.AsQueryable()
                .Select(d => DateTime.Now);

            var stubProvider = new StubQueryProviderProxy(query.Provider);
            var mockQueryable = MockQueryableHelper.CreateMockQueryable(query, stubProvider);

            // act: apply LinqDefer to the mocked query/provider stub
            var queryWithDefer = mockQueryable.Object.ThenDoDeferred(config);
            var results = queryWithDefer.ToArray();

            // assert: the underlying mocked provider wasn't asked for any data to be retrieved
            Assert.AreEqual(1, stubProvider.CreateQueryCalledTypes.Count);
            Assert.AreEqual(0, stubProvider.CreateQueryCalledTypes[0].GetFields().Length);
        }

        [TestMethod]
        public void Passes_through_entirety_if_just_source_expression()
        {
            // arrange: analyser which says underlying provider should only handle getting the
            // Now property value, and stub provider to record which type is requested to be 
            // retrieved by that provider
            Mock<IExpressionAnalyser> mockAnalyser;
            var config = MockQueryableHelper.CreateConfigWithMockRejectingAnalyser(out mockAnalyser);

            mockAnalyser.Setup(a =>
                a.ShouldProviderHandleExpression(It.Is<ExpressionNodePath>(e =>
                    e.Current.Expression is MemberExpression && ((MemberExpression)e.Current.Expression).Member.Name == "Now")))
                    .Returns(true);

            var query = TestData.Sample.AsQueryable()
                .Select(d => DateTime.Now);

            var stubProvider = new StubQueryProviderProxy(query.Provider);
            var mockQueryable = MockQueryableHelper.CreateMockQueryable(query, stubProvider);

            // act: apply LinqDefer to the mocked query/provider stub
            var queryWithDefer = mockQueryable.Object.ThenDoDeferred(config);
            var results = queryWithDefer.ToArray();

            // assert: the underlying mocked provider was asked for one field precisely, a DateTime one
            Assert.AreEqual(1, stubProvider.CreateQueryCalledTypes.Count);
            var members = stubProvider.CreateQueryCalledTypes[0].GetFields();
            Assert.AreEqual(1, members.Length);
            Assert.IsTrue(members[0].FieldType == typeof(DateTime));
        }

        [TestMethod]
        public void Passes_through_just_source_expression()
        {
            // arrange: analyser which says underlying provider should only handle getting the
            // Now property value, and stub provider to record which type is requested to be 
            // retrieved by that provider
            Mock<IExpressionAnalyser> mockAnalyser;
            var config = MockQueryableHelper.CreateConfigWithMockRejectingAnalyser(out mockAnalyser);

            mockAnalyser.Setup(a =>
                a.ShouldProviderHandleExpression(It.Is<ExpressionNodePath>(e =>
                    e.Current.Expression is MemberExpression && ((MemberExpression)e.Current.Expression).Member.Name == "Now")))
                    .Returns(true);

            var query = TestData.Sample.AsQueryable()
                .Select(d => new Tuple<DateTime, int>(DateTime.Now, 123));

            var stubProvider = new StubQueryProviderProxy(query.Provider);
            var mockQueryable = MockQueryableHelper.CreateMockQueryable(query, stubProvider);

            // act: apply LinqDefer to the mocked query/provider stub
            var queryWithDefer = mockQueryable.Object.ThenDoDeferred(config);
            var results = queryWithDefer.ToArray();

            // assert: the underlying mocked provider was asked for one field precisely, a DateTime one
            Assert.AreEqual(1, stubProvider.CreateQueryCalledTypes.Count);
            var members = stubProvider.CreateQueryCalledTypes[0].GetFields();
            Assert.AreEqual(1, members.Length);
            Assert.IsTrue(members[0].FieldType == typeof(DateTime));
        }
    }
}
