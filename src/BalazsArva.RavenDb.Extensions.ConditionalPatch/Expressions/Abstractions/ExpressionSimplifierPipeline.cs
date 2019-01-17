using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public static class ExpressionSimplifierPipeline
    {
        private static readonly IEnumerable<IExpressionSimplifier> expressionSimplifiers;

        static ExpressionSimplifierPipeline()
        {
            expressionSimplifiers = new List<IExpressionSimplifier>
            {
                new BinaryExpressionSimplifier(),
                new LambdaExpressionSimplifier(),
                new ConditionalExpressionSimplifier(),
                new MemberExpressionSimplifier(),
                new MethodCallExpressionSimplifier(),
                new UnaryExpressionSimplifier(),
            };
        }

        public static Expression ProcessExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            foreach (var processor in expressionSimplifiers)
            {
                if (processor.TrySimplifyExpression(expression, out var result))
                {
                    return result;
                }
            }

            return expression;
        }
    }
}