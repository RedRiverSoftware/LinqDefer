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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LinqDefer.ExpressionAnalysers.EntityFramework
{
    /// <summary>
    /// A LinqDefer expression analyser which determines which expressions to pass through to the underlying
    /// LINQ provider.  The EfSupportedAutoAnalyser passes through any expressions that Entity Framework can
    /// handle - all other expressions are evaluated afterwards.
    /// This class is not currently implemented.  The DataAccessOnlyAnalyser can be used and will generate
    /// expressions which work with Entity Framework.
    /// </summary>
    public class EfSupportedAutoAnalyser : IExpressionAnalyser
    {
        /// <summary>
        /// Initialises the analyser by supplying the source parameter - the object whose data
        /// is to be retrieved by the underlying provider.
        /// Currently unimplemented for EfSupportedAutoAnalyser.
        /// </summary>
        /// <param name="sourceParameter">The lambda expression parameter of the main Select expression</param>
        public void Initialise(ParameterExpression sourceParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the supplied expression should be passed to the underlying provider
        /// in its entirety.  If the expression is not a leaf node (if there are children), the
        /// caller can call again on child nodes to determine if part of the expression is
        /// suitable instead.
        /// Currently unimplemented for EfSupportedAutoAnalyser.
        /// </summary>
        /// <param name="node">The path through the expression tree, ending with the node in question</param>
        /// <returns>'true' if the underlying provider should be handed this expression to retrieve</returns>
        public bool ShouldProviderHandleExpression(ExpressionNodePath node)
        {
            throw new NotImplementedException();
        }
    }
}
