using System;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions
{
    public static class ExpressionParser
    {
        public static string CreateJsScriptFromExpression(Expression expression, ScriptParameterDictionary parameters)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return ExpressionProcessorPipeline.ProcessExpression(expression, parameters);
        }
    }

    /*
    public static class ExpressionRuntimeValuesResolver
    {
        public static Expression ResolveRuntimeValues(Expression expression)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                return binaryExpression.Update(
                    ResolveRuntimeValues(binaryExpression.Left),
                    binaryExpression.Conversion,
                    ResolveRuntimeValues(binaryExpression.Right));
            }
            else if (expression is System.Linq.Expressions.BlockExpression)
            {
                throw new NotSupportedException();
            }
            else if (expression is System.Linq.Expressions.ConditionalExpression conditionalExpression)
            {
                return conditionalExpression.Update(
                    ResolveRuntimeValues(conditionalExpression.Test),
                    ResolveRuntimeValues(conditionalExpression.IfTrue),
                    ResolveRuntimeValues(conditionalExpression.IfFalse));
            }
            else if (expression is System.Linq.Expressions.ConstantExpression constantExpression)
            {
                return constantExpression;
            }
            else if (expression is System.Linq.Expressions.DebugInfoExpression)
            {
                throw new NotSupportedException();
            }
            else if (expression is System.Linq.Expressions.DefaultExpression defaultExpression)
            {
                throw new NotSupportedException();
            }
            else if (expression is System.Linq.Expressions.DynamicExpression dynamicExpression)
            {
                throw new NotSupportedException();
            }
            else if (expression is System.Linq.Expressions.GotoExpression)
            {
                throw new NotSupportedException();
            }
            else if (expression is System.Linq.Expressions.IndexExpression indexExpression)
            {
                return indexExpression.Update(
                    ResolveRuntimeValues(indexExpression.Object),
                    indexExpression.Arguments.Select(arg => ResolveRuntimeValues(arg)));
            }
            else if (expression is System.Linq.Expressions.InvocationExpression invocationExpression)
            {
                return invocationExpression.Update(
                    ResolveRuntimeValues(invocationExpression.Expression),
                    invocationExpression.Arguments.Select(arg => ResolveRuntimeValues(arg)));
            }
            else if (expression is System.Linq.Expressions.LabelExpression)
            {
                throw new NotSupportedException();
            }
            else if (expression is System.Linq.Expressions.LambdaExpression lambdaExpression)
            {
                return Expression.Lambda(
                    ResolveRuntimeValues(lambdaExpression.Body),
                    
            }
        }
    }
    */
}