//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqDefer.Exceptions;
using LinqDefer.ExpressionAnalysers;
using LinqDefer.Helpers;
using LinqDefer.RuntimeTypes;

namespace LinqDefer
{
    /// <summary>
    /// Expression transformation class which identifies and extracts parts of a LINQ query which are
    /// determined by an IExpressionAnalyser as unsuitable.  Use the static Transform method.
    /// </summary>
    internal class DeferExpressionTransformer : ExpressionVisitor
    {
        private const string SelectMethodName = "Select";
        private readonly DeferTransformation _transformation;
        private readonly ExpressionNodePath _currentNodePath;
        private readonly IExpressionAnalyser _analyser;

        /// <summary>
        /// Analyses the provided expression and returns a DeferTransformation instance containing
        /// information necessary to defer execution of expressions identified as unsuitable by the
        /// supplied IExpressionAnalyser
        /// </summary>
        /// <param name="expression">The LINQ query expression to be analysed</param>
        /// <param name="analyserFactory">A factory lambda used to retrieve an instance of the desired IExpressionAnalyser</param>
        /// <returns>The results of the transformation process</returns>
        public static DeferTransformation Transform(Expression expression, Func<IExpressionAnalyser> analyserFactory)
        {
            var analyser = analyserFactory();
            var instance = new DeferExpressionTransformer(analyser);

            instance._transformation.TransformedExpression = instance.Visit(expression);

            return instance._transformation;
        }

        private DeferExpressionTransformer(IExpressionAnalyser analyser)
        {
            _analyser = analyser;
            _transformation = new DeferTransformation();
            _currentNodePath = new ExpressionNodePath();
        }

        /// <summary>
        /// Analyses and transforms the supplied expression node
        /// </summary>
        /// <param name="node">Expression/node to be analysed</param>
        /// <returns>The transformed expression</returns>
        public override Expression Visit(Expression node)
        {
            var classifiedExpr = new ClassifiedExpression(node);
            _currentNodePath.Insert(0, classifiedExpr);

            try
            {
                // if we're under the main select, check with the analyser if the expression should be 
                // passed on; if so, no need to visit children, just replace with a parameter
                if (this._currentNodePath.AncestorHasClassification<MainSelectCallClassification>())
                {
                    if (ShouldProviderHandleExpression(_currentNodePath))
                    {
                        var sourceExpr = RegisterSourceExpression(0);
                        return sourceExpr.ParameterReplacement;
                    }
                }

                // not a source experssion, keep processing..
                var result = base.Visit(node);
                return result;
            }
            finally
            {
                _currentNodePath.RemoveAt(0);
            }
        }

        private bool ShouldProviderHandleExpression(ExpressionNodePath nodePath)
        {
            // first, check if we want to filter this out from the analyser

            // case for exclusion: expression is a parameter to a lambda
            if (nodePath.Count > 1 && nodePath[1].Expression is LambdaExpression &&
                ((LambdaExpression) nodePath[1].Expression).Parameters.Contains(nodePath[0].Expression))
            {
                return false;
            }

            // finally, check with analyser
            return this._analyser.ShouldProviderHandleExpression(nodePath);
        }

        private void ClassifyCurrentExpression(ExpressionClassification classification)
        {
            _currentNodePath[0].Add(classification);
        }

        private bool IsQueryableSelectMethod(MethodInfo method)
        {
            return method.DeclaringType == typeof(Queryable) && method.Name == SelectMethodName;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (IsQueryableSelectMethod(node.Method) && !_currentNodePath.AncestorHasClassification<MainSelectCallClassification>())
            {
                if (_currentNodePath.Count != 1)
                {
                    throw new LinqDeferException(".Select() must be the final part of the LINQ expression before .ThenDoDeferred()");
                }
                return TransformMainSelectMethodCall(node);
            }

            return base.VisitMethodCall(node);
        }

        private MappedSourceExpression RegisterSourceExpression(int sourceExpressionIndex)
        {
            var sourceExpression = _currentNodePath[sourceExpressionIndex];
            sourceExpression.Add(new SourceExpressionClassification());

            // note, the source expression identified here will be replaced with a ParameterExpression
            // by the final part of the main Visit method

            var numExpressions = _transformation.IdentifiedSourceExpressions.Count;
            var newParameterName = string.Format("source_expr_{0}", numExpressions);
            var parameter = Expression.Parameter(sourceExpression.Expression.Type, newParameterName);
            var sourceExpr = new MappedSourceExpression(sourceExpression.Expression.Type, sourceExpression.Expression, parameter);
            _transformation.IdentifiedSourceExpressions.Add(sourceExpr);
            return sourceExpr;
        }

        private LambdaExpression ValidateAndGetLambda(MethodCallExpression node)
        {
            // first parameter to select is the source expression, second is the lambda in Quote (Unary) form
            // note: it does not seem possible to create a LambaExpression which is not automatically wrapped in
            // a UnaryExpression (quote); thus, this is just a Debug.Assert
            var quotedLambda = node.Arguments[1] as UnaryExpression;
            Debug.Assert(quotedLambda != null, "Argument to Select MethodCallException was not a Quote (UnaryExpression)");

            // note: it does not seem possible to create a QuoteExpression which does not have
            // a LambdaExpression as an operand; thus, this is just a Debug.Assert
            var lambda = quotedLambda.Operand as LambdaExpression;
            Debug.Assert(lambda != null, "Operand of Quote was not Lambda");

            // note: it does not seem possible to create a call to the Queryable.Select overload which does not
            // have a lambda with one parameter; thus, this is just a Debug.Assert
            Debug.Assert(lambda.Parameters.Count == 1, "Expected lambda in main Select() call to have single parameter");

            return lambda;
        }

        private Expression TransformMainSelectMethodCall(MethodCallExpression node)
        {
            ClassifyCurrentExpression(new MainSelectCallClassification());

            _transformation.MainSelectCall = node;

            var lambda = ValidateAndGetLambda(node);
            var sourceItemType = lambda.Parameters[0].Type;
            _transformation.MainSelectSourceParameter = lambda.Parameters[0];
            _transformation.MainSelectLambdaBody = lambda.Body;

            // initialise the analyser with the source parameter
            _analyser.Initialise(_transformation.MainSelectSourceParameter);

            // visit children to collect identified source expressions, deferred member expressions
            var transformedSelectCall = (MethodCallExpression)base.VisitMethodCall(node);
            
            // create the final post-process labmda: the original expression with parameters in place of
            // the required data, with parameters for the source data
            var finalLambda = ValidateAndGetLambda(transformedSelectCall);
            var parameterisedLambda = Expression.Quote(Expression.Lambda(finalLambda.Body,
                _transformation.IdentifiedSourceExpressions.Select(e => e.ParameterReplacement)));
            
            // create intermediate type to hold source expression values (everything we need to eval later)
            var sourceExprTypes = _transformation.IdentifiedSourceExpressions.Select(e => e.Type);
            var intermediateType = IntermediateTypeCache.GetType(sourceExprTypes);
            _transformation.IntermediateItemType = intermediateType.Type;

            // combine the intermediate type fields array with the source expressions to get pairs
            var fieldsAndExpressions = intermediateType.Fields.Zip(_transformation.IdentifiedSourceExpressions,
                (f, e) => new Tuple<FieldInfo, MappedSourceExpression>(f, e)).ToArray();

            // build another lambda to call the first, extracting parameter values from the intermediate object
            var intermediateParam = Expression.Parameter(intermediateType.Type, "intermediate");
            var body = Expression.Invoke(parameterisedLambda,
                fieldsAndExpressions.Select(t => Expression.Field(intermediateParam, t.Item1)));
            _transformation.PostProcessLambda = Expression.Lambda(body, false, intermediateParam);

            // now we build a new Select() expression which will create new instances of our intermediate type
            // holding all the values we need to do defered eval afterwards
            var newExpr = Expression.New(intermediateType.Type);
            var initExpr = Expression.MemberInit(
                newExpr,
                fieldsAndExpressions.Select(p => Expression.Bind(p.Item1, p.Item2.SourceExpression)));
            var selectLambda = Expression.Lambda(initExpr, lambda.TailCall, lambda.Parameters);
            var quotedLambda = Expression.Quote(selectLambda);

            var newSelectMethod = typeof(Queryable).GetGenericMethod(
                "Select",
                BindingFlags.Public | BindingFlags.Static,
                new[]
                {
                    sourceItemType,
                    intermediateType.Type
                },
                new[]
                {
                    typeof(IQueryable<>).MakeGenericType(sourceItemType),
                    typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(sourceItemType, intermediateType.Type))
                });

            var selectCall = Expression.Call(newSelectMethod, node.Arguments[0], quotedLambda);

            return selectCall;
        }
    }
}