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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LinqDefer.ExpressionAnalysers.DataAccessOnly
{
    /// <summary>
    /// A LinqDefer expression analyser which determines which expressions to pass through to the underlying
    /// LINQ provider.  The DataAccessOnlyAnalyser only asks the underlying provider to retrieve the source object,
    /// a property of it, a non-static method call on it, an array index on it, or some combination of those
    /// operations.
    /// </summary>
    public class DataAccessOnlyAnalyser : IExpressionAnalyser
    {
        private readonly Dictionary<Expression, bool> _validityCache;
        private ParameterExpression _sourceParameter;

        /// <summary>
        /// Creates a new instance of the DataAccessOnlyAnalyser class
        /// </summary>
        public DataAccessOnlyAnalyser()
        {
            _validityCache = new Dictionary<Expression, bool>();
        }

        /// <summary>
        /// Initialises the analyser by supplying the source parameter - the object whose data
        /// is to be retrieved by the underlying provider.
        /// </summary>
        /// <param name="sourceParameter">The lambda expression parameter of the main Select expression</param>
        public void Initialise(ParameterExpression sourceParameter)
        {
            _sourceParameter = sourceParameter;
        }

        /// <summary>
        /// Returns true if the supplied expression should be passed to the underlying provider
        /// in its entirety.  If the expression is not a leaf node (if there are children), the
        /// caller can call again on child nodes to determine if part of the expression is
        /// suitable instead.
        /// </summary>
        /// <param name="node">The path through the expression tree, ending with the node in question</param>
        /// <returns>'true' if the underlying provider should be handed this expression to retrieve</returns>
        public bool ShouldProviderHandleExpression(ExpressionNodePath node)
        {
            var isDataAccessExpr = IsDataAccessExpression(node.Current.Expression);

            return isDataAccessExpr;
        }

        private bool IsSourceParameterExpression(Expression expression)
        {
            return expression == _sourceParameter;
        }

        private bool IsValidMemberAccessExpresion(Expression expression)
        {
            var me = expression as MemberExpression;
            if (me == null)
            {
                return false;
            }
            return IsDataAccessExpression(me.Expression);
        }

        private bool IsValidObjectMethodCallExpression(Expression expression)
        {
            var mce = expression as MethodCallExpression;
            if (mce == null)
            {
                return false;
            }
            return IsDataAccessExpression(mce.Object);
        }

        private bool IsValidArrayIndexExpression(Expression expression)
        {
            var sbe = expression as BinaryExpression;
            if (sbe != null
                && expression.NodeType == ExpressionType.ArrayIndex
                && IsDataAccessExpression(sbe.Left) 
                && (IsDataAccessExpression(sbe.Right) || IsConstantExpression(sbe.Right))
                )
            {
                return true;
            }

            return false;
        }

        private bool IsConstantExpression(Expression expression)
        {
            var ce = expression as ConstantExpression;
            if (ce == null)
            {
                return false;
            }
            return true;
        }

        private bool IsDataAccessExpression(Expression expression)
        {
            if (expression == null)
            {
                return false;
            }

            bool isValid;
            if (!_validityCache.TryGetValue(expression, out isValid))
            {
                _validityCache[expression] = isValid =
                    IsSourceParameterExpression(expression)
                    || IsValidMemberAccessExpresion(expression)
                    || IsValidObjectMethodCallExpression(expression)
                    || IsValidArrayIndexExpression(expression);
            }

            return isValid;
        }
    }
}
