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
using System.Text;
using System.Threading.Tasks;

namespace LinqDefer.ExpressionAnalysers
{
    /// <summary>
    /// Represents an expression analyser for LinqDefer;
    /// determines what expressions are passed to the underlying provider
    /// and which are processed after the fact (deferred).
    /// </summary>
    public interface IExpressionAnalyser
    {
        /// <summary>
        /// Initialises the analyser by supplying the source parameter - the object whose data
        /// is to be retrieved by the underlying provider.
        /// </summary>
        /// <param name="sourceParameter">The lambda expression parameter of the main Select expression</param>
        void Initialise(ParameterExpression sourceParameter);

        /// <summary>
        /// Returns true if the supplied expression should be passed to the underlying provider
        /// in its entirety.  If the expression is not a leaf node (if there are children), the
        /// caller can call again on child nodes to determine if part of the expression is
        /// suitable instead.
        /// </summary>
        /// <param name="node">The path through the expression tree, ending with the node in question</param>
        /// <returns>'true' if the underlying provider should be handed this expression to retrieve</returns>
        bool ShouldProviderHandleExpression(ExpressionNodePath node);
    }
}
