//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using LinqDefer.ExpressionAnalysers;
using LinqDefer.ExpressionAnalysers.DataAccessOnly;

namespace LinqDefer
{
    /// <summary>
    /// Class used to config LinqDefer - for example, the analyser to be used to determine
    /// which expressions are deferred.
    /// </summary>
    public class LinqDeferConfiguration
    {
        private static readonly LinqDeferConfiguration DefaultConfiguration;

        /// <summary>
        /// The default LinqDefer configuration - uses the DataAccessOnlyAnalyser
        /// </summary>
        public static LinqDeferConfiguration Default
        {
            get { return DefaultConfiguration; }
        }

        static LinqDeferConfiguration()
        {
            DefaultConfiguration = new LinqDeferConfiguration(() => new DataAccessOnlyAnalyser());
        }

        /// <summary>
        /// A factory function for expression analyser instances
        /// </summary>
        public Func<IExpressionAnalyser> AnalyserFactory { get; private set; }

        /// <summary>
        /// Creates a new LinqDeferConfiguration instance based on the supplied expression analyser factory function
        /// </summary>
        /// <param name="analyserFactory">Function which returns the expression analyser instance to be used</param>
        public LinqDeferConfiguration(Func<IExpressionAnalyser> analyserFactory)
        {
            AnalyserFactory = analyserFactory;
        }
    }
}