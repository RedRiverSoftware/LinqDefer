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
using System.Linq.Expressions;

namespace LinqDefer
{
    /// <summary>
    /// Holds information about the LinqDefer expression transformation process outcome
    /// </summary>
    public class DeferTransformation
    {
        public DeferTransformation()
        {
            IdentifiedSourceExpressions = new List<MappedSourceExpression>();
        }

        /// <summary>
        /// The result of the transformation - suitable for execution by the underlying provider
        /// </summary>
        public Expression TransformedExpression { get; set; }

        /// <summary>
        /// The type that LinqDefer created to hold the intermediate results yielded by the underlying provider
        /// </summary>
        public Type IntermediateItemType { get; set; }

        /// <summary>
        /// The main Select() method call
        /// </summary>
        public MethodCallExpression MainSelectCall { get; set; }

        /// <summary>
        /// The lambda parameter indicating the source object in the main Select() call
        /// </summary>
        public ParameterExpression MainSelectSourceParameter { get; set; }

        /// <summary>
        /// The lambda body from the main Select() call (typically a NewExpression)
        /// </summary>
        public Expression MainSelectLambdaBody { get; set; }

        /// <summary>
        /// The set of expressions referring to source data found in the main Select() lambda
        /// and their according replacement ParameterExpressions
        /// </summary>
        public List<MappedSourceExpression> IdentifiedSourceExpressions { get; private set; }

        /// <summary>
        /// The original main Select() lambda, with parameters where necessary
        /// </summary>
        public LambdaExpression PostProcessLambda { get; set; }
    }
}