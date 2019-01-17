using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class MethodCallExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is MethodCallExpression methodCallExpression && ExpressionHelper.IsRuntimeObjectBoundExpression(methodCallExpression))
            {
                result = Expression.Constant(RuntimeExpressionValueResolver.GetValue(methodCallExpression));

                return true;
            }

            result = default;

            return false;
        }
    }
}