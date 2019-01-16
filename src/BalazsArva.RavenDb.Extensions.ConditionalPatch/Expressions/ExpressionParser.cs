using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions
{
    public static class ExpressionParser
    {
        public static string CreateJsScriptFromExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return ExpressionProcessorPipeline.GetScriptFromConditionExpression(expression);
        }
    }
}