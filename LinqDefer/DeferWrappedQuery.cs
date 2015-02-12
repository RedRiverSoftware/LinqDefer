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

namespace LinqDefer
{
    /// <summary>
    /// Holds information about a Defer-wrapped query.
    /// Actual query execution logic coordinated by DeferProvider.ExecuteQuery.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeferWrappedQuery<T> : IOrderedQueryable<T>
    {
        private readonly DeferProvider _provider;
        private readonly Expression _expression;

        /// <summary>
        /// The untransformed source expression
        /// </summary>
        public Expression Expression { get { return this._expression; } }

        /// <summary>
        /// The provider (an instance of DeferProvider)
        /// </summary>
        public IQueryProvider Provider { get { return this._provider; } }

        /// <summary>
        /// The type of element returned by the query
        /// </summary>
        public Type ElementType { get { return typeof(T); } }

        /// <summary>
        /// Creates a new instance of DeferWrappedQuery
        /// </summary>
        /// <param name="provider">The DeferProvider instance to use</param>
        /// <param name="expression">The untransformed source expression</param>
        public DeferWrappedQuery(DeferProvider provider, Expression expression)
        {
            _provider = provider;
            _expression = expression;
        }

        /// <summary>
        /// Gets an enumerator for query results; causes query execution
        /// </summary>
        /// <returns>A typed generic enumerator for results</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._provider.ExecuteQuery<T>(this._expression);
        }

        /// <summary>
        /// Gets an enumerator for query results; causes query execution
        /// </summary>
        /// <returns>An untyped enumerator for results</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._provider.ExecuteQuery<T>(this._expression);
        }
    }
}