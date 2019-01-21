using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class LambdaExpressionSimplifier : IExpressionSimplifier
    {
        private readonly IExpressionSimplifierPipeline _expressionSimplifierPipeline;

        public LambdaExpressionSimplifier(IExpressionSimplifierPipeline expressionSimplifierPipeline)
        {
            _expressionSimplifierPipeline = expressionSimplifierPipeline ?? throw new ArgumentNullException(nameof(expressionSimplifierPipeline));
        }

        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is LambdaExpression lambdaExpression)
            {
                var simplifiedBodyExpression = _expressionSimplifierPipeline.ProcessExpression(lambdaExpression.Body);
                if (simplifiedBodyExpression != lambdaExpression.Body)
                {
                    result = Expression.Lambda(simplifiedBodyExpression, lambdaExpression.Name, lambdaExpression.TailCall, lambdaExpression.Parameters);

                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}