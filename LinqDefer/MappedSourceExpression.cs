//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Linq.Expressions;

namespace LinqDefer
{
    /// <summary>
    /// An expression identified as 'source' - to be passed on to the underlying provider.  Holds the type
    /// of the expression, as well as the ParameterExpression which replaces it in the post-process lambda.
    /// </summary>
    public class MappedSourceExpression
    {
        public MappedSourceExpression(Type type, Expression sourceExpression, ParameterExpression parameter)
        {
            Type = type;
            SourceExpression = sourceExpression;
            ParameterReplacement = parameter;
        }

        /// <summary>
        /// The expression to be passed to the provider
        /// </summary>
        public Expression SourceExpression { get; set; }

        /// <summary>
        /// The runtime type of expression's evaluable value
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The parameter used in place of the expression in the post-process lambda
        /// </summary>
        public ParameterExpression ParameterReplacement { get; set; }
    }
}