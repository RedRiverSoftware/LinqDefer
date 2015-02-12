//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqDefer
{
    /// <summary>
    /// Extension methods for LinqDefer, allowing deferred execution of parts of a LINQ
    /// query after an underlying provider has retrieved the data it requires to be
    /// evaluated.
    /// </summary>
    public static class LinqDeferExtensions
    {
        /// <summary>
        /// Causes the preceeding query expression to be searched for instances of calls to
        /// Defer.Eval() and transformed.
        /// The query will run as usual, but with expressions with Defer.Eval() being evaluated
        /// after the required information is retrieved using the underlying provider.
        /// This method performs materialisation (the query will be run).  The underlying IQueryable
        /// can be retrieved by using DeferProvider.Wrap directly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IEnumerable<T> ThenDoDeferred<T>(this IQueryable<T> query, LinqDeferConfiguration config = null)
        {
            config = config ?? LinqDeferConfiguration.Default;

            var wrappedQuery = DeferProvider.Wrap(query, config);
            var materialisedResults = wrappedQuery.ToList();

            return materialisedResults;
        }
    }
}
