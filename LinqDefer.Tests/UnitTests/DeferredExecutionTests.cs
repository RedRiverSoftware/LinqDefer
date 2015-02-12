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
using LinqDefer.Helpers;
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqDefer.Tests.UnitTests
{
    [TestClass]
    public class DeferredExecutionTests
    {
        [TestMethod]
        [ExpectedException(typeof(LinqDeferException))]
        public void Exception_in_post_process_throws_LinqDeferException()
        {
            // arrange: analyser which says underlying provider should do nothing (everything is deferred),
            // and query to cause exception to be thrown
            Mock<IExpressionAnalyser> mockAnalyser;
            var config = MockQueryableHelper.CreateConfigWithMockRejectingAnalyser(out mockAnalyser);

            var query = TestData.Sample.AsQueryable()
                .Select(d => ExceptionThrower.Throw());

            var stubProvider = new StubQueryProviderProxy(query.Provider);
            var mockQueryable = MockQueryableHelper.CreateMockQueryable(query, stubProvider);

            // act: apply LinqDefer to the mocked query/provider stub
            var queryWithDefer = mockQueryable.Object.ThenDoDeferred(config);

            // final act & assert
            var results = queryWithDefer.ToArray();
        }

        [TestMethod]
        public void Deferred_method_calls_occur_after_main_query()
        {
            // arrange: analyser which says underlying provider should only handle getting the GetNextA
            // method call to our test type, and stub provider to record which type is requested to be 
            // retrieved by that provider
            Mock<IExpressionAnalyser> mockAnalyser;
            var config = MockQueryableHelper.CreateConfigWithMockRejectingAnalyser(out mockAnalyser);
            var sequenceGen = new SimpleSequenceGenerator();

            mockAnalyser.Setup(a =>
                a.ShouldProviderHandleExpression(It.Is<ExpressionNodePath>(e =>
                    e.Current.Expression is MethodCallExpression && ((MethodCallExpression)e.Current.Expression).Method.Name == "GetNextA")))
                    .Returns(true);

            var query = TestData.Sample.AsQueryable()
                .Select(d => new ThreePropertyClass<int>
                {
                    PropOne = sequenceGen.GetNextA(),
                    PropTwo = sequenceGen.GetNextB(),
                    PropThree = sequenceGen.GetNextA()
                });

            var stubProvider = new StubQueryProviderProxy(query.Provider);
            var mockQueryable = MockQueryableHelper.CreateMockQueryable(query, stubProvider);

            // act: apply LinqDefer to the mocked query/provider stub
            var queryWithDefer = mockQueryable.Object.ThenDoDeferred(config);
            var results = queryWithDefer.ToArray();

            // assert: the underlying mocked provider was asked for two fields precisely, both int ones;
            // also that the Two member of each result is greater than One and Three, indicating it was
            // executed after the GetNextA calls passed to the underlying provider
            Assert.AreEqual(1, stubProvider.CreateQueryCalledTypes.Count);
            var members = stubProvider.CreateQueryCalledTypes[0].GetFields();
            Assert.AreEqual(2, members.Length);
            Assert.IsTrue(members.All(m => m.FieldType == typeof(int)));
            foreach (var result in results)
            {
                Assert.IsTrue(result.PropTwo > result.PropOne);
                Assert.IsTrue(result.PropTwo > result.PropThree);
            }
        }
    }
}
