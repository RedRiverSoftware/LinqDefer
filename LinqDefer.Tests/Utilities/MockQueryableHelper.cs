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
using LinqDefer.ExpressionAnalysers;
using Moq;

namespace LinqDefer.Tests.Utilities
{
    public class MockQueryableHelper
    {
        public static LinqDeferConfiguration CreateConfigWithMockRejectingAnalyser(out Mock<IExpressionAnalyser> mockAnalyser)
        {
            var analyser = new Mock<IExpressionAnalyser>();
            analyser.Setup(a => a.ShouldProviderHandleExpression(It.IsAny<ExpressionNodePath>())).Returns(false);
            var config = new LinqDeferConfiguration(() => analyser.Object);
            mockAnalyser = analyser;
            return config;
        }

        public static Mock<IQueryable<T>> CreateMockQueryable<T>(IQueryable<T> query, StubQueryProviderProxy stubProvider)
        {
            var mockQueryable = new Mock<IQueryable<T>>();

            mockQueryable.Setup(r => r.GetEnumerator()).Returns(query.GetEnumerator());
            mockQueryable.Setup(r => r.Provider).Returns(stubProvider);
            mockQueryable.Setup(r => r.ElementType).Returns(query.ElementType);
            mockQueryable.Setup(r => r.Expression).Returns(query.Expression);
            return mockQueryable;
        }
    }
}
