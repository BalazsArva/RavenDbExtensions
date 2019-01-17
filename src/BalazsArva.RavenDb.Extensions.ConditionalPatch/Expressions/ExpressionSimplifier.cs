using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions
{
    public static class ExpressionSimplifier
    {
        public static Expression SimplifyExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return ExpressionSimplifierPipeline.ProcessExpression(expression);
        }
    }
}