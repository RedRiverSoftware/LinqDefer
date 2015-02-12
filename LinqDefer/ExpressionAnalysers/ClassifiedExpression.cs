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
using System.Linq.Expressions;

namespace LinqDefer.ExpressionAnalysers
{
    /// <summary>
    /// Wrapper class holding an Expression, decorated with a list of ExpressionClassification objects indicating
    /// properties of the expression relevant to transformation.
    /// </summary>
    public class ClassifiedExpression
    {
        private readonly List<ExpressionClassification> _classifications;
        private readonly Expression _expression;
 
        public Expression Expression { get { return this._expression; } }

        /// <summary>
        /// Creates a new instance of ClassifiedExpression to wrap the supplied expression
        /// </summary>
        /// <param name="expression">Expression to wrap</param>
        public ClassifiedExpression(Expression expression)
        {
            _expression = expression;
            _classifications = new List<ExpressionClassification>();
        }

        /// <summary>
        /// Tags the expression with a new classification
        /// </summary>
        /// <param name="classification">The expression decorator to tag with</param>
        public void Add(ExpressionClassification classification)
        {
            _classifications.Add(classification);
        }

        /// <summary>
        /// Returns 'true' if the expression has any classifications of the given type
        /// </summary>
        /// <typeparam name="T">Type of classification to be checked</typeparam>
        /// <returns>'true' if the expression has any classifications of the given type</returns>
        public bool HasClassification<T>()
        {
            return GetClassifications<T>().Any();
        }

        /// <summary>
        /// Retrieves the list of classifications for the expression
        /// </summary>
        /// <typeparam name="T">The type of classification decorator objects to be retrieved</typeparam>
        /// <returns>The list of classifications for the expression, of the given type</returns>
        public IEnumerable<T> GetClassifications<T>()
        {
            return _classifications.OfType<T>();
        }
    }
}