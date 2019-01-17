using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class LambdaExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is LambdaExpression lambdaExpression)
            {
                var simplifiedBodyExpression = ExpressionSimplifier.SimplifyExpression(lambdaExpression.Body);

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