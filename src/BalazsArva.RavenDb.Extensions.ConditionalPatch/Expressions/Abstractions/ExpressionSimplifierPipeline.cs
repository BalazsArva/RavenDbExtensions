using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions
{
    public class ExpressionSimplifierPipeline : IExpressionSimplifierPipeline
    {
        private readonly IEnumerable<IExpressionSimplifier> expressionSimplifiers;

        public ExpressionSimplifierPipeline()
        {
            expressionSimplifiers = new List<IExpressionSimplifier>
            {
                new BinaryExpressionSimplifier(this),
                new LambdaExpressionSimplifier(this),
                new ConditionalExpressionSimplifier(this),
                new MemberExpressionSimplifier(this),
                new MethodCallExpressionSimplifier(this),
                new UnaryExpressionSimplifier(this)
            };
        }

        public Expression ProcessExpression(Expression expression)
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