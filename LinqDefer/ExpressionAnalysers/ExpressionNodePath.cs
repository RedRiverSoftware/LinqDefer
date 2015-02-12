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

namespace LinqDefer.ExpressionAnalysers
{
    /// <summary>
    /// Represents a node path through an expression tree - a specific node
    /// and its ancestors, back to the root
    /// </summary>
    public class ExpressionNodePath : List<ClassifiedExpression>
    {
        /// <summary>
        /// The current node for the path
        /// </summary>
        public ClassifiedExpression Current { get { return Count >= 1 ? this[0] : null; } }

        /// <summary>
        /// The current parent node for the path
        /// </summary>
        public ClassifiedExpression Parent { get { return Count >= 2 ? this[1] : null; } }

        /// <summary>
        /// Returns 'true' if any ancestor of the current node has the given classification.
        /// Does not check the current node itself.
        /// </summary>
        /// <typeparam name="T">Type of classification to be checked</typeparam>
        /// <returns>'true' if any ancestor of the current node has the given classification</returns>
        public bool AncestorHasClassification<T>()
        {
            return this.Skip(1).Any(p => p.HasClassification<T>());
        }
    }
}