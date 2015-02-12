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
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqDefer.ExpressionAnalysers;
using LinqDefer.Helpers;

namespace LinqDefer
{
    public class DeferProvider : IQueryProvider
    {
        private readonly IQueryProvider _innerProvider;
        private readonly LinqDeferConfiguration _config;

        private DeferProvider(IQueryProvider innerProvider, LinqDeferConfiguration config)
        {
            _innerProvider = innerProvider;
            _config = config;
        }

        /// <summary>
        /// Wraps and processes the supplied query for deferred evaluation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IQueryable<T> Wrap<T>(IQueryable<T> query, LinqDeferConfiguration config)
        {
            var provider = new DeferProvider(query.Provider, config);
            var wrappedQuery = provider.CreateQuery<T>(query.Expression);
            return wrappedQuery;
        }

        /// <summary>
        /// Wraps and processes the supplied query for deferred evaluation
        /// </summary>
        /// <param name="query"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IQueryable Wrap(IQueryable query, LinqDeferConfiguration config)
        {
            var provider = new DeferProvider(query.Provider, config);
            var wrappedQuery = provider.CreateQuery(query.Expression);
            return wrappedQuery;
        }

        /// <summary>
        /// Creates a query which wraps and processes the supplied expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IQueryable CreateQuery(Expression expression)
        {
            // just build a constructor for DeferWrappedQuery for the appropriate type, invoke,
            // and return the result.
            var collectionType = TypeHelper.FindIEnumerable(expression.Type);
            var wrappedQueryType = typeof(DeferWrappedQuery<>).MakeGenericType(collectionType.GenericTypeArguments[0]);
            var constructorArgs = new object[] { this, expression };

            var ctor = wrappedQueryType.GetConstructor(
                bindingAttr: BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(DeferProvider), typeof(Expression) },
                modifiers: null);

            return (IQueryable)ctor.Invoke(constructorArgs);
        }

        /// <summary>
        /// Creates a query which, when executed, will wrap and run the supplied expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new DeferWrappedQuery<T>(this, expression);
        }

        /// <summary>
        /// Wraps and executes the given expression and returns the result
        /// Used for non-enumerable returns like Count().
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object Execute(Expression expression)
        {
            throw new NotSupportedException("DeferProvider does not support non-enumerable execution.  Use .ThenDoDeferred() to force materialisation then call further methods on that result");
        }

        /// <summary>
        /// Wraps and executes the given expression and returns the result.
        /// Used for non-enumerable returns like Count().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T Execute<T>(Expression expression)
        {
            throw new NotSupportedException("DeferProvider does not support non-enumerable execution.  Use .ThenDoDeferred() to force materialisation then call further methods on that result");
        }

        /// <summary>
        /// Wraps and executes the supplied expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerator<T> ExecuteQuery<T>(Expression expression)
        {
            // transform the expression into the intermediate query, building the transformation
            // object at the same time for postprocessing
            var transformation = DeferExpressionTransformer.Transform(expression, _config.AnalyserFactory);

            // create the inner query, which produces the intermediate objects
            var createQueryMethod = typeof(IQueryProvider).GetGenericMethod("CreateQuery", BindingFlags.Instance | BindingFlags.Public, new[] { transformation.IntermediateItemType }, new[] { typeof(Expression) });
            var intermediateQuery = createQueryMethod.Invoke(_innerProvider, new object[] { transformation.TransformedExpression });

            // get the enumerator for the results of the inner query
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(transformation.IntermediateItemType);
            var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");
            var intermediateEnumerator = getEnumeratorMethod.Invoke(intermediateQuery, new object[] { });

            // wrap the inner enumerator with our post-processing enumerator and we're done
            var postProcessEnumerator = new DeferPostProcessEnumerator<T>((IEnumerator)intermediateEnumerator, transformation.PostProcessLambda);
            return postProcessEnumerator;
        }
    }
}