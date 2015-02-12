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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace LinqDefer.Tests.Utilities
{
    public class StubQueryProviderProxy : IQueryProvider
    {
        private readonly IQueryProvider _underlyingProvider;
        public List<Type> CreateQueryCalledTypes { get; private set; }

        public StubQueryProviderProxy(IQueryProvider underlyingProvider)
        {
            _underlyingProvider = underlyingProvider;
            CreateQueryCalledTypes = new List<Type>();
        }

        [ExcludeFromCodeCoverage]
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            CreateQueryCalledTypes.Add(typeof(TElement));
            return _underlyingProvider.CreateQuery<TElement>(expression);
        }

        [ExcludeFromCodeCoverage]
        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}